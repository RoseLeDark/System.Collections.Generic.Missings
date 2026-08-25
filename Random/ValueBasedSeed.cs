
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
	/// \addtogroup Random
	/// @{

	/// <summary>
	/// Provides a seed implementation based on a simple integer value. The input
	/// integer is expanded into multiple 32‑bit components using bitwise and
	/// arithmetic transformations. This seed type is suitable for all random
	/// engines in SystemEx.
	/// </summary>
	public struct ValueBasedSeed : ISeed<int> {
        private uint[] m_seed;

        /// <summary>
        /// Gets the number of 32‑bit values contained in the seed.
        /// </summary>
        public int Length => m_seed.Length;
        /// <summary>
        /// Returns the underlying seed values as an array of 32‑bit unsigned integers.
        /// </summary>
        /// <returns>
        /// A <see cref="uint"/> array containing the current seed values.
        /// </returns>
        public uint[] GetSeed () => m_seed;
        /// <summary>
        /// Updates the seed using the specified integer value. The value is expanded
        /// into four distinct 32‑bit components using bitwise and arithmetic
        /// operations. This method does not perform hashing; it simply adapts the
        /// integer into a usable seed format for random engines.
        /// </summary>
        /// <param name="value">
        /// The integer used to refresh the seed.
        /// </param>
        public void Update ( int value ) {
            uint v = (uint)value;

            m_seed[0] = (uint)v ^ 0xA3B1C2D3u;
            m_seed[1] = (uint)(v << 1) ^ 0xF00DBAAFu;
            m_seed[2] = (uint)(v >> 1) ^ 0x12345678u;
            m_seed[3] = (uint)v * 2654435761u;
        }
        /// <summary>
        /// Provides indexed access to individual seed values. Both reading and
        /// writing are supported, allowing engines or mixing operations to modify
        /// specific seed components directly.
        /// </summary>
        /// <param name="index">The zero‑based index of the seed value.</param>
        public uint this[int index] {
            get => m_seed[index];
            set => m_seed[index] = value;
        }
        /// <summary>
        /// Initializes a new <see cref="ValueBasedSeed"/> using the specified integer.
        /// The seed is immediately populated using <see cref="Update(int)"/>.
        /// </summary>
        /// <param name="value">
        /// The initial integer used to populate the seed.
        /// </param>
        /// <param name="length">
        /// The desired number of seed values. A minimum of four values is enforced.
        /// </param>
        public ValueBasedSeed ( int value, int length = 4 ) {
            m_seed = new uint[System.Math.Max(4, length)];

            Update(value);
        }
    }
	/// @}
}
