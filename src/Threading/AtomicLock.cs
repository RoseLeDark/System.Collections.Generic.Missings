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
	/// A minimal user‑mode spin lock based on an atomic integer.
	/// The lock stores the managed thread ID of the owning thread.
	/// A value of <c>0</c> means the lock is free.
	///
	/// This lock is designed for very short critical sections where
	/// kernel-level synchronization would be too expensive. It uses
	/// <see cref="Interlocked.Exchange(ref int, int)"/> to acquire the lock
	/// and <see cref="Thread.SpinWait(int)"/> for contention handling.
	///
	/// The lock is non‑reentrant, non‑fair, and does not block the thread.
	/// Instead, it spins until the lock becomes available or until a
	/// timeout expires. This makes it suitable for high‑performance
	/// user‑mode synchronization primitives such as barriers.
	///
	/// Ownership is determined by comparing the stored thread ID with
	/// <see cref="Thread.CurrentThread.ManagedThreadId"/>. Only the owning
	/// thread is allowed to release the lock.
	/// </summary>
	public sealed class AtomicLock : ISpinlock<int> {

		private int m_lock = 0;
		/// <summary>
		/// Gets the raw integer value representing the current lock state.
		/// A value of <c>0</c> means unlocked; otherwise it contains the
		/// managed thread ID of the owning thread.
		/// </summary>
		public int Handle => Volatile.Read(ref m_lock);
		/// <summary>
		/// Indicates whether the lock is currently held by any thread.
		/// </summary>
		public bool IsHeld => Volatile.Read(ref m_lock) != 0;
		/// <summary>
		/// Indicates whether the calling thread currently owns the lock.
		/// </summary>
		public bool IsHeldbyCurrent => Volatile.Read(ref m_lock) == Thread.CurrentThread.ManagedThreadId;

		public bool IsThreadOwnerTrackingEnabled => true;

		[Obsolete]
		public bool IsLocked => IsHeld;

		/// <summary>
		/// Attempts to acquire the lock within the specified timeout.
		/// Throws an exception if another thread already owns the lock.
		/// </summary>
		public bool Lock ( int ms ) {
			return internalLock(ms, true);
		}
		/// <summary>
		/// Attempts to acquire the lock within the specified time span.
		/// Throws an exception if another thread already owns the lock.
		/// </summary>
		public bool Lock ( TimeSpan span ) {
			return internalLock(span.Milliseconds, true);
		}
		/// <summary>
		/// Attempts to acquire the lock without throwing exceptions.
		/// Returns <c>true</c> if the lock was acquired; otherwise <c>false</c>.
		/// </summary>
		public bool TryLock () {
			return internalLock(10, false);
		}
		/// <summary>
		/// Releases the lock if the calling thread is the owner.
		/// </summary>
		public void Unlock () {
			if( IsHeldbyCurrent )
				Interlocked.Exchange(ref m_lock, 0);
		}
		/// <summary>
		/// Not implemented. Always returns <c>false</c>.
		/// Included only to satisfy the <see cref="ILock{T}"/> interface.
		/// </summary>
		public bool Wait ( TimeSpan span, bool exitContext ) {
			return false;
		}
		/// <summary>
		/// Internal lock acquisition logic. Uses atomic exchange and spin‑waiting.
		/// If <paramref name="ecxp"/> is <c>true</c>, an exception is thrown when
		/// another thread already owns the lock.
		/// </summary>
		private bool internalLock ( int ms, bool ecxp ) {
			if ( IsHeldbyCurrent ) return true;
			if ( IsHeld ) if ( ecxp ) throw new Exception("Held by other thread"); else return false;

			var lockTaken = false;

			if ( ms <= 0 ) {

				while ( !lockTaken ) {
					lockTaken = Interlocked.Exchange(ref m_lock, Thread.CurrentThread.ManagedThreadId) == 0;
					if ( !lockTaken )
						Thread.SpinWait(1);
				}
			} else {

				var sw = System.Diagnostics.Stopwatch.StartNew();

				while ( !lockTaken && sw.ElapsedMilliseconds < ms ) {
					lockTaken = Interlocked.Exchange(ref m_lock, Thread.CurrentThread.ManagedThreadId) == 0;
					if ( !lockTaken )
						Thread.SpinWait(1);
				}
			}

			return lockTaken;
		}
	}
}
