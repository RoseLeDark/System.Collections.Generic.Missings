using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Utils {
    public static class BitUtils {

        public static bool GetBit(this byte value, short pos)
            => ((value >> pos) & 1) != 0;
        public static bool GetBit(this short value, short pos)
            => ((value >> pos) & 1) != 0;
        public static bool GetBit(this int value, int pos)
            => ((value >> pos) & 1) != 0;

        public static bool GetBit(this long value, int pos)
            => ((value >> pos) & 1L) != 0;

        public static bool GetBit(this ushort value, short pos)
            => ((value >> pos) & 1) != 0;
        public static bool GetBit(this uint value, int pos)
            => ((value >> pos) & 1U) != 0;

        public static bool GetBit(this ulong value, int pos)
            => ((value >> pos) & 1UL) != 0;

        public static byte SetBit(this byte value, bool bit, byte pos) {
            return (byte)(bit ? (byte)value | (1 << pos) : (byte)(value & ~(1 << pos)));
        }
        public static short SetBit(this short value, bool bit, int pos) {
            ushort mask = (ushort)(1 << pos);
            return (short)(bit ? (ushort)((ushort)value | mask) : (ushort)(value & ~mask));
        }

        public static int SetBit(this int value, bool bit, int pos)
            => bit ? (value | (1 << pos)) : (value & ~(1 << pos));
        public static long SetBit(this long value, bool bit, int pos ) {
            long mask = 1L << pos;
            return bit ? (value | mask) : (value & ~mask);
        }

        public static short SetBit(this ushort value, bool bit, short pos) {
            return (short)(bit ? (int)value | (1 << pos) : (short)(value & ~(1 << pos)));
        }
        public static uint SetBit(this uint value, bool bit, int pos) {
            uint  mask = 1U << pos;
            return bit ? (value | mask) : (value & ~mask);
        }
        public static ulong SetBit(this ulong value, bool bit, int pos) {
            ulong mask = 1UL << pos;
            return bit ? (value | mask) : (value & ~mask);
        }

        public static int MaskRange(int start, int length) {
            if ( length <= 0 ) return 0;
            if ( start < 0 || start > 31 ) return 0;
            if ( length >= 32 ) return unchecked((int)0xFFFFFFFF);

            int mask = (1 << length) - 1;
            return mask << start;
        }

        public static int RotateLeft(this byte value, int count) {
            count &= 7; // modulo 8
            uint v = (uint)value;
            return (int)((v << count) | (v >> (8 - count)));
        }
        public static int RotateLeft(this short value, int count) {
            count &= 15; // modulo 15
            uint v = (uint)value;
            return (int)((v << count) | (v >> (16 - count)));
        }
        public static int RotateLeft(this int value, int count) {
            count &= 31; // modulo 32
            uint v = (uint)value;
            return (int)((v << count) | (v >> (32 - count)));
        }
        public static long RotateLeft(this long value, int count) {
            count &= 63;
            ulong v = (ulong)value;
            return (long)((v << count) | (v >> (64 - count)));
        }



        public static long RotateRight(this byte value, int count) {
            count &= 7;
            ulong v = (ulong)value;
            return (long)((v >> count) | (v << (8 - count)));
        }
        public static long RotateRight(this short value, int count) {
            count &= 15;
            ulong v = (ulong)value;
            return (long)((v >> count) | (v << (16 - count)));
        }
        public static int RotateRight(this int value, int count) {
            count &= 31;
            uint v = (uint)value;
            return (int)((v >> count) | (v << (32 - count)));
        }
        public static long RotateRight(this long value, int count) {
            count &= 63;
            ulong v = (ulong)value;
            return (long)((v >> count) | (v << (64 - count)));
        }

        public static byte FlipBit(this byte value, int pos)
            => (byte)(value ^ (1 << pos));

        public static short FlipBit(this short value, int pos)
            => (short)(value ^ (1 << pos));
        public static int FlipBit(this int value, int pos)
            => value ^ (1 << pos);

        public static long FlipBit(this long value, int pos)
            => value ^ (1L << pos);

        public static uint FlipBit(this uint value, int pos)
            => value ^ (1U << pos);

        public static ushort FlipBit(this ushort value, int pos)
            => (ushort)(value ^ (1 << pos));

        public static ulong FlipBit(this ulong value, int pos)
            => value ^ (1UL << pos);

    }
}
