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
using SystemEx.Collections.Generic;
using SystemEx.Collections.Generic.Interfaces;

namespace SystemEx.Collections.Model {

    /// \addtogroup node
    /// @{
    /// <summary>
    /// Defines the supported traversal orders for <see cref="Node{T}"/> structures.
    /// </summary>
    public enum TraversOrder {
        /// <summary>Visit the current node before its children and siblings.</summary>
        Preorder,

        /// <summary>Visit the left subtree, then the node, then the right subtree (not implemented).</summary>
        Inorder,

        /// <summary>Visit children and siblings before the current node.</summary>
        Postorder,

        /// <summary>Traverse the linked list in forward direction.</summary>
        ListOrder,

        /// <summary>Traverse the linked list in reverse direction.</summary>
        ReservListOrder
    }

    /// <summary>
    /// Random‑access iterator for intrusive <see cref="LinkedNode{T}"/> chains.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="LinkedNodeIterrator{T}"/> provides STL‑style traversal over
    /// intrusive doubly‑linked lists built from <see cref="LinkedNode{T}"/>.
    /// It operates on snapshot clones of nodes to allow safe algorithmic
    /// manipulation without mutating the underlying list structure.
    /// </para>
    /// 
    /// <para>
    /// The iterator supports bidirectional and offset‑based movement:
    /// </para>
    /// <list type="bullet">
    ///   <item><description><c>Forward()</c> – advance by one node</description></item>
    ///   <item><description><c>Back()</c> – move to the previous node</description></item>
    ///   <item><description><c>Advance(offset)</c> – move by a signed offset</description></item>
    ///   <item><description><c>Clone()</c> – create a copy of the iterator at its current position</description></item>
    /// </list>
    /// 
    /// <para>
    /// This type integrates with the SystemEx iterator interfaces via
    /// <see cref="IRandomAccessIterator{T}"/>, making it suitable for generic
    /// algorithms such as <c>Distance</c>, <c>Advance</c>, <c>LowerBound</c>
    /// and <c>UpperBound</c>.
    /// </para>
    /// </remarks>
    public class LinkedNodeIterrator<T> : IRandomAccessIterator<T>  {
        private LinkedNode<T> m_pCurrent;

        public T? Current => m_pCurrent.HasValue ? m_pCurrent.Value : default(T);

        public bool IsEnd => !m_pCurrent.HasNext;

        public bool IsBegin => !m_pCurrent.HasPrev;

        public LinkedNode<T> Node { get; internal set; }

        public LinkedNodeIterrator(LinkedNode<T> node) {
            m_pCurrent = node.Clone();
        }
        public LinkedNodeIterrator ( LinkedNode<T> node, long offset ) {
            m_pCurrent = node.Advance(offset);
        }


        /// <summary>
        /// Advances the iterator by the specified signed offset.
        /// </summary>
        /// <param name="offset">
        /// Positive values move forward, negative values move backward.
        /// Movement stops at the chain boundaries.
        /// </param>
        /// <returns>The same iterator instance, positioned at the new node.</returns>
        public IRandomAccessIterator<T> Advance ( long offset ) {
            m_pCurrent = m_pCurrent.Advance(offset);
            return this;
        }
        /// <summary>
        /// Moves the iterator one step backward if a previous node exists.
        /// </summary>
        public void Back () {
            if(m_pCurrent.HasPrev)
                m_pCurrent = m_pCurrent.Prev!;
        }
        /// <summary>
        /// Creates a copy of this iterator at its current position.
        /// </summary>
        /// <returns>
        /// A new <see cref="LinkedNodeIterrator{T}"/> instance pointing to the same
        /// logical node.
        /// </returns>
        public IIterator<T> Clone () {
            return new LinkedNodeIterrator<T>(m_pCurrent);
        }

        /// <summary>
        /// Moves the iterator one step forward if a successor exists.
        /// </summary>
        public void Forward () {
            if ( m_pCurrent.HasNext )
                m_pCurrent = m_pCurrent.Next!;
        }
        /// <summary>
        /// Moves the iterator by the specified signed offset.
        /// </summary>
        /// <param name="i">Positive for forward, negative for backward movement.</param>
        public void Forward ( long i ) {
            m_pCurrent = m_pCurrent.Advance(i);
        }

       
    };

    /// <summary>
    /// Intrusive doubly‑linked node storing a value and two directional links.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="LinkedNode{T}"/> extends <see cref="GenericNode{T}"/> with
    /// bidirectional connectivity through <c>Prev</c> and <c>Next</c> pointers.
    /// This makes the type suitable for intrusive list structures that require
    /// constant‑time insertion, removal, splicing, and node‑level manipulation
    /// without auxiliary container objects.
    /// </para>
    ///
    /// <para>
    /// The class provides a rich set of C++‑style operations commonly found in
    /// <c>std::list</c> and other intrusive list implementations:
    /// </para>
    /// <list type="bullet">
    ///   <item>
    ///     <description>
    ///       <c>InsertBefore()</c> / <c>InsertAfter()</c> – insert an existing node
    ///       directly before or after this node.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <c>EmplaceBefore()</c> / <c>EmplaceAfter()</c> – construct and insert
    ///       a new node relative to this node.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <c>Erase()</c>, <c>EraseNext()</c>, <c>ErasePrev()</c> – remove this
    ///       node or adjacent nodes from the chain.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <c>Remove()</c> / <c>Detach()</c> – detach this node from the list
    ///       without destroying its value.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <c>ReplaceWith()</c> – replace this node with another node while
    ///       preserving positional relationships.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <c>SwapWith()</c>, <c>SwapNext()</c>, <c>SwapPrev()</c> – exchange
    ///       node positions without modifying stored values.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <c>SpliceBefore()</c> / <c>SpliceAfter()</c> – move a node from any
    ///       list and insert it before or after this node.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <c>Splice(first, last)</c> – splice an entire node range into the
    ///       position of this node.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <c>Back()</c> / <c>Root()</c> – navigate to the last or first node in
    ///       the chain.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <c>Advance()</c> – move forward or backward by an offset.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <c>AsRange()</c>, <c>Slice()</c>, <c>AsChain()</c> – create STL‑style
    ///       iterator ranges, slices, and chained ranges.
    ///     </description>
    ///   </item>
    /// </list>
    ///
    /// <para>
    /// Unlike container‑based lists, <see cref="LinkedNode{T}"/> stores linkage
    /// directly inside each node. This intrusive design eliminates allocation
    /// overhead, improves cache locality, and enables fine‑grained structural
    /// manipulation without requiring a separate list object.
    /// </para>
    ///
    /// <para>
    /// For forward‑only intrusive structures, use <c>Node&lt;T&gt;</c>.  
    /// For tree‑based intrusive structures, use <c>TreeNode&lt;T&gt;</c>.
    /// </para>
    /// </remarks>
    public class LinkedNode<T> : GenericNode<T>, IEnumerable<T> {

        private LinkedNode<T>? m_pNext, m_pPrev;

        /// <summary>
        /// Gets or sets the forward link to the next node in the chain.
        /// </summary>
        public LinkedNode<T>? Next { get => m_pNext; protected set => m_pNext = value; }

        /// <summary>
        /// Gets or sets the backwards link to the prev node in the chain.
        /// </summary>
        public LinkedNode<T>? Prev { get => m_pPrev; set => m_pPrev = value; }

        /// <summary>
        /// Indicates whether this linked node has a successor.
        /// </summary>
        public bool HasNext => m_pNext != null;

        /// <summary>
        /// Indicates whether this linked node has 
        /// </summary>
        public bool HasPrev => m_pPrev != null;

        /// <summary>
        /// Indicates whether this linked node is a leaf (no successor and no ).
        /// </summary>
        public bool IsLeaf => !HasNext && !HasPrev ;

        /// <summary>
        /// Gets the number of link slots (always 2).
        /// </summary>
        public int Count => 2;

        /// <summary>
        /// Iterator positioned at the first node in the chain.
        /// </summary>
        public override IIterator<T> Begin() => new LinkedNodeIterrator<T>(this);

        /// <summary>
        /// Iterator positioned at the last node in the chain.
        /// </summary>
        public override IIterator<T> End() => new LinkedNodeIterrator<T>(Back());

        /// <summary>
        /// Iterator positioned at the node offset from the first node.
        /// </summary>
        public IIterator<T> At ( long index ) => new LinkedNodeIterrator<T>(Root(), index);

        /// <summary>
        /// Iterator positioned at this node plus an offset.
        /// </summary>
        public IIterator<T> Offset ( long offset ) => new LinkedNodeIterrator<T>(this, offset);

        /// <summary>
        /// Reverse Iterator positioned at the last node in the chain.
        /// </summary>
        public IIterator<T> ReversFirst () => End();
        /// <summary>
        /// Reverse Iterator positioned at the first node in the chain.
        /// </summary>
        public IIterator<T> ReversEnd () => Begin();

        /// <summary>
        /// Initializes a linked node with a default value.
        /// </summary>
        public LinkedNode () : this(default(T)) { }

        /// <summary>
        /// Initializes a linked node with a value 
        /// </summary>
        public LinkedNode (T? value) : base(value) {
            Next = null;
            Prev = null;
        }

        /// <summary>
        /// Copy constructor: copies value and linkednodes
        /// </summary>
        public LinkedNode ( LinkedNode<T> other ) : base(other) {
            Next = other.Next;
            Prev = other.Prev;
        }

        public LinkedNode ( LinkedNodeIterrator<T> it ) : this(it.Node) {

        }

        /// <summary>
        /// Returns the last node in the chain starting from this node.
        /// </summary>
        public virtual LinkedNode<T> Back () {
            LinkedNode<T> _ptemp = this;

            while ( _ptemp.HasNext ) {
                _ptemp = _ptemp.Next!;
            }

            return _ptemp;
        }

        /// <summary>
        /// Returns the first node in the chain starting from this node.
        /// </summary>
        public virtual LinkedNode<T> Root () {
            LinkedNode<T> _ptemp = this;

            while ( _ptemp.HasPrev ) {
                _ptemp = _ptemp.Prev!;
            }

            return _ptemp;
        }

        /// <summary>
        /// Advances from this node by the specified signed offset and reports the remaining offset.
        /// </summary>
        /// <param name="index">
        /// Desired movement offset; positive moves forward, negative moves backward.
        /// </param>
        /// <param name="r">
        /// Receives the remaining offset if the chain end or beginning is reached.
        /// </param>
        /// <returns>The node reached after advancing.</returns>
        public LinkedNode<T> Advance ( long index, out long r ) {
            LinkedNode<T> temp = this;
            long n = index;

            if ( n > 0 ) {
                while ( n != 0 && temp.HasNext ) {
                    temp = temp.Next!;
                    n--;
                }
            } else if ( n < 0 ) {
                while ( n != 0 && temp.HasPrev ) {
                    temp = temp.Prev!;
                    n++;
                }
            }

            r = n;
            return temp;
        }

        /// <summary>
        /// Advances from this node by the specified signed offset.
        /// </summary>
        /// <param name="n">Positive for forward, negative for backward movement.</param>
        /// <returns>The node reached after advancing.</returns>
        public LinkedNode<T> Advance ( long n ) {
            LinkedNode<T> temp = this;

            if ( n > 0 ) {
                while ( n != 0 && temp!.HasNext ) {
                    temp = temp.Next!;
                }
            } else if ( n < 0 ) {
                while ( n != 0 && temp!.HasPrev ) {
                    temp = temp.Prev!;
                }
            }

            return temp;
        }
        #region INSERT
        /// <summary>
        /// Inserts an existing node directly after this node in the chain.
        /// </summary>
        /// <param name="node">The node to insert after this node.</param>
        /// <returns>The inserted node.</returns>
        public LinkedNode<T> InsertAfter ( LinkedNode<T> node ) {
            if ( node == null ) return this;

            node.Prev = this;
            node.Next = this.Next;

            if ( this.Next != null )
                this.Next.Prev = node;

            this.Next = node;

            return node;
        }
        /// <summary>
        /// Inserts the node referenced by the given iterator directly after this node.
        /// </summary>
        /// <param name="it">Iterator whose current node will be inserted.</param>
        /// <returns>The inserted node.</returns>
        public LinkedNode<T> InsertAfter ( LinkedNodeIterrator<T> it ) => InsertAfter(it.Node);

        /// <summary>
        /// Inserts an existing node directly before this node in the chain.
        /// </summary>
        /// <param name="node">The node to insert before this node.</param>
        /// <returns>The inserted node.</returns>
        public LinkedNode<T> InsertBefore ( LinkedNode<T> node ) {
            if ( node == null ) return this;
            node.Next = this;
            node.Prev = this.Prev;
            if ( this.Prev != null )
                this.Prev.Next = node;
            this.Prev = node;
            return node;
        }

        /// <summary>
        /// Inserts the node referenced by the given iterator directly before this node.
        /// </summary>
        /// <param name="it">Iterator whose current node will be inserted.</param>
        /// <returns>The inserted node.</returns>
        public LinkedNode<T> InsertBefore ( LinkedNodeIterrator<T> it ) => InsertBefore(it.Node);

        #endregion
        /// <summary>
        /// Constructs a new node from the specified value and inserts it after this node.
        /// </summary>
        /// <param name="value">The value to store in the new node.</param>
        /// <returns>The newly constructed and inserted node.</returns>
        public LinkedNode<T> EmplaceAfter ( T value ) {
            var node = new LinkedNode<T>(value);

            node.Prev = this;
            node.Next = this.Next;

            if ( this.Next != null )
                this.Next.Prev = node;

            this.Next = node;

            return node;
        }

        /// <summary>
        /// Constructs a new node from the specified value and inserts it before this node.
        /// </summary>
        /// <param name="value">The value to store in the new node.</param>
        /// <returns>The newly constructed and inserted node.</returns>
        public LinkedNode<T> EmplaceBefore ( T value ) {
            var node = new LinkedNode<T>(value);

            node.Next = this;
            node.Prev = this.Prev;

            if ( this.Prev != null )
                this.Prev.Next = node;

            this.Prev = node;

            return node;
        }


        /// <summary>
        /// Constructs a new node from the specified value and replaces this node with it.
        /// </summary>
        /// <param name="value">The value to store in the replacement node.</param>
        /// <returns>The replacement node.</returns>
        public LinkedNode<T> EmplaceWith ( T value ) => ReplaceWith(new LinkedNode<T>(value));

        /// <summary>
        /// Removes this node from the chain and reconnects its neighbors.
        /// </summary>
        /// <remarks>
        /// This method handles all positions: isolated node, head, tail, and middle.
        /// In TRACE builds, the node is additionally isolated by clearing its links.
        /// </remarks>
        /// <returns>The removed node (this).</returns>
        public LinkedNode<T>? Erase () {
            LinkedNode<T>? prev = Prev;
            LinkedNode<T>? next = Next;

            // Fall 1: Node ist isoliert
            if ( prev == null && next == null ) {
                // nichts zu verbinden
            }
            // Fall 2: Node ist am Anfang
            else if ( prev == null ) {
                next!.Prev = null;
            }
            // Fall 3: Node ist am Ende
            else if ( next == null ) {
                prev!.Next = null;
            }
            // Fall 4: Node ist mittendrin
            else {
                prev!.Next = next;
                next!.Prev = prev;
            }

#if TRACE
            // optional: isolieren
            Prev = null;
            Next = null;
#endif

            return this;
        }

        /// <summary>
        /// Removes the successor of this node from the chain, if present.
        /// </summary>
        /// <returns>The removed node, or <c>null</c> if no successor exists.</returns>
        public LinkedNode<T>? EraseNext () {
            if ( !HasNext )
                return null;

            LinkedNode<T> removed = Next!;
            LinkedNode<T>? nextNext = removed.Next;

            // Verbinden: this -> nextNext
            Next = nextNext;

            if ( nextNext != null )
                nextNext.Prev = this;

            // Entfernten Node isolieren
            removed.Next = null;
            removed.Prev = null;

            return removed;
        }
        /// <summary>
        /// Removes the predecessor of this node from the chain, if present.
        /// </summary>
        /// <returns>The removed node, or <c>null</c> if no predecessor exists.</returns>
        public LinkedNode<T>? ErasePrev () {
            if ( !HasPrev )
                return null;

            LinkedNode<T> removed = Prev!;
            LinkedNode<T>? prevPrev = removed.Prev;

            // Verbinden: prevPrev -> this
            Prev = prevPrev;

            if ( prevPrev != null )
                prevPrev.Next = this;

            // Entfernten Node isolieren
            removed.Next = null;
            removed.Prev = null;

            return removed;
        }
        /// <summary>
        /// Detaches this node from the chain by reconnecting its neighbors.
        /// </summary>
        /// <remarks>
        /// Unlike <see cref="Erase"/>, this method always isolates the node and
        /// does not rely on conditional compilation.
        /// </remarks>
        public LinkedNode<T> Remove () {
            if ( Prev != null )
                Prev.Next = Next;

            if ( Next != null )
                Next.Prev = Prev;

            Prev = null;
            Next = null;

            return this;
        }

        /// <summary>
        /// Synonym for <see cref="Remove"/>, detaching this node from the chain.
        /// </summary>
        public LinkedNode<T> Detach () {
            if ( Prev != null )
                Prev.Next = Next;

            if ( Next != null )
                Next.Prev = Prev;

            Prev = null;
            Next = null;

            return this;
        }




        /// <summary>
        /// Replaces this node with the specified node while preserving neighbor links.
        /// </summary>
        /// <param name="node">The node that will take this node's position.</param>
        /// <returns>The replacement node.</returns>
        public LinkedNode<T> ReplaceWith ( LinkedNode<T> node ) {
            if ( node == null ) return this;

            node.Prev = Prev;
            node.Next = Next;

            if ( Prev != null )
                Prev.Next = node;

            if ( Next != null )
                Next.Prev = node;

            Prev = null;
            Next = null;

            return node;
        }

        /// <summary>
        /// Replaces this node with the node referenced by the given iterator.
        /// </summary>
        /// <param name="it">Iterator whose current node will be used as replacement.</param>
        /// <returns>The replacement node.</returns>
        public LinkedNode<T> ReplaceWith ( LinkedNodeIterrator<T> it ) => ReplaceWith(it.Node);

        /// <summary>
        /// Swaps the positions of this node and the specified node within their chains.
        /// </summary>
        /// <param name="node">The node to swap with.</param>
        public void SwapWith ( LinkedNode<T> node ) {
            if ( node == null || node == this )
                return;

            // Save neighbors
            var aPrev = this.Prev;
            var aNext = this.Next;
            var bPrev = node.Prev;
            var bNext = node.Next;

            // Swap Prev/Next pointers
            this.Prev = bPrev;
            this.Next = bNext;

            node.Prev = aPrev;
            node.Next = aNext;

            // Fix neighbors
            if ( this.Prev != null )
                this.Prev.Next = this;
            if ( this.Next != null )
                this.Next.Prev = this;

            if ( node.Prev != null )
                node.Prev.Next = node;
            if ( node.Next != null )
                node.Next.Prev = node;
        }

        /// <summary>
        /// Appends the specified node at the end of the chain starting from this node.
        /// </summary>
        /// <param name="node">Node to append.</param>
        /// <returns>The appended node.</returns>
        public LinkedNode<T> PushBack ( LinkedNode<T> node ) {
            LinkedNode<T> last = Back();
            last.Next = node;
            node.Prev = last;
            return node;
        }

        /// <summary>
        /// Swaps the positions of two nodes within their respective chains.
        /// </summary>
        /// <param name="a">First node.</param>
        /// <param name="b">Second node.</param>
        public void Swap ( LinkedNode<T> a, LinkedNode<T> b ) {
            if ( a == b )
                return;

            // 1. A entfernen
            var aPrev = a.Prev;
            var aNext = a.Next;
            a.Erase();

            // 2. B entfernen
            var bPrev = b.Prev;
            var bNext = b.Next;
            b.Erase();

            // 3. A an B-Position einfügen
            if ( bPrev != null )
                bPrev.InsertAfter(a);
            else if ( bNext != null )
                bNext.InsertBefore(a);

            // 4. B an A-Position einfügen
            if ( aPrev != null )
                aPrev.InsertAfter(b);
            else if ( aNext != null )
                aNext.InsertBefore(b);
        }

        /// <summary>
        /// Swaps the successor links of this node and another node.
        /// </summary>
        /// <param name="other">The node whose successor will be swapped with this node's successor.</param>
        /// <returns>A pair containing this node and the other node.</returns>
        public Pair<LinkedNode<T>, LinkedNode<T>> SwapNext (LinkedNode<T> other ) {

            if ( other != this ) {
                LinkedNode<T>? aNext = this.Next;
                LinkedNode<T>? bNext = other.Next;

                this.Next = bNext;
                if ( bNext != null )
                    bNext.Prev = this;

                other.Next = aNext;
                if ( aNext != null )
                    aNext.Prev = other;
            }

            return new Pair<LinkedNode<T>, LinkedNode<T>>(this, other);
        }

        /// <summary>
        /// Swaps the predecessor links of this node and another node.
        /// </summary>
        /// <param name="other">The node whose predecessor will be swapped with this node's predecessor.</param>
        /// <returns>A pair containing this node and the other node.</returns>
        public Pair<LinkedNode<T>, LinkedNode<T>> SwapPrev ( LinkedNode<T> other ) {

            if ( other != this ) {
                LinkedNode<T>? aPrev = this.Prev;
                LinkedNode<T>? bPrev = other.Prev;

                this.Prev = bPrev;
                if ( bPrev != null )
                    bPrev.Next = this;

                other.Prev = aPrev;
                if ( aPrev != null )
                    aPrev.Next = other;
            }

            return new Pair<LinkedNode<T>, LinkedNode<T>>(this, other);
        }
        /// <summary>
        /// Moves the specified node from its current list and inserts it before this node.
        /// </summary>
        /// <param name="node">The node to splice before this node.</param>
        /// <returns>The spliced node.</returns>
        public LinkedNode<T> SpliceBefore ( LinkedNode<T> node ) {
            if ( node == null ) return this;

            // Detach node from its current list
            if ( node.Prev != null )
                node.Prev.Next = node.Next;
            if ( node.Next != null )
                node.Next.Prev = node.Prev;

            node.Prev = Prev;
            node.Next = this;

            if ( Prev != null )
                Prev.Next = node;

            Prev = node;

            return node;
        }


        /// <summary>
        /// Moves the specified node from its current list and inserts it after this node.
        /// </summary>
        /// <param name="node">The node to splice after this node.</param>
        /// <returns>The spliced node.</returns>
        public LinkedNode<T> SpliceAfter ( LinkedNode<T> node ) {
            if ( node == null ) return this;

            // Detach node from its current list
            if ( node.Prev != null )
                node.Prev.Next = node.Next;
            if ( node.Next != null )
                node.Next.Prev = node.Prev;

            node.Next = Next;
            node.Prev = this;

            if ( Next != null )
                Next.Prev = node;

            Next = node;

            return node;
        }

        /// <summary>
        /// Replaces this node in the chain with the range [first, last].
        /// </summary>
        /// <param name="first">First node of the range to splice in.</param>
        /// <param name="last">Last node of the range to splice in.</param>
        /// <returns>
        /// A pair containing the original node (now isolated) and the first node
        /// of the inserted range.
        /// </returns>
        public Pair<LinkedNode<T>, LinkedNode<T>> Splice ( LinkedNode<T> first, LinkedNode<T> last ) {
            LinkedNode<T>? before = Prev;
            LinkedNode<T>? after  = Next;

            // Entferne 'this' aus seiner Position
            if ( before != null )
                before.Next = after;
            if ( after != null )
                after.Prev = before;

            // Füge [first..last] an die Stelle ein, wo 'this' war
            if ( before != null )
                before.Next = first;
            first.Prev = before;

            if ( after != null )
                after.Prev = last;
            last.Next = after;

            // 'this' isolieren
            Prev = null;
            Next = null;

            return new Pair<LinkedNode<T>, LinkedNode<T>>(this, first);
        }

        

        /// <summary>
        /// Returns the number of steps to the beginning or ending of the chain.
        /// <param name="ToEnd"/>if <c>true</c> then returns the number of steps to the ending of the chain </param>
        /// </summary>
        public ulong Distance ( bool ToEnd = false ) {
            ulong _temp = 0;
            LinkedNode<T> _node = this;

            if ( ToEnd ) {
                while ( _node.HasNext ) {
                    _node = _node.Next!;
                    _temp++;
                }
            } else {
                while ( HasPrev ) {
                    _temp++;
                    _node = _node.Prev!;
                }
            }
            return _temp;
        }

        /// <summary>
        /// Creates an iterator range covering the entire chain starting at this node.
        /// </summary>
        /// <returns>
        /// A <see cref="LinkedNodeRange{T}"/> spanning from <c>Begin()</c> to <c>End()</c>.
        /// </returns>
        public LinkedNodeRange<T> AsRange () {
            return new LinkedNodeRange<T>((LinkedNodeIterrator<T>)Begin(), (LinkedNodeIterrator<T>)End());
        }
        /// <summary>
        /// Creates a slice view starting at the specified offset with the given length.
        /// </summary>
        /// <param name="start">Zero‑based offset from the beginning of the chain.</param>
        /// <param name="length">Number of elements in the slice.</param>
        /// <returns>A <see cref="LinkedNodeSlice{T}"/> representing the slice.</returns>
        public LinkedNodeSlice<T> Slice ( int start, int length ) {
            LinkedNodeIterrator<T> _p = (LinkedNodeIterrator<T> )Begin();
            _p = (LinkedNodeIterrator<T>)_p.Advance(start);

            return new LinkedNodeSlice<T>(_p, length);
        }
        /// <summary>
        /// Creates a chained range containing this node's full [begin, end) interval.
        /// </summary>
        /// <returns>A <see cref="LinkedNodeChain{T}"/> with a single segment.</returns>
        public LinkedNodeChain<T> AsChain () {
            return new LinkedNodeChain<T>().Add((LinkedNodeIterrator<T>)Begin(), (LinkedNodeIterrator<T>)End());
        }

        /// <summary>
        /// Performs a traversal over the chain using the specified order.
        /// </summary>
        /// <param name="order">
        /// The traversal order; only <see cref="TraversOrder.ListOrder"/> and
        /// <see cref="TraversOrder.ReservListOrder"/> are supported for
        /// <see cref="LinkedNode{T}"/>.
        /// </param>
        /// <param name="action">
        /// Callback invoked for each visited node.
        /// </param>
        public virtual void Travers ( TraversOrder order, Action<LinkedNode<T>> action ) {
            switch ( order ) {
                case TraversOrder.ListOrder:
                    TraversListForward(this, action);
                    break;

                case TraversOrder.ReservListOrder:
                    TraversListBackward(this, action);
                    break;
                default: break;
            }
        }
#if false
        private void TraversPreorder ( LinkedNode<T> linkedNode, Action<LinkedNode<T>> action ) {

            action(linkedNode);

            // Childs
            if ( linkedNode.HasNext ) {
                TraversListForward(linkedNode.Next!, action);
            }
            if( linkedNode.HasPrev ) {
                TraversListBackward(linkedNode.Prev!, action);
            }
        }

        private void TraversPostorder ( LinkedNode<T> linkedNode, Action<LinkedNode<T>> action ) {
            if ( linkedNode.HasNext ) {
                TraversListForward(linkedNode.Next!, action);
            }
            if ( linkedNode.HasPrev ) {
                TraversListBackward(linkedNode.Prev!, action);
            }

            action(linkedNode);
        }

        private void TraversInorder ( LinkedNode<T> linkedNode, Action<LinkedNode<T>> action ) {
            if ( linkedNode.HasNext ) {
                TraversListForward(linkedNode.Next!, action);
            }
            
            action(linkedNode);

            if ( linkedNode.HasPrev ) {
                TraversListBackward(linkedNode.Prev!, action);
            }
        }
#endif
        /// <summary>
        /// Traverses the chain backward starting from the specified node.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method performs a simple linear backward traversal using the
        /// <c>Prev</c> pointer of each <see cref="LinkedNode{T}"/>.  
        /// It is used internally by <see cref="Travers"/> when
        /// <see cref="TraversOrder.ReservListOrder"/> is selected.
        /// </para>
        /// 
        /// <para>
        /// The traversal stops when a <c>null</c> link is reached.
        /// </para>
        /// </remarks>
        /// <param name="node">Starting node for backward traversal.</param>
        /// <param name="action">Callback invoked for each visited node.</param>
        private static void TraversListBackward ( LinkedNode<T> node, Action<LinkedNode<T>> action ) {
            var temp = node;
            while ( temp != null ) {
                action(temp);
                temp = temp.Prev;
            }
        }

        /// <summary>
        /// Traverses the chain forward starting from the specified node.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method performs a simple linear forward traversal using the
        /// <c>Next</c> pointer of each <see cref="LinkedNode{T}"/>.  
        /// It is used internally by <see cref="Travers"/> when
        /// <see cref="TraversOrder.ListOrder"/> is selected.
        /// </para>
        /// 
        /// <para>
        /// The traversal stops when a <c>null</c> link is reached.
        /// </para>
        /// </remarks>
        /// <param name="node">Starting node for forward traversal.</param>
        /// <param name="action">Callback invoked for each visited node.</param>
        private static void TraversListForward ( LinkedNode<T> node, Action<LinkedNode<T>> action ) {
            var temp = node;
            while ( temp != null ) {
                action(temp);
                temp = temp.Next;
            }
        }




        /// <summary>
        /// Returns a non‑generic enumerator for the chain.
        /// </summary>
        /// <remarks>
        /// This method delegates to the generic enumerator implementation.
        /// </remarks>
        public IEnumerator GetEnumerator () {
            return GetEnumerator();
        }

        /// <summary>
        /// Enumerates the chain starting from the root node.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The enumeration begins at <see cref="Root"/> and proceeds forward
        /// using <c>Next</c> links until the end of the chain is reached.
        /// </para>
        /// 
        /// <para>
        /// This provides compatibility with <c>foreach</c> and LINQ operations,
        /// while preserving intrusive list semantics.
        /// </para>
        /// </remarks>
        /// <returns>
        /// A forward iterator over the values stored in the chain.
        /// </returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator () {
            LinkedNode<T> root = Root();

            while ( root != null ) {
                yield return root.Value!;
                root = root.Next!;
            }
        }
        /// <summary>
        /// Creates a shallow clone of this node, copying its value and link pointers.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The clone preserves <c>Prev</c> and <c>Next</c> references, making it
        /// suitable for iterator snapshotting and algorithmic traversal.
        /// </para>
        /// 
        /// <para>
        /// The cloned node is not inserted into any chain; callers must attach it
        /// explicitly if structural integration is desired.
        /// </para>
        /// </remarks>
        /// <returns>
        /// A new <see cref="LinkedNode{T}"/> instance containing the same value and
        /// link references as this node.
        /// </returns>
        public new LinkedNode<T> Clone () {
            return new LinkedNode<T>(this);
        }
    }
}
