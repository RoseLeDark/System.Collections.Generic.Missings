using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Threading {

    /// <summary>
    /// Provides a lightweight atomic counter with increment, decrement and assignment
    /// </summary>
    public class SafeCounter : IEquatable<SafeCounter> {
        private long m_value;
        private long m_startValue;

        /// <summary>
        /// Gets the current value.
        /// </summary>
        public long Value => Volatile.Read(ref m_value);

        /// <summary>
        /// Returns true if the counter is zero.
        /// </summary>
        public bool IsZero => Volatile.Read(ref m_value) == 0;

        /// <summary>
        /// Initializes the counter to zero.
        /// </summary>
        public SafeCounter () {
            m_value = 0L;

		}

        /// <summary>
        /// Initializes the counter with a specific value.
        /// </summary>
        public SafeCounter ( long value ) {
            m_value = value;
            m_startValue = value;
		}

        /// <summary>
        /// Initializes the counter from another counter.
        /// </summary>
        public SafeCounter ( SafeCounter other ) {
            m_value = other.Value;
            m_startValue = other.m_startValue;

		}

        /// <summary>
        /// Assigns the value of another counter.
        /// </summary>
        public SafeCounter Assign ( SafeCounter other ) {
            Interlocked.Exchange(ref m_value, other.m_value);
            return this;
        }

        /// <summary>
        /// Converts the counter to its underlying long value.
        /// </summary>
        public static implicit operator long ( SafeCounter c ) => Volatile.Read(ref c.m_value);


		/// <summary>
		/// Assigns a raw long value.
		/// </summary>
		public SafeCounter Assign ( long value ) {
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
            => Interlocked.Exchange(ref m_value, m_value + 1);

        /// <summary>
        /// Postfix decrement.
        /// </summary>
        public long DecrementPost ()
            => Interlocked.Exchange(ref m_value, m_value - 1);


        /// <summary>
        /// Prefix decrement operator (--x).
        /// </summary>
        public static SafeCounter operator -- ( SafeCounter a ) {
            Interlocked.Decrement(ref a.m_value);
            return a;
        }

        /// <summary>
        /// Prefix increment operator (++x).
        /// </summary>
        public static SafeCounter operator ++ ( SafeCounter a ) {
            Interlocked.Increment(ref a.m_value);
            return a;
        }

        /// <summary>
        /// Adds a raw long value to the counter atomically.
        /// </summary>
        public static SafeCounter operator + ( SafeCounter a, long value ) {
            Interlocked.Add(ref a.m_value, value);
            return a;
        }

        /// <summary>
        /// Subtracts a raw long value from the counter atomically.
        /// </summary>
        public static SafeCounter operator - ( SafeCounter a, long value ) {
            Interlocked.Add(ref a.m_value, -value);
            return a;
        }
        /// <summary>
        /// Compares two counters for equality using atomic reads.
        /// </summary>
        public static bool operator == ( SafeCounter? a, SafeCounter? b ) {
            if ( ReferenceEquals(a, b) )
                return true;
            if ( a is null || b is null )
                return false;
            return Volatile.Read(ref a.m_value) == Volatile.Read(ref b.m_value) && a.m_startValue == b.m_startValue;
        }

        /// <summary>
        /// Compares two counters for inequality using atomic reads.
        /// </summary>
        public static bool operator != ( SafeCounter? a, SafeCounter? b ) {
            return !(a == b);
        }


		public static bool operator < ( SafeCounter? a, SafeCounter? b ) {
		
			if ( a is null || b is null )
				return false;
			return Volatile.Read(ref a.m_value) < Volatile.Read(ref b.m_value);
		}

		public static bool operator > ( SafeCounter? a, SafeCounter? b ) {

			if ( a is null || b is null )
				return false;
			return Volatile.Read(ref a.m_value) > Volatile.Read(ref b.m_value);
		}

		public static bool operator <= ( SafeCounter? a, SafeCounter? b ) {

			if ( a is null || b is null )
				return false;
			return Volatile.Read(ref a.m_value) <= Volatile.Read(ref b.m_value);
		}

		public static bool operator >= ( SafeCounter? a, SafeCounter? b ) {

			if ( a is null || b is null )
				return false;
			return Volatile.Read(ref a.m_value) >= Volatile.Read(ref b.m_value);
		}
		/// <summary>
		/// Standard override for equality comparison.
		/// </summary>
		public override bool Equals ( object? obj ) {
            if ( obj is SafeCounter other )
                return this == other;
            return false;
        }

        /// <summary>
        /// Standard override for hash code generation.
        /// </summary>
        public override int GetHashCode ()
            => Volatile.Read(ref m_value).GetHashCode();

       
        /// <inheritdoc/>
        public bool Equals ( SafeCounter? other ) {
           // if ( other is null ) return false;

            return this == other;
        }

        /// <summary>
        /// Reset the counter
        /// </summary>
        public void Reset() {
            Assign(m_startValue);
        }
	}
}
