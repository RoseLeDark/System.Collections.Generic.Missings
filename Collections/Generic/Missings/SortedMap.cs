using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic.Missings {
    [Serializable]
#pragma warning disable CA1710 // Bezeichner müssen ein korrektes Suffix aufweisen
    public class SortedMap<T, TU> : IEnumerable<Pair<T, TU>>, ICollection<Pair<T, TU>>, IEnumerable, ISortedMap<T, TU>
#pragma warning restore CA1710 // Bezeichner müssen ein korrektes Suffix aufweisen
        where T : notnull
        where TU : notnull {
        private List<Pair<T, TU>> m_elements = new List<Pair<T, TU>>();
        private SortFunc<T,TU> m_sort;





        public int Count => m_elements.Count;

        public bool IsEmpty => Count == 0;

        public bool IsFull => false;

        public bool IsReadOnly => false;

        public SortFunc<T, TU> SortFunctions {
            get => m_sort;
            set {
                m_sort = value;
                if ( AutoSort ) Sort();
            }
        }


        public bool AutoSort { get; set; }

        public Pair<T, TU>? First {
            get {
                if ( m_elements.Count == 0 )
                    throw new InvalidOperationException("Map is empty");
                return m_elements[0];
            }
        }

        public Pair<T, TU>? Last {
            get {
                if ( m_elements.Count == 0 )
                    throw new InvalidOperationException("Map is empty");
                return m_elements[m_elements.Count - 1];
            }
        }

        public int Size => Int32.MaxValue;


        public SortedMap(SortFunc<T, TU> sort) {
            m_sort = sort;
            m_elements = new List<Pair<T, TU>>();
            AutoSort = true;
        }

        public SortedMap(IMap<T, TU> source, SortFunc<T, TU> sort) {
            m_sort = sort;

            m_elements = [.. source.ToArray()];
            Sort();
        }

        public void Add(Pair<T, TU> item) {
            m_elements.Add(item);
            if( AutoSort) Sort();
        }

        public Pair<T, TU> this[int Key] {
            get => m_elements[Key];
            set => m_elements[Key] = value;
        }

        public bool Insert(int pos, Pair<T, TU> item) {
            m_elements.Insert(pos, item);
            Sort();

            return true; 
        }
        public bool InsertRange(int pos, IEnumerable<Pair<T, TU>> items) {
            m_elements.InsertRange(pos, items);
            if ( AutoSort ) Sort();

            return true;
        }

        public IEnumerable<Pair<T, TU>> Find(T Key) {
            foreach ( var item in m_elements ) {
                if ( item.First!.Equals(Key) )
                    yield return item;
            }
        }

        public delegate bool Compare(Pair<T, TU> A, T Key, TU Value);

        public List<Pair<T, TU>> Findex(Compare func, T Key, TU Value) {
            List<Pair<T, TU>> _find = new List<Pair<T, TU>>();

            foreach ( var item in m_elements ) {
                if ( func(item, Key, Value) )
                    _find.Add(item);
            }
            return _find;
        }
        public void Traverse(TraversMode mode, int startIndex, int endIndex, Action<Pair<T, TU>> func) {
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
                if ( item.First == null ) continue;
                if ( item.First.Equals(Key) ) _find++;
            }

            return _find;
        }

        public UInt64 NumberOfElementsWithValue(TU Value) {
            UInt64 _find = 0;

            foreach ( var item in m_elements ) {
                if ( item.Second == null ) continue;
                if ( item.Second.Equals(Value) ) _find++;
            }

            return _find;
        }

        public void Clear() {
            m_elements.Clear();
        }

        public bool Contains(Pair<T, TU> item) {
            return m_elements.Contains(item);
        }
        public bool ContainsKey(T Key) {
            foreach ( var p in m_elements ) {
                if ( p.First!.Equals(Key) )
                    return true;
            }
            return false;
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
            return m_elements.GetEnumerator();
        }

        public bool Remove(Pair<T, TU> item) {
            bool _ret = m_elements.Remove(item);

            if ( AutoSort )  Sort();

            return _ret;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        public bool TryGet(T Key, out TU Value) {
            foreach ( var p in m_elements ) {
                if ( p.First!.Equals(Key) ) {
                    Value = p.Second!;
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

        public void RemoveAt(int start, int iend) {
            if ( start > iend ) return;

            m_elements.RemoveRange(start, iend - start);
            if ( AutoSort ) Sort();

        }

        public Pair<T, TU>? FindFirst(T key) {
            foreach ( var p in m_elements ) {
                if ( p.EqualFirst(key) ) return p;
            }
            return null;
        }

        public Pair<T, TU>? FindLast(T key) {
            for ( int i = m_elements.Count - 1; i >= 0; i-- ) {
                if ( m_elements[i].EqualFirst(key) )
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

        public Pair<T, TU>[] ToArray() {
            return [.. m_elements];
        }

        public IMap<T, TU> ToUnorderedMap() {
            Map<T, TU> map = [.. m_elements];
            return map;
        }

        
    }
}
