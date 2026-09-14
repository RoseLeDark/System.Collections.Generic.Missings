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

#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
