using SystemEx.Collections.Generic;

namespace SystemEx.AI.Backend {
    // \addtogroup AI
    /// @{
    /// 


    /// <summary>
    /// Represents the payload sent to a remote Web‑API based AI backend.
    /// 
    /// This struct is serialized into JSON and transmitted via HTTP POST.
    /// It contains:
    /// - The target URL
    /// - The system prompt
    /// - The raw user prompt
    /// - Context information
    /// - Additional model parameters
    /// 
    /// The generic type <typeparamref name="T"/> represents the raw prompt type
    /// used by the model (e.g., string, structured object).
    /// </summary>
    /// <typeparam name="T">
    /// The type of the raw prompt passed to the backend.
    /// </typeparam>
    public struct WebAIBackendPayload<T> {
        /// <summary>
        /// Target URL of the remote AI service (OpenAI, Azure, custom gateway).
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// The model name (e.g., gpt-4o-mini, gpt-4o, gpt-3.5-turbo).
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// System instruction / system prompt.
        /// </summary>
        public string SystemPrompt { get; set; }

        /// <summary>
        /// Raw user prompt (generic type).
        /// </summary>
        public T PromptRaw { get; set; }

        /// <summary>
        /// Conversation context (previous messages, metadata).
        /// </summary>
        public Map<string, object> Context { get; set; }

        /// <summary>
        /// Generation parameters (temperature, max_tokens, etc.).
        /// </summary>
        public Map<string, object> Parameters { get; set; }

        /// <summary>
        /// Optional: Tools / function-calling definitions.
        /// </summary>
        public List<object> Tools { get; set; }

        /// <summary>
        /// Optional: tool selection mode ("auto", "none", or specific tool).
        /// </summary>
        public string ToolChoice { get; set; }

        /// <summary>
        /// Optional: metadata for logging or session tracking.
        /// </summary>
        public Map<string, object> Metadata { get; set; }
    }

    ///@}
}
