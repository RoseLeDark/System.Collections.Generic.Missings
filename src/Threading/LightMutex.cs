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
	/// A lightweight non‑blocking mutex built on top of <see cref="LightCountingSpinlock{T}"/>.
	/// This mutex provides binary spinlock semantics (capacity 1) combined with strict
	/// owner‑tracking to ensure that only the thread which successfully acquired the lock
	/// may access the protected value or release the lock.
	/// </summary>
	/// <typeparam name="T">
	/// The type of the value protected by the mutex.
	/// </typeparam>
	public class LightMutex<T> : LightCountingSpinlock<T> {
        /// <summary>
        /// Stores the thread ID of the thread that currently owns the mutex.
        /// A value of 0 means that the mutex is not owned by any thread.
        /// </summary>
        private int m_ownerThreadId;

        /// <summary>
        /// Gets a value indicating whether the current thread is the owner of the mutex.
        /// The mutex is considered held by the current thread only if:
        /// <list type="bullet">
        /// <item><description>The mutex is locked (capacity consumed)</description></item>
        /// <item><description>The stored owner thread ID matches the current thread ID</description></item>
        /// </list>
        /// </summary>
        public override bool IsHeldbyCurrent {
            get {
                return IsHeld &&
                       m_ownerThreadId == Thread.CurrentThread.ManagedThreadId;
            }
        }

        /// <summary>
        /// Gets or sets the value protected by the mutex.
        /// Access is only permitted to the thread that currently owns the mutex.
        /// Any attempt to read or write the value from a non‑owner thread results
        /// in an <see cref="UnauthorizedAccessException"/>.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown when the current thread does not own the mutex.
        /// </exception>
        public override T? Value {
            get {
                if ( !IsHeldbyCurrent )
                    throw new UnauthorizedAccessException();
                return m_value;
            }
            set {
                if ( !IsHeldbyCurrent )
                    throw new UnauthorizedAccessException();
                m_value = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LightMutex{T}"/> class.
        /// The underlying counting spinlock is configured as a binary spinlock
        /// with a minimum capacity of 0 and a maximum capacity of 1.
        /// </summary>
        /// <param name="name">
        /// Optional name used for diagnostics or debugging.
        /// </param>
        public LightMutex ( string name = "LightMutex" )
            : base(0, 1, name) {
        }

        /// <summary>
        /// Attempts to acquire the mutex within the specified timeout.
        /// If the underlying binary spinlock is successfully acquired,
        /// the current thread becomes the owner of the mutex.
        /// </summary>
        /// <param name="ms">
        /// Timeout in milliseconds. A value of -1 means infinite waiting.
        /// </param>
        /// <returns>
        /// <c>true</c> if the mutex was successfully acquired;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Lock ( int ms ) {
            bool _ret = false;

            if ( base.Lock(ms) ) {
                m_ownerThreadId = Environment.CurrentManagedThreadId;
                _ret = true;
            }

            return _ret;
        }

        /// <summary>
        /// Releases the mutex. Only the thread that owns the mutex may unlock it.
        /// Attempting to unlock the mutex from a non‑owner thread results in an
        /// <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a thread other than the owner attempts to unlock the mutex.
        /// </exception>
        public override void Unlock () {
            if ( m_ownerThreadId != Environment.CurrentManagedThreadId )
                throw new InvalidOperationException("Unlock by non-owner thread.");

            m_ownerThreadId = 0;
            base.Unlock();
        }
    }
	
}
