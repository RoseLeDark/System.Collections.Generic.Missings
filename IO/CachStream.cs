// SPDX-License-Identifier: EUPL-1.2

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

using SystemEx.Collections.Generic;
using SystemEx.Collections.Generic.Interfaces;
using SystemEx.Utils;


namespace SystemEx.IO {
    /// <summary>
    /// A <see cref="Stream"/> wrapper around a <see cref="Cache"/> instance.  
    /// Provides sequential read/write access to a cache, including endian‑aware
    /// primitive serialization, range operations, and chunked asynchronous
    /// copy methods.  
    /// Seeking is supported only through the underlying cache.
    /// </summary>
    /// <typeparam name="TCache">
    /// The cache type used as the backing store. Must derive from <see cref="Cache"/>.
    /// </typeparam>
    public class CacheStream<TCache> : Stream where TCache : Cache {
        /// <summary>
        /// The underlying cache used for all read/write operations.
        /// </summary>
        private TCache m_cache;


        /// <summary>
        /// Creates a new stream wrapper for the specified cache.
        /// </summary>
        public CacheStream(TCache cache) {
            m_cache = cache;
        }
        /// <summary>
        /// Gets or sets the current position within the cache.
        /// </summary>
        public override long Position { 
            get => (long)m_cache.Position; 
            set => m_cache.SetSavePosition((ulong)value); 
        }

        /// <summary>
        /// Copies the contents of this stream to another stream using the specified buffer size.
        /// </summary>
        public override void CopyTo(Stream destination, int bufferSize) {
            if ( GetType() != typeof(ICache) ) {
                if ( !m_cache.CanRead ) throw new InvalidOperationException("is Locked");
                base.CopyTo(destination, bufferSize);
            } else {
                base.CopyTo(destination, bufferSize);
            }
        }
        /// <summary>
        /// Writes the entire cache content to another stream.
        /// </summary>
        public virtual void WriteTo(Stream stream) {
            if ( !m_cache.CanRead ) throw new InvalidOperationException("src is Locked");
            if ( stream.GetType() != typeof(ICache) ) {
                CacheStream<TCache> _x = (CacheStream<TCache>)stream;

                if ( !_x.CanWrite ) throw new InvalidOperationException("is Locked");
                _x.WriteRange(0, ToArray());

            } else {
                stream.Write(ToArray());
            }

        }
        // <summary>
        /// Indicates whether the stream supports reading.
        /// </summary>
        public override bool CanRead => m_cache.CanRead;
        /// <summary>
        /// Indicates whether the stream supports seeking.  
        /// Always <c>true</c> for <see cref="CacheStream{TCache}"/>.
        /// </summary>
        public override bool CanSeek => true;
        /// <summary>
        /// Indicates whether the stream supports writing.
        /// </summary>
        public override bool CanWrite => m_cache.CanWrite;
        /// <summary>
        /// Gets the length of the underlying cache.
        /// </summary>
        public override long Length => m_cache.Length;
        /// <summary>
        /// Gets the number of bytes written by the last write operation.
        /// </summary>
        public int Written { get; internal set; }
        /// <summary>
        /// Gets the LongLength of the underlying cache.
        /// </summary>
        public ulong LongLength => (ulong)m_cache.Length;

        public bool IsEmpty => m_cache.Length == 0;

        /// <summary>
        /// Flushes the stream. No‑op for cache streams.
        /// </summary>
        public override void Flush() {
            return;
        }
        /// <summary>
        /// Reads bytes from the cache into the specified buffer.
        /// </summary>
        public override int Read(byte[] buffer, int offset, int count) {
            if ( !CanRead ) return 0;
            return m_cache.Read(buffer, offset, count);
        }
        /// <summary>
        /// Writes bytes from the buffer into the cache.
        /// </summary>
        public override void Write(byte[] buffer, int offset, int count) {
            if ( !CanWrite ) { Written = 0; return; }
            Written = m_cache.Write(buffer, offset, count);
        }
        /// <summary>
        /// Moves the cache position using the specified origin and offset.
        /// </summary>
        public override long Seek(long offset, SeekOrigin origin) {
            return (long)m_cache.Seek(origin, (int)offset);
        }

        public override void SetLength(long value) {
           
        }

        public int Write(char value) => m_cache.Write((byte)value);
        public int Write(byte value) => m_cache.Write((byte)value);

        public int Write(uint value, Endian endian) {
            byte[] _buffer = value.ToBytes(endian);
            return m_cache.Write(_buffer, 0, _buffer.Length);
        }
        public int Write(int value, Endian endian) {
            byte[] _buffer = value.ToBytes(endian);
            return m_cache.Write(_buffer, 0, _buffer.Length);
        }
        public int Write(short value, Endian endian) {
            byte[] _buffer = value.ToBytes(endian);
            return m_cache.Write(_buffer, 0, _buffer.Length);
        }
        public int Write(ushort value, Endian endian) {
            byte[] _buffer = value.ToBytes(endian);
            return m_cache.Write(_buffer, 0, _buffer.Length);
        }
        public int Write(long value, Endian endian) {
            byte[] _buffer = value.ToBytes(endian);
            return m_cache.Write(_buffer, 0, _buffer.Length);
        }
        public int Write(ulong value, Endian endian) {
            byte[] _buffer = value.ToBytes(endian);
            return m_cache.Write(_buffer, 0, _buffer.Length);
        }
        public int Write(float value, Endian endian) {
            byte[] _buffer = value.ToBytes(endian);
            return m_cache.Write(_buffer, 0, _buffer.Length);
        }
        public int Write(double value, Endian endian) {
            byte[] _buffer = value.ToBytes(endian);
            return m_cache.Write(_buffer, 0, _buffer.Length);
        }

        public uint ReadUInt(Endian endian) => m_cache.ReadUInt((ulong)Position, endian);
        public int ReadInt(Endian endian) => m_cache.ReadInt((ulong)Position, endian);
        public short ReadShort(Endian endian) => m_cache.ReadShort((ulong)Position, endian);
        public ushort ReadUShort(Endian endian) => m_cache.ReadUShort((ulong)Position, endian);
        public long ReadLong(Endian endian) => m_cache.ReadLong((ulong)Position, endian);
        public ulong ReadULong(Endian endian) => m_cache.ReadULong((ulong)Position, endian);
        public char ReadChar(ulong position) => m_cache.ReadChar((ulong)Position);
        public float ReadFloat(Endian endian) => m_cache.ReadFloat((ulong)Position, endian);
        public double ReadDouble(Endian endian) => m_cache.ReadDouble((ulong)Position, endian);

        public ulong WriteRange(byte[] data)
        => m_cache.WriteRange((ulong)Position, data);

        public ulong WriteRange(ulong iend, byte[] data)
            => m_cache.WriteRange((ulong)Position, iend, data);

        public byte[]? ReadRange(uint count)
            => m_cache.ReadRange((ulong)Position, count);

        /// <summary>
        /// Returns the entire cache content as a byte array.
        /// </summary>
        public byte[] ToArray() => m_cache.ToArray();

        /// <summary>
        /// Asynchronously copies the contents of a source cache into this stream's cache
        /// using fixed‑size chunks.  
        /// Supports cancellation and lock checking.
        /// </summary>
        public virtual Task WriteAsync(ICache src, CancellationToken cancellationToken = default) {
            if ( !m_cache.CanWrite ) throw new InvalidOperationException("dest is Locked");
            if ( !src.CanRead ) throw new InvalidOperationException("src is Locked");

            if ( cancellationToken.IsCancellationRequested )
                return Task.FromCanceled(cancellationToken);

            try {
                const int CHUNK = 64 * 1024; // 64 KiB
                ulong total = (ulong)src.Length;
                ulong pos = 0;

                while ( pos < total ) {
                    if ( cancellationToken.IsCancellationRequested )
                        return Task.FromCanceled(cancellationToken);

                    int toRead = (int)System.Math.Min((ulong)CHUNK, total - pos);
                    byte[]? chunk = src.ReadRange(pos, (uint)toRead);

                    if ( chunk == null || chunk.Length == 0 )
                        break;

                    m_cache.WriteRange(pos, chunk);
                    pos += (ulong)chunk.Length;
                }

                return Task.CompletedTask;
            } catch ( OperationCanceledException oce ) {
                return Task.FromCanceled(oce.CancellationToken);
            } catch ( Exception ex ) {
                return Task.FromException(ex);
            }
        }
        /// <summary>
        /// Asynchronously reads the contents of this stream's cache into another cache
        /// using fixed‑size chunks.  
        /// Supports cancellation and lock checking.
        /// </summary>
        public virtual Task<ICache> ReadAsync(ICache dest, CancellationToken cancellationToken = default) {
            if ( !m_cache.CanRead ) throw new InvalidOperationException("src is Locked");
            if ( !dest.CanWrite ) throw new InvalidOperationException("dest is Locked");

            if ( cancellationToken.IsCancellationRequested )
                return Task.FromCanceled<ICache>(cancellationToken);

            try {
                const int CHUNK = 64 * 1024; // 64 KiB
                ulong total = (ulong)m_cache.Length;
                ulong pos = 0;

                while ( pos < total ) {
                    if ( cancellationToken.IsCancellationRequested )
                        return Task.FromCanceled<ICache>(cancellationToken);

                    int toRead = (int)System.Math.Min((ulong)CHUNK, total - pos);
                    byte[]? chunk = m_cache.ReadRange(pos, (uint)toRead);

                    if ( chunk == null || chunk.Length == 0 )
                        break;

                    dest.WriteRange(pos, chunk);
                    pos += (ulong)chunk.Length;
                }

                return Task.FromResult(dest);
            } catch ( OperationCanceledException oce ) {
                return Task.FromCanceled<ICache>(oce.CancellationToken);
            } catch ( Exception ex ) {
                return Task.FromException<ICache>(ex);
            }
        }
    }
}
