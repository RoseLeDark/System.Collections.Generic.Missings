namespace SystemEx.Collection.Generic {
    public class TupleList : List<ITuple> {

        public TupleList() { }
        public TupleList(int size) : base(size) { }
        public TupleList(IEnumerable<ITuple> collection) : base(collection) { }

        public List<TU> GetAll<TU>() where TU : ITuple {
            List<TU> _ret = new List<TU>();

            foreach ( var item in this ) {
                if ( item == null ) continue;
                if ( item is TU ) _ret.Add((TU)item);
            }

            return _ret;
        }

        public List<ITuple> GetByCount(byte count) {
            List<ITuple> _ret = new List<ITuple>();

            foreach ( var item in this ) {
                if ( item == null ) continue;
                if ( item.Count == count ) _ret.Add(item);
            }

            return _ret;
        }
    }
}
