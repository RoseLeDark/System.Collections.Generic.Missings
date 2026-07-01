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
using System.Drawing;
using System.Runtime.InteropServices;
using SystemEx.Collections.Generic.Interfaces;

namespace SystemEx.Collections.Generic {
    /// \addtogroup collections
    /// @{
    /// <summary>
    /// A simple random-access iterator for array-based data structures.
    /// Provides forward, backward, and offset-based movement.
    /// </summary>
    /// <typeparam name="T">The element type stored in the underlying array.</typeparam>
    public class ArrayRandomAccessIterator<T> : IRandomAccessIterator<T>, IForeachIterator<T> {

        /// <summary>
        /// The underlying array being iterated over.
        /// </summary>
        private readonly T[] m_values;

        /// <summary>
        /// The current index within the array.
        /// </summary>
        private int m_ipos;

        /// <summary>
        /// Gets the element at the current iterator position.
        /// </summary>
        public T Current => m_values[m_ipos];

        /// <summary>
        /// Indicates whether the iterator has reached the end of the array.
        /// </summary>
        public bool IsEnd => m_values.Length == m_ipos;

        /// <summary>
        /// Indicates whether the iterator is positioned at the beginning of the array.
        /// </summary>
        public bool IsBegin => m_ipos == 0;

        /// <summary>
        /// Creates a new iterator for the specified array at the given position.
        /// </summary>
        /// <param name="values">The array to iterate over.</param>
        /// <param name="pos">The initial iterator position.</param>
        public ArrayRandomAccessIterator(T[] values, int pos) {
            m_values = values;
            m_ipos = pos;
        }

        /// <summary>
        /// Returns a new iterator advanced by the specified offset.
        /// The original iterator remains unchanged.
        /// </summary>
        /// <param name="offset">The number of positions to move forward.</param>
        /// <returns>A new iterator positioned at the computed index.</returns>
        public IRandomAccessIterator<T> Advance( long offset ) {
            var newpos = offset + m_ipos;
            if ( newpos > m_values.Length )
                newpos = m_values.Length;

            return new ArrayRandomAccessIterator<T>(m_values, (int)newpos);
        }

        /// <summary>
        /// Moves the iterator one step backward, unless it is already at the beginning.
        /// </summary>
        public void Back() {
            if ( m_ipos > 0 )
                m_ipos--;
        }

        /// <summary>
        /// Creates a deep clone of the iterator, including a copy of the underlying array.
        /// </summary>
        /// <returns>A new iterator instance with its own array copy.</returns>
        public IIterator<T> Clone() {
            return new ArrayRandomAccessIterator<T>(m_values.ToArray(), m_ipos);
        }

        

        /// <summary>
        /// Moves the iterator one step forward, unless it is already at the end.
        /// </summary>
        public void Forward() {
            if ( !IsEnd ) m_ipos++;
        }
        /// <summary>
        /// Moves the iterator N step forward
        /// </summary>
        public void Forward ( long i ) {
            var n = i;
            while ( n > 0 ) {
                --n;
                Forward();
            }
        }

        object IEnumerator.Current => Current!;

        /// <summary>
        /// Returns this iterator as an enumerator.
        /// </summary>
        public IEnumerator<T> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;

        /// <summary>
        /// Moves to the next element for foreach enumeration.
        /// </summary>
        /// <returns><c>true</c> if the iterator advanced; otherwise <c>false</c>.</returns>
        public bool MoveNext() {
            if ( !IsEnd ) { m_ipos++; return true; }
            return false;
        }
        /// <summary>
        /// Reset is not supported for this iterator.
        /// </summary>
        public void Reset() { }
        /// <summary>
        /// Disposes the iterator. No resources are held.
        /// </summary>
        public void Dispose() {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Equality operator for comparing two iterators.
        /// </summary>
        public static bool operator ==(ArrayRandomAccessIterator<T>? a, ArrayRandomAccessIterator<T>? b) {
            if ( ReferenceEquals(a, b) ) return true;
            if ( a is null || b is null ) return false;
            return a.Equals(b);
        }

        /// <summary>
        /// Inequality operator for comparing two iterators.
        /// </summary>
        public static bool operator !=(ArrayRandomAccessIterator<T>? a, ArrayRandomAccessIterator<T>? b) {
            return !(a == b);
        }
        /// <inheritdoc/>
        public override bool Equals(object? obj) {
            if ( obj is ListIterator<T> ) {
                return Equals((ListIterator<T>)obj);
            }
            return false;
        }
        /// <inheritdoc/>
        public override int GetHashCode() {
            unchecked {
                int h = m_values.GetHashCode();
                h = (h * 397) ^ m_ipos;
                return h;
            }
        }
    }

    /// <summary>
    /// A dynamic array implementation that supports optional auto-growth,
    /// indexed access, insertion, removal, traversal, and basic search operations.
    /// </summary>
    /// <typeparam name="T">The element type stored in the array.</typeparam>
    public class Array<T> : IEnumerable<T>, IDynamicArray<T>, ICollection<T> {
#pragma warning disable CA1051
        /// <summary>
        /// Internal storage buffer for array elements.
        /// </summary>
        protected T[] m_elements;
        /// <summary>
        /// Current number of valid elements stored in the array.
        /// </summary>
        protected int m_index;
#pragma warning restore CA1051
        /// <summary>
        /// Gets the total capacity of the array.
        /// </summary>
        public int Size => m_elements.Length;
        /// <summary>
        /// Gets the first element of the array.
        /// </summary>
        public T Front => m_elements[0];
        /// <summary>
        /// Gets the last element of the array.
        /// </summary>
        public T Back => m_elements[m_elements.Length - 1];

        /// <summary>
        /// Gets a random-access iterator positioned at the beginning of the array.
        /// The iterator operates on a copy of the current array state.
        /// </summary>
        public ArrayRandomAccessIterator<T> First
            => new ArrayRandomAccessIterator<T>(this.ToArray(), 0);

        /// <summary>
        /// Gets a random-access iterator positioned at the logical end of the array.
        /// The iterator operates on a copy of the current array state.
        /// </summary>
        public ArrayRandomAccessIterator<T> End
            => new ArrayRandomAccessIterator<T>(this.ToArray(), Size-1);

        /// <summary>
        /// Return a random-access iterator positioned at the specified index.
        /// The iterator operates on a copy of the current array state.
        /// </summary>
        public ArrayRandomAccessIterator<T> At(int index) {
            if ( index > Size-1 ) index = Size-1;
            return new ArrayRandomAccessIterator<T>(this.ToArray(), index);
        }

        /// <summary>
        /// Indicates whether the array is full.
        /// </summary>
        public bool IsFull => m_index == Size;
        /// <summary>
        /// Indicates whether the array contains no elements.
        /// </summary>
        public bool IsEmpty => m_index == 0;
        /// <summary>
        /// Gets or sets the number of elements the array grows by when AutoGrow is enabled.
        /// </summary>
        public int GrowSize { get; set; }
        /// <summary>
        /// Enables or disables automatic resizing when the array becomes full.
        /// </summary>
        public virtual bool AutoGrow { get; set; }
        /// <summary>
        /// Indicates whether the array has a fixed size (AutoGrow disabled).
        /// </summary>
        public bool IsFixed => AutoGrow == false;

        /// <summary>
        /// 
        /// </summary>
        public int Count => m_elements.Length;
        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Provides indexed access to the array elements.
        /// </summary>
        public T this[int adress] {
                get => m_elements[adress];
                set => m_elements[adress] = value;
         }

        public Array( int size = 2 ) {
            m_elements = new T[size];
            m_index = size;
            GrowSize = 16;
            AutoGrow = true;
        }
        /// <summary>
        /// Creates a new array with the specified initial size.
        /// </summary>
        public Array(int size, int growSize) {
            m_elements = new T[size];
            m_index = 0;


            if ( (growSize > 0) ) {
                AutoGrow = true;
                GrowSize = growSize;
            } else {
                AutoGrow = false;
                GrowSize = 16;
            }
        }

        /// <summary>
        /// Creates a new array using an existing buffer.
        /// </summary>
        public Array(T[] e, int growSize = 16) {
            m_elements = e;
            m_index = 0;

            if ( (growSize > 0) ) {
                AutoGrow = true;
                GrowSize = growSize;
            } else {
                AutoGrow = false;
                GrowSize = 16;
            }
        }

        /// <summary>
        /// Creates a new array from an enumerable collection.
        /// </summary>
        public Array(IEnumerable<T> e, int growSize = 16) {
            m_elements = e.ToArray();
            m_index = m_elements.Length - 1;

            if ( (growSize > 0) ) {
                AutoGrow = true;
                GrowSize = growSize;
            } else {
                AutoGrow = false;
                GrowSize = 16;
            }
        }

        /// <summary>
        /// Adds an element to the end of the array.
        /// </summary>
        public virtual bool Add(T entry) {
            if ( m_index >= Size ) {
                if ( AutoGrow )
                    return Resize(Size + GrowSize);
                return false;
            }

            m_elements[m_index] = entry;
            m_index++;
            return true;
        }
        /// <summary>
        /// Add a range of elements to the end of the array.
        /// </summary>
        /// <param name="entry">The range of elements to add</param>
        /// <returns></returns>
        public virtual int AddRange ( T[] entry) {
            int _ret = 0;
            for( ; _ret < entry.Length ; _ret++ ) {
                if ( !Add(entry[_ret]) ) break;
            }
            return _ret;
        }
        /// <summary>
        /// Retrieves an element at the specified index.
        /// </summary>
        public bool Get(int index, ref T item) {
            if ( IsEmpty ) return false;
            item = m_elements[index];
            return true;
        }

        /// <summary>
        /// Removes the last element from the array.
        /// </summary>
        public virtual bool Remove() {
            if ( IsEmpty ) return false;
            m_index--;
            return true;
        }

        /// <summary>
        /// Resizes the internal buffer to the specified size.
        /// </summary>
        public virtual bool Resize(int size) {
            if ( size == m_elements.Length ) return false;
            if ( m_index > size )
                m_index = size;

            try {
                Array.Resize(ref m_elements, size);
            } catch {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Returns the index of the first occurrence of the specified item.
        /// </summary>
        public int IndexOf(T item) {
            return Array.IndexOf<T>(m_elements, item);
        }


        /// <summary>
        /// Returns an enumerator that iterates through the valid elements.
        /// </summary>
        public IEnumerator<T> GetEnumerator() {
            for(int i=0; i<m_index;i++)
                yield return m_elements[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Inserts an element at the specified position.
        /// </summary>
        public virtual int Insert(int pos, T item) {
            if ( pos < 0 || pos > m_index )
                throw new ArgumentOutOfRangeException(nameof(pos));

            if ( m_index >= Size ) {
                if ( !AutoGrow )
                    throw new InvalidOperationException("Array is full and AutoGrow is disabled.");
                Resize(Size + GrowSize);
            }

            m_elements[pos] = item;
            return 1;
        }
        /// <summary>
        /// Inserts a range of elements starting at the specified position.
        /// </summary>
        public virtual int InsertRange(int pos, IEnumerable<T> items) {
            if ( pos < 0 || pos > Size ) return 0;

            // Materialisieren, damit wir Count kennen
            var list = items as ICollection<T> ?? new List<T>(items);
            int count = list.Count;

            if ( count == 0 )  return 0;

            // Prüfen ob genug Platz ist
            int required = m_index + count;
            if ( required > Size ) {
                if ( !AutoGrow )
                    return 0;

                // so lange wachsen, bis es passt
                int newSize = Size;
                while ( required > newSize )
                    newSize += GrowSize;

                if ( !Resize(newSize) ) return -1;
            }

            // Platz schaffen: Block nach rechts schieben
            for ( int i = m_index - 1; i >= pos; i-- )
                m_elements[i + count] = m_elements[i];

            // Elemente einfügen
            int idx = pos;
            int written = 0;
            foreach ( var item in list ) {
                m_elements[idx++] = item;
                written++;
            }
            return written;
        }
        /// <summary>
        /// Counts how many elements equal the specified key.
        /// </summary>
        public UInt64 NumberOfElements(T Key) {
            UInt64 _find = 0;

            foreach ( var item in m_elements ) {
                if ( item == null ) continue;
                if ( item.Equals(Key) ) _find++;
            }

            return _find;
        }
        /// <summary>
        /// Traverses a range of elements in forward or backward order.
        /// </summary>
        public void Traverse(TraversMode mode, int startIndex, int endIndex, Action<T> func) {
            int start = System.Math.Max(startIndex, 0);
            int end = System.Math.Min(endIndex, Size);

            if ( mode == TraversMode.Forwards ) {
                for ( int i = start; i < end; i++ )
                    func(m_elements[i]);
            } else if ( mode == TraversMode.Backwards ) {
                for ( int i = end; i >= start; i-- )
                    func(m_elements[i]);
            }
        }
        /// <summary>
        /// Determines whether the array contains the specified value.
        /// </summary>
        public bool Is(T value) {
            foreach ( var p in m_elements ) {
                if ( p != null )
                    if ( p.Equals(value) ) return true;
            }
            return false;
        }
        /// <summary>
        /// Finds the first occurrence of the specified key.
        /// </summary>
        public T? FindFirst(T key) {
            foreach ( var p in m_elements )
                if ( p != null && p.Equals(key) )
                    return p;
            return default;
        }

        /// <summary>
        /// Finds the last occurrence of the specified key.
        /// </summary>
        public T? FindLast(T key) {
            for ( int i = m_elements.Length - 1; i >= 0; i-- ) {
                var p = m_elements[i];
                if ( p != null && p.Equals(key) )
                    return p;
            }
            return default;
        }

        /// <summary>
        /// Attempts to find the index of the specified key.
        /// </summary>
        public bool TryGet(T Key, out int index) {
            for ( int i = 0; i < m_index; i++ ) {
                var p = m_elements[i];
                if ( p != null && p.Equals(Key) ) {
                    index = i;
                    return true;
                }
            }
            index = -1;
            return false;
        }

        /// <summary>
        /// Copies a range of elements into a T array.
        /// </summary>
        public int CopyTo(uint sourceOffset, T[] destination, uint destinationOffset, uint count) {
            if ( destination == null ) return 0;

            int src = (int)sourceOffset;
            int dst = (int)destinationOffset;

            if ( src > Size ) src = Size;

            int toCopy = System.Math.Min((int)count,
            System.Math.Min(System.Math.Max(0, Size - src),
                     System.Math.Max(0, destination.Length - dst)));

            if ( toCopy <= 0 ) return 0;

            Buffer.BlockCopy(m_elements, src, destination, dst, toCopy);
            return toCopy;
        }


        /// <summary>
        /// Copies data from a T array into this array.
        /// </summary>
        public int CopyFrom(T[] source, uint sourceOffset, uint destinationOffset, uint count) {
            if ( source == null ) return 0;

            int src = (int)sourceOffset;
            int dst = (int)destinationOffset;

            if ( dst > Size ) dst = Size;

            int toCopy = System.Math.Min((int)count,
                System.Math.Min(System.Math.Max(0, source.Length - src),
                    System.Math.Max(0, Size - dst)));

            // Wenn nichts passt → prüfen ob wir wachsen müssen
            if ( toCopy <= 0 ) {
                // Prüfen ob AutoGrow aktiv ist
                if ( !AutoGrow ) return 0;

                // Wir brauchen mindestens count Bytes Platz ab dst
                int required = dst + (int)count;

                int newSize = Size;
                while ( required > newSize )
                    newSize += GrowSize;

                if ( !Resize(newSize) ) return 0;

                // Nach Resize neu berechnen
                toCopy = System.Math.Min((int)count, System.Math.Min(System.Math.Max(0, source.Length - src), System.Math.Max(0, Size - dst)));

                if ( toCopy <= 0 )
                    return 0;
            }

            Buffer.BlockCopy(source, src, m_elements, dst, toCopy);
            return toCopy;
        }
        /// <summary>
        /// Returns a copy of the internal buffer.
        /// </summary>
        public T[] ToArray() => m_elements.ToArray();

        void ICollection<T>.Add(T item) => Add(item);

        /// <summary>
        /// 
        /// </summary>
        public void Clear() {
            return;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item) => Is(item);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex) {
            CopyTo(0, array, 0, (uint)arrayIndex);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item) {
            return false;
        }
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
