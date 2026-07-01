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

namespace SystemEx.Collections.Generic.Interfaces {
    /// \addtogroup collections
    /// @{
    /// \addtogroup interfaces
    /// @{
    /// <summary>
    /// Defines the basic contract for a node in a multi‑linked structure,
    /// such as a tree, graph, or intrusive node network.
    /// Provides access to the stored value and metadata about child counts.
    /// </summary>
    /// <typeparam name="T">The type of value stored in the node.</typeparam>
    public interface INode<T> {

        /// <summary>
        /// Gets or sets the value stored in this node.
        /// </summary>
        T? Value { get; set; }

        /// <summary>
        /// Gets the number of child nodes associated with this node.
        /// Returns <c>null</c> if the implementation does not support child tracking.
        /// </summary>
        int? NChilds { get; }

    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
