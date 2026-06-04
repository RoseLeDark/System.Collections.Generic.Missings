using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Collection.Generic {
    public class FixedArray<T> : IEnumerable<T> , IArray<T> {
        private T[] m_elements;

        private int m_index;

        public int Size => m_elements.Length;
        public T Front => m_elements[0];
        public T Back => m_elements[m_elements.Length - 1];
        public T Next => m_elements[m_index];

        public bool IsFull => m_index == Size;

        public bool IsEmpty => m_index == 0;

        public bool IsFixed => true;

        public T this[int adress] {
            get => m_elements[adress];
            set => m_elements[adress] = value;
        }
        public FixedArray(int size) {
            m_elements = new T[size];
            m_index = 0;
        }

        public FixedArray(T[] e) {
            m_elements = e;
            m_index = 0;
        }

        public bool Add(T entry) {
            if ( m_index >= Size ) {
                return false;
            }

            m_elements[m_index] = entry;
            m_index++;

            return true;
        }
        public bool Get(int index, ref T item) {
            if ( IsEmpty ) return false;
            item = m_elements[index];
            return true;
        }
        public bool Remove() {
            if ( IsEmpty ) return false;
            m_index--;
            return true;
        }

        public int IndexOf(T item) {
            return Array.IndexOf<T>(m_elements, item);
        }

        public IEnumerator<T> GetEnumerator() {
            for ( int i = 0; i < m_index; i++ )
                yield return m_elements[i];
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public void Insert(int pos, T item) {
            if ( pos < 0 || pos > m_index )
                throw new ArgumentOutOfRangeException(nameof(pos));

            // Wenn voll → ggf. wachsen
            if ( m_index >= Size ) {
                 throw new InvalidOperationException("Array is full and AutoGrow is disabled.");
            }

            // Elemente nach rechts schieben
            for ( int i = m_index; i > pos; i-- )
                m_elements[i] = m_elements[i - 1];

            // Einfügen
            m_elements[pos] = item;
            m_index++;
        }

        public void InsertRange(int pos, IEnumerable<T> items) {
            if ( pos < 0 || pos > m_index )
                throw new ArgumentOutOfRangeException(nameof(pos));

            // Materialisieren, damit wir Count kennen
            var list = items as ICollection<T> ?? new List<T>(items);
            int count = list.Count;

            if ( count == 0 )
                return;

            // Prüfen ob genug Platz ist
            int required = m_index + count;
            if ( required > Size ) {
                 throw new InvalidOperationException("Array is full and AutoGrow is disabled.");
            }

            // Platz schaffen: Block nach rechts schieben
            for ( int i = m_index - 1; i >= pos; i-- )
                m_elements[i + count] = m_elements[i];

            // Elemente einfügen
            int idx = pos;
            foreach ( var item in list )
                m_elements[idx++] = item;

            m_index += count;
        }


        public UInt64 NumberOfElements(T Key) {
            UInt64 _find = 0;

            foreach ( var item in m_elements ) {
                if ( item == null ) continue;
                if ( item.Equals(Key) ) _find++;
            }

            return _find;
        }

        public void Traverse(TraversMode mode, int startIndex, int endIndex, Action<T> func) {
            int start = Math.Max(startIndex, 0);
            int end = Math.Min(endIndex, m_index);

            if ( mode == TraversMode.Forwards ) {
                for ( int i = start; i < end; i++ )
                    func(m_elements[i]);
            } else if ( mode == TraversMode.Backwards ) {
                for ( int i = end; i >= start; i-- )
                    func(m_elements[i]);
            }
        }

        public T? FindFirst(T key) {
            foreach ( var p in m_elements ) {
                if ( p != null )
                    if ( p.Equals(key) ) return p;
            }
            return default(T);
        }
        public T? FindLast(T key) {
            for ( int i = m_elements.Length - 1; i >= 0; i-- ) {
                var p = m_elements[i];

                if ( p != null )
                    if ( p.Equals(key) ) return p;
            }
            return default(T);
        }

        public bool TryGet(T Key, out int index) {
            for ( int i = 0; i < m_index; i++ ) {
                var p = m_elements[i];

                if ( p != null )
                    if ( p.Equals(Key) ) {
                        index = i;
                        return true;
                    }
            }
            index = -1;
            return false;
        }

        public void CopyTo(int sourceIndex, byte[] destination, int destinationIndex, int count) {
            Buffer.BlockCopy(m_elements, sourceIndex, destination, destinationIndex, count);
        }


        public T[] ToArray() => m_elements;
    }
}
