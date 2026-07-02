using System;
using System.Text;
using SystemEx.Collections.Generic;

namespace SystemEx.Collections.Model {
    /// \addtogroup model
    /// @{

    /// <summary>
    /// Represents a chained view over multiple <see cref="LinkedNodeRange{T}"/> segments,
    /// similar to <c>std::views::concat</c> in C++.  
    /// A <see cref="LinkedNodeChain{T}"/> allows iteration across several disjoint
    /// ranges of a linked <see cref="Node{T}"/> structure as if they formed
    /// one continuous sequence.
    /// </summary>
    /// <typeparam name="T">The value type stored in the underlying nodes.</typeparam>
    /// <example>
    /// var chain = new LinkedNodeChain&lt;int&gt;()
    ///     .Add(node.First(), node.At(5))
    ///     .Add(node.At(10), node.At(20))
    ///     .Add(node.At(30), node.End());
    ///
    /// foreach (var x in chain)
    ///     Console.WriteLine(x);
    /// </example>
    public sealed class LinkedNodeChain<T> : IEnumerable<T> {

        /// <summary>
        /// Internal list of ranges that make up the chain.
        /// </summary>
        private readonly List<LinkedNodeRange<T>> m_ranges = new();

        /// <summary>
        /// Adds an existing <see cref="LinkedNodeRange{T}"/> to the chain.
        /// </summary>
        /// <param name="range">The range to append.</param>
        /// <returns>The chain itself for fluent chaining.</returns>
        public LinkedNodeChain<T> Add ( LinkedNodeRange<T> range ) {
            m_ranges.Add(range);
            return this;
        }

        /// <summary>
        /// Adds a new range defined by a begin and end iterator.
        /// </summary>
        /// <param name="begin">The iterator marking the start of the range.</param>
        /// <param name="end">The iterator marking the end of the range.</param>
        /// <returns>The chain itself for fluent chaining.</returns>
        public LinkedNodeChain<T> Add ( LinkedNodeIterrator<T> begin, LinkedNodeIterrator<T> end ) {
            m_ranges.Add(new LinkedNodeRange<T>(begin, end));
            return this;
        }

        /// <summary>
        /// Returns an enumerator that iterates through all ranges in sequence.
        /// </summary>
        public IEnumerator<T> GetEnumerator () {
            foreach ( var range in m_ranges ) {
                foreach ( var x in range )
                    yield return x;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator () {
            return GetEnumerator();
        }
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.

}
