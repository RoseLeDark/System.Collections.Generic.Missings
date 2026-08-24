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


namespace SystemEx.Numeric.Utils {
	/// \addtogroup SystemEx.Numeric.Utils
	/// @{

	/// <summary>
	/// Defines the rounding precision used during float-to-Half16 conversion.
	/// The value represents the number of temporary mantissa bits used
	/// before packing into the final 10‑bit Half16 mantissa.
	/// </summary>
	public enum MantisseRoundMode {
        /// <summary>
        /// Classic IEEE‑754 style rounding.  
        /// Uses 13 temporary mantissa bits.  
        /// Provides moderate precision.
        /// </summary>
        Moderate        = 13,

        /// <summary>
        /// ONNX‑style rounding with improved precision.  
        /// Uses 14 temporary mantissa bits.
        /// </summary>
        Precise         = 14,

        /// <summary>
        /// Higher precision rounding.  
        /// Uses 15 temporary mantissa bits.
        /// </summary>
        VeryPrecise     = 15,

        /// <summary>
        /// Advanced high‑precision rounding.  
        /// Uses 16 temporary mantissa bits.
        /// </summary>
        UltraPrecise    = 16,

        /// <summary>
        /// Extremely high precision rounding.  
        /// Uses 18 temporary mantissa bits.
        /// </summary>
        ExtremePrecise  = 18,

        /// <summary>
        /// Maximum rounding precision available.  
        /// Uses 20 temporary mantissa bits.
        /// </summary>
        MaxPrecision    = 20
    }


    /// <summary>
    /// Provides extension methods for converting between different vector types
    /// (float, double, int) and dimensions (2D, 3D, 4D).
    /// </summary>
    public static class Conversition {

        /// <summary>
        /// Converts a 3D float vector to a 2D float vector by dropping the Z component.
        /// </summary>
        public static Vec2f ToVec2f ( this Vec3f vec ) {
            return new Vec2f(vec.X, vec.Y);
        }
        /// <summary>
        /// Converts a 4D float vector to a 2D float vector by dropping the Z and W components.
        /// </summary>
        public static Vec2f ToVec2f ( this Vec4f vec ) {
            return new Vec2f(vec.X, vec.Y);
        }
        /// <summary>
        /// Converts a 2D float vector to a 3D float vector, setting Z to zero.
        /// </summary>
        public static Vec3f ToVec3f ( this Vec2f vec ) {
            return new Vec3f(vec.X, vec.Y, 0.0f);
        }
        /// <summary>
        /// Converts a 4D float vector to a 3D float vector by dropping the W component.
        /// </summary>
        public static Vec3f ToVec3f ( this Vec4f vec ) {
            return new Vec3f(vec.X, vec.Y, vec.Z);
        }
        /// <summary>
        /// Converts a 2D float vector to a 4D float vector, setting Z and W to zero.
        /// </summary>
        public static Vec4f ToVec4f ( this Vec2f vec ) {
            return new Vec4f(vec.X, vec.Y, 0, 0);
        }
        /// <summary>
        /// Converts a 3D float vector to a 4D float vector, setting W to zero.
        /// </summary>
        public static Vec4f ToVec4f ( this Vec3f vec ) {
            return new Vec4f(vec.X, vec.Y, vec.Z, 0);
        }

        /// <summary>
        /// Converts a 3D double vector to a 2D double vector by dropping the Z component.
        /// </summary>
        public static Vec2d ToVec2d ( this Vec3d vec ) {
            return new Vec2d(vec.X, vec.Y);
        }
        /// <summary>
        /// Converts a 4D double vector to a 2D double vector by dropping the Z and W components.
        /// </summary>
        public static Vec2d ToVec2d ( this Vec4d vec ) {
            return new Vec2d(vec.X, vec.Y);
        }
        /// <summary>
        /// Converts a 2D double vector to a 3D double vector, setting Z to zero.
        /// </summary>
        public static Vec3d ToVec3d ( this Vec2d vec ) {
            return new Vec3d(vec.X, vec.Y, 0);
        }
        /// <summary>
        /// Converts a 4D double vector to a 3D double vector
        /// </summary>
        public static Vec3d ToVec3d ( this Vec4d vec ) {
            return new Vec3d(vec.X, vec.Y, vec.Z);
        }
        /// <summary>
        /// Converts a 2D double vector to a 4D double vector, setting Z and W to zero.
        /// </summary>
        public static Vec4d ToVec4d ( this Vec2d vec ) {
            return new Vec4d(vec.X, vec.Y, 0, 0);
        }
        /// <summary>
        /// Converts a 3D double vector to a 4D double vector, setting Z to zero.
        /// </summary>
        public static Vec4d ToVec4d ( this Vec3d vec ) {
            return new Vec4d(vec.X, vec.Y, vec.Z, 0);
        }

        /// <summary>
        /// Converts a 3D int vector to a 2D int vector by dropping the Z component.
        /// </summary>
        public static Vec2i ToVec2i ( this Vec3i vec ) {
            return new Vec2i(vec.X, vec.Y);
        }

        /// <summary>
        /// Converts a 4D int vector to a 2D int vector by dropping the Z and W component.
        /// </summary>
        public static Vec2i ToVec2i ( this Vec4i vec ) {
            return new Vec2i(vec.X, vec.Y);
        }

        /// <summary>
        /// Converts a 2D int vector to a 3D int vector , set Z to 0
        /// </summary>
        public static Vec3i ToVec3i ( this Vec2i vec ) {
            return new Vec3i(vec.X, vec.Y, 0);
        }
        /// <summary>
        /// Converts a 4D int vector to a 3D int vector 
        /// </summary>
        public static Vec3i ToVec3i ( this Vec4i vec ) {
            return new Vec3i(vec.X, vec.Y, vec.Z);
        }

        /// <summary>
        /// Converts a 2D int vector to a 4D int vector 
        /// </summary>
        public static Vec4i ToVec4i ( this Vec2i vec ) {
            return new Vec4i(vec.X, vec.Y, 0, 0);
        }
        /// <summary>
        /// Converts a 3D int vector to a 4D int vector 
        /// </summary>
        public static Vec4i ToVec4i ( this Vec3i vec ) {
            return new Vec4i(vec.X, vec.Y, vec.Z, 0);
        }
        /// <summary>
        /// Converts a 2D double vector to a 2D float vector 
        /// </summary>
        public static Vec2f ToVec2f ( this Vec2d vec ) {
            return new Vec2f((float)vec.X, (float)vec.Y);
        }
        /// <summary>
        /// Converts a 3D double vector to a 3D float vector 
        /// </summary>
        public static Vec3f ToVec3f ( this Vec3d vec ) {
            return new Vec3f((float)vec.X, (float)vec.Y, (float)vec.Z);
        }
        /// <summary>
        /// Converts a 4D double vector to a 4D float vector 
        /// </summary>
        public static Vec4f ToVec4f ( this Vec4d vec ) {
            return new Vec4f((float)vec.X, (float)vec.Y, (float)vec.Z, (float)vec.W);
        }
        /// <summary>
        /// Converts a 2D float vector to a 2D double vector 
        /// </summary>
        public static Vec2d ToVec2d ( this Vec2f vec ) {
            return new Vec2d(vec.X, vec.Y);
        }
        /// <summary>
        /// Converts a 3D float vector to a 3D double vector 
        /// </summary>
        public static Vec3d ToVec3d ( this Vec3f vec ) {
            return new Vec3d(vec.X, vec.Y, vec.Z);
        }
        /// <summary>
        /// Converts a 4D float vector to a 4D double vector 
        /// </summary>
        public static Vec4d ToVec4d ( this Vec4f vec ) {
            return new Vec4d(vec.X, vec.Y, vec.Z, vec.W);
        }

        /// <summary>
        /// Converts a 2D double vector to a 2D int vector 
        /// </summary>
        public static Vec2i ToVec2i ( this Vec2d vec ) {
            return new Vec2i((int)vec.X, (int)vec.Y);
        }
        /// <summary>
        /// Converts a 3D double vector to a 3D int vector 
        /// </summary>
        public static Vec3i ToVec3i ( this Vec3d vec ) {
            return new Vec3i((int)vec.X, (int)vec.Y, (int)vec.Z);
        }
        /// <summary>
        /// Converts a 4D double vector to a 4D int vector 
        /// </summary>
        public static Vec4i ToVec4i ( this Vec4d vec ) {
            return new Vec4i((int)vec.X, (int)vec.Y, (int)vec.Z, (int)vec.W);
        }

        /// <summary>
        /// Converts a 2D float vector to a 2D int vector 
        /// </summary>
        public static Vec2i ToVec2i ( this Vec2f vec ) {
            return new Vec2i((int)vec.X, (int)vec.Y);
        }
        /// <summary>
        /// Converts a 3D float vector to a 3D int vector 
        /// </summary>
        public static Vec3i ToVec3i ( this Vec3f vec ) {
            return new Vec3i((int)vec.X, (int)vec.Y, (int)vec.Z);
        }
        /// <summary>
        /// Converts a 4D float vector to a 4D int vector 
        /// </summary>
        public static Vec4i ToVec4i ( this Vec4f vec ) {
            return new Vec4i((int)vec.X, (int)vec.Y, (int)vec.Z, (int)vec.W);
        }

		/// <summary>
		/// Converts a float vector to a uint 
		/// </summary>
		public static uint ToInteger(this float value) {
            uint _bits = 0;

            unsafe {
                System.Buffer.MemoryCopy(&value, &_bits, sizeof(uint), sizeof(uint));
            }
            return _bits;
        }
		/// <summary>
		/// Converts a double vector to a ulong 
		/// </summary>
		public static ulong ToInteger ( this double value ) {
            ulong _bits = 0;

            unsafe {
                System.Buffer.MemoryCopy(&value, &_bits, sizeof(ulong), sizeof(ulong));
            }
            return _bits;
        }
		/// <summary>
		/// Converts a Half16 vector to a ushort 
		/// </summary>
		public static ushort ToInteger ( this Half16 value ) => value.ToBase;
		/// <summary>
		/// Converts a Half16b vector to a ushort 
		/// </summary>
		public static ushort ToInteger ( this Half16b value ) => value.ToBase;


		/// <summary>
		/// Converts a float vector to a Half16. with  MantisseRoundMode
		/// </summary>
		public static Half16 ToHalf16 ( this float value, MantisseRoundMode mode = MantisseRoundMode.Moderate ) {
            int tempMantissaBits = (int)mode;   // z.B. 13, 14, 15, 16, 18 …

            uint bits = value.ToInteger();      // deine Methode
            bool sign = (bits >> 31) != 0;
            int expF = (int)((bits >> 23) & 0xFF);
            uint mantF = bits & 0x7FFFFF;

            // Spezialfälle
            if ( expF == 0xFF ) {
                if ( mantF != 0 )
                    return Half16.NaN;
                return sign ? Half16.NegativeInfinity : Half16.PositiveInfinity;
            }

            // HiddenBit hinzufügen (float)
            uint mantFull = mantF | 0x800000;

            // Exponent übertragen (klassisch IEEE)
            int expH = expF - 112; // 127 - 15

            // Anzahl Bits, die wir wegschneiden müssen
            int cut = 23 - tempMantissaBits;

            // temporäre Mantisse erzeugen
            uint mantTemp = mantFull >> cut;

            // Rundungsbits extrahieren
            uint guard  = (mantFull >> (cut - 1)) & 1;
            uint round  = (mantFull >> (cut - 2)) & 1;
            uint sticky = (mantFull & ((1u << (cut - 2)) - 1)) != 0 ? 1u : 0u;

            // Round-to-nearest-even
            if ( guard == 1 ) {
                if ( round == 1 || sticky == 1 )
                    mantTemp++;
            }

            // Overflow der temporären Mantisse
            if ( mantTemp >= (1u << tempMantissaBits) ) {
                mantTemp >>= 1;
                expH++;
            }

            // Subnormal
            if ( expH <= 0 ) {
                int shift = 1 - expH;
                mantTemp >>= shift;
                expH = 0;
            }

            // Overflow → Infinity
            if ( expH >= 31 )
                return sign ? Half16.NegativeInfinity : Half16.PositiveInfinity;

            // finale Half16-Mantisse (10 Bits)
            ushort mantH = (ushort)(mantTemp & 0x3FF);

            return new Half16((ushort)(sign ? 1 : 0), (ushort)expH, mantH);
        }


		/// <summary>
		/// Converts a float vector to a Half16b. 
		/// </summary>
		public static Half16b ToHalf16b ( this float value ) {
            if ( float.IsNaN(value) ) { return Half16b.NaN; }

            uint _bits = value.ToInteger();
            bool _isLittle = (Conversion.GetEndian() == Endian.LittleEndian);

            ushort _bfloat = _isLittle ? (ushort)(_bits  & 0xFFFF) :  (ushort)(_bits  >> 16);

            _bits += ((uint)_bfloat & 1) + 0x7FFF;

            _bfloat = _isLittle ? (ushort)(_bits & 0xFFFF) : (ushort)(_bits >> 16);

            return new Half16b(_bfloat);
        }
		/// <summary>
		/// Converts a Half16b vector to a float. 
		/// </summary>
		public static float ToFloat(this Half16b value) {
            // bfloat16 is simply the upper 16 bits of a float
            uint floatBits = (uint)value.ToBase << 16;

            // reinterpret the bits as float
            unsafe {
                float result;
                System.Buffer.MemoryCopy(&floatBits, &result, sizeof(uint), sizeof(uint));
                return result;
            }
        }
		/// <summary>
		/// Converts a Half16 vector to a float. 
		/// </summary>
		public static float ToFloat ( this Half16 value ) {
            ushort bits = value.ToBase;

            uint sign = (uint)(bits >> 15) & 0x1;
            uint exp  = (uint)(bits >> 10) & 0x1F;
            uint mant = (uint)(bits & 0x3FF);

            // Zero
            if ( exp == 0 && mant == 0 ) {
                uint floatBits = sign << 31;
                unsafe {
                    float result;
                    System.Buffer.MemoryCopy(&floatBits, &result, sizeof(uint), sizeof(uint));
                    return result;
                }
            }

            // Infinity / NaN
            if ( exp == 0x1F ) {
                uint floatBits = (sign << 31) | 0x7F800000 | (mant << 13);
                unsafe {
                    float result;
                    System.Buffer.MemoryCopy(&floatBits, &result, sizeof(uint), sizeof(uint));
                    return result;
                }
            }

            uint floatExp;
            uint floatMant;

            if ( exp == 0 ) {
                // Subnormal half → normalized float
                // Shift mantissa until hidden bit appears
                int shift = 1;
                while ( (mant & 0x400) == 0 ) {
                    mant <<= 1;
                    shift++;
                }

                mant &= 0x3FF; // remove hidden bit
                floatExp = (uint)(127 - 15 - shift + 1);
                floatMant = mant << 13;
            } else {
                // Normalized number
                floatExp = exp - 15 + 127;
                floatMant = mant << 13;
            }

            uint floatBitsFinal =
                (sign << 31) |
                (floatExp << 23) |
                floatMant;

            unsafe {
                float result;
                System.Buffer.MemoryCopy(&floatBitsFinal, &result, sizeof(uint), sizeof(uint));
                return result;
            }
        }

    }
    /// @}
}
