using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Collection.Generic {
    public interface IArray<T> {
        public int Size {  get; }
        public T Front { get; }
        public T Back { get; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Bezeichner dürfen nicht mit Schlüsselwörtern übereinstimmen", Justification = "<Ausstehend>")]
        public T Next { get; }

        public bool IsFull { get; }

        public bool IsEmpty { get; }

        public bool IsFixed { get; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Bezeichner dürfen nicht mit Schlüsselwörtern übereinstimmen", Justification = "<Ausstehend>")]
        public bool Get(int index, ref T item);
        public bool Remove();

        

        public void Insert(int pos, T item);

        public void InsertRange(int pos, IEnumerable<T> items);

        public UInt64 NumberOfElements(T Key);

        public void Traverse(TraversMode mode, int startIndex, int endIndex, Action<T> func);

        public T? FindFirst(T key);

        public T? FindLast(T key);

        public bool TryGet(T Key, out int index);

        public T[] ToArray();

    }

    public interface IDynamicArray<T> : IArray<T> {
        public bool Resize(int size);

        public int GrowSize { get; set; }

        public bool AutoGrow { get; set; }
    }
}
