using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace System.Collections.Generic.Missings {
    [Serializable]
    public struct Quad<T, U, W, J> : IEquatable<Quad<T, U, W, J>>, ITuple
        where T : notnull
        where U : notnull
        where W : notnull
        where J : notnull {

        private T m_first;
        private U m_second;
        private W m_third;
        private J m_fourth;

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
        public J Fourth {
            get => m_fourth;
            set => m_fourth = value;
        }
        public readonly int Count => 4;

        public Quad(T first, U second, W third, J fourth) {
            m_first = first;
            m_second = second;
            m_third = third;
            m_fourth = fourth;
        }

        public readonly bool Equals(Quad<T, U, W, J> other) {
            return this.EqualFirst(other.First) && this.EqualSeconds(other.Second) && this.EqualThird(other.Third) && this.EqualFourth(other.Fourth);
        }
        public override string ToString() {
            return string.Create(null, stackalloc char[512], $"[{m_first}, {m_second}, {m_third}, {m_fourth}]");
        }

        public readonly bool EqualFirst(T other) => this.m_first.Equals(other);
        public readonly bool EqualSeconds(U other) => this.m_second.Equals(other);

        public readonly bool EqualThird(W other) => this.m_third.Equals(other);

        public readonly bool EqualFourth(J other) => this.m_fourth.Equals(other);

        public object? Get(int index) {
            if ( index < 0 || index >= Count )
                throw new ArgumentOutOfRangeException("index");

            switch(index) {
            case 0: return m_first;
            case 1: return m_second;
            case 2: return m_third;
            case 3: 
            default:
                return m_fourth;
            }
        }
    }
}
