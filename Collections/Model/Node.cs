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

using SystemEx.Collections.Generic;
using SystemEx.Collections.Generic.Interfaces;


namespace SystemEx.Collections.Model {

    /// <summary>
    /// Forward iterator for intrusive singly-linked <see cref="Node{T}"/> chains.
    /// </summary>
    /// <remarks>
    /// The iterator advances through a chain of nodes using the <c>Next</c> link.
    /// It stops automatically when the last node is reached, determined by
    /// <see cref="Node{T}.HasNext"/>. No sentinel node is used; the last node
    /// itself represents the end position.
    /// 
    /// This iterator is lightweight and allocation-free, intended for
    /// engine-level intrusive data structures.
    /// </remarks>
    public class NodeIterator<T> : IIterator<T>, IEquatable<NodeIterator<T>> {
        private Node<T> m_pNode;

        /// <summary>
        /// Initializes the iterator at the specified node.
        /// </summary>
        /// <param name="node">The node where the iterator starts, or <c>null</c>.</param>
        public NodeIterator ( Node<T> node ) {
            m_pNode = node;
        }
        /// <summary>
        /// Creates a copy of this iterator preserving its current position.
        /// </summary>
        public IIterator<T> Clone () {
            return new NodeIterator<T>(m_pNode);
        }
        
        /// <summary>
        /// Advances the iterator by one node.
        /// </summary>
        /// <remarks>
        /// If the current node has no successor, the iterator remains at the last node.
        /// </remarks>
        public void Forward () {
            if ( m_pNode.HasNext )
                m_pNode = m_pNode.Next!;
        }

        /// <summary>
        /// Advances the iterator by a specified number of nodes.
        /// </summary>
        /// <param name="i">Number of steps to advance.</param>
        /// <remarks>
        /// Traversal stops early if the end of the chain is reached.
        /// </remarks>
        public void Forward ( long i ) {
            while ( i-- > 0 && m_pNode.HasNext )
                m_pNode = m_pNode.Next!;
        }
        /// <summary>
        /// Determines whether two iterators reference the same node.
        /// </summary>
        public bool Equals ( NodeIterator<T>? other ) {
            if ( other == null ) return false;
            return ReferenceEquals(m_pNode, other.m_pNode);
        }
        /// <summary>
        /// Determines whether two iterators reference the same node.
        /// </summary>
        public override bool Equals ( object? obj ) {
            return Equals(obj as NodeIterator<T>);
        }
        /// <summary>
        /// Returns a hash code based on the referenced node.
        /// </summary>
        public override int GetHashCode () {
            return m_pNode?.GetHashCode() ?? 0;
        }
    }

    /// <summary>
    /// Intrusive singly-linked node storing a value and a single forward link.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Node{T}"/> represents the minimal building block for forward-only
    /// intrusive data structures. It stores a value and one <c>Next</c> pointer.
    /// </para>
    /// 
    /// <para>
    /// The class provides C++-style operations commonly found in
    /// <c>std::forward_list</c>:
    /// </para>
    /// <list type="bullet">
    /// <item><description><c>PushBack()</c> – append a node at the end</description></item>
    /// <item><description><c>InsertAfter()</c> – insert a node directly after this node</description></item>
    /// <item><description><c>EmplaceAfter()</c> – construct and insert a node after this node</description></item>
    /// <item><description><c>EraseNext()</c> – remove the node after this node</description></item>
    /// </list>
    /// 
    /// <para>
    /// No backward traversal is supported. For bidirectional intrusive lists,
    /// use <c>LinkedNode&lt;T&gt;</c>.
    /// </para>
    /// </remarks>
    public class Node<T> : GenericNode<T> {
        private  Node<T>? m_pNext;

        /// <summary>
        /// Gets or sets the forward link to the next node in the chain.
        /// </summary>
        public Node<T>? Next { get => m_pNext; protected set => m_pNext = value; }

        /// <summary>
        /// Gets the number of link slots (always 1).
        /// </summary>
        public int Count => 1;

        /// <summary>
        /// Indicates whether this node has a successor.
        /// </summary>
        public bool HasNext => m_pNext != null;

        /// <summary>
        /// Indicates whether this node is a leaf (no successor).
        /// </summary>
        public bool IsLeaf => HasNext;

        /// <summary>
        /// Returns an iterator positioned at this node.
        /// </summary>
        public override IIterator<T> Begin () =>  new NodeIterator<T>(this);

        /// <summary>
        /// Returns an iterator positioned at the last node of the chain.
        /// </summary>
        /// <remarks>
        /// The last node itself represents the end position. Traversal stops when
        /// <see cref="HasNext"/> is <c>false</c>.
        /// </remarks>
        public override IIterator<T> End () => new NodeIterator<T>(Back());

        /// <summary>
        /// Initializes a node with a default value.
        /// </summary>
        public Node () : this( default (T) ) { }

        /// <summary>
        /// Initializes a node with a value and a single forward slot.
        /// </summary>
        public Node ( T? value) : base(value) {
            m_pNext = null;
        }

        /// <summary>
        /// Copy constructor: copies value and forward link array.
        /// </summary>
        public Node (Node<T> other) : base(other) {
            m_pNext = other.m_pNext;
            
        }

        /// <summary>
        /// Appends a node at the end of the chain.
        /// </summary>
        /// <param name="pNext">Node to append.</param>
        /// <returns>The appended node.</returns>
        public virtual Node<T> PushBack ( Node<T> pNext) {
            Node<T> last = Back();
            last.Next = pNext;
            return pNext;
        }

        /// <summary>
        /// Inserts a node directly after this node.
        /// </summary>
        /// <param name="pNext">Node to insert.</param>
        /// <returns>The inserted node.</returns>
        public virtual Node<T> InsertAfter ( Node<T> pNext ) {
            pNext.Next = this.Next;
            this.Next = pNext;
            return pNext;
        }

        /// <summary>
        /// Constructs and inserts a node directly after this node.
        /// </summary>
        /// <param name="value">Value for the new node.</param>
        /// <returns>The newly created node.</returns>
        public virtual Node<T> EmplaceAfter ( T value ) {
            Node<T> node = new Node<T>(value);
            node.Next = this.Next;
            this.Next = node;
            return node;
        }

        /// <summary>
        /// Removes this node's successor.
        /// </summary>
        /// <remarks>
        /// If no successor exists, this node is returned unchanged.
        /// Otherwise, the successor is detached and returned.
        /// </remarks>
        public virtual Node<T> Erase () {
            Node<T>? _p = this.HasNext ?  new Node<T>(this.Next!) : this;
            this.Next = null;

            return _p!;
        }

        /// <summary>
        /// Removes the node after this node.
        /// </summary>
        /// <returns>The removed node, or <c>null</c> if no successor exists.</returns>
        public virtual Node<T>? EraseNext () {
            if ( !HasNext )
                return null;

            Node<T>? removed = this.Next;
            this.Next = removed!.Next;
            removed.Next = null;
            return removed;
        }

        /// <summary>
        /// Returns the last node in the chain.
        /// </summary>
        public virtual Node<T> Back () {
            Node<T> _ptemp = this;

            while(_ptemp.HasNext) {
                _ptemp = _ptemp.Next!;
            }

            return _ptemp;
        }
    }
    /// @}
}
