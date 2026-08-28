/* 
 * SPDX-License-Identifier: EUPL-1.2
 *
 * Copyright (c) 2026 Amber-Sophia Schröck <ambersophia.schroeck@mail.de>
 *
 * This file is licensed under the European Union Public Licence (EUPL) version 1.2.
 * You can obtain a copy of the licence at:
 *   https://joinup.ec.europa.eu/collection/eupl/eupl-text-eupl-12
 *
 * Unless required by applicable law or agreed to in writing, software distributed
 * under the Licence is distributed on an "AS IS" basis, WITHOUT WARRANTIES OR
 * CONDITIONS OF ANY KIND, either express or implied.
 *
 * If you modify this file, retain this notice and add a short description of your
 * changes and the date.
 */

namespace SystemEx.Collections.Generic {
	/// \addtogroup Collections
	/// @{

	/// <summary>
	/// Defines the generic FIFO queue.
	/// Provides operations for inserting elements at the back and removing elements from the front.
	/// </summary>
	/// <typeparam name="T">The type of elements stored in the queue.</typeparam>
	public class Queue<T> : IQueue<T>, IAutoGrowe {

        private long m_growSize;
        private bool m_autoGrow;

        /// <summary>
        /// Internal storage buffer for the queue elements.
        /// </summary>
        internal T[] m_elements;

        /// <summary>
        /// The number of elements currently stored in the queue.
        /// </summary>
        private int m_count;

        /// <summary>
        /// Gets the number of elements currently stored in the queue.
        /// </summary>
        public int Size => m_count;

        /// <summary>
        /// Gets or sets the number of elements the Vector grows by when AutoGrow is enabled.
        /// </summary>
        public long GrowSize {
            get => (m_autoGrow ? m_growSize : -1);
            set {
                m_growSize = value;
                m_autoGrow = (m_growSize > 0);
            }
        }
        /// <summary>
        /// Enables or disables automatic resizing when the Vector becomes full.
        /// </summary>
        public bool AutoGrow { 
            get {
                return (m_growSize == -1 ? false : m_autoGrow);
            }
            set {
                m_autoGrow = value;
            }
        }

        /// <summary>
        /// Indicates whether the queue contains no elements.
        /// </summary>
        public bool IsEmpty => m_count == 0;

        /// <summary>
        /// Indicates whether the queue has reached its maximum capacity.
        /// </summary>
        public bool IsFull => m_count == m_elements.Length;

        /// <summary>
        /// Gets the element at the front of the queue.
        /// </summary>
        public T Front => m_elements[0];



        /// <summary>
        /// Creates a new queue with the specified capacity.
        /// </summary>
        /// <param name="size">The maximum number of elements the queue can hold.</param>
        /// <param name="growSize">Number of elements to add when automatic growth occurs.</param>
        public Queue ( int size, int growSize ) {
            m_elements = new T[size];
            m_count = 0;
            GrowSize = growSize;
        }

        /// <summary>
        /// Creates a new Queue using an existing buffer.
        /// The buffer is adopted as-is, and Count is set
        /// to the last valid index. 
        /// </summary>
        /// <param name="e">
        /// Existing array used as the internal storage.
        /// </param>
        /// <param name="growSize">
        /// Number of elements to add when automatic growth occurs.
        /// </param>
        public Queue ( T[] e, int growSize = 16 ) {
            m_elements = e;
            m_count = e.Length;

            GrowSize = growSize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public Queue ( Queue<T> other ) {
            m_count = other.m_count;
            m_elements = new T[other.m_count];
            m_growSize = other.m_growSize;
            m_autoGrow = other.m_autoGrow;
            Buffer.LongCopy<T>(other.m_elements, 0, m_elements, 0, (long)other.Size);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public Queue ( Deque<T> other ) {
            m_count = other.Count;
            m_elements = new T[m_count];
            m_growSize = other.GrowSize;
            m_autoGrow = other.AutoGrow;
            Buffer.LongCopy<T>(other.m_elements, 0, m_elements, 0, (long)other.Size);
        }

        /// <summary>
        /// Removes all elements from the queue.
        /// </summary>
        public void Clear () {
            m_count = 0;
        }

        /// <summary>
        /// Removes the element at the front of the queue.  
        /// Remaining elements are shifted one position to the left.
        /// </summary>
        /// <param name="value">Receives the removed element.</param>
        /// <returns><c>true</c> if an element was removed; otherwise <c>false</c>.</returns>
        public bool PopFront ( ref Optional<T> value ) {
            if ( IsEmpty ) {
                return false;
            }

            value = m_elements[0];

            // Shift all elements to the left
            for ( int i = 0 ; i < m_count - 1 ; i++ )
                m_elements[i] = m_elements[i + 1];

            m_count--;
            return true;
        }


        /// <summary>
        /// Adds an element to the back of the queue if space is available.
        /// </summary>
        /// <param name="value">The element to add.</param>
        public bool PushBack ( T value ) {
			if ( IsFull ) {
				if ( !AutoGrow )
					return false;

				Grow();
			}
			m_elements[m_count] = value;
            m_count++;
            return true;
        }

        /// <summary>
        /// Creates a FlexSpan view over the current contents of the Queue.
        /// The span directly references the internal array and does not allocate.
        ///
        /// </summary>

        public static QueueFlexSpan<T> AsFlexSpan ( ref Queue<T> que, FlexSpanMode mode = FlexSpanMode.System ) {
            return new QueueFlexSpan<T>(ref que, 0, que.m_count, mode);
        }


       // public static UniqueQueue<T> AsUniqueQueue(ref Queue<T> que)
        /// <summary>
        /// Grows the internal buffer by GrowSize if AutoGrow is enabled.
        /// </summary>
        /// <returns>
        /// True if growth succeeded; false if AutoGrow was disabled.
        /// </returns>
        public bool Grow () {
            if ( !AutoGrow ) return false;
            return Resize(GrowSize);
        }
        /// <summary>
        /// Resizes the internal buffer to the specified size.
        /// Adjusts the logical index if it exceeds the new size.
        /// </summary>
        /// <param name="size">New buffer size.</param>
        /// <returns>
        /// True if resizing succeeded; false if resizing was unnecessary or failed.
        /// </returns>
        private bool Resize ( long size ) {
            long realSize = m_elements.Length + size;

            try {
                Array.Resize(ref m_elements, (int)realSize);
            } catch {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Adds an element to the back of the queue.
        /// </summary>
        /// <param name="value">The element to enqueue.</param>
        public void Enqueue ( T value ) => PushBack(value);

        /// <summary>
        /// Removes the element at the front of the queue.
        /// </summary>
        /// <param name="value">Receives the removed element.</param>
        /// <returns><c>true</c> if an element was removed; otherwise <c>false</c>.</returns>
        public bool Dequeue ( ref Optional<T> value ) => PopFront(ref value);




        internal Optional<T> ElementAt(long index) {
            if ( index >= m_elements.Length ) return Optional<T>.NONE;

            return m_elements[index];
        }
        internal bool Replace ( long pos, T? value ) {
            if ( pos >= m_elements.Length ) return false;
            if ( value == null ) return false;

            m_elements[pos] = value;

            return true;
        }
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
