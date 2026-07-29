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

namespace SystemEx.AI {
    public interface IModelBackend<T , TAI> {
        /// Optional: Backend-spezifische Fähigkeiten (z.B. Vision, Audio, Tools)
        Map<string, object> Capabilities { get; }

        /// Optional: Backend-spezifische Konfiguration
        Map<string, object> Configuration { get; }

        /// <summary>
        /// The backend name used for metadata and diagnostics.
        /// </summary>
        public string BackendName { get; }

        /// <summary>
        /// Gets or sets the identifier of the AI model that should be used by the backend.
        /// 
        /// This value determines which model the Windows AI Runtime will load when
        /// executing a request. It can be changed at any time during runtime, allowing
        /// dynamic switching between different AI models (e.g., "phi-3-mini",
        /// "phi-3-medium", "gpt-4o", etc.).
        /// 
        /// The <see cref="ModelName"/> is independent from the agent name used in the
        /// <see cref="Model{T}"/> constructor. While the agent name describes the
        /// logical assistant (e.g., "DevAssistant"), the <see cref="ModelName"/>
        /// specifies the actual AI model used for inference.
        /// </summary>
        /// <returns>
        /// A string representing the model identifier passed to the Windows AI Runtime.
        /// </returns>
        string ModelName { get; set; }

        /// <summary>
        /// Indicates whether the backend is available.
        /// The OS decides availability based on installed AI components.
        /// </summary>
        bool IsAvailable { get; }

        bool RegistTool ( IModelTool<T> tool );
        bool UnregistTool ( string toolName );
        IReadOnlyList<TAI> ListTools ();

        void Begin ( Map<string, object> config );

        /// Führt das Modell aus
        Task<IModelResult<T>> InvokeAsync ( string systemPrompt, IModelPromp<T> input);

        void End ( bool wait );

        bool HasTool ( string toolName );
    }
}
