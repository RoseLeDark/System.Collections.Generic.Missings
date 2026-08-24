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

using System.Collections;

namespace SystemEx.Collections.Generic {

	/// \addtogroup SystemEx.Collections.Generic 
	/// @{
	/// <summary>
	/// A fixed-size queue that stores up to two elements. When full, the oldest
	/// element is automatically removed to make room for a new one.
	/// </summary>
	/// <typeparam name="T">The type of elements stored in the queue.</typeparam>
	public ref struct BinQueue<T> : IBinQueue<T> {

        /// <summary>
        /// Internal storage buffer for the BinQueue elements.
        /// </summary>
        private T[] m_elements;

        /// <summary>
        /// The number of elements currently stored in the BinQueue.
        /// </summary>
        private int m_count;

        /// <summary>
        /// Gets the number of elements currently stored in the BinQueue.
        /// </summary>
        public int Count => m_count;


        /// <summary>
        /// Gets the total capacity of the BinQueue.
        /// </summary>
        public int Size => 2;

        /// <summary>
        /// Indicates whether the BinQueue contains no elements.
        /// </summary>
        public bool IsEmpty => m_count == 0;

        /// <summary>
        /// Indicates whether the BinQueue has reached its maximum capacity.
        /// </summary>
        public bool IsFull => m_count == 2;

        /// <summary>
        /// Gets the element at the front of the BinQueue.
        /// </summary>
        public T Front => m_elements[0];

        /// <summary>
        /// Gets the element at the back of the BinQueue.
        /// </summary>
        public T End => m_elements[1];

        /// <summary>
        /// Creates a new BinQueue
        /// </summary>
        public BinQueue ( ) {
            m_elements = new T[2];
            m_count = 0;
        }


        // <summary>
        /// Adds a new element to the queue. If the queue is full, the oldest
        /// element is removed automatically before inserting the new one.
        /// </summary>
        /// <param name="value">The value to enqueue.</param>
        public void Enqueue ( T value ) {
            if ( IsFull ) {
                m_elements[0] = m_elements[1];
                m_elements[1] = value;
                m_count = 2; // optional, aber sauber
            } else {
                m_elements[m_count] = value;
                m_count++;
            }
        }

        /// <summary>
        /// Removes the element at the front of the queue.
        /// </summary>
        public Optional<T> Dequeue ( ) {
            Optional<T> _ret = Optional<T>.NONE;

            if ( !IsEmpty ) {
                _ret = m_elements[0];
                m_elements[0] = m_elements[1];
                m_count--;
            }
            return _ret;
        }

        /// <summary>
        /// Removes all elements from the BinQueue without modifying the underlying buffer.
        /// </summary>
        public void Clear () {
            m_elements = new T[2];
            m_count = 0;
        }

        /// <summary>
        /// Creates a FlexSpan view over the current contents of the BinQueue.
        /// The span directly references the internal array and does not allocate.
        ///
        /// <returns>
        /// A FlexSpan that views the range [0 .. m_count) of the internal buffer.
        /// </returns>
        public static FlexSpan<T> AsFlexSpan ( ref BinQueue<T> que, FlexSpanMode mode = FlexSpanMode.System ) {
            return new FlexSpan<T>(ref que.m_elements!, 0, que.m_count, mode);
        }
        
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
