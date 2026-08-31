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

namespace SystemEx.Numeric {
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

	public static partial class Conversition {
#region SYSTEM
		/// <summary>
		/// Converts a float to a uint 
		/// </summary>
		public static uint ToInteger ( this float value ) {
			uint _bits = 0;

			unsafe {
				System.Buffer.MemoryCopy(&value, &_bits, sizeof(uint), sizeof(uint));
			}
			return _bits;
		}
		/// <summary>
		/// Converts a double to a ulong 
		/// </summary>
		public static ulong ToInteger ( this double value ) {
			ulong _bits = 0;

			unsafe {
				System.Buffer.MemoryCopy(&value, &_bits, sizeof(ulong), sizeof(ulong));
			}
			return _bits;
		}
#endregion

#region Half16
		/// <summary>
		/// Converts a Half16 to a ushort 
		/// </summary>
		public static ushort ToInteger ( this Half16 value ) => value.ToBase;

		/// <summary>
		/// Converts a Half16 to a float. 
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
		/// <summary>
		/// Converts a float  to a Half16. with  MantisseRoundMode
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
#endregion

#region Half16b
		/// <summary>
		/// Converts a Half16b to a ushort 
		/// </summary>

		public static ushort ToInteger ( this Half16b value ) => value.ToBase;


		/// <summary>
		/// Converts a float to a Half16b. 
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
		/// Converts a Half16b to a float. 
		/// </summary>
		public static float ToFloat ( this Half16b value ) {
			// bfloat16 is simply the upper 16 bits of a float
			uint floatBits = (uint)value.ToBase << 16;

			// reinterpret the bits as float
			unsafe {
				float result;
				System.Buffer.MemoryCopy(&floatBits, &result, sizeof(uint), sizeof(uint));
				return result;
			}
		}


#endregion

#region FloatE4M3

		/// <summary>
		/// Converts a Half16b to a float. 
		/// </summary>
		public static float ToFloat ( this FloatE4M3 value ) {

			// raw FP8 byte (assumes ToBase is convertible to byte)
			byte raw = (byte)value.ToBase;

			// sign, exponent, mantissa
			int sign = (raw >> 7) & 0x1;
			int exp  = (raw >> 3) & 0x0F;
			int mant = raw & 0x07;

			// NaN (industry E4M3 canonical NaN: exp == 0xF && mant == 0x7)
			if ( exp == 0x0F && mant == 0x07 )
				return float.NaN;

			// Zero (positive or negative)
			if ( exp == 0 && mant == 0 )
				return sign == 1 ? -0.0f : 0.0f;

			// Compute value
			// Bias = 7, hidden bit = 1 for normals, mantissa fraction = mant / 8.0
			int bias = 7;
			float fraction;
			int e;

			if ( exp == 0 ) {
				// subnormal: value = (-1)^sign * (mant/8) * 2^(1 - bias)
				fraction = mant / 8.0f;      // 0.xxx
				e = 1 - bias;                // -6
			} else {
				// normalized: value = (-1)^sign * (1 + mant/8) * 2^(exp - bias)
				fraction = 1.0f + (mant / 8.0f); // 1.xxx
				e = exp - bias;
			}

			float result = (sign == 1 ? -1f : 1f) * fraction * MathF.Pow(2.0f, e);
			return result;
		}


		public static byte ToInteger ( this FloatE4M3 value ) {
			return value.ToBase.Value;
		}

		/// <summary>
		/// Encode a 32‑bit IEEE‑754 float into an FP8 E4M3 value (returned as FloatE4M3).
		/// No BitConverter or reinterpret casts are used; the method relies on the
		/// existing float->uint ToInteger() helper and constructs the FP8 byte directly.
		/// </summary>
		public static FloatE4M3 ToFloatE4M3 ( this float value ) {
			// Decompose float into fields
			Decompose(value,
				out byte signBit,
				out byte rawExp,
				out int unbiasedExp,
				out uint rawMant,
				out bool isZero,
				out bool isSubnormal,
				out bool isInfinity,
				out bool isNaN);

			if ( isNaN ) return FloatE4M3.NaN;
			// Infinity -> map to FP8 NaN (E4M3 has no infinity)
			if ( isInfinity ) return FloatE4M3.NaN;
			// Zero (preserve sign)
			if ( isZero ) return FloatE4M3.Zero;

			// FP8 parameters
			const int fp8Bias = 7;
			const int fp8MantBits = 3;
			const int floatMantBits = 23;
			const int cut = floatMantBits - fp8MantBits; // 20

			// Compute target exponent field: exp8 = unbiasedExp + bias8
			int exp8 = unbiasedExp + fp8Bias;

			// Prepare mantissa with or without hidden bit
			// For normals: hidden bit present (1 << 23)
			// For subnormals: no hidden bit
			uint mantFull = isSubnormal ? rawMant : (rawMant | (1u << floatMantBits)); // 24-bit value

			// Truncate mantissa to (hidden + fp8MantBits) bits
			// mantTemp contains hidden<<fp8MantBits | truncated_fraction
			uint mantTemp = mantFull >> cut; // up to 1<<(fp8MantBits+1)

			// Extract rounding bits (guard, round, sticky) from the dropped part
			uint guard  = (mantFull >> (cut - 1)) & 1u;
			uint round  = (mantFull >> (cut - 2)) & 1u;
			uint sticky = (mantFull & ((1u << (cut - 2)) - 1u)) != 0 ? 1u : 0u;

			// Least significant bit of mantTemp (for tie-to-even)
			uint lsb = mantTemp & 1u;

			// Round-to-nearest-even
			if ( guard == 1u ) {
				if ( round == 1u || sticky == 1u || (lsb == 1u && round == 0u && sticky == 0u) )
					mantTemp++;
			}

			// If mantissa overflowed (hidden + fp8MantBits -> fp8MantBits+1 bits),
			// shift right and increment exponent.
			uint overflowThreshold = 1u << (fp8MantBits + 1); // 1 << 4 == 16
			if ( mantTemp >= overflowThreshold ) {
				mantTemp >>= 1;
				exp8++;
			}

			// If exponent underflows to <= 0, produce FP8 subnormal by shifting mantissa
			if ( exp8 <= 0 ) {
				int shift = 1 - exp8;
				if ( shift >= 32 )
					mantTemp = 0;
				else
					mantTemp >>= shift;
				exp8 = 0;
			}

			// If exponent overflows beyond max (0x0E is max normal, 0x0F reserved for NaN),
			// encode as FP8 NaN (industry choice for E4M3 without infinity)
			if ( exp8 >= 0x0F )
				return new FloatE4M3(0x7F);

			// Final 3-bit mantissa (remove hidden bit)
			byte finalMant = (byte)(mantTemp & ((1u << fp8MantBits) - 1u)); // & 0x07
			byte finalExp  = (byte)exp8;
			byte finalSign = signBit;

			// Construct and return FloatE4M3 (uses constructor that accepts sign, exponent, mantissa)
			return new FloatE4M3(finalSign, finalExp, finalMant);
		}
#endregion

		/// <summary>
		/// Decomposes a 32‑bit IEEE‑754 single precision float into its raw bit fields
		/// and common classification flags.
		/// 
		/// This helper does NOT use BitConverter or reinterpret casts. It relies on the
		/// existing float->uint reinterpret helper (ToInteger) that you already have.
		/// The method returns both the raw exponent field and the unbiased exponent
		/// (useful for conversion math), the raw mantissa bits, and boolean flags for
		/// zero, subnormal, infinity and NaN.
		/// </summary>
		/// <param name="value">The single precision float to decompose.</param>
		/// <param name="signBit">Output: 0 for positive, 1 for negative.</param>
		/// <param name="rawExponent">Output: raw 8‑bit exponent field (0..255).</param>
		/// <param name="unbiasedExponent">Output: exponent with IEEE bias removed; for subnormals this is 1 - 127.</param>
		/// <param name="rawMantissa">Output: raw 23‑bit mantissa field (fraction bits only).</param>
		/// <param name="isZero">Output: true if ±0.0.</param>
		/// <param name="isSubnormal">Output: true if subnormal (exp==0 && mantissa!=0).</param>
		/// <param name="isInfinity">Output: true if ±Infinity.</param>
		/// <param name="isNaN">Output: true if NaN.</param>
		public static void Decompose ( this float value,
			out byte signBit,
			out byte rawExponent,
			out int unbiasedExponent,
			out uint rawMantissa,
			out bool isZero,
			out bool isSubnormal,
			out bool isInfinity,
			out bool isNaN ) {
			// reinterpret float bits to uint using your existing helper
			uint bits = value.ToInteger();

			// extract fields
			signBit = (byte)((bits >> 31) & 0x1);
			rawExponent = (byte)((bits >> 23) & 0xFF);
			rawMantissa = bits & 0x7FFFFFu;

			// classification
			isZero = (rawExponent == 0 && rawMantissa == 0);
			isSubnormal = (rawExponent == 0 && rawMantissa != 0);
			isInfinity = (rawExponent == 0xFF && rawMantissa == 0);
			isNaN = (rawExponent == 0xFF && rawMantissa != 0);

			// unbiased exponent: for normals exp - 127, for subnormals use 1 - 127
			if ( rawExponent == 0 )
				unbiasedExponent = 1 - 127; // subnormal exponent
			else
				unbiasedExponent = (int)rawExponent - 127;
		}

	}
}
