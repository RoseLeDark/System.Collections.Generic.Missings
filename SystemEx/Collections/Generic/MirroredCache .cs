using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using SystemEx.Collection.Generic.Interfaces;
using SystemEx.Utils;

namespace SystemEx.Collection.Generic {
    internal class MirroredCache : Cache {
        private readonly Cache m_secondary;

        public Cache Primary => this;
        public Cache Secondary => m_secondary;

        public int Size => Length;

        public MirroredCache(int size) : base(size, CacheType.Both) {
            m_secondary = new Cache(size, CacheType.Both);
        }

        public override ulong WriteRange(ulong start, ulong end, byte[] data) {
            // 1. Original in Primary
            ulong written = base.WriteRange(start, end, data);

            // 2. Spiegel erzeugen
            byte[] mirrored = Mirror(data);

            // 3. Spiegel in Secondary
            m_secondary.WriteRange(start, end, mirrored);

            return written;
        }

        public override byte[]? ReadRange(ulong position, uint count) {
            byte[]? _readed =  base.ReadRange(position, count);
            byte[]? _mirrowed = m_secondary.ReadRange(position, count);

            if ( _mirrowed == null || _readed == null ) return null;

            MirrorRead(_mirrowed);

            bool cheak = _readed.EqualArray(_mirrowed);

            return cheak ? _readed : null;
         }
        private static byte[] MirrorRead(byte[] data) {
            int i = 0;
            int j = data.Length - 1;

            while ( i < j ) {
                byte tmp = data[i];
                data[i] = data[j];
                data[j] = tmp;
                i++;
                j--;
            }
            return data;
        }

        private static byte[] Mirror(byte[] data) {
            byte[] m = new byte[data.Length];
            int last = data.Length - 1;

            for ( int i = 0; i < data.Length; i++ )
                m[i] = data[last - i];

            return m;
        }

        
    }
}
