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
using System.Runtime.CompilerServices;
using SystemEx.Collections.Generic;

namespace SystemEx.AI {
    public struct ModelPromp<T> : IModelPromp<T> {
        private readonly T m_strPrompt;
        private Map<string, object> m_context;
        private Optional<string> m_sessionID;
        private Map<string, object> m_parameter;
        private Map<string, object> m_tags;

        public T Prompt => m_strPrompt;
        public Map<string, object> Context => m_context;
        public Optional<string> SessionId => m_sessionID;
        public Map<string, object> Parameters => m_parameter;
        public Map<string, object> Tags => m_tags;
        public bool Cancel { get; }

        public ModelPromp ( T prompt, Optional<string> sessionId ) {

            m_strPrompt = prompt;
            m_context = new Map<string, object>();
            m_sessionID = sessionId;
            m_parameter =  new Map<string, object>();
            Cancel = false;
            m_tags = new Map<string, object>();
        }

        public ModelPromp ( T prompt, Map<string, object> context, Optional<string> sessionId, Optional<Map<string, object>> parameters ) {

            m_strPrompt = prompt;
            m_context = context;
            m_sessionID = sessionId;
            m_parameter = parameters.IsNull ? new Map<string, object>() : parameters.Value!;
            Cancel = false;
            m_tags = new Map<string, object>();
        }

        public ModelPromp (  T prompt, Map<string, object> context, Optional<string> sessionId,
            Optional<Map<string, object>> parameters, bool cancel, Optional<Map<string, object>> tags ) {

            m_strPrompt = prompt;
            m_context = context;
            m_sessionID = sessionId;
            m_parameter = parameters.IsNull ? new Map<string, object>() : parameters.Value!;
            Cancel = cancel;
            m_tags = tags.IsSome ? tags.Value! : new Map<string, object>();
        }

        public object this [string parameter] {
            get {
                return m_parameter[parameter];
            } 
            set {
                m_parameter[parameter] = value;
            }
        }


        public void AddContext ( string key, object value )
            => m_context[key] = value;

        public bool RemoveContext ( string key )
            => m_context.Remove(key);

        public bool HasContext ( string key )
            => m_context.ContainsKey(key);

        public bool TryGetContext ( string key, out object? value )
            => m_context.TryGetValue(key, out value);

        public void AddParameter ( string key, object value )
        => m_parameter[key] = value;

        public bool RemoveParameter ( string key )
            => m_parameter.Remove(key);

        public bool HasParameter ( string key )
            => m_parameter.ContainsKey(key);

        public bool TryGetParameter ( string key, out object? value )
            => m_parameter.TryGetValue(key, out value);

        // ---------- TAGS ----------
        public void AddTag ( string key, object value )
            => m_tags[key] = value;

        public bool RemoveTag ( string key )
            => m_tags.Remove(key);

        public bool HasTag ( string key )
            => m_tags.ContainsKey(key);

        public bool TryGetTag ( string key, out object? value )
            => m_tags.TryGetValue(key, out value);

        public ModelPromp<T> WithParameter ( string key, object value ) {
            var newParams = new Map<string, object>(m_parameter);
            newParams[key] = value;

            return new ModelPromp<T>(
                m_strPrompt,
                m_context,
                m_sessionID,
                newParams,
                Cancel,
                m_tags
            );
        }

        public ModelPromp<T> WithTag ( string key, object value ) {
            var newTags = new Map<string, object>(m_tags);
            newTags[key] = value;

            return new ModelPromp<T>(
                m_strPrompt,
                m_context,
                m_sessionID,
                m_parameter,
                Cancel,
                newTags
            );
        }

        public ModelPromp<T> WithContext ( string key, object value ) {
            var newContext = new Map<string, object>(m_context);
            newContext[key] = value;

            return new ModelPromp<T>(
                m_strPrompt,
                newContext,
                m_sessionID,
                m_parameter,
                Cancel,
                m_tags
            );
        }
        public override string ToString () {
            var sb = new System.Text.StringBuilder();

            sb.Append("ModelPromp<");
            sb.Append(typeof(T).Name);
            sb.Append(">(");

            // Prompt
            sb.Append("Prompt=");
            sb.Append(m_strPrompt?.ToString() ?? "null");
            sb.Append(", ");

            // Session
            sb.Append("SessionId=");
            sb.Append(m_sessionID.IsSome ? m_sessionID.Value : "null");
            sb.Append(", ");

            // Cancel
            sb.Append("Cancel=");
            sb.Append(Cancel);
            sb.Append(", ");

            // Context
            sb.Append("Context=[");
            foreach ( var kv in m_context ) {
                sb.Append(kv.First);
                sb.Append(":");
                sb.Append(kv.Second);
                sb.Append(", ");
            }
            if ( m_context.Count > 0 )
                sb.Length -= 2; // letztes ", " entfernen
            sb.Append("], ");

            // Parameters
            sb.Append("Parameters=[");
            foreach ( var kv in m_parameter ) {
                sb.Append(kv.First);
                sb.Append(":");
                sb.Append(kv.Second);
                sb.Append(", ");
            }
            if ( m_parameter.Count > 0 )
                sb.Length -= 2;
            sb.Append("], ");

            // Tags
            sb.Append("Tags=[");
            foreach ( var kv in m_tags ) {
                sb.Append(kv.First);
                sb.Append(":");
                sb.Append(kv.Second);
                sb.Append(", ");
            }
            if ( m_tags.Count > 0 )
                sb.Length -= 2;
            sb.Append("]");

            sb.Append(")");

            return sb.ToString();
        }

    }

}
