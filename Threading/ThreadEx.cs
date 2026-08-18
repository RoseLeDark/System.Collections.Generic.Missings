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

using SystemEx.Collections.Generic;
using SystemEx.Numeric;

namespace SystemEx.Threading {

    /// <summary>
    /// Represents the lifecycle states used by <see cref="ThreadEx"/> and
    /// lightweight thread implementations. These states describe creation, startup,
    /// execution, pausing, and termination requests in a minimal and predictable manner.
    /// </summary>
    public enum ThreadExState {

        /// <summary>
        /// The thread object is being constructed and has not yet been started.
        /// </summary>
        Creating,

        /// <summary>
        /// The thread has been scheduled to start (Start() was called), but its
        /// execution routine has not begun running yet.
        /// </summary>
        Waiting,

        /// <summary>
        /// The thread has begun executing its entry routine.
        /// </summary>
        Started,

        /// <summary>
        /// A cooperative request to abort the thread has been issued. The thread
        /// should terminate itself at the next safe point.
        /// </summary>
        RequestAbort,

        /// <summary>
        /// A forced termination request has been issued. This indicates an immediate
        /// kill operation rather than a cooperative shutdown.
        /// </summary>
        RequestKill,

        /// <summary>
        /// The thread is paused and will resume only when explicitly signaled.
        /// </summary>
        Pause,

        /// <summary>
        /// The thread is suspended and not allowed to continue until a resume
        /// operation occurs.
        /// </summary>
        Suspend,

        /// <summary>
        /// The thread has fully stopped and will not execute further instructions.
        /// </summary>
        Stopped,

        /// <summary>
        /// The thread is actively running and executing its assigned work.
        /// </summary>
        Running
    }


    /// <summary>
    /// Provides a managed thread wrapper with event‑based control
    /// mechanisms. <see cref="ThreadEx"/> supports starting, stopping, pausing,
    /// resuming, joining, and sending custom events to the running thread.
    /// 
    /// The class uses <see cref="EventGroup{TFastType}"/> for thread state signaling
    /// and <see cref="LightLock"/> for synchronization. Several callback hooks allow
    /// custom behavior during setup, execution, pausing, cleanup, and termination.
    /// </summary>
    /// <remarks>
    /// ThreadEx is designed for deterministic thread control without relying on
    /// heavy synchronization primitives. All state transitions are represented as
    /// bit flags inside an <see cref="EventGroup{Fast_Int}"/>.
    /// 
    /// Reserved event bits:
    /// <list type="bullet">
    /// <item><description>0 — Joinable</description></item>
    /// <item><description>1 — Started</description></item>
    /// <item><description>3 — Pause requested</description></item>
    /// <item><description>4 — Continue requested</description></item>
    /// </list>
    /// 
    /// Custom events may use bits ≥ 4.
    /// </remarks>

    public class ThreadEx  {
        private const byte EVENTGROUP_BIT_JOINABLE = 0;
        private const byte EVENTGROUP_BIT_STARTED =  1;
        private const byte EVENTGROUP_BIT_PAUSE    = 3;
        private const byte EVENTGROUP_BIT_CONTINUE = 4;


        private Thread m_base;
        private Object m_userData;
        private int m_retval;
        private bool m_bRunning;
        private volatile EventGroup<Fast_Int> m_eventGroup;

        private LightLock m_continuemutex;
        private LightLock m_runningMutex;
        private LightLock m_contextMutext;
        private volatile bool m_runn = true;

        private ThreadExState m_eState;
        /// <summary>
        /// Lock Running LightLock
        /// </summary>
        /// <returns></returns>
        protected bool LockRunning () {
            return m_runningMutex.Lock();
        }

        /// <summary>
        /// UnLock Running LightLock
        /// </summary>
        protected void UnlockRunning () {
			m_runningMutex.Unlock();
		}

        /// <summary>
        /// Occurs when the thread performs its main task loop. Called repeatedly
        /// while the thread is running and not suspended.
        /// </summary>
        public Action<ThreadEx, Object>? OnTask { get; set; }

        /// <summary>
        /// Occurs when the thread begins execution, before entering the main loop.
        /// </summary>
        public Action<ThreadEx, Object>? OnBegin { get; set; }

        /// <summary>
        /// Occurs when the thread finishes execution and performs cleanup.
        /// </summary>
        public Action<ThreadEx, Object>? OnCleanUp { get; set; }

        /// <summary>
        /// Occurs when the thread resumes from a suspended state.
        /// </summary>
        public Action<ThreadEx, Object>? OnResume { get; set; }

        /// <summary>
        /// Occurs when the thread is terminated via <see cref="Abort"/> or <see cref="Kill"/>.
        /// </summary>
        public Action<ThreadEx, bool>? OnKill { get; set; }

        /// <summary>
        /// Initializes a new <see cref="ThreadEx"/> instance with the specified thread
        /// priority and optional stack size. The thread object is created but not
        /// started.
        /// </summary>
        /// <param name="strName">Rhe name</param>
        /// <param name="prio">The priority assigned to the underlying thread.</param>
        /// <param name="stackSize">
        /// Optional stack size in bytes. A value of 0 uses the system default.
        /// </param>
        /// <remarks>
        /// The thread entry point is <see cref="ThreadStart(object)"/>.
        /// </remarks>
        public ThreadEx (string strName, ThreadPriority prio, int stackSize = 0)  {
            m_base = new Thread(ThreadStart, stackSize);
            m_base.Priority = prio;
            m_base.Name = strName;
            
           // m_base.Start(this);

            m_continuemutex = new LightLock();
            m_runningMutex = new LightLock();
            m_contextMutext = new LightLock();

            m_eventGroup = new EventGroup<Fast_Int>();

            OnTask = null;
            OnBegin = null;
            OnCleanUp = null;
            OnResume = null;
            OnKill = null;
            m_userData = 0;
            m_eState = ThreadExState.Creating;
        }

        /// <summary>
        /// Starts the thread and waits until the thread signals that it has begun
        /// execution or until the optional timeout expires.
        /// </summary>
        /// <param name="userData">
        /// Arbitrary user data passed to all callback handlers.
        /// </param>
        /// <param name="timeoutMS">
        /// Maximum time in milliseconds to wait for the thread to enter the running
        /// state. A value of -1 indicates an infinite wait.
        /// </param>
        /// <returns>
        /// <c>true</c> if the thread successfully started and signaled the
        /// <c>Started</c> event; otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method is thread‑safe and ensures that a thread cannot be started
        /// twice concurrently.
        /// </remarks>

        public bool Start ( Object userData, int timeoutMS = -1 ) {
            m_userData = userData;

            m_continuemutex.Lock() ;
            m_runningMutex.Lock() ;
            m_contextMutext.Lock();

			if ( m_bRunning ) {
                m_runningMutex.Unlock();
                m_continuemutex.Unlock();
				m_contextMutext.Unlock();
				return false;
            }
            m_runningMutex.Unlock();

            m_base.Start(this);

            //System.Threading.Thread.Sleep(100);
            if( !m_eventGroup.Wait(EVENTGROUP_BIT_STARTED, timeoutMS)) {
                m_continuemutex.Unlock();
                m_runningMutex.Unlock();
				m_contextMutext.Unlock();

				return false;
            }

            m_runningMutex.Lock();
            m_eState = ThreadExState.Waiting;
            m_continuemutex.Unlock();
            m_runningMutex.Unlock();
			m_contextMutext.Unlock();

			return true;
        }

        /// <summary>
        /// Requests termination of the running thread. Optionally interrupts the thread
        /// if <paramref name="hardt"/> is <c>true</c>.
        /// </summary>
        /// <param name="hardt">
        /// Indicates whether the thread should be interrupted via
        /// <see cref="Thread.Interrupt"/>.
        /// </param>
        /// <returns>
        /// Status code:
        /// <list type="bullet">
        /// <item><description>0 — Success</description></item>
        /// <item><description>1 — Thread not running</description></item>
        /// <item><description>42 — Abort called from the same thread</description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// If the thread is suspended, a continue event is automatically issued.
        /// </remarks>

        public int Abort (bool hardt) {
            m_continuemutex.Lock () ;
            m_runningMutex.Lock() ;

            if ( !m_bRunning ) {
                m_runningMutex.Unlock();
                m_continuemutex.Unlock();

                return 1; // ERR_TASK_NOTRUNNING;
            }

            // nicht aus sich selbst killen
            if ( Thread.CurrentThread.ManagedThreadId == m_base.ManagedThreadId ) {

                m_runningMutex.Unlock();
                m_continuemutex.Unlock();

                return 42; // ERR_TASK_CALLFROMSELFTASK
            }


			

			m_runn = false;
            m_eState =  (hardt)? ThreadExState.RequestKill : ThreadExState.RequestAbort;
            if ( hardt ) m_base.Interrupt();

            if ( IsSuspend() )
                m_eventGroup.Set(EVENTGROUP_BIT_CONTINUE);

			m_contextMutext.Lock();
			OnStop(this, hardt);
			m_contextMutext.Unlock();

			m_runningMutex.Unlock();
            m_continuemutex.Unlock();

			

			return 0; // ERR_TASK_OK;
        }
        /// <summary>
        /// Requests a graceful stop of the thread.
        /// </summary>
        /// <returns>See <see cref="Abort(bool)"/> for return codes.</returns>

        public int Stop () => Abort(false);

        /// <summary>
        /// Requests an immediate stop of the thread via interruption.
        /// </summary>
        /// <returns>See <see cref="Abort(bool)"/> for return codes.</returns>

        public int Kill() => Abort(true);


        /// <summary>
        /// Aborts the thread and waits for it to finish execution.
        /// </summary>
        /// <param name="hardt">
        /// Indicates whether the thread should be interrupted.
        /// </param>
        /// <param name="timeoutMs">
        /// Maximum time in milliseconds to wait for the thread to finish.
        /// </param>
        /// <returns>
        /// Status code from <see cref="Abort(bool)"/> or the result of
        /// <see cref="Join(int)"/>.
        /// </returns>

        public int AbortAndWait ( bool hardt = false, int timeoutMs = -1 ) {
            // Erst stoppen
            int stopResult = Abort(hardt);
            if ( stopResult != 0 )
                return stopResult; // Fehler weitergeben

            // Dann auf sauberes Ende warten
            return Join(timeoutMs);
        }

        /// <summary>
        /// Waits for the thread to finish execution or until the timeout expires.
        /// </summary>
        /// <param name="timeOut">
        /// Maximum time in milliseconds to wait. A value of -1 indicates infinite wait.
        /// </param>
        /// <returns>
        /// <list type="bullet">
        /// <item><description>0 — Thread finished</description></item>
        /// <item><description>1 — Thread not running</description></item>
        /// <item><description>2 — Timeout</description></item>
        /// <item><description>42 — Join called from the same thread</description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// Join uses the <c>Joinable</c> event bit to detect thread termination.
        /// </remarks>

        public int Join ( int timeOut = -1 ) {
            if ( !m_base.IsAlive || !m_bRunning ) return 1;//ERR_TASK_NOTRUNNING;

            if ( Thread.CurrentThread.ManagedThreadId == m_base.ManagedThreadId ) {
                Console.WriteLine("WARNING BOT", "Don't do this!! Don't do this.... only you are a cake! ... Bob?");
                return 42;
            }

            return m_eventGroup.Wait(EVENTGROUP_BIT_JOINABLE, timeOut) ? 0 : 2;
        }

        /// <summary>
        /// Requests the thread to enter a suspended state.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the thread is alive and running; otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Suspension is cooperative. The thread checks the pause bit inside its main
        /// loop.
        /// </remarks>

        public bool Suspend() {
            if ( !m_base.IsAlive || !m_bRunning )
                return false;

            m_eventGroup.Set(EVENTGROUP_BIT_PAUSE);
            return true;
        }

        /// <summary>
        /// Requests the thread to resume from a suspended state.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the thread is alive and running; otherwise <c>false</c>.
        /// </returns>

        public bool Resume () {
            if ( !m_base.IsAlive || !m_bRunning )
                return false;

            m_eventGroup.Set(EVENTGROUP_BIT_CONTINUE);
            return true;
        }

        /// <summary>
        /// Sends a custom event to the running thread. Event bits 0–3 are reserved.
        /// </summary>
        /// <param name="Event">The event bit to set.</param>
        /// <returns>
        /// <c>true</c> if the event was delivered; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="Event"/> is less than 4.
        /// </exception>

        public bool SendEvent(byte Event) {
			m_contextMutext.Lock();

			if ( !m_base.IsAlive || !m_bRunning ) return false;//ERR_TASK_NOTRUNNING;
            if ( Event < 4) return false; // 0-3 sind resaviert

            m_eventGroup.Set(Event);

			m_contextMutext.Unlock();

			return true;
        }

        /// <summary>
        /// Determines whether the thread is currently suspended.
        /// </summary>
        /// <returns><c>true</c> if the pause bit is set; otherwise <c>false</c>.</returns>

        public bool IsSuspend () {
            return IsEvent(EVENTGROUP_BIT_PAUSE);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsResume() {
            return IsEvent(EVENTGROUP_BIT_CONTINUE);
        }

        /// <summary>
        /// Checks whether the specified event bit is set.
        /// </summary>
        /// <param name="Event">The event bit to check.</param>
        /// <returns><c>true</c> if the bit is set; otherwise <c>false</c>.</returns>

        protected bool IsEvent(byte Event) {
            return m_eventGroup.IsSet(Event);
        }

        /// <summary>
        /// Waits for the specified event bit to be set or until the timeout expires.
        /// </summary>
        /// <param name="Event">The event bit to wait for.</param>
        /// <param name="timeout">Timeout in milliseconds.</param>
        /// <returns>
        /// <c>true</c> if the event occurred; otherwise <c>false</c>.
        /// </returns>

        protected bool WaitOfEvent(byte Event, int timeout = 1) {
            return m_eventGroup.Wait(Event, timeout);
        }

        /// <summary>
        /// Called before the thread enters its main loop.
        /// </summary>
        /// <param name="e">The thread instance.</param>
        /// <param name="userdata">User data passed to the thread.</param>
        /// <returns>
        /// <c>true</c> if the thread should continue into the main loop.
        /// </returns>

        protected virtual bool OnSetup ( ThreadEx e, Object userdata ) {
            OnBegin?.Invoke(e, userdata);
            return true;
        }

        /// <summary>
        /// Called when the thread is terminated.
        /// </summary>
        /// <param name="e">The thread instance.</param>
        /// <param name="hardt">
        /// Indicates whether the thread was interrupted.
        /// </param>

        protected virtual void OnStop( ThreadEx e, bool hardt) {
            if ( OnKill != null ) OnKill.Invoke(e, hardt);
        }

        /// <summary>
        /// Main execution loop. Handles pause/resume events and repeatedly invokes
        /// <see cref="OnTask"/> while the thread is running.
        /// </summary>
        /// <param name="e">The thread instance.</param>
        /// <param name="userdata">User data passed to the thread.</param>
        /// <returns>
        /// The return value stored in <c>m_retval</c>.
        /// </returns>

        protected virtual int OnRunning ( ThreadEx e, Object userdata ) {
            

            while ( m_runn ) {
                if ( IsSuspend() ) {
                    m_eState = ThreadExState.Pause;
                    WaitOfEvent(EVENTGROUP_BIT_CONTINUE, -1);
                    if ( OnResume != null ) OnResume.Invoke(e, userdata);
                    

                    m_eventGroup.Clear(EVENTGROUP_BIT_CONTINUE);
                    m_eventGroup.Clear(EVENTGROUP_BIT_PAUSE);

                } else {
                    m_eState = ThreadExState.Running;
                    if ( OnTask != null ) OnTask.Invoke(e, userdata);
                }
            }

            return 0;
        }

        /// <summary>
        /// Called after the main loop finishes execution.
        /// </summary>
        /// <param name="e">The thread instance.</param>
        /// <param name="userdata">User data passed to the thread.</param>

        protected virtual void OnExit( ThreadEx e, Object userdata ) {
            if ( OnCleanUp != null ) OnCleanUp.Invoke(e, userdata);
        }

        /// <summary>
        /// Internal thread entry point. Handles setup, execution, cleanup, and
        /// termination signaling.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="ThreadEx"/> instance passed to <see cref="Thread.Start(object)"/>.
        /// </param>
        /// <remarks>
        /// This method sets the <c>Started</c> and <c>Joinable</c> event bits.
        /// </remarks>

        private static void ThreadStart ( object? obj ) {
            int ret = 0;

            if( obj  is ThreadEx thread) {

                thread.m_eventGroup.Set(EVENTGROUP_BIT_STARTED);
                thread.m_eState = ThreadExState.Started;

                // set running
                thread.m_runningMutex.Lock () ;
                thread.m_continuemutex.Lock() ;
                thread.m_bRunning = true;
                thread.m_runningMutex.Unlock();

                if ( thread.OnSetup(thread, thread.m_userData) ) {

                    ret = thread.OnRunning(thread, thread.m_userData);


                    thread.OnExit(thread, thread.m_userData);
                }

                thread.m_runningMutex.Lock () ;
                thread.m_bRunning = false;
                thread.m_eState = ThreadExState.Stopped;
                thread.m_retval = ret;
                thread.m_runningMutex.Unlock();
                thread.m_continuemutex.Unlock();

                // set the join bit
                thread.m_eventGroup.Set(EVENTGROUP_BIT_JOINABLE);
            }
        }
    }
}
