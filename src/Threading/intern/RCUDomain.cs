using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Threading.intern {

	/// <summary>
	/// Represents the writer-side coordination domain for a user‑mode RCU object.
	/// 
	/// <para>
	/// <see cref="RCUGlobalDomain{T}"/> manages two responsibilities:
	/// <list type="bullet">
	///   <item><description>
	///     Tracking active writer sections through a global <see cref="Epoch"/>.
	///   </description></item>
	///   <item><description>
	///     Buffering pending updates in a minimal two‑slot history structure
	///     (<see cref="RCUHistory{T}"/>), allowing readers to commit updates
	///     deterministically once writers have finished.
	///   </description></item>
	/// </list>
	/// </para>
	/// 
	/// <para>
	/// Writers push new values into the domain using <see cref="Push(T?)"/>.
	/// Each push operation enters a writer epoch, ensuring that readers do not
	/// commit updates while a writer is active. Updates are stored in timestamped
	/// form inside the history buffer, which retains at most two entries:
	/// the older pending update and the most recent one.
	/// </para>
	/// 
	/// <para>
	/// Readers retrieve updates using <see cref="Pop(T?)"/>.  
	/// If a writer is active, readers receive a stable fallback value and do not
	/// commit new updates.  
	/// Once all writers have finished (global epoch reaches zero), readers commit
	/// the newest pending update and the history is reduced accordingly.
	/// </para>
	/// 
	/// <para>
	/// This structure provides deterministic, low‑overhead RCU behavior suitable
	/// for user‑mode systems where update batching and stable fallback values are
	/// preferred over kernel‑style grace‑period mechanisms.
	/// </para>
	/// </summary>		
	internal sealed class RCUGlobalDomain<T> {
		private Epoch m_globalEpoch;
		private RCUHistory<T> m_history;

		/// <summary>
		/// Initializes a new <see cref="RCUGlobalDomain{T}"/> instance with an
		/// empty update history and a writer epoch counter set to zero.
		/// </summary>
		public RCUGlobalDomain () {
			m_globalEpoch = new Epoch();
			m_history = new RCUHistory<T>();
		}

		/// <summary>
		/// Pushes a new value into the RCU update history.
		/// 
		/// <para>
		/// The push operation enters a writer epoch via <see cref="UniqueEpoch"/>,
		/// ensuring that readers do not commit updates while the writer is active.
		/// The value is then forwarded to the underlying <see cref="RCUHistory{T}"/>
		/// instance, which stores it together with a timestamp and applies
		/// "latest‑only" semantics.
		/// </para>
		/// </summary>
		/// <param name="entry">The value to enqueue as a pending RCU update.</param>
		public void Push ( T? entry ) {
			using ( var _lock = new UniqueEpoch(ref m_globalEpoch) ) {
				m_history.Push(entry);
			}
		}

		/// <summary>
		/// Retrieves a pending update from the history, applying writer‑aware
		/// semantics to ensure stable behavior during concurrent update production.
		/// 
		/// <para>
		/// If a writer is active, the method returns the provided default value
		/// or, when two pending updates exist, the older pending update.  
		/// If no writer is active, the newest pending update is committed and
		/// returned.  
		/// If no pending updates exist, the default value is returned.
		/// </para>
		/// </summary>
		/// <param name="deft">
		/// The fallback value returned when committing a new update is not permitted.
		/// </param>
		/// <returns>
		/// The committed update or the fallback value, depending on the current
		/// writer state and history contents.
		/// </returns>
		public T? Pop ( T? deft ) {
			T? _newValue = default(T);

			return m_history.Pop(out _newValue, deft, m_globalEpoch.Value != 0) ? _newValue : deft;
		}
	}
}
