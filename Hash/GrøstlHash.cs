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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using SystemEx.Collections.Generic;
using static System.Reflection.Metadata.BlobBuilder;
using static SystemEx.Hash.GrøstlHash;

namespace SystemEx.Hash {
	/// \addtogroup Hash
	/// @{

	/// <summary>
	/// Implements the Grøstl-256 and Grøstl-512 hashing functions as part of
	/// the SystemEx low-level cryptographic subsystem. This class provides a
	/// fully self-contained, allocation-free, deterministic hash engine based
	/// on the original Grøstl specification (SHA-3 finalist).
	///
	/// The implementation is designed for systems programming scenarios where
	/// predictable behavior, portability, and runtime independence are required.
	/// All operations are performed using fixed-size buffers, explicit state
	/// transformations, and constant-time primitives where applicable.
	///
	/// GroestlHash exposes two public hashing interfaces:
	/// <list type="bullet">
	/// <item><description><see cref="Compute"/> — Produces a 32-bit hash derived from the full Grøstl-256 digest.</description></item>
	/// <item><description><see cref="ComputeLong"/> — Produces a 64-bit hash derived from the full Grøstl-512 digest.</description></item>
	/// </list>
	///
	/// Both variants support endian-aware extraction and optional seed mixing,
	/// enabling integration into heterogeneous environments, custom keying
	/// schemes, and internal hashing pipelines.
	///
	/// This class is intended for:
	/// <list type="bullet">
	/// <item><description>Deterministic hashing inside low-level frameworks</description></item>
	/// <item><description>Internal key generation and seeding mechanisms</description></item>
	/// <item><description>Data integrity checks and non-cryptographic fingerprinting</description></item>
	/// <item><description>Embedded or isolated verification modules</description></item>
	/// </list>
	/// </summary>
	public sealed class GrøstlHash : IHash {
        internal  struct PaddingState {
            internal byte BytesInBlock;
            internal byte FirstPaddingBlock;
            internal byte Last_PaddingBlock;
        };

        Endian m_endian;
        /// <summary>
        /// Craate a new instance
        /// </summary>
        /// <param name="endian">The suing endian for creating a hash</param>
        public GrøstlHash ( Endian endian ) {
            m_endian = endian;
        }

        #region HELPER
        /// <summary>
        /// XORs two arrays of uints in place. The destination array is modified to contain the result of the XOR operation.
        /// </summary>
        /// <param name="dest">The destination array.</param>
        /// <param name="src">The source array.</param>
        /// <param name="n">The number of elements to XOR.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void MemXor ( uint[] dest, uint[] src, int n ) {
            uint i = 0;

            while ( n-- > 0 ) {
                dest[i] ^= src[i];
                i++;
            }
        }
        /// <summary>
        /// Sets the message for the Grøstl hash computation.
        /// </summary>
        /// <param name="buffer">The buffer to set the message in.</param>
        /// <param name="input">The input data.</param>
        /// <param name="s">The padding state.</param>
        /// <param name="inlen">The length of the input data.</param>
        /// <param name="STATEBYTES">The number of bytes in each state block.</param>
        /// <param name="STATECOLS">The number of columns in each state block.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetMessage ( uint[] buffer, FixedVector<byte> input, PaddingState s, ulong inlen, uint STATEBYTES, uint STATECOLS ) {

            uint i = 0;
            for ( i = 0 ; i < s.BytesInBlock ; i++ )
                buffer[BYTESLICE(i, (int)STATECOLS)] = input[(int)i];

            if ( s.BytesInBlock != STATEBYTES ) {
                if ( s.FirstPaddingBlock == 1 ) {
                    buffer[BYTESLICE(i, (int)STATECOLS)] = 0x80;
                    i++;
                }

                for ( ; i < STATEBYTES ; i++ )
                    buffer[BYTESLICE(i, (int)STATECOLS)] = 0;

                if ( s.Last_PaddingBlock == 1 ) {
                    ulong blocks = inlen / STATEBYTES;

                    blocks += (uint)((s.FirstPaddingBlock == s.Last_PaddingBlock) ? 1 : 2);

                    for ( int j = (int)STATEBYTES - 8 ; j < STATEBYTES ; j++ ) {
                        int pos = (int)STATEBYTES - j - 1;

                        buffer[BYTESLICE((uint)j, (int)STATECOLS)] = (byte)((blocks >> (8 * pos)) & 0xFF);
                    }
                }
            }
        }
        /// <summary>
        /// Multiplies a uint by 2 in the Galois field GF(2^8).
        /// </summary>
        /// <param name="x">The uint to multiply.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Mul2 ( ref uint x ) {
            uint t = x & 0x80808080u;   // extract MSBs of each byte
            x ^= t;                     // flip MSBs before shift
            x <<= 1;                    // shift left (multiply by 2)
            t >>= 7;                    // move MSBs into LSB position
            t ^= (t << 1);              // GF(2^8) reduction step
            x ^= t;                     // apply reduction
            x ^= (t << 3);              // final reduction term
        }
            /// <summary>
            /// Calculates the slice index for a given input and state.
            /// </summary>
            /// <param name="input">The input value.</param>
            /// <param name="state">The state value.</param>
            /// <returns>The calculated slice index.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint BYTESLICE ( uint input, int state  ) {
            //#define BYTESLICE(i) (((i)%8)*STATECOLS+(i)/8)
            return  ( ((input) % 8) * (uint)state + (input) / 8);
        }
        private static void MixBytes ( uint[,] a, uint[] b, int s ) {
            int i;
            uint t0, t1, t2;

            // b[i] = a[i][s]
            for ( i = 0 ; i < 8 ; i++ )
                b[i] = a[i, s];

            // y_i = a_{i+6}
            for ( i = 0 ; i < 8 ; i++ )
                a[i, s] = b[(i + 2) % 8];

            // t_i = a_i + a_{i+1}
            for ( i = 0 ; i < 7 ; i++ )
                b[i] ^= b[(i + 1) % 8];
            b[7] ^= a[6, s];

            // y_i = a_{i+6} + t_i
            for ( i = 0 ; i < 8 ; i++ )
                a[i, s] ^= b[(i + 4) % 8];

            // y_i = y_i + t_{i+2}
            for ( i = 0 ; i < 8 ; i++ )
                a[i, s] ^= b[(i + 6) % 8];

            // x_i = t_i + t_{i+3}
            t0 = b[0];
            t1 = b[1];
            t2 = b[2];

            for ( i = 0 ; i < 5 ; i++ )
                b[i] ^= b[(i + 3) % 8];

            b[5] ^= t0;
            b[6] ^= t1;
            b[7] ^= t2;

            // z_i = 02 * x_i
            for ( i = 0 ; i < 8 ; i++ )
                Mul2(ref b[i]);

            // w_i = z_i + y_{i+4}
            for ( i = 0 ; i < 8 ; i++ )
                b[i] ^= a[i, s];

            // v_i = 02 * w_i
            for ( i = 0 ; i < 8 ; i++ )
                Mul2(ref b[i]);

            // b_i = v_{i+3} + y_{i+4}
            for ( i = 0 ; i < 8 ; i++ )
                a[i, s] ^= b[(i + 3) % 8];
        }

        #endregion


        #region Permutation

        /// <summary>
        /// Performs the permutation operation for the Grøstl hash computation.
        /// </summary>
        /// <param name="x">The input array.</param>
        /// <param name="q">The padding state.</param>
        /// <param name="state">The state value.</param>
        /// <param name="cols">The number of columns in each state block.</param>
        /// <param name="rR">The number of rounds.</param>
        /// <param name="shift">The shift values.</param>
        /// <param name="ColumnConstant">The column constants.</param>
        private static void Permutation ( uint[] x, int q, int state, int cols, int rR, byte[,] shift, uint[] ColumnConstant ) {
            uint[] tmp = new uint[cols];
            uint constant = 0;

            for ( int round = 0 ; round < rR ; round++ ) {
                constant += 0x01010101u;

                // AddRoundConstant
                if ( q == 0 ) {
                    for ( int j = 0 ; j < ColumnConstant.Length ; j++ )
                        x[j] ^= ColumnConstant[j] ^ constant;
                } else {
                    for ( int i = 0 ; i < state ; i++ )
                        x[i] = ~x[i];

                    for ( int j = 0 ; j < ColumnConstant.Length ; j++ )
                        x[state - ColumnConstant.Length + j] ^= ColumnConstant[j] ^ constant;
                }

                // SubBytes + ShiftBytes
                for ( int row = 0 ; row < 8 ; row++ ) {
                    int baseIndex = row * cols;

                    // Load row
                    for ( int col = 0 ; col < cols ; col++ )
                        tmp[col] = x[baseIndex + col];

                    // Apply S-box + shift
                    for ( int col = 0 ; col < cols ; col++ ) {
                        int idx = (col + shift[q, row]) % cols;
                        byte b = (byte)(tmp[idx] & 0xFF);
                        x[baseIndex + col] = (x[baseIndex + col] & 0xFFFFFF00u) | Ss[b];
                    }
                }

                // MixBytes
                uint[,] mat = new uint[8, cols];
                for ( int r = 0 ; r < 8 ; r++ )
                    for ( int c = 0 ; c < cols ; c++ )
                        mat[r, c] = x[r * cols + c];

                for ( int c = 0 ; c < cols ; c++ )
                    MixBytes(mat, tmp, c);

                for ( int r = 0 ; r < 8 ; r++ )
                    for ( int c = 0 ; c < cols ; c++ )
                        x[r * cols + c] = mat[r, c];
            }
        }
        #endregion


        #region TABLE
        /// <summary>
        /// Gets the S-box values for the Grøstl hash computation.
        /// </summary>
        private static readonly byte[] Ss = {
            0x63, 0x7c, 0x77, 0x7b, 0xf2, 0x6b, 0x6f, 0xc5,
            0x30, 0x01, 0x67, 0x2b, 0xfe, 0xd7, 0xab, 0x76,
            0xca, 0x82, 0xc9, 0x7d, 0xfa, 0x59, 0x47, 0xf0,
            0xad, 0xd4, 0xa2, 0xaf, 0x9c, 0xa4, 0x72, 0xc0,
            0xb7, 0xfd, 0x93, 0x26, 0x36, 0x3f, 0xf7, 0xcc,
            0x34, 0xa5, 0xe5, 0xf1, 0x71, 0xd8, 0x31, 0x15,
            0x04, 0xc7, 0x23, 0xc3, 0x18, 0x96, 0x05, 0x9a,
            0x07, 0x12, 0x80, 0xe2, 0xeb, 0x27, 0xb2, 0x75,
            0x09, 0x83, 0x2c, 0x1a, 0x1b, 0x6e, 0x5a, 0xa0,
            0x52, 0x3b, 0xd6, 0xb3, 0x29, 0xe3, 0x2f, 0x84,
            0x53, 0xd1, 0x00, 0xed, 0x20, 0xfc, 0xb1, 0x5b,
            0x6a, 0xcb, 0xbe, 0x39, 0x4a, 0x4c, 0x58, 0xcf,
            0xd0, 0xef, 0xaa, 0xfb, 0x43, 0x4d, 0x33, 0x85,
            0x45, 0xf9, 0x02, 0x7f, 0x50, 0x3c, 0x9f, 0xa8,
            0x51, 0xa3, 0x40, 0x8f, 0x92, 0x9d, 0x38, 0xf5,
            0xbc, 0xb6, 0xda, 0x21, 0x10, 0xff, 0xf3, 0xd2,
            0xcd, 0x0c, 0x13, 0xec, 0x5f, 0x97, 0x44, 0x17,
            0xc4, 0xa7, 0x7e, 0x3d, 0x64, 0x5d, 0x19, 0x73,
            0x60, 0x81, 0x4f, 0xdc, 0x22, 0x2a, 0x90, 0x88,
            0x46, 0xee, 0xb8, 0x14, 0xde, 0x5e, 0x0b, 0xdb,
            0xe0, 0x32, 0x3a, 0x0a, 0x49, 0x06, 0x24, 0x5c,
            0xc2, 0xd3, 0xac, 0x62, 0x91, 0x95, 0xe4, 0x79,
            0xe7, 0xc8, 0x37, 0x6d, 0x8d, 0xd5, 0x4e, 0xa9,
            0x6c, 0x56, 0xf4, 0xea, 0x65, 0x7a, 0xae, 0x08,
            0xba, 0x78, 0x25, 0x2e, 0x1c, 0xa6, 0xb4, 0xc6,
            0xe8, 0xdd, 0x74, 0x1f, 0x4b, 0xbd, 0x8b, 0x8a,
            0x70, 0x3e, 0xb5, 0x66, 0x48, 0x03, 0xf6, 0x0e,
            0x61, 0x35, 0x57, 0xb9, 0x86, 0xc1, 0x1d, 0x9e,
            0xe1, 0xf8, 0x98, 0x11, 0x69, 0xd9, 0x8e, 0x94,
            0x9b, 0x1e, 0x87, 0xe9, 0xce, 0x55, 0x28, 0xdf,
            0x8c, 0xa1, 0x89, 0x0d, 0xbf, 0xe6, 0x42, 0x68,
            0x41, 0x99, 0x2d, 0x0f, 0xb0, 0x54, 0xbb, 0x16
        };
#endregion

        /// <summary>
        /// Computes the Grøstl hash of the given input.
        /// </summary>
        /// <param name="input">The input data to hash.</param>
        /// <param name="CRYPTO_BYTES">The number of bytes to use for the hash computation.</param>
        /// <param name="shift">The shift values for the permutation.</param>
        /// <param name="ColumnConstant">The column constants for the permutation.</param>
        /// <returns>The computed Grøstl hash as a byte array.</returns>
        private static byte[] Compute_Hash ( FixedVector<byte> input, uint CRYPTO_BYTES, byte[,] shift, uint[] ColumnConstant ) {

            byte ROUNDS     = (byte)(CRYPTO_BYTES <= 32 ? 10 : 14);
            byte STATEBYTES = (byte)(CRYPTO_BYTES <= 32 ? 64 : 128);
            byte STATEWORDS = (byte)(STATEBYTES / 4 );
            byte STATECOLS = (byte)(STATEBYTES / 8 );

            uint[] ctx = new uint[STATEWORDS];
            uint[] buffer = new uint[STATEWORDS];

            ulong rlen = (ulong)input.Count;
            PaddingState s = new PaddingState { BytesInBlock = STATEBYTES, FirstPaddingBlock = 0, Last_PaddingBlock = 0 };

            

            // Initial value
            for ( int i = 0 ; i < STATEWORDS ; i++ )
                ctx[i] = 0;

            ((uint[])ctx)[BYTESLICE((byte)(STATEBYTES - 2), STATECOLS)] = ((CRYPTO_BYTES * 8) >> 8);
            ((uint[])ctx)[BYTESLICE((byte)(STATEBYTES - 1), STATECOLS)] = ((CRYPTO_BYTES * 8));

            int inPos = 0;

            while ( s.Last_PaddingBlock == 0 ) {
                if ( rlen < STATEBYTES ) {
                    if ( s.FirstPaddingBlock == 0 ) {
                        s.BytesInBlock = (byte)rlen;
                        s.FirstPaddingBlock = 1;
                        s.Last_PaddingBlock = (s.BytesInBlock < STATEBYTES - 8) ? (byte)1 : (byte)0;
                    } else {
                        s.BytesInBlock = 0;
                        s.FirstPaddingBlock = 0;
                        s.Last_PaddingBlock = 1;
                    }
                } else
                    rlen -= STATEBYTES;



                // Compression

                SetMessage(buffer, input, s, (ulong)input.Count, STATEBYTES, STATECOLS);
                MemXor(buffer, ctx, STATEWORDS);
                Permutation(buffer, 0, STATEWORDS, STATECOLS, (byte)ROUNDS, shift,  ColumnConstant);
                MemXor(ctx, buffer, STATEWORDS);
                SetMessage(buffer, input, s, (ulong)input.Count, STATEBYTES, STATECOLS);
                Permutation(buffer, 1, STATEWORDS, STATECOLS, (byte)ROUNDS, shift, ColumnConstant);
                MemXor(ctx, buffer, STATEWORDS);

                inPos += (int)STATEBYTES;
            }

            // Output transformation
            for ( int i = 0 ; i < STATEWORDS ; i++ )
                buffer[i] = ctx[i];

            Permutation(buffer, 0, STATEWORDS, STATECOLS, (byte)ROUNDS, shift, ColumnConstant);

            MemXor(ctx, buffer, STATEWORDS);

            // Truncate
            byte[] outx = new byte[CRYPTO_BYTES];
            for ( uint i = STATEBYTES - CRYPTO_BYTES ; i < STATEBYTES ; i++ ) {
                outx[i - (STATEBYTES - CRYPTO_BYTES)] = (byte)ctx[BYTESLICE(i, STATECOLS)];
            }

            return outx;
        }


        

        

        /// <summary>
        /// Computes a 64-bit hash value based on the Grøstl-512 digest for the
        /// specified input buffer, with optional seed mixing and endian-aware
        /// extraction.
        /// </summary>
        /// <param name="input">
        /// The input byte sequence to be hashed. This is treated as an opaque
        /// binary buffer and processed according to the Grøstl-512 specification,
        /// including padding and compression. The full 512-bit digest is computed
        /// internally and then reduced to a 64-bit value.
        /// </param>
        /// <param name="seed">
        /// An optional 64-bit seed value that is XOR-mixed into the final hash
        /// result. This allows callers to derive distinct hash domains, build
        /// keyed hash variants, or introduce additional application-specific
        /// entropy without altering the core Grøstl-512 computation.
        /// </param>
        /// <returns>
        /// A <see cref="Hash64"/> instance containing the 64-bit hash value derived
        /// from the Grøstl-512 digest of <paramref name="input"/>, after endian-aware
        /// extraction and XOR-mixing with <paramref name="seed"/>.
        /// </returns>
        public Hash64 ComputeLong ( FixedVector<byte> input, ulong seed ) {
            uint[]  ColumnConstant = { 0x30201000, 0x70605040, 0xb0a09080, 0xf0e0d0c0 };
            byte[,] ShiftValues = { {0, 1, 2, 3, 4, 5, 6, 11}, {1, 3, 5, 11, 0, 2, 4, 6}    };

            byte[] digest = Compute_Hash(input, 64, ShiftValues, ColumnConstant); // 64 Bytes (Grøstl-512)

            ulong value = digest.ToULong(m_endian);

            value ^= seed;
            return new Hash64(value);
        }
        /// <summary>
        /// Computes a 32-bit hash value based on the Grøstl-256 digest for the
        /// specified input buffer, with optional seed mixing and endian-aware
        /// extraction.
        /// </summary>
        /// <param name="input">
        /// The input byte sequence to be hashed. This is treated as an opaque
        /// binary buffer and processed according to the Grøstl-256 specification,
        /// including padding and compression. The full 256-bit digest is computed
        /// internally and then reduced to a 32-bit value.
        /// </param>
        /// <param name="seed">
        /// An optional 21-bit seed value that is XOR-mixed into the final hash
        /// result. This allows callers to derive distinct hash domains, build
        /// keyed hash variants, or introduce additional application-specific
        /// entropy without altering the core Grøstl-256 computation.
        /// </param>
        /// <returns>
        /// A <see cref="Hash32"/> instance containing the 32-bit hash value derived
        /// from the Grøstl-256 digest of <paramref name="input"/>, after endian-aware
        /// extraction and XOR-mixing with <paramref name="seed"/>.
        /// </returns>
        public Hash32 Compute ( FixedVector<byte> input, uint seed ) {

            uint[]  ColumnConstant = { 0x30201000u,  0x70605040u };
            byte[,] ShiftValues = { { 0, 1, 2, 3, 4, 5, 6, 7 },  { 1, 3, 5, 7, 0, 2, 4, 6 }    };

            byte[] digest = Compute_Hash(input, 32, ShiftValues, ColumnConstant); // 32 Bytes (Grøstl-256)

            uint value = digest.ToUInt(m_endian);

            value ^= seed;
            return new Hash32(value);
        }
    }
    /// @}
}
