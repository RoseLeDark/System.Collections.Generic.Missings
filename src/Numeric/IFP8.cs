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

	public interface IFP8 : IFloat<Fast_Byte> { }

	/// <summary>
	/// Defines an 8‑bit floating‑point format based on <see cref="IFloat{TSelf, TBias}"/>.
	/// 
	/// <para>
	/// <see cref="IFP8{TSelf}"/> is intended for extremely compact numeric
	/// representations such as FP8 variants used in machine learning or embedded
	/// systems. The exact bit layout (sign, exponent, mantissa) is determined by
	/// the <see cref="IFloat{TSelf, TBias}"/> implementation.
	/// </para>
	/// </summary>
	public interface IFP8<TSelf> : IFP8, IFloat<TSelf, Fast_Byte>
		where TSelf : struct, IFP8<TSelf> {
		static abstract	bool  IsMXSupport { get; }
		static abstract TSelf FromComponent (Fast_Byte sign, Fast_Byte mantissa, Fast_Byte expotent);
	}


}
