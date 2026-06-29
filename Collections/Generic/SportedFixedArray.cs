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
using SystemEx.Utils;
using SystemEx.Collections.Generic.Interfaces;
using System.Runtime.CompilerServices;

namespace SystemEx.Collections.Generic {
    /// \addtogroup collections
    /// @{
    /// <summary>
    /// A fixed-size array that supports automatic sorting using either a comparer
    /// interface or a delegate-based sort function. Sorting can be triggered
    /// manually or automatically after insert and remove operations.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the array.</typeparam>
    public class SportedFixedArray<T> : FixedArray<T> {
        /// <summary>
        /// Delegate used to compare two elements when no comparer interface is provided.
        /// </summary>
        private SortObjectFunc<T> m_sort;

         /// <summary>
        /// Optional comparer interface used to compare elements.
        /// If set, it overrides the delegate-based comparison.
        /// </summary>
        private ICompared<T>? m_comparer;

        /// <summary>
        /// Gets or sets the comparer interface used for sorting.
        /// When changed, the array is automatically sorted if <see cref="AutoSort"/> is enabled.
        /// </summary>
        public ICompared<T>? Comparer {
            get => m_comparer;
            set {
                m_comparer = value;
                if ( AutoSort ) Sort();
            }
        }

        /// <summary>
        /// Gets or sets the delegate-based sort function.
        /// When changed, the array is automatically sorted if <see cref="AutoSort"/> is enabled.
        /// </summary>
        public SortObjectFunc<T> SortFunctions {
            get => m_sort!;
            set {
                m_sort = value;
                if ( AutoSort ) Sort();
            }
        }
        /// <summary>
        /// Enables or disables automatic sorting after insert or remove operations.
        /// </summary>
        public bool AutoSort { get; set; }

        /// <summary>
        /// Creates a new sorted fixed-size array with the specified capacity and sort function.
        /// </summary>
        /// <param name="size">The number of elements the array can hold.</param>
        /// <param name="sort">The delegate used to compare elements.</param>
        public SportedFixedArray(int size, SortObjectFunc<T> sort)
            : base(size) {
            m_sort = sort;
        }

        /// <summary>
        /// Creates a new sorted fixed-size array using an existing buffer and sort function.
        /// </summary>
        /// <param name="e">The initial element buffer.</param>
        /// <param name="sort">The delegate used to compare elements.</param>
        public SportedFixedArray(T[] e, SortObjectFunc<T> sort)
            : base(e) {
            m_sort = sort;
        }

        /// <summary>
        /// Inserts an element at the specified position and optionally sorts the array.
        /// </summary>
        public override int Insert(int pos, T item) {
            int _ret = base.Insert(pos, item);
            if ( _ret >= 1 && AutoSort ) Sort();
            return _ret;
        }
        /// <summary>
        /// Inserts a range of elements starting at the specified position
        /// and optionally sorts the array.
        /// </summary>  
        public override int InsertRange(int pos, IEnumerable<T> items) {
            int _ret  = base.InsertRange(pos, items);
            if ( _ret >= 1 && AutoSort ) Sort();
            return _ret;
        }

        /// <summary>
        /// Removes the last element from the array and optionally sorts the array.
        /// </summary>
        public override bool Remove() {
            bool _ret = base.Remove();
            if ( _ret && AutoSort ) Sort();
            return _ret;
        }
        /// <summary>
        /// Sorts the entire array using either the comparer interface or the delegate-based sort function.
        /// Implements a simple bubble-sort algorithm.
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
        /// Swaps two elements in the internal buffer.
        /// </summary>
        /// <param name="i">The index of the first element.</param>
        /// <param name="j">The index of the second element.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Swap(int i, int j) {
            var tmp = m_elements[i];
            m_elements[i] = m_elements[j];
            m_elements[j] = tmp;
        }

        /// <summary>
        /// Returns a new array containing the same elements but without sorting behavior.
        /// </summary>
        /// <returns>An <see cref="IArray{T}"/> containing the unsorted elements.</returns>
        public IArray<T> ToUnorderedArray() {
            return new Array<T>(this.ToArray());
        }
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
