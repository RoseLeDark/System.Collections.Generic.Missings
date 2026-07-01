using System;
using System.Text;
using SystemEx.Collections.Generic;
using SystemEx.Collections.Generic.Interfaces;

namespace SystemEx.Collections.Generic {
    public class RopeChunkValue <T> {
        private Array<T> m_pRoot;
        public Array<T> Root => m_pRoot;

        public long Length { get; }


        public RopeChunkValue ( int ChunkSize ) {
            Length = ChunkSize;
            m_pRoot = new Array<T>(ChunkSize);
        }
        public RopeChunkValue ( Array<T> array) {
            Length = array.Count;
            m_pRoot = array;
        }

        public Array<T> GetArray() {
            return Root;
        }
    }
}
