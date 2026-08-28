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


using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Algorithms.Compute {
	/// \addtogroup AlgorithmsCompute
	/// @{

	/// <summary>
	/// Represents a simple classifier that maps an input value to a 
	/// <see cref="Triple"/> decision (True, False, Nin).  
	/// Used for lightweight rule‑based or threshold‑based classification.
	/// </summary>
	/// <typeparam name="T">The input type to classify.</typeparam>
	public interface IClassifier<T> {

		/// <summary>
		/// Classifies the given input and returns a three‑state decision.
		/// </summary>
		Triple Classify ( T input );
	}


	/// <summary>
	/// Represents a similarity evaluator that determines whether two values 
	/// are similar, dissimilar, or indeterminate using a <see cref="Triple"/> 
	/// decision.
	/// </summary>
	/// <typeparam name="T">The type of values being compared.</typeparam>
	public interface ISimilarity<T> {

		/// <summary>
		/// Evaluates the similarity between two values.
		/// </summary>
		Triple IsSimilar ( T a, T b );
	}

	/// <summary>
	/// Represents a threshold evaluator that maps a floating‑point value 
	/// to a <see cref="Triple"/> decision based on configured limits.
	/// </summary>
	public interface IThreshold {

		/// <summary>
		/// Evaluates the given value and returns a threshold‑based decision.
		/// </summary>
		Triple Evaluate ( float value );
	}

	
}
