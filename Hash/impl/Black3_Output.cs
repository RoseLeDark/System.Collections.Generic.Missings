using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using SystemEx.Collections.Generic;

namespace SystemEx.Hash.impl {


    internal struct Blacke3_Output {
        UInt32[] m_cv;
        ulong m_counter;
        byte[]  m_block;
        uint m_blockLen;
        blake3_flags m_flags;

        public Blacke3_Output () {
            m_cv = new uint[8];
            m_block = new byte[Black3Infos.BLAKE3_BLOCK_LEN];
        }

        public Blacke3_Output ( byte[] block, uint[] key, blake3_flags flags) : this() {
            Init(key, block, Black3Infos.BLAKE3_BLOCK_LEN, 0, flags | blake3_flags.PARENT);
        }

        public void Init ( UInt32[] inputCv, byte[] block, ulong counter, uint blockLen, blake3_flags flags ) {
            for ( int i = 0 ; i < inputCv.Length ; i++ )
                m_cv[i] = inputCv[i];

            for ( int i = 0 ; i < block.Length ; i++ )
                m_block[i] = block[i];

            m_counter = counter;
            m_blockLen = blockLen;
            m_flags = flags;
        }

        public void chaining_value ( byte[] cv ) {
            UInt32[] cv_words = new UInt32[8];

            for ( int i = 0 ; i < 8 ; i++ )
                cv_words[i] = m_cv[i];

            //memcpy ( cv_words, m_cv, sizeof(int)*8);

            Black3CompressScalar.in_place_portable(cv_words, m_block, (byte)m_blockLen, m_counter, m_flags);

            Black3Utils.store_cv_words(cv, cv_words);
        }
    }

}
