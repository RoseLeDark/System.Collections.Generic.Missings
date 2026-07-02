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

namespace SystemEx.Collections.Model {
    /// \addtogroup model
    /// @{
    /// <summary>
    /// Intrusive doubly‑linked node extended with an optional sibling link.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="LinkedNodeWithSibling{T, TS}"/> builds upon
    /// <see cref="GenericNode{T}"/> by adding a secondary <c>Sibling</c> reference.
    /// This enables auxiliary relationships between nodes that are not part of
    /// the primary intrusive list structure.
    /// </para>
    ///
    /// <para>
    /// The sibling link is intentionally independent of the <c>Prev</c>/<c>Next</c>
    /// chain. It may reference:
    /// </para>
    /// <list type="bullet">
    ///   <item><description>
    ///     A standalone <see cref="GenericNode{TS}"/> instance.
    ///   </description></item>
    ///   <item><description>
    ///     A node belonging to another intrusive list.
    ///   </description></item>
    ///   <item><description>
    ///     <c>null</c>, indicating no sibling association.
    ///   </description></item>
    /// </list>
    ///
    /// <para>
    /// This type is useful for representing auxiliary metadata, cross‑links,
    /// annotations, or secondary traversal paths without modifying the primary
    /// intrusive list topology.
    /// </para>
    ///
    /// <para>
    /// All standard intrusive operations (<c>InsertBefore</c>, <c>InsertAfter</c>,
    /// <c>Erase</c>, <c>ReplaceWith</c>, <c>SwapWith</c>, <c>Splice</c>, etc.)
    /// behave exactly as in <see cref="LinkedNode{T}"/> and do not affect the
    /// sibling link.
    /// </para>
    /// </remarks>
    public class LinkedNodeWithSibling<T, TS> : LinkedNode<T> {

        /// <summary>
        /// Optional sibling node associated with this node.
        /// </summary>
        /// <remarks>
        /// The sibling link is not part of the intrusive list structure and is
        /// never modified by list operations.
        /// </remarks>
        public GenericNode<TS>? Sibling { get; set; }

        /// <summary>
        /// Initializes a new node with no value and no sibling.
        /// </summary>
        public LinkedNodeWithSibling () : base() {
            Sibling = null;
        }

        /// <summary>
        /// Initializes a new node with the specified value and sibling value.
        /// </summary>
        /// <param name="value">The value stored in this node.</param>
        public LinkedNodeWithSibling ( T? value ) : base(value) {
            Sibling = null;
        }

        /// <summary>
        /// Initializes a new node from an iterator, copying only the primary node value.
        /// </summary>
        /// <param name="it">Iterator referencing the node to copy.</param>
        public LinkedNodeWithSibling ( LinkedNodeIterrator<T> it ) : base(it.Node) {
            Sibling = null;
        }

        /// <summary>
        /// Copy constructor: copies the value, intrusive links, and sibling link.
        /// </summary>
        /// <param name="other">The node to copy.</param>
        public LinkedNodeWithSibling ( LinkedNodeWithSibling<T, TS> other ) : base(other) {
            Sibling = other.Sibling;
        }
    }
    /// @}
}
