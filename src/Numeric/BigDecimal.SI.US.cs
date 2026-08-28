using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Numeric {

	/// <summary>
	/// Provides amerikanische (US‑Customary) engineering units directly within
	/// <see cref="BigDecimal"/>. These units are widely used in US technical
	/// industries, automotive systems, HVAC, construction, and aerospace.
	/// 
	/// <para>
	/// All values are represented exakt as <see cref="BigDecimal"/> without floating‑
	/// point rounding, making them suitable for precise technical calculations.
	/// </para>
	/// </summary>
	public partial struct BigDecimal {
		/// <summary>
		/// Inch (1 in). Base unit in many US technical domains.
		/// </summary>
		public static readonly BigDecimal Inch = new BigDecimal(1, 0);

		/// <summary>
		/// Foot (1 ft = 12 in). Common in construction and mechanical engineering.
		/// </summary>
		public static readonly BigDecimal Foot = new BigDecimal(12, 0);

		/// <summary>
		/// Yard (1 yd = 36 in). Used in manufacturing and textile industries.
		/// </summary>
		public static readonly BigDecimal Yard = new BigDecimal(36, 0);

		/// <summary>
		/// Mile (1 mi = 63360 in). Used for large‑scale distances.
		/// </summary>
		public static readonly BigDecimal Mile = new BigDecimal(63360, 0);

		/// <summary>
		/// Ounce (1 oz). Small mass unit used in everyday amerikanische measurements.
		/// </summary>
		public static readonly BigDecimal Ounce = new BigDecimal(1, 0);

		/// <summary>
		/// Pound (1 lb = 16 oz). Standard US mass unit.
		/// </summary>
		public static readonly BigDecimal Pound = new BigDecimal(16, 0);

		/// <summary>
		/// Cup (1 cup = 8 fl oz).
		/// </summary>
		public static readonly BigDecimal Cup = new BigDecimal(8, 0);

		/// <summary>
		/// Pint (1 pt = 16 fl oz).
		/// </summary>
		public static readonly BigDecimal Pint = new BigDecimal(16, 0);

		/// <summary>
		/// Quart (1 qt = 32 fl oz).
		/// </summary>
		public static readonly BigDecimal Quart = new BigDecimal(32, 0);

		/// <summary>
		/// Gallon (1 gal = 128 fl oz). Standard amerikanische volume unit.
		/// </summary>
		public static readonly BigDecimal Gallon = new BigDecimal(128, 0);

		/// <summary>
		/// PSI (pounds per square inch). Very common in amerikanische automotive
		/// and industrial systems.
		/// </summary>
		public static readonly BigDecimal PSI = new BigDecimal(1, 0);

		/// <summary>
		/// BTU (British Thermal Unit). Used in amerikanische HVAC and heating systems.
		/// </summary>
		public static readonly BigDecimal BTU = new BigDecimal(1, 0);


		/// <summary>
		/// Horsepower (hp). Used in amerikanische automotive and mechanical engineering.
		/// </summary>
		public static readonly BigDecimal Horsepower = new BigDecimal(1, 0);


	}
}
