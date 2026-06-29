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

using System.Runtime.CompilerServices;
using SystemEx.Collections.Generic;
using SystemEx.Collections.Generic.Interfaces;

namespace SystemEx.Hash {
    /// \addtogroup hash
    /// @{
    /// <summary>
    /// Implements the Murmur3 hashing algorithm (32‑bit and 64‑bit)   
    /// 
    /// Both <c>Compute</c> (32‑bit) and <c>ComputeLong</c> (64‑bit) operate on
    /// <see cref="Array{byte}"/> and support endian‑aware block construction.
    /// </summary>
    internal class Murmur3Hasher : IHash {
        /// <summary>
        /// Computes the 32‑bit Murmur3 hash over the given byte array using
        /// iterator‑driven 4‑byte block processing.  
        /// 
        /// The function advances the iterator in 4‑byte steps and constructs
        /// the block value according to the specified <paramref name="endian"/>.  
        /// Remaining bytes (tail) are processed using the same iterator model.
        /// </summary>
        public Hash32 Compute ( Array<byte> input, uint seed, Endian endian ) {
            uint h1 = seed;

            // 4‑Byte Blöcke
            for ( ArrayRandomAccessIterator<byte> it = input.First ; it != input.End ; it.Forward(4) ) {
                // Wenn weniger als 4 Bytes übrig sind → Tail
                if ( Iterator.Distance(it.Clone(), input.End.Clone()) < 4 )
                    break;

                byte b0 = it.Current;
                byte b1 = Iterator.Advance<byte>(it, 1).Current;
                byte b2 = Iterator.Advance<byte>(it, 2).Current;
                byte b3 = Iterator.Advance<byte>(it, 3).Current;

                uint k1;

                if ( endian == Endian.LittleEndian ) {
                    k1 = (uint)(b0
                        | (b1 << 8)
                        | (b2 << 16)
                        | (b3 << 24));
                } else {
                    k1 = (uint)(b3
                        | (b2 << 8)
                        | (b1 << 16)
                        | (b0 << 24));
                }

                // Murmur3‑Mixing
                k1 *= 3432918353U;
                k1 = RotateLeft(k1, 15);
                k1 *= 461845907U;

                h1 ^= k1;
                h1 = RotateLeft(h1, 13);
                h1 = h1 * 5 + 0xe6546b64;
            }

            // --- TAIL ---
            int remainder = Iterator.Distance(input.First.Clone(), input.End.Clone()) % 4;

            if ( remainder > 0 ) {

                ArrayRandomAccessIterator<byte> tailIt = input.At(input.Count - remainder);

                uint k1 = 0;

                switch ( remainder ) {
                case 3:
                k1 |= (uint)tailIt.Current;
                k1 |= (uint)Iterator.Advance<byte>(tailIt, 1).Current << 8;
                k1 |= (uint)Iterator.Advance<byte>(tailIt, 2).Current << 16;
                break;

                case 2:
                k1 |= (uint)tailIt.Current;
                k1 |= (uint)Iterator.Advance<byte>(tailIt, 1).Current << 8;
                break;

                case 1:
                k1 |= tailIt.Current;
                break;
                }

                k1 *= 3432918353U;
                k1 = RotateLeft(k1, 15);
                k1 *= 461845907U;

                h1 ^= k1;
            }

            h1 ^= (uint)input.Count;
            h1 = FMix(h1);

            return new Hash32(h1);
        }


        /// <summary>
        /// Computes the 64‑bit Murmur3 hash using iterator‑driven 8‑byte block
        /// processing.  
        /// 
        /// The function advances the iterator in 8‑byte steps and constructs
        /// the 64‑bit block according to <paramref name="endian"/>.  
        /// and combined into a partial block.
        /// </summary>
        public Hash64 ComputeLong ( Array<byte> input, ulong seed, Endian endian ) {
            int length = input.Count;
            ulong h = seed;   

            // 8‑Byte Blöcke
            for ( ArrayRandomAccessIterator<byte> it = input.First ; it != input.End ; it.Forward(8) ) {

                if ( Iterator.Distance(it, input.End) < 8 )
                    break;

                ulong k;

                byte b0 = it.Current;
                byte b1 = Iterator.Advance<byte>(it, 1).Current;
                byte b2 = Iterator.Advance<byte>(it, 2).Current;
                byte b3 = Iterator.Advance<byte>(it, 3).Current;
                byte b4 = Iterator.Advance<byte>(it, 4).Current;
                byte b5 = Iterator.Advance<byte>(it, 5).Current;
                byte b6 = Iterator.Advance<byte>(it, 6).Current;
                byte b7 = Iterator.Advance<byte>(it, 7).Current;

                if ( endian == Endian.LittleEndian ) {
                    k = (ulong)b0
                      | ((ulong)b1 << 8)
                      | ((ulong)b2 << 16)
                      | ((ulong)b3 << 24)
                      | ((ulong)b4 << 32)
                      | ((ulong)b5 << 40)
                      | ((ulong)b6 << 48)
                      | ((ulong)b7 << 56);
                } else {
                    k = (ulong)b7
                      | ((ulong)b6 << 8)
                      | ((ulong)b5 << 16)
                      | ((ulong)b4 << 24)
                      | ((ulong)b3 << 32)
                      | ((ulong)b2 << 40)
                      | ((ulong)b1 << 48)
                      | ((ulong)b0 << 56);
                }

                // Murmur‑like mixing
                k *= 0x87c37b91114253d5UL;
                k = RotateLeft64(k, 31);
                k *= 0x4cf5ad432745937fUL;

                h ^= k;
                h = RotateLeft64(h, 27);
                h = h * 5 + 0x52dce729;
            }

            int remainder = Iterator.Distance(input.End, input.First) % 8;


            // Tail
            if ( remainder > 0 ) {
                ArrayRandomAccessIterator<byte> tailIt = input.At(input.Count - remainder);

                ulong k = 0;

                for ( int i = 0 ; i < remainder ; i++ ) {
                    byte b = Iterator.Advance<byte>(tailIt, i).Current;
                    k |= (ulong)b << (8 * i);
                }

                k *= 0x87c37b91114253d5UL;
                k = RotateLeft64(k, 31);
                k *= 0x4cf5ad432745937fUL;

                h ^= (ulong)k;
            }

            h ^= (ulong)input.Count;
            h = FMix64(h);

            return new Hash64(h);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong RotateLeft64 ( ulong x, int r ) {
            return (x << r) | (x >> (64 - r));
        }

        private static ulong FMix64 ( ulong k ) {
            k ^= k >> 33;
            k *= 0xff51afd7ed558ccd;
            k ^= k >> 33;
            k *= 0xc4ceb9fe1a85ec53;
            k ^= k >> 33;

            return k;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint RotateLeft ( uint x, byte r ) {
            return x << (int)r | x >> 32 - (int)r;
        }
        internal static uint FMix ( uint h ) {
            h = (uint)(((int)h ^ (int)(h >> 16)) * -2048144789);
            h = (uint)(((int)h ^ (int)(h >> 13)) * -1028477387);
            return h ^ h >> 16;
        }
    }
    /// @}
}
