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
    public sealed class FletcherHash : IHash {
        public Hash32 Compute ( Array<byte> input, uint seed, Endian endian ) {
            uint sum1 = 0xffff;
            uint sum2 = 0xffff;

            int index = 0;
            int length = input.Count;

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

        public Hash64 ComputeLong ( Array<byte> input, ulong seed, Endian endian ) {
            // 64‑Bit Wrapper, da du nur die 32‑Bit‑Variante brauchst
            var h32 = Compute(input, (uint)seed, endian);
            return new Hash64(h32.Value);
        }
    }

}
