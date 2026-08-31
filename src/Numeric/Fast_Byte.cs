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
using System.Runtime.InteropServices;
using SystemEx.Collections.Generic;
using SystemEx.Utils;


namespace SystemEx.Numeric {

	/// <summary>
	/// Represents an 8‑bit fast bit‑manipulation type. 
	/// This struct provides low‑level operations for inspecting, modifying,
	/// rotating, masking, and counting bits inside a single byte.
	/// 
	/// Fast_Byte is intended for systems that require precise bit control,
	/// such as event groups, flag sets, embedded‑style logic, or any 
	/// performance‑critical bitmask operations. 
	/// 
	/// Users must understand bitwise operations, as incorrect usage can 
	/// intentionally overwrite or corrupt the underlying value.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Fast_Byte : IFastType<byte>, IEquatable<Fast_Byte>, IComparableEx<Fast_Byte>, IComparable<Fast_Byte> {
        private byte m_value;
        private byte m_size;

        /// <summary>
        /// Gets the number of bits available in this type (always 8).
        /// </summary>
        public byte Count => m_size;

        /// <summary>
        /// Gets the raw underlying byte value.
        /// </summary>
        public byte Value => m_value;

        /// <summary>
        /// Initializes a new Fast_Byte instance with an optional initial value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fast_Byte () : this(0) { }

        /// <summary>
        /// Initializes a new Fast_Byte instance with an optional initial value.
        /// </summary>
        public Fast_Byte (byte value ) {
            m_value = value;
            m_size = sizeof(byte) * 8;
        }
        /// <summary>
        /// Sets the bit at the specified position to the given value (0 or 1).
        /// The bit is only modified if the new value differs from the current one.
        /// This avoids unnecessary writes and preserves performance.
        /// </summary>
        /// <param name="pos">Bit position (0–7).</param>
        /// <param name="value">New bit value (0 or 1).</param>
        public void At ( byte pos, byte value ) {
            byte current = (byte)((m_value >> pos) & 1);

            if ( current != value ) {

                if ( value == 1 )
                    m_value = (byte)(m_value | (1 << pos));
                else
                    m_value = (byte)(m_value & ~(1 << pos));
            }
        }
        /// <summary>
        /// Produces the one's complement of the current value.
        /// All bits are inverted (bitwise NOT).
        /// </summary>
        public IFastType<byte> CmpOne () => new Fast_Byte( (byte)~m_value );
        /// <summary>
        /// Produces the two's complement of the current value.
        /// This is equivalent to (~value + 1) and is commonly used
        /// for subtraction in low‑level arithmetic.
        /// </summary>
        public IFastType<byte> CmpTwo () => new Fast_Byte( (byte)(~m_value + 1) );

        /// <summary>
        /// Flips (toggles) the bit at the specified position.
        /// </summary>
        public void Flip ( byte pos ) {
			pos &= 7;
			m_value = (byte)(m_value ^ (1 << pos)); 
        }

        /// <summary>
        /// Returns the bit at the specified position (0 or 1).
        /// </summary>
        public byte Is ( byte pos ) {
			pos &= 7;
			return (byte)((m_value >> pos) & 1);
        }

        /// <summary>
        /// Applies a bitmask to the current value using bitwise AND.
        /// Only bits that are 1 in the mask remain set.
        /// </summary>
        public void Mask ( byte mask ) {
            uint v = m_value;
            m_value = (byte) (v & mask);
        }


        /// <summary>
        /// Rotates the bits to the left by the specified count.
        /// Rotation is limited to 0–7 to ensure correct 8‑bit behavior.
        /// </summary>
        public void RotateLeft ( byte count ) {
            count &= 7;
            ulong v = m_value;
            m_value = (byte)((v << count) | (v >> (8 - count)));
        }

        /// <summary>
        /// Rotates the bits to the right by the specified count.
        /// Rotation is limited to 0–7 to ensure correct 8‑bit behavior.
        /// </summary>
        public void RotateRight ( byte count ) {
            count &= 7;
            ulong v = (ulong)m_value;
            m_value = (byte)((v >> count) | (v << (8 - count)));
        }

        /// <summary>
        /// Creates a bitmask with a given start position and length.
        /// The mask contains 'length' consecutive 1‑bits beginning at 'start'.
        /// </summary>
        public byte CreateMask ( byte start, byte length ) {
            if ( length <= 0 ) return 0;
            if ( start < 0 || start > 7 ) return 0;
            if ( length >= 8 ) return unchecked((byte)0xFF);

            int mask = (1 << length) - 1;
            return (byte)(mask << start);
        }

        /// <summary>
        /// Combines this value with another Fast_Byte using bitwise OR.
        /// All bits that are set in either value become set.
        /// </summary>
        public IFastType<byte> Combine ( IFastType<byte> other ) {
            m_value = (byte)(m_value | other.Value);
            return this;
        }

        /// <summary>
        /// Counts the number of bits set to 1 using a fast bit‑hack algorithm.
        /// This avoids loops and provides excellent performance.
        /// </summary>
        public byte IsIt () {
			uint v = m_value;
            v = v - ((v >> 1) & 0x55);
            v = (v & 0x33) + ((v >> 2) & 0x33);

            return (byte)((v + (v >> 4)) & 0x0F);
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
            FixedVector<byte> _set = new FixedVector<byte>(IsIt());

            for ( byte i = 0 ; i < 8 ; i++ ) {
                if(Is(i) == 1) { _set.PushBack(i);  }
            }
            return _set;
        }

        /// <summary>
        /// Returns all bit positions where the bit is set to 0.
        /// </summary>
        public FixedVector<byte> WhereNot () {
            FixedVector<byte> _set = new FixedVector<byte>(IsItNot());

            for ( byte i = 0 ; i < 8 ; i++ ) {
                if ( Is(i) == 0 ) { _set.PushBack(i); }
            }
            return _set;
        }

		/// <summary>
		/// Implicitly converts a byte value into an <see cref="Fast_Byte"/>.
		/// </summary>
		/// <param name="value">The byte value to convert.</param>
		public static implicit operator Fast_Byte ( byte value ) {
			return new Fast_Byte(value);			
		}

		/// <summary>
		/// Implicitly converts a int value into an <see cref="Fast_Byte"/>.
		/// </summary>
		/// <param name="value">The int value to convert.</param>
		public static implicit operator Fast_Byte ( int value ) {
			return new Fast_Byte( (byte) value);
		}

		/// <summary>
		/// Implicitly converts a uint value into an <see cref="Fast_Byte"/>.
		/// </summary>
		/// <param name="value">The uint value to convert.</param>
		public static implicit operator Fast_Byte ( uint value ) {
			return new Fast_Byte((byte)value);
		}

		/// <summary>
		/// Explicitly extracts the underlying value from an <see cref="Fast_Byte"/>.
		/// </summary>
		/// <param name="opt">The optional instance to extract from.</param>
		/// <returns>The stored value.</returns>
		public static explicit operator byte ( Fast_Byte opt ) {
			return opt.m_value;
		}

		/// <summary>
		/// Explicitly extracts the underlying value from an <see cref="Fast_Byte"/>.
		/// </summary>
		/// <param name="opt">The optional instance to extract from.</param>
		/// <returns>The stored value.</returns>
		public static explicit operator int ( Fast_Byte opt ) {
			return opt.m_value;
		}

		/// <inheritdoc/>
		public override bool Equals (object? obj ) {
			if ( obj is Fast_Byte key )
				return Equals(key);
			return false;
		}

		/// <inheritdoc/>
		public bool Equals ( Fast_Byte b ) {
			return this.m_value == b.m_value;
		}

		public bool Equals ( byte b ) {
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
        /// <summary>
        /// Addition
        /// </summary>
        public Fast_Byte Add ( Fast_Byte oth ) { 
            m_value += oth.m_value; return this; 
        }
        /// <summary>
        /// Subtraktion
        /// </summary>
		public Fast_Byte Sub ( Fast_Byte oth ) {
			m_value -= oth.m_value; return this;
		}
        /// <summary>
        /// Multiplikation
        /// </summary>
		public Fast_Byte Mul ( Fast_Byte oth ) {
			m_value *= oth.m_value; return this;
		}
        /// <summary>
        /// Division
        /// </summary>
		public Pair<Fast_Byte, Fast_Byte> Div ( Fast_Byte oth ) {
			m_value /= oth.m_value; 
            byte v = (byte)(m_value % oth.m_value);

            return new(this, new(v)); 
		}

		public CompareResult CompareTo ( Fast_Byte a ) {
            if ( m_value > a.m_value ) return CompareResult.Greater;
            else if( m_value < a.m_value ) return CompareResult.Less;

            return CompareResult.Equal;
		}
		int IComparable<Fast_Byte>.CompareTo ( Fast_Byte a ) {
			return (int) CompareTo (a);
		}

        public static Fast_Byte Min ( Fast_Byte a, Fast_Byte b ) {
            return a.m_value < b.m_value ? a : b;
        }
		public static Fast_Byte Max ( Fast_Byte a, Fast_Byte b ) {
			return a.m_value > b.m_value ? a : b;
		}

		/// <inheritdoc/>
		public static bool operator == ( Fast_Byte a, Fast_Byte b ) {
			return a.m_value == b.m_value;
		}

		/// <inheritdoc/>
		public static bool operator != ( Fast_Byte a, Fast_Byte b ) {
			return !(a == b);
		}

		/// <inheritdoc/>
		public static bool operator <= ( Fast_Byte a, Fast_Byte b ) {
			return a.m_value <= b.m_value;
		}
		/// <inheritdoc/>
		public static bool operator >= ( Fast_Byte a, Fast_Byte b ) {
			return a.m_value >= b.m_value;
		}
		/// <inheritdoc/>
		public static bool operator < ( Fast_Byte a, Fast_Byte b ) {
			return a.m_value < b.m_value;
		}
		/// <inheritdoc/>
		public static bool operator > ( Fast_Byte a, Fast_Byte b ) {
			return a.m_value > b.m_value;
		}

		public static Fast_Byte operator + ( Fast_Byte a, Fast_Byte b ) {
			a.m_value += b.m_value;
			return a;
		}
		public static Fast_Byte operator - ( Fast_Byte a, Fast_Byte b ) {
			a.m_value -= b.m_value;
            return a;
		}
		public static Fast_Byte operator - ( Fast_Byte a ) {
			a.m_value = (byte)(-a.m_value);
			return a;
		}
		public static Fast_Byte operator -- ( Fast_Byte a) {
			a.m_value--;
			return a;
		}
		public static Fast_Byte operator ++ ( Fast_Byte a ) {
            a.m_value++;
            return a;
		}
		public static Fast_Byte operator * ( Fast_Byte a, Fast_Byte b ) {
			a.m_value *= b.m_value;
			return a;
		}
		public static Fast_Byte operator / ( Fast_Byte a, Fast_Byte b ) {
			a.m_value /= b.m_value;
			return a;
		}

		public static Fast_Byte operator & ( Fast_Byte a, Fast_Byte b ) {
			return (byte)(a.m_value & b.m_value);
		}
		public static Fast_Byte operator % ( Fast_Byte a, Fast_Byte b ) {
			return (byte)(a.m_value % b.m_value);
		}
		public static Fast_Byte operator | ( Fast_Byte a, Fast_Byte b ) {
			return (byte)(a.m_value | b.m_value);
		}
		public static Fast_Byte operator ^ ( Fast_Byte a, Fast_Byte b ) {
			return (byte)(a.m_value ^ b.m_value);
		}
		
		public static Fast_Byte operator << ( Fast_Byte a, Fast_Byte b ) {
			a.m_value = (byte)( a.m_value << b.m_value);
			return a;
		}
		public static Fast_Byte operator >> ( Fast_Byte a, Fast_Byte b ) {
			a.m_value = (byte)(a.m_value >> b.m_value);
			return a;
		}

	}

}
