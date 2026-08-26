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

namespace SystemEx.Numeric {
	/// \addtogroup Numeric
	/// @{

	/// <summary>
	/// Provides low-level IEEE‑754 bit inspection and manipulation for 32‑bit floating-point values.
	/// This type follows the naming and structural conventions of the Fast_Type family, but is
	/// specialized for floating-point analysis rather than general-purpose bitmask operations.
	/// 
	/// Exposes:
	///   - Sign bit (1 bit)
	///   - Exponent (8 bits)
	///   - Mantissa (23 bits)
	/// 
	/// Uses SystemEx byte conversion utilities for endian-safe reinterpretation.
	/// </summary>
	public struct Fast_Float {

        private float m_value;

        /// <summary>
        /// Gets the underlying floating-point value.
        /// </summary>
        public float Value => m_value;

        /// <summary>
        /// Initializes a new Fast_Float instance with an optional initial value.
        /// </summary>
        public Fast_Float ( float value = 0f ) {
            m_value = value;
        }

        /// <summary>
        /// Reinterprets the float as a 32-bit unsigned integer using SystemEx endian utilities.
        /// </summary>
        public uint ToBits () {
            byte[] _raw = m_value.ToBytes(Endian.System);
            return _raw.ToUInt(Endian.System);

        }

        public Fast_Int ToFastBits() {
            return new Fast_Int(ToBits());
        }

        /// <summary>
        /// Reinterprets a 32-bit unsigned integer as a float and stores it.
        /// </summary>
        public void FromBits ( uint bits ) {
            byte[] _raw = bits.ToBytes(Endian.System);
            m_value =  _raw.ToFloat(Endian.System);
        }

        public void FromFastBits ( Fast_Int bits ) {
            FromBits(bits.Value);
        }

        /// <summary>
        /// Gets the sign bit (0 = positive, 1 = negative).
        /// </summary>
        public Fast_Byte GetSign () {
            return new Fast_Byte( (byte)((ToBits() >> 31) & 1U) );
        }

        /// <summary>
        /// Gets the 8-bit exponent field (raw, not biased).
        /// </summary>
        public Fast_Byte GetExponent () {
            return new Fast_Byte((byte)((ToBits() >> 23) & 0xFFU));

        }

        /// <summary>
        /// Gets the 23-bit mantissa field.
        /// </summary>
        public Fast_Int GetMantissa () {
            return new Fast_Int( ToBits() & 0x7FFFFFU );
        }

        /// <summary>
        /// Sets the sign bit (0 or 1).
        /// </summary>
        public void SetSign ( Fast_Byte sign ) {
            uint bits = ToBits();
            bits = (bits & 0x7FFFFFFF) | ((uint)(sign.Value & 1) << 31);
            FromBits(bits);
        }

        /// <summary>
        /// Sets the exponent field (raw 8-bit value).
        /// </summary>
        public void SetExponent ( Fast_Byte exponent ) {
            uint bits = ToBits();
            bits = (bits & 0x807FFFFF) | ((uint)exponent.Value << 23);
            FromBits(bits);
        }

        /// <summary>
        /// Sets the mantissa field (raw 23-bit value).
        /// </summary>
        public void SetMantissa ( Fast_Int mantissa ) {
            uint bits = ToBits();
            bits = (bits & 0xFF800000) | (mantissa.Value & 0x7FFFFF);
            FromBits(bits);
        }

        /// <summary>
        /// Flips a single bit inside the mantissa field (0–22).
        /// </summary>
        public void FlipMantissaBit ( byte pos ) {
            if ( pos > 22 ) return;

            uint bits = ToBits();
            uint mask = 1U << pos;
            uint mantissa = bits & 0x7FFFFF;

            mantissa ^= mask;

            bits = (bits & 0xFF800000) | mantissa;
            FromBits(bits);
        }

        /// <summary>
        /// Flips a single bit inside the exponent field (0–7).
        /// </summary>
        public void FlipExponentBit ( byte pos ) {
            if ( pos > 7 ) return;

            uint bits = ToBits();
            uint exponent = (bits >> 23) & 0xFFU;

            exponent ^= (1U << pos);

            bits = (bits & 0x807FFFFF) | (exponent << 23);
            FromBits(bits);
        }

        /// <summary>
        /// Returns true if the value is NaN.
        /// </summary>
        public bool IsNaN () => float.IsNaN(m_value);

        /// <summary>
        /// Returns true if the value is positive or negative infinity.
        /// </summary>
        public bool IsInfinity () => float.IsInfinity(m_value);

        /// <summary>
        /// Returns true if the value is a subnormal (denormalized) float.
        /// </summary>
        public bool IsSubnormal () {
            uint bits = ToBits();
            uint exponent = (bits >> 23) & 0xFFU;
            uint mantissa = bits & 0x7FFFFF;
            return exponent == 0 && mantissa != 0;
        }
    }
	/// @}
}
