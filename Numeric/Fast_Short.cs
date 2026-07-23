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

    /// <summary>
    /// Represents an 16‑bit fast bit‑manipulation type. 
    /// This struct provides low‑level operations for inspecting, modifying,
    /// rotating, masking, and counting bits inside a single ushort.
    /// 
    /// Fast_Short is intended for systems that require precise bit control,
    /// such as event groups, flag sets, embedded‑style logic, or any 
    /// performance‑critical bitmask operations. 
    /// 
    /// Users must understand bitwise operations, as incorrect usage can 
    /// intentionally overwrite or corrupt the underlying value.
    /// </summary>
    public struct Fast_Short : IFastType<ushort> {
        private ushort m_value;
        private byte m_size;

        /// <summary>
        /// Gets the number of bits available in this type (always 16).
        /// </summary>
        public byte Count => m_size;

        /// <summary>
        /// Gets the raw underlying ushort value.
        /// </summary>
        public ushort Value => m_value;

        /// <summary>
        /// Initializes a new Fast_Short instance with an optional initial value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fast_Short () : this(0) { }

        /// <summary>
        /// Initializes a new Fast_Short instance with an optional initial value.
        /// </summary>
        public Fast_Short ( ushort value  ) {
            m_value = value;
            m_size = sizeof(ushort) * 8;
        }
        /// <summary>
        /// Sets the bit at the specified position to the given value (0 or 1).
        /// The bit is only modified if the new value differs from the current one.
        /// This avoids unnecessary writes and preserves performance.
        /// </summary>
        /// <param name="pos">Bit position (0–16).</param>
        /// <param name="value">New bit value (0 or 1).</param>
        public void At ( byte pos, byte value ) {
            byte current = (byte)((m_value >> pos) & 1);

            if ( current != value ) {

                if ( value == 1 )
                    m_value = (ushort)(m_value | (1 << pos));
                else
                    m_value = (ushort)(m_value & ~(1 << pos));
            }
        }
        /// <summary>
        /// Produces the one's complement of the current value.
        /// All bits are inverted (bitwise NOT).
        /// </summary>
        public IFastType<ushort> CmpOne () => new Fast_Short((ushort)~m_value);
        /// <summary>
        /// Produces the two's complement of the current value.
        /// This is equivalent to (~value + 1) and is commonly used
        /// for subtraction in low‑level arithmetic.
        /// </summary>
        public IFastType<ushort> CmpTwo () => new Fast_Short((ushort)(~m_value + 1));

        /// <summary>
        /// Flips (toggles) the bit at the specified position.
        /// </summary>
        public void Flip ( byte pos ) {
            m_value = (ushort)(m_value ^ (1 << pos));
        }

        /// <summary>
        /// Returns the bit at the specified position (0 or 1).
        /// </summary>
        public byte Is ( byte pos ) => (byte)((m_value >> pos) & 1);

        /// <summary>
        /// Applies a bitmask to the current value using bitwise AND.
        /// Only bits that are 1 in the mask remain set.
        /// </summary>
        public void Mask ( ushort mask ) {
            uint v = m_value;
            m_value = (ushort)(v & mask);
        }


        /// <summary>
        /// Rotates the bits to the left by the specified count.
        /// Rotation is limited to 0–15 to ensure correct 16‑bit behavior.
        /// </summary>
        public void RotateLeft ( byte count ) {
            count &= (byte)(Count - 1);
            ulong v = m_value;
            m_value = (ushort)((v << count) | (v >> (Count - count)));
        }

        /// <summary>
        /// Rotates the bits to the right by the specified count.
        /// Rotation is limited to 0–15 to ensure correct 16‑bit behavior.
        /// </summary>
        public void RotateRight ( byte count ) {
            count &= (byte)(Count - 1);
            ulong v = (ulong)m_value;
            m_value = (ushort)((v >> count) | (v << (Count - count)));
        }

        /// <summary>
        /// Creates a bitmask with a given start position and length.
        /// The mask contains 'length' consecutive 1‑bits beginning at 'start'.
        /// </summary>
        public ushort CreateMask ( byte start, byte length ) {
            if ( length <= 0 ) return 0;
            if ( start < 0 || start > Count-1 ) return 0;
            if ( length >= Count ) return unchecked((ushort)0xFFFF);

            int mask = (1 << length) - 1;
            return (ushort)(mask << start);
        }

        /// <summary>
        /// Combines this value with another Fast_Short using bitwise OR.
        /// All bits that are set in either value become set.
        /// </summary>
        public IFastType<ushort> Combine ( IFastType<ushort> other ) {
            m_value = (ushort)(m_value | other.Value);
            return this;
        }

        /// <summary>
        /// Counts the number of bits set to 1 in a 16‑bit unsigned value (ushort).
        /// This method uses a branchless bit‑hack algorithm that avoids loops and 
        /// provides excellent performance on all platforms.
        ///
        /// The algorithm is based on a well‑known parallel bit counting technique:
        /// 1. Subtract shifted half‑bit groups.
        /// 2. Collapse bit pairs into nibbles.
        /// 3. Sum the nibbles to produce the final population count.
        ///
        /// This approach is significantly faster than iterating over all 16 bits,
        /// especially in performance‑critical or embedded‑style systems.
        /// </summary>
        /// <returns>
        /// The number of bits set to 1 in the underlying 16‑bit value.
        /// </returns>
        public byte IsIt () {
            uint v = m_value; // promote to 32‑bit for safe arithmetic

            // Step 1: subtract half‑bit groups
            v = v - ((v >> 1) & 0x5555);

            // Step 2: collapse into 2‑bit groups
            v = (v & 0x3333) + ((v >> 2) & 0x3333);

            // Step 3: collapse into 4‑bit groups (nibbles)
            v = (v + (v >> 4)) & 0x0F0F;

            // Step 4: final sum of both nibbles
            v = v + (v >> 8);

            return (byte)(v & 0x1F); // 16 bits → max popcount = 16
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
