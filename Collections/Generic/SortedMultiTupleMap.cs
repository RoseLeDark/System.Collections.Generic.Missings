// SPDX-License-Identifier: EUPL-1.2

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
    /// <summary>
    /// A sorted tuple map that allows multiple entries with identical tuple keys.  
    /// Unlike <see cref="SortedTupleMap"/>, which prevents duplicate tuples,
    /// <see cref="SortedMultiTupleMap"/> accepts all items without duplicate checks
    /// while still maintaining full sorting behavior.
    /// </summary>
    public class SortedMultiTupleMap : SortedTupleMap {
        /// <summary>
        /// Creates a new multi‑tuple sorted map using the specified sorting function.
        /// </summary>
        /// <param name="sort">The delegate used to compare two tuples.</param>
        public SortedMultiTupleMap(SortTupleFunc sort)
            : base(sort) { }

        /// <summary>
        /// Creates a new multi‑tuple sorted map initialized with the contents of
        /// another tuple map and using the specified sorting function.
        /// </summary>
        /// <param name="source">The source map whose elements are copied.</param>
        /// <param name="sort">The delegate used to compare two tuples.</param>
        public SortedMultiTupleMap(ITupleMap source, SortTupleFunc sort)
            : base(source, sort) { }

        /// <summary>
        /// Adds a tuple to the map without performing duplicate checks.  
        /// Sorting is applied automatically when <see cref="SortedTupleMap.AutoSort"/>
        /// is enabled.
        /// </summary>
        /// <param name="item">The tuple to add.</param>
        public override void Add(Interfaces.ITuple item) {
            m_elements.Add(item);
            if ( AutoSort )
                Sort();
        }
    }

}
