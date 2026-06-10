using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Collection.Generic {
    public class SortedMultiTupleMap : SortedTupleMap {
        public SortedMultiTupleMap(SortTupleFunc sort) : base(sort) { }
        public SortedMultiTupleMap(ITupleMap source, SortTupleFunc sort) : base(source, sort) { }

        public override void Add(ITuple item) {
            m_elements.Add(item);
            if ( AutoSort ) Sort();
        }
    }
}
