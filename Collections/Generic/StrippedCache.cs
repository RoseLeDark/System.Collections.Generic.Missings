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
using SystemEx.Collections.Generic.Interfaces;

namespace SystemEx.Collections.Generic {
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
    /// using a temporary <see cref="FixedArray{T}"/> buffer.
    /// </remarks>
    public class StrippedCache : Cache {
        /// <summary>
        /// Temporary buffer used for chunked writes when data does not align
        /// perfectly with segment boundaries.
        /// </summary>
        private readonly FixedArray<byte> m_segmentTemp;

        /// <summary>
        /// Additional cache segments beyond the base segment.
        /// </summary>
        private readonly ICache[] m_caches;
        /// <summary>
        /// Gets the total length of the cache in bytes.
        /// </summary>
        public override ulong Length => LongLength;

        /// <summary>
        /// Creates a new segmented cache with the specified number of segments
        /// and segment size.
        /// </summary>
        /// <param name="cacheCount">The total number of segments.</param>
        /// <param name="cacheSize">The size of each segment in bytes.</param>
        public StrippedCache(int cacheCount, int cacheSize) : base(cacheSize, CacheType.Both) {

            m_caches = new ICache[cacheCount-1];
            for ( int i = 0; i < cacheCount-1; i++ )
                m_caches[i] = new Cache(cacheSize, CacheType.Both);

            LongLength = (ulong)(cacheCount * cacheSize);
           // m_currentCache = 0;
            SetSavePosition(0);

            m_segmentTemp = new FixedArray<byte>(cacheSize);
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
            if ( iend > LongLength ) iend = LongLength;

            ulong rangeLen = iend - start;
            ulong writableLen = (ulong)data.Length < rangeLen ? (ulong)data.Length : rangeLen;

            ulong totalWritten = 0;
            ulong pos = start;
            int dataOffset = 0;
            ulong segmentSize = base.LongLength;

            while ( totalWritten < writableLen ) {
                ulong cacheIdx = pos / segmentSize;
                ulong cacheOff = pos % segmentSize;
                if ( cacheIdx >= (ulong)(m_caches.Length + 1) ) break;

                ulong spaceInCache = segmentSize - cacheOff;
                ulong remaining = writableLen - totalWritten;
                uint chunkLen = (uint)((remaining < spaceInCache) ? remaining : spaceInCache);

                // Fast path: ganzes data-Array passt und kein Offset nötig
                if ( chunkLen == data.Length && dataOffset == 0 && cacheOff == 0 ) {

                    ulong written = (cacheIdx == 0)
                        ? base.WriteRange(cacheOff, cacheOff + (ulong)chunkLen, data)
                        : m_caches[cacheIdx - 1].WriteRange(cacheOff, cacheOff + (ulong)chunkLen, data);

                    if ( written == 0 ) break;

                    totalWritten += written;
                    pos += written;
                    dataOffset += (int)written;

                } else {

                    m_segmentTemp.CopyFrom(data, (uint)dataOffset, 0, chunkLen);
                    // Hole internen Buffer (ToArray liefert m_elements)
                    byte[] internalBuf = m_segmentTemp.ToArray();

                    // Schreibe den Chunk in den jeweiligen Cache
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
            byte[]? _ret = null; 

            if ( index == 0 ) _ret = base.ToArray();
            else if( m_caches.Length > index ) _ret = m_caches[index].ToArray();

            return _ret;
        }


    }
}
