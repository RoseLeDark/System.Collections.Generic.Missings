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

using SystemEx.Algorithms.Interfaces;
using SystemEx.Collections.Generic;
using SystemEx.Numeric;

namespace SystemEx {
	/// <summary>
	/// Provides exact rational mathematics for the <see cref="Ratio"/> type,
	/// following the structural pattern of <see cref="System.MathF"/> but without
	/// any floating‑point operations or irrational functions.
	/// </summary>
	public static class MathR {

		/// <summary>
		/// Raises a ratio to an integer power exactly.
		/// <para>
		/// Negative exponents invert the ratio before exponentiation.
		/// </para>
		/// </summary>
		public static Ratio Pow ( Ratio r, int exp ) {
			if ( exp == 0 ) return Ratio.One;
			if ( exp < 0 ) return Pow(r.Inverse(), -exp);

			long num = 1;
			long den = 1;

			for ( int i = 0 ; i < exp ; i++ ) {
				num *= r.Numerator;
				den *= r.Denominator;
			}

			return new Ratio(num, den);
		}

		/// <summary>
		/// Converts degrees to radians.
		/// </summary>
		public static Ratio DegreesToRadians ( Ratio deg ) {
			return new Ratio(deg.Numerator, deg.Denominator) *
				   new Ratio(3141592653589793, 1000000000000000000); // π approx
		}

		/// <summary>
		/// Converts radians to degrees.
		/// </summary>
		public static Ratio RadiansToDegrees ( Ratio rad ) {
			return rad * new Ratio(180, 1) /
				   new Ratio(3141592653589793, 1000000000000000000); // π approx
		}

		/// <summary>
		/// Converts degrees to turns (full rotations).
		/// </summary>
		public static Ratio DegreesToTurns ( Ratio deg ) {
			return deg / 360;
		}

		/// <summary>
		/// Converts turns (full rotations) to degrees.
		/// </summary>
		public static Ratio TurnsToDegrees ( Ratio turns ) {
			return turns * 360;
		}

		/// <summary>
		/// Returns the sign of this ratio as a interger.
		/// 
		/// <para>
		/// The result is -1, 0, or 1 depending on the value of the ratio.
		/// </para>
		/// </summary>
		public static int Sign ( Ratio value ) {
			if ( value.Numerator == 0 ) return 0;
			return (value.Numerator > 0) ? 1 : -1;
		}

		/// <summary>
		/// Returns the absolute value of the giving ratio 
		/// <see cref="System.Math.Abs(long)"/> to both numerator and denominator.
		/// 
		/// </summary>
		/// <returns>
		/// A new <see cref="Ratio"/> whose numerator and denominator are non‑negative.
		/// </returns>
		public static Ratio Abs (Ratio value) {
			return new Ratio(
				System.Math.Abs(value.Numerator),
				System.Math.Abs(value.Denominator)
			);
		}

		/// <summary>
		/// Clamps this ratio between the specified minimum and maximum values.
		/// </summary>
		public static Ratio Clamp ( Ratio value, Ratio min, Ratio max ) {
			if ( value < min ) return min;
			if ( value > max ) return max;
			return value;
		}

		/// <summary>
		/// Clamps this ratio between the specified minimum and maximum values.
		/// </summary>
		public static Ratio Clamp ( Ratio value, Ratio min, Ratio max, ISimpleCompare<Ratio> cmp ) {
			return cmp.Compare(value, min) ? min : cmp.Compare(max, value) ? max : value;
		}

		/// <summary>
		/// Returns the smaller of two ratios.
		/// </summary>
		public static Ratio Min ( Ratio a, Ratio b )
		{
			return (b < a) ? b : a;
		}

		/// <summary>
		/// Returns the smaller of two ratios using a custom comparison strategy.
		/// </summary>
		public static Ratio Min ( Ratio a, Ratio b, ISimpleCompare<Ratio> cmp ) {
			return (cmp.Compare(b, a)) ? b : a;
		}

		/// <summary>
		/// Returns the larger of two ratios.
		/// </summary>
		public static Ratio Max ( Ratio a, Ratio b ) {
			return (a < b) ? b : a;
		}

		/// <summary>
		/// Returns the larger of two ratios using a custom comparison strategy.
		/// </summary>
		public static Ratio Max ( Ratio a, Ratio b, ISimpleCompare<Ratio> cmp ) {
			return (cmp.Compare(a, b)) ? b : a;
		}

		/// <summary>
		/// Returns both the minimum and maximum of two ratios using a custom comparison strategy.
		/// </summary>
		public static Pair<Ratio, Ratio> MinMax( Ratio a, Ratio b, ISimpleCompare<Ratio> cmp ) {
			return (cmp.Compare(b, a))	? new Pair<Ratio, Ratio>(b, a) 
										: new Pair<Ratio, Ratio>(a, b);
		}
		/// <summary>
		/// Returns both the minimum and maximum of two ratios.
		/// </summary>
		public static Pair<Ratio, Ratio> MinMax ( Ratio a, Ratio b ) {
			return (b < a)	? new Pair<Ratio, Ratio>(b, a)
							: new Pair<Ratio, Ratio>(a, b);
		}

		/// <summary>
		/// Computes the square root of a <see cref="Ratio"/> and returns the result
		/// inside a <see cref="Result"/> object. The method distinguishes between
		/// rational and irrational square roots:
		///
		/// <para>
		/// <b>• Rational root:</b><br/>
		/// If both the numerator and denominator are perfect squares, the result is
		/// returned as a new <see cref="Ratio"/> instance stored at index <c>[0]</c>.
		/// </para>
		///
		/// <para>
		/// <b>• Irrational root:</b><br/>
		/// If either part is not a perfect square, the method returns two
		/// <see cref="double"/> values representing <c>sqrt(numerator)</c> and
		/// <c>sqrt(denominator)</c>, stored at indices <c>[0]</c> and <c>[1]</c>.
		/// </para>
		///
		/// <para>
		/// <b>• Exception handling:</b><br/>
		/// Any thrown exception is captured using <see cref="Result.Catch(Exception)"/>
		/// and returned as part of the <see cref="Result"/> object.
		/// </para>
		/// </summary>
		/// <param name="value">The rational number whose square root is computed.</param>
		/// <returns>
		/// A <see cref="Result"/> containing either:
		/// <list type="bullet">
		/// <item><description>A rational square root (<see cref="Ratio"/>)</description></item>
		/// <item><description>Two <see cref="double"/> values for irrational roots</description></item>
		/// <item><description>An exception if an error occurred</description></item>
		/// </list>
		/// </returns>

		public static Result Sqrt ( Ratio value ) {
			Result _ret = new Result();

			try {
				var _num = System.Math.Sqrt(value.Numerator);
				var _dem = System.Math.Sqrt(value.Denominator);

				if (IsSquareble(value)) {

					_ret[0] = new Ratio( (long)_num , (long)_dem);

				} else {

					_ret[0] = _num;
					_ret[1] = _dem;

				}
			} catch ( Exception  ex) {
				_ret.Catch(ex);
			}
			return _ret;
		}

		/// <summary>
		/// Determines whether the square root of a <see cref="Ratio"/> is rational.
		/// A ratio is squareable if both its numerator and denominator are perfect
		/// squares.
		/// </summary>
		/// <param name="value">The ratio to test.</param>
		/// <returns>
		/// <c>true</c> if both numerator and denominator are perfect squares;
		/// otherwise <c>false</c>.
		/// </returns>
		public static bool IsSquareble(Ratio value)  {
			var _a = (long)System.Math.Sqrt(value.Numerator);
			var _b = (long)System.Math.Sqrt(value.Denominator);

			return (_a * _a == value.Numerator && _b * _b == value.Denominator);

		}
	}
}
