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

using SystemEx.AI.Backend;
using SystemEx.Collections.Generic;

namespace SystemEx.AI {

	/// \addtogroup AI
	/// @{

	/// <summary>
	/// Provides a basic implementation of a web‑based AI model wrapper.  
	/// 
	/// <para>
	/// <see cref="WebAIModel"/> integrates a remote HTTP‑driven backend 
	/// (<see cref="WebAIBackend{T}"/>) with the generic <see cref="Model{TPrompt, TResult}"/> 
	/// abstraction.  
	/// It configures default runtime parameters, establishes the backend endpoint, 
	/// and exposes convenience methods for adjusting model‑specific settings such 
	/// as temperature, token limits, sampling parameters, and API keys.
	/// </para>
	/// 
	/// <para>
	/// This class serves as a lightweight foundation for connecting external 
	/// web‑hosted AI services to the SystemEx AI pipeline.
	/// </para>
	/// </summary>
	public class WebAIModel : Model<string, object> {
		/// <summary>
		/// Initializes a new web‑based AI model using the specified endpoint URL 
		/// and model identifier.  
		/// Default configuration values are applied to ensure consistent behavior 
		/// across backends, including JSON output formatting and disabled streaming.
		/// </summary>
		/// <param name="strURL">The remote backend endpoint URL.</param>
		/// <param name="stringModelName">The model identifier used by the backend.</param>
		/// <param name="FreeAPI">
		/// Indicates whether the backend is accessed without authentication 
		/// or API‑key requirements.
		/// </param>
		public WebAIModel (string strURL, string stringModelName ,  bool FreeAPI = true)
            : base("ExampleAIObjerct", stringModelName,
            """
            You are an AI assistant with access to tools.

            BEHAVIOR:
            - Be concise but thorough
            - Always respond in the user's language

            IMPORTANT:
            - For calculations, ALWAYS use the calculator tool
            - Never make up data: use tools to get real information
            """,
            new WebAIBackend<string>(strURL, FreeAPI)
            ) {
            AddConfig(WebAIBackend<string>.WB_CONFIG_STREAM, false);
            AddConfig(WebAIBackend<string>.WB_CONFIG_URL, strURL);
            AddConfig(WebAIBackend<string>.WB_CONFIG_RESPONSE_FORMAT, new Pair<string, string>("type", "json_object"));
        }

		/// <summary>
		/// Attempts to change the backend model by parsing a command of the form 
		/// <c>"chg_model &lt;modelName&gt;"</c>.  
		/// If the command is valid, the internal <see cref="Model.ModelName"/> 
		/// property is updated.
		/// </summary>
		/// <param name="command">The command string containing the new model name.</param>
		/// <returns>
		/// True if the model name was successfully changed; otherwise false.
		/// </returns>
		public bool TryChangeModel ( string command ) {
            if ( !command.StartsWith("chg_model ", StringComparison.OrdinalIgnoreCase) )
                return false;

            var newModel = command.Substring("chg_model ".Length).Trim();

            if ( string.IsNullOrWhiteSpace(newModel) )
                return false;

            // Backend-Modell wechseln
            this.ModelName = newModel;

            return true;
        }
		/// <summary>
		/// Sets the sampling temperature used by the backend.  
		/// The value is clamped to the range [0.0, 2.0].
		/// </summary>
		/// <param name="value">The desired temperature.</param>
		/// <returns>
		/// True if the configuration value was applied; otherwise false.
		/// </returns>
		public bool SetTemperatur(float value ) {
            value = System.Math.Clamp(value, 0.0f, 2.0f);
            return AddConfig(WebAIBackend<string>.WB_CONFIG_TEMPERATUR, value);
        }
		/// <summary>
		/// Sets the maximum number of tokens the backend may generate.
		/// </summary>
		/// <param name="value">The token limit.</param>
		/// <returns>
		/// True if the configuration value was applied; otherwise false.
		/// </returns>
		public bool SetMaxTokens ( int value ) {
            return AddConfig(WebAIBackend<string>.WB_CONFIG_MAX_TOKENS, value);
        }
		/// <summary>
		/// Sets the nucleus‑sampling parameter <c>top‑p</c>.  
		/// The value is clamped to the range [0.0, 1.0].
		/// </summary>
		/// <param name="value">The top‑p sampling value.</param>
		/// <returns>
		/// True if the configuration value was applied; otherwise false.
		/// </returns>
		public bool SetTopP ( float value ) {
            value = System.Math.Clamp(value, 0.0f, 1.0f);
            return AddConfig(WebAIBackend<string>.WB_CONFIG_TOP_P, value);
        }
		/// <summary>
		/// Updates the backend endpoint URL used for model execution.
		/// </summary>
		/// <param name="strURL">The new backend URL.</param>
		/// <returns>
		/// True if the configuration value was applied; otherwise false.
		/// </returns>
		public bool SetURL(string strURL) {
            return AddConfig(WebAIBackend<string>.WB_CONFIG_URL, strURL);
        }
		/// <summary>
		/// Sets the API key used for authenticated backend access.  
		/// This value is stored as a configuration parameter and may be used 
		/// by the backend during request construction.
		/// </summary>
		/// <param name="key">The API key string.</param>
		/// <returns>
		/// True if the configuration value was applied; otherwise false.
		/// </returns>
		public bool SetAPIKey(string key) {
            return AddConfig("API_KEY", key);
        }
         
    }
	/// @}
}
