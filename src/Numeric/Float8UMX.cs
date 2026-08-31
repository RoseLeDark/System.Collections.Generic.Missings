using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SystemEx.Numeric {
	/// <summary>
	/// Represents a generic FP8 MX‑block consisting of 32 FP8 elements of type <typeparamref name="T"/> 
	/// sharing a common block exponent. 
	/// 
	/// <para>
	/// MX‑blocks are used to accelerate FP8 vector operations by applying a shared exponent scaling 
	/// across all 32 elements. This allows efficient SIMD‑style arithmetic while preserving FP8 semantics.
	/// </para>
	/// 
	/// <para>
	/// The type parameter <typeparamref name="T"/> must implement <see cref="IFP8{T}"/> and 
	/// <see cref="IP8MXEnable"/>, providing FP8 format metadata such as HiddenBit, MantissaMask, 
	/// ExponentBias, MaxExponent, and ShiftRaster.
	/// </para>
	/// </summary>
	public class Float8UMX<T> where T : struct, IFP8<T>, IP8UMXEnable<Fast_Byte> {
		
		private readonly byte m_sharedExponent;
		private T[] m_vector;

		/// <summary>
		/// Gets or sets the FP8 element at the specified index within the MX‑block.
		/// </summary>
		public T this[int index] {
			get => m_vector[index];
			set => m_vector[index] = value;
		}

		/// <summary>
		/// Gets the shared exponent applied to all FP8 elements in this MX‑block.
		/// </summary>
		public byte SharedExponent => m_sharedExponent;

		/// <summary>
		/// Initializes a new MX‑block with a shared exponent and a 32‑element FP8 vector.
		/// </summary>
		/// <param name="sharedExponent">The block‑wide exponent scaling factor.</param>
		/// <param name="vector">The FP8 element array (must contain exactly 32 elements).</param>
		public Float8UMX ( byte sharedExponent, T[]? vector ) {
			if ( !T.IsMXSupport ) throw new Exception("Formt not suppert");
			if ( vector == null ) throw new ArgumentNullException(nameof(vector));
			if ( vector.Length != 32 ) throw new ArgumentException("Die MX-Blockgröße must be 32", nameof(vector));

			m_sharedExponent = sharedExponent;
			m_vector = vector;
		}

		/// <summary>
		/// Adds two MX‑blocks element‑wise, applying exponent alignment and FP8 renormalization.
		/// </summary>
		public static Float8UMX<T> Add ( Float8UMX<T> a, Float8UMX<T> b ) {
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

				Fast_Byte mantA = (byte)(a[i].Mantissa | (expA != 0 ? a[i].HiddenBit : 0x00));
				Fast_Byte mantB = (byte)(b[i].Mantissa | (expB != 0 ? b[i].HiddenBit : 0x00));

				int realExpA = (expA == 0 ? 1 : expA.Value) - scaleDiffA;
				int realExpB = (expB == 0 ? 1 : expB.Value) - scaleDiffB;

				int finalElementExp = System.Math.Max(realExpA, realExpB);

				// 4. Mantissen ausrichten im Fast_UShort / Fast_UInt Äquivalent
				ushort shiftedA = (ushort)(mantA.Value << a[i].ShiftRaster.Value);
				ushort shiftedB = (ushort)(mantB.Value << b[i].ShiftRaster.Value);

				if ( realExpA >= realExpB ) {
					shiftedB >>= (realExpA - realExpB);
				} else {
					shiftedA >>= (realExpB - realExpA);
				}

				Fast_UShort resMant = (ushort)(shiftedA + shiftedB);

				if ( resMant == 0 ) { result[i] = T.Zero; continue; }

				// 6. Renormalisierung im Rechenregister
				while ( resMant >= ((Fast_UShort)a[i].HiddenBit << a[i].ShiftRaster ) ) {
					resMant >>= 1;
					finalElementExp++;
				}
				while ( resMant < (a[i].HiddenBit << ( (byte)a[i].ShiftRaster - 1)) && finalElementExp > 1 ) {
					resMant <<= 1;
					finalElementExp--;
				}

				resMant >>= (byte)a[i].ShiftRaster;
				resMant &= (byte)a[i].MantissaMask; // Hidden Bit löschen

				// 7. Erzeugung über deine statische Interface-Methode: FromComponent!
				if ( finalElementExp <= 0 ) {
					result[i] = T.FromComponent(0, (byte)resMant, 0);
				} else if ( finalElementExp >=  a[i].MaxExponent.Value ) {
					result[i] = T.PositiveInfinity;
				} else {
					result[i] = T.FromComponent(0, (byte)resMant, (byte)finalElementExp);
				}
			}

			return RenormalizeBlockOverflow(finalScale, result);
		}
		/// <summary>
		/// Multiplies two MX‑blocks element‑wise using FP8 multiplication rules.
		/// </summary>
		public static Float8UMX<T> Mul ( Float8UMX<T> a, Float8UMX<T> b ) {
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
				Fast_Byte mantA = (byte)(a[i].Mantissa | (expA != 0 ? a[i].HiddenBit : 0x00));
				Fast_Byte mantB = (byte)(b[i].Mantissa | (expB != 0 ? a[i].HiddenBit : 0x00));

				int realExpA = expA == 0 ? 1 : expA.Value;
				int realExpB = expB == 0 ? 1 : expB.Value;

				// Exponenten addieren und den E5M2-Bias ((byte)a[i].ExponentBias) abziehen
				int finalElementExp = realExpA + realExpB - (byte)a[i].ExponentBias;

				// Multiplikation der Mantissen im Fast_UShort-Äquivalent
				ushort resMant = (ushort)(mantA.Value * mantB.Value);

				if ( resMant == 0 ) { result[i] = T.Zero; continue; }

				// Renormalisierung im Shiftraster
				while ( resMant >= (ushort)(a[i].HiddenBit << 2) ) {
					resMant >>= 1;
					finalElementExp++;
				}
				while ( resMant < (byte)a[i].HiddenBit && finalElementExp > 1 ) {
					resMant <<= 1;
					finalElementExp--;
				}

				resMant &= a[i].MantissaMask.Value; // Hidden Bit entfernen

				if ( finalElementExp <= 0 ) {
					result[i] = T.FromComponent(0, (byte)resMant, 0);
				} else if ( finalElementExp >= a[i].MaxExponent.Value ) {
					result[i] = T.PositiveInfinity;
				} else {
					result[i] = T.FromComponent(0, (byte)resMant, (byte)finalElementExp);
				}
			}

			// Globale Block-Sättigung bei Infinities
			return RenormalizeBlockOverflow(finalScale, result);
		}
		/// <summary>
		/// Divides two MX‑blocks element‑wise using FP8 division rules.
		/// </summary>
		public static Float8UMX<T> Div ( Float8UMX<T> a, Float8UMX<T> b ) {
			T[] result = new T[32];

			// Bei der Division subtrahieren sich die Block-Exponenten
			int finalScale = a.m_sharedExponent - b.m_sharedExponent + (byte)a[0].ExponentBias;

			for ( int i = 0 ; i < 32 ; i++ ) {
				if ( T.IsNaN(a[i]) || T.IsNaN(b[i]) ) { result[i] = T.NaN; continue; }
				if ( T.IsZero(b[i]) ) { result[i] = T.IsZero(a[i]) ? T.NaN : T.PositiveInfinity; continue; }
				if ( T.IsInfinity(b[i]) ) { result[i] = T.IsInfinity(a[i]) ? T.NaN : T.Zero; continue; }
				if ( T.IsZero(a[i]) || T.IsInfinity(a[i]) ) { result[i] = a[i]; continue; }

				Fast_Byte expA = a[i].Exponent;
				Fast_Byte expB = b[i].Exponent;

				Fast_Byte mantA = (byte)(a[i].Mantissa | (expA != 0 ? a[i].HiddenBit : 0x00));
				Fast_Byte mantB = (byte)(b[i].Mantissa | (expB != 0 ? a[i].HiddenBit : 0x00));

				int realExpA = expA == 0 ? 1 : expA.Value;
				int realExpB = expB == 0 ? 1 : expB.Value;

				int finalElementExp = realExpA - realExpB + (byte)a[i].ExponentBias;

				// Vor-Shiften im Rechenregister für die Ganzzahldivision
				ushort extendedMantA = (ushort)(mantA.Value << 4);
				ushort resMant = (ushort)(extendedMantA / mantB.Value);

				if ( resMant == 0 ) { result[i] = T.Zero; continue; }

				// Renormalisierung
				while ( resMant >= ((byte)a[i].HiddenBit << 1) ) {
					resMant >>= 1;
					finalElementExp++;
				}
				while ( resMant < (byte)a[i].HiddenBit && finalElementExp > 1 ) {
					resMant <<= 1;
					finalElementExp--;
				}

				resMant &= (byte)a[i].MantissaMask; // Hidden Bit entfernen

				if ( finalElementExp <= 0 ) {
					result[i] = T.FromComponent(0, (byte)resMant, 0);
				} else if ( finalElementExp >=  a[i].MaxExponent.Value ) {
					result[i] = T.PositiveInfinity;
				} else {
					result[i] = T.FromComponent(0, (byte)resMant, (byte)finalElementExp);
				}
			}

			return RenormalizeBlockOverflow(finalScale, result);
		}
		/// <summary>
		/// Subtracts two MX‑blocks element‑wise using unsigned FP8 subtraction rules.
		/// </summary>
		public static Float8UMX<T> Sub ( Float8UMX<T> a, Float8UMX<T> b ) {
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

				Fast_Byte mantA = (byte)(a[i].Mantissa | (expA != 0 ? a[i].HiddenBit : 0x00));
				Fast_Byte mantB = (byte)(b[i].Mantissa | (expB != 0 ? a[i].HiddenBit : 0x00));

				int realExpA = (expA == 0 ? 1 : expA.Value) - scaleDiffA;
				int realExpB = (expB == 0 ? 1 : expB.Value) - scaleDiffB;

				int finalElementExp = realExpA;

				ushort shiftedA = (ushort)(mantA.Value << a[i].ShiftRaster.Value);
				ushort shiftedB = (ushort)(mantB.Value << a[i].ShiftRaster.Value);

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

				while ( resMant >= ((byte)a[i].HiddenBit << a[i].ShiftRaster.Value) ) {
					resMant >>= 1;
					finalElementExp++;
				}

				while ( resMant < ((byte)a[i].HiddenBit << (a[i].ShiftRaster.Value - 1)) && finalElementExp > 1 ) {
					resMant <<= 1;
					finalElementExp--;
				}

				resMant >>= 4;
				resMant &= (byte)a[i].MantissaMask; // Hidden Bit löschen

				if ( finalElementExp <= 0 ) {
					result[i] = T.FromComponent(0, (byte)resMant, 0);
				} else if ( finalElementExp >=  a[i].MaxExponent.Value ) {
					result[i] = T.PositiveInfinity;
				} else {
					result[i] = T.FromComponent(0, (byte)resMant, (byte)finalElementExp);
				}
			}

			return RenormalizeBlockOverflow(finalScale, result);
		}

		/// <summary>
		/// Renormalizes the MX‑block if any element overflows into infinity by increasing 
		/// the shared exponent and downscaling all FP8 elements accordingly.
		/// </summary>
		private static Float8UMX<T> RenormalizeBlockOverflow ( int finalScale, T[] result ) {
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
						Fast_Byte mant = (byte)(result[i].Mantissa | (exp != 0 ? result[i].HiddenBit : 0x00));

						int nextExp = exp == 0 ? 0 : exp.Value - 1;

						if ( exp != 0 && nextExp == 0 ) {
							result[i] = T.FromComponent(0, (byte)(mant.Value & result[i].MantissaMask), 0);
						} else {
							result[i] = T.FromComponent(0, result[i].Mantissa, (byte)nextExp);
						}
					}
				}
			} while ( blockOverflow );

			byte safeSharedExponent = (byte)(finalScale > 0xFF ? 0xFF : (finalScale < 0 ? 0 : finalScale));
			return new Float8UMX<T>(safeSharedExponent, result);
		}

		/// <summary>
		/// Determines whether this MX‑block is equal to another MX‑block by comparing 
		/// shared exponent and all 32 FP8 elements.
		/// </summary>
		public bool Equals ( Float8UMX<T> other ) {
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

		/// <inheritdoc/>
		public override bool Equals ( object? obj ) {
			if ( obj is Float8UMX<T> o ) return Equals(o);
			return false;
		}

		/// <summary>
		/// See <see cref="Float8UMX{T}.Add(Float8UMX{T}, Float8UMX{T})"/>
		/// </summary>
		public static Float8UMX<T> operator + ( Float8UMX<T> a, Float8UMX<T> b )
			=> Add( a, b );

		/// <summary>
		/// See <see cref="Float8UMX{T}.Sub(Float8UMX{T}, Float8UMX{T})"/>
		/// </summary>
		public static Float8UMX<T> operator - ( Float8UMX<T> a, Float8UMX<T> b )
			=> Sub(a, b);

		/// <summary>
		/// See <see cref="Float8UMX{T}.Mul(Float8UMX{T}, Float8UMX{T})"/>
		/// </summary>
		public static Float8UMX<T> operator * ( Float8UMX<T> a, Float8UMX<T> b )
			=> Mul(a, b);

		/// <summary>
		/// See <see cref="Float8UMX{T}.Div(Float8UMX{T}, Float8UMX{T})"/>
		/// </summary>
		public static Float8UMX<T> operator / ( Float8UMX<T> a, Float8UMX<T> b )
			=> Div(a, b);

		/// <summary>Equality operator.</summary>
		public static bool operator == ( Float8UMX<T> a, Float8UMX<T> b )
			=> a.Equals( b );

		/// <summary>Inequality operator.</summary>
		public static bool operator != ( Float8UMX<T> a, Float8UMX<T> b )
			=> !(a== b);

		/// <inheritdoc/>
		public override int GetHashCode () {
			return m_sharedExponent.GetHashCode() ^ m_vector.GetHashCode();
		}
	}
}
