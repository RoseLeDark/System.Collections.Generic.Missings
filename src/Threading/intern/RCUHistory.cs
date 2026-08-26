using System;
using System.Collections.Generic;
using System.Text;
using SystemEx.Collections.Generic;

namespace SystemEx.Threading.intern {
	/// <summary>
	/// Maintains a minimal two‑slot history buffer for pending RCU updates.
	/// 
	/// <para>
	/// <see cref="RCUHistory{T}"/> stores at most two timestamped entries:
	/// <list type="bullet">
	///   <item><description><c>[0]</c> — the older pending update</description></item>
	///   <item><description><c>[1]</c> — the most recent pending update</description></item>
	/// </list>
	/// </para>
	/// 
	/// <para>
	/// Writers push new values into the history together with a timestamp.  
	/// If the history is not yet full, the new entry is appended.  
	/// Once both slots are occupied, only strictly newer updates (based on
	/// <see cref="TimeOnly"/>) replace the most recent entry; older or equal
	/// timestamps are ignored.  
	/// </para>
	/// 
	/// <para>
	/// Readers commit updates by calling <see cref="Pop"/>.  
	/// If one pending update exists, it is returned and the history becomes empty.  
	/// If two pending updates exist, the newest one is returned and the older one
	/// becomes the sole remaining entry.  
	/// If no pending updates exist, <see cref="Pop"/> returns <c>false</c>.
	/// </para>
	/// 
	/// <para>
	/// This structure is designed for user‑mode RCU implementations where only the
	/// most recent update matters and older updates can be safely discarded.
	/// It provides deterministic behavior with minimal memory overhead and without
	/// requiring dynamic allocation.
	/// </para>
	/// </summary>
	internal sealed class RCUHistory<T> {
		private Pair<TimeOnly, T?>[] m_entry;
		private byte m_index;

		/// <summary>
		/// Initializes a new <see cref="RCUHistory{T}"/> instance with an empty
		/// two‑slot history buffer.
		/// </summary>
		public RCUHistory () {
			m_entry = new Pair<TimeOnly, T?>[2];
			m_index = 0;
		}

		/// <summary>
		/// Pushes a new value into the history together with a timestamp.
		/// 
		/// <para>
		/// Behavior:
		/// <list type="bullet">
		///   <item><description>
		///     If the history is empty (<c>m_index == 0</c>), the entry becomes the
		///     first pending update.
		///   </description></item>
		///   <item><description>
		///     If one entry exists (<c>m_index == 1</c>), the new entry becomes the
		///     most recent pending update.
		///   </description></item>
		///   <item><description>
		///     If two entries exist (<c>m_index == 2</c>), the new entry replaces the
		///     most recent one only if its timestamp is strictly newer.  
		///     Otherwise, the update is ignored.
		///   </description></item>
		/// </list>
		/// </para>
		/// </summary>
		/// <param name="entry">The value to enqueue as a pending RCU update.</param>
		public void Push ( T? entry ) {
			var ts = TimeOnly.FromDateTime(DateTime.UtcNow);
	
			if ( m_index == 0 ) { m_entry[0] = new Pair<TimeOnly, T?>(ts, entry); m_index = 1; }
			if ( m_index == 1 ) { m_entry[1] = new Pair<TimeOnly, T?>(ts, entry); m_index = 2; }

			if ( m_index == 2 ) {
				if( m_entry[1].First < ts) {
					m_entry[0] = m_entry[1];
					m_entry[1] = new Pair<TimeOnly, T?>(ts, entry);
				}
			}
		}

		/// <summary>
		/// Retrieves a pending RCU update from the history, applying writer‑aware
		/// semantics to ensure stable and deterministic behavior during concurrent
		/// update production.
		/// 
		/// <para>
		/// The method always returns a value (either a pending update or the provided
		/// default), but the boolean return value indicates whether a pending update
		/// was actually committed.
		/// </para>
		/// 
		/// <para><strong>Writer‑aware behavior</strong></para>
		/// <list type="bullet">
		///   <item><description>
		///     If no pending updates exist (<c>m_index == 0</c>), the default value
		///     <paramref name="def"/> is returned and the method reports success.
		///   </description></item>
		///   <item><description>
		///     If exactly one pending update exists (<c>m_index == 1</c>) and a writer
		///     is active (<paramref name="haveWriter"/> is <c>true</c>), the pending
		///     update is <em>not</em> committed. Instead, the default value is returned.
		///   </description></item>
		///   <item><description>
		///     If two pending updates exist (<c>m_index == 2</c>) and a writer is active,
		///     the older update (slot <c>[0]</c>) is returned. This provides a stable
		///     pre‑update value while writers continue producing new updates.
		///   </description></item>
		/// </list>
		/// 
		/// <para><strong>Normal commit behavior (no active writer)</strong></para>
		/// <list type="bullet">
		///   <item><description>
		///     If one pending update exists (<c>m_index == 1</c>), it is committed and
		///     the history becomes empty.
		///   </description></item>
		///   <item><description>
		///     If two pending updates exist (<c>m_index == 2</c>), the newest update
		///     (slot <c>[1]</c>) is committed and the older update becomes the sole
		///     remaining entry.
		///   </description></item>
		/// </list>
		/// 
		/// <para>
		/// This method is designed for user‑mode RCU systems where readers may need
		/// to retrieve a stable value even while writers are still producing updates.
		/// It ensures that readers never commit new updates during active write phases,
		/// while still allowing deterministic commit behavior once writers have finished.
		/// </para>
		/// </summary>
		/// <param name="ret">
		/// The returned value, either a committed update, a fallback value, or
		/// <c>default</c> depending on the current history state.
		/// </param>
		/// <param name="def">
		/// The fallback value returned when a writer is active and committing a new
		/// update is not permitted.
		/// </param>
		/// <param name="haveWriter">
		/// Indicates whether a writer is currently active. When <c>true</c>, commit
		/// behavior is restricted as described above.
		/// </param>
		/// <returns>
		/// <c>true</c> if a pending update was committed; otherwise <c>false</c>.
		/// </returns>
		public bool Pop(out T? ret, T? def, bool haveWriter = false) {
			ret = default(T);
			bool _ret = false;

			if (m_index == 0) {
				ret = def;
				_ret = true;
			}
			if(m_index == 1) {

				if ( haveWriter ) {
					ret = def;
					_ret = true;
				} else {
					m_index = 0;
					ret = m_entry[0].Second;
					_ret = true;
				}
			}

			if( m_index == 2 ) {
				
				if( haveWriter ) {
					ret = m_entry[0].Second;
					_ret = true;
				} else {
					m_index = 1;
					ret = m_entry[1].Second;
					_ret = true;
				}
				
			}
			return _ret;
			
		}
	}
}
