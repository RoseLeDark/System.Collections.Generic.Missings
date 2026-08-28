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
using SystemEx.Collections.Generic;

namespace SystemEx.AI {
	/// \addtogroup AI
	/// @{

	/// <summary>
	/// Represents the result of an AI model execution.
	/// 
	/// A <see cref="ModelResult{T}"/> contains:
	/// - The final model output (<see cref="Result"/>)
	/// - Metadata describing execution details (<see cref="Metadata"/>)
	/// - Optional error information (<see cref="Error"/>)
	/// - Optional raw backend output (<see cref="Raw"/>)
	/// - A timestamp indicating when the result was produced (<see cref="Timestamp"/>)
	/// - A success flag (<see cref="Success"/>)
	/// 
	/// This struct is immutable and safe to pass by value.
	/// </summary>
	/// <typeparam name="T">
	/// The type of the model's output.
	/// </typeparam>
	public struct ModelResult<T> : IModelResult<T> {
        /// <summary>
        /// The final result produced by the model.
        /// </summary>
        private readonly T m_result;

        /// <summary>
        /// Metadata describing execution details such as duration, backend name,
        /// model identifier, tokens, raw output, etc.
        /// </summary>
        private Map<ModelMeta, object> m_metaData;

        /// <summary>
        /// Optional exception if the model execution failed.
        /// </summary>
        private readonly Optional<Exception> m_exection;

        /// <summary>
        /// Optional raw backend output (e.g., JSON, text, or internal runtime data).
        /// </summary>
        private readonly Optional<object> m_raw;

        /// <summary>
        /// Timestamp indicating when the result was created.
        /// </summary>
        private readonly DateTime m_time;

        /// <summary>
        /// Indicates whether the model execution succeeded.
        /// </summary>
        private readonly bool m_okkay;


        // ---------------------------------------------------------------------
        // Public properties
        // ---------------------------------------------------------------------

        /// <summary>
        /// Gets the final model output.
        /// </summary>
        public T Result => m_result;

        /// <summary>
        /// Gets the metadata associated with the model execution.
        /// </summary>
        public Map<ModelMeta, object> Metadata => m_metaData;

        /// <summary>
        /// Gets the optional error information.
        /// If <see cref="Success"/> is true, this value is null.
        /// </summary>
        public Optional<Exception> Error => m_exection;

        /// <summary>
        /// Gets the raw backend output, if available.
        /// </summary>
        public Optional<object> Raw => m_raw;

        /// <summary>
        /// Gets a value indicating whether the model execution succeeded.
        /// </summary>
        public bool Success => m_okkay;

        /// <summary>
        /// Gets the timestamp when the result was created (UTC).
        /// </summary>
        public DateTime Timestamp => m_time;


        // ---------------------------------------------------------------------
        // Constructor
        // ---------------------------------------------------------------------

        /// <summary>
        /// Creates a new <see cref="ModelResult{T}"/> instance.
        /// </summary>
        /// <param name="result">The final model output.</param>
        /// <param name="metadata">Execution metadata.</param>
        /// <param name="error">Optional error information.</param>
        /// <param name="raw">Optional raw backend output.</param>
        public ModelResult (
            T result,
            Map<ModelMeta, object> metadata,
            Optional<Exception> error,
            Optional<object> raw ) {
            m_result = result;
            m_metaData = metadata;
            m_exection = error;
            m_raw = raw;

            m_time = DateTime.UtcNow;

            // Success flag: true if no error was provided.
            m_okkay = error.IsNull;
        }
    }
    
}
