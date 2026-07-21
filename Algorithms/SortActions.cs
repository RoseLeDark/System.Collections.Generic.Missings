using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SystemEx.Algorithms.Interfaces;
using SystemEx.Algorythmen;
using SystemEx.Collections.Generic.Interfaces;
using SystemEx.Random;
using SystemEx.Utils;

namespace SystemEx.Algorithms {


    /// <summary>
    /// Provides a set of sorting algorithms that operate on any container
    /// implementing <see cref="IContainerEx{T}"/>.  
    /// All algorithms use <see cref="ISimpleCompare{T}"/> as their comparison strategy.
    /// </summary>
    public static class SortActions {

        public static void HeapSort<T, C> ( ref C container, ISimpleCompare<T> cmp )
            where C : IContainerEx<T> {

        }

        /// <summary>
        /// Performs a BubbleSort on the container.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <typeparam name="C">Container type.</typeparam>
        /// <param name="container">The container to sort.</param>
        /// <param name="cmp">Comparison strategy (e.g., Less).</param>
        /// <remarks>
        /// BubbleSort is simple and stable.  
        /// Best suited for small containers or educational purposes.  
        /// Worst-case complexity: O(n²).
        /// </remarks>
        public static void BubbleSort<T, C> ( ref C container, ISimpleCompare<T> cmp )
            where C : IContainerEx<T> {

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

        /// <summary>
        /// Performs an InsertionSort on the container.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <typeparam name="C">Container type.</typeparam>
        /// <param name="container">The container to sort.</param>
        /// <param name="cmp">Comparison strategy.</param>
        /// <remarks>
        /// InsertionSort is stable and extremely efficient for nearly sorted data.  
        /// Ideal for small to medium-sized containers.  
        /// Worst-case complexity: O(n²), but excellent real-world performance.
        /// </remarks>
        public static void InsertionSort<T, C> ( ref C container, ISimpleCompare<T> cmp )
            where C : IContainerEx<T> {

            long n = container.Length;

            for ( var i = 1 ; i < n ; i++ ) {

                for ( var j = i ; j > 0 && cmp.Compare(container.ElementAt(j - 1), container.ElementAt(j)) ; j-- ) {

                    container.Swap(j - 1, j);
                }
            }
        }

        /// <summary>
        /// Performs a GnomeSort on the container.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <typeparam name="C">Container type.</typeparam>
        /// <param name="container">The container to sort.</param>
        /// <param name="cmp">Comparison strategy.</param>
        /// <remarks>
        /// GnomeSort is a simple, stable sorting algorithm similar to InsertionSort.  
        /// Good for small to medium-sized containers.  
        /// Complexity: O(n²).
        /// </remarks>
        public static void GnomeSorter<T, C> ( ref C container, ISimpleCompare<T> cmp )
            where C : IContainerEx<T> {

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
        /// Performs a QuickSort using a randomized pivot.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <typeparam name="C">Container type.</typeparam>
        /// <param name="container">The container to sort.</param>
        /// <param name="cmp">Comparison strategy.</param>
        /// <remarks>
        /// QuickSort is very fast with average complexity O(n log n).  
        /// Not stable.  
        /// Random pivot selection avoids worst-case behavior on already sorted data.
        /// </remarks>
        public static void QuickSorter<T, C> ( ref C container, ISimpleCompare<T> cmp )
            where C : IContainerEx<T> {

            void Sort ( ref C container, long left, long right, ISimpleCompare<T> cmp ) {
                long i = left;
                long j = right;

                T pivot = container.ElementAt(RandUtils.RandLong(left, right + 1, Endian.System));

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
        /// Performs a ShellSort using the classic gap sequence (n/2, n/4, ..., 1).
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <typeparam name="C">Container type.</typeparam>
        /// <param name="container">The container to sort.</param>
        /// <param name="cmp">Comparison strategy.</param>
        /// <remarks>
        /// ShellSort is a fast, iterative sorting algorithm.  
        /// Typically runs in O(n^(3/2)).  
        /// Good for large containers and avoids recursion.  
        /// Uses gapped insertion sort to reduce disorder quickly.
        /// </remarks>
        public static void ShellSorter<T, C> ( ref C container, ISimpleCompare<T> cmp )
                where C : IContainerEx<T> {
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
        /// Performs a CombSort on the container.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <typeparam name="C">Container type implementing <see cref="IContainerEx{T}"/>.</typeparam>
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
                where C : IContainerEx<T> {

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

                for ( long i = 0 ; i <n - gap ; i++ ) {
                    if ( cmp.Compare(container.ElementAt(i), container.ElementAt(i + gap))  ) {
                        container.Swap(i, i + gap);

                       
                        sorted = false;
                    }
                }
            }
        }
    }
}
