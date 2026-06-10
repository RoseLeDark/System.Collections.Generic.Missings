using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Collection.Generic {
    public interface IMap {
        public int Count { get; } // elements
        public bool IsReadOnly { get; }
        public bool IsEmpty { get; }

        public bool IsFull { get; }

        public int Size { get; } // gesammt size

        public void RemoveAt(int pos);
        public void RemoveAt(int start, int iend);

        public void Clear();
    }

    public interface IMap<T, TU> : IMap {
        public Pair<T, TU>? First { get; }
        public Pair<T, TU>? Last { get;  }

        public void Add(Pair<T, TU> item);
        public bool Remove(Pair<T, TU> item);

        public bool Insert(int pos, Pair<T, TU> item);
        

        public bool InsertRange(int pos, IEnumerable<Pair<T, TU>> items);


        public IEnumerable<Pair<T, TU>> Find(T Key);

        public void Traverse(TraversMode mode, int startIndex, int endIndex, Action<Pair<T, TU>> func);

        public Pair<T, TU>? FindFirst(T key);
        public Pair<T, TU>? FindLast(T key);

        public Pair<T, TU>[] ToArray();
    }
}
