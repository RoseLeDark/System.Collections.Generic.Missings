
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


using SystemEx.Collections.Generic;
using SystemEx.Hash.Black;

namespace SystemEx.Hash {
	/// \addtogroup SystemEx.Hash
	/// @{
	/// <summary>
	/// Public BLAKE3 hasher implementation used by the SystemEx hashing subsystem.
	/// 
	/// <para>
	/// <see cref="Black3Hasher"/> is a high‑level wrapper around the internal
	/// BLAKE3 chunk and tree‑hashing engine. It supports keyed hashing, endian‑aware
	/// output conversion, and integrates with the SystemEx attribute‑driven
	/// hashing model via <see cref="HashAlgorithmAttribute"/>.
	/// </para>
	/// 
	/// <para>
	/// Unlike simple one‑shot hash functions, this hasher drives the full BLAKE3
	/// state machine: chunk accumulation, compression, chaining‑value propagation,
	/// and finalization. It produces stable 32‑bit and 64‑bit hash values suitable
	/// for identifiers, lightweight integrity checks, or general hashing tasks.
	/// </para>
	/// </summary>
	public sealed class Black3Hasher : IHash {
		/// <summary>
		/// Internal key material used for keyed hashing.
		/// </summary>
		Vector<byte> m_key;

		/// <summary>
		/// Endian mode used when converting the final chaining value.
		/// </summary>
		Endian m_endian;

		/// <summary>
		/// Initializes a new BLAKE3 hasher with the given endian mode and key.
		/// </summary>
		/// <param name="endian">Endian mode for output conversion.</param>
		/// <param name="IV">Key or initialization vector.</param>
		public Black3Hasher ( Endian endian, byte[] IV ) {
			m_key = new Vector<byte>(IV);
			m_endian = endian;
		}

		/// <summary>
		/// Computes the BLAKE3 hash of the given input.
		/// </summary>
		/// <param name="input">The input data to hash.</param>
		/// <param name="seed">The seed value for the hash computation.</param>
		/// <returns>The computed BLAKE3 hash as a Hash32 object.</returns>
		public Hash32 Compute ( FixedVector<byte> input, uint seed ) {
			Black3 hash = new Black3(m_key.ToNative());

			hash.Chunk.Update(input.ToNative());

			var output = hash.Chunk.Finalize();

			byte[] cv = new byte[Black3Infos.BLAKE3_OUT_LEN];
			output.ChainingValue(cv);

			return new Hash32(cv.ToUInt());
		}

		/// <summary>
		/// Computes the BLAKE3 hash of the given input.
		/// </summary>
		/// <param name="input">The input data to hash.</param>
		/// <param name="seed">The seed value for the hash computation.</param>
		/// <returns>The computed BLAKE3 hash as a Hash64 object.</returns>
		public Hash64 ComputeLong ( FixedVector<byte> input, ulong seed ) {
			Black3 hash = new Black3(m_key.ToNative());

			hash.Chunk.Update(input.ToNative());

			var output = hash.Chunk.Finalize();

			byte[] cv = new byte[Black3Infos.BLAKE3_OUT_LEN];
			output.ChainingValue(cv);

			return new Hash64((ulong)cv.ToUInt());
		}
	}
    // @}
}
