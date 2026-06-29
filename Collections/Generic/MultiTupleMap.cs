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

using System.Collections;
using SystemEx.Collections.Generic.Interfaces;

namespace SystemEx.Collections.Generic {
    /// \addtogroup collections
    /// @{
    /// <summary>
    /// A tuple map that allows multiple entries with identical tuple keys.  
    /// Unlike <see cref="TupleMap"/>, which prevents duplicate tuples,
    /// </summary>
    public class MultiTupleMap : TupleMap {
        /// <summary>
        /// Creates a empty  multi‑tuple map.
        /// </summary>
        public MultiTupleMap() : base() {  }
        /// <summary>
        /// Creates a multi‑tuple map initialized with the specified elements.
        /// </summary>
        public MultiTupleMap(IEnumerable<ITuple> elements) : base(elements) { }
        /// <summary>
        /// Adds a tuple to the map without performing duplicate checks.  
        /// </summary>
        /// <param name="item">The tuple to add.</param>
        public override void Add(ITuple item) { m_elements.Add(item); }
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
