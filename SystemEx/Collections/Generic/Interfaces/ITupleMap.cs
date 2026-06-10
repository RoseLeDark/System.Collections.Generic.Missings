using System;
using System.Collections.Generic;
using System.Text;
using SystemEx.Collection.Generic;

namespace SystemEx.Collection.Generic {
    public interface ITupleMap : IMap {
        public ITuple? First { get; }
        public ITuple? Last { get; }

        public bool InsertRange(int pos, IEnumerable<ITuple> items);

        public IEnumerable<ITuple> Find(object Key);
        public void Traverse(TraversMode mode, int startIndex, int endIndex, Action<ITuple> func);

        public ITuple? FindFirst(object key);
        public ITuple? FindLast(object key);

        public ITuple[] ToArray();
    }
}
