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

using SystemEx.Algorithms.Interfaces;
using SystemEx.Collections.Generic;

namespace SystemEx.Algorithms {

    /// \addtogroup Algorithms
    /// @{
    /// <summary>
    /// Provides a set of sorting algorithms that operate on any container
    /// implementing <see cref="IVector{T}"/>.  
    /// All algorithms use <see cref="ISimpleCompare{T}"/> as their comparison strategy.
    /// </summary>
    public static class SortActions {


        /// <summary>
        /// Find Min and Max and set mn and max on the two ends
        /// </summary>
        /// <typeparam name="C">Container type.</typeparam>
        /// <typeparam name="T">The Value Type</typeparam>
        /// <param name="container">The container to sort.</param>
        /// <param name="cmp">Comparison strategy (e.g., Less).</param>
        public static void HeaptMaker<T, C> ( ref C container, ISimpleCompare<T> cmp  )
            where C : IVector<T>, ISwappable<long> {

            long n = container.Length;
            if ( n <= 1 ) return;

            long minIndex = 0;
            long maxIndex = 0;

            // Min/Max MUST use a real comparer, not ISimpleCompare<T>
            for ( long i = 1 ; i < n ; i++ ) {
                var a = container.ElementAt(i);

                if ( cmp.Compare(a, container.ElementAt(minIndex) ) == triple.False ) minIndex = i; // a < min ?
                if ( cmp.Compare(a, container.ElementAt(maxIndex) ) == triple.Nin ) maxIndex = i;  // a > max ?
            }

            // Move min to front
            if ( minIndex != 0 )
                container.Swap(0, minIndex);

            // Move max to end
            if ( maxIndex != n - 1 )
                container.Swap(maxIndex, n - 1);
        }

        public static void HeaptMaker<T> ( ref T[] container, ISimpleCompare<T> cmp ) {

            long n = container.Length;
            if ( n <= 1 ) return;

            long minIndex = 0;
            long maxIndex = 0;

            // Min/Max MUST use a real comparer, not ISimpleCompare<T>
            for ( long i = 1 ; i < n ; i++ ) {
                var a = container[i];

                if ( cmp.Compare(a, container[minIndex] ) == triple.False ) minIndex = i; // a < min ?
                if ( cmp.Compare(a, container[maxIndex] ) == triple.Nin ) maxIndex = i;  // a > max ?
            }

            // Move min to front
            if ( minIndex != 0 )
                Swap(ref container, 0, minIndex);

            // Move max to end
            if ( maxIndex != n - 1 )
                Swap(ref container, maxIndex, n - 1);
        }

        public static T[]  HeaptMaker<T> (IEnumerable<T> seq, ISimpleCompare<T>  cmp) {
            T[] arr = seq.ToArray();
            HeaptMaker ( ref arr, cmp);
            return arr;
        }


        /// <summary>
        /// Performs a BubbleSort on the container.
        /// </summary>
        /// <typeparam name="C">Container type.</typeparam>
        /// <typeparam name="T">The Value Type</typeparam>
        /// <param name="container">The container to sort.</param>
        /// <param name="cmp">Comparison strategy (e.g., Less).</param>
        /// <remarks>
        /// BubbleSort is simple and stable.  
        /// Best suited for small containers or educational purposes.  
        /// Worst-case complexity: O(n²).
        /// </remarks>
        public static void BubbleSort<T, C> ( ref C container, ISimpleCompare<T> cmp )
            where C : IVector<T>, ISwappable<long> {

            long n = container.Length;

            for ( long i = 0 ; i < n - 1 ; i++ ) {

                bool wasChanged = false;

                for ( long j = 0 ; j < n - i - 1 ; j++ ) {

                    if ( cmp.Compare(container.ElementAt(j), container.ElementAt(j + 1)) ) {

                        container.Swap(j, j + 1);
                        wasChanged = true;
                    }
                }

                if ( !wasChanged ) {
                    break;
                }
            }
        }

        public static void Swap<T>( ref T[] container, long a, long b ) {
            T temp = container[a];
            container[a] = container[b];
            container[b] = temp;
        }
        /// <summary>
        /// Performs a BubbleSort on a Array.
        /// </summary>
        public static void BubbleSort<T> ( ref T[] container, ISimpleCompare<T> cmp ) {

            long n = container.Length;

            for ( long i = 0 ; i < n - 1 ; i++ ) {

                bool wasChanged = false;

                for ( long j = 0 ; j < n - i - 1 ; j++ ) {

                    if ( cmp.Compare( container[j], container[j + 1] ) ) {

                        Swap(ref container, j, j + 1);
                        wasChanged = true;
                    }
                }

                if ( !wasChanged ) {
                    break;
                }
            }
        }

        public static T[] BubbleSort<T> (IEnumerable<T> seq, ISimpleCompare<T>  cmp) {
            T[] arr = seq.ToArray();
            BubbleSort(ref arr, cmp);
            return arr;
        }


        /// <summary>
        /// Performs an InsertionSort on the container.
        /// </summary>
        /// <typeparam name="C">Container type.</typeparam>
        /// <typeparam name="T">The Value Type</typeparam>
        /// <param name="container">The container to sort.</param>
        /// <param name="cmp">Comparison strategy.</param>
        /// <remarks>
        /// InsertionSort is stable and extremely efficient for nearly sorted data.  
        /// Ideal for small to medium-sized containers.  
        /// Worst-case complexity: O(n²), but excellent real-world performance.
        /// </remarks>
        public static void InsertionSort<T, C> ( ref C container, ISimpleCompare<T> cmp )
            where C : IVector<T>, ISwappable<long> {

            long n = container.Length;

            for ( var i = 1 ; i < n ; i++ ) {

                for ( var j = i ; j > 0 && cmp.Compare(container.ElementAt(j - 1), container.ElementAt(j)) ; j-- ) {

                    container.Swap(j - 1, j);
                }
            }
        }
        /// <summary>
        /// Performs a InsertionSort on a Array.
        /// </summary>
        public static void InsertionSort<T> ( ref T[] container, ISimpleCompare<T> cmp )  {

            long n = container.Length;

            for ( var i = 1 ; i < n ; i++ ) {

                for ( var j = i ; j > 0 && cmp.Compare(container[j - 1], container[j] ) ; j-- ) {

                    Swap(ref container, j - 1, j);
                }
            }
        }


        public static T[] InsertionSort<T> (IEnumerable<T> seq, ISimpleCompare<T>  cmp)  {
            T[] arr = seq.ToArray();
            InsertionSort(ref arr, cmp);
            return arr;
        }

        /// <summary>
        /// Performs a GnomeSort on the container.
        /// </summary>
        /// <typeparam name="C">Container type.</typeparam>
        /// <typeparam name="T">The Value Type</typeparam>
        /// <param name="container">The container to sort.</param>
        /// <param name="cmp">Comparison strategy.</param>
        /// <remarks>
        /// GnomeSort is a simple, stable sorting algorithm similar to InsertionSort.  
        /// Good for small to medium-sized containers.  
        /// Complexity: O(n²).
        /// </remarks>
        public static void GnomeSorter<T, C> ( ref C container, ISimpleCompare<T> cmp )
            where C : IVector<T>, ISwappable<long> {

            long j = 0;
            long n = container.Length;

            while ( j < n ) {

                if ( j == 0 || cmp.Compare(container.ElementAt(j), container.ElementAt(j - 1)) ) {
                    j++;
                } else {

                    container.Swap(j, j - 1);
                    j--;
                }
            }
        }


        /// <summary>
        /// Performs a GnomeSort on a Array.
        /// </summary>
        public static void GnomeSort<T> ( ref T[] container, ISimpleCompare<T> cmp ) { 

            long j = 0;
            long n = container.Length;

            while ( j < n ) {

                if ( j == 0 || cmp.Compare(container[j], container[j - 1] ) ) {
                    j++;
                } else {
                    Swap(ref container, j - 1, j);
                    j--;
                }
            }
        }
        /// <summary>
        /// Performs a GnomeSort on a IEnumerable not in place!
        /// </summary>
        public static T[] GnomeSort<T> (IEnumerable<T> seq, ISimpleCompare<T>  cmp) {
            T[] arr = seq.ToArray();
            GnomeSort(ref arr, cmp);
            return arr;
        }

        /// <summary>
        /// Performs a QuickSort using a randomized pivot.
        /// </summary>
        /// <typeparam name="C">Container type.</typeparam>
        /// <typeparam name="T">The Value Type</typeparam>
        /// <param name="container">The container to sort.</param>
        /// <param name="cmp">Comparison strategy.</param>
        /// <remarks>
        /// QuickSort is very fast with average complexity O(n log n).  
        /// Not stable.  
        /// Random pivot selection avoids worst-case behavior on already sorted data.
        /// </remarks>
        public static void QuickSorter<T, C> ( ref C container, ISimpleCompare<T> cmp )
            where C : IVector<T>, ISwappable<long> {

            void Sort ( ref C container, long left, long right, ISimpleCompare<T> cmp ) {
                long i = left;
                long j = right;

                Optional<T> pivot = container.ElementAt(RandUtils.RandLong(left, right + 1, Endian.System));

                while ( i <= j ) {
                    while ( cmp.Compare(container.ElementAt(i), pivot) ) i++;
                    while ( cmp.Compare(pivot, container.ElementAt(j)) ) j--;

                    if ( i <= j ) {
                        container.Swap(i, j);
                        i++;
                        j--;
                    }
                }

                if ( left < j ) Sort(ref container, left, j, cmp);
                if ( i < right ) Sort(ref container, i, right, cmp);
            }

            Sort(ref container, 0, container.Count - 1, cmp);
        }

        /// <summary>
        /// Performs a QuickSort on a Array.
        /// </summary>
        public static void QuickSort<T> ( ref T[] container, ISimpleCompare<T> cmp )
            {

            void Sort ( ref T[] container, long left, long right, ISimpleCompare<T> cmp ) {
                long i = left;
                long j = right;
                long v = RandUtils.RandLong(left, right + 1, Endian.System);

                T pivot = container[ v ];

                while ( i <= j ) {
                    while ( cmp.Compare(container[i], pivot) ) i++;
                    while ( cmp.Compare(pivot, container[j]) ) j--;

                    if ( i <= j ) {
                        Swap(ref container, i, j);
                        i++;
                        j--;
                    }
                }

                if ( left < j ) Sort(ref container, left, j, cmp);
                if ( i < right ) Sort(ref container, i, right, cmp);
            }

            Sort(ref container, 0, container.LongLength - 1, cmp);
        }
        /// <summary>
        /// Performs a QuickSort on a IEnumerable not in place!
        /// </summary>
        public static T[] QuickSort<T> (IEnumerable<T> seq, ISimpleCompare<T>  cmp) {
            T[] arr = seq.ToArray();
            QuickSort(ref arr, cmp);
            return arr;
        }

        /// <summary>
        /// Performs a ShellSort using the classic gap sequence (n/2, n/4, ..., 1).
        /// </summary>
        /// <typeparam name="C">Container type.</typeparam>
        /// <typeparam name="T">The Value Type</typeparam>
        /// <param name="container">The container to sort.</param>
        /// <param name="cmp">Comparison strategy.</param>
        /// <remarks>
        /// ShellSort is a fast, iterative sorting algorithm.  
        /// Typically runs in O(n^(3/2)).  
        /// Good for large containers and avoids recursion.  
        /// Uses gapped insertion sort to reduce disorder quickly.
        /// </remarks>
        public static void ShellSorter<T, C> ( ref C container, ISimpleCompare<T> cmp )
                where C : IVector<T>, ISwappable<long> {
            long n = container.Length;

            for ( long gap = n / 2 ; gap > 0 ; gap /= 2 ) {

                for ( long i = gap ; i < n ; i++ ) {

                    long j = i;

                    // solange das Element "links" größer ist, tauschen
                    while ( j >= gap && cmp.Compare(container.ElementAt(j), container.ElementAt(j - gap)) ) {
                        container.Swap(j, j - gap);
                        j -= gap;
                    }
                }
            }
        }

        /// <summary>
        /// Performs a ShellSort on a Array.
        /// </summary>
        public static void ShellSort<T> ( ref T[] container, ISimpleCompare<T> cmp ) {

            long n = container.Length;

            for ( long gap = n / 2 ; gap > 0 ; gap /= 2 ) {

                for ( long i = gap ; i < n ; i++ ) {

                    long j = i;

                    // solange das Element "links" größer ist, tauschen
                    while ( j >= gap && cmp.Compare(container[j], container[j - gap] ) ) {
                        Swap(ref container, j, j - gap);
                        j -= gap;
                    }
                }
            }
        }
        /// <summary>
        /// Performs a ShellSort on a IEnumerable not in place!
        /// </summary>
        public static T[] ShellSort<T> ( IEnumerable<T> seq, ISimpleCompare<T> cmp )  {
            T[] arr = seq.ToArray();
            ShellSort(ref arr, cmp);
            return arr;
        }

        /// <summary>
        /// Performs a CombSort on the container.
        /// </summary>
        /// <typeparam name="C">Container type implementing <see cref="IVector{T}"/>.</typeparam>
        /// <typeparam name="T">The Value Type</typeparam>
        /// <param name="container">The container to sort.</param>
        /// <param name="cmp">Comparison strategy (e.g., Less).</param>
        /// <remarks>
        /// CombSort is an improvement over BubbleSort that eliminates small values ("turtles")
        /// early by using a shrinking gap.  
        /// Average performance is significantly better than BubbleSort, often close to O(n log n).  
        /// Not stable.  
        /// Useful when a simple, fast, non‑recursive sort is desired.
        /// </remarks>
        public static void CombSorter<T, C> ( ref C container, ISimpleCompare<T> cmp )
                where C : IVector<T>, ISwappable<long> {

            long n = container.Length;
            long gap = n;
            bool sorted = false;
            float FAKTOR = 1.3f;

            while ( !sorted ) {
                gap = (long)System.Math.Floor(gap / FAKTOR);
                if ( gap <= 1 ) {
                    gap = 1;
                    sorted = true;
                }

                for ( long i = 0 ; i < n - gap ; i++ ) {
                    if ( cmp.Compare(container.ElementAt(i), container.ElementAt(i + gap)) ) {
                        container.Swap(i, i + gap);


                        sorted = false;
                    }
                }
            }
        }

        /// <summary>
        /// Performs a CombSort on a Array.
        /// </summary>
        public static void CombSort<T> ( ref T[] container, ISimpleCompare<T> cmp )  {

            long n = container.Length;
            long gap = n;
            bool sorted = false;
            float FAKTOR = 1.3f;

            while ( !sorted ) {
                gap = (long)System.Math.Floor(gap / FAKTOR);
                if ( gap <= 1 ) {
                    gap = 1;
                    sorted = true;
                }

                for ( long i = 0 ; i < n - gap ; i++ ) {
                    if ( cmp.Compare(container[i], container[i + gap]) ) {
                        Swap(ref container, i, i + gap);


                        sorted = false;
                    }
                }
            }
        }
        /// <summary>
        /// Performs a CombSort on a IEnumerable not in place!
        /// </summary>
        public static T[] CombSort<T> ( IEnumerable<T> seq, ISimpleCompare<T>  cmp ) {
            T[] arr = seq.ToArray();
            CombSort(ref arr, cmp);
            return arr;
        }
    }
    /// @}
}
