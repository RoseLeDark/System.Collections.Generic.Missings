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
using SystemEx.Numeric;

namespace SystemEx.Threading {
	/// \addtogroup  Threading
	/// @{

	/// <summary>
	/// Represents a lightweight event group based on a fast bit‑storage type.
	/// <see cref="EventGroup{TFastType}"/> allows individual bit positions to be
	/// set, cleared, flipped, queried, or waited on. The underlying bit container
	/// is provided by <typeparamref name="TFastType"/>, which must implement
	/// <see cref="IFastType"/>.
	/// 
	/// This class is thread‑safe and uses <see cref="LightLock"/> to synchronize
	/// access to the bit field.
	/// </summary>
	/// <typeparam name="TFastType">
	/// A fast bit‑storage type implementing <see cref="IFastType"/>. The type must
	/// provide methods for reading, writing, and flipping individual bit positions.
	/// </typeparam>
	public class EventGroup<TFastType> where TFastType : IFastType {

        private TFastType m_bits;
        private LightSpinlock<int> m_lock;

        /// <summary>
        /// Initializes a new <see cref="EventGroup{TFastType}"/> instance with the
        /// default bit container and a new lightweight lock.
        /// </summary>
        public EventGroup () {
            m_bits = default!;
            m_lock = new LightSpinlock<int>();
        }

        /// <summary>
        /// Sets the bit at the specified position to 1.
        /// </summary>
        /// <param name="pos">The bit position to set.</param>
        public void Set ( byte pos) {
            m_lock.Lock(-1);
            m_bits.At(pos, 1);
            m_lock.Unlock();
        }

        /// <summary>
        /// Clears the bit at the specified position (sets it to 0).
        /// </summary>
        /// <param name="pos">The bit position to clear.</param>
        public void Clear ( byte pos ) {
            m_lock.Lock(-1);
            m_bits.At(pos, 0);
            m_lock.Unlock();
        }
        /// <summary>
        /// Flips the bit at the specified position (0 becomes 1, 1 becomes 0).
        /// </summary>
        /// <param name="pos">The bit position to flip.</param>
        public void Flip ( byte pos ) {
            m_lock.Lock(-1);
            m_bits.Flip(pos);
            m_lock.Unlock();
        }
        /// <summary>
        /// Determines whether the bit at the specified position is set.
        /// </summary>
        /// <param name="pos">The bit position to check.</param>
        /// <returns>
        /// <c>true</c> if the bit is set; otherwise <c>false</c>.
        /// </returns>
        public bool IsSet ( byte pos ) {
            m_lock.Lock(-1);
            bool result = m_bits.Is(pos) == 1;
            m_lock.Unlock();
            return result;
        }
        /// <summary>
        /// Waits until the bit at the specified position becomes set or until the
        /// optional timeout expires. This method performs a lightweight busy‑wait
        /// loop using <see cref="Thread.Yield"/> to reduce contention.
        /// </summary>
        /// <param name="pos">The bit position to wait for.</param>
        /// <param name="timeoutMs">
        /// The maximum number of milliseconds to wait. A value of -1 indicates an
        /// infinite wait.
        /// </param>
        /// <returns>
        /// <c>true</c> if the bit was set before the timeout elapsed;
        /// otherwise <c>false</c>.
        /// </returns>
        public bool Wait ( byte pos, int timeoutMs = -1 ) {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            while ( true ) {
                if ( IsSet(pos) )
                    return true;

                if ( timeoutMs >= 0 && sw.ElapsedMilliseconds >= timeoutMs )
                    return false;

                Thread.Yield();
            }
        }
    }
	
}
