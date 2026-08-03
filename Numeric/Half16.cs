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
    /// Represents a 16‑bit IEEE‑754 binary16 floating‑point value with full
    /// bit‑level control over sign, exponent, and mantissa.  
    /// 
    /// Half16 is a deterministic, platform‑independent implementation of the
    /// half‑precision format. Unlike <see cref="System.Half"/>, this type exposes
    /// all internal fields, supports manual construction, and implements full
    /// arithmetic without converting to <see cref="float"/>.
    /// 
    /// <para>
    /// The struct is sequentially laid out and marked with
    /// <see cref="HashAlgorithmAttribute"/> so that hashing can be performed
    /// deterministically using SystemEx hashing algorithms.
    /// </para>
    /// <para>
    /// Half16 also implements <see cref="IHashable{Half16}"/> so that instances
    /// can be converted into a deterministic byte sequence for hashing or
    /// serialization.
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [HashAlgorithm(typeof(BernsteinHash), Endian.System)]
    public struct Half16 : IHalf<Half16> {
        private ushort m_value;

        /// <summary>
        /// Number of bits used for the sign field (always 1).
        /// </summary>
        public ushort SignBits => 1;
        /// <summary>
        /// Number of bits used for the exponent field (IEEE‑754 binary16 uses 5).
        /// </summary>
        public ushort ExponentBits => 5;
        /// <summary>
        /// Number of bits used for the mantissa (fraction) field (10 bits).
        /// </summary>
        public ushort MantissaBits => 10;
        /// <summary>
        /// Exponent bias used by the binary16 format (15).
        /// </summary>
        public ushort ExponentBias => 15;
        /// <summary>
        /// Total number of bits in the representation (16).
        /// </summary>
        public ushort TotalBits => 16;
        /// <summary>
        /// Gets the sign bit (true = negative).
        /// </summary>
        public bool Sign => (bool) ( ((m_value >> ExponentBias ) & 0x1) == 1);
        /// <summary>
        /// Gets the exponent field (5 bits).
        /// </summary>
        public ushort Exponent => (ushort)((m_value >> MantissaBits ) & 0x1F);
        /// <summary>
        /// Gets the mantissa (fraction) field (10 bits).
        /// </summary>
        public ushort Mantissa => (ushort)(m_value & 0x3ff);
        /// <summary>
        /// Represents +0.
        /// </summary>
        public static Half16 Zero => new Half16(0x0000);
        /// <summary>
        /// Represents +1.
        /// </summary>
        public static Half16 One => new Half16(0x3c00);
        /// <summary>
        /// Represents −1.
        /// </summary>
        public static Half16 NegativeOne => new Half16(1, 15, 0);
        /// <summary>
        /// Represents positive infinity.
        /// </summary>
        public static Half16 PositiveInfinity => new Half16(0x7c00);
        /// <summary>
        /// Represents negative infinity.
        /// </summary>
        public static Half16 NegativeInfinity => new Half16(0xFC00);

        /// <summary>
        /// Smallest representable positive increment.
        /// </summary>
        public static Half16 Epsilon => new Half16(0x4170);

        /// <summary>
        /// Quiet NaN value.
        /// </summary>
        public static Half16 NaN => new Half16(0xFE00);

        /// <summary>
        /// Alternative NaN encoding.
        /// </summary>
        public static Half16 NaN2 => new Half16(0x7E00);

        /// <summary>
        /// Smallest representable negative value.
        /// </summary>
        public static Half16 MinValue => new Half16(0xFBFF);

        /// <summary>
        /// Largest representable positive value.
        /// </summary>
        public static Half16 MaxValue => new Half16(0x7BFF);

        /// <summary>
        /// Euler's number (approximation).
        /// </summary>
        public static Half16 E => new Half16(0x4170);

        /// <summary>
        /// π (approximation).
        /// </summary>
        public static Half16 Pi => new Half16(0x4248);

        /// <summary>
        /// τ = 2π (approximation).
        /// </summary>
        public static Half16 Tau => new Half16(0x4648);
        /// <summary>
        /// Returns the larger of two Half16 values.
        /// </summary>
        public static Half16 Max ( Half16 x, Half16 y ) => (x > y) ? x : y;

        /// <summary>
        /// Returns the smaller of two Half16 values.
        /// </summary>
        public static Half16 Min ( Half16 x, Half16 y ) => (x < y) ? x : y;

        /// <summary>
        /// Returns the absolute value of the given Half16.
        /// </summary>
        public static Half16 Abs ( Half16 x ) =>
            new Half16(0, x.Exponent, x.Mantissa);


        /// <summary>
        /// Creates a Half16 from a raw 16‑bit value.
        /// </summary>
        public Half16 ( ushort value ) {
            m_value = value;
        }

        /// <summary>
        /// Creates a Half16 from explicit sign, exponent, and mantissa fields.
        /// </summary>
        public Half16 ( ushort sign, ushort exponent, ushort mantissa ) {
            m_value = (ushort)(((sign & 0x1) << ExponentBias ) | ((exponent & 0x1F) << MantissaBits ) | (mantissa & 0x3FF) );
        }

        /// <summary>
        /// Returns the raw 16‑bit representation.
        /// </summary>
        public ushort ToBase => m_value;

        public static Half16 NegativeZero => throw new NotImplementedException();

        /// <summary>
        /// Converts the value into a byte array using the specified endianness.
        /// </summary>
        public byte[] ToBytes ( Endian endian ) =>
            m_value.ToBytes(endian);

        /// <summary>
        /// Constructs a Half16 from a byte array.
        /// </summary>
        public static Half16 FromBytes ( byte[] input, int offsets, Endian endian ) {
            ushort value = input.ToUShort(offsets, endian);
            return new Half16(value);
        }
        /// <summary>
        /// Returns true if the value is +0 or −0.
        /// </summary>
        public static bool IsZero ( Half16 value ) =>
            (value.m_value & ~0x8000) == 0;

        /// <summary>
        /// Returns true if the value is negative.
        /// </summary>
        public static bool IsNegative ( Half16 val ) =>
            (val.m_value & 0x8000) != 0;

        /// <summary>
        /// Returns true if the value is a NaN.
        /// </summary>
        public static bool IsNaN ( Half16 val ) =>
            (val.Exponent == 0x1F) && (val.Mantissa != 0);

        /// <summary>
        /// Returns true if the value is ±infinity.
        /// </summary>
        public static bool IsInfinity ( Half16 val ) =>
            (val.Exponent == 0x1F) && (val.Mantissa == 0);

        /// <summary>
        /// Returns −1, 0, or +1 depending on the sign of the value.
        /// </summary>
        public static Half16 Signum ( Half16 x ) {
            if ( IsNaN(x) ) return Half16.NaN;
            if ( IsZero(x) ) return Half16.Zero;
            return x.Sign ? Half16.NegativeOne : Half16.One;
        }

        /// <summary>
        /// Clamps the value between the specified minimum and maximum.
        /// </summary>
        public static Half16 Clamp ( Half16 x, Half16 min, Half16 max ) {
            if ( x < min ) return min;
            if ( x > max ) return max;
            return x;
        }
        /// <summary>
        /// Returns true if the value is finite (not NaN or infinity).
        /// </summary>
        public static bool IsFinite ( Half16 x ) =>
            x.Exponent != 0x1F;

        /// <summary>
        /// Returns true if the value is a subnormal number.
        /// </summary>
        public static bool IsSubnormal ( Half16 x ) =>
            x.Exponent == 0 && x.Mantissa != 0;

        /// <summary>
        /// Returns true if the value is a normal (non‑subnormal, non‑special) number.
        /// </summary>
        public static bool IsNormal ( Half16 x ) {
            ushort e = x.Exponent;
            return e != 0 && e != 0x1F;
        }
        /// <summary>
        /// Computes the floor of the value.
        /// </summary>
        public static Half16 Floor ( Half16 x ) {
            if ( IsNaN(x) || IsInfinity(x) ) return x;

            ushort e = x.Exponent;

            if ( e >= 15 ) {
                int shift = e - 15;
                uint mant = (uint)(x.Mantissa | 0x400);
                mant >>= shift;
                mant <<= shift;
                return new Half16((ushort)(x.Sign ? 1 : 0), e, (ushort)(mant & 0x3FF));
            }

            if ( IsZero(x) ) return x;

            return x.Sign ? NegativeOne : Zero;
        }

        /// <summary>
        /// Computes the ceiling of the value.
        /// </summary>
        public static Half16 Ceil ( Half16 x ) {
            if ( IsNaN(x) || IsInfinity(x) ) return x;

            ushort e = x.Exponent;

            if ( e >= 15 ) {
                int shift = e - 15;
                uint mant = (uint)(x.Mantissa | 0x400);
                mant >>= shift;
                mant <<= shift;
                return new Half16((ushort)(x.Sign ? 1 : 0), e, (ushort)(mant & 0x3FF));
            }

            if ( IsZero(x) ) return x;

            return x.Sign ? Zero : One;
        }

        /// <summary>
        /// Truncates the fractional part of the value.
        /// </summary>
        public static Half16 Trunc ( Half16 x ) {
            if ( IsNaN(x) || IsInfinity(x) ) return x;

            ushort e = x.Exponent;

            if ( e >= 15 ) {
                int shift = e - 15;
                uint mant = (uint)(x.Mantissa | 0x400);
                mant >>= shift;
                mant <<= shift;
                return new Half16((ushort)(x.Sign ? 1 : 0), e, (ushort)(mant & 0x3FF));
            }

            return Zero;
        }

        /// <summary>
        /// Returns true if the value is an integer.
        /// </summary>
        public static bool IsInteger ( Half16 x ) {
            if ( IsNaN(x) || IsInfinity(x) ) return false;

            ushort e = x.Exponent;

            if ( e < 15 ) return IsZero(x);

            int shift = e - 15;
            uint mant = (uint)(x.Mantissa | 0x400);

            return (mant & ((1u << shift) - 1)) == 0;
        }


        /// <summary>
        /// Less‑than comparison operator with full IEEE‑754 semantics.
        /// </summary>
        public static bool operator < ( Half16 a, Half16 b ) {
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
        public static bool operator <= ( Half16 a, Half16 b ) {
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
        /// Equality operator with IEEE‑754 zero handling.
        /// </summary>
        public static bool operator == ( Half16 a, Half16 b ) {
            if ( IsNaN(a) || IsNaN(b) ) return false;
            return (a.m_value == b.m_value) || (IsZero(a) && IsZero(b));
        }

        /// <summary>
        /// Greater‑than operator.
        /// </summary>
        public static bool operator > ( Half16 a, Half16 b ) => b < a;

        /// <summary>
        /// Greater‑than‑or‑equal operator.
        /// </summary>
        public static bool operator >= ( Half16 a, Half16 b ) => b <= a;

        /// <summary>
        /// Inequality operator.
        /// </summary>
        public static bool operator != ( Half16 a, Half16 b ) => !(a == b);

        /// <summary>
        /// Addition operator.
        /// </summary>
        public static Half16 operator + ( Half16 a, Half16 b ) => Add(a, b);

        /// <summary>
        /// Subtraction operator.
        /// </summary>
        public static Half16 operator - ( Half16 a, Half16 b ) => a + Negate(b);

        /// <summary>
        /// Multiplication operator.
        /// </summary>
        public static Half16 operator * ( Half16 a, Half16 b ) => Mul(a, b);

        /// <summary>
        /// Division operator.
        /// </summary>
        public static Half16 operator / ( Half16 a, Half16 b ) => Div(a, b);

        /// <summary>
        /// Increment operator.
        /// </summary>
        public static Half16 operator ++ ( Half16 a ) => a + One;

        /// <summary>
        /// Decrement operator.
        /// </summary>
        public static Half16 operator -- ( Half16 a ) => a - One;

        /// <summary>
        /// Normalizes a mantissa/exponent pair into a valid Half16 value.
        /// </summary>
        static Half16 Normalize ( ushort sign, int exp, uint mant ) {
            if ( mant == 0 ) return new Half16(sign, 0, 0);

            while ( (mant & 0x400) == 0 ) {
                mant <<= 1;
                exp--;
            }

            if ( exp <= 0 )
                return new Half16(sign, 0, (ushort)(mant >> (1 - exp)));

            if ( exp >= 31 )
                return new Half16(sign, 0x1F, 0);

            ushort finalMant = (ushort)(mant & 0x3FF);

            return new Half16(sign, (ushort)exp, finalMant);
        }

        /// <summary>
        /// Rounds a mantissa using round‑to‑nearest‑even.
        /// </summary>
        static uint RoundToNearestEven ( uint mant ) {
            uint guard = (mant >> 2) & 1;
            uint round = (mant >> 1) & 1;
            uint sticky = mant & 1;

            uint add = 0;

            if ( round != 0 ) {
                if ( guard != 0 || sticky != 0 )
                    add = 1;
            }

            return (mant >> 3) + add;
        }
        /// <summary>
        /// Returns the negated value.
        /// </summary>
        public static Half16 Negate ( Half16 h ) {
            ushort sign = (ushort)(h.Sign ? 1 : 0);
            return new Half16((ushort)(sign ^ 1), h.Exponent, h.Mantissa);
        }


        
        /// <summary>
        /// Multiplies two Half16 values using full bit‑level arithmetic.
        /// </summary>
        public static Half16 Mul ( Half16 a, Half16 b ) {
            if ( IsNaN(a) || IsNaN(b) ) return Half16.NaN;
            if ( IsZero(a) || IsZero(b) ) return Half16.Zero;

            ushort sign = (ushort)((a.Sign ? 1 : 0) ^ (b.Sign ? 1 : 0) );

            if ( IsInfinity(a) || IsInfinity(b) ) {
                return new Half16(sign, 0x1F, 0);
            }

            ushort expA = a.Exponent;
            ushort expB = b.Exponent;

            uint mantA = (expA == 0) ? a.Mantissa : (uint)(a.Mantissa | 0x400);
            uint mantB = (expB == 0) ? b.Mantissa: (uint)(b.Mantissa | 0x400);

            int exp = expA + expB - 15;

            uint mant = mantA * mantB; // bis zu 20 Bits

            // Normalisieren
            while ( (mant & 0x80000) != 0 ) {
                mant >>= 1;
                exp++;
            }

            mant >>= 10; // zurück auf 11 Bits (leading + 10 mantissa)

            return Normalize(sign, exp, mant);
        }

        /// <summary>
        /// Adds two <see cref="Half16"/> values using full IEEE‑754 style
        /// mantissa/exponent arithmetic, including handling of special values
        /// such as NaN, infinities, and signed zero.
        /// 
        /// The operation is performed entirely in the half‑precision domain
        /// without converting to <see cref="float"/> or <see cref="double"/>.
        /// </summary>
        /// <param name="a">The left operand.</param>
        /// <param name="b">The right operand.</param>
        /// <returns>
        /// The sum <c>a + b</c> as a <see cref="Half16"/> value. If the operation
        /// is undefined (e.g. +∞ + −∞), <see cref="NaN"/> is returned.
        /// </returns>
        public static Half16 Add ( Half16 a, Half16 b ) {
            if ( IsNaN(a) || IsNaN(b) ) return Half16.NaN;
            if ( IsInfinity(a) && IsInfinity(b) && a.Sign != b.Sign ) return Half16.NaN;

            // Sonderfälle
            if ( IsInfinity(a) ) return a;
            if ( IsInfinity(b) ) return b;

            if ( IsZero(a) ) return b;
            if ( IsZero(b) ) return a;

            ushort expA = a.Exponent;
            ushort expB = b.Exponent;

            uint mantA = (expA == 0) ? a.Mantissa : (uint)(a.Mantissa | 0x400);
            uint mantB = (expB == 0) ? b.Mantissa: (uint)(b.Mantissa | 0x400);

            int exp = expA;

            int diff = expA - expB;

            if ( diff > 0 ) {
                mantB >>= diff;
            } else if ( diff < 0 ) {
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

            if ( mant == 0 ) return new Half16(0, 0, 0);

            while ( (mant & 0x400) == 0 ) {
                mant <<= 1;
                exp--;
            }

            return Normalize(sign, exp, mant);
        }

        /// <summary>
        /// Divides one <see cref="Half16"/> value by another using full
        /// mantissa/exponent arithmetic.
        /// 
        /// The operation is performed entirely in half‑precision and handles
        /// special values (NaN, infinities, signed zero) according to IEEE‑754‑like
        /// rules.
        /// </summary>
        /// <param name="a">The dividend.</param>
        /// <param name="b">The divisor.</param>
        /// <returns>
        /// The quotient <c>a / b</c> as a <see cref="Half16"/> value. If the
        /// operation is undefined (e.g. division by zero, ∞ / ∞), 
        /// <see cref="NaN"/> is returned.
        /// </returns>
        public static Half16 Div(Half16 a, Half16 b) {
            if ( IsNaN(a) || IsNaN(b) ) return Half16.NaN;
            if ( IsZero(b) ) return Half16.NaN;
            if ( IsZero(a) ) return Half16.Zero;

            if ( IsInfinity(a) && IsInfinity(b) ) return Half16.NaN;

            ushort sign = (ushort)((a.Sign ? 1 : 0) ^ (b.Sign ? 1 : 0) );

            if ( IsInfinity(a) ) {
                return new Half16(sign, 0x1F, 0);
            }
            if ( IsInfinity(b) ) return Half16.Zero;

            ushort expA = a.Exponent;
            ushort expB = b.Exponent;

            uint mantA = (expA == 0) ? a.Mantissa : (uint)(a.Mantissa | 0x400);
            uint mantB = (expB == 0) ? b.Mantissa: (uint)(b.Mantissa | 0x400);

            int exp = expA - expB + 15;

            uint mant = (mantA << 13) / mantB;

            while ( (mant & 0x400) == 0 ) {
                mant <<= 1;
                exp--;
            }

            return Normalize(sign, exp, mant);
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
        public CompareResult CompareTo ( Half16 b ) {
            CompareResult _ret = CompareResult.Equal;

            if ( IsNaN(this) && !IsNaN(b) ) _ret = CompareResult.AIsSmallerB;

            else {
                if ( this < b ) _ret = CompareResult.AIsSmallerB;
                else if ( this > b ) _ret = CompareResult.AIsLargerB;
            }

            return _ret;
        }

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
            if(x.Value != 0) return (int)x.Value;
            
            return m_value.GetHashCode();
        }
        /// <summary>
        /// Determines whether this instance is equal to another
        /// <see cref="Half16"/> value, using the same semantics as the
        /// <see cref="operator ==(Half16,Half16)"/>.
        /// </summary>
        /// <param name="other">The value to compare with.</param>
        /// <returns>
        /// <c>true</c> if the values are equal; otherwise <c>false</c>.
        /// </returns>
        public bool Equals ( Half16 other ) {
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
            return (obj is Half16) && Equals((Half16)obj);
        }
        /// <summary>
        /// Compares this instance with another object and returns an integer
        /// indicating the relative order, following the standard
        /// <see cref="IComparable"/> contract.
        /// 
        /// <para>
        /// If <paramref name="obj"/> is a <see cref="Half16"/>, the comparison
        /// is delegated to <see cref="CompareTo(Half16)"/> and the resulting
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
            if ( (obj is Half16) ) {
                return (int)CompareTo((Half16)(obj));
            }
            throw new ArgumentException("Object is not a Float16 object");
        }
        /// <summary>
        /// Explicit <see cref="IComparable{T}"/> implementation that forwards
        /// to the extended <see cref="CompareTo(Half16)"/> method and casts
        /// the <see cref="CompareResult"/> to <see cref="int"/>.
        /// 
        /// This keeps the standard .NET comparison API compatible while still
        /// exposing a strongly typed comparison result via
        /// <see cref="IComparableEx{Half16}"/>.
        /// </summary>
        /// <param name="other">The value to compare with.</param>
        /// <returns>
        /// A signed integer indicating the relative order.
        /// </returns>
        int IComparable<Half16>.CompareTo ( Half16 other ) {
            return (int)CompareTo(other);
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

    }
    /// @}
}
