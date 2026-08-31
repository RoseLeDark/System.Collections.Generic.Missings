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

namespace SystemEx.Hash.Black {
	/// \addtogroup HashBlack
	/// @{
	/// <summary>
	/// Internal BLAKE3 hasher implementation. This class manages the chaining
	/// values, chunk state, and compression stack required for incremental hashing.
	/// 
	/// <para>
	/// This type is not intended for public use. It provides the internal logic
	/// behind the SystemEx BLAKE3 wrapper and exposes only the minimal API needed
	/// by higher‑level hashing components.
	/// </para>
	/// </summary>
	internal class Black3 {

		/// <summary>
		/// Internal key words used for initialization and keyed hashing.
		/// </summary>
		internal UInt32[] m_key;

		/// <summary>
		/// Current chunk state used during incremental hashing.
		/// </summary>
		internal Black3ChunkState m_chunk;

		/// <summary>
		/// Length of the chaining‑value stack.
		/// </summary>
		internal ulong m_stack_len;

		/// <summary>
		/// Stack of intermediate chaining values used for tree hashing.
		/// </summary>
		internal byte[] m_stack;
		/// <summary>
		/// Gets the current chunk state.
		/// </summary>
		internal Black3ChunkState Chunk => m_chunk;

		/// <summary>
		/// Initializes a new BLAKE3 hasher in unkeyed mode.
		/// Equivalent to <c>blake3_hasher_init()</c>.
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
		/// Initializes a new BLAKE3 hasher in keyed mode.
		/// Equivalent to <c>blake3_hasher_init_keyed()</c>.
		/// </summary>
		/// <param name="key">The 256‑bit key used for keyed hashing.</param>
		/// <exception cref="ArgumentException">
		/// Thrown when the key is smaller than the required size.
		/// </exception>
		public Black3 ( byte[] key ) {
            if ( key.Length < 72 ) throw new ArgumentException("key to small");
            m_key = new UInt32[8];

            Black3Utils.LoadKeyWords(key, m_key);

            m_chunk = new Black3ChunkState(m_key, Blake3Flags.KEYED_HASH, this);

            m_stack_len = 0;
            m_stack = new byte[(Black3Infos.BLAKE3_MAX_DEPTH + 1) * Black3Infos.BLAKE3_OUT_LEN];
        }

		/// <summary>
		/// Pushes a new chaining value onto the stack and merges existing values
		/// according to the BLAKE3 tree hashing rules.
		/// </summary>
		/// <param name="new_cv">The new chaining value.</param>
		/// <param name="chunk_counter">The current chunk counter.</param>
		internal void PushCv ( byte[] new_cv, ulong chunk_counter ) {
			MergeCVStack(chunk_counter + 1 );

            //memcpy(m_stack[m_stack_len * Black3Infos.BLAKE3_OUT_LEN], new_cv, Black3Infos.BLAKE3_OUT_LEN);
            int dst = (int)( m_stack_len * Black3Infos.BLAKE3_OUT_LEN);

            for ( int i = 0 ; i < Black3Infos.BLAKE3_OUT_LEN ; i++ ) {
                m_stack[dst + i] = new_cv[i];
            }


            m_stack_len += 1;
        }


		/// <summary>
		/// Merges chaining values on the stack until the tree structure matches
		/// the required depth for the given total length.
		/// </summary>
		/// <param name="total_len">The total number of processed chunks.</param>
		internal void MergeCVStack ( ulong total_len ) {
            ulong post_merge_stack_len = (ulong)Black3Utils.PopCnt(total_len);

            while ( m_stack_len > post_merge_stack_len ) {
                int  pos = (int)( (m_stack_len - 2) * Black3Infos.BLAKE3_OUT_LEN );

                byte[] parent_node = new byte[Black3Infos.BLAKE3_OUT_LEN];

                for ( int i = 0 ; i < Black3Infos.BLAKE3_OUT_LEN ; i++ ) {
                    parent_node[i] = m_stack[pos + i];
                }


                Blacke3_Output output = new Blacke3_Output( parent_node, m_key, m_chunk.m_flags);

                output.ChainingValue(parent_node);

                for ( int i = 0 ; i < Black3Infos.BLAKE3_OUT_LEN ; i++ )
                    m_stack[pos + i] = parent_node[i];

                m_stack_len -= 1;
            }
        }
    }
	
}