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
	/// A lightweight interface for serializable N‑element tuple consisting of strongly typed 
	/// </summary>
	public interface ITuple<TKey> where TKey : notnull  {
        /// <summary>
        /// Gets or sets the first element of the pair.
        /// </summary>
        TKey First { get; set; }

        /// <summary>
        /// Gets the number of elements stored in the tuple.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Determines whether the first element of the tuple is equal to the specified key.
        /// </summary>
        /// <param name="key">The value to compare against the first element.</param>
        /// <returns>
        /// <c>true</c> if the first element equals <paramref name="key"/>; 
        /// otherwise <c>false</c>.
        /// </returns>
        bool EqualFirst(TKey key);


        /// <summary>
        /// Retrieves the vaue at the specified index, not key
        /// </summary>
        /// <param name="index">The zero-based index of the element.</param>
        /// <returns>
        /// The element at the given index, or <c>null</c> if the index is out of range.
        /// </returns>
        Optional<object> Get (int index);
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
	
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
