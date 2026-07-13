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

using SystemEx.Utils;
using SystemEx.Collections.Generic;

namespace SystemEx.Collections.Generic.Interfaces {
    /// \addtogroup collections
    /// @{
    /// \addtogroup interfaces
    /// @{
    /// <summary>
    /// Delegate used to compare two key/value pairs and return their ordering relation.
    /// </summary>
    /// <typeparam name="T">The key type.</typeparam>
    /// <typeparam name="TU">The value type.</typeparam>
    /// <param name="a">The first pair to compare.</param>
    /// <param name="b">The second pair to compare.</param>
    /// <returns>A <see cref="CompareResult"/> describing the ordering.</returns>
    public delegate CompareResult SortFunc<T, TU>(Pair<T, TU> a, Pair<T, TU> b);

    /// <summary>
    /// Delegate used to compare two tuples and return their ordering relation.
    /// </summary>
    /// <param name="a">The first tuple.</param>
    /// <param name="b">The second tuple.</param>
    /// <returns>A <see cref="CompareResult"/> describing the ordering.</returns>
    public delegate CompareResult SortTupleFunc(ITuple a, ITuple b);

    /// <summary>
    /// Delegate used to compare two objects of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type being compared.</typeparam>
    /// <param name="a">The first object.</param>
    /// <param name="b">The second object.</param>
    /// <returns>A <see cref="CompareResult"/> describing the ordering.</returns>
    public delegate CompareResult SortObjectFunc<T>(T a, T b);

    /// <summary>
    /// Extends <see cref="IArray{T}"/> with sorting capabilities using either
    /// a comparer interface or a delegate-based sort function.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the array.</typeparam>
    public interface ISortedArray<T> : IArray<T> {

        /// <summary>
        /// Gets or sets the delegate-based sort function used to compare elements.
        /// </summary>
        SortObjectFunc<T> SortFunctions { get; set; }

        /// <summary>
        /// Gets or sets the comparer interface used to compare elements.
        /// If set, it overrides the delegate-based comparison.
        /// </summary>
        ICompared<T>? Comparer { get; set; }

        /// <summary>
        /// Enables or disables automatic sorting after modification operations.
        /// </summary>
        bool AutoSort { get; set; }

        /// <summary>
        /// Sorts the entire array using the configured comparison method.
        /// </summary>
        void Sort();

        /// <summary>
        /// Returns a new array containing the same elements but without sorting behavior.
        /// </summary>
        /// <returns>An <see cref="IArray{T}"/> containing the unsorted elements.</returns>
        IArray<T> ToUnorderedArray();
    }

    /// <summary>
    /// Extends <see cref="IMap{T, TU}"/> with sorting capabilities for key/value pairs.
    /// Sorting can be performed using a delegate or a comparer interface.
    /// </summary>
    /// <typeparam name="T">The key type.</typeparam>
    /// <typeparam name="TU">The value type.</typeparam>
    public interface ISortedMap<T, TU> : IMap<T, TU> where T : notnull {

        /// <summary>
        /// Gets or sets the delegate-based sort function used to compare key/value pairs.
        /// </summary>
        SortFunc<T, TU> SortFunctions { get; set; }

        /// <summary>
        /// Gets or sets the comparer interface used to compare key/value pairs.
        /// If set, it overrides the delegate-based comparison.
        /// </summary>
        ICompared<IPair<T, TU>>? Comparer { get; set; }

        /// <summary>
        /// Enables or disables automatic sorting after modification operations.
        /// </summary>
        bool AutoSort { get; set; }

        /// <summary>
        /// Sorts the entire map using the configured comparison method.
        /// </summary>
        void Sort();

        /// <summary>
        /// Returns a new map containing the same elements but without sorting behavior.
        /// </summary>
        /// <returns>An <see cref="IMap{T, TU}"/> containing the unsorted elements.</returns>
        IMap<T, TU> ToUnorderedMap();
    }

    /// <summary>
    /// Extends <see cref="ITupleMap"/> with sorting capabilities for tuple elements.
    /// Sorting can be performed using a delegate or a comparer interface.
    /// </summary>
    public interface ISortedTupleMap : ITupleMap {

        /// <summary>
        /// Gets or sets the delegate-based sort function used to compare tuples.
        /// </summary>
        SortTupleFunc SortFunctions { get; set; }

        /// <summary>
        /// Gets or sets the comparer interface used to compare tuples.
        /// If set, it overrides the delegate-based comparison.
        /// </summary>
        ICompared<ITuple>? Comparer { get; set; }

        /// <summary>
        /// Enables or disables automatic sorting after modification operations.
        /// </summary>
        bool AutoSort { get; set; }

        /// <summary>
        /// Sorts the entire tuple map using the configured comparison method.
        /// </summary>
        void Sort();

        /// <summary>
        /// Returns a new tuple map containing the same elements but without sorting behavior.
        /// </summary>
        /// <returns>An <see cref="ITupleMap"/> containing the unsorted elements.</returns>
        ITupleMap ToUnorderedMap();
    }
}
