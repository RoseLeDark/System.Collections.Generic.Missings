using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace SystemEx.Threading {


	public class BarrierNewPhaseLockException : Exception {
		public BarrierNewPhaseLockException ()
			: base(null) {
		}

		public BarrierNewPhaseLockException ( Exception? innerException )
			: base(null, innerException) {
		}

		public BarrierNewPhaseLockException ( string? message )
			: base(message, null) {
		}

		public BarrierNewPhaseLockException ( string? message, Exception? innerException )
			: base(message, innerException) {
		}
	}
	/// <summary>
	/// Represents a reusable multi‑phase synchronization barrier.
	/// Threads call <see cref="Arrive"/> or <see cref="ArriveAndWait"/> to
	/// signal arrival at the current phase. When the number of arrivals
	/// reaches <c>m_max</c>, the barrier opens, executes the completion
	/// callback, resets its counters, and begins the next phase.
	/// </summary>
	/// <typeparam name="T">
	/// The lock type used to block threads during phase synchronization.
	/// Must implement <see cref="ILock"/>.
	/// </typeparam>
	public sealed class Barrier<T> : NoCopyable where T : ILock {
		/// <summary>
		/// The minimum counter value assigned after each phase reset.
		/// </summary>
		private uint m_min;

		/// <summary>
		/// The number of expected arrivals required to complete a phase.
		/// This value may be reduced dynamically when threads call
		/// <see cref="ArriveAndDrop"/>.
		/// </summary>
		private uint m_max;

		/// <summary>
		/// The current arrival counter for the active phase.
		/// Incremented atomically by <see cref="Arrive"/>.
		/// </summary>
		private SafeCounter m_current;

		/// <summary>
		/// Tracks the number of completed phases.
		/// Incremented after each successful phase transition.
		/// </summary>
		private SafeCounter m_phase;

		/// <summary>
		/// The lock instance used to block threads until the barrier opens.
		/// </summary>
		private T m_locker;

		/// <summary>
		/// Counts how many threads have requested to drop out of future phases.
		/// Applied atomically during the next phase reset.
		/// </summary>
		private SafeCounter m_drops;

		/// <summary>
		/// Optional callback invoked whenever the barrier completes a phase.
		/// Receives the phase index and the barrier instance.
		/// </summary>
		public Action<int, Barrier<T>> OnComplition;


		/// <summary>
		/// Initializes a new barrier with the specified maximum participant count.
		/// The barrier starts closed and will open once <c>max</c> arrivals occur.
		/// </summary>
		public Barrier ( uint max, T locker )
			: this(0, max, locker, InternalComplition) { }

		/// <summary>
		/// Initializes a new barrier with a completion callback.
		/// </summary>
		public Barrier ( uint max, T locker, Action<int, Barrier<T>> onCompl )
			: this(0, max, locker, onCompl) { }

		/// <summary>
		/// Initializes a new barrier with explicit minimum and maximum counters,
		/// a lock instance, and an optional completion callback.
		/// </summary>
		public Barrier (uint min, uint max, T locker, Action<int, Barrier<T> > onCompl ) {
			if(min == max) {
				m_min = min;
				m_max = min + 1;
			} else {
				m_min = System.Math.Min(min, max);
				m_max = System.Math.Max(min, max); 
			}
			m_current = new SafeCounter(min);
			m_phase = new SafeCounter(0);
			m_drops = new SafeCounter(0);

			OnComplition = onCompl;
			m_locker = locker;
			m_locker.Lock(-1);
		}

		/// <summary>
		/// Signals arrival at the barrier. When the arrival count reaches
		/// <c>m_max</c>, the barrier opens, executes the completion callback,
		/// resets its counters, and begins the next phase.
		/// </summary>
		/// <param name="n">Number of arrival increments to apply.</param>
		/// <param name="waitMS">
		/// Spin‑wait duration (in CPU ticks) while the barrier is open,
		/// allowing other threads to pass through before it closes again.
		/// </param>
		/// <returns>The updated arrival count.</returns>
		public int Arrive ( uint n = 1, uint waitMS = 10 ) {
			while ( n > 0 ) {
				if ( m_current >= m_max )  {
					m_current.Assign(m_max);
					break;
				}

				m_current += n;
				n--;
			}
			var now = (int)m_current.Value;

			if(now == m_max) {
				m_locker.Unlock();

				OnComplition?.Invoke( (int)m_phase.Value, this);

				Thread.SpinWait((int)waitMS);

				if ( m_locker.Lock(-1) ) {
					if( m_drops > 0 ) {
						if( m_max - m_drops >= m_min )
							m_max -= (uint)m_drops.Value;
						else
							throw new BarrierNewPhaseLockException("To many drops");
					}
					m_current.Assign(m_min);
					m_phase++;
				} else {
					throw new BarrierNewPhaseLockException();
				}
			}
			return now;
		}

		/// <summary>
		/// Blocks the calling thread until the current phase completes.
		/// If the barrier is already open, the call returns immediately.
		/// </summary>
		public void Wait () {
			if ( m_locker.Lock(-1) )
				m_locker.Unlock();
		}

		/// <summary>
		/// Convenience method combining <see cref="Arrive"/> and <see cref="Wait"/>.
		/// The thread signals arrival and then waits for the phase to complete.
		/// </summary>
		public void ArriveAndWait ( uint n = 1, uint waitMS = 10 ) {
			Arrive(n, waitMS);
			Wait();
		}

		/// <summary>
		/// Signals arrival at the barrier and requests to drop out of future phases.
		/// The thread participates in the current phase but reduces the expected
		/// participant count for all subsequent phases.
		/// Equivalent to <c>std::barrier::arrive_and_drop()</c> in C++20.
		/// </summary>
		public void ArriveAndDrop ( uint n = 1, uint waitMS = 10 ) {
			int now = Arrive(n, waitMS);

			if(m_max > m_min) {
				m_drops++;
			}
		}
		


		/// <summary>
		/// Default completion callback used when no callback is supplied.
		/// Writes the phase index to the debug console.
		/// </summary>
		private static void InternalComplition ( int phase, Barrier<T> sender ) {
#if DEBUG
			Console.WriteLine("Barrier<T> enter to phase: {0}", phase);
#endif
		}
	}
}
