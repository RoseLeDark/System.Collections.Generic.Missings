using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Collection.Generic {
    public class StarNode<T> : Node<T> {
        public StarNode(T value) : base(value) {
        }

        public void AddChild(Node<T> child) {
            m_pChilds.Add(child); 
        }

        public IEnumerable<Node<T>> Children => m_pChilds; 


    }

}
