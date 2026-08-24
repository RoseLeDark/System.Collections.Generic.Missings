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
	/// A dynamically sized tuple storing elements as <see cref="object"/> values.
	/// Provides indexed access, mutation, and compatibility with the <see cref="ITuple"/>
	/// interface used throughout the SystemEx collection and iterator framework.
	/// </summary>
	public class Tuple<TKey> : ITuple<TKey> where TKey : notnull {
        private TKey m_index;

        /// <summary>
        /// Internal storage for tuple elements.
        /// </summary>
        private object[] m_elements;

        /// <summary>
        /// Gets the number of elements stored in the tuple.
        /// </summary>
        public int Count => (int)m_elements.Length + 1;

        public TKey First { 
            get => m_index; 
            set => m_index = value; 
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero‑based index of the element.</param>
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown when the index is outside the valid range.
        /// </exception>
        public object this[int index] {
            get => Get(index);
            set => Set(index, value);
        }


        /// <summary>
        /// Creates a tuple with the specified capacity.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="N">The number of elements the tuple can hold.</param>
        public Tuple (TKey key, int N) {
            m_elements = new object[N];
            m_index = key;
        }

        /// <summary>
        /// Creates a tuple using an existing <see cref="FixedVector{T}"/> as storage.
        /// </summary>
        /// <param name="elements">The array used as the underlying element buffer.</param>
        /// <param name="key"></param>
        public Tuple( TKey key, FixedVector<object> elements) {
            m_elements = elements.ToNative();
            m_index = key;
        }

        /// <summary>
        /// Retrieves the element at the specified index.
        /// </summary>
        /// <param name="index">The zero‑based index of the element.</param>
        /// <returns>The element at the given index.</returns>
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown when the index is outside the valid range.
        /// </exception>
        public Optional<object> Get(int index) {
            if ( index < 0 || index >= Count )
#pragma warning disable CA2201
                throw new IndexOutOfRangeException("index");
#pragma warning restore CA2201

            return index == 0 ? m_index : m_elements[index];
            
        }

        /// <summary>
        /// Sets the element at the specified index to the given value.
        /// </summary>
        /// <param name="index">The index to modify.</param>
        /// <param name="value">The new value to assign.</param>
        /// <remarks>
        /// If the index is out of range, the operation is ignored.
        /// </remarks>
        public void Set(int index, object value ) {
            if ( index < 0 || index >= m_elements.Length)
                return;

            if ( index == 0 ) {
                if(value is TKey key) {
                    m_index = key;
                }
            }
            m_elements[index] = value;
        }

        /// <summary>
        /// Determines whether the first element equals the specified key.
        /// </summary>
        /// <param name="key">The value to compare against the first element.</param>
        /// <returns>
        /// Always throws <see cref="NotImplementedException"/> because this tuple
        /// does not define a semantic "first" element.
        /// </returns>
        public bool EqualFirst ( TKey key ) {
            return m_index.Equals(key);
        }
        public bool EqualValue(long index, object value ) {
            bool _ret = false;

            if ( index < 1 || index >= Count ) {
                _ret = m_elements[index].Equals(value);
            } else if(index == 0) {
                _ret = m_index.Equals(value);
            }
            return _ret;
        }
      
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
