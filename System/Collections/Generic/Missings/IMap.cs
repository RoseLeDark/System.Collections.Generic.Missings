using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic.Missings {
    public enum TraversMode {
        Forwards,
        Backwards,
    }

    public interface IMap<T, U>
        where T : notnull
        where U : notnull 
    {
        public int Count { get; } // elements
        public bool IsReadOnly { get; }
        public bool IsEmpty { get; }

        public bool IsFull { get; }

        public int Size { get; } // gesammt size

        public Pair<T, U> First { get; }
        public Pair<T, U> Last { get;  }


        public void Clear();

        public void Add(Pair<T, U> item);
        public bool Remove(Pair<T, U> item);

        public void Insert(int pos, Pair<T, U> item);
        public void RemoveAt(int pos);
        public void RemoveAt(int start, int end);

        public void InsertRange(int pos, IEnumerable<Pair<T, U>> items);


        public IEnumerable<Pair<T, U>> Find(T Key);

        

        public void Traverse(TraversMode mode, int startIndex, int endIndex, Action<Pair<T, U>> func);

        public Pair<T, U>? FindFirst(T key);
        public Pair<T, U>? FindLast(T key);

        public Pair<T, U>[] ToArray();

    }
}
