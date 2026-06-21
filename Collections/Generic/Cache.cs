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

using System.Collections;
using SystemEx.Collections.Generic.Interfaces;
using SystemEx.Utils;

namespace SystemEx.Collections.Generic{
    /// <summary>
    /// Specifies the access mode of a <see cref="Cache"/> instance.
    /// </summary>
    public enum CacheType {
        /// <summary>
        /// Cache is intended for writing data to a device.
        /// </summary>
        ToDevice,

        /// <summary>
        /// Cache is intended for reading data from a device.
        /// </summary>
        FromDevice,

        /// <summary>
        /// Cache supports both reading and writing operations.
        /// </summary>
        Both
    }

    /// <summary>
    /// Exception thrown when an operation is attempted on a shared or locked cache.
    /// </summary>
    public class CacheIsSharedException : Exception {
        /// <summary>
        /// Exception thrown when an operation is attempted on a shared or locked cache.
        /// </summary>
        public CacheIsSharedException() : base() { }
    }
    /// <summary>
    /// Represents a low-level byte buffer with position tracking, 
    /// typed read/write helpers, and optional locking behavior.
    /// </summary>
    public class Cache : ICache {
        /// <summary>
        /// Internal raw byte buffer.
        /// </summary>
        private FixedArray<byte> m_rawBuffer;
        /// <summary>
        /// Current read/write position within the buffer.
        /// </summary>
        private ulong m_position;
        /// <summary>
        /// Indicates whether the cache is locked for reading or writing.
        /// </summary>
        private bool m_isLocked;
        /// <summary>
        /// Gets or sets the internal lock state.
        /// </summary>
        protected bool IsLocked { get { return m_isLocked; }  set => m_isLocked = value;  }
        /// <summary>
        /// Provides indexed access to the raw buffer.
        /// </summary>
        public byte this[int adress] {
            get {  return m_rawBuffer[adress]; }
            set {  m_rawBuffer[adress] = value; }
        }

        /// <summary>
        /// Gets the total buffer length in bytes.
        /// </summary>
        public virtual int Length => m_rawBuffer.Size;
        /// <summary>
        /// Gets the logical length of the cache as an unsigned 64-bit value.
        /// </summary>
        public virtual ulong LongLength { get; internal set; }
        /// <summary>
        /// Indicates whether the cache contains no data.
        /// </summary>
        public bool IsEmpty => LongLength == 0;
        /// <summary>
        /// Gets the configured cache type (read, write, or both).
        /// </summary>
        public CacheType Type { get; private set; }
        /// <summary>
        /// Indicates whether the cache is writable.
        /// </summary>
        public bool CanWrite { get => !m_isLocked;  }
        /// <summary>
        /// Indicates whether the cache is readable.
        /// </summary>
        public bool CanRead { get => !m_isLocked; }
        /// <summary>
        /// Gets the current read/write position.
        /// </summary>
        public ulong Position { get => m_position;  }
        /// <summary>
        /// Sets the internal position without validation.
        /// Intended for controlled use by higher-level abstractions.
        /// </summary>
        internal void SetSavePosition(ulong valu) { m_position = valu; }
        /// <summary>
        /// Initializes a new cache with the specified capacity and type.
        /// </summary>
        public Cache(int capacity, CacheType type) {
            m_rawBuffer = new FixedArray<byte>(capacity);
            this.Type = type;
            LongLength = (ulong)m_rawBuffer.Size;
        }
        /// <summary>
        /// Initializes a new cache from given array
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="type"></param>
        public Cache(byte[] arr, CacheType type) {
            m_rawBuffer = new FixedArray<byte>(arr);
            this.Type = type;
            LongLength = (ulong)m_rawBuffer.Size;
        }

        /// <summary>
        /// Initializes a new cache from given array
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="type"></param>
        public Cache(Array<byte> arr, CacheType type) {
            m_rawBuffer = new FixedArray<byte>(arr.ToArray());
            this.Type = type;
            LongLength = (ulong)m_rawBuffer.Size;
        }
        /// <summary>
        /// Moves the internal position according to the specified origin and offset.
        /// </summary>
        /// <param name="org">The seek origin.</param>
        /// <param name="pos">The offset relative to the origin.</param>
        /// <returns>The new position.</returns>
        public ulong Seek(SeekOrigin org, int pos) {
            if ( m_rawBuffer.Size == 0 ) return 0;

            switch ( org ) {

            case SeekOrigin.Begin:
                m_position = (ulong)System.Math.Max(pos, 0);
                break;
            case SeekOrigin.Current:
                if ( pos >= 0 )
                    m_position += (ulong)pos;
                else {
                    ulong neg = (ulong)(-pos);
                    m_position = (m_position > neg) ? (m_position - neg) : 0;
                }
                break;
            case SeekOrigin.End:
                if ( pos >= 0 ) {
                    ulong p = (ulong)pos;
                    m_position = (p > LongLength) ? 0 : (LongLength - p);
                } else {
                    // End + negative pos = End - (-pos) = End + pos
                    ulong add = (ulong)(-pos);
                    m_position = LongLength + add; // wird unten gekappt
                }
                break;
            }

            if ( m_position > LongLength )
                m_position = LongLength;

            return m_position;
        }

        /// <summary>
        /// Writes a byte range into the cache starting at the specified position.
        /// </summary>
        public virtual ulong WriteRange(ulong position, byte[] data) {

            return WriteRange(position, (ulong)data.LongLength, data);
        }
        /// <summary>
        /// Writes a byte range into the cache between <paramref name="start"/> and <paramref name="iend"/>.
        /// </summary>
        public virtual ulong WriteRange(ulong start, ulong iend, byte[] data) {
            if ( m_isLocked )
                throw new InvalidOperationException("is Locked");

            // Start ungültig?
            if ( start < 0 || start >= (ulong)m_rawBuffer.Size )
                return 0;

            // End über Größe → kappen
            if ( iend > (ulong)m_rawBuffer.Size )
                iend = (ulong)m_rawBuffer.Size;

            // Bereich ungültig?
            if ( iend <= start )
                return 0;

            ulong rangeLen = iend - start;
            ulong writable = System.Math.Min((uint)rangeLen, (uint)data.Length);

            for ( ulong i = 0; i < (ulong)writable; i++ )
                m_rawBuffer[(int)(start + i)] = data[i];

            return (ulong)writable;
        }

        /// <summary>
        /// Converts an unmanaged value to its raw byte representation.
        /// </summary>
        public static unsafe byte[] ToBytes<T>(ref T value) where T : unmanaged {
            
            int size = sizeof(T);
            byte[] data = new byte[size];

            fixed ( T* pValue = &value )
            fixed ( byte* pData = data ) {
                Buffer.MemoryCopy(pValue, pData, size, size);
            }

            return data;
        }



        // ------------------------------------------------------------
        // Typed Write Methods
        // ------------------------------------------------------------

        /// <summary>
        /// Writes a 32-bit unsigned integer at the specified position.
        /// </summary>
        public int Write(ulong position, uint value, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = value.ToBytes(endian);

            return (int)WriteRange(position, b);
        }
        /// <summary>
        /// Writes a 32-bit signed integer at the specified position.
        /// </summary>
        public int Write(ulong position, int value, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = value.ToBytes(endian);

            return (int)WriteRange(position, b);
        }
        /// <summary>
        /// Writes a 16-bit signed short at the specified position.
        /// </summary>
        public int Write(ulong position, short value, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = value.ToBytes(endian);

            return (int)WriteRange(position, b);
        }
        /// <summary>
        /// Writes a character at the specified position.
        /// </summary>
        public int Write(ulong position, char value) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = value.ToBytes(Endian.LittleEndian);

            return (int)WriteRange(position, b);
        }
        /// <summary>
        /// Writes a byte at the specified position.
        /// </summary>
        public int Write(ulong position, byte value) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = value.ToBytes(Endian.LittleEndian);

            return (int)WriteRange(position, b);
        }
        /// <summary>
        /// Writes a 16-bit unsigned short at the specified position.
        /// </summary>
        public int Write(ulong position, ushort value, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = value.ToBytes(endian);

            return (int)WriteRange(position, b);
        }
        /// <summary>
        /// Writes a 64-bit signed long at the specified position.
        /// </summary>
        public int Write(ulong position, long value, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = value.ToBytes(endian);

            return (int)WriteRange(position, b);
        }
        /// <summary>
        /// Writes a 64-bit unsigned long at the specified position.
        /// </summary>
        public int Write(ulong position, ulong value, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = value.ToBytes(endian);

            return (int)WriteRange(position, b);
        }
        /// <summary>
        /// Writes a single-precision floating-point value at the specified position.
        /// </summary>
        public int Write(ulong position, float value, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = value.ToBytes(endian);

            return (int)WriteRange(position, b);
        }
        /// <summary>
        /// Writes a double-precision floating-point value at the specified position.
        /// </summary>
        public int Write(ulong position, double value, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = value.ToBytes(endian);

            return (int)WriteRange(position, b);
        }
        // ------------------------------------------------------------
        // Typed Read Methods
        // ------------------------------------------------------------

        /// <summary>
        /// Reads a 32-bit unsigned integer from the specified position.
        /// </summary>
        public uint ReadUInt(ulong position, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = new byte[4];

            b[0] = m_rawBuffer[(int)position + 0];
            b[1] = m_rawBuffer[(int)position + 1];
            b[2] = m_rawBuffer[(int)position + 2];
            b[3] = m_rawBuffer[(int)position + 3];

            return b.ToUInt(endian);
        }
        /// <summary>
        /// Reads a 32-bit signed integer from the specified position.
        /// </summary>
        public int ReadInt(ulong position, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = new byte[4];
            int _pos = (int)position;

            b[0] = m_rawBuffer[_pos + 0];
            b[1] = m_rawBuffer[_pos + 1];
            b[2] = m_rawBuffer[_pos + 2];
            b[3] = m_rawBuffer[_pos + 3];

            return b.ToInt(endian);
        }
        /// <summary>
        /// Reads a 16-bit signed short from the specified position.
        /// </summary>
        public short ReadShort(ulong position, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = new byte[2];
            int _pos = (int)position;

            b[0] = m_rawBuffer[_pos + 0];
            b[1] = m_rawBuffer[_pos + 1];

            return b.ToShort(endian);
        }

        /// <summary>
        /// Reads a 16-bit unsigned short from the specified position.
        /// </summary>
        public ushort ReadUShort(ulong position, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = new byte[2];
            int _pos = (int)position;

            b[0] = m_rawBuffer[_pos + 0];
            b[1] = m_rawBuffer[_pos + 1];

            return b.ToUShort(endian);
        }
        /// <summary>
        /// Reads a 64-bit signed long from the specified position.
        /// </summary>
        public long ReadLong(ulong position, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = new byte[8];
            int _pos = (int)position;

            b[0] = m_rawBuffer[_pos + 0];
            b[1] = m_rawBuffer[_pos + 1];
            b[2] = m_rawBuffer[_pos + 2];
            b[3] = m_rawBuffer[_pos + 3];
            b[4] = m_rawBuffer[_pos + 4];
            b[5] = m_rawBuffer[_pos + 5];
            b[6] = m_rawBuffer[_pos + 6];
            b[7] = m_rawBuffer[_pos + 7];

            return b.ToLong(endian);
        }
        /// <summary>
        /// Reads a 64-bit unsigned long from the specified position.
        /// </summary>
        public ulong ReadULong(ulong position, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = new byte[8];
            int _pos = (int)position;

            b[0] = m_rawBuffer[_pos + 0];
            b[1] = m_rawBuffer[_pos + 1];
            b[2] = m_rawBuffer[_pos + 2];
            b[3] = m_rawBuffer[_pos + 3];
            b[4] = m_rawBuffer[_pos + 4];
            b[5] = m_rawBuffer[_pos + 5];
            b[6] = m_rawBuffer[_pos + 6];
            b[7] = m_rawBuffer[_pos + 7];

            return b.ToULong(endian);
        }
        /// <summary>
        /// Reads a single character from the specified position.
        /// </summary>
        public char ReadChar(ulong position) {
            byte b = m_rawBuffer[(int)position];
            return (char)b;
        }
        /// <summary>
        /// Reads a single-precision floating-point value from the specified position.
        /// </summary>
        public float ReadFloat(ulong position, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = new byte[4];

            b[0] = m_rawBuffer[(int)position + 0];
            b[1] = m_rawBuffer[(int)position + 1];
            b[2] = m_rawBuffer[(int)position + 2];
            b[3] = m_rawBuffer[(int)position + 3];

            return b.ToFloat(endian);
        }
        /// <summary>
        /// Reads a double-precision floating-point value from the specified position.
        /// </summary>
        public double ReadDouble(ulong position, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = new byte[8];

            b[0] = m_rawBuffer[(int)position + 0];
            b[1] = m_rawBuffer[(int)position + 1];
            b[2] = m_rawBuffer[(int)position + 2];
            b[3] = m_rawBuffer[(int)position + 3];
            b[4] = m_rawBuffer[(int)position + 4];
            b[5] = m_rawBuffer[(int)position + 5];
            b[6] = m_rawBuffer[(int)position + 6];
            b[7] = m_rawBuffer[(int)position + 7];

            return b.ToDouble(endian);
        }
        /// <summary>
        /// Writes a single byte at the current position and advances the position.
        /// </summary>
        public int Write(byte data) {
            var written = WriteRange(m_position, m_position + 1, new byte[1] { data } );
            m_position += written;
            return (int)written;
        }
        /// <summary>
        /// Reads a single byte from the specified position.
        /// </summary>
        public byte Read(ulong position) {
            return m_rawBuffer[(int)position];
        }
        /// <summary>
        /// Reads a range of bytes starting at the specified position.
        /// </summary>
        public virtual byte[]? ReadRange(ulong position, uint count) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            if ( (int)position + count > m_rawBuffer.Size ) return null;

            byte[] result = new byte[count];
            m_rawBuffer.CopyTo((uint)position, result, 0, (uint)count);
            return result;
        }
        /// <summary>
        /// Reads bytes into the specified buffer and advances the internal position.
        /// </summary>
        public int Read(byte[] buffer, int offset, int count) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");
            if ( offset < 0 || count < 0 ) throw new ArgumentOutOfRangeException("offset,count");
            ArgumentNullException.ThrowIfNull(buffer);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(offset, buffer.Length);

            if ( offset + count > buffer.Length ) {
                // Wenn der Caller mehr verlangt als im Zielbuffer Platz ist,
                // lesen wir nur so viel, wie in den Zielbuffer passt.
                count = buffer.Length - offset;
            }

            // Wenn die Leseposition bereits am oder jenseits des Endes ist: EOF
            if ( (ulong)m_rawBuffer.Size <= m_position ) return 0;

            // Verfügbare Bytes im Rohpuffer ab aktueller Position
            ulong available = (ulong)m_rawBuffer.Size - m_position;
            int toRead = (int)System.Math.Min((ulong)count, available);

            if ( toRead <= 0 ) return 0;

            // ReadRange erwartet position als ulong und count als uint
            byte[]? chunk = ReadRange(m_position, (uint)toRead);
            if ( chunk == null || chunk.Length == 0 ) return 0;

            Array.Copy(chunk, 0, buffer, offset, chunk.Length);

            // interne Position vorziehen
            m_position += (ulong)chunk.Length;

            return chunk.Length;
        }

        /// <summary>
        /// Writes bytes from the specified buffer and advances the internal position.
        /// </summary>
        /// <param name="buffer">The byte buffer to write</param>
        /// <param name="offset"></param>
        /// <param name="count">Size of the buffer to write</param>
        /// <returns>NUmber of bytes are written</returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public int Write(byte[] buffer, int offset, int count) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");
            if ( offset < 0 || count < 0 ) throw new ArgumentOutOfRangeException("offset,count");

            ArgumentNullException.ThrowIfNull(buffer);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(offset, buffer.Length);
            


            // Adjust count if caller asked for more than fits in the target buffer
            if ( offset + count > buffer.Length ) {
                count = buffer.Length - offset;
            }

            // If write position is already at or beyond end, nothing to write
            if ( (ulong)m_rawBuffer.Size <= m_position ) return 0;

            // How many bytes are available in the raw buffer from current position
            ulong available = (ulong)m_rawBuffer.Size - m_position;
            int toWrite = (int)System.Math.Min((ulong)count, available);

            if ( toWrite <= 0 ) return 0;

            for ( int i = 0; i < toWrite; i++ ) {
                m_rawBuffer[(int)(m_position + (ulong)i)] = buffer[offset + i];
            }

            // advance internal position
            m_position += (ulong)toWrite;

            return toWrite;
        }
        /// <summary>
        /// Returns a copy of the internal buffer.
        /// </summary>
        public byte[] ToArray() {
            if ( m_isLocked ) throw new CacheIsSharedException();
            return m_rawBuffer.ToArray();
        }
        /// <summary>
        /// Returns a copy of the internal buffer, as <see cref="Array{T}"/> 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="CacheIsSharedException"></exception>
        public Array<byte> ToArrayEx() {
            if ( m_isLocked ) throw new CacheIsSharedException();
            return m_rawBuffer;
        }
        
    }
}
