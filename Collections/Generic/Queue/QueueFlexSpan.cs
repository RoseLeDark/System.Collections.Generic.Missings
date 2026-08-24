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

namespace SystemEx.Collections.Generic {
	/// \addtogroup SystemEx.Collections.Generic 
	/// @{

	public ref struct QueueFlexSpan<T> {
        public ref struct Enumerator : IEnumerator<T> {
            /// <summary>
            /// The span being enumerated.
            /// Stored by value; underlying container is referenced via ref.
            /// </summary>
            private readonly QueueFlexSpan<T> m_span;

            /// <summary>
            /// The next index to yield.
            /// </summary>
            private int m_index;

            /// <summary>
            /// Indicates whether another element can be produced.
            /// Ring mode always returns true (infinite wrap-around).
            /// Other modes stop at ViewLength.
            /// </summary>
            public bool HasNext {
                get {
                    bool _ret = false;
                    if ( m_span.m_eMode == FlexSpanMode.Ring )
                        _ret = true;
                    else
                        _ret = (m_index + 1) < (m_span.m_lEnd - m_span.m_lStart);
                    return _ret;
                }
            }

            /// <summary>
            /// Initializes the enumerator.
            /// </summary>
            internal Enumerator ( QueueFlexSpan<T> span ) {
                m_span = span;
                m_index = -1;
            }


            /// <summary>
            /// Moves to the next element in the span.
            /// </summary>
            public bool MoveNext () {
                bool _ret = false;

                if ( HasNext ) {
                    m_index++;
                    if ( m_index < 0 ) m_index = 0;
                    _ret = true;
                }
                return _ret;
            }

            /// <summary>
            /// Gets or sets the current element.
            /// SetAt() uses Replace() on the underlying container.
            /// </summary>
            public T Current {
                get => m_span.ElementAt(m_index)!;
                set => m_span.SetAt(m_index, value);
            }

            /// <inheritdoc />
            T IEnumerator<T>.Current => Current;

            /// <inheritdoc />
            object IEnumerator.Current => Current!;

            /// <summary>
            /// Resets the enumerator.
            /// Ring mode resets to index 0.
            /// Other modes reset to -1.
            /// </summary>
            void IEnumerator.Reset () => m_index = m_span.m_eMode == FlexSpanMode.Ring ? 0 : m_index = -1;

            /// <summary>
            /// No resources to dispose.
            /// </summary>
            void IDisposable.Dispose () { }
        }
        /// <summary>
        /// Reference to the underlying container.
        /// ref ensures no container copy occurs.
        /// </summary>
        private readonly ref Queue<T> m_pReference;
        /// <summary>
        /// Total length of the underlying array.
        /// </summary>
        private readonly long m_llength;

        /// <summary>
        /// Indexing mode (System, Reverse, Ring).
        /// </summary>
        private FlexSpanMode m_eMode;

        /// <summary>
        /// Start index of the view.
        /// </summary>
        private long m_lStart;

        /// <summary>
        /// End index of the view (exclusive).
        /// </summary>
        private long m_lEnd;

        /// <summary>
        /// True if the span has zero length.
        /// </summary>
        public bool IsEmpty => m_llength == 0;

        /// <summary>
        /// Total length of the underlying array.
        /// </summary>
        public long Length => m_llength;

        /// <summary>
        /// Length of the view (End - Start).
        /// </summary>
        public long ViewLength => m_lEnd - m_lStart;

        /// <summary>
        /// Start index of the view.
        /// </summary>
        public long Start => m_lStart;

        /// <summary>
        /// End index of the view (exclusive).
        /// </summary>
        public long End => m_lEnd;

        /// <summary>
        /// A Empty Object
        /// </summary>
        public static QueueFlexSpan<T> Empty => default;

        /// <summary>
        /// Creates a FlexSpan view starting at <paramref name="start"/> and extending
        /// to the end of the underlying container.
        /// 
        /// This constructor does not allocate memory. It simply creates a logical view
        /// over the container using the specified indexing mode.
        /// </summary>
        /// <param name="pqueue">
        /// Reference to the underlying container. Passed by ref to avoid copying the
        /// container struct and to ensure the view always reflects the actual container.
        /// </param>
        /// <param name="start">
        /// The starting index of the view. Must be within the bounds of the container.
        /// </param>
        /// <param name="mode">
        /// Indexing mode used by the view:
        /// System  = forward indexing,
        /// Reverse = backward indexing,
        /// Ring    = circular wrap-around indexing.
        /// </param>
        public QueueFlexSpan ( ref Queue<T> pqueue, long start, FlexSpanMode mode ) {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(start, pqueue.Size);

            m_pReference = ref pqueue;
            m_llength = pqueue.Size;
            m_eMode = mode;

            // View 
            m_lStart = start;
            m_lEnd = m_llength;
        }
        /// <summary>
        /// Creates a FlexSpan view with explicit start and length.
        /// 
        /// The view covers the range [start .. start + length), using the specified
        /// indexing mode. No memory is allocated; this is a pure logical slice.
        /// </summary>
        /// <param name="queue">
        /// Reference to the underlying container. Passed by ref so the FlexSpan
        /// reflects the actual container rather than a copy.
        /// </param>
        /// <param name="start">
        /// Starting index of the view. Must be within the container bounds.
        /// </param>
        /// <param name="length">
        /// Number of elements in the view. Must not exceed the remaining container
        /// length starting at <paramref name="start"/>.
        /// </param>
        /// <param name="mode">
        /// Indexing mode used by the view:
        /// System  = forward indexing,
        /// Reverse = backward indexing,
        /// Ring    = circular wrap-around indexing.
        /// </param>
        public QueueFlexSpan ( ref Queue<T> queue, long start, long length, FlexSpanMode mode ) {

            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((ulong)length, (ulong)(queue.Size - start));
            ArgumentOutOfRangeException.ThrowIfGreaterThan((ulong)start, (ulong)queue.Size);

            m_pReference = ref queue;
            m_llength = queue.Size;
            m_eMode = mode;

            // View 
            m_lStart = start;
            m_lEnd = start + length;

        }


        /// <summary>
        /// Copies the contents of this FlexSpan's view into another FlexSpan.
        /// 
        /// Both spans must have compatible lengths. Copying is performed element-by-element
        /// using the underlying container's Replace() method.
        /// </summary>
        /// <param name="destination">
        /// The target FlexSpan receiving the copied elements. Must have a ViewLength
        /// greater than or equal to this span's ViewLength.
        /// </param>
        public void CopyTo ( QueueFlexSpan<T> destination ) {
            if ( destination.ViewLength < this.ViewLength )
                throw new ArgumentException("Destination FlexSpan is too small.");


            for ( long i = 0 ; i < ViewLength ; i++ )
                destination.SetAt(i, this.ElementAt(i));
        }


        /// <summary>
        /// Returns the element at the given index according to the current indexing mode.
        /// 
        /// System:  Start + index  
        /// Reverse: End - (1 + index)  
        /// Ring:    Start + (index % ViewLength)
        /// </summary>
        /// <param name="index">
        /// Index inside the view (0-based). Must be less than ViewLength.
        /// </param>
        /// <returns>
        /// The element mapped to the underlying container according to the active mode.
        /// </returns>

        public T? ElementAt ( long index ) {

            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, ViewLength);

            long pos = 0;

            switch ( m_eMode ) {
            case FlexSpanMode.System:
            pos = m_lStart + index;
            break;

            case FlexSpanMode.Reverse:
            pos = m_lEnd - (1 + index);
            break;

            case FlexSpanMode.Ring:
            pos = m_lStart + (index % ViewLength);
            break;
            default:
            throw new InvalidOperationException();
            }

            return m_pReference.ElementAt(pos).Value;
        }


        /// <summary>
        /// Writes a value into the underlying container at the mapped position.
        /// 
        /// The mapping depends on the current indexing mode:
        /// System:  Start + index  
        /// Reverse: End - (1 + index)  
        /// Ring:    Start + (index % ViewLength)
        /// 
        /// Replace() is used to perform the write operation. No length extension occurs.
        /// </summary>
        /// <param name="index">
        /// Index inside the view (0-based). Must be less than ViewLength.
        /// </param>
        /// <param name="value">
        /// The value to write. Must not be null.
        /// </param>
        /// <returns>
        /// True if the underlying container accepted the write operation.
        /// </returns>
        public bool SetAt ( long index, T? value ) {
            if ( value == null ) return false;

            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, ViewLength);

            long pos = 0;

            switch ( m_eMode ) {
            case FlexSpanMode.System:
            pos = m_lStart + index;
            break;

            case FlexSpanMode.Reverse:
            pos = m_lEnd - (1 + index);
            break;

            case FlexSpanMode.Ring:
            pos = m_lStart + (index % ViewLength);
            break;
            default:
            throw new InvalidOperationException();
            }

            return m_pReference.Replace(pos, value);
        }
    }
    // @}
}

