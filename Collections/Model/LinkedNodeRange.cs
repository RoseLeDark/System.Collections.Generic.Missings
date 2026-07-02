using System.Collections;


namespace SystemEx.Collections.Model {
    /// \addtogroup collections
    /// @{
    /// <summary>
    /// Represents a simple range defined by a begin and end iterator over
    /// intrusive <see cref="LinkedNode{T}"/> structures.  
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
    public class LinkedNodeRange<T> : IEnumerable<T> {

        /// <summary>
        /// Iterator marking the beginning of the range (inclusive).
        /// </summary>
        private LinkedNodeIterrator<T> m_begin;

        /// <summary>
        /// Iterator marking the end of the range (exclusive).
        /// </summary>
        private LinkedNodeIterrator<T> m_end;

        /// <summary>
        /// Creates a new range from the specified begin and end iterators.
        /// </summary>
        /// <param name="begin">The iterator marking the start of the range.</param>
        /// <param name="end">The iterator marking the end of the range.</param>
        public LinkedNodeRange ( LinkedNodeIterrator<T> begin, LinkedNodeIterrator<T> end ) {
            m_begin = begin;
            m_end = end;
        }

        /// <summary>
        /// Returns an enumerator that iterates from <c>begin</c> up to,
        /// but not including, <c>end</c>.
        /// </summary>
        public IEnumerator<T> GetEnumerator () {
            LinkedNodeIterrator<T> it = (LinkedNodeIterrator<T>)m_begin.Clone();
            if ( it == null )
                throw new NotSupportedException();

            while ( !it.Equals(m_end) ) {
                if(it.Current != null)
                    yield return it.Current;

                it.Forward();
            }
        }

        IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
    }
    /// @}
}
