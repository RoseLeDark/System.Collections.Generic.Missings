
namespace System.Collections.Generic.Missings {
    public class Deque<T> {
        private T[] m_elements;
        private int m_count;

        public int Count => m_count;
        public int Size => m_elements.Length;
        public bool IsEmpty => m_count == 0;
        public bool IsFull => m_count == m_elements.Length;

        public T Front => m_elements[0];

        public T End => m_elements[m_count - 1];

        public Deque(int size) {
            m_elements = new T[size];
            m_count = 0;
        }

        public void PushBack(T value) {
            if ( IsFull ) return;
            m_elements[m_count++] = value;
        }

        public bool PopBack(ref T value) {
            if ( IsEmpty ) return false;
            value = m_elements[m_count - 1];
            m_count--;
            return true;
        }

        public void PushFront(T value) {
            if ( IsFull ) return;

            // alles nach rechts schieben
            for ( int i = m_count; i > 0; i-- )
                m_elements[i] = m_elements[i - 1];

            m_elements[0] = value;
            m_count++;
        }

        public bool PopFront(ref T value) {
            if ( IsEmpty ) return false;

            value = m_elements[0];

            // alles nach links schieben
            for ( int i = 0; i < m_count - 1; i++ )
                m_elements[i] = m_elements[i + 1];

            m_count--;
            return true;
        }

        public void Clear() {
            m_count = 0;
        }
    }

}
