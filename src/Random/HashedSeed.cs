
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
using SystemEx.Hash;

namespace SystemEx.Random {
	/// \addtogroup Random
	/// @{
    
	/// <summary>
	/// Provides a seed implementation based on a 32‑bit hash value. The hash may
	/// originate from any hashing algorithm available in SystemEx, including
	/// Bernstein, FNV‑1a, Murmur, Blake3, XXH3, and other supported hash functions.
	///
	/// This seed type is designed for use with all random engines in SystemEx. 
	/// </summary>
	public struct HashedSeed : ISeed<Hash32> {
        private uint[] m_seed;

        /// <inheritdoc/>
        public int Length => m_seed.Length;
        /// <inheritdoc/>
        public uint[] GetSeed () => m_seed;
        /// <summary>
        /// Updates the seed using the specified <see cref="Hash32"/> value.
        /// The hash is expanded into four distinct 32‑bit values using bitwise
        /// and arithmetic transformations. This method does not compute the hash;
        /// </summary>
        /// <param name="value">
        /// The <see cref="Hash32"/> instance used to refresh the seed. The hash may
        /// originate from any hashing algorithm supported by SystemEx.
        /// </param>
        public void Update ( Hash32 value ) {
            var v = value.Value;

            m_seed[0] = (uint)v ^ 0xA3B1C2D3u;
            m_seed[1] = (uint)(v << 1) ^ 0xF00DBAAFu;
            m_seed[2] = (uint)(v >> 1) ^ 0x12345678u;
            m_seed[3] = (uint)v * 2654435761u;
        }
        /// <inheritdoc/>
        public uint this[int index] {
            get => m_seed[index];
            set => m_seed[index] = value;
        }
        /// <summary>
        /// Initializes a new <see cref="HashedSeed"/> using the specified hash.
        /// The seed is immediately populated using <see cref="Update(Hash32)"/>.
        /// </summary>
        /// <param name="hash">
        /// The initial <see cref="Hash32"/> value used to populate the seed.
        /// The hash may originate from any hashing algorithm supported by SystemEx.
        /// </param>
        public HashedSeed ( Hash32 hash ) {
            m_seed = new uint[4];

            Update(hash);
        }
    }
    /// @}
}
