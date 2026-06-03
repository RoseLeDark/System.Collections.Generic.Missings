using System;
using System.Collections.Generic;
using System.Text;


namespace System.Collections.Generic.Missings {
    public enum TraversOrder {
        Preorder,
        Inorder,
        Postorder,
        ListOrder,
        ReservListOrder
    }

    public class Node<T> : INode<T> {
        const int NEXT = 1;
        const int PREV = 0;

        public  const byte MINSIZE = 2;

        internal  Array<Node<T>> m_pChilds;
        internal  Array<Node<T>> m_pSiblings;

        public Node<T> Next {  get => m_pChilds[NEXT];  set => m_pChilds[NEXT] = value; } 
        public Node<T> Prev {  get => m_pChilds[PREV];  set => m_pChilds[PREV] = value; } 

        public T Value { get; set; }

        public int? NChilds { get { return m_pChilds == null ? 0 : m_pChilds.Size; } }
        public int? NSiblings { get { return m_pSiblings == null ? 0 : m_pSiblings.Size; } }

        public bool HasNext => m_pChilds[NEXT] != this;

        public bool HasPrev => m_pChilds[PREV] != this;

        public int? NodeSize { get => NChilds + NSiblings;  }


        public Node(T val ) {
            Value = val;
            
            m_pChilds = new Array<Node<T>>(MINSIZE);
            m_pSiblings = new Array<Node<T>>(MINSIZE);

            m_pChilds[PREV] = m_pChilds[NEXT] = this;
        }

        public Node(int nChilds, int nSiblings, T iValue) {
            m_pChilds = new Array<Node<T>>(nChilds);
            m_pSiblings = new Array<Node<T>>(nSiblings);
            Value = iValue;

            m_pChilds[PREV] = m_pChilds[NEXT] = this;
        }

        public Node(T val, Array<Node<T>> pChilds, Array<Node<T>> pSiblings) {
            Value = val;
            m_pChilds = pChilds;
            m_pSiblings = pSiblings;
        }

        public Node<T> Root() {
            Node<T> temp =  this;

            while(temp.HasPrev) {

                temp = temp.Prev;
            } ;

            return temp;
        }

        public Node<T> Last() {
            Node<T> temp =  this;

            while ( temp.HasNext ) {

                temp = temp.Next;
            } ;

            return temp;
        }

        public Node<T> GetAt(int index, out int r) {
            Node<T> temp = this;
            int n = index;

            if ( n > 0 ) {
                while ( n != 0 && temp.HasNext ) {
                    temp = temp.Next;
                    n--;
                }
            } else if ( n < 0 ) {
                while ( n != 0 && temp.HasPrev ) {
                    temp = temp.Prev;
                    n++;
                }
            }

            r = n;
            return temp;
        }

        public Node<T> Insert(Node<T> pNext) {
            // detach this if already linked
            if ( HasNext || HasPrev )
                remove();

            // now insert
            m_pChilds[NEXT] = pNext;
            m_pChilds[PREV] = pNext.Prev;
            pNext.Prev.m_pChilds[NEXT] = this;
            pNext.m_pChilds[PREV] = this;

            return this;
        }

        public void remove() {
            m_pChilds[NEXT].m_pChilds[PREV] = m_pChilds[PREV];
            m_pChilds[PREV].m_pChilds[NEXT] = m_pChilds[NEXT];

#if TRACE
            // optional: isolieren
            m_pChilds[NEXT] = this;
            m_pChilds[PREV] = this;
#endif
        }

        public void Splice(ref Node<T> first, ref Node<T> last) {
            last.m_pChilds[PREV].m_pChilds[NEXT] = this;
            first.m_pChilds[PREV].m_pChilds[NEXT] = last;
            this.m_pChilds[PREV].m_pChilds[NEXT] = first;

            Node<T> pTemp = this.m_pChilds[PREV];
            this.m_pChilds[PREV] = last.m_pChilds[PREV];
            last.m_pChilds[PREV] = first.m_pChilds[PREV];
            first.m_pChilds[PREV] = pTemp;
        }
    
  
        public void Reverse() {
            Node<T>  pNode = this;

            do {
                if ( pNode != null ) {
                    Node<T> pTemp = pNode.m_pChilds[NEXT];
                    pNode.m_pChilds[NEXT] = pNode.m_pChilds[PREV];
                    pNode.m_pChilds[PREV] = pTemp;
                    pNode = pNode.m_pChilds[PREV];
                }
            } while ( pNode != this );
        }
        public void InsertRagen(ref Node<T> pFirst, ref Node<T> pFinal) {
            m_pChilds[PREV].m_pChilds[NEXT] = pFirst; pFirst.m_pChilds[PREV] = m_pChilds[PREV];
            m_pChilds[PREV] = pFinal; pFinal.m_pChilds[NEXT] = this;
        }

        public void Swap(ref Node<T> other) {
            if ( this == other )
                return;

            // Backup A
            var aPrev = this.Prev;
            var aNext = this.Next;
            var aValue = this.Value;

            // Backup B
            var bPrev = other.Prev;
            var bNext = other.Next;
            var bValue = other.Value;

            // Copy B into A
            this.Prev = bPrev;
            this.Next = bNext;
            this.Value = bValue;

            // Copy A into B
            other.Prev = aPrev;
            other.Next = aNext;
            other.Value = aValue;

            // Fix neighbors of A
            this.Prev.Next = this;
            this.Next.Prev = this;

            // Fix neighbors of B
            other.Prev.Next = other;
            other.Next.Prev = other;
        }
        public ulong Distance() {
            ulong _temp = 0;
            while(HasPrev) {
                _temp++;

            }
            return _temp;
        }
        public virtual void Travers(TraversOrder order, Action<Node<T>> action) {
            switch ( order ) {
            case TraversOrder.ListOrder:
                TraversListForward(action);
                break;

            case TraversOrder.ReservListOrder:
                TraversListBackward(action);
                break;

            case TraversOrder.Preorder:
                TraversPreorder(this, action);
                break;

            case TraversOrder.Inorder:
                break;

            case TraversOrder.Postorder:
                TraversPostorder(this, action);
                break;
            }
        }
        private void TraversListForward(Action<Node<T>> action) {
            var temp = this;
            while ( temp.HasNext ) {
                action(temp);
                temp = temp.Next;
            }
            action(temp); // letztes Element
        }

        private void TraversListBackward(Action<Node<T>> action) {
            var temp = this;
            while ( temp.HasPrev ) {
                action(temp);
                temp = temp.Prev;
            }
            action(temp); // erstes Element
        }
        private static void TraversPreorder(Node<T> node, Action<Node<T>> action) {
            if ( node == null ) return;

            action(node);

            // Childs
            for ( int i = 2; i < node.m_pChilds.Size; i++ )
                TraversPreorder(node.m_pChilds[i], action);

            // Siblings
            for ( int i = 0; i < node.m_pSiblings.Size; i++ )
                TraversPreorder(node.m_pSiblings[i], action);
        }
        private static void TraversPostorder(Node<T> node, Action<Node<T>> action) {
            if ( node == null ) return;

            for ( int i = 2; i < node.m_pChilds.Size; i++ )
                TraversPostorder(node.m_pChilds[i], action);

            for ( int i = 0; i < node.m_pSiblings.Size; i++ )
                TraversPostorder(node.m_pSiblings[i], action);

            action(node);
        }

        
        
    }

    public class NodeIterrator<T> : IRandomAccessIterator<T>, IForeachIterator<T>, IEnumerable<T>, IEnumerator<T>, IEquatable {
        private Node<T> m_pCurrent;

        public int AdvanceRest { get; private set; }
        public T Current { get => m_pCurrent.Value!; set => m_pCurrent.Value = value; }
        object IEnumerator.Current => Current!;

        public bool IsEnd => !m_pCurrent.HasNext;

        public bool IsBegin => !m_pCurrent.HasPrev;

        public ITerator<T> Clone() {
            return new NodeIterrator<T>(m_pCurrent);
        }

        public void Forward() {
            m_pCurrent = m_pCurrent.Next;
        }

        public void Back() {
            m_pCurrent = m_pCurrent.Prev;
        }

        public IEnumerator<T> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;

        public bool MoveNext() {
            if ( IsEnd )
                return false;

            Forward();
            return !IsEnd;
        }

        public void Reset() { }

        public void Dispose() {
            GC.SuppressFinalize(this);
        }

        public IRandomAccessIterator<T> Advance(int offset) {
            int r = 0;
            m_pCurrent = m_pCurrent.GetAt(offset, out r);
            AdvanceRest = r;

            return this;
        }

        public NodeIterrator(Node<T> current) {
             m_pCurrent = current;
        }
        public NodeIterrator(Node<T> current, int index) {
            int u = 0;
            m_pCurrent = current.GetAt(index, out u);
        }

        public bool Equals(NodeIterrator<T>? other) {
            if ( other == null ) return false;

            return m_pCurrent.Equals(other.m_pCurrent);
        }

        public override bool Equals(object? obj) {
            if ( obj is NodeIterrator<T> ) {
                return Equals((NodeIterrator<T>)obj);
            }
            return false;
        }
        public override int GetHashCode() {
            return m_pCurrent.GetHashCode();
        }

    }

    public static class NodeIteratorExtensions {
        public static NodeIterrator<T> First<T>(this Node<T> node)
            => new NodeIterrator<T>(node.Root());

        public static NodeIterrator<T> ReversFirst<T>(this Node<T> node)
            => new NodeIterrator<T>(node.Last());

        public static NodeIterrator<T> At<T>(this Node<T> node, int index)
           => new NodeIterrator<T>(node.Last(), index);

        public static NodeIterrator<T> Offset<T>(this Node<T> node, int offset)
           => new NodeIterrator<T>(node, offset);

        public static NodeIterrator<T> End<T>(this Node<T> node)
            => new NodeIterrator<T>(node.Last());

        public static NodeIterrator<T> ReversEnd<T>(this Node<T> node)
            => new NodeIterrator<T>(node.Root());
    }

}
