using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SystemEx.Hash.impl {
    internal static class Black3CompressScalar {

        public static void in_place_portable ( uint[] cv, byte[] block, byte block_len, UInt64 counter, blake3_flags flags ) {
            uint[] state = new uint[16];

            compress_pre(state, cv, block, block_len, counter, flags);

            cv[0] = state[0] ^ state[8];
            cv[1] = state[1] ^ state[9];
            cv[2] = state[2] ^ state[10];
            cv[3] = state[3] ^ state[11];
            cv[4] = state[4] ^ state[12];
            cv[5] = state[5] ^ state[13];
            cv[6] = state[6] ^ state[14];
            cv[7] = state[7] ^ state[15];
        }
        public static void xof_portable ( uint[] cv, byte[] block, byte block_len, UInt64 counter, blake3_flags flags, byte[] uout ) {
            uint[] state = new uint[16];
            compress_pre(state, cv, block, block_len, counter, flags);

            // XOR + store32 (tempo, unrolled)
            byte[] x;

            x = (state[0] ^ state[8]).ToBytes(Endian.LittleEndian);
            uout[0] = x[0]; uout[1] = x[1]; uout[2] = x[2]; uout[3] = x[3];

            x = (state[1] ^ state[9]).ToBytes(Endian.LittleEndian);
            uout[4] = x[0]; uout[5] = x[1]; uout[6] = x[2]; uout[7] = x[3];

            x = (state[2] ^ state[10]).ToBytes(Endian.LittleEndian);
            uout[8] = x[0]; uout[9] = x[1]; uout[10] = x[2]; uout[11] = x[3];

            x = (state[3] ^ state[11]).ToBytes(Endian.LittleEndian);
            uout[12] = x[0]; uout[13] = x[1]; uout[14] = x[2]; uout[15] = x[3];

            x = (state[4] ^ state[12]).ToBytes(Endian.LittleEndian);
            uout[16] = x[0]; uout[17] = x[1]; uout[18] = x[2]; uout[19] = x[3];

            x = (state[5] ^ state[13]).ToBytes(Endian.LittleEndian);
            uout[20] = x[0]; uout[21] = x[1]; uout[22] = x[2]; uout[23] = x[3];

            x = (state[6] ^ state[14]).ToBytes(Endian.LittleEndian);
            uout[24] = x[0]; uout[25] = x[1]; uout[26] = x[2]; uout[27] = x[3];

            x = (state[7] ^ state[15]).ToBytes(Endian.LittleEndian);
            uout[28] = x[0]; uout[29] = x[1]; uout[30] = x[2]; uout[31] = x[3];

            x = (state[8] ^ cv[0]).ToBytes(Endian.LittleEndian);
            uout[32] = x[0]; uout[33] = x[1]; uout[34] = x[2]; uout[35] = x[3];

            x = (state[9] ^ cv[1]).ToBytes(Endian.LittleEndian);
            uout[36] = x[0]; uout[37] = x[1]; uout[38] = x[2]; uout[39] = x[3];

            x = (state[10] ^ cv[2]).ToBytes(Endian.LittleEndian);
            uout[40] = x[0]; uout[41] = x[1]; uout[42] = x[2]; uout[43] = x[3];

            x = (state[11] ^ cv[3]).ToBytes(Endian.LittleEndian);
            uout[44] = x[0]; uout[45] = x[1]; uout[46] = x[2]; uout[47] = x[3];

            x = (state[12] ^ cv[4]).ToBytes(Endian.LittleEndian);
            uout[48] = x[0]; uout[49] = x[1]; uout[50] = x[2]; uout[51] = x[3];

            x = (state[13] ^ cv[5]).ToBytes(Endian.LittleEndian);
            uout[52] = x[0]; uout[53] = x[1]; uout[54] = x[2]; uout[55] = x[3];

            x = (state[14] ^ cv[6]).ToBytes(Endian.LittleEndian);
            uout[56] = x[0]; uout[57] = x[1]; uout[58] = x[2]; uout[59] = x[3];

            x = (state[15] ^ cv[7]).ToBytes(Endian.LittleEndian);
            uout[60] = x[0]; uout[61] = x[1]; uout[62] = x[2]; uout[63] = x[3];

        }

        public static void shift_input ( byte[] input, int len, ref int remain ) {
            remain = len - 64;

            if ( remain > 0 ) {
                for ( int i = 0 ; i < remain ; i++ )
                    input[i] = input[i + 64];

                for ( int i = remain ; i < len ; i++ )
                    input[i] = 0;
            } else {
                for ( int i = 0 ; i < len ; i++ )
                    input[i] = 0;
            }
        }


        public static void hash_one_portable ( byte[] input, UInt64 blocks, UInt32[] key, UInt64 counter, blake3_flags flags, blake3_flags flags_start, blake3_flags flags_end, byte[] bout ) {
            uint[] cv = new uint[Black3Infos.BLAKE3_KEY_LEN];
            byte n = 0;

            while ( n < Black3Infos.BLAKE3_KEY_LEN ) {
                cv[n] = key[n];
                n++;
            }


            blake3_flags block_flags = flags | flags_start;
            int remaining = 0;

            while ( blocks > 0 ) {
                if ( blocks == 1 ) {
                    block_flags |= flags_end;
                }
                in_place_portable(cv, input, Black3Infos.BLAKE3_BLOCK_LEN, counter, block_flags);
                // input = &input[BLAKE3_BLOCK_LEN];
                /*int total = input.Length;
                int remaining = total - 64;

                if ( remaining > 0 ) {
                    // byte-für-byte nach vorne schieben
                    for ( int i = 0 ; i < remaining ; i++ )
                        input[i] = input[i + 64];

                    // alte Reste am Ende löschen (optional, aber sauber)
                    for ( int i = remaining ; i < total ; i++ )
                        input[i] = 0;
                } else {
                    // Keine verbleibenden Bytes: fülle das Array mit Nullen
                    for ( int i = 0 ; i < total ; i++ )
                        input[i] = 0;
                }*/
                shift_input(input, input.Length, ref remaining);

                blocks -= 1;
                block_flags = flags;
            }
            Black3Utils.store_cv_words(bout, cv);
        }
        public static List<byte[]> blake3_hash_many_portable ( List<byte[]> input,
                               UInt64 blocks, uint[] key, UInt64 counter, bool increment_counter, blake3_flags flags, blake3_flags flags_start,
                               blake3_flags flags_end ) {

            List<byte[]> _ret = new List<byte[]>(input.Count);



            for ( int i = 0 ; i < input.Count ; i++ ) {
                byte[] bout = new byte[Black3Infos.BLAKE3_BLOCK_LEN];

                hash_one_portable(input[i], blocks, key, counter, flags, flags_start, flags_end, bout);

                if ( increment_counter ) {
                    counter += 1;
                }

                _ret.Add(bout);
            }
            return _ret;
        }

        public static void g ( uint[] state, UInt64 a, UInt64 b, UInt64 c, UInt64 d, uint x, uint y ) {
            state[a] = state[a] + state[b] + x;
            state[d] = Black3Utils.rotr32(state[d] ^ state[a], 16);
            state[c] = state[c] + state[d];
            state[b] = Black3Utils.rotr32(state[b] ^ state[c], 12);
            state[a] = state[a] + state[b] + y;
            state[d] = Black3Utils.rotr32(state[d] ^ state[a], 8);
            state[c] = state[c] + state[d];
            state[b] = Black3Utils.rotr32(state[b] ^ state[c], 7);

        }

        public static void round_fn ( uint[] state, uint[] msg, UInt64 round ) {
            // Mix the columns.
            g(state, 0, 4, 8, 12, msg[Black3Utils.MSG_SCHEDULE[round, 0]], msg[Black3Utils.MSG_SCHEDULE[round, 1]]);
            g(state, 1, 5, 9, 13, msg[Black3Utils.MSG_SCHEDULE[round, 2]], msg[Black3Utils.MSG_SCHEDULE[round, 3]]);
            g(state, 2, 6, 10, 14, msg[Black3Utils.MSG_SCHEDULE[round, 4]], msg[Black3Utils.MSG_SCHEDULE[round, 5]]);
            g(state, 3, 7, 11, 15, msg[Black3Utils.MSG_SCHEDULE[round, 6]], msg[Black3Utils.MSG_SCHEDULE[round, 7]]);

            // Mix the rows.
            g(state, 0, 5, 10, 15, msg[Black3Utils.MSG_SCHEDULE[round, 8]], msg[Black3Utils.MSG_SCHEDULE[round, 9]]);
            g(state, 1, 6, 11, 12, msg[Black3Utils.MSG_SCHEDULE[round, 10]], msg[Black3Utils.MSG_SCHEDULE[round, 11]]);
            g(state, 2, 7, 8, 13, msg[Black3Utils.MSG_SCHEDULE[round, 12]], msg[Black3Utils.MSG_SCHEDULE[round, 13]]);
            g(state, 3, 4, 9, 14, msg[Black3Utils.MSG_SCHEDULE[round, 14]], msg[Black3Utils.MSG_SCHEDULE[round, 15]]);
        }

        public static void compress_pre ( uint[] state, uint[] cv, byte[] block, byte block_len, UInt64 counter, blake3_flags flags ) {
            uint[] block_words = new uint[16];

            block_words[0] = block.ToUInt(0, Endian.LittleEndian);
            block_words[1] = block.ToUInt(4, Endian.LittleEndian);
            block_words[2] = block.ToUInt(8, Endian.LittleEndian);
            block_words[3] = block.ToUInt(12, Endian.LittleEndian);
            block_words[4] = block.ToUInt(16, Endian.LittleEndian);
            block_words[5] = block.ToUInt(20, Endian.LittleEndian);
            block_words[6] = block.ToUInt(24, Endian.LittleEndian);
            block_words[7] = block.ToUInt(28, Endian.LittleEndian);
            block_words[8] = block.ToUInt(32, Endian.LittleEndian);
            block_words[9] = block.ToUInt(36, Endian.LittleEndian);
            block_words[10] = block.ToUInt(40, Endian.LittleEndian);
            block_words[11] = block.ToUInt(44, Endian.LittleEndian);
            block_words[12] = block.ToUInt(48, Endian.LittleEndian);
            block_words[13] = block.ToUInt(52, Endian.LittleEndian);
            block_words[14] = block.ToUInt(56, Endian.LittleEndian);
            block_words[15] = block.ToUInt(60, Endian.LittleEndian);

            state[0] = cv[0];
            state[1] = cv[1];
            state[2] = cv[2];
            state[3] = cv[3];
            state[4] = cv[4];
            state[5] = cv[5];
            state[6] = cv[6];
            state[7] = cv[7];
            state[8] = (uint)Black3Utils.IV[0];
            state[9] = (uint)Black3Utils.IV[1];
            state[10] = (uint)Black3Utils.IV[2];
            state[11] = (uint)Black3Utils.IV[3];
            state[12] = Black3Utils.counter_low(counter);
            state[13] = Black3Utils.counter_high(counter);
            state[14] = (uint)block_len;
            state[15] = (uint)flags;

            round_fn(state, block_words, 0);
            round_fn(state, block_words, 1);
            round_fn(state, block_words, 2);
            round_fn(state, block_words, 3);
            round_fn(state, block_words, 4);
            round_fn(state, block_words, 5);
            round_fn(state, block_words, 6);
        }

        
    }
}
