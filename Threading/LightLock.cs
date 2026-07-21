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

namespace SystemEx.Threading {
    /// <summary>
    /// Provides a minimal wrapper around <see cref="Monitor"/> to offer lightweight
    /// mutual exclusion. <see cref="LightLock"/> is intended for short, low‑overhead
    /// critical sections where a full synchronization primitive would be unnecessary.
    /// </summary>
    public struct LightLock {
        private readonly object m_lock;

        /// <summary>
        /// Initializes a new instance of the <see cref="LightLock"/> struct.
        /// A dedicated lock object is created for use with <see cref="Monitor"/>.
        /// </summary>
        public LightLock () {
            m_lock = new object();
        }

        /// <summary>
        /// Attempts to enter the critical section by acquiring the internal lock.
        /// This method uses <see cref="Monitor.Enter(object, ref bool)"/> and returns
        /// a boolean indicating whether the lock was successfully taken.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the lock was successfully acquired; otherwise <c>false</c>.
        /// </returns>
        public bool Lock () {
            bool lockTaken = false;
            Monitor.Enter(m_lock, ref lockTaken);

            return lockTaken;
        }

        /// <summary>
        /// Exits the critical section by releasing the internal lock. The caller
        /// must ensure that every successful call to <see cref="Lock"/> is paired
        /// with a corresponding call to <see cref="Unlock"/>.
        /// </summary>
        public void Unlock () {
            Monitor.Exit(m_lock);
        }
        /// <summary>
        /// Suspends the current thread while releasing the internal lock, waiting for
        /// a pulse notification or until the specified timeout expires. This method
        /// wraps <see cref="Monitor.Wait(object, TimeSpan, bool)"/> and returns a
        /// boolean indicating whether the wait completed successfully.
        /// </summary>
        /// <param name="span">
        /// The maximum amount of time to wait for a pulse signal.
        /// </param>
        /// <param name="exitContext">
        /// Indicates whether the synchronization context should be exited before the
        /// wait begins. This parameter is forwarded directly to
        /// <see cref="Monitor.Wait(object, TimeSpan, bool)"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the thread was notified before the timeout elapsed;
        /// otherwise <c>false</c>.
        /// </returns>
        public bool Wait ( TimeSpan span, bool exitContext ) {
            return Monitor.Wait(m_lock, span, exitContext);
        }
    }
}
