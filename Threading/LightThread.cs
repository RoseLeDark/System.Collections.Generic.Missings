using System;
using System.Collections.Generic;
using System.Text;
using SystemEx.Collections.Generic;

namespace SystemEx.Threading {


    /// <summary>
    /// A lightweight and minimal thread implementation designed for simple and common
    /// synchronization scenarios. Unlike the complex and business-oriented threading
    /// abstractions in .NET, <see cref="LightThread"/> provides a straightforward
    /// wait-and-wake mechanism suitable for 90% of everyday use cases.
    /// <para>
    /// The thread can enter a controlled wait state and be resumed explicitly through
    /// <see cref="Signal(bool)"/>. No advanced scheduling, no task frameworks, and no
    /// heavy runtime features are involved.
    /// </para>
    /// </summary>
    public class LightThread : ThreadEx {
        private readonly LightLock m_waitState;
        private readonly AutoResetEvent m_block;

        /// <summary>
        /// Optional callback invoked when the thread is signaled. The boolean parameter
        /// indicates whether the signal originated from a broadcast operation.
        /// </summary>
        public Action<LightThread, bool>? OnSignal { get; set; }

        /// <summary>
        /// Creates a new lightweight thread with the specified priority and optional stack size.
        /// <para>
        /// The thread starts in a locked wait state and will not continue until
        /// <see cref="Signal(bool)"/> is called. This design avoids the overhead of
        /// high-level .NET threading constructs and keeps behavior predictable and simple.
        /// </para>
        /// </summary>
        /// <param name="strName">the name </param>
        /// <param name="prio">The priority of the underlying thread.</param>
        /// <param name="stackSize">Optional stack size for the thread.</param>
        public LightThread (string strName, ThreadPriority prio, int stackSize = 0 ) 
            : base (strName,prio, stackSize) {
            // Begin in a locked logical wait state
            m_waitState = new LightLock();
            m_waitState.Lock();

            // OS-level blocking primitive
            m_block = new AutoResetEvent(false);
        }
        /// <summary>
        /// Resumes the thread from its wait state. This method unlocks the internal
        /// wait lock and signals the OS-level wait handle, allowing the thread to continue.
        /// <para>
        /// This is the lightweight equivalent of a wake operation, without any of the
        /// heavy abstractions found in standard .NET threading APIs.
        /// </para>
        /// </summary>
        /// <param name="bBroadCast">
        /// Indicates whether the signal originated from a broadcast operation.
        /// </param>
        public void Signal (bool bBroadCast) {
            LockRunning();

            m_waitState.Unlock();

            // task_utils::notify_give(this);
            m_block.Set();

            if ( OnSignal != null ) OnSignal.Invoke(this, bBroadCast);
            UnlockRunning();
        }
        /// <summary>
        /// Blocks the thread until the associated <see cref="LightConditionVariable"/>
        /// signals it. The thread is added to the condition variable's wait queue and
        /// temporarily releases the provided lock during the wait period.
        /// <para>
        /// This method provides a minimal and predictable wait mechanism without relying
        /// on .NET's complex task or monitor systems.
        /// </para>
        /// </summary>
        /// <param name="cv">The condition variable to wait on.</param>
        /// <param name="cvl">
        /// The lock protecting the condition variable. It is released before waiting
        /// and reacquired afterwards.
        /// </param>
        /// <param name="timeoutMs"></param>
        /// <returns>
        /// <c>true</c> if the logical wait state was successfully reacquired;
        /// otherwise <c>false</c>.
        /// </returns>
        public bool Wait ( ref LightConditionVariable cv, ref LightLock cvl, int timeoutMs = -1) {
            LockRunning();

            cv.Add(this);

            cvl.Unlock();
            bool _ret = m_waitState.Lock ();
#if GERTDEBUG
            cv.TotalWaits++;
#endif
			cvl.Lock ();

            //task_utils::notify_take(true, timeOut);
            bool _os = true;
            if( timeoutMs > 0 ) _os = m_block.WaitOne(timeoutMs);
            else _os = m_block.WaitOne();

            UnlockRunning();

            return _ret & _os;
        }
    }
}
