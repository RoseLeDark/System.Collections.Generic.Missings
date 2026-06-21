/* SPDX-License-Identifier: EUPL-1.2
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
    /// A dynamic map storing <see cref="Pair{T, TU}"/> elements in a list.  
    /// The map supports duplicate keys but prevents duplicate pairs unless
    /// explicitly overridden.  
    /// Provides key lookup, value lookup, traversal, range operations,
    /// and indexed access.
    /// </summary>
    /// <typeparam name="T">The key type (non‑null).</typeparam>
    /// <typeparam name="TU">The value type (non‑null).</typeparam>
    [Serializable]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Naming", "CA1710:Identifiers should have correct suffix",
        Justification = "<Pending>")]
    public class Map<T, TU> :
        IEnumerable<Pair<T, TU>>,
        ICollection<Pair<T, TU>>,
        IEnumerable,
        IMap<T, TU>,
        IReadOnlyMap<T, TU>,
        ITraverse<Pair<T, TU>> {
        /// <summary>
        /// Internal list storing all key/value pairs.
        /// </summary>
        internal List<Pair<T, TU>> m_elements;

        /// <summary>
        /// Protected accessor for derived classes to manipulate the underlying list.
        /// </summary>
        protected List<Pair<T, TU>> Elements {
            get => m_elements;
            set => m_elements = value;
        }

        /// <summary>
        /// Gets the number of stored elements.
        /// </summary>
        public int Count => m_elements.Count;

        /// <summary>
        /// Indicates whether the map contains no elements.
        /// </summary>
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// Always <c>false</c>; the map grows dynamically.
        /// </summary>
        public bool IsFull => false;

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
                if ( m_elements.Count == 0 )
                    throw new InvalidOperationException("Map is empty");
                return m_elements[0]!;
            }
        }

        /// <summary>
        /// Gets the last element in the map.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the map is empty.</exception>
        public Pair<T, TU>? Last {
            get {
                if ( m_elements.Count == 0 )
                    throw new InvalidOperationException("Map is empty");
                return m_elements[m_elements.Count - 1]!;
            }
        }

        /// <summary>
        /// Gets the theoretical maximum size of the map.
        /// Always <see cref="Int32.MaxValue"/>.
        /// </summary>
        public int Size => Int32.MaxValue;
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

        IEnumerable<T> IReadOnlyMap<T, TU>.Keys => Keys;

        IEnumerable<TU> IReadOnlyMap<T, TU>.Values => Values;

        /// <summary>
        /// Creates an empty map.
        /// </summary>
        public Map() {
            m_elements = new List<Pair<T, TU>>();
        }

        /// <summary>
        /// Creates a map initialized with the specified elements.
        /// </summary>
        public Map(IEnumerable<Pair<T, TU>> elements) {
            m_elements = [.. elements];
        }

        /// <summary>
        /// Adds a pair to the map if it does not already exist.
        /// </summary>
        public virtual void Add(Pair<T, TU> item) {
            if ( !m_elements.Contains(item) )
                m_elements.Add(item);
        }

        /// <summary>
        /// Adds a key/value pair to the map.
        /// </summary>
        public virtual void Add(T k, TU v) {
            Add(new Pair<T, TU>(k, v));
        }

        /// <summary>
        /// Attempts to add a key/value pair to the map.
        /// </summary>
        public virtual bool TryAdd(T k, TU v) {
            var pair = new Pair<T, TU>(k, v);
            Add(pair);
            return Contains(pair);
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        public Pair<T, TU> this[int Key] {
            get => m_elements[Key];
            set => m_elements[Key] = value;
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        public TU? this[T key] {
            get => Get(key);
            set {
                if ( value is null )
                    throw new ArgumentNullException(nameof(value));
                Set(key, value);
            }
        }

        /// <summary>
        /// Updates the value for the specified key.  
        /// If the key exists, the old pair is removed and a new pair is appended.  
        /// If the key does not exist, a new pair is added.
        /// </summary>
        private bool Set(T key, TU value) {
            for ( int i = 0; i < m_elements.Count; i++ ) {
                if ( m_elements[i].EqualFirst(key) ) {

                    if ( m_elements[i].EqualSecond(value) )
                        return true;

                    int last = m_elements.Count - 1;
                    m_elements[i] = m_elements[last];
                    m_elements.RemoveAt(last);

                    m_elements.Add(new Pair<T, TU>(key, value));
                    return true;
                }
            }

            return TryAdd(key, value);
        }

        /// <summary>
        /// Inserts a pair at the specified index.
        /// </summary>
        public virtual bool Insert(int pos, Pair<T, TU> item) {
            m_elements.Insert(pos, item);
            return true;
        }

        /// <summary>
        /// Inserts a range of pairs at the specified index.
        /// </summary>
        public virtual bool InsertRange(int pos, IEnumerable<Pair<T, TU>> items) {
            m_elements.InsertRange(pos, items);
            return true;
        }

        /// <summary>
        /// Finds all pairs whose key matches the specified value.
        /// </summary>
        public IEnumerable<Pair<T, TU>> Find(T Key) {
            foreach ( var item in m_elements ) {
                if ( item.First!.Equals(Key) )
                    yield return item;
            }
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

            foreach ( var item in m_elements ) {
                if ( func(item, Key, Value) )
                    _find.Add(item);
            }
            return _find;
        }

        /// <summary>
        /// Traverses a range of elements in forward or backward order.
        /// </summary>
        public void Traverse(TraversMode mode, int startIndex, int endIndex, Action<Pair<T, TU>> func) {
            int start = System.Math.Max(startIndex, 0);
            int end =   System.Math.Min(endIndex, m_elements.Count);

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

            foreach ( var item in m_elements ) {
                if ( item.First == null ) continue;
                if ( item.First.Equals(Key) ) _find++;
            }

            return _find;
        }

        /// <summary>
        /// Counts how many elements have the specified value.
        /// </summary>
        public UInt64 NumberOfElementsWithValue(TU Value) {
            UInt64 _find = 0;

            foreach ( var item in m_elements ) {
                if ( item.Second == null ) continue;
                if ( item.Second.Equals(Value) ) _find++;
            }

            return _find;
        }

        /// <summary>
        /// Removes all elements from the map.
        /// </summary>
        public void Clear() {
            m_elements.Clear();
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
            foreach ( var p in m_elements ) {
                if ( p.First!.Equals(Key) )
                    return true;
            }
            return false;
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
            return m_elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Removes the specified pair from the map.
        /// </summary>
        public bool Remove(Pair<T, TU> item) {
            return m_elements.Remove(item);
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
        /// <summary>
        /// Remove the first element with the specified key.
        /// </summary>
        /// <param name="key">The key to remove from the map.</param>
        /// <returns>true when remove and false when not.</returns>
        public bool Remove(T key) {
            bool _ret = false;

            for(int i = 0; i < m_elements.Count; i++ ) {
                if ( m_elements[i].EqualFirst(key) ) {
                    m_elements.RemoveAt(i);
                    break;
                }
            }
            return _ret;
        }

        IEnumerator<Pair<T, TU>> IEnumerable<Pair<T, TU>>.GetEnumerator() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the element at the specified index.
        /// </summary>
        public void RemoveAt(int pos) {
            m_elements.RemoveAt(pos);
        }

        /// <summary>
        /// Removes a range of elements.
        /// </summary>
        public void RemoveAt(int start, int iend) {
            if ( start > iend ) return;
            m_elements.RemoveRange(start, iend - start);
        }

        /// <summary>
        /// Finds the first element with the specified key.
        /// </summary>
        public Pair<T, TU>? FindFirst(T key) {
            foreach ( var p in m_elements ) {
                if ( p.EqualFirst(key) ) return p;
            }
            return null;
        }

        /// <summary>
        /// Finds the last element with the specified key.
        /// </summary>
        public Pair<T, TU>? FindLast(T key) {
            for ( int i = m_elements.Count - 1; i >= 0; i-- ) {
                if ( m_elements[i].EqualFirst(key) )
                    return m_elements[i];
            }
            return null;
        }

        /// <summary>
        /// Returns all stored elements as an array.
        /// </summary>
        public Pair<T, TU>[] ToArray() {
            return [.. m_elements];
        }

        
    }

}
