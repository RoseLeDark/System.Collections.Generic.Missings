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

namespace SystemEx.Numeric {
	public partial struct  BigDecimal {
		/// <summary>
		/// Converts this <see cref="BigDecimal"/> into an IEEE‑754 <see cref="double"/>
		/// using mathematically correct scaling. The conversion applies exact
		/// base‑10 to base‑2 exponent transformation and performs a precise
		/// mantissa adjustment. 
		///
		/// <para>
		/// If the resulting value exceeds the representable range of
		/// <see cref="double"/>, an <see cref="OverflowException"/> is thrown.
		/// </para>
		/// </summary>
		public double ToDouble () {
			if ( IsZero ) return 0.0;

			const double LOG2_10 = 3.32192809488736234787;

			double man = (double)Mantissa;
			double exp2 = Exponent * LOG2_10;

			double result = man * System.Math.Pow(2.0, exp2);

			if ( double.IsInfinity(result) )
				throw new OverflowException("BigDecimal too large for double.");

			return result;
		}

		/// <summary>
		/// Implicitly converts an IEEE‑754 <see cref="double"/> into a
		/// <see cref="BigDecimal"/> by reconstructing the binary exponent and
		/// significand. This conversion is exact and does not introduce
		/// floating‑point rounding artifacts beyond those already present in
		/// the input <see cref="double"/>.
		///
		/// <para>
		/// NaN and Infinity cannot be represented as <see cref="BigDecimal"/>
		/// and will cause an <see cref="OverflowException"/>.
		/// </para>
		/// </summary>
		public static implicit operator BigDecimal ( double value ) {
			if ( double.IsNaN(value) || double.IsInfinity(value) )
				throw new OverflowException("Cannot convert NaN or Infinity to a BigDecimal.");

			BigDecimal _ret = Zero;

			if ( value != 0.0 ) {
				long _raw = value.ToBytes(Endian.System).ToLong(Endian.System);

				int _e = (int)((_raw >> 52) & 0x7FF);
				long _man = _raw & 0xFFFFFFFFFFFFFL;

				// Normalize subnormal numbers
				if ( _e == 0 ) _e++;
				else _man |= 0x10000000000000L;

				// Convert exponent from biased IEEE‑754 to unbiased binary exponent
				_e -= 1075;

				BigInteger significand = new BigInteger(_man);
				if ( _raw < 0 )
					significand = -significand;

				// Convert binary exponent to decimal exponent
				if ( _e >= 0 ) {
					_ret = new BigDecimal(significand * BigInteger.Pow(new BigInteger(2), _e), 0);
				} else {
					var _uns = significand * BigInteger.Pow(new BigInteger(5), -_e);
					_ret = new BigDecimal(_uns, -_e);
				}
			}

			return _ret;
		}

		/// <summary>
		/// Implicitly converts a 32‑bit signed integer into a <see cref="BigDecimal"/>.
		/// </summary>
		public static implicit operator BigDecimal ( int value ) => new BigDecimal(value);

		/// <summary>
		/// Implicitly converts a 64‑bit signed integer into a <see cref="BigDecimal"/>.
		/// </summary>
		public static implicit operator BigDecimal ( long value ) => new BigDecimal(value);

		/// <summary>
		/// Implicitly converts a 64‑bit unsigned integer into a <see cref="BigDecimal"/>.
		/// </summary>
		public static implicit operator BigDecimal ( ulong value ) => new BigDecimal(new BigInteger(value), 0);

		/// <summary>
		/// Implicitly converts a <see cref="BigInteger"/> into a <see cref="BigDecimal"/>.
		/// </summary>
		public static implicit operator BigDecimal ( BigInteger value ) => new BigDecimal(value, 0);

		/// <summary>
		/// Explicitly converts a <see cref="BigDecimal"/> into a <see cref="BigInteger"/>.
		/// The conversion is only valid if the decimal exponent allows an exact
		/// integer representation. If the value contains fractional components,
		/// an <see cref="ArithmeticException"/> is thrown.
		///
		/// <para>
		/// Extremely large exponent values may cause an <see cref="OverflowException"/>.
		/// </para>
		/// </summary>
		public static explicit operator BigInteger ( BigDecimal value ) {
			BigInteger _ret = BigInteger.Zero;

			int _exp = value.Exponent;
			var _man = value.Mantissa;

			if ( _exp == 0 ) {
				_ret = BigInteger.Zero;
			} else if ( _exp < 0 ) {
				var _nExp = -(long)_exp;
				if ( _nExp > int.MaxValue && _nExp > 100_000_000 )
					throw new OverflowException();

				_ret = _man * PowerOfTen((int)_nExp);
			} else {
				BigInteger remainder;
				_ret = BigInteger.DivRem(_man, PowerOfTen(_exp), out remainder);
				if ( !remainder.IsZero )
					throw new ArithmeticException("The value cannot be converted to an integer.");
			}
			return _ret;
		}

		/// <summary>
		/// Implicitly converts a <see cref="decimal"/> into a <see cref="BigDecimal"/>
		/// by extracting the internal 96‑bit integer representation and the scale
		/// factor. The conversion is exact and preserves all decimal digits.
		/// </summary>
		public static implicit operator BigDecimal ( decimal value ) {
			int[] bits = decimal.GetBits(value);

			BigInteger _mantisse =
			(new BigInteger((uint)bits[2]) << 64) |
			(new BigInteger((uint)bits[1]) << 32) |
			new BigInteger((uint)bits[0]);

			return new BigDecimal(
				((bits[3] & unchecked((int)0x80000000)) != 0) ? -_mantisse : _mantisse,
				(bits[3] >> 16) & 0x7F
			);
		}

		/// <summary>
		/// Explicitly converts a <see cref="BigDecimal"/> into a <see cref="double"/>
		/// using <see cref="ToDouble"/>. This conversion may lose precision if the
		/// <see cref="BigDecimal"/> exceeds the 53‑bit mantissa capacity of IEEE‑754.
		/// </summary>
		public static explicit operator double ( BigDecimal value )
			=> value.ToDouble();
	}
}

