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

namespace SystemEx.Random {
	/// \addtogroup SystemEx.Random
	/// @{
	/// <summary>
	/// Represents the ISAAC 32-bit random number generator.
	/// </summary>
	public sealed class Isaac32Engine {
        private const uint GoldenRatio = 0x9e3779b9u; // dein TGoldenRatio für 32 Bit
        private const int Size = 256;

        private uint m_cnt;
        private readonly uint[] m_rsl = new uint[Size];
        private readonly uint[] m_mem = new uint[Size];
        private uint m_a, m_b, m_c;

        /// <summary>
        /// Initializes a new instance of the <see cref="Isaac32Engine"/> class using
        /// a seed object implementing <see cref="ISeed"/>. The first three values of
        /// the seed are used as the primary ISAAC seeds (a, b, c). Any additional
        /// values are forwarded as the optional seed array s in Seed(a, b, c, s) >.
        /// </summary>
        /// <param name="seed">
        /// The seed object used to initialize the engine. If the seed contains more
        /// than three values, the remaining values are passed as the seed array.
        /// </param>
        public Isaac32Engine (ISeed seed) {
            var arr = seed.GetSeed();
            int len = arr.Length;

            uint a = len > 0 ? arr[0] : 0;
            uint b = len > 1 ? arr[1] : 0;
            uint c = len > 2 ? arr[2] : 0;

            uint[]? s = null;

            if ( len > 3 ) {
                s = new uint[len - 3];
                Array.Copy(arr, 3, s, 0, s.Length);
            }

            Seed(a, b, c, s);
        }
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
               m_rsl[i] = s != null ? s[i] : 0;

            m_a = a;
            m_b = b;
            m_c = c;

            Shuffle(ref aa, ref bb, ref cc, ref dd, ref ee, ref ff, ref gg, ref hh);
            Shuffle(ref aa, ref bb, ref cc, ref dd, ref ee, ref ff, ref gg, ref hh);
            Shuffle(ref aa, ref bb, ref cc, ref dd, ref ee, ref ff, ref gg, ref hh);
            Shuffle(ref aa, ref bb, ref cc, ref dd, ref ee, ref ff, ref gg, ref hh);

            for ( int i = 0 ; i < Size ; i += 8 ) {
                aa +=m_rsl[i + 0]; bb +=m_rsl[i + 1]; cc +=m_rsl[i + 2]; dd +=m_rsl[i + 3];
                ee +=m_rsl[i + 4]; ff +=m_rsl[i + 5]; gg +=m_rsl[i + 6]; hh +=m_rsl[i + 7];

                Shuffle(ref aa, ref bb, ref cc, ref dd, ref ee, ref ff, ref gg, ref hh);

                m_mem[i + 0] = aa; m_mem[i + 1] = bb; m_mem[i + 2] = cc; m_mem[i + 3] = dd;
                m_mem[i + 4] = ee; m_mem[i + 5] = ff; m_mem[i + 6] = gg; m_mem[i + 7] = hh;
            }

            for ( int i = 0 ; i < Size ; i += 8 ) {
                aa += m_mem[i + 0]; bb += m_mem[i + 1]; cc += m_mem[i + 2]; dd += m_mem[i + 3];
                ee += m_mem[i + 4]; ff += m_mem[i + 5]; gg += m_mem[i + 6]; hh += m_mem[i + 7];

                Shuffle(ref aa, ref bb, ref cc, ref dd, ref ee, ref ff, ref gg, ref hh);

                m_mem[i + 0] = aa; m_mem[i + 1] = bb; m_mem[i + 2] = cc; m_mem[i + 3] = dd;
                m_mem[i + 4] = ee; m_mem[i + 5] = ff; m_mem[i + 6] = gg; m_mem[i + 7] = hh;
            }

            setup();
            m_cnt = Size - 1;
        }

        /// <summary>
        /// Generates the next random number in the sequence.
        /// </summary>
        /// <returns>The next random number.</returns>  
        public uint Next () {
            if ( m_cnt == 0 ) {
                setup();
                m_cnt = Size - 1;
                return m_rsl[m_cnt];
            }

            return m_rsl[m_cnt--];
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
        private void setup () {
            uint x = 0, y = 0;
            uint[] mm = m_mem;
            uint[] r =m_rsl;

            uint a = m_a;
            uint b = m_b + (++m_c);

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

            m_b = b;
            m_a = a;
        }
    }
	///@}
}
