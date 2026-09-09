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
using SystemEx.Rand.Engine;

namespace SystemEx.Rand {

#if DEFAULT_RANDENGIEN_MTWIST
	/// <summary>
	/// Provides the high-level random number generator implementation using the
	/// <see cref="MTwisterEngine"/> as the underlying engine. This variant is
	/// compiled when <c>DEFAULT_RANDENGIEN_MTWIST</c> is defined.
	/// </summary>
	public sealed class Randx : GenericRand<MTwisterEngine> {

		/// <summary>
		/// Initializes a new instance of the <see cref="Randx"/> class using the
		/// specified seed values. Only <paramref name="seedA"/> is used by the
		/// <see cref="MTwisterEngine"/>; the remaining parameters are ignored to
		/// maintain a consistent constructor signature.
		/// </summary>
		/// <param name="seedA">The primary seed value.</param>
		/// <param name="seedB">Unused for this engine variant.</param>
		/// <param name="seedC">Unused for this engine variant.</param>
		public Randx ( uint seedA = 0, uint seedB = 0, uint seedC = 0 )
			: base(new MTwisterEngine(seedA)) { }

		public Randx ( ISeed seed )
			: base(new MTwisterEngine(seed) ) { }
	}

#else

	/// <summary>
	/// Provides the high-level random number generator implementation using the
	/// <see cref="Isaac32Engine"/> as the underlying engine. This variant is
	/// compiled when <c>DEFAULT_RANDENGIEN_MTWIST</c> is not defined.
	/// </summary>
	public sealed class Randx : GenericRand<Isaac32Engine> {

		/// <summary>
		/// Initializes a new instance of the <see cref="Randx"/> class using the
		/// specified seed values. All three seed parameters are forwarded to the
		/// <see cref="Isaac32Engine"/> for initialization.
		/// </summary>
		/// <param name="seedA">The first seed value.</param>
		/// <param name="seedB">The second seed value.</param>
		/// <param name="seedC">The third seed value.</param>
		public Randx ( uint seedA = 0, uint seedB = 0, uint seedC = 0 )
			: base(new Isaac32Engine(seedA, seedB, seedC)) { }

		public Randx ( ISeed seed )
			: base(new Isaac32Engine(seed) ) { }
	}
#endif

}
