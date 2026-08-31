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
using SystemEx.Utils;


namespace SystemEx.Numeric {
	/// \addtogroup Numeric
	/// @{

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
	public struct Fast_UShort : IFastType<ushort>, IComparable<Fast_UShort>, IComparableEx<Fast_UShort> {
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
        public Fast_UShort () : this(0) { }

        /// <summary>
        /// Initializes a new Fast_Short instance with an optional initial value.
        /// </summary>
        public Fast_UShort ( ushort value  ) {
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
        public IFastType<ushort> CmpOne () => new Fast_UShort((ushort)~m_value);
        /// <summary>
        /// Produces the two's complement of the current value.
        /// This is equivalent to (~value + 1) and is commonly used
        /// for subtraction in low‑level arithmetic.
        /// </summary>
        public IFastType<ushort> CmpTwo () => new Fast_UShort((ushort)(~m_value + 1));

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

		/// <summary>
		/// Addition
		/// </summary>
		public Fast_UShort Add ( Fast_UShort oth ) {
			m_value += oth.m_value; return this;
		}
		/// <summary>
		/// Subtraktion
		/// </summary>
		public Fast_UShort Sub ( Fast_UShort oth ) {
			m_value -= oth.m_value; return this;
		}
		/// <summary>
		/// Multiplikation
		/// </summary>
		public Fast_UShort Mul ( Fast_UShort oth ) {
			m_value *= oth.m_value; return this;
		}
		/// <summary>
		/// Division
		/// </summary>
		public Pair<Fast_UShort, Fast_UShort> Div ( Fast_UShort oth ) {
			m_value /= oth.m_value;
			byte v = (byte)(m_value % oth.m_value);

			return new(this, new(v));
		}

		public CompareResult CompareTo ( Fast_UShort a ) {
			if ( m_value > a.m_value ) return CompareResult.Greater;
			else if ( m_value < a.m_value ) return CompareResult.Less;

			return CompareResult.Equal;
		}
		int IComparable<Fast_UShort>.CompareTo ( Fast_UShort a ) {
			return (int)CompareTo(a);
		}

		public static Fast_UShort Min ( Fast_UShort a, Fast_UShort b ) {
			return a.m_value < b.m_value ? a : b;
		}
		public static Fast_UShort Max ( Fast_UShort a, Fast_UShort b ) {
			return a.m_value > b.m_value ? a : b;
		}

		/// <inheritdoc/>
		public static bool operator == ( Fast_UShort a, Fast_UShort b ) {
			return a.m_value == b.m_value;
		}

		/// <inheritdoc/>
		public static bool operator != ( Fast_UShort a, Fast_UShort b ) {
			return !(a == b);
		}

		/// <inheritdoc/>
		public static bool operator <= ( Fast_UShort a, Fast_UShort b ) {
			return a.m_value <= b.m_value;
		}
		/// <inheritdoc/>
		public static bool operator >= ( Fast_UShort a, Fast_UShort b ) {
			return a.m_value >= b.m_value;
		}
		/// <inheritdoc/>
		public static bool operator < ( Fast_UShort a, Fast_UShort b ) {
			return a.m_value < b.m_value;
		}
		/// <inheritdoc/>
		public static bool operator > ( Fast_UShort a, Fast_UShort b ) {
			return a.m_value > b.m_value;
		}

		public static Fast_UShort operator + ( Fast_UShort a, Fast_UShort b ) {
			a.m_value += b.m_value;
			return a;
		}
		public static Fast_UShort operator - ( Fast_UShort a, Fast_UShort b ) {
			a.m_value -= b.m_value;
			return a;
		}
		public static Fast_UShort operator - ( Fast_UShort a ) {
			a.m_value = (byte)(-a.m_value);
			return a;
		}
		public static Fast_UShort operator -- ( Fast_UShort a ) {
			a.m_value--;
			return a;
		}
		public static Fast_UShort operator ++ ( Fast_UShort a ) {
			a.m_value++;
			return a;
		}
		public static Fast_UShort operator * ( Fast_UShort a, Fast_UShort b ) {
			a.m_value *= b.m_value;
			return a;
		}
		public static Fast_UShort operator / ( Fast_UShort a, Fast_UShort b ) {
			a.m_value /= b.m_value;
			return a;
		}

		/// <summary>
		/// Implicitly converts a byte value into an <see cref="Fast_UShort"/>.
		/// </summary>
		/// <param name="value">The byte value to convert.</param>
		public static implicit operator Fast_UShort ( ushort value ) {
			return new Fast_UShort(value);
		}

		public static implicit operator Fast_UShort ( Fast_Byte value ) {
			return new Fast_UShort(value.Value);
		}

		/// <summary>
		/// Implicitly converts a int value into an <see cref="Fast_UShort"/>.
		/// </summary>
		/// <param name="value">The int value to convert.</param>
		public static implicit operator Fast_UShort ( int value ) {
			return new Fast_UShort((ushort)value);
		}

		/// <summary>
		/// Implicitly converts a uint value into an <see cref="Fast_UShort"/>.
		/// </summary>
		/// <param name="value">The uint value to convert.</param>
		public static implicit operator Fast_UShort ( uint value ) {
			return new Fast_UShort((ushort)value);
		}

		/// <summary>
		/// Explicitly extracts the underlying value from an <see cref="Fast_UShort"/>.
		/// </summary>
		/// <param name="opt">The optional instance to extract from.</param>
		/// <returns>The stored value.</returns>
		public static explicit operator ushort ( Fast_UShort opt ) {
			return opt.m_value;
		}

		/// <summary>
		/// Explicitly extracts the underlying value from an <see cref="Fast_UShort"/>.
		/// </summary>
		/// <param name="opt">The optional instance to extract from.</param>
		/// <returns>The stored value.</returns>
		public static explicit operator int ( Fast_UShort opt ) {
			return opt.m_value;
		}

		/// <inheritdoc/>
		public override bool Equals ( object? obj ) {
			if ( obj is Fast_UShort key )
				return Equals(key);
			return false;
		}

		/// <inheritdoc/>
		public bool Equals ( Fast_UShort b ) {
			return this.m_value == b.m_value;
		}

		public bool Equals ( ushort b ) {
			return this.m_value == b;
		}

		/// <inheritdoc/>
		public override int GetHashCode () {
			return m_value.GetHashCode();
		}
		/// <inheritdoc/>
		public override string ToString () {
			return m_value.ToString();
		}

		public static Fast_UShort operator & ( Fast_UShort a, Fast_UShort b ) {
			return (ushort)(a.m_value & b.m_value);
		}
		public static Fast_UShort operator % ( Fast_UShort a, Fast_UShort b ) {
			return (ushort)(a.m_value % b.m_value);
		}
		public static Fast_UShort operator | ( Fast_UShort a, Fast_UShort b ) {
			return (ushort)(a.m_value | b.m_value);
		}
		public static Fast_UShort operator ^ ( Fast_UShort a, Fast_UShort b ) {
			return (ushort)(a.m_value ^ b.m_value);
		}

		public static Fast_UShort operator << ( Fast_UShort a, Fast_UShort b ) {
			a.m_value = (ushort)(a.m_value << b.m_value);
			return a;
		}
		public static Fast_UShort operator >> ( Fast_UShort a, Fast_UShort b ) {
			a.m_value = (ushort)(a.m_value >> b.m_value);
			return a;
		}
	}
	
}
