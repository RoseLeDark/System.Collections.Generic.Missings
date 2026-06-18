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

namespace SystemEx.Collections.Generic {
    /// <summary>
    /// A specialized <see cref="Node{T}"/> that represents a star‑shaped structure,
    /// where the node may have an arbitrary number of child nodes.
    /// </summary>
    /// <typeparam name="T">The value type stored in the node.</typeparam>
    public class StarNode<T> : Node<T> {

        /// <summary>
        /// Initializes a new star node with the specified value.
        /// </summary>
        /// <param name="value">The value stored in the node.</param>
        public StarNode(T value) : base(value) {
        }

        /// <summary>
        /// Adds a child node to this star node.  
        /// The child is appended to the internal child array.
        /// </summary>
        /// <param name="child">The child node to add.</param>
        public void AddChild(Node<T> child) {
            m_pChilds.Add(child);
        }

        /// <summary>
        /// Gets an enumerable sequence of all child nodes stored in this star node.
        /// </summary>
        public IEnumerable<Node<T>> Children => m_pChilds;
    }


}
