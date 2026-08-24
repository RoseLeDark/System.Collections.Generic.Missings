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

using System.Numerics;

namespace SystemEx.Hash.Black {
	/// \addtogroup SystemEx.Hash.Black
	/// @{
	/// <summary>
	/// Internal utility helpers for the BLAKE3 hashing subsystem.
	/// 
	/// <para>
	/// This class provides constants, message schedules, bit operations,
	/// key loading, block loading, and chaining‑value storage helpers used
	/// by the portable scalar compression implementation.
	/// </para>
	/// 
	/// <para>
	/// All members are internal and intended only for use inside the
	/// SystemEx.Hash.Black3 module.
	/// </para>
	/// </summary>
	internal static class Black3Utils {
		/// <summary>
		/// Initialization vector (IV) used by BLAKE3.
		/// </summary>
		public static  UInt32[] IV = {  0x6A09E667, 0xBB67AE85, 0x3C6EF372, 0xA54FF53A,
                                        0x510E527F, 0x9B05688C, 0x1F83D9AB, 0x5BE0CD19 
                                     };
		/// <summary>
		/// Message schedule used for BLAKE3 rounds.
		/// </summary>
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
		/// <summary>
		/// Counts leading zeros in a 64‑bit value.
		/// </summary>
		public static int Clz ( ulong x ) {
            if ( x == 0 )
                return 64;

            return 63 - BitOperations.Log2(x);
        }
#endif
		/// <summary>
		/// Returns the index of the highest set bit.
		/// </summary>
		public static uint HighestOne ( UInt64 x ) => 63 ^ (uint)Clz(x);
		/// <summary>
		/// Rotates a 32‑bit word right by the given count.
		/// </summary>
		public static UInt32 Rotr32 ( UInt32 w, Int32 c ) => (w >> c) | (w << (32 - c));
		/// <summary>
		/// Rounds a value down to the nearest power of two.
		/// </summary>
		public static UInt64 RoundDown2Power2 ( UInt64 x ) => (ulong)(1 << (int)HighestOne(x | (ulong)1));
		/// <summary>
		/// Returns the low 32 bits of a 64‑bit counter.
		/// </summary>
		public static UInt32 CounterLow ( UInt64 counter ) => (UInt32)counter;
		/// <summary>
		/// Returns the high 32 bits of a 64‑bit counter.
		/// </summary>
		public static UInt32 CounterHigh ( UInt64 counter ) => (UInt32)(counter >> 32);
		/// <summary>
		/// Stores a 32‑bit word into a byte array in little‑endian order.
		/// </summary>
		public static void Store32 ( byte[] dst, UInt32 w ) => dst = w.ToBytes(Endian.LittleEndian);
		/// <summary>
		/// Counts the number of set bits in a 64‑bit value.
		/// </summary>
		public static uint PopCnt ( ulong x ) {
            uint count = 0;
            while ( x != 0 ) {
                count += 1;
                x &= x - 1;
            }
            return count;
        }

		/// <summary>
		/// Returns the internal BLAKE3 version string.
		/// </summary>
		public static string BLakeString => "1.8.5";
		/// <summary>
		/// Loads eight 32‑bit key words from a byte array.
		/// </summary>
		public static void LoadKeyWords ( byte[] key, uint[] key_words ) {
            key_words[0] = key.ToUInt(0, Endian.LittleEndian);
            key_words[1] = key.ToUInt(4, Endian.LittleEndian);
            key_words[2] = key.ToUInt(8, Endian.LittleEndian);
            key_words[3] = key.ToUInt(12, Endian.LittleEndian);
            key_words[4] = key.ToUInt(16, Endian.LittleEndian);
            key_words[5] = key.ToUInt(20, Endian.LittleEndian);
            key_words[6] = key.ToUInt(24, Endian.LittleEndian);
            key_words[7] = key.ToUInt(28, Endian.LittleEndian);

        }

		/// <summary>
		/// Loads sixteen 32‑bit block words from a byte array.
		/// </summary>
		public static void LoadBlockWords ( byte[] block, UInt32[] block_words ) {
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



		/// <summary>
		/// Stores eight chaining‑value words into a byte array in little‑endian order.
		/// </summary>
		public static void StoreCVWords ( byte[] bout, UInt32[] cv_words ) {
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
    /// @}
}
