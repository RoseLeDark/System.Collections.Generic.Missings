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

namespace SystemEx.AI.Backend {
	/// \addtogroup AI::Backend
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

    
}
