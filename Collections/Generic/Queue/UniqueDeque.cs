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

using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Collections.Generic.Queue {
	/// \addtogroup SystemEx.Collections.Generic 
	/// @{
	/// <summary>
	/// Represents a deque that only accepts unique elements.
	/// Attempts to insert a value already contained in the queue will fail.
	/// </summary>
	/// <typeparam name="T">The type of elements stored in the queue.</typeparam>
	public struct UniqueDeque<T> : IDeque <T> {
        private Deque<T> m_queue;

        /// <summary>
        /// Gets the number of elements currently stored in the queue.
        /// </summary>
        public int Size => m_queue.Size;

        /// <summary>
        /// Gets a value indicating whether the queue contains no elements.
        /// </summary>
        public bool IsEmpty => m_queue.IsEmpty;
        /// <summary>
        /// Gets a value indicating whether the queue has reached its maximum capacity.
        /// </summary>
        public bool IsFull => m_queue.IsFull;

        public T Front => m_queue.Front;

        public T End => m_queue.End;

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
        public bool PopFront ( out Optional<T> value ) => m_queue.PopFront(out value);
        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueQueue{T}"/> struct.
        /// </summary>
        /// <param name="size">The initial capacity of the queue.</param>
        /// <param name="growSize">
        /// The number of additional elements to allocate when the queue grows.
        /// </param>
        public UniqueDeque ( int size, int growSize = 2 ) {
            m_queue = new Deque<T>(size, growSize);
        }

        /// <inheritdoc/>
        public bool PushBack ( T value ) {
            bool _ret = false;

            if ( !Contains(value) )
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
        private bool Contains ( T value ) {
            bool _ret = false;
            DequeFlexSpan<T> span = Deque<T>.AsFlexSpan(ref m_queue, FlexSpanMode.System);

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
        /// <inheritdoc/>
        public bool PopBack ( ref Optional<T> value ) => m_queue.PopBack(ref value);
        /// <inheritdoc/>
        public bool PushFront (  T value ) {
            bool _ret = false;

            if ( !Contains(value) )
                _ret = m_queue.PushBack(value);
            return _ret;
        }
    }
}
