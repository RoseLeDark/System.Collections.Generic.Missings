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

using SystemEx.Numeric;

namespace SystemEx.Algorithms.Compute {
	/// \addtogroup SystemEx.Algorithms.Compute
	/// @{
	/// <summary>
	/// Implements cosine similarity for <see cref="ICVector"/> instances.  
	/// Returns a normalized similarity score in the range [-1, 1].
	/// </summary>
	public sealed class CosineDistanceF : ICDistance {
		/// <summary>
		/// Computes the cosine similarity between two vectors.
		/// </summary>
		public float Compute ( ICVector a, ICVector b ) {
            float dot = 0f, magA = 0f, magB = 0f;

            for ( int i = 0 ; i < a.Dimension ; i++ ) {
                float x = a[i];
                float y = b[i];

                dot += x * y;
                magA += x * x;
                magB += y * y;
            }

            return dot / (MathF.Sqrt(magA) * MathF.Sqrt(magB));
        }
    }
	///@}

}
