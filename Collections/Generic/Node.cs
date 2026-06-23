/* 
 * SPDX-License-Identifier: EUPL-1.2
 *
 * Copyright (c) 2026 Amber-Sophia Schröck <ambersophia.schroeck@mail.de>
 *
 * This file is licensed under the European Union Public Licence (EUPL) version 1.2.
 * You can obtain a copy of the licence at:
 *   https://joinup.ec.europa.eu/collection/eupl/eupl-text-eupl-12
 *
 * Unless required by applicable law or agreed to in writing, software distributed
 * under the Licence is distributed on an "AS IS" basis, WITHOUT WARRANTIES OR
 * CONDITIONS OF ANY KIND, either express or implied.
 *
 * If you modify this file, retain this notice and add a short description of your
 * changes and the date.
 */

using System.Collections;
using System.ComponentModel.Design.Serialization;
using SystemEx.Collections.Generic.Interfaces;


namespace SystemEx.Collections.Generic {
    /// <summary>
    /// Defines the supported traversal orders for <see cref="Node{T}"/> structures.
    /// </summary>
    public enum TraversOrder {
        /// <summary>Visit the current node before its children and siblings.</summary>
        Preorder,

        /// <summary>Visit the left subtree, then the node, then the right subtree (not implemented).</summary>
        Inorder,

        /// <summary>Visit children and siblings before the current node.</summary>
        Postorder,

        /// <summary>Traverse the linked list in forward direction.</summary>
        ListOrder,

        /// <summary>Traverse the linked list in reverse direction.</summary>
        ReservListOrder
    }

    /// <summary>
    /// Iterator for navigating through a doubly linked <see cref="Node{T}"/> chain.
    /// Supports random access, forward/backward movement, foreach enumeration,
    /// and cloning of iterator state.
    /// </summary>
    /// <typeparam name="T">The value type stored in each node.</typeparam>
    public class NodeIterrator<T> : IRandomAccessIterator<T>, IForeachIterator<T>, IEnumerable<T>, IEnumerator<T> {
        /// <summary>
        /// The node currently referenced by the iterator.
        /// </summary>
        private Node<T> m_pCurrent;
        /// <summary>
        /// Gets the remaining offset after an <see cref="Advance(int)"/> operation.
        /// </summary>
        public int AdvanceRest { get; private set; }
        /// <summary>
        /// Gets or sets the value of the current node.
        /// </summary>
        public T Current { get => m_pCurrent.Value!; set => m_pCurrent.Value = value; }
        object IEnumerator.Current => Current!;
        /// <summary>
        /// Indicates whether the iterator has reached the end of the chain.
        /// </summary>
        public bool IsEnd => !m_pCurrent.HasNext;
        /// <summary>
        /// Indicates whether the iterator is positioned at the beginning of the chain.
        /// </summary>
        public bool IsBegin => !m_pCurrent.HasPrev;
        /// <summary>
        /// Creates a clone of this iterator referencing the same logical position.
        /// </summary>
        public IIterator<T> Clone() {
            return new NodeIterrator<T>(m_pCurrent);
        }
        /// <summary>
        /// Moves the iterator one step forward.
        /// </summary>
        public void Forward() {
            m_pCurrent = m_pCurrent.Next;
        }
        /// <summary>
        /// Moves the iterator one step backward.
        /// </summary>
        public void Back() {
            m_pCurrent = m_pCurrent.Prev;
        }
        /// <summary>
        /// The type of objects to enumerate.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator() => this;
        
        IEnumerator IEnumerable.GetEnumerator() => this;
        /// <summary>
        /// Go to the next node
        /// </summary>
        /// <returns></returns>
        public bool MoveNext() {
            if ( IsEnd )
                return false;

            Forward();
            return !IsEnd;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Reset() { }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose() {
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Advances the iterator by the specified offset.
        /// </summary>
        /// <param name="offset">Positive or negative offset.</param>
        /// <returns>The iterator itself.</returns>
        public IRandomAccessIterator<T> Advance(int offset) {
            int r = 0;
            m_pCurrent = m_pCurrent.GetAt(offset, out r);
            AdvanceRest = r;

            return this;
        }
        /// <summary>
        /// Creates a new iterator starting at the specified node.
        /// </summary>
        public NodeIterrator(Node<T> current) {
            m_pCurrent = current.Clone();
        }
        /// <summary>
        /// Creates a new iterator starting at the specified node plus an index offset.
        /// </summary>
        public NodeIterrator(Node<T> current, int index) {
            int u = 0;
            var x = current.Clone();
            m_pCurrent = x.GetAt(index, out u);
        }
        /// <summary>
        /// Determines whether this iterator references the same node as another iterator.
        /// </summary>
        public bool Equals(NodeIterrator<T>? other) {
            if ( other == null ) return false;

            return m_pCurrent.Equals(other.m_pCurrent);
        }
        /// <summary>
        /// Is the given node equels witrh this node
        /// </summary>
        /// <param name="obj">the oter node</param>
        /// <returns>true the given obj ist equal, otherwise false</returns>
        public override bool Equals(object? obj) {
            if ( obj is NodeIterrator<T> ) {
                return Equals((NodeIterrator<T>)obj);
            }
            return false;
        }
        /// <summary>
        /// Get the Hash Code
        /// </summary>
        /// <returns>the hash code</returns>
        public override int GetHashCode() {
            return m_pCurrent.GetHashCode();
        }

    }
    /// <summary>
    /// Represents a doubly linked node with optional child and sibling arrays.
    /// Supports list-style navigation (Prev/Next), tree-style traversal,
    /// splicing, swapping, reversing, and random access movement.
    /// </summary>
    /// <typeparam name="T">The value stored in the node.</typeparam>
    public class Node<T> : INode<T> {
        const int NEXT = 1;
        const int PREV = 0;
        /// <summary>
        /// Minimum size for child and sibling arrays.
        /// </summary>
        public  const byte MINSIZE = 2;

        internal  Array<Node<T>> m_pChilds;
        internal  Array<Node<T>> m_pSiblings;
        internal  T? m_value;
        /// <summary>
        /// Gets or sets the next node in the linked chain.
        /// </summary>
        public Node<T> Next {  get => m_pChilds[NEXT];  set => m_pChilds[NEXT] = value; }

        /// <summary>
        /// Gets or sets the previous node in the linked chain.
        /// </summary>
        public Node<T> Prev {  get => m_pChilds[PREV];  set => m_pChilds[PREV] = value; }
        /// <summary>
        /// Gets or sets the value stored in this node.
        /// </summary>
        public T? Value { get => m_value; set => m_value = value; }

        /// <summary>
        /// Number of child entries (may include Prev/Next slots).
        /// </summary>
        public int? NChilds => m_pChilds == null ? 0 : m_pChilds.Size;

        /// <summary>
        /// Number of sibling entries.
        /// </summary>
        public int? NSiblings => m_pSiblings == null ? 0 : m_pSiblings.Size;
        /// <summary>
        /// Indicates whether this node has a next neighbor.
        /// </summary>
        public bool HasNext => m_pChilds[NEXT] != this;

        /// <summary>
        /// Indicates whether this node has a previous neighbor.
        /// </summary>
        public bool HasPrev => m_pChilds[PREV] != this;

        /// <summary>
        /// Combined size of child and sibling arrays.
        /// </summary>
        public int? NodeSize => NChilds + NSiblings;

        /// <summary>
        /// Iterator positioned at the first node in the chain.
        /// </summary>
        public NodeIterrator<T> First => new NodeIterrator<T>(Root());

        /// <summary>
        /// Iterator positioned at the last node in the chain.
        /// </summary>
        public NodeIterrator<T> End => new NodeIterrator<T>(Last());

        /// <summary>
        /// Iterator positioned at the node offset from the last node.
        /// </summary>
        public NodeIterrator<T> At(int index) => new NodeIterrator<T>(Last(), index);

        /// <summary>
        /// Iterator positioned at this node plus an offset.
        /// </summary>
        public NodeIterrator<T> Offset(int offset) => new NodeIterrator<T>(this, offset);
        /// <summary>
        /// Reverse Iterator positioned at the last node in the chain.
        /// </summary>
        public NodeIterrator<T> ReversFirst => End;
        /// <summary>
        /// Reverse Iterator positioned at the first node in the chain.
        /// </summary>
        public NodeIterrator<T> ReversEnd => First;

        /// <summary>
        /// Create a default node
        /// </summary>
        protected Node() {
            m_value = default(T);

            m_pChilds = new Array<Node<T>>(MINSIZE);
            m_pSiblings = new Array<Node<T>>(MINSIZE);

            m_pChilds[PREV] = m_pChilds[NEXT] = this;
        }
        /// <summary>
        /// Creates a node with default child/sibling arrays.
        /// </summary>
        public Node(T val ) {
            Value = val;
            
            m_pChilds = new Array<Node<T>>(MINSIZE);
            m_pSiblings = new Array<Node<T>>(MINSIZE);

            m_pChilds[PREV] = m_pChilds[NEXT] = this;
        }
        /// <summary>
        /// Creates a node with specified child/sibling array sizes.
        /// </summary>
        public Node(int nChilds, int nSiblings, T iValue) {
            m_pChilds = new Array<Node<T>>(nChilds);
            m_pSiblings = new Array<Node<T>>(nSiblings);
            Value = iValue;

            m_pChilds[PREV] = m_pChilds[NEXT] = this;
        }
        /// <summary>
        /// Creates a node using existing child/sibling arrays.
        /// </summary>
        public Node(T val, Array<Node<T>> pChilds, Array<Node<T>> pSiblings) {
            Value = val;
            m_pChilds = pChilds;
            m_pSiblings = pSiblings;
        }
        /// <summary>
        /// Copy constructor (shallow copy of arrays).
        /// </summary>
        public Node(Node<T> node) {
            Value = node.Value;
            m_pChilds = node.m_pChilds;
            m_pSiblings = node.m_pSiblings;
        }
        /// <summary>
        /// Returns the first node in the linked chain.
        /// </summary>
        public Node<T> Root() {
            Node<T> temp =  this;

            while(temp.HasPrev) {

                temp = temp.Prev;
            } ;

            return temp;
        }
        /// <summary>
        /// Returns the last node in the linked chain.
        /// </summary>
        public Node<T> Last() {
            Node<T> temp =  this;

            while ( temp.HasNext ) {

                temp = temp.Next;
            } ;

            return temp;
        }
        /// <summary>
        /// Moves forward or backward by the specified index.
        /// </summary>
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
        /// <summary>
        /// Inserts this node before the specified node.
        /// </summary>
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
        /// <summary>
        /// Removes this node from the linked chain.
        /// </summary>
        public void remove() {
            m_pChilds[NEXT].m_pChilds[PREV] = m_pChilds[PREV];
            m_pChilds[PREV].m_pChilds[NEXT] = m_pChilds[NEXT];

#if TRACE
            // optional: isolieren
            m_pChilds[NEXT] = this;
            m_pChilds[PREV] = this;
#endif
        }
        /// <summary>
        /// Splices a range of nodes before this node.
        /// </summary>
        public void Splice(ref Node<T> first, ref Node<T> last) {
            last.m_pChilds[PREV].m_pChilds[NEXT] = this;
            first.m_pChilds[PREV].m_pChilds[NEXT] = last;
            this.m_pChilds[PREV].m_pChilds[NEXT] = first;

            Node<T> pTemp = this.m_pChilds[PREV];
            this.m_pChilds[PREV] = last.m_pChilds[PREV];
            last.m_pChilds[PREV] = first.m_pChilds[PREV];
            first.m_pChilds[PREV] = pTemp;
        }

        /// <summary>
        /// Reverses the linked chain starting at this node.
        /// </summary>
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
        /// <summary>
        /// Inserts a range of nodes before this node.
        /// </summary>
        public void InsertRagen(ref Node<T> pFirst, ref Node<T> pFinal) {
            m_pChilds[PREV].m_pChilds[NEXT] = pFirst; pFirst.m_pChilds[PREV] = m_pChilds[PREV];
            m_pChilds[PREV] = pFinal; pFinal.m_pChilds[NEXT] = this;
        }
        /// <summary>
        /// Swaps this node with another node, preserving neighbor links.
        /// </summary>
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

        /// <summary>
        /// Returns the number of steps to the beginning of the chain.
        /// </summary>
        public ulong Distance() {
            ulong _temp = 0;
            while(HasPrev) {
                _temp++;

            }
            return _temp;
        }
        /// <summary>
        /// Performs a traversal using the specified order.
        /// </summary>
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
        /// <summary>
        /// Creates a shallow clone of this node.
        /// </summary>
        public Node<T> Clone() {
            return new Node<T>(this);
        }
        /// <summary>
        /// Create a Range <see cref="NodeRange{T}"/>
        /// </summary>
        /// <returns></returns>
        public NodeRange<T> AsRange() {
            return new NodeRange<T>(First, End);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public NodeSlice<T> Slice(int start, int length) {
            return new NodeSlice<T>((NodeIterrator<T>)First.Advance(start), length);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public NodeChain<T> AsChain() {
            return new NodeChain<T>().Add(First, End);
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


}
