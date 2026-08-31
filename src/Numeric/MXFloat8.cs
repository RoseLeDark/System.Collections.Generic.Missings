using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SystemEx.Numeric {
	/// <summary>
	/// Represents a 32‑element FP8 block with a shared exponent.
	/// This format is used for high‑performance quantization where
	/// dynamic range is preserved per‑block instead of per‑element.
	/// </summary>
	public class MXFloat8<T> where T : struct, IFP8<T> {
		
		private readonly byte m_sharedExponent;
		private T[] m_vector;

		public T this[int index] {
			get => m_vector[index];
			set => m_vector[index] = value;
		}

		public byte SharedExponent => m_sharedExponent;

		public MXFloat8 ( byte sharedExponent, T[]? vector ) {
			if ( !T.IsMXSupport ) throw new Exception("Formt not suppert");
			if ( vector == null ) throw new ArgumentNullException(nameof(vector));
			if ( vector.Length != 32 ) throw new ArgumentException("Die MX-Blockgröße must be 32", nameof(vector));

			m_sharedExponent = sharedExponent;
			m_vector = vector;
		}

		public static MXFloat8<T> Add ( MXFloat8<T> a, MXFloat8<T> b ) {
			T[] result = new T[32];
			byte finalScale = System.Math.Max(a.m_sharedExponent, b.m_sharedExponent);

			for ( int i = 0 ; i < 32 ; i++ ) {
				// 1. Sonderfälle der Einzelelemente abfangen (über dein Interface)
				if ( T.IsNaN(a[i]) || T.IsNaN(b[i]) ) { result[i] = T.NaN; continue; }
				if ( T.IsZero(a[i]) && T.IsZero(b[i]) ) { result[i] = T.Zero; continue; }

				// 2. Skalierungsdifferenz der MX-Blöcke berechnen
				int scaleDiffA = finalScale - a.m_sharedExponent;
				int scaleDiffB = finalScale - b.m_sharedExponent;

				// 3. Komponenten direkt als Fast_Byte über dein Interface abgreifen
				Fast_Byte expA = a[i].Exponent;
				Fast_Byte expB = b[i].Exponent;

				Fast_Byte mantA = (byte)(a[i].Mantissa | (expA != 0 ? 0x04 : 0x00));
				Fast_Byte mantB = (byte)(b[i].Mantissa | (expB != 0 ? 0x04 : 0x00));

				int realExpA = (expA == 0 ? 1 : expA.Value) - scaleDiffA;
				int realExpB = (expB == 0 ? 1 : expB.Value) - scaleDiffB;

				int finalElementExp = System.Math.Max(realExpA, realExpB);

				// 4. Mantissen ausrichten im Fast_UShort / Fast_UInt Äquivalent
				ushort shiftedA = (ushort)(mantA.Value << 4);
				ushort shiftedB = (ushort)(mantB.Value << 4);

				if ( realExpA >= realExpB ) {
					shiftedB >>= (realExpA - realExpB);
					finalElementExp = realExpA;
				} else {
					shiftedA >>= (realExpB - realExpA);
					finalElementExp = realExpB;
				}

				// 5. Arithmetik (Addition/Subtraktion anhand des echten Vorzeichens)
				ushort resMant = 0;
				byte finalSign = 0;

				// Nutzt dein Sign-Property aus dem Interface
				if ( a[i].Sign == b[i].Sign ) {
					resMant = (ushort)(shiftedA + shiftedB);
					finalSign = (byte)(a[i].Sign ? 1 : 0);
				} else {
					if ( shiftedA >= shiftedB ) {
						resMant = (ushort)(shiftedA - shiftedB);
						finalSign = (byte)(a[i].Sign ? 1 : 0);
					} else {
						resMant = (ushort)(shiftedB - shiftedA);
						finalSign = (byte)(b[i].Sign ? 1 : 0);
					}
				}

				if ( resMant == 0 ) { result[i] = T.Zero; continue; }

				// 6. Renormalisierung im Rechenregister
				while ( resMant >= 0x80 ) {
					resMant >>= 1;
					finalElementExp++;
				}
				while ( resMant < 0x40 && finalElementExp > 1 ) {
					resMant <<= 1;
					finalElementExp--;
				}

				resMant >>= 4;
				resMant &= 0x03; // Hidden Bit löschen

				// 7. Erzeugung über deine statische Interface-Methode: FromComponent!
				if ( finalElementExp <= 0 ) {
					result[i] = T.FromComponent(finalSign, (byte)resMant, 0);
				} else if ( finalElementExp >= 0x1F ) {
					result[i] = finalSign == 1 ? T.NegativeInfinity : T.PositiveInfinity;
				} else {
					result[i] = T.FromComponent(finalSign, (byte)resMant, (byte)finalElementExp);
				}
			}

			// 8. Globale Block-Sättigung bei verbliebenen Infinities
			bool blockOverflow;
			do {
				blockOverflow = false;
				for ( int i = 0 ; i < 32 ; i++ ) {
					if ( T.IsInfinity(result[i]) ) { blockOverflow = true; break; }
				}

				if ( blockOverflow ) {
					if ( finalScale == 0xFF ) break;

					finalScale++;
					for ( int i = 0 ; i < 32 ; i++ ) {
						if ( T.IsZero(result[i]) || T.IsNaN(result[i]) ) continue;

						Fast_Byte sign = (byte)(result[i].Sign ? 1 : 0);
						Fast_Byte exp = result[i].Exponent;
						Fast_Byte mant = (byte)(result[i].Mantissa | (exp != 0 ? 0x04 : 0x00));

						int nextExp = exp == 0 ? 0 : exp.Value - 1;

						if ( exp != 0 && nextExp == 0 ) {
							result[i] = T.FromComponent(sign, (byte)(mant.Value & 0x03), 0);
						} else {
							result[i] = T.FromComponent(sign, result[i].Mantissa, (byte)nextExp);
						}
					}
				}
			} while ( blockOverflow );

			return RenormalizeBlockOverflow(finalScale, result);
		}

		public static MXFloat8<T> Mul ( MXFloat8<T> a, MXFloat8<T> b ) {
			T[] result = new T[32];

			// Im MX-Standard addieren sich die Block-Exponenten bei der Multiplikation
			int finalScale = a.m_sharedExponent + b.m_sharedExponent;

			for ( int i = 0 ; i < 32 ; i++ ) {
				if ( T.IsNaN(a[i]) || T.IsNaN(b[i]) ) { result[i] = T.NaN; continue; }
				if ( T.IsZero(a[i]) || T.IsZero(b[i]) ) { result[i] = T.Zero; continue; }
				if ( T.IsInfinity(a[i]) || T.IsInfinity(b[i]) ) { result[i] = T.PositiveInfinity; continue; }

				Fast_Byte expA = a[i].Exponent;
				Fast_Byte expB = b[i].Exponent;

				// Hidden Bit hinzufügen (Bit 2)
				Fast_Byte mantA = (byte)(a[i].Mantissa | (expA != 0 ? 0x04 : 0x00));
				Fast_Byte mantB = (byte)(b[i].Mantissa | (expB != 0 ? 0x04 : 0x00));

				int realExpA = expA == 0 ? 1 : expA.Value;
				int realExpB = expB == 0 ? 1 : expB.Value;

				// Exponenten addieren und den E5M2-Bias (15) abziehen
				int finalElementExp = realExpA + realExpB - 15;

				// Multiplikation der Mantissen im Fast_UShort-Äquivalent
				ushort resMant = (ushort)(mantA.Value * mantB.Value);

				if ( resMant == 0 ) { result[i] = T.Zero; continue; }

				// Renormalisierung im Shiftraster
				while ( resMant >= 0x10 ) {
					resMant >>= 1;
					finalElementExp++;
				}
				while ( resMant < 0x04 && finalElementExp > 1 ) {
					resMant <<= 1;
					finalElementExp--;
				}

				resMant &= 0x03; // Hidden Bit entfernen

				if ( finalElementExp <= 0 ) {
					result[i] = T.FromComponent(new Fast_Byte(0), new Fast_Byte((byte)resMant), new Fast_Byte(0));
				} else if ( finalElementExp >= 0x1F ) {
					result[i] = T.PositiveInfinity;
				} else {
					result[i] = T.FromComponent(new Fast_Byte(0), new Fast_Byte((byte)resMant), new Fast_Byte((byte)finalElementExp));
				}
			}

			// Globale Block-Sättigung bei Infinities
			return RenormalizeBlockOverflow(finalScale, result);
		}

		public static MXFloat8<T> Div ( MXFloat8<T> a, MXFloat8<T> b ) {
			T[] result = new T[32];

			// Bei der Division subtrahieren sich die Block-Exponenten
			int finalScale = a.m_sharedExponent - b.m_sharedExponent + 15;

			for ( int i = 0 ; i < 32 ; i++ ) {
				if ( T.IsNaN(a[i]) || T.IsNaN(b[i]) ) { result[i] = T.NaN; continue; }
				if ( T.IsZero(b[i]) ) { result[i] = T.IsZero(a[i]) ? T.NaN : T.PositiveInfinity; continue; }
				if ( T.IsInfinity(b[i]) ) { result[i] = T.IsInfinity(a[i]) ? T.NaN : T.Zero; continue; }
				if ( T.IsZero(a[i]) || T.IsInfinity(a[i]) ) { result[i] = a[i]; continue; }

				Fast_Byte expA = a[i].Exponent;
				Fast_Byte expB = b[i].Exponent;

				Fast_Byte mantA = (byte)(a[i].Mantissa | (expA != 0 ? 0x04 : 0x00));
				Fast_Byte mantB = (byte)(b[i].Mantissa | (expB != 0 ? 0x04 : 0x00));

				int realExpA = expA == 0 ? 1 : expA.Value;
				int realExpB = expB == 0 ? 1 : expB.Value;

				int finalElementExp = realExpA - realExpB + 15;

				// Vor-Shiften im Rechenregister für die Ganzzahldivision
				ushort extendedMantA = (ushort)(mantA.Value << 4);
				ushort resMant = (ushort)(extendedMantA / mantB.Value);

				if ( resMant == 0 ) { result[i] = T.Zero; continue; }

				// Renormalisierung
				while ( resMant >= 0x08 ) {
					resMant >>= 1;
					finalElementExp++;
				}
				while ( resMant < 0x04 && finalElementExp > 1 ) {
					resMant <<= 1;
					finalElementExp--;
				}

				resMant &= 0x03; // Hidden Bit entfernen

				if ( finalElementExp <= 0 ) {
					result[i] = T.FromComponent(new Fast_Byte(0), new Fast_Byte((byte)resMant), new Fast_Byte(0));
				} else if ( finalElementExp >= 0x1F ) {
					result[i] = T.PositiveInfinity;
				} else {
					result[i] = T.FromComponent(new Fast_Byte(0), new Fast_Byte((byte)resMant), new Fast_Byte((byte)finalElementExp));
				}
			}

			return RenormalizeBlockOverflow(finalScale, result);
		}

		public static MXFloat8<T> Sub ( MXFloat8<T> a, MXFloat8<T> b ) {
			T[] result = new T[32];
			byte finalScale = System.Math.Max(a.m_sharedExponent, b.m_sharedExponent);

			for ( int i = 0 ; i < 32 ; i++ ) {
				if ( T.IsNaN(a[i]) || T.IsNaN(b[i]) ) { result[i] = T.NaN; continue; }
				if ( T.IsInfinity(b[i]) ) { result[i] = T.Zero; continue; } // Sättigung bei -Inf
				if ( T.IsInfinity(a[i]) ) { result[i] = T.IsInfinity(b[i]) ? T.NaN : T.PositiveInfinity; continue; }
				if ( T.IsZero(b[i]) ) { result[i] = a[i]; continue; }
				if ( T.IsZero(a[i]) ) { result[i] = T.Zero; continue; } // Sättigung: 0 - x = 0

				int scaleDiffA = finalScale - a.m_sharedExponent;
				int scaleDiffB = finalScale - b.m_sharedExponent;

				Fast_Byte expA = a[i].Exponent;
				Fast_Byte expB = b[i].Exponent;

				Fast_Byte mantA = (byte)(a[i].Mantissa | (expA != 0 ? 0x04 : 0x00));
				Fast_Byte mantB = (byte)(b[i].Mantissa | (expB != 0 ? 0x04 : 0x00));

				int realExpA = (expA == 0 ? 1 : expA.Value) - scaleDiffA;
				int realExpB = (expB == 0 ? 1 : expB.Value) - scaleDiffB;

				int finalElementExp = realExpA;

				ushort shiftedA = (ushort)(mantA.Value << 4);
				ushort shiftedB = (ushort)(mantB.Value << 4);

				if ( realExpA >= realExpB ) {
					shiftedB >>= (realExpA - realExpB);
					finalElementExp = realExpA;
				} else {
					shiftedA >>= (realExpB - realExpA);
					finalElementExp = realExpB;
				}

				// --- UNSIGNED SÄTTIGUNGSPRÜFUNG ---
				// Wenn der subtrahierte Wert im gemeinsamen Raster größer oder gleich ist -> Sättigung auf Null!
				if ( shiftedB >= shiftedA ) {
					result[i] = T.Zero;
					continue;
				}

				ushort resMant = (ushort)(shiftedA - shiftedB);

				if ( resMant == 0 ) { result[i] = T.Zero; continue; }

				while ( resMant >= 0x80 ) {
					resMant >>= 1;
					finalElementExp++;
				}
				while ( resMant < 0x40 && finalElementExp > 1 ) {
					resMant <<= 1;
					finalElementExp--;
				}

				resMant >>= 4;
				resMant &= 0x03; // Hidden Bit löschen

				if ( finalElementExp <= 0 ) {
					result[i] = T.FromComponent(new Fast_Byte(0), new Fast_Byte((byte)resMant), new Fast_Byte(0));
				} else if ( finalElementExp >= 0x1F ) {
					result[i] = T.PositiveInfinity;
				} else {
					result[i] = T.FromComponent(new Fast_Byte(0), new Fast_Byte((byte)resMant), new Fast_Byte((byte)finalElementExp));
				}
			}

			return RenormalizeBlockOverflow(finalScale, result);
		}

		private static MXFloat8<T> RenormalizeBlockOverflow ( int finalScale, T[] result ) {
			bool blockOverflow;
			do {
				blockOverflow = false;
				for ( int i = 0 ; i < 32 ; i++ ) {
					if ( T.IsInfinity(result[i]) ) { blockOverflow = true; break; }
				}

				if ( blockOverflow ) {
					if ( finalScale >= 0xFF ) { finalScale = 0xFF; break; } // Harte Sättigungsgrenze

					finalScale++;
					for ( int i = 0 ; i < 32 ; i++ ) {
						if ( T.IsZero(result[i]) || T.IsNaN(result[i]) ) continue;

						Fast_Byte exp = result[i].Exponent;
						Fast_Byte mant = (byte)(result[i].Mantissa | (exp != 0 ? 0x04 : 0x00));

						int nextExp = exp == 0 ? 0 : exp.Value - 1;

						if ( exp != 0 && nextExp == 0 ) {
							result[i] = T.FromComponent(new Fast_Byte(0), new Fast_Byte((byte)(mant.Value & 0x03)), new Fast_Byte(0));
						} else {
							result[i] = T.FromComponent(new Fast_Byte(0), result[i].Mantissa, new Fast_Byte((byte)nextExp));
						}
					}
				}
			} while ( blockOverflow );

			byte safeSharedExponent = (byte)(finalScale > 0xFF ? 0xFF : (finalScale < 0 ? 0 : finalScale));
			return new MXFloat8<T>(safeSharedExponent, result);
		}

		public bool Equals ( MXFloat8<T> other ) {
			bool _ret = true;

			if ( m_sharedExponent == other.SharedExponent ) {
				for ( int i = 0 ; i < 32 ; i++ ) {
					if ( this[i] != other[i] ) {
						_ret = false;
						break;
					}
				}
			} else {
				_ret = false;
			}

			return _ret;
		}
		public override bool Equals ( object? obj ) {
			if ( obj is MXFloat8<T> o ) return Equals(o);
			return false;
		}

		public static MXFloat8<T> operator + ( MXFloat8<T> a, MXFloat8<T> b )
			=> Add( a, b );
		public static MXFloat8<T> operator - ( MXFloat8<T> a, MXFloat8<T> b )
			=> Sub(a, b);
		public static MXFloat8<T> operator * ( MXFloat8<T> a, MXFloat8<T> b )
			=> Mul(a, b);
		public static MXFloat8<T> operator / ( MXFloat8<T> a, MXFloat8<T> b )
			=> Div(a, b);

		public static bool operator == ( MXFloat8<T> a, MXFloat8<T> b )
			=> a.Equals( b );

		public static bool operator != ( MXFloat8<T> a, MXFloat8<T> b )
			=> !(a== b);

		public override int GetHashCode () {
			return m_sharedExponent.GetHashCode() ^ m_vector.GetHashCode();
		}
	}

	public class FloatMxUE5M2 : MXFloat8<FloatUE5M2> {
		public FloatMxUE5M2 ( byte sharedExponent )
			: base(sharedExponent, new FloatUE5M2[32]) { }
		public FloatMxUE5M2  ( byte sharedExponent, FloatUE5M2[] vector ) 
			: base(sharedExponent, vector) { }
	}
}
