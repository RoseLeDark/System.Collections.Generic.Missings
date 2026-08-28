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

	/// <summary>
	/// Defines the set of supported U.S. customary engineering units used for
	/// formatting and interpreting <see cref="BigDecimal"/> values. These units
	/// represent common American measurement systems frequently encountered in
	/// mechanical, civil, automotive, industrial, and HVAC engineering.
	/// </summary>
	public enum USUnit {

		/// <summary>
		/// Inch (in). A fundamental U.S. length unit widely used in manufacturing,
		/// machining, construction, and mechanical engineering.
		/// </summary>
		Inch,

		/// <summary>
		/// Foot (ft). Equal to 12 inches. Commonly used in construction, surveying,
		/// architecture, and general engineering applications.
		/// </summary>
		Foot,

		/// <summary>
		/// Yard (yd). Equal to 3 feet or 36 inches. Used in textiles, manufacturing,
		/// and certain civil engineering contexts.
		/// </summary>
		Yard,

		/// <summary>
		/// Mile (mi). A large-scale U.S. length unit used for geographic distances,
		/// transportation engineering, and navigation.
		/// </summary>
		Mile,

		/// <summary>
		/// Ounce (oz). A small U.S. mass unit used in everyday measurements,
		/// packaging, and lightweight material specifications.
		/// </summary>
		Ounce,

		/// <summary>
		/// Pound (lb). A standard U.S. mass unit equal to 16 ounces. Widely used in
		/// mechanical engineering, material science, and industrial applications.
		/// </summary>
		Pound,

		/// <summary>
		/// Cup. A U.S. volume unit equal to 8 fluid ounces. Commonly used in food
		/// science, chemistry labs, and consumer product measurements.
		/// </summary>
		Cup,

		/// <summary>
		/// Pint (pt). A U.S. volume unit equal to 16 fluid ounces. Used in food
		/// production, chemical mixtures, and fluid handling.
		/// </summary>
		Pint,

		/// <summary>
		/// Quart (qt). A U.S. volume unit equal to 32 fluid ounces or one quarter
		/// of a gallon. Used in automotive fluids, chemical processing, and food
		/// manufacturing.
		/// </summary>
		Quart,

		/// <summary>
		/// Gallon (gal). A large U.S. volume unit equal to 128 fluid ounces. Common
		/// in automotive engineering, HVAC systems, fuel measurement, and industrial
		/// fluid handling.
		/// </summary>
		Gallon,

		/// <summary>
		/// PSI (pounds per square inch). A standard U.S. pressure unit widely used
		/// in automotive engineering, hydraulics, pneumatics, and industrial systems.
		/// </summary>
		PSI,

		/// <summary>
		/// BTU (British Thermal Unit). A U.S. energy unit used extensively in HVAC,
		/// heating systems, thermodynamics, and energy engineering.
		/// </summary>
		BTU,

		/// <summary>
		/// Horsepower (hp). A U.S. power unit used in automotive engineering,
		/// mechanical systems, and industrial machinery performance ratings.
		/// </summary>
		Horsepower
	}


	/// <summary>
	/// Defines the set of supported SI engineering units for formatting
	/// <see cref="BigDecimal"/> values. These units correspond to standard
	/// physical quantities used throughout scientific, electrical, mechanical,
	/// and general technical disciplines.
	/// </summary>
	public enum SIUnit {

		/// <summary>
		/// Ohm (Ω). The SI unit of electrical resistance, used extensively in
		/// electronics, circuit design, signal processing, and power systems.
		/// </summary>
		Ohm,

		/// <summary>
		/// Ampere (A). The SI base unit of electric current, fundamental in
		/// electrical engineering, electromagnetism, and power distribution.
		/// </summary>
		Ampere,

		/// <summary>
		/// Volt (V). The SI unit of electric potential and electromotive force,
		/// used in circuit analysis, power systems, and electronic device design.
		/// </summary>
		Volt,

		/// <summary>
		/// Farad (F). The SI unit of capacitance, widely used in electronics,
		/// signal filtering, energy storage, and RF engineering.
		/// </summary>
		Farad,

		/// <summary>
		/// Henry (H). The SI unit of inductance, used in electromagnetics,
		/// transformer design, power electronics, and RF systems.
		/// </summary>
		Henry,

		/// <summary>
		/// Meter (m). The SI base unit of length, used across physics, engineering,
		/// construction, manufacturing, and scientific measurement.
		/// </summary>
		Meter,

		/// <summary>
		/// Gram (g). The SI unit of mass (derived from kilogram), used in chemistry,
		/// material science, laboratory measurement, and industrial processes.
		/// </summary>
		Gram,

		/// <summary>
		/// Joule (J). The SI unit of energy, used in thermodynamics, mechanics,
		/// electrical systems, and general physics.
		/// </summary>
		Joule,

		/// <summary>
		/// Watt (W). The SI unit of power, representing energy per unit time.
		/// Used in electrical engineering, mechanics, HVAC, and energy systems.
		/// </summary>
		Watt,

		/// <summary>
		/// Hertz (Hz). The SI unit of frequency, used in signal processing,
		/// communications, acoustics, control systems, and physics.
		/// </summary>
		Hertz
	}


	public partial struct BigDecimal {

		/// <summary>
		/// Converts this <see cref="BigDecimal"/> into a canonical scientific
		/// notation string. The value is normalized before formatting to ensure
		/// removal of trailing zeros and consistent exponent representation.
		///
		/// <para>
		/// Examples:
		/// <code>
		/// 12345 → "1.2345e4"
		/// 0.00123 → "1.23e-3"
		/// </code>
		/// </para>
		/// </summary>
		public override string ToString () {
			if ( m_bMantissa.IsZero )
				return "0";

			var n = Normalize();
			BigInteger man = n.Mantissa;
			int exp = n.Exponent;

			if ( exp == 0 )
				return man.ToString();

			if ( BigInteger.Abs(man) < 10 )
				return $"{man}e{exp}";

			string s = BigInteger.Abs(man).ToString();
			bool neg = man.Sign < 0;

			char first = s[0];
			string rest = s.Substring(1);

			int adjExp = exp + (s.Length - 1);

			if ( rest.Length == 0 )
				return neg ? $"-{first}e{adjExp}" : $"{first}e{adjExp}";

			return neg
				? $"-{first}.{rest}e{adjExp}"
				: $"{first}.{rest}e{adjExp}";
		}

		/// <summary>
		/// Formats the value using engineering notation, where the exponent is
		/// always a multiple of three. This representation aligns with SI prefixes
		/// and is commonly used in electrical and mechanical engineering.
		///
		/// <para>
		/// Examples:
		/// <code>
		/// 12000 → "12e3"
		/// 0.00045 → "450e-6"
		/// 1.2e4 → "12e3"
		/// </code>
		/// </para>
		/// </summary>
		public string ToEngineeringString () {
			if ( m_bMantissa.IsZero )
				return "0";

			var n = Normalize();
			BigInteger man = n.Mantissa;
			int exp = n.Exponent;

			string s = BigInteger.Abs(man).ToString();
			bool neg = man.Sign < 0;

			int sciExp = exp + (s.Length - 1);
			int engExp = sciExp - (sciExp % 3);
			int shift = sciExp - engExp;

			BigInteger engMan = man;
			if ( shift > 0 )
				engMan = man / BigInteger.Pow(10, shift);

			string engStr = BigInteger.Abs(engMan).ToString();

			return neg
				? $"-{engStr}e{engExp}"
				: $"{engStr}e{engExp}";
		}

		/// <summary>
		/// Parses a <see cref="BigDecimal"/> from scientific notation such as
		/// "1.23e-9". The mantissa may contain a decimal point, and the exponent
		/// must follow either 'e' or 'E'.
		/// </summary>
		public static BigDecimal ParseScientific ( string s ) {
			if ( string.IsNullOrWhiteSpace(s) )
				throw new FormatException("Input string is empty.");

			s = s.Trim();

			int idx = s.IndexOfAny(new[] { 'e', 'E' });
			if ( idx < 0 )
				throw new FormatException("Scientific notation requires an exponent.");

			string mantissaPart = s.Substring(0, idx);
			string exponentPart = s.Substring(idx + 1);

			if ( !int.TryParse(exponentPart, out int exponent) )
				throw new FormatException("Invalid exponent in scientific notation.");

			bool neg = mantissaPart.StartsWith("-");
			if ( neg )
				mantissaPart = mantissaPart.Substring(1);

			int decimalPos = mantissaPart.IndexOf('.');
			BigInteger mantissa;
			int scale = 0;

			if ( decimalPos >= 0 ) {
				string digits = mantissaPart.Replace(".", "");
				mantissa = BigInteger.Parse(digits);
				scale = -(mantissaPart.Length - decimalPos - 1);
			} else {
				mantissa = BigInteger.Parse(mantissaPart);
			}

			if ( neg )
				mantissa = -mantissa;

			int finalExp = exponent + scale;
			return new BigDecimal(mantissa, finalExp).Normalize();
		}

		/// <summary>
		/// Parses a <see cref="BigDecimal"/> from engineering notation such as
		/// "12e3" or "450e-6". The exponent is typically a multiple of three,
		/// although this is not strictly enforced.
		/// </summary>
		public static BigDecimal ParseEngineering ( string s ) {
			if ( string.IsNullOrWhiteSpace(s) )
				throw new FormatException("Input string is empty.");

			s = s.Trim();

			int idx = s.IndexOfAny(new[] { 'e', 'E' });
			if ( idx < 0 )
				throw new FormatException("Engineering notation requires an exponent.");

			string mantissaPart = s.Substring(0, idx);
			string exponentPart = s.Substring(idx + 1);

			if ( !int.TryParse(exponentPart, out int exponent) )
				throw new FormatException("Invalid exponent in engineering notation.");

			BigInteger mantissa = BigInteger.Parse(mantissaPart);
			return new BigDecimal(mantissa, exponent).Normalize();
		}

		/// <summary>
		/// Attempts to parse a <see cref="BigDecimal"/> from scientific notation.
		/// Returns <c>true</c> on success; otherwise <c>false</c>.
		/// </summary>
		public static bool TryParseScientific ( string? s, out BigDecimal value ) {
			value = BigDecimal.Zero;

			if ( string.IsNullOrWhiteSpace(s) )
				return false;

			s = s.Trim();

			int idx = s.IndexOfAny(new[] { 'e', 'E' });
			if ( idx < 0 )
				return false;

			string mantissaPart = s.Substring(0, idx);
			string exponentPart = s.Substring(idx + 1);

			if ( !int.TryParse(exponentPart, out int exponent) )
				return false;

			bool neg = mantissaPart.StartsWith("-");
			if ( neg )
				mantissaPart = mantissaPart.Substring(1);

			int decimalPos = mantissaPart.IndexOf('.');
			BigInteger mantissa;
			int scale = 0;

			if ( decimalPos >= 0 ) {
				string digits = mantissaPart.Replace(".", "");
				if ( !BigInteger.TryParse(digits, out mantissa) )
					return false;

				scale = -(mantissaPart.Length - decimalPos - 1);
			} else {
				if ( !BigInteger.TryParse(mantissaPart, out mantissa) )
					return false;
			}

			if ( neg )
				mantissa = -mantissa;

			int finalExp = exponent + scale;
			value = new BigDecimal(mantissa, finalExp).Normalize();
			return true;
		}

		/// <summary>
		/// Attempts to parse a <see cref="BigDecimal"/> from engineering notation.
		/// Returns <c>true</c> on success; otherwise <c>false</c>.
		/// </summary>
		public static bool TryParseEngineering ( string? s, out BigDecimal value ) {
			value = BigDecimal.Zero;

			if ( string.IsNullOrWhiteSpace(s) )
				return false;

			s = s.Trim();

			int idx = s.IndexOfAny(new[] { 'e', 'E' });
			if ( idx < 0 )
				return false;

			string mantissaPart = s.Substring(0, idx);
			string exponentPart = s.Substring(idx + 1);

			if ( !int.TryParse(exponentPart, out int exponent) )
				return false;

			if ( !BigInteger.TryParse(mantissaPart, out BigInteger mantissa) )
				return false;

			value = new BigDecimal(mantissa, exponent).Normalize();
			return true;
		}

		/// <summary>
		/// Formats the value using U.S. customary engineering units. The value is
		/// divided by the corresponding unit factor and then expressed in
		/// engineering notation.
		/// </summary>
		public string ToUSString ( USUnit unit ) {
			BigDecimal factor = unit switch
			{
				USUnit.Inch => Inch,
				USUnit.Foot => Foot,
				USUnit.Yard => Yard,
				USUnit.Mile => Mile,
				USUnit.Ounce => Ounce,
				USUnit.Pound => Pound,
				USUnit.Cup => Cup,
				USUnit.Pint => Pint,
				USUnit.Quart => Quart,
				USUnit.Gallon => Gallon,
				USUnit.PSI => PSI,
				USUnit.BTU => BTU,
				USUnit.Horsepower => Horsepower,
				_ => One
			};

			BigDecimal v = this / factor;
			return $"{v.ToEngineeringString()} {unit}";
		}

		/// <summary>
		/// Formats the value using SI prefixes and a specified SI unit. The value
		/// is first converted to engineering notation, ensuring alignment with
		/// standard SI scaling conventions.
		/// </summary>
		public string ToSIString ( SIUnit unit ) {
			string eng = ToEngineeringString();

			string suffix = unit switch
			{
				SIUnit.Ohm => "Ω",
				SIUnit.Ampere => "A",
				SIUnit.Volt => "V",
				SIUnit.Farad => "F",
				SIUnit.Henry => "H",
				SIUnit.Meter => "m",
				SIUnit.Gram => "g",
				SIUnit.Joule => "J",
				SIUnit.Watt => "W",
				SIUnit.Hertz => "Hz",
				_ => ""
			};

			return $"{eng} {suffix}";
		}
	}
}
