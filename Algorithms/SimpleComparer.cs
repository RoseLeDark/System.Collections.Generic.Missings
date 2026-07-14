using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using SystemEx.Algorithms.Interfaces;
using SystemEx.Collections.Generic.Interfaces;
using SystemEx.Utils;

namespace SystemEx.Algorithms {

    /// <summary>
    /// Provides a standard comparison implementation for types that implement
    /// <see cref="IComparable{T}"/>. This comparer defines a strict total ordering
    /// based on <see cref="IComparable{T}.CompareTo(T)"/> and includes explicit
    /// handling for <c>null</c> values.
    /// </summary>
    /// <typeparam name="T">
    /// The value type being compared. Must implement <see cref="IComparable{T}"/>.
    /// </typeparam>
    public class SimpleComparer<T> : ICompared<T> where T : IComparable<T> {


        /// <summary>
        /// Compares two values using <see cref="IComparable{T}.CompareTo(T)"/>.
        /// </summary>
        /// <param name="x">The first value to compare. May be <c>null</c>.</param>
        /// <param name="y">The second value to compare. May be <c>null</c>.</param>
        /// <returns>
        /// A <see cref="CompareResult"/> describing the ordering:
        /// <list type="bullet">
        /// <item><description><see cref="CompareResult.AIsSmallerB"/> if <c>x</c> is <c>null</c> or smaller.</description></item>
        /// <item><description><see cref="CompareResult.AIsLargerB"/> if <c>x</c> is larger or <c>y</c> is <c>null</c>.</description></item>
        /// <item><description><see cref="CompareResult.Equal"/> if both values are equal.</description></item>
        /// <item><description><see cref="CompareResult.Null"/> if both values are <c>null</c>.</description></item>
        /// </list>
        /// </returns>
        public CompareResult Compare ( T? x, T? y ) {
            if ( x == null && y != null ) return CompareResult.AIsSmallerB;
            if ( x != null && y == null ) return CompareResult.AIsLargerB;
            if ( x == null && y == null ) return CompareResult.Null;

            int cmp = x!.CompareTo(y);

            if ( cmp < 0 ) return CompareResult.AIsSmallerB;
            if ( cmp > 0 ) return CompareResult.AIsLargerB;

            return CompareResult.Equal;
        }
    }

    /// <summary>
    /// Provides a numeric comparison implementation for types implementing
    /// <see cref="INumber{T}"/>. This comparer supports extended relational
    /// semantics such as <c>&lt;=</c> and <c>&gt;=</c>, allowing finer-grained
    /// comparison results beyond strict ordering.
    /// </summary>
    /// <typeparam name="T">
    /// The numeric type being compared. Must implement <see cref="INumber{T}"/>.
    /// </typeparam>
    public sealed class ValueComparer<T> : ICompared<T> where T : INumber<T> {

        /// <summary>
        /// Compares two numeric values using relational operators provided by
        /// <see cref="INumber{T}"/>. Supports both strict and non‑strict ordering.
        /// </summary>
        /// <param name="x">The first numeric value. May be <c>null</c>.</param>
        /// <param name="y">The second numeric value. May be <c>null</c>.</param>
        /// <returns>
        /// A <see cref="CompareResult"/> describing the relation:
        /// <list type="bullet">
        /// <item><description><see cref="CompareResult.AIsEqualSmallerB"/> if <c>x &lt;= y</c>.</description></item>
        /// <item><description><see cref="CompareResult.AIsEqualLargerB"/> if <c>x &gt;= y</c>.</description></item>
        /// <item><description><see cref="CompareResult.AIsSmallerB"/> if <c>x &lt; y</c>.</description></item>
        /// <item><description><see cref="CompareResult.AIsLargerB"/> if <c>x &gt; y</c>.</description></item>
        /// <item><description><see cref="CompareResult.Equal"/> if both values are numerically equal.</description></item>
        /// <item><description><see cref="CompareResult.Null"/> if both values are <c>null</c>.</description></item>
        /// </list>
        /// </returns>
        public CompareResult Compare ( T? x, T? y ) {
            if ( x == null && y != null ) return CompareResult.AIsSmallerB;
            if ( x != null && y == null ) return CompareResult.AIsLargerB;
            if ( x == null && y == null ) return CompareResult.Null;

            T A = x!;
            T B = y!;

            if ( A <= B ) return CompareResult.AIsEqualSmallerB;
            if ( A >= B ) return CompareResult.AIsEqualLargerB;
            if ( A < B ) return CompareResult.AIsSmallerB;
            if ( A > B ) return CompareResult.AIsLargerB;

            return CompareResult.Equal;
        }
    }
}
