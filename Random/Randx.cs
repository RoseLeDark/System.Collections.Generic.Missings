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
	/// \addtogroup SystemEx.Random
	/// @{
	/// <summary>
	/// Provides a simple interface for generating random numbers using the ISAAC algorithm.
	/// </summary>
	public sealed class Randx {
        private readonly Isaac32Engine _core;
        /// <summary>
        /// Initializes a new instance of the <see cref="Randx"/> class with the specified
        /// </summary>
        /// <param name="seedA">The first seed value.</param>
        /// <param name="seedB">The second seed value.</param>
        /// <param name="seedC">The third seed value.</param>
        public Randx ( uint seedA = 0, uint seedB = 0, uint seedC = 0 ) {
            _core = new Isaac32Engine(seedA, seedB, seedC);
        }
        /// <summary>
        /// Generates the next random 32-bit number in the sequence.
        /// </summary>
        /// <returns>The next random 32-bit number.</returns>
        public uint Next32 () {
            return _core.Next();
        }
        /// <summary>
        /// Generates the next random 64-bit number in the sequence.
        /// </summary>
        /// <returns>The next random 64-bit number.</returns>
        public ulong Next64 () {
            // Zwei ISAAC‑Werte kombinieren → stabil, deterministisch
            ulong hi = _core.Next();
            ulong lo = _core.Next();
            return (hi << 32) | lo;
        }
        /// <summary>
        /// Generates the next random byte in the sequence.
        /// </summary>
        /// <returns>The next random byte.</returns>
        public byte NextByte () {
            return (byte)(_core.Next() & 0xFF);
        }
        /// <summary>
        /// Generates the next random number in the specified range.
        /// </summary>
        /// <param name="buffer">The buffer to fill with random bytes.</param>
        public void NextBytes ( byte[] buffer ) {
            for ( int i = 0 ; i < buffer.Length ; i++ )
                buffer[i] = NextByte();
        }
        /// <summary>
        /// Generates the next random number in the specified range.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>The next random number.</returns>
        public uint Next ( uint min, uint max ) {
            uint r = Next32();
            return min + (r % (max - min));
        }
        /// <summary>
        /// Generates the next random number in the specified range.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>The next random number.</returns>
        public ulong Next ( ulong min, ulong max ) {
            ulong r = Next64();
            return min + (r % (max - min));
        }
        /// <summary>
        /// Generates the next random character in the sequence.
        /// </summary>
        /// <returns>The next random character.</returns>
        public char NextChar () {
            return (char)(Next32() & 0xFF);
        }
        /// <summary>
        /// Generates the next random string in the sequence.
        /// </summary>
        /// <param name="length">The length of the string to generate.</param>
        /// <returns>The next random string.</returns>
        public string NextString ( int length ) {
            char[] c = new char[length];
            for ( int i = 0 ; i < length ; i++ )
                c[i] = NextChar();
            return new string(c);
        }
        /// <summary>
        /// Generates the next random hash seed in the sequence.
        /// </summary>
        /// <returns>The next random hash seed.</returns>
        public uint NextHashSeed32 () {
            return Next32();
        }
        /// <summary>
        /// Generates the next random hash seed in the sequence.
        /// </summary>
        /// <returns>The next random hash seed.</returns>
        public ulong NextHashSeed64 () {
            return Next64();
        }
    }
	///@}
}
