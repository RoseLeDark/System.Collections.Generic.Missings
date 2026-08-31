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
	/// A basic arithmetic calculator tool.
	/// 
	/// This tool performs simple mathematical operations such as:
	/// - Addition
	/// - Subtraction
	/// - Multiplication
	/// - Division
	/// 
	/// It is intended for use by AI models that should not invent
	/// numerical results but instead rely on deterministic tool output.
	/// </summary>
	public sealed class CalculatorTool : IModelTool<string> {
        /// <summary>
        /// Gets the unique tool name used by the AI runtime.
        /// </summary>
        public string Name => "calculate";

        /// <summary>
        /// Gets a human-readable description of what the tool does.
        /// </summary>
        public string Description => "Performs basic arithmetic operations.";

        /// <summary>
        /// Defines the parameters accepted by this tool.
        /// </summary>
        /// <returns>
        /// A sequence of <see cref="ModelToolParameter"/> describing the tool's inputs.
        /// </returns>
        public IEnumerable<ModelToolParameter> GetParameters () {
            yield return new ModelToolParameter(
                "a",
                typeof(double),
                "First number."
            );

            yield return new ModelToolParameter(
                "b",
                typeof(double),
                "Second number."
            );

            yield return new ModelToolParameter(
                "op",
                typeof(string),
                "Operation: add, sub, mul, div."
            );
        }

		/// <summary>
		/// Executes the calculator logic.
		/// </summary>
		/// <param name="args">Map containing tool arguments.</param>
		/// <param name="ct">Cancellation token.</param>
		/// <returns>
		/// A numeric result of the requested operation.
		/// </returns>
		/// <exception cref="DivideByZeroException">
		/// Thrown when attempting to divide by zero.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown when an unknown operation is requested.
		/// </exception>
		public async Task<object?> ExecuteAsync (
            Map<string, object?> args,
            CancellationToken ct ) {
            double a = Convert.ToDouble(args["a"]);
            double b = Convert.ToDouble(args["b"]);
            string op = args["op"].ToString() ?? "add";

            return op switch
            {
                "add" => a + b,
                "sub" => a - b,
                "mul" => a * b,
                "div" => b == 0 ? throw new DivideByZeroException() : a / b,
                _ => throw new ArgumentException($"Unknown operation '{op}'.")
            };
        }
    }
    
}
