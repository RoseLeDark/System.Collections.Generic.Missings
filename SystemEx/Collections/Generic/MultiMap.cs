using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Collection.Generic {
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TT"></typeparam>
    /// <typeparam name="TU"></typeparam>
    public class MultiMap<TT, TU> : Map<TT,TU> {

        public MultiMap() : base() { }
        public MultiMap(IEnumerable<Pair<TT, TU>> elements) : base(elements) {  }

        public override void Add(Pair<TT, TU> item) { m_elements.Add(item); }

    }
}
