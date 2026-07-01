using System;
using System.Text;
using SystemEx.Collections.Generic;
using SystemEx.Collections.Generic.Interfaces;

namespace SystemEx.Collections.Model {
    /// <summary>
    /// Non‑intrusive grouped node storing a value and an expandable collection
    /// of associated <see cref="GenericNode{T}"/> instances.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="GroupedNode{T}"/> extends <see cref="GenericNode{T}"/> with
    /// an internal dynamic array used to store auxiliary nodes that are not part
    /// of the intrusive <c>Prev</c>/<c>Next</c> chain.
    /// </para>
    ///
    /// <para>
    /// This type is ideal for representing metadata collections, sibling groups,
    /// annotations, ownership histories, or secondary node clusters that must not
    /// interfere with the primary intrusive list structure.
    /// </para>
    ///
    /// <para>
    /// The grouped nodes are stored in a simple <c>Array&lt;GenericNode&lt;T&gt;&gt;</c>
    /// and do not participate in intrusive operations such as
    /// <c>InsertBefore</c>, <c>InsertAfter</c>, <c>Erase</c>, <c>Splice</c>,
    /// or <c>SwapWith</c>.
    /// </para>
    /// </remarks>
    public class GroupedNode<T> : GenericNode<T> {
        private Array<GenericNode<T>> m_grouped;

        public ArrayRandomAccessIterator<GenericNode<T>> GroupBegin() => m_grouped.First;
        public ArrayRandomAccessIterator<GenericNode<T>> GroupEnd()   => m_grouped.End;

        public ArrayRandomAccessIterator<GenericNode<T>> GroupAt ( int index ) => m_grouped.At(index);

        /// <summary>
        /// Gets the total capacity of the array.
        /// </summary>
        public int Size => m_grouped.Size;
        /// <summary>
        /// Gets the first element of the array.
        /// </summary>
        public GenericNode<T> Front => m_grouped.Front;
        /// <summary>
        /// Gets the last element of the array.
        /// </summary>
        public GenericNode<T> Back => m_grouped.Back;

        /// <summary>
        /// Initializes an empty grouped node with default value and capacity.
        /// </summary>
        public GroupedNode () : base() {
            m_grouped = new Array<GenericNode<T>>(8);
        }

        /// <summary>
        /// Initializes a grouped node with the specified value.
        /// </summary>
        /// <param name="value">The value stored in this node.</param>
        public GroupedNode ( T? value ) : base(value) {
            m_grouped = new Array<GenericNode<T>>(8);
        }

        /// <summary>
        /// Copy constructor: copies the value and initializes a new empty group.
        /// </summary>
        /// <remarks>
        /// The grouped nodes are not deep‑copied.  
        /// This preserves lightweight clone semantics consistent with SystemEx.
        /// </remarks>
        public GroupedNode ( GroupedNode<T> other ) : base(other) {
            m_grouped = new Array<GenericNode<T>>(8);
        }

        /// <summary>
        /// Appends a node to the grouped collection.
        /// </summary>
        /// <param name="node">The node to append.</param>
        public void PushBack ( GenericNode<T> node ) {
            m_grouped.Add(node);
        }
    }
}
