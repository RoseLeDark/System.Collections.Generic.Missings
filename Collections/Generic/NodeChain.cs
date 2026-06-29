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

using System.Collections;
using SystemEx.Collections.Generic.Interfaces;

namespace SystemEx.Collections.Generic {
    /// \addtogroup collections
    /// @{

    /// <summary>
    /// Represents a chained view over multiple <see cref="NodeRange{T}"/> segments,
    /// similar to <c>std::views::concat</c> in C++.  
    /// A <see cref="NodeChain{T}"/> allows iteration across several disjoint
    /// ranges of a linked <see cref="Node{T}"/> structure as if they formed
    /// one continuous sequence.
    /// </summary>
    /// <typeparam name="T">The value type stored in the underlying nodes.</typeparam>
    /// <example>
    /// var chain = new NodeChain&lt;int&gt;()
    ///     .Add(node.First(), node.At(5))
    ///     .Add(node.At(10), node.At(20))
    ///     .Add(node.At(30), node.End());
    ///
    /// foreach (var x in chain)
    ///     Console.WriteLine(x);
    /// </example>
    public sealed class NodeChain<T> : IEnumerable<T> {

        /// <summary>
        /// Internal list of ranges that make up the chain.
        /// </summary>
        private readonly List<NodeRange<T>> m_ranges = new();

        /// <summary>
        /// Adds an existing <see cref="NodeRange{T}"/> to the chain.
        /// </summary>
        /// <param name="range">The range to append.</param>
        /// <returns>The chain itself for fluent chaining.</returns>
        public NodeChain<T> Add(NodeRange<T> range) {
            m_ranges.Add(range);
            return this;
        }

        /// <summary>
        /// Adds a new range defined by a begin and end iterator.
        /// </summary>
        /// <param name="begin">The iterator marking the start of the range.</param>
        /// <param name="end">The iterator marking the end of the range.</param>
        /// <returns>The chain itself for fluent chaining.</returns>
        public NodeChain<T> Add(NodeIterrator<T> begin, NodeIterrator<T> end) {
            m_ranges.Add(new NodeRange<T>(begin, end));
            return this;
        }

        /// <summary>
        /// Returns an enumerator that iterates through all ranges in sequence.
        /// </summary>
        public IEnumerator<T> GetEnumerator() {
            foreach ( var range in m_ranges ) {
                foreach ( var x in range )
                    yield return x;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.

}
