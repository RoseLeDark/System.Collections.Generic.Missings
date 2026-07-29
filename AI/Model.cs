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
using System.Text;
using SystemEx.Collections.Generic;

namespace SystemEx.AI {
    public class Model<T, TTOOL> : IModel<T, TTOOL> {
        private readonly List<IModelTool<T>> m_tools;
        private Map<string, object> m_caps;
        private Map<string, object> m_cfg;
        private Map<string, object> m_states;
        private Map<string, object> m_env;
        private readonly IModelBackend <T, TTOOL> m_backend;
        private string m_strName;
        private string m_strModelName;
        private string m_strPrompt;
        private string m_workPath;

        public string Name { get => m_strName;  }
        public string SystemPrompt { get => m_strPrompt; set => m_strPrompt = value; }
        public string WorkPath { get => m_workPath; set => m_workPath = value; }

        public Map<string, object> Capabilities => m_caps;
        public Map<string, object> Configuration => m_cfg;
        public Map<string, object> State => m_states;
        public Map<string, object> Environment => m_env;

        public IModelBackend<T, TTOOL> Backend => m_backend;

        public CancellationToken? Cancel { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the AI model that should be used by the backend.
        /// 
        /// This value determines which model  Runtime will load when
        /// executing a request. It can be changed at any time during runtime, allowing
        /// dynamic switching between different AI models.
        /// 
        /// </summary>
        /// <returns>
        /// A string representing the model identifier passed on the backend.
        /// </returns>
        public string ModelName {
            get => m_backend.ModelName;
            set => m_backend.ModelName = value;
        }

        public Model ( string name, string systemPrompt, string strModelName,
             string workPath, IModelBackend<T, TTOOL> backend) {

            m_strName = name;
            m_strPrompt = systemPrompt;
            m_workPath = workPath;
            m_backend = backend;
            m_backend.ModelName = strModelName;

            m_caps = new Map<string, object>();
            m_cfg = new Map<string, object>();
            m_states = new Map<string, object>();
            m_env = new Map<string, object>();

            m_tools = new List<IModelTool<T>>();
        }

        public Model ( string name, string strModelName, string systemPrompt,
            string workPath, IModelBackend<T, TTOOL> backend, Map<string, object> capabilities,
            Map<string, object> configuration, Map<string, object> environment ) {

            m_strName = name;
            m_strPrompt = systemPrompt;
            m_workPath = workPath;

            m_backend = backend;
            m_backend.ModelName = strModelName;

            m_caps = capabilities;
            m_cfg = configuration;
            m_states = new Map<string, object>();
            m_env = environment;

            m_tools = new List<IModelTool<T>>();
        }
       

        // ---------------------------------------------------------
        // Capability Indexer
        // ---------------------------------------------------------
        public bool this[string capability] {
            get {
                Optional<object> val = m_caps.Get(capability);
                return !val.IsNull;
            }
        }

        public bool HaveCap ( string strCapabilities ) {
            return this[strCapabilities];
        }

        // ---------------------------------------------------------
        // Configuration
        // ---------------------------------------------------------
        public bool AddConfig ( string key, object value ) {
            m_caps[key] = value;
            return true;
        }

        public bool GetConfigValue ( string key, ref object value ) {
            Optional<object> opt = m_caps.Get(key);
            if ( opt.IsNull ) return false;
            value = opt.Value!;
            return true;
        }

        // ---------------------------------------------------------
        // Tool Management
        // ---------------------------------------------------------
        public bool AddTool ( IModelTool<T> tool ) {
            return Backend.RegistTool(tool);
        }

        public bool RemoveTool ( string toolName ) {
            return Backend.UnregistTool(toolName);
        }

        public bool HasTool ( string toolName ) {
            return Backend.HasTool(toolName);
        }

        // ---------------------------------------------------------
        // Model Execution
        // ---------------------------------------------------------
        public void Begin () {
            Backend.Begin(Configuration);
        }
        public Task<IModelResult<T>> RunAsync ( IModelPromp<T> input )
            => Backend.InvokeAsync(SystemPrompt, input);

        public void End ( bool wait = false ) {
            Backend.End(wait);
        }
    }

}
