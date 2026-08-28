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

using System.Diagnostics;
using SystemEx.Collections.Generic;
using SystemEx.Numeric;

namespace SystemEx.Threading {
	/// \addtogroup  Threading
	/// @{

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
	/// Defines the types of invoke messages that can be delivered to a
	/// <see cref="LightThread"/> during an active invoke context.
	/// 
	/// Each message type represents a different category of thread‑side
	/// operation, ranging from general data delivery to coroutine scheduling
	/// and stack transfer.
	/// </summary>
	public enum ThreadInvokeMessage {
		/// <summary>
		/// A general-purpose message containing arbitrary external or internal
		/// data for the thread.  
		/// Typical use cases include input events (e.g., mouse or device data),
		/// sensor updates, or any other runtime information the thread may
		/// consume.
		/// </summary>
		Message,

		/// <summary>
		/// A property or configuration update for the thread.  
		/// Used to modify thread-specific settings, behavior flags, or other
		/// control parameters.
		/// </summary>
		Propertie,

		/// <summary>
		/// A coroutine scheduling request.  
		/// The message payload contains the entry point for a new coroutine
		/// that the thread should initialize and execute in future cycles.
		/// </summary>
		CoRoutine,

		/// <summary>
		/// A stack transfer message used by coroutine execution.  
		/// Provides a stack of objects that represents coroutine state,
		/// arguments, or continuation data for internal coroutine processing.
		/// </summary>
		Stack
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
	public partial class ThreadEx  {
		public static readonly byte EVENTGROUP_BIT_JOINABLE = 0;
		public static readonly byte EVENTGROUP_BIT_STARTED =  1;
		public static readonly byte EVENTGROUP_BIT_PAUSE    = 3;
		public static readonly byte EVENTGROUP_BIT_CONTINUE = 4;

        public static readonly byte EVENTGROUP_BIT_INVOKESTATE = 5;

		public static readonly byte EVENTGROUP_BIT_INVOKEMESSAGE = 6;

		private volatile Collections.Generic.Queue< Triple<byte, ThreadInvokeMessage, object > > m_contextQueue;


		private Thread m_base;
        private Object? m_userData;
        private int m_retval;
        private bool m_bRunning;
        protected volatile EventGroup<Fast_Int> m_eventGroup;

        private Spinlock m_continuemutex;
        private Spinlock m_runningMutex;
        private MutexLock  m_contextMutext;
        private volatile bool m_runn = true;

        private Collections.Generic.Stack<Object> m_coRoutineStack;

        private Collections.Generic.Deque<Object> m_messageQueue;

		protected ThreadExState m_eState;
        /// <summary>
        /// Lock Running LightLock
        /// </summary>
        /// <returns></returns>
        protected bool LockRunning () {
            return m_runningMutex.Lock(-1);
        }

        /// <summary>
        /// UnLock Running LightLock
        /// </summary>
        protected void UnlockRunning () {
            if( m_runningMutex.IsHeld )
			    m_runningMutex.Unlock();
		}

        /// <summary>
        /// Occurs when the thread performs its main task loop. Called repeatedly
        /// while the thread is running and not suspended.
        /// </summary>
        public Func<ThreadEx, Object?, int>? OnTask { get; set; }

        /// <summary>
        /// Occurs when the thread begins execution, before entering the main loop.
        /// </summary>
        public Action<ThreadEx, Object?>? OnBegin { get; set; }

        /// <summary>
        /// Occurs when the thread finishes execution and performs cleanup.
        /// </summary>
        public Action<ThreadEx, Object?>? OnCleanUp { get; set; }

        /// <summary>
        /// Occurs when the thread resumes from a suspended state.
        /// </summary>
        public Action<ThreadEx, Object?>? OnResume { get; set; }

        /// <summary>
        /// Occurs when the thread is terminated via <see cref="Abort"/> or <see cref="Kill"/>.
        /// </summary>
        public Action<ThreadEx, bool>? OnKill { get; set; }

        public int ID { get; private set; }

        /// <summary>
        /// Get the name of the thread.
        /// </summary>
        public string Name => m_base.Name == null ? "ThreadX" : m_base.Name;

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
        public ThreadEx (string strName, ThreadPriority prio, int stackSize )  {
            m_base = new Thread(ThreadStart, stackSize);
            m_base.Priority = prio;
            m_base.Name = strName;
            
           // m_base.Start(this);

            m_continuemutex = new Spinlock();
            m_runningMutex = new Spinlock();
            m_contextMutext = new MutexLock($"{strName}_cmt");

            m_eventGroup = new EventGroup<Fast_Int>();

            OnTask = null;
            OnBegin = null;
            OnCleanUp = null;
            OnResume = null;
            OnKill = null;
            m_userData = 0;
            m_eState = ThreadExState.Creating;
            m_contextQueue = new Collections.Generic.Queue<Triple<byte, ThreadInvokeMessage, object>>(4, 2);
            m_coRoutineStack = new Collections.Generic.Stack<object>(64);
            m_messageQueue = new Deque<object>();

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

		public bool Start ( Object? userData, int timeoutMS = -1 ) {
            m_userData = userData;

            m_continuemutex.Lock(-1) ;
            m_runningMutex.Lock(-1) ;
            m_contextMutext.Lock(-1);

			if ( m_bRunning ) {
                m_runningMutex.Unlock();
                m_continuemutex.Unlock();
				m_contextMutext.Unlock();
				return false;
            }
            

            m_base.Start(this);

            //System.Threading.Thread.Sleep(100);
            if( !m_eventGroup.Wait(EVENTGROUP_BIT_STARTED, timeoutMS)) {
                m_continuemutex.Unlock();
                m_runningMutex.Unlock();
				m_contextMutext.Unlock();

				return false;
            }

            
            m_eState = ThreadExState.Waiting;
            m_continuemutex.Unlock();
            m_runningMutex.Unlock();
			m_contextMutext.Unlock();

			return true;
        }
		private Stopwatch m_CommitWindow = new Stopwatch();
        private int m_CommitWinOpen = 0;

		/// <summary>
		/// Begins a new invoke context by acquiring the context mutex, enabling
		/// the invoke-state bit, and opening a timed commit window.  
		/// 
		/// The commit window duration is defined by <paramref name="msOpen"/> and
		/// is measured using the thread-local commit stopwatch.  
		/// While the invoke-state bit is set, commit operations may enqueue
		/// messages or finalize the context until the commit window expires.
		/// </summary>
		/// <param name="msOpen">
		/// The duration of the commit window in milliseconds.  
		/// Commit operations must occur within this time span; otherwise the
		/// context is automatically terminated by the running thread.
		/// </param>
		/// <returns>
		/// <c>true</c> if the context mutex was successfully acquired and the
		/// invoke context was started;  
		/// <c>false</c> if the mutex could not be locked and the invoke context
		/// could not be initiated.
		/// </returns>
		public bool Invoke (int msOpen = 1000) {

            if ( m_contextMutext.Lock(-1) ) {
                SendEvent(EVENTGROUP_BIT_INVOKESTATE);

                m_CommitWinOpen = msOpen;
				m_CommitWindow.Start();

				return true;
            }

            return false;
		}

		/// <summary>
		/// Finalizes the active invoke context without adding a message, provided
		/// the commit window has not expired.  
		/// 
		/// If the commit window is still open and <paramref name="end"/> is true,
		/// the invoke context is terminated.  
		/// If the commit window has elapsed, <see cref="checkEndContext"/> handles
		/// the automatic context termination.
		/// </summary>
		/// <param name="end">
		/// Indicates whether the invoke context should be terminated.  
		/// When <c>true</c>, the invoke-state bit is cleared, the commit window
		/// timer is stopped and reset, and the context mutex is released.
		/// </param>
		/// <returns>
		/// <c>true</c> if the invoke context was still active and the commit
		/// operation completed successfully;  
		/// <c>false</c> if the commit window had already expired and the context
		/// was automatically terminated.
		/// </returns>
		public bool Commit ( bool end = true ) {
			if ( !m_contextMutext.IsHeld ) {
				throw new InvalidOperationException("Not Invoke state");
			}

			if ( ! cheackEndContext(false) ) {

				if ( end ) {
					m_eventGroup.Clear(EVENTGROUP_BIT_INVOKESTATE);
					m_CommitWindow.Stop();
					m_CommitWindow.Reset();
					m_contextMutext.Unlock();
				}
			} else {
				return false;
			}
			return true;
		}

		/// <summary>
		/// Commits a message into the active invoke context, provided the commit
		/// window has not expired.  
		/// 
		/// If the commit window is still open, the message is appended to the
		/// context queue with the specified priority.  
		/// When <paramref name="end"/> is true, the invoke context is terminated
		/// after the message is committed.
		/// 
		/// If the commit window has already elapsed, the context is automatically
		/// closed by <see cref="checkEndContext"/> and no message is added.
		/// </summary>
		/// <param name="message">
		/// The message payload consisting of a ThreadInvokeMessage identifier and an
		/// associated object.
		/// </param>
		/// <param name="prio">
		/// The priority value assigned to the message.
		/// </param>
		/// <param name="end">
		/// Indicates whether the invoke context should be terminated after
		/// committing the message.  
		/// When <c>true</c>, the invoke-state bit is cleared, the commit window
		/// timer is stopped and reset, and the context mutex is released.
		/// </param>
		/// <returns>
		/// <c>true</c> if the message was successfully committed while the invoke
		/// context was still active;  
		/// <c>false</c> if the commit window had already expired and the context
		/// was automatically closed.
		/// </returns>
		public bool Commit( ThreadInvokeMessage type, Object message, byte prio, bool end = false ) {
            if( !m_contextMutext.IsHeld ) {
                throw new InvalidOperationException("Not Invoke state");
            }

            if ( ! cheackEndContext(false) ) {
                m_contextQueue.PushBack(new Triple<byte, ThreadInvokeMessage, object> (prio, type, message ) );


                if ( end ) {
                    m_eventGroup.Clear(EVENTGROUP_BIT_INVOKESTATE);
                    m_CommitWindow.Stop();
                    m_CommitWindow.Reset();
                    m_contextMutext.Unlock();
                }
            } else {
                return false;
            }
            return true;
        }

	

		/// <summary>
		/// Aborts the active invoke context.  
		/// 
		/// If the invoke-state bit is set, the context is terminated immediately
		/// through <see cref="checkEndContext"/> using abort semantics.  
		/// All queued messages are discarded, the invoke-state bit is cleared,
		/// the commit window timer is stopped and reset, and the context mutex
		/// is released.
		/// </summary>
		/// <returns>
		/// <c>true</c> if an active invoke context was aborted;  
		/// <c>false</c> if no invoke context was active.
		/// </returns>
		public bool Abandon () {
            if ( !IsEvent(EVENTGROUP_BIT_INVOKESTATE) ) 
                return false;

			cheackEndContext(true);

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
            m_continuemutex.Lock (-1) ;
            m_runningMutex.Lock(-1) ;

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

			m_contextMutext.Lock(-1);
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
			m_contextMutext.Lock(-1);

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

        protected virtual bool OnSetup ( ThreadEx e, Object? userdata ) {
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

        protected virtual int OnRunning ( ThreadEx e, Object? userdata ) {

			

			int _ret = 0;

            while ( m_runn && _ret == 0) {

				if ( m_eventGroup.IsSet(EVENTGROUP_BIT_INVOKESTATE) ) {

                    cheackEndContext(false);
				}

                if( m_eventGroup.IsSet(EVENTGROUP_BIT_INVOKEMESSAGE) )  {
                    m_eventGroup.Clear(EVENTGROUP_BIT_INVOKEMESSAGE);

					while (!m_contextQueue.IsEmpty) {
                        Optional<Triple<byte, ThreadInvokeMessage, object>> _ref = new Triple<byte, ThreadInvokeMessage, object>();

						if ( !m_contextQueue.PopFront(ref _ref) )
							continue;


						var msgType = _ref.Value!.Second;
						var payload = _ref.Value!.Third;

						switch ( msgType ) {

						case ThreadInvokeMessage.Message:
						    m_messageQueue.PushBack(payload);
						    break;

						case ThreadInvokeMessage.Stack:
						    if ( payload is Pair<byte, object> g )
							    m_coRoutineStack.Push(g.Second, g.First);
						    break;

						case ThreadInvokeMessage.Propertie:
						    break;
						case ThreadInvokeMessage.CoRoutine:
						    break;
						}
					}

				}

				if ( IsSuspend() ) {
                    m_eState = ThreadExState.Pause;
                    WaitOfEvent(EVENTGROUP_BIT_CONTINUE, -1);
                    if ( OnResume != null ) OnResume.Invoke(e, userdata);
                    

                    m_eventGroup.Clear(EVENTGROUP_BIT_CONTINUE);
                    m_eventGroup.Clear(EVENTGROUP_BIT_PAUSE);

                } else {
                    m_eState = ThreadExState.Running;
                    if ( OnTask != null ) {
						_ret =  OnTask.Invoke(e, userdata);
                    }
                }
            }

            return _ret;
        }

		/// <summary>
		/// Checks whether the current commit window has expired and, if so,
		/// terminates the active invoke context.  
		/// 
		/// If <paramref name="abort"/> is true, all queued context messages are
		/// discarded. Otherwise, if the queue contains pending messages, the
		/// thread-local event bit for "messages available" is set.
		/// 
		/// This method clears the invoke-state bit, stops and resets the commit
		/// window timer, and releases the context mutex.  
		/// It returns <c>true</c> only when the commit window has elapsed and the
		/// context was actually terminated.
		/// </summary>
		/// <param name="abort">
		/// Indicates whether the context should be aborted.  
		/// When <c>true</c>, all queued messages are removed.  
		/// When <c>false</c>, pending messages are preserved and the
		/// "messages available" event bit is set.
		/// </param>
		/// <returns>
		/// <c>true</c> if the commit window has expired and the invoke context
		/// was terminated; otherwise <c>false</c>.
		/// </returns>
		private bool cheackEndContext(bool abort) {

			if ( m_CommitWindow.ElapsedMilliseconds <= m_CommitWinOpen )
				return false;

			if ( abort ) {
				m_contextQueue.Clear();
			} else if ( !m_contextQueue.IsEmpty ) {
				m_eventGroup.Set((byte)(EVENTGROUP_BIT_INVOKESTATE + 1));
			}

			m_eventGroup.Clear(EVENTGROUP_BIT_INVOKESTATE);
			m_CommitWindow.Stop();
			m_CommitWindow.Reset();
			m_contextMutext.Unlock();

            return true;
		}

        /// <summary>
        /// Called after the main loop finishes execution.
        /// </summary>
        /// <param name="e">The thread instance.</param>
        /// <param name="userdata">User data passed to the thread.</param>

        protected virtual void OnExit( ThreadEx e, Object? userdata ) {
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
                thread.m_runningMutex.Lock (-1) ;
                thread.m_continuemutex.Lock(-1) ;
                thread.m_bRunning = true;
                //thread.m_runningMutex.Unlock();
                thread.ID = Thread.CurrentThread.ManagedThreadId;


				if ( thread.OnSetup(thread, thread.m_userData) ) {

					thread.m_runningMutex.Unlock();
					ret = thread.OnRunning(thread, thread.m_userData);


					thread.OnExit(thread, thread.m_userData);
				}


				thread.m_runningMutex.Lock (-1) ;
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
