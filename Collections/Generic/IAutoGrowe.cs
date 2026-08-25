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

namespace SystemEx.Collections.Generic {
	/// \addtogroup Collections
	/// @{

	/// <summary>
	/// Defines a contract for collections that can automatically grow their
	/// internal storage when additional capacity is required.
	/// 
	/// <para>
	/// Implementations use <see cref="GrowSize"/> to determine how much the
	/// underlying buffer should expand and <see cref="AutoGrow"/> to control
	/// whether growth occurs implicitly during insertion.
	/// </para>
	/// 
	/// <para>
	/// The <see cref="Grow"/> method performs the actual capacity increase and
	/// returns <c>true</c> if the operation succeeded.
	/// </para>
	/// </summary>
	public interface IAutoGrowe {
		/// <summary>
		/// Gets or sets the number of elements by which the collection grows
		/// when additional capacity is needed.
		/// </summary>
		long GrowSize { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether the collection should
		/// automatically grow when full.
		/// </summary>
		bool AutoGrow { get; set; }
		/// <summary>
		/// Attempts to increase the collection's capacity according to
		/// <see cref="GrowSize"/>. Returns <c>true</c> if the growth succeeded.
		/// </summary>
		bool Grow ();

    }
	/// @}
}
