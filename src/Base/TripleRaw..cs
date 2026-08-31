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

namespace SystemEx {
#pragma warning disable CS8981

	/// <summary>
	/// Represents a three-valued logic type with True, False, and Nin (neither true nor false) states.
	/// </summary>
	public enum triple : sbyte {

        /// <summary>
        /// The state representing true.
        /// </summary>
        True = 1,
        /// <summary>
        /// The state representing false.
        /// </summary>
        False = 0,
        /// <summary>
        /// The state representing neither true nor false.
        /// </summary>
        Nin = -1
    }
    
#pragma warning restore CS8981 
}
