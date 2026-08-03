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
using SystemEx;
using SystemEx.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SystemEx.Utils {
    /// <summary>
    /// Specifies the result of a comparison between two values.
    /// </summary>
    public enum CompareResult : sbyte {
        /// <summary>
        /// <see cref="CompareResult.AIsSmallerB"/>
        /// </summary>
        Less = AIsSmallerB,
        /// <summary>
        /// <see cref="CompareResult.AIsLargerB"/>
        /// </summary>
        Greater = 1,
        /// <summary>
        /// <see cref="CompareResult.AIsEqualSmallerB"/>
        /// </summary>
        EqualLess = AIsEqualSmallerB,
        /// <summary>
        /// <see cref="CompareResult.AIsEqualLargerB"/>
        /// </summary>
        EqualGreater = AIsEqualLargerB,
        /// <summary>
        /// A is larger as B
        /// </summary>
        AIsLargerB = 1,
        /// <summary>
        /// A is smaller  as B
        /// </summary>
        AIsSmallerB = -1,
        /// <summary>
        /// A is Equal B
        /// </summary>
        Equal = 0,
        /// <summary>
        /// A is equal greater B
        /// </summary>
        AIsEqualLargerB = 2,
        /// <summary>
        /// A is equal smaller B
        /// </summary>
        AIsEqualSmallerB = 3,
        /// <summary>
        /// A or B or A and B are null
        /// </summary>
        Null = 10
    }

    /// <summary>
    /// Represents a comparison function for two values of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The value type to compare.</typeparam>
    /// <param name="a">The first value.</param>
    /// <param name="b">The second value.</param>
    /// <returns>The comparison result.</returns>
    public delegate CompareResult CompFunc<T>(T? a, T? b);

    /// <summary>
    /// Provides generic algorithm utilities similar to the C++ STL, including
    /// fill, copy, move, min/max, clamp, rotation, and lexicographical comparison
    /// for arrays.
    /// </summary>
    public static class Algorithm {

        /// <summary>
        /// Returns a random unsigned 64-bit integer in the given range using the specified endian mode.
        /// </summary>
        public static ulong Rand(ulong min, ulong max, Endian endian)
            => RandUtils.RandULong(min, max, endian);

        /// <summary>
        /// Returns a random signed 64-bit integer in the given range using the specified endian mode.
        /// </summary>
        public static long Rand(long min, long max, Endian endian)
            => RandUtils.RandLong(min, max, endian);

        /// <summary>
        /// Swaps the values of two variables.
        /// </summary>
        public static void Swap<T>(ref T x, ref T y) {
            T temp = x;
            x = y;
            y = temp;
        }

        /// <summary>
        /// Fills the entire array with the specified value.
        /// </summary>
        public static void Fill<T>(this T[] items, T value) {
            for ( int i = 0; i < items.Length; i++ )
                items[i] = value;
        }

        /// <summary>
        /// Fills a subrange of the array with the specified value.
        /// </summary>
        public static void FillN<T>(this T[] items, uint start, uint count, T value) {
            uint end = start + count;
            if ( end > items.Length )
                throw new ArgumentException("Index out of range");

            for ( uint i = start; i < start + count; i++ )
                items[i] = value;
        }

        /// <summary>
        /// Returns the maximum element in the array according to the given comparison function.
        /// </summary>
        public static T? MaxElement<T>(this T[] items, CompFunc<T> cmp) {
            T? largest = items[0];

            foreach ( T item in items ) {
                if ( cmp(largest, item) == CompareResult.AIsLargerB )
                    largest = item;
            }
            return largest;
        }

        /// <summary>
        /// Returns the minimum element in the array according to the given comparison function.
        /// </summary>
        public static T? MinElement<T>(this T[] items, CompFunc<T> cmp) {
            T? smallest = items[0];

            foreach ( T item in items ) {
                if ( cmp(smallest, item) == CompareResult.AIsSmallerB )
                    smallest = item;
            }
            return smallest;
        }

        /// <summary>
        /// Returns both the minimum and maximum elements in the array.
        /// </summary>
        public static Pair<T, T> MinMaxElement<T>(this T[] items, CompFunc<T> cmp) where T : notnull {
            T? min = MinElement(items, cmp);
            T? max = MaxElement(items, cmp);
            return new Pair<T, T>(min!, max!);
        }

        /// <summary>
        /// Returns the minimum and maximum of two values.
        /// </summary>
        public static Pair<T, T> MinMax<T>(T a, T b, CompFunc<T> cmp ) where T : notnull {
            return cmp(a, b) == CompareResult.AIsSmallerB
                ? new Pair<T, T>(b, a)
                : new Pair<T, T>(a, b);
        }

        /// <summary>
        /// Returns the smaller of two values according to the comparison function.
        /// </summary>
        public static T Min<T>(T a, T b, CompFunc<T> cmp) =>
            cmp(a, b) == CompareResult.AIsSmallerB ? a : b;

        /// <summary>
        /// Returns the larger of two values according to the comparison function.
        /// </summary>
        public static T Max<T>(T a, T b, CompFunc<T> cmp) =>
            cmp(a, b) == CompareResult.AIsLargerB ? a : b;

        /// <summary>
        /// Clamps a value to the inclusive range [min, max] using the comparison function.
        /// </summary>
        public static T Clamp<T>(T value, T min, T max, CompFunc<T> cmp) {
            if ( cmp(value, min) == CompareResult.AIsSmallerB ) return min;
            if ( cmp(value, max) == CompareResult.AIsLargerB ) return max;
            return value;
        }

        /// <summary>
        /// Copies a range of elements from one array to another.
        /// </summary>
        public static void Copy<T>(T[] src, uint srcIndex, T[] dst, uint dstIndex, uint count) {
            if ( srcIndex + count > src.Length )
                throw new ArgumentException("src out of range");
            if ( dstIndex + count > dst.Length )
                throw new ArgumentException("dst out of range");

            for ( uint i = 0; i < count; i++ )
                dst[dstIndex + i] = src[srcIndex + i];
        }

        /// <summary>
        /// Moves a range of elements from one array to another, correctly handling overlapping ranges.
        /// </summary>
        public static void Move<T>(T[] src, uint srcIndex, T[] dst, uint dstIndex, uint count) {
            if ( srcIndex + count > src.Length )
                throw new ArgumentException("src out of range");
            if ( dstIndex + count > dst.Length )
                throw new ArgumentException("dst out of range");

            if ( src == dst && dstIndex > srcIndex ) {
                for ( uint i = count; i > 0; i-- )
                    dst[dstIndex + i - 1] = src[srcIndex + i - 1];
            } else {
                for ( uint i = 0; i < count; i++ )
                    dst[dstIndex + i] = src[srcIndex + i];
            }
        }

        /// <summary>
        /// Reverses the entire array in place.
        /// </summary>
        public static void Reverse<T>(T[] items) {
            uint i = 0;
            uint j = (uint)items.Length - 1;

            while ( i < j ) {
                Swap(ref items[i], ref items[j]);
                i++;
                j--;
            }
        }

        /// <summary>
        /// Rotates the array around the specified middle index using the standard
        /// three-reverse algorithm.
        /// </summary>
        public static void Rotate<T>(T[] items, uint middle) {
            uint n = (uint)items.Length;
            if ( middle >= n ) return;

            ReverseRange(items, 0, middle - 1);
            ReverseRange(items, middle, n - 1);
            ReverseRange(items, 0, n - 1);
        }

        /// <summary>
        /// Reverses a subrange of the array in place.
        /// </summary>
        private static void ReverseRange<T>(T[] items, uint start, uint end) {
            while ( start < end ) {
                Swap(ref items[start], ref items[end]);
                start++;
                end--;
            }
        }

        /// <summary>
        /// Checks whether two arrays are element-wise equal according to the comparison function.
        /// </summary>
        public static bool Equal<T>(T[] a, T[] b, CompFunc<T> cmp) {
            if ( a.Length != b.Length ) return false;

            for ( int i = 0; i < a.Length; i++ )
                if ( cmp(a[i], b[i]) != CompareResult.Equal )
                    return false;

            return true;
        }

        /// <summary>
        /// Performs a lexicographical comparison between two arrays using the comparison function.
        /// </summary>
        public static bool LexicographicalCompare<T>(T[] a, T[] b, CompFunc<T> cmp) {
            int n = System.Math.Min(a.Length, b.Length);

            for ( int i = 0; i < n; i++ ) {
                var r = cmp(a[i], b[i]);
                if ( r == CompareResult.AIsSmallerB ) return true;
                if ( r == CompareResult.AIsLargerB ) return false;
            }

            return a.Length < b.Length;
        }
        /// <summary>
        /// Simple Quik Sort
        /// </summary>
        public static void QuickSort<T> ( T[] items, CompFunc<T> cmp ) {
            if ( items.Length <= 1 ) return;

            int low = 0;
            int high = items.Length - 1;

            while ( true ) {
                int i = low;
                int j = high;
                T pivot = items[(low + high) >> 1];

                do {
                    while ( cmp(items[i], pivot) == CompareResult.AIsSmallerB ) i++;
                    while ( cmp(pivot, items[j]) == CompareResult.AIsSmallerB ) j--;

                    if ( i <= j ) {
                        Algorithm.Swap(ref items[i], ref items[j]);
                        i++;
                        j--;
                    }
                } while ( i <= j );

                if ( low < j ) {
                    QuickSortRange(items, low, j, cmp);
                }

                if ( i < high ) {
                    low = i;
                } else break;
            }
        }

        private static void QuickSortRange<T> ( T[] items, int low, int high, CompFunc<T> cmp ) {
            int i = low;
            int j = high;
            T pivot = items[(low + high) >> 1];

            while ( true ) {
                while ( cmp(items[i], pivot) == CompareResult.AIsSmallerB ) i++;
                while ( cmp(pivot, items[j]) == CompareResult.AIsSmallerB ) j--;

                if ( i <= j ) {
                    Algorithm.Swap(ref items[i], ref items[j]);
                    i++;
                    j--;
                }

                if ( i > j ) break;
            }

            if ( low < j ) QuickSortRange(items, low, j, cmp);
            if ( i < high ) QuickSortRange(items, i, high, cmp);
        }
        /// <summary>
        /// Simple Heap sort
        /// </summary>
        public static void HeapSort<T> ( T[] items, CompFunc<T> cmp ) {
            int n = items.Length;

            for ( int k = n / 2 ; k > 0 ; k-- )
                DownHeap(items, k, n, cmp);

            while ( n > 1 ) {
                Algorithm.Swap(ref items[0], ref items[n - 1]);
                n--;
                DownHeap(items, 1, n, cmp);
            }
        }

        private static void DownHeap<T> ( T[] items, int k, int n, CompFunc<T> cmp ) {
            T temp = items[k - 1];

            while ( k <= n / 2 ) {
                int child = 2 * k;

                if ( child < n && cmp(items[child - 1], items[child]) == CompareResult.AIsSmallerB )
                    child++;

                if ( cmp(temp, items[child - 1]) == CompareResult.AIsSmallerB ) {
                    items[k - 1] = items[child - 1];
                    k = child;
                } else break;
            }

            items[k - 1] = temp;
        }

        /// <summary>
        /// Simple InsertionSort
        /// </summary>
        public static void InsertionSort<T> ( T[] items, CompFunc<T> cmp ) {
            for ( int i = 0 ; i < items.Length ; i++ ) {
                T t = items[i];
                int j = i;

                while ( j > 0 && cmp(t, items[j - 1]) == CompareResult.AIsSmallerB ) {
                    items[j] = items[j - 1];
                    j--;
                }

                items[j] = t;
            }
        }
        /// <summary>
        /// Is Sorted
        /// </summary>
        /// <returns>true when items are sorted</returns>
        public static bool IsSorted<T> ( T[] items, CompFunc<T> cmp ) {
            for ( int i = 1 ; i < items.Length ; i++ ) {
                if ( cmp(items[i], items[i - 1]) == CompareResult.AIsSmallerB )
                    return false;
            }
            return true;
        }


    }
}
