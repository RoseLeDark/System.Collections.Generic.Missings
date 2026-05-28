using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic.Missings {
    [Serializable]
    public class SortedMap<T, U> : IEnumerable<Pair<T, U>>, ICollection<Pair<T, U>>, IEnumerable, ISortedMap<T, U>
        where T : notnull
        where U : notnull {
        private List<Pair<T, U>> m_elements = new List<Pair<T, U>>();
        private SortFunc<T,U> m_sort;





        public int Count => m_elements.Count;

        public bool IsEmpty => Count == 0;

        public bool IsFull => false;

        public bool IsReadOnly => false;

        public SortFunc<T, U> SortFunctions {
            get => m_sort;
            set {
                m_sort = value;
                if ( AutoSort ) Sort();
            }
        }


        public bool AutoSort { get; set; }

        public Pair<T, U> First {
            get {
                if ( m_elements.Count == 0 )
                    throw new InvalidOperationException("Map is empty");
                return m_elements[0];
            }
        }

        public Pair<T, U> Last {
            get {
                if ( m_elements.Count == 0 )
                    throw new InvalidOperationException("Map is empty");
                return m_elements[m_elements.Count - 1];
            }
        }

        public int Size => Int32.MaxValue;

        public SortedMap(SortFunc<T, U> sort) {
            m_sort = sort;
            m_elements = new List<Pair<T, U>>();
            AutoSort = true;
        }

        public SortedMap(IMap<T, U> source, SortFunc<T, U> sort) {
            m_sort = sort;

            m_elements = [.. source.ToArray()];
            Sort();
        }

        public void Add(Pair<T, U> item) {
            m_elements.Add(item);
            if( AutoSort) Sort();
        }

        public Pair<T, U> this[int Key] {
            get => m_elements[Key];
            set => m_elements[Key] = value;
        }

        public void Insert(int pos, Pair<T, U> item) {
            m_elements.Insert(pos, item);
            Sort();
        }
        public void InsertRange(int pos, IEnumerable<Pair<T, U>> items) {
            m_elements.InsertRange(pos, items);
            if ( AutoSort ) Sort();
        }

        public IEnumerable<Pair<T, U>> Find(T Key) {
            foreach ( var item in m_elements ) {
                if ( item.Key!.Equals(Key) )
                    yield return item;
            }
        }

        public delegate bool Compare(Pair<T, U> A, T Key, U Value);

        public List<Pair<T, U>> Findex(Compare func, T Key, U Value) {
            List<Pair<T, U>> _find = new List<Pair<T, U>>();

            foreach ( var item in m_elements ) {
                if ( func(item, Key, Value) )
                    _find.Add(item);
            }
            return _find;
        }
        public void Traverse(TraversMode mode, int startIndex, int endIndex, Action<Pair<T, U>> func) {
            int start = Math.Max(startIndex, 0);
            int end = Math.Min(endIndex, m_elements.Count);

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

            foreach ( var item in m_elements ) {
                if ( item.Key == null ) continue;
                if ( item.Key.Equals(Key) ) _find++;
            }

            return _find;
        }

        public UInt64 NumberOfElementsWithValue(U Value) {
            UInt64 _find = 0;

            foreach ( var item in m_elements ) {
                if ( item.Value == null ) continue;
                if ( item.Value.Equals(Value) ) _find++;
            }

            return _find;
        }

        public void Clear() {
            m_elements.Clear();
        }

        public bool Contains(Pair<T, U> item) {
            return m_elements.Contains(item);
        }
        public bool ContainsKey(T Key) {
            foreach ( var p in m_elements ) {
                if ( p.Key!.Equals(Key) )
                    return true;
            }
            return false;
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
            return m_elements.GetEnumerator();
        }

        public bool Remove(Pair<T, U> item) {
            bool _ret = m_elements.Remove(item);

            if ( AutoSort )  Sort();

            return _ret;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        public bool TryGet(T Key, out U Value) {
            foreach ( var p in m_elements ) {
                if ( p.Key!.Equals(Key) ) {
                    Value = p.Value!;
                    return true;
                }
            }
            Value = default!;
            return false;
        }

        public void RemoveAt(int pos) {
            m_elements.RemoveAt(pos);
            if ( AutoSort ) Sort();
        }

        public void RemoveAt(int start, int end) {
            if ( start > end ) return;

            m_elements.RemoveRange(start, end - start);
            if ( AutoSort ) Sort();

        }

        public Pair<T, U>? FindFirst(T key) {
            foreach ( var p in m_elements ) {
                if ( p.EqualKeys(key) ) return p;
            }
            return null;
        }

        public Pair<T, U>? FindLast(T key) {
            for ( int i = m_elements.Count - 1; i >= 0; i-- ) {
                if ( m_elements[i].EqualKeys(key) )
                    return m_elements[i];
            }
            return null;
        }

        public void Sort() {
            for ( int i = 0; i < m_elements.Count - 1; i++ ) {
                for ( int j = i + 1; j < m_elements.Count; j++ ) {
                    if ( m_sort(m_elements[i], m_elements[j]) > 0 ) {
                        var tmp = m_elements[i];
                        m_elements[i] = m_elements[j];
                        m_elements[j] = tmp;
                    }
                }
            }
        }

        public Pair<T, U>[] ToArray() {
            return [.. m_elements];
        }

        public IMap<T, U> ToUnorderedMap() {
            Map<T, U> map = [.. m_elements];
            return map;
        }

        
    }
}
