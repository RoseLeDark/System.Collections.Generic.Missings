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
	/// Implements a counting spinlock, a non‑blocking synchronization primitive that
	/// allows a bounded number of threads to enter a critical section concurrently.
	/// Unlike traditional binary spinlocks, which permit only a single thread at a
	/// time, a counting spinlock maintains an atomic counter representing the current
	/// available capacity.
	/// 
	/// <para>
	/// The lock is initialized with a minimum and maximum capacity. Each thread
	/// attempting to enter performs an atomic decrement on the counter:
	/// </para>
	/// 
	/// <list type="bullet">
	/// <item>
	/// <description>
	/// If the resulting value is greater than or equal to <c>minCapacity</c>,
	/// the thread successfully enters the critical section.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// If the value drops below <c>minCapacity</c>, the thread is denied access and
	/// must spin‑wait until capacity becomes available again.
	/// </description>
	/// </item>
	/// </list>
	/// 
	/// <para>
	/// When a thread exits the critical section, it performs an atomic increment,
	/// returning capacity to the lock and allowing waiting threads to proceed.
	/// </para>
	/// 
	/// <para>
	/// Counting spinlocks are strictly non‑blocking and do not yield or sleep while
	/// waiting. This makes them suitable for extremely short critical sections,
	/// high‑performance kernel operations, interrupt handlers, and other environments
	/// where blocking is forbidden.
	/// </para>
	/// 
	/// <para>
	/// Typical use cases include:
	/// </para>
	/// <list type="bullet">
	/// <item><description>Bounded resource pools</description></item>
	/// <item><description>Fixed‑size worker queues</description></item>
	/// <item><description>Throttling parallel operations</description></item>
	/// <item><description>High‑frequency CPU‑bound tasks</description></item>
	/// </list>
	/// 
	/// <para>
	/// Because waiting threads actively spin, excessive contention can degrade system
	/// performance. Counting spinlocks should therefore be used only when critical
	/// sections are extremely short and contention is expected to be low.
	/// </para>
	/// </summary>
	public class LightCountingSpinlock<T> : ISpinlock<bool> {
        private readonly int m_min;
        private readonly int m_max;
        private SafeCounter m_counter;
#pragma warning disable CS1591 // Fehledes XML-Kommentar für öffentlich sichtbaren Typ oder Element
        protected T? m_value;
#pragma warning restore CS1591 // Fehledes XML-Kommentar für öffentlich sichtbaren Typ oder Element
        private readonly string m_name;

        /// <summary>
        /// Gets the descriptive name assigned to this lock instance.
        /// </summary>
        public string Name => m_name;

		/// <summary>
		/// Indicates whether the lock is currently saturated (no capacity left).
		/// </summary>
		public bool IsHeld => m_counter.Value <= m_min;

		/// <summary>
		/// Counting spinlocks do not track thread ownership.
		/// This property always returns <c>false</c>.
		/// </summary>
		public virtual bool IsHeldbyCurrent => false;

        /// <summary>
        /// Indicates whether thread‑owner tracking is enabled.
        /// Counting spinlocks do not support owner tracking.
        /// </summary>
        public virtual bool IsThreadOwnerTrackingEnabled => false;

        /// <summary>
        /// Gets the current counter value representing available capacity.
        /// </summary>
        public bool Handle => m_counter.Value <= m_min;


        /// <summary>
        /// Gets or sets the protected value. Access is permitted only when the
        /// calling thread has successfully acquired capacity.
        /// </summary>
        public virtual T? Value {
            get {
                if ( !IsHeld )
                    throw new UnauthorizedAccessException();
                return m_value;
            }
            set {
                if ( !IsHeld )
                    throw new UnauthorizedAccessException();
                m_value = value;
            }
        }

        bool ILock<bool>.Handle => IsHeld;


        /// <summary>
        /// Initializes a new instance of the <see cref="LightCountingSpinlock{T}"/>
        /// with the specified minimum and maximum capacity.
        /// </summary>
        public LightCountingSpinlock ( int minCapacity, int maxCapacity, string name = "CountingSpinlock" ) {
            if ( maxCapacity <= 0 )
                throw new ArgumentOutOfRangeException(nameof(maxCapacity));

            if ( minCapacity < 0 || minCapacity >= maxCapacity )
                throw new ArgumentOutOfRangeException(nameof(minCapacity));

            m_min = minCapacity;
            m_max = maxCapacity;
            m_counter = new SafeCounter(m_max);
            m_value = default;
            m_name = name;
        }

        /// <summary>
        /// Attempts to acquire capacity using busy‑wait atomic operations.
        /// </summary>
        public virtual bool Lock ( int millisecondsTimeout = -1) {
            bool _ret = false;
            bool _usedTimer = millisecondsTimeout > 0;

            

            if ( _usedTimer ) {
                long start = Environment.TickCount64;

                while ( true ) {

                    // Timeout erreicht?
                    if ( millisecondsTimeout >= 0 ) {
                        long elapsed = Environment.TickCount64 - start;
                        if ( elapsed >= millisecondsTimeout )
                            break; // nicht geschafft
                    }

                    // Wenn keine Kapazität → sofort spinnen
                    if ( IsHeld ) {
                        Thread.Yield();
                        continue;
                    }

                    // Atomar dekrementieren
                    m_counter--;
                    long newValue = m_counter.Value;

                    // Erfolg: nicht unter minCapacity gerutscht
                    if ( newValue >= m_min ) {
                        _ret = true;
						break;
                    }

                    // Unter minCapacity gerutscht → revert + spin
                    m_counter++;
					Thread.Yield();
				}
            } else {

                while ( true ) {

                    // Wenn keine Kapazität → sofort spinnen
                    if ( IsHeld ) {
                        Thread.SpinWait(1);
                        
                        continue;
                    }

                    // Atomar dekrementieren
                    m_counter--;
                    long newValue = m_counter.Value;

                    // Erfolg: nicht unter minCapacity gerutscht
                    if ( newValue >= m_min ) {
                        _ret = true;
						break;
                    }

                    // Unter minCapacity gerutscht → revert + spin
                    m_counter.Increment();
                    Thread.SpinWait(1);
                }

            }

            return _ret;
        }


        /// <summary>
        /// Attempts to acquire capacity using busy‑wait atomic operations.
        /// </summary>
        public virtual bool Lock ( TimeSpan span ) {
            // Unendlich warten, wenn TimeSpan extrem groß ist 
            if ( span == Timeout.InfiniteTimeSpan || span.TotalMilliseconds > int.MaxValue )
                return Lock(-1);

            // Negative oder Null → kein Warten
            if ( span <= TimeSpan.Zero )
                return Lock(0);

            // Normale Umrechnung
            int ms = (int)span.TotalMilliseconds;
            return Lock(ms);
        }
        /// <summary>
        /// Releases capacity by performing an atomic increment.
        /// </summary>
        public virtual void Unlock () {
            var current = m_counter.Value;
            if ( current >= m_max )
                throw new InvalidOperationException("Unlock called too many times.");
	
			m_counter++;
        }

        /// <summary>
        /// Attempts to acquire capacity without blocking.
        /// </summary>
        public virtual bool TryLock ( ) {
            bool _ret = false;

            long newValue = m_counter.Decrement();

            if ( newValue >= m_min ) {
                _ret = true;
			} else
                m_counter++;

            return _ret;
        }

        /// <summary>
        /// Counting spinlocks do not support wait/pulse semantics.
        /// </summary>
        public virtual bool Wait ( TimeSpan span, bool exitContext ) {
            return false;
        }
    }
	
}
