/* 
 * SPDX-License-Identifier: EUPL-1.2
 *
 * Copyright (c) 2026 Amber-Sophia Schröck <ambersophia.schroeck@mail.de>
 *
 * This file is licensed under the European Union Public Licence (EUPL) version 1.2.
 * You can obtain a copy of the licence at:
 *   https://joinup.ec.europa.eu/collection/eupl/eupl-text-eupl-12
 *
 * Unless required by applicable law or agreed to in writing, software distributed
 * under the Licence is distributed on an "AS IS" basis, WITHOUT WARRANTIES OR
 * CONDITIONS OF ANY KIND, either express or implied.
 *
 * If you modify this file, retain this notice and add a short description of your
 * changes and the date.
 */


using System.Runtime.InteropServices;
using SystemEx.Collections.Generic;
using SystemEx.Hash;
using SystemEx.Utils;

namespace SystemEx.Numeric {

	/// <summary>
	/// Represents an FP8 value encoded in the industry‑standard E4M3 format.
	/// 
	/// This format uses:
	/// <para>• 1 sign bit</para>
	/// <para>• 5 exponent bits (bias = 7)</para>
	/// <para>• 2 mantissa bits</para>
	/// 
	/// The hidden bit is always 1 for normalized numbers. Subnormal numbers
	/// use an exponent of zero and do not include the hidden bit.
	/// 
	/// This implementation is fully self‑contained and performs all arithmetic,
	/// comparisons, and classifications purely on FP8 bit‑patterns without
	/// converting to host floating‑point types.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	[HashAlgorithm(typeof(BernsteinHash), Endian.System)]
	internal struct FloatE5M2 : IFP8<FloatE5M2> {
		private Fast_Byte m_baseBytes;

		/// <summary>Raw 8‑bit storage (FP8 E5M2 encoded).</summary>
		public Fast_Byte ToBase => m_baseBytes;

		/// <summary>Sign bits (always 1).</summary>
		public Fast_Byte SignBits => 1;
		/// <summary>Exponent bits (5).</summary>
		public Fast_Byte ExponentBits => 5;
		/// <summary>Mantissa bits (2).</summary>
		public Fast_Byte MantissaBits => 2;
		/// <summary>Exponent bias (2^(k-1)-1 = 15).</summary>
		public Fast_Byte ExponentBias => 15;
		/// <summary>Total bits (8).</summary>
		public Fast_Byte TotalBits => 8;
		/// <summary>Hidden bit mask (1 << MantissaBits).</summary>
		public Fast_Byte HiddenBit => (byte)(1 << 2); // 0x04

		/// <summary>
		/// Gets the sign bit. True indicates a negative value.
		/// </summary>
		public bool Sign => m_baseBytes.Is(7) == 1;

		/// <summary>
		/// Gets the sign bit, as Fast_Byte
		/// </summary>
		public Fast_Byte NSign => m_baseBytes.Is(7);

		/// <summary>Exponent field (bits 2..6).</summary>
		public Fast_Byte Exponent => ((m_baseBytes >> 2) & 0x1F);

		/// <summary>Mantissa field (bits 0..1).</summary>
		public Fast_Byte Mantissa => (m_baseBytes & 0x03);

		/// <summary>
		/// Represents the FP8 value +1.0.
		/// </summary>
		public static FloatE5M2 One => new FloatE5M2(0x3C);
		/// <summary>
		/// Represents the FP8 value -1.0.
		/// </summary>
		public static FloatE5M2 NegativeOne => new FloatE5M2(0xBC);
		/// <summary>
		/// Represents the FP8 value +0.0.
		/// </summary>
		public static FloatE5M2 Zero => new FloatE5M2(0x00);
		/// <summary>
		/// Represents the FP8 value -0.0.
		/// </summary>
		public static FloatE5M2 NegativeZero => new FloatE5M2(0x80);
		

		public static FloatE5M2 PositiveInfinity => new FloatE5M2(0x7C);

		public static FloatE5M2 NegativeInfinity => new FloatE5M2(0xFC);

		public static FloatE5M2 NaN => new FloatE5M2(0x7E);

		public static FloatE5M2 NaN2 => new FloatE5M2(0x7F);

		/// <summary>
		/// Smallest representable positive subnormal value (2^-16 ≈ 0.00001525).
		/// </summary>
		public static FloatE5M2 Epsilon => new FloatE5M2(0x01);

		public static FloatE5M2 E => new FloatE5M2(0x41);

		public static FloatE5M2 Tau => new FloatE5M2(0x4);

		public static FloatE5M2 Pi => new FloatE5M2(0x46);

		/// <summary>
		/// Largest representable finite positive value (57344).
		/// </summary>
		public static FloatE5M2 MaxValue => new FloatE5M2(0x7B);

		/// <summary>
		/// Smallest representable finite negative value (-57344).
		/// </summary>
		public static FloatE5M2 MinValue => new FloatE5M2(0xFB);

		public static bool IsMXSupport => false;

		/// <summary>
		/// Create from raw byte.
		/// </summary>
		public FloatE5M2 ( byte raw ) { m_baseBytes = raw; }

		/// <summary>
		/// Create from fields.
		/// </summary>
		public FloatE5M2 ( byte sign, byte exponent, byte mantissa ) {
			m_baseBytes = (byte)(((sign & 1) << 7) | ((exponent & 0x1F) << 2) | (mantissa & 0x03));
		}

		/// <summary>
		/// True if the value is Zero
		/// </summary>
		public static bool IsZero ( FloatE5M2 value )
			=> value.Exponent == 0x00 && value.Mantissa == 0x00;

		public static bool IsNegative ( FloatE5M2 value ) => value.Sign;
		/// <summary>
		/// True if the value is Not-a-Number (NaN).
		/// </summary>
		public static bool IsNaN ( FloatE5M2 value )
			=> value.Exponent == 0x1F && value.Mantissa != 0x00;

		/// <summary>
		/// True if the value is positive or negative infinity.
		/// </summary>
		public static bool IsInfinity ( FloatE5M2 value )
			=> value.Exponent == 0x1F && value.Mantissa == 0x00;

		public static bool IsFinite ( FloatE5M2 value ) => !IsNaN(value);
		/// <summary>
		/// True if the value is subnormal (denormalized).
		/// </summary>
		public static bool IsSubnormal ( FloatE5M2 value )
			=> value.Exponent == 0x00 && value.Mantissa != 0x00;

		/// <summary>
		/// True if the value is Normal
		/// </summary>
		public static bool IsNormal ( FloatE5M2 value ) 
			=> value.Exponent != 0 && value.Exponent != 0x1F;

		public static bool IsInteger ( FloatE5M2 value ) {
			if ( IsNaN(value) ) return false;

			// Unterhalb vom Bias (1.0) ist nur die Null eine Ganzzahl
			if ( value.Exponent < 15 ) return IsZero(value);

			// Ab Exponent 17 (Bias + 2) schiebt sich das Komma komplett hinter die Mantisse.
			// Ab hier ist JEDER darstellbare Wert (auch Infinity) eine Ganzzahl.
			if ( value.Exponent >= 17 ) return !IsInfinity(value);

			int shift = value.Exponent.Value - 15;
			return (value.Mantissa & (1 >> shift) ) == 0;
		}




		/// <summary>
		/// Returns the absolute value of the FP8 number.
		/// </summary>
		public static FloatE5M2 Abs ( FloatE5M2 value ) {
			Fast_Byte temp = value.m_baseBytes;
			temp.Mask(0x7F); // Setzt das Vorzeichen-Bit (Bit 7) blitzschnell auf 0
			return new FloatE5M2(temp.Value);
		}

		/// <summary>
		/// Returns the negation of the FP8 number.
		/// </summary>
		public static FloatE5M2 Negate ( FloatE5M2 x ) {
			Fast_Byte temp = x.m_baseBytes;
			temp.Flip(7); // Invertiert das Vorzeichen-Bit (Bit 7) direkt via XOR
			return new FloatE5M2(temp.Value);
		}

		/// <summary>
		/// Returns −1, 0, or +1 depending on the sign of the value.
		/// </summary>
		public static FloatE5M2 Signum ( FloatE5M2 x ) {
			if ( IsNaN(x) ) return FloatE5M2.NaN;
			if ( IsZero(x) ) return FloatE5M2.Zero;
			return x.Sign ? FloatE5M2.NegativeOne : FloatE5M2.One;
		}

		/// <summary>
		/// Returns the largest integer less than or equal to the value.
		/// </summary>
		public static FloatE5M2 Floor ( FloatE5M2 x ) {
			if ( IsNaN(x) ) return x;
			if ( x.Exponent >= x.ExponentBias ) return x;
			if ( IsZero(x) ) return x;
			return x.Sign ? FloatE5M2.NegativeOne : FloatE5M2.Zero;
		}

		/// <summary>
		/// Returns the smallest integer greater than or equal to the value.
		/// </summary>
		public static FloatE5M2 Ceil ( FloatE5M2 x ) {
			if ( IsNaN(x) ) return x;
			if ( x.Exponent >= x.ExponentBias ) return x;
			if ( IsZero(x) ) return x;
			return x.Sign ? FloatE5M2.Zero : FloatE5M2.One;
		}

		/// <summary>
		/// Truncates the fractional part of the FP8 value.
		/// </summary>
		public static FloatE5M2 Trunc ( FloatE5M2 x ) {
			if ( IsNaN(x) ) return x;
			if ( x.Exponent >= x.ExponentBias ) return x;
			return FloatE5M2.Zero;
		}

		/// <summary>
		/// Clamps the value to the inclusive range [min, max].
		/// </summary>
		public static FloatE5M2 Clamp ( FloatE5M2 x, FloatE5M2 min, FloatE5M2 max ) {
			if ( x < min ) return min;
			if ( x > max ) return max;
			return x;
		}

		public static FloatE5M2 Add ( FloatE5M2 a, FloatE5M2 b ) {
			// 1. Sonderfälle (NaN und Infinity) direkt auf Bit-Ebene abfangen
			if ( IsNaN(a) || IsNaN(b) ) return NaN;
			if ( IsInfinity(a) ) return IsInfinity(b) && (a.Sign != b.Sign) ? NaN : a;
			if ( IsInfinity(b) ) return b;
			if ( IsZero(a) ) return b;
			if ( IsZero(b) ) return a;

			Fast_Byte expA = a.Exponent;
			Fast_Byte expB = b.Exponent;

			// Implizites Bit (Hidden Bit) hinzufügen: 1.0 bei normalen Zahlen, 0.0 bei subnormalen
			Fast_Byte mantA = (byte)(a.Mantissa | (expA != 0 ? 0x04 : 0x00));
			Fast_Byte mantB = (byte)(b.Mantissa | (expB != 0 ? 0x04 : 0x00));

			// Exponenten anpassen (Subnormale Zahlen haben implizit Exponent 1)
			Fast_Int realExpA = expA == 0 ? 1 : expA;
			Fast_Int realExpB = expB == 0 ? 1 : expB;

			Fast_Int finalExp = realExpA;
			byte finalSign = 0;

			// --- Nutzung deines größeren Registers für die Berechnung ---
			// Wir shiften die extrahierten Basismantissen nach links in das Fixpunktraster
			Fast_UShort shiftedA = (ushort)(mantA << 4);
			Fast_UShort shiftedB = (ushort)(mantB << 4);
			Fast_UShort resMant = 0;

			// 1. Schritt: Mantissen auf denselben Exponenten ausrichten
			if ( realExpA >= realExpB ) {
				var shift = realExpA - realExpB;
				shiftedB >>= (int)shift;
				finalExp = realExpA;
			} else {
				var shift = realExpB - realExpA;
				shiftedA >>= (int)shift;
				finalExp = realExpB;
			}

			// 2. Schritt: Mathematische Addition / Subtraktion basierend auf den echten Vorzeichen
			if ( a.Sign == b.Sign ) {
				// Gleiche Vorzeichen -> Einfach addieren
				resMant = (ushort)(shiftedA + shiftedB);
				finalSign = (byte)(a.Sign ? 1 : 0);
			} else {
				// Unterschiedliche Vorzeichen -> Subtrahieren (Kleinere von Größerer abziehen)
				if ( shiftedA >= shiftedB ) {
					resMant = (ushort)(shiftedA - shiftedB);
					finalSign = (byte)(a.Sign ? 1 : 0);
				} else {
					resMant = (ushort)(shiftedB - shiftedA);
					finalSign = (byte)(b.Sign ? 1 : 0);
				}
			}

			if ( resMant == 0 ) return finalSign == 1 ? NegativeZero : Zero;

			// 3. Schritt: Renormalisierung des Ergebnisses im größeren Register
			// Wir suchen das führende Bit im 4er-Shiftraster (0x08 << 4 entspricht dezimal 128)
			while ( resMant >= 0x80 ) {
				resMant >>= 1;
				finalExp++;
			}
			while ( resMant < 0x40 && finalExp > 1 ) // (0x04 << 4 entspricht dezimal 64)
			{
				resMant <<= 1;
				finalExp--;
			}

			// Zurück in das 2-Bit-E5M2-Raster schieben (Fixpunktraster entfernen)
			resMant >>= 4;
			resMant &= 0x03; // Hidden Bit auf Bit-Position 2 entfernen

			// Überlauf zu Infinity prüfen (Der maximale normale Exponent ist 30)
			if ( finalExp >= 0x1F )
				return finalSign == 1 ? NegativeInfinity : PositiveInfinity;

			// Unterlauf zu Subnormal prüfen
			if ( finalExp <= 0 )
				return new FloatE5M2(finalSign, 0, (byte)resMant);

			return new FloatE5M2(finalSign, (byte)finalExp, (byte)resMant);
		}

		public static FloatE5M2 Mul ( FloatE5M2 a, FloatE5M2 b ) {
			// 1. Sonderfälle auf Bit-Ebene prüfen
			if ( IsNaN(a) || IsNaN(b) ) return NaN;

			// Kreuzprüfung für Unendlich und Null
			if ( IsInfinity(a) || IsInfinity(b) )
				return (IsZero(a) || IsZero(b)) ? NaN : ((a.Sign ^ b.Sign) ? NegativeInfinity : PositiveInfinity);

			if ( IsZero(a) || IsZero(b) )
				return (a.Sign ^ b.Sign) ? NegativeZero : Zero;

			// Vorzeichen über XOR bestimmen
			byte finalSign = (byte)(a.Sign ^ b.Sign ? 1 : 0);

			Fast_Byte expA = a.Exponent;
			Fast_Byte expB = b.Exponent;

			// Mantissen extrahieren inklusive Hidden Bit (Bit Position 2)
			Fast_Byte mantA = (byte)(a.Mantissa | (expA != 0 ? 0x04 : 0x00));
			Fast_Byte mantB = (byte)(b.Mantissa | (expB != 0 ? 0x04 : 0x00));

			// Exponenten für subnormale Zahlen korrigieren
			Fast_Int realExpA = expA == 0 ? 1 : expA;
			Fast_Int realExpB = expB == 0 ? 1 : expB;

			// Exponenten addieren und den E5M2-Bias (15) abziehen
			Fast_Int finalExp = realExpA + realExpB - 15;

			// --- Nutzung deines größeren Registers für die Berechnung ---
			// Multiplikation der Mantissen (Maximalwert: 7 * 7 = 49, passt locker in Fast_UShort / Fast_UInt)
			Fast_UShort resMant = (ushort)(mantA * mantB);

			if ( resMant == 0 ) return finalSign == 1 ? NegativeZero : Zero;

			// Renormalisierung: Da wir zwei 3-Bit-Zahlen multipliziert haben,
			// wandert das implizite Hidden Bit standardmäßig auf Position 4 (Wert 16 / 0x10).
			while ( resMant >= 0x10 ) {
				resMant >>= 1;
				finalExp++;
			}
			while ( resMant < 0x04 && finalExp > 1 ) {
				resMant <<= 1;
				finalExp--;
			}

			// Hidden Bit auf Position 2 entfernen (Wert 4 ausmaskieren)
			resMant &= 0x03;

			// Überlauf zu Infinity prüfen
			if ( finalExp >= 0x1F )
				return finalSign == 1 ? NegativeInfinity : PositiveInfinity;

			// Unterlauf zu Subnormal prüfen
			if ( finalExp <= 0 )
				return new FloatE5M2(finalSign, 0, (byte)resMant);

			return new FloatE5M2(finalSign, (byte)finalExp, (byte)resMant);
		}

		public static FloatE5M2 Div ( FloatE5M2 a, FloatE5M2 b ) {
			// 1. Sonderfälle auf Bit-Ebene abfangen
			if ( IsNaN(a) || IsNaN(b) ) return NaN;
			if ( IsZero(b) ) return IsZero(a) ? NaN : (a.Sign ^ b.Sign ? NegativeInfinity : PositiveInfinity);
			if ( IsInfinity(b) ) return IsInfinity(a) ? NaN : (a.Sign ^ b.Sign ? NegativeZero : Zero);
			if ( IsZero(a) || IsInfinity(a) ) return a.Sign ^ b.Sign ? Negate(a) : a;

			// Vorzeichen bestimmen
			byte finalSign = (byte)(a.Sign ^ b.Sign ? 1 : 0);

			Fast_Byte expA = a.Exponent;
			Fast_Byte expB = b.Exponent;

			Fast_Byte mantA = (byte)(a.Mantissa | (expA != 0 ? 0x04 : 0x00));
			Fast_Byte mantB = (byte)(b.Mantissa | (expB != 0 ? 0x04 : 0x00));

			Fast_Int realExpA = expA == 0 ? 1 : expA;
			Fast_Int realExpB = expB == 0 ? 1 : expB;

			// Exponenten subtrahieren und Bias (15) wieder aufschlagen
			Fast_Int finalExp = realExpA - realExpB + 15;

			// --- Nutzung deines größeren Registers für die Bruchrechnung ---
			// Wir schieben den Zähler nach links, um genug "Futter" für die Ganzzahldivision zu haben
			Fast_UInt extendedMantA = (uint)(mantA << 4);
			Fast_UInt resMant = (uint)(extendedMantA / mantB);

			if ( resMant == 0 ) return finalSign == 1 ? NegativeZero : Zero;

			// Renormalisierung im Shiftraster
			while ( resMant >= 0x08 ) {
				resMant >>= 1;
				finalExp++;
			}
			while ( resMant < 0x04 && finalExp > 1 ) {
				resMant <<= 1;
				finalExp--;
			}

			// Hidden Bit isolieren/löschen (nur die untersten 2 Bits behalten)
			resMant &= 0x03;

			// Grenzen validieren
			if ( finalExp >= 0x1F )
				return finalSign == 1 ? NegativeInfinity : PositiveInfinity;

			if ( finalExp <= 0 )
				return new FloatE5M2(finalSign, 0, (byte)resMant);

			return new FloatE5M2(finalSign, (byte)finalExp, (byte)resMant);
		}

		// <summary>
		/// Returns the smaller of two FP8 values.
		/// </summary>
		public static FloatE5M2 Min ( FloatE5M2 a, FloatE5M2 b )
			=> a < b ? a : b;

		/// <summary>
		/// Returns the larger of two FP8 values.
		/// </summary>
		public static FloatE5M2 Max ( FloatE5M2 a, FloatE5M2 b )
			=> a > b ? a : b;

		/// <summary>
		/// Compares this FP8 value with another using FP8 ordering rules.
		/// </summary>
		public int CompareTo ( object? obj ) {
			if ( (obj is FloatE5M2 o) ) {
				return (int)CompareTo(o);
			}
			throw new ArgumentException("Object is not a FloatE5M2 object");
		}

		/// <summary>
		/// Explicit <see cref="IComparable{T}"/> implementation that forwards
		/// to the extended <see cref="CompareTo(FloatE5M2)"/> method and casts
		/// the <see cref="CompareResult"/> to <see cref="int"/>.
		/// 
		/// This keeps the standard .NET comparison API compatible while still
		/// exposing a strongly typed comparison result via
		/// <see cref="IComparableEx{FloatE5M2}"/>.
		/// </summary>
		/// <param name="other">The value to compare with.</param>
		/// <returns>
		/// A signed integer indicating the relative order.
		/// </returns>
		int IComparable<FloatE5M2>.CompareTo ( FloatE5M2 other ) {
			return (int)CompareTo(other);
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public CompareResult CompareTo ( FloatE5M2 b ) {
			CompareResult _ret = CompareResult.Equal;

			if ( IsNaN(this) && !IsNaN(b) ) _ret = CompareResult.AIsSmallerB;

			else {
				if ( this < b ) _ret = CompareResult.AIsSmallerB;
				else if ( this > b ) _ret = CompareResult.AIsLargerB;
			}

			return _ret;
		}
		/// <summary>
		/// Converts the FP8 value into a raw byte vector.
		/// </summary>
		public FixedVector<byte> ToBytes () {
			return new FixedVector<byte>(m_baseBytes.ToBytes());
		}
		/// <summary>
		/// Constructs an FP8 value from a byte array.
		/// </summary>
		public static FloatE5M2 FromBytes ( byte[] bytes, long offset, Endian endian ) {
			return new FloatE5M2(bytes[0]);
		}
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public byte[] ToBytes ( Endian endian ) {
			return m_baseBytes.ToBytes(endian);
		}
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public void ToBytes ( ref byte[] destination, long offset, Endian endian ) {
			// Encode the underlying value into a temporary byte array.
			byte[] _dest = m_baseBytes.ToBytes(endian);

			// Ensure the destination buffer is large enough.
			long requiredSize = offset + _dest.LongLength;
			Buffer.LongCapacity(ref destination, requiredSize);

			// Copy the encoded bytes into the destination buffer at the given offset.
			Buffer.LongCopy(_dest, 0, destination, offset, _dest.LongLength);
		}

		
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public override bool Equals ( object? obj ) {
			if ( obj is FloatE5M2 a ) return this == a;
			return false;
		}
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public override int GetHashCode () {
			return m_baseBytes.GetHashCode();
		}
		public static bool operator < ( FloatE5M2 a, FloatE5M2 b ) {
			if ( IsNaN(a) || IsNaN(b) ) return false;

			bool _neg = IsNegative(a);

			if ( _neg != IsNegative(b) ) {
				if ( IsZero(a) && IsZero(b) )
					return false;
				return _neg;
			}

			return (a.m_baseBytes != b.m_baseBytes) && ((a.m_baseBytes < b.m_baseBytes) ^ _neg);
		}

		public static bool operator <= ( FloatE5M2 a, FloatE5M2 b ) {
			if ( IsNaN(a) || IsNaN(b) ) return false;

			bool _neg = IsNegative(a);

			if ( _neg != IsNegative(b) ) {
				if ( IsZero(a) && IsZero(b) )
					return false;
				return _neg;
			}

			return (a.m_baseBytes == b.m_baseBytes) && ((a.m_baseBytes < b.m_baseBytes) ^ _neg);
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public static bool operator >= ( FloatE5M2 a, FloatE5M2 b ) {
			return !(a < b);
		}
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public static bool operator == ( FloatE5M2 a, FloatE5M2 b ) {
			if ( IsNaN(a) || IsNaN(b) ) return false;
			return (a.m_baseBytes == b.m_baseBytes) || (IsZero(a) && IsZero(b));
		}
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public static bool operator != ( FloatE5M2 a, FloatE5M2 b ) {
			return !(a == b);
		}

		/// <summary>
		/// Greater‑than operator.
		/// </summary>
		public static bool operator > ( FloatE5M2 a, FloatE5M2 b ) => !(b <= a);

		/// <summary>
		/// Addition operator.
		/// </summary>
		public static FloatE5M2 operator + ( FloatE5M2 a, FloatE5M2 b ) => Add(a, b);

		/// <summary>
		/// Subtraction operator.
		/// </summary>
		public static FloatE5M2 operator - ( FloatE5M2 a, FloatE5M2 b ) => a + Negate(b);

		/// <summary>
		/// Multiplication operator.
		/// </summary>
		public static FloatE5M2 operator * ( FloatE5M2 a, FloatE5M2 b ) => Mul(a, b);

		/// <summary>
		/// Division operator.
		/// </summary>
		public static FloatE5M2 operator / ( FloatE5M2 a, FloatE5M2 b ) => Div(a, b);

		/// <summary>
		/// Increment operator.
		/// </summary>
		public static FloatE5M2 operator ++ ( FloatE5M2 a ) => a + One;

		/// <summary>
		/// Decrement operator.
		/// </summary>
		public static FloatE5M2 operator -- ( FloatE5M2 a ) => a - One;

		/// <summary>
		/// Encodes sign, exponent, and mantissa fields into a single FP8 byte.
		/// </summary>
		internal static byte Encode ( byte sign, byte exponent, byte mantissa ) {
			Fast_Byte b = 0;

			
			b.At(0, (byte)((mantissa >> 0) & 1));
			b.At(1, (byte)((mantissa >> 1) & 1));

			b.At(2, (byte)((exponent >> 0) & 1));
			b.At(3, (byte)((exponent >> 1) & 1));
			b.At(4, (byte)((exponent >> 2) & 1));
			b.At(5, (byte)((exponent >> 3) & 1));
			b.At(6, (byte)((exponent >> 4) & 1));
			b.At(7, sign);           // Signbit



			return b.Value;
		}

		public bool Equals ( FloatE5M2 other ) {
			return ( this == other );
		}

		public static FloatE5M2 FromComponent ( Fast_Byte sign, Fast_Byte mantissa, Fast_Byte expotent ) {
			return new FloatE5M2(Encode(sign.Value, expotent.Value, mantissa.Value));
		}
	}
}
