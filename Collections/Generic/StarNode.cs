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
	/// A specialized <see cref="Node{T}"/> that represents a star‑shaped structure,
	/// where the node may have an arbitrary number of child nodes.
	/// </summary>
	/// <typeparam name="T">The value type stored in the node.</typeparam>
	public class StarNode<T> : Node<T> {

        public StarNode ( int nChilds, T value ) 
            : base(nChilds) {  base.Value = value;  }
        public StarNode ( int nChilds )  
            : base(nChilds) { }
         

        /// <summary>
        /// Adds a child node to this star node.  
        /// The child is appended to the internal child array.
        /// </summary>
        /// <param name="child">The child node to add.</param>
        public int AddChild(Node<T> child) {
            int index = get_free_index();

            if ( index != -1 ) 
                m_pNodex[index] = child;

            return index;
        }

        public void RemoveChild ( int index ) {
            if ( index < 0 || index >= m_pNodex.Count )
                return;

            m_pNodex[index] = null;
        }


        private int get_free_index() {
            int _ret = -1;
            for(int i = 0; i < m_pNodex.Count ; i++ ) {
                if ( m_pNodex[i] == null ) { _ret = i;  break; }
            }
            return _ret;
        }
    }

#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
