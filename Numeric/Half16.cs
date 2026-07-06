using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;

namespace SystemEx.Numeric {
    /// <summary>
    /// 
    /// </summary>
    public struct Half16 : IHalf<Half16>, IEquatable<Half16> {
        private ushort m_value;

        public ushort SignBits => 1;
        public ushort ExponentBits => 5;
        public ushort MantissaBits => 10;
        public ushort ExponentBias => 15;
        public ushort TotalBits => 16;
        /// <inheritdoc />
        public bool Sign => (bool) ( ((m_value >> 15) & 0x1) == 1);
        /// <inheritdoc />
        public ushort Exponent => (ushort)((m_value >> 10) & 0x1F);
        /// <inheritdoc />
        public ushort Mantissa => (ushort)(m_value & 0x3ff);
        /// <inheritdoc />
        public static Half16 Zero => new Half16(0x0000);
        /// <inheritdoc />
        public static Half16 One => new Half16(0x3c00);

        public static Half16 NegativeOne => new Half16(1, 15, 0);
        /// <inheritdoc />
        public static Half16 PositiveInfinity => new Half16(0x7c00);
        /// <inheritdoc />
        public static Half16 NegativeInfinity => new Half16(0xfc00);
        /// <inheritdoc />
        public static Half16 Epsilon => new Half16(0x4170);
        /// <inheritdoc />
        public static Half16 NaN => new Half16(0xfe00);
        /// <inheritdoc />
        public static Half16 NaN2 => new Half16(0x7e00);
        /// <inheritdoc />
        public static Half16 MinValue => new Half16(0xfbff);
        /// <inheritdoc />
        public static Half16 MaxValue => new Half16(0x7bff);

        /// <inheritdoc />
        public static Half16 E => new Half16(0x4170);

        /// <inheritdoc />
        public static Half16 Pi => new Half16(0x4248);

        /// <inheritdoc />
        public static Half16 Tau => new Half16(0x4648);

        public static Half16 Max ( Half16 x, Half16 y ) => (x > y) ? x : y;
        public static Half16 Min ( Half16 x, Half16 y ) => (x < y) ? x : y;

        public bool IsNaN => (Exponent == 0x1F) && (Mantissa != 0);
        //public bool IsInfinity   => (Exponent == 0x1F) && (Mantissa == 0);

        public static Half16 Abs ( Half16 x ) => new Half16(0, x.Exponent, x.Mantissa);


        public Half16 ( ushort value) {
            m_value = value;
        }
        public Half16( ushort sign, ushort exponent, ushort mantissa ) {
            m_value = (ushort)((sign << 15) | (exponent << 10) |  mantissa);
        }

        public ushort AsUShort() { 
            return m_value;  
        }

        public byte[] ToBytes(Endian endian) { 
            return m_value.ToBytes(endian); 
        }

        public static Half16 FromBytes ( byte[] input, int offsets, Endian endian) {
            ushort value = input.ToUShort(offsets, endian);
            return new Half16(value);
        }

        public static bool IsZero ( Half16 value ) {
            return (value.m_value & ~ 0x8000) == 0;
        }
        public static bool IsNegative (Half16 val) {
            return (val.m_value & 0x8000) != 0; 
        }
        public static bool IsNan(Half16 val) {
            return (val.Exponent == 0x1F) && (val.Mantissa != 0);
        }
        public static bool IsInfinity(Half16 val) {
            return (val.Exponent == 0x1F) && (val.Mantissa == 0);
        }

        public static Half16 Signum ( Half16 x ) {
            if ( x.IsNaN ) return Half16.NaN;
            if ( IsZero(x) ) return Half16.Zero;

            return x.Sign ? Half16.NegativeOne : Half16.One;
        }
        public static Half16 Clamp ( Half16 x, Half16 min, Half16 max ) {
            if ( x < min ) return min;
            if ( x > max ) return max;
            return x;
        }
        public static bool IsFinite ( Half16 x ) {
            ushort exp = x.Exponent;
            return exp != 0x1F;
        }

        public static bool IsSubnormal ( Half16 x ) {
            return x.Exponent == 0 && x.Mantissa != 0;
        }
        public static bool IsNormal ( Half16 x ) {
            ushort e = x.Exponent;
            return e != 0 && e != 0x1F;
        }

        public static Half16 Floor ( Half16 x ) {
            if ( x.IsNaN || IsInfinity(x) ) return x;

            ushort e = x.Exponent;

            if ( e >= 15 ) {
                int shift = e - 15;
                uint mant = (uint)(x.Mantissa | 0x400);
                mant >>= shift;
                mant <<= shift;
                return new Half16( (ushort)(x.Sign ? 1 : 0), e, (ushort)(mant & 0x3FF));
            }

            if ( IsZero(x) ) return x;

            return x.Sign ? NegativeOne : Zero;
        }

        public static Half16 Ceil ( Half16 x ) {
            if ( x.IsNaN || IsInfinity(x) ) return x;

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

        public static Half16 Trunc ( Half16 x ) {
            if ( x.IsNaN || IsInfinity(x) ) return x;

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

        public static bool IsInteger ( Half16 x ) {
            if ( x.IsNaN || IsInfinity(x) ) return false;

            ushort e = x.Exponent;

            if ( e < 15 ) return IsZero(x);

            int shift = e - 15;
            uint mant = (uint)(x.Mantissa | 0x400);

            return (mant & ((1u << shift) - 1)) == 0;
        }


        public static bool operator < ( Half16 a, Half16 b ) {
            if ( a.IsNaN|| b.IsNaN)   return false;
            bool _ret = false;

            bool _neg = IsNegative(a);

            if ( _neg != IsNegative(b) ) {
                // +0 and -0 are equal → not less
                if ( IsZero(a) && IsZero(b) ) 
                    _ret = false;
                else  
                    _ret = _neg; 
            } else {
                _ret = (a.m_value != b.m_value) && ((a.m_value < b.m_value) ^ _neg); 
            }
            return _ret;
        }
        public static bool operator <= ( Half16 a, Half16 b ) {
            if ( a.IsNaN || b.IsNaN ) return false;
            bool _ret = false;

            bool _neg = IsNegative(a);

            if ( _neg != IsNegative(b) ) {
                if ( IsZero(a) && IsZero(b) )
                    _ret = true;
                else
                    _ret = _neg; 
            } else {
                _ret = (a.m_value == b.m_value) || ((a.m_value < b.m_value) ^ _neg);
            }
            return _ret;
        }
        static Half16 Normalize ( ushort sign, int exp, uint mant ) {
            if ( mant == 0 ) return new Half16(sign, 0, 0);

            // Mantisse normalisieren: Leading-Bit suchen
            while ( (mant & 0x400) == 0 ) {
                mant <<= 1;
                exp--;
            }

            // Exponentbereich clampen
            if ( exp <= 0 )
                return new Half16(sign, 0, (ushort)(mant >> (1 - exp)));

            if ( exp >= 31 )
                return new Half16(sign, 0x1F, 0);

            // Mantisse auf 10 Bits reduzieren
            ushort finalMant = (ushort)(mant & 0x3FF);

            return new Half16(sign, (ushort)exp, finalMant);
        }
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
        static Half16 Negate ( Half16 h ) {
            ushort sign = (ushort)( h.Sign ? 1 : 0 );
            return new Half16((ushort)(sign ^ 1), h.Exponent, h.Mantissa);
        }



        public static bool operator == ( Half16 a, Half16 b ) {
            if ( a.IsNaN || b.IsNaN ) return false;

            return (a.m_value == b.m_value) || IsZero(a) && IsZero(b);
        }
        public static bool operator > ( Half16 a, Half16 b ) {
            return b < a;
        }
        public static bool operator >= ( Half16 a, Half16 b ) {
            return b <= a;
        }
        public static bool operator != ( Half16 a, Half16 b ) {
            return !(a == b);
        }

        public static Half16 operator + ( Half16 a, Half16 b ) {
            return Add(a, b);
        }
        public static Half16 operator - ( Half16 a, Half16 b ) {
            return a + Negate(b);
        }
        public static Half16 operator * ( Half16 a, Half16 b ) {
            return Mul(a, b);
        }
        public static Half16 operator / ( Half16 a, Half16 b ) {
            return Div(a, b);
        }

        public static Half16 operator ++ ( Half16 a ) {
            return a + One;
        }

        public static Half16 operator -- ( Half16 a ) {
            return a - One;
        }


        


        public static Half16 Mul ( Half16 a, Half16 b ) {
            if ( a.IsNaN || b.IsNaN ) return Half16.NaN;
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
        public static Half16 Add ( Half16 a, Half16 b ) {
            if ( a.IsNaN || b.IsNaN ) return Half16.NaN;
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
        public static Half16 Div(Half16 a, Half16 b) {
            if ( a.IsNaN || b.IsNaN ) return Half16.NaN;
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
       
        /// <inheritdoc/>
        public override int GetHashCode () {
            return m_value.GetHashCode();
        }
        /// <inheritdoc/>
        public bool Equals ( Half16 other ) {
            return this == other;
        }

        public override bool Equals ( object obj ) {
            return Equals((Half16)obj);
        }
    }
}
