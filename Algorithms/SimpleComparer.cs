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


using System.Numerics;
using SystemEx.Utils;

namespace SystemEx.Algorithms {
    /// \addtogroup Algorithms
    /// @{

    /// <summary>
    /// Provides a standard comparison implementation for values wrapped in
    /// <see cref="Optional{T}"/> where <typeparamref name="T"/> implements
    /// <see cref="IComparable{T}"/>.
    /// 
    /// The comparison delegates to <see cref="Optional{T}.CompareTo(Optional{T})"/>,
    /// which defines ordering semantics for present and non‑present values.
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
        public CompareResult Compare( Optional<T> x, Optional<T> y ){

            var cmp = x.CompareTo(y);

            return cmp;
        }
    }

    /// <summary>
    /// Provides a numeric comparison implementation for values wrapped in
    /// <see cref="Optional{T}"/> where <typeparamref name="T"/> implements
    /// <see cref="INumber{T}"/>.
    /// 
    /// This comparer supports both strict and non‑strict relational operators,
    /// enabling finer‑grained comparison results for numeric types.
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
        /// <item><description><see cref="CompareResult.EqualLess"/> if <c>x &lt;= y</c>.</description></item>
        /// <item><description><see cref="CompareResult.EqualGreater"/> if <c>x &gt;= y</c>.</description></item>
        /// <item><description><see cref="CompareResult.Less"/> if <c>x &lt; y</c>.</description></item>
        /// <item><description><see cref="CompareResult.Greater"/> if <c>x &gt; y</c>.</description></item>
        /// <item><description><see cref="CompareResult.Equal"/> if both values are numerically equal.</description></item>
        /// <item><description><see cref="CompareResult.Null"/> if both values are <c>null</c>.</description></item>
        /// </list>
        /// </returns>
        public CompareResult Compare( Optional<T> x, Optional<T> y ){
            if ( x.IsNull && y.IsSome ) return CompareResult.Less;
            if ( x.IsSome && y.IsNull ) return CompareResult.Greater;
            if ( x.IsNull && y.IsNull ) return CompareResult.Null;

            T A = (T)x;
            T B = (T)y;

            if ( A <= B ) return CompareResult.EqualLess;
            if ( A >= B ) return CompareResult.EqualGreater;
            if ( A < B ) return CompareResult.Less;
            if ( A > B ) return CompareResult.Greater;

            return CompareResult.Equal;
        }
    }
    /// @}
}
