using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Random {
    public sealed class Randx {
        private readonly Isaac32Engine _core;

        public Randx ( uint seedA = 0, uint seedB = 0, uint seedC = 0 ) {
            _core = new Isaac32Engine(seedA, seedB, seedC);
        }

        public uint Next32 () {
            return _core.Next();
        }

        public ulong Next64 () {
            // Zwei ISAAC‑Werte kombinieren → stabil, deterministisch
            ulong hi = _core.Next();
            ulong lo = _core.Next();
            return (hi << 32) | lo;
        }

        public byte NextByte () {
            return (byte)(_core.Next() & 0xFF);
        }

        public void NextBytes ( byte[] buffer ) {
            for ( int i = 0 ; i < buffer.Length ; i++ )
                buffer[i] = NextByte();
        }

        public uint Next ( uint min, uint max ) {
            uint r = Next32();
            return min + (r % (max - min));
        }

        public ulong Next ( ulong min, ulong max ) {
            ulong r = Next64();
            return min + (r % (max - min));
        }

        public char NextChar () {
            return (char)(Next32() & 0xFF);
        }

        public string NextString ( int length ) {
            char[] c = new char[length];
            for ( int i = 0 ; i < length ; i++ )
                c[i] = NextChar();
            return new string(c);
        }

        public uint NextHashSeed32 () {
            return Next32();
        }

        public ulong NextHashSeed64 () {
            return Next64();
        }
    }
}
