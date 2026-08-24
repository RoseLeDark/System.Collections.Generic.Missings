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
using SystemEx.Hash;

namespace SystemEx.Random {
	/// \addtogroup SystemEx.Random
	/// @{
	/// <summary>
	/// Provides a time‑based seed implementation using <see cref="DateTime"/> as
	/// the update source. The seed consists of multiple 32‑bit values derived from
	/// the current timestamp, system tick count, and optionally a GUID hash.
	/// </summary>
	public struct TimeBasedSeed : ISeed<DateTime> {
        private uint[] m_seed;
        /// <summary>
        /// Gets the number of 32‑bit values contained in the seed.
        /// </summary>
        public int Length => m_seed.Length;
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
        /// Initializes a new <see cref="TimeBasedSeed"/> with the specified length.
        /// The seed is immediately populated using <see cref="DateTime.UtcNow"/>.
        /// </summary>
        /// <param name="length">
        /// The desired number of seed values. A minimum of three values is enforced.
        /// </param>
        public TimeBasedSeed (int length = 4) {
            m_seed = new uint[System.Math.Max(3, length)];
            Update(DateTime.UtcNow);
        }
        /// <summary>
        /// Returns the underlying seed values as an array of 32‑bit unsigned integers.
        /// </summary>
        /// <returns>
        /// A <see cref="uint"/> array containing the current seed values.
        /// </returns>
        public uint[] GetSeed () => m_seed;
        /// <summary>
        /// Updates the seed using the specified <see cref="DateTime"/> value.
        /// The timestamp is split into two 32‑bit components, followed by the
        /// current system tick count. If the seed contains more than three values,
        /// an additional GUID‑based value is generated.
        /// </summary>
        /// <param name="value">
        /// The <see cref="DateTime"/> instance used to refresh the seed.
        /// </param>
        public void Update ( DateTime value ) {
            ulong t = (ulong)value.Ticks;

            m_seed[0] = (uint)t;
            m_seed[1] = (uint)(t >> 32);
            m_seed[2] = (uint)Environment.TickCount;
            if ( m_seed.Length > 3 ) m_seed[3] = (uint)Guid.NewGuid().GetHashCode();
        }
    }
	///@}
}
