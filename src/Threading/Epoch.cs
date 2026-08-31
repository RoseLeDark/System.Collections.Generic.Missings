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
	/// Represents a lightweight Read-Copy-Update (RCU) epoch counter used to
	/// track active reader sections. Each call to <see cref="Assign"/> enters
	/// a reader epoch, and each call to <see cref="Leave"/> exits it. Writers
	/// may only proceed when the epoch value indicates that no readers are active.
	/// </summary>
	public struct Epoch {
		private long m_value;

		/// <summary>
		/// Gets the current epoch value. A value greater than zero indicates
		/// that one or more readers are active.
		/// </summary>
		public long Value => Interlocked.Read(ref m_value);

		/// <summary>
		/// Gets a value indicating whether a writer may safely proceed.
		/// Writers are allowed only when no readers are active.
		/// </summary>
		public bool CanWrite => Interlocked.Read(ref m_value) <= 0;

		/// <summary>
		/// Initializes a new <see cref="Epoch"/> instance with an initial
		/// reader count of zero.
		/// </summary>
		public Epoch () {
			m_value = 0;
		}

		/// <summary>
		/// Enters a reader epoch by incrementing the internal counter.
		/// </summary>
		public void Assign () {
			Interlocked.Increment(ref m_value);
		}

		/// <summary>
		/// Leaves a reader epoch by decrementing the internal counter.
		/// </summary>
		public void Leave () {
			Interlocked.Decrement(ref m_value);
		}

		/// <summary>
		/// Determines whether epoch <paramref name="a"/> occurs after epoch
		/// <paramref name="b"/> based on their internal counter values.
		/// </summary>
		public static bool IsAfter ( Epoch a, Epoch b )
			=> Interlocked.Read(ref a.m_value) > Interlocked.Read(ref b.m_value);

		/// <summary>
		/// Returns <c>true</c> when the epoch indicates active readers.
		/// </summary>
		public static bool operator true ( Epoch value ) {
			return Interlocked.Read(ref value.m_value) > 1;
		}

		/// <summary>
		/// Returns <c>false</c> when the epoch indicates no active readers.
		/// </summary>
		public static bool operator false ( Epoch value ) {
			return Interlocked.Read(ref value.m_value) <= 0;
		}

		/// <summary>
		/// Explicitly converts the epoch into a boolean value indicating
		/// whether active readers are present.
		/// </summary>
		public static explicit operator bool ( Epoch value ) {
			return Interlocked.Read(ref value.m_value) > 1;
		}
	}


	/// <summary>
	/// Provides an RAII-style scoped reader epoch for RCU-based objects.
	/// The epoch is incremented when the instance is created and decremented
	/// when the instance is disposed. This guarantees deterministic reader
	/// tracking even in the presence of exceptions.
	/// </summary>
	public ref struct UniqueEpoch : IDisposable {
		private ref Epoch m_epoch;

		/// <summary>
		/// Creates a new <see cref="UniqueEpoch"/> and marks the beginning
		/// of a reader section by incrementing the epoch counter.
		/// </summary>
		/// <param name="epoch">The epoch instance to operate on.</param>
		public UniqueEpoch ( ref Epoch epoch ) {
			m_epoch = ref epoch;
			m_epoch.Assign();
		}

		/// <summary>
		/// Marks the end of the reader section by decrementing the epoch counter.
		/// </summary>
		public void Dispose () {
			m_epoch.Leave();
		}
	}
	
}
