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
	/// \addtogroup Collections
	/// @{

	/// <summary>
	/// A sparse, linear container that stores elements as <see cref="Tuple{T}"/> 
	/// objects, each consisting of a single key of type <typeparamref name="T"/> 
	/// and an arbitrary number of additional values.
	/// 
	/// <para>
	/// Unlike <see cref="Map{T, TU}"/>, which stores fixed two‑value pairs, 
	/// <see cref="TupleMap{T}"/> supports variable‑length, heterogeneous value 
	/// sequences per key. This makes it suitable for scenarios where each key 
	/// represents a record, row, or structured entry with multiple fields.
	/// </para>
	/// 
	/// <para>
	/// The container behaves like a sparse vector: inactive slots remain allocated 
	/// but are ignored during traversal, lookup, and enumeration. This design 
	/// enables efficient slicing, copying, and low‑level operations without 
	/// compacting or reallocating the underlying storage.
	/// </para>
	/// 
	/// <para>
	/// Automatic growth is controlled through <see cref="GrowSize"/> and 
	/// <see cref="AutoGrow"/>. When enabled, the internal buffer expands 
	/// automatically to accommodate additional tuples.
	/// </para>
	/// </summary>
	public class TupleMap<T> : IEnumerable<Tuple<T>>, IEnumerable, ITraverse<Tuple<T>>, ICollection<Tuple<T>>
	 where T : notnull {
		/// <summary>
		/// Stores the configured growth size used when automatic expansion is enabled.
		/// </summary>
		private long m_growSize;

		/// <summary>
		/// Indicates whether automatic growth is currently enabled.
		/// </summary>
		private bool m_autoGrow;
		/// <summary>
		/// Internal storage buffer containing all tuples
		/// </summary>
		private Tuple<T>[] m_elements;
		/// <summary>
		/// State buffer marking active (1) and inactive (0) entries.
		/// </summary>
		private byte [] m_state;

		/// <summary>
		/// Gets the theoretical maximum logical size of the tuple.
		/// </summary>
		public long Size => Int64.MaxValue;

		/// <summary>
		/// Logical number of active elements stored in the tuple.
		/// </summary>
		private long m_index;

		/// <summary>
		/// Gets a collection containing all active keys in the tuple.
		/// </summary>
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
		/// <summary>
		/// Gets a collection containing all active Values in the tuple.
		/// </summary>
		public ICollection< ICollection<Object> > Values {
			get {
				List< ICollection<Object>  > tmp = new List<ICollection<Object>>();

				for ( int i = 0 ; i < Length ; i++ ) {
					if ( m_state[i] == 0 ) continue;

					ICollection<Object>  elements = new List<object>();
					for(int j = 1 ; j < m_elements[i].Count - 1 ; i++ ) {
						elements.Add(m_elements[i].Get(j));
					}
					tmp.Add(elements);
				}
				return tmp;
			}
		}

		/// <summary>
		/// Gets the number of allocated slots in the internal buffer.
		/// </summary>
		public long Length => m_elements.LongLength;
		/// <summary>
		/// Gets the first active element in the tuplemap.
		/// </summary>
		public Tuple<T> Front => m_elements[0];
		/// <summary>
		/// Gets the last active element in the tuplemap.
		/// </summary>
		public Tuple<T> Back => m_elements[Count - 1];
		/// <summary>
		/// Gets or sets the automatic growth size. When positive, the map grows
		/// by this amount whenever additional capacity is required.
		/// </summary>
		public long GrowSize {
			get => (m_autoGrow ? m_growSize : 0);
			set {
				m_growSize = value;
				m_autoGrow = (m_growSize > 0);
			}
		}
		/// <summary>
		/// Gets or sets a value indicating whether the map should automatically
		/// grow when full.
		/// </summary>
		public bool AutoGrow { get => (m_growSize == 0 ? false : m_autoGrow); set => m_autoGrow = value; }

		/// <summary>
		/// Gets the number of active elements stored in the tuplemap.
		/// </summary>
		public long Count => m_index;
		/// <summary>
		/// Gets a value indicating whether the map is full.
		/// </summary>
		public bool IsFull => (AutoGrow ? false : m_index >= Length);
		/// <summary>
		/// Gets a value indicating whether the map contains no active elements.
		/// </summary>
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
		/// <summary>
		/// Gets the first active element in the tuplemap.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// Thrown when the map contains no active elements.
		/// </exception>
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
		/// <summary>
		/// Gets the last active element in the tuplemap.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// Thrown when the map contains no active elements.
		/// </exception>
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
		/// <summary>
		/// Gets the number of active elements stored in the map (ICollection implementation).
		/// </summary>
		int ICollection<Tuple<T>>.Count => (int)this.Count;
		/// <summary>
		/// Always false; the map supports modification.
		/// </summary>
		public bool IsReadOnly => false;
		/// <summary>
		/// Gets or sets the value associated with the specified key.
		/// If the key does not exist, a new entry is appended.
		/// </summary>
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

		/// <summary>
		/// Appends a key–value pair to the end of the tuplemap.
		/// </summary>
		public bool PushBack ( Tuple<T> entry ) {
			if ( ContainsKey(entry.First) ) return false;

			if ( m_index >= Length ) {
				if ( !AutoGrow ) return false;
				if ( !Grow() ) return false;
			}

			m_elements[m_index] = entry;
			m_state[m_index] = 1;
			m_index++;
			return true;
		}
		/// <summary>
		/// Inserts a key–value pair at the front of the tuplemap.
		/// </summary>
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
		/// <summary>
		/// Inserts an element at the specified index, shifting elements to the right.
		/// </summary>
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
		/// <summary>
		/// Finds all entries matching the specified key.
		/// </summary>
		public IEnumerable<Tuple<T>> Find ( T Key ) {
			for ( long i = 0 ; i < m_elements.LongLength ; i++ ) {

				if ( m_elements[i].First.Equals(Key) && m_state[i] == 1 ) {
					yield return m_elements[i];

					
				}
			}
		}
		/// <summary>
		/// Inserts a range of elements starting at the specified index.
		/// </summary>
		public bool InsertRange ( int start, IEnumerable<Tuple<T>> items ) {
			var _arr = items.ToArray();

			for ( long i = 0 ; i < _arr.Length ; i++ ) {
				if ( !Insert(start + i, _arr[i]) )
					return false;
			}
			return true;
		}
		/// <summary>
		/// Inserts an element at the specified index, shifting elements to the right.
		/// </summary>
		public bool Insert ( long start, long end, Tuple<T> entry ) => Insert(start, entry);

		/// <summary>
		/// Inserts a range of elements starting at the specified index.
		/// </summary>
		public bool InsertRange ( long start, Tuple<T>[] entrys ) {
			for ( long i = 0 ; i < entrys.Length ; i++ ) {
				if ( !Insert(start + i, entrys[i]) )
					return false;
			}
			return true;
		}
		
		/// <summary>
		/// Replaces the element at the specified index.
		/// </summary>
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
		/// <summary>
		/// Replaces the value associated with the specified key.
		/// </summary>
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
		/// <summary>
		/// Replaces a range of elements with the specified entry.
		/// </summary>
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

		/// <summary>
		/// Replaces a range of elements with the specified entries.
		/// </summary>
		public bool ReplaceRange ( long start, Tuple<T>[] entrys ) {
			for ( long i = 0 ; i < entrys.Length ; i++ ) {
				if ( !Replace(start + i, entrys[i]) )
					return false;
			}
			return true;
		}

		/// <summary>
		/// Removes the last active element.
		/// </summary>
		public bool Erase () {
			if ( IsEmpty ) return false;
			m_state[m_index - 1] = 0;
			m_index--;



			return true;
		}
		/// <summary>
		/// Removes the element at the specified index.
		/// </summary>
		public bool Erase ( long index ) {
			if ( IsEmpty ) return false;
			if ( index >= Length ) return false;

			m_state[index] = 0;

			return true;
		}
		/// <summary>
		/// Removes all elements in the specified range.
		/// </summary>
		public bool Erase ( long start, long end ) {
			if ( start < 0 || end < start ) return false;
			if ( start >= Length ) return false;

			var _real_end = System.Math.Min(end, Length);

			for ( long i = start ; i < _real_end ; i++ ) {
				Erase(i);
			}


			return true;
		}
		/// <summary>
		/// Removes the specified  tuple.
		/// </summary>
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
		/// <summary>
		/// Swaps two elements in the map, including their active/inactive state.
		/// </summary>
		/// <param name="i">Index of the first element.</param>
		/// <param name="j">Index of the second element.</param>
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

		/// <summary>
		/// Returns the element at the specified index.
		/// </summary>
		/// <param name="index">The index to retrieve.</param>
		/// <returns>The element at the given index.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when the index is outside the allocated range.
		/// </exception>
		/// <exception cref="Exception">
		/// Thrown when the slot is inactive.
		/// </exception>
		public Tuple<T> ElementAt ( long index ) {
			if ( IsEmpty || index >= Length ) throw new ArgumentOutOfRangeException();
			if ( m_state[index] == 0 ) throw new Exception("No element on this position");
			return m_elements[index];
		}
		/// <summary>
		/// Grows the internal buffer by <see cref="GrowSize"/> if automatic growth is enabled.
		/// </summary>
		/// <returns>True if the buffer was successfully resized; otherwise false.</returns>
		public bool Grow () {
			if ( !AutoGrow ) return false;
			return Resize(Length + GrowSize);
		}
		/// <summary>
		/// Clears the map by resetting the logical index and reinitializing the buffers.
		/// </summary>
		public void Clear () {
			m_index = 0;
			var len = Length;
			m_elements = new Tuple<T>[len];
			m_state = new byte[len];
		}
		/// <summary>
		/// Enumerates all active elements in the tuplemap.
		/// </summary>
		public IEnumerator<Tuple<T>> GetEnumerator () {
			for ( int i = 0 ; i < m_index ; i++ )
				if ( m_state[i] == 1 )
					yield return m_elements[i];
		}
		/// <summary>
		/// Traverses a range of elements using the specified traversal mode.
		/// </summary>
		/// <param name="mode">Traversal direction (forwards or backwards).</param>
		/// <param name="startIndex">Start index of traversal.</param>
		/// <param name="endIndex">End index of traversal.</param>
		/// <param name="func">Callback invoked for each active element.</param>
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
		/// <summary>
		/// Copies a range of elements to another map starting at the specified index.
		/// </summary>
		public Pair<bool, long> CopyTo ( TupleMap<T> vector, ulong VectorIndex ) {
			return CopyTo(0, vector, 0, VectorIndex);
		}

		/// <summary>
		/// Copies a range of elements to another tuplemap.
		/// </summary>
		/// <param name="sourceOffset">Offset in the source tuplemap.</param>
		/// <param name="destination">Destination tuplemap.</param>
		/// <param name="destinationOffset">Offset in the destination tuplemap.</param>
		/// <param name="count">Number of elements to copy.</param>
		/// <returns>A pair indicating success and number of copied elements.</returns>
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


		/// <summary>
		/// Copies a range of elements from another map into this tuplemap.
		/// Automatically grows the buffer if required.
		/// </summary>
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

		/// <summary>
		/// Resizes the internal buffers to the specified size.
		/// </summary>
		/// <param name="size">New buffer size.</param>
		/// <returns>True if resizing succeeded; otherwise false.</returns>
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
		/// <summary>
		/// Gets the element type stored in the tuplemap.
		/// </summary>
		public Type GetElementType () {
			return typeof(Tuple<T>);
		}
		/// <summary>
		/// Determines whether any element has the specified key.
		/// </summary>
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
		/// <summary>
		/// Retrieves the value associated with the specified key.
		/// </summary>
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

		/// <summary>
		/// Adds an element to the tuplemap.
		/// </summary>
		public void Add ( Tuple<T> item ) => PushBack(item);
		/// <summary>
		/// Removes the element at the specified index.
		/// </summary>
		public void RemoveAt ( int pos ) => Erase(pos);

		/// <summary>
		/// Removes all elements in the specified range.
		/// </summary>
		public void RemoveAt ( int start, int iend ) => Erase(start, iend);

		/// <summary>
		/// Removes the specified key–value pair.
		/// </summary>
		public bool Remove ( Tuple<T> item ) => Erase(item);
		/// <summary>
		/// Removes the element with the specified key.
		/// </summary>
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
		/// <summary>
		/// Determines whether the specified item exists.
		/// </summary>
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



		/// <summary>
		/// Copies the internal buffer to an external array.
		/// </summary>
		public void CopyTo ( Tuple<T>[] array, int arrayIndex ) {
			m_elements.CopyTo(array, arrayIndex);
		}
		/// <summary>
		/// Attempts to retrieve a value by key.
		/// </summary>
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

		/// <summary>
		/// Appends all active elements from another tuplemap.
		/// </summary>
		public void PushBack ( TupleMap<T> other ) {
			for ( long i = 0 ; i < other.Count ; i++ ) {
				if ( other.m_state[i] == 1 )
					this.PushBack(other.m_elements[i]);
			}
		}
		/// <summary>
		/// Enumerates all active elements.
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator () {
			return GetEnumerator();
		}
	}
}
