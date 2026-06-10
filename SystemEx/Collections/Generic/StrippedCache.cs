using System;
using System.Collections.Generic;
using System.Text;
using SystemEx.Collection.Generic.Interfaces;
using SystemEx.Utils;

namespace SystemEx.Collection.Generic {
    public class StrippedCache : Cache {
        private readonly FixedArray<byte> m_segmentTemp;
        private readonly ICache[] m_caches;
        private int m_currentCache;

        public override int Length => (int)LongLength;

        public StrippedCache(int cacheCount, int cacheSize) : base(cacheSize, CacheType.Both) {

            m_caches = new ICache[cacheCount-1];
            for ( int i = 0; i < cacheCount-1; i++ )
                m_caches[i] = new Cache(cacheSize, CacheType.Both);

            LongLength = (ulong)(cacheCount * cacheSize);
            m_currentCache = 0;
            Position = 0;

            m_segmentTemp = new FixedArray<byte>(cacheSize);
        }

        

        public override ulong WriteRange(ulong position, byte[] data) {
            ulong written = WriteRange(Position + position, (ulong)data.LongLength, data);
            Position += written;
            return written;
        }


        public override ulong WriteRange(ulong start, ulong end, byte[] data) {
            if ( end <= start ) return 0;
            if ( end > LongLength ) end = LongLength;

            ulong rangeLen = end - start;
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





        public byte[]? ToArray(int index) {
            byte[]? _ret = null; 

            if ( index == 0 ) _ret = base.ToArray();
            else if( m_caches.Length > index ) _ret = m_caches[index].ToArray();

            return _ret;
        }


    }
}
