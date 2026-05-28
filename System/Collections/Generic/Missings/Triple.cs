using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic.Missings {
    [Serializable]
    public struct Triple<T, U, W> :  IEquatable<Triple<T, U, W>>, ITuple
        where T : notnull
        where U : notnull
        where W : notnull {

        private T m_first;
        private U m_second;
        private W m_third;

        public T First {
            get => m_first;
            set => m_first = value;
        }
        public U Second {
            get => m_second;
            set => m_second = value;
        }
        public W Third {
            get => m_third;
            set => m_third = value;
        }
        public readonly int Count => 3;

        public Triple(T first, U second, W third) {
            m_first = first;
            m_second = second;
            m_third = third;
        }

        public readonly bool Equals(Triple<T, U, W> other) {
            return this.EqualFirst(other.First) && this.EqualSeconds(other.Second) && this.EqualThird(other.Third);
        }
        public override string ToString() {
            return string.Create(null, stackalloc char[384], $"[{m_first}, {m_second}, {m_third}]");
        }

        public readonly bool EqualFirst(T other) => this.m_first.Equals(other);
        public readonly bool EqualSeconds(U other) => this.m_second.Equals(other);

        public readonly bool EqualThird(W other) => this.m_third.Equals(other);

        public object? Get(int index) {
            if ( index < 0 || index >= Count )
                throw new ArgumentOutOfRangeException("index");

            switch ( index ) {
            case 0: return m_first;
            case 1: return m_second;
            case 2: 
            default:
                return m_third;
            }
        }
    }
}
