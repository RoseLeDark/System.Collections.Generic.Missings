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
        /// Attempts to acquire the lock using a <see cref="TimeSpan"/> timeout without
        /// implying a blocking intent. This method is semantically equivalent to
        /// <see cref="Lock(TimeSpan)"/> but expresses a non‑blocking usage pattern.
        /// </summary>
        /// <param name="span">Maximum duration to attempt acquiring the lock.</param>
        /// <returns>
        /// <c>true</c> if the lock was acquired; otherwise <c>false</c>.
        /// </returns>
        bool TryLock ( TimeSpan span );

        /// <summary>
        /// Indicates whether the lock is currently held by the calling thread.
        /// Implementations must update this flag whenever the lock is acquired or
        /// released.
        /// </summary>
        bool IsLocked { get; }
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


    /// <summary>
    /// Provides a scoped unlock mechanism for an existing <see cref="ILock"/> instance.
    /// Upon construction, the lock is immediately released. When the scope ends,
    /// <see cref="Dispose"/> reacquires the lock using the specified timeout.
    /// 
    /// <see cref="ScopedUnlock{TLOCK}"/> is useful for wait‑and‑resume patterns,
    /// producer/consumer scenarios, or any situation where a lock must be temporarily
    /// released while guaranteeing reacquisition.
    /// </summary>
    /// <typeparam name="TLOCK">
    /// The lock type implementing <see cref="ILock"/>.
    /// </typeparam>
    public ref struct ScopedUnlock<TLOCK> : IDisposable
            where TLOCK : ILock {

        private ref TLOCK m_lock;
        private readonly int m_iMS;

        /// <summary>
        /// Initializes a new scoped unlock. The provided lock is released immediately,
        /// allowing other threads or processes to proceed. When the scope ends,
        /// the lock is reacquired using the specified timeout.
        /// </summary>
        /// <param name="ulock">Reference to the lock instance to operate on.</param>
        /// <param name="ms">
        /// Timeout in milliseconds used when reacquiring the lock. A value of <c>-1</c>
        /// indicates an immediate attempt.
        /// </param>
        public ScopedUnlock ( ref TLOCK ulock, int ms = -1) {
            m_lock = ref ulock;
            m_iMS = ms;
            m_lock.Unlock();
        }
        /// <summary>
        /// Reacquires the lock using the timeout specified during construction.
        /// This method is automatically invoked when the scope ends.
        /// </summary>
        public void Dispose () {
            m_lock.Lock(m_iMS);
        }

        /// <summary>
        /// Returns <c>true</c> if the underlying lock is currently not held.
        /// This operator allows scoped unlocks to be used in conditional expressions.
        /// </summary>
        public static implicit operator bool ( ScopedUnlock<TLOCK> slock ) {
            return !slock.m_lock.IsLocked;
        }
    }

    /// <summary>
    /// Provides a scoped lock mechanism for an existing <see cref="ILock"/> instance.
    /// Upon construction, the lock is acquired using the specified timeout. When the
    /// scope ends, <see cref="Dispose"/> releases the lock.
    /// 
    /// <see cref="ScopedLock{TLOCK}"/> offers RAII‑style mutual exclusion similar to
    /// C++'s <c>std::scoped_lock</c>, ensuring deterministic lock acquisition and release.
    /// </summary>
    /// <typeparam name="TLOCK">
    /// The lock type implementing <see cref="ILock"/>.
    /// </typeparam>
    public ref struct ScopedLock<TLOCK> : IDisposable
        where TLOCK : ILock {
        private ref TLOCK m_lock;

        /// <summary>
        /// Initializes a new scoped lock. The provided lock is acquired immediately
        /// using the specified timeout. If the lock cannot be acquired, the caller
        /// may inspect the result via the implicit boolean operator.
        /// </summary>
        /// <param name="ulock">Reference to the lock instance to operate on.</param>
        /// <param name="ms">
        /// Timeout in milliseconds used when acquiring the lock. A value of <c>-1</c>
        /// indicates an immediate attempt.
        /// </param>
        public ScopedLock ( ref TLOCK ulock, int ms = -1 ) {
            m_lock = ref ulock;
            m_lock.Lock(ms);
        }

        /// <summary>
        /// Releases the lock. This method is automatically invoked when the scope ends.
        /// </summary>
        public void Dispose () {
            m_lock.Unlock();
        }

        /// <summary>
        /// Returns <c>true</c> if the underlying lock is currently held. This operator
        /// allows scoped locks to be used in conditional expressions, mirroring the
        /// behavior of C++ RAII lock wrappers.
        /// </summary>
        public static implicit operator bool ( ScopedLock<TLOCK> slock ) {
            return slock.m_lock.IsLocked;
        }
    }
}
