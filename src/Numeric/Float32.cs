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

using SystemEx.Collections.Generic;
using SystemEx.Utils;

namespace SystemEx.Numeric {

	/// <summary>
	/// Represents a 32‑bit IEEE‑754 single‑precision floating‑point value.
	/// 
	/// This format uses:
	/// <para>• 1 sign bit</para>
	/// <para>• 8 exponent bits (bias = 127)</para>
	/// <para>• 23 mantissa bits</para>
	/// 
	/// Normalized numbers include an implicit hidden bit (1 << 23).  
	/// Subnormal numbers use an exponent of zero and omit the hidden bit.
	/// 
	/// <para>
	/// The value is stored as a raw 32‑bit unsigned integer, allowing exact
	/// bit‑level manipulation of sign, exponent, and mantissa fields without
	/// relying on the host floating‑point unit.
	/// </para>
	/// 
	/// <para>
	/// All conversions use SystemEx endian‑aware utilities, ensuring consistent
	/// reinterpretation between byte arrays, raw bit patterns, and float values.
	/// </para>
	/// </summary>
	public struct Float32 : IFloat<Float32, uint> {
		private uint m_value;

		/// <inheritdoc/>
		public static Float32 Zero => 0.0f;
		/// <inheritdoc/>
		public static Float32 One => 1.0f;
		/// <inheritdoc/>
		public static Float32 NegativeOne => -1.0f;
		/// <inheritdoc/>
		public static Float32 NegativeZero => -0.0f;
		/// <inheritdoc/>
		public static Float32 PositiveInfinity => Single.PositiveInfinity;
		/// <inheritdoc/>
		public static Float32 NegativeInfinity => Single.NegativeInfinity;
		/// <inheritdoc/>
		public static Float32 NaN => Single.NaN;
		/// <inheritdoc/>
		public static Float32 NaN2 => throw new NotImplementedException();
		/// <inheritdoc/>
		public static Float32 Epsilon => Single.Epsilon;
		/// <inheritdoc/>
		public static Float32 E => Single.E;
		/// <inheritdoc/>
		public static Float32 Tau => Single.Tau;
		/// <inheritdoc/>
		public static Float32 Pi => Single.Tau;
		/// <inheritdoc/>
		public uint ToBase => m_value;
		/// <inheritdoc/>
		public uint SignBits => 1;
		/// <inheritdoc/>
		public uint ExponentBits => 8;
		/// <inheritdoc/>
		public uint MantissaBits => 23;
		/// <inheritdoc/>
		public uint ExponentBias => 127;
		/// <inheritdoc/>
		public uint TotalBits => 32;
		/// <inheritdoc/>
		public uint HiddenBit => 0x0080_0000;
		/// <inheritdoc/>
		public bool Sign => (m_value >> 31) != 0;
		/// <inheritdoc/>
		public uint Exponent => (m_value >> 23) & 0xFF;
		/// <inheritdoc/>
		public uint Mantissa => m_value & 0x7FFFFF;

		/// <summary>
		/// Smallest representable negative value.
		/// </summary>
		public static Float32 MinValue => Single.MinValue;

		/// <summary>
		/// Largest representable positive value.
		/// </summary>
		public static Float32 MaxValue => Single.MaxValue;

		/// <summary>
		/// Create a Float32 from a float value
		/// </summary>
		public Float32 (float fval) {
			m_value = fval.ToInteger();
		}

		/// <summary>
		/// Converts implicit a <see cref="float"/> value to <see cref="Float32"/>.
		/// </summary>
		/// <param name="val"></param>
		public static implicit operator Float32 (float val) {
			return new Float32(val);
		}

		/// <summary>
		/// Converts explicit a <see cref="Float32"/> to <see cref="float"/>. without Byteconverter.
		/// </summary>
		/// <param name="val"></param>
		public static explicit operator float ( Float32 val ) {
			byte[] _bb = val.ToBytes(Endian.System);
			return _bb.ToFloat();
		}
		/// <inheritdoc/>
		public static Float32 Abs ( Float32 value ) {
			return System.MathF.Abs( (float)value );
		}
		/// <inheritdoc/>
		public static Float32 Add ( Float32 a, Float32 b ) {
			return a.m_value + b.m_value;
		}
		/// <inheritdoc/>
		public static Float32 Ceil ( Float32 value ) {
			return System.MathF.Ceiling( (float)value );
		}
		/// <inheritdoc/>
		public static Float32 Clamp ( Float32 x, Float32 min, Float32 max ) {
			float xf = (float)x;
			float minf = (float)min;
			float maxf = (float)max;

			if ( xf < minf ) return min;
			if ( xf > maxf ) return max;
			return x;
		}
		/// <inheritdoc/> 
		public static Float32 Div ( Float32 a, Float32 b ) {
			return a.m_value / b.m_value;
		}
		/// <inheritdoc/>
		public static Float32 Sub ( Float32 a, Float32 b ) {
			return a.m_value  - b.m_value;
		}
		/// <inheritdoc/>
		public static Float32 Floor ( Float32 value ) {
			return System.MathF.Floor( (float)value );
		}
		/// <inheritdoc/>
		public static Float32 FromBytes ( byte[] bytes, long offset, Endian endian ) {
			return bytes.ToFloat(offset, endian);
		}
		/// <inheritdoc/>
		public static bool IsFinite ( Float32 value ) {
			uint exp = (value.m_value >> 23) & 0xFF;
			return exp != 0xFF;
		}
		/// <inheritdoc/>
		public static bool IsInfinity ( Float32 value ) {
			return Single.IsInfinity((float)value);
		}
		/// <inheritdoc/>
		public static bool IsInteger ( Float32 value ) {
			return Single.IsInteger((float)value);
		}
		/// <inheritdoc/>
		public static bool IsNaN ( Float32 value ) {
			return Single.IsNaN((float)value);
		}
		/// <inheritdoc/>
		public static bool IsNegative ( Float32 value ) {
			return Single.IsNegative((float)value);
		}
		/// <inheritdoc/>
		public static bool IsNormal ( Float32 value ) {
			return Single.IsNormal((float)value);
		}
		/// <inheritdoc/>
		public static bool IsSubnormal ( Float32 value ) {
			return Single.IsSubnormal((float)value);
		}
		/// <inheritdoc/>
		public static bool IsZero ( Float32 value ) {
			return (value.m_value & 0x7FFFFFFF) == 0;
		}
		/// <inheritdoc/>
		public static Float32 Max ( Float32 a, Float32 b ) {
			return a < b ? b : a;
		}
		/// <inheritdoc/>
		public static Float32 Min ( Float32 a, Float32 b ) {
			return (a < b) ? a : b;
		}
		/// <inheritdoc/>
		public static Float32 Mul ( Float32 a, Float32 b ) {
			return a.m_value * b.m_value;
		}
		/// <inheritdoc/>
		public static Float32 Negate ( Float32 value ) {
			return - ((float)value);
		}
		/// <inheritdoc/>
		public static Float32 Signum ( Float32 value ) {
			float f = (float)value;
			if ( float.IsNaN(f) ) return Float32.NaN;
			if ( f > 0f ) return Float32.One;
			if ( f < 0f ) return Float32.NegativeOne;
			return Float32.Zero;
		}
		/// <inheritdoc/>
		public static Float32 Trunc ( Float32 value ) {
			return MathF.Truncate((float)value);
		}
		/// <inheritdoc/>
		public bool Equals ( Float32 other ) {
			return this == other;
		}
		/// <inheritdoc/>
		public FixedVector<byte> ToBytes () {
			byte[] bb =  m_value.ToBytes(Endian.System);
			return new FixedVector<byte>(bb);
		}
		/// <inheritdoc/>
		public byte[] ToBytes ( Endian endian ) {
			return m_value.ToBytes(Endian.System); 
		}

		public void ToBytes ( ref byte[] destination, long offset, Endian endian ) {
			// Encode the underlying value into a temporary byte array.
			byte[] _dest = ToBytes(endian);

			// Ensure the destination buffer is large enough.
			long requiredSize = offset + _dest.LongLength;
			Buffer.LongCapacity(ref destination, requiredSize);

			// Copy the encoded bytes into the destination buffer at the given offset.
			Buffer.LongCopy(_dest, 0, destination, offset, _dest.LongLength);
		}

		public CompareResult CompareTo ( Float32 a ) {
			if ( this > a ) return CompareResult.Less;
			if ( this < a ) return CompareResult.Greater;
			return CompareResult.Equal;
		}
		int IComparable<Float32>.CompareTo ( Float32 obj ) {
			return (int)CompareTo(obj);
		}

		public int CompareTo(object? obj) { 
			if(obj is  Float32 o) 
				return (int)CompareTo(o);
			throw new Exception("Is Not a Float32");
		}
		public override bool Equals ( object? obj ) {
			return CompareTo(obj) == 0; 
		}

		public override int GetHashCode () {
			return m_value.GetHashCode();
		}

		public static bool operator == ( Float32 a, Float32 b ) {
			return a.m_value == b.m_value;
		}

		public static bool operator != ( Float32 a, Float32 b ) {
			return !(a == b);
		}

		public static bool operator < ( Float32 a, Float32 b ) {
			return a.m_value <= b.m_value;
		}

		public static bool operator > ( Float32 a, Float32 b ) {
			return a.m_value > b.m_value;
		}

		public static bool operator <= ( Float32 a, Float32 b ) {
			return a.m_value <= b.m_value;
		}

		public static bool operator >= ( Float32 a, Float32 b ) {
			return a.m_value >= b.m_value;
		}

		

		/// <summary>
		/// Addition operator.
		/// </summary>
		public static Float32 operator + ( Float32 a, Float32 b ) => Add(a, b);

		/// <summary>
		/// Subtraction operator.
		/// </summary>
		public static Float32 operator - ( Float32 a, Float32 b ) => Sub(a, b);

		/// <summary>
		/// Multiplication operator.
		/// </summary>
		public static Float32 operator * ( Float32 a, Float32 b ) => Mul(a, b);

		/// <summary>
		/// Division operator.
		/// </summary>
		public static Float32 operator / ( Float32 a, Float32 b ) => Div(a, b);

		/// <summary>
		/// Increment operator.
		/// </summary>
		public static Float32 operator ++ ( Float32 a ) => a + One;

		/// <summary>
		/// Decrement operator.
		/// </summary>
		public static Float32 operator -- ( Float32 a ) => a - One;

		
	}
}
