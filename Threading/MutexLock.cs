using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Threading {
    /// <summary>
    /// A thin wrapper around operating‑system synchronization primitives
    /// (<see cref="Mutex"/> and <see cref="EventWaitHandle"/>).
    /// 
    /// <para>
    /// This class provides a blocking, kernel‑managed mutex lock.  
    /// It is fundamentally different from <see cref="LightMutex{T}"/>, which is a 
    /// non‑blocking, atomic spinlock‑based mutex implemented entirely in user space.
    /// </para>
    /// 
    /// <para>
    /// <b>Important:</b> This OS‑level mutex performs kernel transitions, may block
    /// threads, and does not use atomic operations. It should not be mixed or 
    /// substituted with <see cref="LightMutex{T}"/> or <see cref="LightCountingSpinlock{T}"/> 
    /// unless the synchronization model explicitly requires blocking behavior.
    /// </para>
    /// </summary>
    public sealed class MutexLock : ILock<Mutex> {
        /// <summary>
        /// The underlying operating‑system mutex object.
        /// This mutex is managed by the Windows kernel and may cause blocking
        /// and context switches when contended.
        /// </summary>
        private readonly Mutex m_isMutex;

        /// <summary>
        /// An OS‑level auto‑reset event used for external wait signaling.
        /// This is not part of the mutex semantics itself, but provided as an
        /// auxiliary synchronization mechanism.
        /// </summary>
        private readonly EventWaitHandle m_evntHandle;
        /// <summary>
        /// The name used to identify the mutex and event handle.
        /// </summary>
        private string m_strName;

        /// <summary>
        /// Indicates whether the current thread has successfully acquired the mutex.
        /// This flag is purely local state and does not reflect global ownership
        /// in the operating system.
        /// </summary>
        private bool m_bLocked;

        /// <summary>
        /// Gets the underlying OS mutex handle.
        /// </summary>
        public Mutex Handle => m_isMutex;

        /// <summary>
        /// Gets the name associated with this mutex instance.
        /// </summary>
        public string Name => m_strName;

        /// <summary>
        /// Gets a value indicating whether the mutex is currently held by this instance.
        /// Note that this does not guarantee that the current thread is the owner,
        /// only that <see cref="Lock(TimeSpan)"/> or <see cref="Lock(int)"/> returned true.
        /// </summary>
        public bool IsLocked => m_bLocked;

        /// <summary>
        /// Initializes a new instance of the <see cref="MutexLock"/> class.
        /// Creates an OS‑level named mutex and a corresponding auto‑reset event.
        /// </summary>
        /// <param name="name">The base name used for the mutex and event handle.</param>
        public MutexLock ( string name ) {
            m_strName = name;
            m_isMutex = new Mutex(false, name + ":mutex");
            m_evntHandle = new EventWaitHandle(false, EventResetMode.AutoReset, name + ":eventhandle");
        }
        /// <summary>
        /// Attempts to acquire the OS‑level mutex within the specified timeout.
        /// This operation may block the calling thread.
        /// </summary>
        /// <param name="span">The timeout duration.</param>
        /// <returns>
        /// <c>true</c> if the mutex was acquired; otherwise <c>false</c>.
        /// </returns>
        public bool Lock ( TimeSpan span ) =>
            (m_bLocked = m_isMutex.WaitOne(span));

        /// <summary>
        /// Attempts to acquire the OS‑level mutex 
        /// </summary>
        /// <returns>
        /// <c>true</c> if the mutex was acquired; otherwise <c>false</c>.
        /// </returns>
        public bool TryLock (  ) =>
            (m_bLocked = m_isMutex.WaitOne());

		/// <summary>
		/// Attempts to acquire the OS‑level mutex within the specified timeout.
		/// This is functionally identical to <see cref="Lock(TimeSpan)"/>.
		/// </summary>
		/// <param name="span">The timeout duration.</param>
		/// <returns>
		/// <c>true</c> if the mutex was acquired; otherwise <c>false</c>.
		/// </returns>
		public bool TryLock ( TimeSpan span ) =>
			(m_bLocked = m_isMutex.WaitOne(span));

		/// <summary>
		/// Releases the OS‑level mutex. Only the owning thread may call this method.
		/// </summary>
		public void Unlock () {
            m_isMutex.ReleaseMutex();
            m_bLocked = false;
        }

        /// <summary>
        /// Waits on the associated event handle for the specified duration.
        /// The mutex is temporarily released before waiting and reacquired afterwards.
        /// </summary>
        /// <param name="span">The timeout duration.</param>
        /// <param name="exitContext">
        /// Ignored. Provided for compatibility with WaitHandle semantics.
        /// </param>
        /// <returns>
        /// <c>true</c> if the event was signaled; otherwise <c>false</c>.
        /// </returns>
        public bool Wait ( TimeSpan span, bool exitContext ) {
            Unlock();
            bool r = m_evntHandle.WaitOne(span);
            Lock(TimeSpan.Zero);
            return r;
        }

        /// <summary>
        /// Attempts to acquire the OS‑level mutex within the specified timeout in milliseconds.
        /// </summary>
        /// <param name="ms">Timeout in milliseconds.</param>
        /// <returns>
        /// <c>true</c> if the mutex was acquired; otherwise <c>false</c>.
        /// </returns>
        public bool Lock ( int ms ) {
            return (m_bLocked = m_isMutex.WaitOne(ms));
        }

    }

}
