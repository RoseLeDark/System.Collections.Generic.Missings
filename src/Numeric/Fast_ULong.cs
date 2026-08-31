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


using System.Runtime.CompilerServices;
using SystemEx.Collections.Generic;


namespace SystemEx.Numeric {
	/// \addtogroup Numeric
	/// @{

	/// <summary>
	/// Represents an 64‑bit fast bit‑manipulation type. 
	/// This struct provides low‑level operations for inspecting, modifying,
	/// rotating, masking, and counting bits inside a single ulong.
	/// 
	/// Fast_Longis intended for systems that require precise bit control,
	/// such as event groups, flag sets, embedded‑style logic, or any 
	/// performance‑critical bitmask operations. 
	/// 
	/// Users must understand bitwise operations, as incorrect usage can 
	/// intentionally overwrite or corrupt the underlying value.
	/// </summary>
	public struct Fast_ULong: IFastType<ulong> {
        private ulong m_value;
        private byte m_size;

        /// <summary>
        /// Gets the number of bits available in this type (always 64).
        /// </summary>
        public byte Count => m_size;

        /// <summary>
        /// Gets the raw underlying 64‑bit unsigned integer value.
        /// </summary>

        public ulong Value => m_value;

        /// <summary>
        /// Initializes a new Fast_Long instance with an optional initial value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fast_ULong() : this(0) { }

        /// <summary>
        /// Initializes a new Fast_Longinstance with an optional initial value.
        /// </summary>
        public Fast_ULong( ulong value ) {
            m_value = value;
            m_size = sizeof(ulong) * 8;
        }
        /// <summary>
        /// Sets the bit at the specified position to the given value (0 or 1).
        /// The bit is only modified if the new value differs from the current one.
        /// This avoids unnecessary writes and preserves performance.
        /// </summary>
        /// <param name="pos">Bit position (0–16).</param>
        /// <param name="value">New bit value (0 or 1).</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void At ( byte pos, byte value ) {
            byte current = (byte)((m_value  >> pos) & 1U) ;

            if ( current != value ) {
                m_value = (value == 1) ? (m_value | 1U << pos) : (m_value & ~(1U << pos));
            }
        }
        /// <summary>
        /// Produces the one's complement of the current value.
        /// All bits are inverted (bitwise NOT).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IFastType<ulong> CmpOne () => new Fast_ULong((ulong)~m_value);
        /// <summary>
        /// Produces the two's complement of the current value.
        /// This is equivalent to (~value + 1) and is commonly used
        /// for subtraction in low‑level arithmetic.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IFastType<ulong> CmpTwo () => new Fast_ULong((ulong)(~m_value + 1));

        /// <summary>
        /// Flips (toggles) the bit at the specified position.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Flip ( byte pos ) {
            m_value = (ulong)(m_value ^ (1UL << pos));
        }

        /// <summary>
        /// Returns the bit at the specified position (0 or 1).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte Is ( byte pos ) => (byte)((m_value >> pos) & 1);

        /// <summary>
        /// Applies a bitmask to the current value using bitwise AND.
        /// Only bits that are 1 in the mask remain set.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Mask ( ulong mask ) {
            ulong v = m_value;
            m_value = (v & mask);
        }


        /// <summary>
        /// Rotates the bits to the left by the specified count.
        /// Rotation is limited to 0–31 to ensure correct 32‑bit behavior.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RotateLeft ( byte count ) {
            count &= (byte)(Count - 1);
            ulong v = (ulong)m_value;
            m_value = ((v << count) | (v >> (Count - count)));
        }

        /// <summary>
        /// Rotates the bits to the right by the specified count.
        /// Rotation is limited to 0–15 to ensure correct 32‑bit behavior.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RotateRight ( byte count ) {
            count &= (byte)(Count - 1);
            ulong v = (ulong)m_value;
            m_value = ((v >> count) | (v << (Count - count)));
        }

        /// <summary>
        /// Creates a bitmask with a given start position and length.
        /// The mask contains 'length' consecutive 1‑bits beginning at 'start'.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong CreateMask ( byte start, byte length ) {
            if ( length <= 0 ) return 0;
            if ( start < 0 || start > (Count - 1) ) return 0;
            if ( length >= Count ) return unchecked((ulong)0xFFFFFFFFFFFFFFFF);

            long mask = (1 << length) - 1;
            return (ulong)(mask << start);
        }

        /// <summary>
        /// Combines this value with another Fast_Longusing bitwise OR.
        /// All bits that are set in either value become set.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IFastType<ulong> Combine ( IFastType<ulong> other ) {
            m_value = (m_value | other.Value);
            return this;
        }

        /// <summary>
        /// Counts the number of bits set to 1 in a 64‑bit unsigned integer (ulong).
        /// Uses the branchless parallel bit‑count algorithm ("Hacker's Delight"),
        /// which collapses bit groups step‑by‑step without loops.
        /// 
        /// This method is extremely fast and predictable, making it ideal for
        /// embedded‑style systems, event groups, bitmask engines, and any
        /// performance‑critical numeric processing.
        /// </summary>
        /// <returns>
        /// The number of bits set to 1 (range: 0–64).
        /// </returns>
        public byte IsIt () {
            ulong v = m_value;

            //  subtract half‑bit groups
            v = v - ((v >> 1) & 0x5555555555555555UL);

            // collapse into 2‑bit groups
            v = (v & 0x3333333333333333UL) + ((v >> 2) & 0x3333333333333333UL);

            // collapse into 4‑bit groups
            v = (v + (v >> 4)) & 0x0F0F0F0F0F0F0F0FUL;

            //  collapse into 8‑bit groups
            v = v + (v >> 8);

            // collapse into 16‑bit groups
            v = v + (v >> 16);

            // collapse into 32‑bit groups
            v = v + (v >> 32);

            // Final result: lower 7 bits contain the popcount (0–64)
            return (byte)(v & 0x7F);
        }




        /// <summary>
        /// Counts the number of bits set to 0.
        /// Equivalent to Count - IsIt().
        /// </summary>
        public byte IsItNot () => (byte)(Count - IsIt());

        /// <summary>
        /// Returns all bit positions where the bit is set to 1.
        /// </summary>
        public FixedVector<byte> Where () {
            var _size = IsIt();

            FixedVector<byte> _set = new FixedVector<byte>();

            if ( _size != 0 ) {

                for ( byte i = 0 ; i < Count ; i++ ) {
                    if ( Is(i) == 1 ) { _set.PushBack(i); }
                }
            }
            return _set;
        }

        /// <summary>
        /// Returns all bit positions where the bit is set to 0.
        /// </summary>
        public FixedVector<byte> WhereNot () {
            var _size = IsItNot();

            FixedVector<byte> _set = new FixedVector<byte>();

            if ( _size != 0 ) {

                for ( byte i = 0 ; i < Count ; i++ ) {
                    if ( Is(i) == 0 ) { _set.PushBack(i); }
                }
            }
            return _set;
        }
    }
	
}
