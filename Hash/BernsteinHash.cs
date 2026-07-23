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


namespace SystemEx.Hash {
    /// \addtogroup hash
    /// @{
    /// <summary>
    /// Simple non‑cryptographic hash function based on the Bernstein family of hash algorithms.
    /// 
    /// This implementation provides both 32‑bit and 64‑bit variants and supports seeding,
    /// allowing deterministic or randomized hash streams depending on the caller.
    /// 
    /// Characteristics:
    ///   - Very fast
    ///   - Deterministic
    ///   - Suitable for hash tables, indexing, lightweight hashing
    ///   - Not intended for cryptographic use
    /// 
    /// The algorithm uses a classic multiply‑and‑xor mixing step:
    ///     hash = (hash * M) ^ byte
    /// where M is a constant chosen for diffusion.
    /// </summary>
    public class BernsteinHash : IHash {
        private const uint Default = 5381; // DJB2‑Seed
        private const ulong Default64 = 53811835;

        Endian m_endian;
        /// <summary>
        /// Craate a new instance
        /// </summary>
        /// <param name="endian">The suing endian for creating a hash</param>
        public BernsteinHash ( Endian endian ) {
            m_endian = endian;
        }

        /// <summary>
        /// Computes a simple 32‑bit hash from the given byte sequence.
        ///
        /// The hash starts with the provided <paramref name="seed"/> and mixes each byte
        /// using a small multiplier (31), similar to traditional Bernstein/DJB hash variants.
        ///
        /// Endian does not affect this algorithm directly; it is included for interface
        /// compatibility with other SystemEx hashers.
        /// </summary>
        /// <param name="input">The input data to hash.</param>
        /// <param name="seed">The seed value for the hash computation.</param>
        /// <returns>The computed Bernstein hash as a Hash32 object.</returns>
        public Hash32 Compute ( FixedVector<byte> input, uint seed ) {
            if ( input.Count == 0 )
                return new Hash32(0);

            uint hash = seed == 0 ? Default : seed;

            // Simple deterministic byte loop
            for ( int i = 0 ; i < input.Count ; i++ ) {
                hash = (hash * 33) + input[i];
            }
            hash ^= (hash >> 16);
            return new Hash32(hash);
        }

        /// <summary>
        /// Computes a simple 64‑bit hash from the given byte sequence.
        ///
        /// The hash starts with the provided <paramref name="seed"/> and mixes each byte
        /// using a larger multiplier (1315423911), a common constant used in JSHash‑style
        /// Bernstein derivatives to improve diffusion in 64‑bit space.
        ///
        /// Endian does not affect this algorithm directly; it is included for interface
        /// compatibility with other SystemEx hashers.
        /// </summary>
        /// <param name="input">The input data to hash.</param>
        /// <param name="seed">The seed value for the hash computation.</param>
        /// <returns>The computed Bernstein hash as a Hash64 object.</returns>
        public Hash64 ComputeLong ( FixedVector<byte> input, ulong seed ) {
            if ( input.Count == 0 )
                return new Hash64(0);

            ulong hash = seed == 0 ? Default64 : seed;

            // Larger multiplier for 64‑bit
            for ( int i = 0 ; i < input.Count ; i++ ) {
                hash = (hash * 1315423911L) + input[i];
            }
            hash ^= (hash >> 32);
            return new Hash64(hash);
        }
    }
    /// @}
}
