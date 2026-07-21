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


using System.Reflection.Metadata;
using System.Text;
using SystemEx.Collections.Generic;

namespace SystemEx.Threading {
    /// <summary>
    /// A lightweight and minimal condition variable designed for simple thread
    /// synchronization scenarios. Unlike the heavy and business‑oriented primitives
    /// in .NET, <see cref="LightConditionVariable"/> provides a straightforward
    /// FIFO wait‑list and wake‑up mechanism suitable for most everyday use cases.
    /// <para>
    /// Threads can register themselves using <see cref="Add(LightThread)"/> and
    /// will be resumed explicitly through <see cref="Signal"/> or
    /// <see cref="Broadcast"/>. No complex monitor logic or advanced runtime
    /// scheduling is involved.
    /// </para>
    /// </summary>
    public struct LightConditionVariable {
        private Deque<LightThread> m_waits;
        private readonly LightLock m_lockable;
        private string m_strName;

        /// <summary>
        /// Has this ConditionVariable any waiters
        /// </summary>
        public bool HasWaiters => !m_waits.IsEmpty;

        /// <summary>
        /// Count of Waiters
        /// </summary>
        public int Count => m_waits.Count;

#if DEBUG
        /// <summary>
        /// Debug callback invoked whenever a thread is added to this condition variable.
        /// The parameters provide the condition variable instance, the thread being added,
        /// and the current number of waiting threads after insertion.
        /// </summary>
        public Action<SimpleConditionVariable, SimpleThread, int>? OnAdd;

        /// <summary>
        /// Debug callback invoked whenever a broadcast operation is performed.
        /// The integer parameter indicates how many threads were signaled.
        /// </summary>
        public Action<SimpleConditionVariable, int>? OnBroadcast;

        /// <summary>
        /// Total number of wait operations registered on this condition variable.
        /// This counter is incremented whenever a thread enters the wait list.
        /// </summary>
        public int TotalWaits { get; internal set; }

        /// <summary>
        /// Total number of single-thread signal operations performed.
        /// This counter reflects how often <see cref="Signal"/> was invoked.
        /// </summary>
        public int TotalSignals { get; private set; }

        /// <summary>
        /// Total number of broadcast operations performed.
        /// This counter reflects how often <see cref="Broadcast"/> was invoked.
        /// </summary>
        public int TotalBroadcasts { get; private set; }
#endif
        /// <summary>
        /// Get The Name of this Object
        /// </summary>
        public string Name => m_strName;

        /// <summary>
        /// Initializes a new lightweight condition variable with an internal
        /// FIFO wait‑list. The structure is intentionally minimal and avoids
        /// the overhead of traditional .NET synchronization constructs.
        /// </summary>
        public LightConditionVariable (string strName) {
            m_waits = new Deque<LightThread>(8, 4);
            m_lockable = new LightLock();
            m_strName = strName;

#if DEBUG
            OnAdd = null;
            OnBroadcast = null;
#endif

        }

        /// <summary>
        /// Wakes a single waiting thread, if any. The first thread in the FIFO
        /// wait‑list is resumed. This operation is intentionally simple and does
        /// not involve any advanced signaling semantics.
        /// </summary>
        public void Signal () {
            m_lockable.Lock();

            if ( m_waits.IsEmpty ) return;

            LightThread? _th;

            if ( m_waits.PopFront(out _th) ) {
                if ( _th != null ) _th.Signal(false);
            }

#if DEBUG
            TotalSignals++;
#endif
            m_lockable.Unlock();
        }

        /// <summary>
        /// Wakes all threads currently waiting on this condition variable.
        /// Each thread in the FIFO wait‑list is resumed in order.
        /// </summary>
        public void Broadcast () {
            m_lockable.Lock();

            while ( !m_waits.IsEmpty ) {
                LightThread? _th;

                if ( m_waits.PopFront(out _th) ) {
                    if ( _th != null ) _th.Signal(true);
#if DEBUG
                    if ( OnBroadcast != null ) OnBroadcast.Invoke(this, m_waits.Count);
#endif
                }

            }

#if DEBUG
            TotalBroadcasts++;
#endif

            m_lockable.Unlock();
        }

        /// <summary>
        /// Convenience method that either wakes a single thread or all threads,
        /// depending on the <paramref name="all"/> parameter.
        /// </summary>
        /// <param name="all">
        /// If <c>true</c>, all waiting threads are resumed; otherwise only one.
        /// </param>
        public void Notify ( bool all = false ) {
            if ( !all ) Signal();
            else Broadcast();
        }

        /// <summary>
        /// Clear all Waiter from the Wait Queue
        /// </summary>
        public void Clear () {
            m_lockable.Lock();
            m_waits.Clear();
            m_lockable.Unlock();
        }


        /// <summary>
        /// Adds a thread to the internal FIFO wait‑list. This method is used by
        /// <see cref="LightThread"/> during its wait operation. The thread will
        /// remain blocked until <see cref="Signal"/> or <see cref="Broadcast"/>
        /// is invoked.
        /// </summary>
        /// <param name="task">The thread to add to the wait‑list.</param>
        internal void Add ( LightThread task ) {
            m_lockable.Lock();
            m_waits.PushBack(task);
#if DEBUG
            if ( OnAdd != null ) OnAdd.Invoke(this, task, m_waits.Count);
#endif
            m_lockable.Unlock();
        }

#if DEBUG
        /// <summary>
        /// Produces a debug dump containing internal statistics and state information
        /// for this condition variable. Intended for diagnostics and development-time
        /// inspection only.
        /// </summary>
        /// <returns>
        /// A formatted string describing wait counts, signal activity, broadcast
        /// operations, and the current number of waiting threads.
        /// </returns>
        public string DumpDebug () {
            m_lockable.Lock();

            var sb = new StringBuilder();

            sb.AppendLine("SimpleConditionVariable Debug Dump");
            sb.AppendLine("----------------------------------");
            sb.AppendLine($"TotalWaits:       {TotalWaits}");
            sb.AppendLine($"TotalSignals:     {TotalSignals}");
            sb.AppendLine($"TotalBroadcasts:  {TotalBroadcasts}");
            sb.AppendLine($"CurrentWaiters:   {m_waits.Count}");

            m_lockable.Unlock();

            return sb.ToString();
        }
#endif

    }
}
