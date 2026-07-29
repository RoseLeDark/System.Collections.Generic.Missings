using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SystemEx.Collections.Generic;

namespace SystemEx.AI.Backend {
    public sealed class FreeWebAIBackend<T> : IModelBackend<T, Object> {
        public Map<string, object> Capabilities { get; }
        public Map<string, object> Configuration { get; }

        public string BackendName => "FreeWebAI";
        public bool IsAvailable => true;

        public string ModelName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private readonly HttpClient m_http;

        public FreeWebAIBackend ( string strURL ) {
            m_http = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            m_http.DefaultRequestHeaders.UserAgent.ParseAdd("SystemEX-FreeWebAI/1.0");
            m_http.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );

            Capabilities = new Map<string, object>
            {
                ["AIBACKEND_CAP_TEXT"] = 1,
                ["AIBACKEND_CAP_CHAT"] = 1,
                ["AIBACKEND_CAP_JSON"] = 1,
                ["AIBACKEND_CAP_REMOTE"] = 1,
                ["AI_CAP_FREE"] = 1,
                ["AIPLATFORM_CAP_ALL"] = 1
            };

            Configuration = new Map<string, object>
            {
                ["FREEWB_BACKEND_CONFIG_URL"] = strURL
            };
        }
        public void Begin ( Map<string, object> config ) {
            throw new NotImplementedException();
        }


        public void End ( bool wait ) {
            throw new NotImplementedException();
        }

        public async Task<IModelResult<T>> InvokeAsync ( string systemPrompt, IModelPromp<T> input )
        { 
            try {
                var payload = new {
                    model = "",
                    system = systemPrompt,
                    prompt = input.Prompt,
                    context = input.Context,
                    parameters = input.Parameters
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                Optional<object> url = Configuration.Get("FREEWB_BACKEND_CONFIG_URL");
                if ( url.IsNull ) throw new KeyNotFoundException("FREEWB_BACKEND_CONFIG_URL");

                var response = await m_http.PostAsync(url.Value! as string, content);
                var raw = await response.Content.ReadAsStringAsync();

                var parsed = JsonSerializer.Deserialize<Dictionary<string, object>>(raw);
                if ( parsed == null || !parsed.TryGetValue("output", out var outputObj) )
                    throw new InvalidDataException("Backend returned no 'output' field.");

                T output = outputObj is T t ? t : (T)Convert.ChangeType(outputObj, typeof(T));

                return new ModelResult<T>(
                    result: output,
                    metadata: new Map<string, object>
                    {
                        ["AI_META_BACKEND"] = BackendName,
                        ["AI_META_RAW"] = raw,
                        ["AI_META_STATUS"] = response.StatusCode,
                        ["AI_META_URL"] = url.Value,
                        ["AI_META_MODEL"] = url.Value
                    },
                    error: null,
                    raw: raw
                );
            } catch ( Exception ex ) {
                return new ModelResult<T>(
                    result: default!,
                    metadata: new Map<string, object>(),
                    error: ex,
                    raw: null
                );
            }
        }

        public bool RegistTool ( IModelTool<T> tool ) {
            return false;
        }

        public bool UnregistTool ( string toolName ) {
            return false;
        }
        public bool HasTool ( string toolName ) {
            return false;
        }

        public IReadOnlyList<object> ListTools () {
            throw new NotImplementedException();
        }

        

        
    }


}
