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


namespace SystemEx.Collections.Generic {
    /// \addtogroup collections
    /// @{
    /// <summary>
    /// Represents a simple range defined by a begin and end iterator over
    /// intrusive <see cref="Node{T}"/> structures.  
    /// Conceptually similar to <c>std::ranges::subrange</c>, but designed
    /// specifically for your intrusive node and iterator system.
    /// </summary>
    /// <typeparam name="T">The value type stored in the underlying nodes.</typeparam>
    /// <example>
    /// var range = new NodeRange&lt;int&gt;(node.First(), node.At(10));
    ///
    /// foreach (var x in range)
    ///     Console.WriteLine(x);
    /// </example>
    public struct NodeRange<T> : IEnumerable<T> {

        /// <summary>
        /// Iterator marking the beginning of the range (inclusive).
        /// </summary>
        private NodeIterrator<T> m_begin;

        /// <summary>
        /// Iterator marking the end of the range (exclusive).
        /// </summary>
        private NodeIterrator<T> m_end;

        /// <summary>
        /// Creates a new range from the specified begin and end iterators.
        /// </summary>
        /// <param name="begin">The iterator marking the start of the range.</param>
        /// <param name="end">The iterator marking the end of the range.</param>
        public NodeRange(NodeIterrator<T> begin, NodeIterrator<T> end) {
            m_begin = begin;
            m_end = end;
        }

        /// <summary>
        /// Returns an enumerator that iterates from <c>begin</c> up to,
        /// but not including, <c>end</c>.
        /// </summary>
        public IEnumerator<T> GetEnumerator() {
            var it = m_begin as NodeIterrator<T>;
            if ( it == null )
                throw new NotSupportedException();

            while ( !it.Equals(m_end) ) {
                yield return it.Current;
                it.Forward();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
