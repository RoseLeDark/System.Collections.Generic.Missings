using static System.Net.WebRequestMethods;

namespace SystemEx.Collection.Generic {
    [Serializable]
    public struct Quad<TT, TU, TW, TJ> :  ITuple {

        private TT m_first;
        private TU m_second;
        private TW m_third;
        private TJ m_fourth;

        public TT First {
            readonly get => m_first;
            set => m_first = value;
        }
        public TU Second {
            readonly get => m_second;
            set => m_second = value;
        }
        public TW Third {
            readonly get => m_third;
            set => m_third = value;
        }
        public TJ Fourth {
            readonly get => m_fourth;
            set => m_fourth = value;
        }
        public readonly int Count => 4;

        public Quad(TT first, TU second, TW third, TJ fourth) {
            m_first = first;
            m_second = second;
            m_third = third;
            m_fourth = fourth;
        }

        public bool Equals(Quad<TT, TU, TW, TJ> other) {
            return this.EqualFirst(other.First) && this.EqualSecond(other.Second) && this.EqualThird(other.Third) && this.EqualFourth(other.Fourth);
        }
        public override string ToString() {
            return string.Create(null, stackalloc char[512], $"[{m_first}, {m_second}, {m_third}, {m_fourth}]");
        }

        public readonly bool EqualFirst(TT? other) {
            if(this.m_first == null) throw new ArgumentNullException(nameof(other), "All Quad have a first");
            return this.m_first.Equals(other);
        }

        public readonly bool EqualSecond(TU? other) {
            if ( this.m_second == null ) throw new ArgumentNullException(nameof(other), "All Quad have a second");
            return this.m_second.Equals(other);
        }

        public readonly bool EqualThird(TW? other) {
            if ( this.m_third == null ) throw new ArgumentNullException(nameof(other), "All Quad have a third");
            return this.m_third.Equals(other);
        }

        public readonly bool EqualFourth(TJ? other) {
            if ( this.m_fourth == null ) throw new ArgumentNullException(nameof(other), "All Quad have a fourth");
            return this.m_fourth.Equals(other);
        }

        public readonly object? Get(int index) {
            if ( index < 0 || index >= Count )
#pragma warning disable CA2201 // Keine reservierten Ausnahmetypen auslösen
                throw new IndexOutOfRangeException("index");
#pragma warning restore CA2201 // Keine reservierten Ausnahmetypen auslösen

            switch (index) {
            case 0: return m_first;
            case 1: return m_second;
            case 2: return m_third;
            case 3: return m_fourth;
            default: return null; // wird nie erreicht aber save ist save
            }
        }

        bool ITuple.EqualFirst(object key) {
            return EqualFirst((TT)key);
        }
    }
}
