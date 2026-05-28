namespace System.Collections.Generic.Missings {
    public class Queue<T> {
        private Deque<T> m_deque;

        public int Size => m_deque.Size; // 2

        public bool IsEmpty => m_deque.IsEmpty;
        public bool IsFull => m_deque.IsFull;

        public T Front => m_deque.Front;

        public Queue(int size) {
            m_deque = new Deque<T>(size);
        }
        public Queue(Deque<T> d) {
            m_deque = d;
        }
        public void Enqueue(T value) => m_deque.PushBack(value);
        public bool Dequeue(ref T value) => m_deque.PopFront(ref value);
        

        public void Clear() {
            m_deque.Clear();
        }
    }

}
