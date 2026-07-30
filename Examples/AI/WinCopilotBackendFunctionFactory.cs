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
using System.Threading.Tasks;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using SystemEx.AI;

namespace ExampleAIWindowsBackend {
    /// <summary>
    /// Converts internal <see cref="IModelTool{T}"/> definitions into
    /// <see cref="AITool"/> objects compatible with the Windows AI Runtime.
    /// 
    /// This factory is used by the backend to expose tools to the AI model.
    /// </summary>
    /// <typeparam name="T">
    /// The prompt type used by the model.
    /// </typeparam>
    public sealed class WinCopilotBackendFunctionFactory<T> : IAIFunctionFactory<T, AITool> {


        public bool Convert ( IModelTool<T> tool, out AITool converted ) {
            converted = null;

            try {
                // 1. Convert internal parameter definitions into AIFunctionParameter objects.
                var parameters = tool.GetParameters()
            .Select(p => new AIFunctionParameter(
                name: p.Name,
                type: ConvertToAIType(p.Type),
                description: p.Description
            ))
            .ToArray();

                // 2. Create the delegate that executes the tool logic.
                AIFunctionDelegate handler = async (args, ct) =>
                {
                    // Execute the tool using arguments provided by the AI runtime.
                    var result = await tool.ExecuteAsync(args, ct);

                    // Must be JSON‑serializable.
                    return result;
                };

                // 3. Build the final AITool object.
                converted = AIFunctionFactory.Create(
                    name: tool.Name,
                    function: handler,
                    description: tool.Description,
                    parameters: parameters
                );

                return true;
            } catch {
                // Conversion failed → return false and leave converted = null
                return false;
            }

        }

        /// <summary>
        /// Converts a .NET <see cref="Type"/> into an AI‑compatible <see cref="AIType"/>.
        /// </summary>
        /// <param name="t">The .NET type to convert.</param>
        /// <returns>
        /// The corresponding <see cref="AIType"/> used by the AI runtime.
        /// </returns>
        private AIType ConvertToAIType ( Type t ) {
            if ( t == typeof(int) || t == typeof(double) || t == typeof(float) )
                return AIType.Number;

            if ( t == typeof(string) )
                return AIType.String;

            if ( t == typeof(bool) )
                return AIType.Boolean;

            return AIType.Object;
        }
    }
}
