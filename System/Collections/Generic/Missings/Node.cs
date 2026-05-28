using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic.Missings {
    public class Node<T> {
        public Node<T> Next { get; internal set; }
        public Node<T> Prev { get; internal set; }

        public T? Value { get; set; }

        public Node(T val) {
            Value = val;
            Prev = Next = this;
        }
        public bool HasNext => Next != this;

        public bool HasPrev => Prev != this;



        public Node<T>? Root() {
            if ( Prev == null ) return this;

            return HasPrev ? Prev.Root() : this;
        }

        public Node<T> Last() {
            if ( Next == null ) return this;

            return HasNext ? Next.Last() : this;
        }
        public void Insert(ref Node<T> pNext) {
            Next = pNext;
            Prev = pNext.Prev;
            pNext.Prev.Next = this;
            pNext.Prev = this;
        }
        public void remove() {
            Next.Prev = Prev;
            Prev.Next = Next;
        }

        public void Splice(ref Node<T> first, ref Node<T> last) {
            last.Prev.Next = this;
            first.Prev.Next = last;
            this.Prev.Next = first;

            Node<T> pTemp = this.Prev;
            this.Prev = last.Prev;
            last.Prev = first.Prev;
            first.Prev = pTemp;
        }
        /**
        * @brief Reverses the order of nodes in the circular  this node is a part of.
        */
        public void Reverse() {
            Node<T>  pNode = this;

            do {
                if ( pNode != null ) {
                    Node<T> pTemp = pNode.Next;
                    pNode.Next = pNode.Prev;
                    pNode.Prev = pTemp;
                    pNode = pNode.Prev;
                }
            } while ( pNode != this );
        }
        public void InsertRage(ref Node<T> pFirst, ref Node<T> pFinal) {
            Prev.Next = pFirst; pFirst.Prev = Prev;
            Prev = pFinal; pFinal.Next = this;
        }
        /**
         * @brief remove a range of elements
         */
        public static void remove_range(ref Node<T> pFirst, ref Node<T> pFinal) {
            pFinal.Next.Prev = pFirst.Prev;
            pFirst.Prev.Next = pFinal.Next;
        }

    }

}
