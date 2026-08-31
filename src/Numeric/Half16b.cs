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
	/// \addtogroup Numeric
	/// @{

	/// <summary>
	/// This value type represents A Half16b value.
	/// See https://cloud.google.com/blog/products/ai-machine-learning/bfloat16-the-secret-to-high-performance-on-cloud-tpus
	/// for details,
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
    [HashAlgorithm(typeof(BernsteinHash), Endian.System)]
    public struct Half16b : IHalf<Half16b> {

        private ushort m_value;

        /// <summary>
        /// Number of bits used for the sign field (always 1).
        /// </summary>
        public ushort SignBits => 1;
        /// <summary>
        /// Number of bits used for the exponent field (bfloat16 uses 8).
        /// </summary>
        public ushort ExponentBits => 8;
        /// <summary>
        /// Number of bits used for the mantissa (fraction) field (7 bits).
        /// </summary>
        public ushort MantissaBits => 7;

		/// <inheritdoc/>
		public ushort ExponentBias => 127;
		/// <inheritdoc/>
		public ushort TotalBits => 16;
		/// <inheritdoc/>
		public ushort HiddenBit => 0x80;

		/// <inheritdoc/>
		public bool Sign => (bool)(((m_value >> 15) & 0x1) == 1);
		/// <inheritdoc/>
		public ushort Exponent => (ushort)((m_value >> MantissaBits) & 0xFF);
		/// <inheritdoc/>
		public ushort Mantissa => (ushort)(m_value & 0x7F);
		/// <inheritdoc/>
		public static Half16b Zero => new Half16b(0x0000);
		/// <inheritdoc/>
		public static Half16b One => new Half16b(0x3F80);
		/// <inheritdoc/>
		public static Half16b NegativeOne => new Half16b(0xBF80);
		/// <inheritdoc/>
		public static Half16b NegativeZero => new Half16b(0x8000);
		/// <inheritdoc/>
		public static Half16b PositiveInfinity => new Half16b(0x7F80);
		/// <inheritdoc/>
		public static Half16b NegativeInfinity => new Half16b(0xFF80);
		/// <inheritdoc/>
		public static Half16b NaN => new Half16b(0xFFC1);

		/// <inheritdoc/>
		public static Half16b Epsilon => new Half16b(0x0080);

        /// <summary>
        /// Alternative NaN encoding.
        /// </summary>
        public static Half16b NaN2 => new Half16b(0x7FC1);

        /// <summary>
        /// Smallest representable negative value.
        /// </summary>
        public static Half16b MinValue => new Half16b(0xFF7F);

        /// <summary>
        /// Largest representable positive value.
        /// </summary>
        public static Half16b MaxValue => new Half16b(0x7F7F);

        /// <summary>
        /// Euler's number (approximation).
        /// </summary>
        public static Half16b E => new Half16b(0x402D);

        /// <summary>
        /// π (approximation).
        /// </summary>
        public static Half16b Pi => new Half16b(0x4049);

        /// <summary>
        /// τ = 2π (approximation).
        /// </summary>
        public static Half16b Tau => new Half16b(0x40C9);
        /// <summary>
        /// Returns the larger of two Half16 values.
        /// </summary>
        public static Half16b Max ( Half16b x, Half16b y ) => (x > y) ? x : y;

        /// <summary>
        /// Returns the smaller of two Half16 values.
        /// </summary>
        public static Half16b Min ( Half16b x, Half16b y ) => (x < y) ? x : y;

        public Half16b (  ) {
            m_value = 0x0000;
        }

        public Half16b(ushort value) {
            m_value = value;
        }
        public Half16b(Half16b other) {
            m_value = other.m_value;
        }

        /// <summary>
        /// Creates a Half16b from explicit sign, exponent, and mantissa fields.
        /// </summary>
        public Half16b ( ushort sign, ushort exponent, ushort mantissa ) {
            m_value = (ushort)(((sign & 0x1) << 15) | ((exponent & 0xFF) << MantissaBits ) | (mantissa & 0x7F));
        }

        /// <summary>
        /// Returns the raw 16‑bit representation.
        /// </summary>
        public ushort ToBase => m_value;

        /// <summary>
        /// Converts the value into a byte array using the specified endianness.
        /// </summary>
        public byte[] ToBytes ( Endian endian ) =>
            m_value.ToBytes(endian);

        public void ToBytes ( ref byte[] destination, long offset, Endian endian ) {
            // Encode the underlying value into a temporary byte array.
            byte[] _dest = m_value.ToBytes(endian);

            // Ensure the destination buffer is large enough.
            long requiredSize = offset + _dest.LongLength;
            Buffer.LongCapacity(ref destination, requiredSize);

            // Copy the encoded bytes into the destination buffer at the given offset.
            Buffer.LongCopy(_dest, 0, destination, offset, _dest.LongLength);
        }


        /// <summary>
        /// Constructs a Half16 from a byte array.
        /// </summary>
        public static Half16b FromBytes ( byte[] input, long offsets, Endian endian ) {
            ushort value = input.ToUShort(offsets, endian);
            return new Half16b(value);
        }
        public static bool IsFinite ( Half16b x ) =>
            x.Exponent != 0xFF;

		/// <inheritdoc/>
		public static bool IsInfinity ( Half16b val ) =>
                (val.Exponent == 0xFF) && (val.Mantissa == 0);

		/// <inheritdoc/>
		public static bool IsInteger ( Half16b x ) {
            if ( IsNaN(x) || IsInfinity(x) )
                return false;

            ushort e = x.Exponent;

            if ( e < 127 )
                return IsZero(x);

            int shift = e - 127;
            uint mant = (uint)(x.Mantissa | x.HiddenBit );

            return (mant & ((1u << shift) - 1)) == 0;
        }


        /// <summary>
        /// Returns true if the value is a NaN.
        /// </summary>
        public static bool IsNaN ( Half16b val ) =>
            (val.Exponent == 0xFF) && (val.Mantissa != 0);


		/// <inheritdoc/>
		public static bool IsNegative ( Half16b val ) =>
            (val.m_value & 0x8000) != 0;

		/// <inheritdoc/>
		public static bool IsNormal ( Half16b x ) {
            ushort e = x.Exponent;
            return e != 0 && e != 0xFF;
        }

		/// <inheritdoc/>
		public static bool IsSubnormal ( Half16b x ) =>
            x.Exponent == 0 && x.Mantissa != 0;

		/// <inheritdoc/>
		public static bool IsZero ( Half16b value ) =>
            (value.m_value & ~0x8000) == 0;

		/// <inheritdoc/>
		public static Half16b Abs ( Half16b x ) =>
            new Half16b(0, x.Exponent, x.Mantissa);


		/// <inheritdoc/>
		public static Half16b Add ( Half16b a, Half16b b ) {
            if ( IsNaN(a) || IsNaN(b) ) return Half16b.NaN;
            if ( IsInfinity(a) && IsInfinity(b) && a.Sign != b.Sign ) return Half16b.NaN;

            if ( IsInfinity(a) ) return a;
            if ( IsInfinity(b) ) return b;

            if ( IsZero(a) ) return b;
            if ( IsZero(b) ) return a;

            ushort expA = a.Exponent;
            ushort expB = b.Exponent;

            uint mantA = (expA == 0) ? a.Mantissa : (uint)(a.Mantissa | a.HiddenBit);
            uint mantB = (expB == 0) ? b.Mantissa : (uint)(b.Mantissa | b.HiddenBit);

            int exp = expA;
            int diff = expA - expB;

            if ( diff > 0 )
                mantB >>= diff;
            else if ( diff < 0 ) {
                mantA >>= -diff;
                exp = expB;
            }

            uint mant;
            ushort sign;

            if ( a.Sign == b.Sign ) {
                mant = mantA + mantB;
                sign = (ushort)(a.Sign ? 1 : 0);
            } else {
                if ( mantA >= mantB ) {
                    mant = mantA - mantB;
                    sign = (ushort)(a.Sign ? 1 : 0);
                } else {
                    mant = mantB - mantA;
                    sign = (ushort)(b.Sign ? 1 : 0);
                }
            }

            if ( mant == 0 )
                return new Half16b(0, 0, 0);

            while ( (mant & a.HiddenBit) == 0 ) {
                mant <<= 1;
                exp--;
            }

            return Normalize(sign, exp, mant, a.HiddenBit);
        }

		/// <inheritdoc/>
		public static Half16b Ceil ( Half16b x ) {
            if ( IsNaN(x) || IsInfinity(x) )
                return x;

            ushort e = x.Exponent;

            if ( e >= 127 ) {
                int shift = e - 127;
                uint mant = (uint)(x.Mantissa | x.HiddenBit);
                mant >>= shift;
                mant <<= shift;
                return new Half16b((ushort)(x.Sign ? 1 : 0), e, (ushort)(mant & 0x7F));
            }

            if ( IsZero(x) )
                return x;

            return x.Sign ? Half16b.Zero : Half16b.One;
        }

		/// <inheritdoc/>
		public static Half16b Div ( Half16b a, Half16b b ) {
            if ( IsNaN(a) || IsNaN(b) ) return Half16b.NaN;
            if ( IsZero(b) ) return Half16b.NaN;
            if ( IsZero(a) ) return Half16b.Zero;

            if ( IsInfinity(a) && IsInfinity(b) ) return Half16b.NaN;

            ushort sign = (ushort)((a.Sign ? 1 : 0) ^ (b.Sign ? 1 : 0));

            if ( IsInfinity(a) )
                return new Half16b(sign, 0xFF, 0);

            if ( IsInfinity(b) )
                return Half16b.Zero;

            ushort expA = a.Exponent;
            ushort expB = b.Exponent;

            uint mantA = (expA == 0) ? a.Mantissa : (uint)(a.Mantissa | a.HiddenBit);
            uint mantB = (expB == 0) ? b.Mantissa : (uint)(b.Mantissa | b.HiddenBit);

            int exp = expA - expB + 127;

            uint mant = (mantA << 13) / mantB;

            while ( (mant & 0x80) == 0 ) {
                mant <<= 1;
                exp--;
            }

            return Normalize(sign, exp, mant, a.HiddenBit);
        }

		/// <inheritdoc/>
		public static Half16b Floor ( Half16b x ) {
            if ( IsNaN(x) || IsInfinity(x) )
                return x;

            ushort e = x.Exponent;

            if ( e >= 127 ) {
                int shift = e - 127;
                uint mant = (uint)(x.Mantissa | x.HiddenBit);
                mant >>= shift;
                mant <<= shift;
                return new Half16b((ushort)(x.Sign ? 1 : 0), e, (ushort)(mant & 0x7F));
            }

            if ( IsZero(x) )
                return x;

            return x.Sign ? Half16b.NegativeOne : Half16b.Zero;
        }


		/// <inheritdoc/>
		public static Half16b Mul ( Half16b a, Half16b b ) {
            if ( IsNaN(a) || IsNaN(b) ) return Half16b.NaN;
            if ( IsZero(a) || IsZero(b) ) return Half16b.Zero;

            ushort sign = (ushort)((a.Sign ? 1 : 0) ^ (b.Sign ? 1 : 0));

            if ( IsInfinity(a) || IsInfinity(b) )
                return new Half16b(sign, 0xFF, 0);

            ushort expA = a.Exponent;
            ushort expB = b.Exponent;

            uint mantA = (expA == 0) ? a.Mantissa : (uint)(a.Mantissa | a.HiddenBit);
            uint mantB = (expB == 0) ? b.Mantissa : (uint)(b.Mantissa | b.HiddenBit);

            int exp = expA + expB - 127;

            uint mant = mantA * mantB;

            while ( (mant & 0x4000) != 0 ) {
                mant >>= 1;
                exp++;
            }

            mant >>= 7;

            return Normalize(sign, exp, mant, a.HiddenBit);
        }

		/// <inheritdoc/>
		public static Half16b Negate ( Half16b h ) {
            ushort sign = (ushort)(h.Sign ? 1 : 0);
            return new Half16b((ushort)(sign ^ 1), h.Exponent, h.Mantissa);
        }
		/// <inheritdoc/>
		public static Half16b Signum ( Half16b x ) {
            if ( IsNaN(x) ) return Half16b.NaN;
            if ( IsZero(x) ) return Half16b.Zero;
            return x.Sign ? Half16b.NegativeOne : Half16b.One;
        }
		/// <inheritdoc/>
		public static Half16b Trunc ( Half16b x ) {
            if ( IsNaN(x) || IsInfinity(x) )
                return x;

            ushort e = x.Exponent;

            if ( e >= 127 ) {
                int shift = e - 127;
                uint mant = (uint)(x.Mantissa | x.HiddenBit);
                mant >>= shift;
                mant <<= shift;
                return new Half16b((ushort)(x.Sign ? 1 : 0), e, (ushort)(mant & 0x7F));
            }

            return Half16b.Zero;
        }

		/// <inheritdoc/>
		public static Half16b Clamp ( Half16b x, Half16b min, Half16b max ) {
            if ( x < min ) return min;
            if ( x > max ) return max;
            return x;
        }

        /// <summary>
        /// Converts this instance into a deterministic byte array suitable for
        /// hashing, serialization, or low‑level processing.
        /// 
        /// The returned array contains the raw 16‑bit representation of the
        /// value in the system's native endianness, wrapped in a SystemEx
        /// <see cref="FixedVector{T}"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="FixedVector{T}"/> containing the raw bytes of this
        /// <see cref="Half16"/> value.
        /// </returns>
        public FixedVector<byte> ToBytes () {
            return new FixedVector<byte>(m_value.ToBytes());
        }

        /// <summary>
        /// Compares this instance with another <see cref="Half16"/> and returns
        /// a <see cref="CompareResult"/> describing the relationship.
        /// 
        /// <para>
        /// NaN is treated as unordered: if this instance is NaN and the other
        /// is not, the result is <see cref="CompareResult.AIsSmallerB"/> to
        /// provide a deterministic ordering for sorting and data structures.
        /// </para>
        /// </summary>
        /// <param name="b">The value to compare with this instance.</param>
        /// <returns>
        /// A <see cref="CompareResult"/> indicating whether this instance is
        /// smaller than, equal to, or larger than <paramref name="b"/>.
        /// </returns>
        public CompareResult CompareTo ( Half16b b ) {
            CompareResult _ret = CompareResult.Equal;

            if ( IsNaN(this) && !IsNaN(b) ) _ret = CompareResult.AIsSmallerB;

            else {
                if ( this < b ) _ret = CompareResult.AIsSmallerB;
                else if ( this > b ) _ret = CompareResult.AIsLargerB;
            }

            return _ret;
        }

        /// <summary>
        /// Less‑than comparison operator with full IEEE‑754 semantics.
        /// </summary>
        public static bool operator < ( Half16b a, Half16b b ) {
            if ( IsNaN(a) || IsNaN(b) ) return false;

            bool _neg = IsNegative(a);

            if ( _neg != IsNegative(b) ) {
                if ( IsZero(a) && IsZero(b) )
                    return false;
                return _neg;
            }

            return (a.m_value != b.m_value) && ((a.m_value < b.m_value) ^ _neg);
        }

        /// <summary>
        /// Less‑than‑or‑equal comparison operator.
        /// </summary>
        public static bool operator <= ( Half16b a, Half16b b ) {
            if ( IsNaN(a) || IsNaN(b) ) return false;

            bool _neg = IsNegative(a);

            if ( _neg != IsNegative(b) ) {
                if ( IsZero(a) && IsZero(b) )
                    return true;
                return _neg;
            }

            return (a.m_value == b.m_value) || ((a.m_value < b.m_value) ^ _neg);
        }

        /// <summary>
        /// Equality operator with  zero handling.
        /// </summary>
        public static bool operator == ( Half16b a, Half16b b ) {
            if ( IsNaN(a) || IsNaN(b) ) return false;
            return (a.m_value == b.m_value) || (IsZero(a) && IsZero(b));
        }


        /// <summary>
        /// Greater‑than operator.
        /// </summary>
        public static bool operator > (  Half16b  a,  Half16b  b ) => !(b <= a);

        /// <summary>
        /// Greater‑than‑or‑equal operator.
        /// </summary>
        public static bool operator >= (  Half16b  a,  Half16b  b ) => !(b < a);

        /// <summary>
        /// Inequality operator.
        /// </summary>
        public static bool operator != (  Half16b  a,  Half16b  b ) => !(a == b);

        /// <summary>
        /// Addition operator.
        /// </summary>
        public static  Half16b  operator + (  Half16b  a,  Half16b  b ) => Add(a, b);

        /// <summary>
        /// Subtraction operator.
        /// </summary>
        public static  Half16b  operator - (  Half16b  a,  Half16b  b ) => a + Negate(b);

        /// <summary>
        /// Multiplication operator.
        /// </summary>
        public static  Half16b  operator * (  Half16b  a,  Half16b  b ) => Mul(a, b);

        /// <summary>
        /// Division operator.
        /// </summary>
        public static  Half16b  operator / (  Half16b  a,  Half16b  b ) => Div(a, b);

        /// <summary>
        /// Increment operator.
        /// </summary>
        public static  Half16b  operator ++ (  Half16b  a ) => a + One;

        /// <summary>
        /// Decrement operator.
        /// </summary>
        public static  Half16b  operator -- (  Half16b  a ) => a - One;


        /// <summary>
        /// Returns a hash code for this instance.
        /// 
        /// The hash is computed using <see cref="HashFactory.Hash32{T}(T,uint)"/>
        /// with a fixed seed. If the computed hash is non‑zero, it is used;
        /// otherwise the raw <see cref="ushort"/> value is hashed.
        /// 
        /// This ensures a deterministic, stable hash suitable for use in
        /// dictionaries, sets, and other hash‑based containers.
        /// </summary>
        public override int GetHashCode () {
            var x =  HashFactory.Hash32(this, 674545);
            if ( x.Value != 0 ) return (int)x.Value;

            return m_value.GetHashCode();
        }
        /// <summary>
        /// Determines whether this instance is equal to another
        /// <see cref="Half16"/> value, using the same semantics as the
        /// <see cref="operator ==(Half16b,Half16b)"/>.
        /// </summary>
        /// <param name="other">The value to compare with.</param>
        /// <returns>
        /// <c>true</c> if the values are equal; otherwise <c>false</c>.
        /// </returns>
        public bool Equals ( Half16b other ) {
            return this == other;
        }

        /// <summary>
        /// Determines whether this instance is equal to another object.
        /// 
        /// The object is considered equal if it is a <see cref="Half16"/>
        /// and compares equal using <see cref="Equals(Half16)"/>.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is a <see cref="Half16"/>
        /// and equal to this instance; otherwise <c>false</c>.
        /// </returns>
        public override bool Equals ( object? obj ) {
            if ( obj == null ) return false;
            return (obj is Half16b) && Equals((Half16b)obj);
        }
        /// <summary>
        /// Compares this instance with another object and returns an integer
        /// indicating the relative order, following the standard
        /// <see cref="IComparable"/> contract.
        /// 
        /// <para>
        /// If <paramref name="obj"/> is a <see cref="Half16b"/>, the comparison
        /// is delegated to <see cref="CompareTo(Half16b)"/> and the resulting
        /// <see cref="CompareResult"/> is cast to <see cref="int"/>.
        /// </para>
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns>
        /// A signed integer indicating the relative order:
        /// negative if this instance is smaller, zero if equal,
        /// positive if larger.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="obj"/> is not a <see cref="Half16"/>.
        /// </exception>
        public int CompareTo ( object? obj ) {
            if ( (obj is Half16b) ) {
                return (int)CompareTo((Half16b)(obj));
            }
            throw new ArgumentException("Object is not a Float16 object");
        }
        /// <summary>
        /// Explicit <see cref="IComparable{T}"/> implementation that forwards
        /// to the extended <see cref="CompareTo(Half16b)"/> method and casts
        /// the <see cref="CompareResult"/> to <see cref="int"/>.
        /// 
        /// This keeps the standard .NET comparison API compatible while still
        /// exposing a strongly typed comparison result via
        /// <see cref="IComparableEx{Half16b}"/>.
        /// </summary>
        /// <param name="other">The value to compare with.</param>
        /// <returns>
        /// A signed integer indicating the relative order.
        /// </returns>
        int IComparable<Half16b>.CompareTo ( Half16b other ) {
            return (int)CompareTo(other);
        }


		static Half16b Normalize ( ushort sign, int exp, uint mant, ushort hiddenbit ) {
            if ( mant == 0 )
                return new Half16b(sign, 0, 0);

            while ( (mant & hiddenbit) == 0 ) {
                mant <<= 1;
                exp--;
            }

            if ( exp <= 0 )
                return new Half16b(sign, 0, (ushort)(mant >> (1 - exp)));

            if ( exp >= 0xFF )
                return new Half16b(sign, 0xFF, 0);

            ushort finalMant = (ushort)(mant & 0x7F);

            return new Half16b(sign, (ushort)exp, finalMant);
        }
        static uint RoundToNearestEven ( uint mant ) {
            uint guard  = (mant >> 2) & 1;
            uint round  = (mant >> 1) & 1;
            uint sticky = mant & 1;

            uint add = 0;

            if ( round != 0 ) {
                if ( guard != 0 || sticky != 0 )
                    add = 1;
            }

            return (mant >> 3) + add;
        }
    }
	
}
