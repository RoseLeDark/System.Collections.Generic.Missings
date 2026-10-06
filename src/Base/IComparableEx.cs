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

using SystemEx.Numeric;
using SystemEx.Utils;

namespace SystemEx {
	/// <summary>
	/// Specifies the result of a comparison between two values.
	/// 
	/// <para>
	/// This enumeration is used by <see cref="CompFunc{T}"/> and the generic
	/// algorithms in <see cref="Algorithm"/> to express ordering relations
	/// between two operands <c>A</c> and <c>B</c>. It generalizes the usual
	/// "less/greater/equal" semantics with additional states for "equal but
	/// smaller" and "equal but larger" to support nuanced ordering logic.
	/// </para>
	/// </summary>
	public enum CompareResult : sbyte {
		/// <summary>
		/// Alias for <see cref="CompareResult.AIsSmallerB"/>. Indicates that
		/// the first operand is strictly smaller than the second.
		/// </summary>
		Less = AIsSmallerB,

		/// <summary>
		/// Alias for <see cref="CompareResult.AIsLargerB"/>. Indicates that
		/// the first operand is strictly larger than the second.
		/// </summary>
		Greater = 1,

		/// <summary>
		/// Alias for <see cref="CompareResult.AIsEqualSmallerB"/>. Indicates
		/// that the first operand is equal to the second but considered
		/// "smaller" in a secondary ordering dimension.
		/// </summary>
		EqualLess = AIsEqualSmallerB,

		/// <summary>
		/// Alias for <see cref="CompareResult.AIsEqualLargerB"/>. Indicates
		/// that the first operand is equal to the second but considered
		/// "larger" in a secondary ordering dimension.
		/// </summary>
		EqualGreater = AIsEqualLargerB,

		/// <summary>
		/// The first operand <c>A</c> is strictly larger than the second
		/// operand <c>B</c>.
		/// </summary>
		AIsLargerB = 1,

		/// <summary>
		/// The first operand <c>A</c> is strictly smaller than the second
		/// operand <c>B</c>.
		/// </summary>
		AIsSmallerB = -1,

		/// <summary>
		/// The operands <c>A</c> and <c>B</c> are considered equal in the
		/// primary ordering dimension.
		/// </summary>
		Equal = 0,

		/// <summary>
		/// The operands are equal in the primary dimension, but <c>A</c> is
		/// treated as "greater" in a secondary dimension (e.g. tie‑breaking).
		/// </summary>
		AIsEqualLargerB = 2,

		/// <summary>
		/// The operands are equal in the primary dimension, but <c>A</c> is
		/// treated as "smaller" in a secondary dimension.
		/// </summary>
		AIsEqualSmallerB = 3,

		/// <summary>
		/// One or both operands are <c>null</c>, or the comparison function
		/// cannot produce a meaningful ordering result.
		/// </summary>
		Null = 10
	}
	/// <summary>
	/// Provides an extended and strongly typed comparison contract for SystemEx.
	/// 
	/// Unlike <see cref="System.IComparable{T}"/>, which returns an integer
	/// (-1, 0, +1), this interface uses the explicit <see cref="CompareResult"/>
	/// enumeration. This makes comparison outcomes easier to interpret and avoids
	/// ambiguity, especially in low‑level or domain‑specific types.
	/// 
	/// <para>
	/// <b>Compatibility with IComparable&lt;T&gt;:</b><br/>
	/// Since <see cref="CompareResult"/> is backed by an integer, any type can
	/// implement both interfaces simultaneously. The standard CompareTo method
	/// can simply cast the extended result:
	/// </para>
	/// 
	/// <code>
	/// public sealed class Foo : IComparableEx&lt;Foo&gt;, IComparable&lt;Foo&gt;
	/// {
	///     public CompareResult CompareTo(Foo other)
	///     {
	///         // Custom comparison logic...
	///         return CompareResult.Equal;
	///     }
	///
	///     int IComparable&lt;Foo&gt;.CompareTo(Foo other)
	///     {
	///         // Cast the extended comparison result to an int.
	///         return (int)CompareTo(other);
	///     }
	/// }
	/// </code>
	/// 
	/// <para>
	/// This interface is intentionally generic and can be implemented by any type:
	/// numeric primitives, geometric structures, colors, states, or any other
	/// domain‑specific objects requiring deterministic comparison semantics.
	/// </para>
	/// </summary>
	/// <typeparam name="T">
	/// The type that this instance can be compared against.
	/// </typeparam>
	public interface IComparableEx<T> {
        /// <summary>
        /// Compares this instance with the specified value and returns a
        /// <see cref="CompareResult"/> describing the relationship between them.
        /// 
        /// Implementations may define their own comparison rules, such as:
        /// magnitude-based, lexicographical, structural, bitwise, or any
        /// domain-specific logic required by the type.
        /// 
        /// The comparison must be deterministic and should not depend on
        /// external state, floating‑point environment, or platform-specific
        /// behavior. This makes the interface suitable for low-level systems,
        /// serialization, math primitives, and engine-independent utilities.
        /// </summary>
        /// <param name="a">
        /// The value to compare with this instance.
        /// </param>
        /// <returns>
        /// A <see cref="CompareResult"/> indicating whether this instance is
        /// less than, equal to, or greater than <paramref name="a"/>.
        /// </returns>
        CompareResult CompareTo ( T a );
    }
	//@}
}