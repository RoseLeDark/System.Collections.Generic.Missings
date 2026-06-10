using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Collection.Generic {
    public class SortedMultiMap<TT, TU> : SortedMap<TT, TU> {
        public SortedMultiMap(SortFunc<TT, TU> sort) : base(sort) { }
        public SortedMultiMap(IMap<TT, TU> source, SortFunc<TT, TU> sort) : base(source, sort) { }

        public override void Add(Pair<TT, TU> item) {
            m_elements.Add(item);
            if ( AutoSort ) Sort();
        }
    }
}
