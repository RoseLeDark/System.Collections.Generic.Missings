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
    public sealed class AdlerHash : IHash {
        private const uint Mod = 65521;
        private const ulong Mod64 = 4294967291UL; // großer Prim
        // Adler32
        public Hash32 Compute ( Array<byte> input, uint seed, Endian endian ) {
            uint a = 1;
            uint b = 0;

            for ( int i = 0 ; i < input.Count ; i++ ) {
                a = (a + input[i] ) % Mod;
                b = (b + a) % Mod;
            }

            var ui = (b << 16) | a;

            return new Hash32(ui);
        }

        public Hash64 ComputeLong ( Array<byte> input, ulong seed, Endian endian ) {
            ulong  a = 1;
            ulong  b = 0;

            for ( int i = 0 ; i < input.Count ; i++ ) {
                a = (a + input[i]) % Mod64;
                b = (b + a) % Mod64;
            }

            var ui = (b << 32) | a;

            return new Hash64(ui);
        }
    }

}
