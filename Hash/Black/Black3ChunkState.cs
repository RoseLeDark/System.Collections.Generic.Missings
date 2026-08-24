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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SystemEx.Collections.Generic;

namespace SystemEx.Hash.Black {
	/// \addtogroup SystemEx.Hash.Black
	/// @{
	/// <summary>
	/// Internal state manager for a single BLAKE3 chunk.
	/// 
	/// <para>
	/// This class tracks the chaining value, chunk counter, block buffer,
	/// buffer length, and compression flags required during incremental
	/// hashing. It handles chunk filling, finalization, and resetting
	/// according to the BLAKE3 specification.
	/// </para>
	/// 
	/// <para>
	/// All members are internal and used only by the SystemEx.Hash.Black3
	/// hashing subsystem.
	/// </para>
	/// </summary>
	internal class Black3ChunkState {
		/// <summary>
		/// Current chaining value (8 x 32‑bit words).
		/// </summary>
		public UInt32[] m_cv;

		/// <summary>
		/// Counter identifying the current chunk index.
		/// </summary>
		public UInt64 m_chunk_counter;

		/// <summary>
		/// Block buffer used to accumulate chunk data.
		/// </summary>
		public byte[] m_buf;

		/// <summary>
		/// Number of bytes currently stored in the buffer.
		/// </summary>
		public int m_buf_len;

		/// <summary>
		/// Number of full blocks already compressed in this chunk.
		/// </summary>
		public byte m_blocks_compressed;

		/// <summary>
		/// Compression flags for this chunk.
		/// </summary>
		public Blake3Flags m_flags;

		/// <summary>
		/// Reference to the parent hasher for pushing chaining values.
		/// </summary>
		private Black3 m_parent;

		/// <summary>
		/// Gets the total number of bytes processed in this chunk.
		/// </summary>
		public int Length => Black3Infos.BLAKE3_BLOCK_LEN * m_blocks_compressed + m_buf_len;

		/// <summary>
		/// Gets the CHUNK_START flag if this is the first block in the chunk.
		/// </summary>
		public Blake3Flags StartFlag => m_blocks_compressed == 0 ? Blake3Flags.CHUNK_START : 0;

		/// <summary>
		/// Initializes a new chunk state with the given key, flags, and parent hasher.
		/// </summary>
		public Black3ChunkState ( UInt32[] key, Blake3Flags flags, Black3 parent ) {
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
		/// <summary>
		/// Updates the chunk with new input data. Handles filling, finalizing,
		/// and resetting the chunk when full.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public void Update ( byte[] input ) {
            if ( input.Length == 0 ) return;
            int input_pos = 0;
            int input_len =input.Length;

            var remaining = input;

			// Fill existing buffer if partially full
			if ( m_buf_len > 0 ) {
                int take = Black3Infos.BLAKE3_CHUNK_LEN - m_buf_len;
                if ( take > input_len )
                    take = input_len;

                for ( int i = 0 ; i < take ; i++ )
                    m_buf[m_buf_len + i] = input[input_pos + i];

                m_buf_len += (byte)take;
                input_pos += take;
                input_len -= take;

				// If chunk is full, finalize it
				if ( input_len > 0 ) {
                    Blacke3_Output output = Finalize();

                    byte[] chunk_cv = new byte[32];
                    output.ChainingValue(chunk_cv);

                    m_parent.PushCv(chunk_cv, m_chunk_counter);

                    Reset(m_parent.m_key, m_chunk_counter + 1);
                } else {
                    return;
                }
            }
			// Process full chunks directly
			while ( input_len >= Black3Infos.BLAKE3_CHUNK_LEN ) {
               
                for ( int i = 0 ; i < Black3Infos.BLAKE3_CHUNK_LEN ; i++ )
                    m_buf[i] = input[input_pos + i];

                m_buf_len = Black3Infos.BLAKE3_CHUNK_LEN;

                // finalize chunk
                Blacke3_Output output = Finalize ();

                byte[] chunk_cv = new byte[32];
                output.ChainingValue(chunk_cv);

                m_parent.PushCv(chunk_cv, m_chunk_counter);

                Reset(m_parent.m_key, m_chunk_counter + 1);

                input_pos += Black3Infos.BLAKE3_CHUNK_LEN;
                input_len -= Black3Infos.BLAKE3_CHUNK_LEN;
            }

			// Copy remaining bytes
			if ( input_len > 0 ) {
                for ( int i = 0 ; i < input_len ; i++ )
                    m_buf[m_buf_len + i] = input[input_pos + i];

                m_buf_len += (byte)input_len;
            }

        }
		/// <summary>
		/// Resets the chunk state for the next chunk.
		/// </summary>
		public void Reset ( uint[] key, ulong chunk_counter ) {
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
		/// <summary>
		/// Finalizes the current chunk and produces a BLAKE3 output node.
		/// </summary>
		public Blacke3_Output Finalize () {
			Blake3Flags oflags = m_flags | StartFlag | Blake3Flags.CHUNK_END;

            if ( m_buf_len < Black3Infos.BLAKE3_BLOCK_LEN ) {
                for ( int i = 0 ; i < m_buf_len ; i++ )
                    m_buf[i] = 0;
            }


            var output = new Blacke3_Output();
            output.Init(m_cv.ToArray(), m_buf, m_chunk_counter, (uint)m_buf_len, oflags);
            return output;
        }
    }
    ///@}
}
