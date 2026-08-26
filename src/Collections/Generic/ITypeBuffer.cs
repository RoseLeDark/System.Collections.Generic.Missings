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
using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Collections.Generic {
	/// \addtogroup Collections
	/// @{

	/// <summary>
	/// Defines a typed, fixed‑stride buffer abstraction for unmanaged element types.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <c>ITypeBuffer&lt;T&gt;</c> represents a simple, element‑oriented view over a raw byte buffer
	/// (for example a <c>Cache</c>). Implementations expose element access, cloning, clearing and
	/// fill semantics for unmanaged value types. All conversions between <typeparamref name="T"/>
	/// and the underlying bytes are the responsibility of the concrete implementation.
	/// </para>
	/// <para>
	/// The interface intentionally keeps the surface small and portable: implementations may perform
	/// endian conversion, bounds validation, and copy semantics as required by the runtime.
	/// </para>
	/// </remarks>
	/// <typeparam name="T">An unmanaged value type stored in the buffer.</typeparam>
	public interface ITypeBuffer<T>
        where T : unmanaged {
        /// <summary>
        /// Gets the number of elements contained in the buffer.
        /// </summary>
        /// <remarks>
        /// The element count is fixed for the lifetime of the buffer instance unless the concrete
        /// implementation explicitly supports resizing.
        /// </remarks>
        int Length { get; }

        /// <summary>
        /// Gets or sets the element at the specified zero‑based index.
        /// </summary>
        /// <param name="i">Zero‑based element index.</param>
        /// <returns>The element value at index <paramref name="i"/>.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown when <paramref name="i"/> is outside the valid range.</exception>
        T this[int i] { get; set; }

        /// <summary>
        /// Fills the entire buffer with the specified value.
        /// </summary>
        /// <param name="value">The value to write into every element slot.</param>
        /// <returns>The current instance for fluent usage.</returns>
        /// <remarks>
        /// Implementations should write each element using the buffer's configured stride and
        /// conversion semantics. This operation may perform per‑element writes and therefore can be
        /// more expensive than a raw memory fill for large buffers.
        /// </remarks>
        ITypeBuffer<T> Fill ( T value );

        /// <summary>
        /// Clears the buffer content and resets any internal usage state.
        /// </summary>
        /// <remarks>
        /// This method zeroes the underlying storage and resets usage counters (if any). It does not
        /// modify external cursors or positions that are managed outside the buffer instance.
        /// </remarks>
        void Clear ();

        /// <summary>
        /// Alias for <see cref="Clear"/>; zeroes the buffer content.
        /// </summary>
        /// <remarks>
        /// Provided for API symmetry; implementations may forward this call to <see cref="Clear"/>.
        /// </remarks>
        void Zero ();

        /// <summary>
        /// Creates a deep clone of the buffer.
        /// </summary>
        /// <returns>A new <see cref="ITypeBuffer{T}"/> instance containing a copy of the current data.</returns>
        /// <remarks>
        /// The returned instance must not share mutable internal state with the source buffer unless
        /// the implementation explicitly documents shared semantics.
        /// </remarks>
        ITypeBuffer<T> Clone ();
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
	/// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
