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
	/// Defines the minimal contract for a synchronization primitive capable of
	/// providing mutual exclusion. Implementations of <see cref="ILock"/> may wrap
	/// lightweight in‑process mechanisms (such as <see cref="Monitor"/>) or
	/// heavier inter‑process constructs (such as <see cref="Mutex"/>).
	/// </summary>
	public interface ILock {
        /// <summary>
        /// Attempts to acquire the lock using a millisecond timeout. A negative value
        /// indicates an immediate, non‑blocking attempt. Positive values specify the
        /// maximum duration to wait for the lock.
        /// </summary>
        /// <param name="ms">Timeout in milliseconds, or <c>-1</c> for immediate attempt.</param>
        /// <returns>
        /// <c>true</c> if the lock was successfully acquired; otherwise <c>false</c>.
        /// </returns>
        bool Lock ( int ms );

        /// <summary>
        /// Attempts to acquire the lock using a <see cref="TimeSpan"/> timeout.
        /// Implementations should block until the lock is acquired or the timeout
        /// expires.
        /// </summary>
        /// <param name="span">Maximum duration to wait for the lock.</param>
        /// <returns>
        /// <c>true</c> if the lock was successfully acquired; otherwise <c>false</c>.
        /// </returns>
        bool Lock ( TimeSpan span );

        /// <summary>
        /// Releases the lock. Every successful call to <see cref="Lock(int)"/> or
        /// <see cref="Lock(TimeSpan)"/> must be paired with a corresponding call to
        /// <see cref="Unlock"/> to avoid deadlocks or inconsistent state.
        /// </summary>
        void Unlock ( );

        /// <summary>
        /// Temporarily releases the lock and suspends the current thread until a
        /// notification is received or the specified timeout expires. Upon completion,
        /// the lock is automatically reacquired. This method provides a generalized
        /// wait‑and‑reacquire pattern similar to <see cref="Monitor.Wait(object, TimeSpan, bool)"/>.
        /// </summary>
        /// <param name="span">Maximum duration to wait for a notification.</param>
        /// <param name="exitContext">
        /// Indicates whether the synchronization context should be exited before waiting.
        /// </param>
        /// <returns>
        /// <c>true</c> if the wait completed due to a notification; otherwise <c>false</c>.
        /// </returns>
        bool Wait ( TimeSpan span, bool exitContext );

        /// <summary>
        /// Attempts to acquire the lock without
        /// implying a blocking intent. This method is semantically equivalent to
        /// </summary>
        /// <returns>
        /// <c>true</c> if the lock was acquired; otherwise <c>false</c>.
        /// </returns>
        bool TryLock ( );

		/// <summary>
		/// Indicates whether the lock is currently held by the calling thread.
		/// Implementations must update this flag whenever the lock is acquired or
		/// released.
		/// </summary>
		bool IsHeld { get; }

	}

    /// <summary>
    /// Extends <see cref="ILock"/> by exposing the underlying synchronization handle.
    /// This is useful for advanced scenarios where the caller may need direct access
    /// to the wrapped primitive (e.g., <see cref="Mutex"/>, <see cref="Monitor"/> object).
    /// </summary>
    /// <typeparam name="T">
    /// The type of the underlying synchronization handle.
    /// </typeparam>
    public interface ILock<T> : ILock {

        /// <summary>
        /// Gets the underlying synchronization handle associated with this lock.
        /// </summary>
        T Handle { get;  }

    }


    

    
}
