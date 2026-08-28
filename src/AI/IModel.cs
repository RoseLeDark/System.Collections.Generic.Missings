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
	/// Represents a generic AI model with configuration, environment information,
	/// backend integration, tool support, and execution capabilities.
	/// </summary>
	/// <typeparam name="T">The primary data type used by the model (input/output).</typeparam>
	/// <typeparam name="TTOOL">The tool type supported by the model.</typeparam>
	public interface IModel<T, TTOOL> {
        /// <summary>
        /// Gets the model's unique name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the system prompt used by the model.
        /// This is typically a static instruction or initialization text.
        /// </summary>
        string SystemPrompt { get; }

        /// <summary>
        /// Gets the backend‑specific configuration map.
        /// Configuration values are backend‑defined and may vary between systems.
        /// </summary>
        Map<string, object> Configuration { get; }

        /// <summary>
        /// Gets the environment map describing the client machine.
        /// This includes OS, architecture, hardware, paths, runtime, and other system properties.
        /// </summary>
        Map<Environment, object> Environment { get; }

        /// <summary>
        /// Gets the backend implementation used by this model.
        /// </summary>
        IModelBackend<T, TTOOL> Backend { get; }

        /// <summary>
        /// Indexer that checks whether the model supports a specific backend capability.
        /// </summary>
        /// <param name="capabilities">The capability key to check.</param>
        /// <returns>True if the capability exists and is enabled.</returns>
        bool this[BackendCapabilities capabilities] { get; }

        /// <summary>
        /// Adds or updates a configuration value.
        /// </summary>
        /// <param name="key">Configuration key.</param>
        /// <param name="value">Configuration value.</param>
        /// <returns>True if the value was added or updated successfully.</returns>
        bool AddConfig ( string key, object value );

        /// <summary>
        /// Retrieves a configuration value.
        /// </summary>
        /// <param name="key">Configuration key.</param>
        /// <param name="value">Reference to the output value.</param>
        /// <returns>True if the key exists and the value was retrieved.</returns>
        bool GetConfigValue ( string key, ref object value );

        /// <summary>
        /// Checks whether the model has a specific backend capability.
        /// </summary>
        /// <param name="pCapabilities">Capability key.</param>
        /// <returns>True if the capability exists and is enabled.</returns>
        bool HaveCap ( BackendCapabilities pCapabilities );

        /// <summary>
        /// Adds a tool to the model.
        /// </summary>
        /// <param name="tool">Tool instance.</param>
        /// <returns>True if the tool was added successfully.</returns>
        bool AddTool ( IModelTool<T> tool );

        /// <summary>
        /// Removes a tool from the model by name.
        /// </summary>
        /// <param name="toolName">Name of the tool.</param>
        /// <returns>True if the tool was removed.</returns>
        bool RemoveTool ( string toolName );

        /// <summary>
        /// Checks whether a tool with the given name exists.
        /// </summary>
        /// <param name="toolName">Tool name.</param>
        /// <returns>True if the tool is present.</returns>
        bool HasTool ( string toolName );


        /// <summary>
        /// Initializes the model using the provided configuration and set the environment data automatic.
        /// This method is called before the first execution and allows the backend
        /// to prepare resources, validate settings, and adapt to the client's system.
        /// </summary>
        /// <param name="configuration">
        /// Backend‑specific configuration values (API keys, URLs, model parameters, etc.).
        /// </param>
        bool Initialization ( Map<string, object> configuration );

        /// <summary>
        /// Initializes the model using the provided configuration and environment data.
        /// This method is called before the first execution and allows the backend
        /// to prepare resources, validate settings, and adapt to the client's system.
        /// </summary>
        /// <param name="configuration">
        /// Backend‑specific configuration values (API keys, URLs, model parameters, etc.).
        /// </param>
        /// <param name="environment">
        /// Environment information describing the client machine (OS, hardware, paths, runtime).
        /// </param>
        bool Initialization ( Map<string, object> configuration, Map<Environment, object> environment );

        /// <summary>
        /// Executes the model asynchronously using the provided prompt.
        /// </summary>
        /// <param name="input">Model input prompt and context.</param>
        /// <returns>A task that produces the model result.</returns>
        Task<IModelResult<T>> RunAsync ( IModelPromp<T> input );

        /// <summary>
        /// Releases all resources associated with the model.
        /// </summary>
        void Release ();
    }

	
}
