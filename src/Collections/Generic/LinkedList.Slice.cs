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
	public partial class LinkedList<TValue> {
		public Pair<LinkedListSliceResult, LinkedList<TValue>?> CreateSlice ( long start, long end ) {
			// 1. Start finden
			var node = m_root;
			long index = 0;
			var _ret = new Pair<LinkedListSliceResult, LinkedList<TValue>?>  (LinkedListSliceResult.OK, null);

			while ( index < start && node != null && !node.IsGhostNode ) {
				node = node.Next!;
				index++;
			}

			if ( node == null || node.IsGhostNode )
				_ret.First = LinkedListSliceResult.StartOutOfRange;

			else {
				var sliceRoot = new LinkedList<TValue>(node);

				if ( end > -1 ) {
					var steps = end - start;

					while ( steps > 0 && node.Next != null ) {
						node = node.Next;
						steps--;
					}

					if ( steps != 0 ) _ret.First = LinkedListSliceResult.EndOutOfRange;

					sliceRoot.m_current = node;
					sliceRoot.m_current.Next = new LinkedListNode<TValue>();
				}
				_ret.Second = sliceRoot;
			}
			return _ret;
		}
	}
}
