using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Collection.Generic {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1711:Bezeichner dürfen kein falsches Suffix aufweisen", Justification = "<Ausstehend>")]
    public class BinQueue<T> {
        private Deque<T> m_deque;

        public int Count => m_deque.Count;

        public int Size => m_deque.Size; // 2

        public bool IsEmpty => m_deque.IsEmpty;
        public bool IsFull => m_deque.IsFull;

        public T Front => m_deque.Front;

        public T End => m_deque.End;


        public BinQueue() {
            m_deque = new Deque<T>(2);
        }

        public void Enqueue(T value) {
            if ( IsFull ) {
                T dummy = default!;
                m_deque.PopFront(ref dummy);
            }
            m_deque.PushBack(value);
        }
        public bool Dequeue(ref T value) => m_deque.PopFront(ref value);

        public void Clear() {
            m_deque.Clear();
        }
    }
}
