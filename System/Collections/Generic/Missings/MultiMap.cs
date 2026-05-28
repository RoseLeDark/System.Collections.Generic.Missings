using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic.Missings {
    public class MultiMap<T, U> : Map<T,U>
        where T : notnull
        where U : notnull 
    {

        public MultiMap() : base() {

        }
        public MultiMap(IEnumerable<Pair<T, U>> elements) : base(elements) {

        }

        public override void Add(Pair<T, U> item) {
            m_elements.Add(item);
        }

    }
}
