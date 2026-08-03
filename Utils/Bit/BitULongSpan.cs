using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace SystemEx.Utils {
    public ref struct BitULongSpan {
        public ref struct Enumerator : IEnumerator<bool> {
            /// <summary>
            /// The span being enumerated.
            /// </summary>
            private BitULongSpan m_span;

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
            internal Enumerator ( BitULongSpan span ) {
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

        private ref ulong m_value;

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

        public short Bits => (short)(sizeof(ulong) * 8);
        public bool IsSigned => true;
        public bool IsUnsigned => false;

        private FlexSpanMode m_mode;

        public Enumerator GetEnumerator () => new Enumerator(this);

        public BitULongSpan ( ref ulong value, short startPos, short endPos, FlexSpanMode mode ) {
            m_value = ref value;
            m_mode = mode;
            m_lEnd = endPos;
            m_lStart = startPos;
        }

        private bool GetAt ( int pos ) {
            if ( pos >= Bits ) throw new ArgumentOutOfRangeException("pos");
            return ((m_value >> pos) & 1) != 0;
        }

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

        private void SetAt ( int pos, bool value ) {
            if ( pos >= Bits ) throw new ArgumentOutOfRangeException("pos");

            byte current = (byte)((m_value  >> pos) & 1U) ;

            m_value = ((value) ? (m_value | 1U << pos) : (m_value & ~(1U << pos)));
        }

        public void ElementAt ( byte index, bool value ) {

            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, ViewLength);


            switch ( m_mode ) {
            case FlexSpanMode.System:
            SetAt(m_lStart + index, value);
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

        public FlexSpanMode GetMode () {
            return m_mode;
        }

        public bool this[byte index] {
            get => ElementAt(index);
            set => ElementAt(index, value);
        }

        /// <summary>
        /// Returns an empty FlexSpan.
        /// </summary>
        public static BitIntSpan Empty => default;


        /// <summary>
        /// Creates a new FlexSpan starting at the given offset.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitULongSpan Slice ( short start, FlexSpanMode? mode = null )
            => new BitULongSpan(ref m_value, (short)(m_lStart + start), (short)(m_lEnd - start), mode ?? m_mode);

        /// <summary>
        /// Creates a new FlexSpan with a given offset and length.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BitULongSpan Slice ( short start, short length, FlexSpanMode? mode = null )
            => new BitULongSpan(ref m_value, (short)(m_lStart + start), length > Bits ? Bits : length, mode ?? m_mode);

        /// <summary>
        /// Structural equality comparison.
        /// </summary>
        public static bool operator == ( BitULongSpan left, BitULongSpan right ) =>
            left.m_value == right.m_value &&
            left.m_mode == right.m_mode &&
            left.m_lStart == right.m_lStart &&
            left.m_lEnd == right.m_lEnd;

        /// <summary>
        /// 
        /// </summary>
        public static bool operator != ( BitULongSpan left, BitULongSpan right ) =>
            !(left == right);

        [Obsolete("Equals() always throw an exception. Use the equality operator instead.")]
        public override bool Equals ( object? obj ) => throw new NotSupportedException();

        [Obsolete("GetHashCode() always throw an exception.")]
        public override int GetHashCode () => throw new NotSupportedException();
    }
}
