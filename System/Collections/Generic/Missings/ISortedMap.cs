using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic.Missings {

    public delegate int SortFunc<T, U> (Pair<T, U> a, Pair<T, U> b) where T : notnull where U : notnull;

    public interface ISortedMap<T, U> : IMap<T, U>
        where T : notnull
        where U : notnull {

        

        public SortFunc<T,U> SortFunctions { get; set; }
        public bool AutoSort { get; set; }

        public void Sort();

        public IMap<T, U> ToUnorderedMap();
    }
}
