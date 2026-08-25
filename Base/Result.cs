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
	/// Represents a flexible multi-value result container that can hold
	/// successful values, multiple return objects, assertion messages,
	/// or exception information.
	/// 
	/// <para>
	/// <see cref="Result"/> is designed for compiler pipelines, mathematical
	/// operations, and general-purpose workflows where the exact return type
	/// may vary. It supports dynamic result expansion, optional type extraction,
	/// and structured exception handling.
	/// </para>
	/// 
	/// <para>
	/// A result is considered successful when no exception has been captured
	/// and all assertions have passed. Exceptions and assertion failures are
	/// stored internally and can be inspected or re-thrown.
	/// </para>
	/// </summary>
	public struct Result {
		/// <summary>
		/// Internal storage for all returned objects, including values,
		/// diagnostic information, or assertion messages.
		/// </summary>
		private object?[] m_result;
		/// <summary>
		/// Stores an exception if the operation failed.
		/// </summary>
		private Exception? m_innerExption;
		/// <summary>
		/// Indicates whether the operation completed successfully.
		/// </summary>
		private bool m_isSuccess;
		/// <summary>
		/// Gets the number of stored result items.
		/// </summary>
		public int Count => m_result.Length;
		/// <summary>
		/// Gets a value indicating whether the operation succeeded.
		/// </summary>
		public bool IsSuccess => m_isSuccess;
		/// <summary>
		/// Gets a value indicating whether an exception has been captured.
		/// </summary>
		public bool IsException => m_innerExption != null;

		/// <summary>
		/// Optional callback invoked when an exception is captured.
		/// </summary>
		public Action<Exception, Result> OnException;

		/// <summary>
		/// Optional callback invoked when an assertion fails.
		/// </summary>
		public Action<bool, Result> OnAssert;

		/// <summary>
		/// Gets or sets a result item at the specified index.
		/// 
		/// <para>
		/// Setting a value beyond the current count automatically expands
		/// the internal storage array.
		/// </para>
		/// </summary>
		/// <exception cref="IndexOutOfRangeException">
		/// Thrown when attempting to read an index outside the valid range.
		/// </exception>
		public object? this [int index] {
			get {
				if ( index >= Count ) throw new IndexOutOfRangeException();
				return m_result[index];
			} 
			set {
				if ( index >= Count ) {
					Array.Resize(ref m_result, index + 2);
					m_result[index] = value;
				}
			}
		}

		/// <summary>
		/// Initializes a new successful <see cref="Result"/> instance
		/// with a single empty slot.
		/// </summary>
		public Result () {
			m_result = new object[1];
			m_innerExption = null;
			m_isSuccess = true;
			OnException = ExceptionHandler;
			OnAssert = AssertHandler;
		}

		/// <summary>
		/// Initializes a new failed <see cref="Result"/> instance
		/// containing the specified exception.
		/// </summary>
		/// <param name="exp">The captured exception.</param>
		public Result (Exception exp) {
			m_result = new object[1];
			m_innerExption = exp;

			if (m_innerExption.StackTrace != null)
				m_result[0] = m_innerExption.StackTrace;
			
			m_isSuccess = false;
			OnException = ExceptionHandler;
			OnAssert = AssertHandler;
		}


		/// <summary>
		/// Initializes a new successful <see cref="Result"/> instance
		/// containing a single value.
		/// </summary>
		/// <param name="value">The value to store.</param>
		public Result (object value) {
			m_result = new object[1];
			m_result[0] = value;
			m_innerExption = null;
			m_isSuccess = true;
			OnException = ExceptionHandler;
			OnAssert = AssertHandler;
		}
		/// <summary>
		/// Initializes a new successful <see cref="Result"/> instance
		/// containing multiple values.
		/// </summary>
		/// <param name="value">An array of objects to store.</param>
		public Result ( object[] value ) {
			m_result = new object[value.Length];
			Array.Copy(value, m_result, value.Length);

			m_innerExption = null;
			m_isSuccess = true;
			OnException = ExceptionHandler;
			OnAssert = AssertHandler;
		}

		/// <summary>
		/// Attempts to retrieve a stored value as the specified type.
		/// </summary>
		/// <typeparam name="T">The desired type.</typeparam>
		/// <param name="index">The index of the stored item.</param>
		/// <returns>
		/// An <see cref="Optional{T}"/> containing the value if the type matches;
		/// otherwise <see cref="Optional{T}.NONE"/>.
		/// </returns>
		/// <exception cref="IndexOutOfRangeException">
		/// Thrown when the index is outside the valid range.
		/// </exception>
		public Optional<T> GetAs<T>(int index = 0) {
			if ( index >= Count ) throw new IndexOutOfRangeException();

			var Item = m_result[index];
			if ( Item is T it ) return it;
			else return Optional<T>.NONE;
		}

		public object? Get( int index = 0) {
			if ( index >= Count ) throw new IndexOutOfRangeException();

			return m_result[index];
		}

		/// <summary>
		/// Captures an exception, marks the result as failed, appends
		/// diagnostic information, and invokes the exception callback.
		/// </summary>
		/// <param name="ex">The exception to capture.</param>
		public void Catch ( Exception ex ) {
			m_innerExption = ex;
			m_isSuccess = false;

			if ( m_innerExption.StackTrace != null )
				this[Count] = m_innerExption.StackTrace;

			OnException?.Invoke(ex, this);
		}

		/// <summary>
		/// Evaluates an assertion and records a failure message if the
		/// condition is false.
		/// </summary>
		/// <param name="condition">The condition to evaluate.</param>
		/// <param name="text">The message to store on failure.</param>
		/// <returns>The evaluated condition.</returns>
		public bool Assert(bool condition, string text) {

			if(! condition ) {
				m_isSuccess = false;
				m_result[Count] = text;

				OnAssert?.Invoke(condition, this);
			}
			
			return condition;
		}


		/// <summary>
		/// Throws the captured exception if one exists.
		/// </summary>
		/// <exception cref="Exception">
		/// The previously captured exception.
		/// </exception>
		public void Throw() {
			if(m_innerExption != null) throw m_innerExption;
		}



		/// <summary>
		/// Default exception callback. Does nothing unless overridden.
		/// </summary>
		private void ExceptionHandler ( Exception exp, Result sender ) { }

		/// <summary>
		/// Default assertion callback. Does nothing unless overridden.
		/// </summary>
		private void AssertHandler ( bool condition, Result sender ) { }
	}
	/// @}
}
