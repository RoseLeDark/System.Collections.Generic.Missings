using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Algorithms.Interfaces {
    public interface ISimpleCompare<T> {

        bool Compare ( T a, T b );
    }
}
