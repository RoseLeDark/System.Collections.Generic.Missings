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
using SystemEx.Hash;

namespace SystemEx.Numeric {
	/// \addtogroup SystemEx.Numeric
	/// @{
	/// <summary>
	/// Defines the structural layout and behavioral contract of a custom
	/// floating‑point format. Implementations describe how a binary floating‑point
	/// number is encoded into sign, exponent, and mantissa fields, including
	/// exponent bias, hidden bit rules, and total bit width.
	/// 
	/// <para>
	/// <see cref="IFloat{TSelf, TBias}"/> abstracts the representation of
	/// non‑IEEE floating formats such as <c>Half16</c>, <c>Half16b</c>,
	/// <c>BFloat16</c>, or any other custom 16‑bit or N‑bit float. It provides:
	/// </para>
	/// 
	/// <list type="bullet">
	/// <item>
	/// <description>
	/// Structural metadata (bit counts, bias, hidden bit).
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// Field extraction (sign, exponent, mantissa).
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// Classification (zero, infinity, NaN, subnormal, normal, integer).
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// Arithmetic operations (add, multiply, divide, unary ops).
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// Comparison operators and ordering semantics.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// Serialization support via <see cref="IByteSerializable{TSelf}"/>.
	/// </description>
	/// </item>
	/// </list>
	/// 
	/// <para>
	/// Implementations must be <c>struct</c> types and self‑referential
	/// (<typeparamref name="TSelf"/>). This ensures value‑type semantics and
	/// prevents boxing during arithmetic or comparison operations.
	/// </para>
	/// </summary>
	public interface IFloat<TSelf, TBias> : IEquatable<TSelf>, IComparable, IComparable<TSelf>, 
        IComparableEx<TSelf>, IHashable<TSelf>, IByteSerializable<TSelf>

        where TSelf : struct, IFloat<TSelf, TBias> {
		/// <summary>
		/// Gets the underlying raw storage type used for bit extraction.
		/// Implementations typically return <c>ushort</c> or <c>uint</c> depending
		/// on the float width.
		/// </summary>
		public TBias ToBase { get;  }

		/// <summary>
		/// Number of bits used for the sign field.
		/// </summary>
		public TBias SignBits { get; }

		/// <summary>
		/// Number of bits used for the exponent field.
		/// </summary>
		public TBias ExponentBits { get; }

		/// <summary>
		/// Number of bits used for the mantissa (fraction) field.
		/// </summary>
		public TBias MantissaBits { get; }

		/// <summary>
		/// Exponent bias applied to the exponent field. Defines the offset used
		/// when converting between encoded exponent and real exponent.
		/// </summary>
		public TBias ExponentBias { get; }

		/// <summary>
		/// Total number of bits in the floating‑point representation.
		/// </summary>
		public TBias TotalBits { get; }

		/// <summary>
		/// Gets the hidden bit (implicit leading mantissa bit). For IEEE‑like
		/// formats this is typically <c>1</c> for normalized numbers and <c>0</c>
		/// for subnormals.
		/// </summary>
		public ushort HiddenBit { get; }

		/// <summary>
		/// Gets the sign bit. <c>true</c> indicates a negative value.
		/// </summary>
		public bool Sign { get; }

		/// <summary>
		/// Gets the exponent field as stored in the encoded representation.
		/// </summary>
		public TBias Exponent { get; }

		/// <summary>
		/// Gets the mantissa (fraction) field as stored in the encoded
		/// representation.
		/// </summary>
		public TBias Mantissa { get; }

		// --- Static constants ---
		/// <summary>
		/// Represents the floating‑point value zero.
		/// </summary>
		static abstract TSelf Zero { get; }

		/// <summary>
		/// Represents the floating‑point value one.
		/// </summary>
		static abstract TSelf One { get; }

		/// <summary>
		/// Represents the floating‑point value negative one.
		/// </summary>
		static abstract TSelf NegativeOne { get; }

		/// <summary>
		/// Represents negative zero (sign bit set, exponent and mantissa zero).
		/// </summary>
		static abstract TSelf NegativeZero { get; }

		/// <summary>
		/// Represents positive infinity.
		/// </summary>
		static abstract TSelf PositiveInfinity { get; }

		/// <summary>
		/// Represents negative infinity.
		/// </summary>
		static abstract TSelf NegativeInfinity { get; }

		/// <summary>
		/// Represents a quiet NaN value.
		/// </summary>
		static abstract TSelf NaN { get; }

		/// <summary>
		/// Represents an alternative NaN encoding (implementation‑specific).
		/// </summary>
		static abstract TSelf NaN2 { get; }

		/// <summary>
		/// Represents the smallest positive increment representable by the format.
		/// </summary>
		static abstract TSelf Epsilon { get; }

		/// <summary>
		/// Represents Euler’s number <c>e</c>.
		/// </summary>
		static abstract TSelf E { get; }

		/// <summary>
		/// Represents τ = 2π.
		/// </summary>
		static abstract TSelf Tau { get; }

		/// <summary>
		/// Represents π.
		/// </summary>
		static abstract TSelf Pi { get; }

		// --- Static classification ---
		/// <summary>
		/// Determines whether the value is zero (positive or negative).
		/// </summary>
		static abstract bool IsZero ( TSelf value );

		/// <summary>
		/// Determines whether the value is negative.
		/// </summary>
		static abstract bool IsNegative ( TSelf value );

		/// <summary>
		/// Determines whether the value is a NaN (not‑a‑number).
		/// </summary>
		static abstract bool IsNaN ( TSelf value );

		/// <summary>
		/// Determines whether the value is positive or negative infinity.
		/// </summary>
		static abstract bool IsInfinity ( TSelf value );

		/// <summary>
		/// Determines whether the value is finite (not NaN or infinity).
		/// </summary>
		static abstract bool IsFinite ( TSelf value );

		/// <summary>
		/// Determines whether the value is subnormal (exponent field zero but
		/// mantissa non‑zero).
		/// </summary>
		static abstract bool IsSubnormal ( TSelf value );

		/// <summary>
		/// Determines whether the value is a normalized floating‑point number.
		/// </summary>
		static abstract bool IsNormal ( TSelf value );

		/// <summary>
		/// Determines whether the value represents an integer (mantissa zero).
		/// </summary>
		static abstract bool IsInteger ( TSelf value );

		// --- Static unary operations ---
		/// <summary>
		/// Returns the absolute value of the given floating‑point number.
		/// </summary>
		static abstract TSelf Abs ( TSelf value );

		/// <summary>
		/// Returns the negation of the given floating‑point number.
		/// </summary>
		static abstract TSelf Negate ( TSelf value );

		/// <summary>
		/// Returns the sign of the value: −1, 0, or +1.
		/// </summary>
		static abstract TSelf Signum ( TSelf value );

		/// <summary>
		/// Returns the largest integer less than or equal to the value.
		/// </summary>
		static abstract TSelf Floor ( TSelf value );

		/// <summary>
		/// Returns the smallest integer greater than or equal to the value.
		/// </summary>
		static abstract TSelf Ceil ( TSelf value );

		/// <summary>
		/// Truncates the fractional part of the value.
		/// </summary>
		static abstract TSelf Trunc ( TSelf value );

		/// <summary>
		/// Clamps the value to the inclusive range [min, max].
		/// </summary>
		static abstract TSelf Clamp ( TSelf x, TSelf min, TSelf max );

		// --- Static binary operations ---
		/// <summary>
		/// Adds two floating‑point values.
		/// </summary>
		static abstract TSelf Add ( TSelf a, TSelf b );

		/// <summary>
		/// Multiplies two floating‑point values.
		/// </summary>
		static abstract TSelf Mul ( TSelf a, TSelf b );

		/// <summary>
		/// Divides one floating‑point value by another.
		/// </summary>
		static abstract TSelf Div ( TSelf a, TSelf b );

		/// <summary>
		/// Returns the smaller of two floating‑point values.
		/// </summary>
		static abstract TSelf Min ( TSelf a, TSelf b );

		/// <summary>
		/// Returns the larger of two floating‑point values.
		/// </summary>
		static abstract TSelf Max ( TSelf a, TSelf b );
		// --- Static comparison ---
		/// <summary>
		/// Determines whether <paramref name="a"/> is strictly less than <paramref name="b"/>.
		/// </summary>
		static abstract bool operator < ( TSelf a, TSelf b );

		/// <summary>
		/// Determines whether <paramref name="a"/> is strictly greater than <paramref name="b"/>.
		/// </summary>
		static abstract bool operator > ( TSelf a, TSelf b );

		/// <summary>
		/// Determines whether <paramref name="a"/> is less than or equal to <paramref name="b"/>.
		/// </summary>
		static abstract bool operator <= ( TSelf a, TSelf b );

		/// <summary>
		/// Determines whether <paramref name="a"/> is greater than or equal to <paramref name="b"/>.
		/// </summary>
		static abstract bool operator >= ( TSelf a, TSelf b );

		/// <summary>
		/// Determines whether two floating‑point values are equal.
		/// </summary>
		static abstract bool operator == ( TSelf a, TSelf b );

		/// <summary>
		/// Determines whether two floating‑point values are not equal.
		/// </summary>
		static abstract bool operator != ( TSelf a, TSelf b );

	}
    /// @}
}
