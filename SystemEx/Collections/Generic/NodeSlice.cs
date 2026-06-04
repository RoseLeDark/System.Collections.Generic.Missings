using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Collection.Generic {
    /// <summary>
    /// Ein Slice ist ein Range mit Start + Länge, wie Span<T>.
    /// <example>
    /// var slice = new NodeSlice<int>(node.At(5), 10);
    /// foreach (var x in slice)
    ///      Console.WriteLine(x);
    /// </example>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct NodeSlice<T> : IEnumerable<T> {
        private readonly NodeIterrator<T> m_Begin;
        private readonly int m_Length;

        public NodeSlice(NodeIterrator<T> begin, int length) {
            m_Begin = begin;
            m_Length = length;
        }

        public NodeIterrator<T>? End {
            get {
                var it = m_Begin.Clone() as NodeIterrator<T>;
                if ( it != null ) {
                    it.Advance(m_Length);
                }
                return it;
            }
        }

        public IEnumerator<T> GetEnumerator() {
            NodeIterrator<T> it = (NodeIterrator<T>)m_Begin.Clone();
            int count = 0;

            while ( count < m_Length ) {
                yield return it.Current;
                it.Forward();
                count++;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

}
