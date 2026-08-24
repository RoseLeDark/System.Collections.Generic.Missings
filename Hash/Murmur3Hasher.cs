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

namespace SystemEx.Hash {
#if TESTING
    /// \addtogroup SystemEx.Hash
	/// @{
    /// <summary>
    /// Implements the Murmur3 hashing algorithm (32‑bit and 64‑bit)   
    /// 
    /// Both <c>Compute</c> (32‑bit) and <c>ComputeLong</c> (64‑bit) operate on
    /// <see cref="Array{byte}"/> and support endian‑aware block construction.
    /// </summary>
    internal  sealed class Murmur3Hash : IHash {

        Endian m_endian;
        /// <summary>
        /// Craate a new instance
        /// </summary>
        /// <param name="endian">The suing endian for creating a hash</param>
        public Murmur3Hash ( Endian endian ) {
            m_endian = endian;
        }

        /// <summary>
        /// Computes the 32‑bit Murmur3 hash over the given byte array using
        /// iterator‑driven 4‑byte block processing.  
        /// 
        /// The function advances the iterator in 4‑byte steps and constructs  
        /// Remaining bytes (tail) are processed using the same iterator model.
        /// </summary>
        public Hash32 Compute ( Vector<byte> input, uint seed) {
            uint h1 = seed;

            // 4‑Byte Blöcke
            for ( ArrayRandomAccessIterator<byte> it = input.First ; it != input.End ; it.Forward(4) ) {
                // Wenn weniger als 4 Bytes übrig sind → Tail
                if ( Iterator.Distance(it.Clone(), input.End.Clone()) < 4 )
                    break;

                uint k1;

                byte[] bvalue = new byte[4]  {
                    it.Current,
                    Iterator.Advance<byte>(it, 1).Current,
                    Iterator.Advance<byte>(it, 2).Current,
                    Iterator.Advance<byte>(it, 3).Current
                };

                k1 = bvalue.ToUInt(m_endian);

                // Murmur3‑Mixing
                k1 *= 3432918353U;
                k1 = RotateLeft(k1, 15);
                k1 *= 461845907U;

                h1 ^= k1;
                h1 = RotateLeft(h1, 13);
                h1 = h1 * 5 + 0xe6546b64;
            }

            // --- TAIL ---
            int remainder = Iterator.Distance(input.First, input.End) % 4;

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
        /// </summary>
        public Hash64 ComputeLong ( Array<byte> input, ulong seed ) {
            int length = input.Count;
            ulong h = seed;   

            // 8‑Byte Blöcke
            for ( ArrayRandomAccessIterator<byte> it = input.First ; it != input.End ; it.Forward(8) ) {

                if ( Iterator.Distance(it, input.End) < 8 )
                    break;

                ulong k = 0;

                byte[] bvalue = new byte[8]  {
                    it.Current,
                    Iterator.Advance<byte>(it, 1).Current,
                    Iterator.Advance<byte>(it, 2).Current,
                    Iterator.Advance<byte>(it, 3).Current,
                    Iterator.Advance<byte>(it, 4).Current,
                    Iterator.Advance<byte>(it, 5).Current,
                    Iterator.Advance<byte>(it, 6).Current,
                    Iterator.Advance<byte>(it, 7).Current
                };

                k = bvalue.ToULong(m_endian);

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
#endif
}
