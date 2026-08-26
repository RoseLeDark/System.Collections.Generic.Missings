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
	/// Exception thrown when a barrier phase transition fails due to an invalid
	/// lock state or an inconsistent phase update.
	/// 
	/// <para>
	/// This exception is raised when the barrier attempts to enter a new phase
	/// but the writer lock cannot be reacquired, or when the number of dropped
	/// participants exceeds the allowed threshold.
	/// </para>
	/// 
	/// <para>
	/// Typical failure scenarios include:
	/// <list type="bullet">
	/// <item>
	/// <description>
	/// The barrier cannot reacquire its internal lock after releasing it for
	/// phase completion.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// A phase drop count results in an invalid new maximum participant count.
	/// </description>
	/// </item>
	/// </list>
	/// </para>
	/// </summary>
	public class BarrierNewPhaseLockException : Exception {

		/// <summary>
		/// Initializes a new instance of the <see cref="BarrierNewPhaseLockException"/>
		/// class with no message or inner exception.
		/// </summary>
		public BarrierNewPhaseLockException ()
			: base(null) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BarrierNewPhaseLockException"/>
		/// class with the specified inner exception.
		/// </summary>
		/// <param name="innerException">
		/// The exception that caused the barrier phase transition to fail.
		/// </param>
		public BarrierNewPhaseLockException ( Exception? innerException )
			: base(null, innerException) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BarrierNewPhaseLockException"/>
		/// class with a custom error message.
		/// </summary>
		/// <param name="message">
		/// The message describing the reason for the failure.
		/// </param>
		public BarrierNewPhaseLockException ( string? message )
			: base(message, null) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BarrierNewPhaseLockException"/>
		/// class with a custom error message and an inner exception.
		/// </summary>
		/// <param name="message">
		/// The message describing the reason for the failure.
		/// </param>
		/// <param name="innerException">
		/// The exception that caused the barrier phase transition to fail.
		/// </param>
		public BarrierNewPhaseLockException ( string? message, Exception? innerException )
			: base(message, innerException) {
		}
	}



	/// <summary>
	/// A user‑mode barrier that synchronizes multiple threads across repeated phases.
	/// The barrier waits until a specified number of threads have arrived, then opens
	/// and allows all participants to continue. After the phase completes, the barrier
	/// resets and becomes ready for the next cycle.
	///
	/// The implementation uses two separate locks to keep the logic clean:
	/// <list type="bullet">
	///   <item>
	///     <description>
	///     <c>m_hold</c> controls when the barrier is open or closed. Threads arriving
	///     at the barrier wait until this lock is released.
	///     </description>
	///   </item>
	///   <item>
	///     <description>
	///     <c>m_context</c> protects the phase‑transition logic. Only one thread is
	///     allowed to perform the phase reset, update counters, apply drops, and invoke
	///     the completion callback.
	///     </description>
	///   </item>
	/// </list>
	///
	/// Each thread calls <c>Arrive()</c> to increment the arrival counter. When the
	/// counter reaches the required maximum, the barrier opens and all waiting threads
	/// proceed. Afterward, the barrier resets its counters and prepares for the next
	/// phase. The optional <c>OnComplition</c> callback is invoked whenever a phase
	/// finishes.
	///
	/// Because this is a user‑mode barrier, it does not serialize console output.
	/// Multiple threads may print messages at the same time, causing the text to
	/// appear out of order. This is normal and does not indicate any problem with the
	/// synchronization logic.
	/// </summary>
	public sealed class Barrier : NoCopyable  {
		private AtomicLock m_hold;
		private AtomicLock m_context;
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

		private uint m_maxCounts;
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
		/// Counts how many threads have requested to drop out of future phases.
		/// Applied atomically during the next phase reset.
		/// </summary>
		private SafeCounter m_drops;

		/// <summary>
		/// Optional callback invoked whenever the barrier completes a phase.
		/// Receives the phase index and the barrier instance.
		/// </summary>
		public Action<int, Barrier> OnComplition;

		private long currentPhase = 0;

		public long Current => currentPhase;

		/// <summary>
		/// Initializes a new barrier with the specified maximum participant count.
		/// The barrier starts closed and will open once <c>max</c> arrivals occur.
		/// </summary>
		public Barrier ( uint max  )
			: this(0, max, InternalComplition) { }

		/// <summary>
		/// Initializes a new barrier with a completion callback.
		/// </summary>
		public Barrier ( uint max,  Action<int, Barrier> onCompl )
			: this(0, max, onCompl) { }

		/// <summary>
		/// Initializes a new barrier with explicit minimum and maximum counters,
		/// a lock instance, and an optional completion callback.
		/// </summary>
		public Barrier (uint min, uint max, Action<int, Barrier > onCompl ) {
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
			m_hold = new AtomicLock();
			m_hold.Lock(1000);

			m_context = new AtomicLock();
		}

		public long WaitOpen( uint waitMS = 10 ) {
			while(true) {
				if ( (int)m_current.Value == m_max ) {
					m_current.Assign(m_min);
					break;
				}

				Thread.SpinWait((int)waitMS);
			}

			m_hold.Unlock();

			if ( !m_hold.IsHeld ) {

				m_context.Lock(-1);
					
				OnComplition?.Invoke((int)m_phase.Value, this);
				

				if ( m_hold.Lock(-1) ) {
					if ( m_drops > 0 ) {
						if ( m_max - m_drops >= m_min )
							m_max -= (uint)m_drops.Value;
						else
							throw new BarrierNewPhaseLockException("To many drops");
					}
					m_current.Assign(m_min);
					m_phase++;
					currentPhase = m_phase.Value;
				}

				m_context.Unlock();
			}
		

			return currentPhase;
		}
		LightLock l = new LightLock();

		public int Arrive (uint n) {
			using ( var _l = new ScopedLock<LightLock>(ref l) ) {

				while ( n != 0 ) {
					m_current++;
					n--;
				}

				if ( m_current.Value >= m_max ) {
					m_current.Assign(m_max);
				}

				return (int)m_current.Value;

			}
		}

		/// <summary>
		/// Blocks the calling thread until the current phase completes.
		/// If the barrier is already open, the call returns immediately.
		/// </summary>
		public void Wait () {
			while (! m_context.IsHeld) { Thread.Yield(); }
		}

		/// <summary>
		/// Convenience method combining <see cref="Arrive"/> and <see cref="Wait"/>.
		/// The thread signals arrival and then waits for the phase to complete.
		/// </summary>
		public void ArriveAndWait ( uint n = 1) {
			Arrive(n);
			Wait();
		}

		/// <summary>
		/// Signals arrival at the barrier and requests to drop out of future phases.
		/// The thread participates in the current phase but reduces the expected
		/// participant count for all subsequent phases.
		/// Equivalent to <c>std::barrier::arrive_and_drop()</c> in C++20.
		/// </summary>
		public void ArriveAndDrop ( uint n = 1 ) {
			 Arrive(n);

			if(m_max > m_min) {
				m_drops++;
			}
		}
		


		/// <summary>
		/// Default completion callback used when no callback is supplied.
		/// Writes the phase index to the debug console.
		/// </summary>
		private static void InternalComplition ( int phase, Barrier sender ) {
#if DEBUG
			Console.WriteLine("Barrier<T> enter to phase: {0}", phase);
#endif
		}
	}
	/// @}
}
