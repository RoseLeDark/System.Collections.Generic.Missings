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
	/// Basic backend implementation for all using models with this framework.
	/// </summary>
	/// <typeparam name="T">
	/// The type of the model's output and raw prompt.
	/// </typeparam>
	/// <typeparam name="TAI">The type of the model's output as TAI</typeparam>
	public interface IModelBackend<T , TAI> {

		/// <summary>
		/// Describes backend capabilities (text, chat, JSON, remote, etc.).
		/// </summary>
		Map<BackendCapabilities, object> Capabilities { get; }

		/// <summary>
		/// Stores backend configuration values such as the base URL.
		/// </summary>
		Map<string, object> Configuration { get; }

		/// <summary>
		/// Stores backend environment
		/// </summary>
		Map<SystemEx.AI.Environment, object> Enviro { get; }

        /// <summary>
        /// The backend name used for metadata and diagnostics.
        /// </summary>
        public string BackendName { get; }

        /// <summary>
        /// Gets or sets the identifier of the AI model that should be used by the backend.
        /// 
        /// This value determines which model the Windows AI Runtime will load when
        /// executing a request. It can be changed at any time during runtime, allowing
        /// dynamic switching between different AI models (e.g., "phi-3-mini",
        /// "phi-3-medium", "gpt-4o", etc.).
        /// 
        /// The <see cref="ModelName"/> is independent from the agent name used in the
        /// <see cref="Model{T}"/> constructor. While the agent name describes the
        /// logical assistant (e.g., "DevAssistant"), the <see cref="ModelName"/>
        /// specifies the actual AI model used for inference.
        /// </summary>
        /// <returns>
        /// A string representing the model identifier passed to the Windows AI Runtime.
        /// </returns>
        string ModelName { get; set; }

        /// <summary>
        /// Indicates whether the backend is available.
        /// The OS decides availability based on installed AI components.
        /// </summary>
        bool IsAvailable { get; }

		/// <summary>
		/// Regist a tool with this model
		/// </summary>
		/// <returns><c>true</c> when added or <c>false</c> when not</returns>
		bool RegistTool ( IModelTool<T> tool );
		/// <summary>
		/// Unregist a tool with this model
		/// </summary>
		/// <returns><c>true</c> when added or <c>false</c> when not</returns>
		bool UnregistTool ( string toolName );

        /// <summary>
        /// Get A list of all regist tools with this model
        /// </summary>
        IReadOnlyList<TAI> ListTools ();

		/// <summary>
		/// Initialization the model
		/// </summary>
		/// <param name="configuration">The configuration for this model.</param>
		/// <param name="environment">The enviroment for thie model.</param>
		/// <returns><c>true</c> when init or <c>false</c> when not</returns>
		bool Initialization ( Map<string, object> configuration, Map<Environment, object> environment );

		/// <summary>
		/// Executes the model by sending a payload to the model.
		/// </summary>
		/// <param name="systemPrompt">The system instruction for the model.</param>
		/// <param name="input">The user prompt and context.</param>
		/// <returns>
		/// A <see cref="ModelResult{T}"/> containing:
		/// - The parsed output
		/// - Metadata
		/// - Raw JSON
		/// - Error information (if any)
		/// </returns>
		Task<IModelResult<T>> InvokeAsync ( string systemPrompt, IModelPromp<T> input);

		/// <summary>
		/// Release the model
		/// </summary>
        void Release ( bool wait );

		/// <summary>
		/// Has this model tools
		/// </summary>
        bool HasTool ( string toolName );

		/// <summary>
		/// Get Value from config
		/// </summary>
		Optional<object> GetValue ( string key );
		/// <summary>
		/// Set config
		/// </summary>
		void SetConfig ( string key, object value );

    }

    /// @}
}
