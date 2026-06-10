using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Collection.Generic {
    public class FixedArray<T> : IEnumerable<T> , IArray<T> {
        internal T[] m_elements;
        internal int m_index;

        public int Size => m_elements.Length;
        public T Front => m_elements[0];
        public T Back => m_elements[m_elements.Length - 1];

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
            m_index = e.Length;
        }

        public virtual bool Add(T entry) {
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
        public virtual bool Remove() {
            if ( IsEmpty ) return false;
            m_index--;
            return true;
        }

        public int IndexOf(T item) {
            return Array.IndexOf<T>(m_elements, item);
        }

        public IEnumerator<T> GetEnumerator() {
            for ( int i = 0; i < Size; i++ )
                yield return m_elements[i];
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public virtual int Insert(int pos, T item) {
            if ( pos < 0 || pos >= Size ) return 0;

            m_elements[pos] = item;
            m_index = pos + 1;
            return 1;
        }

        public virtual int InsertRange(int pos, IEnumerable<T> items) {
            if ( pos < 0 || pos > Size ) return 0; 

            // Materialisieren, damit wir Count kennen
            var list = items as ICollection<T> ?? new List<T>(items);
            int count = list.Count;

            if ( count == 0 )
                return 0;

            // Prüfen ob genug Platz ist
            int space = Size - pos;          // wie viel passt ab pos?
            int toWrite = count > space ? space : count;

            int idx = pos;
            int written = 0;
            foreach ( var item in list ) {
                if ( written >= toWrite )
                    break;

                m_elements[idx++] = item;
                written++;
            }

            return written;
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
            int end = Math.Min(endIndex, Size);

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
            for ( int i = 0; i < Size; i++ ) {
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

        public int CopyTo(uint sourceOffset, byte[] destination, uint destinationOffset, uint count) {
            if ( destination == null ) return 0;

            int src = (int)sourceOffset;
            int dst = (int)destinationOffset;

            if ( src > Size ) src = Size;

            int toCopy = Math.Min((int)count,
                Math.Min(Math.Max(0, Size - src),
                 Math.Max(0, destination.Length - dst)));

            if ( toCopy <= 0 ) return 0;

            Buffer.BlockCopy(m_elements, src, destination, dst, toCopy);
            return toCopy;
        }



        public int CopyFrom(byte[] source, uint sourceOffset, uint destinationOffset, uint count) {
            if ( source == null ) return 0;

            int src = (int)sourceOffset;
            int dst = (int)destinationOffset;

            if ( dst > Size ) dst = Size;

            int toCopy = Math.Min((int)count,
                Math.Min(Math.Max(0, source.Length - src),
                 Math.Max(0, Size - dst)));

            if ( toCopy <= 0 ) return 0;

            Buffer.BlockCopy(source, src, m_elements, dst, toCopy);
            return toCopy;
        }



        public T[] ToArray() => m_elements;
    }
}
