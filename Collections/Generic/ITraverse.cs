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

    /// \addtogroup collections
    /// @{
    /// \addtogroup interfaces
    /// @{
    /// <summary>
    /// Specifies the traversal direction for iterating over a sequence.
    /// </summary>
    public enum TraversMode {
        /// <summary>
        /// Traverses elements from lower to higher indices.
        /// </summary>
        Forwards,

        /// <summary>
        /// Traverses elements from higher to lower indices.
        /// </summary>
        Backwards,

        InOrder,
        PreOrder,
    }

    /// <summary>
    /// Defines a traversal operation over a range of elements using a specified
    /// direction and index boundaries.
    /// </summary>
    /// <typeparam name="T">The type of elements being traversed.</typeparam>
    public interface ITraverse<T> {

        /// <summary>
        /// Traverses a range of elements using the specified traversal mode.
        /// </summary>
        /// <param name="mode">The traversal direction.</param>
        /// <param name="startIndex">The starting index (inclusive).</param>
        /// <param name="endIndex">The ending index (exclusive).</param>
        /// <param name="func">The action to apply to each visited element.</param>
        void Traverse(TraversMode mode, long startIndex, long endIndex, Action<T> func);
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
