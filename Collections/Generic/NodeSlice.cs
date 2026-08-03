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

namespace SystemEx.Collections.Generic {
    /// \addtogroup collections
    /// @{
    /// <summary>
    /// Represents a slice over intrusive <see cref="Node{T}"/> structures,
    /// defined by a starting iterator and a fixed length.  
    /// Conceptually similar to <c>Span&lt;T&gt;</c>, but operating on your
    /// intrusive node/iterator system instead of contiguous memory.
    /// </summary>
    /// <typeparam name="T">The value type stored in the underlying nodes.</typeparam>
    /// <example>
    /// var slice = new NodeSlice&lt;int&gt;(node.At(5), 10);
    ///
    /// foreach (var x in slice)
    ///     Console.WriteLine(x);
    /// </example>
    public readonly struct NodeSlice<T> : IEnumerable<T> {

        /// <summary>
        /// Iterator marking the beginning of the slice.
        /// </summary>
        private readonly NodeIterrator<T> m_Begin;

        /// <summary>
        /// Number of elements included in the slice.
        /// </summary>
        private readonly int m_Length;

        /// <summary>
        /// Creates a new slice starting at the given iterator and spanning
        /// the specified number of elements.
        /// </summary>
        /// <param name="begin">The iterator marking the start of the slice.</param>
        /// <param name="length">The number of elements in the slice.</param>
        public NodeSlice(NodeIterrator<T> begin, int length) {
            m_Begin = begin;
            m_Length = length;
        }

        /// <summary>
        /// Gets an iterator positioned at the end of the slice
        /// (i.e., <c>begin + length</c>).
        /// </summary>
        public NodeIterrator<T>? End {
            get {
                var it = m_Begin as NodeIterrator<T>;
                if ( it != null ) {
                   
                }
                return it;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the slice from
        /// <c>begin</c> up to <c>begin + length</c>.
        /// </summary>
        public IEnumerator<T> GetEnumerator() {
            NodeIterrator<T> it = (NodeIterrator<T>)m_Begin;
            int count = 0;

            while ( count < m_Length ) {
                yield return it.Current;
                it.Forward();
                count++;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
