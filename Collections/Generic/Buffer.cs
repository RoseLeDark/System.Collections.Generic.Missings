

using System.Collections;

namespace SystemEx.Collections.Generic {
    public class RingBuffer<T> : ICollection<T>  {
        private Array<T> m_buffer;


        private int m_lenght;

        private int m_top;

        private int m_count;

        private int m_tail;
        public int Capacity => m_lenght;

        public int Tail { get { return m_tail; } protected set { m_tail = value; } }
        public int Head { get { return m_top; } protected set { m_top = value; } }
        public virtual bool IsEmpty => m_count == 0; 
        public virtual bool IsFull => m_count == m_lenght;
        int ICollection<T>.Count =>  m_count;
        bool ICollection<T>.IsReadOnly => false;

        
        public int Size => m_count; 



        public RingBuffer(int capacity)  {
            m_count = 0;
            m_lenght = capacity;
        }
        public void Clear() {
            m_count = 0;
            m_top = 0;
            m_tail = 0;
            m_buffer = new Array<T>(m_lenght);
        }

        public bool Contains(T item) {
            int bufferIndex;
            EqualityComparer<T> comparer;
            bool result;

            bufferIndex = m_top;
            comparer = EqualityComparer<T>.Default;
            result = false;

            for ( int i = 0; i < m_count; i++, bufferIndex++ ) {
                if ( bufferIndex == m_lenght ) {
                    bufferIndex = 0;
                }

                if ( item == null && m_buffer[bufferIndex] == null || m_buffer[bufferIndex] != null && comparer.Equals(m_buffer[bufferIndex], item) ) {
                    result = true;
                    break;
                }
            }

            return result;
        }

        public void CopyTo(T[] array) {
            this.CopyTo(array, 0);
        }

        public virtual void CopyTo(int index, T[] array, int arrayIndex, int count) {
            int bufferIndex;

            if ( count > m_count ) {
                throw new ArgumentOutOfRangeException(nameof(count), count, "The read count cannot be greater than the buffer size.");
            }

            bufferIndex = m_top + index;

            for ( int i = 0; i < count; i++, bufferIndex++, arrayIndex++ ) {
                if ( bufferIndex >= m_lenght ) {
                    bufferIndex -= m_lenght;
                }
                array[arrayIndex] = m_buffer[bufferIndex];
            }
        }

        public void CopyTo(T[] array, int arrayIndex) {
            this.CopyTo(0, array, arrayIndex, System.Math.Min(m_count, array.Length - arrayIndex));
        }

        public T[] Pop(int count) {
            T[] result;

            result = new T[count];

            this.Pop(result);

            return result;
        }

        public int Pop(T[] array) {
            return this.Pop(array, 0, array.Length);
        }

        public virtual int Pop(T[] array, int arrayIndex, int count) {
            int realCount;
            int dstIndex;

            realCount = System.Math.Min(count, m_count);
            dstIndex = arrayIndex;

            for ( int i = 0; i < realCount; i++, m_top++, dstIndex++ ) {
                if ( m_top == m_lenght ) {
                    m_top = 0;
                }

                array[dstIndex] = m_buffer[m_top];
            }

            if ( m_top == m_lenght ) {
                m_top = 0;
            }

            m_count -= realCount;

            return realCount;
        }

        public virtual T? Pop() {
            T item = default(T);

            if ( this.IsEmpty ) {

            } else {

                item = m_buffer[m_top];
                if ( ++m_top == m_lenght ) {
                    m_top = 0;
                }
                m_count--;
            }
            return item;
        }

       public IEnumerator<T> GetEnumerator() {
            int bufferIndex;

            bufferIndex = m_top;

            for ( int i = 0; i < m_count; i++, bufferIndex++ ) {
                if ( bufferIndex == m_lenght ) {
                    bufferIndex = 0;
                }

                yield return m_buffer[bufferIndex];
            }
        }

        public virtual T PopLast() {
            T item;
            int index;

            if ( this.IsEmpty ) {
                throw new InvalidOperationException("The buffer is empty.");
            }

            index = this.GetTailIndex(0);
            item = m_buffer[index];

            if ( --m_tail < 0 ) {
                m_tail = 0;
            }
            m_count--;

            return item;
        }

        public T[] PopLast(int count) {
            T[] result;

            result = new T[count];

            this.PopLast(result);

            return result;
        }

        public int PopLast(T[] array) {
            return this.PopLast(array, 0, array.Length);
        }

       public virtual int PopLast(T[] array, int arrayIndex, int count) {
            int realCount;

            realCount = System.Math.Min(count, m_count);

            for ( int i = realCount; i > 0; i-- ) {
                array[(arrayIndex + i) - 1] = this.PopLast();
            }

            return realCount;
        }

       public virtual T Peek() {
            T item;

            if ( this.IsEmpty ) {
                throw new InvalidOperationException("The buffer is empty.");
            }

            item = m_buffer[m_top];

            return item;
        }

        public virtual T[] Peek(int count) {
            T[] items;

            if ( this.IsEmpty ) {
                throw new InvalidOperationException("The buffer is empty.");
            }

            items = new T[count];
            this.CopyTo(items);

            return items;
        }

        public T PeekAt(int index) {
            if ( this.IsEmpty ) {
                throw new InvalidOperationException("The buffer is empty.");
            }

            if ( index < 0 || index >= m_count ) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return m_buffer[this.GetHeadIndex(index)];
        }

        public virtual T PeekLast() {
            T item;
            int index;

            if ( this.IsEmpty ) {
                throw new InvalidOperationException("The buffer is empty.");
            }

            index = this.GetTailIndex(0);
            item = m_buffer[index];

            return item;
        }

        public T[] PeekLast(int count) {
            T[] result;

            result = new T[count];

            this.PeekLast(result);

            return result;
        }

        public int PeekLast(T[] array) {
            return this.PeekLast(array, 0, array.Length);
        }

        public virtual int PeekLast(T[] array, int arrayIndex, int count) {
            int realCount;

            realCount = System.Math.Min(count, m_count);

            for ( int i = 0; i < realCount; i++ ) {
                array[arrayIndex + (realCount - (i + 1))] = m_buffer[this.GetTailIndex(i)];
            }

            return realCount;
        }

        public int Push(T[] array) {
            return this.Push(array, 0, array.Length);
        }

        public virtual int Push(T[] array, int arrayIndex, int count) {
            if ( count > m_lenght - m_count ) {
                throw new InvalidOperationException("The buffer does not have sufficient capacity to put new items.");
            }

            int i;
            for ( i = 0; i < count; i++ ) {
                this.Push(array[arrayIndex + i]);
            }
            return i;
        }

        public virtual void Push(T item) {
            if ( m_count == m_lenght ) {
                throw new InvalidOperationException("The buffer does not have sufficient capacity to put new items.");
            }

            m_buffer[m_tail] = item;

            m_tail++;
            if ( m_count == m_lenght ) {
                m_top++;
                if ( m_top >= m_lenght ) {
                    m_top -= m_lenght;
                }
            }

            if ( m_tail == m_lenght ) {
                m_tail = 0;
            }

            if ( m_count != m_lenght ) {
                m_count++;
            }
        }

        public void Next(int count) {
            m_top = this.GetHeadIndex(count);
        }

        public T[] ToArray() {
            T[] result;

            result = new T[m_count];

            this.CopyTo(result);

            return result;
        }

        void ICollection<T>.Add(T item) {
            this.Push(item);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return this.GetEnumerator();
        }

       IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        bool ICollection<T>.Remove(T item) {
            throw new NotSupportedException("Cannot remove items from collection.");
        }
        private int GetHeadIndex(int index) {
            int newIndex;

            newIndex = m_top + index;

            if ( newIndex >= m_lenght ) {
                newIndex -= m_lenght;
            }

            return newIndex;
        }

        private int GetTailIndex(int index) {
            int bufferIndex;

            bufferIndex = m_tail == 0
              ? m_count - (index + 1)
              : m_tail - (index + 1);

            if ( bufferIndex < 0 ) {
                bufferIndex += m_lenght;
            }

            return bufferIndex;
        }

    }
}
