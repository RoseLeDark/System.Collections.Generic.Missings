using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic.Missings {
    [Serializable]
    public struct Pair<T, U> : IPair<T, U>, IEquatable<Pair<T, U>>, ITuple
        where T : notnull
        where U : notnull {

        private T m_key;
        private U m_value;

        public T Key {
            get => m_key;
            set => m_key = value;
        }
        public U Value {
            get => m_value;
            set => m_value = value;
        }

        public  readonly int Count => 2;

        public Pair(T key, U Value) {
            m_key = key;
            m_value = Value;
        }

        public readonly bool Equals(Pair<T, U> other) {
            return this.EqualKeys(other.Key) && this.EqualValues(other.Value);
        }
        public override string ToString() {
            return string.Create(null, stackalloc char[256], $"[{m_key}, {m_value}]");
        }

        public readonly bool EqualKeys(T other) => this.m_key.Equals(other);
        public readonly bool EqualValues(U other) => this.m_value.Equals(other);

        public object? Get(int index) {
            if(index < 0 || index >= Count) 
                throw new ArgumentOutOfRangeException("index");

            if ( index == 0 ) return m_key;
            else return m_value;
        }
    }
}
