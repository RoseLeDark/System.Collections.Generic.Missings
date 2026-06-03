using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace System.Collections.Generic.Missings {

    /// <summary>
    /// Eine Chain ist eine View über mehrere Ranges oder Slices.  Wie std::views::concat.
    /// </summary>
    /// <example>
    /// var chain = new NodeChain<int>()
    ///     .Add(node.First(), node.At(5))
    ///     .Add(node.At(10), node.At(20)
    ///     .Add(node.At(30), node.End());
    /// foreach (var x in chain)
    ///     Console.WriteLine(x);
    /// </example>
    /// <typeparam name="T"></typeparam>
    public sealed class NodeChain<T> : IEnumerable<T> {
        private readonly List<NodeRange<T>> m_ranges = new();

        public NodeChain<T> Add(NodeRange<T> range) {
            m_ranges.Add(range);
            return this;
        }

        public NodeChain<T> Add(NodeIterrator<T> begin, NodeIterrator<T> end) {
            m_ranges.Add(new NodeRange<T>(begin, end));
            return this;
        }

        public IEnumerator<T> GetEnumerator() {
            foreach ( var range in m_ranges ) {
                foreach ( var x in range )
                    yield return x;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

}
