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

using System.Diagnostics.CodeAnalysis;

namespace SystemEx.Collections.Generic.Interfaces {
    /// \addtogroup collections
    /// @{
    /// \addtogroup interfaces
    /// @{
    /// 
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
    public interface IMap<T, TU> : IMap, ICollection<Pair<T, TU>> where T : notnull {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TU this[T key] {
            get;
            set;
        }

        /// <summary>
        /// Returns a collections of the keys in this map.
        /// </summary>
        ICollection<T> Keys {
            get;
        }
        /// <summary>
        /// Returns a collections of the values in this map.
        /// </summary>
        ICollection<TU> Values {
            get;
        }
        /// <summary>
        /// Gets the first element in the map, or <c>null</c> if the map is empty.
        /// </summary>
        Pair<T, TU>? First { get; }

        /// <summary>
        /// Gets the last element in the map, or <c>null</c> if the map is empty.
        /// </summary>
        Pair<T, TU>? Last { get; }

        /// <summary>
        /// Adds a key-value pair to the dictionary.
        /// </summary>
        /// <param name="key">The key of the pair to add.</param>
        /// <param name="value">The value of the pair to add.</param>
        void Add(T key, TU value);


        /// <summary>
        /// Removes the specified key from the map.
        /// </summary>
        /// <param name="key">The key to find and remove from the map</param>
        /// <returns><c>true</c> if the element was removed; otherwise <c>false</c>.</returns>
        bool Remove(T key);

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


        /// <summary>
        /// Finds the first key/value pair whose key matches the specified value.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <param name="value">when find a key in the map then out the value.</param>
        /// <returns>true when find a matching pair, with given key, or  <c>false</c> if none is found.</returns>
        bool TryGeValue(T key, [MaybeNullWhen(false)] out TU value);
    }

    

    /// <summary>
    /// Defines a read‑only associative container mapping keys of type <typeparamref name="T"/>
    /// to values of type <typeparamref name="TU"/>.
    /// 
    /// This interface provides lookup operations, key/value enumeration and
    /// index‑based access without exposing any modification capabilities.
    /// </summary>
    /// <typeparam name="T">
    /// The key type. Must be suitable for equality comparison.
    /// </typeparam>
    /// <typeparam name="TU">
    /// The value type associated with each key.
    /// </typeparam>
    public interface IReadOnlyMap<T, TU>
        : IEnumerable<Pair<T, TU>>, IReadOnlyCollection<Pair<T, TU>> where T : notnull {
        /// <summary>
        /// Determines whether the map contains an entry for the specified key.
        /// </summary>
        /// <param name="key">The key to test for existence.</param>
        /// <returns>
        /// <c>true</c> if the key exists in the map; otherwise <c>false</c>.
        /// </returns>
        bool ContainsKey(T key);

        /// <summary>
        /// Attempts to retrieve the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value should be retrieved.</param>
        /// <param name="value">
        /// When this method returns <c>true</c>, contains the value associated with
        /// <paramref name="key"/>; otherwise <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the key exists and a value was returned; otherwise <c>false</c>.
        /// </returns>
        bool TryGeValue(T key, [MaybeNullWhen(false)] out TU value);

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value should be returned.</param>
        /// <returns>
        /// The value associated with <paramref name="key"/>.
        /// </returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown if the key does not exist in the map.
        /// </exception>
        TU this[T key] { get; }

        /// <summary>
        /// Gets an enumerable collection of all keys contained in the map.
        /// </summary>
        IEnumerable<T> Keys { get; }

        /// <summary>
        /// Gets an enumerable collection of all values contained in the map.
        /// </summary>
        IEnumerable<TU> Values { get; }
    }


#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}

