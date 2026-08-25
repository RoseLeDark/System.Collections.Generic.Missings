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
using System.Runtime.CompilerServices;
using SystemEx.Collections.Generic;

namespace SystemEx {
	/// \addtogroup SystemEx
	/// @{

	/// <summary>
	/// Defines how FlexSpan indexes its underlying array.
	/// </summary>
	public enum FlexSpanMode {
        /// <summary> circular indexing (wrap-around)</summary>
        Ring,
        /// <summary> reverse indexing </summary>
        Reverse,
        /// <summary> forward indexing</summary>
        System
    }

    /// <summary>
    /// A lightweight view over an array supporting System, Reverse, and Ring (circular) indexing.
    /// FlexSpan does not allocate and provides ref-return access to elements.
    /// </summary>
    public ref struct FlexSpan<T>  {

        /// <summary>
        /// Enumerator for FlexSpan. Supports forward, reverse, and ring traversal.
        /// </summary>
        public ref struct Enumerator : IEnumerator<T> {
            /// <summary>
            /// The span being enumerated.
            /// </summary>
            private readonly FlexSpan<T> m_span;

            /// <summary>
            /// The next index to yield.
            /// </summary>
            private int m_index;

            /// <summary>
            /// Indicates whether another element can be produced.
            /// Ring mode always returns true (infinite iteration).
            /// </summary>
            public bool HasNext {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Enumerator ( FlexSpan<T> span ) {
                m_span = span;
                m_index = -1;
            }


            /// <summary>
            /// Moves to the next element in the span.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext () {
                bool _ret = false;

                if( HasNext ) {
                    m_index++;
                    if ( m_index < 0 ) m_index = 0;
                    _ret = true;
                }
                return _ret;
            }

            /// <summary>
            /// Gets the element at the current enumerator position.
            /// </summary>
            public ref T Current {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref m_span[m_index];
            }

            /// <inheritdoc />
            T IEnumerator<T>.Current => Current;

            /// <inheritdoc />
            object IEnumerator.Current => Current!;

            /// <summary>
            /// Resets the enumerator to the initial position.
            /// Ring mode resets to index 0; other modes reset to -1.
            /// </summary>
            void IEnumerator.Reset () => m_index = m_span.m_eMode == FlexSpanMode.Ring ? 0 : m_index = -1;

            /// <summary>
            /// No resources to dispose.
            /// </summary>
            void IDisposable.Dispose () { }
        }

        /// <summary>
        /// Reference to the underlying array.
        /// </summary>
        private readonly ref T[] m_pReference;

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
        /// Provides ref-access to the element at the given index within the view.
        /// </summary>
        public ref T this[int index] {
            get => ref ElementAt(index);
        }

        /// <summary>
        /// Creates a FlexSpan over an array starting at a given index.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FlexSpan ( ref T[] reference, long start, FlexSpanMode mode ) {
            if ( reference == null ) throw new NullReferenceException();
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(start, reference.Length);

            m_pReference = ref reference;
            m_llength = reference.Length;
            m_eMode = mode;

            // View 
            m_lStart = start;
            m_lEnd = m_llength;
        }

        /// <summary>
        /// Creates a FlexSpan over an array with a defined view length.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FlexSpan ( ref T[] array, long start, long length, FlexSpanMode mode ) {
            if ( array == null ) throw new NullReferenceException();

            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((ulong)length, (ulong)(array.Length - start));
            ArgumentOutOfRangeException.ThrowIfGreaterThan((ulong)start, (ulong)array.Length);

            m_pReference = ref array;
            m_llength = array.Length;
            m_eMode = mode;

            // View 
            m_lStart = start;
            m_lEnd = start + length;

        }
        /// <summary>
        /// Returns an empty FlexSpan.
        /// </summary>
        public static FlexSpan<T> Empty => default;



        /// <summary>
        /// Creates a new FlexSpan starting at the given offset.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FlexSpan<T> Slice ( int start, FlexSpanMode? mode = null )
            => new FlexSpan<T>(ref m_pReference, m_lStart + start, m_llength - start, mode ?? m_eMode);

        /// <summary>
        /// Creates a new FlexSpan with a given offset and length.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FlexSpan<T> Slice ( int start, int length, FlexSpanMode? mode = null )
            => new FlexSpan<T>(ref m_pReference, m_lStart + start, length, mode ?? m_eMode);

        /// <summary>
        /// Structural equality comparison.
        /// </summary>
        public static bool operator == ( FlexSpan<T> left, FlexSpan<T> right ) =>
            left.m_llength == right.m_llength &&
            left.m_pReference.Equals(right.m_pReference) &&
            left.m_eMode == right.m_eMode &&
            left.m_lStart == right.m_lStart &&
            left.m_lEnd == right.m_lEnd;

        /// <summary>
        /// Structural not equality comparison.
        /// </summary>
        public static bool operator != ( FlexSpan<T> left, FlexSpan<T> right ) => !(left == right);

        [Obsolete("Equals() on FlexSpan will always throw an exception. Use the equality operator instead.")]
        public override bool Equals ( object obj ) => throw new NotSupportedException();

        [Obsolete("GetHashCode() on FlexSpan will always throw an exception.")]
        public override int GetHashCode () => throw new NotSupportedException();

        /// <summary>
        /// Returns an enumerator for the span.
        /// </summary>
        public Enumerator GetEnumerator () => new Enumerator(this);


        /// <summary>
        /// Copies the view into a new Array{T}.
        /// </summary>
        public Vector<T> ToArray () {
            if ( m_llength == 0 )
                return new Vector<T>();

            var destination = new Vector<T>( (int)(m_lEnd - m_lStart) );

            for ( long i = m_lStart, j = 0 ; i < m_lEnd ; i++, j++ ) {
                T _e = ElementAt(j);
                destination[(int)j] = _e;
#if DEBUG
                Console.WriteLine("ToArray, i = {0} j = {1} {2} [{3} ... {4}]", i, j, _e , m_lStart, m_lEnd);
#endif
            }

            return destination;
        }
        /// <summary>
        /// Copies this FlexSpan's view into another FlexSpan.
        /// </summary>
        public void CopyTo ( FlexSpan<T> destination ) {
            if ( destination.ViewLength < this.ViewLength )
                throw new ArgumentException("Destination FlexSpan is too small.");


            for ( long i = 0 ; i < ViewLength ; i++ )
                destination.ElementAt(i) = this.ElementAt(i);
        }
        /// <summary>
        /// Attempts to copy the view into another FlexSpan.
        /// </summary>
        public bool TryCopyTo ( FlexSpan<T> destination ) {
            bool _ret = false;
            try {
                CopyTo(destination);

                _ret = true;
            } catch(Exception ) {
                _ret = false;
            }
            return _ret;
        }
        /// <summary>
        /// Fills the view with a given value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Fill ( T value ) {

            for ( long i = 0 ; i < ViewLength ; i++ )
                ElementAt(i) = value;

        }

        /// <summary>
        /// Returns a reference to the element at the given index according to the current mode.
        /// System   = forward indexing
        /// Reverse  = reverse indexing
        /// Ring     = circular indexing (wrap-around)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T ElementAt ( long index ) {

            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, ViewLength);


            switch ( m_eMode ) {
                case FlexSpanMode.System:
                    return ref m_pReference[m_lStart + index];

                case FlexSpanMode.Reverse:
                    return ref m_pReference[m_lEnd - (1 + index)];


                case FlexSpanMode.Ring: {
                    long pos = m_lStart + (index % ViewLength);

                    return ref m_pReference[pos];
                }
            default:
                    throw new InvalidOperationException();
            }
        }
    }
	//@}
}
