using System;
using System.Text;
using SystemEx.Collections.Generic;
using SystemEx.Collections.Generic.Interfaces;


namespace SystemEx.Collections.Model {
    /// \addtogroup model
    /// @{
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
        private Vector<GenericNode<T>> m_grouped;

        /// <summary>
        /// Returns an iterator positioned at the first grouped node.
        /// </summary>
        /// <remarks>
        /// This provides STL‑style access to the grouped collection without exposing
        /// the underlying array implementation.
        /// </remarks>
        //public ArrayRandomAccessIterator<GenericNode<T>> GroupBegin () => m_grouped.First;

        /// <summary>
        /// Returns an iterator positioned one past the last grouped node.
        /// </summary>
        /// <remarks>
        /// Suitable for range‑based algorithms and STL‑style iteration.
        /// </remarks>
       // public ArrayRandomAccessIterator<GenericNode<T>> GroupEnd () => m_grouped.End;

        /// <summary>
        /// Returns an iterator positioned at the specified index within the group.
        /// </summary>
        /// <param name="index">Zero‑based index of the grouped node.</param>
        //public ArrayRandomAccessIterator<GenericNode<T>> GroupAt ( int index ) => m_grouped.At(index);

        /// <summary>
        /// Gets the total capacity of the grouped node array.
        /// </summary>
        public long Size => m_grouped.Length;

        /// <summary>
        /// Gets the first grouped node.
        /// </summary>
        /// <remarks>
        /// Throws if the group is empty.
        /// </remarks>
        public GenericNode<T> Front => m_grouped.Front;

        /// <summary>
        /// Gets the last grouped node.
        /// </summary>
        /// <remarks>
        /// Throws if the group is empty.
        /// </remarks>
        public GenericNode<T> Back => m_grouped.Back;

        /// <summary>
        /// Initializes an empty grouped node with default value and capacity.
        /// </summary>
        public GroupedNode () : base() {
            m_grouped = new Vector<GenericNode<T>>(8);
        }

        /// <summary>
        /// Initializes a grouped node with the specified value.
        /// </summary>
        /// <param name="value">The value stored in this node.</param>
        public GroupedNode ( T? value ) : base(value) {
            m_grouped = new Vector<GenericNode<T>>(8);
        }

        /// <summary>
        /// Copy constructor: copies the value and initializes a new empty group.
        /// </summary>
        /// <remarks>
        /// The grouped nodes are not deep‑copied.  
        /// This preserves lightweight clone semantics consistent with SystemEx.
        /// </remarks>
        public GroupedNode ( GroupedNode<T> other ) : base(other) {
            m_grouped = new Vector<GenericNode<T>>(8);
        }

        /// <summary>
        /// Appends a node to the grouped collection.
        /// </summary>
        /// <param name="node">The node to append.</param>
        public void PushBack ( GenericNode<T> node ) {
            m_grouped.PushBack(node);
        }
    }
/// @}
}
