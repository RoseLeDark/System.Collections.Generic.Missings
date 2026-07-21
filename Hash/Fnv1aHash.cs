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

namespace SystemEx.Hash {
    /// \addtogroup hash
    /// @{
    /// <summary>
    /// Implements the FNV-1a hash algorithm.
    /// </summary> 
    public sealed class Fnv1aHash : IHash {
        private const ulong Offset64 = 14695981039346656037UL;
        private const ulong Prime64  = 1099511628211UL;

        private const uint Prime32 = 16777619u;
        private const uint Offset32 = 2166136261u;

        Endian m_endian;
        /// <summary>
        /// Craate a new instance
        /// </summary>
        /// <param name="endian">The suing endian for creating a hash</param>
        public Fnv1aHash ( Endian endian ) {
            m_endian = endian;
        }

        /// <summary>
        /// Computes the FNV-1a hash of the given input.
        /// </summary>
        /// <param name="input">The input data to hash.</param>
        /// <param name="seed">The seed value for the hash computation.</param>
        /// <returns>The computed FNV-1a hash as a Hash32 object.</returns>
        public Hash32 Compute ( Array<byte> input, uint seed ) {
            uint hash = seed == 0 ? Offset32 : seed;

            for ( int i = 0 ; i < input.Count ; i++ ) {
                hash ^= input[i];
                hash *= Prime32;
            }

            hash ^= (hash >> 16);
            return new Hash32(hash);
        }
        /// <summary>
        /// Computes the FNV-1a hash of the given input.   
        /// </summary>
        /// <param name="input">The input data to hash.</param>
        /// <param name="seed">The seed value for the hash computation.</param>
        /// <returns>The computed FNV-1a hash as a Hash64 object.</returns>
        public Hash64 ComputeLong ( Array<byte> input, ulong seed ) {
            ulong hash = seed == 0 ? Offset64 : seed;

            for ( int i = 0 ; i < input.Count ; i++ ) {
                hash ^= input[i];
                hash *= Prime64;
            }

            // Dein typischer Amber‑Mixer
            hash ^= (hash >> 32);

            return new Hash64(hash);
        }
    }
    /// @}
}
