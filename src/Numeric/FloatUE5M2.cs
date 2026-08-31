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
	/// Represents an unsigned FP8 floating‑point value in E5M2 format.
	/// 
	/// This type has no sign bit; therefore all values are non‑negative.
	/// <para>
	/// • MinValue is always 0.  
	/// • Negation is a no‑op.  
	/// • Subtraction is saturating: results below 0 clamp to 0.  
	/// </para>
	/// 
	/// The implementation follows strict FP8 bit‑pattern semantics and performs
	/// all arithmetic without converting to host floating‑point types.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	[HashAlgorithm(typeof(BernsteinHash), Endian.System)]
	public struct FloatUE5M2 : IFP8<FloatUE5M2>, IP8UMXEnable<Fast_Byte> {
		private Fast_Byte m_baseBytes;

		/// <summary>Raw 8‑bit storage (FP8 E5M2 encoded).</summary>
		public Fast_Byte ToBase => m_baseBytes;

		/// <summary>Sign bits (always 1).</summary>
		public Fast_Byte SignBits => (byte)1;
		/// <summary>Exponent bits (5).</summary>
		public Fast_Byte ExponentBits => (byte)5;
		/// <summary>Mantissa bits (2).</summary>
		public Fast_Byte MantissaBits => (byte)2;
		/// <summary>Exponent bias (2^(k-1)-1 = 15).</summary>
		public Fast_Byte ExponentBias => (byte)15;
		/// <summary>Total bits (8).</summary>
		public Fast_Byte TotalBits => (byte)8;
		/// <summary>Hidden bit mask (1 << MantissaBits).</summary>
		public Fast_Byte HiddenBit => (byte)(1 << 2); // 0x04

		public Fast_Byte MantissaMask => (byte)0x03;
		public Fast_Byte MaxExponent => (byte)0x1F;
		public Fast_Byte ShiftRaster => (byte)4;

		/// <summary>
		/// Gets the sign bit. Always false
		/// </summary>
		public bool Sign => false;

		/// <summary>
		/// Gets the sign bit, as Fast_Byte
		/// </summary>
		public Fast_Byte NSign => 0;

		/// <summary>Exponent field (bits 2..6).</summary>
		public Fast_Byte Exponent => ((m_baseBytes >> 2) & MaxExponent);

		/// <summary>Mantissa field (bits 0..1).</summary>
		public Fast_Byte Mantissa => (m_baseBytes & MantissaMask);

		/// <summary>
		/// Represents the FP8 value +1.0.
		/// </summary>
		public static FloatUE5M2 One => new FloatUE5M2(0x3C);
		/// <summary>
		/// Represents the FP8 value +1.0.
		/// </summary>
		public static FloatUE5M2 NegativeOne => One;
		/// <summary>
		/// Represents the FP8 value +0.0.
		/// </summary>
		public static FloatUE5M2 Zero => new FloatUE5M2(0x00);
		/// <summary>
		/// Represents the FP8 value +0.0.
		/// </summary>
		public static FloatUE5M2 NegativeZero => Zero;

		/// <summary>
		/// Is MXFloat8 Supported: yes
		/// </summary>
		public static bool IsMXSupport => true;

		public static FloatUE5M2 PositiveInfinity => new FloatUE5M2(0x7C);

		public static FloatUE5M2 NegativeInfinity => PositiveInfinity;

		public static FloatUE5M2 NaN => new FloatUE5M2(0x7E);

		public static FloatUE5M2 NaN2 => NaN;

		/// <summary>
		/// Smallest representable positive subnormal value (2^-16 ≈ 0.00001525).
		/// </summary>
		public static FloatUE5M2 Epsilon => new FloatUE5M2(0x01);

		public static FloatUE5M2 E => new FloatUE5M2(0x41);

		public static FloatUE5M2 Tau => new FloatUE5M2(0x4);

		public static FloatUE5M2 Pi => new FloatUE5M2(0x46);

		/// <summary>
		/// Largest representable finite positive value (57344).
		/// </summary>
		public static FloatUE5M2 MaxValue => new FloatUE5M2(0x7B);

		/// <summary>
		/// Smallest representable finite negative value (0).
		/// </summary>
		public static FloatUE5M2 MinValue => Zero;


		/// <summary>
		/// Create from raw byte.
		/// </summary>
		public FloatUE5M2 ( byte raw ) { m_baseBytes = raw; }

		/// <summary>
		/// Create from fields.
		/// </summary>
		public FloatUE5M2 ( byte sign, byte exponent, byte mantissa ) {
			m_baseBytes = (byte)((1 << 7) | ((exponent & 0x1F) << 2) | (mantissa & 0x03));
		}

		/// <summary>
		/// True if the value is Zero
		/// </summary>
		public static bool IsZero ( FloatUE5M2 value )
			=> value.Exponent == 0x00 && value.Mantissa == 0x00;

		public static bool IsNegative ( FloatUE5M2 value ) => value.Sign;
		/// <summary>
		/// True if the value is Not-a-Number (NaN).
		/// </summary>
		public static bool IsNaN ( FloatUE5M2 value )
			=> value.Exponent == 0x1F && value.Mantissa != 0x00;

		/// <summary>
		/// True if the value is positive or negative infinity.
		/// </summary>
		public static bool IsInfinity ( FloatUE5M2 value )
			=> value.Exponent == 0x1F && value.Mantissa == 0x00;

		public static bool IsFinite ( FloatUE5M2 value ) => !IsNaN(value);
		/// <summary>
		/// True if the value is subnormal (denormalized).
		/// </summary>
		public static bool IsSubnormal ( FloatUE5M2 value )
			=> value.Exponent == 0x00 && value.Mantissa != 0x00;

		/// <summary>
		/// True if the value is Normal
		/// </summary>
		public static bool IsNormal ( FloatUE5M2 value )
			=> value.Exponent != 0 && value.Exponent != 0x1F;

		public static bool IsInteger ( FloatUE5M2 value ) {
			if ( IsNaN(value) ) return false;

			// Unterhalb vom Bias (1.0) ist nur die Null eine Ganzzahl
			if ( value.Exponent < 15 ) return IsZero(value);

			// Ab Exponent 17 (Bias + 2) schiebt sich das Komma komplett hinter die Mantisse.
			// Ab hier ist JEDER darstellbare Wert (auch Infinity) eine Ganzzahl.
			if ( value.Exponent >= 17 ) return !IsInfinity(value);

			int shift = value.Exponent.Value - 15;
			return (value.Mantissa & (1 >> shift)) == 0;
		}


		


		/// <summary>
		/// Returns the absolute value of the FP8 number.
		/// </summary>
		public static FloatUE5M2 Abs ( FloatUE5M2 value ) {
			return value;
		}

		/// <summary>
		/// Returns the negation of the FP8 number.
		/// </summary>
		public static FloatUE5M2 Negate ( FloatUE5M2 x ) {
			return x;
		}

		/// <summary>
		/// Returns +1, 0, or +1 depending on the sign of the value.
		/// </summary>
		public static FloatUE5M2 Signum ( FloatUE5M2 x ) {
			if ( IsNaN(x) ) return FloatUE5M2.NaN;
			if ( IsZero(x) ) return FloatUE5M2.Zero;
			return FloatUE5M2.One;
		}

		/// <summary>
		/// Returns the largest integer less than or equal to the value.
		/// </summary>
		public static FloatUE5M2 Floor ( FloatUE5M2 value ) {
			if ( IsNaN(value) || IsInfinity(value) ) return value;

			byte exp = (byte)(value.Exponent);

			// Werte kleiner als 1.0 werden immer zu 0 abgerundet
			if ( exp < 15 ) return Zero;

			// Ab Exponent 17 (Wert >= 4.0) ist jede darstellbare Zahl bereits eine Ganzzahl
			if ( exp >= 17 ) return value;

			// Bereich dazwischen (Exponent 15 und 16)
			int shift = (int)(exp - 15);

			// Wir erstellen eine Bitmaske für die Nachkommastellen der Mantisse
			// Bei Exp 15 (shift = 0): Alle 2 Bits der Mantisse sind Nachkomma
			// Bei Exp 16 (shift = 1): Das unterste Bit (Bit 0) ist Nachkomma
			byte fractionMask = (byte)( 0x03 >> shift);

			// Wenn keine Nachkommastellen gesetzt sind, ist es bereits eine Ganzzahl
			if ( (value.Mantissa & fractionMask) == 0 ) return value;

			// Nachkommastellen wegschneiden (Bits auf 0 setzen)
			byte newMantissa = (byte)(value.Mantissa & ~fractionMask);
			return new FloatUE5M2(0, exp, newMantissa);
		}

		/// <summary>
		/// Returns the smallest integer greater than or equal to the value.
		/// </summary>
		public static FloatUE5M2 Ceil ( FloatUE5M2 value ) {
			if ( IsNaN(value) || IsInfinity(value) ) return value;

			// Werte kleiner oder gleich 0 bleiben 0
			if ( IsZero(value) ) return Zero;

			byte exp = (byte)(value.Exponent);

			// Werte zwischen 0.0 (exklusive) und 1.0 werden immer zu 1.0 aufgerundet
			if ( exp < 15 ) return One;

			// Ab Exponent 17 (Wert >= 4.0) ist jede darstellbare Zahl bereits eine Ganzzahl
			if ( exp >= 17 ) return value;

			// Bereich dazwischen (Exponent 15 und 16)
			int shift = exp - 15;
			byte fractionMask = (byte)(0x03 >> shift);

			// Wenn keine Nachkommastellen gesetzt sind, ist es bereits eine Ganzzahl
			if ( (value.Mantissa & fractionMask) == 0 ) return value;

			// Nachkommastellen wegschneiden und 1 auf den Ganzzahl-Anteil addieren
			// Wir addieren den Wert 1 an der Stelle, wo das Komma sitzt (1 << shift)
			byte steppedMantissa = (byte)((value.Mantissa & ~fractionMask) + (1 << shift));
			byte finalExp = exp;

			// Falls die Mantisse durch das Aufrunden überläuft (wird >= 4 bzw. Bit 2 wird gesetzt)
			if ( (steppedMantissa & 0x04) != 0 ) {
				steppedMantissa >>= 1;
				finalExp++;
			}

			steppedMantissa &= 0x03; // Hidden Bit abschneiden

			if ( finalExp >= 0x1F ) return PositiveInfinity;

			return new FloatUE5M2(0, finalExp, steppedMantissa);
		}

		/// <summary>
		/// Truncates the fractional part of the FP8 value.
		/// </summary>
		public static FloatUE5M2 Trunc ( FloatUE5M2 x ) {
			if ( IsNaN(x) ) return x;
			if ( x.Exponent >= x.ExponentBias ) return x;
			return FloatUE5M2.Zero;
		}

		/// <summary>
		/// Clamps the value to the inclusive range [min, max].
		/// </summary>
		public static FloatUE5M2 Clamp ( FloatUE5M2 x, FloatUE5M2 min, FloatUE5M2 max ) {
			if ( x < min ) return min;
			if ( x > max ) return max;
			return x;
		}

		public static FloatUE5M2 Add ( FloatUE5M2 a, FloatUE5M2 b ) {
			// 1. Sonderfälle (NaN und Infinity) direkt auf Bit-Ebene abfangen
			if ( IsNaN(a) || IsNaN(b) ) return NaN;
			if ( IsInfinity(a) ) return IsInfinity(b) && (a.Sign != b.Sign) ? NaN : a;
			if ( IsInfinity(b) ) return b;
			if ( IsZero(a) ) return b;
			if ( IsZero(b) ) return a;

			byte expA = a.Exponent.Value;
			byte expB = b.Exponent.Value;

			// Implizites Bit (Hidden Bit) hinzufügen: 1.0 bei normalen Zahlen, 0.0 bei subnormalen
			byte mantA = (byte)(a.Mantissa | (expA != 0 ? 0x04 : 0x00));
			byte mantB = (byte)(b.Mantissa | (expB != 0 ? 0x04 : 0x00));

			// Exponenten anpassen (Subnormale Zahlen haben implizit Exponent 1)
			ushort realExpA = (ushort)(expA == 0 ? 1 : expA);
			ushort realExpB = (ushort)(expB == 0 ? 1 : expB);

			Fast_Int finalExp = realExpA;

			// --- Nutzung deines größeren Registers für die Berechnung ---
			// Wir shiften die extrahierten Basismantissen nach links in das Fixpunktraster
			ushort shiftedA = (ushort)(mantA << 4);
			ushort shiftedB = (ushort)(mantB << 4);
			ushort resMant = 0;

			// 1. Schritt: Mantissen auf denselben Exponenten ausrichten
			if ( realExpA >= realExpB ) {
				var shift = realExpA - realExpB;
				shiftedB >>= shift;
				finalExp = realExpA;
			} else {
				var shift = realExpB - realExpA;
				shiftedA >>= shift;
				finalExp = realExpB;
			}

			
			// Gleiche Vorzeichen -> Einfach addieren
			resMant = (ushort)(shiftedA + shiftedB);

			if ( resMant == 0 ) return Zero;


			while ( resMant >= 0x80 ) {
				resMant >>= 1;
				finalExp++;
			}
			while ( resMant < 0x40 && finalExp > 1 ) {
				resMant <<= 1;
				finalExp--;
			}

			resMant >>= 4;
			resMant &= 0x03; // Hidden Bit auf Bit-Position 2 entfernen

			if ( finalExp >= 0x1F ) return PositiveInfinity;

			// Unterlauf zu Subnormal prüfen
			if ( finalExp <= 0 )
				return new FloatUE5M2(0, 0, (byte)resMant);

			return new FloatUE5M2(0, (byte)finalExp, (byte)resMant);
		}

		public static FloatUE5M2 Mul ( FloatUE5M2 a, FloatUE5M2 b ) {
			// 1. Sonderfälle auf Bit-Ebene prüfen
			if ( IsNaN(a) || IsNaN(b) ) return NaN;

			// Kreuzprüfung für Unendlich und Null
			if ( IsInfinity(a) || IsInfinity(b) )
				return (IsZero(a) || IsZero(b)) ? NaN : PositiveInfinity;

			if ( IsZero(a) || IsZero(b) )
				return Zero;

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

			if ( resMant == 0 ) return Zero;

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
				return PositiveInfinity;

			// Unterlauf zu Subnormal prüfen
			if ( finalExp <= 0 )
				return new FloatUE5M2(0, 0, (byte)resMant);

			return new FloatUE5M2(0, (byte)finalExp, (byte)resMant);
		}

		public static FloatUE5M2 Div ( FloatUE5M2 a, FloatUE5M2 b ) {
			// 1. Sonderfälle auf Bit-Ebene abfangen
			if ( IsNaN(a) || IsNaN(b) ) return NaN;
			if ( IsZero(b) ) return IsZero(a) ? NaN : PositiveInfinity;
			if ( IsInfinity(b) ) return IsInfinity(a) ? NaN : Zero;
			if ( IsZero(a) || IsInfinity(a) ) return a;

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

			if ( resMant == 0 ) return Zero;

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
				return PositiveInfinity;

			if ( finalExp <= 0 )
				return new FloatUE5M2(0, 0, (byte)resMant);

			return new FloatUE5M2(0, (byte)finalExp, (byte)resMant);
		}

		public static FloatUE5M2 Sub ( FloatUE5M2 a, FloatUE5M2 b ) {
			// 1. Sonderfälle auf Bit-Ebene abfangen
			if ( IsNaN(a) || IsNaN(b) ) return NaN;

			// Unendlich-Regeln
			if ( IsInfinity(a) ) return IsInfinity(b) ? NaN : PositiveInfinity;
			if ( IsInfinity(b) ) return Zero; // Sättigung: Irgendwas minus Unendlich wird zu 0 gekappt
			if ( IsZero(b) ) return a;
			if ( IsZero(a) ) return Zero; // Sättigung: 0 minus Irgendwas wird zu 0 gekappt

			// Wenn b größer oder gleich a ist, laufen wir garantiert unter oder gegen Null
			if ( a <= b ) return Zero;

			Fast_Byte expA = a.Exponent;
			Fast_Byte expB = b.Exponent;

			// Hidden Bit hinzufügen
			Fast_Byte mantA = (byte)(a.Mantissa | (expA != 0 ? 0x04 : 0x00));
			Fast_Byte mantB = (byte)(b.Mantissa | (expB != 0 ? 0x04 : 0x00));

			Fast_Int realExpA = expA == 0 ? 1 : expA;
			Fast_Int realExpB = expB == 0 ? 1 : expB;

			Fast_Int finalExp = realExpA;

			// Nutzen deines größeren Registers, damit beim Shiften nichts verloren geht
			Fast_UShort shiftedA = (ushort)(mantA << 4);
			Fast_UShort shiftedB = (ushort)(mantB << 4);
			Fast_UShort resMant = 0;

			// Mantissen auf denselben Exponenten ausrichten
			if ( realExpA >= realExpB ) {
				var shift = realExpA - realExpB;
				shiftedB >>= (int)shift;
				finalExp = realExpA;
			} else {
				var shift = realExpB - realExpA;
				shiftedA >>= (int)shift;
				finalExp = realExpB;
			}

			// Da a > b garantiert ist, ist shiftedA immer größer als shiftedB
			resMant = (ushort)(shiftedA - shiftedB);

			if ( resMant == 0 ) return Zero;

			// Renormalisierung im Shiftraster (Wir suchen das führende Bit)
			while ( resMant >= 0x80 ) {
				resMant >>= 1;
				finalExp++;
			}
			while ( resMant < 0x40 && finalExp > 1 ) {
				resMant <<= 1;
				finalExp--;
			}

			// Zurück in das 2-Bit-Raster stutzen
			resMant >>= 4;
			resMant &= 0x03; // Hidden Bit löschen

			// Überlauf zu Infinity prüfen
			if ( finalExp >= 0x1F ) return PositiveInfinity;

			// Unterlauf zu Subnormal prüfen
			if ( finalExp <= 0 ) return new FloatUE5M2(0, 0, (byte)resMant);

			return new FloatUE5M2(0, (byte)finalExp, (byte)resMant);
		}

		// <summary>
		/// Returns the smaller of two FP8 values.
		/// </summary>
		public static FloatUE5M2 Min ( FloatUE5M2 a, FloatUE5M2 b )
			=> a < b ? a : b;

		/// <summary>
		/// Returns the larger of two FP8 values.
		/// </summary>
		public static FloatUE5M2 Max ( FloatUE5M2 a, FloatUE5M2 b )
			=> a > b ? a : b;

		/// <summary>
		/// Compares this FP8 value with another using FP8 ordering rules.
		/// </summary>
		public int CompareTo ( object? obj ) {
			if ( (obj is FloatUE5M2 o) ) {
				return (int)CompareTo(o);
			}
			throw new ArgumentException("Object is not a FloatUE5M2 object");
		}

		/// <summary>
		/// Explicit <see cref="IComparable{T}"/> implementation that forwards
		/// to the extended <see cref="CompareTo(FloatUE5M2)"/> method and casts
		/// the <see cref="CompareResult"/> to <see cref="int"/>.
		/// 
		/// This keeps the standard .NET comparison API compatible while still
		/// exposing a strongly typed comparison result via
		/// <see cref="IComparableEx{FloatUE5M2}"/>.
		/// </summary>
		/// <param name="other">The value to compare with.</param>
		/// <returns>
		/// A signed integer indicating the relative order.
		/// </returns>
		int IComparable<FloatUE5M2>.CompareTo ( FloatUE5M2 other ) {
			return (int)CompareTo(other);
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public CompareResult CompareTo ( FloatUE5M2 b ) {
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
		public static FloatUE5M2 FromBytes ( byte[] bytes, long offset, Endian endian ) {
			return new FloatUE5M2(bytes[0]);
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
			if ( obj is FloatUE5M2 a ) return this == a;
			return false;
		}
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public override int GetHashCode () {
			return m_baseBytes.GetHashCode();
		}
		public static bool operator < ( FloatUE5M2 a, FloatUE5M2 b ) {
			if ( IsNaN(a) || IsNaN(b) ) return false;

			if ( IsZero(a) && IsZero(b) )
				return false;

			return (a.m_baseBytes != b.m_baseBytes) && ((a.m_baseBytes < b.m_baseBytes));
		}

		public static bool operator <= ( FloatUE5M2 a, FloatUE5M2 b ) {
			if ( IsNaN(a) || IsNaN(b) ) return false;
			if ( IsZero(a) && IsZero(b) ) return false;

			return (a.m_baseBytes == b.m_baseBytes) && ((a.m_baseBytes < b.m_baseBytes));
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public static bool operator >= ( FloatUE5M2 a, FloatUE5M2 b )
			=> (a > b) || (a == b);
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public static bool operator == ( FloatUE5M2 a, FloatUE5M2 b ) {
			if ( IsNaN(a) || IsNaN(b) ) return false;
			return (a.m_baseBytes == b.m_baseBytes) || (IsZero(a) && IsZero(b));
		}
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public static bool operator != ( FloatUE5M2 a, FloatUE5M2 b ) {
			return !(a == b);
		}

		/// <summary>
		/// Greater‑than operator.
		/// </summary>
		public static bool operator > ( FloatUE5M2 a, FloatUE5M2 b ) {
			if ( IsNaN(a) || IsNaN(b) ) return false;
			if ( IsZero(a) && IsZero(b) ) return false;
			return a.m_baseBytes > b.m_baseBytes;
		}

		/// <summary>
		/// Addition operator.
		/// </summary>
		public static FloatUE5M2 operator + ( FloatUE5M2 a, FloatUE5M2 b ) => Add(a, b);

		/// <summary>
		/// Subtraction operator.
		/// </summary>
		public static FloatUE5M2 operator - ( FloatUE5M2 a, FloatUE5M2 b ) => Sub(a, b);

		/// <summary>
		/// Multiplication operator.
		/// </summary>
		public static FloatUE5M2 operator * ( FloatUE5M2 a, FloatUE5M2 b ) => Mul(a, b);

		/// <summary>
		/// Division operator.
		/// </summary>
		public static FloatUE5M2 operator / ( FloatUE5M2 a, FloatUE5M2 b ) => Div(a, b);

		/// <summary>
		/// Increment operator.
		/// </summary>
		public static FloatUE5M2 operator ++ ( FloatUE5M2 a ) => a + One;

		/// <summary>
		/// Decrement operator.
		/// </summary>
		public static FloatUE5M2 operator -- ( FloatUE5M2 a ) => a - One;
		/// <summary>
		/// Encodes sign, exponent, and mantissa fields into a single FP8 byte.
		/// </summary>
		private static byte Encode ( byte sign, byte exponent, byte mantissa ) =>
			(byte)(((sign & 1) << 7) | ((exponent & 0x1F) << 2) | (mantissa & 0x03));

		public bool Equals ( FloatUE5M2 other ) {
			return (this == other);
		}

		public static FloatUE5M2 FromComponent ( Fast_Byte sign, Fast_Byte mantissa, Fast_Byte expotent ) {
			return new FloatUE5M2(Encode(sign.Value, expotent.Value, mantissa.Value));
		}
	}
}
