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

using System.Collections;
using System.Drawing;
using SystemEx.Algorithms;
using SystemEx.Algorithms.Interfaces;

namespace SystemEx.Collections.Generic {
    /// \addtogroup collections
    /// @{
    /// <summary>
    /// A fixed-size array container that provides indexed access, insertion,
    /// traversal, and basic search operations. Unlike dynamic arrays, this
    /// structure never grows and always maintains a constant capacity.
    /// </summary>
    public struct FixedVector<T> : IVector<T>, ISwappable<long>, IUsedIterrator<T, RandomAccessIterator<T, FixedVector<T>>> {
        /// <summary>
        /// Internal storage buffer for Vector elements.
        /// </summary>
        private T[] m_elements;
        /// <summary>
        /// Current number of valid elements stored in the Vector.
        /// </summary>
        private long m_index;

        /// <summary>
        /// The real size
        /// </summary>
        public long Length => m_elements.LongLength;

        /// <summary>
        /// Gets the first element of the Vector.
        /// </summary>
        public Optional<T> Front => m_elements[0];
        /// <summary>
        /// Gets the last element of the Vector.
        /// </summary>
        public Optional<T> Back => m_elements[Count];

        /// <summary>
        /// Indicates whether the Vector is full.
        /// </summary>
        public bool IsFull => ( m_index >= Length);

        /// <summary>
        /// Indicates whether the Vector contains no elements.
        /// </summary>
        public bool IsEmpty => m_index == 0;


        /// <summary>
        /// Gets the number of valid elements currently stored in the vector.
        /// 
        /// This is the logical element count, not the underlying array length.
        /// The value corresponds to the next free index (m_index).
        /// </summary>
        public long Count => m_index;

        /// <summary>
        /// Gets the element at the current logical position (m_index).
        /// 
        /// This is primarily useful during manual iteration or when treating the
        /// vector as a stack-like structure. Accessing Current when the vector is
        /// empty or m_index is out of range is undefined.
        /// </summary>
        public Optional<T> Current => m_elements[m_index];

        /// <summary>
        /// Creates a <see cref="Find{T, TContainer}"/> wrapper for this vector,
        /// providing direct access to element lookup utilities.
        /// </summary>
        /// <param name="vec">
        /// The vector instance to operate on.
        /// </param>
        /// <returns>
        /// A <see cref="Find{T, TContainer}"/> bound to the given vector.
        /// </returns>
        public static Find<T, Vector<T>> AsFinder ( ref Vector<T> vec )
            => new Find<T, Vector<T>>(ref vec);


        /// <summary>
        /// Creates a <see cref="VectorSet{T, Vector{T}}"/> view over the given vector.
        /// </summary>
        /// <typeparam name="T">Element type stored in the vector.</typeparam>
        /// <param name="vec">
        /// The vector whose contents should be interpreted as a sorted, unique set.
        /// </param>
        /// <param name="comparer">
        /// Optional comparison strategy used to determine ordering inside the set.
        /// If <c>null</c>, the default <see cref="Less{T}"/> comparer is used,
        /// which orders elements in ascending order.
        /// </param>
        /// <param name="sorter">
        /// Optional sorting algorithm used to arrange the vector before constructing the set.
        /// If <c>null</c>, <see cref="SortActions.ShellSorter"/> is used as the default,
        /// providing fast, predictable, non‑recursive sorting suitable for general use.
        /// </param>
        /// <returns>
        /// A <see cref="VectorSet{T, Vector{T}}"/> that wraps the provided vector, ensuring
        /// that elements are sorted according to the chosen comparer and that duplicates
        /// are handled according to the set's semantics.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method transforms a <see cref="Vector{T}"/> into a logical set by:
        /// </para>
        /// <list type="number">
        ///   <item>
        ///     <description>
        ///     Sorting the vector using the provided or default sorting algorithm.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///     Applying the comparison strategy to define the ordering of elements.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///     Constructing a <see cref="VectorSet{T, Vector{T}}"/> wrapper that interprets
        ///     the sorted vector as a unique, ordered collection.
        ///     </description>
        ///   </item>
        /// </list>
        /// <para>
        /// Beginners:  
        /// Think of this as “turn my vector into a sorted set”, where the library chooses
        /// sensible defaults if you don’t specify anything.
        /// </para>
        /// <para>
        /// Professionals:  
        /// This method provides full control over ordering and sorting policies while
        /// maintaining predictable defaults (<see cref="Less{T}"/> + ShellSort).  
        /// It allows custom comparers (e.g., reverse order, length‑based, domain‑specific)
        /// and custom sorting strategies (QuickSort, CombSort, etc.) without forcing
        /// the user to configure them for common cases.
        /// </para>
        /// </remarks>
        public static VectorSet<T, Vector<T>> AsSet (
            ref Vector<T> vec,
            ISimpleCompare<T>? comparer = null,
            SortAction<ISimpleCompare<T>, Vector<T>>? sorter = null )
            => new VectorSet<T, Vector<T>>(
                ref vec,
                comparer == null ? new Less<T>() : comparer,
                sorter == null ? SortActions.ShellSorter : sorter
            );


        /// <summary>
        /// Creates a <see cref="VectorMultiSet{T, Vector{T}}"/> view over the given vector,
        /// allowing duplicate elements while preserving a defined ordering.
        /// </summary>
        /// <typeparam name="T">Element type stored in the vector.</typeparam>
        /// <param name="vec">
        /// The vector whose contents should be interpreted as an ordered multiset.
        /// </param>
        /// <param name="comparer">
        /// Optional comparison strategy used to determine ordering inside the multiset.
        /// If <c>null</c>, the default <see cref="Less{T}"/> comparer is used,
        /// producing ascending order.
        /// </param>
        /// <param name="sorter">
        /// Optional sorting algorithm used to arrange the vector before constructing
        /// the multiset.  
        /// If <c>null</c>, <see cref="SortActions.ShellSorter"/> is used as a safe,
        /// fast, non‑recursive default.
        /// </param>
        /// <returns>
        /// A <see cref="VectorMultiSet{T, Vector{T}}"/> wrapper that interprets the vector
        /// as a sorted collection that may contain duplicates.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method transforms the vector into a logical multiset by:
        /// </para>
        /// <list type="number">
        ///   <item>
        ///     <description>Sorting the vector using the chosen or default sorter.</description>
        ///   </item>
        ///   <item>
        ///     <description>Applying the comparer to define element ordering.</description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///     Constructing a multiset wrapper that allows duplicates while maintaining
        ///     sorted order.
        ///     </description>
        ///   </item>
        /// </list>
        /// <para>
        /// Beginners:  
        /// Think of this as “turn my vector into a sorted collection that allows duplicates”.
        /// </para>
        /// <para>
        /// Professionals:  
        /// Provides full control over ordering and sorting policies while maintaining
        /// predictable defaults (<see cref="Less{T}"/> + ShellSort).  
        /// Suitable for frequency tables, histograms, and domain‑specific ordering.
        /// </para>
        /// </remarks>

        public static VectorMultiSet<T, FixedVector<T>> AsMultiSet (
            ref FixedVector<T> vec,
            ISimpleCompare<T>? comparer = null,
            SortAction<ISimpleCompare<T>, FixedVector<T>>? sorter = null )
            => new VectorMultiSet<T, FixedVector<T>>(
                ref vec,
                comparer == null ? new Less<T>() : comparer,
                sorter == null ? SortActions.ShellSorter : sorter
            );



        /// <summary>
        /// Creates an <see cref="VectorUnorderedSet{T, FixedVector{T}}"/> view over the Array,
        /// representing a unique collection without any defined ordering.
        /// </summary>
        /// <typeparam name="T">Element type stored in the Array.</typeparam>
        /// <param name="vec">
        /// The Array whose contents should be interpreted as an unordered set.
        /// </param>
        /// <returns>
        /// An <see cref="VectorUnorderedSet{T, FixedVector{T}}"/> wrapper that treats the Array
        /// as a unique, unordered collection.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Unlike <see cref="VectorSet{T, FixedVector{T}}"/>, this structure does not sort the Array.
        /// It simply ensures that elements are treated as unique, ignoring duplicates.
        /// </para>
        /// <para>
        /// Beginners:  
        /// Think of this as “a set where order does not matter”.
        /// </para>
        /// <para>
        /// Professionals:  
        /// Useful when ordering is irrelevant or when hashing/bucketing semantics are
        /// preferred.  
        /// No sorting or comparison strategy is required.
        /// </para>
        /// </remarks>

        public static VectorUnorderedSet<T, FixedVector<T>> AsUnorderedSet ( ref FixedVector<T> vec )
            => new VectorUnorderedSet<T, FixedVector<T>>(ref vec);

        /// <summary>
        /// Creates a FlexSpan view over the entire Array starting at index 0.
        /// 
        /// The view uses the specified indexing mode (System, Reverse, Ring) and
        /// provides a span-like interface backed directly by this Array.
        /// </summary>
        /// <param name="Array">
        /// Reference to the Array. Passed by ref to avoid copying the struct and
        /// to ensure the FlexSpan reflects the actual container.
        /// </param>
        /// <param name="mode">
        /// Indexing mode for the view:
        /// System  = forward indexing,
        /// Reverse = backward indexing,
        /// Ring    = circular wrap-around indexing.
        /// </param>
        /// <returns>
        /// A FlexSpan representing the full Array.
        /// </returns>
        public static VectorFlexSpan<T, FixedVector<T>> AsFlexSpan ( ref FixedVector<T> vector, FlexSpanMode mode = FlexSpanMode.System )
            => new VectorFlexSpan<T, FixedVector<T>>(ref vector, 0, mode);

            

        /// <summary>
        /// Creates a FlexSpan view over a specific range of the Array.
        /// 
        /// The view covers the range [start .. end) and uses the specified indexing mode.
        /// No memory is allocated; this is a pure logical slice backed by the Array.
        /// </summary>
        /// <param name="Array">
        /// Reference to the Array. Passed by ref so the FlexSpan operates on the
        /// actual container rather than a copy.
        /// </param>
        /// <param name="start">
        /// Starting index of the view. Must be within the Array's logical bounds.
        /// </param>
        /// <param name="end">
        /// Exclusive end index of the view. Must be greater than or equal to start
        /// and within the Array's logical bounds.
        /// </param>
        /// <param name="mode">
        /// Indexing mode for the view:
        /// System  = forward indexing,
        /// Reverse = backward indexing,
        /// Ring    = circular wrap-around indexing.
        /// </param>
        /// <returns>
        /// A FlexSpan representing the specified range of the Array.
        /// </returns>
        public static VectorFlexSpan<T, FixedVector<T>> AsFlexSpan ( ref FixedVector<T> vector, long start, long end, FlexSpanMode mode = FlexSpanMode.System )
            => new VectorFlexSpan<T, FixedVector<T>>(ref vector, start, end, mode);

        /// <summary>
        /// Creates a logical segment (sub-Array) over the internal buffer.
        /// The segment shares the same underlying Array and does not copy data.
        /// 
        /// This is *not* a slicing List-like structure: it behaves as a memory segment
        /// with its own logical length but without shifting or relocating elements.
        /// </summary>
        /// <param name="start">
        /// The starting index of the segment.
        /// </param>
        /// <param name="length">
        /// The number of elements included in the segment.
        /// </param>
        public FixedVector<T> AsSegment ( long start, long length ) {
            if ( start < 0 || length < 0 )
                throw new ArgumentOutOfRangeException();

            // Ensure the segment lies fully inside the internal buffer
            ArgumentOutOfRangeException.ThrowIfLessThan((start + length), m_elements.Length);

            // Create a new Array<T> that shares the same buffer
            var seg = new FixedVector<T>(m_elements);
            // Logical end of the segment
            seg.m_index = (int)(start + length);

            return seg;
        }
        /// <summary>
        /// Get The Type of T
        /// </summary>
        /// <returns>The Type of T</returns>
        public Type GetElementType () {
            return typeof(T);
        }
        /// <summary>
        /// Provides indexed access to the Array elements.
        /// </summary>
        public T this[long index] {
            get => m_elements[index];
            set => Insert(index, value);
        }

        public RandomAccessIterator<T, FixedVector<T>> Begin => new RandomAccessIterator<T, FixedVector<T>>(this, 0);
        public RandomAccessIterator<T, FixedVector<T>> End => new RandomAccessIterator<T, FixedVector<T>>(this, Count);



        /// <summary>
        /// Creates a new Array with a specified initial capacity.
        /// The Array starts empty (Count = 0).
        /// </summary>
        /// <param name="size"> Initial capacity of the internal buffer. No elements are considered valid yet. </param>
        public FixedVector ( long size) {
            m_elements = new T[size];
            m_index = 0;
        }

        /// <summary>
        /// Creates a new Array using an existing buffer.
        /// The buffer is adopted as-is, and Count is set
        /// to the last valid index. 
        /// </summary>
        /// <param name="e">
        /// Existing array used as the internal storage.
        /// </param>
        public FixedVector ( T[] e ) {
            m_elements = e;
            m_index = e.LongLength;
        }

        /// <summary>
        /// Copy ctor
        /// </summary>
        public FixedVector ( FixedVector<T> other ) {
            m_elements = new T[other.Length];
            Buffer.LongCopy<T>(other.m_elements, 0, m_elements, 0, other.Length);
        }

        /// <summary>
        /// Appends an element to the end of the Array.
        /// </summary>
        /// <param name="entry">Element to append.</param>
        /// <returns>
        /// True if the element was appended; false if the Array was full.
        /// </returns>
        public bool PushBack ( T entry ) {
            if ( m_index >= Length ) {
                return false;
            }

            m_elements[m_index] = entry;
            m_index++;
            return true;
        }


        /// <summary>
        /// Inserts an element at the specified index, shifting elements to the right.
        /// </summary>
        /// <param name="index">Insertion index.</param>
        /// <param name="entry">Element to insert.</param>
        /// <returns>
        /// True if insertion succeeded; false if the index was invalid.
        /// </returns>
        public bool Insert ( long index, T entry ) {
            if ( index < 0 ) return false;

            // Grow wie im Indexer
            if ( index >= m_elements.Length || m_index >= m_elements.Length ) {
                return false;
            }

            // Speicher nach rechts verschieben
            for ( long i = m_index ; i > index ; i-- )
                m_elements[i] = m_elements[i - 1];

            m_elements[index] = entry;
            m_index++;

            return true;
        }
        /// <summary>
        /// Fills a range of indices with a single value.
        /// </summary>
        /// <param name="start">Start index.</param>
        /// <param name="end">End index (inclusive).</param>
        /// <param name="entry">Value to write.</param>
        /// <returns>
        /// True if the operation succeeded; false if the range was invalid.
        /// </returns>
        public bool Insert ( long start, long end, T entry ) {
            if ( start < 0 || end < start ) return false;

            bool _ret = true;

            // Prüfen ob wir bis end schreiben können
            if ( end >= m_elements.Length ) {
                _ret = false;
            } else {

                // Bereich füllen
                for ( long i = start ; i <= end ; i++ )
                    m_elements[i] = entry;

                // m_index anpassen
                if ( end + 1 > m_index )
                    m_index = end + 1;
            }
            return _ret;
        }

        /// <summary>
        /// Inserts a sequence of elements starting at the specified index.
        /// </summary>
        /// <param name="start">Start index.</param>
        /// <param name="entrys">Elements to insert.</param>
        /// <returns>
        /// True if all elements were inserted; false if any insertion failed.
        /// </returns>
        public bool InsertRange ( long start, T[] entrys ) {
            bool _ret = true;

            for ( long i = 0 ; i < entrys.Length ; i++ ) {
                if ( !Insert(start + i, entrys[i]) ) {
                    _ret = false;
                    break;
                }
            }
            return _ret;
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// Automatically grows the buffer if needed and AutoGrow is enabled.
        /// </summary>
        /// <param name="index">Index to replace.</param>
        /// <param name="entry">New value.</param>
        /// <returns>
        /// True if replacement succeeded; false if the index was invalid or growth was not allowed.
        /// </returns>
        public bool Replace ( long index, T entry ) {
            if ( index < 0 ) return false;

            bool _ret = false;

            if ( index < m_elements.Length ) {
                m_elements[index] = entry;
                _ret = true;
            }
            return _ret;
        }

        /// <summary>
        /// Replaces a range of elements with a single value.
        /// </summary>
        /// <param name="start">Start index.</param>
        /// <param name="end">End index (inclusive).</param>
        /// <param name="entry">Value to write.</param>
        /// <returns>
        /// True if replacement succeeded; false if the range was invalid.
        /// </returns>
        public bool Replace ( long start, long end, T entry ) {
            if ( start < 0 || end < start ) return false;

            for ( long i = start ; i <= end ; i++ )
                m_elements[i] = entry;

            return true;
        }

        /// <summary>
        /// Replaces a sequence of elements starting at the specified index.
        /// </summary>
        /// <param name="start">Start index.</param>
        /// <param name="entrys">Elements to write.</param>
        /// <returns>
        /// True if all replacements succeeded; false if any failed.
        /// </returns>
        public bool ReplaceRange ( long start, T[] entrys ) {
            bool _ret = true;
            for ( long i = 0 ; i < entrys.Length ; i++ ) {
                if ( !Replace(start + i, entrys[i]) ) {
                    _ret = false;
                    break;
                }
            }
            return _ret;
        }

        /// <summary>
        /// Removes the last element from the Array.
        /// </summary>
        /// <returns>
        /// True if an element was removed; false if the Array was empty.
        /// </returns>
        public bool Erase () {
            if ( IsEmpty ) return false;
            m_index--;
            return true;
        }
        /// <summary>
        /// Erase overloads are API stubs and always return true.
        /// Actual removal logic is handled by Remove() methods.
        /// </summary>
        public bool Erase ( long index ) {
            return true;
        }
        /// <summary>
        /// Erase overloads are API stubs and always return true.
        /// Actual removal logic is handled by Remove() methods.
        /// </summary>
        public bool Erase ( long start, long end ) {
            return true;
        }
        /// <summary>
        /// Erase overloads are API stubs and always return true.
        /// Actual removal logic is handled by Remove() methods.
        /// </summary>
        public bool Erase ( T value ) {
            return true;
        }
        /// <summary>
        /// Swaps two elements inside the valid range.
        /// </summary>
        /// <param name="i">First index.</param>
        /// <param name="j">Second index.</param>
        public void Swap ( long i, long j ) {
            if ( i < 0 || j < 0 ) return;
            if ( i >= m_index || j >= m_index ) return;

            T tmp = m_elements[i];
            m_elements[i] = m_elements[j];
            m_elements[j] = tmp;
        }

        /// <summary>
        /// Returns the element at the specified index.
        /// </summary>
        /// <param name="index">Index to access.</param>
        /// <returns>The element at the given index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the Array is empty or the index exceeds the logical length.
        /// </exception>
        public Optional<T> ElementAt ( long index ) {
            if ( IsEmpty || index >= Length ) throw new ArgumentOutOfRangeException();

            return m_elements[index];
        }

        /// <summary>
        /// Fixed Buffer can not be Grow
        /// </summary>
        /// <returns>
        /// return always false
        /// </returns>
        public bool Grow () {
            return false;
        }

        /// <summary>
        /// Clears the Array by resetting the logical index.
        /// </summary>
        public void Clear () {
            m_index = 0;
            return;
        }


        /// <summary>
        /// Enumerates all valid elements in forward order.
        /// </summary>
        public IEnumerator<T> GetEnumerator () {
            for ( int i = 0 ; i < m_index ; i++ )
                yield return m_elements[i];
        }
        /// <summary>
        /// Get the array from the underlayes data
        /// </summary>
        /// <returns></returns>
        public T[] ToNative () => m_elements.ToArray();

        /// <summary>
        /// Create a duplicate from this
        /// </summary>
        /// <returns>The new instance from this</returns>
        public IVector<T> Duplicate () {
            return new FixedVector<T>(this);
        }
        /// <summary>
        /// Traverses a range of elements in forward or backward order.
        /// </summary>
        /// <param name="mode">Traversal direction.</param>
        /// <param name="startIndex">Start index.</param>
        /// <param name="endIndex">End index.</param>
        /// <param name="func">Action applied to each element.</param>
        public void Traverse ( TraversMode mode, long startIndex, long endIndex, Action<T> func ) {
            var start = System.Math.Max(startIndex, 0);
            var end = System.Math.Min(endIndex,  m_index);

            if ( mode == TraversMode.Forwards ) {
                for ( long i = start ; i < end ; i++ )
                    func(m_elements[i]);
            } else if ( mode == TraversMode.Backwards ) {
                for ( long i = end ; i >= start ; i-- )
                    func(m_elements[i]);
            }
        }


        /// <summary>
        /// Copies all valid elements of this Array into another Array,
        /// starting at the specified destination index.
        /// 
        /// This overload copies from the beginning of this Array (sourceOffset = 0)
        /// into the destination vector at <paramref name="VectorIndex"/>.
        /// </summary>
        /// <param name="vector">
        /// Destination vector receiving the copied elements.
        /// </param>
        /// <param name="VectorIndex">
        /// Destination offset inside <paramref name="vector"/> where copying begins.
        /// </param>
        public Pair<bool, long> CopyTo ( FixedVector<T> vector, ulong VectorIndex ) {
            return CopyTo(0, vector, 0, VectorIndex);
        }
        

        /// <summary>
        /// Copies a range of elements from this vector into another vector.
        /// Copying is performed using Buffer.LongCopy(), which supports long offsets.
        /// No automatic growth occurs in the destination; the caller must ensure
        /// sufficient capacity.
        /// </summary>
        /// <param name="sourceOffset">
        /// Offset inside this vector where copying begins.
        /// </param>
        /// <param name="destination">
        /// Destination vector receiving the copied elements.
        /// </param>
        /// <param name="destinationOffset">
        /// Offset inside <paramref name="destination"/> where copied data is written.
        /// </param>
        /// <param name="count">
        /// Maximum number of elements to copy.
        /// The actual number copied may be smaller if either vector does not have
        /// enough remaining elements.
        /// </param>
        /// <returns>
        /// The number of elements actually copied and the status as <see cref="Pair{Bool, Long}"/> 
        /// </returns>
        public Pair<bool, long> CopyTo ( uint sourceOffset, FixedVector<T> destination, ulong destinationOffset, ulong count ) {

            long src = (long)sourceOffset;
            long dst = (long)destinationOffset;

            if ( src > Length ) src = (long)Length;

            long toCopy = System.Math.Min((long)count,
            System.Math.Min(System.Math.Max(0, (long)Length - src),
                     System.Math.Max(0, destination.Length - dst)));

            if ( toCopy <= 0 ) return new Pair<bool, long>(false, 0);

            Buffer.LongCopy<T>(m_elements, src, destination.m_elements, dst, toCopy);
            return new Pair<bool, long>(true, toCopy);
        }


        /// <summary>
        /// Copies data from another vector into this vector.
        /// 
        /// If the destination range exceeds the current capacity and AutoGrow is enabled,
        /// the vector automatically expands using Resize(). After copying, the logical
        /// element count (m_index) is updated if necessary.
        /// </summary>
        /// <param name="source">
        /// Source vector providing the data.
        /// </param>
        /// <param name="sourceOffset">
        /// Offset inside <paramref name="source"/> where copying begins.
        /// </param>
        /// <param name="destinationOffset">
        /// Offset inside this vector where copied data is written.
        /// </param>
        /// <param name="count">
        /// Maximum number of elements to copy.
        /// The actual number copied may be smaller depending on available space.
        /// </param>
        /// <returns>
        /// TThe number of elements actually copied and the status as <see cref="Pair{Bool, Long}"/> 
        /// </returns>
        public Pair<bool, long> CopyFrom ( FixedVector<T> source, ulong sourceOffset, ulong destinationOffset, ulong count ) {
            long src = (long)sourceOffset;
            long dst = (long)destinationOffset;

            // Clamp source offset
            if ( src > source.Length )
                src = source.Length;

            // Clamp destination offset
            if ( dst > Length )
                dst = Length;

            // Compute how much can actually be copied
            long toCopy = System.Math.Min((long)count,
                System.Math.Min(
                    System.Math.Max(0, source.Length - src),
                    System.Math.Max(0, Length - dst)
                ));

            // Nothing fits → return (false, 0)
            if ( toCopy <= 0 )
                return new Pair<bool, long>(false, 0);

            // Perform the copy
            Buffer.LongCopy<T>(source.m_elements, src, m_elements, dst, toCopy);

            // Update logical length only if we wrote beyond current end
            long end = dst + toCopy;
            if ( end > m_index )
                m_index = end;

            return new Pair<bool, long>(true, toCopy);
        }

    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
