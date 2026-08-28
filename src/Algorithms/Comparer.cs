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


using SystemEx.Algorithms;
using SystemEx.Utils;

namespace SystemEx.Algorithms {
	/// <summary>
	/// Provides a three-valued comparison based on standard .NET ordering.
	/// Determines whether A is smaller, greater, or equal to B.
	/// </summary>
	/// <typeparam name="T">
	/// The type of values being compared.
	/// </typeparam>
	public sealed class NetStdCompare<T> : ISimpleCompare<T> {
        /// <summary>
        /// Returns <see cref="triple.True"/> if A is strictly smaller than B,
        /// <see cref="triple.False"/> if A is strictly greater than B,
        /// and <see cref="triple.Nin"/> if both values are equal.
        ///
        /// Comparison is only performed when both optionals contain valid,
        /// non-null values. If either optional is null, the comparison is
        /// undefined and this method returns <see cref="triple.Nin"/>.
        /// </summary>
        public Triple Compare ( Optional<T> a, Optional<T> b ) {
            if ( a.IsNull || b.IsNull )
                return triple.Nin;

            int cmp = Comparer<T>.Default.Compare((T)a, (T)b);

            if ( cmp < 0 ) return triple.True;   // a < b
            if ( cmp > 0 ) return triple.False;  // a > b
            return triple.Nin;                 // equal
        }
    }


    /// <summary>
    /// Provides a three-valued equality comparison.
    /// Determines whether A is equal to B.
    /// </summary>
    /// <typeparam name="T">
    /// The type of values being compared.
    /// </typeparam>
    public sealed class KleeneEqual<T> : ISimpleCompare<T> {
        /// <summary>
        /// Returns <c>true</c> if A is equal to B;
        /// otherwise <c>false</c>.
        ///
        /// Comparison is only performed when both optionals contain valid,
        /// non-null values. If either optional is null, the comparison is
        /// undefined and this method returns <see cref="triple.Nin"/>.
        /// </summary>
        public Triple Compare ( Optional<T> a, Optional<T> b ) {
            if ( a.IsNull || b.IsNull )
                return triple.Nin;

            int cmp = Comparer<T>.Default.Compare((T)a, (T)b);

            if ( cmp == 0 ) return triple.True;
            return triple.False;
        }
    }

    /// <summary>
    /// Provides a three-valued comparison that determines whether
    /// A is strictly smaller than B.
    /// </summary>
    /// <typeparam name="T">
    /// The type of values being compared.
    /// </typeparam>
    public sealed class KleenLess<T> : ISimpleCompare<T> {
        /// <summary>
        /// Returns <c>true</c> if A is strictly smaller
        /// than B; otherwise <c>false</c>.
        ///
        /// Comparison is only performed when both optionals contain valid,
        /// non-null values. If either optional is null, the comparison is
        /// undefined and this method returns <see cref="triple.Nin"/>.
        /// </summary>
        public Triple Compare ( Optional<T> a, Optional<T> b ) {
            if ( a.IsNull || b.IsNull )
                return triple.Nin;

            return Comparer<T>.Default.Compare((T)a, (T)b) < 0;
        }
    }

    /// <summary>
    /// Provides a three-valued comparison that determines whether
    /// A is strictly greater than B.
    /// </summary>
    /// <typeparam name="T">
    /// The type of values being compared.
    /// </typeparam>
    public sealed class KleenGreater<T> : ISimpleCompare<T> {
        /// <summary>
        /// Returns <c>true</c> if A is strictly greater
        /// than B; otherwise <c>false</c>.
        ///
        /// Comparison is only performed when both optionals contain valid,
        /// non-null values. If either optional is null, the comparison is
        /// undefined and this method returns <see cref="triple.Nin"/>.
        /// </summary>
        public Triple Compare ( Optional<T> a, Optional<T> b ) {
            if ( a.IsNull || b.IsNull )
                return triple.Nin;

            return Comparer<T>.Default.Compare((T)a, (T)b) > 0;
        }
    }


    /// <summary>
    /// Provides a three-valued comparison that determines whether
    /// A is equal to B.
    /// </summary>
    /// <typeparam name="T">
    /// The type of values being compared.
    /// </typeparam>
    public sealed class KleenEqualTo<T> : ISimpleCompare<T> {
        /// <summary>
        /// Returns <c>true</c> if A is equal to B;
        /// otherwise <c>false</c>.
        ///
        /// Comparison is only performed when both optionals contain valid,
        /// non-null values. If either optional is null, the comparison is
        /// undefined and this method returns <see cref="triple.Nin"/>.
        /// </summary>
        public Triple Compare ( Optional<T> a, Optional<T> b ) {
            if ( a.IsNull || b.IsNull )
                return triple.Nin;

            return Comparer<T>.Default.Compare((T)a, (T)b) == 0;
        }
    }



    /// <summary>
    /// Provides a Tripleean comparison that determines whether
    /// A is strictly smaller than B.
    /// </summary>
    /// <typeparam name="T">
    /// The type of values being compared.
    /// </typeparam>
    public sealed class Less<T> : ISimpleCompare<T> {
        /// <summary>
        /// Returns <c>true</c>A is strictly smaller
        /// than B; otherwise <c>false</c>.
        /// 
        /// Comparison is only performed when both optionals contain valid,
        /// non-null values. If either optional is null, the comparison is
        /// undefined and this method returns <c>false</c>.
        /// </summary>
        public Triple Compare ( Optional<T> a, Optional<T> b ) {
            if ( a.IsNull || b.IsNull )
                return false;

            return Comparer<T>.Default.Compare((T)a, (T)b) < 0;
        }
    }

    /// <summary>
    /// Provides a Tripleean comparison that determines whether
    /// A is strictly larger than B.
    /// </summary>
    /// <typeparam name="T">
    /// The type of values being compared.
    /// </typeparam>
    public sealed class Greater<T> : ISimpleCompare<T> {
        /// <summary>
        /// Returns <c>true</c> if A is strictly larger
        /// than B; otherwise <c>false</c>.
        /// 
        /// Comparison is only performed when both optionals contain valid,
        /// non-null values. If either optional is null, the comparison is
        /// undefined and this method returns <c>false</c>.
        /// </summary>
        public Triple Compare ( Optional<T> a, Optional<T> b ) {
            if ( a.IsNull || b.IsNull )
                return false;

            return Comparer<T>.Default.Compare((T)a, (T)b) > 0;
        }
    }

    /// <summary>
    /// Provides a Tripleean comparison that determines whether
    /// A and B are equal.
    /// </summary>
    /// <typeparam name="T">
    /// The type of values being compared.
    /// </typeparam>
    public sealed class EqualTo<T> : ISimpleCompare<T> {
        /// <summary>
        /// Returns <c>true</c> if A and B
        /// compare equal; otherwise <c>false</c>.
        /// 
        /// Comparison is only performed when both optionals contain valid,
        /// non-null values. If either optional is null, the comparison is
        /// undefined and this method returns <c>false</c>.
        /// </summary>
        public Triple Compare ( Optional<T> a, Optional<T> b ) {
            if ( a.IsNull || b.IsNull )
                return false;

            return Comparer<T>.Default.Compare((T)a, (T)b) == 0;
        }
    }


    /// <summary>
    /// Provides a Tripleean comparison that determines whether
    /// A and B are not equal.
    /// </summary>
    /// <typeparam name="T">
    /// The type of values being compared.
    /// </typeparam>
    public sealed class NotEqualTo<T> : ISimpleCompare<T> {
        /// <summary>
        /// Returns <c>true</c> if A and B
        /// do not compare equal; otherwise <c>false</c>.
        /// 
        /// Comparison is only performed when both optionals contain valid,
        /// non-null values. If either optional is null, the comparison is
        /// undefined and this method returns <c>false</c>.
        /// </summary>
        public Triple Compare ( Optional<T> a, Optional<T> b ) {
            if ( a.IsNull || b.IsNull )
                return false;

            return Comparer<T>.Default.Compare((T)a, (T)b) != 0;
        }
    }

    /// <summary>
    /// Provides a Tripleean comparison that determines whether
    /// X is smaller than or equal to B.
    /// </summary>
    /// <typeparam name="T">
    /// The type of values being compared.
    /// </typeparam>
    public sealed class LessEqual<T> : ISimpleCompare<T> {
        /// <summary>
        /// Returns <c>true</c> if A is smaller than or equal
        /// to B; otherwise <c>false</c>.
        /// 
        /// Comparison is only performed when both optionals contain valid,
        /// non-null values. If either optional is null, the comparison is
        /// undefined and this method returns <c>false</c>.
        /// </summary>
        public Triple Compare ( Optional<T> a, Optional<T> b ) {
            if ( a.IsNull || b.IsNull )
                return false;

            return Comparer<T>.Default.Compare((T)a, (T)b) <= 0;
        }
    }


    /// <summary>
    /// Provides a Tripleean comparison that determines whether
    /// A is larger than or equal to B.
    /// </summary>
    /// <typeparam name="T">
    /// The type of values being compared.
    /// </typeparam>
    public sealed class GreaterEqual<T> : ISimpleCompare<T> {
        /// <summary>
        /// Returns <c>true</c> if A is larger than or equal
        /// to B; otherwise <c>false</c>.
        /// 
        /// Comparison is only performed when both optionals contain valid,
        /// non-null values. If either optional is null, the comparison is
        /// undefined and this method returns <c>false</c>.
        /// </summary>
        public Triple Compare ( Optional<T> a, Optional<T> b ) {
            if ( a.IsNull || b.IsNull )
                return false;

            return Comparer<T>.Default.Compare((T)a, (T)b) >= 0;
        }
    }

    

}
