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

using SystemEx.Collections.Generic;

namespace SystemEx {
	/// \addtogroup SystemEx
	/// @{

	/// <summary>
	/// Forward iterator over a normalized numeric range. Supports stepping
	/// by one unit and exposes the current value and end-of-range state.
	/// </summary>
	public struct NumberRangeIterator<T> : Iterrator<T> {

        private Vector<T> m_range;

        /// <summary>End boundary of the range (inclusive).</summary>
        private readonly long m_end;
        private long m_index;
        /// <summary>
        /// Initializes a new iterator
        /// </summary>
        public NumberRangeIterator ( Vector<T> range, long end ) {
            m_range = range;
            m_end = end;
            m_index = 0;
        }

        /// <summary>
        /// Gets the element at the current iterator position.
        /// </summary>
        public Optional<T> Current => m_range[m_index];

        /// <summary>
        /// Indicates whether the iterator has reached the end boundary.
        /// </summary>
        public bool IsEnd =>  m_index > m_end;

        public long Index { get => m_index; set => m_index = value; }

        /// <summary>
        /// Moves the iterator one step forward.
        /// </summary>
        public void Forward () {
            if ( !IsEnd )
                m_index += 1;
        }

        /// <summary>
        /// Moves the iterator N step forward
        /// </summary>
        public void Forward ( long i ) {
            var n = i;
            while ( n > 0 ) {
                --n;
                Forward();
            }
        }
    }
	/// @}
}
