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


namespace SystemEx.Algorithms.Compute {
	/// \addtogroup AlgorithmsCompute
	/// @{

	/// <summary>
	/// Represents a generic computational model that transforms an input value 
	/// into an output value.  
	/// Used for lightweight algorithmic or mathematical models within the 
	/// Compute namespace.
	/// </summary>
	/// <typeparam name="TInput">The input type.</typeparam>
	/// <typeparam name="TOutput">The output type.</typeparam>
	internal interface IModel<TInput, TOutput> {

		/// <summary>
		/// Evaluates the model using the specified input and returns the result.
		/// </summary>
		TOutput Evaluate ( TInput input );
	}

	
}
