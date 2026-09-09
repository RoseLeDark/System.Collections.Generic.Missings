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
	/// Represents a single slot inside a lock‑free LightQueue.
	/// Each entry stores a sequence number used for producer/consumer
	/// coordination, and an optional value of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">
	/// The type of the value stored in the queue entry.
	/// </typeparam>
	public struct LightSaveQueueEntry<T> {

		/// <summary>
		/// Backing field for the sequence number. This value is used to
		/// determine whether the slot is ready to be written, ready to be
		/// consumed, or belongs to a future cycle of the ring buffer.
		/// </summary>
		private long m_number;

		/// <summary>
		/// Backing field for the stored value. May be <c>null</c> for
		/// reference types or default for value types when the slot is empty.
		/// </summary>
		private T? m_value;

		/// <summary>
		/// Gets or sets the sequence number associated with this slot.
		/// Volatile operations ensure correct visibility across threads.
		/// </summary>
		public long Number {  
			get {
				return Volatile.Read(ref m_number);
			} 
			set {
				Volatile.Write(ref m_number, value);
			}
		}
		/// <summary>
		/// Gets or sets the value stored in this queue entry.
		/// </summary>
		public T? Value {
			get {
				return m_value;
			}
			set {
				m_value = value;
			}
		}
	}
}
