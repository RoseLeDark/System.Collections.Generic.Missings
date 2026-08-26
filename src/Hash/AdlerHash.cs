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
	/// \addtogroup Hash
	/// @{

	/// <summary>
	/// Implements the Adler hash algorithm.
	/// </summary>
	/// <remarks>
	/// The Adler hash algorithm is a checksum algorithm which was invented by Mark Adler in 1995.
	///  It is a modification of the Fletcher checksum, which was invented by John G. Fletcher in 1982.
	/// </remarks>
	public sealed class AdlerHash : IHash {
        private const uint Mod = 65521;
        private const ulong Mod64 = 4294967291UL; // großer Prim

        Endian m_endian;
        /// <summary>
        /// Craate a new instance
        /// </summary>
        /// <param name="endian">The suing endian for creating a hash</param>
        public AdlerHash( Endian endian  ) {
            m_endian = endian;
        }
        /// <summary>
        /// Computes the Adler hash of the given input. 
        /// </summary>
        /// <param name="input">The input data to hash.</param>
        /// <param name="seed">The seed value for the hash computation.</param>
        /// <returns>The computed Adler hash as a Hash32 object.</returns>
        public Hash32 Compute ( FixedVector<byte> input, uint seed ) {
            uint a = 1;
            uint b = 0;

            for ( int i = 0 ; i < input.Length ; i++ ) {
                a = (a + input[i] ) % Mod;
                b = (b + a) % Mod;
            }

            var ui = (b << 16) | a;

            return new Hash32(ui);
        }
        /// <summary>
        /// Computes the Adler hash of the given input.
        /// </summary>
        /// <param name="input">The input data to hash.</param>
        /// <param name="seed">The seed value for the hash computation.</param>
        /// <returns>The computed Adler hash as a Hash64 object.</returns>
        public Hash64 ComputeLong ( FixedVector<byte> input, ulong seed ) {
            ulong  a = 1;
            ulong  b = 0;

            for ( int i = 0 ; i < input.Length ; i++ ) {
                a = (a + input[i]) % Mod64;
                b = (b + a) % Mod64;
            }

            var ui = (b << 32) | a;

            return new Hash64(ui);
        }
    }
    /// @}
}
