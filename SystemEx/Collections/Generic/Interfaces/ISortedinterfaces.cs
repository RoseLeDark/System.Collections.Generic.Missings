using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SystemEx.Collection.Generic.Interfaces;
using SystemEx.Utils;

namespace SystemEx.Collection.Generic {

    public delegate CompareResult SortFunc<T, TU> (Pair<T, TU> a, Pair<T, TU> b) ;
    public delegate CompareResult SortTupleFunc(ITuple a, ITuple b);

    public delegate CompareResult SortObjectFunc<T>(T a, T b);

    public interface ISortedArray<T> : IArray<T> {
        public SortObjectFunc<T> SortFunctions { get; set; }
        public ICompared<T>? Comparer { get; set;  }
        public bool AutoSort { get; set; }

        public void Sort();

        public IArray<T> ToUnorderedArray();
    }

    public interface ISortedMap<T, TU> : IMap<T, TU> {
        public SortFunc<T,TU> SortFunctions { get; set; }

        public ICompared<IPair<T, TU>>? Comparer {  get; set; }
        public bool AutoSort { get; set; }

        public void Sort();

        public IMap<T, TU> ToUnorderedMap();
    }

    public interface ISortedTupleMap : ITupleMap {
        public SortTupleFunc SortFunctions { get; set; }

        public ICompared<ITuple>? Comparer { get; set; }
        public bool AutoSort { get; set; }

        public void Sort();

        public ITupleMap ToUnorderedMap();
    }
}
