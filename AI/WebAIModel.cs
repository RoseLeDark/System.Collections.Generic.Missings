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
    public class WebAIModel : Model<string, object> {
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

        public bool SetTemperatur(float value ) {
            value = System.Math.Clamp(value, 0.0f, 2.0f);
            return AddConfig(WebAIBackend<string>.WB_CONFIG_TEMPERATUR, value);
        }
        public bool SetMaxTokens ( int value ) {
            return AddConfig(WebAIBackend<string>.WB_CONFIG_MAX_TOKENS, value);
        }
        public bool SetTopP ( float value ) {
            value = System.Math.Clamp(value, 0.0f, 1.0f);
            return AddConfig(WebAIBackend<string>.WB_CONFIG_TOP_P, value);
        }
        public bool SetURL(string strURL) {
            return AddConfig(WebAIBackend<string>.WB_CONFIG_URL, strURL);
        }

        public bool SetAPIKey(string key) {
            return AddConfig("API_KEY", key);
        }
         
    }
}
