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


#pragma warning disable CS8500 
namespace SystemEx {
    /// <summary>
    /// Performs a raw, pointer‑based copy operation between two managed arrays.
    /// 
    /// No bounds checking is performed. The caller is responsible for ensuring
    /// that the offsets and element count are valid for both arrays.
    /// </summary>
    public static class Buffer {

        /// <summary>
        /// Copies a sequence of elements from one array to another using raw
        /// pointer arithmetic. Both arrays are pinned for the duration of the
        /// operation, and the copy is performed element‑by‑element.
        /// </summary>
        /// <typeparam name="T">
        /// The element type of the arrays. Must be a blittable type to ensure
        /// safe pointer access.
        /// </typeparam>
        /// <param name="src">
        /// The source array. If <c>null</c>, the method returns immediately.
        /// </param>
        /// <param name="srcOffset">
        /// The starting index within the source array.
        /// </param>
        /// <param name="dst">
        /// The destination array. If <c>null</c>, the method returns immediately.
        /// </param>
        /// <param name="dstOffset">
        /// The starting index within the destination array.
        /// </param>
        /// <param name="count">
        /// The number of elements to copy.
        /// </param>
        /// <remarks>
        /// This method does not validate array bounds or ensure that the element
        /// type is blittable. Incorrect usage may result in memory corruption or
        /// undefined behavior.
        /// </remarks>
        public static unsafe void LongCopy<T> ( T[] src, long srcOffset, T[] dst, long dstOffset, long count ) {
            if ( src == null || dst == null ) return;

            fixed ( T* pSrc = src )
            fixed ( T* pDst = dst ) {
                T* s = pSrc + srcOffset;
                T* d = pDst + dstOffset;

                for ( long i = 0 ; i < count ; i++ )
                    d[i] = s[i];
            }
        }

        /// <summary>
        /// Fills a segment of an array with a specified value using raw pointer access.
        /// 
        /// The array is pinned for the duration of the operation, and each element in
        /// the specified range is written directly via pointer arithmetic. This method
        /// provides a low‑level, unsafe alternative to <see cref="System.Array.Fill{T}(T[], T, int, int)"/>.
        /// 
        /// No bounds checking is performed. The caller must ensure that the offset and
        /// count are valid for the target array.
        /// </summary>
        /// <typeparam name="T">
        /// The element type of the array. Must be blittable to ensure safe pointer access.
        /// </typeparam>
        /// <param name="dst">The destination array to fill.</param>
        /// <param name="dstOffset">The starting index within the array.</param>
        /// <param name="value">The value to assign to each element.</param>
        /// <param name="count">The number of elements to fill.</param>

        public static unsafe void LongFill<T> ( T[] dst, long dstOffset, T value, long count ) {
            if ( dst == null ) return;

            fixed ( T* pDst = dst ) {
                T* d = pDst + dstOffset;

                for ( long i = 0 ; i < count ; i++ )
                    d[i] = value;
            }
        }


        /// <summary>
        /// Moves a block of elements within a single array using raw pointer access.
        /// 
        /// This method behaves similarly to <see cref="System.Buffer.MemoryCopy(void*, void*, ulong, ulong)"/> but operates
        /// on managed arrays. The array is pinned and the specified number of elements
        /// are copied from the source offset to the destination offset.
        /// 
        /// Overlapping regions are supported, but the caller must ensure that the
        /// offsets and count are valid. No bounds checking is performed.
        /// </summary>
        /// <typeparam name="T">
        /// The element type of the array. Must be blittable to ensure safe pointer access.
        /// </typeparam>
        /// <param name="array">The array containing both the source and destination regions.</param>
        /// <param name="srcOffset">The starting index of the source region.</param>
        /// <param name="dstOffset">The starting index of the destination region.</param>
        /// <param name="count">The number of elements to move.</param>

        public static unsafe void LongMemCopy<T> ( T[] array, long srcOffset, long dstOffset, long count ) {
            if ( array == null ) return;

            fixed ( T* p = array ) {
                T* src = p + srcOffset;
                T* dst = p + dstOffset;

                if ( dst > src ) {
                    // backwards copy for overlapping regions
                    for ( long i = count - 1 ; i >= 0 ; i-- )
                        dst[i] = src[i];
                } else {
                    // forward copy
                    for ( long i = 0 ; i < count ; i++ )
                        dst[i] = src[i];
                }
            }
        }


        /// <summary>
        /// Swaps two elements in an array using raw pointer access.
        /// 
        /// The array is pinned and the values at the specified indices are exchanged
        /// directly through pointer operations. This method provides a minimal‑overhead
        /// alternative to traditional element swapping.
        /// 
        /// No bounds checking is performed. The caller must ensure that both indices
        /// are valid for the array.
        /// </summary>
        /// <typeparam name="T">
        /// The element type of the array. Must be blittable to ensure safe pointer access.
        /// </typeparam>
        /// <param name="array">The array containing the elements to swap.</param>
        /// <param name="indexA">The index of the first element.</param>
        /// <param name="indexB">The index of the second element.</param>

        public static unsafe void LongSwap<T> ( T[] array, long indexA, long indexB ) {
            if ( array == null ) return;

            fixed ( T* p = array ) {
                T* a = p + indexA;
                T* b = p + indexB;

                T tmp = *a;
                *a = *b;
                *b = tmp;
            }
        }


        /// <summary>
        /// Sets a block of memory within an array to zero using raw pointer access.
        /// 
        /// The array is pinned and each element in the specified range is overwritten
        /// with <c>default(T)</c>. This method provides a low‑level alternative to
        /// clearing memory without invoking higher‑level array operations.
        /// 
        /// No bounds checking is performed. The caller must ensure that the offset and
        /// count are valid for the target array.
        /// </summary>
        /// <typeparam name="T">
        /// The element type of the array. Must be blittable to ensure safe pointer access.
        /// </typeparam>
        /// <param name="dst">The array whose memory region will be zeroed.</param>
        /// <param name="dstOffset">The starting index of the region to clear.</param>
        /// <param name="count">The number of elements to reset to <c>default(T)</c>.</param>

        public static unsafe void LongZeroMemory<T> ( T[] dst, long dstOffset, long count ) {
            if ( dst == null ) return;


            fixed ( T* pDst = dst ) {
                T* d = pDst + dstOffset;

                for ( long i = 0 ; i < count ; i++ )
                    d[i] = default;
            }

        }

    }
}
#pragma warning restore CS8500