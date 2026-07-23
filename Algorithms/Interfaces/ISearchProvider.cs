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
using SystemEx.Algorythmen;
using SystemEx.Collections.Generic;
using SystemEx.Collections.Generic.Interfaces;
using SystemEx.Utils;

namespace SystemEx.Algorithms.Interfaces {
    /// \addtogroup Algorithms
    /// @{
    /// \addtogroup interfaces
    /// @{
    /// <summary>
    /// Defines a pluggable search strategy for use with the <c>Search</c> object.
    /// 
    /// A search provider encapsulates the lookup logic and can implement
    /// any strategy: linear scan, binary search, segmented search, pattern
    /// matching, or domain‑specific evaluation.
    /// 
    /// The provider receives the container by reference to allow efficient
    /// access without copying. It never modifies the container.
    /// </summary>
    /// <typeparam name="TContainer">
    /// The container type implementing <see cref="IVector{T}"/>.
    /// </typeparam>
    /// <typeparam name="T">
    /// The element type stored in the container.
    /// </typeparam>
    public interface ISearchProvider<T, TContainer>
        where TContainer : IVector<T> {
        /// <summary>
        /// Searches the container for the specified value using the provider's
        /// lookup strategy.
        /// </summary>
        /// <param name="container">The container to search.</param>
        /// <param name="value">The value to locate.</param>
        /// <param name="comp">The comparer</param>
        /// <returns>
        /// The index of the first matching element, or -1 if no match is found.
        /// </returns>
        long Find ( ref TContainer container, ICompared<T> comp, T value );

        /// <summary>
        /// Searches the container using a custom predicate callback.
        /// The callback receives each element and a boolean flag that can be
        /// used to indicate a match condition.
        /// </summary>
        /// <param name="container">The container to search.</param>
        /// <param name="func">
        /// A predicate callback invoked for each element. The callback receives
        /// the element and a boolean indicating whether it matches the desired
        /// condition.
        /// </param>
        /// <returns>
        /// The index of the first element for which the callback indicates a
        /// match, or -1 if no element satisfies the predicate.
        /// </returns>
        long Find ( ref TContainer container, Func<T?, CompareResult> func );


        /// <summary>
        /// Performs a full linear scan over the container and returns all elements
        /// that satisfy the given predicate together with their positions.
        /// 
        /// <para>
        /// Unlike <c>Find</c>, which only returns the number of matches,
        /// <c>Where</c> provides detailed match information. Each result contains
        /// both the index of the element inside the container and the element
        /// itself. This is especially useful when <typeparamref name="T"/> is a
        /// reference type and positional context is required.
        /// </para>
        /// </summary>
        /// <param name="container">The container to search.</param>
        /// <param name="func">Predicate used to test each element.</param>
        /// <returns>
        /// A <see cref="Vector{T}"/> containing all matching elements.
        /// Each pair stores:
        /// <list type="bullet">
        /// <item><description>The index of the element in the container</description></item>
        /// <item><description>The element itself</description></item>
        /// </list>
        /// </returns>

        Vector< Pair<long, T> > Where ( ref TContainer container, Func<T?, CompareResult> func );
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
