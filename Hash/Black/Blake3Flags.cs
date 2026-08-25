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


namespace SystemEx.Hash.Black {
	/// \addtogroup HashBlack
	/// @{
	/// <summary>
	/// Internal BLAKE3 flag definitions used during chunk and tree hashing.
	/// 
	/// <para>
	/// These flags control how each block or chunk is interpreted by the
	/// compression function, including start/end markers, parent node
	/// processing, keyed hashing, and key derivation modes.
	/// </para>
	/// 
	/// <para>
	/// This enum is internal and used only by the SystemEx.Hash.Black3
	/// hashing subsystem.
	/// </para>
	/// </summary>
	internal enum Blake3Flags : byte {
		/// <summary>
		/// Marks the beginning of a chunk.
		/// </summary>
		CHUNK_START         = 1 << 0,
		/// <summary>
		/// Marks the end of a chunk.
		/// </summary>
		CHUNK_END           = 1 << 1,
		/// <summary>
		/// Indicates that the block represents a parent node in the tree.
		/// </summary>
		PARENT              = 1 << 2,
		/// <summary>
		/// Marks the root node of the hashing tree.
		/// </summary>
		ROOT                = 1 << 3,
		/// <summary>
		/// Enables keyed hashing mode.
		/// </summary>
		KEYED_HASH          = 1 << 4,
		/// <summary>
		/// Indicates that the block contains key derivation context.
		/// </summary>
		DERIVE_KEY_CONTEXT  = 1 << 5,
		/// <summary>
		/// Indicates that the block contains key derivation material.
		/// </summary>
		DERIVE_KEY_MATERIAL = 1 << 6,
    };

	/// @}
}
