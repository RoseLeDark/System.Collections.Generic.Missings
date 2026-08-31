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
	/// Represents a 32‑element MX‑block of <see cref="FloatUE5M2"/> values 
	/// using <see cref="Float8UMX{T}"/> as the underlying generic FP8 MX engine.
	/// 
	/// <para>
	/// This type provides a convenient strongly‑typed wrapper for 
	/// <see cref="FloatUE5M2"/> MX blocks, allowing direct construction 
	/// without specifying the generic parameter.
	/// </para>
	/// 
	/// <para>
	/// The shared exponent controls the block‑wide scaling factor used 
	/// during MX arithmetic (Add/Sub/Mul/Div). All FP8 operations follow 
	/// the unsigned E5M2 FP8 rules implemented in <see cref="FloatUE5M2"/>.
	/// </para>
	/// </summary>
	public class FloatUE5M2b32 : Float8UMX<FloatUE5M2> {


		/// <summary>
		/// Initializes a new MX‑block with shared exponent 0 and 
		/// 32 zero‑initialized <see cref="FloatUE5M2"/> elements.
		/// </summary>
		public FloatUE5M2b32 ()
			: base(0x00, new FloatUE5M2[32]) { }

		/// <summary>
		/// Initializes a new MX‑block with shared exponent 0 and 
		/// the provided 32‑element FP8 vector.
		/// </summary>
		/// <param name="vector">Array of 32 <see cref="FloatUE5M2"/> values.</param>
		public FloatUE5M2b32 ( FloatUE5M2[] vector )
			: base(0x00, vector) { }

		/// <summary>
		/// Initializes a new MX‑block with the specified shared exponent 
		/// and FP8 element vector.
		/// </summary>
		/// <param name="sharedExponent">Block‑wide exponent.</param>
		/// <param name="vector">Array of 32 <see cref="FloatUE5M2"/> values.</param>
		public FloatUE5M2b32 ( byte sharedExponent, FloatUE5M2[] vector )
			: base(sharedExponent, vector) { }
	}
}
