using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Collection.Generic {


    public enum TraversMode {
        Forwards,
        Backwards,
    }

    public interface ITraverse<T> {
        public void Traverse(TraversMode mode, int startIndex, int endIndex, Action<T> func);
    }
}
