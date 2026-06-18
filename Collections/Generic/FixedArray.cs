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

using System.Collections;
using System.Drawing;
using SystemEx.Collections.Generic.Interfaces;

namespace SystemEx.Collections.Generic {
    /// <summary>
    /// A fixed-size array container that provides indexed access, insertion,
    /// traversal, and basic search operations. Unlike dynamic arrays, this
    /// structure never grows and always maintains a constant capacity.
    /// </summary>
    public class FixedArray<T> : Array<T> {
        /// <summary>
        /// Returns only false, disable Property
        /// </summary>
        public override bool AutoGrow { get { return false; } set { } }
        /// <summary>
        /// Creates a new fixed-size array with the specified capacity.
        /// </summary>
        /// <param name="size">The number of elements the array can hold.</param>
        public FixedArray(int size) : base(size, 0) { }
        /// <summary>
        /// Creates a new fixed-size array using an existing buffer.
        /// </summary>
        /// <param name="e">The initial element buffer.</param>
        public FixedArray(T[] e) : base(e, 0) { }

        /// <summary>
        /// Disable Resize
        /// </summary>
        /// <returns>only false</returns>
        public override bool Resize(int size) {
            return false;
        }

        /// <summary>
        /// Inserts an element at the specified position, overwriting the existing value.
        /// </summary>
        /// <param name="pos">The position to insert at.</param>
        /// <param name="item">The element to insert.</param>
        /// <returns>The number of elements written (1 or 0).</returns>
        public override int Insert(int pos, T item) {
            if ( pos < 0 ) return 0;

            m_elements[pos] = item;
            return 1;
        }
        /// <summary>
        /// Inserts a range of elements starting at the specified position.
        /// Only as many elements as fit into the remaining space are written.
        /// </summary>
        /// <param name="pos">The starting index.</param>
        /// <param name="items">The items to insert.</param>
        /// <returns>The number of elements successfully written.</returns>
        public override int InsertRange(int pos, IEnumerable<T> items) {
            if ( pos < 0 ) return 0; 

            // Materialisieren, damit wir Count kennen
            var list = items as ICollection<T> ?? new List<T>(items);
            int count = list.Count;

            if ( count == 0 ) return 0;

            // Prüfen ob genug Platz ist
            int space = Size - pos;          // wie viel passt ab pos?
            int toWrite = count > space ? space : count;

            int idx = pos;
            int written = 0;
            foreach ( var item in list ) {
                if ( written >= toWrite )
                    break;

                m_elements[idx++] = item;
                written++;
            }

            return written;
        }
    }
}
