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
    /// \addtogroup collections
    /// @{
    /// <summary>
    /// Represents a grouping node that stores an arbitrary number of intrusive
    /// <see cref="Node{T}"/> instances inside a standard <see cref="List{T}"/>.  
    /// Unlike <see cref="Node{T}"/>’s own Prev/Next chain, a <see cref="GroupNode{T}"/>
    /// does not form a linked structure; it simply aggregates nodes into a collection.
    /// </summary>
    /// <typeparam name="T">The value type stored in the underlying nodes.</typeparam>
    public class GroupNode<T> : Node<T> {
        /// <summary>
        /// Internal list storing all nodes that belong to this group.
        /// </summary>
        private List<INode<T>> m_nodes;
        /// <summary>
        /// 
        /// </summary>
        public GroupNode() : base(2) {
            m_nodes = new List<INode<T>>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public GroupNode(T val) : base(2) {
            m_nodes = new List<INode<T>>();
            this.Value = val;
        }

        /// <summary>
        /// Returns a random‑access iterator positioned at the first node in the group.
        /// </summary>
        public new IRandomAccessIterator<INode<T>> First => m_nodes.First<INode<T>>();

        /// <summary>
        /// Returns a random‑access iterator positioned at the end of the group.
        /// </summary>
        public new IRandomAccessIterator<INode<T>> End => m_nodes.End<INode<T>>();

        /// <summary>
        /// Returns a random‑access iterator positioned at the specified index.
        /// </summary>
        /// <param name="index">The index of the node.</param>
        public new IRandomAccessIterator<INode<T>> At ( int index ) {
            return m_nodes.At<INode<T>>(index);
        }

        /// <summary>
        /// Indicates whether the group contains no nodes.
        /// </summary>
        public bool IsEmpty => m_nodes.Count == 0;

        /// <summary>
        /// Adds a node to the group.
        /// </summary>
        /// <param name="node">The node to add.</param>
        public void Add ( Node<T> node ) {
            m_nodes.Add(node);
        }

        /// <summary>
        /// Removes a node from the group if it exists.
        /// </summary>
        /// <param name="node">The node to remove.</param>
        public void Remove ( Node<T> node ) {
            m_nodes.Remove(node);
        }


        /// <summary>
        /// Enumerates all nodes stored in the group.
        /// </summary>
        public IEnumerable<INode<T>> Nodes => m_nodes;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodes"></param>
        public void AddRange ( IEnumerable<INode<T>> nodes ) {
            m_nodes.AddRange(nodes);
        }
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
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
