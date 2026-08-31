using System;
using System.Collections.Generic;
using System.Text;
using SystemEx.Collections.Generic;
using SystemEx.Utils;

namespace SystemEx.Numeric {

	/// <summary>
	/// Represents a 64‑bit IEEE‑754 single‑precision floating‑point value.
	/// 
	/// This format uses:
	/// <para>• 1 sign bit</para>
	/// <para>• 11 exponent bits (bias = 1023)</para>
	/// <para>• 52 mantissa bits</para>
	/// 
	/// Normalized numbers include an implicit hidden bit (1 << 52).  
	/// Subnormal numbers use an exponent of zero and omit the hidden bit.
	/// 
	/// <para>
	/// The value is stored as a raw 64‑bit unsigned long, allowing exact
	/// bit‑level manipulation of sign, exponent, and mantissa fields without
	/// relying on the host floating‑point unit.
	/// </para>
	/// 
	/// <para>
	/// All conversions use SystemEx endian‑aware utilities, ensuring consistent
	/// reinterpretation between byte arrays, raw bit patterns, and float values.
	/// </para>
	/// </summary>
	public struct Float64 : IFloat<Float64, ulong> {
		private ulong m_value;

		/// <inheritdoc/>
		public static Float64 Zero => 0.0;
		/// <inheritdoc/>
		public static Float64 One => 1.0;
		/// <inheritdoc/>
		public static Float64 NegativeOne => -1.0;
		/// <inheritdoc/>
		public static Float64 NegativeZero => -0.0;
		/// <inheritdoc/>
		public static Float64 PositiveInfinity => Double.PositiveInfinity;
		/// <inheritdoc/>
		public static Float64 NegativeInfinity => Double.NegativeInfinity;
		/// <inheritdoc/>
		public static Float64 NaN => Double.NaN;
		/// <inheritdoc/>
		public static Float64 NaN2 => Double.NaN;
		/// <inheritdoc/>
		public static Float64 Epsilon => Double.Epsilon;
		/// <inheritdoc/>
		public static Float64 E => Double.E;
		/// <inheritdoc/>
		public static Float64 Tau => Double.Tau;
		/// <inheritdoc/>
		public static Float64 Pi => Double.Tau;
		/// <inheritdoc/>
		public ulong ToBase => m_value;
		/// <inheritdoc/>
		public ulong SignBits => 1;
		/// <inheritdoc/>
		public ulong ExponentBits => 11;
		/// <inheritdoc/>
		public ulong MantissaBits => 52;
		/// <inheritdoc/>
		public ulong ExponentBias => 1023;
		/// <inheritdoc/>
		public ulong TotalBits => 64;
		/// <inheritdoc/>
		public ulong HiddenBit => 0x0010_0000_0000_0000UL;
		/// <inheritdoc/>
		public bool Sign => (m_value >> 63) != 0;
		/// <inheritdoc/>
		public ulong Exponent => (m_value >> 52) & 0x7FFUL;
		/// <inheritdoc/>
		public ulong Mantissa => m_value & 0x000F_FFFF_FFFF_FFFFUL;

		/// <summary>
		/// Smallest representable negative value.
		/// </summary>
		public static Float64 MinValue => Double.MinValue;

		/// <summary>
		/// Largest representable positive value.
		/// </summary>
		public static Float64 MaxValue => Double.MaxValue;


		/// <summary>
		/// Create a Float64 from a double value
		/// </summary>
		public Float64 ( double fval ) {
			m_value = fval.ToInteger();
		}

		/// <summary>
		/// Converts implicit a <see cref="double"/> value to <see cref="Float64"/>.
		/// </summary>
		/// <param name="val"></param>
		public static implicit operator Float64 ( double val ) {
			return new Float64(val);
		}

		/// <summary>
		/// Converts explicit a <see cref="Float64"/> to <see cref="double"/>. without Byteconverter.
		/// </summary>
		/// <param name="val"></param>
		public static explicit operator double ( Float64 val ) {
			byte[] _bb = val.ToBytes(Endian.System);
			return _bb.ToFloat();
		}
		/// <inheritdoc/>
		public static Float64 Abs ( Float64 value ) {
			return System.Math.Abs((double)value);
		}
		/// <inheritdoc/>
		public static Float64 Add ( Float64 a, Float64 b ) {
			return a.m_value + b.m_value;
		}
		/// <inheritdoc/>
		public static Float64 Ceil ( Float64 value ) {
			return System.Math.Ceiling((double)value);
		}
		/// <inheritdoc/>
		public static Float64 Clamp ( Float64 x, Float64 min, Float64 max ) {
			double xf = (double)x;
			double minf = (double)min;
			double maxf = (double)max;

			if ( xf < minf ) return min;
			if ( xf > maxf ) return max;
			return x;
		}
		/// <inheritdoc/> 
		public static Float64 Div ( Float64 a, Float64 b ) {
			return a.m_value / b.m_value;
		}
		/// <inheritdoc/>
		public static Float64 Sub ( Float64 a, Float64 b ) {
			return a.m_value - b.m_value;
		}
		/// <inheritdoc/>
		public static Float64 Floor ( Float64 value ) {
			return System.Math.Floor((double)value);
		}
		/// <inheritdoc/>
		public static Float64 FromBytes ( byte[] bytes, long offset, Endian endian ) {
			return bytes.ToDouble(offset, endian);
		}
		/// <inheritdoc/>
		public static bool IsFinite ( Float64 value ) {
			ulong exp = (value.m_value >> 23) & 0xFF;
			return exp != 0xFF;
		}
		/// <inheritdoc/>
		public static bool IsInfinity ( Float64 value ) {
			return Double.IsInfinity((double)value);
		}
		/// <inheritdoc/>
		public static bool IsInteger ( Float64 value ) {
			return Double.IsInteger((double)value);
		}
		/// <inheritdoc/>
		public static bool IsNaN ( Float64 value ) {
			return Double.IsNaN((double)value);
		}
		/// <inheritdoc/>
		public static bool IsNegative ( Float64 value ) {
			return Double.IsNegative((double)value);
		}
		/// <inheritdoc/>
		public static bool IsNormal ( Float64 value ) {
			return Double.IsNormal((double)value);
		}
		/// <inheritdoc/>
		public static bool IsSubnormal ( Float64 value ) {
			return Double.IsSubnormal((double)value);
		}
		/// <inheritdoc/>
		public static bool IsZero ( Float64 value ) {
			return (value.m_value & 0x7FFFFFFF) == 0;
		}
		/// <inheritdoc/>
		public static Float64 Max ( Float64 a, Float64 b ) {
			return a < b ? b : a;
		}
		/// <inheritdoc/>
		public static Float64 Min ( Float64 a, Float64 b ) {
			return (a < b) ? a : b;
		}
		/// <inheritdoc/>
		public static Float64 Mul ( Float64 a, Float64 b ) {
			return a.m_value * b.m_value;
		}
		/// <inheritdoc/>
		public static Float64 Negate ( Float64 value ) {
			return -((double)value);
		}
		/// <inheritdoc/>
		public static Float64 Signum ( Float64 value ) {
			double f = (double)value;
			if ( double.IsNaN(f) ) return Float64.NaN;
			if ( f > 0f ) return Float64.One;
			if ( f < 0f ) return Float64.NegativeOne;
			return Float64.Zero;
		}
		/// <inheritdoc/>
		public static Float64 Trunc ( Float64 value ) {
			return System.Math.Truncate((double)value);
		}
		/// <inheritdoc/>
		public bool Equals ( Float64 other ) {
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

		public CompareResult CompareTo ( Float64 a ) {
			if ( this > a ) return CompareResult.Less;
			if ( this < a ) return CompareResult.Greater;
			return CompareResult.Equal;
		}
		int IComparable<Float64>.CompareTo ( Float64 obj ) {
			return (int)CompareTo(obj);
		}

		public int CompareTo ( object? obj ) {
			if ( obj is Float64 o )
				return (int)CompareTo(o);
			throw new Exception("Is Not a Float64");
		}
		public override bool Equals ( object? obj ) {
			return CompareTo(obj) == 0;
		}

		public override int GetHashCode () {
			return m_value.GetHashCode();
		}

		public static bool operator == ( Float64 a, Float64 b ) {
			return a.m_value == b.m_value;
		}

		public static bool operator != ( Float64 a, Float64 b ) {
			return !(a == b);
		}

		public static bool operator < ( Float64 a, Float64 b ) {
			return a.m_value <= b.m_value;
		}

		public static bool operator > ( Float64 a, Float64 b ) {
			return a.m_value > b.m_value;
		}

		public static bool operator <= ( Float64 a, Float64 b ) {
			return a.m_value <= b.m_value;
		}

		public static bool operator >= ( Float64 a, Float64 b ) {
			return a.m_value >= b.m_value;
		}



		/// <summary>
		/// Addition operator.
		/// </summary>
		public static Float64 operator + ( Float64 a, Float64 b ) => Add(a, b);

		/// <summary>
		/// Subtraction operator.
		/// </summary>
		public static Float64 operator - ( Float64 a, Float64 b ) => Sub(a, b);

		/// <summary>
		/// Multiplication operator.
		/// </summary>
		public static Float64 operator * ( Float64 a, Float64 b ) => Mul(a, b);

		/// <summary>
		/// Division operator.
		/// </summary>
		public static Float64 operator / ( Float64 a, Float64 b ) => Div(a, b);

		/// <summary>
		/// Increment operator.
		/// </summary>
		public static Float64 operator ++ ( Float64 a ) => a + One;

		/// <summary>
		/// Decrement operator.
		/// </summary>
		public static Float64 operator -- ( Float64 a ) => a - One;

	}
}
