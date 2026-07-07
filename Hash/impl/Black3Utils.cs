using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SystemEx.Hash.impl {
    internal static class Black3Utils {
        public static  UInt32[] IV = {  0x6A09E667, 0xBB67AE85, 0x3C6EF372, 0xA54FF53A,
                                        0x510E527F, 0x9B05688C, 0x1F83D9AB, 0x5BE0CD19 
                                     };

        public static byte[,] MSG_SCHEDULE = {
            {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15},
            {2, 6, 3, 10, 7, 0, 4, 13, 1, 11, 12, 5, 9, 14, 15, 8},
            {3, 4, 10, 12, 13, 2, 7, 14, 6, 5, 9, 0, 11, 15, 8, 1},
            {10, 7, 12, 9, 14, 3, 13, 15, 4, 0, 11, 2, 5, 8, 1, 6},
            {12, 13, 9, 11, 15, 10, 14, 8, 7, 2, 5, 3, 0, 1, 6, 4},
            {9, 14, 11, 5, 8, 12, 15, 1, 13, 3, 0, 10, 2, 6, 4, 7},
            {11, 15, 5, 0, 1, 9, 8, 6, 14, 10, 2, 12, 3, 4, 7, 13},
        }; // 7,16
#if ONLY_STATS
        public static int Clz ( ulong x ) {
            if ( x == 0 ) return 64;

            int n = 0;
            if ( (x & 0xFFFFFFFF00000000) == 0 ) { n += 32; x <<= 32; }
            if ( (x & 0xFFFF000000000000) == 0 ) { n += 16; x <<= 16; }
            if ( (x & 0xFF00000000000000) == 0 ) { n += 8; x <<= 8; }
            if ( (x & 0xF000000000000000) == 0 ) { n += 4; x <<= 4; }
            if ( (x & 0xC000000000000000) == 0 ) { n += 2; x <<= 2; }
            if ( (x & 0x8000000000000000) == 0 ) { n += 1; }

            return n;
        }
#else
        public static int Clz ( ulong x ) {
            if ( x == 0 )
                return 64;

            return 63 - BitOperations.Log2(x);
        }
#endif

        public static uint highest_one ( UInt64 x ) => 63 ^ (uint)Clz(x);
        public static UInt32 rotr32 ( UInt32 w, Int32 c ) => (w >> c) | (w << (32 - c));
        public static UInt64 round_down_to_power_of_2 ( UInt64 x ) => (ulong)(1 << (int)highest_one(x | (ulong)1));
        public static UInt32 counter_low ( UInt64 counter ) => (UInt32)counter;

        public static UInt32 counter_high ( UInt64 counter ) => (UInt32)(counter >> 32);

        public static void store32 ( byte[] dst, UInt32 w ) => dst = w.ToBytes(Endian.LittleEndian);
        public static uint popcnt ( ulong x ) {
            uint count = 0;
            while ( x != 0 ) {
                count += 1;
                x &= x - 1;
            }
            return count;
        }


        public static string BLakeString => "1.8.5";

        public static void load_key_words ( byte[] key, uint[] key_words ) {
            key_words[0] = key.ToUInt(0, Endian.LittleEndian);
            key_words[1] = key.ToUInt(4, Endian.LittleEndian);
            key_words[2] = key.ToUInt(8, Endian.LittleEndian);
            key_words[3] = key.ToUInt(12, Endian.LittleEndian);
            key_words[4] = key.ToUInt(16, Endian.LittleEndian);
            key_words[5] = key.ToUInt(20, Endian.LittleEndian);
            key_words[6] = key.ToUInt(24, Endian.LittleEndian);
            key_words[7] = key.ToUInt(28, Endian.LittleEndian);

        }


        public static void load_block_words ( byte[] block, UInt32[] block_words ) {
            block_words[0] = block.ToUInt(0, Endian.LittleEndian);// load32(&block[i * 4]);
            block_words[1] = block.ToUInt(4, Endian.LittleEndian);// load32(&block[i * 4]);
            block_words[2] = block.ToUInt(8, Endian.LittleEndian);// load32(&block[i * 4]);
            block_words[3] = block.ToUInt(12, Endian.LittleEndian);// load32(&block[i * 4]);
            block_words[4] = block.ToUInt(16, Endian.LittleEndian);// load32(&block[i * 4]);
            block_words[5] = block.ToUInt(20, Endian.LittleEndian);// load32(&block[i * 4]);
            block_words[6] = block.ToUInt(24, Endian.LittleEndian);// load32(&block[i * 4]);
            block_words[7] = block.ToUInt(28, Endian.LittleEndian);// load32(&block[i * 4]);
            block_words[8] = block.ToUInt(32, Endian.LittleEndian);// load32(&block[i * 4]);
            block_words[9] = block.ToUInt(36, Endian.LittleEndian);// load32(&block[i * 4]);
            block_words[10] = block.ToUInt(40, Endian.LittleEndian);// load32(&block[i * 4]);
            block_words[11] = block.ToUInt(44, Endian.LittleEndian);// load32(&block[i * 4]);
            block_words[12] = block.ToUInt(48, Endian.LittleEndian);// load32(&block[i * 4]);
            block_words[13] = block.ToUInt(52, Endian.LittleEndian);// load32(&block[i * 4]);
            block_words[14] = block.ToUInt(56, Endian.LittleEndian);// load32(&block[i * 4]);
            block_words[15] = block.ToUInt(60, Endian.LittleEndian);// load32(&block[i * 4]);
        }




        public static void store_cv_words ( byte[] bout, UInt32[] cv_words ) {
            for ( int i = 0 ; i < 8 ; i++ ) {
                byte[] x = cv_words[i].ToBytes(Endian.LittleEndian);
                var o = i * 4;

                bout[o    ] = x[0];
                bout[o + 1] = x[1];
                bout[o + 2] = x[2];
                bout[o + 3] = x[3];
            }
        }
    }
}
