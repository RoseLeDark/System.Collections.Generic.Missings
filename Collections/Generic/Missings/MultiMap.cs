using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic.Missings {
    public class MultiMap<TT, TU> : Map<TT,TU>
        where TT : notnull
        where TU : notnull 
    {

        public MultiMap() : base() {

        }
        public MultiMap(IEnumerable<Pair<TT, TU>> elements) : base(elements) {

        }

        public override void Add(Pair<TT, TU> item) {
            m_elements.Add(item);
        }

    }
}
