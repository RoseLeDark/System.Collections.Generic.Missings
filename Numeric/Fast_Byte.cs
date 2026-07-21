using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using SystemEx.Collections.Generic;

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
    public struct Fast_Byte : IFastType<byte> {
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
            m_value = (byte)(m_value ^ (1 << pos)); 
        }

        /// <summary>
        /// Returns the bit at the specified position (0 or 1).
        /// </summary>
        public byte Is ( byte pos ) => (byte)((m_value >> pos) & 1);

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
        public Array<byte> Where () {
            Array<byte> _set = new Array<byte>(IsIt());

            for ( byte i = 0 ; i < 8 ; i++ ) {
                if(Is(i) == 1) { _set.PushBack(i);  }
            }
            return _set;
        }

        /// <summary>
        /// Returns all bit positions where the bit is set to 0.
        /// </summary>
        public Array<byte> WhereNot () {
            Array<byte> _set = new Array<byte>(IsItNot());

            for ( byte i = 0 ; i < 8 ; i++ ) {
                if ( Is(i) == 0 ) { _set.PushBack(i); }
            }
            return _set;
        }
    }
}
