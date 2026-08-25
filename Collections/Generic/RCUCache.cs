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
using SystemEx.Threading;

namespace SystemEx.Collections.Generic {
	/// \addtogroup Collections
	/// @{

	/// <summary>
	/// Provides a Read‑Copy‑Update (RCU) wrapper around a <see cref="Cache"/> instance.
	/// Readers operate lock‑free using epoch tracking, while writers acquire an
	/// exclusive lock to ensure consistent updates. This enables deterministic,
	/// high‑performance concurrent access to a byte‑addressable memory buffer.
	/// </summary>
	public class RCUCache : ICache, IValueReader, IValueWriter {
		private Cache m_origCache;
		private Epoch m_epochReads;
		private readonly ILock m_writerLock;

		/// <summary>
		/// Provides RCU‑safe indexed access to the underlying cache. Read operations
		/// enter an epoch and are lock‑free; write operations acquire the writer lock
		/// and ensure that no readers are active.
		/// </summary>
		public byte this[int adress] {
			get {
				using ( var _le = new UniqueEpoch(m_epochReads) ) {
					return m_origCache[adress];
				}
			}
			set {
				using ( var lk = new UniqueLock<ILock>(m_writerLock) ) {
					if ( m_epochReads )
						throw new InvalidOperationException("Readers active");

					m_origCache[adress] = value;
				}
			}
		}

		/// <summary>
		/// Gets the total buffer length in bytes.
		/// </summary>
		public virtual ulong Length {
			get {
				using ( var _le = new UniqueEpoch(m_epochReads) ) {
					return m_origCache.Length;
				}
			}
		}
		/// <summary>
		/// Gets the total free bytes.
		/// </summary>
		public virtual ulong Free {
			get {
				using ( var _le = new UniqueEpoch(m_epochReads) ) {
					return m_origCache.Free;
				}
			}
		}
		/// <summary>
		/// Gets the total used bytes.
		/// </summary>
		public virtual ulong Used {
			get {
				using ( var _le = new UniqueEpoch(m_epochReads) ) {
					return m_origCache.Used;
				}
			}
		}

		/// <summary>
		/// Indicates whether the cache contains no data.
		/// </summary>
		public bool IsEmpty {
			get {
				using ( var _le = new UniqueEpoch(m_epochReads) ) {
					return m_origCache.IsEmpty;
				}
			}
		}
		/// <summary>
		/// Gets the configured cache type (read, write, or both).
		/// </summary>
		public CacheType Type {
			get {
				using ( var _le = new UniqueEpoch(m_epochReads) ) {
					return m_origCache.Type;
				}
			}
		}
		/// <summary>
		/// Indicates whether the cache is writable.
		/// </summary>
		public bool CanWrite {
			get {
				return m_epochReads.CanWrite && m_origCache.CanWrite;
			}
		}
		/// <summary>
		/// Indicates whether the cache is readable.
		/// </summary>
		public bool CanRead {
			get {
				using ( var _le = new UniqueEpoch(m_epochReads) ) {
					return m_origCache.CanRead;
				}
			}
		}
		/// <summary>
		/// Gets and Set the current read/write position.
		/// </summary>
		public ulong Position {
			get {
				using ( var _le = new UniqueEpoch(m_epochReads) ) {
					return m_origCache.Position;
				}
			}
			set {
				using ( var lk = new UniqueLock<ILock>(m_writerLock) ) {
					if ( m_epochReads )
						throw new InvalidOperationException("Readers active");

					m_origCache.Position = value;
				}
			}
		}
		/// <summary>
		/// Initializes a new <see cref="RCUCache"/> with the specified capacity and
		/// cache type. A lightweight lock is created automatically for writer
		/// synchronization.
		/// </summary>
		public RCUCache ( long capacity, CacheType type = CacheType.OnlySystem ) {
			m_origCache = new Cache(capacity, type);
			m_epochReads = new Epoch();
			m_writerLock = new LightLock();
		}
		/// <summary>
		/// Initializes a new <see cref="RCUCache"/> with the specified capacity,
		/// writer lock, and cache type. Readers operate lock‑free using epoch
		/// tracking; writers use the provided lock.
		/// </summary>
		public RCUCache ( long capacity, ILock locker, CacheType type = CacheType.OnlySystem ) {
			m_origCache = new Cache(capacity, type);
			m_epochReads = new Epoch();
			m_writerLock = locker;
		}

		/// <summary>
		/// Initializes a new <see cref="RCUCache"/> from a byte array. The underlying
		/// cache is wrapped with RCU semantics using the specified writer lock.
		/// </summary>
		public RCUCache ( byte[] arr, ILock locker, CacheType type ) {
			m_origCache = new Cache(arr, type);
			m_epochReads = new Epoch();
			m_writerLock = locker;
		}
		/// <summary>
		/// Initializes a new <see cref="RCUCache"/> from a <see cref="FixedVector{byte}"/>.
		/// The underlying cache is wrapped with RCU semantics using the specified
		/// writer lock.
		/// </summary>

		public RCUCache ( FixedVector<byte> arr, ILock locker, CacheType type ) {
			m_origCache = new Cache(arr, type);
			m_epochReads = new Epoch();
			m_writerLock = locker;
		}
		/// <summary>
		/// Initializes a new <see cref="RCUCache"/> by copying an existing cache.
		/// The new instance uses RCU semantics with the specified writer lock.
		/// </summary>

		public RCUCache ( Cache cache, ILock locker ) {
			m_origCache = new Cache(cache);
			m_epochReads = new Epoch();
			m_writerLock = locker;
		}


		/// <summary>
		/// Set Cache to Zero
		/// </summary>
		public void SetZero () {
			using ( var lk = new UniqueLock<ILock>(m_writerLock) ) {
				if ( m_epochReads )
					throw new InvalidOperationException("Readers active");

				m_origCache.SetZero();
			}
		}
		/// <summary>
		/// See SetZero
		/// </summary>
		public void Clear () {
			using ( var lk = new UniqueLock<ILock>(m_writerLock) ) {
				if ( m_epochReads )
					throw new InvalidOperationException("Readers active");

				m_origCache.SetZero();
			}
		}
		/// <summary>
		/// Moves the internal position according to the specified origin and offset
		/// using exclusive RCU semantics. Writers acquire the lock and ensure that
		/// no readers are active during the operation.
		/// </summary>

		public ulong Seek ( SeekOrigin org, int pos ) {
			using ( var lk = new UniqueLock<ILock>(m_writerLock) ) {
				if ( m_epochReads )
					throw new InvalidOperationException("Readers active");

				return m_origCache.Seek(org, pos);
			}
		}
		/// <summary>
		/// Writes a byte range into the underlying cache using exclusive RCU semantics.
		/// Writers acquire the lock and ensure that no readers are active.
		/// </summary>

		public virtual ulong WriteRange ( ulong start, ulong iend, byte[] data ) {
			using ( var lk = new UniqueLock<ILock>(m_writerLock) ) {
				if ( m_epochReads )
					throw new InvalidOperationException("Readers active");

				return m_origCache.WriteRange(start, iend, data);
			}
		}
		/// <summary>
		/// Writes a byte range into the underlying cache using exclusive RCU semantics.
		/// Writers acquire the lock and ensure that no readers are active.
		/// </summary>

		public virtual ulong WriteRange ( ulong position, byte[] data ) {

			using ( var lk = new UniqueLock<ILock>(m_writerLock) ) {
				if ( m_epochReads )
					throw new InvalidOperationException("Readers active");

				return m_origCache.WriteRange(position, (ulong)data.LongLength, data);
			}

		}
		/// <summary>
		/// Writes a byte range into the underlying cache using exclusive RCU semantics.
		/// Writers acquire the lock and ensure that no readers are active.
		/// </summary>

		public ulong WriteRange ( ulong position, FixedVector<byte> data ) {

			using ( var lk = new UniqueLock<ILock>(m_writerLock) ) {
				if ( m_epochReads )
					throw new InvalidOperationException("Readers active");

				return m_origCache.WriteRange(position, (ulong)data.Length, data.ToNative());
			}

		}

		/// <summary>
		/// Writes bytes from the specified buffer and advances the internal position
		/// using exclusive RCU semantics. Writers acquire the lock and ensure that
		/// no readers are active during the operation.
		/// </summary>
		/// <param name="buffer">The byte buffer to write</param>
		/// <param name="offset"></param>
		/// <param name="count">Size of the buffer to write</param>
		/// <returns>NUmber of bytes are written</returns>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public int Write ( byte[] buffer, int offset, int count ) {
			using ( var lk = new UniqueLock<ILock>(m_writerLock) ) {
				if ( m_epochReads )
					throw new InvalidOperationException("Readers active");

				return m_origCache.Write(buffer, offset, count);
			}
		}
		/// <summary>
		/// Returns a copy of the underlying cache buffer using exclusive RCU semantics.
		/// Writers acquire the lock and ensure that no readers are active.
		/// </summary>

		public byte[] ToArray () {
			using ( var lk = new UniqueLock<ILock>(m_writerLock) ) {
				if ( m_epochReads )
					throw new InvalidOperationException("Readers active");

				return m_origCache.ToArray();
			}
		}
		/// <summary>
		/// Returns a <see cref="FixedVector{byte}"/> copy of the underlying cache
		/// using exclusive RCU semantics.
		/// </summary>
		public FixedVector<byte> ToArrayEx () {
			using ( var lk = new UniqueLock<ILock>(m_writerLock) ) {
				if ( m_epochReads )
					throw new InvalidOperationException("Readers active");

				return m_origCache.ToArrayEx();
			}
		}
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
		/// <summary>
		/// Reads bytes into the specified buffer using lock‑free RCU semantics.
		/// The read operation is performed inside an epoch to ensure stability of
		/// the underlying memory during the operation.
		/// </summary>
		public int Read ( byte[] buffer, int offset, int count ) {
			using ( var _le = new UniqueEpoch(m_epochReads) ) {
				return m_origCache.Read(buffer, offset, count);
			}
		}
		/// <summary>
		/// Reads a single byte from the specified position using lock‑free RCU
		/// semantics. The read is performed inside an epoch.
		/// </summary>
		public byte ReadByte ( ulong position) {
			using ( var _le = new UniqueEpoch(m_epochReads) ) {
				return m_origCache.ReadByte(position);
			}
		}
		/// <summary>
		/// Reads a range of bytes starting at the specified position using lock‑free
		/// RCU semantics. The read is performed inside an epoch.
		/// </summary>
		public virtual byte[]? ReadRange ( ulong position, uint count ) {
			using ( var _le = new UniqueEpoch(m_epochReads) ) {
				return m_origCache.ReadRange(position, count);
			}
		}

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
	}
	/// @}
}
