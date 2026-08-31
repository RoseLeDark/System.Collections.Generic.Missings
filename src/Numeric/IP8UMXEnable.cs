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
	/// <summary>
	/// Provides FP8‑format specific parameters required for MX‑block arithmetic.
	/// 
	/// <para>
	/// Implementations expose the structural properties of an FP8 type such as:
	/// <list type="bullet">
	/// <item><description>The mantissa bit mask</description></item>
	/// <item><description>The maximum exponent value</description></item>
	/// <item><description>The shift‑raster used for MX mantissa alignment</description></item>
	/// </list>
	/// These values allow <c>Float8UMX{T}</c> to operate generically across
	/// different FP8 formats (e.g., E5M2, E4M3, E3M4) without hard‑coded constants.
	/// </para>
	/// </summary>
	/// <typeparam name="Tbase">
	/// The underlying numeric type used to represent FP8 bit‑fields (typically
	/// <c>Fast_Byte</c> or <c>byte</c>).
	/// </typeparam>
	public interface IP8UMXEnable<Tbase> {
		/// <summary>
		/// Gets the bit mask used to extract the mantissa field of the FP8 value.
		/// The mask corresponds to <c>(1 &lt;&lt; MantissaBits) - 1</c>.
		/// </summary>
		Tbase MantissaMask { get; }

		/// <summary>
		/// Gets the maximum exponent value supported by the FP8 format.
		/// This corresponds to <c>(1 &lt;&lt; ExponentBits) - 1</c>.
		/// </summary>
		Tbase MaxExponent { get; }

		/// <summary>
		/// Gets the shift‑raster used for MX mantissa alignment.
		/// This value defines how far the mantissa is shifted left during
		/// MX arithmetic operations (Add/Sub/Mul/Div).
		/// </summary>
		Tbase ShiftRaster { get; }
	}

}
