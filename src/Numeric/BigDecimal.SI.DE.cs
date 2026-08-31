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

namespace SystemEx.Numeric {


	public partial struct BigDecimal {

		/// <summary>
		/// Yocto (10^-24). Represents an extremely small scale factor used in
		/// advanced scientific contexts such as particle physics and quantum-level
		/// measurements.
		/// </summary>
		public static readonly BigDecimal Yocto = new BigDecimal(1, -24);

		/// <summary>
		/// Zepto (10^-21). Used for ultra-small magnitudes in nanotechnology,
		/// molecular chemistry, and precision scientific instrumentation.
		/// </summary>
		public static readonly BigDecimal Zepto = new BigDecimal(1, -21);

		/// <summary>
		/// Atto (10^-18). Commonly used in high‑precision measurement systems,
		/// including attoseconds, attocoulombs, and other quantum-scale quantities.
		/// </summary>
		public static readonly BigDecimal Atto  = new BigDecimal(1, -18);

		/// <summary>
		/// Femto (10^-15). Frequently used in optics, photonics, and quantum physics,
		/// such as femtosecond laser pulses and femtowatt power levels.
		/// </summary>
		public static readonly BigDecimal Femto = new BigDecimal(1, -15);

		/// <summary>
		/// Pico (10^-12). Widely used in electronics and RF engineering, including
		/// picofarads (pF), picoamps (pA), and picosecond timing.
		/// </summary>
		public static readonly BigDecimal Pico  = new BigDecimal(1, -12);

		/// <summary>
		/// Nano (10^-9). A standard engineering scale used for nanofarads (nF),
		/// nanometers (nm), nanoseconds (ns), and other common nano‑level quantities.
		/// </summary>
		public static readonly BigDecimal Nano  = new BigDecimal(1, -9);

		/// <summary>
		/// Micro (10^-6). A widely used SI prefix in electrical and mechanical
		/// engineering, including microamps (µA), microfarads (µF), and micrometers (µm).
		/// Supports both 'µ' and 'u' representations in parsing.
		/// </summary>
		public static readonly BigDecimal Micro = new BigDecimal(1, -6);

		/// <summary>
		/// Milli (10^-3). A common engineering scale used for milliamps (mA),
		/// millimeters (mm), millivolts (mV), and similar everyday technical units.
		/// </summary>
		public static readonly BigDecimal Milli = new BigDecimal(1, -3);

		/// <summary>
		/// Centi (10^-2). Less frequently used in scientific notation but common in
		/// everyday measurements such as centimeters (cm).
		/// </summary>
		public static readonly BigDecimal Centi = new BigDecimal(1, -2);

		/// <summary>
		/// Deci (10^-1). Rarely used in engineering applications but included for
		/// completeness as part of the SI standard.
		/// </summary>
		public static readonly BigDecimal Deci  = new BigDecimal(1, -1);


		// ---------------------------------------------------------------------
		// Large prefixes (positive powers of ten)
		// ---------------------------------------------------------------------

		/// <summary>
		/// Deca (10^1). Infrequently used in technical fields but part of the
		/// official SI prefix set.
		/// </summary>
		public static readonly BigDecimal Deca  = new BigDecimal(1, 1);

		/// <summary>
		/// Hecto (10^2). Primarily used in meteorology, for example in hectopascals (hPa).
		/// </summary>
		public static readonly BigDecimal Hecto = new BigDecimal(1, 2);

		/// <summary>
		/// Kilo (10^3). A very common engineering scale used for kiloohms (kΩ),
		/// kilograms (kg), kilohertz (kHz), and similar quantities.
		/// </summary>
		public static readonly BigDecimal Kilo  = new BigDecimal(1, 3);

		/// <summary>
		/// Mega (10^6). Standard prefix for large-scale values such as megawatts (MW),
		/// megabytes (MB), and megahertz (MHz).
		/// </summary>
		public static readonly BigDecimal Mega  = new BigDecimal(1, 6);

		/// <summary>
		/// Giga (10^9). Used for very large digital storage capacities, high-frequency
		/// signals, and large-scale energy measurements.
		/// </summary>
		public static readonly BigDecimal Giga  = new BigDecimal(1, 9);

		/// <summary>
		/// Tera (10^12). Represents extremely large quantities, commonly used for
		/// terabytes (TB) and terahertz (THz).
		/// </summary>
		public static readonly BigDecimal Tera  = new BigDecimal(1, 12);

		/// <summary>
		/// Peta (10^15). Used for massive computational workloads, data volumes,
		/// and large-scale physical measurements.
		/// </summary>
		public static readonly BigDecimal Peta  = new BigDecimal(1, 15);

		/// <summary>
		/// Exa (10^18). Typically encountered in high‑performance computing,
		/// astrophysics, and extremely large scientific datasets.
		/// </summary>
		public static readonly BigDecimal Exa   = new BigDecimal(1, 18);

		/// <summary>
		/// Zetta (10^21). Represents extraordinarily large magnitudes used in
		/// theoretical physics and global-scale data systems.
		/// </summary>
		public static readonly BigDecimal Zetta = new BigDecimal(1, 21);

		/// <summary>
		/// Yotta (10^24). The largest officially recognized SI prefix, used for
		/// astronomical-scale quantities and extreme data measurements.
		/// </summary>
		public static readonly BigDecimal Yotta = new BigDecimal(1, 24);
	}

}
