using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace System.Collections.Generic.Missings {
    [Serializable]
    [Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:Bezeichner müssen ein korrektes Suffix aufweisen", Justification = "<Ausstehend>")]
    public class FixedMap<T, TU> : IEnumerable<Pair<T, TU>>, ICollection<Pair<T, TU>>, IEnumerable, IMap<T, TU>
        where T : notnull
        where TU : notnull {

        private Pair<T, TU>[] m_elements;
        private int m_count;
        private int m_size;

        public FixedMap(int N) {
            m_size = N;
            m_count = 0;
            m_elements = new Pair<T, TU>[N];
        }

        public int Count => m_count;

        public bool IsEmpty => Count == 0;

        public bool IsFull => m_count == m_size;

        public bool IsReadOnly => false;

        public Pair<T, TU>? First {
            get {
                if ( IsEmpty )
                    throw new InvalidOperationException("Map is empty");
                return m_elements[0];
            }
        }

        public Pair<T, TU>? Last {
            get {
                if ( IsEmpty )
                    throw new InvalidOperationException("Map is empty");
                return m_elements[m_count - 1];
            }
        }

        public int Size => m_size;

        public void Add(Pair<T, TU> item) {
            if ( m_count == m_size ) return ;

            m_elements[m_count] = item;
            m_count++;

            return  ;
        }

        public Pair<T, TU> this[int Key] {
            get => m_elements[Key];
            set => m_elements[Key] = value;
        }

        // Fixed = Replace
        public bool Insert(int pos, Pair<T, TU> item) {
            if ( IsFull ) return false;
            if ( pos < 0 || pos > m_count ) return false;

            m_elements[pos] = item;
            return true;
        }

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

        public IEnumerable<Pair<T, TU>> Find(T Key) {
            for ( int i = 0; i < m_count; i++ )
                if ( m_elements[i].EqualFirst(Key) )
                    yield return m_elements[i];

        }

        public delegate bool Compare(Pair<T, TU> A, T Key, TU Value);

        public List<Pair<T, TU>> Findex(Compare func, T Key, TU Value) {
            List<Pair<T, TU>> _find = new List<Pair<T, TU>>();

            for ( int i = 0; i < m_count; i++ ) {
                if ( func(m_elements[i], Key, Value) )
                    _find.Add(m_elements[i]);
            }
            return _find;
        }
        public void Traverse(TraversMode mode, int startIndex, int endIndex, Action<Pair<T, TU>> func) {
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
                if ( m_elements[i].EqualFirst(Key) ) _find++;
            }

            return _find;
        }

        public UInt64 NumberOfElementsWithValue(TU Value) {
            UInt64 _find = 0;

            for ( int i = 0; i < m_count; i++ ) {
                if ( m_elements[i].EqualSecond(Value) ) _find++;
            }

            return _find;
        }

        public void Clear() {
            Array.Clear(m_elements);
            m_count = 0;
        }

        public bool Contains(Pair<T, TU> item) {
            return m_elements.Contains(item);
        }
        public bool ContainsKey(T Key) {
            return NumberOfElementsWithKey(Key) > 0;
        }

        public TU? Get(T Key) {
            var p = FindFirst(Key);
            if ( p.HasValue ) return p.Value.Second;
            throw new KeyNotFoundException();

        }

        public void CopyTo(Pair<T, TU>[] array, int arrayIndex) {
            m_elements.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Pair<T, TU>> GetEnumerator() {
            for(int i= 0; i < m_count; i++) {
                yield return m_elements[i];
            }
        }

        public bool Remove(Pair<T, TU> item) {
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
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

        public void RemoveAt(int pos) {
            return;
        }

        public void RemoveAt(int start, int iend) {
            return;
        }

        public Pair<T, TU>? FindFirst(T key) {
            for(int i = 0; i < m_count; i++ ) {
                if ( m_elements[i].EqualFirst(key) ) return m_elements[i];
            }
            return null;
        }

        public Pair<T, TU>? FindLast(T key) {
            for ( int i = m_count - 1; i >= 0; i-- ) {
                if ( m_elements[i].EqualFirst(key) )
                    return m_elements[i];
            }
            return null;
        }

        public Pair<T, TU>[] ToArray() {
            return m_elements;
        }
    }
}
