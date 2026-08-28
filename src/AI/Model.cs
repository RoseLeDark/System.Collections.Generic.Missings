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
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using SystemEx.Collections.Generic;
using AIEnv = SystemEx.AI.Environment;

namespace SystemEx.AI {
	/// \addtogroup AI
	/// @{

	/// <summary>
	/// Represents a generic AI model instance within the SystemEX framework.
	/// 
	/// This class acts as the high‑level controller for:
	/// - Model configuration (capabilities, environment, runtime settings)
	/// - Tool registration and management
	/// - Backend interaction (Begin, RunAsync, End)
	/// - Prompt execution and session handling
	/// 
	/// The model is generic over:
	/// <typeparamref name="T"/>     → The prompt/input type
	/// <typeparamref name="TTOOL"/> → The runtime tool type used by the backend
	/// 
	/// A model does NOT execute AI logic itself. Instead, it delegates all
	/// execution to the backend (<see cref="IModelBackend{T, TTOOL}"/>).
	/// </summary>
	public class Model<T, TTOOL> : IModel<T, TTOOL> {
        /// <summary>
        /// Internal list of tools registered for this model.
        /// Tools are forwarded to the backend when added.
        /// </summary>
        private readonly List<IModelTool<T>> m_tools;


        /// <summary>
        /// Configuration map containing runtime settings (API keys, model options, etc.).
        /// </summary>
        private Map<string, object> m_cfg;

        /// <summary>
        /// State map used to store dynamic runtime information.
        /// </summary>
        private Map<string, object> m_states;

        /// <summary>
        /// Environment map describing external conditions or metadata.
        /// </summary>
        private Map<Environment, object> m_env;

        /// <summary>
        /// Backend responsible for executing model requests.
        /// </summary>
        private readonly IModelBackend<T, TTOOL> m_backend;

        /// <summary>
        /// Logical name of the model instance (e.g., "DevAssistant").
        /// </summary>
        private string m_strName;

        /// <summary>
        /// The system prompt used by the model.
        /// </summary>
        private string m_strPrompt;

        // ---------------------------------------------------------------------
        // Public properties
        // ---------------------------------------------------------------------

        /// <summary>
        /// Gets the system prompt used by the model.
        /// This is typically a static instruction or initialization text.
        /// </summary>
        public string SystemPrompt { get => m_strPrompt; set => m_strPrompt = value; }

        /// <summary>
        /// Gets the logical name of this model instance.
        /// </summary>
        public string Name => m_strName;


        /// <summary>
        /// Configuration map containing runtime settings.
        /// </summary>
        public Map<string, object> Configuration => m_cfg;

        /// <summary>
        /// State map used for dynamic runtime information.
        /// </summary>
        public Map<string, object> State => m_states;

        /// <summary>
        /// Backend responsible for executing model requests.
        /// </summary>
        public IModelBackend<T, TTOOL> Backend => m_backend;

        /// <summary>
        /// Optional cancellation token used to cancel long‑running operations.
        /// </summary>
        public CancellationToken? Cancel { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the AI model used by the backend.
        /// 
        /// This value determines which model the backend will load during execution.
        /// It can be changed at any time, allowing dynamic switching between models.
        /// </summary>
        /// <returns>
        /// A string representing the model identifier passed to the backend.
        /// </returns>
        public string ModelName {
            get => m_backend.ModelName;
            set => m_backend.ModelName = value;
        }
        /// <summary>
        /// The Environment
        /// </summary>
        public Map<Environment, object> Environment { get => m_env; protected set => m_env = value; }

        /// <summary>
        /// Creates a new model instance with default capability, configuration,
        /// state, and environment maps.
        /// </summary>
        /// <param name="name">Logical name of the model instance.</param>
        /// <param name="strModelName">AI model identifier used by the backend. Add to config and to backend</param>
        /// <param name="backend">Backend responsible for executing requests.</param>
        /// <param name="strPrompt">"Ich bin eine KI die nur so schalu is t wie 42"</param>
        public Model ( string name, string strModelName, string strPrompt, IModelBackend<T, TTOOL> backend) {

            m_strName = name;
            m_backend = backend;
            m_backend.ModelName = strModelName;

            m_cfg = new Map<string, object>();
            m_states = new Map<string, object>();
            m_env = new Map<AIEnv, object>();
            m_strPrompt = strPrompt;

            m_tools = new List<IModelTool<T>>();
        }


        // ---------------------------------------------------------
        // Capability Indexer
        // ---------------------------------------------------------

        /// <summary>
        /// Checks whether a capability exists in the capability map.
        /// </summary>
        /// <param name="capability">Capability key.</param>
        /// <returns>True if the capability exists; otherwise false.</returns>
        public bool this[BackendCapabilities capability] {
            get {
                return HaveCap(capability);
            }
        }
        /// <summary>
        /// Checks whether the model has a specific capability.
        /// </summary>
        /// <param name="pCapabilities">Capability key.</param>
        /// <returns>True if the capability exists.</returns>
        public bool HaveCap ( BackendCapabilities pCapabilities ) {
            object? _value;
            bool _ret = false;

            // Wenn der Key nicht existiert → Capability nicht vorhanden
            if ( !Backend.Capabilities.TryGetValue(pCapabilities, out _value) )
                return false;

            if(_value != null ) {
                if ( _value is int i ) _ret = i >= 1;
                if ( _value is bool b ) _ret = b;
                if ( _value is string stt ) _ret = stt.Contains("true", StringComparison.CurrentCultureIgnoreCase);
            }

            return _ret;
        }

        // ---------------------------------------------------------
        // Configuration
        // ---------------------------------------------------------
        /// <summary>
        /// Adds or updates a configuration value.
        /// </summary>
        /// <param name="key">Configuration key.</param>
        /// <param name="value">Configuration value.</param>
        /// <returns>Always true.</returns>
        public bool AddConfig ( string key, object value ) {
            Backend.SetConfig(key, value);
            return Backend.GetValue(key).HasValue;
        }

        /// <summary>
        /// Retrieves a configuration value.
        /// </summary>
        /// <param name="key">Configuration key.</param>
        /// <param name="value">Output value.</param>
        /// <returns>True if the key exists; otherwise false.</returns>
        public bool GetConfigValue ( string key, ref object value ) {
            bool _ret = false;
           Optional<object> _raw = Backend.GetValue(key);

           if(_raw.IsSome) {
                value = _raw.Value!;
                _ret = true;
           }
            return _ret;
        }

        // ---------------------------------------------------------
        // Tool Management
        // ---------------------------------------------------------
        /// <summary>
        /// Registers a tool with the backend.
        /// </summary>
        /// <param name="tool">Tool to register.</param>
        /// <returns>True if registration succeeded.</returns>
        public bool AddTool ( IModelTool<T> tool )
            => Backend.RegistTool(tool);

        /// <summary>
        /// Removes a tool from the backend.
        /// </summary>
        /// <param name="toolName">Name of the tool to remove.</param>
        /// <returns>True if the tool was removed.</returns>
        public bool RemoveTool ( string toolName )
            => Backend.UnregistTool(toolName);

        /// <summary>
        /// Checks whether a tool is registered.
        /// </summary>
        /// <param name="toolName">Tool name.</param>
        /// <returns>True if the tool exists.</returns>
        public bool HasTool ( string toolName )
            => Backend.HasTool(toolName);


        /// <inheritdoc/>
        public bool Initialization ( Map<string, object> configuration ) {
            // Set env
            Map<AIEnv, object> environment = new Map<AIEnv, object>();

            // OS
            if ( OperatingSystem.IsWindows() )
                environment.PushBack(AIEnv.AI_ENV_OS_WINDOWS, true);
            else if ( OperatingSystem.IsLinux() )
                environment.PushBack(AIEnv.AI_ENV_OS_LINUX, true);
            else if ( OperatingSystem.IsMacOS() )
                environment.PushBack(AIEnv.AI_ENV_OS_MACOS, true);

            // Architecture
            if ( RuntimeInformation.OSArchitecture == Architecture.X64 )
                environment.PushBack(AIEnv.AI_ENV_ARCH_X64, true);
            else if ( RuntimeInformation.OSArchitecture == Architecture.Arm64 )
                environment.PushBack(AIEnv.AI_ENV_ARCH_ARM64, true);

            // Runtime
            environment.PushBack(AIEnv.AI_ENV_RT_DOTNET, System.Environment.Version.ToString());

            // Paths
            environment.PushBack(AIEnv.AI_ENV_FS_TEMP_DIR, Path.GetTempPath());
            environment.PushBack(AIEnv.AI_ENV_FS_WORK_DIR, System.Environment.CurrentDirectory);
            environment.PushBack(AIEnv.AI_ENV_FS_HOME_DIR, System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile));

            // Locale / Culture
            environment.PushBack(AIEnv.AI_ENV_LOCALE, CultureInfo.CurrentCulture.Name);
            environment.PushBack(AIEnv.AI_ENV_CULTURE, CultureInfo.CurrentCulture.DisplayName);

            // Network (simple check)
            bool online = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
            environment.PushBack(online ? AIEnv.AI_ENV_NET_ONLINE : AIEnv.AI_ENV_NET_OFFLINE, true);

            // Hardware (basic detection)
            environment.PushBack(AIEnv.AI_ENV_HW_CPU, true);

            return Initialization(configuration, environment);
        }

        /// <inheritdoc/>
        public bool Initialization ( Map<string, object> configuration, Map<Environment, object> environment ) {
            return Backend.Initialization(configuration, environment);
        }

        /// <summary>
        /// Executes the model asynchronously using the backend.
        /// </summary>
        /// <param name="input">Prompt and context information.</param>
        /// <returns>Model result including metadata and raw output.</returns>
        public Task<IModelResult<T>> RunAsync ( IModelPromp<T> input )
            => Backend.InvokeAsync(SystemPrompt, input);

        /// <summary>
        /// Finalizes backend execution.
        /// </summary>
        public void Release ( ) {
            Backend.Release(true);
        }
    }
    
}
