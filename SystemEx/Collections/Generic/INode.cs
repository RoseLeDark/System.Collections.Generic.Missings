using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Collection.Generic {
    public interface INode<T> {
        public T Value { get; set; }

        public int? NChilds { get; }
        public int? NSiblings { get;  }

        
    }
}
