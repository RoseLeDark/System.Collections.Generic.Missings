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
using System.Runtime.CompilerServices;
using SystemEx;
using SystemEx.Utils;

namespace SystemEx.Threading {
    /// <summary>
    /// Represents a thread-safe index counter that can be incremented, decremented, and compared atomically.
    /// </summary>
    public struct SmallIndex : IEquatable<SmallIndex>, 
                          IEquatableEx<int>,
                            IComparable<SmallIndex>, 
                            IComparableEx<SmallIndex>,
                            IComparable<int>,
                            IComparableEx<int>,
                            IFormattable {
        private int m_value;

        /// <summary>
        /// Returns true if the index is zero.
        /// </summary>
        public bool IsZero => Volatile.Read(ref m_value) == 0;

        public int Value {
            get => Volatile.Read(ref m_value);
            set => Volatile.Write(ref m_value, value);
        }

        public SmallIndex () {
            m_value = default;
        }

        public SmallIndex ( int value ) {
            m_value = value;
        }

        /// <summary>
        /// Assigns the value of another index.
        /// </summary>
        public SmallIndex Assign ( SmallIndex other ) {
            Interlocked.Exchange(ref m_value, other.m_value);
            return this;
        }

        /// <summary>
		/// Assigns a  value.
		/// </summary>
		public SmallIndex Assign ( int value ) {
			Interlocked.Exchange(ref m_value, value);
			return this;
		}

        /// <summary>
        /// Prefix increment.
        /// </summary>
        public long Increment ()
            => Interlocked.Increment(ref m_value);

        /// <summary>
        /// Prefix decrement.
        /// </summary>
        public long Decrement ()
            => Interlocked.Decrement(ref m_value);

        /// <summary>
        /// Postfix increment.
        /// </summary>
        public long IncrementPost ()
            => Interlocked.Exchange(ref m_value, Volatile.Read(ref m_value) + 1);

        /// <summary>
        /// Postfix decrement.
        /// </summary>
        public long DecrementPost ()
            => Interlocked.Exchange(ref m_value, Volatile.Read(ref m_value) - 1);

        /// <summary>
		/// Standard override for equality comparison.
		/// </summary>
		public override bool Equals ( object? obj ) {
            if ( obj is SmallIndex other )
                return this == other;
            return false;
        }

        /// <summary>
        /// Standard override for hash code generation.
        /// </summary>
        public override int GetHashCode ()
            => Volatile.Read(ref m_value).GetHashCode();

       
        /// <inheritdoc/>
        public bool Equals ( SmallIndex other ) {
            return this == other;
        }

        /// <inheritdoc/>
        public bool Equals ( int other ) {
            return this.Value == other;
        }
        /// <inheritdoc/>
        public override string ToString () {
            return Volatile.Read(ref m_value).ToString();
        }
        
        /// <inheritdoc/>
        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return Volatile.Read(ref m_value).ToString(format, formatProvider);
        }

        /// <summary>
        /// Compares the index to another index.
        /// </summary>
        public CompareResult CompareTo(SmallIndex other) {
			return (CompareResult)Value.CompareTo(other.Value);
		}

        /// <summary>
        /// Compares the index to another index.
        /// </summary>
        int IComparable<SmallIndex>.CompareTo(SmallIndex other)
        {
            return Value.CompareTo(other.Value);
        }
        /// <summary>
        /// Compares the index to a value.
        /// </summary>
        public CompareResult CompareTo(int other) {
			return (CompareResult)Value.CompareTo(other);
		}

        /// <summary>
        /// Compares the index to a value.
        /// </summary>
        int IComparable<int>.CompareTo(int other)
        {
            return Value.CompareTo(other);
        }

        /// <summary>
        /// Converts the index to its underlying T value.
        /// </summary>
        public static implicit operator int ( SmallIndex i ) => Volatile.Read(ref i.m_value);

        /// <summary>
        /// Converts a  value to an index.
        /// </summary>
        public static implicit operator SmallIndex ( int value ) => new SmallIndex(value);
        /// <summary>
        /// Prefix decrement operator (--x).
        /// </summary>
        public static SmallIndex operator -- ( SmallIndex a ) {
            Interlocked.Decrement(ref a.m_value);
            return a;
        }

        /// <summary>
        /// Prefix increment operator (++x).
        /// </summary>
        public static SmallIndex operator ++ ( SmallIndex a ) {
            Interlocked.Increment(ref a.m_value);
            return a;
        }

       /// <summary>
        /// Adds a  value to the counter atomically.
        /// </summary>
        public static SmallIndex operator + ( SmallIndex a, int value ) {
            Interlocked.Add(ref a.m_value, value);
            return a;
        }

        /// <summary>
        /// Subtracts a  value from the counter atomically.
        /// </summary>
        public static SmallIndex operator - ( SmallIndex a, int value ) {
            Interlocked.Add(ref a.m_value, -value);
            return a;
        }
        /// <summary>
        /// Compares two counters for equality using atomic reads.
        /// </summary>
        public static bool operator == ( SmallIndex a, SmallIndex b ) {
            return Volatile.Read(ref a.m_value) == Volatile.Read(ref b.m_value);
        }

        /// <summary>
        /// Compares two counters for inequality using atomic reads.
        /// </summary>
        public static bool operator != ( SmallIndex a, SmallIndex b ) {
            return !(a == b);
        }


		public static bool operator < ( SmallIndex a, SmallIndex b ) {
			return Volatile.Read(ref a.m_value) < Volatile.Read(ref b.m_value);
		}

		public static bool operator > ( SmallIndex a, SmallIndex b ) {
			return Volatile.Read(ref a.m_value) > Volatile.Read(ref b.m_value);
		}

		public static bool operator <= ( SmallIndex a, SmallIndex b ) {
			return Volatile.Read(ref a.m_value) <= Volatile.Read(ref b.m_value);
		}

		public static bool operator >= ( SmallIndex a, SmallIndex b ) {
			return Volatile.Read(ref a.m_value) >= Volatile.Read(ref b.m_value);
		}
    }
}