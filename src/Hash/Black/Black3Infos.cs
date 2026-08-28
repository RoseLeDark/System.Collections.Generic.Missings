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
	/// Internal BLAKE3 constant definitions used by the hashing subsystem.
	/// 
	/// <para>
	/// This class provides fixed sizes and limits required by the portable
	/// compression implementation, including block length, key length,
	/// chunk size, output size, and maximum tree depth.
	/// </para>
	/// 
	/// <para>
	/// These values are part of the BLAKE3 specification and should not be
	/// modified. The type is internal and used only by the SystemEx hashing
	/// components.
	/// </para>
	/// </summary>
	internal class Black3Infos {
		/// <summary>
		/// Size of the BLAKE3 key in bytes.
		/// </summary>
		public const byte BLAKE3_KEY_LEN = 32;

		/// <summary>
		/// Size of a single BLAKE3 block in bytes.
		/// </summary>
		public const byte BLAKE3_BLOCK_LEN = 64;

		/// <summary>
		/// Size of a BLAKE3 chunk in bytes.
		/// </summary>
		public const ushort BLAKE3_CHUNK_LEN = 1024;

		/// <summary>
		/// Maximum depth of the BLAKE3 tree hashing structure.
		/// </summary>
		public const byte BLAKE3_MAX_DEPTH = 54;

		/// <summary>
		/// Size of a BLAKE3 output chaining value in bytes.
		/// </summary>
		public const byte BLAKE3_OUT_LEN = 32;

		/// <summary>
		/// Total size of the chaining‑value stack.
		/// </summary>
		public const int BLAKE3_CV_STACK_LEN = (BLAKE3_MAX_DEPTH + 1) * BLAKE3_OUT_LEN;
	}
    
}
