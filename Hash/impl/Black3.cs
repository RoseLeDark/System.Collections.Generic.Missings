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

using System;
using System.Text;
using SystemEx.Collections.Generic;

namespace SystemEx.Hash.impl {

    internal class Black3 {

        internal UInt32[] m_key;
        internal Black3ChunkState m_chunk;
        internal ulong m_stack_len;
        internal byte[] m_stack; // cv_stack[(BLAKE3_MAX_DEPTH + 1) * BLAKE3_OUT_LEN];

        internal Black3ChunkState Chunk => m_chunk;

        /// <summary>
        /// blake3_hasher_init ();
        /// </summary>
        public Black3 () {
            m_key = new UInt32[8];
            m_key[0] = Black3Utils.IV[0]; m_key[1] = Black3Utils.IV[1]; m_key[2] = Black3Utils.IV[2]; m_key[3] = Black3Utils.IV[3];
            m_key[4] = Black3Utils.IV[4]; m_key[5] = Black3Utils.IV[5]; m_key[6] = Black3Utils.IV[6]; m_key[7] = Black3Utils.IV[7];

            m_chunk = new Black3ChunkState(m_key, 0, this);
            //chunk_state_init(m_key, 0);
            m_stack_len = 0;
            m_stack = new byte[(Black3Infos.BLAKE3_MAX_DEPTH + 1) * Black3Infos.BLAKE3_OUT_LEN];
        }

        /// <summary>
        /// blake3_hasher_init_keyed
        /// </summary>
        /// <param name="key"></param>
        /// <exception cref="ArgumentException"></exception>
        public Black3 ( byte[] key ) {
            if ( key.Length < 72 ) throw new ArgumentException("key to small");
            m_key = new UInt32[8];

            Black3Utils.load_key_words(key, m_key);

            m_chunk = new Black3ChunkState(m_key, blake3_flags.KEYED_HASH, this);

            m_stack_len = 0;
            m_stack = new byte[(Black3Infos.BLAKE3_MAX_DEPTH + 1) * Black3Infos.BLAKE3_OUT_LEN];
        }


        internal void push_cv ( byte[] new_cv, ulong chunk_counter ) {
            merge_cv_stack(chunk_counter + 1 );

            //memcpy(m_stack[m_stack_len * Black3Infos.BLAKE3_OUT_LEN], new_cv, Black3Infos.BLAKE3_OUT_LEN);
            int dst = (int)( m_stack_len * Black3Infos.BLAKE3_OUT_LEN);

            for ( int i = 0 ; i < Black3Infos.BLAKE3_OUT_LEN ; i++ ) {
                m_stack[dst + i] = new_cv[i];
            }


            m_stack_len += 1;
        }


        internal void merge_cv_stack ( ulong total_len ) {
            ulong post_merge_stack_len = (ulong)Black3Utils.popcnt(total_len);

            while ( m_stack_len > post_merge_stack_len ) {
                int  pos = (int)( (m_stack_len - 2) * Black3Infos.BLAKE3_OUT_LEN );

                byte[] parent_node = new byte[Black3Infos.BLAKE3_OUT_LEN];

                for ( int i = 0 ; i < Black3Infos.BLAKE3_OUT_LEN ; i++ ) {
                    parent_node[i] = m_stack[pos + i];
                }


                Blacke3_Output output = new Blacke3_Output( parent_node, m_key, m_chunk.m_flags);

                output.chaining_value(parent_node);

                for ( int i = 0 ; i < Black3Infos.BLAKE3_OUT_LEN ; i++ )
                    m_stack[pos + i] = parent_node[i];

                m_stack_len -= 1;
            }
        }
    }
}