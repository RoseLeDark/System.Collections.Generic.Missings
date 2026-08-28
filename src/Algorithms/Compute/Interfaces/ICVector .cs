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
	/// Represents a read‑only computational vector with a fixed dimension.  
	/// Provides indexed access to floating‑point components.
	/// </summary>
	public interface ICVector {

		/// <summary>
		/// Gets the number of dimensions of the vector.
		/// </summary>
		int Dimension { get; }

		/// <summary>
		/// Gets the component value at the specified index.
		/// </summary>
		float this[int index] { get; }
	}

	
}
