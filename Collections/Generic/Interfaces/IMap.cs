// SPDX-License-Identifier: EUPL-1.2

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
    /// <summary>
    /// Defines the basic contract for a map-like container that stores elements
    /// in positional order and supports indexed removal and clearing operations.
    /// </summary>
    public interface IMap {

        /// <summary>
        /// Gets the number of elements currently stored in the map.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Indicates whether the map is read-only.
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// Indicates whether the map contains no elements.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Indicates whether the map has reached its maximum capacity.
        /// </summary>
        bool IsFull { get; }

        /// <summary>
        /// Gets the total capacity of the map.
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Removes the element at the specified position.
        /// </summary>
        /// <param name="pos">The index of the element to remove.</param>
        void RemoveAt(int pos);

        /// <summary>
        /// Removes a range of elements from <paramref name="start"/> to <paramref name="iend"/> (exclusive).
        /// </summary>
        /// <param name="start">The starting index.</param>
        /// <param name="iend">The ending index (exclusive).</param>
        void RemoveAt(int start, int iend);

        /// <summary>
        /// Removes all elements from the map.
        /// </summary>
        void Clear();
    }
    /// <summary>
    /// Defines a typed map structure that stores key/value pairs as <see cref="Pair{T, TU}"/>.
    /// Provides insertion, removal, searching, traversal, and conversion utilities.
    /// </summary>
    /// <typeparam name="T">The key type.</typeparam>
    /// <typeparam name="TU">The value type.</typeparam>
    public interface IMap<T, TU> : IMap {
        /// <summary>
        /// Gets the first element in the map, or <c>null</c> if the map is empty.
        /// </summary>
        Pair<T, TU>? First { get; }

        /// <summary>
        /// Gets the last element in the map, or <c>null</c> if the map is empty.
        /// </summary>
        Pair<T, TU>? Last { get; }

        /// <summary>
        /// Adds a new key/value pair to the map.
        /// </summary>
        /// <param name="item">The pair to add.</param>
        void Add(Pair<T, TU> item);

        /// <summary>
        /// Removes the specified key/value pair from the map.
        /// </summary>
        /// <param name="item">The pair to remove.</param>
        /// <returns><c>true</c> if the element was removed; otherwise <c>false</c>.</returns>
        bool Remove(Pair<T, TU> item);

        /// <summary>
        /// Inserts a key/value pair at the specified position.
        /// </summary>
        /// <param name="pos">The index at which to insert.</param>
        /// <param name="item">The pair to insert.</param>
        /// <returns><c>true</c> if the insertion succeeded; otherwise <c>false</c>.</returns>
        bool Insert(int pos, Pair<T, TU> item);

        /// <summary>
        /// Inserts a range of key/value pairs starting at the specified position.
        /// </summary>
        /// <param name="pos">The starting index.</param>
        /// <param name="items">The pairs to insert.</param>
        /// <returns><c>true</c> if the insertion succeeded; otherwise <c>false</c>.</returns>
        bool InsertRange(int pos, IEnumerable<Pair<T, TU>> items);

        /// <summary>
        /// Finds all key/value pairs whose key matches the specified value.
        /// </summary>
        /// <param name="Key">The key to search for.</param>
        /// <returns>An enumerable collection of matching pairs.</returns>
        IEnumerable<Pair<T, TU>> Find(T Key);

        /// <summary>
        /// Traverses a range of elements using the specified traversal mode.
        /// </summary>
        /// <param name="mode">Traversal direction.</param>
        /// <param name="startIndex">The starting index (inclusive).</param>
        /// <param name="endIndex">The ending index (exclusive).</param>
        /// <param name="func">The action to apply to each visited pair.</param>
        void Traverse(TraversMode mode, int startIndex, int endIndex, Action<Pair<T, TU>> func);

        /// <summary>
        /// Finds the first key/value pair whose key matches the specified value.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <returns>The first matching pair, or <c>null</c> if none is found.</returns>
        Pair<T, TU>? FindFirst(T key);

        /// <summary>
        /// Finds the last key/value pair whose key matches the specified value.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <returns>The last matching pair, or <c>null</c> if none is found.</returns>
        Pair<T, TU>? FindLast(T key);

        /// <summary>
        /// Returns all key/value pairs stored in the map as an array.
        /// </summary>
        /// <returns>An array containing all pairs in the map.</returns>
        Pair<T, TU>[] ToArray();
    }
}
