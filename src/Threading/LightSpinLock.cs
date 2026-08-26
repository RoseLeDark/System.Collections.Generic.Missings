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
	/// \addtogroup  Threading
	/// @{

	/// <summary>
	/// Provides a lightweight atomic spinlock that protects a single value.
	/// <see cref="LightSpinlock{T}"/> uses pure atomic operations and busy‑wait
	/// acquisition, making it suitable for extremely short critical sections.
	/// </summary>
	/// <typeparam name="T">The type of the protected value.</typeparam>
	public struct LightSpinlock<T> : ISpinlock<bool> {
        private int m_locked;
        private T? m_value;
        private readonly string m_name;
        private int m_owner;

        /// <summary>
        /// Gets the descriptive name assigned to this lock instance.
        /// </summary>
        public string Name => m_name;

        /// <summary>
        /// Indicates whether the lock is currently held by any thread.
        /// </summary>
        public bool IsHeld => m_locked == 1;

        /// <summary>
        /// Indicates whether the lock is held by the calling thread.
        /// </summary>
        public bool IsHeldbyCurrent =>
            (m_owner == System.Threading.Thread.CurrentThread.ManagedThreadId) && IsHeld;

        /// <summary>
        /// Indicates whether thread‑owner tracking is enabled.
        /// </summary>
        public bool IsThreadOwnerTrackingEnabled => true;

        /// <summary>
        /// Gets the underlying atomic flag used by the spinlock.
        /// </summary>
        public bool Handle => m_locked == 1;
		/// <summary>
		/// Indicates whether the lock is currently held.
		/// </summary>
		[Obsolete]
		public bool IsLocked => IsHeld;

		/// <summary>
		/// Gets or sets the protected value. Access is not atomic; callers must
		/// acquire the lock before reading or writing.
		/// </summary>
		public T? Value {
            get {
                if ( !IsHeldbyCurrent ) throw new UnauthorizedAccessException();
                return m_value;
            }
            set {
                if ( !IsHeldbyCurrent ) throw new UnauthorizedAccessException();
                m_value = value;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="LightSpinlock{T}"/> struct.
        /// </summary>
        /// <param name="name">Optional human‑readable name for diagnostics.</param>
        public LightSpinlock ( string name = "LightSpinlock" ) {
            m_locked = 0;
            m_value = default;
            m_name = name;
        }

        /// <summary>
        /// Initializes the spinlock with an initial value.
        /// </summary>
        public LightSpinlock ( T value, string name = "LightSpinlock" ) {
            m_locked = 0;
            m_value = value;
            m_name = name;
            m_owner = -1;
        }

        /// <summary>
        /// Acquires the lock using busy‑wait atomic operations.
        /// </summary>
        public bool Lock ( int span ) {
            // span is ignored, matching the C++ version
            while ( Interlocked.CompareExchange(ref m_locked, 1, 0) != 0 )
                Thread.SpinWait(1);

            m_owner = System.Threading.Thread.CurrentThread.ManagedThreadId;
            return true;
        }

        /// <summary>
        /// Acquires the lock using busy‑wait atomic operations.
        /// </summary>
        public bool Lock ( TimeSpan span ) {
            return Lock(0);
        }

        /// <summary>
        /// Releases the lock.
        /// </summary>
        public void Unlock () {
            if ( m_owner == System.Threading.Thread.CurrentThread.ManagedThreadId )
                Interlocked.Exchange(ref m_locked, 0);
            else
                throw new UnauthorizedAccessException();
        }

        /// <summary>
        /// Spinlocks do not support wait/pulse semantics.
        /// </summary>
        public bool Wait ( TimeSpan span, bool exitContext ) {
            return false;
        }

        /// <summary>
        /// Attempts to acquire the lock without blocking.
        /// </summary>
        public bool TryLock (  ) {
            var _x = Interlocked.CompareExchange(ref m_locked, 1, 0) == 0;

            if(_x) m_owner = System.Threading.Thread.CurrentThread.ManagedThreadId;
            return _x;
        }
    }
	/// @}
}
