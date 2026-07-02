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
using SystemEx.Base;
using SystemEx.Collections.Generic.Interfaces;

namespace SystemEx.Collections.Generic {
    /// \addtogroup collections
    /// @{
    /// <summary>
    /// A simple fixed‑size double‑ended queue (deque) implemented using a linear array.
    /// Supports pushing and popping at both the front and the back.  
    /// This implementation does not use a ring buffer; shifting is performed when
    /// inserting or removing at the front.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the deque.</typeparam>
    public class Deque<T> {
        /// <summary>
        /// Internal storage buffer for the deque elements.
        /// </summary>
        private T[] m_elements;

        /// <summary>
        /// The number of elements currently stored in the deque.
        /// </summary>
        private int m_count;

        /// <summary>
        /// Gets the number of elements currently stored in the deque.
        /// </summary>
        public int Count => m_count;


        /// <summary>
        /// Gets the total capacity of the deque.
        /// </summary>
        public int Size => m_elements.Length;

        /// <summary>
        /// Indicates whether the deque contains no elements.
        /// </summary>
        public bool IsEmpty => m_count == 0;

        /// <summary>
        /// Indicates whether the deque has reached its maximum capacity.
        /// </summary>
        public bool IsFull => m_count == m_elements.Length;

        /// <summary>
        /// Gets the element at the front of the deque.
        /// </summary>
        public T Front => m_elements[0];

        /// <summary>
        /// Gets the element at the back of the deque.
        /// </summary>
        public T End => m_elements[m_count - 1];

        /// <summary>
        /// Creates a new deque with the specified capacity.
        /// </summary>
        /// <param name="size">The maximum number of elements the deque can hold.</param>
        public Deque(int size) {
            m_elements = new T[size];
            m_count = 0;
        }

        /// <summary>
        /// Adds an element to the back of the deque if space is available.
        /// </summary>
        /// <param name="value">The element to add.</param>
        public void PushBack(T value) {
            if ( IsFull ) return;
            m_elements[m_count++] = value;
        }

        /// <summary>
        /// Removes the element at the back of the deque.
        /// </summary>
        /// <param name="value">Receives the removed element.</param>
        /// <returns><c>true</c> if an element was removed; otherwise <c>false</c>.</returns>
        public bool PopBack(ref T value) {
            if ( IsEmpty ) return false;
            value = m_elements[m_count - 1];
            m_count--;
            return true;
        }

        /// <summary>
        /// Adds an element to the front of the deque.  
        /// Existing elements are shifted one position to the right.
        /// </summary>
        /// <param name="value">The element to add.</param>
        public void PushFront(T value) {
            if ( IsFull ) return;

            // Shift all elements to the right
            for ( int i = m_count; i > 0; i-- )
                m_elements[i] = m_elements[i - 1];

            m_elements[0] = value;
            m_count++;
        }

        /// <summary>
        /// Removes the element at the front of the deque.  
        /// Remaining elements are shifted one position to the left.
        /// </summary>
        /// <param name="value">Receives the removed element.</param>
        /// <returns><c>true</c> if an element was removed; otherwise <c>false</c>.</returns>
        public bool PopFront(ref T value) {
            if ( IsEmpty ) return false;

            value = m_elements[0];

            // Shift all elements to the left
            for ( int i = 0; i < m_count - 1; i++ )
                m_elements[i] = m_elements[i + 1];

            m_count--;
            return true;
        }

        /// <summary>
        /// Removes all elements from the deque without modifying the underlying buffer.
        /// </summary>
        public void Clear() {
            m_count = 0;
        }

        /// <summary>
        /// Creates a FlexSpan view over the current contents of the deque.
        /// The span directly references the internal array and does not allocate.
        ///
        /// </summary>
        /// <param name="mode">
        /// The indexing mode of the span. 
        /// </param>
        /// <returns>
        /// A FlexSpan that views the range [0 .. m_count) of the internal buffer.
        /// </returns>
        public FlexSpan<T> AsFlexSpan ( FlexSpanMode mode = FlexSpanMode.System ) {
            return new FlexSpan<T>(ref m_elements!, 0, m_count, mode);
        }
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
