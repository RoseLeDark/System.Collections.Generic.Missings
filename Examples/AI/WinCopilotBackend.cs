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
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using SystemEx;
using SystemEx.AI;
using SystemEx.AI.Backend;
using SystemEx.Collections.Generic;

namespace ExampleAIWindowsBackend {

    

    /// <summary>
    /// Windows Global AI backend implementation.
    /// 
    /// This backend connects the SystemEX model layer to the Windows AI Runtime.
    /// It is generic over the prompt type <typeparamref name="T"/> and exposes
    /// runtime tools of type <see cref="AITool"/>.
    /// 
    /// By implementing <see cref="IModelBackend{TPrompt, TTool}"/>, the backend:
    /// - Accepts prompts of type <typeparamref name="T"/>
    /// - Registers internal tools and converts them into <see cref="AITool"/> objects
    /// - Forwards configuration values (API keys, runtime settings, etc.)
    /// - Executes model requests using the Windows AI Runtime
    /// - Returns structured <see cref="ModelResult{T}"/> objects with metadata
    /// 
    /// This design keeps the backend fully generic and allows other backends
    /// (Web, Azure, Local) to use different tool types while sharing the same
    /// model and tool infrastructure.
    /// </summary>
    /// <typeparam name="T">
    /// The prompt type used by the model (e.g., string, structured object).
    /// </typeparam>
    public class WinCopilotBackend<T> : IModelBackend<T, AITool> {
        private readonly IAIFunctionFactory<T, AITool> m_factory;
        private readonly List<AITool> m_runtimeTools;
        private readonly AIClient m_client;
        private string m_strModelName = "phi-3-mini";
        private Map<string, object> m_cfg;

        /// <summary>
        /// Describes backend capabilities such as text, vision, audio, GPU support, etc.
        /// </summary>
        public Map<BackendCapabilities, object> Capabilities { get; }

        /// <summary>
        /// Stores backend configuration values such as API keys, runtime settings, etc.
        /// </summary>
        public Map<string, object> Configuration => m_cfg;


        /// <summary>
        /// The backend name used for metadata and diagnostics.
        /// </summary>
        public string BackendName => "WindowsGlobalAI";

        /// <summary>
        /// Indicates whether the backend is available.
        /// The OS decides availability based on installed AI components.
        /// </summary>
        public bool IsAvailable => true;

        /// <summary>
        /// The used Model
        /// </summary>
        public string ModelName {
            get => m_strModelName;
            set => m_strModelName = value;
        }


        /// <summary>
        /// Creates a new Windows Global AI backend instance.
        /// </summary>
        /// <param name="ApiKey">
        /// API key used for authenticated model access.
        /// Stored in <see cref="Configuration"/> and applied during <see cref="Begin"/>.
        /// </param>
        /// <param name="factory">
        /// Factory used to convert internal tools into runtime‑compatible <see cref="AITool"/> objects.
        /// </param>
        public WinCopilotBackend ( IAIFunctionFactory<T, AITool> factory ) {

            m_client = new AIClient(); // Windows KI Runtime
            m_factory = factory;
            m_runtimeTools = new List<AITool>();

            Capabilities = new Map<BackendCapabilities, object>
            {
                [BackendCapabilities.AI_BACKEND_CAPS_TEXT] = 1,
                [BackendCapabilities.AI_BACKEND_CAPS_VISION] = 1,
                [BackendCapabilities.AI_BACKEND_CAPS_AUDIO] = 1,
                [BackendCapabilities.AI_BACKEND_CAPS_TOOLS] = 1,

                [BackendCapabilities.AI_BACKEND_CAPS_LOCAL] = 1,
                [BackendCapabilities.AI_BACKEND_CAPS_GPU] = 1,
                [BackendCapabilities.AI_BACKEND_CAPS_RUNTIME_LOCAL] = 1,
                [BackendCapabilities.AI_BACKEND_CAPS_TRANSPORT_NATIVE] = 1,

                [BackendCapabilities.AI_PLATFORM_CAP_WINDOWS] = 1,
            };
            m_cfg = new Map<string, object>();
        }
        /// <summary>
        /// Applies configuration values to the Windows AI Runtime.
        /// Called automatically by <see cref="Model{T}.Begin"/>.
        /// </summary>
        /// <param name="configuration">
        /// Configuration values provided by the model.
        /// </param>
        public bool Initialization ( Map<string, object> configuration, Map<SystemEx.AI.Environment, object> environment ) {

            if ( configuration.TryGetValue("WCP_BACKEND_API_KEY", out var keyObj) &&
                keyObj is string apiKey && !string.IsNullOrWhiteSpace(apiKey) ) {

                m_client.SetConfig("API_KEY", apiKey);
             }

            m_cfg.PushBack(configuration);

            if ( environment.TryGetValue(SystemEx.AI.Environment.AI_ENV_OS_WINDOWS, out var isWindows) && isWindows is bool win && win ) {
                m_client.SetConfig("RUNTIME_MODE", "WINDOWS_NATIVE");
            }

            if ( environment.TryGetValue(SystemEx.AI.Environment.AI_ENV_HW_GPU, out var gpuObj) && gpuObj is bool hasGpu && hasGpu ) {
                m_client.SetConfig("USE_GPU", true);
            }
            if ( environment.TryGetValue(SystemEx.AI.Environment.AI_ENV_FS_TEMP_DIR, out var tempObj) && tempObj is string tempPath ) {
                m_client.SetConfig("TEMP_DIR", tempPath);
            }
            if ( environment.TryGetValue(SystemEx.AI.Environment.AI_ENV_LOCALE, out var localeObj) && localeObj is string locale ) {
                m_client.SetConfig("LOCALE", locale);
            }
            return true;

        }
        /// <summary>
        /// Finalizes backend operations.
        /// </summary>
        /// <param name="wait">
        /// If true, waits until all runtime tasks have completed.
        /// </param>
        public void Release ( bool wait ) {
            if ( wait ) {
                // Warten bis alle Tasks der Runtime abgeschlossen sind
                m_client.WaitForCompletion();
            }
        }

        /// <summary>
        /// Executes a model request using the Windows AI Runtime.
        /// </summary>
        /// <param name="systemPrompt">The system instruction for the model.</param>
        /// <param name="input">The user prompt and context.</param>
        /// <returns>
        /// A <see cref="ModelResult{T}"/> containing:
        /// - The model output  
        /// - Metadata  
        /// - Raw runtime response  
        /// - Error information (if any)  
        /// </returns>
        public async Task<IModelResult<T>> InvokeAsync ( string systemPrompt,  IModelPromp<T> input )
        {
            try {
                var execStart = DateTime.UtcNow;
                var sw = Stopwatch.StartNew();
                

                var request = new AIRequest
                {
                    Model = ModelName,
                    SystemPrompt = systemPrompt,
                    Input = input.Prompt,
                    Context = input.Context,
                    Parameters = input.Parameters,
                    Tools = m_runtimeTools
                };

                var response = await m_client.GenerateAsync(request);
                sw.Stop();

                var execEnd  = DateTime.UtcNow;

                return new ModelResult<T>(
                    result: (T)response.Output,
                    metadata: new Map<ModelMeta, object>
                    {


                        [ModelMeta.AI_META_TOKENS_OUT] = response.Tokens,
                        [ModelMeta.AI_META_TOKENS_IN] = response.InputTokens,
                        [ModelMeta.AI_META_TOKENS_TOTAL] = response.TotalTokens,

                        [ModelMeta.AI_META_SESSION_ID] = input.SessionId,
                        [ModelMeta.AI_META_MODEL_PARAMETERS] = input.Parameters,
                        [ModelMeta.AI_META_TOOLS_USED] = m_runtimeTools.Select(t => t.Name).ToList(),
                        [ModelMeta.AI_META_SYSTEMPROMPT_HASH] = systemPrompt.GetHashCode(),

                        [ModelMeta.AI_META_BACKEND_CONFIG] = Configuration,

                        [ModelMeta.AI_META_DURATION] = response.Duration,
                        [ModelMeta.AI_META_EXECUTION_MODE] = response.ExecutionMode,

                        [ModelMeta.AI_META_EXEC_START] = execStart,
                        [ModelMeta.AI_META_EXEC_END] = execEnd,
                        [ModelMeta.AI_META_EXEC_TIME] = sw.ElapsedMilliseconds,

                        [ModelMeta.AI_META_BACKEND] = BackendName,
                        [ModelMeta.AI_META_BACKEND_TYPE] = "WindowsAI",
                        [ModelMeta.AI_META_PLATFORM] = "Windows",

                        [ModelMeta.AI_META_MODEL] = ModelName,
                        [ModelMeta.AI_META_MODEL_VERSION] = m_client.Version,
                        [ModelMeta.AI_META_MODEL_CAPS] = Capabilities,

                        [ModelMeta.AI_META_RUNTIME_VERSION] = m_client.RuntimeVersion,
                        [ModelMeta.AI_META_CLIENT_VERSION] = m_client.Version,

                        [ModelMeta.AI_META_FORMAT] = MetaFormat.AI_FORMAT_TEXT,


                    },
                    error: null,
                    raw: response.Raw
                );
            } catch ( Exception ex ) {
               
                return new ModelResult<T>(
                    result: default!,
                    metadata: new Map<ModelMeta, object>() {

                        [ModelMeta.AI_META_SESSION_ID] = input.SessionId,
                        [ModelMeta.AI_META_BACKEND_CONFIG] = Configuration,
                        [ModelMeta.AI_META_TOOLS_USED] = m_runtimeTools.Select(t => t.Name).ToList(),

                        [ModelMeta.AI_META_MODEL] = ModelName,
                        [ModelMeta.AI_META_MODEL_VERSION] = m_client.Version,
                        [ModelMeta.AI_META_MODEL_CAPS] = Capabilities,
                        [ModelMeta.AI_META_MODEL_PARAMETERS] = input.Parameters,

                        [ModelMeta.AI_META_BACKEND] = BackendName,
                        [ModelMeta.AI_META_BACKEND_TYPE] = "WindowsAI",
                        [ModelMeta.AI_META_PLATFORM] = "Windows",

                        [ModelMeta.AI_META_SYSTEMPROMPT_HASH] = systemPrompt.GetHashCode(),

                        [ModelMeta.AI_META_EXEC_START] = DateTime.UtcNow,          // Startzeit
                        [ModelMeta.AI_META_EXEC_END] = DateTime.UtcNow,          // Fehlerzeit
                        [ModelMeta.AI_META_EXEC_TIME] = 0,                        // Fehler → 0 oder gemessen

                        [ModelMeta.AI_META_TOKENS_IN] = 0,
                        [ModelMeta.AI_META_TOKENS_OUT] = 0,
                        [ModelMeta.AI_META_TOKENS_TOTAL] = 0,

                        [ModelMeta.AI_META_FORMAT] = MetaFormat.AI_FORMAT_TEXT,
                        [ModelMeta.AI_META_TRACE_ID] = Guid.NewGuid().ToString(),

                        [ModelMeta.AI_META_ERROR_MESSAGE] = ex.Message,
                        [ModelMeta.AI_META_ERROR_TYPE] = ex.GetType().Name,
                        [ModelMeta.AI_META_ERROR_STACK] = ex.StackTrace ?? "",
                        [ModelMeta.AI_META_ERROR_CODE] = ex.HResult,



                    },
                    error: ex,
                    raw: null
                );
            }
        }
        /// <summary>
        /// Registers a tool and exposes it to the AI runtime.
        /// </summary>
        /// <param name="tool">The internal tool definition.</param>
        /// <returns>True if the tool was successfully registered.</returns>
        public bool RegistTool ( IModelTool<T> tool ) {
            AITool runtimeTool;

            if( m_factory.Convert(tool, out runtimeTool) )   // IModelTool<T> → AITool
                m_runtimeTools.Add(runtimeTool);

            return true;
        }
        /// <summary>
        /// Removes a tool from the runtime tool list.
        /// </summary>
        /// <param name="toolName">The name of the tool to remove.</param>
        /// <returns>True if the tool was found and removed.</returns>
        public bool UnregistTool ( string toolName ) {
            var idx = m_runtimeTools.FindIndex(t => t.Name == toolName);
            if ( idx < 0 ) return false;
            m_runtimeTools.RemoveAt(idx);
            return true;
        }
        /// <summary>
        /// Is the tool with name <paramref name="toolName"/> exposes to the AI runtime.
        /// </summary>
        /// <param name="toolName">The Name of the tool.</param>
        /// <returns>True if the tool was found.</returns>
        public bool HasTool ( string toolName ) {
            var idx = m_runtimeTools.FindIndex(t => t.Name == toolName);
            return (idx >= 0);
        }

        /// <summary>
        /// Returns a read‑only list of all registered tools.
        /// </summary>
        /// <returns>A read‑only list of <see cref="AITool"/> objects.</returns>
        public IReadOnlyList<AITool> ListTools ()
            => m_runtimeTools.AsReadOnly();

        
    }
}
