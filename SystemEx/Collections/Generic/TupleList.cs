using System.Collections;

namespace SystemEx.Collection.Generic {
    public class TupleList : IEnumerable<ITuple>, IEnumerable {
        internal List<ITuple> m_elements;

        public int Count => m_elements.Count;

        public ITuple this[int index] { get => m_elements[index]; set => m_elements[index] = value; }

        public TupleList() { m_elements = new List<ITuple>(); }
        public TupleList(int size)  { m_elements = new List<ITuple>(size); }
        public TupleList(IEnumerable<ITuple> collection) { m_elements = new List<ITuple>(collection); }

        public List<TU> GetAll<TU>() where TU : ITuple {
            List<TU> _ret = new List<TU>();

            foreach ( var item in m_elements ) {
                if ( item == null ) continue;
                if ( item is TU ) _ret.Add((TU)item);
            }

            return _ret;
        }

        public List<ITuple> GetByCount(byte count) {
            List<ITuple> _ret = new List<ITuple>();

            foreach ( var item in m_elements ) {
                if ( item == null ) continue;
                if ( item.Count == count ) _ret.Add(item);
            }

            return _ret;
        }

        public virtual void Add(ITuple tuple) {
            m_elements.Add(tuple);
        }

        public virtual void AddRange(IEnumerable<ITuple> items) { 
            m_elements.AddRange(items); 
        }

        public void Clear() => m_elements.Clear();

        public bool Contains(ITuple item) => m_elements.Contains(item);

        public void CopyTo(ITuple[] array, int arrayIndex) => m_elements.CopyTo(array, arrayIndex);

        public virtual bool Remove(ITuple item) => m_elements.Remove(item);

        public IEnumerator<ITuple> GetEnumerator() => m_elements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int IndexOf(ITuple item)  => m_elements.IndexOf(item);

        public virtual void Insert(int index, ITuple item) => m_elements.Insert(index, item);

        public virtual void RemoveAt(int index) => m_elements.RemoveAt(index);
    }
}
