using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Collection.Generic {
    public class MultiTupleMap : TupleMap {
        public MultiTupleMap() : base() {  }
        public MultiTupleMap(IEnumerable<ITuple> elements) : base(elements) { }

        public override void Add(ITuple item) { m_elements.Add(item); }
    }
}
