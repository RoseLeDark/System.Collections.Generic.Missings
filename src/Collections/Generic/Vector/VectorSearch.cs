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

using SystemEx.Algorithms;
using SystemEx.Utils;

namespace SystemEx.Collections.Generic {
	/// \addtogroup Collections
	/// @{

	/// <summary>
	/// Provides an extensible search wrapper over a container using a configurable
	/// <see cref="ISearchProvider{TContainer, T}"/> strategy.
	///
	/// <para>
	/// <b>Purpose:</b>
	/// The <c>Search</c> struct is designed for advanced and domain‑specific
	/// search algorithms that go beyond the simple linear scan implemented by
	/// <see cref="Find{T, TContainer}"/>. While <c>Find</c> offers a fixed,
	/// straightforward lookup, <c>Search</c> allows plugging in custom providers
	/// such as binary search, segmented search, pattern matching, or any
	/// specialized logic required by the application.
	/// </para>
	///
	/// <para>
	/// <b>Fallback behavior:</b>
	/// A <c>LinearSearchProvider</c> may be used as a fallback, but it is not the
	/// primary purpose of this type. <c>Search</c> exists to support complex,
	/// optimized, or domain‑specific search strategies.
	/// </para>
	///
	/// <para>
	/// <b>Equality operators:</b>
	/// The operators <c>==</c> and <c>!=</c> are overloaded to provide concise
	/// existence checks:
	/// <code>
	/// if (search == value) { ... }   // value exists
	/// if (search != value) { ... }   // value does not exist
	/// </code>
	/// These operators do <b>not</b> define structural equality. They only test
	/// whether the given value is present in the underlying container.
	/// </para>
	///
	/// <para>
	/// <b>Equals(object):</b>
	/// Always returns <c>false</c>. <c>Search</c> is a <c>ref struct</c> and cannot
	/// be compared to arbitrary boxed objects. Structural equality is intentionally
	/// not defined.
	/// </para>
	/// </summary>
	public ref struct VectorSearch<T, TContainer>
        where TContainer : IVector<T>
        where T : IComparable<T> {

        private ref TContainer m_container;
        private readonly ISearchProvider<T, TContainer> m_provider;

        /// <summary>
        /// Creates a new Search object bound to the given container and
        /// search provider.
        /// </summary>
        /// <param name="container">The container to search.</param>
        /// <param name="provider">
        /// The search provider implementing the lookup strategy.
        /// </param>
        public VectorSearch ( ref TContainer container, ISearchProvider<T, TContainer> provider ) {
            m_container = ref container;
            m_provider = provider;
        }

        /// <summary>
        /// Searches for the specified value using the provider's strategy.
        /// </summary>
        /// <param name="value">The value to search for.</param>
        /// <param name="comp"></param>
        /// <returns>
        /// The index of the first matching element, or -1 if not found.
        /// </returns>
        public long Find ( T value, ICompared<T> comp ) {
            return m_provider.Find(ref m_container, comp, value);
        }
        /// <summary>
        /// Performs a predicate‑based search using a callback function.
        /// The provider determines how the predicate is evaluated.
        /// </summary>
        /// <param name="func">
        /// A callback invoked for each element. The callback receives the
        /// element and a boolean flag indicating whether it matches.
        /// </param>
        /// <returns>
        /// The index of the first element for which the predicate indicates
        /// a match, or -1 if no element satisfies the condition.
        /// </returns>
        public long FindEx(Func< Optional<T>, CompareResult> func ) {
            return m_provider.Find(ref m_container, func);
        }

        public Vector<Pair<long, Optional<T> >> Where ( Func<Optional<T>, CompareResult> func ) {
            return m_provider.Where(ref m_container, func);
        }

        /// <summary>
        /// Determines whether the specified value exists in the container.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns>
        /// True if the value exists; otherwise false.
        /// </returns>
        public bool Exists ( T value, ICompared<T> comp ) {
            return Find(value, comp) >= 0;
        }
        /// <summary>
        /// Checks whether the search object contains the specified value.
        /// Equivalent to <see cref="Exists(T)"/>.
        /// </summary>
        public static bool operator == ( VectorSearch<T, TContainer> a, T value) {
            return a.Exists(value, new SimpleComparer<T>() );
        }
        /// <summary>
        /// Checks whether the search object does not contain the specified value.
        /// Equivalent to <c>!Exists(value)</c>.
        /// </summary>
        public static bool operator != ( VectorSearch<T, TContainer> a, T value ) {
            return !a.Exists(value, new SimpleComparer<T>() );
        }
        /// <summary>
        /// Search objects do not define structural equality.
        /// This override always returns false.
        /// </summary>
        public override bool Equals ( object? obj ) {
            return false;
        }
        /// <summary>
        /// Returns the hash code of the underlying container.
        /// </summary>
        public override int GetHashCode () {
            return m_container.GetHashCode();
        }
    }
    
}
