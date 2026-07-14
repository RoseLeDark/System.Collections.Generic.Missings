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
    /// \addtogroup collections
    /// @{
    /// <summary>
    /// A map that maintains its elements in sorted order using either a custom
    /// comparer (<see cref="ICompared{T}"/>) or a delegate-based sorting function
    /// (<see cref="SortFunc{T, TU}"/>).  
    /// Sorting is performed eagerly whenever elements are added or inserted,
    /// depending on the <see cref="AutoSort"/> setting.
    /// </summary>
    /// <typeparam name="T">The key type.</typeparam>
    /// <typeparam name="TU">The value type.</typeparam>
    
    public class SortedMap<T, TU> : Map<T, TU>, ISortedMap<T, TU> where T : notnull
    {
        /// <summary>
        /// Delegate-based sorting function used when no comparer is provided.
        /// </summary>
        private SortFunc<T, TU>? m_sort;

        /// <summary>
        /// Optional comparer object implementing <see cref="ICompared{T}"/>.
        /// When set, it overrides <see cref="SortFunc{T, TU}"/>.
        /// </summary>
        private ICompared<IPair<T, TU>>? m_comparer;

        /// <summary>
        /// Gets or sets the comparer used for sorting.  
        /// Setting this property triggers a sort when <see cref="AutoSort"/> is enabled.
        /// </summary>
        public ICompared<IPair<T, TU>>? Comparer {
            get => m_comparer;
            set {
                m_comparer = value;
                if ( AutoSort ) Sort();
            }
        }

        /// <summary>
        /// Gets or sets the delegate-based sorting function.  
        /// Setting this property triggers a sort when <see cref="AutoSort"/> is enabled.
        /// </summary>
        public SortFunc<T, TU>? SortFunctions {
            get => m_sort; 
            set {
                m_sort = value;
                if ( AutoSort ) Sort();
            }
        }

        /// <summary>
        /// Indicates whether the map should automatically sort itself after
        /// insertions or modifications.
        /// </summary>
        public bool AutoSort { get; set; }

        /// <summary>
        /// Creates a sorted map using a custom comparer.
        /// </summary>
        public SortedMap(ICompared<IPair<T, TU>> comparer) : base() {
            Comparer = comparer;
            m_sort = null;
            AutoSort = true;
        }

        /// <summary>
        /// Creates a sorted map using a delegate-based sorting function.
        /// </summary>
        public SortedMap(SortFunc<T, TU> sort) : base() {
            m_sort = sort;
            AutoSort = true;
            m_comparer = null;
        }

        /// <summary>
        /// Creates a sorted map initialized with the specified elements and sorting function.
        /// </summary>
        public SortedMap(IEnumerable<Pair<T, TU>> elements, SortFunc<T, TU> sort)
            : base(elements) {
            m_sort = sort;
            AutoSort = true;
        }

        /// <summary>
        /// Creates a sorted map from another map using the specified sorting function.
        /// </summary>
        public SortedMap(IMap<T, TU> source, SortFunc<T, TU> sort) : base() {
            m_sort = sort;
            m_elements = [.. source.ToArray()];
            AutoSort = true;
            Sort();
        }

        /// <summary>
        /// Adds an element and re-sorts the map if <see cref="AutoSort"/> is enabled.
        /// </summary>
        public override void Add(Pair<T, TU> item) {
            base.Add(item);
            if ( AutoSort ) Sort();
        }

        /// <summary>
        /// Inserts an element at the specified position and re-sorts the map.
        /// </summary>
        public override bool Insert(int pos, Pair<T, TU> item) {
            m_elements.Insert(pos, item);
            if ( AutoSort ) Sort();
            return true;
        }

        /// <summary>
        /// Inserts a range of elements and re-sorts the map if <see cref="AutoSort"/> is enabled.
        /// </summary>
        public override bool InsertRange(int pos, IEnumerable<Pair<T, TU>> items) {
            m_elements.InsertRange(pos, items);
            if ( AutoSort ) Sort();
            return true;
        }

        /// <summary>
        /// Sorts the map using either the comparer or the delegate-based function.  
        /// Implements a simple O(n²) comparison-based sort.
        /// </summary>
        public void Sort() {
            for ( int i = 0; i < Size - 1; i++ ) {
                for ( int j = i + 1; j < Size; j++ ) {

                    CompareResult cmp = m_comparer != null
                        ? m_comparer.Compare(m_elements[i], m_elements[j])
                        : m_sort!(m_elements[i], m_elements[j]);

                    if ( cmp == CompareResult.AIsLargerB ) {
                        Swap(i, j);
                    }
                }
            }
        }

        /// <summary>
        /// Swaps two elements in the internal list.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Swap(int i, int j) {
            var tmp = m_elements[i];
            m_elements[i] = m_elements[j];
            m_elements[j] = tmp;
        }

        /// <summary>
        /// Returns a new <see cref="Map{T, TU}"/> containing the same elements
        /// but without any sorting behavior.
        /// </summary>
        public IMap<T, TU> ToUnorderedMap() {
            Map<T, TU> map = [.. m_elements];
            return map;
        }

        /// <summary>
        /// Attempts to add a key/value pair to the map and re-sorts the map.
        /// <see cref="Map{T, TU}"/>
        /// </summary>
        public override bool TryAdd(T k, TU v) {
            var pair = base.TryAdd(k, v);
            if ( AutoSort ) Sort();
            return pair;
        }
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
