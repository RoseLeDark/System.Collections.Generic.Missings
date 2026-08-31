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



namespace SystemEx.Threading {

	/// <summary>
	/// Defines a minimal lock‑protected value container interface. Implementations
	/// provide controlled read and write access to a value of type
	/// <typeparamref name="T"/> using a caller‑supplied synchronization mechanism.
	/// 
	/// <para>
	/// The interface abstracts simple lock‑based protection for a single value.
	/// Read and write operations may block depending on the lock implementation
	/// and the specified timeout.
	/// </para>
	/// </summary>
	/// <typeparam name="T">The type of the protected value.</typeparam>
	public interface ILockedObject<T> {

		/// <summary>
		/// Gets or sets the protected value. Implementations must ensure that
		/// access is synchronized through the underlying lock mechanism.
		/// </summary>
		public T? Value { get; set; }

		/// <summary>
		/// Writes a new value using the underlying lock. The operation may block
		/// for up to <paramref name="timeoutms"/> milliseconds depending on the
		/// lock implementation.
		/// </summary>
		/// <param name="value">The new value to store.</param>
		/// <param name="timeoutms">
		/// The timeout in milliseconds. A value of <c>-1</c> indicates an
		/// infinite wait.
		/// </param>
		public void WriteValue ( T? value, int timeoutms );

		/// <summary>
		/// Reads the protected value using the underlying lock. The operation may
		/// block for up to <paramref name="timeoutms"/> milliseconds depending on
		/// the lock implementation.
		/// 
		/// <para>
		/// The returned <see cref="Result"/> typically contains the value at
		/// index <c>0</c>, but implementations may include additional diagnostic
		/// information.
		/// </para>
		/// </summary>
		/// <param name="timeoutms">
		/// The timeout in milliseconds. A value of <c>-1</c> indicates an
		/// infinite wait.
		/// </param>
		/// <returns>
		/// A <see cref="Result"/> containing the retrieved value and optional
		/// metadata.
		/// </returns>
		public Result ReadValue ( int timeoutms );
	}
	
}