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

using System.Runtime.CompilerServices;
using SystemEx.Collections.Generic.Interfaces;
using SystemEx.Utils;

namespace SystemEx.Collections.Generic {
    /// <summary>
    /// A dynamically sized array that supports automatic sorting using either a comparer
    /// interface or a delegate-based sort function. Sorting can be triggered manually
    /// or automatically after insert, add, or remove operations.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the array.</typeparam>
    public class SortedArray<T> : Array<T>, ISortedArray<T> {

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
        /// Enables or disables automatic sorting after modification operations.
        /// </summary>
        public bool AutoSort { get; set; }

        /// <summary>
        /// Creates a new sorted array with the specified initial size, sort function,
        /// and optional growth size.
        /// </summary>
        /// <param name="size">Initial capacity of the array.</param>
        /// <param name="sorter">Delegate used to compare elements.</param>
        /// <param name="growSize">Number of elements to grow when resizing.</param>
        public SortedArray(int size, SortObjectFunc<T> sorter, int growSize = 16)
            : base(size, growSize) {
            m_sort = sorter;
        }

        /// <summary>
        /// Adds an element to the array and optionally sorts the array.
        /// </summary>
        public override bool Add(T entry) {
            bool result = base.Add(entry);
            if ( result && AutoSort ) Sort();
            return result;
        }

        /// <summary>
        /// Inserts an element at the specified position and optionally sorts the array.
        /// </summary>
        public override int Insert(int pos, T item) {
            int result = base.Insert(pos, item);
            if ( result >= 1 && AutoSort ) Sort();
            return result;
        }

        /// <summary>
        /// Inserts a range of elements starting at the specified position
        /// and optionally sorts the array.
        /// </summary>
        public override int InsertRange(int pos, IEnumerable<T> items) {
            int result = base.InsertRange(pos, items);
            if ( result >= 1 && AutoSort ) Sort();
            return result;
        }

        /// <summary>
        /// Removes the last element from the array and optionally sorts the array.
        /// </summary>
        public override bool Remove() {
            bool result = base.Remove();
            if ( result && AutoSort ) Sort();
            return result;
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
        /// <param name="i">Index of the first element.</param>
        /// <param name="j">Index of the second element.</param>
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

}
