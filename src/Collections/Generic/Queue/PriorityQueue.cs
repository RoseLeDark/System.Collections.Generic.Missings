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

using System.Numerics;
using System.Runtime.CompilerServices;
using SystemEx.Algorithms;

namespace SystemEx.Collections.Generic {
	/// \addtogroup Collections
	/// @{

	/// <summary>
	/// Represents a delegate that performs a sorting or rearrangement operation
	/// on a priority‑queue container.  
	/// The action receives the underlying array by reference and a comparison
	/// strategy used to determine the ordering of elements.
	/// </summary>
	/// <typeparam name="T">
	/// The element type stored in the container.
	/// </typeparam>
	/// <typeparam name="TCompare">
	/// The comparison strategy used to evaluate element ordering.
	/// </typeparam>
	/// <param name="container">
	/// Reference to the underlying array of elements to be sorted or rearranged.
	/// </param>
	/// <param name="comparer">
	/// The comparison provider used to determine the relative ordering of elements.
	/// </param>
	public delegate void PriorityQueueAction<T, TCompare> ( ref T[] container, TCompare comparer );


	/// <summary>
	/// A Simple priority queue implemented .  
	/// Elements are stored as <see cref="Pair{T, TU}"/> where <c>T</c> is the value
	/// and <c>TU</c> is the priority.  
	/// Supports enqueue, dequeue, peek, priority updates, cloning, and median‑based
	/// selection.
	/// </summary>
	/// <typeparam name="TElement">The stored value type.</typeparam>
	/// <typeparam name="TPriority">
	/// The priority type, must implement <see cref="INumber{TU}"/> to allow
	/// arithmetic and comparison operations.
	/// </typeparam>
	public struct PriorityQueue<TElement, TPriority> : IAutoGrowe , ISwappable<long>
        where TPriority : INumber<TPriority>
        where TElement : notnull {

        struct PriorityQueueCompare : ISimpleCompare<Pair<TElement, TPriority>> {
            public Triple Compare ( Optional<Pair<TElement, TPriority>> a, Optional<Pair<TElement, TPriority>> b ) {
                Triple _ret = triple.True;

                if ( !a.IsNull && !b.IsNull ) {

                    if ( a.Value.Second < b.Value.Second ) _ret = triple.False;
                    if ( a.Value.Second > b.Value.Second ) _ret = triple.Nin;
                }
                return _ret;
            }
        }

        public static readonly ISimpleCompare<Pair<TElement, TPriority>> DEFAULTCOMPAR = new PriorityQueueCompare();

        private long m_growSize;
        private bool m_autoGrow;
        private bool m_isDirty;

        private PriorityQueueAction<Pair<TElement, TPriority>, ISimpleCompare<Pair<TElement, TPriority>>>? m_sorter;

        /// <summary>
        /// Internal sorted map storing all (value, priority) pairs.
        ///</summary>
        private Pair<TElement, TPriority>[] m_map;
        /// <summary>
        /// Current number of valid elements stored in the Vector.
        /// </summary>
        private long m_index;
        /// <summary>
        /// The real size
        /// </summary>
        public long Length => m_map.LongLength;

        /// <summary>
        /// Gets the number of elements in the queue.
        /// </summary>
        public long Count => m_index;

        /// <summary>
        /// Gets the element with the smallest priority.
        /// </summary>
        public Pair<TElement, TPriority> Current => m_map[0];

        private bool m_autoSort;


        /// <summary>
        /// Gets the element with the minimum priority.
        /// </summary>
        public Pair<TElement, TPriority> Min => m_map[0];

        /// <summary>
        /// Gets the element with the maximum priority.
        /// </summary>
        public Pair<TElement, TPriority> Max => m_map[Count - 1];

        /// <summary>
        /// Gets the element whose priority is closest to the statistical median.
        /// </summary>
        public Pair<TElement, TPriority>? Median => GetClosestToMedian();

        /// <summary>
        /// Indicates whether the Vector is full.
        /// </summary>
        public bool IsFull => (AutoGrow ? false : m_index >= Length);

        /// <summary>
        /// Indicates whether the Vector contains no elements.
        /// </summary>
        public bool IsEmpty => m_index == 0;
        /// <summary>
        /// Gets or sets the number of elements the Vector grows by when AutoGrow is enabled.
        /// </summary>
        public long GrowSize {
            get => (m_autoGrow ? m_growSize : 0);
            set {
                m_growSize = value;
                m_autoGrow = (m_growSize > 0);
            }
        }
        /// <summary>
        /// Enables or disables automatic resizing when the Vector becomes full.
        /// </summary>
        public bool AutoGrow { get => (m_growSize == 0 ? false : m_autoGrow); set => m_autoGrow = value; }

		/// <summary>
		/// Indicates whether the queue automatically sorts itself after insertions.
		/// </summary>
		public bool AutoSort => m_autoSort;

		/// <summary>
		/// Indicates whether the internal buffer has been modified without being sorted.
		/// </summary>
		public bool IsDirty => m_isDirty;

		/// <summary>
		/// Gets or sets the delegate used to sort the queue.
		/// When <see cref="AutoSort"/> is enabled, assigning a new sorter triggers sorting.
		/// </summary>
		public PriorityQueueAction<Pair<TElement, TPriority>, ISimpleCompare<Pair<TElement, TPriority>>>? SortFunctions {
            get => m_sorter;
            set {
                m_sorter = value;
                if ( AutoSort ) Sort();
            }
        }

        /// <summary>
        /// Creates a priority queue using the default priority comparison.
        /// </summary>
        public PriorityQueue(int size, int GrowSize = 8) {
            m_map = new Pair<TElement, TPriority>[size];
            m_autoSort = true;
            m_isDirty = false;
            m_sorter = SortActions.ShellSort< Pair<TElement, TPriority>  >;
        }
		/// <summary>
		/// Creates a priority queue 
		/// </summary>
		public PriorityQueue ( int size, int growSize = 2, PriorityQueueAction<Pair<TElement, TPriority>, ISimpleCompare<Pair<TElement, TPriority>>>? sorter = null) {

            m_map = new Pair<TElement, TPriority>[size];
            m_sorter = sorter;
            m_autoSort = true;
            m_isDirty = false;
        }

		/// <summary>
		/// Creates a priority queue from given elements, with sort and custom priority comparison.
		/// </summary>
		public PriorityQueue ( Pair<TElement, TPriority>[] elements, int growSize = 2, PriorityQueueAction<Pair<TElement, TPriority>, ISimpleCompare<Pair<TElement, TPriority>>>? sorter = null) {

            m_map = new Pair<TElement, TPriority>[elements.LongLength];
            Buffer.LongCopy(elements, 0, m_map, 0, elements.LongLength);
            m_autoSort = true;
            m_isDirty = false;
            m_sorter = sorter;
            Sort();
        }


        /// <summary>
        /// Inserts a new value with the specified priority.
        /// </summary>
        public bool Enqueue ( TElement item, TPriority priority ) {

			if ( m_index >= m_map.Length ) {
				if ( !AutoGrow )
					return false;
   
				Grow();
			}


			m_map[m_index] = new Pair<TElement, TPriority>(item, priority);
            m_index++;

            if ( AutoSort ) Sort();
            else m_isDirty = true;
            return true;
        }

        /// <summary>
        /// Returns the value with the smallest priority without removing it.
        /// </summary>
        public Optional<TElement> Peek () {
            Optional<TElement> _ret = Optional<TElement>.NONE;

 
            if(!IsEmpty){
                _ret = m_map[0].First;
            }
            return _ret;
        }

		/// <summary>
		/// Attempts to retrieve the element with the smallest priority without removing it.
		/// </summary>
		/// <param name="element">Receives the value if available.</param>
		/// <param name="priority">Receives the priority if available.</param>
		/// <returns>
		/// True if the queue contains at least one element; otherwise false.
		/// </returns>

		public bool TryPeek ( out Optional<TElement> element, out Optional<TPriority> priority ) {
            if ( IsEmpty ) {
                element = Optional<TElement>.NONE;
                priority = Optional<TPriority>.NONE;
                return false;
            }

            element = m_map[0].First;
            priority = m_map[0].Second;

            return true;
        }

        /// <summary>
        /// Removes and returns the value with the smallest priority.
        /// </summary>
        public Optional<TElement> Dequeue () {
            Optional<TElement> _ret = Optional<TElement>.NONE;

            if ( !IsEmpty ) {
                _ret = m_map[0].First;
            }

            // Shift all elements to the left
            for ( int i = 0 ; i < m_index - 1 ; i++ )
                m_map[i] = m_map[i + 1];

            m_index--;
            return _ret;
        }

		/// <summary>
		/// Attempts to remove and return the element with the smallest priority.
		/// </summary>
		/// <param name="element">Receives the dequeued value.</param>
		/// <param name="priority">Receives the dequeued priority.</param>
		/// <returns>
		/// True if an element was removed; otherwise false.
		/// </returns>

		public bool TryDequeue ( out Optional<TElement> element, out Optional<TPriority> priority ) {
            if ( IsEmpty ) {
                element = Optional<TElement>.NONE;
                priority = Optional < TPriority >.NONE;
                return false;
            }

            element = m_map[0].First;
            priority = m_map[0].Second;

            // shift left
            for ( int i = 0 ; i < m_index - 1 ; i++ )
                m_map[i] = m_map[i + 1];

            m_index--;
            return true;
        }

		/// <summary>
		/// Removes the element with the smallest priority and inserts a new one.
		/// </summary>
		/// <param name="element">The value to insert.</param>
		/// <param name="priority">The priority of the new value.</param>
		/// <returns>
		/// The value that was removed from the queue.
		/// </returns>
		public Optional<TElement> DequeueEnqueue ( TElement element, TPriority priority ) {
            Optional<TElement> _ret = Dequeue();
            Enqueue(element, priority);
            return _ret;
        }

		/// <summary>
		/// Inserts a new element and then removes the element with the smallest priority.
		/// </summary>
		/// <param name="element">The value to insert.</param>
		/// <param name="priority">The priority of the new value.</param>
		/// <returns>
		/// The value removed from the queue.
		/// </returns>
		public Optional<TElement> EnqueueDequeue ( TElement element, TPriority priority ) {
            Enqueue(element, priority);
            return Dequeue();
        }

		/// <summary>
		/// Inserts a sequence of (value, priority) pairs into the queue.
		/// Sorting is temporarily disabled for performance.
		/// </summary>
		/// <param name="items">The items to insert.</param>
		public void EnqueueRange ( Pair<TElement, TPriority>[] items ) {
            bool sort_state = m_autoSort;
            m_autoSort = false;

            for ( long i = items.LongLength - 1 ; i >= 0 ; i-- ) {

                if ( m_index >= m_map.Length ) {
                    if ( AutoGrow ) {
                        if ( !Grow() ) break;   
                    } else break;
                }


                m_map[m_index].First = items[i].First;
                m_map[m_index].Second = items[i].Second;
                m_index++;
            }

            m_autoSort = sort_state;
            if ( AutoSort ) Sort();
            else m_isDirty = true;
        }

		/// <summary>
		/// Inserts multiple values into the queue using the same priority for all of them.
		/// Sorting is temporarily disabled for performance.
		/// </summary>
		/// <param name="elements">The values to insert.</param>
		/// <param name="priority">The priority assigned to each value.</param>
		public void EnqueueRange ( TElement[] elements, TPriority priority ) {
            bool sort_state = m_autoSort;
            m_autoSort = false;

            for ( long i = elements.LongLength - 1 ; i >= 0 ; i-- ) {

                if ( m_index >= m_map.Length ) {
                    if ( AutoGrow ) {
                        if ( !Grow() ) break;
                    } else break;
                }


                m_map[m_index].First = elements[i];
                m_map[m_index].Second = priority;
                m_index++;
            }

            m_autoSort = sort_state;
            if ( AutoSort ) Sort();
            else m_isDirty = true;
        }
        


        /// <summary>
        /// Updates the priority of an existing value.
        /// </summary>
        public void UpdatePriority( TElement search, TPriority newPriority, long N = 1 ) {
            if ( N <= 0 ) N = 1;

            for ( int i = 0 ; i < m_index  ; i++ ) {

                if ( m_map[i].First.Equals(search) ) {
                    m_map[i].Second = newPriority;
                    N--;
                    if ( N == 0 ) break;
                }
            }
            if ( AutoSort ) Sort();
            else m_isDirty = true;
        }
		/// <summary>
		/// Ensures that the internal buffer has at least the specified capacity.
		/// </summary>
		/// <param name="capacity">The minimum required capacity.</param>
		/// <returns>
		/// The new buffer size after resizing.
		/// </returns>
		public long EnsureCapacity ( int capacity ) {
            Resize(capacity);

            return m_map.LongLength;
        }
        /// <summary>
        /// Sorts the map using either the DEFAULTCOMPAR or the delegate-based function.  
        /// Implements a simple O(n²) comparison-based sort.
        /// </summary>
        public void Sort () {
            if ( m_sorter != null ) {
                m_sorter(ref m_map, DEFAULTCOMPAR);
            } else {
                // Fallback
                for ( int i = 0 ; i < Count - 1 ; i++ ) {
                    for ( int j = i + 1 ; j < Count ; j++ ) {
                        bool cmp = DEFAULTCOMPAR.Compare( m_map.ElementAt(i), m_map.ElementAt(j) );

                        if ( !cmp ) {
                            Swap(i, j);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Swaps two elements in the internal list.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Swap ( long i, long j ) {
            var tmp = m_map[i];
            m_map[i] = m_map[j];
            m_map[j] = tmp;
        }

		/// <summary>
		/// Computes the element whose priority is closest to the statistical median.
		/// </summary>
		/// <returns>
		/// The element closest to the median priority, or null if the queue is empty.
		/// </returns>
		private Pair<TElement, TPriority>? GetClosestToMedian () {
            int n = m_map.Length;
            if ( n == 0 ) return null;

            Vector<TPriority> prios = new Vector<TPriority>(n);

            var set = Vector<TPriority>.AsMultiSet(ref prios, new Less<TPriority>());

            for(int i= 0 ; i < m_map.Length ; i++ )
                set.Insert(i, m_map[i].Second);

            TPriority median;
            if ( (n & 1) == 1 ) {
                median = prios[n / 2];
            } else {
                TPriority a = prios[(n / 2) - 1];
                TPriority b = prios[n / 2];
                median = (a + b) / TPriority.CreateChecked(2);
            }

            Pair<TElement, TPriority> best = m_map[0];
            TPriority bestDist = Abs(best.Second - median);

            for ( int i = 1; i < n; i++ ) {
                var p = m_map[i];
                TPriority dist = Abs(p.Second - median);

                if ( dist < bestDist ) {
                    best = p;
                    bestDist = dist;
                }
            }

            return best;
        }

		/// <summary>
		/// Computes the absolute value of the given priority.
		/// </summary>
		/// <param name="value">The priority value.</param>
		/// <returns>The absolute value.</returns>
		private static TPriority Abs ( TPriority value ) {
            return value < TPriority.Zero ? -value : value;
        }

        /// <summary>
        /// Grows the internal buffer by GrowSize if AutoGrow is enabled.
        /// </summary>
        /// <returns>
        /// True if growth succeeded; false if AutoGrow was disabled.
        /// </returns>
        public bool Grow () {
            if ( !AutoGrow ) return false;
            return Resize(Length + GrowSize);
        }

        /// <summary>
        /// Resizes the internal buffer to the specified size.
        /// Adjusts the logical index if it exceeds the new size.
        /// </summary>
        /// <param name="size">New buffer size.</param>
        /// <returns>
        /// True if resizing succeeded; false if resizing was unnecessary or failed.
        /// </returns>
        private bool Resize ( long size ) {
            if ( size == Length ) return false;
            if ( m_index > size )
                m_index = size;

            try {
                Array.Resize(ref m_map, (int)size);
            } catch {
                return false;
            }
            return true;
        }
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
