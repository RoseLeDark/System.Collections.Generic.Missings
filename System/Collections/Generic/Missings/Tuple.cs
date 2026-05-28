using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace System.Collections.Generic.Missings {
    public class Tuple<T> : ITuple where T : notnull {
        private T[] m_elements;

        public int Count => m_elements.Length;

        public T this[int index] {
            get => m_elements[index];
            set => Set(index, value);
        }
        public object? Get(int index) {
            if(index < 0 || index >= m_elements.Length)
                throw new IndexOutOfRangeException();

            return m_elements[index];
        }

        public void Set(int index, T value) {
            if ( index < 0 || index >= m_elements.Length ) return;

            m_elements[index] = value;
        }
        public Tuple() {
            m_elements = new T[5];
        }
        public Tuple(T[] elements) {
            m_elements = elements;
        }
        public Tuple(int size ) {
            m_elements = new T[size];
        }
    }
}
