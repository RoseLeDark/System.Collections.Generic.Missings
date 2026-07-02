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
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Random {
    /// <summary>
    /// Represents the ISAAC 32-bit random number generator.
    /// </summary>
    public sealed class Isaac32Engine {
        private const uint GoldenRatio = 0x9e3779b9u; // dein TGoldenRatio für 32 Bit
        private const int Size = 256;

        private uint _cnt;
        private readonly uint[] _rsl = new uint[Size];
        private readonly uint[] _mem = new uint[Size];
        private uint _a, _b, _c;
        /// <summary>
        /// Initializes a new instance of the <see cref="Isaac32Engine"/> class with the specified seed values.
        /// </summary>
        /// <param name="a">The first seed value.</param>
        /// <param name="b">The second seed value.</param>
        /// <param name="c">The third seed value.</param>
        public Isaac32Engine ( uint a = 0, uint b = 0, uint c = 0 ) {
            Seed(a, b, c, null);
        }
        /// <summary>
        /// Seeds the random number generator with the specified values.
        /// </summary>
        /// <param name="a">The first seed value.</param>
        /// <param name="b">The second seed value.</param>
        /// <param name="c">The third seed value.</param>
        /// <param name="s">The seed array, or null to use default values.</param>
        public void Seed ( uint a, uint b, uint c, uint[]? s = null ) {
            uint aa, bb, cc, dd, ee, ff, gg, hh;
            aa = bb = cc = dd = ee = ff = gg = hh = GoldenRatio;

            // Seed‑Array oder Null → wie bei dir m_rsl
            for ( int i = 0 ; i < Size ; i++ )
                _rsl[i] = s != null ? s[i] : 0;

            _a = a;
            _b = b;
            _c = c;

            Shuffle(ref aa, ref bb, ref cc, ref dd, ref ee, ref ff, ref gg, ref hh);
            Shuffle(ref aa, ref bb, ref cc, ref dd, ref ee, ref ff, ref gg, ref hh);
            Shuffle(ref aa, ref bb, ref cc, ref dd, ref ee, ref ff, ref gg, ref hh);
            Shuffle(ref aa, ref bb, ref cc, ref dd, ref ee, ref ff, ref gg, ref hh);

            for ( int i = 0 ; i < Size ; i += 8 ) {
                aa += _rsl[i + 0]; bb += _rsl[i + 1]; cc += _rsl[i + 2]; dd += _rsl[i + 3];
                ee += _rsl[i + 4]; ff += _rsl[i + 5]; gg += _rsl[i + 6]; hh += _rsl[i + 7];

                Shuffle(ref aa, ref bb, ref cc, ref dd, ref ee, ref ff, ref gg, ref hh);

                _mem[i + 0] = aa; _mem[i + 1] = bb; _mem[i + 2] = cc; _mem[i + 3] = dd;
                _mem[i + 4] = ee; _mem[i + 5] = ff; _mem[i + 6] = gg; _mem[i + 7] = hh;
            }

            for ( int i = 0 ; i < Size ; i += 8 ) {
                aa += _mem[i + 0]; bb += _mem[i + 1]; cc += _mem[i + 2]; dd += _mem[i + 3];
                ee += _mem[i + 4]; ff += _mem[i + 5]; gg += _mem[i + 6]; hh += _mem[i + 7];

                Shuffle(ref aa, ref bb, ref cc, ref dd, ref ee, ref ff, ref gg, ref hh);

                _mem[i + 0] = aa; _mem[i + 1] = bb; _mem[i + 2] = cc; _mem[i + 3] = dd;
                _mem[i + 4] = ee; _mem[i + 5] = ff; _mem[i + 6] = gg; _mem[i + 7] = hh;
            }

            Isaac();
            _cnt = Size - 1;
        }

        /// <summary>
        /// Generates the next random number in the sequence.
        /// </summary>
        /// <returns>The next random number.</returns>  
        public uint Next () {
            if ( _cnt == 0 ) {
                Isaac();
                _cnt = Size - 1;
                return _rsl[_cnt];
            }

            return _rsl[_cnt--];
        }
        /// <summary>
        /// Shuffles the specified values.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="c">The third value.</param>
        /// <param name="d">The fourth value.</param>
        /// <param name="e">The fifth value.</param>
        /// <param name="f">The sixth value.</param>
        /// <param name="g">The seventh value.</param>
        /// <param name="h">The eighth value.</param>
        private static void Shuffle ( ref uint a, ref uint b, ref uint c, ref uint d,
                                    ref uint e, ref uint f, ref uint g, ref uint h ) {
            a ^= b << 11; d += a; b += c;
            b ^= c >> 2; e += b; c += d;
            c ^= d << 8; f += c; d += e;
            d ^= e >> 16; g += d; e += f;
            e ^= f << 10; h += e; f += g;
            f ^= g >> 4; a += f; g += h;
            g ^= h << 8; b += g; h += a;
            h ^= a >> 9; c += h; a += b;
        }
        /// <summary>
        /// Gets the indexed value from the specified array.
        /// </summary>
        /// <param name="mm">The array.</param>
        /// <param name="x">The index.</param>
        /// <returns>The indexed value.</returns>
        private static uint Ind ( uint[] mm, uint x ) {
            // 32‑Bit: Index = (x & (255 << 2)) / 4
            int idx = (int)((x & (255u << 2)) >> 2);
            return mm[idx];
        }
        /// <summary>
        /// Performs a random number generation step.
        /// </summary>
        /// <param name="mix">The mixing value.</param>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="mm">The memory array.</param>
        /// <param name="m">The memory index.</param>
        /// <param name="m2">The second memory index.</param>
        /// <param name="r">The result array.</param>
        /// <param name="rIdx">The result index.</param>
        /// <param name="x">The first temporary value.</param>
        /// <param name="y">The second temporary value.</param>
        private static void RngStep ( uint mix, ref uint a, ref uint b,
                                    uint[] mm, ref int m, ref int m2,
                                    uint[] r, ref int rIdx, ref uint x, ref uint y ) {
            x = mm[m];
            a = (a ^ mix) + mm[m2++];
            y = mm[m] = Ind(mm, x) + a + b;
            r[rIdx++] = b = Ind(mm, y >> 8) + x;
            m++;
        }

        /// <summary>
        /// Generates a sequence of random numbers using the ISAAC algorithm.
        /// </summary>
        private void Isaac () {
            uint x = 0, y = 0;
            uint[] mm = _mem;
            uint[] r = _rsl;

            uint a = _a;
            uint b = _b + (++_c);

            int m = 0;
            int m2 = 128;
            int rIdx = 0;

            for ( ; m < 128 ; ) {
                RngStep(a << 13, ref a, ref b, mm, ref m, ref m2, r, ref rIdx, ref x, ref y);
                RngStep(a >> 6, ref a, ref b, mm, ref m, ref m2, r, ref rIdx, ref x, ref y);
                RngStep(a << 2, ref a, ref b, mm, ref m, ref m2, r, ref rIdx, ref x, ref y);
                RngStep(a >> 16, ref a, ref b, mm, ref m, ref m2, r, ref rIdx, ref x, ref y);
            }

            m2 = 0;
            for ( ; m2 < 128 ; ) {
                RngStep(a << 13, ref a, ref b, mm, ref m, ref m2, r, ref rIdx, ref x, ref y);
                RngStep(a >> 6, ref a, ref b, mm, ref m, ref m2, r, ref rIdx, ref x, ref y);
                RngStep(a << 2, ref a, ref b, mm, ref m, ref m2, r, ref rIdx, ref x, ref y);
                RngStep(a >> 16, ref a, ref b, mm, ref m, ref m2, r, ref rIdx, ref x, ref y);
            }

            _b = b;
            _a = a;
        }
    }

}
