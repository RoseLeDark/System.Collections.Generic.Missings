using System;
using System.Collections.Generic;
using System.Text;
using SystemEx.Utils;

namespace SystemEx.Collection.Generic.Interfaces {

    public interface ICompared<in T> where T : allows ref struct  {
        CompareResult Compare(T? x, T? y);
    }
}
