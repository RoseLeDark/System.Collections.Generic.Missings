using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Collection.Generic {
    public interface IPair<T, TU> : ITuple {
        public T First { get; set;  }
        public TU Second { get; set; }

        public bool EqualFirst(T other);
        public bool EqualSecond(TU other);
    }
}
