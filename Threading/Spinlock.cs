using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Threading {

    /// <summary>
    /// Provides a thin wrapper around <see cref="System.Threading.SpinLock"/> to expose
    /// a busy‑wait mutual exclusion primitive through the <see cref="ISpinlock{T}"/> interface.
    /// 
    /// <see cref="Spinlock"/> is intended for extremely short critical sections where
    /// blocking or kernel transitions would introduce unnecessary overhead. The lock
    /// repeatedly attempts to acquire ownership using atomic operations and does not
    /// provide any wait/pulse semantics.
    /// </summary>
    public struct Spinlock : ISpinlock<SpinLock> {
        private SpinLock m_spin;
        private readonly string m_name;
        private bool m_isLocked;

        /// <summary>
        /// Gets the descriptive name assigned to this lock instance.
        /// </summary>
        public string Name => m_name;

        /// <summary>
        /// Indicates whether the lock is currently held. This flag is updated whenever
        /// the underlying <see cref="SpinLock"/> is entered or exited.
        /// </summary>
        public bool IsLocked => m_isLocked;

        /// <summary>
        /// Gets the underlying <see cref="System.Threading.SpinLock"/> instance.
        /// </summary>
        public SpinLock Handle => m_spin;

        /// <inheritdoc/>
        public bool IsHeld => m_spin.IsHeld;
        /// <inheritdoc/>
        public bool IsHeldbyCurrent => m_spin.IsHeldByCurrentThread;
        /// <inheritdoc/>
        public bool IsThreadOwnerTrackingEnabled => m_spin.IsThreadOwnerTrackingEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="Spinlock"/> struct.
        /// </summary>
        /// <param name="name">Optional human‑readable name for diagnostics.</param>
        /// <param name="enableThreadOwnerTracking">
        /// Enables internal owner tracking for debugging. This incurs additional overhead
        /// and should be disabled for high‑performance scenarios.
        /// </param>
        public Spinlock ( string name = "SpinLock", bool enableThreadOwnerTracking = false ) {
            m_spin = new SpinLock(enableThreadOwnerTracking);
            m_name = name;
            m_isLocked = false;
        }

        /// <summary>
        /// Attempts to acquire the lock using a millisecond timeout. The calling thread
        /// repeatedly attempts to enter the underlying <see cref="SpinLock"/> until the
        /// timeout expires or the lock is acquired.
        /// </summary>
        /// <param name="ms">Timeout in milliseconds, or <c>-1</c> for immediate entry.</param>
        /// <returns><c>true</c> if the lock was acquired; otherwise <c>false</c>.</returns>
        public bool Lock ( int ms ) {
            if ( ms < 0 ) {
                m_spin.Enter(ref m_isLocked);
            } else {
                var lockTaken = false;
                var sw = System.Diagnostics.Stopwatch.StartNew();

                while ( !lockTaken && sw.ElapsedMilliseconds < ms ) {
                    m_spin.TryEnter(0, ref lockTaken);
                    if ( !lockTaken )
                        Thread.SpinWait(1);
                }

                m_isLocked = lockTaken;
            }

            return m_isLocked;
        }

        /// <summary>
        /// Attempts to acquire the lock using a <see cref="TimeSpan"/> timeout.
        /// </summary>
        /// <param name="span">Maximum duration to attempt acquiring the lock.</param>
        /// <returns><c>true</c> if the lock was acquired; otherwise <c>false</c>.</returns>
        public bool Lock ( TimeSpan span ) {
            if ( span.Ticks <= 0 ) {
                m_spin.Enter(ref m_isLocked);
            } else {
                var lockTaken = false;
                var sw = System.Diagnostics.Stopwatch.StartNew();

                while ( !lockTaken && sw.Elapsed < span ) {
                    m_spin.TryEnter(0, ref lockTaken);
                    if ( !lockTaken )
                        Thread.SpinWait(1);
                }

                m_isLocked = lockTaken;
            }

            return m_isLocked;
        }

        /// <summary>
        /// Releases the lock by exiting the underlying <see cref="SpinLock"/>. The caller
        /// must ensure that every successful lock acquisition is paired with a matching
        /// call to <see cref="Unlock"/>.
        /// </summary>
        public void Unlock () {
            if ( m_isLocked ) {
                m_spin.Exit();
                m_isLocked = false;
            }
        }

        /// <summary>
        /// Spin locks do not support wait‑and‑pulse semantics. This method always returns
        /// <c>false</c> and performs no operation.
        /// </summary>
        public bool Wait ( TimeSpan span, bool exitContext ) {
            return false;
        }

        /// <summary>
        /// Attempts to acquire the lock using a <see cref="TimeSpan"/> timeout without
        /// implying a blocking intent. This method is functionally identical to
        /// <see cref="Lock(TimeSpan)"/>.
        /// </summary>
        public bool TryLock ( TimeSpan span ) {
            if ( span.Ticks <= 0 ) {
                m_spin.TryEnter(0, ref m_isLocked);
            } else {
                var lockTaken = false;
                var sw = System.Diagnostics.Stopwatch.StartNew();

                while ( !lockTaken && sw.Elapsed < span ) {
                    m_spin.TryEnter(0, ref lockTaken);
                    if ( !lockTaken )
                        Thread.SpinWait(1);
                }

                m_isLocked = lockTaken;
            }

            return m_isLocked;
        }
    }


}
