using System;
using System.Collections.Generic;
using System.Text;
using SystemEx.Collections.Generic;

namespace SystemEx.Collections.Model {

    public delegate bool TraverseFunc<T> ( BinaryTree<T> n, bool left, long depth );


    public class Tree<T, TRE> : GenericNode<T>  {
        /// <summary>
        /// 
        /// </summary>
        protected FixedArray<TRE?> m_pElemtents;

        public int Childs { get;  }

        protected Tree (int childs, T? value) 
            : base(value) {
            m_pElemtents = new FixedArray<TRE?>(childs);
        }
    }
}
