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
    /// Implements the Fletcher hash algorithm. 
    /// </summary>
    public sealed class FletcherHash : IHash {
        Endian m_endian;
        /// <summary>
        /// Craate a new instance
        /// </summary>
        /// <param name="endian">The suing endian for creating a hash</param>
        public FletcherHash ( Endian endian ) {
            m_endian = endian;
        }
        /// <summary>
        /// Computes the Fletcher hash of the given input.
        /// </summary>
        /// <param name="input">The input data to hash.</param>
        /// <param name="seed">The seed value for the hash computation.</param>
        /// <returns>The computed Fletcher hash as a Hash32 object.</returns>
        public Hash32 Compute ( FixedVector<byte> input, uint seed ) {
            uint sum1 = 0xffff;
            uint sum2 = 0xffff;

            int index = 0;
            int length = (int) input.Count;

            while ( length > 0 ) {
                int tlen = (length > 360 ? 360 : length);
                length -= tlen;

                for ( int i = 0 ; i < tlen ; i++ ) {
                    sum1 = (sum1 + input[index++]) % 0xffff;
                    sum2 = (sum2 + sum1) % 0xffff;
                }
            }

            uint result = (sum2 << 16) | sum1;
            return new Hash32(result);
        }
        /// <summary>
        /// Computes the Fletcher hash of the given input.
        /// </summary>
        /// <param name="input">The input data to hash.</param>
        /// <param name="seed">The seed value for the hash computation.</param>
        /// <returns>The computed Fletcher hash as a Hash64 object.</returns>
        public Hash64 ComputeLong ( FixedVector<byte> input, ulong seed ) {
            // 64‑Bit Wrapper, da du nur die 32‑Bit‑Variante brauchst
            var h32 = Compute(input, (uint)seed );
            return new Hash64(h32.Value);
        }
    }
    /// @}
}
