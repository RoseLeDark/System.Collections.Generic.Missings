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

using SystemEx.Base;
using SystemEx.Utils;

namespace SystemEx.Collections.Generic {
	/// \addtogroup Collections
	/// @{

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
        Both,
        /// <summary>
        /// Cache ist only on host, don't used with a device
        /// </summary>
        OnlySystem,
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
    public class Cache : ICache, IValueReader, IValueWriter {
        private ulong m_maxUsedAdress;
        /// <summary>
        /// Internal raw byte buffer.
        /// </summary>
        private FixedVector<byte> m_rawBuffer;
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
        public virtual ulong Length => (ulong)m_rawBuffer.Length;
        /// <summary>
        /// Gets the total free bytes.
        /// </summary>
        public virtual ulong Free => Length - m_maxUsedAdress;
        /// <summary>
        /// Gets the total used bytes.
        /// </summary>
        public virtual ulong Used => Length - Free;

        /// <summary>
        /// Indicates whether the cache contains no data.
        /// </summary>
        public bool IsEmpty => Length == 0;
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
        /// Gets and Set the current read/write position.
        /// </summary>
        public ulong Position { get => m_position; set => m_position = value; }
        /// <summary>
        /// Sets the internal position without validation.
        /// Intended for controlled use by higher-level abstractions.
        /// </summary>
        internal void SetSavePosition(ulong valu) { m_position = valu; }
        /// <summary>
        /// Initializes a new cache with the specified capacity and type.
        /// </summary>
        public Cache(long capacity, CacheType type = CacheType.OnlySystem) {
            m_rawBuffer = new FixedVector<byte>(capacity);
            this.Type = type;
        }

		/// <summary>
		/// Initializes a new cache from given array
		/// </summary>
		/// <param name="arr"></param>
		/// <param name="type"></param>
		public Cache ( byte[] arr, CacheType type ) {
			m_rawBuffer = new FixedVector<byte>(arr);
			this.Type = type;
		}

		/// <summary>
		/// Initializes a new cache from given array
		/// </summary>
		/// <param name="arr"></param>
		/// <param name="type"></param>
		public Cache ( FixedVector<byte> arr, CacheType type ) {
			if ( m_isLocked ) throw new InvalidOperationException("is Locked");

			m_rawBuffer = new FixedVector<byte>(arr.ToNative());
			this.Type = type;
		}
		/// <summary>
		/// Create a Copy from stream
		/// </summary>
		/// <param name="cache">The orignal cache</param>
		/// <exception cref="InvalidOperationException"></exception>
		public Cache ( Cache cache ) {
			if ( m_isLocked ) throw new InvalidOperationException("is Locked");

			m_rawBuffer = new FixedVector<byte>(cache.ToArray());
			this.Type = cache.Type;
		}


		/// <summary>
		/// Set Cache to Zero
		/// </summary>
		public void SetZero() {
            for ( int i = 0; i < m_rawBuffer.Length; i++ )
                m_rawBuffer[i] = 0;
            m_maxUsedAdress = 0;
        }
        /// <summary>
        /// See SetZero
        /// </summary>
        public void Clear () => SetZero();

        
        /// <summary>
        /// Moves the internal position according to the specified origin and offset.
        /// </summary>
        /// <param name="org">The seek origin.</param>
        /// <param name="pos">The offset relative to the origin.</param>
        /// <returns>The new position.</returns>
        public ulong Seek(SeekOrigin org, int pos) {
            if ( m_rawBuffer.Length == 0 ) return 0;

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
                    m_position = (p > Length) ? 0 : (Length - p);
                } else {
                    // End + negative pos = End - (-pos) = End + pos
                    ulong add = (ulong)(-pos);
                    m_position = Length + add; // wird unten gekappt
                }
                break;
            }

            if ( m_position > Length )
                m_position = Length;

            return m_position;
        }

        /// <summary>
        /// Writes a byte range into the cache starting at the specified position.
        /// </summary>
        public virtual ulong WriteRange(ulong position, byte[] data) {
            return WriteRange(position, (ulong)data.LongLength, data);
        }

		public ulong WriteRange ( ulong position, FixedVector<byte> data ) {
			return WriteRange(position, (ulong)data.Length, data.ToNative() );
		}

		/// <summary>
		/// Writes a byte range into the cache between <paramref name="start"/> and <paramref name="iend"/>.
		/// </summary>
		public virtual ulong WriteRange(ulong start, ulong iend, byte[] data) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            // Start ungültig?
            if ( start < 0 || start >= (ulong)m_rawBuffer.Length )
                return 0;

            // End über Größe → kappen
            if ( iend > (ulong)m_rawBuffer.Length )
                iend = (ulong)m_rawBuffer.Length;

            // Bereich ungültig?
            if ( iend <= start )
                return 0;

            ulong rangeLen = iend - start;
            ulong writable = System.Math.Min((uint)rangeLen, (uint)data.Length);

            for ( ulong i = 0; i < (ulong)writable; i++ )
                m_rawBuffer[(int)(start + i)] = data[i];

            if ( (start + writable) > m_maxUsedAdress ) m_maxUsedAdress = start + writable;

            return (ulong)writable;
        }

		/// <summary>
		/// Reads bytes into the specified buffer and advances the internal position.
		/// </summary>
		public int Read ( byte[] buffer, int offset, int count ) {
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
			if ( (ulong)m_rawBuffer.Length <= m_position ) return 0;

			// Verfügbare Bytes im Rohpuffer ab aktueller Position
			ulong available = (ulong)m_rawBuffer.Length - m_position;
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
		/// Reads a single byte from the specified position.
		/// </summary>
		public byte ReadByte ( ulong position ) {
			return m_rawBuffer[(long)position];
		}
		/// <summary>
		/// Reads a range of bytes starting at the specified position.
		/// </summary>
		public virtual byte[]? ReadRange ( ulong position, uint count ) {
			var readed = ReadRangeEx(position, count);

			return readed.ToNative();
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
		public int Write ( byte[] buffer, int offset, int count ) {
			if ( m_isLocked ) throw new InvalidOperationException("is Locked");
			if ( offset < 0 || count < 0 ) throw new ArgumentOutOfRangeException("offset,count");

			ArgumentNullException.ThrowIfNull(buffer);
			ArgumentOutOfRangeException.ThrowIfGreaterThan(offset, buffer.Length);



			// Adjust count if caller asked for more than fits in the target buffer
			if ( offset + count > buffer.Length ) {
				count = buffer.Length - offset;
			}

			// If write position is already at or beyond end, nothing to write
			if ( (ulong)m_rawBuffer.Length <= m_position ) return 0;

			// How many bytes are available in the raw buffer from current position
			ulong available = (ulong)m_rawBuffer.Length - m_position;
			int toWrite = (int)System.Math.Min((ulong)count, available);

			if ( toWrite <= 0 ) return 0;

			for ( int i = 0 ; i < toWrite ; i++ ) {
				m_rawBuffer[(int)(m_position + (ulong)i)] = buffer[offset + i];
			}

			// advance internal position
			m_position += (ulong)toWrite;

			if ( m_position > m_maxUsedAdress ) m_maxUsedAdress = m_position;

			return toWrite;
		}

		#region IValueWriter
		/// <summary>
		/// Writes a typed value at the specified position using endian‑aware
		/// conversion and exclusive RCU semantics.
		/// </summary>
		public void Write ( ulong position, uint value, Endian endian ) {
			byte[] b = value.ToBytes(endian);
			WriteRange(position, b);
		}
		/// <inheritdoc/>
		public void Write ( uint value, Endian endian ) {
			byte[] b = value.ToBytes(endian);
			WriteRange(Position, b);
		}

		/// <summary>
		/// Writes a typed value at the specified position using endian‑aware
		/// conversion and exclusive RCU semantics.
		/// </summary>
		public void Write ( ulong position, int value, Endian endian ) {
			byte[] b = value.ToBytes(endian);
			WriteRange(position, b);
		}
		/// <inheritdoc/>
		public void Write ( int value, Endian endian ) {
			byte[] b = value.ToBytes(endian);
			WriteRange(Position, b);
		}

		/// <summary>
		/// Writes a typed value at the specified position using endian‑aware
		/// conversion and exclusive RCU semantics.
		/// </summary>
		public void Write ( ulong position, short value, Endian endian ) {
			byte[] b = value.ToBytes(endian);
			WriteRange(position, b);
		}
		/// <inheritdoc/>
		public void Write ( short value, Endian endian ) {
			byte[] b = value.ToBytes(endian);
			WriteRange(Position, b);
		}

		/// <summary>
		/// Writes a typed value at the specified position using endian‑aware
		/// conversion and exclusive RCU semantics.
		/// </summary>
		public void WriteByte ( byte value ) {
			byte[] b = value.ToBytes(Endian.LittleEndian);
			WriteRange(Position, b);
		}

		/// <summary>
		/// Writes a typed value at the specified position using endian‑aware
		/// conversion and exclusive RCU semantics.
		/// </summary>
		public void Write ( ulong position, ushort value, Endian endian ) {
			byte[] b = value.ToBytes(endian);
			WriteRange(position, b);
		}
		/// <inheritdoc/>
		public void Write ( ushort value, Endian endian ) {
			byte[] b = value.ToBytes(endian);
			WriteRange(Position, b);
		}

		/// <summary>
		/// Writes a typed value at the specified position using endian‑aware
		/// conversion and exclusive RCU semantics.
		/// </summary>
		public void Write ( ulong position, long value, Endian endian ) {
			byte[] b = value.ToBytes(endian);
			WriteRange(position, b);
		}
		/// <inheritdoc/>
		public void Write ( long value, Endian endian ) {
			byte[] b = value.ToBytes(endian);
			WriteRange(Position, b);
		}

		/// <summary>
		/// Writes a typed value at the specified position using endian‑aware
		/// conversion and exclusive RCU semantics.
		/// </summary>
		public void Write ( ulong position, ulong value, Endian endian ) {
			byte[] b = value.ToBytes(endian);
			WriteRange(position, b);
		}
		/// <inheritdoc/>
		public void Write ( ulong value, Endian endian ) {
			byte[] b = value.ToBytes(endian);
			WriteRange(Position, b);
		}

		/// <summary>
		/// Writes a typed value at the specified position using endian‑aware
		/// conversion and exclusive RCU semantics.
		/// </summary>
		public void Write ( ulong position, float value, Endian endian ) {
			byte[] b = value.ToBytes(endian);
			WriteRange(position, b);
		}
		/// <inheritdoc/>
		public void Write ( float value, Endian endian ) {
			byte[] b = value.ToBytes(endian);
			WriteRange(Position, b);
		}

		/// <summary>
		/// Writes a typed value at the specified position using endian‑aware
		/// conversion and exclusive RCU semantics.
		/// </summary>
		public void Write ( ulong position, double value, Endian endian ) {
			byte[] b = value.ToBytes(endian);
			WriteRange(position, b);
		}
		/// <inheritdoc/>
		public void Write ( double value, Endian endian ) {
			byte[] b = value.ToBytes(endian);
			WriteRange(Position, b);
		}
		#endregion


		#region IValueReader

		/// <inheritdoc/>
		public uint ReadUInt ( Endian endian, uint ervlue = 0 ) {
			byte _size = sizeof(uint);
			byte[] buffer = new byte[_size];

			if ( Read(buffer, 0, _size) == _size ) {
				return buffer.ToUInt(endian);
			}
			return ervlue;
		}
		/// <inheritdoc/>
		public int ReadInt ( Endian endian, int ervlue = 0 ) {
			byte _size = sizeof(int);
			byte[] buffer = new byte[_size];

			if ( Read(buffer, 0, _size) == _size ) {
				return buffer.ToInt(endian);
			}
			return ervlue;
		}
		/// <inheritdoc/>
		public short ReadShort ( Endian endian, short ervlue = 0 ) {
			byte _size = sizeof(short);
			byte[] buffer = new byte[_size];

			if ( Read(buffer, 0, _size) == _size ) {
				return buffer.ToShort(endian);
			}
			return ervlue;
		}
		/// <inheritdoc/>
		public ushort ReadUShort ( Endian endian, ushort ervlue = 0 ) {
			byte _size = sizeof(ushort);
			byte[] buffer = new byte[_size];

			if ( Read(buffer, 0, _size) == _size ) {
				return buffer.ToUShort(endian);
			}
			return ervlue;
		}
		/// <inheritdoc/>
		public long ReadLong ( Endian endian, long ervlue = 0 ) {
			byte _size = sizeof(long);

			byte[] buffer = new byte[_size];

			if ( Read(buffer, 0, _size) == _size ) {
				return buffer.ToLong(endian);
			}
			return ervlue;
		}
		/// <inheritdoc/>
		public ulong ReadULong ( Endian endian, ulong ervlue = 0 ) {
			byte _size = sizeof(ulong);

			byte[] buffer = new byte[_size];

			if ( Read(buffer, 0, _size) == _size ) {
				return buffer.ToULong(endian);
			}
			return ervlue;
		}
		/// <inheritdoc/>
		public float ReadFloat ( Endian endian, float ervlue = 0 ) {
			byte _size = sizeof(float);

			byte[] buffer = new byte[_size];

			if ( Read(buffer, 0, _size) == _size ) {
				return buffer.ToFloat(endian);
			}
			return ervlue;
		}

		/// <inheritdoc/>
		public double ReadDouble ( Endian endian, double ervlue = 0 ) {
			byte _size = sizeof(double);

			byte[] buffer = new byte[_size];

			if ( Read(buffer, 0, _size) == _size ) {
				return buffer.ToDouble(endian);
			}
			return ervlue;
		}
		#endregion



		/// <summary>
		/// Reads a range of bytes starting at the specified position.
		/// </summary>
		public virtual FixedVector<byte> ReadRangeEx ( ulong position, uint count ) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            if ( (int)position + count > m_rawBuffer.Length ) throw new IndexOutOfRangeException();

            FixedVector<byte> result = new FixedVector<byte>(count);
            m_rawBuffer.CopyTo((uint)position, result, 0, (uint)count);
            return result;
        }
        
        /// <summary>
        /// Returns a copy of the internal buffer.
        /// </summary>
        public byte[] ToArray() {
            if ( m_isLocked ) throw new CacheIsSharedException();
            return m_rawBuffer.ToNative();
        }
        /// <summary>
        /// Returns a copy of the internal buffer, as <see cref="FixedVector{T}"/> 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="CacheIsSharedException"></exception>
        public FixedVector<byte> ToArrayEx() {
            if ( m_isLocked ) throw new CacheIsSharedException();
            return (FixedVector<byte>)m_rawBuffer.Duplicate();
        }

        /// <summary>
        /// Creates a FlexSpan view over the valid portion of this array.
        /// The span does not copy data; it directly references the internal buffer.
        /// </summary>
        /// <param name="mode">
        /// The indexing mode of the span (System, Reverse, Ring).
        /// </param>
        /// <returns>
        /// A FlexSpan that views the range [0 .. Ende).
        /// </returns>
        public virtual VectorFlexSpan<byte, FixedVector<byte>> AsFlexSpan ( FlexSpanMode mode = FlexSpanMode.System )
            => FixedVector<byte>.AsFlexSpan(ref m_rawBuffer, mode);


        /// <summary>
        /// Creates a FlexSpan view starting at the specified offset.
        /// The span references the internal buffer directly and does not allocate.
        /// </summary>
        public virtual VectorFlexSpan<byte, FixedVector<byte> > AsFlexSpan ( long start, FlexSpanMode mode = FlexSpanMode.System )
            => FixedVector<byte>.AsFlexSpan(ref m_rawBuffer, start, m_rawBuffer.Length, mode);

        /// <summary>
        /// Creates a FlexSpan view starting at the specified offset.
        /// The span references the internal buffer directly and does not allocate.
        /// </summary>
        public virtual VectorFlexSpan<byte, FixedVector<byte>> AsFlexSpan ( long start, long endi, FlexSpanMode mode = FlexSpanMode.System )
            => FixedVector<byte>.AsFlexSpan(ref m_rawBuffer, start, endi, mode);

		

#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
		/// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
	}
}
