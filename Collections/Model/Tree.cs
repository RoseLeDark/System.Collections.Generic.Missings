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
using System;
using System.Collections.Generic;
using System.Text;
using SystemEx.Collections.Generic;

namespace SystemEx.Collections.Model {
    /// <summary>
    /// Represents a delegate for traversing a binary tree.
    /// </summary>
    /// <param name="n">The current node in the traversal.</param>
    /// <param name="left">Indicates whether the current node is a left child.</param>
    /// <param name="depth">The depth of the current node in the tree.</param>
    /// <typeparam name="T">The type of the value stored in the tree nodes.</typeparam>
    /// <returns>true if the traversal should continue; otherwise, false.</returns>
    public delegate bool TraverseFunc<T> ( BinaryTree<T> n, bool left, long depth );

    /// <summary>
    /// Represents a generic tree structure with a specified number of child nodes.
    /// </summary>
    /// <typeparam name="T">The type of the value stored in the tree nodes.</typeparam>
    /// <typeparam name="TRE">The type of the tree node.</typeparam>
    public class Tree<T, TRE> : GenericNode<T>  {
        /// <summary>
        /// Gets the array of child nodes in the tree.
        /// </summary>
        protected FixedVector<TRE?> m_pElemtents;
        /// <summary>
        /// Gets the number of child nodes in the tree.
        /// </summary>
        public int Childs { get;  }
        /// <summary>
        /// Initializes a new instance of the Tree class.
        /// </summary>
        /// <param name="childs">The number of child nodes in the tree.</param>
        /// <param name="value">The value stored in the tree node.</param>
        protected Tree (int childs, T? value) 
            : base(value) {
            m_pElemtents = new FixedVector<TRE?>(childs);
        }
    }
    /// @}
}
