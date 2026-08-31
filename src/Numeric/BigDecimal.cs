using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using SystemEx.Collections.Generic;

namespace SystemEx.Numeric {

	/// <summary>
	/// Represents an arbitrary‑precision decimal number stored in scientific
	/// notation using a <see cref="BigInteger"/> mantissa and a base‑10 exponent.
	/// 
	/// <para>
	/// A <see cref="BigDecimal"/> models values of the form:
	/// <code>
	///     value = mantissa × 10^exponent
	/// </code>
	/// The mantissa is stored as an unbounded <see cref="BigInteger"/>, while the
	/// exponent is a signed 32‑bit integer. This representation allows exact
	/// modeling of extremely small or extremely large numbers without rounding
	/// artifacts or loss of precision.
	/// </para>
	/// 
	/// <para>
	/// Unlike floating‑point types (<c>float</c>, <c>double</c>), <see cref="BigDecimal"/>
	/// does not normalize its mantissa automatically. The caller may choose any
	/// mantissa/exponent combination that represents the desired value. This makes
	/// the type suitable for scientific, financial, and mathematical applications
	/// where deterministic precision and explicit exponent control are required.
	/// </para>
	/// </summary>
	public partial struct BigDecimal : IComparable, IComparable<BigDecimal>, IEquatable<BigDecimal> {
		private BigInteger m_bMantissa;
		private int m_iExponent;

		/// <summary>
		/// Gets a <see cref="BigDecimal"/> representing zero (0 × 10^0).
		/// </summary>
		public static BigDecimal Zero { get; } = new BigDecimal(BigInteger.Zero, 0);

		/// <summary>
		/// Gets a <see cref="BigDecimal"/> representing one (1 × 10^0).
		/// </summary>
		public static BigDecimal One { get; } = new BigDecimal(BigInteger.One, 0);

		/// <summary>
		/// Gets a <see cref="BigDecimal"/> representing negative one (-1 × 10^0).
		/// </summary>
		public static BigDecimal NegativeOne { get; } = new BigDecimal(BigInteger.MinusOne, 0);

		/// <summary>Gets a value that represents the number 0.5 (one half).</summary>
		public static BigDecimal OneHalf => new BigDecimal(5, -1);

		/// <summary>
		/// Gets the numeric radix (base) used by <see cref="BigDecimal"/>.
		/// For decimal numbers this is always 10.
		/// </summary>
		public static int Radix => 10;

		/// <summary>
		/// Returns the greater of two <see cref="BigDecimal"/> values.
		/// </summary>
		public static BigDecimal Max ( BigDecimal x, BigDecimal y ) => x.CompareTo(y) >= 0 ? x : y;

		/// <summary>
		/// Returns the lesser of two <see cref="BigDecimal"/> values.
		/// </summary>
		public static BigDecimal Min ( BigDecimal x, BigDecimal y ) => x.CompareTo(y) <= 0 ? x : y;

		/// <summary>
		/// Indicates whether the specified <see cref="BigDecimal"/> is in canonical
		/// representation. Currently always returns <c>true</c>.
		/// </summary>
		public static bool IsCanonical ( BigDecimal value ) => true;

		/// <summary>
		/// Determines whether a value is a complex number. Always <c>false</c>.
		/// </summary>
		public static bool IsComplexNumber ( BigDecimal value ) => false;

		/// <summary>
		/// Determines whether a value is finite. Always <c>true</c> for <see cref="BigDecimal"/>.
		/// </summary>
		public static bool IsFinite ( BigDecimal value ) => true;

		/// <summary>
		/// Determines whether a value is imaginary. Always <c>false</c>.
		/// </summary>
		public static bool IsImaginaryNumber ( BigDecimal value ) => false;

		/// <summary>
		/// Determines whether a value is infinite. Always <c>false</c> for <see cref="BigDecimal"/>.
		/// </summary>
		public static bool IsInfinity ( BigDecimal value ) => false;
		/// <summary>
		/// Determines whether a value is not a number (NaN). Always <c>false</c>
		/// for <see cref="BigDecimal"/>.
		/// </summary>
		public static bool IsNaN ( BigDecimal value ) => false;
		/// <summary>
		/// Determines whether a value is negative infinity. Always <c>false</c>.
		/// </summary>
		public static bool IsNegativeInfinity ( BigDecimal value ) => false;
		/// <summary>
		/// Determines whether a value is considered normal. For <see cref="BigDecimal"/>,
		/// this is defined as any non‑zero value.
		/// </summary>
		public static bool IsNormal ( BigDecimal value ) => !value.IsZero;
		/// <summary>
		/// Determines whether a value is positive infinity. Always <c>false</c>.
		/// </summary>
		public static bool IsPositiveInfinity ( BigDecimal value ) => false;
		/// <summary>
		/// Determines whether a value is a real number. Always <c>true</c> for
		/// <see cref="BigDecimal"/>.
		/// </summary>
		public static bool IsRealNumber ( BigDecimal value ) => true;
		/// <summary>
		/// Determines whether a value is subnormal. Always <c>false</c> for
		/// <see cref="BigDecimal"/>.
		/// </summary>
		public static bool IsSubnormal ( BigDecimal value ) => false;
		/// <summary>
		/// Gets the maximum representable <see cref="BigDecimal"/> value within the
		/// current safe range. The mantissa is limited to 96 bits.
		/// </summary>
		public static readonly BigDecimal MaxValue = new BigDecimal((BigInteger.One << 96) - BigInteger.One, 0);
		/// <summary>
		/// Gets a <see cref="BigDecimal"/> representing the safe mantissa limit
		/// used for internal operations. The mantissa is limited to 256 bits.
		/// </summary>
		public static readonly BigDecimal SafeMantissaLimit = new BigDecimal((BigInteger.One << 256) - BigInteger.One, 0);
		/// <summary>
		/// Gets the minimum representable <see cref="BigDecimal"/> value within the
		/// current safe range. The mantissa is limited to 96 bits and negative.
		/// </summary>
		public static readonly BigDecimal MinValue = new BigDecimal(-(BigInteger.One << 96), 0);

		/// <summary>
		/// Gets a <see cref="BigDecimal"/> representing ten (10 × 10^0).
		/// </summary>
		public static BigDecimal Ten { get; } = new BigDecimal(new BigInteger(10), 0);

		/// <summary>
		/// Gets the sign of the mantissa as a <see cref="Triple"/> value:
		/// <list type="bullet">
		///   <item><description><c>True</c>  — positive</description></item>
		///   <item><description><c>False</c> — negative</description></item>
		///   <item><description><c>Nin</c>   — zero</description></item>
		/// </list>
		/// </summary>
		public Triple Sign =>
			m_bMantissa.Sign == 1 ? triple.True :
			m_bMantissa.Sign == -1 ? triple.False :
			triple.Nin;

		/// <summary>
		/// Gets whether the value is positive or zero.
		/// </summary>
		public bool IsPositive => m_bMantissa.Sign >= 0;

		/// <summary>
		/// Gets whether the value is strictly negative.
		/// </summary>
		public bool IsNegative => m_bMantissa.Sign < 0;

		/// <summary>
		/// Gets whether the value is exactly zero.
		/// </summary>
		public bool IsZero => m_bMantissa.IsZero;

		/// <summary>
		/// Gets or sets the base‑10 exponent of the number.
		/// 
		/// <para>
		/// The exponent determines the scale of the value:
		/// <code>
		///     value = mantissa × 10^exponent
		/// </code>
		/// Changing the exponent does not modify the mantissa. The caller is
		/// responsible for maintaining a consistent representation.
		/// </para>
		/// </summary>
		public int Exponent {
			get => m_iExponent;
			set => m_iExponent = value;
		}
		/// <summary>
		/// Gets the number of significant decimal digits in the mantissa.
		/// 
		/// <para>
		/// For non‑zero values this is defined as:
		/// <code>
		///     precision = floor(log10(|mantissa|)) + 1
		/// </code>
		/// For zero, the precision is defined as 1.
		/// </para>
		/// </summary>
		public int Precision => m_bMantissa.IsZero ? 1 : (int)System.Math.Floor(BigInteger.Log10(BigInteger.Abs(m_bMantissa))) + 1;


		/// <summary>
		/// Gets the mantissa component of the number. The mantissa is stored as an
		/// arbitrary‑precision integer and may be positive, negative, or zero.
		/// </summary>
		public BigInteger Mantissa => m_bMantissa;

		/// <summary>
		/// Initializes a new <see cref="BigDecimal"/> with the specified mantissa
		/// and an exponent of zero.
		/// </summary>
		public BigDecimal ( BigInteger value )
			: this(value, 0) { }

		/// <summary>
		/// Initializes a new <see cref="BigDecimal"/> from a 32‑bit integer.
		/// </summary>
		public BigDecimal ( int value )
			: this(new BigInteger(value), 0) { }

		/// <summary>
		/// Initializes a new <see cref="BigDecimal"/> from a 64‑bit integer.
		/// </summary>
		public BigDecimal ( long value )
			: this(new BigInteger(value), 0) { }

		/// <summary>
		/// Initializes a new <see cref="BigDecimal"/> with the specified mantissa
		/// and exponent.
		/// </summary>
		/// <param name="baseValue">The mantissa.</param>
		/// <param name="scale">The base‑10 exponent.</param>
		public BigDecimal ( BigInteger baseValue, int scale ) {
			m_bMantissa = baseValue;
			m_iExponent = scale;
		}
		/// <summary>
		/// Initializes a new <see cref="BigDecimal"/> from a tuple containing a
		/// mantissa and exponent pair.
		/// </summary>
		/// <param name="tuple">
		/// A pair whose <c>First</c> element is the mantissa and whose <c>Second</c>
		/// element is the base‑10 exponent.
		/// </param>
		public BigDecimal ( Pair<BigInteger, Int32> tuple ) {
			m_bMantissa = tuple.First;
			m_iExponent = tuple.Second;
		}

		/// <summary>
		/// Initializes a new <see cref="BigDecimal"/> from a tuple containing a
		/// mantissa and exponent pair.
		/// </summary>
		public BigDecimal ( Collections.Generic.Tuple<BigInteger> tuple ) {
		
				m_bMantissa = tuple.First;
				m_iExponent = (tuple.Count == 1) ? 0 : (int)tuple.Get(1);
			
		}
		/// <summary>
		/// Initializes a new <see cref="BigDecimal"/> from a rational value
		/// represented by a numerator and denominator.
		/// 
		/// <para>
		/// The division <c>numerator / denominator</c> is performed using the
		/// existing <see cref="BigDecimal"/> division logic, and the resulting
		/// mantissa and exponent are stored in this instance.
		/// </para>
		/// </summary>
		/// <param name="numerator">The rational numerator.</param>
		/// <param name="denominator">The rational denominator.</param>
		public BigDecimal ( BigInteger numerator, BigInteger denominator ) {
			BigDecimal _erg = numerator / denominator;
			m_bMantissa = _erg.Mantissa;
			m_iExponent = _erg.Exponent;
		}

		/// <summary>
		/// Compares this instance to another object. Only <see cref="BigDecimal"/>
		/// instances are supported.
		/// </summary>
		public int CompareTo ( object? obj ) {
			if ( obj is null )
				return 1;

			if ( obj is BigDecimal other )
				return CompareTo(other);

			throw new ArgumentException("Object is not a BigDecimal.", nameof(obj));
		}

		/// <summary>
		/// Normalizes the BigDecimal by removing trailing powers of ten from the mantissa
		/// and adjusting the exponent accordingly. The normalized form ensures that the
		/// mantissa is not divisible by 10, unless the value is zero.
		/// </summary>
		public BigDecimal Normalize () {
			if ( m_bMantissa.IsZero )
				return Zero; // canonical zero

			BigInteger man = m_bMantissa;
			int exp = m_iExponent;

			// Remove trailing zeros from mantissa
			while ( true ) {
				BigInteger div = BigInteger.DivRem(man, 10, out BigInteger rem);
				if ( !rem.IsZero )
					break;

				man = div;
				exp += 1;
			}

			return new BigDecimal(man, exp);
		}
		/// <summary>
		/// Compares this <see cref="BigDecimal"/> to another instance and returns
		/// a value indicating their relative order.
		/// 
		/// <para>
		/// The comparison takes into account both mantissa and exponent. Values
		/// with different scales are aligned using powers of ten so that their
		/// numeric value can be compared without loss of precision.
		/// </para>
		/// </summary>
		/// <param name="other">The value to compare with this instance.</param>
		/// <returns>
		/// A negative number if this instance is less than <paramref name="other"/>,
		/// zero if they are equal, or a positive number if this instance is greater.
		/// </returns>
		public int CompareTo ( BigDecimal other ) {
			var sing_cmp = m_bMantissa.Sign.CompareTo(other.m_bMantissa.Sign);
			if ( sing_cmp != 0 ) return sing_cmp;
			if ( m_bMantissa.IsZero ) return 0;

			int _ret = 0;
			var _ExptAd = (long)Precision - Exponent;
			var _oExptAd = (long)other.Precision - other.Exponent;

			BigInteger _os = other.m_bMantissa * PowerOfTen(CheckExponentRange((long)Exponent - other.Exponent));
			BigInteger _st = m_bMantissa * PowerOfTen(CheckExponentRange((long)other.Exponent - Exponent));

			if ( m_iExponent == other.m_iExponent ) {
				_ret = m_bMantissa.CompareTo(other.m_bMantissa);

			} else if ( _ExptAd != _oExptAd ) {

				var _r = _ExptAd  < _oExptAd ? -1 : 1;
				_ret = m_bMantissa.Sign > 0 ? _r : -_r;
			} else {
				_ret = (Exponent > other.Exponent) ? m_bMantissa.CompareTo(_os) : _st.CompareTo(other.m_bMantissa);
			}

			return _ret;
		}
		/// <summary>
		/// Computes a hash code for this <see cref="BigDecimal"/> instance.
		/// 
		/// <para>
		/// The value is first normalized using <see cref="Normalize"/> to ensure that
		/// numerically equivalent representations (e.g. 100 × 10⁻² and 1 × 10⁰)
		/// produce identical hash codes. The hash is then derived from the normalized
		/// mantissa and exponent.
		/// </para>
		/// 
		/// <para>
		/// This guarantees that the hash code reflects the actual numeric value rather
		/// than the internal mantissa/exponent representation, which may vary because
		/// <see cref="BigDecimal"/> does not normalize automatically.
		/// </para>
		/// </summary>
		public override int GetHashCode () {
			var n = Normalize();
			return HashCode.Combine(n.Mantissa, n.Exponent);
		}


		/// <summary>
		/// Returns the number of decimal digits in the specified integer value.
		/// </summary>
		public static Int32 NumberOfDigits ( BigInteger value ) => (Int32)System.Math.Ceiling(BigInteger.Log10(value * value.Sign));



		/// <summary>
		/// Determines whether this instance and the specified <see cref="BigDecimal"/>
		/// represent the same numeric value.
		/// </summary>
		public bool Equals ( BigDecimal other ) => CompareTo(other) == 0;

		/// <summary>
		/// Determines whether this instance and the specified object represent the same
		/// numeric value. Only <see cref="BigDecimal"/> instances are supported.
		/// </summary>
		public override bool Equals ( object? obj ) => obj is BigDecimal other && Equals(other);

		/// <summary>
		/// Determines whether two <see cref="BigDecimal"/> values are numerically equal.
		/// </summary>
		public static bool operator == ( BigDecimal a, BigDecimal b ) 
			=> a.CompareTo(b) == 0;
		/// <summary>
		/// Determines whether the first <see cref="BigDecimal"/> is numerically smaller
		/// than the second.
		/// </summary>
		public static bool operator < ( BigDecimal a, BigDecimal b ) 
			=> a.CompareTo(b) < 0;
		/// <summary>
		/// Determines whether the first <see cref="BigDecimal"/> is numerically greater
		/// than the second.
		/// </summary>
		public static bool operator > ( BigDecimal a, BigDecimal b )
			=> a.CompareTo(b) > 0;
		/// <summary>
		/// Determines whether the first <see cref="BigDecimal"/> is numerically greater
		/// than or equal to the second.
		/// </summary>
		public static bool operator >= ( BigDecimal a, BigDecimal b )
			=> !(a < b);

		/// <summary>
		/// Determines whether the first <see cref="BigDecimal"/> is numerically smaller
		/// than or equal to the second.
		/// </summary>
		public static bool operator <= ( BigDecimal a, BigDecimal b ) =>
			!(a > b);

		/// <summary>
		/// Determines whether two <see cref="BigDecimal"/> values are not numerically equal.
		/// </summary>
		public static bool operator != ( BigDecimal a, BigDecimal b ) =>
			!(a == b);

	
		

	}
}
