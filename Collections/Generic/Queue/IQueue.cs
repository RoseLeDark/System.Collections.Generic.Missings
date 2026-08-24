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
	/// \addtogroup SystemEx.Collections.Generic 
	/// @{
	/// <summary>
	/// Defines the non-generic base interface for queue-like containers.
	/// Provides size information, capacity checks, and a method to clear the queue.
	/// </summary>
	public interface IQueue {
        /// <summary>
        /// Gets the number of elements currently stored in the queue.
        /// </summary>
        int Size { get;  }

        /// <summary>
        /// Gets a value indicating whether the queue contains no elements.
        /// </summary>
        public bool IsEmpty { get; }

        //// <summary>
        /// Gets a value indicating whether the queue has reached its maximum capacity.
        /// </summary>
        public bool IsFull { get; }

        /// <summary>
        /// Removes all elements from the queue.
        /// </summary>
        public void Clear ();

    }
    /// <summary>
    /// Defines the generic FIFO queue interface.
    /// Provides operations for inserting elements at the back and removing elements from the front.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the queue.</typeparam>
    public interface IQueue<T> : IQueue {
        /// <summary>
        /// Inserts an element at the back of the queue.
        /// </summary>
        /// <param name="value">The element to insert.</param>
        /// <returns>
        /// <c>true</c> if the element was successfully inserted;
        /// otherwise <c>false</c> (for example, if the queue is full).
        /// </returns>
        public bool PushBack ( T value );

        /// <summary>
        /// Removes the element at the front of the queue.
        /// Remaining elements are shifted one position to the left.
        /// </summary>
        /// <param name="value">
        /// When this method returns, contains the removed element if the operation succeeded;
        /// otherwise contains <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if an element was removed;
        /// otherwise <c>false</c> (for example, if the queue is empty).
        /// </returns>
        public bool PopFront ( ref Optional<T> value );
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
