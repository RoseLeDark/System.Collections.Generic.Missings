using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Threading {
	/// <summary>
	/// A bounded lock‑free multi‑producer, multi‑consumer (MPMC) queue.
	/// 
	/// Producers and consumers coordinate through atomic sequence checks
	/// and CAS operations on <c>m_head</c> and <c>m_tail</c>.
	/// </summary>
	/// <typeparam name="T">
	/// The type of items stored in the queue.
	/// </typeparam>
	public struct LightMMQueue<T> : ILockFreeQueue<T> {
		private long m_head;
		private long m_tail;

		private LightSaveQueueEntry<T>[] m_entrys;

		/// <summary>
		/// Determines whether the queue is empty in a lock-free, best-effort manner.
		/// The result is suitable for guarding dequeue operations.
		/// </summary>
		public bool IsEmpty {
			get {
				long _h = Volatile.Read(ref m_head);
				long _t = Volatile.Read(ref m_tail);

				return _t - _h <= 0;
			}
		}

		/// <summary>
		/// Gets the total capacity of the queue. The queue is bounded and
		/// cannot grow beyond this size.
		/// </summary>
		public long Capacity
		   => m_entrys.LongLength;

		public bool SupportMultiConsumer => true;

		public bool SupportMultiProducer => true;

		/// <summary>
		/// Determines whether the queue is full in a lock-free, best-effort manner.
		/// The result is suitable for guarding enqueue operations.
		/// </summary>
		public bool IsFull {
			get {
				long _h = Volatile.Read(ref m_head);
				long _t = Volatile.Read(ref m_tail);

				long _df = _t - _h;
				return _df >= m_entrys.LongLength;
			}
		}
		/// <summary>
		/// Gets an approximate count of items in the queue. This value is not
		/// strictly linearizable but provides a best‑effort estimate suitable
		/// for monitoring or non‑critical logic.
		/// </summary>
		public long Count {
			get {
				long _h = Volatile.Read(ref m_head);
				long _t = Volatile.Read(ref m_tail);

				long _df = _t - _h;
				if( _df >= m_entrys.LongLength ) 
					return Capacity - 1;

				if ( _h != _t ) {

					return _h < _t ? _t - _h : (long)m_entrys.LongLength - _h + _t;
				}
				return 0;
			}
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="LightMMQueue{T}"/>
		/// with the specified capacity. 
		/// </summary>
		/// <param name="capacity">The fixed size of the queue.</param>
		public LightMMQueue ( long capacity ) {
			m_entrys = new LightSaveQueueEntry<T>[capacity];
	
			for ( long i = 0 ; i < Capacity ; i++ ) {
				m_entrys[i].Number = i;
				m_entrys[i].Value = default;
			}
		}
		/// <summary>
		/// Clears the queue. When <paramref name="bFast"/> is true, the queue
		/// is reset by reinitializing all slot sequence numbers and values.
		/// Otherwise, items are dequeued until the queue becomes empty.
		/// </summary>
		public void Clear ( bool bFast ) {

			if ( bFast ) {
				for ( long i = 0 ; i < Capacity ; i++ ) {
					m_entrys[i].Number = i;
					m_entrys[i].Value = default;
				}
			} else {
				while ( Dequeue(out _) ) ;
			}
		}
		/// <summary>
		/// Attempts to enqueue an item into the queue. Multiple producers may
		/// safely call this method concurrently. The operation succeeds only
		/// if the target slot's sequence number indicates that it is ready
		/// for writing.
		/// </summary>
		/// <param name="item">The item to enqueue.</param>
		/// <returns>
		/// <c>true</c> if the item was successfully enqueued;
		/// <c>false</c> if the queue is full.
		/// </returns>
		public bool Enqueue ( T item ) {
			SpinWait _sp = default;
			bool _ret = false;

			while ( true ) {

				long _h = Volatile.Read(ref m_head);
				long _t = Volatile.Read(ref m_tail);

				if( (_t - _h) >= m_entrys.LongLength) {
					break;
				}

				long _seq = m_entrys[_t].Number;
				var _df = _seq - _t;

				if ( _df == 0 ) {
					if ( Interlocked.CompareExchange(ref m_tail, _t + 1, _t) == _t ) {
						m_entrys[_t].Value = item;
						m_entrys[_t].Number = _t + 1;
						_ret = true;
						break;
					}
				} else if ( _df < 0 ) {
					_ret = false;
					break;
				}

				_sp.SpinOnce();
			}
			return _ret;
		}
		public bool Dequeue ( out T? result ) {
			SpinWait _sp = default;
			bool _ret = false;
			result = default(T);


			while ( true ) {
				long _h = Volatile.Read(ref m_head);
				long _t = Volatile.Read(ref m_tail);

				if ( _t - _h <= 0) { 
					break;
				}

				long _seq = m_entrys[_h].Number;
				long _df = _seq - (_h + 1);

				if ( _df == 0 ) {
					if ( Interlocked.CompareExchange(ref m_head, _h + 1, _h) == _h ) {

						result = m_entrys[_h].Value;
						m_entrys[_h].Value = default;
						m_entrys[_h].Number = _h + m_entrys.LongLength;
						_ret = true;
						break;
					}
				} else if ( _df < 0 ) {
					_t = Volatile.Read(ref m_tail);
					if ( _t - _h <= 0 ) {

						_ret = false;
						break;
					}
				}

				_sp.SpinOnce();
			}
			return _ret;
		}

		/// <summary>
		/// Attempts to read the next item without removing it from the queue.
		/// This operation is lock-free and does not modify the queue state.
		/// </summary>
		/// <param name="result">
		/// Receives the next item if available; otherwise <c>null</c>.
		/// </param>
		/// <returns>
		/// <c>true</c> if an item is available to peek; otherwise <c>false</c>.
		/// </returns>
		public bool Peek ( out T? result ) {
			SpinWait _sp = default;
			bool _ret = false;
			result = default(T);

			while ( true ) {

				long _h = Volatile.Read(ref m_head);
				long _t = Volatile.Read(ref m_tail);

				if ( _t - _h <= 0 ) {
					_ret = false;
					break;
				}

				long _sq = m_entrys[_h].Number;
				long _df = _sq - (_h + 1);

				if ( _df == 0 ) {
					// Slot is ready to be consumed → safe to peek
					result = m_entrys[_h].Value;
					_ret = true;
					break;
				}

				_sp.SpinOnce();
			}

			return _ret;
		}

	}
}
