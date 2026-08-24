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
	/// \addtogroup SystemEx.AI
	/// @{
	/// <summary>
	/// Represents a concrete implementation of <see cref="IModelPromp{T}"/> used to 
	/// encapsulate model input, contextual metadata, session information, execution 
	/// parameters, and optional tags.  
	/// 
	/// <para>
	/// <see cref="ModelPromp{T}"/> is the standard prompt container used by all 
	/// <see cref="IModelBackend"/> implementations. It provides a mutable structure 
	/// for building and enriching prompts before they are passed into the model 
	/// pipeline, while also supporting functional-style cloning through the 
	/// <c>With*</c> methods.
	/// </para>
	/// </summary>
	/// <typeparam name="T">
	/// The underlying prompt type (e.g., text, tokens, binary data, AST, etc.).
	/// </typeparam>
	public struct ModelPromp<T> : IModelPromp<T> {
        private readonly T m_strPrompt;
        private Map<string, object> m_context;
        private Optional<string> m_sessionID;
        private Map<string, object> m_parameter;
        private Map<string, object> m_tags;

		/// <inheritdoc/>
		public T Prompt => m_strPrompt;
		/// <inheritdoc/>
		public Map<string, object> Context => m_context;
		/// <inheritdoc/>
		public Optional<string> SessionId => m_sessionID;
		/// <inheritdoc/>
		public Map<string, object> Parameters => m_parameter;
		/// <inheritdoc/>
		public Map<string, object> Tags => m_tags;
		/// <inheritdoc/>
		public bool Cancel { get; }

		/// <summary>
		/// Initializes a new prompt with the specified raw value and session identifier.
		/// Context, parameters, and tags are created empty.
		/// </summary>
		/// <param name="prompt">The raw prompt value.</param>
		/// <param name="sessionId">Optional session identifier.</param>
		public ModelPromp ( T prompt, Optional<string> sessionId ) {

            m_strPrompt = prompt;
            m_context = new Map<string, object>();
            m_sessionID = sessionId;
            m_parameter =  new Map<string, object>();
            Cancel = false;
            m_tags = new Map<string, object>();
        }
		/// <summary>
		/// Initializes a new prompt using explicit context and parameter maps.
		/// </summary>
		/// <param name="prompt">The raw prompt value.</param>
		/// <param name="context">The contextual metadata map.</param>
		/// <param name="sessionId">Optional session identifier.</param>
		/// <param name="parameters">
		/// Optional parameter map; if null, an empty map is created.
		/// </param>
		public ModelPromp ( T prompt, Map<string, object> context, Optional<string> sessionId, Optional<Map<string, object>> parameters ) {

            m_strPrompt = prompt;
            m_context = context;
            m_sessionID = sessionId;
            m_parameter = parameters.IsNull ? new Map<string, object>() : parameters.Value!;
            Cancel = false;
            m_tags = new Map<string, object>();
        }
		/// <summary>
		/// Initializes a new prompt with full control over context, parameters, tags, 
		/// and cancellation state.
		/// </summary>
		/// <param name="prompt">The raw prompt value.</param>
		/// <param name="context">The contextual metadata map.</param>
		/// <param name="sessionId">Optional session identifier.</param>
		/// <param name="parameters">Optional parameter map.</param>
		/// <param name="cancel">Indicates whether the prompt should be cancelled.</param>
		/// <param name="tags">Optional tag map.</param>
		public ModelPromp (  T prompt, Map<string, object> context, Optional<string> sessionId,
            Optional<Map<string, object>> parameters, bool cancel, Optional<Map<string, object>> tags ) {

            m_strPrompt = prompt;
            m_context = context;
            m_sessionID = sessionId;
            m_parameter = parameters.IsNull ? new Map<string, object>() : parameters.Value!;
            Cancel = cancel;
            m_tags = tags.IsSome ? tags.Value! : new Map<string, object>();
        }

        /// <inheritdoc/>
        public object this [string parameter] {
            get {
                return m_parameter[parameter];
            } 
            set {
                m_parameter[parameter] = value;
            }
        }

		/// <summary>
		/// Adds or replaces a context value associated with the specified key.
		/// </summary>
		public void AddContext ( string key, object value )
            => m_context[key] = value;
		/// <summary>
		/// Removes a context entry if it exists.
		/// </summary>
		/// <returns>
		/// True if the entry was removed; otherwise false.
		/// </returns>
		public bool RemoveContext ( string key )
            => m_context.Remove(key);
		/// <summary>
		/// Determines whether a context entry with the specified key exists.
		/// </summary>
		public bool HasContext ( string key )
            => m_context.ContainsKey(key);
		/// <summary>
		/// Attempts to retrieve a context value.
		/// </summary>
		/// <param name="key">The context key.</param>
		/// <param name="value">The retrieved value, if present.</param>
		/// <returns>
		/// True if the value exists; otherwise false.
		/// </returns>
		public bool TryGetContext ( string key, out object? value )
            => m_context.TryGetValue(key, out value);
		/// <summary>
		/// Adds or replaces a parameter value associated with the specified key.
		/// </summary>
		public void AddParameter ( string key, object value )
            => m_parameter[key] = value;
		/// <summary>
		/// Removes a parameter entry if it exists.
		/// </summary>
		public bool RemoveParameter ( string key )
            => m_parameter.Remove(key);
		/// <summary>
		/// Determines whether a parameter entry with the specified key exists.
		/// </summary>
		public bool HasParameter ( string key )
            => m_parameter.ContainsKey(key);
		/// <summary>
		/// Attempts to retrieve a parameter value.
		/// </summary>
		/// <param name="key">The parameter key.</param>
		/// <param name="value">The retrieved value, if present.</param>
		/// <returns>
		/// True if the value exists; otherwise false.
		/// </returns>
		public bool TryGetParameter ( string key, out object? value )
            => m_parameter.TryGetValue(key, out value);

		/// <summary>
		/// Adds or replaces a tag value associated with the specified key.
		/// </summary>
		public void AddTag ( string key, object value )
            => m_tags[key] = value;
		/// <summary>
		/// Removes a tag entry if it exists.
		/// </summary>
		public bool RemoveTag ( string key )
            => m_tags.Remove(key);
		/// <summary>
		/// Determines whether a tag entry with the specified key exists.
		/// </summary>
		public bool HasTag ( string key )
            => m_tags.ContainsKey(key);
		/// <summary>
		/// Attempts to retrieve a tag value.
		/// </summary>
		/// <param name="key">The tag key.</param>
		/// <param name="value">The retrieved value, if present.</param>
		/// <returns>
		/// True if the value exists; otherwise false.
		/// </returns>
		public bool TryGetTag ( string key, out object? value )
            => m_tags.TryGetValue(key, out value);
		/// <summary>
		/// Creates a new prompt instance with an updated parameter value.
		/// The original instance remains unchanged.
		/// </summary>
		/// <param name="key">The parameter key.</param>
		/// <param name="value">The new value.</param>
		/// <returns>A new <see cref="ModelPromp{T}"/> instance.</returns>
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
		/// <summary>
		/// Creates a new prompt instance with an updated tag value.
		/// </summary>
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
		/// <summary>
		/// Creates a new prompt instance with an updated context value.
		/// </summary>
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
		/// <summary>
		/// Returns a human‑readable representation of the prompt, including 
		/// prompt value, session identifier, cancellation state, context, 
		/// parameters, and tags.
		/// </summary>
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
	///@}
}
