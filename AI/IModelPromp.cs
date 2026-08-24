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
	/// Represents a model‑agnostic prompt container used by all 
	/// <see cref="IModelBackend"/> implementations and embedded models.
	/// 
	/// <para>
	/// An <see cref="IModelPromp{T}"/> encapsulates the raw prompt, contextual 
	/// metadata, session information, backend parameters, and optional tags.  
	/// It provides a unified structure for passing input into any model pipeline,
	/// regardless of modality or backend type.
	/// </para>
	/// </summary>
	/// <typeparam name="T">
	/// The underlying prompt type (e.g., text, tokens, bytes, AST, etc.).
	/// </typeparam>
	public interface IModelPromp<T> {
		/// <summary>
		/// Gets the raw prompt data supplied to the model.
		/// This may represent text, tokens, binary data, or any other prompt format.
		/// </summary>
		public T Prompt { get; }
		/// <summary>
		/// Gets a metadata map containing contextual information such as 
		/// temperature, top‑p, tool hints, memory handles, or session‑level settings.
		/// </summary>
		public Map<string, object> Context { get; }
		/// <summary>
		/// Gets the optional session identifier used to maintain conversational 
		/// continuity across multiple model invocations.
		/// </summary>
		public Optional<string> SessionId { get; }
		/// <summary>
		/// Gets a map of backend‑specific parameters that may influence execution, 
		/// routing, or tool behavior.
		/// </summary>
		public Map<string, object> Parameters { get; }
		/// <summary>
		/// Gets a tag map used for classification, annotation, or pipeline‑level 
		/// processing. Tags do not affect model execution directly but may be used 
		/// by tools or middleware.
		/// </summary>
		public Map<string, object> Tags { get; }
		/// <summary>
		/// Gets a value indicating whether the prompt should be cancelled before 
		/// reaching the backend. Useful for pre‑processing tools or pipeline guards.
		/// </summary>
		public bool Cancel { get; }
		/// <summary>
		/// Gets or sets a backend parameter by name.  
		/// Provides convenient indexed access to <see cref="Parameters"/>.
		/// </summary>
		/// <param name="parameter">The parameter key.</param>
		/// <returns>The associated parameter value.</returns>
		object this[string parameter] { get; set; }
    }
    /// @}
}
