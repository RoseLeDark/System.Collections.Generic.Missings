using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic.Missings {
    public class GroupNode<T> : Node<T> {
        private Node<T>? m_head;

        public GroupNode(T value) : base(value) {
            m_head = null;
        }

        public bool IsEmpty => m_head == null;

        public void Add(Node<T> node) {
            if ( m_head == null ) {
                // Erste Node in der Gruppe
                m_head = node;
                node.Next = node;
                node.Prev = node;
                return;
            }

            // Einfügen am Ende der Liste
            var last = m_head.Prev;

            last.Next = node;
            node.Prev = last;

            node.Next = m_head;
            m_head.Prev = node;
        }

        public void Remove(Node<T> node) {
            if ( m_head == null )
                return;

            if ( node.Next == node && node.Prev == node ) {
                // Einzelnes Element
                m_head = null;
                return;
            }

            if ( m_head == node )
                m_head = node.Next;

            node.Prev.Next = node.Next;
            node.Next.Prev = node.Prev;

            node.Next = node;
            node.Prev = node;
        }

        public NodeIterrator<T> First() {
            if ( m_head == null )
                throw new InvalidOperationException("Group is empty.");

            return new NodeIterrator<T>(m_head);
        }

        public NodeIterrator<T> End() {
            if ( m_head == null )
                throw new InvalidOperationException("Group is empty.");

            return new NodeIterrator<T>(m_head.Prev);
        }

        public IEnumerable<Node<T>> Nodes {
            get {
                if ( m_head == null )
                    yield break;

                var it = m_head;
                do {
                    yield return it;
                    it = it.Next;
                }
                while ( it != m_head );
            }
        }

        public override void Travers(TraversOrder order, Action<Node<T>> action) {
            foreach ( var n in Nodes )
                action(n);
        }
        public NodeRange<T> AsRange() {
            return new NodeRange<T>(First(), End());
        }

        public NodeSlice<T> Slice(int start, int length) {
            return new NodeSlice<T>((NodeIterrator<T>)First().Advance(start) , length);
        }

        public NodeChain<T> AsChain() {
            return new NodeChain<T>().Add(First(), End());
        }

    }


}
