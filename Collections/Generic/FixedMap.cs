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
using SystemEx.Collections.Generic.Interfaces;

namespace SystemEx.Collections.Generic {
    /// <summary>
    /// A fixed‑size map storing <see cref="Pair{T, TU}"/> elements in a
    /// contiguous array.  
    /// Unlike dynamic maps, a <see cref="FixedMap{T, TU}"/> never grows:
    /// once the capacity is reached, no further insertions are possible.
    /// </summary>
    /// <typeparam name="T">The key type </typeparam>
    /// <typeparam name="TU">The value type </typeparam>
    [Serializable]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Naming", "CA1710:Identifiers should have correct suffix",
        Justification = "<Pending>")]
    public class FixedMap<T, TU> :
        IEnumerable<Pair<T, TU>>,
        ICollection<Pair<T, TU>>,
        IEnumerable,
        IMap<T, TU>, 
        IReadOnlyMap<T, TU>
        where T : notnull
        where TU : notnull {
        /// <summary>
        /// Internal array storing all key/value pairs.
        /// </summary>
        private Pair<T, TU>[] m_elements;

        /// <summary>
        /// Number of elements currently stored in the map.
        /// </summary>
        private int m_count;

        /// <summary>
        /// Maximum number of elements the map can hold.
        /// </summary>
        private int m_size;

        /// <summary>
        /// Creates a new fixed‑size map with the specified capacity.
        /// </summary>
        /// <param name="N">The maximum number of elements.</param>
        public FixedMap(int N) {
            m_size = N;
            m_count = 0;
            m_elements = new Pair<T, TU>[N];
        }

        /// <summary>
        /// Gets the number of stored elements.
        /// </summary>
        public int Count => m_count;

        /// <summary>
        /// Gets the maximum capacity of the map.
        /// </summary>
        public int Size => m_size;

        /// <summary>
        /// Indicates whether the map contains no elements.
        /// </summary>
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// Indicates whether the map has reached its maximum capacity.
        /// </summary>
        public bool IsFull => m_count == m_size;

        /// <summary>
        /// Always <c>false</c>; the map supports modification.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets the first element in the map.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the map is empty.</exception>
        public Pair<T, TU>? First {
            get {
                if ( IsEmpty )
                    throw new InvalidOperationException("Map is empty");
                return m_elements[0];
            }
        }

        /// <summary>
        /// Gets the last element in the map.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the map is empty.</exception>
        public Pair<T, TU>? Last {
            get {
                if ( IsEmpty )
                    throw new InvalidOperationException("Map is empty");
                return m_elements[m_count - 1];
            }
        }

        /// <summary>
        /// Gets an enumerable collection of all keys contained in the map.
        /// </summary>
        public ICollection<T> Keys {
            get {
                List<T> tmp = new List<T>();
                foreach ( var t in m_elements ) { tmp.Add(t.First); }
                return tmp;
            }
        }
        /// <summary>
        /// Gets an enumerable collection of all values contained in the map.
        /// </summary>
        public ICollection<TU> Values {
            get {
                List<TU> tmp = new List<TU>();
                foreach ( var t in m_elements ) { tmp.Add(t.Second); }
                return tmp;
            }
        }
        /// <summary>
        /// Gets an enumerable collection of all keys contained in the map.
        /// </summary>
        IEnumerable<T> IReadOnlyMap<T, TU>.Keys => Keys;
        /// <summary>
        /// Gets an enumerable collection of all values contained in the map.
        /// </summary>
        IEnumerable<TU> IReadOnlyMap<T, TU>.Values => Values;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TU this[T key] {
            get => Get(key);
            set {
                    for ( int i = 0; i < m_count; i++ ) {
                        if ( m_elements[i].EqualFirst(key) ) {
                            m_elements[i].Second = value;
                        }
                    }
                }
        }

        /// <summary>
        /// Adds a pair to the map if space is available and the pair does not already exist.
        /// </summary>
        public void Add(Pair<T, TU> item) {
            if ( m_count == m_size ) return;
            if ( Contains(item) ) return;

            m_elements[m_count] = item;
            m_count++;
        }

        /// <summary>
        /// Attempts to add a pair to the map.
        /// </summary>
        /// <returns><c>true</c> if added; otherwise <c>false</c>.</returns>
        public bool TryAdd(Pair<T, TU> item) {
            if ( m_count == m_size ) return false;
            if ( Contains(item) ) return false;

            m_elements[m_count] = item;
            m_count++;
            return true;
        }

        /// <summary>
        /// Attempts to add a key/value pair to the map.
        /// </summary>
        public bool TryAdd(T f, TU sec) {
            return TryAdd(new Pair<T, TU>(f, sec));
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        public Pair<T, TU> this[int Key] {
            get => m_elements[Key];
            set => m_elements[Key] = value;
        }

        /// <summary>
        /// Inserts or replaces an element at the specified position.
        /// </summary>
        public bool Insert(int pos, Pair<T, TU> item) {
            if ( IsFull ) return false;
            if ( pos < 0 || pos > m_count ) return false;

            m_elements[pos] = item;
            return true;
        }

        /// <summary>
        /// Inserts or replaces a sequence of elements starting at the specified position.
        /// </summary>
        public bool InsertRange(int pos, IEnumerable<Pair<T, TU>> items) {
            if ( IsFull ) return false;
            if ( items == null ) return false;

            int _i = pos;
            foreach ( var item in items ) {
                if ( _i < 0 || _i > m_count ) return false;
                m_elements[_i] = item;
                _i++;
            }
            return true;
        }

        /// <summary>
        /// Finds all elements whose key matches the specified value.
        /// </summary>
        public IEnumerable<Pair<T, TU>> Find(T Key) {
            for ( int i = 0; i < m_count; i++ )
                if ( m_elements[i].EqualFirst(Key) )
                    yield return m_elements[i];
        }

        /// <summary>
        /// Delegate used for custom search logic in <see cref="Findex"/>.
        /// </summary>
        public delegate bool Compare(Pair<T, TU> A, T Key, TU Value);

        /// <summary>
        /// Performs a custom search using the provided comparison function.
        /// </summary>
        public List<Pair<T, TU>> Findex(Compare func, T Key, TU Value) {
            List<Pair<T, TU>> _find = new List<Pair<T, TU>>();

            for ( int i = 0; i < m_count; i++ ) {
                if ( func(m_elements[i], Key, Value) )
                    _find.Add(m_elements[i]);
            }
            return _find;
        }

        /// <summary>
        /// Traverses a range of elements in forward or backward order.
        /// </summary>
        public void Traverse(TraversMode mode, int startIndex, int endIndex, Action<Pair<T, TU>> func) {
            int start = System.Math.Max(startIndex, 0);
            int end = System.Math.Min(endIndex, m_count);

            if ( mode == TraversMode.Forwards ) {
                for ( int i = start; i < end; i++ )
                    func(m_elements[i]);
            } else if ( mode == TraversMode.Backwards ) {
                for ( int i = end; i >= start; i-- )
                    func(m_elements[i]);
            }
        }

        /// <summary>
        /// Counts how many elements have the specified key.
        /// </summary>
        public UInt64 NumberOfElementsWithKey(T Key) {
            UInt64 _find = 0;

            for ( int i = 0; i < m_count; i++ )
                if ( m_elements[i].EqualFirst(Key) )
                    _find++;

            return _find;
        }

        /// <summary>
        /// Counts how many elements have the specified value.
        /// </summary>
        public UInt64 NumberOfElementsWithValue(TU Value) {
            UInt64 _find = 0;

            for ( int i = 0; i < m_count; i++ )
                if ( m_elements[i].EqualSecond(Value) )
                    _find++;

            return _find;
        }

        /// <summary>
        /// Clears the map and resets the element count.
        /// </summary>
        public void Clear() {
            Array.Clear(m_elements);
            m_count = 0;
        }

        /// <summary>
        /// Determines whether the map contains the specified pair.
        /// </summary>
        public bool Contains(Pair<T, TU> item) {
            return m_elements.Contains(item);
        }

        /// <summary>
        /// Determines whether any element has the specified key.
        /// </summary>
        public bool ContainsKey(T Key) {
            return NumberOfElementsWithKey(Key) > 0;
        }

        /// <summary>
        /// Retrieves the value associated with the specified key.
        /// </summary>
        /// <exception cref="KeyNotFoundException">Thrown when the key is not found.</exception>
        public TU? Get(T Key) {
            var p = FindFirst(Key);
            if ( p.HasValue ) return p.Value.Second;
            throw new KeyNotFoundException();
        }

        /// <summary>
        /// Copies the elements into the specified array.
        /// </summary>
        public void CopyTo(Pair<T, TU>[] array, int arrayIndex) {
            m_elements.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator over the stored elements.
        /// </summary>
        public IEnumerator<Pair<T, TU>> GetEnumerator() {
            for ( int i = 0; i < m_count; i++ )
                yield return m_elements[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Removal is not supported in <see cref="FixedMap{T, TU}"/>.
        /// </summary>
        public bool Remove(Pair<T, TU> item) {
            return false;
        }

        /// <summary>
        /// Attempts to retrieve the value associated with the specified key.
        /// </summary>
        public bool TryGet(T Key, out TU Value) {
            for ( int i = 0; i < m_count; i++ ) {
                if ( m_elements[i].EqualFirst(Key) ) {
                    Value = m_elements[i].Second!;
                    return true;
                }
            }
            Value = default!;
            return false;
        }

        /// <summary>
        /// Removal by index is not supported in <see cref="FixedMap{T, TU}"/>.
        /// </summary>
        public void RemoveAt(int pos) {
            return;
        }

        /// <summary>
        /// Range removal is not supported in <see cref="FixedMap{T, TU}"/>.
        /// </summary>
        public void RemoveAt(int start, int iend) {
            return;
         
        }

        /// <summary>
        /// Finds the first element with the specified key.
        /// </summary>
        public Pair<T, TU>? FindFirst(T key) {
            for ( int i = 0; i < m_count; i++ )
                if ( m_elements[i].EqualFirst(key) )
                    return m_elements[i];

            return null;
        }

        /// <summary>
        /// Finds the last element with the specified key.
        /// </summary>
        public Pair<T, TU>? FindLast(T key) {
            for ( int i = m_count - 1; i >= 0; i-- )
                if ( m_elements[i].EqualFirst(key) )
                    return m_elements[i];

            return null;
        }

        /// <summary>
        /// Returns the internal array of elements.  
        /// Note: The returned array may contain unused slots.
        /// </summary>
        public Pair<T, TU>[] ToArray() {
            return m_elements;
        }
        /// <summary>
        /// Adds a key/value pair to the map.
        /// </summary>
        public void Add(T key, TU value) {
            Add(new Pair<T, TU>(key, value));
        }

        /// <summary>
        /// Range removal is not supported in <see cref="FixedMap{T, TU}"/>.
        /// </summary>
        public bool Remove(T key) {
            return false;
        }

        /// <summary>
        /// Attempts to retrieve the value associated with the specified key.
        /// </summary>
        public bool TryGeValue(T key, [MaybeNullWhen(false)] out TU value) {
            foreach ( var p in m_elements ) {
                if ( p.EqualFirst(key) ) {
                    value = p.Second!;
                    return true;
                }
            }
            value = default!;
            return false;
        }
    }

}
