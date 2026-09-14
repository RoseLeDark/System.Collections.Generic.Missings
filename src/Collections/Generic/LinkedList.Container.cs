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


using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SystemEx.Collections.Generic {
	public partial class LinkedList<TValue> {
		public Optional<LinkedListNode<TValue>> Front => m_root;

		public Optional<LinkedListNode<TValue>> Back => new LinkedListNode<TValue>();

		public bool IsFull => Count == long.MaxValue;

		Optional<LinkedListNode<TValue>> IReadOnlyContainer<LinkedListNode<TValue>>.Current => this.Current;

		public long Count => throw new NotImplementedException();

		public long Length => throw new NotImplementedException();

		public Optional<LinkedListNode<TValue>> ElementAt ( long index ) {
			var node = m_root;

			while ( index > 0 && node != null && !node.IsGhostNode ) {
				node = node.Next!;
				index--;
			}

			if ( node == null || node.IsGhostNode )
				throw new ArgumentOutOfRangeException(nameof(index));

			return node;
		}

		public void Swap ( long a, long b ) {
			if ( a == b ) return;

			var nodeA = ElementAt(a).Value!;
			var nodeB = ElementAt(b).Value!;

			// Falls einer ein GhostNode ist → Fehler
			if ( nodeA.IsGhostNode || nodeB.IsGhostNode )
				throw new InvalidOperationException("Cannot swap ghost nodes.");

			// Nachbarn sichern
			var aPrev = nodeA.Prev;
			var aNext = nodeA.Next;

			var bPrev = nodeB.Prev;
			var bNext = nodeB.Next;

			// NodeA an Stelle von NodeB setzen
			if ( bPrev != null ) bPrev.Next = nodeA;
			if ( bNext != null ) bNext.Prev = nodeA;

			nodeA.Prev = bPrev;
			nodeA.Next = bNext;

			// NodeB an Stelle von NodeA setzen
			if ( aPrev != null ) aPrev.Next = nodeB;
			if ( aNext != null ) aNext.Prev = nodeB;

			nodeB.Prev = aPrev;
			nodeB.Next = aNext;

			// Root aktualisieren falls nötig
			if ( m_root == nodeA ) m_root = nodeB;
			else if ( m_root == nodeB ) m_root = nodeA;

			// Cursor aktualisieren falls nötig
			if ( m_current == nodeA ) m_current = nodeB;
			else if ( m_current == nodeB ) m_current = nodeA;
		}


		public Type GetElementType () {
			return typeof(LinkedListNode<TValue>);
		}

		public bool Insert ( long index, LinkedListNode<TValue> entry ) {
			Seek(start, SeekOrigin.Begin);
			Push(entry); return true;	
		}

		public bool Insert ( long start, long end, LinkedListNode<TValue> entry ) {
			Seek(start, SeekOrigin.Begin);

			while ( end != 0 ) {
				Push(entry);
				end--;
			}

			return true;
		}

		public bool Replace ( long index, LinkedListNode<TValue> entry ) {
			throw new NotImplementedException();
		}

		public bool Erase () => Pop() != null;
		

		public bool Erase ( long index ) {
			if( WalkerUtils(index) != index) {
				return false;+

			}
			return Pop() != null;
		}

		public void Clear () {
			ToBack();

			while (!IsEmpty)  { if ( PopBack() == null ) break;  }
		}

		private LinkedListNode<TValue> ToBack () {
			var node = m_current;

			while ( !node.IsGhostNode )  
				node = node.Next!;

			return node;
		}

		private LinkedListNode<TValue> ToFront () {
			var node = m_current;

			while ( node != m_root )   // solange wir NICHT am lokalen Root dieses Containers sind
				node = node.Prev!;

			return node;
		}


		private long WalkerUtils ( long steps ) {
			var node = ToFront ();
			long _ret = 0;

			while ( steps > 0 && node.Next != null ) {
				node = node.Next;
				steps--;
				_ret++;
			}
			m_current = node;

			return _ret;
		}
	}
}
