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
	/// Provides a lightweight reentrant futex-like synchronization primitive.
	/// <para>
	/// A <c>LightFutex</c> combines a fast userspace lock word with an optional
	/// kernel-assisted wait path via <see cref="AutoResetEvent"/>. The lock is
	/// reentrant for the owning thread: if the current thread already holds the
	/// futex, additional lock attempts succeed immediately without modifying the
	/// internal state.
	/// </para>
	/// <para>
	/// The futex is considered free when <c>m_state == -1</c>. Any non-negative
	/// value represents the thread ID of the owning thread.
	/// </para>
	/// </summary>
	public class LightFutex : ISpinlock<Object> {
		private int m_state;                 // -1 = free, >= 0 = owned by thread ID
		private readonly AutoResetEvent m_wait;

		/// <summary>
		/// Gets a value indicating whether the current thread is the owner of the futex.
		/// </summary>
		public bool IsHeldbyCurrent => Volatile.Read(ref m_state) == Thread.CurrentThread.ManagedThreadId;

		/// <summary>
		/// Indicates that this futex supports thread‑owner tracking.
		/// </summary>
		public bool IsThreadOwnerTrackingEnabled => true;

		/// <summary>
		/// Gets a handle representing the futex state. This value is informational only.
		/// </summary>
		public object Handle => 0;

		/// <summary>
		/// Gets a value indicating whether the futex is currently held by any thread.
		/// </summary>
		public bool IsHeld => Volatile.Read(ref m_state) != -1;

		/// <summary>
		/// Initializes a new instance of the <see cref="LightFutex"/> struct.
		/// The futex starts in the free state (<c>-1</c>).
		/// </summary>
		public LightFutex () {
			m_state = -1;
			m_wait = new(false);
		}

		/// <summary>
		/// Attempts to acquire the futex within the specified timeout in milliseconds.
		/// </summary>
		/// <param name="ms">Timeout in milliseconds.</param>
		/// <returns>
		/// <c>true</c> if the futex was acquired or reentered by the owning thread;
		/// otherwise, <c>false</c>.
		/// </returns>
		public bool Lock ( int ms ) {
			return Lock(new TimeSpan(ms));
		}

		/// <summary>
		/// Attempts to acquire the futex within the specified timeout.
		/// <para>
		/// If the futex is already held, the call succeeds only if the current thread
		/// is the owner (reentrant acquisition). Otherwise, the fast userspace CAS
		/// path is attempted. If contention persists, the thread enters the wait path
		/// until signaled or the timeout expires.
		/// </para>
		/// </summary>
		/// <param name="span">The maximum time to wait for the futex.</param>
		/// <returns>
		/// <c>true</c> if the futex was acquired or reentered; otherwise, <c>false</c>.
		/// </returns>
		public bool Lock ( TimeSpan span ) {
			bool _ret = false;

			// Reentrant acquisition or immediate denial
			if ( IsHeld )
				return IsHeldbyCurrent;

			// Fast path: userspace CAS
			if ( Interlocked.CompareExchange(ref m_state, Thread.CurrentThread.ManagedThreadId, -1) == -1 )
				_ret = true;

			// Slow path: wait until signaled
			while ( !_ret ) {
				if ( Interlocked.CompareExchange(ref m_state, Thread.CurrentThread.ManagedThreadId, -1) == -1 ) {
					_ret = true; break;
				}

				_ret = m_wait.WaitOne(span);
			}

			return _ret;
		}
		/// <summary>
		/// Attempts to acquire the futex without blocking longer than one millisecond.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the futex was acquired or reentered; otherwise, <c>false</c>.
		/// </returns>
		public bool TryLock () {
			return Lock(1);
		}

		/// <summary>
		/// Releases the futex if the current thread is the owner.
		/// <para>
		/// The futex is reset to the free state (<c>-1</c>) and one waiting thread
		/// is signaled.
		/// </para>
		/// </summary>
		public void Unlock () {

			if ( IsHeldbyCurrent ) {
				// freigeben
				Interlocked.Exchange(ref m_state, -1);
				// einen Wartenden wecken
				m_wait.Set();
			}
		}
		/// <summary>
		/// Waits on the internal wait handle for the specified duration.
		/// This method does not interact with the futex state.
		/// </summary>
		/// <param name="span">The maximum time to wait.</param>
		/// <param name="exitContext">Ignored. Provided for interface compatibility.</param>
		/// <returns>
		/// <c>true</c> if the wait handle was signaled; otherwise, <c>false</c>.
		/// </returns>
		public bool Wait ( TimeSpan span, bool exitContext ) {
			return m_wait.WaitOne(span, exitContext);
		}
	}
}
