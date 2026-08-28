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
using SystemEx.Algorithms;
using SystemEx.Collections.Generic;

namespace SystemEx.Numeric {
	public partial struct BigDecimal {

		/// <summary>
		/// Specifies the rounding strategy used by <see cref="BigDecimal.Round(int, RoundMode)"/>.
		/// These modes correspond to common numerical rounding conventions used in
		/// scientific, financial, and engineering computations.
		/// </summary>
		public enum RoundMode {

			/// <summary>
			/// Round to the nearest value; if equidistant, round to the nearest even number.
			/// Also known as “banker's rounding”.
			/// </summary>
			ToEven,

			/// <summary>
			/// Round away from zero, increasing magnitude regardless of sign.
			/// </summary>
			AwayFromZero,

			/// <summary>
			/// Round toward zero, decreasing magnitude regardless of sign.
			/// </summary>
			TowardZero,

			/// <summary>
			/// Round toward negative infinity. Equivalent to mathematical floor.
			/// </summary>
			Floor,

			/// <summary>
			/// Round toward positive infinity. Equivalent to mathematical ceiling.
			/// </summary>
			Ceiling
		}

		/// <summary>Returns the greater of this value and <paramref name="other"/> (numeric comparison).</summary>
		/// <param name="other">The value to compare against.</param>
		/// <returns>This value if it is greater than or equal to <paramref name="other"/>; otherwise <paramref name="other"/>.</returns>
		/// <remarks>This mirrors Java's <c>BigDecimal.max</c>.</remarks>
		public BigDecimal Max ( BigDecimal other ) => CompareTo(other) >= 0 ? this : other;

		/// <summary>Returns the lesser of this value and <paramref name="other"/> (numeric comparison).</summary>
		/// <param name="other">The value to compare against.</param>
		/// <returns>This value if it is less than or equal to <paramref name="other"/>; otherwise <paramref name="other"/>.</returns>
		/// <remarks>This mirrors Java's <c>BigDecimal.min</c>.</remarks>
		public BigDecimal Min ( BigDecimal other ) => CompareTo(other) <= 0 ? this : other;


		/// <summary>
		/// Returns a pair containing the minimum and maximum of this value and
		/// <paramref name="other"/> using the default ascending numeric comparison.
		/// </summary>
		/// <param name="other">The value to compare against.</param>
		/// <returns>
		/// A <see cref="Pair{T1,T2}"/> where the first element is the smaller value
		/// and the second element is the larger value.
		/// </returns>
		/// <remarks>
		/// This overload uses <see cref="Less{T}"/> as the default comparison strategy,
		/// equivalent to standard numeric “less‑than” ordering.
		/// </remarks>
		public Pair<BigDecimal, BigDecimal> MinMax(BigDecimal other) {
			
			return MinMax(other, new Less<BigDecimal>());
		}

		
		/// <summary>
		/// Returns a pair containing the minimum and maximum of this value and
		/// <paramref name="other"/> using a custom comparison strategy.
		/// </summary>
		/// <param name="other">The value to compare against.</param>
		/// <param name="cmp">
		/// A comparison strategy implementing <see cref="ISimpleCompare{T}"/> used
		/// to determine ordering between the two values.
		/// </param>
		/// <returns>
		/// A <see cref="Pair{T1,T2}"/> where the first element is the smaller value
		/// and the second element is the larger value, according to the provided
		/// comparison strategy.
		/// </returns>
		/// <remarks>
		/// This overload allows callers to supply domain‑specific comparison logic,
		/// such as custom ordering rules or alternative “less‑than” semantics.
		/// </remarks>
		public Pair<BigDecimal, BigDecimal> MinMax ( BigDecimal other , ISimpleCompare<BigDecimal> cmp) {
			return cmp.Compare(this, other) ?
				new Pair<BigDecimal, BigDecimal>(other, this) :
				new Pair<BigDecimal, BigDecimal>(this, other);
		}

		/// <summary>
		/// Adds two <see cref="BigDecimal"/> values with exact precision by aligning
		/// their exponents and summing their mantissas.
		/// </summary>
		public static BigDecimal operator + ( BigDecimal a, BigDecimal b ) {
			// If one is zero → fast path
			if ( a.IsZero ) return b;
			if ( b.IsZero ) return a;

			// Align exponents
			if ( a.Exponent == b.Exponent )
				return new BigDecimal(a.Mantissa + b.Mantissa, a.Exponent).Normalize();

			if ( a.Exponent > b.Exponent ) {
				int diff = a.Exponent - b.Exponent;
				BigInteger bm = b.Mantissa * PowerOfTen(diff);
				return new BigDecimal(a.Mantissa + bm, b.Exponent).Normalize();
			} else {
				int diff = b.Exponent - a.Exponent;
				BigInteger am = a.Mantissa * PowerOfTen(diff);
				return new BigDecimal(am + b.Mantissa, a.Exponent).Normalize();
			}
		}

		/// <summary>
		/// Subtracts two <see cref="BigDecimal"/> values with exact precision.
		/// </summary>
		public static BigDecimal operator - ( BigDecimal a, BigDecimal b ) {
			return a + (-b);
		}

		/// <summary>
		/// Returns the negated value of this <see cref="BigDecimal"/>.
		/// </summary>
		public static BigDecimal operator - ( BigDecimal a ) {
			return new BigDecimal(-a.Mantissa, a.Exponent);
		}

		/// <summary>
		/// Return the same value
		/// </summary>
		public static BigDecimal operator + ( BigDecimal a ) {
			return new BigDecimal(a.Mantissa, a.Exponent);
		}

		/// <summary>
		/// Multiplies two <see cref="BigDecimal"/> values with exact precision.
		/// Mantissas are multiplied and exponents are added.
		/// </summary>
		public static BigDecimal operator * ( BigDecimal a, BigDecimal b ) {
			if ( a.IsZero || b.IsZero )
				return Zero;

			BigInteger m = a.Mantissa * b.Mantissa;
			int e = a.Exponent + b.Exponent;

			return new BigDecimal(m, e).Normalize();
		}

		/// <summary>
		/// Divides two <see cref="BigDecimal"/> values with exact precision.
		/// If the division is not exact, an <see cref="ArithmeticException"/> is thrown.
		/// </summary>
		public static BigDecimal operator / ( BigDecimal a, BigDecimal b ) {
			if ( b.IsZero )
				throw new DivideByZeroException();

			if ( a.IsZero )
				return Zero;

			// Adjust mantissas
			BigInteger am = a.Mantissa;
			BigInteger bm = b.Mantissa;

			// Try exact division
			BigInteger q = BigInteger.DivRem(am, bm, out BigInteger r);

			if ( !r.IsZero )
				throw new ArithmeticException("Non-exact division in BigDecimal.");

			int e = a.Exponent - b.Exponent;

			return new BigDecimal(q, e).Normalize();
		}

		/// <summary>
		/// Computes the modulo of two <see cref="BigDecimal"/> values.
		/// Exponents must match; otherwise the operation is undefined.
		/// </summary>
		public static BigDecimal operator % ( BigDecimal a, BigDecimal b ) {
			if ( a.Exponent != b.Exponent )
				throw new ArithmeticException("Modulo requires equal exponents.");

			BigInteger r = a.Mantissa % b.Mantissa;
			return new BigDecimal(r, a.Exponent).Normalize();
		}

		/// <summary>
		/// Raises a <see cref="BigDecimal"/> to an integer power.
		/// </summary>
		public static BigDecimal Pow ( BigDecimal value, int power ) {
			if ( power == 0 )
				return One;

			if ( power < 0 )
				throw new NotSupportedException("Negative powers not supported yet.");

			BigInteger m = BigInteger.Pow(value.Mantissa, power);
			int e = value.Exponent * power;

			return new BigDecimal(m, e).Normalize();
		}


		/// <summary>
		/// Returns the absolute value of this <see cref="BigDecimal"/>.
		/// </summary>
		public BigDecimal Abs () {
			return new BigDecimal(BigInteger.Abs(Mantissa), Exponent);
		}

		/// <summary>
		/// Returns -1, 0, or +1 depending on the sign of the value.
		/// </summary>
		public int Signum () {
			return Mantissa.Sign;
		}

		/// <summary>
		/// Multiplies the value by 10^n without modifying the mantissa.
		/// </summary>
		public BigDecimal ShiftLeft ( int n ) {
			return new BigDecimal(Mantissa, Exponent + n);
		}

		/// <summary>
		/// Multiplies the value by 10^n without modifying the mantissa.
		/// </summary>
		public BigDecimal ShifRight( int n ) {
			return new BigDecimal(Mantissa, Exponent - n);
		}

		/// <summary>Clamps <paramref name="value"/> to the inclusive range [<paramref name="min"/>, <paramref name="max"/>].</summary>
		public static BigDecimal Clamp ( BigDecimal value, BigDecimal min, BigDecimal max ) {
			if ( min.CompareTo(max) > 0 )
				throw new ArgumentException("min cannot be greater than max.", nameof(min));

			if ( value.CompareTo(min) < 0 )
				return min;
			if ( value.CompareTo(max) > 0 )
				return max;
			return value;
		}

		/// <summary>
		/// Computes the natural logarithm (ln) of this <see cref="BigDecimal"/>.
		/// Uses IEEE‑754 double precision internally.
		/// </summary>
		public BigDecimal Log () {
			if ( IsZero || IsNegative )
				throw new ArithmeticException("Log undefined for zero or negative values.");

			double d = ToDouble();
			return System.Math.Log(d);
		}

		/// <summary>
		/// Computes e^x for this <see cref="BigDecimal"/>.
		/// </summary>
		public BigDecimal Exp () {
			double d = ToDouble();
			return System.Math.Exp(d);
		}
		/// <summary>
		/// Computes the square root of this <see cref="BigDecimal"/>.
		/// </summary>
		public BigDecimal Sqrt () {
			if ( IsNegative )
				throw new ArithmeticException("Sqrt undefined for negative values.");

			double d = ToDouble();
			return System.Math.Sqrt(d);
		}

		/// <summary>
		/// Raises a <see cref="BigDecimal"/> to a <see cref="BigDecimal"/> power.
		/// Uses scientific functions internally.
		/// </summary>
		public static BigDecimal Pow ( BigDecimal x, BigDecimal y ) {
			if ( x.IsZero )
				return Zero;

			double xd = x.ToDouble();
			double yd = y.ToDouble();

			return System.Math.Pow(xd, yd);
		}

		/// <summary>
		/// Computes the sine of this <see cref="BigDecimal"/> using IEEE‑754 double
		/// precision internally. The result is converted back into a BigDecimal.
		/// </summary>
		public BigDecimal Sin () {
			return System.Math.Sin(ToDouble());
		}

		/// <summary>
		/// Computes the cosine of this <see cref="BigDecimal"/>.
		/// </summary>
		public BigDecimal Cos () {
			return System.Math.Cos(ToDouble());
		}

		/// <summary>
		/// Computes the tangent of this <see cref="BigDecimal"/>.
		/// </summary>
		public BigDecimal Tan () {
			return System.Math.Tan(ToDouble());
		}

		/// <summary>
		/// Computes the hyperbolic sine (sinh) of this BigDecimal.
		/// </summary>
		public BigDecimal Sinh () {
			return System.Math.Sinh(ToDouble());
		}

		/// <summary>
		/// Computes the hyperbolic cosine (cosh).
		/// </summary>
		public BigDecimal Cosh () {
			return System.Math.Cosh(ToDouble());
		}

		/// <summary>
		/// Computes the hyperbolic tangent (tanh).
		/// </summary>
		public BigDecimal Tanh () {
			return System.Math.Tanh(ToDouble());
		}

		/// <summary>
		/// Computes the base‑10 logarithm (log10).
		/// </summary>
		public BigDecimal Log10 () {
			return System.Math.Log10(ToDouble());
		}

		/// <summary>
		/// Computes the base‑2 logarithm (log2).
		/// </summary>
		public BigDecimal Log2 () {
			return System.Math.Log(ToDouble(), 2.0);
		}

		/// <summary>
		/// Computes 10^x for this BigDecimal.
		/// </summary>
		public BigDecimal Exp10 () {
			return System.Math.Pow(10.0, ToDouble());
		}

		/// <summary>
		/// Computes 2^x for this BigDecimal.
		/// </summary>
		public BigDecimal Exp2 () {
			return System.Math.Pow(2.0, ToDouble());
		}

		/// <summary>
		/// Computes the arcsine (inverse sine) using IEEE‑754 double precision.
		/// </summary>
		public BigDecimal Asin () {
			return System.Math.Asin(ToDouble());
		}

		/// <summary>
		/// Computes the inverse hyperbolic tangent (atanh).
		/// </summary>
		public BigDecimal Atanh ( double d ) {
			return System.Math.Asin(ToDouble()); // (Note: implementation appears incorrect)
		}

		/// <summary>
		/// Computes the two‑argument arctangent using IEEE‑754 double precision.
		/// </summary>
		public BigDecimal Atan2 ( BigDecimal x ) {
			return System.Math.Atan2(ToDouble(), (double)x);
		}

		private static BigInteger PowerOfTen ( long n ) {
			if ( n == 0 ) return BigInteger.One;
			if ( n < 0 ) throw new ArgumentOutOfRangeException(nameof(n), "Exponent must be non-negative.");

			// Efficient: use BigInteger.Pow
			return BigInteger.Pow(10, (int)n);
		}
		private static int CheckExponentRange ( long n ) {
			if ( n < 0 )
				throw new ArgumentOutOfRangeException(nameof(n), "Exponent difference must not be negative.");

			if ( n > int.MaxValue )
				throw new OverflowException("Exponent difference too large for BigInteger.Pow.");

			return (int)n;
		}

		/// <summary>
		/// Rounds this <see cref="BigDecimal"/> to a specified number of decimal digits.
		/// Uses BigInteger arithmetic for exact rounding without floating‑point errors.
		/// </summary>
		/// <param name="digits">Number of decimal digits to keep.</param>
		/// <param name="mode">Rounding mode.</param>
		public BigDecimal Round ( int digits, RoundMode mode ) {
			// If exponent >= digits → already exact
			if ( Exponent >= digits )
				return this;

			int shift = digits - Exponent;

			// Shift mantissa right: divide by 10^shift
			BigInteger pow = PowerOfTen(shift);

			BigInteger div = BigInteger.DivRem(Mantissa, pow, out BigInteger rem);

			if ( rem.IsZero )
				return new BigDecimal(div, digits).Normalize();

			bool negative = Mantissa.Sign < 0;
			BigInteger absRem = BigInteger.Abs(rem);
			BigInteger half = pow >> 1; // 10^shift / 2

			bool roundUp = false;

			switch ( mode ) {
			case BigDecimal.RoundMode.ToEven:
			if ( absRem > half ) roundUp = true;
			else if ( absRem == half && !div.IsEven ) roundUp = true;
			break;

			case BigDecimal.RoundMode.AwayFromZero:
			roundUp = true;
			break;

			case BigDecimal.RoundMode.TowardZero:
			roundUp = false;
			break;

			case BigDecimal.RoundMode.Floor:
			if ( negative ) roundUp = true;
			break;

			case BigDecimal.RoundMode.Ceiling:
			if ( !negative ) roundUp = true;
			break;
			}

			if ( roundUp )
				div += negative ? -1 : 1;

			return new BigDecimal(div, digits).Normalize();
		}
		/// <summary>
		/// Rounds this value to the nearest integer using the specified rounding mode.
		/// </summary>
		public BigDecimal RoundToInteger ( RoundMode mode = RoundMode.ToEven ) {
			return Round(0, mode);
		}

		/// <summary>
		/// Rounds this value to the specified decimal exponent using the given rounding mode.
		/// </summary>
		public BigDecimal RoundToExponent ( int exp, RoundMode mode = RoundMode.ToEven ) {
			return Round(exp, mode);
		}

	
	}
}
