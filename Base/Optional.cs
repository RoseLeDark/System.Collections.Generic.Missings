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


using SystemEx.Utils;

namespace SystemEx {

    /// <summary>
    /// Represents a optional value container that stores a value only when
    /// <see cref="HasValue"/> is true. Unlike <see cref="Nullable{T}"/>, this type does not
    /// preserve <c>null</c> as a stored value; assigning <c>null</c> clears the optional state.
    /// </summary>
    /// <typeparam name="T">
    /// The underlying value type. Can be either a reference type or a value type.
    /// </typeparam>
    public struct Optional<T> : IComparableEx<Optional<T>>, IComparable<Optional<T>>, IEquatable<Optional<T>> {
        private T m_value;
        private bool m_hasValue;

        /// <summary>
        /// A predefined empty optional instance representing the absence of a value.
        /// </summary>
        public static readonly Optional<T> NONE = new Optional<T> { m_hasValue = false };

        /// <summary>
        /// Gets or sets the underlying value. 
        /// When retrieving, <c>default(T)</c> is returned if no value is present.
        /// When assigning, <c>null</c> clears the optional state, while any non-null value
        /// sets <see cref="HasValue"/> to true and stores the value.
        /// </summary>
        public T? Value { 
            get {
                if ( m_hasValue ) return m_value;
                else return default(T);
            }
            set {
                if ( value == null ) {
                    m_hasValue = false;
                } else {
                    m_hasValue = true; 
                    m_value = value;
                }
            }
         }

        /// <summary>
        /// Indicates whether this optional instance currently contains a value.
        /// </summary>
        public bool HasValue  { set => m_hasValue = value; get => m_hasValue; }

        /// <summary>
        /// Indicates whether this optional instance currently represents an empty state.
        /// This is functionally equivalent to checking <see cref="HasValue"/> and returning
        /// <c>false</c> when no value is present.
        /// </summary>
        public bool IsNull => !m_hasValue;

        /// <summary>
        /// Indicates whether this optional instance currently contains a value.
        /// </summary>
        public bool IsSome => m_hasValue;

        /// <summary>
        /// Created A empty Optional Object
        /// </summary>
        public Optional () {
            m_hasValue = false;
            m_value = default!;
        }
        /// <summary>
        /// Initializes a new instance of <see cref="Optional{T}"/> using the provided value.
        /// Assigning <c>null</c> results in an empty optional; otherwise the value is stored.
        /// </summary>
        /// <param name="value">
        /// The value to assign. If <c>null</c>, the optional becomes empty.
        /// </param>
        public Optional ( T? value ) {
            if ( value == null ) {
                m_hasValue = false;
            } else {
                m_hasValue = true;
                m_value = value;
            }
        }

        /// <summary>
        /// Clears the optional state, removing any stored value and marking the instance
        /// as empty.
        /// </summary>
        public void Nullable () {
            m_hasValue = false;
        }

        /// <summary>
        /// Compares this optional instance with another optional instance and returns a
        /// <see cref="CompareResult"/> describing their relative ordering.
        /// </summary>
        /// <param name="other">
        /// The optional instance to compare against.
        /// </param>
        /// <returns>
        /// A <see cref="CompareResult"/> value indicating whether this instance is less than,
        /// equal to, or greater than the specified instance.
        /// </returns>
        /// <remarks>
        /// Comparison rules:
        /// <list type="bullet">
        /// <item><description>
        /// An empty optional (<see cref="HasValue"/> is <c>false</c>) is always considered
        /// greater than a non‑empty optional, ensuring that empty values sort last.
        /// </description></item>
        /// <item><description>
        /// If both optionals are empty, they are considered equal.
        /// </description></item>
        /// <item><description>
        /// If both contain values, the comparison is performed using
        /// <see cref="Comparer{T}.Default"/> and the result is cast directly to
        /// <see cref="CompareResult"/>.
        /// </description></item>
        /// </list>
        /// </remarks>
        public CompareResult CompareTo ( Optional<T> other ) {
            // Empty vs. non-empty
            if ( !m_hasValue && other.m_hasValue )
                return CompareResult.Greater; // empty sorts last -  A > B

            if ( m_hasValue && !other.m_hasValue )
                return CompareResult.Less; // non-empty sorts first

            // Both empty
            if ( !m_hasValue && !other.m_hasValue )
                return CompareResult.Equal;

            // Both have values → compare normally
            int cmp = Comparer<T>.Default.Compare(m_value, other.m_value);

            return (CompareResult)cmp;
        }

        /// <summary>
        /// Compares this optional instance with another optional instance using the
        /// <see cref="CompareTo(Optional{T})"/> method and returns a standard integer
        /// comparison result compatible with <see cref="IComparable{T}"/>.
        /// </summary>
        /// <param name="other">
        /// The optional instance to compare against.
        /// </param>
        /// <returns>
        /// <c>-1</c> if this instance is considered less than <paramref name="other"/>,
        /// <c>0</c> if both instances are equal,
        /// or <c>1</c> if this instance is greater.
        /// </returns>
        /// <remarks>
        /// This method simply forwards to <see cref="CompareTo(Optional{T})"/> and casts
        /// the resulting <see cref="CompareResult"/> to an integer, making the optional
        /// type compatible with standard .NET sorting and comparison mechanisms.
        /// </remarks>
        int IComparable<Optional<T>>.CompareTo ( Optional<T> other ) {
            return (int)CompareTo(other);
        }

        public bool Equals ( Optional<T> b ) {
            bool _ret = false;

            if ( IsNull && b.IsNull ) _ret = true;
            else if ( IsNull && b.IsSome ) _ret = false;
            else if ( IsSome && b.IsNull ) _ret = false;
            else
                _ret = Value!.GetHashCode() == b.Value!.GetHashCode();

            return _ret;
        }

        /// <summary>
        /// Implicitly converts a nullable value into an <see cref="Optional{T}"/>.
        /// A <c>null</c> value produces an empty optional; otherwise the value is stored.
        /// </summary>
        /// <param name="value">The nullable value to convert.</param>
        public static implicit operator Optional<T> ( T? value ) {
            Optional<T> o = new Optional<T>();
            o.Value = value;
            return o;
        }

        /// <summary>
        /// Explicitly extracts the underlying value from an <see cref="Optional{T}"/>.
        /// This operation does not check <see cref="HasValue"/>; callers must ensure
        /// that a value is present before casting.
        /// </summary>
        /// <param name="opt">The optional instance to extract from.</param>
        /// <returns>The stored value.</returns>
        public static explicit operator T ( Optional<T> opt ) {
            return opt.m_value;
        }

        /// <summary>
        /// Explicitly converts an <see cref="Optional{T}"/> to a boolean indicating whether
        /// the instance contains a value. This allows optional values to be used directly
        /// in conditional expressions.
        /// </summary>
        /// <param name="opt">The optional instance to evaluate.</param>
        /// <returns>
        /// <c>true</c> if the optional contains a value; otherwise <c>false</c>.
        /// </returns>
        public static explicit operator bool ( Optional<T> opt ) {
            return opt.m_hasValue;
        }

        /// <summary>
        /// Determines whether the optional instance should be considered logically true.
        /// This operator returns <c>true</c> when a value is present, enabling the optional
        /// to participate naturally in conditional expressions and short‑circuit logic.
        /// </summary>
        /// <param name="opt">The optional instance to evaluate.</param>
        /// <returns>
        /// <c>true</c> if the optional contains a value; otherwise <c>false</c>.
        /// </returns>
        public static bool operator true ( Optional<T> opt ) {
            return opt.m_hasValue;
        }

        /// <summary>
        /// Determines whether the optional instance should be considered logically false.
        /// This operator returns <c>true</c> when no value is present, allowing the optional
        /// to be used directly in boolean contexts that rely on <c>false</c> evaluation.
        /// </summary>
        /// <param name="opt">The optional instance to evaluate.</param>
        /// <returns>
        /// <c>true</c> if the optional does not contain a value; otherwise <c>false</c>.
        /// </returns>
        public static bool operator false ( Optional<T> opt ) {
            return !opt.m_hasValue;
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool operator == ( Optional<T> a, Optional<T> b ) {
            bool _ret = false;

                 if ( a.IsNull && a.IsNull ) _ret = true;
            else if ( a.IsNull && b.IsSome ) _ret = false;
            else if ( a.IsSome && b.IsNull ) _ret = false;
            else 
                _ret = a.Value!.GetHashCode() == b.Value!.GetHashCode() ;

            return _ret;
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool operator != ( Optional<T> a, Optional<T> b ) {
            return !(a == b);
        }

        public override bool Equals ( object? obj ) {
            if ( obj is Optional<T> key ) 
                return Equals(key);
            return false;
        }

        public override int GetHashCode () {
            return m_value!.GetHashCode();
        }
    }


}
