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
	public class LinkedListNode<TValue> 
		: INode<LinkedListNode<TValue>, TValue>, IEquatable<LinkedListNode<TValue>>, IEquatable<TValue> {

		private LinkedListNode<TValue>?  m_next = null;
		private LinkedListNode<TValue>?  m_previous = null;
		private TValue? m_value;
		private DateTime m_creationTime;
		private DateTime m_updateTime;


		public bool HasParent => m_previous != null;

		public bool IsRoot => m_previous == null;

		public bool IsEnd => m_next == null;

		public int CountChilds => 2; // Prev and Next

		public bool HasValue => m_value != null;

		public bool IsGhostNode => m_value == null && m_next == null;

		public LinkedListNode<TValue>? Next { get => GetChild(1); set { m_next = value; } }
		public LinkedListNode<TValue>? Prev { get => GetChild(0); set { m_previous = value; } }

		public bool IsNext => m_next != null;

		public bool IsPrevious => m_previous != null;

		public TValue? Value {
			get { return m_value; }
			set { m_value = value; m_updateTime = DateTime.Now; }
		}

		public DateTime CreatenTime => m_creationTime;

		public DateTime UpdateTime {
			get => m_updateTime;
			set => m_updateTime = value;
		}

		

		public LinkedListNode () {
			m_next = null;
			m_previous = null;
			m_value = default(TValue);
			m_creationTime = DateTime.Now;
			m_updateTime = m_creationTime;
		}

		public LinkedListNode ( TValue value ) {
			m_next = new LinkedListNode<TValue>();// GHOST
			m_previous = null;
			m_value = value;
			m_creationTime = DateTime.Now;
			m_updateTime = m_creationTime;
		}

		

		public LinkedListNode ( TValue value, LinkedListNode<TValue>? prev ) {
			m_next = new LinkedListNode<TValue>(); // GHOST
			m_previous = prev;
			m_value = value;
			m_creationTime = DateTime.Now;
			m_updateTime = m_creationTime;
		}


		public LinkedListNode ( LinkedListNode<TValue> other ) {
			m_next = other.m_next;
			m_previous = other.m_previous;
			m_value = other.Value;

			m_creationTime = DateTime.Now;
			m_updateTime = m_creationTime;
		}

		public LinkedListNode<TValue>? GetChild ( int id ) {
			return (id == 0) ? m_previous : (id > 0) ? m_next : null;
		}

		public bool IsChild ( int id ) {
			return (id == 0) ? (m_previous != null) :
				(id > 0) ? (m_next != null) : false;
		}

		public bool Equals ( LinkedListNode<TValue>? other ) {
			if(other == null) return false;

			if ( m_next != other.m_next ) return false;
			if ( m_previous != other.m_previous ) return false;

			if ( m_creationTime != other.m_creationTime ) return false;
			if ( m_updateTime != other.m_updateTime ) return false;

			return Equals(other.m_value);
		}
		public bool Equals ( TValue? other ) {
			if ( m_value == null && other == null ) return true;
			if ( m_value != null) return m_value.Equals(other);

			return false;
		}

		public override int GetHashCode () {
			HashCode hash = new HashCode();
			hash.Add(this.m_next);
			hash.Add(this.m_previous);
			hash.Add(this.m_value);
			hash.Add(this.m_creationTime);
			hash.Add(this.m_updateTime);
			hash.Add(this.Value);
			hash.Add(this.CreatenTime);
			hash.Add(this.UpdateTime);
			return hash.ToHashCode();
		}
	}

	public enum LinkedListSliceResult : byte {
		EndOutOfRange = 8,
		StartOutOfRange = 4,
		Error = 2,
		OK = 0,
	}


	public partial class LinkedList<TValue> : IEnumerable<TValue>, IContainer<LinkedListNode<TValue>>, ISwappable<long> {
		

		private LinkedListNode <TValue> m_root;
		private LinkedListNode <TValue> m_current;

		public LinkedListNode <TValue> Root => m_root;
		public LinkedListNode <TValue> Current => m_current;

		public bool IsEmpty => m_root.IsGhostNode;

		

		public LinkedList () {
			m_root = new LinkedListNode<TValue>(null);
			m_current = m_root;
		}

		public LinkedList ( TValue root ) 
			: this() {

			PushBack(root);
		}

		public LinkedList ( LinkedListNode<TValue> root ) {
			
			m_root = root;
			m_current = root.IsEnd ? root : root.Next!;
		}
		public void Push ( LinkedListNode<TValue> newNode ) {

			if ( m_current.IsGhostNode ) {
				PushBack(newNode);
				return;
			}

			// Tail des Segments = Ghost am Ende von newNode
			var tail = newNode;
			while ( !tail.IsGhostNode )
				tail = tail.Next!;

			// Fall 2: current ist Value → Segment direkt hinter current einfügen
			var next = m_current.Next!; // in deinem Modell existiert immer ein Next (Value oder Ghost)

			// newNode als Segment‑Head zwischen current und next einhängen
			newNode.Prev = m_current;
			m_current.Next = newNode;

			tail.Next = next;
			next.Prev = tail;

			// Cursor bleibt auf m_current
		}

		public void Push ( TValue value ) {
			// Fall 1: current ist Ghost → PushBack
			if ( m_current.IsGhostNode ) {
				PushBack(value);
			}


			// Fall 2: current ist Value → neuen Node hinter current einfügen
			var next = m_current.Next; // kann Value oder Ghost sein

			var newNode = new LinkedListNode<TValue>(value, m_current);
			// newNode.Next ist automatisch ein Ghost → aber wir überschreiben das gleich

			// newNode soll zwischen current und next stehen
			newNode.Next = next;
			next!.Prev = newNode;

			// Cursor bleibt auf current
		}


		public void PushBack ( TValue value ) {

			if ( m_root.IsGhostNode ) {
				m_root = new LinkedListNode<TValue>(value);
				//m_root.Next ist ghost wird im ctor erstellt
				m_current.Prev = m_root;

				m_current = m_root.Next!;
				return;
			}

			var node = m_current;

			while ( !node.IsGhostNode )
				node = node.Next!;

			// node ist jetzt der Ghost-Node


			node = new LinkedListNode<TValue>(value, node.Prev); // jetzt ist node.Prev immer noch Node.Prev und next Ghost
			m_current = node.Next!; // Zeigt jetzt auf neuen ghost

		}

		public void PushBack ( LinkedListNode<TValue> newNode ) {
			if ( m_root.IsGhostNode ) {
				m_root = newNode;

				var _node = m_root;

				while ( !_node.IsGhostNode )
					_node = _node.Next!;

				// jetzt haben wir das ende von root erreicht bzw dem neuen current

				m_current = _node; // m_current zeigt auf neun ghost
			}

			// Fall 2

			var node = m_current;

			while ( !node.IsGhostNode )
				node = node.Next!;

			newNode.Prev = node.Prev;
			node.Prev!.Next = newNode;

			var end = newNode;

			while ( !end.IsGhostNode )
				end = end.Next!;

			m_current = end;
		}


		public void PushFront ( TValue value ) {
			// Fall 1: Liste leer → Root ist Ghost-Node
			if ( m_root.IsGhostNode ) {
				m_root.Value = value;

				var ghost = new LinkedListNode<TValue>();
				m_root.Next = ghost;
				ghost.Prev = m_root;

				m_current = ghost;
				return;
			}

			// Fall 2: Root ist NICHT der echte Root → PushFront verboten
			if ( !m_root.IsRoot )
				throw new InvalidOperationException("Cannot PushFront on a partial list.");

			// Fall 3: Root ist echter Value-Node → neuen Node davor einfügen
			var newRoot = new LinkedListNode<TValue>(value, null);
			newRoot.Next = m_root;

			m_root.Prev = newRoot;

			m_root = newRoot;
		}



		public TValue? PopBack () {
			// Fall 1: Liste leer
			if ( m_root.IsGhostNode )
				return default;

			// Ghost am Ende finden
			var ghost = m_current;
			while ( !ghost.IsGhostNode )
				ghost = ghost.Next!;

			// Value-Node davor
			var last = ghost.Prev;

			// Fall 2: nur ein Value-Node existiert (Root)
			if ( last == null ) {
				var value = m_root.Value;

				// Root wird zum Ghost
				m_root.Value = default;

				// alter Ghost entfernen
				m_root.Next!.Prev = null;
				m_root.Next = null;

				// Cursor zeigt auf Root (Ghost)
				m_current = m_root;

				return value;
			}

			// Normalfall: mehrere Nodes
			var result = last.Value;

			// last wird zum Ghost
			last.Value = default;

			// alten Ghost entfernen
			ghost.Prev = null;
			last.Next = null;

			// Cursor auf neuen Ghost
			m_current = last;

			return result;
		}

		public TValue? PopFront () {
			if ( m_root.IsGhostNode )
				throw new InvalidOperationException("list is empty");

			if ( !m_root.IsRoot )
				throw new InvalidOperationException("Cannot PopFront on a partial list.");

			// Wert sichern
			TValue? value = m_root.Value;

			// nächster Node
			var next = m_root.Next!; // immer vorhanden (Value oder Ghost)

			// Root entfernen
			next.Prev = null;
			m_root = next;

			// Cursor bleibt unverändert,
			// außer wenn die Liste jetzt leer ist (Root ist Ghost)
			if ( m_root.IsGhostNode )
				m_current = m_root;

			return value;
		}


		public TValue? Pop () {
			if ( m_current.IsGhostNode )
				return default;

			// Root-Fall: Current == Root
			if ( m_current == m_root ) {
				if ( !m_root.HasValue ) throw new Exception("Bob? Bob Somthing is wrong with use, root is empty");

				TValue? ret = m_root.Value ;

				var oldRoot = m_root;
				var newRoot = m_root.Next!; // immer vorhanden (Value oder Ghost)

				// Wenn rechts noch Werte stehen (Next ist kein Ghost)
				if ( !newRoot.IsGhostNode ) {
					// Neuer Root übernimmt die Prev-Kette des alten Root
					newRoot.Prev = oldRoot.Prev;

					// Prev.Next zeigt auf neuen Root (falls vorhanden)
					if ( oldRoot.Prev != null )
						oldRoot.Prev.Next = newRoot;

					// Alten Root isolieren
					oldRoot.Prev = null;
					oldRoot.Next = null;

					// Root und Cursor verschieben
					m_root = newRoot;
					m_current = newRoot;

					return ret;
				}

				// Wenn Next ein Ghost ist → Root ist letzter Wert
				// Root wird zu Ghost
				m_root = new LinkedListNode<TValue>();
				m_current = m_root;

				return ret;
			}
			// ❗ Wenn prev == null → normaler Pop wird NICHT ausgeführt
			if ( m_current.Prev == null )
				return default;

			// Normaler Pop
			TValue? value = m_current.Value;

			var prev = m_current.Prev;
			var next = m_current.Next!;

			prev.Next = next;
			next.Prev = prev;

			m_current.Prev = null;
			m_current.Next = null;

			m_current = prev!;

			return value;
		}


		public long Seek(long offset, SeekOrigin origin) {
			long _ret = 0;

			

			if(origin == System.IO.SeekOrigin.Current)	{
				var node = m_current;
				

				if ( offset > 0 ) {
					while ( offset > 0 && node.Next != null ) {
						node = node.Next;
						offset--;
						_ret++;
					}
				} else if ( offset < 0 ) {
					while ( offset < 0 && node.Prev != null ) {
						node = node.Prev;
						offset++;
						_ret++;
					}
				}

				m_current = node;
			}
			
			if ( origin == System.IO.SeekOrigin.Begin ) {
				var node = ToFront();

				if ( offset > 0 ) {
					while ( offset > 0 && node.Next != null ) {
						node = node.Next;
						offset--;
						_ret++;
					}
				} 

				m_current = node;
			}

			if(origin == SeekOrigin.End) {
				var back = ToBack();

				if ( offset < 0 ) {
					while ( offset < 0 && back.Prev != null ) {
						back = back.Prev;
						offset++;
						_ret++;
					}

					m_current = back;
			}
 
			return _ret;
		}


	}
}
