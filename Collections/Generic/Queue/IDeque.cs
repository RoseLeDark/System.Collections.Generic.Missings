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
    /// \addtogroup collections
    /// @{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDeque {
        int Size { get;  }

        /// <summary>
        /// Indicates whether the deque contains no elements.
        /// </summary>
        public bool IsEmpty { get; }

        /// <summary>
        /// Indicates whether the deque has reached its maximum capacity.
        /// </summary>
        public bool IsFull { get; }


        public void Clear ();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDeque<T> : IDeque {
        /// <summary>
        /// Gets the element at the front of the deque.
        /// </summary>
        public T Front { get; }

        /// <summary>
        /// Gets the element at the back of the deque.
        /// </summary>
        public T End { get; }

        public bool PushBack ( T value );

        /// <summary>
        /// Removes the element at the back of the deque.
        /// </summary>
        /// <param name="value">Receives the removed element.</param>
        /// <returns><c>true</c> if an element was removed; otherwise <c>false</c>.</returns>
        public bool PopBack ( ref T value );

        /// <summary>
        /// Adds an element to the front of the deque.  
        /// Existing elements are shifted one position to the right.
        /// </summary>
        /// <param name="value">The element to add.</param>
        public bool PushFront ( T value );

        /// <summary>
        /// Removes the element at the front of the deque.  
        /// Remaining elements are shifted one position to the left.
        /// </summary>
        /// <param name="value">Receives the removed element.</param>
        /// <returns><c>true</c> if an element was removed; otherwise <c>false</c>.</returns>
        public bool PopFront ( out T? value );
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
