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
	/// <para>• 4 exponent bits (bias = 7)</para>
	/// <para>• 3 mantissa bits</para>
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
	public struct FloatUE4M3 : IFP8<FloatUE4M3>, IP8UMXEnable<Fast_Byte> {
		private Fast_Byte m_baseBytes;

		/// <summary>
		/// Returns the raw FP8 storage byte.
		/// </summary>
		public Fast_Byte ToBase => m_baseBytes;

		/// <summary>
		/// Number of bits used for the sign field (always 1).
		/// </summary>
		public Fast_Byte SignBits => 1;

		/// <summary>
		/// Number of bits used for the exponent field (always 4).
		/// </summary>
		public Fast_Byte ExponentBits => 4;

		/// <summary>
		/// Number of bits used for the mantissa field (always 3).
		/// </summary>
		public Fast_Byte MantissaBits => 3;

		/// <summary>
		/// Exponent bias used by the E4M3 format (always 7).
		/// </summary>
		public Fast_Byte ExponentBias => 7;

		/// <summary>
		/// Total number of bits in the FP8 representation (always 8).
		/// </summary>
		public Fast_Byte TotalBits => 8;

		/// <summary>
		/// Hidden bit used for normalized numbers. Always 1 for E4M3.
		/// </summary>
		public Fast_Byte HiddenBit => 1;

		/// <summary>
		/// Gets the sign bit. True indicates a negative value.
		/// </summary>
		public bool Sign => m_baseBytes.Is(7) == 1;

		/// <summary>
		/// Gets the sign bit, as Fast_Byte
		/// </summary>
		public Fast_Byte NSign => m_baseBytes.Is(7);

		/// <summary>
		/// Extracts the exponent field (bits 3–6).
		/// </summary>
		public Fast_Byte Exponent {
			get {
				byte mask = m_baseBytes.CreateMask(3, 4); // E4M3: 4 Exponentbits ab Bit 3
				return (byte)((m_baseBytes.Value & mask) >> 3);
			}
		}
		/// <summary>
		/// Extracts the mantissa field (bits 0–2).
		/// </summary>
		public Fast_Byte Mantissa {
			get {
				byte mask = m_baseBytes.CreateMask(0, 3); // E4M3: 3 Mantissabits ab Bit 0
				return (byte)(m_baseBytes.Value & mask);
			}
		}
		/// <summary>
		/// Represents the FP8 value +0.0.
		/// </summary>
		public static FloatUE4M3 Zero => new FloatUE4M3(0x00);


		/// <summary>
		/// Represents the FP8 value +1.0.
		/// </summary>
		public static FloatUE4M3 One => new FloatUE4M3(0x38);

		/// <summary>
		/// Represents the FP8 value +1.0.
		/// </summary>
		public static FloatUE4M3 NegativeOne => One;

		/// <summary>
		/// Represents the FP8 value +0.0.
		/// </summary>
		public static FloatUE4M3 NegativeZero => Zero;

		/// <summary>
		/// Infinity is not supported in the E4M3 format.
		/// </summary>
		public static FloatUE4M3 PositiveInfinity => throw new NotSupportedException();

		/// <summary>
		/// Infinity is not supported in the E4M3 format.
		/// </summary>
		public static FloatUE4M3 NegativeInfinity => throw new NotSupportedException();

		/// <summary>
		/// Represents the canonical NaN encoding (exponent = 0x0F, mantissa = 0x07).
		/// </summary>
		public static FloatUE4M3 NaN => new FloatUE4M3(0x7F);

		/// <summary>
		/// Represents an alternative NaN encoding (sign bit set).
		/// </summary>
		public static FloatUE4M3 NaN2 => NaN;

		/// <summary>
		/// Smallest positive subnormal increment.
		/// </summary>
		public static FloatUE4M3 Epsilon => new FloatUE4M3(0x01);

		/// <summary>
		/// Approximation of Euler’s number e.
		/// </summary>
		public static FloatUE4M3 E => new FloatUE4M3(0x43);

		/// <summary>
		/// Approximation of τ = 2π.
		/// </summary>
		public static FloatUE4M3 Tau => new FloatUE4M3(0x4D);

		/// <summary>
		/// Approximation of π.
		/// </summary>
		public static FloatUE4M3 Pi => new FloatUE4M3(0x45);

		/// <summary>
		/// Smallest representable negative value.
		/// </summary>
		public static FloatUE4M3 MinValue => Zero;

		/// <summary>
		/// Largest representable positive value.
		/// </summary>
		public static FloatUE4M3 MaxValue => new FloatUE4M3(0x7E);

		/// <inheritdoc/>
		public static bool IsMXSupport => true;
		/// <inheritdoc/>
		public Fast_Byte MantissaMask => 0x07;
		/// <inheritdoc/>
		public Fast_Byte MaxExponent => 0x0F;
		/// <inheritdoc/>
		public Fast_Byte ShiftRaster => 5;

		/// <summary>
		/// Initializes a new FP8 value set to zero.
		/// </summary>
		public FloatUE4M3 () {
			m_baseBytes = 0;
		}
		/// <summary>
		/// Initializes a new FP8 value from a raw byte.
		/// </summary>
		public FloatUE4M3 ( byte raw ) => m_baseBytes = raw;

		/// <summary>
		/// Constructs an FP8 value from explicit sign, exponent, and mantissa fields.
		/// </summary>
		public FloatUE4M3 ( byte sign, byte exponent, byte mantissa )
			=> m_baseBytes = Encode(0, exponent, mantissa);

		/// <summary>
		/// Determines whether the value is zero (positive or negative).
		/// </summary>
		public static bool IsZero ( FloatUE4M3 value )
			=> (value.m_baseBytes & 0x7F) == 0;

		/// <summary>
		/// Determines whether the value is negative.
		/// </summary>
		public static bool IsNegative ( FloatUE4M3 value )
			=> false;

		/// <summary>
		/// Determines whether the value is a NaN.
		/// </summary>
		public static bool IsNaN ( FloatUE4M3 value )
			=> value.Exponent == 0x0F && value.Mantissa == 0x07;

		/// <summary>
		/// E4M3 does not support infinity.
		/// </summary>
		public static bool IsInfinity ( FloatUE4M3 value )
			=> false;

		/// <summary>
		/// Determines whether the value is finite (not NaN).
		/// </summary>
		public static bool IsFinite ( FloatUE4M3 value )
			=> !IsNaN(value);

		/// <summary>
		/// Determines whether the value is subnormal (exponent = 0 and mantissa ≠ 0).
		/// </summary>
		public static bool IsSubnormal ( FloatUE4M3 value )
			=> value.Exponent == 0 && value.Mantissa != 0;

		/// <summary>
		/// Determines whether the value is a normalized FP8 number.
		/// </summary>
		public static bool IsNormal ( FloatUE4M3 value )
			=> value.Exponent != 0 && value.Exponent != 0x0F;

		/// <summary>
		/// Determines whether the value represents an integer.
		/// </summary>
		public static bool IsInteger ( FloatUE4M3 x ) {
			if ( IsNaN(x) ) return false;
			if ( x.Exponent < x.ExponentBias ) return IsZero(x);
			return x.Mantissa == 0;
		}

		/// <summary>
		/// Returns the absolute value of the FP8 number.
		/// </summary>
		public static FloatUE4M3 Abs ( FloatUE4M3 value )
			=> new FloatUE4M3(0, value.Exponent.Value, value.Mantissa.Value);

		/// <summary>
		/// Returns the negation of the FP8 number.
		/// </summary>
		public static FloatUE4M3 Negate ( FloatUE4M3 x )
			=> new FloatUE4M3(0, x.Exponent.Value, x.Mantissa.Value);

		/// <summary>
		/// Returns −1, 0, or +1 depending on the sign of the value.
		/// </summary>
		public static FloatUE4M3 Signum ( FloatUE4M3 x ) {
			if ( IsNaN(x) ) return FloatUE4M3.NaN;
			if ( IsZero(x) ) return FloatUE4M3.Zero;
			return FloatUE4M3.One;
		}

		/// <summary>
		/// Returns the largest integer less than or equal to the value.
		/// </summary>
		public static FloatUE4M3 Floor ( FloatUE4M3 x ) {
			if ( IsNaN(x) ) return x;
			if ( x.Exponent >= x.ExponentBias ) return x;
			if ( IsZero(x) ) return x;
			return FloatUE4M3.Zero;
		}

		/// <summary>
		/// Returns the smallest integer greater than or equal to the value.
		/// </summary>
		public static FloatUE4M3 Ceil ( FloatUE4M3 x ) {
			if ( IsNaN(x) ) return x;
			if ( x.Exponent >= x.ExponentBias ) return x;
			if ( IsZero(x) ) return x;
			return FloatUE4M3.One;
		}

		/// <summary>
		/// Truncates the fractional part of the FP8 value.
		/// </summary>
		public static FloatUE4M3 Trunc ( FloatUE4M3 x ) {
			if ( IsNaN(x) ) return x;
			if ( x.Exponent >= x.ExponentBias ) return x;
			return FloatUE4M3.Zero;
		}

		/// <summary>
		/// Clamps the value to the inclusive range [min, max].
		/// </summary>
		public static FloatUE4M3 Clamp ( FloatUE4M3 x, FloatUE4M3 min, FloatUE4M3 max ) {
			if ( x < min ) return min;
			if ( x > max ) return max;
			return x;
		}
		/// <summary>
		/// Adds two FP8 values using E4M3 arithmetic rules.
		/// </summary>
		public static FloatUE4M3 Add ( FloatUE4M3 a, FloatUE4M3 b ) {
			if ( IsNaN(a) || IsNaN(b) ) return FloatUE4M3.NaN;
			if ( IsZero(a) ) return b;
			if ( IsZero(b) ) return a;

			Fast_Byte expA = a.Exponent;
			Fast_Byte expB = b.Exponent;

			Fast_Int mantA = (expA == 0 ? a.Mantissa : (a.Mantissa | 0x08));
			Fast_Int mantB = (expB == 0 ? b.Mantissa : (b.Mantissa | 0x08));

			Fast_Byte exp = expA;
			Fast_Int diff = expA - expB;

			if ( diff > 0 )
				mantB >>= diff;
			else if ( diff < 0 ) {
				mantA >>= -diff;
				exp = expB;
			}

			Fast_Int mant;

			mant = mantA + mantB;

			if ( mant == 0 )
				return FloatUE4M3.Zero;

			while ( (mant & 0x08) == 0 ) {
				mant <<= 1;
				exp--;
			}

			if ( exp <= 0 )
				return FloatUE4M3.Zero;

			if ( exp >= 0x0F )
				return FloatUE4M3.NaN;

			Fast_Byte finalMant = (byte)(mant & 0x07);

			return new FloatUE4M3(0, (byte)exp.Value, finalMant.Value);
		}

		public static FloatUE4M3 Sub ( FloatUE4M3 a, FloatUE4M3 b ) {
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
			Fast_Int mantA = (expA == 0 ? a.Mantissa : (a.Mantissa | 0x08));
			Fast_Int mantB = (expB == 0 ? b.Mantissa : (b.Mantissa | 0x08));

			Fast_Int realExpA = expA == 0 ? 1 : expA;
			Fast_Int realExpB = expB == 0 ? 1 : expB;

			Fast_Int finalExp = realExpA;

			// Nutzen deines größeren Registers, damit beim Shiften nichts verloren geht
			Fast_UShort shiftedA = (ushort)(mantA << 5);
			Fast_UShort shiftedB = (ushort)(mantB << 5);
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
			while ( resMant >= 0x200 ) {
				resMant >>= 1;
				finalExp++;
			}
			while ( resMant < 0x100 && finalExp > 1 ) {
				resMant <<= 1;
				finalExp--;
			}

			// Zurück in das 3-Bit-Raster stutzen
			resMant >>= 5;
			resMant &= 0x07; // Hidden Bit löschen

			// Überlauf zu Infinity prüfen
			if ( finalExp >= 0x0F ) return PositiveInfinity;

			// Unterlauf zu Subnormal prüfen
			if ( finalExp <= 0 ) return new FloatUE4M3(0, 0, (byte)resMant);

			return new FloatUE4M3(0, (byte)finalExp, (byte)resMant);
		}

		/// <summary>
		/// Multiplies two FP8 values using E4M3 arithmetic rules.
		/// </summary>
		public static FloatUE4M3 Mul ( FloatUE4M3 a, FloatUE4M3 b ) {
			if ( IsNaN(a) || IsNaN(b) ) return FloatUE4M3.NaN;
			if ( IsZero(a) || IsZero(b) ) return FloatUE4M3.Zero;

			Fast_Byte exp = a.Exponent + b.Exponent - a.ExponentBias;

			Fast_Int mantA = (a.Exponent == 0 ? a.Mantissa : (a.Mantissa | 0x08));
			Fast_Int mantB = (b.Exponent == 0 ? b.Mantissa : (b.Mantissa | 0x08));

			Fast_Int mant = mantA * mantB;

			while ( (mant & 0x40) != 0 ) {
				mant >>= 1;
				exp++;
			}

			mant >>= 3;

			if ( exp <= 0 )
				return FloatUE4M3.Zero;

			if ( exp >= 0x0F )
				return FloatUE4M3.NaN;

			byte finalMant = (byte)(mant & 0x07);

			return new FloatUE4M3(0, exp.Value, finalMant);
		}
		/// <summary>
		/// Divides one FP8 value by another using E4M3 arithmetic rules.
		/// </summary>
		public static FloatUE4M3 Div ( FloatUE4M3 a, FloatUE4M3 b ) {
			if ( IsNaN(a) || IsNaN(b) ) return FloatUE4M3.NaN;
			if ( IsZero(b) ) return FloatUE4M3.NaN;
			if ( IsZero(a) ) return FloatUE4M3.Zero;


			Fast_Byte exp = a.Exponent - b.Exponent + a.ExponentBias;

			Fast_Int mantA = (a.Exponent == 0 ? a.Mantissa : (a.Mantissa | 0x08));
			Fast_Int mantB = (b.Exponent == 0 ? b.Mantissa : (b.Mantissa | 0x08));

			Fast_Int mant = (mantA << 6) / mantB;

			while ( (mant & 0x08) == 0 ) {
				mant <<= 1;
				exp--;
			}

			if ( exp <= 0 )
				return FloatUE4M3.Zero;

			if ( exp >= 0x0F )
				return FloatUE4M3.NaN;

			byte finalMant = (byte)(mant & 0x07);

			return new FloatUE4M3(0, exp.Value, finalMant);
		}

		// <summary>
		/// Returns the smaller of two FP8 values.
		/// </summary>
		public static FloatUE4M3 Min ( FloatUE4M3 a, FloatUE4M3 b )
			=> a < b ? a : b;

		/// <summary>
		/// Returns the larger of two FP8 values.
		/// </summary>
		public static FloatUE4M3 Max ( FloatUE4M3 a, FloatUE4M3 b )
			=> a > b ? a : b;

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public bool Equals ( FloatUE4M3 other ) {
			return this == other;
		}
		/// <summary>
		/// Compares this FP8 value with another using FP8 ordering rules.
		/// </summary>
		public int CompareTo ( object? obj ) {
			if ( (obj is FloatUE4M3 o) ) {
				return (int)CompareTo(o);
			}
			throw new ArgumentException("Object is not a FloatUE4M3 object");
		}

		/// <summary>
		/// Explicit <see cref="IComparable{T}"/> implementation that forwards
		/// to the extended <see cref="CompareTo(FloatUE4M3)"/> method and casts
		/// the <see cref="CompareResult"/> to <see cref="int"/>.
		/// 
		/// This keeps the standard .NET comparison API compatible while still
		/// exposing a strongly typed comparison result via
		/// <see cref="IComparableEx{FloatUE4M3}"/>.
		/// </summary>
		/// <param name="other">The value to compare with.</param>
		/// <returns>
		/// A signed integer indicating the relative order.
		/// </returns>
		int IComparable<FloatUE4M3>.CompareTo ( FloatUE4M3 other ) {
			return (int)CompareTo(other);
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public CompareResult CompareTo ( FloatUE4M3 a ) {
			CompareResult _ret = CompareResult.Equal;

			if ( IsNaN(this) && !IsNaN(a) ) _ret = CompareResult.AIsSmallerB;

			else {
				if ( this < a ) _ret = CompareResult.AIsSmallerB;
				else if ( this > a) _ret = CompareResult.AIsLargerB;
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
		public static FloatUE4M3 FromBytes ( byte[] bytes, long offset, Endian endian ) {
			return new FloatUE4M3(bytes[0]);
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
			if ( obj is FloatUE4M3 a ) return this == a;
			return false;
		}
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public override int GetHashCode () {
			return m_baseBytes.GetHashCode();
		}
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public static bool operator < ( FloatUE4M3 a, FloatUE4M3 b ) {
			if ( IsNaN(a) || IsNaN(b) ) return false;

			if ( IsZero(a) && IsZero(b) ) return false;

			return (a.m_baseBytes != b.m_baseBytes) && ((a.m_baseBytes < b.m_baseBytes));
		}
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public static bool operator <= ( FloatUE4M3 a, FloatUE4M3 b ) {
			if ( IsNaN(a) || IsNaN(b) ) return false;
			if ( IsZero(a) && IsZero(b) ) return true;

			return (a.m_baseBytes == b.m_baseBytes) && ((a.m_baseBytes < b.m_baseBytes));
		}
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public static bool operator >= ( FloatUE4M3 a, FloatUE4M3 b ) {
			return !(a < b);
		}
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public static bool operator == ( FloatUE4M3 a, FloatUE4M3 b ) {
			if ( IsNaN(a) || IsNaN(b) ) return false;
			return (a.m_baseBytes == b.m_baseBytes) || (IsZero(a) && IsZero(b));
		}
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public static bool operator != ( FloatUE4M3 a, FloatUE4M3 b ) {
			return !(a == b);
		}

		/// <summary>
		/// Greater‑than operator.
		/// </summary>
		public static bool operator > ( FloatUE4M3 a, FloatUE4M3 b ) => !(b <= a);


		/// <summary>
		/// Addition operator.
		/// </summary>
		public static FloatUE4M3 operator + ( FloatUE4M3 a, FloatUE4M3 b ) => Add(a, b);

		/// <summary>
		/// Subtraction operator.
		/// </summary>
		public static FloatUE4M3 operator - ( FloatUE4M3 a, FloatUE4M3 b ) => Sub(a, b);

		/// <summary>
		/// Multiplication operator.
		/// </summary>
		public static FloatUE4M3 operator * ( FloatUE4M3 a, FloatUE4M3 b ) => Mul(a, b);

		/// <summary>
		/// Division operator.
		/// </summary>
		public static FloatUE4M3 operator / ( FloatUE4M3 a, FloatUE4M3 b ) => Div(a, b);

		/// <summary>
		/// Increment operator.
		/// </summary>
		public static FloatUE4M3 operator ++ ( FloatUE4M3 a ) => a + One;

		/// <summary>
		/// Decrement operator.
		/// </summary>
		public static FloatUE4M3 operator -- ( FloatUE4M3 a ) {
			return a - One;
		}
		/// <summary>
		/// Encodes sign, exponent, and mantissa fields into a single FP8 byte.
		/// </summary>
		internal static byte Encode ( byte sign, byte exponent, byte mantissa ) {
			Fast_Byte b = 0;

			b.At(7, sign);           // Signbit
			b.At(3, (byte)((exponent >> 0) & 1));
			b.At(4, (byte)((exponent >> 1) & 1));
			b.At(5, (byte)((exponent >> 2) & 1));
			b.At(6, (byte)((exponent >> 3) & 1));

			b.At(0, (byte)((mantissa >> 0) & 1));
			b.At(1, (byte)((mantissa >> 1) & 1));
			b.At(2, (byte)((mantissa >> 2) & 1));

			return b.Value;
		}
		public static FloatUE4M3 FromComponent ( Fast_Byte sign, Fast_Byte mantissa, Fast_Byte expotent ) {
			return new FloatUE4M3(Encode(sign.Value, expotent.Value, mantissa.Value));
		}

		
	}
}


