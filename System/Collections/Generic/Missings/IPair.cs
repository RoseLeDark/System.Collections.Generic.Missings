using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic.Missings {
    public interface IPair<T, U>
        where T : notnull
        where U : notnull 
    {
        public T Key { get; set;  }
        public U Value { get; set; }

        public bool EqualKeys(T other);
        public bool EqualValues(U other);
    }
}
