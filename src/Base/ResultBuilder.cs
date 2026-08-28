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


namespace SystemEx {
	/// \addtogroup SystemEx
	/// @{

	/// <summary>
	/// Provides a fluent builder for constructing complex <see cref="Result"/>
	/// objects across multiple processing steps, compiler stages, or mathematical
	/// operations.
	/// 
	/// <para>
	/// <see cref="ResultBuilder"/> allows values, diagnostics, assertions, and
	/// exceptions to be accumulated progressively. It is designed for scenarios
	/// where the final output may contain multiple heterogeneous items or where
	/// the success state is determined by several independent operations.
	/// </para>
	/// 
	/// <para>
	/// The builder never throws directly; instead, exceptions are captured inside
	/// the underlying <see cref="Result"/> instance. The caller may inspect or
	/// re-throw them using <see cref="Result.Throw"/>.
	/// </para>
	/// </summary>
	public sealed class ResultBuilder {
		/// <summary>
		/// The underlying result instance being constructed.
		/// </summary>
		private Result m_result;

		/// <summary>
		/// Initializes a new <see cref="ResultBuilder"/> with an empty result.
		/// </summary>
		public ResultBuilder () {
			m_result = new Result();
		}

		/// <summary>
		/// Initializes a new <see cref="ResultBuilder"/> using an existing
		/// <see cref="Result"/> instance.
		/// </summary>
		/// <param name="existing">The result to wrap.</param>
		public ResultBuilder ( Result existing ) {
			m_result = existing;
		}

		/// <summary>
		/// Appends a value to the result.
		/// </summary>
		/// <param name="value">The value to append.</param>
		/// <returns>The current builder instance.</returns>
		public ResultBuilder Add ( object value ) {
			m_result[m_result.Count] = value;
			return this;
		}

		/// <summary>
		/// Executes an action and captures any thrown exception inside the result.
		/// </summary>
		/// <param name="action">The action to execute.</param>
		/// <returns>The current builder instance.</returns>
		public ResultBuilder Try ( Action action ) {
			try {
				action();
			} catch ( Exception ex ) {
				m_result.Catch(ex);
			}
			return this;
		}

		/// <summary>
		/// Executes a function and appends its return value to the result.
		/// Any thrown exception is captured.
		/// </summary>
		/// <param name="func">The function to execute.</param>
		/// <returns>The current builder instance.</returns>
		public ResultBuilder Try ( Func<object> func ) {
			try {
				var value = func();
				m_result[m_result.Count] = value;
			} catch ( Exception ex ) {
				m_result.Catch(ex);
			}
			return this;
		}

		/// <summary>
		/// Evaluates an assertion and records a failure message if the condition
		/// is false.
		/// </summary>
		/// <param name="condition">The condition to evaluate.</param>
		/// <param name="message">The message to store on failure.</param>
		/// <returns>The current builder instance.</returns>
		public ResultBuilder Assert ( bool condition, string message ) {
			m_result.Assert(condition, message);
			return this;
		}

		/// <summary>
		/// Appends an exception to the result without throwing it.
		/// </summary>
		/// <param name="ex">The exception to capture.</param>
		/// <returns>The current builder instance.</returns>
		public ResultBuilder Catch ( Exception ex ) {
			m_result.Catch(ex);
			return this;
		}

		/// <summary>
		/// Returns the constructed <see cref="Result"/> instance.
		/// </summary>
		public Result ToResult () {
			return m_result;
		}
	}
	
}
