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

namespace SystemEx.AI.Tools {
	// \addtogroup SystemEx.AI.Tools 
	/// @{
	/// <summary>
	/// A simple tool that returns the current date and time.
	/// 
	/// This tool demonstrates how to expose system information to the AI model.
	/// It supports both local time and UTC time depending on the provided parameter.
	/// </summary>
	public sealed class DateTimeTool : IModelTool<string> {
        /// <summary>
        /// Gets the unique tool name used by the AI runtime.
        /// </summary>
        public string Name => "get_current_datetime";

        /// <summary>
        /// Gets a human-readable description of what the tool does.
        /// </summary>
        public string Description => "Returns the current date and time.";

        /// <summary>
        /// Defines the parameters accepted by this tool.
        /// </summary>
        /// <returns>
        /// A sequence of <see cref="ModelToolParameter"/> describing the tool's inputs.
        /// </returns>
        public IEnumerable<ModelToolParameter> GetParameters () {
            yield return new ModelToolParameter(
                "useUtc",
                typeof(bool),
                "If true, returns UTC time; otherwise local time."
            );
        }

        /// <summary>
        /// Executes the tool logic.
        /// </summary>
        /// <param name="args">Map containing tool arguments.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// A string containing the current date/time in ISO 8601 format.
        /// </returns>
        public async Task<object?> ExecuteAsync (
			Map<string, object?> args,
            CancellationToken ct ) {
            bool useUtc = args.TryGetValue("useUtc", out var v) && v is bool b && b;

            return useUtc
                ? DateTime.UtcNow.ToString("o")
                : DateTime.Now.ToString("o");
        }
    }
    /// @}
}
