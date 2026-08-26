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
using System.Xml.Linq;



namespace SystemEx.Collections.Generic {
	/// \addtogroup Collections
	/// @{

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
    public class NodeIterrator<T> : IEnumerable<T>, IEnumerator<T> {
        /// <summary>
        /// The node currently referenced by the iterator.
        /// </summary>
        private Node<T> m_pCurrent;
        /// <summary>
        /// Gets the remaining offset after an <see cref="Advance(long)"/> operation.
        /// </summary>
        public long AdvanceRest { get; private set; }
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
        /// Moves the iterator one step forward.
        /// </summary>
        public void Forward() {
            if ( IsEnd ) return;
            m_pCurrent = m_pCurrent.Next!;
        }

        /// <summary>
        /// Moves the iterator N step forward
        /// </summary>
        public void Forward ( long i ) {
            var n = i;
            while ( n > 0 ) {
                --n;
                Forward();
            }
        }
        /// <summary>
        /// Moves the iterator one step backward.
        /// </summary>
        public void Back() {
            m_pCurrent = m_pCurrent.Prev!;
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
        /// Creates a new iterator starting at the specified node.
        /// </summary>
        public NodeIterrator(Node<T> current) {
            m_pCurrent = current.Clone();
        }
        /// <summary>
        /// Creates a new iterator starting at the specified node plus an index offset.
        /// </summary>
        public NodeIterrator(Node<T> current, long index ) {
            long u = 0;
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
    /// Represents a doubly linked node with optional child and sibling.
    /// Supports list-style navigation (Prev/Next), tree-style traversal,
    /// splicing, swapping, reversing, and random access movement.
    /// </summary>
    /// <typeparam name="T">The value stored in the node.</typeparam>
    public class Node<T> : INode<T> , IEnumerable<T> {
        const int NEXT = 1;
        const int PREV = 0;

        internal  FixedVector<Node<T>?> m_pNodex;

        internal  T? m_value;



        public T? this[ulong index] {
            get {
#pragma warning disable CA2201 // Keine reservierten Ausnahmetypen auslösen
                if ( Distance(true) >= index) return GetAt((long)index).Value;
                else throw new IndexOutOfRangeException();
#pragma warning restore CA2201 // Keine reservierten Ausnahmetypen auslösen
            }
            set {
                if ( Distance(true) >= index && value != null) 
                    SetAt((long)index, value!);
            }
        }

        /// <summary>
        /// Gets the number of child nodes associated with this node.
        /// Returns <c>null</c> if the implementation does not support child tracking.
        /// </summary>
        public virtual long? NChilds => m_pNodex.Count;

        /// <summary>
        /// Gets the number of child nodes associated with this node.
        /// </summary>
        public virtual ulong Lenght => (ulong)m_pNodex.Count;
        /// <summary>
        /// Gets or sets the next node in the linked chain.
        /// </summary>
        public Node<T>? Next {  get => m_pNodex[NEXT];  set => m_pNodex[NEXT] = value; }

        /// <summary>
        /// Gets or sets the previous node in the linked chain.
        /// </summary>
        public Node<T>? Prev {  get => m_pNodex[PREV];  set => m_pNodex[PREV] = value; }
        /// <summary>
        /// Gets or sets the value stored in this node.
        /// </summary>
        public T? Value { get => m_value; set => m_value = value; }

        /// <summary>
        /// Indicates whether this node has a next neighbor.
        /// </summary>
        public bool HasNext => m_pNodex[NEXT] != null;

        /// <summary>
        /// Indicates whether this node has a previous neighbor.
        /// </summary>
        public bool HasPrev => m_pNodex[PREV] != null;

        /// <summary>
        /// Combined size of child and sibling arrays. Erstnal 4
        /// </summary>
        public long? NodeSize => NChilds;

        

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
        public NodeIterrator<T> At( long index ) => new NodeIterrator<T>(Last(), index);

        /// <summary>
        /// Iterator positioned at this node plus an offset.
        /// </summary>
        public NodeIterrator<T> Offset(long offset) => new NodeIterrator<T>(this, offset);
        /// <summary>
        /// Reverse Iterator positioned at the last node in the chain.
        /// </summary>
        public NodeIterrator<T> ReversFirst => End;
        /// <summary>
        /// Reverse Iterator positioned at the first node in the chain.
        /// </summary>
        public NodeIterrator<T> ReversEnd => First;
        /// <summary>
        /// Get N. Child 
        /// </summary>
        public Node<T>? GetChild(int i) { return m_pNodex[i]; }
        /// <summary>
        /// Create a default node
        /// </summary>
        public Node () {
            m_value = default(T);

            m_pNodex = new FixedVector<Node<T>?>(2);
           
            m_pNodex[PREV] = null;
            m_pNodex[NEXT] = null;
        }
        /// <summary>
        /// Creates a node with default child/sibling arrays.
        /// </summary>
        public Node(T val ) {
            Value = val;
            
            m_pNodex = new FixedVector<Node<T>?>(2);

            m_pNodex[PREV] = null;
            m_pNodex[NEXT] = null;
        }
        /// <summary>
        /// Copy constructor (shallow copy of arrays).
        /// </summary>
        public Node(Node<T> node) {
            Value = node.Value;
            m_pNodex = node.m_pNodex;
        }
        /// <summary>
        /// Protected Node ctor see StarNode
        /// </summary>
        /// <param name="nChilds"></param>
        protected Node(int nChilds) {
            m_value = default(T);

            m_pNodex = new FixedVector<Node<T>?>(nChilds);

            m_pNodex[PREV] = null;
            m_pNodex[NEXT] = null;
        }
        /// <summary>
        /// Returns the first node in the linked chain.
        /// </summary>
        public Node<T> Root() {
            Node<T> temp =  this;
            
            while(temp.HasPrev) {

                temp = temp.Prev!;
            } ;

            return temp;
        }
        /// <summary>
        /// Returns the last node in the linked chain.
        /// </summary>
        public Node<T> Last() {
            Node<T> temp =  this;

            while ( temp.HasNext ) {

                temp = temp.Next!;
            } ;

            return temp;
        }
        /// <summary>
        /// Moves forward or backward by the specified index.
        /// </summary>
        public Node<T> GetAt( long index, out long r) {
            Node<T> temp = this;
            long n = index;

            if ( n > 0 ) {
                while ( n != 0 && temp!.HasNext ) {
                    temp = temp.Next!;
                    n--;
                }
            } else if ( n < 0 ) {
                while ( n != 0 && temp!.HasPrev ) {
                    temp = temp.Prev!;
                    n++;
                }
            }

            r = n;
            return temp;
        }

        /// <summary>
        /// Moves forward or backward by the specified index.
        /// </summary>
        public Node<T> GetAt ( long n) {
            Node<T> temp = this;

            if ( n > 0 ) {
                while ( n != 0 && temp!.HasNext ) {
                    temp = temp.Next!;
                }
            } else if ( n < 0 ) {
                while ( n != 0 && temp!.HasPrev ) {
                    temp = temp.Prev!;
                }
            }

            return temp;
        }

        /// <summary>
        /// Sets the value of the node located <paramref name="index"/> steps away from
        /// this node in the linked chain. Unlike iterator-based access, this method
        /// operates directly on the underlying node structure without cloning, ensuring
        /// that the modification affects the actual chain.
        /// </summary>
        /// <param name="index">
        /// The relative position from this node. Positive values move forward (Next),
        /// negative values move backward (Prev).
        /// </param>
        /// <param name="value">
        /// The new value to assign to the target node.
        /// </param>
        /// <returns>
        /// The node whose value was updated.
        /// </returns>
        public Node<T> SetAt ( long index, T value ) {
            long rest;
            Node<T> node = this.GetAt(index, out rest);
            node.Value = value;
            return node;
        }
        /// <summary>
        /// Find value 
        /// </summary>
        public Node<T>? Find ( T value ) {
            if ( this.Value!.Equals(value)) return this;
            Node<T> temp = this;
            Node<T>? ret = null;

            while (  temp!.HasNext ) {
                T? valu = temp.Value;
                if ( valu == null ) continue;

                if(valu.Equals(value)) {
                    ret = temp;
                    break;
                }
                temp = temp.Next!;
            }

            if( ret == null) {
                temp = this;

                while ( temp!.HasPrev ) {
                    T? valu = temp.Value;
                    if ( valu == null ) continue;

                    if ( valu.Equals(value) ) {
                        ret = temp;
                        break;
                    }
                    temp = temp.Prev!;
                }
            }
            return ret;
        }

        /// <summary>
        /// Add node to end , insert on pNext
        /// </summary>
        public Node<T> InsertLast(Node<T> pNext) {

            if ( m_pNodex[NEXT] == null ) {
                pNext.Prev = this;
                this.Next = pNext;
                
            } else {
                Node<T>? _pt = Last();

                pNext.Prev = _pt;
                _pt.Next = pNext;
            }

            return this;
        }
     
    

        /// <summary>
        /// Removes this node from the linked chain.
        /// </summary>
        public void Remove () {
            Node<T>? prev = m_pNodex[PREV];
            Node<T>? next = m_pNodex[NEXT];

            // Fall 1: Node ist isoliert
            if ( prev == null && next == null ) {
                // nichts zu verbinden
            }
            // Fall 2: Node ist am Anfang
            else if ( prev == null ) {
                next!.m_pNodex[PREV] = null;
            }
            // Fall 3: Node ist am Ende
            else if (  next == null ) {
                prev!.m_pNodex[NEXT] = null;
            }
            // Fall 4: Node ist mittendrin
            else {
                prev!.m_pNodex[NEXT] = next;
                next!.m_pNodex[PREV] = prev;
            }

#if TRACE
            // optional: isolieren
            m_pNodex[PREV] = null;
            m_pNodex[NEXT] = null;
#endif
        }



        /// <summary>
        /// Splices a range of nodes before this node.
        /// </summary>
        public void Splice ( ref Node<T> first, ref Node<T> last ) {
            // 1. Hole die alten Nachbarn
            Node<T>? before = this.Prev;
            Node<T>? after  = this.Next;

            // 2. Entferne 'this' aus seiner Position
            if ( before != null )
                before.Next = after;
            if ( after != null )
                after.Prev = before;

            // 3. Füge [first..last] an die Stelle ein, wo 'this' war

            // Verbinde vor dem Block
            if ( before != null )
                before.Next = first;
            first.Prev = before;

            // Verbinde nach dem Block
            if ( after != null )
                after.Prev = last;
            last.Next = after;

            // 4. 'this' ist jetzt isoliert
            this.Prev = null;
            this.Next = null;
        }



        /// <summary>
        /// Reverses the linked chain starting at this node.
        /// </summary>
        public Node<T> Reverse () {
            Node<T>? current = this;
            Node<T>? newHead = null;

            while ( current != null ) {
                // Swap Prev and Next
                Node<T>? temp = current.m_pNodex[NEXT];
                current.m_pNodex[NEXT] = current.m_pNodex[PREV];
                current.m_pNodex[PREV] = temp;

                newHead = current;

                current = current.m_pNodex[PREV];
            }

            return newHead!;
        }



        /// \deprecated Use InsertRange(ref Node<T> pFirst, ref Node<T> pFinal).
        /// This method will be removed in the next build.
        /// <summary>
        /// Inserts a range of nodes before this node.
        /// </summary>
        /// \deprecated Use <see cref="InsertRange(ref Node{T}, ref Node{T})"/> instead.
        /// This method will be removed in the next build.
        [Obsolete("Use InsertRange(ref Node<T> pFirst, ref Node<T> pFinal) instead. This method will be removed in the next major release.")]
        public void InsertRagen ( ref Node<T> pFirst, ref Node<T> pFinal )
            => InsertRange(ref pFirst, ref  pFinal);

        /// <summary>
        /// Inserts a range of nodes before this node.
        /// </summary>
        public void InsertRange ( ref Node<T> pFirst, ref Node<T> pFinal) {

            Node<T>? prev = m_pNodex[PREV];

            if ( prev != null ) {
                prev.m_pNodex[NEXT] = pFirst;
                pFirst.m_pNodex[PREV] = prev;
            }

            m_pNodex[PREV] = pFinal;
            pFinal.m_pNodex[NEXT] = this;
        }
        /// <summary>
        /// Swaps this node with another node, preserving neighbor links.
        /// </summary>
        public void Swap ( ref Node<T> other ) {
            if ( this == other )
                return;

            // Backup A
            Node<T>? aPrev = this.Prev;
            Node<T>? aNext = this.Next;
            T? aValue = this.Value;

            // Backup B
            Node<T>? bPrev = other.Prev;
            Node<T>? bNext = other.Next;
            T? bValue = other.Value;

            // Copy B into A
            this.Prev = bPrev;
            this.Next = bNext;
            this.Value = bValue;

            // Copy A into B
            other.Prev = aPrev;
            other.Next = aNext;
            other.Value = aValue;

            // Fix neighbors of A (null‑sicher)
            if ( this.Prev != null )
                this.Prev.Next = this;

            if ( this.Next != null )
                this.Next.Prev = this;

            // Fix neighbors of B (null‑sicher)
            if ( other.Prev != null )
                other.Prev.Next = other;

            if ( other.Next != null )
                other.Next.Prev = other;
        }


        /// <summary>
        /// Returns the number of steps to the beginning or ending of the chain.
        /// <param name="ToEnd"/>if <c>true</c> then returns the number of steps to the ending of the chain </param>
        /// </summary>
        public ulong Distance(bool ToEnd = false) {
            ulong _temp = 0;
            Node<T> _node = this;

            if ( ToEnd ) {
                while ( _node.HasNext ) {
                    _node = _node.Next!;
                    _temp++;
                }
            } else {
                while ( HasPrev ) {
                    _temp++;
                    _node = _node.Prev!;
                }
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
            TraversInorder(this, action);
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
        //public NodeSlice<T> Slice(int start, int length) {
          //  return new NodeSlice<T>((NodeIterrator<T>)First.Advance(start), length);
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public NodeChain<T> AsChain() {
            return new NodeChain<T>().Add(First, End);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public System.Collections.IEnumerator GetEnumerator () {
            return GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator () {
            Node<T> root = Root();

            do {
                yield return root.Value!;

                if ( root.HasNext ) root = root.Next!;
                else break;
            } while ( root != null );
        }

        private void TraversListForward(Action<Node<T>> action) {
            var temp = this;
            while ( temp!.HasNext ) {
                action(temp);
                temp = temp.Next;
            }
            action(temp); // letztes Element
        }

        private void TraversListBackward(Action<Node<T>> action) {
            var temp = this;
            while ( temp!.HasPrev ) {
                action(temp);
                temp = temp.Prev;
            }
            action(temp); // erstes Element
        }
        private static void TraversPreorder(Node<T>? node, Action<Node<T>> action) {
            if ( node == null ) return;

            action(node);

            // Childs
            TraversPreorder(node.m_pNodex[PREV], action);
            TraversPreorder(node.m_pNodex[NEXT], action);

        }
        private static void TraversPostorder(Node<T>? node, Action<Node<T>> action) {
            if ( node == null ) return;

            // Childs
            TraversPostorder(node.m_pNodex[PREV], action);
            TraversPostorder(node.m_pNodex[NEXT], action);

           
            action(node);
        }
        private void TraversInorder ( Node<T> node, Action<Node<T>> action ) {
            if ( node == null ) return;

            // Childs
            TraversPostorder(node.m_pNodex[PREV], action);
            action(node);
            TraversPostorder(node.m_pNodex[NEXT], action);

          
        }
   

    }

#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
