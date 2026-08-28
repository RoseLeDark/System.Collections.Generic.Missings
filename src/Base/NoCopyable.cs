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

namespace SystemEx {
	/// \addtogroup SystemEx
	/// @{

	/// <summary>
	/// Base class that strictly forbids any form of copying.
	/// Pedantisch safe: no cloning, no copy constructor, no MemberwiseClone.
	/// </summary>
	public abstract class NoCopyable {
		/// <summary>
		/// Pedantisch safe: private copy constructor throws.
		/// Prevents accidental or reflection-based copying.
		/// </summary>
		private NoCopyable ( NoCopyable other ) {
			throw new NotSupportedException("Copy constructor is disabled.");
		}

		/// <summary>
		/// Protected default constructor.
		/// </summary>
		protected NoCopyable () {
		}

		/// <summary>
		/// Pedantisch safe: cloning is forbidden.
		/// </summary>
		public object Clone ()
			=> throw new NotSupportedException("Clone is disabled.");

		/// <summary>
		/// Pedantisch safe: MemberwiseClone is blocked.
		/// </summary>
		protected new object MemberwiseClone ()
			=> throw new NotSupportedException("MemberwiseClone is disabled.");
	}
	
}
