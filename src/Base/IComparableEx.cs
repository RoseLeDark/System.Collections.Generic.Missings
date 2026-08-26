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
	/// \addtogroup SystemEx
	/// @{

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