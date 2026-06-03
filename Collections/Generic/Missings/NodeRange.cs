using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace System.Collections.Generic.Missings {
    /// <summary>
    /// Ein Range ist einfach: begin → end. 
    /// Wie std::ranges::subrange, aber für deine intrusive Nodes.
    /// </summary>
    /// <example>
    /// var range = new NodeRange<int>(node.First(), node.At(10));
    /// 
    /// foreach (var x in range)
    /// Console.WriteLine(x);
    /// 
    /// </example>
    /// <typeparam name="T"></typeparam>
    public struct NodeRange<T> : IEnumerable<T> {
        private NodeIterrator<T> m_begin;
        private NodeIterrator<T> m_end;

        public NodeRange(NodeIterrator<T> begin, NodeIterrator<T> end) {
            m_begin = begin;
            m_end = end;
        }

        public IEnumerator<T> GetEnumerator() {
            var it = m_begin.Clone() as NodeIterrator<T>;
            if ( it == null ) throw new NotSupportedException();

            while ( !it.Equals(m_end) ) {
                yield return it.Current;
                it.Forward();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

}
