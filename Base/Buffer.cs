using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx {
    public class Buffer {
        public static unsafe void LongCopy<T> ( T[] src, long srcOffset, T[] dst, long dstOffset, long count ) {
            if ( src == null || dst == null ) return;

            fixed ( T* pSrc = src )
            fixed ( T* pDst = dst ) {
                T* s = pSrc + srcOffset;
                T* d = pDst + dstOffset;

                for ( long i = 0 ; i < count ; i++ )
                    d[i] = s[i];
            }
        }
    }
}
