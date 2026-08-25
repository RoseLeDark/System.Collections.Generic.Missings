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

using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace SystemEx.Collections.Generic {
	/// \addtogroup Collections
	/// @{

	/// <summary>
	/// Represents a sparse, linear key–value container.
	/// 
	/// <para>
	/// The container behaves similarly to a sparse vector: inactive slots remain
	/// allocated but are ignored during enumeration and lookup. This design
	/// enables efficient copying, slicing, and low‑level operations without
	/// reallocating or compacting the underlying storage.
	/// </para>
	/// 
	/// <para>
	/// Automatic growth is controlled through <see cref="GrowSize"/> and
	/// <see cref="AutoGrow"/>. When enabled, the internal buffer expands
	/// automatically to accommodate new elements.
	/// </para>
	/// </summary>
	/// <typeparam name="T">Key type. Must be non‑nullable.</typeparam>
	/// <typeparam name="TU">Value type.</typeparam>
	public class Map<T, TU> : IEnumerable< Pair<T, TU> >, IEnumerable, ITraverse<Pair<T, TU>>, ICollection<Pair<T, TU>>
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
		/// Internal storage buffer containing all key–value pairs.
		/// </summary>
		private Pair<T, TU>[] m_elements;

		/// <summary>
		/// State buffer marking active (1) and inactive (0) entries.
		/// </summary>
		private byte[] m_state;

		/// <summary>
		/// Logical number of active elements stored in the map.
		/// </summary>
		private long m_index;

		/// <summary>
		/// Gets the theoretical maximum logical size of the map.
		/// </summary>
		public long Size => Int64.MaxValue;

		/// <summary>
		/// Gets a collection containing all active values in the map.
		/// </summary>
		public ICollection<TU> Values {
            get {
                List<TU> tmp = new List<TU>();
                for(int i = 0 ; i < Length ; i++) {
                    if ( m_state[i] == 0 ) continue;

                    tmp.Add(m_elements[i].Second);
                }
                return tmp;
            }
        }
		/// <summary>
		/// Gets a collection containing all active keys in the map.
		/// </summary>
		public ICollection<T> Keys {
            get {
                List<T> tmp = new List<T>();
                for(int i = 0 ; i < Length ; i++) {
                    if ( m_state[i] == 0 ) continue;
                    else tmp.Add(m_elements[i].First);
                }
                return tmp;
            }
        }

		/// <summary>
		/// Gets the number of allocated slots in the internal buffer.
		/// </summary>
		public long Length => m_elements.LongLength;

		/// <summary>
		/// Gets the first active element in the map.
		/// </summary>
		public Pair<T, TU> Front => m_elements[0];

		/// <summary>
		/// Gets the last active element in the map.
		/// </summary>
		public Pair<T, TU> Back => m_elements[Count - 1];

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
		/// Gets the number of active elements stored in the map.
		/// </summary>
		public long Count => m_index;
		/// <summary>
		/// Gets a value indicating whether the map is full and cannot grow.
		/// </summary>
		public bool IsFull => (AutoGrow ? false : m_index >= Length);
		/// <summary>
		/// Gets a value indicating whether the map contains no active elements.
		/// </summary>
		public bool IsEmpty => m_index == 0;

        /// <summary>
        /// Gets the element at the current logical position (m_index).
        /// 
        /// This is primarily useful during manual iteration or when treating the
        /// vector as a stack-like structure. Accessing Current when the vector is
        /// empty or m_index is out of range is undefined.
        /// </summary>
        public Pair<T, TU>? Current => (m_index > 0 ? m_elements[m_index - 1] : null);

		/// <summary>
		/// Gets the first active element in the map.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// Thrown when the map contains no active elements.
		/// </exception>
		public Pair<T, TU>? First {
            get {
                if ( m_elements.Length == 0 )
                    throw new InvalidOperationException("Map is empty");

                long index = -1;
                for(long i = 0 ; i < Count ; i++  ) { 
                    if(m_state[i] == 1 ) {
                        index = i; break;
                    }
                }
                if ( index == -1 ) throw new Exception("No Elements in the list");
                return m_elements[index];
            }
        }
		/// <summary>
		/// Gets the last active element in the map.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// Thrown when the map contains no active elements.
		/// </exception>
		public Pair<T, TU>? Last {
            get {
                if ( m_elements.Length == 0 )
                    throw new InvalidOperationException("Map is empty");

                long index = -1;
                for (long i = Count - 1 ; i >= 0 ; i ++ ) {
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
		int ICollection<Pair<T, TU>>.Count => (int)this.Count;
		/// <summary>
		/// Always false; the map supports modification.
		/// </summary>
		public bool IsReadOnly => false;

		/// <summary>
		/// Gets or sets the value associated with the specified key.
		/// If the key does not exist, a new entry is appended.
		/// </summary>
		public Optional<TU> this[T key] {
            get {
                return Get(key);
            } 
            set {
                if ( value.IsNull ) return;

                if ( !Replace(key, value) ) {
                    PushBack(new Pair<T, TU>(key, value.Value!));
                }
            }
        }
        /// <summary>
        /// Create a empty map with 8 entrys
        /// </summary>
		public Map () {
			m_elements = new Pair<T, TU>[8];

			m_state = new byte[8];
			m_index = 0;


			GrowSize = 8;
		}
		/// <summary>
		/// Initializes a new map with the specified initial capacity.
		/// </summary>
		public Map ( long size, int growSize = 16 ) {
            m_elements = new Pair<T, TU>[size];

            m_state = new byte[size];
            m_index = 0;


            GrowSize = growSize;
        }
		/// <summary>
		/// Initializes a new map by copying an existing array of pairs.
		/// </summary>
		public Map ( Pair<T, TU>[] e, int growSize = 16 ) {
            m_elements = new Pair<T, TU>[e.Length];
            m_state = new byte[e.Length];

            Buffer.LongCopy<Pair<T, TU>>(e, 0, m_elements, 0, e.Length);
			Buffer.LongFill<byte>(m_state, 0, 1, m_elements.Length);
			m_index = e.LongLength;

            GrowSize = growSize;
        }
		/// <summary>
		/// Initializes a new map from an enumerable sequence of pairs.
		/// </summary>
		public Map ( IEnumerable<Pair<T, TU>> e, int growSize = 16 ) {
            var arr = e.ToArray();

            m_elements = new Pair<T, TU>[arr.Length];

            m_state = new byte[m_elements.Length];

            Buffer.LongCopy<Pair<T, TU>>(arr, 0, m_elements, 0, arr.Length);
            Buffer.LongFill<byte>(m_state, 0,1, m_elements.Length);

            m_index = arr.Length;

            GrowSize = growSize;
        }

		/// <summary>
		/// Initializes a new map by copying another map instance.
		/// </summary>
		public Map ( Map<T, TU> other ) {
            m_elements = new Pair<T, TU>[other.Length];
            m_state = new byte[other.Length]; 

            Buffer.LongCopy<Pair<T, TU>>(other.m_elements, 0, m_elements, 0, other.Length);
            Buffer.LongCopy<byte>(other.m_state, 0, m_state, 0, other.Length);

            m_index = other.m_index;
            GrowSize = other.GrowSize;
        }
		/// <summary>
		/// Creates a logical segment view over the map using the same underlying buffer.
		/// </summary>
		public Map<T, TU> AsSegment ( long start, long length ) {
            if ( start < 0 || length < 0 )
                throw new ArgumentOutOfRangeException();

            // Ensure the segment lies fully inside the internal buffer
            ArgumentOutOfRangeException.ThrowIfLessThan((start + length), m_elements.Length);

            // Create a new Map<T, TU> that shares the same buffer
            // A segment must not grow, otherwise it would corrupt the shared buffer
            var seg = new Map<T, TU>(m_elements, 0);
            // Logical end of the segment
            seg.m_index = (int)(start + length);

            return seg;
        }

		/// <summary>
		/// Appends a key–value pair to the end of the map.
		/// </summary>
		public bool PushBack ( T key, TU value ) {
            return PushBack(new Pair<T, TU>(key, value));
        }

		/// <summary>
		/// Appends a key–value pair to the end of the map.
		/// </summary>
		public bool PushBack ( Pair<T,TU> entry ) {
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
		/// <summary>
		/// Inserts a key–value pair at the front of the map.
		/// </summary>
		public bool PushFront ( T key, TU value ) {
            return PushFront(new Pair<T, TU>(key, value));
        }

		/// <summary>
		/// Inserts a key–value pair at the front of the map.
		/// </summary>
		public bool PushFront ( Pair<T, TU> entry ) {
            if( ContainsKey(entry.First) ) return false;

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
		public bool Insert ( long index, Pair<T, TU> entry ) {
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
		public IEnumerable<Pair<T, TU>> Find ( T Key ) {
            for(long i = 0 ; i < m_elements.LongLength ; i++) {
                
                if(m_elements[i].First.Equals(Key) && m_state[i] == 1 ) {
                    yield return m_elements[i];
                }
            }
        }
		/// <summary>
		/// Inserts a range of elements starting at the specified index.
		/// </summary>
		public bool InsertRange ( int start, IEnumerable<Pair<T, TU>> items ) {
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
		public bool Insert ( long start, long end, Pair<T, TU> entry ) => Insert(start, entry);

		/// <summary>
		/// Inserts a range of elements starting at the specified index.
		/// </summary>
		public bool InsertRange ( long start, Pair<T, TU>[] entrys ) {
            for ( long i = 0 ; i < entrys.Length ; i++ ) {
                if ( !Insert(start + i, entrys[i]) )
                    return false;
            }
            return true;
        }

		/// <summary>
		/// Replaces the value associated with the specified key.
		/// </summary>
		public bool Replace ( T key, Optional<TU> value ) {
            if ( value.IsNull ) return false;
            bool _ret = false;

            if ( ContainsKey(key) ) {
                for ( long i = 0 ; i < Count ; i++ ) {
                    if ( m_elements[i].EqualFirst(key) ) {
                        m_elements[i].Second = value.Value!;
                        m_state[i] = 1;
                        _ret = true;
                        break;
                    }
                }
            }
            return _ret;
        }
		/// <summary>
		/// Replaces the element at the specified index.
		/// </summary>
		public bool Replace ( long index, Pair<T, TU> entry ) {
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
		/// Replaces a range of elements with the specified entry.
		/// </summary>
		public bool Replace ( long start, long end, Pair<T, TU> entry ) {
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
		public bool ReplaceRange ( long start, Pair<T, TU>[] entrys ) {
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

            for(long i = start ; i < _real_end ; i++ ) {
                Erase(i);
            }


            return true;
        }
		/// <summary>
		/// Removes the specified key–value pair.
		/// </summary>
		public bool Erase ( Pair<T, TU> value ) {
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

            Pair<T, TU> tmp = m_elements[i];
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
		public Pair<T, TU> ElementAt ( long index ) {
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
            m_elements = new Pair<T, TU>[len];
            m_state = new byte[len];
        }
		/// <summary>
		/// Enumerates all active elements in the map.
		/// </summary>
		public IEnumerator<Pair<T, TU>> GetEnumerator () {
            for ( int i = 0 ; i < m_index ; i++ )
                if(m_state[i] == 1 )
                    yield return m_elements[i];
        }
		/// <summary>
		/// Traverses a range of elements using the specified traversal mode.
		/// </summary>
		/// <param name="mode">Traversal direction (forwards or backwards).</param>
		/// <param name="startIndex">Start index of traversal.</param>
		/// <param name="endIndex">End index of traversal.</param>
		/// <param name="func">Callback invoked for each active element.</param>
		public void Traverse ( TraversMode mode, long startIndex, long endIndex, Action<Pair<T, TU>> func ) {
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
		public Pair<bool, long> CopyTo ( Map<T, TU> vector, ulong VectorIndex ) {
            return CopyTo(0, vector, 0, VectorIndex);
        }

		/// <summary>
		/// Copies a range of elements to another map.
		/// </summary>
		/// <param name="sourceOffset">Offset in the source map.</param>
		/// <param name="destination">Destination map.</param>
		/// <param name="destinationOffset">Offset in the destination map.</param>
		/// <param name="count">Number of elements to copy.</param>
		/// <returns>A pair indicating success and number of copied elements.</returns>
		public Pair<bool, long> CopyTo ( uint sourceOffset, Map<T, TU> destination, ulong destinationOffset, ulong count ) {

            long src = (long)sourceOffset;
            long dst = (long)destinationOffset;

            if ( src > Length ) src = (long)Length;

            long toCopy = System.Math.Min((long)count,
            System.Math.Min(System.Math.Max(0, (long)Length - src),
                     System.Math.Max(0, destination.Length - dst)));

            if ( toCopy <= 0 ) return new Pair<bool, long>(false, 0);

            Buffer.LongCopy<Pair<T, TU>>(m_elements, src, destination.m_elements, dst, toCopy);
            Buffer.LongCopy<byte>(m_state, src, destination.m_state, dst, toCopy);

            long end = dst + toCopy;
            if ( end > destination.m_index )
                destination.m_index = end;

            return new Pair<bool, long>(true, toCopy);
        }


		/// <summary>
		/// Copies a range of elements from another map into this map.
		/// Automatically grows the buffer if required.
		/// </summary>
		public Pair<bool, long> CopyFrom ( Map<T, TU> source, ulong sourceOffset, ulong destinationOffset, ulong count ) {
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

            Buffer.LongCopy<Pair<T, TU>>(source.m_elements, src, m_elements, dst, toCopy);
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
        public Pair<T, TU>[] ToNative () {
            Pair<T, TU>[] vec  = new Pair<T, TU>[m_index];

            for(long i = 0 ; i < m_index ; i++  ) {
                if(m_state[i] == 1 )
                    vec[i] = new Pair<T, TU>(m_elements[i].First, m_elements[i].Second);
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
		/// Gets the element type stored in the map.
		/// </summary>
		public Type GetElementType () {
            return typeof(Pair<T, TU>);
        }
		/// <summary>
		/// Determines whether any element has the specified key.
		/// </summary>
		public bool ContainsKey ( T Key ) {
            bool _ret = false;
            for ( long i = 0 ; i < Count ; i++ ) {
               
                if(m_elements[i].EqualFirst(Key) && m_state[i] == 1 ) {
                    _ret = true;
                    break;
                }
            }
            return _ret;
        }

        /// <summary>
        /// Determines whether any element has the specified value.
        /// </summary>
        public bool ContainsValue ( TU value ) {
            bool _ret = false;
            for ( long i = 0 ; i < Count ; i++ ) {
                
                if ( m_elements[i].EqualSecond(value) && m_state[i] == 1 ) {
                    _ret = true;
                    break;
                }
            }
            return _ret;
        }
		/// <summary>
		/// Retrieves the value associated with the specified key.
		/// </summary>
		public Optional<TU> Get ( T Key ) {

             Optional<TU> _ret = Optional<TU>.NONE;

            for ( long i = 0 ; i < Count ; i++ ) {
                //if ( p.IsNull ) continue;

                if ( m_elements[i].EqualFirst(Key) && m_state[i] == 1 ) {
                    _ret = m_elements[i].Second;
                    break;
                }
            }
            return _ret;
        }

		/// <summary>
		/// Adds an element to the map.
		/// </summary>
		public void Add ( Pair<T, TU> item ) => PushBack(item);
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
		public bool Remove (  Pair<T, TU>  item ) => Erase(item);
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
		/// Determines whether the specified key–value pair exists.
		/// </summary>
		public bool Contains ( Pair<T, TU> item ) {
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
		public void CopyTo ( Pair<T, TU>[] array, int arrayIndex ) {
            m_elements.CopyTo(array, arrayIndex);
        }
		/// <summary>
		/// Attempts to retrieve a value by key.
		/// </summary>
		public bool TryGeValue ( T key, [MaybeNullWhen(false)] out TU value ) {

            for(long i = 0 ; i < Count ; i++) {
                //if ( p.IsNull ) continue;

                if ( m_elements[i].EqualFirst(key) && m_state[i] == 1 ) {
                    value = m_elements[i].Second;
                    return true;
                }
            }
            value = default!;
            return false;
        }
		/// <summary>
		/// Attempts to retrieve a value by key.
		/// </summary>
		public bool TryGetValue ( T key, out TU? value ) {
            bool _ret = false;
            Optional<TU> _get = Get(key);

            if ( _get.IsSome ) {
                value = _get.Value!;
                _ret = true;
            } else {
                value = default(TU);
            }
            return _ret;
        }


		/// <summary>
		/// Sets a value for the specified key (not implemented).
		/// </summary>
		public void Set ( T v, TU value ) {
            throw new NotImplementedException();
        }
		/// <summary>
		/// Appends all active elements from another map.
		/// </summary>
		public void PushBack ( Map<T, TU> other ) {
            for ( long i = 0 ; i < other.Count ; i++ ) {
                if ( other.m_state[i] == 1)
                    this.PushBack(other.m_elements[i]);
            }
        }
		/// <summary>
		/// Retrieves a value or returns a fallback if the key does not exist.
		/// </summary>
		public TU GetOrDefault ( T key, TU v2 ) {
            Optional<TU> _get = Get(key);

            return (_get.IsSome ? _get.Value! : v2);
        }
		/// <summary>
		/// Enumerates all active elements.
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator () {
            return GetEnumerator();
        }
#if REPLAYE

		/// <summary>
        /// Creates a FlexSpan view over the entire map starting at index 0.
        /// 
        /// The view uses the specified indexing mode (System, Reverse, Ring) and
        /// provides a span-like interface backed directly by this vector.
		/// Gets the first active element in the map.
		/// </summary>
        /// <param name="map">
        /// Reference to the map. Passed by ref to avoid copying the struct and
        /// to ensure the FlexSpan reflects the actual container.
        /// </param>
        /// <param name="mode">
        /// Indexing mode for the view:
        /// System  = forward indexing,
        /// Reverse = backward indexing,
        /// Ring    = circular wrap-around indexing.
        /// </param>
        /// <returns>
        /// A FlexSpan representing the full vector.
        /// </returns>
        public static ContainerFlexSpan<Pair<T, TU>, Map<T, TU>> AsFlexSpan ( ref Map<T, TU> map, FlexSpanMode mode = FlexSpanMode.System )
            => new ContainerFlexSpan<Pair<T, TU>, Map<T, TU>>(ref map, 0, mode);


        public static Slices<Pair<T, TU>,  Map<T, TU> > AsMultiSlices ( ref Map<T, TU> map, int devider ) {
            return new Slices<Pair<T, TU>, Map<T, TU> >(ref map, (int)(map.Count / devider));
        }

        public static ContainerFlexSpan<Pair<T, TU>, Map<T, TU>> AsFlexSpan ( ref Map<T, TU> map, long start, long end, FlexSpanMode mode = FlexSpanMode.System )
            => new ContainerFlexSpan<Pair<T, TU>, Map<T, TU>>(ref map, start, end, mode);

        public static Find<Pair<T, TU>, Map<T, TU>> AsFinder ( ref Map<T, TU> map )
            => new Find<Pair<T, TU>, Map<T, TU>>(ref map);

        public static Set<Pair<T, TU>, Map<T, TU> > AsSet ( ref Map<T, TU> map, ISimpleCompare<Pair<T, TU>>? comparer = null, SortAction<ISimpleCompare<Pair<T, TU>>, Map<T, TU> >? sorter = null )
            => new Set<Pair<T, TU>, Map<T, TU> >( ref map, comparer == null ? new Less<Pair<T, TU>>() : comparer, sorter == null ? SortActions.ShellSorter : sorter );

        public static MultiSet<Pair<T, TU>, Map<T, TU>> AsMultiSet ( ref Map<T, TU> map, ISimpleCompare<Pair<T, TU>>? comparer = null, SortAction<ISimpleCompare<Pair<T, TU>>, Map<T, TU>>? sorter = null )
            => new MultiSet<Pair<T, TU>, Map<T, TU>>(ref map, comparer == null ? new Less<Pair<T, TU>>() : comparer, sorter == null ? SortActions.ShellSorter : sorter);

        public static UnorderedSet<Pair<T, TU>, Map<T, TU>> AsUnorderedSet ( ref Map<T, TU> vec )
            => new UnorderedSet<Pair<T, TU>, Map<T, TU>>(ref vec);

        public RandomAccessIterator<Pair<T, TU>, Map<T, TU>> Begin
            => new RandomAccessIterator<Pair<T, TU>, Map<T, TU>>(this, 0);

        public RandomAccessIterator<Pair<T, TU>, Map<T, TU>> End
            => new RandomAccessIterator<Pair<T, TU>, Map<T, TU>>(this, Count);

        public RandomAccessIterator<Pair<T, TU>, Map<T, TU>> ReverseBegin
            => End;

        public RandomAccessIterator<Pair<T, TU>, Map<T, TU>> ReverseEnd
            => Begin;

        public RandomAccessIterator<Pair<T, TU>, Map<T, TU>> At ( long index ) {
            if ( index >= Length ) return End;
            else return new RandomAccessIterator<Pair<T, TU>, Map<T, TU>>(this, index);
        }

        public static UnorderedMultiSet<Pair<T, TU>, Map<T, TU>> AsUnorderedMultiSet ( ref Map<T, TU> vec )
            => new UnorderedMultiSet<Pair<T, TU>, Map<T, TU>>(ref vec);


        public static Search< Pair<UU, UT>, Map<UU, UT> > AsSearch<UU, UT> ( ref Map<UU, UT> map, ISearchProvider<Pair<UU, UT>, Map<UU, UT>>? provider = null )
            => new Search< Pair<UU, UT> , Map<UU, UT> >(ref map, provider == null ? new LinearSearchProvider< Pair<UU, UT>, Map<UU, UT> >() : provider);

#endif
	}
	/// @}
}
