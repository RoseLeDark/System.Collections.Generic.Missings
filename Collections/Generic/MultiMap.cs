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
    /// A map that allows multiple entries with the same key.  
    /// Unlike <see cref="Map{T, TU}"/>, which prevents duplicate pairs,
    /// a <see cref="MultiMap{TT, TU}"/> accepts all entries without checking
    /// for existing keys or values.
    /// </summary>
    /// <typeparam name="TT">The key type (non‑null).</typeparam>
    /// <typeparam name="TU">The value type (non‑null).</typeparam>
    public class MultiMap<TT, TU> : Map<TT, TU> {

        /// <summary>
        /// Creates an empty multi‑map.
        /// </summary>
        public MultiMap() : base() { }

        /// <summary>
        /// Creates a multi‑map initialized with the specified elements.
        /// </summary>
        /// <param name="elements">The initial key/value pairs.</param>
        public MultiMap(IEnumerable<Pair<TT, TU>> elements) : base(elements) { }

        /// <summary>
        /// Adds a pair to the map without performing any duplicate checks.  
        /// This overrides the base implementation to allow multiple identical
        /// key/value pairs.
        /// </summary>
        /// <param name="item">The pair to add.</param>
        public override void Add(Pair<TT, TU> item) {
            m_elements.Add(item);
        }
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
