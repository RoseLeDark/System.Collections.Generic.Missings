using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;

namespace System.Collections.Generic.Missings {
    public class TupleList : List<ITuple> {

        public TupleList() { }
        public TupleList(int size) : base(size) { }
        public TupleList(IEnumerable<ITuple> collection) : base(collection) { }

        public List<U> GetAll<U>() where U : ITuple {
            List<U> _ret = new List<U>();

            foreach ( var item in this ) {
                if ( item == null ) continue;
                if ( item is U ) _ret.Add((U)item);
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
