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
	public interface ILockFreeQueue<T> {
		/// <summary>
		/// Gets the total capacity of the queue. The queue is bounded and
		/// cannot grow beyond this size.
		/// </summary>
		long Capacity { get; }

		/// <summary>
		/// Gets an approximate count of items in the queue. This value is not
		/// strictly linearizable but provides a best‑effort estimate suitable
		/// for monitoring or non‑critical logic.
		/// </summary>
		long Count { get; }

		/// <summary>
		/// Gets whether the queue currently contains no items.
		/// </summary>
		bool IsEmpty { get; }
		/// <summary>
		/// Gets whether the queue currently full
		/// </summary>
		bool IsFull { get; }

		/// <summary>
		/// Supported the current queue multi consumer
		/// </summary>
		bool SupportMultiConsumer { get; }

		/// <summary>
		/// Supported the current queue multi producer
		/// </summary>
		bool SupportMultiProducer { get; }

		/// <summary>
		/// Clear the queue, be careful with bFast = true: This not save
		/// </summary>
		void Clear ( bool bFast );

		/// <summary>
		/// Attempts to dequeue an item from the queue. 
		/// </summary>
		/// <param name="result">
		/// When successful, receives the dequeued item; otherwise <c>null</c>.
		/// </param>
		/// <returns>
		/// <c>true</c> if an item was dequeued;
		/// <c>false</c> if the queue is empty.
		/// </returns>
		bool Dequeue ( out T? result );

		/// <summary>
		/// Attempts to enqueue an item into the queue. 
		/// </summary>
		/// <param name="item">The item to enqueue.</param>
		/// <returns>
		/// <c>true</c> if the item was successfully enqueued;
		/// <c>false</c> if the queue is full.
		/// </returns>
		bool Enqueue ( T item );

		/// <summary>
		/// Attempts to read the next item without removing it from the queue.
		/// This operation is lock-free and does not modify the queue state.
		/// </summary>
		/// <param name="result">
		/// Receives the next item if available; otherwise <c>null</c>.
		/// </param>
		/// <returns>
		/// <c>true</c> if an item is available to peek; otherwise <c>false</c>.
		/// </returns>
		public bool Peek ( out T? result );
	}
}