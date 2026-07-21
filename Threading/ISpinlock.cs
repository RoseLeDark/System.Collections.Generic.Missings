using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Threading {

    /// <summary>
    /// Extends <see cref="ILock{T}"/> with spin‑lock‑specific diagnostics and metadata.
    /// Implementations of <see cref="ISpinlock{T}"/> represent busy‑wait mutual exclusion
    /// primitives where threads repeatedly attempt to acquire the lock without yielding.
    /// 
    /// This interface exposes additional state information commonly associated with
    /// spin locks, such as whether the lock is currently held and whether the calling
    /// thread owns the lock.
    /// </summary>
    /// <typeparam name="T">
    /// The underlying handle type used by the spin lock implementation.
    /// Typically <see cref="System.Threading.SpinLock"/>.
    /// </typeparam>
    public interface ISpinlock<T> : ILock <T> {

        /// <summary>
        /// Indicates whether the lock is currently held by any thread.
        /// This property reflects the internal state of the spin lock and is intended
        /// for diagnostic or monitoring purposes.
        /// </summary>
        bool IsHeld { get; }

        /// <summary>
        /// Indicates whether the lock is currently held by the calling thread.
        /// Implementations may require thread‑owner tracking to support this property.
        /// </summary>
        bool IsHeldbyCurrent { get; }

        /// <summary>
        /// Indicates whether thread‑owner tracking is enabled for the underlying
        /// spin lock. When enabled, the lock records which thread currently owns it,
        /// allowing additional safety checks at the cost of extra overhead.
        /// </summary>
        bool IsThreadOwnerTrackingEnabled { get; }
    }
}
