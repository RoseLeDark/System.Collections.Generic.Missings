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
	// \addtogroup SystemEx.Threading
	/// @{
	/// <summary>
	/// Provides a minimal wrapper around <see cref="Monitor"/> to offer lightweight
	/// mutual exclusion. <see cref="LightLock"/> is intended for short, low‑overhead
	/// critical sections where a full synchronization primitive would be unnecessary.
	/// </summary>
	public struct LightLock : ILock<object> {
        private readonly object m_lock;
        private string m_strName;
        private bool m_bLocked;

        /// <summary>
        /// Gets the underlying lock handle used by <see cref="Monitor"/>.
        /// </summary>
        public object Handle => m_lock;
        /// <summary>
        /// Gets the descriptive name assigned to this lock instance.
        /// </summary>
        public string Name => m_strName;
        /// <summary>
        /// Indicates whether the lock is currently held by the calling thread.
        /// This flag is updated whenever <see cref="Lock(TimeSpan)"/> or
        /// <see cref="TryLock(TimeSpan)"/> succeeds or when <see cref="Unlock"/> is invoked.
        /// </summary>
        public bool IsLocked => m_bLocked;

        /// <summary>
        /// Initializes a new instance of the <see cref="LightLock"/> struct.
        /// A dedicated lock object is created for use with <see cref="Monitor"/>.
        /// </summary>
        /// <param name="strName">
        /// Optional human‑readable name used for diagnostics or debugging.
        /// </param>
        public LightLock (string strName = "LightLock" ) {
            m_lock = new object();
            m_strName = strName;
            m_bLocked = false;
        }

        /// <summary>
        /// Attempts to enter the critical section by acquiring the internal lock.
        /// This overload accepts a <see cref="TimeSpan"/> timeout and uses
        /// <see cref="Monitor.TryEnter(object, TimeSpan, ref bool)"/> to determine
        /// whether the lock was successfully taken.
        /// </summary>
        /// <param name="span">The maximum duration to wait for the lock.</param>
        /// <returns>
        /// <c>true</c> if the lock was acquired; otherwise <c>false</c>.
        /// </returns>
        public bool Lock (TimeSpan span) {
            Monitor.TryEnter(m_lock, span, ref m_bLocked);

            return m_bLocked;
        }

        /// <summary>
        /// Attempts to acquire the lock using a millisecond timeout. A negative value
        /// indicates an immediate, non‑blocking attempt. Positive values are forwarded
        /// to <see cref="Monitor.TryEnter(object, int, ref bool)"/>.
        /// </summary>
        /// <param name="ms">Timeout in milliseconds, or <c>-1</c> for immediate attempt.</param>
        /// <returns>
        /// <c>true</c> if the lock was acquired; otherwise <c>false</c>.
        /// </returns>
        public bool Lock ( int ms = -1) {

            if ( ms > 0 ) Monitor.TryEnter(m_lock, ms, ref m_bLocked);
            else Monitor.TryEnter(m_lock, ref m_bLocked);

            return m_bLocked;

        }
        /// <summary>
        /// Releases the internal lock. Every successful call to <see cref="Lock(TimeSpan)"/>
        /// or <see cref="Lock(int)"/> must be paired with a corresponding call to
        /// <see cref="Unlock"/> to avoid deadlocks.
        /// </summary>
        public void Unlock () => Monitor.Exit(m_lock);

        /// <summary>
        /// Temporarily releases the lock and suspends the current thread until a pulse
        /// notification is received or the specified timeout expires. Upon completion,
        /// the lock is automatically reacquired. This method wraps
        /// <see cref="Monitor.Wait(object, TimeSpan, bool)"/>.
        /// </summary>
        /// <param name="span">Maximum time to wait for a pulse.</param>
        /// <param name="exitContext">
        /// Indicates whether the synchronization context should be exited before waiting.
        /// </param>
        /// <returns>
        /// <c>true</c> if the wait completed due to a pulse; otherwise <c>false</c>.
        /// </returns>
        public bool Wait ( TimeSpan span, bool exitContext ) => Monitor.Wait(m_lock, span, exitContext);

        /// <summary>
        /// Attempts to acquire the lock using a <see cref="TimeSpan"/> timeout.
        /// This method is functionally identical to <see cref="Lock(TimeSpan)"/> but
        /// semantically expresses a non‑blocking intent.
        /// </summary>
        /// <param name="span">Maximum duration to wait for the lock.</param>
        /// <returns>
        /// <c>true</c> if the lock was acquired; otherwise <c>false</c>.
        /// </returns>
        public bool TryLock (  ) => (m_bLocked = Monitor.TryEnter(m_lock));

    }
	/// @}
}
