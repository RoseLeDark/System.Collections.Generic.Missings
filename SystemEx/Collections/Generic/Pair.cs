namespace SystemEx.Collection.Generic {
    [Serializable]
    public struct Pair<T, TU> : IPair<T, TU>
 {

        private T m_key;
        private TU m_value;

        public T First {
            get => m_key;
            set => m_key = value;
        }
        public TU Second {
            get => m_value;
            set => m_value = value;
        }

        public  readonly int Count => 2;

        public Pair(T first, TU second) {
            m_key = first;
            m_value = second;
        }

        public  bool Equals(Pair<T, TU> other) {
            return this.EqualFirst(other.First) && this.EqualSecond(other.Second);
        }
        public override string ToString() {
            return string.Create(null, stackalloc char[256], $"[{m_key}, {m_value}]");
        }

        public readonly bool EqualFirst(T other) => this.m_key!.Equals(other);
        public readonly bool EqualSecond(TU other) => this.m_value!.Equals(other);

        public readonly object? Get(int index) {
            if(index < 0 || index >= Count) 
                throw new ArgumentOutOfRangeException(nameof(index));

            if ( index == 0 ) return m_key;
            else return m_value;
        }

        bool ITuple.EqualFirst(object key) {
            if(key is T) {
                T _g = (T)key;
                return EqualFirst(_g );
            }
            return false;
        }
    }
}
