using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SystemEx.Collection.Generic {
    [Serializable]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:Bezeichner müssen ein korrektes Suffix aufweisen", Justification = "<Ausstehend>")]
    public class TupleMap : IEnumerable<ITuple>, ICollection<ITuple>, IEnumerable, ITupleMap, ITraverse<ITuple>  {
        
        internal List<ITuple> m_elements;

        protected List<ITuple> Elements {  get { return m_elements; } set { m_elements = value; } }

        public int Count => m_elements.Count;

        public bool IsEmpty => Count == 0;

        public bool IsFull => false;

        public bool IsReadOnly => false;

        public ITuple? First {
            get {
                if ( m_elements.Count == 0 )
                    throw new InvalidOperationException("Map is empty");
                return m_elements[0]!;
            }
        }

        public ITuple? Last {
            get {
                if ( m_elements.Count == 0 ) 
                    throw new InvalidOperationException("Map is empty");
                return m_elements[m_elements.Count - 1]!;
            }
        }

        public ITuple this[int Key] {
            get => m_elements[Key];
            set => m_elements[Key] = value;
        }

        public int Size => Int32.MaxValue;

        public TupleMap() {
            m_elements = new List<ITuple>();
        }
        public TupleMap(IEnumerable<ITuple> elements) {
            m_elements = [.. elements]; ;
        }
        protected virtual bool Add(ITuple item, bool multi) {
            bool _ret = false;

            bool _contains = multi ? false : m_elements.Contains(item);

            if ( _contains == false ) {
                m_elements.Add(item);
                _ret = true;
            }

            return _ret;
        }
        public virtual void Add(ITuple item) {
            Add(item, false);
        }
        

        public virtual bool Insert(int pos, ITuple item) {
            m_elements.Insert(pos, item);
            return true;
        }
        public virtual bool InsertRange(int pos, IEnumerable<ITuple> items) {
            m_elements.InsertRange(pos, items);
            return true;
        }

        public virtual IEnumerable<ITuple> Find(object Key) {
            foreach ( var item in m_elements ) {
                if ( item.Get(0)!.Equals(Key) )
                    yield return item;
            }
        }

        public delegate bool Compare(ITuple A, object Key, object Value);

        public List<ITuple> Findex(Compare func, object Key, object Value) {
            List<ITuple> _find = new List<ITuple>();

            foreach ( var item in m_elements ) {
                if ( func(item, Key, Value) )
                    _find.Add(item);
            }
            return _find;
        }

        public void Traverse(TraversMode mode, int startIndex, int endIndex, Action<ITuple> func) {
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

        public UInt64 NumberOfElementsWithKey(object Key) {
            UInt64 _find = 0;

            foreach ( var item in m_elements ) {
                var obj = item.Get(0);

                if ( obj == null ) continue;
                if ( obj.Equals(Key) ) _find++;
            }

            return _find;
        }

        public UInt64 NumberOfElementsWithValue(object Value) {
            UInt64 _find = 0;

            foreach ( var item in m_elements ) {
                var obj = item.Get(1);

                if(obj == null ) continue;
                if ( obj.Equals(Value) )
                    continue;
                _find++;
            }

            return _find;
        }

        public void Clear() {
            m_elements.Clear();
        }

        public bool Contains(ITuple item) {
            return m_elements.Contains(item);
        }
        public bool ContainsKey(object Key) {
            foreach ( var p in m_elements ) {
                var obj = p.Get(0);

                if ( obj == null ) continue;
                if ( obj.Equals(Key) )
                    return true;
            }
            return false;
        }

        public object? Get(object Key) {
            ITuple? p = FindFirst(Key);
            if ( p != null) return p.Get(1);
            throw new KeyNotFoundException();

        }

        public void CopyTo(ITuple[] array, int arrayIndex) {
            m_elements.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ITuple> GetEnumerator() {
            return m_elements.GetEnumerator();
        }

        public bool Remove(ITuple item) {
            return m_elements.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
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

        public void RemoveAt(int pos) {
            m_elements.RemoveAt(pos);
        }

        public void RemoveAt(int start, int iend) {
            if ( start > iend ) return;

            m_elements.RemoveRange(start, iend - start);

        }

        public ITuple? FindFirst(object key) {
            foreach ( var p in m_elements ) {
                var _i = p.Get(0);
                if(_i == null) continue;

                if ( _i.Equals(key)) return p;   
            }
            return null;
        }

        public ITuple? FindLast(object key) {
            for ( int i = m_elements.Count - 1; i >= 0; i-- ) {
                if ( m_elements[i].Equals(key)  )
                    return m_elements[i];
            }
            return null;
        }
        public ITuple[] ToArray() {
            return [.. m_elements];
        }
    }
}
