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

namespace SystemEx.Utils {

	/// <summary>
	/// Provides a low-level bit window over a referenced <see cref="int"/> value.
	/// This type exposes individual bits as a mutable span-like view with support
	/// for forward, reverse, and ring-based indexing modes.
	/// 
	/// <para>
	/// <b>Warning:</b> This API is intended for advanced developers only.
	/// It operates directly on a referenced integer and mutates its bits without
	/// safety guards, bounds normalization, or copy-on-write semantics.
	/// Incorrect usage may lead to unexpected side effects, corrupted state,
	/// or infinite iteration when using ring mode.
	/// </para>
	/// </summary>
	/// <remarks>
	/// <para>
	/// <b>ref struct:</b> The type is stack-only and cannot be boxed, stored on the heap,
	/// captured by lambdas, or used in async methods.
	/// </para>
	/// <para>
	/// <b>Direct bit mutation:</b> All operations modify the underlying integer in-place.
	/// Multiple spans referencing the same integer will observe each other's changes.
	/// </para>
	/// <para>
	/// <b>Windowed view:</b> The span exposes only the bits between <c>Start</c> and <c>End</c>.
	/// Access outside this range throws exceptions.
	/// </para>
	/// <para>
	/// <b>Indexing modes:</b>
	/// <list type="bullet">
	/// <item><description><b>System</b>: forward indexing (Start → End).</description></item>
	/// <item><description><b>Reverse</b>: backward indexing (End-1 → Start).</description></item>
	/// <item><description><b>Ring</b>: cyclic indexing; enumeration never terminates.</description></item>
	/// </list>
	/// </para>
	/// </remarks>
	public ref struct BitIntSpan {
        /// <summary>
        /// Enumerates the bits of a <see cref="BitIntSpan"/> according to its indexing mode.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Warning:</b> When the span is in <see cref="FlexSpanMode.Ring"/>,
        /// enumeration is infinite. Consumers must implement their own termination
        /// conditions to avoid non-returning loops.
        /// </para>
        /// <para>
        /// The enumerator yields live bit values. Mutating the underlying integer
        /// during enumeration will affect subsequent results.
        /// </para>
        /// </remarks>
        public ref struct Enumerator : IEnumerator<bool> {
            /// <summary>
            /// The span being enumerated.
            /// </summary>
            private BitIntSpan m_span;

            /// <summary>
            /// The next index to yield.
            /// </summary>
            private sbyte m_index;

            /// <summary>
            /// Indicates whether another element can be produced.
            /// Ring mode always returns true (infinite iteration).
            /// </summary>
            public bool HasNext {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get {
                    bool _ret = false;
                    if ( m_span.GetMode() == FlexSpanMode.Ring )
                        _ret = true;
                    else
                        _ret = (m_index + 1) < (m_span.Bits);
                    return _ret;
                }
            }

            /// <summary>
            /// Initializes the enumerator.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Enumerator ( BitIntSpan span ) {
                m_span = span;
                m_index = -1;
            }


            /// <summary>
            /// Moves to the next element in the span.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            /// Gets the element at the current enumerator position.
            /// </summary>
            public bool Current {
                get => m_span[(byte)m_index];
                set => m_span[(byte)m_index] = value;
            }

            /// <inheritdoc />
            bool IEnumerator<bool>.Current => Current;

            /// <inheritdoc />
            object IEnumerator.Current => Current!;

            /// <summary>
            /// Resets the enumerator to the initial position.
            /// Ring mode resets to index 0; other modes reset to -1.
            /// </summary>
            void IEnumerator.Reset () => m_index = (sbyte)(m_span.GetMode() == FlexSpanMode.Ring ? 0 : m_index = -1);

            /// <summary>
            /// No resources to dispose.
            /// </summary>
            void IDisposable.Dispose () { }
        }
        /// <summary>
        /// The unerlay value
        /// </summary>
        private ref int m_value;

        /// <summary>
        /// Start index of the view.
        /// </summary>
        private short m_lStart;

        /// <summary>
        /// End index of the view (exclusive).
        /// </summary>
        private short m_lEnd;


        /// <summary>
        /// Length of the view (End - Start).
        /// </summary>
        public short ViewLength => (short)(m_lEnd - m_lStart);

		/// <summary>
		/// Gets the total number of bits in a <see cref="int"/> value.
		/// </summary>
		public short Bits => (short)(sizeof(int) * 8);

		/// <summary>
		/// Indicates whether the underlying integer is treated as a signed value.
		/// </summary>
		public bool IsSigned => true;

		/// <summary>
		/// Indicates whether the underlying integer is treated as an unsigned value.
		/// </summary>
		public bool IsUnsigned => false;

        private FlexSpanMode m_mode;

		/// <summary>
		/// Returns an enumerator that iterates over the bit span according to the
		/// configured indexing mode. In ring mode, enumeration never terminates.
		/// </summary>
		public Enumerator GetEnumerator () => new Enumerator(this);

        /// <summary>
        /// Initializes a new <see cref="BitIntSpan"/> that exposes a bit-level view
        /// over the referenced <paramref name="value"/>.
        /// </summary>
        /// <param name="value">
        /// A reference to the underlying <see cref="int"/> whose bits will be read
        /// and modified directly. No copy is made.
        /// </param>
        /// <param name="startPos">
        /// The starting bit position of the view (inclusive). Must be within the
        /// valid bit range of the underlying integer.
        /// </param>
        /// <param name="endPos">
        /// The ending bit position of the view (exclusive). Must be greater than
        /// <paramref name="startPos"/> and within the integer's bit width.
        /// </param>
        /// <param name="mode">
        /// The indexing mode used to interpret bit positions:
        /// <see cref="FlexSpanMode.System"/>, <see cref="FlexSpanMode.Reverse"/>,
        /// or <see cref="FlexSpanMode.Ring"/>.
        /// </param>
        /// <remarks>
        /// <para>
        /// <b>Warning:</b> This constructor creates a span-like structure that operates
        /// directly on a referenced integer. All bit reads and writes mutate the original
        /// value in-place. Multiple spans referencing the same integer will observe and
        /// affect each other's changes.
        /// </para>
        /// <para>
        /// Because <see cref="BitIntSpan"/> is a <c>ref struct</c>, it is restricted to
        /// stack-only usage. It cannot be boxed, stored on the heap, used in async methods,
        /// or captured by closures.
        /// </para>
        /// <para>
        /// Incorrect window boundaries or misuse of <see cref="FlexSpanMode.Ring"/> may
        /// lead to out-of-range exceptions or infinite iteration. This API is intended
        /// for advanced developers who fully understand bit-level operations.
        /// </para>
        /// </remarks>
        public BitIntSpan ( ref int value, short startPos, short endPos, FlexSpanMode mode ) {
            m_value = ref value;
            m_mode = mode;
            m_lEnd = endPos;
            m_lStart = startPos;
        }
		/// <summary>
		/// Reads the bit at the specified absolute position within the underlying
		/// integer. Access outside the valid bit range throws an exception.
		/// </summary>
		/// <param name="pos">The absolute bit position.</param>
		/// <returns><c>true</c> if the bit is set; otherwise <c>false</c>.</returns>
		private bool GetAt ( int pos ) {
            if ( pos >= Bits ) throw new ArgumentOutOfRangeException("pos");
            return ((m_value >> pos) & 1) != 0;
        }
        /// <summary>
        /// Retrieves the bit at the specified logical index within the span's view.
        /// </summary>
        /// <param name="index">The zero-based index inside the view window.</param>
        /// <returns>
        /// <c>true</c> if the bit is set; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="index"/> exceeds the view length.
        /// </exception>
        /// <remarks>
        /// <para>
        /// The returned bit depends on the active <see cref="FlexSpanMode"/>:
        /// <list type="bullet">
        /// <item><description><b>System</b>: direct forward mapping.</description></item>
        /// <item><description><b>Reverse</b>: mirrored mapping from the end.</description></item>
        /// <item><description><b>Ring</b>: index wraps modulo the view length.</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <b>Warning:</b> This method reads directly from the referenced integer.
        /// Concurrent modifications will be reflected immediately.
        /// </para>
        /// </remarks>
        public bool ElementAt ( byte index ) {

            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, ViewLength);


            switch ( m_mode ) {
            case FlexSpanMode.System:
            return GetAt(m_lStart + index);

            case FlexSpanMode.Reverse:
            return GetAt(m_lEnd - (1 + index));


            case FlexSpanMode.Ring: {
                    int pos = m_lStart + (index % ViewLength);

                    return GetAt(pos);
                }
            default:
            throw new InvalidOperationException();
            }
        }
		/// <summary>
		/// Set a bit at the specified logical index within the span's view.
		/// </summary>
		/// <param name="pos">The zero-based index inside the view window.</param>
		/// <param name="value">The bit to set</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when <paramref name="pos"/> exceeds the view length.
		/// </exception>
		private void SetAt ( int pos, bool value ) {
            if ( pos >= Bits ) throw new ArgumentOutOfRangeException("pos");

            byte current = (byte)((m_value  >> pos) & 1U) ;

            m_value = ((value) ? (m_value | 1 << pos) : (m_value & ~(1 << pos)));
        }
        /// <summary>
        /// Sets the bit at the specified logical index within the span's view.
        /// </summary>
        /// <param name="index">The zero-based index inside the view window.</param>
        /// <param name="value">The bit value to assign.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="index"/> exceeds the view length.
        /// </exception>
        /// <remarks>
        /// <para>
        /// <b>Warning:</b> This method mutates the underlying integer directly.
        /// No copy is made. All other spans referencing the same integer will
        /// observe the change immediately.
        /// </para>
        /// <para>
        /// The bit position is resolved according to the active indexing mode.
        /// </para>
        /// </remarks>
        public void ElementAt ( byte index, bool value ) {

            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, ViewLength);


            switch ( m_mode ) {
            case FlexSpanMode.System:
                SetAt( m_lStart + index , value);
                return;
            case FlexSpanMode.Reverse:
                SetAt(m_lEnd - (1 + index), value);
                return;
            case FlexSpanMode.Ring: 
                int pos = m_lStart + (index % ViewLength);
                SetAt(pos, value);
                return;
            default:
            throw new InvalidOperationException();
            }
        }

		/// <summary>
		/// Gets the indexing mode used by this bit span.
		/// </summary>
		public FlexSpanMode GetMode () {
            return m_mode;
        }
		/// <summary>
		/// Gets or sets the bit at the specified window-relative index.
		/// This is equivalent to calling <see cref="ElementAt(byte)"/>.
		/// </summary>
		public bool this[byte index] {
            get => ElementAt(index);
            set => ElementAt(index, value);
        }

		/// <summary>
		/// Returns an empty instance.
		/// </summary>
		public static BitIntSpan Empty => default;

		/// <summary>
		/// Creates a new bit span beginning at the specified offset within the
		/// current window. The new span references the same underlying integer.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitIntSpan Slice ( short start, FlexSpanMode? mode = null )
            => new BitIntSpan(ref m_value, (short)(m_lStart + start), (short)(m_lEnd - start), mode ?? m_mode);

		/// <summary>
		/// Creates a new bit span beginning at the specified offset within the
		/// current window. The new span references the same underlying integer.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitIntSpan Slice ( short start, short length, FlexSpanMode? mode = null )
            => new BitIntSpan(ref m_value, (short)(m_lStart + start), length > Bits ? Bits : length, mode ?? m_mode);

		/// <summary>
		/// Determines whether two bit spans reference the same underlying integer
		/// and expose the same window and indexing mode.
		/// </summary>
		public static bool operator == ( BitIntSpan left, BitIntSpan right ) =>
            left.m_value == right.m_value &&
            left.m_mode == right.m_mode &&
            left.m_lStart == right.m_lStart &&
            left.m_lEnd == right.m_lEnd;

		/// <summary>
		/// Determines whether two bit spans differ in referenced value, window
		/// boundaries, or indexing mode.
		/// </summary>
		public static bool operator != ( BitIntSpan left, BitIntSpan right ) =>
            !(left == right);

#pragma warning disable CS0809 // Veraltetes Element überschreibt nicht veraltetes Element
		[Obsolete("Equals() always throw an exception. Use the equality operator instead.")]
        public override bool Equals ( object? obj ) => throw new NotSupportedException();

        [Obsolete("GetHashCode() always throw an exception.")]
		public override int GetHashCode () => throw new NotSupportedException();
#pragma warning restore CS0809 // Veraltetes Element überschreibt nicht veraltetes Element
	}
}
