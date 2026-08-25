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
	/// Represents an 16‑bit fast bit‑manipulation type. 
	/// This struct provides low‑level operations for inspecting, modifying,
	/// rotating, masking, and counting bits inside a single uint.
	/// 
	/// Fast_Int is intended for systems that require precise bit control,
	/// such as event groups, flag sets, embedded‑style logic, or any 
	/// performance‑critical bitmask operations. 
	/// 
	/// Users must understand bitwise operations, as incorrect usage can 
	/// intentionally overwrite or corrupt the underlying value.
	/// </summary>
	public struct Fast_Int : IFastType<uint> {
        private uint m_value;
        private byte m_size;

        /// <summary>
        /// Gets the number of bits available in this type (always 32).
        /// </summary>
        public byte Count => m_size;

        /// <summary>
        /// Gets the raw underlying 32‑bit unsigned integer value.
        /// </summary>

        public uint Value => m_value;

        /// <summary>
        /// Initializes a new Fast_Int instance with an optional initial value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fast_Int () : this(0) { }

        /// <summary>
        /// Initializes a new Fast_Int instance with an optional initial value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fast_Int ( uint value  ) {
            m_value = value;
            m_size = sizeof(uint) * 8;
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
                m_value = (value == 1) ? (m_value | 1U << pos) : (m_value & ~(1U << pos) );
            }
        }
        /// <summary>
        /// Produces the one's complement of the current value.
        /// All bits are inverted (bitwise NOT).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IFastType<uint> CmpOne () => new Fast_Int((uint)~m_value);
        /// <summary>
        /// Produces the two's complement of the current value.
        /// This is equivalent to (~value + 1) and is commonly used
        /// for subtraction in low‑level arithmetic.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IFastType<uint> CmpTwo () => new Fast_Int((uint)(~m_value + 1));

        /// <summary>
        /// Flips (toggles) the bit at the specified position.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Flip ( byte pos ) {
            m_value = (uint)(m_value ^ (1 << pos));
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
        public void Mask ( uint mask ) {
            uint v = m_value;
            m_value = (uint)(v & mask);
        }


        /// <summary>
        /// Rotates the bits to the left by the specified count.
        /// Rotation is limited to 0–31 to ensure correct 32‑bit behavior.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RotateLeft ( byte count ) {
            count &= (byte)(Count - 1);
            ulong v = (ulong)m_value;
            m_value = (uint)((v << count) | (v >> (Count - count)));
        }

        /// <summary>
        /// Rotates the bits to the right by the specified count.
        /// Rotation is limited to 0–15 to ensure correct 32‑bit behavior.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RotateRight ( byte count ) {
            count &= (byte)(Count - 1);
            ulong v = (ulong)m_value;
            m_value = (uint)((v >> count) | (v << (Count - count)));
        }

        /// <summary>
        /// Creates a bitmask with a given start position and length.
        /// The mask contains 'length' consecutive 1‑bits beginning at 'start'.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint CreateMask ( byte start, byte length ) {
            if ( length <= 0 ) return 0;
            if ( start < 0 || start > (Count - 1) ) return 0;
            if ( length >= Count ) return unchecked((uint)0xFFFFFFFF);

            int mask = (1 << length) - 1;
            return (uint)(mask << start);
        }

        /// <summary>
        /// Combines this value with another Fast_Int using bitwise OR.
        /// All bits that are set in either value become set.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IFastType<uint> Combine ( IFastType<uint> other ) {
            m_value = (uint)(m_value | other.Value);
            return this;
        }

        /// <summary>
        /// Counts the number of bits set to 1 in a 32‑bit unsigned integer.
        /// Uses a branchless parallel bit‑count algorithm ("Hacker's Delight").
        /// This avoids loops and provides excellent performance.
        /// </summary>
        public byte IsIt () {
            uint v = m_value;

            v = v - ((v >> 1) & 0x55555555);

            v = (v & 0x33333333) + ((v >> 2) & 0x33333333);

            v = (v + (v >> 4)) & 0x0F0F0F0F;

            v = v + (v >> 8);

            v = v + (v >> 16);

            return (byte)(v & 0x3F);
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
	/// @}
}
