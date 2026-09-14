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

namespace SystemEx.Collections.Generic {
	public partial class LinkedList<TValue> {
		private struct LinkedListEnumerator : IEnumerator<TValue> {
			private LinkedListNode<TValue>? m_node;
			private readonly LinkedListNode<TValue>? m_reset;
			public LinkedListEnumerator ( LinkedListNode<TValue>? start ) {
				m_node = start;
				m_reset = start;
			}

			public TValue Current => m_node!.Value!;
			object IEnumerator.Current => Current!;

			public bool MoveNext () {
				if ( m_node == null || m_node.IsGhostNode )
					return false;

				m_node = m_node.Next;

				return m_node != null && !m_node.IsGhostNode;
			}

			public void Reset () {
				m_node = m_reset;
			}

			public void Dispose () {
				// nichts zu tun
			}
		}

		public IEnumerator<TValue> GetEnumerator () {
			return new LinkedListEnumerator(m_root);
		}

		IEnumerator IEnumerable.GetEnumerator () {
			return GetEnumerator();
		}

		public IEnumerator<TValue> GetEnumerator ( long start ) {
			var node = m_root;
			long index = 0;

			while ( index < start && node != null && !node.IsGhostNode ) {
				node = node.Next!;
				index++;
			}

			return new LinkedListEnumerator(node);
		}
	}
}
