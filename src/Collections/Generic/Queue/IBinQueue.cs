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
	/// A fixed-size queue that stores up to two elements. When full, the oldest
	/// element is automatically removed to make room for a new one.
	/// </summary>
	/// <typeparam name="T">The type of elements stored in the queue.</typeparam>
	public interface IBinQueue<T> : IDeque {
        /// <summary>
        /// Adds a new element to the queue. If the queue is full, the oldest
        /// element is removed automatically before inserting the new one.
        /// </summary>
        /// <param name="value">The value to enqueue.</param>
        void Enqueue ( T value );

        /// <summary>
        /// Removes the element at the front of the queue.
        /// </summary>
        Optional<T> Dequeue ( );

    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
