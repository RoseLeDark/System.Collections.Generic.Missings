using System;
using System.Collections.Generic;
using System.Text;
using SystemEx.Algorythmen;

namespace SystemEx.Algorithms.Interfaces {
    /// \addtogroup Algorithms
    /// @{
    /// \addtogroup interfaces
    /// @{
    /// <summary>
    /// Defines a minimal comparison strategy for two values of type <typeparamref name="T"/>.
    /// 
    /// Implementations provide a single boolean comparison operation, allowing containers,
    /// sorting policies, and search utilities to determine ordering or equivalence without
    /// requiring full <see cref="IComparer{T}"/> or <see cref="IComparable{T}"/> semantics.
    /// </summary>
    /// <typeparam name="T">
    /// The type of elements being compared.
    /// </typeparam>
    public interface ISimpleCompare<T> {
        /// <summary>
        /// Compares two values and returns <c>true</c> if the comparison condition defined
        /// by the implementation holds; otherwise <c>false</c>.
        /// 
        /// The meaning of “comparison condition” depends on the specific comparer:
        /// <list type="bullet">
        ///   <item><description>
        ///     <see cref="Greater{T}"/> returns <c>true</c> if <paramref name="a"/> is strictly
        ///     greater than <paramref name="b"/>.
        ///   </description></item>
        ///   <item><description>
        ///     <see cref="Less{T}"/> returns <c>true</c> if <paramref name="a"/> is strictly
        ///     less than <paramref name="b"/>.
        ///   </description></item>
        ///   <item><description>
        ///     <see cref="EqualTo{T}"/> returns <c>true</c> if both values are equal.
        ///   </description></item>
        /// </list>
        /// 
        /// Comparers may define additional rules for handling empty or null optionals,
        /// depending on the semantics required by the container or algorithm.
        /// </summary>
        /// <param name="a">The first value to compare.</param>
        /// <param name="b">The second value to compare.</param>
        /// <returns>
        /// <c>true</c> if the comparison condition is satisfied; otherwise <c>false</c>.
        /// </returns>
        Triple Compare ( Optional<T> a, Optional<T> b );
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
