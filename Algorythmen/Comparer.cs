using System;
using System.Collections.Generic;
using System.Text;
using SystemEx.Collections.Generic.Interfaces;
using SystemEx.Utils;

namespace SystemEx.Algorythmen {

    /// <summary>
    /// Provides a boolean comparison that determines whether
    /// <paramref name="a"/> is strictly smaller than <paramref name="b"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of values being compared.
    /// </typeparam>
    public sealed class Less<T> : ISimpleCompare<T> {
        /// <summary>
        /// Returns <c>true</c> if <paramref name="a"/> is strictly smaller
        /// than <paramref name="b"/>; otherwise <c>false</c>.
        /// </summary>
        /// <param name="a">The first value to compare.</param>
        /// <param name="b">The second value to compare.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="a"/> is smaller than <paramref name="b"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        public bool Compare ( T a, T b ) {
            return Comparer<T>.Default.Compare(a, b) < 0;
        }
    }



    /// <summary>
    /// Provides a boolean comparison that determines whether
    /// <paramref name="a"/> is strictly larger than <paramref name="b"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of values being compared.
    /// </typeparam>
    public sealed class Greater<T> : ISimpleCompare<T> {
        /// <summary>
        /// Returns <c>true</c> if <paramref name="a"/> is strictly larger
        /// than <paramref name="b"/>; otherwise <c>false</c>.
        /// </summary>
        /// <param name="a">The first value to compare.</param>
        /// <param name="b">The second value to compare.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="a"/> is larger than <paramref name="b"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        public bool Compare ( T a, T b ) {
            return Comparer<T>.Default.Compare(a, b) > 0;
        }
    }



    /// <summary>
    /// Provides a boolean comparison that determines whether
    /// <paramref name="a"/> and <paramref name="b"/> are equal.
    /// </summary>
    /// <typeparam name="T">
    /// The type of values being compared.
    /// </typeparam>
    public sealed class EqualTo<T> : ISimpleCompare<T> {
        /// <summary>
        /// Returns <c>true</c> if <paramref name="a"/> and <paramref name="b"/>
        /// compare equal; otherwise <c>false</c>.
        /// </summary>
        /// <param name="a">The first value to compare.</param>
        /// <param name="b">The second value to compare.</param>
        /// <returns>
        /// <c>true</c> if both values are equal; otherwise <c>false</c>.
        /// </returns>
        public bool Compare ( T a, T b ) {
            return Comparer<T>.Default.Compare(a, b) == 0;
        }
    }


    /// <summary>
    /// Provides a boolean comparison that determines whether
    /// <paramref name="a"/> and <paramref name="b"/> are not equal.
    /// </summary>
    /// <typeparam name="T">
    /// The type of values being compared.
    /// </typeparam>
    public sealed class NotEqualTo<T> : ISimpleCompare<T> {
        /// <summary>
        /// Returns <c>true</c> if <paramref name="a"/> and <paramref name="b"/>
        /// do not compare equal; otherwise <c>false</c>.
        /// </summary>
        /// <param name="a">The first value to compare.</param>
        /// <param name="b">The second value to compare.</param>
        /// <returns>
        /// <c>true</c> if both values differ; otherwise <c>false</c>.
        /// </returns>
        public bool Compare ( T a, T b ) {
            return Comparer<T>.Default.Compare(a, b) != 0;
        }
    }



    /// <summary>
    /// Provides a boolean comparison that determines whether
    /// <paramref name="a"/> is smaller than or equal to <paramref name="b"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of values being compared.
    /// </typeparam>
    public sealed class LessEqual<T> : ISimpleCompare<T> {
        /// <summary>
        /// Returns <c>true</c> if <paramref name="a"/> is smaller than or equal
        /// to <paramref name="b"/>; otherwise <c>false</c>.
        /// </summary>
        /// <param name="a">The first value to compare.</param>
        /// <param name="b">The second value to compare.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="a"/> is less than or equal to
        /// <paramref name="b"/>; otherwise <c>false</c>.
        /// </returns>
        public bool Compare ( T a, T b ) {
            return Comparer<T>.Default.Compare(a, b) <= 0;
        }
    }


    /// <summary>
    /// Provides a boolean comparison that determines whether
    /// <paramref name="a"/> is larger than or equal to <paramref name="b"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of values being compared.
    /// </typeparam>
    public sealed class GreaterEqual<T> : ISimpleCompare<T> {
        /// <summary>
        /// Returns <c>true</c> if <paramref name="a"/> is larger than or equal
        /// to <paramref name="b"/>; otherwise <c>false</c>.
        /// </summary>
        /// <param name="a">The first value to compare.</param>
        /// <param name="b">The second value to compare.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="a"/> is greater than or equal to
        /// <paramref name="b"/>; otherwise <c>false</c>.
        /// </returns>
        public bool Compare ( T a, T b ) {
            return Comparer<T>.Default.Compare(a, b) >= 0;
        }
    }


}
