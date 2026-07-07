using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using SystemEx.Collections.Generic;

namespace SystemEx.Hash.impl {
    internal class Black3ChunkState {
        public UInt32[] m_cv;
        public UInt64 m_chunk_counter;
        public byte[] m_buf;
        public int m_buf_len;
        public byte m_blocks_compressed;
        public blake3_flags m_flags;
        private Black3 m_parent;

        public int Length => Black3Infos.BLAKE3_BLOCK_LEN * m_blocks_compressed + m_buf_len;
        public blake3_flags StartFlag => m_blocks_compressed == 0 ? blake3_flags.CHUNK_START : 0;

        public Black3ChunkState ( UInt32[] key, blake3_flags flags, Black3 parent ) {
            m_cv = new uint[8];
            m_cv[0] = key[0]; m_cv[1] = key[1]; m_cv[2] = key[2]; m_cv[3] = key[3];
            m_cv[4] = key[4]; m_cv[5] = key[5]; m_cv[6] = key[6]; m_cv[7] = key[7];
            m_chunk_counter = 0;
            m_buf = new byte [Black3Infos.BLAKE3_BLOCK_LEN]; 

            m_buf_len = 0;
            m_blocks_compressed = 0;
            m_flags = flags;
            m_parent = parent;

        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public void Update ( byte[] input ) {
            if ( input.Length == 0 ) return;
            int input_pos = 0;
            int input_len =input.Length;

            var remaining = input;

            // Wenn der Chunk schon Daten hat, zuerst auffüllen
            if ( m_buf_len > 0 ) {
                int take = Black3Infos.BLAKE3_CHUNK_LEN - m_buf_len;
                if ( take > input_len )
                    take = input_len;

                // chunk_state_update: Bytes in m_buf kopieren
                for ( int i = 0 ; i < take ; i++ )
                    m_buf[m_buf_len + i] = input[input_pos + i];

                m_buf_len += (byte)take;
                input_pos += take;
                input_len -= take;

                // Wenn der Chunk jetzt voll ist und noch Daten kommen → finalize chunk
                if ( input_len > 0 ) {
                    // chunk_state_output → erzeugt Output-Objekt
                    Blacke3_Output output = Finalize();

                    // chaining value extrahieren
                    byte[] chunk_cv = new byte[32];
                    output.chaining_value(chunk_cv);

                    // hasher_push_cv
                    m_parent.push_cv(chunk_cv, m_chunk_counter);

                    // chunk_state_reset
                    Reset(m_parent.m_key, m_chunk_counter + 1);
                } else {
                    return;
                }
            }
            // Jetzt haben wir einen leeren Chunk und noch input_len > 0
            while ( input_len >= Black3Infos.BLAKE3_CHUNK_LEN ) {
                // Direkt einen vollen Chunk verarbeiten
                // chunk_state_update
                for ( int i = 0 ; i < Black3Infos.BLAKE3_CHUNK_LEN ; i++ )
                    m_buf[i] = input[input_pos + i];

                m_buf_len = Black3Infos.BLAKE3_CHUNK_LEN;

                // finalize chunk
                Blacke3_Output output = Finalize ();

                byte[] chunk_cv = new byte[32];
                output.chaining_value(chunk_cv);

                m_parent.push_cv(chunk_cv, m_chunk_counter);

                Reset(m_parent.m_key, m_chunk_counter + 1);

                input_pos += Black3Infos.BLAKE3_CHUNK_LEN;
                input_len -= Black3Infos.BLAKE3_CHUNK_LEN;
            }

            // Rest in den Chunk kopieren
            if ( input_len > 0 ) {
                for ( int i = 0 ; i < input_len ; i++ )
                    m_buf[m_buf_len + i] = input[input_pos + i];

                m_buf_len += (byte)input_len;
            }

        }

        internal void Reset ( uint[] key, ulong chunk_counter ) {
            m_cv[0] = key[0]; m_cv[1] = key[1];
            m_cv[2] = key[2]; m_cv[3] = key[3];
            m_cv[4] = key[4]; m_cv[5] = key[5];
            m_cv[6] = key[6]; m_cv[7] = key[7];

            m_chunk_counter = chunk_counter;
            m_blocks_compressed = 0;

            for ( int i = 0 ; i < Black3Infos.BLAKE3_BLOCK_LEN ; i++ )
                m_buf[i] = 0;
            m_buf_len = 0;
        }

        public Blacke3_Output Finalize () {
            blake3_flags oflags = m_flags | StartFlag | blake3_flags.CHUNK_END;

            if ( m_buf_len < Black3Infos.BLAKE3_BLOCK_LEN ) {
                for ( int i = 0 ; i < m_buf_len ; i++ )
                    m_buf[i] = 0;
            }


            var output = new Blacke3_Output();
            output.Init(m_cv.ToArray(), m_buf, m_chunk_counter, (uint)m_buf_len, oflags);
            return output;
        }
    }
}
