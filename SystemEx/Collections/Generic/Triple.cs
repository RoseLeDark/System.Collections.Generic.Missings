using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Collection.Generic {
    [Serializable]
#pragma warning disable CA1067 // "Object.Equals(object)" bei Implementierung von "IEquatable<T>" außer Kraft setzen
    public struct Triple<TT, TU, TW> :  IEquatable<Triple<TT, TU, TW>>, ITuple
#pragma warning restore CA1067 // "Object.Equals(object)" bei Implementierung von "IEquatable<T>" außer Kraft setzen
     {

        private TT m_first;
        private TU m_second;
        private TW m_third;

        public TT First {
            get => m_first;
            set => m_first = value;
        }
        public TU Second {
            get => m_second;
            set => m_second = value;
        }
        public TW Third {
            get => m_third;
            set => m_third = value;
        }
        public readonly int Count => 3;

        public Triple(TT first, TU second, TW third) {
            m_first = first;
            m_second = second;
            m_third = third;
        }

        public readonly bool Equals(Triple<TT, TU, TW> other) {
            return this.EqualFirst(other.First) && this.EqualSecond(other.Second) && this.EqualThird(other.Third);
        }
        public override string ToString() {
            return string.Create(null, stackalloc char[384], $"[{m_first}, {m_second}, {m_third}]");
        }

        public readonly bool EqualFirst(TT? other) {
            if ( this.m_first == null ) throw new ArgumentNullException(nameof(other), "All Quad have a first");
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

        public object? Get(int index) {
            if ( index < 0 || index >= Count )
#pragma warning disable CA2201 // Keine reservierten Ausnahmetypen auslösen
                throw new IndexOutOfRangeException("index");
#pragma warning restore CA2201 // Keine reservierten Ausnahmetypen auslösen

            switch ( index ) {
            case 0: return m_first;
            case 1: return m_second;
            case 2: 
            default:
                return m_third;
            }
        }

        public bool EqualFirst(object key) {
            if ( key is TT ) {
                TT _g = (TT)key;
                return EqualFirst(_g);
            }
            return false;
        }
    }
}
