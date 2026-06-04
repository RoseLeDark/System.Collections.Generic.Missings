using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Collection.Generic {

    public delegate int SortFunc<T, TU> (Pair<T, TU> a, Pair<T, TU> b) ;

    public interface ISortedMap<T, TU> : IMap<T, TU> {
        public SortFunc<T,TU> SortFunctions { get; set; }
        public bool AutoSort { get; set; }

        public void Sort();

        public IMap<T, TU> ToUnorderedMap();
    }
}
