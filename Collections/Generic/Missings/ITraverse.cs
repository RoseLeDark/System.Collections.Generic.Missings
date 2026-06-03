using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic.Missings {


    public enum TraversMode {
        Forwards,
        Backwards,
    }

    public interface ITraverse<T> {
        public void Traverse(TraversMode mode, int startIndex, int endIndex, Action<T> func);
    }
}
