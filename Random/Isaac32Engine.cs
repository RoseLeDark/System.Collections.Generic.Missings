using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Random {
    public sealed class Isaac32Engine {
        private const uint GoldenRatio = 0x9e3779b9u; // dein TGoldenRatio für 32 Bit
        private const int Size = 256;

        private uint _cnt;
        private readonly uint[] _rsl = new uint[Size];
        private readonly uint[] _mem = new uint[Size];
        private uint _a, _b, _c;

        public Isaac32Engine ( uint a = 0, uint b = 0, uint c = 0 ) {
            Seed(a, b, c, null);
        }

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

        public uint Next () {
            if ( _cnt == 0 ) {
                Isaac();
                _cnt = Size - 1;
                return _rsl[_cnt];
            }

            return _rsl[_cnt--];
        }

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

        private static uint Ind ( uint[] mm, uint x ) {
            // 32‑Bit: Index = (x & (255 << 2)) / 4
            int idx = (int)((x & (255u << 2)) >> 2);
            return mm[idx];
        }

        private static void RngStep ( uint mix, ref uint a, ref uint b,
                                    uint[] mm, ref int m, ref int m2,
                                    uint[] r, ref int rIdx, ref uint x, ref uint y ) {
            x = mm[m];
            a = (a ^ mix) + mm[m2++];
            y = mm[m] = Ind(mm, x) + a + b;
            r[rIdx++] = b = Ind(mm, y >> 8) + x;
            m++;
        }

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
