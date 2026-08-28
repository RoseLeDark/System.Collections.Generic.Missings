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
	/// \addtogroup Collections
	/// @{

	/// <summary>
	/// Defines the core mutation operations for a writable container.
	/// 
	/// <para>
	/// <see cref="IContainer{T}"/> extends <see cref="IReadOnlyContainer{T}"/> by
	/// providing insertion, replacement, removal, and clearing capabilities.
	/// Implementations may support automatic growth through
	/// <see cref="IAutoGrowe"/> or enforce fixed‑capacity semantics.
	/// </para>
	/// </summary>
	/// <typeparam name="T">The element type stored in the container.</typeparam>
	public interface IContainer<T> : IReadOnlyContainer<T> {

        /// <summary>
        /// Inserts an element at the specified index, shifting elements to the right.
        /// </summary>
        /// <param name="index">Insertion index.</param>
        /// <param name="entry">Element to insert.</param>
        /// <returns>
        /// True if insertion succeeded; false if the index was invalid or growth was not allowed.
        /// </returns>
        public bool Insert ( long index, T entry );


        /// <summary>
        /// Fills a range of indices with a single value.
        /// </summary>
        /// <param name="start">Start index.</param>
        /// <param name="end">End index (inclusive).</param>
        /// <param name="entry">Value to write.</param>
        /// <returns>
        /// True if the operation succeeded; false if the range was invalid or growth was not allowed.
        /// </returns>
        public bool Insert ( long start, long end, T entry );


        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">Index to replace.</param>
        /// <param name="entry">New value.</param>
        /// <returns>
        /// True if replacement succeeded; false if the index was invalid or growth was not allowed.
        /// </returns>
        public bool Replace ( long index, T entry );

        /// <summary>
        /// Removes the last element from the container.
        /// </summary>
        /// <returns>
        /// True if an element was removed; false if the container was empty.
        /// </returns>
        public bool Erase ();

        /// <summary>
        /// Erase overloads are API stubs and always return true.
        /// Actual removal logic is handled by Remove() methods.
        /// </summary>
        public bool Erase ( long index );


        /// <summary>
        /// Clears the container by resetting the logical index.
        /// </summary>
        public void Clear ();



    }

	/// <summary>
	/// Extends <see cref="IContainer{T}"/> with linear append semantics.
	/// 
	/// <para>
	/// A linear container supports efficient push‑back operations and may
	/// automatically grow its internal storage when <see cref="IAutoGrowe.AutoGrow"/>
	/// is enabled. This interface is typically implemented by dynamic arrays,
	/// ring buffers, and other sequential data structures.
	/// </para>
	/// </summary>
	/// <typeparam name="T">The element type stored in the container.</typeparam>
	public interface ILinearContainer<T> : IContainer<T> {
        /// <summary>
        /// Appends an element to the end of the container.
        /// Automatically grows the buffer if AutoGrow is enabled.
        /// </summary>
        /// <param name="entry">Element to append.</param>
        /// <returns>
        /// True if the element was appended; false if the container was full and AutoGrow was disabled.
        /// </returns>
        public bool PushBack ( T entry );
    }

    
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
