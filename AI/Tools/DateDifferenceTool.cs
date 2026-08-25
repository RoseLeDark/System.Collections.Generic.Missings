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
	/// \addtogroup AI::Tools 
	/// @{

	/// <summary>
	/// A tool that calculates the difference between two dates.
	/// 
	/// The result is returned as the number of days between the two timestamps.
	/// Both input values must be valid ISO 8601 date strings.
	/// </summary>
	public sealed class DateDifferenceTool : IModelTool<string> {
        /// <summary>
        /// Gets the unique tool name used by the AI runtime.
        /// </summary>
        public string Name => "calculate_date_difference";

        /// <summary>
        /// Gets a human-readable description of what the tool does.
        /// </summary>
        public string Description => "Calculates the difference between two dates in days.";

        /// <summary>
        /// Defines the parameters accepted by this tool.
        /// </summary>
        /// <returns>
        /// A sequence of <see cref="ModelToolParameter"/> describing the tool's inputs.
        /// </returns>
        public IEnumerable<ModelToolParameter> GetParameters () {
            yield return new ModelToolParameter(
                "start",
                typeof(string),
                "Start date in ISO format."
            );

            yield return new ModelToolParameter(
                "end",
                typeof(string),
                "End date in ISO format."
            );
        }

        /// <summary>
        /// Executes the date difference calculation.
        /// </summary>
        /// <param name="args">Map containing tool arguments.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// A double representing the number of days between the two dates.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when required parameters are missing or invalid.
        /// </exception>
        public async Task<object?> ExecuteAsync (
            Map<string, object?> args,
            CancellationToken ct ) {
            if ( !args.TryGetValue("start", out var s) || s is not string startStr )
                throw new ArgumentException("Missing 'start' parameter.");

            if ( !args.TryGetValue("end", out var e) || e is not string endStr )
                throw new ArgumentException("Missing 'end' parameter.");

            var start = DateTime.Parse(startStr);
            var end = DateTime.Parse(endStr);

            var diff = (end - start).TotalDays;

            return diff;
        }
    }
    /// @}
    /// @}
}
