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

namespace SystemEx.Collections.Generic {
	/// \addtogroup SystemEx.Collections.Generic 
	/// @{
	/// <summary>
	/// Defines a container capability for swapping two elements.
	/// 
	/// Any container implementing <see cref="ISwappable{T}"/> guarantees that it can
	/// exchange the items located at the specified positions. This interface is used
	/// by sorting algorithms, heap builders, and other data‑structure utilities that
	/// require in‑place element reordering.
	/// 
	/// Notes:
	/// <para>
	/// The type parameter <typeparamref name="T"/> represents the index or position
	/// used by the container. In most SystemEx containers this is a <c>long</c>.
	/// </para>
	/// <para>
	/// The interface does not impose any comparison or ordering semantics; it only
	/// provides structural mutation capability. Higher‑level algorithms (sorting,
	/// heapifying, priority queues) rely on this method to perform element exchanges.
	/// </para>
	/// </summary>
	/// <typeparam name="T">
	/// The index type used by the container. Typically <c>long</c>.
	/// </typeparam>
	public interface ISwappable<T> {
        /// <summary>
        /// Swaps the elements located at the given positions.
        /// Implementations must ensure that both indices are valid and that the
        /// operation is performed atomically with respect to the container's state.
        /// </summary>
        /// <param name="i">The first index.</param>
        /// <param name="j">The second index.</param>
        void Swap ( T i, T j );
    }
	/// @}
}