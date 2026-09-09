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

namespace SystemEx.Rand.Engine {
	/// <summary>
	/// Defines the contract for a deterministic 32-bit random number generator engine.
	/// Implementations provide a reproducible sequence of unsigned 32-bit values
	/// based on an initial seed.
	/// </summary>
	public interface IRandomEngine {

		/// <summary>
		/// Gets the seed object that was used to initialize this engine.
		/// The seed allows deterministic reproduction of the generated sequence.
		/// </summary>
		ISeed StartSeed { get; }

		/// <summary>
		/// Generates the next 32-bit unsigned integer in the random sequence.
		/// </summary>
		/// <returns>The next 32-bit random value.</returns>
		uint Next ();
	}

}
