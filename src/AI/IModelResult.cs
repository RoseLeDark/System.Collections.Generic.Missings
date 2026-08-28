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
	/// \addtogroup AI
	/// @{

	/// <summary>
	/// Represents the result produced by a model execution.  
	/// Provides access to the final output, metadata, raw backend information,
	/// error details, and execution status.
	/// 
	/// <para>
	/// An <see cref="IModelResult{T}"/> is returned by all model backends and 
	/// processing pipelines. It encapsulates both the structured result and 
	/// diagnostic information that may be relevant for logging, tracing, 
	/// debugging, or post‑processing.
	/// </para>
	/// </summary>
	/// <typeparam name="T">
	/// The type of the model's output (e.g., text, tokens, embeddings, objects).
	/// </typeparam>
	public interface IModelResult<T> {

		/// <summary>
		/// Gets the final output produced by the model.
		/// </summary>
		T Result { get; }

		/// <summary>
		/// Gets a metadata map containing execution details such as timing,
		/// backend identifiers, token counts, or pipeline‑specific annotations.
		/// </summary>
		Map<ModelMeta, object> Metadata { get; }

		/// <summary>
		/// Gets an optional exception describing an error that occurred during
		/// model execution.  
		/// If <see cref="Success"/> is true, this value is typically null.
		/// </summary>
		Optional<Exception> Error { get; }

		/// <summary>
		/// Gets the raw backend output, if available.  
		/// This may include unprocessed JSON, binary data, or internal model
		/// structures that were not converted into <typeparamref name="T"/>.
		/// </summary>
		Optional<object> Raw { get; }

		/// <summary>
		/// Gets a value indicating whether the model execution completed
		/// successfully without errors.
		/// </summary>
		bool Success { get; }

		/// <summary>
		/// Gets the timestamp (UTC) when the result was created.
		/// Useful for logging, tracing, and chronological ordering.
		/// </summary>
		DateTime Timestamp { get; }
	}

	
}
