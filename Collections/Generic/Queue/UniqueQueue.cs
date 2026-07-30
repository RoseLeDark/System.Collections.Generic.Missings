using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Collections.Generic {

    /// <summary>
    /// Represents a queue that only accepts unique elements.
    /// Attempts to insert a value already contained in the queue will fail.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the queue.</typeparam>
    public struct UniqueQueue<T> : IQueue<T> {
        private Queue<T> m_queue;

        /// <summary>
        /// Gets the number of elements currently stored in the queue.
        /// </summary>
        public int Size =>  m_queue.Size;

        /// <summary>
        /// Gets a value indicating whether the queue contains no elements.
        /// </summary>
        public bool IsEmpty =>  m_queue.IsEmpty;
        /// <summary>
        /// Gets a value indicating whether the queue has reached its maximum capacity.
        /// </summary>
        public bool IsFull =>  m_queue.IsFull;
        /// <summary>
        /// Removes all elements from the queue.
        /// </summary>
        public void Clear () => m_queue.Clear();
        /// <summary>
        /// Removes the element at the front of the queue.
        /// </summary>
        /// <param name="value">
        /// When this method returns, contains the removed element if the operation succeeded;
        /// otherwise contains <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if an element was removed; otherwise <c>false</c>.
        /// </returns>
        public bool PopFront ( ref Optional<T> value ) => m_queue.PopFront(ref value);
        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueQueue{T}"/> struct.
        /// </summary>
        /// <param name="size">The initial capacity of the queue.</param>
        /// <param name="growSize">
        /// The number of additional elements to allocate when the queue grows.
        /// </param>
        public UniqueQueue (int size, int growSize = 2 ) {
            m_queue = new Queue<T>(size, growSize);
        }
        /// <summary>
        /// Inserts an element at the back of the queue if it is not already present.
        /// </summary>
        /// <param name="value">The element to insert.</param>
        /// <returns>
        /// <c>true</c> if the element was successfully inserted;
        /// <c>false</c> if the element already exists in the queue or the queue is full.
        /// </returns>
        public bool PushBack ( T value ) {
            bool _ret = false;

            if(!Contains(value))
                _ret = m_queue.PushBack(value);
            return _ret;
        }
        /// <summary>
        /// Determines whether the specified value already exists in the queue.
        /// </summary>
        /// <param name="value">The value to search for.</param>
        /// <returns>
        /// <c>true</c> if the value is already contained in the queue;
        /// otherwise <c>false</c>.
        /// </returns>
        private bool Contains(T value) {
            bool _ret = false;
            QueueFlexSpan<T> span = Queue < T >.AsFlexSpan(ref m_queue, FlexSpanMode.System);

            for ( int i = 0 ; i < span.Length ; i++ ) {
                var item = span.ElementAt(i);
                if ( item == null ) continue;

                if ( item.Equals(value) ) {
                    _ret = true;
                    break;
                }
            }
            return _ret;
        }
    }
}
