using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic.Missings {
    public interface ITuple {
        int Count { get; }
        object? Get(int index);
    }

}
