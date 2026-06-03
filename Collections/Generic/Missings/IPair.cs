using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic.Missings {
    public interface IPair<T, TU> {
        public T First { get; set;  }
        public TU Second { get; set; }

        public bool EqualFirst(T other);
        public bool EqualSecond(TU other);
    }
}
