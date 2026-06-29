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

using SystemEx.Utils;
using SystemEx.Collections.Generic.Interfaces;
using System.Runtime.CompilerServices;

namespace SystemEx.Collections.Generic {
    /// \addtogroup collections
    /// @{
    /// <summary>
    /// A tuple map that maintains its elements in sorted order.  
    /// Sorting is performed using either a custom <see cref="ICompared{T}"/>
    /// implementation or a delegate of type <see cref="SortTupleFunc"/>.  
    /// The map automatically re-sorts itself after insertions when
    /// <see cref="AutoSort"/> is enabled.
    /// </summary>
    [Serializable]
#pragma warning disable CA1710 // Bezeichner müssen ein korrektes Suffix aufweisen
    public class SortedTupleMap : TupleMap, ISortedTupleMap
#pragma warning restore CA1710 // Bezeichner müssen ein korrektes Suffix aufweisen
    {

        /// <summary>
        /// Delegate-based sorting function used when no comparer is provided.
        /// </summary>
        private SortTupleFunc m_sort;

        /// <summary>
        /// Optional comparer object implementing <see cref="ICompared{T}"/>.
        /// When set, it overrides <see cref="SortTupleFunc"/>.
        /// </summary>
        private ICompared<Interfaces.ITuple>? m_comparer;

        /// <summary>
        /// Gets or sets the comparer used for sorting.  
        /// Setting this property triggers a sort when <see cref="AutoSort"/> is enabled.
        /// </summary>
        public ICompared<Interfaces.ITuple>? Comparer {
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
        public SortTupleFunc SortFunctions {
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
        /// Creates a new sorted tuple map using the specified sorting function.
        /// </summary>
        /// <param name="sort">The delegate used to compare two tuples.</param>
        public SortedTupleMap(SortTupleFunc sort) : base() {
            m_sort = sort;
            AutoSort = true;
        }

        /// <summary>
        /// Creates a new sorted tuple map initialized with the contents of another map.
        /// </summary>
        /// <param name="source">The source map whose elements are copied.</param>
        /// <param name="sort">The delegate used to compare two tuples.</param>
        public SortedTupleMap(ITupleMap source, SortTupleFunc sort) : base() {
            m_sort = sort;
            m_elements = [.. source.ToArray()];
            Sort();
        }

        /// <summary>
        /// Adds a tuple to the map and re-sorts the collection if enabled.
        /// </summary>
        public override void Add(Interfaces.ITuple item) {
            base.Add(item);
            if ( AutoSort ) Sort();
        }

        /// <summary>
        /// Inserts a tuple at the specified position and re-sorts the collection.
        /// </summary>
        public override bool Insert(int pos, Interfaces.ITuple item) {
            m_elements.Insert(pos, item);
            Sort();
            return true;
        }

        /// <summary>
        /// Inserts a range of tuples at the specified position and re-sorts the collection.
        /// </summary>
        public override bool InsertRange(int pos, IEnumerable<Interfaces.ITuple> items) {
            m_elements.InsertRange(pos, items);
            if ( AutoSort ) Sort();
            return true;
        }

        /// <summary>
        /// Sorts the tuple map using either the comparer or the delegate-based function.  
        /// Implements a simple O(n²) comparison-based sort.
        /// </summary>
        public void Sort() {
            for ( int i = 0; i < base.Count - 1; i++ ) {
                for ( int j = i + 1; j < base.Count; j++ ) {

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
        /// Returns a new <see cref="TupleMap"/> containing the same elements
        /// but without any sorting behavior.
        /// </summary>
        public ITupleMap ToUnorderedMap() {
            TupleMap map = [.. m_elements];
            return map;
        }
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
