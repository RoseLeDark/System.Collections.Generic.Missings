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
using SystemEx.Threading;

namespace SystemEx.IO {
	// \addtogroup IO
	/// @{

	/// <summary>
	/// Indicates the current operational state of an <see cref="RCUStream{TStream}"/>.
	/// Readers observe <see cref="Current"/> when no writer is active, and
	/// <see cref="Update"/> when a writer holds the exclusive lock.
	/// </summary>
	public enum RCUStreamState {
		/// <summary>
		/// No writer is active; read operations observe the current stable data.
		/// </summary>
		Current,

		/// <summary>
		/// A writer is active; readers may observe stale data while the update
		/// operation is in progress.
		/// </summary>
		Update
	}

	/// <summary>
	/// A Read-Copy-Update (RCU) wrapper around a <see cref="Stream"/> that provides
	/// lock-free read operations and exclusive write operations. Reader activity is
	/// tracked through an <see cref="Epoch"/>, while writers are synchronized using
	/// an <see cref="ILock"/>. This ensures deterministic behavior in concurrent
	/// read/write scenarios without requiring external synchronization.
	/// </summary>
	/// <typeparam name="TStream">
	/// The underlying stream type being wrapped. Must derive from <see cref="Stream"/>.
	/// </typeparam>
	public class RCUStream<TStream> : Stream , IValueWriter, IValueReader
		where TStream : Stream {

		private TStream m_origStream;
		private Epoch m_epochReads;
		private ILock m_writerLock;

		/// <inheritdoc/>
		public RCUStreamState State 
			=> ( (m_writerLock.IsHeld) ? RCUStreamState.Update : RCUStreamState.Current);

		/// <inheritdoc/>
		public override bool CanWrite 
			=> m_epochReads.CanWrite && m_origStream.CanWrite;

		/// <inheritdoc/>
		public override bool CanSeek 
			=> m_epochReads.CanWrite && m_origStream.CanSeek;

		/// <inheritdoc/>
		public override bool CanRead => m_origStream.CanRead;

		/// <inheritdoc/>
		public override long Length => m_origStream.Length;

		/// <inheritdoc/>
		public override long Position {

			get {
				using ( var _le = new UniqueEpoch(ref m_epochReads) ) {
					return m_origStream.Position;
				}
			} set {
				using ( var lk = new ScopedLock<ILock>(ref m_writerLock) ) {
					if ( m_epochReads )
						throw new InvalidOperationException("Readers active");

					m_origStream.Position = value;
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RCUStream{TStream}"/> class
		/// using the specified underlying stream and writer lock. The stream is
		/// immediately ready for concurrent read operations and exclusive write
		/// operations according to RCU semantics.
		/// </summary>
		/// <param name="stream">The underlying stream to wrap.</param>
		/// <param name="locker">
		/// The lock used to synchronize writer access. Readers remain lock-free.
		/// </param>
		public RCUStream (TStream stream, ILock locker) {
			m_origStream = stream;
			m_epochReads = new Epoch();
			m_writerLock = locker;

		}
		/// <inheritdoc/>
		public override void WriteByte ( byte value ) {
			using ( var lk = new ScopedLock<ILock>(ref m_writerLock) ) {
				if ( m_epochReads )
					throw new InvalidOperationException("Readers active");

				m_origStream.WriteByte(value);
			}
		}

		/// <inheritdoc/>
		public override void Write ( byte[] buffer, int offset, int count ) {

			using ( var lk = new ScopedLock<ILock>(ref m_writerLock) ) {
				if ( m_epochReads )
					throw new InvalidOperationException("Readers active");

				m_origStream.Write(buffer, offset, count);
			}
		}

		/// <inheritdoc/>
		public override async Task WriteAsync ( byte[] buffer, int offset, int count, CancellationToken cancellationToken ) {
				if ( m_epochReads )
					throw new InvalidOperationException("Readers active");

				await m_origStream.WriteAsync(buffer, offset, count, cancellationToken);
		}
		/// <inheritdoc/>
		public override int ReadByte () {
			using ( var _le = new UniqueEpoch(ref m_epochReads) ) {
				return m_origStream.ReadByte();
			}
		}

		/// <inheritdoc/>
		public override int Read ( byte[] buffer, int offset, int count ) {

			using ( var _le = new UniqueEpoch(ref m_epochReads) ) {
				return m_origStream.Read(buffer, offset, count);
			}

		}

		/// <inheritdoc/>
		public override async Task<int> ReadAsync ( byte[] buffer, int offset, int count, CancellationToken cancellationToken ) {
				return await m_origStream.ReadAsync(buffer, offset, count, cancellationToken);
		}

		/// <inheritdoc/>
		public override void CopyTo ( Stream destination, int bufferSize ) {
			using ( var _le = new UniqueEpoch(ref m_epochReads) ) {
				m_origStream.CopyTo(destination, bufferSize);
			}
		}

		/// <inheritdoc/>
		public override async Task CopyToAsync ( Stream destination, int bufferSize, CancellationToken cancellationToken ) {
				await m_origStream.CopyToAsync(destination, bufferSize, cancellationToken);
		}

		/// <inheritdoc/>
		public override long Seek ( long offset, SeekOrigin origin ) {
			using ( var lk = new ScopedLock<ILock>(ref m_writerLock) ) {
				if ( m_epochReads )
					throw new InvalidOperationException("Readers active");

				return m_origStream.Seek(offset, origin);
			}
		}

		/// <inheritdoc/>
		public override void SetLength ( long value ) {
			using ( var lk = new ScopedLock<ILock>(ref m_writerLock) ) {
				if ( m_epochReads )
					throw new InvalidOperationException("Readers active");

				m_origStream.SetLength(value);
			}
		}

		/// <inheritdoc/>
		public override void Flush () {
			using ( var lk = new ScopedLock<ILock>(ref m_writerLock) ) {
				if ( m_epochReads )
					throw new InvalidOperationException("Readers active");

				m_origStream.Flush();
			}
		}



		#region SYSTEMEX_ADDON

		/// <inheritdoc/>
		public void Write ( short value, Endian endian ) {
			byte[] _buffer = value.ToBytes(endian);
			Write(_buffer, 0, _buffer.Length);
		}
		/// <inheritdoc/>
		public void Write ( ushort value, Endian endian ) {
			byte[] _buffer = value.ToBytes(endian);
			Write(_buffer, 0, _buffer.Length);
		}
		/// <inheritdoc/>
		public void Write ( uint value, Endian endian ) {
			byte[] _buffer = value.ToBytes(endian);
			Write(_buffer, 0, _buffer.Length);
		}
		/// <inheritdoc/>
		public void Write ( int value, Endian endian ) {
			byte[] _buffer = value.ToBytes(endian);
			Write(_buffer, 0, _buffer.Length);
		}
		/// <inheritdoc/>
		public void Write ( long value, Endian endian ) {
			byte[] _buffer = value.ToBytes(endian);
			Write(_buffer, 0, _buffer.Length);
		}
		/// <inheritdoc/>
		public void Write ( ulong value, Endian endian ) {
			byte[] _buffer = value.ToBytes(endian);
			Write(_buffer, 0, _buffer.Length);
		}
		/// <inheritdoc/>
		public void Write ( float value, Endian endian ) {
			byte[] _buffer = value.ToBytes(endian);
			Write(_buffer, 0, _buffer.Length);
		}
		/// <inheritdoc/>
		public void Write ( double value, Endian endian ) {
			byte[] _buffer = value.ToBytes(endian);
			Write(_buffer, 0, _buffer.Length);
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

		#endregion
	}
	
}
