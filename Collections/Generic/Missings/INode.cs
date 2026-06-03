using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic.Missings {
    public interface INode<T> {
        public T Value { get; set; }

        public int? NChilds { get; }
        public int? NSiblings { get;  }

        
    }
}
