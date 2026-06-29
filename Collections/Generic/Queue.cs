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

using SystemEx.Collections.Generic.Interfaces;

namespace SystemEx.Collections.Generic {
    /// \addtogroup collections
    /// @{
    /// <summary>
    /// A simple fixed‑size FIFO queue implemented as a thin wrapper around
    /// a <see cref="Deque{T}"/>.  
    /// Provides enqueue and dequeue operations using the deque’s front/back
    /// insertion and removal semantics.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the queue.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Naming", "CA1711:Identifiers should not have incorrect suffix",
        Justification = "<Pending>")]
    public class Queue<T> {

        /// <summary>
        /// Internal deque used to implement queue behavior.
        /// </summary>
        private Deque<T> m_deque;

        /// <summary>
        /// Gets the total capacity of the queue.
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
        /// Gets the element at the front of the queue without removing it.
        /// </summary>
        public T Front => m_deque.Front;

        /// <summary>
        /// Creates a new queue with the specified capacity.
        /// </summary>
        /// <param name="size">The maximum number of elements the queue can hold.</param>
        public Queue(int size) {
            m_deque = new Deque<T>(size);
        }

        /// <summary>
        /// Creates a new queue that wraps an existing deque.
        /// </summary>
        /// <param name="d">The deque to use as the underlying storage.</param>
        public Queue(Deque<T> d) {
            m_deque = d;
        }

        /// <summary>
        /// Adds an element to the back of the queue.
        /// </summary>
        /// <param name="value">The element to enqueue.</param>
        public void Enqueue(T value) => m_deque.PushBack(value);

        /// <summary>
        /// Removes the element at the front of the queue.
        /// </summary>
        /// <param name="value">Receives the removed element.</param>
        /// <returns><c>true</c> if an element was removed; otherwise <c>false</c>.</returns>
        public bool Dequeue(ref T value) => m_deque.PopFront(ref value);

        /// <summary>
        /// Removes all elements from the queue.
        /// </summary>
        public void Clear() {
            m_deque.Clear();
        }
    }

#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
