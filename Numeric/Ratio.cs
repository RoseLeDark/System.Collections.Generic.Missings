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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SystemEx.Collections.Generic;
using SystemEx.Hash;
using SystemEx.Utils;

namespace SystemEx.Numeric {
	/// \addtogroup SystemEx.Numeric
	/// @{
	/// <summary>
	/// Represents a rational number using a signed 64‑bit numerator and denominator.
	/// <para>
	/// A Ratio stores an exact fractional value without automatic reduction.
	/// This allows the representation of structural fractions such as gear ratios
	/// (e.g., 20/60 is preserved and not simplified to 1/3).
	/// </para>
	///
	/// <para>
	/// The type provides full rational arithmetic, comparison operators, normalization,
	/// conversion to floating‑point types, and serialization support.  
	/// Negative denominators are normalized internally so that the denominator is always positive.
	/// </para>
	///
	/// <para>
	/// The struct is sequentially laid out for deterministic memory representation and
	/// supports hashing via the <see cref="HashAlgorithmAttribute"/>.
	/// </para>
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	[HashAlgorithm(typeof(BernsteinHash), Endian.System)]
	public struct Ratio : IEquatable<Ratio>, IComparable<Ratio>, IComparableEx<Ratio>, IComparer<Ratio>, IHashable<Ratio> {

		/// <summary>
		/// Represents the SI prefix atto (10^-18).
		/// </summary>
		public static Ratio Atto => new Ratio(1, 1000000000000000000);
		/// <summary>
		/// Represents the SI prefix femto (10^-15).
		/// </summary>
		public static Ratio Femto => new Ratio(1, 1000000000000000);
		/// <summary>
		/// Represents the SI prefix pico (10^-12).
		/// </summary>
		public static Ratio Pico => new Ratio(1, 1000000000000);
		/// <summary>
		/// Represents the SI prefix nano (10^-9).
		/// </summary>
		public static Ratio Nano => new Ratio(1, 1000000000);
		/// <summary>
		/// Represents the SI prefix micro (10^-6).
		/// </summary>
		public static Ratio Micro => new Ratio(1, 1000000);
		/// <summary>
		/// Represents the SI prefix milli (10^-3).
		/// </summary>
		public static Ratio Milli => new Ratio (1, 1000);
		/// <summary>
		/// Represents the SI prefix centi (10^-2).
		/// </summary>
		public static Ratio Centi => new Ratio(1, 100);
		/// <summary>
		/// Represents the SI prefix deci (10^-1).
		/// </summary>
		public static Ratio Deci => new Ratio(1, 10);
		/// <summary>
		/// Represents the SI prefix deca (10^1).
		/// </summary>
		public static Ratio Deca => new Ratio(10, 1);
		/// <summary>
		/// Represents the SI prefix hecto (10^2).
		/// </summary>
		public static Ratio Hecto => new Ratio(100, 1);
		/// <summary>
		/// Represents the SI prefix kilo (10^3).
		/// </summary>
		public static Ratio Kilo => new Ratio(1000, 1);
		/// <summary>
		/// Represents the SI prefix mega (10^6).
		/// </summary>
		public static Ratio Mega => new Ratio(1000000, 1);
		/// <summary>
		/// Represents the SI prefix giga (10^9).
		/// </summary>
		public static Ratio Giga => new Ratio(1000000000, 1);
		/// <summary>
		/// Represents the SI prefix tera (10^12).
		/// </summary>
		public static Ratio Tera => new Ratio(1000000000000, 1);
		/// <summary>
		/// Represents the SI prefix peta (10^15).
		/// </summary>
		public static Ratio Peta => new Ratio(1000000000000000, 1);

		/// <summary>
		/// Represents the SI prefix exa (10^18).
		/// </summary>
		public static Ratio Exa  => new Ratio(1000000000000000000, 1);

		/// <summary>
		/// Represents the ratio 1/1.
		/// </summary>
		public static Ratio One => new Ratio(1, 1);

		/// <summary>
		/// Represents the ratio 0/1.
		/// </summary>
		public static Ratio Zero => new Ratio(0, 1);

		/// <summary>
		/// The smallest representable ratio value, using <see cref="long.MinValue"/> as numerator.
		/// </summary>
		public static Ratio MinValue => new Ratio(long.MinValue, 1);

		/// <summary>
		/// The largest representable ratio value, using <see cref="long.MaxValue"/> as numerator.
		/// </summary>
		public static Ratio MaxValue => new Ratio(long.MaxValue, 1);

		/// <summary>
		/// Represents the ratio 1/2.
		/// </summary>
		public static Ratio Half => new Ratio(1, 2);

		/// <summary>
		/// Represents the ratio 1/3.
		/// </summary>
		public static Ratio Third => new Ratio(1, 3);

		/// <summary>
		/// Represents the ratio 2/3.
		/// </summary>
		public static Ratio TwoThirds => new Ratio(2, 3);

		/// <summary>
		/// Represents the ratio 1/4.
		/// </summary>
		public static Ratio Quarter => new Ratio(1, 4);

		/// <summary>
		/// Represents the ratio 3/4.
		/// </summary>
		public static Ratio ThreeQuarters => new Ratio(3, 4);

		/// <summary>
		/// Represents the ratio 1/8.
		/// </summary>
		public static Ratio Eighth => new Ratio(1, 8);

		/// <summary>
		/// Represents the ratio 1/16.
		/// </summary>
		public static Ratio Sixteenth => new Ratio(1, 16);

		/// <summary>
		/// Represents the ratio 1/10.
		/// </summary>
		public static Ratio Tenth => new Ratio(1, 10);

		/// <summary>
		/// Represents the ratio 1/5.
		/// </summary>
		public static Ratio Fifth => new Ratio(1, 5);

		/// <summary>
		/// Represents the ratio 2/5.
		/// </summary>
		public static Ratio TwoFifths => new Ratio(2, 5);

		/// <summary>
		/// Represents the ratio 3/5.
		/// </summary>
		public static Ratio ThreeFifths => new Ratio(3, 5);

		/// <summary>
		/// Represents the ratio 4/5.
		/// </summary>
		public static Ratio FourFifths => new Ratio(4, 5);

		private long m_numerator;
		private long m_denominator;

		/// <summary>
		/// Gets or sets the numerator of the ratio.
		/// </summary>
		public long Numerator {
			get => m_numerator;
			set => m_numerator = value;
		}

		/// <summary>
		/// Gets or sets the denominator of the ratio.
		/// 
		/// <para>
		/// A value of zero is not allowed and throws <see cref="DivideByZeroException"/>.
		/// Negative denominators are normalized by flipping the sign of both numerator and denominator.
		/// </para>
		/// </summary>
		public long Denominator {
			get => m_denominator;
			set {
				if ( value == 0 ) throw new DivideByZeroException();
				if ( value < 0 ) { m_numerator = -m_numerator; m_denominator = -value; } 
				else m_denominator = value;
			}
		}


		/// <summary>
		/// Gets whether the ratio is positive (denominator &gt; 0).
		/// </summary>
		public bool IsPositive => m_denominator > 0;

		/// <summary>
		/// Gets whether the ratio is negative (denominator &lt; 0).
		/// </summary>
		public bool IsNegative => m_denominator < 0;

		/// <summary>
		/// Gets whether the ratio equals zero.
		/// </summary>
		public bool IsZero => m_numerator == 0;

		/// <summary>
		/// Gets whether the ratio is a non‑integer rational number (denominator != 1).
		/// </summary>
		public bool IsRational => m_denominator != 1;

		/// <summary>
		/// Gets whether the ratio represents an integer value (denominator == 1).
		/// </summary>
		public bool IsInteger => m_denominator == 1;


		/// <summary>
		/// Initializes a new ratio with the value 1/1.
		/// </summary>
		public Ratio (  ) {
			m_numerator = 1;
			m_denominator = 1;
		}


		/// <summary>
		/// Initializes a new ratio with the specified numerator and denominator.
		/// </summary>
		/// <param name="num">The numerator.</param>
		/// <param name="den">
		/// The denominator.  
		/// Must not be zero.  
		/// Negative denominators are normalized.
		/// </param>
		/// <exception cref="DivideByZeroException">
		/// Thrown when <paramref name="den"/> is zero.
		/// </exception>
		public Ratio ( long num, long den ) {
			if ( den == 0 ) throw new DivideByZeroException();
			if ( den < 0 ) { num = -num; den = -den; }

			m_numerator = num ;
			m_denominator = den ;
		}
		/// <summary>
		/// Converts the ratio to a <see cref="double"/>.
		/// </summary>
		public double ToDouble () {
			return (((double)m_numerator) / m_denominator) ;
		}
		/// <summary>
		/// Converts the ratio to a <see cref="decimal"/>.
		/// </summary>
		public decimal ToDecimal () {
			return (((decimal)m_numerator) / m_denominator) ;
		}
		/// <summary>
		/// Returns the multiplicative inverse of this ratio (denominator/numerator).
		/// </summary>
		public Ratio Inverse () {
			return new Ratio(m_denominator, m_numerator);
		}
		/// <summary>
		/// Returns a normalized version of this ratio by dividing numerator and denominator
		/// by their greatest common divisor.
		/// </summary>
		/// <returns>A reduced ratio with the same value.</returns>
		public Ratio Normalize () {
			long gcd = GreatestCommonDivisor(System.Math.Abs(m_numerator), m_denominator);

			return new Ratio(m_numerator / gcd, m_denominator / gcd);
		}


		


		/// <summary>
		/// Computes the greatest common divisor (GCD) of two positive integers.
		/// </summary>
		private static long GreatestCommonDivisor ( long a, long b ) {
			long temp;

			if ( a > b ) {
				temp = b;
				b = a;
				a = temp;
			}

			while ( b != 0 ) {
				temp = a % b;
				a = b;
				b = temp;
			}

			return a;
		}
		// <summary>
		/// Computes the least common multiple (LCM) of two integers.
		/// </summary>
		private long LeastCommonDivisor ( long a, long b ) => a * (b / GreatestCommonDivisor(a, b) );

		/// <summary>
		/// Adds two ratios.
		/// </summary>
		public static Ratio operator + ( Ratio a, Ratio b ) =>
			new Ratio(a.m_numerator * b.m_denominator +  b.m_numerator * a.m_denominator, 
				a.m_denominator * b.m_denominator);

		/// <summary>
		/// Multiplies two ratios.
		/// </summary>
		public static Ratio operator * ( Ratio a, Ratio b ) =>
			new Ratio( a.m_numerator * b.m_numerator, 
				a.m_denominator * b.m_denominator);

		/// <summary>
		/// Divides one ratio by another.
		/// </summary>
		public static Ratio operator / ( Ratio a, Ratio b ) =>
			 a * b.Inverse ();

		/// <summary>
		/// Subtracts one ratio from another.
		/// </summary>
		public static Ratio operator - ( Ratio a, Ratio b ) {
			return a + -b;
		}

		/// <summary>
		/// Negates the ratio.
		/// </summary>
		public static Ratio operator - ( Ratio a ) {
			return new Ratio(-1 * a.m_numerator, a.m_denominator);
		}

		/// <summary>
		/// Returns the ratio unchanged.
		/// </summary>
		public static Ratio operator + ( Ratio a ) {
			return a;
		}

		/// <summary>
		/// Determines whether two ratios are exactly equal.
		/// </summary>
		public static bool operator == (Ratio a, Ratio b) {
			return a.Denominator == b.Denominator && a.Numerator == b.Numerator;
		}

		/// <summary>
		/// Determines whether two ratios differ.
		/// </summary>
		public static bool operator != ( Ratio a, Ratio b ) {
			return !(a == b);
		}
		/// <summary>
		/// Determines whether one ratio is strictly less than another.
		/// </summary>
		public static bool operator < ( Ratio a, Ratio b ) {
			return (a.Numerator * b.Denominator) < (b.Numerator * a.Denominator);
		}
		/// <summary>
		/// Determines whether one ratio is less than or equal to another.
		/// </summary>
		public static bool operator <= ( Ratio a, Ratio b ) {
			return !(b < a);
		}
		/// <summary>
		/// Determines whether one ratio is greater than or equal to another.
		/// </summary>
		public static bool operator >= ( Ratio a, Ratio b ) {
			return !(a < b);
		}
		/// <summary>
		/// Determines whether one ratio is strictly greater than another.
		/// </summary>
		public static bool operator > ( Ratio a, Ratio b ) {
			return b < a;
		}
		/// <summary>
		/// Converts the ratio to a string in the form "numerator/denominator".
		/// If the denominator is 1, only the numerator is returned.
		/// </summary>
		public override string ToString () {
			if ( Denominator != 1 )
				return string.Format("{0}/{1}", Numerator, Denominator);
			return "" + Numerator;
		}
		/// <summary>
		/// Determines whether this ratio is equal to another ratio.
		/// </summary>
		public bool Equals ( Ratio other ) {
			
			return this == other;
		}

		/// <inheritdoc/>
		public override bool Equals ( object? obj ) {
			if ( obj is Ratio fr ) {
				return Equals(fr);
			}

			return false;
		}

		/// <summary>
		/// Converts an integer to a ratio (n/1).
		/// </summary>
		public static implicit operator Ratio ( int n ) {
			return new Ratio(n, 1);
		}

		/// <summary>
		/// Converts a ratio to a <see cref="double"/>.
		/// </summary>
		public static implicit operator double ( Ratio f ) {
			return f.ToDouble();
		}

		/// <summary>
		/// Converts a ratio to a <see cref="decimal"/>.
		/// </summary>
		public static implicit operator decimal ( Ratio f ) {
			return f.ToDecimal();
		}

		/// <summary>
		/// Compares this ratio to another ratio.
		/// </summary>
		public CompareResult CompareTo ( Ratio a ) {
			return InternalCompare(this, a);
		}

		/// <summary>
		/// Compares two ratios.
		/// </summary>
		public int Compare ( Ratio x, Ratio y ) {
			return (int)InternalCompare(x, y);
		}

		int IComparable<Ratio>.CompareTo ( Ratio other ) {
			return (int)InternalCompare(this, other);
		}
		/// <summary>
		/// Internal comparison helper.
		/// </summary>
		private CompareResult InternalCompare( Ratio x, Ratio y ) =>
			(x > y) ? CompareResult.Greater : (x < y) ? CompareResult.Less : CompareResult.Equal;

		/// <inheritdoc/>
		public FixedVector<byte> ToBytes () {
			Cache m = new Cache(sizeof(long) * 2);

			m.WriteRange(0, m_numerator.ToBytes() );
			m.WriteRange(8, m_denominator.ToBytes());

			return m.ToArrayEx();

		}
		/// <summary>
		/// Computes a hash code for this object.
		///
		/// <para>
		/// The primary hash is generated using <see cref="HashFactory"/> and the
		/// <see cref="HashAlgorithmAttribute"/> applied to this struct.  
		/// If hashing fails (rare), a fallback XOR‑based hash is used.
		/// </para>
		/// </summary>
		public override int GetHashCode () {
			var x =  HashFactory.Hash32(this, 674545);
			if ( x.Value != 0 ) return (int)x.Value;

			return m_numerator.GetHashCode() ^ m_denominator.GetHashCode();
		}

		//@}
	}
}
