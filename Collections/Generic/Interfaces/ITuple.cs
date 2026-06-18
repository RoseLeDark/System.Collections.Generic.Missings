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

namespace SystemEx.Collections.Generic.Interfaces {
    public interface ITuple {
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
        bool EqualFirst(object key);


        /// <summary>
        /// Retrieves the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element.</param>
        /// <returns>
        /// The element at the given index, or <c>null</c> if the index is out of range.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Bezeichner dürfen nicht mit Schlüsselwörtern übereinstimmen", Justification = "<Ausstehend>")]
        object? Get(int index);
    }

}
