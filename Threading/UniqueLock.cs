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
	/// Provides an RAII-style scoped lock for any <see cref="ILock"/> implementation.
	/// The lock is acquired in the constructor and automatically released when the
	/// instance is disposed. This ensures deterministic lock release even in the
	/// presence of exceptions.
	/// </summary>
	/// <typeparam name="TL">
	/// The lock type implementing <see cref="ILock"/>.
	/// </typeparam>
	public readonly struct UniqueLock<TL> : IDisposable
			 where TL : ILock {

		private readonly TL m_lock;

		/// <summary>
		/// Creates a new <see cref="UniqueLock{TL}"/> and attempts to acquire the
		/// specified lock. If the lock cannot be acquired within the given timeout,
		/// an <see cref="UnauthorizedAccessException"/> is thrown.
		/// </summary>
		/// <param name="l">The lock instance to acquire.</param>
		/// <param name="timeout">
		/// The timeout in milliseconds. A value of -1 indicates an infinite wait.
		/// </param>
		/// <exception cref="UnauthorizedAccessException">
		/// Thrown when the lock cannot be acquired within the specified timeout.
		/// </exception>
		public UniqueLock ( TL l, int timeout = -1 ) {
			m_lock = l;

			if ( !m_lock.Lock(timeout) )
				throw new UnauthorizedAccessException();
		}
		/// <summary>
		/// Releases the lock acquired by this instance.
		/// </summary>
		public void Dispose () {
			m_lock.Unlock();
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
	public readonly struct UniqueLock<T, TL> : IDisposable
		 where TL : ILock {
		private readonly TL m_lock;

		/// <summary>
		/// Gets the value captured at the moment the lock was acquired. This is a
		/// snapshot and does not provide write access to the underlying object.
		/// </summary>
		public T Value { get; }

		/// <summary>
		/// Creates a new <see cref="UniqueLock{T, TL}"/> and attempts to acquire the
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
		public UniqueLock ( TL l, ref T value, int timeout = -1 ) {
			m_lock = l;

			if ( !m_lock.Lock(timeout) )
				throw new UnauthorizedAccessException();

			Value = value;
		}
		/// <summary>
		/// Releases the lock acquired by this instance.
		/// </summary>
		public void Dispose () {
			m_lock.Unlock();
		}
	}
	/// @}
	
}
