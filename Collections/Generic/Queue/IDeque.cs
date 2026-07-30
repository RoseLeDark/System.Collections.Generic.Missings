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
    /// Defines the non-generic base interface for double-ended queue (deque) containers.
    /// A deque extends the basic queue interface with support for operations at both ends.
    /// </summary>
    public interface IDeque : IQueue {
    }

    /// <summary>
    /// Defines the generic interface for a double-ended queue (deque).
    /// A deque allows insertion and removal of elements at both the front and the back.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the deque.</typeparam>
    public interface IDeque<T> : IDeque {
        /// <summary>
        /// Gets the element at the front of the deque.
        /// </summary>
        /// <remarks>
        /// Accessing this property does not modify the deque.
        /// </remarks>
        T Front { get; }

        /// <summary>
        /// Gets the element at the back of the deque.
        /// </summary>
        /// <remarks>
        /// Accessing this property does not modify the deque.
        /// </remarks>
        T End { get; }

        /// <summary>
        /// Removes the element at the back of the deque.
        /// </summary>
        /// <param name="value">
        /// When this method returns, contains the removed element if the operation succeeded;
        /// otherwise remains unchanged.
        /// </param>
        /// <returns>
        /// <c>true</c> if an element was removed;
        /// otherwise <c>false</c>.
        /// </returns>
        bool PopBack ( ref Optional<T> value );

        /// <summary>
        /// Inserts an element at the front of the deque.
        /// </summary>
        /// <param name="value">The element to insert at the front.</param>
        /// <returns>
        /// <c>true</c> if the element was successfully inserted;
        /// otherwise <c>false</c> .
        /// </returns>
        bool PushFront ( T value );


    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
