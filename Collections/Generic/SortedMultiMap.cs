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

using System.Numerics;
using System.Runtime.CompilerServices;
using SystemEx.Utils;
using SystemEx.Collections.Generic.Interfaces;
using SystemEx.Algorithms.Interfaces;

namespace SystemEx.Collections.Generic {
    /// <summary>
    /// A map that allows multiple entries with the same key.  
    /// Unlike <see cref="Map{T, TU}"/>, which prevents duplicate pairs,
    /// a <see cref="MultiMap{T, TU}"/> accepts all entries without checking
    /// for existing keys or values. /// Sorting is performed eagerly whenever elements are added or inserted,
    /// depending on the <see cref="SortedMap{T, TU}"/> setting.
    /// </summary>
    /// <typeparam name="T">The key type (non‑null).</typeparam>
    /// <typeparam name="TU">The value type (non‑null).</typeparam>
    public class SortedMultiMap<T, TU> : SortedMap<T, TU> where T : notnull {
        /// <summary>
        /// Creates a sorted map using a delegate-based sorting function.
        /// </summary>
        public SortedMultiMap(SortFunc<T, TU> sort) : base(sort) { }
        /// <summary>
        /// Creates a sorted map from another map using the specified sorting function.
        /// </summary>
        public SortedMultiMap(IMap<T, TU> source, SortFunc<T, TU> sort) : base(source, sort) { }
        /// <summary>
        /// Creates a sorted map using a custom comparer.
        /// </summary>
        public SortedMultiMap(ICompared<IPair<T, TU>> comparer) : base(comparer) { }
        /// <summary>
        /// Creates a sorted map initialized with the specified elements and sorting function.
        /// </summary>
        public SortedMultiMap(IEnumerable<Pair<T, TU>> elements, SortFunc<T, TU> sort) : base(elements, sort) { }
        /// <summary>
        /// Adds an element and re-sorts the map if <see cref="SortedMap{T, TU}.AutoSort"/> is enabled.
        /// </summary>
        public override void Add(Pair<T, TU> item) {
            m_elements.Add(item);
            if ( AutoSort ) Sort();
        }
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
