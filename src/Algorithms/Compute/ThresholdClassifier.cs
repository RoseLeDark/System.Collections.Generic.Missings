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
	/// Implements a threshold‑based classifier using two boundaries:  
	/// - Values above the true threshold → <see cref="triple.True"/>  
	/// - Values below the false threshold → <see cref="triple.False"/>  
	/// - Values in between → <see cref="triple.Nin"/>
	/// </summary>
	public sealed class ThresholdClassifier : IThreshold {
        private readonly float m_trueThreshold;
        private readonly float m_falseThreshold;

		/// <summary>
		/// Initializes a new threshold classifier with the specified true and false limits.
		/// </summary>
		public ThresholdClassifier ( float trueThreshold, float falseThreshold ) {
            m_trueThreshold = trueThreshold;
            m_falseThreshold = falseThreshold;
        }
		/// <summary>
		/// Evaluates the given value and returns a three‑state threshold decision.
		/// </summary>
		public Triple Evaluate ( float value ) {
            if ( value >= m_trueThreshold ) return triple.True;
            if ( value <= m_falseThreshold ) return triple.False;
            return triple.Nin;
        }
    }
	
}
