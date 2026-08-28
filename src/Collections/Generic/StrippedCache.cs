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

namespace SystemEx.Collections.Generic {
	/// \addtogroup Collections
	/// @{

	/// <summary>
	/// A segmented cache implementation that splits a large logical address space
	/// into multiple fixed-size cache segments.  
	/// The first segment is the base <see cref="Cache"/> instance, while all
	/// additional segments are stored in an internal array of <see cref="ICache"/>.
	/// </summary>
	/// <remarks>
	/// This class supports large continuous write operations by automatically
	/// determining the correct segment and offset for each write.  
	/// Writes may span multiple segments, and data is written chunk-by-chunk
	/// using a temporary <see cref="FixedVector{T}"/> buffer.
	/// </remarks>
	public class StrippedCache : Cache {
        /// <summary>
        /// Temporary buffer used for chunked writes when data does not align
        /// perfectly with segment boundaries.
        /// </summary>
        private readonly FixedVector<byte> m_segmentTemp;

		/// <summary>
		/// Total length of all segments combined.
		/// </summary>
		private readonly ulong m_totalLength;

		/// <summary>
		/// Additional cache segments beyond the base segment.
		/// </summary>
		private readonly Cache[] m_caches;
        /// <summary>
        /// Gets the total length of the cache in bytes.
        /// </summary>
        public override ulong Length => m_totalLength;

		/// <summary>
		/// Creates a new segmented cache with the specified number of segments
		/// and segment size.
		/// </summary>
		/// <param name="cacheCount">The total number of segments.</param>
		/// <param name="cacheSize">The size of each segment in bytes.</param>
		public StrippedCache ( int cacheCount, int cacheSize ) : base(cacheSize, CacheType.Both) {

			m_caches = new Cache[cacheCount - 1];
			for ( int i = 0 ; i < cacheCount - 1 ; i++ )
				m_caches[i] = new Cache(cacheSize, CacheType.Both);

			m_totalLength = (ulong)(cacheCount * cacheSize);
			// m_currentCache = 0;
			SetSavePosition(0);

			m_segmentTemp = new FixedVector<byte>(cacheSize);
		}

		/// <summary>
		/// Creates a <see cref="FlexSpan{T}"/> view into the stripped cache at the
		/// specified global offset. The stripped cache is composed of multiple
		/// fixed-size cache segments arranged sequentially. This method resolves
		/// the global position into the correct segment and returns a span view
		/// over that segment's internal buffer.
		/// 
		/// The returned span does not allocate and directly references the
		/// underlying segment storage.
		/// </summary>
		/// <param name="start">
		/// Global byte offset within the combined stripped-cache space.
		/// Must be within the range <c>[0, LongLength)</c>.
		/// </param>
		/// <param name="mode">
		/// Indexing mode used by the returned <see cref="FlexSpan{T}"/>.
		/// Supports System, Reverse, and Ring indexing.
		/// </param>
		/// <returns>
		/// A <see cref="FlexSpan{T}"/> referencing the correct cache segment
		/// at the resolved local offset.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when <paramref name="start"/> is outside the valid range.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public override VectorFlexSpan<byte, FixedVector<byte>> AsFlexSpan ( long start, FlexSpanMode mode = FlexSpanMode.System ) {
			if ( start < 0 || start >= (long)m_totalLength )
				throw new ArgumentOutOfRangeException(nameof(start));

			long segmentSize = (long)base.Length;
			long segmentIndex = start / segmentSize;
			long localOffset = start % segmentSize;

			if ( segmentIndex == 0 )
				return base.AsFlexSpan(localOffset, mode);
			else
				return m_caches[segmentIndex - 1].AsFlexSpan(localOffset, mode);
		}

        


        /// <summary>
        /// Writes a byte array starting at the current position plus the given offset.
        /// </summary>
        /// <param name="position">Offset relative to the current write position.</param>
        /// <param name="data">The data to write.</param>
        /// <returns>The number of bytes written.</returns>
        public override ulong WriteRange(ulong position, byte[] data) {
			ulong written = WriteRange(Position + position, (ulong)data.LongLength, data);
			SetSavePosition(Position + written);
			return written;
		}

        /// <summary>
        /// Writes a range of bytes into the segmented cache.  
        /// Automatically determines the correct segment and offset for each chunk.
        /// </summary>
        /// <param name="start">The starting absolute position.</param>
        /// <param name="iend">The ending absolute position (exclusive).</param>
        /// <param name="data">The data to write.</param>
        /// <returns>The number of bytes successfully written.</returns>
        public override ulong WriteRange(ulong start, ulong iend, byte[] data) {
			if ( iend <= start ) return 0;
			if ( iend > m_totalLength ) iend = m_totalLength;

			ulong rangeLen = iend - start;
			ulong writableLen = (ulong)data.Length < rangeLen ? (ulong)data.Length : rangeLen;

			ulong totalWritten = 0;
			ulong pos = start;
			int dataOffset = 0;

			ulong segmentSize = base.Length;

			while ( totalWritten < writableLen ) {
				ulong cacheIdx = pos / segmentSize;
				ulong cacheOff = pos % segmentSize;

				if ( cacheIdx >= (ulong)(m_caches.Length + 1) )
					break;

				ulong spaceInCache = segmentSize - cacheOff;
				ulong remaining = writableLen - totalWritten;
				uint chunkLen = (uint)((remaining < spaceInCache) ? remaining : spaceInCache);

				if ( chunkLen == data.Length && dataOffset == 0 && cacheOff == 0 ) {
					ulong written = (cacheIdx == 0)
					? base.WriteRange(cacheOff, cacheOff + (ulong)chunkLen, data)
					: m_caches[cacheIdx - 1].WriteRange(cacheOff, cacheOff + (ulong)chunkLen, data);

					if ( written == 0 ) break;

					totalWritten += written;
					pos += written;
					dataOffset += (int)written;
				} else {
					m_segmentTemp.CopyFrom(new FixedVector<byte>(data), (uint)dataOffset, 0, chunkLen);
					byte[] internalBuf = m_segmentTemp.ToNative();

					ulong writtenChunk = (cacheIdx == 0)
					? base.WriteRange(cacheOff, cacheOff + (ulong)chunkLen, internalBuf)
					: m_caches[cacheIdx - 1].WriteRange(cacheOff, cacheOff + (ulong)chunkLen, internalBuf);

					if ( writtenChunk == 0 ) break;

					totalWritten += writtenChunk;
					pos += writtenChunk;
					dataOffset += (int)writtenChunk;
				}
			}

			return totalWritten;
		}





        /// <summary>
        /// Returns the raw byte array of the specified segment.
        /// </summary>
        /// <param name="index">The segment index (0 = base segment).</param>
        /// <returns>The segment data, or <c>null</c> if the index is invalid.</returns>
        public byte[]? ToArray(int index) {
			if ( index == 0 ) return base.ToArray();
			if ( index < m_caches.Length ) return m_caches[index].ToArray();
			return null;
		}


    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
