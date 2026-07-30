using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SystemEx.Collections.Generic;

namespace SystemEx.AI.Backend {

    // \addtogroup AI
    /// @{
    /// 
    /// <summary>
    /// backend implementation for AI models that communicate with
    /// remote Web‑API services using HTTP POST requests.
    /// 
    /// This backend:
    /// - Serializes model input into UserFormat
    /// - Sends it to a remote endpoint
    /// - Receives responses
    /// - Deserializes the result into <typeparamref name="T"/>
    /// 
    /// It is designed for free or public AI endpoints that do not require
    /// authentication or advanced runtime integration.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the model's output and raw prompt.
    /// </typeparam>
    public class WebAIBackend<T> : IModelBackend<T, Object> {
        public const string WB_CONFIG_MAX_TOKENS           = "WEBAI_MAX_TOKENS";
        public const string WB_CONFIG_TEMPERATUR           = "WEBAI_TEMPERATURE";
        public const string WB_CONFIG_TOP_P                = "WEBAI_TOP_P";
        public const string WB_CONFIG_RESPONSE_FORMAT      = "WEBAI_RESPONSE_FORMAT";
        public const string WB_CONFIG_STREAM               = "WEBAI_STREAM";
        public const string WB_CONFIG_URL                  = "WB_BACKEND_CONFIG_URL";

        Map<BackendCapabilities, object> m_caps;
        private Map<string, object> m_configuration;
        private Map<Environment, object> m_environment;


        /// <summary>
        /// Describes backend capabilities (text, chat, JSON, remote, etc.).
        /// </summary>
        public Map<BackendCapabilities, object> Capabilities => m_caps;

        /// <summary>
        /// Stores backend configuration values such as the base URL.
        /// </summary>
        public Map<string, object> Configuration { get => m_configuration; set => m_configuration = value; }

        
        public Map<SystemEx.AI.Environment, object> Enviro { get => m_environment; set => m_environment = value; }

        /// <summary>
        /// The backend name used for metadata and diagnostics.
        /// </summary>
        public string BackendName => "FreeWebAI";

        /// <summary>
        /// Indicates whether the backend is available.
        /// Always true for Web‑API based backends.
        /// </summary>
        public bool IsAvailable => true;

        /// <summary>
        /// Web‑API Modelname
        /// </summary>
        public virtual string ModelName { get; set; } = "default";


        /// <summary>
        /// Internal HTTP client used for sending requests.
        /// </summary>
        private readonly HttpClient m_http;

        /// <summary>
        /// Set config
        /// </summary>
        public void SetConfig ( string key, object value ) {
            m_configuration[key] = value;
        }

        /// <summary>
        /// Get Value fraom config
        /// </summary>
        public Optional<object> GetValue ( string key ) {
            return m_configuration.Get(key);
        }

        /// <summary>
        /// Creates the final URL used for the HTTP request.
        /// 
        /// This method is <see langword="virtual"/> so that derived backends may
        /// customize how the request URL is constructed. Typical use cases include:
        /// - Appending query parameters
        /// - Switching between multiple endpoints
        /// - Adding API keys or version identifiers
        /// - Routing based on payload content
        /// 
        /// The default implementation simply returns <see cref="WebAIBackendPayload{T}.URL"/>,
        /// making the backend functional without requiring subclass overrides.
        /// </summary>
        /// <param name="payload">
        /// The payload that will be sent to the backend. This may contain additional
        /// information that derived classes can use to construct dynamic URLs.
        /// </param>
        /// <returns>
        /// The fully constructed URL used for the HTTP POST request.
        /// </returns>
        public virtual string OnCreateURL ( WebAIBackendPayload<T> payload ) {
            return $"{payload.URL}?model={ModelName}";
        }

        /// <summary>
        /// Serializes the payload into JSON and wraps it in a <see cref="StringContent"/>
        /// suitable for HTTP POST requests.
        /// </summary>
        /// <param name="payload">The payload to serialize.</param>
        /// <returns>JSON content for HTTP transmission.</returns>
        protected virtual StringContent OnSerialize ( WebAIBackendPayload<T> payload ) {
            var json = JsonSerializer.Serialize(payload);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        /// <summary>
        /// Deserializes the raw JSON response from the backend using <see cref="JsonElement"/>.
        /// 
        /// This method expects the backend to return a JSON object containing an
        /// "output" field. The entire JSON document is preserved as a <see cref="JsonElement"/>
        /// so that callers can inspect additional metadata.
        /// </summary>
        /// <param name="raw">Raw JSON string returned by the backend.</param>
        /// <returns>
        /// A pair containing:
        /// - The parsed output of type <typeparamref name="T"/>
        /// - The full JSON document as <see cref="JsonElement"/>
        /// </returns>
        /// <exception cref="InvalidDataException">
        /// Thrown when the backend does not return an "output" field.
        /// </exception>
        protected virtual Triple<T, MetaFormat, object> OnDeserialize ( string raw ) {
            // ---------------------------------------------------------
            // 1. Versuch: JSON erkennen
            // ---------------------------------------------------------
            try {
                JsonElement root = JsonSerializer.Deserialize<JsonElement>(raw);

                if ( root.ValueKind == JsonValueKind.Object &&
                    root.TryGetProperty("output", out JsonElement outputElement) ) {
                    // Output extrahieren
                    T output;

                    if ( typeof(T) == typeof(string) ) {
                        output = (T)(object)outputElement.ToString();
                    } else {
                        output = JsonSerializer.Deserialize<T>(outputElement.GetRawText())
                                 ?? throw new InvalidDataException("Failed to convert 'output' to target type.");
                    }

                    return new Triple<T, MetaFormat, object>(
                        output,
                        MetaFormat.AI_FORMAT_JSON,
                        root
                    );
                }

                // JSON erkannt, aber kein "output" → fallback
                if ( typeof(T) == typeof(string) ) {
                    return new Triple<T, MetaFormat, object>(
                        (T)(object)raw,
                        MetaFormat.AI_FORMAT_JSON,
                        root
                    );
                }

                throw new InvalidDataException("JSON response missing 'output' field.");
            } catch {
                // ---------------------------------------------------------
                // 2. Kein JSON → Fallback auf TEXT
                // ---------------------------------------------------------
                if ( typeof(T) == typeof(string) ) {
                    return new Triple<T, MetaFormat, object>(
                        (T)(object)raw,
                        MetaFormat.AI_FORMAT_TEXT,
                        raw
                    );
                }

                // ---------------------------------------------------------
                // 3. Kein JSON + T ist nicht string → Fehler
                // ---------------------------------------------------------
                throw new InvalidDataException(
                    "Backend returned non-JSON and T is not string. Override OnDeserialize for custom formats."
                );
            }
        }



        /// <summary>
        /// Initializes a new Web‑API backend with the specified base URL.
        /// </summary>
        /// <param name="strURL">Base URL of the remote AI service.</param>
        public WebAIBackend ( string strURL, bool freeUsable = true ) {
            m_http = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

            m_http.DefaultRequestHeaders.UserAgent.ParseAdd("SystemEX-FreeWebAI/1.0");
            m_http.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );

            m_caps = new Map<BackendCapabilities, object>
            {
                [BackendCapabilities.AI_BACKEND_CAPS_TEXT] = 1,
                [BackendCapabilities.AI_BACKEND_CAPS_CHAT] = 1,
                [BackendCapabilities.AI_BACKEND_CAPS_JSON] = 1,

                [BackendCapabilities.AI_BACKEND_CAPS_REMOTE] = 1,
                [BackendCapabilities.AI_BACKEND_CAPS_NEEDS_INTERNET] = 1,
                

                [BackendCapabilities.AI_BACKEND_CAPS_TRANSPORT_REST] = 1,
                [BackendCapabilities.AI_BACKEND_CAPS_RUNTIME_CLOUD] = 1,

                [BackendCapabilities.AI_PLATFORM_CAP_ALL] = 1,
            };

            if( freeUsable ) {
                m_caps[BackendCapabilities.AI_CAP_FREE] = 1;
            } else {
                m_caps[BackendCapabilities.AI_CAP_FREE] = 0;
                m_caps[BackendCapabilities.AI_BACKEND_CAPS_NEEDS_API_KEY] = 1;
            }

            m_configuration = new Map<string, object>
            {
                ["WB_BACKEND_CONFIG_URL"] = strURL
            };
            m_environment = new Map<Environment, object>();
        }

        
        public virtual bool Initialization ( Map<string, object> configuration, Map<Environment, object> environment ) {

            foreach(var it in configuration) {
                m_configuration.PushBack(it);
            }

            m_environment = environment;

            return true;                    
        }

        
        public virtual void Release ( bool wait ) { }


        /// <summary>
        /// Executes the model by sending a JSON payload to the remote Web‑API.
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
        public async Task<IModelResult<T>> InvokeAsync ( string systemPrompt, IModelPromp<T> input ) {
            // Payload wird direkt sauber initialisiert
            var payload = new WebAIBackendPayload<T>
            {
                URL = string.Empty,
                Model = (string)Configuration.GetOrDefault("MODEL_NAME", "gpt-4o-mini"),

                SystemPrompt = systemPrompt ?? string.Empty,
                PromptRaw = input.Prompt,

                Context = input.Context,  // conversation metadata

                Parameters = BuildWebParameters(input.Parameters),

                Tools = new List<object>(),
                ToolChoice = "no",

                Metadata = new Map<string, object>
                {
                    ["session_id"] = input.SessionId.HasValue ? input.SessionId.Value! : null,
                    ["tags"] = input.Tags,
                    ["backend"] = "WebBackend"
                }
            };

            try {
                Optional<object> urlOpt = Configuration.Get(WB_CONFIG_URL);
                if ( urlOpt.IsNull || urlOpt.Value is not string baseUrl || string.IsNullOrWhiteSpace(baseUrl) )
                    throw new KeyNotFoundException("WB_CONFIG_URL is missing or invalid.");

                payload.URL = baseUrl;

                string finalUrl = OnCreateURL(payload);
                if ( string.IsNullOrWhiteSpace(finalUrl) )
                    throw new InvalidDataException("OnCreateURL returned an empty URL.");

                payload.URL = finalUrl;


                var content = OnSerialize(payload);
                if ( content == null )
                    throw new InvalidDataException("OnSerialize returned null content.");


                HttpResponseMessage response;
                try {
                    response = await m_http.PostAsync(finalUrl, content);
                } catch ( Exception httpEx ) {
                    throw new HttpRequestException($"HTTP request failed: {httpEx.Message}", httpEx);
                }

                string raw = await response.Content.ReadAsStringAsync();

                if ( !response.IsSuccessStatusCode ) {
                    throw new HttpRequestException(
                        $"Backend returned HTTP {(int)response.StatusCode} ({response.StatusCode}). Raw: {raw}"
                    );
                }



                Triple<T, MetaFormat, object> parsed = OnDeserialize(raw);
                T output = parsed.First;

                return new ModelResult<T>(
                    result: output,
                    metadata: new Map<ModelMeta, object>
                    {
                        [ModelMeta.AI_META_BACKEND] = BackendName,
                        [ModelMeta.AI_META_BACKEND_TYPE] = "WebAPI",
                        [ModelMeta.AI_META_MODEL] = ModelName ?? "unknown",
                        [ModelMeta.AI_META_MODEL_CAPS] = Capabilities,
                        [ModelMeta.AI_META_RAW] = raw,
                        [ModelMeta.AI_META_HTTP_STATUS] = response.StatusCode,
                        [ModelMeta.AI_META_HTTP_HEADERS] = response.Headers.ToString(),
                        [ModelMeta.AI_META_URL] = finalUrl,
                        [ModelMeta.AI_META_CONTENT_OUT] = parsed.Third,
                        [ModelMeta.AI_META_CONTENT_IN] = content,
                        [ModelMeta.AI_META_EXEC_TIME] = DateTime.UtcNow, 
                        [ModelMeta.AI_META_FORMAT] = parsed.Second,
                    },
                    error: null,
                    raw: raw
                );
            } catch ( Exception ex ) {
                return new ModelResult<T>(
                result: default!,
                metadata: new Map<ModelMeta, object>
                {
                    [ModelMeta.AI_META_BACKEND] = BackendName,
                    [ModelMeta.AI_META_BACKEND_TYPE] = "WebAPI",
                    [ModelMeta.AI_META_MODEL] = ModelName ?? "unknown",
                    [ModelMeta.AI_META_MODEL_CAPS] = Capabilities,
                    [ModelMeta.AI_META_ERROR_MESSAGE] = ex.Message,
                    [ModelMeta.AI_META_ERROR_TYPE] = ex.GetType().Name,
                    [ModelMeta.AI_META_ERROR_STACK] = ex.StackTrace ?? "",
                    [ModelMeta.AI_META_ERROR_CODE] = ex.HResult,
                    [ModelMeta.AI_META_URL] = payload.URL
                },
                error: ex,
                raw: null
            );
            }
        }

        

        private Map<string, object> BuildWebParameters ( Map<string, object> modelParams) {

            var p = new Map<string, object>();

            // 1. Model parameters (temperature, max_tokens, etc.)
            p.PushBack(modelParams);

            // 2. Configuration (API keys, runtime settings)
            if ( m_configuration.TryGetValue(WB_CONFIG_TEMPERATUR, out var temp) )
                p["temperature"] = temp;

            if ( m_configuration.TryGetValue(WB_CONFIG_MAX_TOKENS, out var maxTok) )
                p["max_tokens"] = maxTok;

            if ( m_configuration.TryGetValue(WB_CONFIG_TOP_P, out var topP) )
                p["top_p"] = topP;

            if ( m_configuration.TryGetValue(WB_CONFIG_RESPONSE_FORMAT, out var respFmt) )
                p["response_format"] = respFmt;

            if ( m_configuration.TryGetValue(WB_CONFIG_STREAM, out var stream) )
                p["stream"] = stream;


#if WITH_OTIONAL
            // 3. Environment (optional, ChatGPT ignoriert diese - nur für passende User APIs
            foreach ( var kv in m_environment )
                p[$"env_{kv.First}"] = kv.Second;
#endif
            return p;
        }

        /// <summary>
        /// Web‑API backends do not support tools.
        /// </summary>
        public bool RegistTool ( IModelTool<T> tool ) => false;

        /// <summary>
        /// Web‑API backends do not support tools.
        /// </summary>
        public bool UnregistTool ( string toolName ) => false;

        /// <summary>
        /// Web‑API backends do not support tools.
        /// </summary>
        public bool HasTool ( string toolName ) => false;

        /// <summary>
        /// Web‑API backends do not support tools.
        /// </summary>
        public IReadOnlyList<object> ListTools () => Array.Empty<object>();

    }

    ///@}
}
