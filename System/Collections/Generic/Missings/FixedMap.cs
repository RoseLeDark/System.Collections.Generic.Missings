using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace System.Collections.Generic.Missings {
    [Serializable]
    public class FixedMap<T, U> : IEnumerable<Pair<T, U>>, ICollection<Pair<T, U>>, IEnumerable, IMap<T, U>
        where T : notnull
        where U : notnull {

        private Pair<T, U>[] m_elements;
        private int m_count;
        private int m_size;

        public FixedMap(int N) {
            m_size = N;
            m_count = 0;
            m_elements = new Pair<T, U>[N];
        }

        public int Count => m_count;

        public bool IsEmpty => Count == 0;

        public bool IsFull => m_count == m_size;

        public bool IsReadOnly => false;

        public Pair<T, U> First {
            get {
                if ( IsEmpty )
                    throw new InvalidOperationException("Map is empty");
                return m_elements[0];
            }
        }

        public Pair<T, U> Last {
            get {
                if ( IsEmpty )
                    throw new InvalidOperationException("Map is empty");
                return m_elements[m_count - 1];
            }
        }

        public int Size => m_size;

        public void Add(Pair<T, U> item) {
            if ( m_count == m_size ) throw new Exception("FixedMap is Full");

            m_elements[m_count] = item;
            m_count++;
        }

        public Pair<T, U> this[int Key] {
            get => m_elements[Key];
            set => m_elements[Key] = value;
        }

        // Fixed = Replace
        public void Insert(int pos, Pair<T, U> item) {
            if ( IsFull ) throw new Exception("FixedMap is full");
            if ( pos < 0 || pos > m_count ) throw new IndexOutOfRangeException();

            m_elements[pos] = item;
        }

        public void InsertRange(int pos, IEnumerable<Pair<T, U>> items) {
            if ( IsFull ) throw new Exception("FixedMap is full");
            if ( items == null ) return;

            int _i = pos;
            foreach ( var item in items ) {
                if ( _i < 0 || _i > m_count ) throw new IndexOutOfRangeException();

                m_elements[_i] = item;
                _i++;
            }
        }

        public IEnumerable<Pair<T, U>> Find(T Key) {
            for ( int i = 0; i < m_count; i++ )
                if ( m_elements[i].EqualKeys(Key) )
                    yield return m_elements[i];

        }

        public delegate bool Compare(Pair<T, U> A, T Key, U Value);

        public List<Pair<T, U>> Findex(Compare func, T Key, U Value) {
            List<Pair<T, U>> _find = new List<Pair<T, U>>();

            for ( int i = 0; i < m_count; i++ ) {
                if ( func(m_elements[i], Key, Value) )
                    _find.Add(m_elements[i]);
            }
            return _find;
        }
        public void Traverse(TraversMode mode, int startIndex, int endIndex, Action<Pair<T, U>> func) {
            int start = Math.Max(startIndex, 0);
            int end = Math.Min(endIndex, m_count);

            if ( mode == TraversMode.Forwards ) {
                for ( int i = start; i < end; i++ )
                    func(m_elements[i]);
            } else if ( mode == TraversMode.Backwards ) {
                for ( int i = end; i >= start; i-- )
                    func(m_elements[i]);
            }
        }
        public UInt64 NumberOfElementsWithKey(T Key) {
            UInt64 _find = 0;

            for ( int i = 0; i < m_count; i++ ) {
                if ( m_elements[i].EqualKeys(Key) ) _find++;
            }

            return _find;
        }

        public UInt64 NumberOfElementsWithValue(U Value) {
            UInt64 _find = 0;

            for ( int i = 0; i < m_count; i++ ) {
                if ( m_elements[i].EqualValues(Value) ) _find++;
            }

            return _find;
        }

        public void Clear() {
            Array.Clear(m_elements);
            m_count = 0;
        }

        public bool Contains(Pair<T, U> item) {
            return m_elements.Contains(item);
        }
        public bool ContainsKey(T Key) {
            return NumberOfElementsWithKey(Key) > 0;
        }

        public U? Get(T Key) {
            var p = FindFirst(Key);
            if ( p.HasValue ) return p.Value.Value;
            throw new KeyNotFoundException();

        }

        public void CopyTo(Pair<T, U>[] array, int arrayIndex) {
            m_elements.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Pair<T, U>> GetEnumerator() {
            for(int i= 0; i < m_count; i++) {
                yield return m_elements[i];
            }
        }

        public bool Remove(Pair<T, U> item) {
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        public bool TryGet(T Key, out U Value) {
            for ( int i = 0; i < m_count; i++ ) {
                if ( m_elements[i].EqualKeys(Key) ) {
                    Value = m_elements[i].Value!;
                    return true;
                }
            }
            Value = default!;
            return false;
        }

        public void RemoveAt(int pos) {
            return;
        }

        public void RemoveAt(int start, int end) {
            return;
        }

        public Pair<T, U>? FindFirst(T key) {
            for(int i = 0; i < m_count; i++ ) {
                if ( m_elements[i].EqualKeys(key) ) return m_elements[i];
            }
            return null;
        }

        public Pair<T, U>? FindLast(T key) {
            for ( int i = m_count - 1; i >= 0; i-- ) {
                if ( m_elements[i].EqualKeys(key) )
                    return m_elements[i];
            }
            return null;
        }

        public Pair<T, U>[] ToArray() {
            return m_elements;
        }
    }
}
