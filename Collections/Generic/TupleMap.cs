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

using SystemEx.Collections.Generic.Interfaces;
using System.Collections;

namespace SystemEx.Collections.Generic {
    /// <summary>
    /// A dynamic, list‑backed map structure that stores <see cref="ITuple"/> objects.
    /// Provides tuple‑based lookup, traversal, range operations, and key/value
    /// convenience helpers.  
    /// This implementation allows arbitrary tuple arity, but assumes that
    /// index 0 represents the key and index 1 represents the value.
    /// </summary>
    [Serializable]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Naming", "CA1710:Identifiers should have correct suffix",
        Justification = "<Pending>")]
    public class TupleMap :
        IEnumerable<ITuple>,
        ICollection<ITuple>,
        IEnumerable,
        ITupleMap,
        ITraverse<ITuple> {
        /// <summary>
        /// Internal storage for all tuple elements.
        /// </summary>
        internal List<ITuple> m_elements;

        /// <summary>
        /// Protected accessor for derived classes to manipulate the underlying list.
        /// </summary>
        protected List<ITuple> Elements {
            get => m_elements;
            set => m_elements = value;
        }

        /// <summary>
        /// Gets the number of tuples stored in the map.
        /// </summary>
        public int Count => m_elements.Count;

        /// <summary>
        /// Indicates whether the map contains no elements.
        /// </summary>
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// Indicates whether the map has reached a fixed capacity.
        /// Always <c>false</c> because the underlying list is dynamic.
        /// </summary>
        public bool IsFull => false;

        /// <summary>
        /// Indicates whether the map is read‑only.
        /// Always <c>false</c>.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets the first tuple in the map.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the map is empty.</exception>
        public ITuple? First {
            get {
                if ( m_elements.Count == 0 )
                    throw new InvalidOperationException("Map is empty");
                return m_elements[0]!;
            }
        }

        /// <summary>
        /// Gets the last tuple in the map.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the map is empty.</exception>
        public ITuple? Last {
            get {
                if ( m_elements.Count == 0 )
                    throw new InvalidOperationException("Map is empty");
                return m_elements[m_elements.Count - 1]!;
            }
        }

        /// <summary>
        /// Gets or sets the tuple at the specified index.
        /// </summary>
        public ITuple this[int Key] {
            get => m_elements[Key];
            set => m_elements[Key] = value;
        }

        /// <summary>
        /// Gets the theoretical maximum size of the map.
        /// Always <see cref="Int32.MaxValue"/>.
        /// </summary>
        public int Size => Int32.MaxValue;

        /// <summary>
        /// Creates an empty tuple map.
        /// </summary>
        public TupleMap() {
            m_elements = new List<ITuple>();
        }

        /// <summary>
        /// Creates a tuple map initialized with the specified elements.
        /// </summary>
        public TupleMap(IEnumerable<ITuple> elements) {
            m_elements = [.. elements];
        }

        /// <summary>
        /// Adds a tuple to the map, optionally allowing duplicates.
        /// </summary>
        protected virtual bool Add(ITuple item, bool multi) {
            bool contains = multi ? false : m_elements.Contains(item);

            if ( !contains ) {
                m_elements.Add(item);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a tuple to the map.  
        /// Duplicate tuples are not allowed.
        /// </summary>
        public virtual void Add(ITuple item) {
            Add(item, false);
        }

        /// <summary>
        /// Inserts a tuple at the specified position.
        /// </summary>
        public virtual bool Insert(int pos, ITuple item) {
            m_elements.Insert(pos, item);
            return true;
        }

        /// <summary>
        /// Inserts a range of tuples at the specified position.
        /// </summary>
        public virtual bool InsertRange(int pos, IEnumerable<ITuple> items) {
            m_elements.InsertRange(pos, items);
            return true;
        }

        /// <summary>
        /// Finds all tuples whose first element equals the specified key.
        /// </summary>
        public virtual IEnumerable<ITuple> Find(object Key) {
            foreach ( var item in m_elements ) {
                if ( item.Get(0)!.Equals(Key) )
                    yield return item;
            }
        }

        /// <summary>
        /// Delegate used for custom tuple matching in <see cref="Findex"/>.
        /// </summary>
        public delegate bool Compare(ITuple A, object Key, object Value);

        /// <summary>
        /// Performs a custom search using a user‑provided comparison function.
        /// </summary>
        public List<ITuple> Findex(Compare func, object Key, object Value) {
            List<ITuple> result = new List<ITuple>();

            foreach ( var item in m_elements ) {
                if ( func(item, Key, Value) )
                    result.Add(item);
            }
            return result;
        }

        /// <summary>
        /// Traverses a range of tuples in forward or backward order.
        /// </summary>
        public void Traverse(TraversMode mode, int startIndex, int endIndex, Action<ITuple> func) {
            int start = System.Math.Max(startIndex, 0);
            int end = System.Math.Min(endIndex, m_elements.Count);

            if ( mode == TraversMode.Forwards ) {
                for ( int i = start; i < end; i++ )
                    func(m_elements[i]);
            } else {
                for ( int i = end; i >= start; i-- )
                    func(m_elements[i]);
            }
        }

        /// <summary>
        /// Counts how many tuples have a first element equal to the specified key.
        /// </summary>
        public UInt64 NumberOfElementsWithKey(object Key) {
            UInt64 count = 0;

            foreach ( var item in m_elements ) {
                var obj = item.Get(0);
                if ( obj != null && obj.Equals(Key) )
                    count++;
            }

            return count;
        }

        /// <summary>
        /// Counts how many tuples have a second element equal to the specified value.
        /// </summary>
        public UInt64 NumberOfElementsWithValue(object Value) {
            UInt64 count = 0;

            foreach ( var item in m_elements ) {
                var obj = item.Get(1);
                if ( obj != null && obj.Equals(Value) )
                    count++;
            }

            return count;
        }

        /// <summary>
        /// Removes all tuples from the map.
        /// </summary>
        public void Clear() {
            m_elements.Clear();
        }

        /// <summary>
        /// Determines whether the map contains the specified tuple.
        /// </summary>
        public bool Contains(ITuple item) => m_elements.Contains(item);

        /// <summary>
        /// Determines whether any tuple has a first element equal to the specified key.
        /// </summary>
        public bool ContainsKey(object Key) {
            foreach ( var p in m_elements ) {
                var obj = p.Get(0);
                if ( obj != null && obj.Equals(Key) )
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Retrieves the value (index 1) of the first tuple matching the specified key.
        /// </summary>
        /// <exception cref="KeyNotFoundException">Thrown when no matching tuple exists.</exception>
        public object? Get(object Key) {
            ITuple? p = FindFirst(Key);
            if ( p != null ) return p.Get(1);
            throw new KeyNotFoundException();
        }

        /// <summary>
        /// Copies the tuples into the specified array.
        /// </summary>
        public void CopyTo(ITuple[] array, int arrayIndex) {
            m_elements.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator over the stored tuples.
        /// </summary>
        public IEnumerator<ITuple> GetEnumerator() => m_elements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Removes the specified tuple from the map.
        /// </summary>
        public bool Remove(ITuple item) => m_elements.Remove(item);

        /// <summary>
        /// Attempts to retrieve the value (index 1) of the first tuple matching the key.
        /// </summary>
        public bool TryGet(object Key, out object Value) {
            foreach ( var p in m_elements ) {
                if ( p.Get(0)!.Equals(Key) ) {
                    Value = p.Get(1)!;
                    return true;
                }
            }
            Value = default!;
            return false;
        }

        /// <summary>
        /// Removes the tuple at the specified index.
        /// </summary>
        public void RemoveAt(int pos) {
            m_elements.RemoveAt(pos);
        }

        /// <summary>
        /// Removes a range of tuples.
        /// </summary>
        public void RemoveAt(int start, int iend) {
            if ( start > iend ) return;
            m_elements.RemoveRange(start, iend - start);
        }

        /// <summary>
        /// Finds the first tuple whose first element equals the specified key.
        /// </summary>
        public ITuple? FindFirst(object key) {
            foreach ( var p in m_elements ) {
                var obj = p.Get(0);
                if ( obj != null && obj.Equals(key) )
                    return p;
            }
            return null;
        }

        /// <summary>
        /// Finds the last tuple whose first element equals the specified key.
        /// </summary>
        public ITuple? FindLast(object key) {
            for ( int i = m_elements.Count - 1; i >= 0; i-- ) {
                if ( m_elements[i].Equals(key) )
                    return m_elements[i];
            }
            return null;
        }

        /// <summary>
        /// Returns all tuples as an array.
        /// </summary>
        public ITuple[] ToArray() => [.. m_elements];
    }

}
