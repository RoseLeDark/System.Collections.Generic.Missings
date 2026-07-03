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
    /// Implements the Weinberg hash algorithm.
    /// </summary>
    public sealed class WeinbergHash : IHash {
        /// <summary>
        /// Computes the Weinberg hash of the given input.
        /// </summary>
        /// <param name="input">The input data to hash.</param>
        /// <param name="seed">The seed value for the hash computation.</param>
        /// <param name="endian">The endianness for the hash computation.</param>
        /// <returns>The computed Weinberg hash as a 32-bit value.</returns>
        public Hash32 Compute ( Array<byte> input, uint seed, Endian endian ) {
            uint hash = seed;
            uint g;

            for ( int i = 0 ; i < input.Count ; i++ ) {
                hash = (hash << 4) + input[i];
                g = hash & 0xF0000000u;

                if ( g != 0 )
                    hash ^= (g >> 24) ^ g;
            }

            hash ^= (hash >> 16);
            return new Hash32(hash);
        }
        /// <summary>
        /// Computes the Weinberg hash of the given input.
        /// </summary>
        /// <param name="input">The input data to hash.</param>
        /// <param name="seed">The seed value for the hash computation.</param>
        /// <param name="endian">The endianness for the hash computation.</param>
        /// <returns>The computed Weinberg hash as a 64-bit value.</returns>
        public Hash64 ComputeLong ( Array<byte> input, ulong seed, Endian endian ) {
            ulong hash = seed;
            ulong g;

            for ( int i = 0 ; i < input.Count ; i++ ) {
                hash = (hash << 4) + input[i];
                g = hash & 0xF000000000000000ul;

                if ( g != 0 )
                    hash ^= (g >> 56) ^ g;
            }

            hash ^= (hash >> 32);
            return new Hash64(hash);
        }
    }
    /// @}
}
