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
using SystemEx.Collections.Generic.Interfaces;

namespace SystemEx.Collections.Generic {
    /// <summary>
    /// Represents a grouping node that stores an arbitrary number of intrusive
    /// <see cref="Node{T}"/> instances inside a standard <see cref="List{T}"/>.  
    /// Unlike <see cref="Node{T}"/>’s own Prev/Next chain, a <see cref="GroupNode{T}"/>
    /// does not form a linked structure; it simply aggregates nodes into a collection.
    /// </summary>
    /// <typeparam name="T">The value type stored in the underlying nodes.</typeparam>
    public class GroupNode<T> : INode<T> {
        /// <summary>
        /// Internal list storing all nodes that belong to this group.
        /// </summary>
        private List<INode<T>> m_nodes;
        const int NEXT = 1;
        const int PREV = 0;
        /// <summary>
        /// Minimum size for child and sibling arrays.
        /// </summary>
        public  const byte MINSIZE = 2;

        internal  Array<INode<T>> m_pChilds;
        internal  Array<INode<T>> m_pSiblings;

        /// <summary>
        /// Gets or sets the next node in the linked chain.
        /// </summary>
        public INode<T> Next { get => m_pChilds[NEXT]; set => m_pChilds[NEXT] = value; }

        /// <summary>
        /// Gets or sets the previous node in the linked chain.
        /// </summary>
        public INode<T> Prev { get => m_pChilds[PREV]; set => m_pChilds[PREV] = value; }
        /// <summary>
        /// Get or sets the current value
        /// </summary>
        public T? Value { get => m_nodes[0].Value; set => m_nodes[0].Value = value; }

        /// <summary>
        /// Number of childs in this group,
        /// </summary>
        public int? NChilds =>  m_pChilds.Count;

        /// <summary>
        /// Number of sibling in this group.
        /// </summary>
        public int? NSiblings => m_pSiblings.Count;
        /// <summary>
        /// Get Sibling at
        /// </summary>
        /// <param name="index">index</param>
        /// <returns></returns>
        public INode<T> GetSiblingsAt(int index) => m_pSiblings.ElementAt(index);
        /// <summary>
        /// Get child at
        /// </summary>
        /// <param name="index">index</param>
        /// <returns></returns>
        public INode<T> GetChildAt(int index) => m_pChilds.ElementAt(index);
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
        /// Initializes a new group node.  
        /// The base <c><see cref="INode{T}"/> </c>
        /// because the group itself does not represent a single node value.
        /// </summary>
        public GroupNode(T value) : base() {
            m_nodes = new List<INode<T>>();
            m_nodes.Add(new Node<T>(value));

            m_pChilds = new Array<INode<T>>(MINSIZE);
            m_pSiblings = new Array<INode<T>>(MINSIZE);

            m_pChilds[PREV] = m_pChilds[NEXT] = this;
        }

        /// <summary>
        /// Creates a node with specified child/sibling array sizes.
        /// </summary>
        public GroupNode(int nChilds, int nSiblings, T value) {
            m_pChilds = new Array<INode<T>>(nChilds);
            m_pSiblings = new Array<INode<T>>(nSiblings);
            m_nodes = new List<INode<T>>();
            m_nodes.Add(new Node<T>(value));

            m_pChilds[PREV] = m_pChilds[NEXT] = this;
        }
        /// <summary>
        /// Creates a node using existing child/sibling arrays.
        /// </summary>
        public GroupNode(T val, Array<INode<T>> pChilds, Array<INode<T>> pSiblings) {
            m_nodes = new List<INode<T>>();
            m_nodes.Add(new Node<T>(val));
            m_pChilds = pChilds;
            m_pSiblings = pSiblings;
        }
        /// <summary>
        /// Copy constructor (shallow copy of arrays).
        /// </summary>
        public GroupNode(GroupNode<T> node) {
            m_nodes = node.m_nodes;
            m_pChilds = node.m_pChilds;
            m_pSiblings = node.m_pSiblings;
        }
        /// <summary>
        /// Copy constructor from 1 node
        /// </summary>
        /// <param name="node"></param>
        public GroupNode(Node<T> node) {
            m_nodes = new List<INode<T>>();
            m_nodes.Add(new Node<T>(node.Value!));
            m_pChilds = new Array<INode<T>>( node.m_pChilds);
            m_pSiblings = new Array<INode<T>>(node.m_pSiblings);
        }

        /// <summary>
        /// Indicates whether the group contains no nodes.
        /// </summary>
        public bool IsEmpty => m_nodes.Count == 0;

        /// <summary>
        /// Adds a node to the group.
        /// </summary>
        /// <param name="node">The node to add.</param>
        public void Add(Node<T> node) {
            m_nodes.Add(node);
        }

        /// <summary>
        /// Removes a node from the group if it exists.
        /// </summary>
        /// <param name="node">The node to remove.</param>
        public void Remove(Node<T> node) {
            m_nodes.Remove(node);
        }

        /// <summary>
        /// Returns a random‑access iterator positioned at the first node in the group.
        /// </summary>
        public IRandomAccessIterator<INode<T>> First => m_nodes.First<INode<T>>();

        /// <summary>
        /// Returns a random‑access iterator positioned at the end of the group.
        /// </summary>
        public IRandomAccessIterator<INode<T>> End => m_nodes.End<INode<T>>();

        /// <summary>
        /// Returns a random‑access iterator positioned at the specified index.
        /// </summary>
        /// <param name="index">The index of the node.</param>
        public IRandomAccessIterator<INode<T>> At(int index) {
            return m_nodes.At<INode<T>>(index);
        }

        /// <summary>
        /// Enumerates all nodes stored in the group.
        /// </summary>
        public IEnumerable<INode<T>> Nodes => m_nodes;

        

        /// <summary>
        /// Traverses all nodes in the group using the provided action.  
        /// The traversal order parameter is ignored; the group is always iterated
        /// in list order.
        /// </summary>
        /// <param name="order">Ignored.</param>
        /// <param name="action">The action to apply to each node.</param>
        public void Travers(TraversOrder order, Action<INode<T>> action) {
            foreach ( var n in m_nodes )
                action(n);
        }
    }
}
