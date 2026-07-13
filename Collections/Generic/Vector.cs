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
using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SystemEx.Base;
using SystemEx.Collections.Generic.Interfaces;

namespace SystemEx.Collections.Generic {


    /// \addtogroup collections
    /// @{
    /// <summary>
    /// A dynamic Vector implementation that supports optional auto-growth,
    /// indexed access, insertion, removal, traversal, and basic search operations.
    /// </summary>
    /// <typeparam name="T">The element type stored in the Vector.</typeparam>
    public struct Vector<T> : IContainerEx<T>   {
        private long m_growSize;
        private bool m_autoGrow;
        
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
        public T Front => m_elements[0];
        /// <summary>
        /// Gets the last element of the Vector.
        /// </summary>
        public T Back => m_elements[Count];

        /// <summary>
        /// Indicates whether the Vector is full.
        /// </summary>
        public bool IsFull => (AutoGrow ? false : m_index >= Length); 

        /// <summary>
        /// Indicates whether the Vector contains no elements.
        /// </summary>
        public bool IsEmpty => m_index == 0;
        /// <summary>
        /// Gets or sets the number of elements the Vector grows by when AutoGrow is enabled.
        /// </summary>
        public long GrowSize { get => (m_autoGrow ? m_growSize : 0);
            set {
                m_growSize = value;
                m_autoGrow = (m_growSize > 0);
            }
        }
        /// <summary>
        /// Enables or disables automatic resizing when the Vector becomes full.
        /// </summary>
        public  bool AutoGrow { get => (m_growSize == 0 ? false : m_autoGrow); set => m_autoGrow = value; }


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
        public T Current => m_elements[m_index];

        /// <summary>
        /// Creates a FlexSpan view over the entire vector starting at index 0.
        /// 
        /// The view uses the specified indexing mode (System, Reverse, Ring) and
        /// provides a span-like interface backed directly by this vector.
        /// </summary>
        /// <param name="vector">
        /// Reference to the vector. Passed by ref to avoid copying the struct and
        /// to ensure the FlexSpan reflects the actual container.
        /// </param>
        /// <param name="mode">
        /// Indexing mode for the view:
        /// System  = forward indexing,
        /// Reverse = backward indexing,
        /// Ring    = circular wrap-around indexing.
        /// </param>
        /// <returns>
        /// A FlexSpan representing the full vector.
        /// </returns>
        public static ContainerFlexSpan<T, Vector<T> > AsFlexSpan (ref Vector<T> vector, FlexSpanMode mode = FlexSpanMode.System )
            => new ContainerFlexSpan<T, Vector<T>>(ref vector, 0, mode);



        /// <summary>
        /// Creates a FlexSpan view over a specific range of the vector.
        /// 
        /// The view covers the range [start .. end) and uses the specified indexing mode.
        /// No memory is allocated; this is a pure logical slice backed by the vector.
        /// </summary>
        /// <param name="vector">
        /// Reference to the vector. Passed by ref so the FlexSpan operates on the
        /// actual container rather than a copy.
        /// </param>
        /// <param name="start">
        /// Starting index of the view. Must be within the vector's logical bounds.
        /// </param>
        /// <param name="end">
        /// Exclusive end index of the view. Must be greater than or equal to start
        /// and within the vector's logical bounds.
        /// </param>
        /// <param name="mode">
        /// Indexing mode for the view:
        /// System  = forward indexing,
        /// Reverse = backward indexing,
        /// Ring    = circular wrap-around indexing.
        /// </param>
        /// <returns>
        /// A FlexSpan representing the specified range of the vector.
        /// </returns>
        public static ContainerFlexSpan<T, Vector<T>> AsFlexSpan ( ref Vector<T> vector, long start, long end, FlexSpanMode mode = FlexSpanMode.System )
            => new ContainerFlexSpan<T, Vector<T>>(ref vector, start, end, mode);
     


        /// <summary>
        /// Creates a logical segment (sub-Vector) over the internal buffer.
        /// The segment shares the same underlying Vector and does not copy data.
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
        public Vector<T> AsSegment ( long start, long length ) {
            if ( start < 0 || length < 0 )
                throw new ArgumentOutOfRangeException();

            // Ensure the segment lies fully inside the internal buffer
            ArgumentOutOfRangeException.ThrowIfLessThan((start + length), m_elements.Length);

            // Create a new Vector<T> that shares the same buffer
            // A segment must not grow, otherwise it would corrupt the shared buffer
            var seg = new Vector<T>(m_elements, 0);
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
        /// Provides indexed access to the Vector elements.
        /// </summary>
        public T this[long index] {
            get => m_elements[index];
            set => Insert(index, value);
        }


        /// <summary>
        /// Creates a new Vector with a specified initial capacity.
        /// The vector starts empty (Count = 0) and grows according to GrowSize.
        /// </summary>
        /// <param name="size"> Initial capacity of the internal buffer. No elements are considered valid yet. </param>
        /// <param name="growSize"> Number of elements to add when automatic growth occurs.</param>
        public Vector ( long size, int growSize  = 16 ) {
            m_elements = new T[size];
            m_index = 0;


            GrowSize = growSize;
        }

        /// <summary>
        /// Creates a new Vector using an existing buffer.
        /// The buffer is adopted as-is, and Count is set
        /// to the last valid index. 
        /// </summary>
        /// <param name="e">
        /// Existing array used as the internal storage.
        /// </param>
        /// <param name="growSize">
        /// Number of elements to add when automatic growth occurs.
        /// </param>
        public Vector ( T[] e, int growSize = 16 ) {
            m_elements = e;
            m_index = e.LongLength;

            GrowSize = growSize;
        }

        /// <summary>
        /// Creates a new Vector from an enumerable collection.
        /// The internal buffer is created from the collection, and Count is set
        /// to the last valid index.
        /// </summary>
        /// <param name="e">
        /// Source collection used to populate the vector.
        /// </param>
        /// <param name="growSize">
        /// Number of elements to add when automatic growth occurs.
        /// </param>
        public Vector ( IEnumerable<T> e, int growSize = 16 ) {
            m_elements = e.ToArray();
            m_index = m_elements.Length - 1;

            GrowSize = growSize;
        }


        /// <summary>
        /// Copy ctor
        /// </summary>
        public Vector ( Vector<T> other )  {
            m_elements = new T[other.Length];
            Buffer.LongCopy<T>(other.m_elements, 0, m_elements, 0, other.Length);

            m_index = other.m_index;
            GrowSize = other.GrowSize;
        }
        /// <summary>
        /// Appends an element to the end of the vector.
        /// Automatically grows the buffer if AutoGrow is enabled.
        /// </summary>
        /// <param name="entry">Element to append.</param>
        /// <returns>
        /// True if the element was appended; false if the vector was full and AutoGrow was disabled.
        /// </returns>
        public bool PushBack ( T entry ) {
            if ( m_index >= Length ) {
                if ( AutoGrow ) Grow();
                return false;
            }

            m_elements[m_index] = entry;
            m_index++;
            return true;
        }
        /// <summary>
        /// Inserts an element at the specified index, shifting elements to the right.
        /// Automatically grows the buffer if needed and AutoGrow is enabled.
        /// </summary>
        /// <param name="index">Insertion index.</param>
        /// <param name="entry">Element to insert.</param>
        /// <returns>
        /// True if insertion succeeded; false if the index was invalid or growth was not allowed.
        /// </returns>
        public bool Insert ( long index, T entry ) {
            if ( index < 0 ) return false;

            // Grow wie im Indexer
            if ( index >= m_elements.Length || m_index >= m_elements.Length ) {
                if ( AutoGrow ) {
                    if ( !Resize(m_elements.Length + GrowSize) )
                        return false;
                } else {
                    return false;
                }
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
        /// Automatically grows the buffer if needed and AutoGrow is enabled.
        /// </summary>
        /// <param name="start">Start index.</param>
        /// <param name="end">End index (inclusive).</param>
        /// <param name="entry">Value to write.</param>
        /// <returns>
        /// True if the operation succeeded; false if the range was invalid or growth was not allowed.
        /// </returns>
        public bool Insert ( long start, long end, T entry ) {
            if ( start < 0 || end < start ) return false;

            // Prüfen ob wir bis end schreiben können
            if ( end >= m_elements.Length ) {
                if ( AutoGrow ) {
                    if ( !Resize(end + GrowSize) )
                        return false;
                } else {
                    return false;
                }
            }

            // Bereich füllen
            for ( long i = start ; i <= end ; i++ )
                m_elements[i] = entry;

            // m_index anpassen
            if ( end + 1 > m_index )
                m_index = end + 1;

            return true;
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
            for ( long i = 0 ; i < entrys.Length ; i++ ) {
                if ( !Insert(start + i, entrys[i]) )
                    return false;
            }
            return true;
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

            if( m_index >= m_elements.Length ) Grow();
            
            if ( index >= m_elements.Length ) {
                if ( AutoGrow ) {
                    if ( !Resize(m_elements.Length + GrowSize) )
                        return false;
                } else {
                    return false;
                }
            }

            m_elements[index] = entry;
            return true;
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
            if ( m_index >= m_elements.Length ) Grow();

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
            for ( long i = 0 ; i < entrys.Length ; i++ ) {
                if ( !Replace(start + i, entrys[i]) )
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Removes the last element from the vector.
        /// </summary>
        /// <returns>
        /// True if an element was removed; false if the vector was empty.
        /// </returns>
        public bool Erase() {
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
        /// Thrown if the vector is empty or the index exceeds the logical length.
        /// </exception>
        public T ElementAt ( long index ) {
            if ( IsEmpty || index >= Length ) throw new ArgumentOutOfRangeException();

            return m_elements[index];
        }
        /// <summary>
        /// Grows the internal buffer by GrowSize if AutoGrow is enabled.
        /// </summary>
        /// <returns>
        /// True if growth succeeded; false if AutoGrow was disabled.
        /// </returns>
        public bool Grow () {
            if ( !AutoGrow ) return false;
            return Resize(Length + GrowSize);
        }


        /// <summary>
        /// Clears the vector by resetting the logical index.
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
        /// Copies all valid elements of this vector into another vector,
        /// starting at the specified destination index.
        /// 
        /// This overload copies from the beginning of this vector (sourceOffset = 0)
        /// into the destination vector at <paramref name="VectorIndex"/>.
        /// </summary>
        /// <param name="vector">
        /// Destination vector receiving the copied elements.
        /// </param>
        /// <param name="VectorIndex">
        /// Destination offset inside <paramref name="vector"/> where copying begins.
        /// </param>
        public Pair<bool, long> CopyTo ( Vector<T> vector, ulong VectorIndex ) {
            return CopyTo(0, vector, 0, VectorIndex);
        }


        /// <summary>
        /// Copies a range of elements from this vector into another vector.
        /// 
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
        /// The number of elements actually copied and the status.
        /// </returns>
        public Pair<bool, long> CopyTo ( uint sourceOffset, Vector<T> destination, ulong destinationOffset, ulong count ) {

            long src = (long)sourceOffset;
            long dst = (long)destinationOffset;

            if( src > Length ) src = (long)Length;

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
        /// The number of elements actually copied and the status
        /// </returns>
        public Pair<bool, long> CopyFrom ( Vector<T> source, ulong sourceOffset, ulong destinationOffset, ulong count ) {
            long src = (long)sourceOffset;
            long dst = (long)destinationOffset;

            if ( dst > Length )
                dst = Length;

            long toCopy = System.Math.Min((long)count,  
                System.Math.Min(System.Math.Max(0, source.Length - src),
                System.Math.Max(0, Length - dst)));

            // Wenn nichts passt → prüfen ob wir wachsen müssen
            if ( toCopy <= 0 ) {
                if ( !AutoGrow )
                    return new Pair<bool, long>(false, 0);

                long required = dst + (long)count;

                long newSize = Length;
                while ( required > newSize )
                    newSize += GrowSize;

                if ( !Resize(newSize) )
                    return new Pair<bool, long>(false, 0);

                // Nach Resize neu berechnen
                toCopy = System.Math.Min((long)count,
                    System.Math.Min(System.Math.Max(0, source.Length - src),
                        System.Math.Max(0, (long)Length - dst)));

                if ( toCopy <= 0 )
                    return new Pair<bool, long>(false, 0);
            }

            Buffer.LongCopy<T>(source.m_elements, src, m_elements, dst, toCopy);

            // m_index anpassen, falls wir weiter hinten geschrieben haben
            long end = dst + toCopy;
            if ( end > m_index )
                m_index = end;

            return new Pair<bool, long>(true, toCopy); 
        }

        /// <summary>
        /// Returns a copy of the internal buffer.
        /// </summary>
        public T[] ToNative () => m_elements.ToArray();

        /// <summary>
        /// Create a duplicate from this
        /// </summary>
        /// <returns>The new instance from this</returns>
        public IContainerEx<T> Duplicate () {
            return new Vector<T>(this);
        }

        /// <summary>
        /// Resizes the internal buffer to the specified size.
        /// Adjusts the logical index if it exceeds the new size.
        /// </summary>
        /// <param name="size">New buffer size.</param>
        /// <returns>
        /// True if resizing succeeded; false if resizing was unnecessary or failed.
        /// </returns>
        private bool Resize ( long size ) {
            if ( size == Length ) return false;
            if ( m_index > size )
                m_index = size;

            try {
                Array.Resize(ref m_elements, (int)size);
            } catch {
                return false;
            }
            return true;
        }

        
    }

#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
