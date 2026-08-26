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

namespace SystemEx.Utils {
	/// \addtogroup Utils
	/// @{

	/// <summary>
	/// Specifies the result of a comparison between two values.
	/// 
	/// <para>
	/// This enumeration is used by <see cref="CompFunc{T}"/> and the generic
	/// algorithms in <see cref="Algorithm"/> to express ordering relations
	/// between two operands <c>A</c> and <c>B</c>. It generalizes the usual
	/// "less/greater/equal" semantics with additional states for "equal but
	/// smaller" and "equal but larger" to support nuanced ordering logic.
	/// </para>
	/// </summary>
	public enum CompareResult : sbyte {
		/// <summary>
		/// Alias for <see cref="CompareResult.AIsSmallerB"/>. Indicates that
		/// the first operand is strictly smaller than the second.
		/// </summary>
		Less = AIsSmallerB,

		/// <summary>
		/// Alias for <see cref="CompareResult.AIsLargerB"/>. Indicates that
		/// the first operand is strictly larger than the second.
		/// </summary>
		Greater = 1,

		/// <summary>
		/// Alias for <see cref="CompareResult.AIsEqualSmallerB"/>. Indicates
		/// that the first operand is equal to the second but considered
		/// "smaller" in a secondary ordering dimension.
		/// </summary>
		EqualLess = AIsEqualSmallerB,

		/// <summary>
		/// Alias for <see cref="CompareResult.AIsEqualLargerB"/>. Indicates
		/// that the first operand is equal to the second but considered
		/// "larger" in a secondary ordering dimension.
		/// </summary>
		EqualGreater = AIsEqualLargerB,

		/// <summary>
		/// The first operand <c>A</c> is strictly larger than the second
		/// operand <c>B</c>.
		/// </summary>
		AIsLargerB = 1,

		/// <summary>
		/// The first operand <c>A</c> is strictly smaller than the second
		/// operand <c>B</c>.
		/// </summary>
		AIsSmallerB = -1,

		/// <summary>
		/// The operands <c>A</c> and <c>B</c> are considered equal in the
		/// primary ordering dimension.
		/// </summary>
		Equal = 0,

		/// <summary>
		/// The operands are equal in the primary dimension, but <c>A</c> is
		/// treated as "greater" in a secondary dimension (e.g. tie‑breaking).
		/// </summary>
		AIsEqualLargerB = 2,

		/// <summary>
		/// The operands are equal in the primary dimension, but <c>A</c> is
		/// treated as "smaller" in a secondary dimension.
		/// </summary>
		AIsEqualSmallerB = 3,

		/// <summary>
		/// One or both operands are <c>null</c>, or the comparison function
		/// cannot produce a meaningful ordering result.
		/// </summary>
		Null = 10
	}

	/// <summary>
	/// Represents a comparison function for two values of type
	/// <typeparamref name="T"/>.
	/// 
	/// <para>
	/// The function receives two operands <c>a</c> and <c>b</c> and returns a
	/// <see cref="CompareResult"/> describing their ordering relation. This
	/// delegate is used throughout <see cref="Algorithm"/> to implement
	/// generic, type‑agnostic algorithms such as sorting, min/max selection,
	/// and lexicographical comparison.
	/// </para>
	/// </summary>
	/// <typeparam name="T">The value type to compare.</typeparam>
	/// <param name="a">The first value.</param>
	/// <param name="b">The second value.</param>
	/// <returns>
	/// A <see cref="CompareResult"/> indicating how <c>a</c> relates to <c>b</c>.
	/// </returns>
	public delegate CompareResult CompFunc<T>(T? a, T? b);

	//// <summary>
	/// Provides generic algorithm utilities similar to the C++ STL, including
	/// fill, copy, move, min/max, clamp, rotation, equality, lexicographical
	/// comparison, and several sorting algorithms for arrays.
	/// 
	/// <para>
	/// All algorithms operate on plain <see cref="System.Array"/> instances
	/// and rely on a caller‑supplied comparison function
	/// (<see cref="CompFunc{T}"/>) to define ordering semantics. This allows
	/// the same algorithm to be reused for arbitrary types without imposing
	/// <see cref="IComparable"/> constraints.
	/// </para>
	/// </summary>
	public static class Algorithm {

		/// <summary>
		/// Returns a random unsigned 64‑bit integer in the given range using
		/// the specified endian mode.
		/// </summary>
		/// <param name="min">Inclusive lower bound of the random range.</param>
		/// <param name="max">Inclusive upper bound of the random range.</param>
		/// <param name="endian">
		/// The endian mode used by the underlying random generator.
		/// </param>
		/// <returns>A random <see cref="ulong"/> in the range [min, max].</returns>
		public static ulong Rand(ulong min, ulong max, Endian endian)
            => RandUtils.RandULong(min, max, endian);

		/// <summary>
		/// Returns a random signed 64‑bit integer in the given range using
		/// the specified endian mode.
		/// </summary>
		/// <param name="min">Inclusive lower bound of the random range.</param>
		/// <param name="max">Inclusive upper bound of the random range.</param>
		/// <param name="endian">
		/// The endian mode used by the underlying random generator.
		/// </param>
		/// <returns>A random <see cref="long"/> in the range [min, max].</returns>
		public static long Rand(long min, long max, Endian endian)
            => RandUtils.RandLong(min, max, endian);

		/// <summary>
		/// Swaps the values of two variables.
		/// </summary>
		/// <typeparam name="T">The type of the values to swap.</typeparam>
		/// <param name="x">The first value.</param>
		/// <param name="y">The second value.</param>
		public static void Swap<T>(ref T x, ref T y) {
            T temp = x;
            x = y;
            y = temp;
        }

		/// <summary>
		/// Fills the entire array with the specified value.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="items">The target array to fill.</param>
		/// <param name="value">The value to assign to each element.</param>
		public static void Fill<T>(this T[] items, T value) {
            for ( int i = 0; i < items.Length; i++ )
                items[i] = value;
        }

		/// <summary>
		/// Fills a subrange of the array with the specified value.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="items">The target array to fill.</param>
		/// <param name="start">The starting index of the range.</param>
		/// <param name="count">The number of elements to fill.</param>
		/// <param name="value">The value to assign.</param>
		/// <exception cref="ArgumentException">
		/// Thrown when the specified range exceeds the array bounds.
		/// </exception>
		public static void FillN<T>(this T[] items, uint start, uint count, T value) {
            uint end = start + count;
            if ( end > items.Length )
                throw new ArgumentException("Index out of range");

            for ( uint i = start; i < start + count; i++ )
                items[i] = value;
        }

		/// <summary>
		/// Returns the maximum element in the array according to the given
		/// comparison function.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="items">The array to scan.</param>
		/// <param name="cmp">The comparison function.</param>
		/// <returns>The largest element found, or the first element if all are equal.</returns>
		public static T? MaxElement<T>(this T[] items, CompFunc<T> cmp) {
            T? largest = items[0];

            foreach ( T item in items ) {
                if ( cmp(largest, item) == CompareResult.AIsLargerB )
                    largest = item;
            }
            return largest;
        }

		/// <summary>
		/// Returns the minimum element in the array according to the given
		/// comparison function.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="items">The array to scan.</param>
		/// <param name="cmp">The comparison function.</param>
		/// <returns>The smallest element found, or the first element if all are equal.</returns>
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
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="items">The array to scan.</param>
		/// <param name="cmp">The comparison function.</param>
		/// <returns>
		/// A <see cref="Pair{T, T}"/> containing the minimum and maximum values.
		/// </returns>
		public static Pair<T, T> MinMaxElement<T>(this T[] items, CompFunc<T> cmp) where T : notnull {
            T? min = MinElement(items, cmp);
            T? max = MaxElement(items, cmp);
            return new Pair<T, T>(min!, max!);
        }

		/// <summary>
		/// Returns the minimum and maximum of two values.
		/// </summary>
		/// <typeparam name="T">The value type.</typeparam>
		/// <param name="a">The first value.</param>
		/// <param name="b">The second value.</param>
		/// <param name="cmp">The comparison function.</param>
		/// <returns>
		/// A <see cref="Pair{T, T}"/> containing the smaller and larger value.
		/// </returns>
		public static Pair<T, T> MinMax<T>(T a, T b, CompFunc<T> cmp ) where T : notnull {
            return cmp(a, b) == CompareResult.AIsSmallerB
                ? new Pair<T, T>(b, a)
                : new Pair<T, T>(a, b);
        }

		/// <summary>
		/// Returns the smaller of two values according to the comparison function.
		/// </summary>
		/// <typeparam name="T">The value type.</typeparam>
		/// <param name="a">The first value.</param>
		/// <param name="b">The second value.</param>
		/// <param name="cmp">The comparison function.</param>
		public static T Min<T>(T a, T b, CompFunc<T> cmp) =>
            cmp(a, b) == CompareResult.AIsSmallerB ? a : b;

		/// <summary>
		/// Returns the larger of two values according to the comparison function.
		/// </summary>
		/// <typeparam name="T">The value type.</typeparam>
		/// <param name="a">The first value.</param>
		/// <param name="b">The second value.</param>
		/// <param name="cmp">The comparison function.</param>
		public static T Max<T>(T a, T b, CompFunc<T> cmp) =>
            cmp(a, b) == CompareResult.AIsLargerB ? a : b;

		/// <summary>
		/// Clamps a value to the inclusive range [min, max] using the
		/// comparison function.
		/// </summary>
		/// <typeparam name="T">The value type.</typeparam>
		/// <param name="value">The value to clamp.</param>
		/// <param name="min">The inclusive lower bound.</param>
		/// <param name="max">The inclusive upper bound.</param>
		/// <param name="cmp">The comparison function.</param>
		/// <returns>
		/// <paramref name="min"/> if <paramref name="value"/> is smaller than
		/// <paramref name="min"/>; <paramref name="max"/> if larger than
		/// <paramref name="max"/>; otherwise <paramref name="value"/>.
		/// </returns>
		public static T Clamp<T>(T value, T min, T max, CompFunc<T> cmp) {
            if ( cmp(value, min) == CompareResult.AIsSmallerB ) return min;
            if ( cmp(value, max) == CompareResult.AIsLargerB ) return max;
            return value;
        }

		/// <summary>
		/// Copies a range of elements from one array to another.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="src">The source array.</param>
		/// <param name="srcIndex">The starting index in the source array.</param>
		/// <param name="dst">The destination array.</param>
		/// <param name="dstIndex">The starting index in the destination array.</param>
		/// <param name="count">The number of elements to copy.</param>
		/// <exception cref="ArgumentException">
		/// Thrown when the specified ranges exceed the array bounds.
		/// </exception>
		public static void Copy<T>(T[] src, uint srcIndex, T[] dst, uint dstIndex, uint count) {
            if ( srcIndex + count > src.Length )
                throw new ArgumentException("src out of range");
            if ( dstIndex + count > dst.Length )
                throw new ArgumentException("dst out of range");

            for ( uint i = 0; i < count; i++ )
                dst[dstIndex + i] = src[srcIndex + i];
        }

		/// <summary>
		/// Moves a range of elements from one array to another, correctly
		/// handling overlapping ranges.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="src">The source array.</param>
		/// <param name="srcIndex">The starting index in the source array.</param>
		/// <param name="dst">The destination array.</param>
		/// <param name="dstIndex">The starting index in the destination array.</param>
		/// <param name="count">The number of elements to move.</param>
		/// <exception cref="ArgumentException">
		/// Thrown when the specified ranges exceed the array bounds.
		/// </exception>
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
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="items">The array to reverse.</param>
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
		/// Rotates the array around the specified middle index using the
		/// standard three‑reverse algorithm.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="items">The array to rotate.</param>
		/// <param name="middle">
		/// The index that becomes the new start of the array after rotation.
		/// </param>
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
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="items">The array to modify.</param>
		/// <param name="start">The starting index of the range.</param>
		/// <param name="end">The ending index of the range.</param>
		private static void ReverseRange<T>(T[] items, uint start, uint end) {
            while ( start < end ) {
                Swap(ref items[start], ref items[end]);
                start++;
                end--;
            }
        }

		/// <summary>
		/// Checks whether two arrays are element‑wise equal according to the
		/// comparison function.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="a">The first array.</param>
		/// <param name="b">The second array.</param>
		/// <param name="cmp">The comparison function.</param>
		/// <returns>
		/// <c>true</c> if both arrays have the same length and all elements
		/// compare as <see cref="CompareResult.Equal"/>; otherwise <c>false</c>.
		/// </returns>
		public static bool Equal<T>(T[] a, T[] b, CompFunc<T> cmp) {
            if ( a.Length != b.Length ) return false;

            for ( int i = 0; i < a.Length; i++ )
                if ( cmp(a[i], b[i]) != CompareResult.Equal )
                    return false;

            return true;
        }

		/// <summary>
		/// Performs a lexicographical comparison between two arrays using the
		/// comparison function.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="a">The first array.</param>
		/// <param name="b">The second array.</param>
		/// <param name="cmp">The comparison function.</param>
		/// <returns>
		/// <c>true</c> if <paramref name="a"/> is lexicographically smaller
		/// than <paramref name="b"/>; otherwise <c>false</c>.
		/// </returns>
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
		/// Simple quicksort implementation for arrays using a custom
		/// comparison function.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="items">The array to sort.</param>
		/// <param name="cmp">The comparison function.</param>
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

		/// <summary>
		/// Recursive helper for quicksort that sorts a subrange of the array.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="items">The array to sort.</param>
		/// <param name="low">The lower bound of the range.</param>
		/// <param name="high">The upper bound of the range.</param>
		/// <param name="cmp">The comparison function.</param>
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
		/// Simple heap sort implementation for arrays using a custom
		/// comparison function.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="items">The array to sort.</param>
		/// <param name="cmp">The comparison function.</param>
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

		/// <summary>
		/// Restores the heap property for a subtree rooted at the given index.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="items">The heap array.</param>
		/// <param name="k">The 1‑based index of the root node.</param>
		/// <param name="n">The number of elements in the heap.</param>
		/// <param name="cmp">The comparison function.</param>
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
		/// Simple insertion sort implementation for arrays using a custom
		/// comparison function.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="items">The array to sort.</param>
		/// <param name="cmp">The comparison function.</param>
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
		/// Determines whether the array is sorted in non‑decreasing order
		/// according to the comparison function.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="items">The array to inspect.</param>
		/// <param name="cmp">The comparison function.</param>
		/// <returns>
		/// <c>true</c> if each element is greater than or equal to its
		/// predecessor; otherwise <c>false</c>.
		/// </returns>
		public static bool IsSorted<T> ( T[] items, CompFunc<T> cmp, CompareResult equalIsNot = CompareResult.AIsSmallerB ) {
            for ( int i = 1 ; i < items.Length ; i++ ) {
                if ( cmp(items[i], items[i - 1]) == equalIsNot )
                    return false;
            }
            return true;
        }
    }

    /// @}
}
