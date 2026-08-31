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
			return slock.m_lock.IsHeld;
		}
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
		public ScopedUnlock ( ref TLOCK ulock, int ms = -1 ) {
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
			return !slock.m_lock.IsHeld;
		}
	}

	/// <summary>
	/// Provides an RAII-style scoped lock for any <see cref="ILock"/> implementation
	/// and exposes a snapshot of a protected value. The lock is acquired in the
	/// constructor and automatically released when the instance is disposed.
	/// </summary>
	/// <typeparam name="T">
	/// The type of the protected value.
	/// </typeparam>
	/// <typeparam name="TL">
	/// The lock type implementing <see cref="ILock"/>.
	/// </typeparam>
	public ref struct ScopedLock<T, TLOCK> : IDisposable
		 where TLOCK : ILock {

		private ref TLOCK m_lock;
		private ref T m_value;

		/// <summary>
		/// Gets the value captured at the moment the lock was acquired. This is a
		/// snapshot and does not provide write access to the underlying object.
		/// </summary>
		public ref T Value {
			get {
				if(m_lock.IsHeld) {
					return ref m_value;
				} else {
					throw new Exception("Not allowed");
				}
			}
		}

		/// <summary>
		/// Creates a new <see cref="ScopedLock{T, TL}"/> and attempts to acquire the
		/// specified lock. If the lock cannot be acquired within the given timeout,
		/// an <see cref="UnauthorizedAccessException"/> is thrown.
		/// The provided value is captured by reference and stored as a snapshot.
		/// </summary>
		/// <param name="l">The lock instance to acquire.</param>
		/// <param name="value">
		/// A reference to the protected value. The value is copied into the
		/// <see cref="Value"/> property once the lock is acquired.
		/// </param>
		/// <param name="timeout">
		/// The timeout in milliseconds. A value of -1 indicates an infinite wait.
		/// </param>
		/// <exception cref="UnauthorizedAccessException">
		/// Thrown when the lock cannot be acquired within the specified timeout.
		/// </exception>
		public ScopedLock ( ref TLOCK l, ref T value, int timeout = -1 ) {
			m_lock = ref l;

			if ( !m_lock.Lock(timeout) )
				throw new UnauthorizedAccessException();

			m_value = ref value;
		}
		/// <summary>
		/// Releases the lock acquired by this instance.
		/// </summary>
		public void Dispose () {
			m_lock.Unlock();
		}
	}
	
	
}
