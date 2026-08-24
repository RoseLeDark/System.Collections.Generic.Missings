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

using SystemEx.Collections.Generic;

namespace SystemEx.AI {
	/// \addtogroup SystemEx.AI
	/// @{
	/// <summary>
	/// Represents a model‑executable tool that can be exposed to AI backends
	/// through an <see cref="IAIFunctionFactory"/>.  
	/// 
	/// <para>
	/// An <see cref="IModelTool{T}"/> defines a named operation with a description,
	/// a parameter schema, and an asynchronous execution method.  
	/// Model backends may convert these tools into runtime‑callable functions
	/// when supported, enabling structured tool‑use inside model pipelines.
	/// </para>
	/// </summary>
	/// <typeparam name="T">
	/// The prompt type used by the model backend when invoking the tool.
	/// </typeparam>
	public interface IModelTool<T> {

		/// <summary>
		/// Gets the unique name of the tool.  
		/// This identifier is used by model backends and tool factories
		/// to expose the tool to the model runtime.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets a human‑readable description of the tool's purpose and behavior.  
		/// Used by backends to generate tool metadata or documentation.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Returns the parameter schema describing all inputs accepted by the tool.  
		/// Each parameter defines a name, type, and optional constraints.
		/// </summary>
		IEnumerable<ModelToolParameter> GetParameters ();

		/// <summary>
		/// Executes the tool asynchronously using the provided argument dictionary.  
		/// The backend supplies validated parameters and a cancellation token.  
		/// The returned object must be serializable or convertible into a model‑usable form.
		/// </summary>
		/// <param name="args">
		/// A map containing the tool parameters mapped by name.
		/// </param>
		/// <param name="ct">
		/// A cancellation token used to abort execution if required by the backend.
		/// </param>
		/// <returns>
		/// A task producing the tool's result, or <c>null</c> if no output is generated.
		/// </returns>
		Task<object?> ExecuteAsync ( Map<string, object?> args, CancellationToken ct );
	}

	///@}
}
