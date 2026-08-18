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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SystemEx.Collections.Generic {
    public struct TupleMap<T> : IEnumerable<Tuple<T>>, IEnumerable, ITraverse<Tuple<T>>, ICollection<Tuple<T>>
	 where T : notnull {

		private long m_growSize;
		private bool m_autoGrow;

		private Tuple<T>[] m_elements;
		private byte [] m_state;

		/// <summary>
		/// Max Elements where can Hold in this Map
		/// </summary>
		public long Size => Int64.MaxValue;

		/// <summary>
		/// Current number of valid elements stored in the Map.
		/// </summary>
		private long m_index;

		public ICollection<T> Keys {
			get {
				List<T> tmp = new List<T>();
				for ( int i = 0 ; i < Length ; i++ ) {
					if ( m_state[i] == 0 ) continue;
					else tmp.Add(m_elements[i].First);
				}
				return tmp;
			}
		}

		public ICollection<Object> Values {
			get {
				List<Object> tmp = new List<Object>();
				for ( int i = 0 ; i < Length ; i++ ) {
					if ( m_state[i] == 0 ) continue;

					tmp.Add(m_elements[i].Get(1) );
				}
				return tmp;
			}
		}

		public long Length => m_elements.LongLength;

		public Tuple<T> Front => m_elements[0];

		public Tuple<T> Back => m_elements[Count - 1];

		public long GrowSize {
			get => (m_autoGrow ? m_growSize : 0);
			set {
				m_growSize = value;
				m_autoGrow = (m_growSize > 0);
			}
		}

		public bool AutoGrow { get => (m_growSize == 0 ? false : m_autoGrow); set => m_autoGrow = value; }


		public long Count => m_index;

		public bool IsFull => (AutoGrow ? false : m_index >= Length);

		public bool IsEmpty => m_index == 0;

		public TupleMap ( long size) {
            m_elements = new Tuple<T>[size];
			m_state = new byte[size];
        }

		public TupleMap ( long size, int growSize = 16 ) {
			m_elements = new Tuple<T>[size];

			m_state = new byte[size];
			m_index = 0;


			GrowSize = growSize;
		}
		public TupleMap ( Tuple<T>[] e, int growSize = 16 ) {
			m_elements = new Tuple<T>[e.Length];
			m_state = new byte[e.Length];

			Buffer.LongCopy<Tuple<T>>(e, 0, m_elements, 0, e.Length);
			Buffer.LongFill<byte>(m_state, 0, 1, m_elements.Length);

			m_index = e.LongLength;

			GrowSize = growSize;
		}

		public TupleMap ( IEnumerable<Tuple<T>> e, int growSize = 16 ) {
			var arr = e.ToArray();

			m_elements = new Tuple<T>[arr.Length];

			m_state = new byte[m_elements.Length];

			Buffer.LongCopy<Tuple<T>>(arr, 0, m_elements, 0, arr.Length);
			Buffer.LongFill<byte>(m_state, 0, 1, m_elements.Length);

			m_index = arr.Length;

			GrowSize = growSize;
		}

		public TupleMap ( TupleMap<T> other ) {
			m_elements = new Tuple<T>[other.Length];
			m_state = new byte[other.Length];

			Buffer.LongCopy<Tuple<T>>(other.m_elements, 0, m_elements, 0, other.Length);
			Buffer.LongCopy<byte>(other.m_state, 0, m_state, 0, other.Length);

			m_index = other.m_index;
			GrowSize = other.GrowSize;
		}

		/// <summary>
		/// Gets the element at the current logical position (m_index).
		/// 
		/// This is primarily useful during manual iteration or when treating the
		/// vector as a stack-like structure. Accessing Current when the vector is
		/// empty or m_index is out of range is undefined.
		/// </summary>
		public Tuple<T>? Current => (m_index > 0 ? m_elements[m_index - 1] : null);

		public Tuple<T>? First {
			get {
				if ( m_elements.Length == 0 )
					throw new InvalidOperationException("Map is empty");

				long index = -1;
				for ( long i = 0 ; i < Count ; i++ ) {
					if ( m_state[i] == 1 ) {
						index = i; break;
					}
				}
				if ( index == -1 ) throw new Exception("No Elements in the list");
				return m_elements[index];
			}
		}

		public Tuple<T>? Last {
			get {
				if ( m_elements.Length == 0 )
					throw new InvalidOperationException("Map is empty");

				long index = -1;
				for ( long i = Count - 1 ; i >= 0 ; i++ ) {
					if ( m_state[i] == 1 ) {
						index = i; break;
					}
				}
				if ( index == -1 ) throw new Exception("No Elements in the list");
				return m_elements[index];
			}
		}
		int ICollection<Tuple<T>>.Count => (int)this.Count;

		public bool IsReadOnly => false;

		public Optional<Tuple<T>> this[T key] {
			get {
				return Get(key);
			}
			set {
				if ( value.IsNull ) return;

				if ( !Replace(key, value.Value!) ) {
					PushBack( value.Value! );
				}
			}
		}

		
		public bool PushBack ( Tuple<T> entry ) {
			if ( ContainsKey(entry.First) ) return false;

			if ( m_index >= Length ) {
				if ( AutoGrow ) Grow();
				return false;
			}

			m_elements[m_index] = entry;
			m_state[m_index] = 1;
			m_index++;
			return true;
		}

		public bool PushFront ( Tuple<T> entry ) {
			if ( ContainsKey(entry.First) ) return false;

			// If full, attempt to grow
			if ( m_index + 1 >= Length ) {
				if ( AutoGrow ) {
					if ( !Resize(Length + GrowSize) )
						return false;
				} else {
					return false;
				}
			}

			// Shift all valid elements one slot to the right
			// m_index is the last valid index, so we shift [0 .. m_index]
			for ( long i = m_index ; i >= 0 ; i-- ) {
				m_elements[i + 1] = m_elements[i];
				m_state[i + 1] = m_state[i];
			}

			// Insert new element at the front
			m_elements[0] = entry;
			m_state[0] = 1;

			// Increase logical count
			m_index++;

			return true;
		}

		public bool Insert ( long index, Tuple<T> entry ) {
			if ( index < 0 ) return false;
			if ( ContainsKey(entry.First) ) return false;

			// Grow wie im Indexer
			if ( index >= m_elements.Length || m_index >= m_elements.Length ) {
				if ( AutoGrow ) {
					if ( !Resize(m_elements.Length + GrowSize) )
						return false;
				} else {
					return false;
				}
			}

			// Speicher nach rechts verschieben
			for ( long i = m_index ; i > index ; i-- ) {
				m_elements[i] = m_elements[i - 1];
				m_state[i] = m_state[i - 1];
			}

			m_elements[index] = entry;
			m_state[index] = 1;

			if ( index >= m_index )
				m_index = index + 1;

			return true;
		}

		public IEnumerable<Tuple<T>> Find ( T Key ) {
			for ( long i = 0 ; i < m_elements.LongLength ; i++ ) {

				if ( m_elements[i].First.Equals(Key) && m_state[i] == 1 ) {
					yield return m_elements[i];

					
				}
			}
		}

		public bool InsertRange ( int start, IEnumerable<Tuple<T>> items ) {
			var _arr = items.ToArray();

			for ( long i = 0 ; i < _arr.Length ; i++ ) {
				if ( !Insert(start + i, _arr[i]) )
					return false;
			}
			return true;
		}

		public bool Insert ( long start, long end, Tuple<T> entry ) => Insert(start, entry);

		public bool InsertRange ( long start, Tuple<T>[] entrys ) {
			for ( long i = 0 ; i < entrys.Length ; i++ ) {
				if ( !Insert(start + i, entrys[i]) )
					return false;
			}
			return true;
		}

		public bool Replace ( long index, Tuple<T> entry ) {
			if ( index < 0 ) return false;

			if ( m_index >= m_elements.Length ) Grow();

			if ( index >= m_elements.Length ) {
				if ( AutoGrow ) {
					if ( !Resize(m_elements.Length + GrowSize) )
						return false;
				} else {
					return false;
				}
			}

			m_elements[index] = entry;
			m_state[index] = 1;

			if ( index >= m_index )
				m_index = index + 1;

			return true;
		}

		public bool Replace ( T key, Tuple<T> entry ) {
			bool _ret = false;
			for ( long i = 0 ; i < m_elements.LongLength ; i++ ) {

				if ( m_elements[i].First.Equals(key) ) {
					m_elements[i] = entry;
					m_elements[i].Set(0, key);
					_ret = true;
					break;
				}
			}
			return _ret;
		}

		public Optional<Tuple<T>> Get ( T key ) {
			Optional<Tuple<T>> _ret = Optional<Tuple<T>>.NONE;

			for ( long i = 0 ; i < m_elements.LongLength ; i++ ) {

				if ( m_elements[i].First.Equals(key) ) {

					_ret = new Optional<Tuple<T>>(m_elements[i]);
					break;
				}
			}
			return _ret;
		}

		public bool Replace ( long start, long end, Tuple<T> entry ) {
			if ( start < 0 || end < start ) return false;
			if ( m_index >= m_elements.Length ) Grow();

			for ( long i = start ; i <= end ; i++ ) {
				m_elements[i] = entry;
				m_state[i] = 1;
			}

			if ( end >= m_index )
				m_index = end + 1;

			return true;
		}


		public bool ReplaceRange ( long start, Tuple<T>[] entrys ) {
			for ( long i = 0 ; i < entrys.Length ; i++ ) {
				if ( !Replace(start + i, entrys[i]) )
					return false;
			}
			return true;
		}


		public bool Erase () {
			if ( IsEmpty ) return false;
			m_state[m_index - 1] = 0;
			m_index--;



			return true;
		}

		public bool Erase ( long index ) {
			if ( IsEmpty ) return false;
			if ( index >= Length ) return false;

			m_state[index] = 0;

			return true;
		}

		public bool Erase ( long start, long end ) {
			if ( start < 0 || end < start ) return false;
			if ( start >= Length ) return false;

			var _real_end = System.Math.Min(end, Length);

			for ( long i = start ; i < _real_end ; i++ ) {
				Erase(i);
			}


			return true;
		}

		public bool Erase ( Tuple<T> value ) {
			bool _ret = false;

			for ( long i = 0 ; i < Count ; i++ ) {

				if ( m_elements[i].Equals(value) ) {
					m_state[i] = 0;
					_ret = true;
				}
			}

			return _ret;
		}

		public void Swap ( long i, long j ) {
			if ( i < 0 || j < 0 ) return;
			if ( i >= m_index || j >= m_index ) return;

			Tuple<T> tmp = m_elements[i];
			m_elements[i] = m_elements[j];
			m_elements[j] = tmp;

			byte tmps = m_state[i];
			m_state[i] = m_state[j];
			m_state[j] = tmps;
		}


		public Tuple<T> ElementAt ( long index ) {
			if ( IsEmpty || index >= Length ) throw new ArgumentOutOfRangeException();
			if ( m_state[index] == 0 ) throw new Exception("No element on this position");
			return m_elements[index];
		}

		public bool Grow () {
			if ( !AutoGrow ) return false;
			return Resize(Length + GrowSize);
		}

		public void Clear () {
			m_index = 0;
			var len = Length;
			m_elements = new Tuple<T>[len];
			m_state = new byte[len];
		}

		public IEnumerator<Tuple<T>> GetEnumerator () {
			for ( int i = 0 ; i < m_index ; i++ )
				if ( m_state[i] == 1 )
					yield return m_elements[i];
		}

		public void Traverse ( TraversMode mode, long startIndex, long endIndex, Action<Tuple<T>> func ) {
			var start = System.Math.Max(startIndex, 0);
			var end = System.Math.Min(endIndex,  m_index);

			if ( mode == TraversMode.Forwards ) {
				for ( long i = start ; i < end ; i++ ) {
					if ( m_state[i] == 1 )
						func(m_elements[i]);
				}
			} else if ( mode == TraversMode.Backwards ) {
				for ( long i = end ; i >= start ; i-- ) {
					if ( m_state[i] == 1 )
						func(m_elements[i]);
				}
			}
		}

		public Pair<bool, long> CopyTo ( TupleMap<T> vector, ulong VectorIndex ) {
			return CopyTo(0, vector, 0, VectorIndex);
		}


		public Pair<bool, long> CopyTo ( uint sourceOffset, TupleMap<T> destination, ulong destinationOffset, ulong count ) {

			long src = (long)sourceOffset;
			long dst = (long)destinationOffset;

			if ( src > Length ) src = (long)Length;

			long toCopy = System.Math.Min((long)count,
			System.Math.Min(System.Math.Max(0, (long)Length - src),
					 System.Math.Max(0, destination.Length - dst)));

			if ( toCopy <= 0 ) return new Pair<bool, long>(false, 0);

			Buffer.LongCopy<Tuple<T>>(m_elements, src, destination.m_elements, dst, toCopy);
			Buffer.LongCopy<byte>(m_state, src, destination.m_state, dst, toCopy);

			long end = dst + toCopy;
			if ( end > destination.m_index )
				destination.m_index = end;

			return new Pair<bool, long>(true, toCopy);
		}



		public Pair<bool, long> CopyFrom ( TupleMap<T> source, ulong sourceOffset, ulong destinationOffset, ulong count ) {
			long src = (long)sourceOffset;
			long dst = (long)destinationOffset;

			if ( dst > Length )
				dst = Length;

			long toCopy = System.Math.Min((long)count,
				System.Math.Min(System.Math.Max(0, source.Length - src),
				System.Math.Max(0, Length - dst)));

			// Wenn nichts passt → prüfen ob wir wachsen müssen
			if ( toCopy <= 0 ) {
				if ( !AutoGrow )
					return new Pair<bool, long>(false, 0);

				long required = dst + (long)count;

				long newSize = Length;
				while ( required > newSize )
					newSize += GrowSize;

				if ( !Resize(newSize) )
					return new Pair<bool, long>(false, 0);

				// Nach Resize neu berechnen
				toCopy = System.Math.Min((long)count,
					System.Math.Min(System.Math.Max(0, source.Length - src),
						System.Math.Max(0, (long)Length - dst)));

				if ( toCopy <= 0 )
					return new Pair<bool, long>(false, 0);
			}

			Buffer.LongCopy<Tuple<T>>(source.m_elements, src, m_elements, dst, toCopy);
			Buffer.LongCopy<byte>(source.m_state, src, m_state, dst, toCopy);

			// m_index anpassen, falls wir weiter hinten geschrieben haben
			long end = dst + toCopy;
			if ( end > m_index )
				m_index = end;

			return new Pair<bool, long>(true, toCopy);
		}

		/// <summary>
		/// Returns a copy of the internal buffer.
		/// </summary>
		public Tuple<T>[] ToNative () {
			Tuple<T>[] vec  = new Tuple<T>[m_index];

			for ( long i = 0 ; i < m_index ; i++ ) {
				if ( m_state[i] == 1 )
					vec[i] = m_elements[i];
			}
			return vec;
		}


		private bool Resize ( long size ) {
			if ( size == Length ) return false;

			try {
				long oldLen = Length;
				Array.Resize(ref m_elements, (int)size);
				Array.Resize(ref m_state, (int)size);
				m_index = oldLen;

			} catch {
				return false;
			}
			return true;
		}

		public Type GetElementType () {
			return typeof(Tuple<T>);
		}

		public bool ContainsKey ( T Key ) {
			bool _ret = false;
			for ( long i = 0 ; i < Count ; i++ ) {

				if ( m_elements[i].EqualFirst(Key) && m_state[i] == 1 ) {
					_ret = true;
					break;
				}
			}
			return _ret;
		}

		/// <summary>
		/// Determines whether any element has the specified value.
		/// </summary>
		public bool ContainsValue<TU> ( TU value ) {
			
			for ( long i = 0 ; i < Count ; i++ ) {

				for ( int j = 0 ; j < m_elements[i].Count ; j++ ) {

					if ( m_elements[i].Get(j).Equals(value) && m_state[i] == 1 ) {
						return true;
					}
				}
			}
			return false;
		}

		public Optional<TU> Get<TU> ( T Key ) {

			Optional<TU> _ret = Optional<TU>.NONE;

			for ( long i = 0 ; i < Count ; i++ ) {
				//if ( p.IsNull ) continue;

				if ( m_elements[i].EqualFirst(Key) && m_state[i] == 1 ) {
					_ret = (TU)m_elements[i].Get(1);
					break;
				}
			}
			return _ret;
		}
		public void Add ( Tuple<T> item ) => PushBack(item);

		public void RemoveAt ( int pos ) => Erase(pos);
		public void RemoveAt ( int start, int iend ) => Erase(start, iend);


		public bool Remove ( Tuple<T> item ) => Erase(item);

		public bool Remove ( T key ) {
			bool _ret = false;

			for ( long i = 0 ; i < Count ; i++ ) {
				if ( m_state[i] == 1 && m_elements[i].EqualFirst(key) ) {
					m_state[i] = 0;
					_ret = true;
					break;
				}
			}
			return _ret;
		}

		public bool Contains ( Tuple<T> item ) {
			bool _ret = false;
			for ( long i = 0 ; i < Length ; i++ ) {

				if ( m_elements[i].Equals(item) && m_state[i] == 1 ) {
					_ret = true;
					break;
				}
			}
			return _ret;
		}




		public void CopyTo ( Tuple<T>[] array, int arrayIndex ) {
			m_elements.CopyTo(array, arrayIndex);
		}

		public bool TryGeValue<TU> ( T key, [MaybeNullWhen(false)] out TU? value ) {

			for ( long i = 0 ; i < Count ; i++ ) {
				//if ( p.IsNull ) continue;

				if ( m_elements[i].EqualFirst(key) && m_state[i] == 1 ) {
					var x = m_elements[i].Get(1);
					value = x.HasValue ? (TU)x.Value! : default(TU);
					return true;
				}
			}
			value = default!;
			return false;
		}



		public void Set<TU> ( T v, TU value ) {
			throw new NotImplementedException();
		}

		public void PushBack ( TupleMap<T> other ) {
			for ( long i = 0 ; i < other.Count ; i++ ) {
				if ( other.m_state[i] == 1 )
					this.PushBack(other.m_elements[i]);
			}
		}

		IEnumerator IEnumerable.GetEnumerator () {
			return GetEnumerator();
		}
	}
}
