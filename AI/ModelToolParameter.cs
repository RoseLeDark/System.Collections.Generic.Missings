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


namespace SystemEx.AI {

	/// \addtogroup SystemEx.AI
	/// @{
	/// \addtogroup SystemEx.AI
	/// @{
	/// <summary>
	/// Represents a single parameter definition used by an 
	/// <see cref="IModelTool{T}"/>.  
	/// 
	/// <para>
	/// A <see cref="ModelToolParameter"/> describes the name, .NET type, and 
	/// human‑readable description of a tool parameter.  
	/// Tool factories and AI backends use this metadata to construct parameter 
	/// schemas, validate input, and expose structured tool interfaces to models.
	/// </para>
	/// </summary>
	public readonly struct ModelToolParameter {

		/// <summary>
		/// Gets the unique parameter name used to identify the value in 
		/// argument dictionaries and tool schemas.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets the .NET type of the parameter.  
		/// This determines how values are validated, serialized, and passed 
		/// into the tool execution pipeline.
		/// </summary>
		public Type Type { get; }

		/// <summary>
		/// Gets a human‑readable description of the parameter's purpose.  
		/// Used by backends, documentation systems, and model‑side tool introspection.
		/// </summary>
		public string Description { get; }

		/// <summary>
		/// Initializes a new parameter definition with the specified name, type, 
		/// and description.
		/// </summary>
		/// <param name="name">The parameter name.</param>
		/// <param name="type">The .NET type of the parameter.</param>
		/// <param name="description">A human‑readable description.</param>
		public ModelToolParameter ( string name, Type type, string description ) {
			Name = name;
			Type = type;
			Description = description;
		}
	}
	/// @}
}
