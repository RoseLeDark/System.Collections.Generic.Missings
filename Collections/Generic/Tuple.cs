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
using SystemEx.Collections.Generic.Interfaces;


namespace SystemEx.Collections.Generic {

    /// \addtogroup collections
    /// @{
    /// <summary>
    /// A dynamically sized tuple storing elements as <see cref="object"/> values.
    /// Provides indexed access, mutation, and compatibility with the <see cref="ITuple"/>
    /// interface used throughout the SystemEx collection and iterator framework.
    /// </summary>
    public class Tuple : ITuple {

        /// <summary>
        /// Internal storage for tuple elements.
        /// </summary>
        private FixedVector<object> m_elements;

        /// <summary>
        /// Gets the number of elements stored in the tuple.
        /// </summary>
        public int Count => (int)m_elements.Count;

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero‑based index of the element.</param>
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown when the index is outside the valid range.
        /// </exception>
        public object this[int index] {
            get => m_elements.ElementAt(index);
            set => Set(index, value);
        }

        /// <summary>
        /// Creates a tuple with a default capacity of 5 elements.
        /// </summary>
        public Tuple() {
            m_elements = new FixedVector<object>(5);
        }

        /// <summary>
        /// Creates a tuple with the specified capacity.
        /// </summary>
        /// <param name="N">The number of elements the tuple can hold.</param>
        public Tuple(int N) {
            m_elements = new FixedVector<object>(N);
        }

        /// <summary>
        /// Creates a tuple using an existing <see cref="FixedVector{T}"/> as storage.
        /// </summary>
        /// <param name="elements">The array used as the underlying element buffer.</param>
        public Tuple(FixedVector<object> elements) {
            m_elements = elements;
        }

        /// <summary>
        /// Retrieves the element at the specified index.
        /// </summary>
        /// <param name="index">The zero‑based index of the element.</param>
        /// <returns>The element at the given index.</returns>
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown when the index is outside the valid range.
        /// </exception>
        public object? Get(int index) {
            if ( index < 0 || index >= Count )
#pragma warning disable CA2201
                throw new IndexOutOfRangeException("index");
#pragma warning restore CA2201

            return m_elements.ElementAt(index);
        }

        /// <summary>
        /// Sets the element at the specified index to the given value.
        /// </summary>
        /// <param name="index">The index to modify.</param>
        /// <param name="value">The new value to assign.</param>
        /// <remarks>
        /// If the index is out of range, the operation is ignored.
        /// </remarks>
        public void Set(int index, object value) {
            if ( index < 0 || index >= m_elements.Count )
                return;

            m_elements.Insert(index, value);
        }

        /// <summary>
        /// Determines whether the first element equals the specified key.
        /// </summary>
        /// <param name="key">The value to compare against the first element.</param>
        /// <returns>
        /// Always throws <see cref="NotImplementedException"/> because this tuple
        /// does not define a semantic "first" element.
        /// </returns>
        bool ITuple.EqualFirst(object key) {
            throw new NotImplementedException();
        }
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
