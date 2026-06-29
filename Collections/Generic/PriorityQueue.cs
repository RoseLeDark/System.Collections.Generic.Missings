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
using SystemEx.Collections.Generic.Interfaces;
using SystemEx.Utils;

namespace SystemEx.Collections.Generic {
    /// \addtogroup collections
    /// @{
    /// <summary>
    /// Iterator for <see cref="PriorityQueueEx{T, TU}"/> that consumes elements
    /// in priority order.  
    /// Each call to <see cref="Forward"/> dequeues the next element from the
    /// underlying priority queue.
    /// </summary>
    /// <typeparam name="T">The stored value type.</typeparam>
    /// <typeparam name="TU">The priority type (must implement <see cref="INumber{TU}"/>).</typeparam>
    public class PriorityQueueExIterator<T, TU> : IForwardIterator<T> where TU : INumber<TU> {
        /// <summary>
        /// The queue from which elements are consumed.
        /// </summary>
        private PriorityQueueEx<T, TU> m_queue;
        /// <summary>
        /// The most recently dequeued element.
        /// </summary>
        private T? m_current;
        /// <summary>
        /// Creates a new iterator bound to the specified priority queue.
        /// </summary>
        public PriorityQueueExIterator(PriorityQueueEx<T, TU> queue) {
            m_queue = queue;
        }

        /// <summary>
        /// Gets the current element returned by the last <see cref="Forward"/> call.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if no element has been dequeued yet.</exception>
        public T Current {
            get {
                if ( m_current == null )
                    throw new ArgumentException();
                return m_current;
            }
        }

        /// <summary>
        /// Indicates whether the iterator has reached the end of the queue.
        /// </summary>
        public bool IsEnd => m_queue.Count == 0;

        /// <summary>
        /// Creates a shallow clone of this iterator referencing the same queue.
        /// </summary>
        public IIterator<T> Clone() {
            return new PriorityQueueExIterator<T, TU>(m_queue);
        }

        /// <summary>
        /// Advances the iterator by dequeuing the next element from the queue.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if the queue is empty.</exception>
        public void Forward() {
            if ( IsEnd )
                throw new ArgumentException();

            m_current = m_queue.Dequeue();
        }
    }
    /// <summary>
    /// A priority queue implemented on top of <see cref="SortedMap{T, TU}"/>.  
    /// Elements are stored as <see cref="Pair{T, TU}"/> where <c>T</c> is the value
    /// and <c>TU</c> is the priority.  
    /// Supports enqueue, dequeue, peek, priority updates, cloning, and median‑based
    /// selection.
    /// </summary>
    /// <typeparam name="T">The stored value type.</typeparam>
    /// <typeparam name="TU">
    /// The priority type, must implement <see cref="INumber{TU}"/> to allow
    /// arithmetic and comparison operations.
    /// </typeparam>
    public class PriorityQueueEx<T, TU>
        where TU : INumber<TU> {
        /// <summary>
        /// Internal sorted map storing all (value, priority) pairs.
        ///</summary>
        private readonly SortedMap<T, TU> m_map;

        /// <summary>
        /// Gets the number of elements in the queue.
        /// </summary>
        public int Count => m_map.Size;

        /// <summary>
        /// Gets the element with the smallest priority.
        /// </summary>
        public Pair<T, TU> Current => m_map[0];

        /// <summary>
        /// Gets a new iterator that consumes elements in priority order.
        /// </summary>
        public PriorityQueueExIterator<T, TU> Iterator =>
            new PriorityQueueExIterator<T, TU>(this.Clone());

        /// <summary>
        /// Gets or sets the comparer used for sorting the internal map.
        /// </summary>
        public ICompared<IPair<T, TU>>? Comparer {
            get => m_map.Comparer;
            set => m_map.Comparer = value;
        }

        /// <summary>
        /// Gets or sets the delegate‑based sorting function.
        /// </summary>
        public SortFunc<T, TU>? SortFunctions {
            get => m_map.SortFunctions;
            set => m_map.SortFunctions = value;
        }


        /// <summary>
        /// Gets the element with the minimum priority.
        /// </summary>
        public Pair<T, TU>? Min => m_map[0];

        /// <summary>
        /// Gets the element with the maximum priority.
        /// </summary>
        public Pair<T, TU>? Max => m_map[m_map.Size - 1];

        /// <summary>
        /// Gets the element whose priority is closest to the statistical median.
        /// </summary>
        public Pair<T, TU>? Median => GetClosestToMedian();

        /// <summary>
        /// Creates a priority queue using the default priority comparison.
        /// </summary>
        public PriorityQueueEx() {
            m_map = new SortedMap<T, TU>(SortByPriority);
        }

        /// <summary>
        /// Creates a priority queue using a custom comparer.
        /// </summary>
        public PriorityQueueEx(ICompared<IPair<T, TU>> comparer) {
            m_map = new SortedMap<T, TU>(comparer);
        }

        /// <summary>
        /// Creates a priority queue using a custom sorting function.
        /// </summary>
        public PriorityQueueEx(SortFunc<T, TU> sort) {
            m_map = new SortedMap<T, TU>(sort);
        }

        /// <summary>
        /// Creates a priority queue initialized with the specified elements.
        /// </summary>
        public PriorityQueueEx(IEnumerable<Pair<T, TU>> map) {
            m_map = new SortedMap<T, TU>(map, SortByPriority);
        }

        /// <summary>
        /// Creates a deep clone of this priority queue, including sorting settings.
        /// </summary>
        public PriorityQueueEx<T, TU> Clone() {
            var c = new PriorityQueueEx<T, TU>(m_map);
            c.SortFunctions = SortFunctions;
            c.Comparer = Comparer;
            c.m_map.AutoSort = m_map.AutoSort;
            return c;
        }


        /// <summary>
        /// Inserts a new value with the specified priority.
        /// </summary>
        public bool Enqueue(T item, TU priority) {
            return m_map.TryAdd(item, priority);
        }

        /// <summary>
        /// Returns the value with the smallest priority without removing it.
        /// </summary>
        public T Peek() {
            return m_map[0].First;
        }

        /// <summary>
        /// Removes and returns the value with the smallest priority.
        /// </summary>
        public T Dequeue() {
            var p = m_map[0];
            m_map.RemoveAt(0);
            return p.First;
        }

        /// <summary>
        /// Updates the priority of an existing value.
        /// </summary>
        public void UpdatePriority(T item, TU newPriority) {
            m_map[item] = newPriority;
        }

        /// <summary>
        /// Computes the element whose priority is closest to the median priority.
        /// </summary>
        private Pair<T, TU>? GetClosestToMedian() {
            int n = m_map.Size;
            if ( n == 0 )
                return null;

            SortedArray<TU> prios = new SortedArray<TU>(n, SortByPriorityArray);
            for ( int i = 0; i < n; i++ )
                prios.Add(m_map[i].Second);

            TU median;
            if ( (n & 1) == 1 ) {
                median = prios[n / 2];
            } else {
                TU a = prios[(n / 2) - 1];
                TU b = prios[n / 2];
                median = (a + b) / TU.CreateChecked(2);
            }

            Pair<T, TU> best = m_map[0];
            TU bestDist = Abs(best.Second - median);

            for ( int i = 1; i < n; i++ ) {
                var p = m_map[i];
                TU dist = Abs(p.Second - median);

                if ( dist < bestDist ) {
                    best = p;
                    bestDist = dist;
                }
            }

            return best;
        }

        private static TU Abs(TU value) {
            return value < TU.Zero ? -value : value;
        }

        private static CompareResult SortByPriorityArray(TU a, TU b) {
            if ( a < b ) return CompareResult.AIsSmallerB;
            if ( a > b ) return CompareResult.AIsLargerB;
            return CompareResult.Equal;
        }

        private static CompareResult SortByPriority(Pair<T, TU> a, Pair<T, TU> b) {
            if ( a.Second < b.Second ) return CompareResult.AIsSmallerB;
            if ( a.Second > b.Second ) return CompareResult.AIsLargerB;
            return CompareResult.Equal;
        }
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
