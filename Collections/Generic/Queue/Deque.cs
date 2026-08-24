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
using SystemEx.Collections.Generic;

namespace SystemEx.Collections.Generic {



	/// \addtogroup SystemEx.Collections.Generic 
	/// @{
	/// <summary>
	/// A simple fixed‑size double‑ended queue (deque) implemented using a linear array.
	/// Supports pushing and popping at both the front and the back.  
	/// This implementation does not use a ring buffer; shifting is performed when
	/// inserting or removing at the front.
	/// </summary>
	/// <typeparam name="T">The type of elements stored in the deque.</typeparam>
	public struct Deque<T> : IDeque<T>, IAutoGrowe {
        private long m_growSize;
        private bool m_autoGrow;

        /// <summary>
        /// Internal storage buffer for the deque elements.
        /// </summary>
        internal T[] m_elements;

        /// <summary>
        /// The number of elements currently stored in the deque.
        /// </summary>
        private int m_count;

        /// <summary>
        /// Gets the number of elements currently stored in the deque.
        /// </summary>
        public int Count => m_count;

        /// <summary>
        /// Gets or sets the number of elements the Vector grows by when AutoGrow is enabled.
        /// </summary>
        public long GrowSize {
            get => (m_autoGrow ? m_growSize : 0);
            set {
                m_growSize = value;
                m_autoGrow = (m_growSize > 0);
            }
        }
        /// <summary>
        /// Enables or disables automatic resizing when the Vector becomes full.
        /// </summary>
        public bool AutoGrow { get => (m_growSize == 0 ? false : m_autoGrow); set => m_autoGrow = value; }



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
        /// <param name="growSize">Number of elements to add when automatic growth occurs.</param>
        public Deque (int size, int growSize) {
            m_elements = new T[size];
            m_count = 0;
            GrowSize = growSize;
        }

        /// <summary>
        /// Creates a new Dequeue using an existing buffer.
        /// The buffer is adopted as-is, and Count is set
        /// to the last valid index. 
        /// </summary>
        /// <param name="e">
        /// Existing array used as the internal storage.
        /// </param>
        /// <param name="growSize">
        /// Number of elements to add when automatic growth occurs.
        /// </param>
        public Deque ( T[] e, int growSize = 16 ) {
            m_elements = e;
            m_count = e.Length;

            GrowSize = growSize;
        }

        /// <summary>
        /// Adds an element to the back of the deque if space is available.
        /// </summary>
        /// <param name="value">The element to add.</param>
        public bool PushBack(T value) {
            if ( IsFull ) {
                if ( AutoGrow ) Grow();
                return false;
            }
            m_elements[m_count] = value;
            m_count++;
            return true;
        }

        /// <summary>
        /// Removes the element at the back of the deque.
        /// </summary>
        /// <param name="value">Receives the removed element.</param>
        /// <returns><c>true</c> if an element was removed; otherwise <c>false</c>.</returns>
        public bool PopBack(ref Optional<T> value ) {
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
        public bool PushFront (T value) {
            if ( IsFull ) {
                if ( AutoGrow ) Grow();
                return false;
            }

            // Shift all elements to the right
            for ( int i = m_count; i > 0; i-- )
                m_elements[i] = m_elements[i - 1];

            m_elements[0] = value;
            m_count++;
            return true;
        }

        /// <summary>
        /// Removes the element at the front of the deque.  
        /// Remaining elements are shifted one position to the left.
        /// </summary>
        /// <param name="value">Receives the removed element.</param>
        /// <returns><c>true</c> if an element was removed; otherwise <c>false</c>.</returns>
        public bool PopFront(out Optional<T> value ) {
            if ( IsEmpty ) {
                value = Optional<T>.NONE;
                return false;
            }

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
        /// Creates a FlexSpan view over the current  deque.
        /// </summary>

        public static DequeFlexSpan<T> AsFlexSpan ( ref Deque<T> que, FlexSpanMode mode = FlexSpanMode.System ) {
            return new DequeFlexSpan<T>(ref que, 0, que.m_count, mode);
        }
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




        internal Optional<T> ElementAt ( long index ) {
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
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
