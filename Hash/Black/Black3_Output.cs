
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
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using SystemEx.Collections.Generic;

namespace SystemEx.Hash.Black {
	/// \addtogroup HashBlack
	/// @{

	/// <summary>
	/// Internal BLAKE3 output node used during tree hashing.
	/// Stores chaining values, block data, and compression parameters.
	/// 
	/// <para>
	/// This type is not part of the public API. It provides the internal
	/// structure required to compute parent nodes and chaining values
	/// during incremental hashing.
	/// </para>
	/// </summary>
	internal struct Blacke3_Output {
		/// <summary>
		/// The current chaining value words (8 x 32‑bit).
		/// </summary>
		UInt32[] m_cv;

		/// <summary>
		/// The chunk counter associated with this output node.
		/// </summary>
		ulong m_counter;

		/// <summary>
		/// The block buffer used for compression.
		/// </summary>
		byte[] m_block;

		/// <summary>
		/// Length of the block in bytes.
		/// </summary>
		uint m_blockLen;

		/// <summary>
		/// Compression flags controlling how this node is processed.
		/// </summary>
		Blake3Flags m_flags;
		/// <summary>
		/// Initializes an empty output node with default buffers.
		/// </summary>
		public Blacke3_Output () {
            m_cv = new uint[8];
            m_block = new byte[Black3Infos.BLAKE3_BLOCK_LEN];
        }
		/// <summary>
		/// Initializes a parent output node using the given block, key, and flags.
		/// </summary>
		public Blacke3_Output ( byte[] block, uint[] key, Blake3Flags flags ) : this() {
            Init(key, block, Black3Infos.BLAKE3_BLOCK_LEN, 0, flags | Blake3Flags.PARENT);
        }
		/// <summary>
		/// Initializes the output node with chaining values, block data,
		/// counter, block length, and compression flags.
		/// </summary>
		public void Init ( UInt32[] inputCv, byte[] block, ulong counter, uint blockLen, Blake3Flags flags ) {
            for ( int i = 0 ; i < inputCv.Length ; i++ )
                m_cv[i] = inputCv[i];

            for ( int i = 0 ; i < block.Length ; i++ )
                m_block[i] = block[i];

            m_counter = counter;
            m_blockLen = blockLen;
            m_flags = flags;
        }
		/// <summary>
		/// Computes the next chaining value by compressing the current node
		/// and writes the result into the provided buffer.
		/// </summary>
		public void ChainingValue ( byte[] cv ) {
            UInt32[] cv_words = new UInt32[8];

            for ( int i = 0 ; i < 8 ; i++ )
                cv_words[i] = m_cv[i];

            //memcpy ( cv_words, m_cv, sizeof(int)*8);

            Black3CompressScalar.InPlacePortable(cv_words, m_block, (byte)m_blockLen, m_counter, m_flags);

            Black3Utils.StoreCVWords(cv, cv_words);
        }
    }
	/// @}

}
