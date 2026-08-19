using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace SystemEx.Threading {
	/// <summary>
	/// A one-shot countdown synchronization primitive.
	/// The latch starts in the locked state and releases all waiting threads
	/// once its internal counter reaches zero. After releasing, the latch
	/// remains permanently open.
	/// </summary>
	/// <typeparam name="T">
	/// The lock type used to block threads until the latch opens.
	/// Must implement <see cref="ILock"/>.
	/// </typeparam>
	public sealed class Latch<T> : NoCopyable where T : ILock {
		private SafeCounter m_counter;
		private T m_lockType;

		/// <summary>
		/// Initializes the latch with the specified countdown value.
		/// The latch is locked immediately and will unlock once the
		/// counter reaches zero through calls to <see cref="CountDown"/>.
		/// </summary>
		/// <param name="count">The initial countdown value.</param>
		/// <param name="locker">
		/// The lock instance used to block threads until the latch opens.
		/// </param>
		public Latch (int count, T locker) {
			m_counter = new SafeCounter(count);
			m_lockType = locker;
			m_lockType.Lock(-1);
		}
		/// <summary>
		/// Decrements the internal counter by the specified amount.
		/// When the counter reaches zero or below, the latch unlocks
		/// permanently and all waiting threads are released.
		/// </summary>
		/// <param name="update">
		/// The number of decrement operations to perform.
		/// Defaults to 1.
		/// </param>
		public void CountDown(int update = 1) {
			while(update > 0) {
				m_counter--;
				update--;
			}

			if ( m_counter <= 0 ) 
				m_lockType.Unlock();
		}

		/// <summary>
		/// Blocks the calling thread until the latch opens.
		/// If the latch is already open, the call returns immediately.
		/// </summary>
		/// <param name="wait">
		/// The maximum wait time passed to the underlying lock.
		/// A value of -1 indicates an infinite wait.
		/// </param>
		/// <returns>
		/// <c>true</c> if the thread successfully waited for the latch to open;
		/// otherwise <c>false</c> if the wait timed out.
		/// </returns>
		public bool Wait (int wait = 1) {
			bool locked = m_lockType.Lock(wait);
			if ( locked )
				m_lockType.Unlock();

			return locked;
		}

		/// <summary>
		/// Attempts to wait for the latch without blocking.
		/// Returns immediately with the result of a non-blocking lock attempt.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the latch is already open;
		/// otherwise <c>false</c>.
		/// </returns>
		public bool TryWait() {
			bool x = m_lockType.TryLock();
			m_lockType.Unlock();
			return x;
		}
	}
}
