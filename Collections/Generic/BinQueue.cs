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
using SystemEx.Collections.Generic.Interfaces;

namespace SystemEx.Collections.Generic {
    /// <summary>
    /// A fixed-size queue that stores up to two elements. When full, the oldest
    /// element is automatically removed to make room for a new one.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the queue.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Naming",
        "CA1711:Identifiers should not have incorrect suffix",
        Justification = "<Pending>")]
    public class BinQueue<T> {

        /// <summary>
        /// Internal deque used as the underlying storage container.
        /// </summary>
        private Deque<T> m_deque;

        /// <summary>
        /// Gets the number of elements currently stored in the queue.
        /// </summary>
        public int Count => m_deque.Count;

        /// <summary>
        /// Gets the maximum capacity of the queue (always 2).
        /// </summary>
        public int Size => m_deque.Size;

        /// <summary>
        /// Indicates whether the queue contains no elements.
        /// </summary>
        public bool IsEmpty => m_deque.IsEmpty;

        /// <summary>
        /// Indicates whether the queue has reached its maximum capacity.
        /// </summary>
        public bool IsFull => m_deque.IsFull;

        /// <summary>
        /// Gets the element at the front of the queue.
        /// </summary>
        public T Front => m_deque.Front;

        /// <summary>
        /// Gets the element at the end of the queue.
        /// </summary>
        public T End => m_deque.End;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinQueue{T}"/> class
        /// with a fixed capacity of two elements.
        /// </summary>
        public BinQueue() {
            m_deque = new Deque<T>(2);
        }

        /// <summary>
        /// Adds a new element to the queue. If the queue is full, the oldest
        /// element is removed automatically before inserting the new one.
        /// </summary>
        /// <param name="value">The value to enqueue.</param>
        public void Enqueue(T value) {
            if ( IsFull ) {
                T dummy = default!;
                m_deque.PopFront(ref dummy);
            }
            m_deque.PushBack(value);
        }

        /// <summary>
        /// Removes the element at the front of the queue.
        /// </summary>
        /// <param name="value">Receives the removed value if successful.</param>
        /// <returns>
        /// <c>true</c> if an element was removed; otherwise <c>false</c>.
        /// </returns>
        public bool Dequeue(ref T value) => m_deque.PopFront(ref value);

        /// <summary>
        /// Removes all elements from the queue.
        /// </summary>
        public void Clear() {
            m_deque.Clear();
        }
    }

}
