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
    /// Defines a map structure specialized for storing and retrieving <see cref="ITuple"/> objects.
    /// Provides tuple‑based search, traversal, and range‑insertion functionality.
    /// </summary>
    public interface ITupleMap : IMap {

        /// <summary>
        /// Gets the first tuple in the map, or <c>null</c> if the map is empty.
        /// </summary>
        ITuple? First { get; }

        /// <summary>
        /// Gets the last tuple in the map, or <c>null</c> if the map is empty.
        /// </summary>
        ITuple? Last { get; }

        /// <summary>
        /// Inserts a sequence of tuples starting at the specified position.
        /// </summary>
        /// <param name="pos">The index at which insertion begins.</param>
        /// <param name="items">The tuples to insert.</param>
        /// <returns>
        /// <c>true</c> if the insertion succeeded; otherwise <c>false</c>.
        /// </returns>
        bool InsertRange(int pos, IEnumerable<ITuple> items);

        /// <summary>
        /// Finds all tuples whose first element matches the specified key.
        /// </summary>
        /// <param name="Key">The key to search for.</param>
        /// <returns>An enumerable collection of matching tuples.</returns>
        IEnumerable<ITuple> Find(object Key);

        /// <summary>
        /// Traverses a range of tuples using the specified traversal mode.
        /// </summary>
        /// <param name="mode">Traversal direction.</param>
        /// <param name="startIndex">The starting index (inclusive).</param>
        /// <param name="endIndex">The ending index (exclusive).</param>
        /// <param name="func">The action to apply to each visited tuple.</param>
        void Traverse(TraversMode mode, int startIndex, int endIndex, Action<ITuple> func);

        /// <summary>
        /// Finds the first tuple whose first element matches the specified key.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <returns>
        /// The first matching tuple, or <c>null</c> if no match is found.
        /// </returns>
        ITuple? FindFirst(object key);

        /// <summary>
        /// Finds the last tuple whose first element matches the specified key.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <returns>
        /// The last matching tuple, or <c>null</c> if no match is found.
        /// </returns>
        ITuple? FindLast(object key);

        /// <summary>
        /// Returns all tuples stored in the map as an array.
        /// </summary>
        /// <returns>An array containing all tuples in the map.</returns>
        ITuple[] ToArray();
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
