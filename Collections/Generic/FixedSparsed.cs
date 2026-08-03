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


using System.Reflection.Metadata;
using SystemEx.Drawing;
using SystemEx.Threading;

namespace SystemEx.Collections.Generic {
    /// <summary>
    /// A sparse container that stores elements in fixed slots without enforcing
    /// contiguous or linear placement. Slots may be empty, and operations do not
    /// shift or compact elements. Designed for scenarios where random-access
    /// sparse storage is required without the overhead of dynamic resizing.
    /// </summary>
    /// <typeparam name="T">The element type stored in the container.</typeparam>
    public struct FixedSparsed<T> : IContainer<T> {
        private Optional<T>[] m_elements;
        private long m_index;

        /// <summary>
        /// Gets the element stored at the first slot (index 0).
        /// </summary>
        public Optional<T> Front => m_elements[0];

        /// <summary>
        /// Gets the first occupied element in the container.
        /// Throws if the container is empty.
        /// </summary>
        public T First {
            get {
                if ( IsEmpty )
                    throw new InvalidOperationException("Map is empty");

                long index = -1;
                for ( long i = 0 ; i < Length ; i++ ) {
                    if ( m_elements[i].IsSome) {
                        index = i; break;
                    }
                }

                return m_elements[index].Value!;
            }
        }
        /// <summary>
        /// Gets the element stored at the last slot (index Length - 1).
        /// </summary>
        public Optional<T> Back => m_elements[Length - 1];
        /// <summary>
        /// Gets the last occupied element in the container.
        /// Throws if the container is empty.
        /// </summary>
        public T Last {
            get {
                if ( IsEmpty )
                    throw new InvalidOperationException("Map is empty");

                long index = -1;
               
                for ( long i = Length -1; i >= 0 ; i-- ) {
                    if ( m_elements[i].IsSome ) {
                        index = i; break;
                    }
                }

                return m_elements[index].Value!;
            }
        }
        /// <summary>
        /// Indicates whether all slots are occupied.
        /// </summary>
        public bool IsFull => Length - Count == 0;

        /// <summary>
        /// Indicates whether no slots are occupied.
        /// </summary>
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// Gets the element at the current index.
        /// Throws if the container is empty.
        /// </summary>
        public Optional<T> Current {  get { if ( IsEmpty ) throw new IndexOutOfRangeException(); return m_elements[m_index]; } }

        /// <summary>
        /// Gets the number of occupied slots.
        /// </summary>
        public long Count => getUsedCount();

        /// <summary>
        /// Gets the total number of slots in the container.
        /// </summary>
        public long Length => m_elements.LongLength;

        /// <summary>
        /// Initializes a new sparse container with the specified number of slots.
        /// </summary>
        public FixedSparsed ( long N) {
            m_elements = new Optional<T>[N];
            m_index = 0;
        }


        /// <summary>
        /// Clears all elements while preserving the container size.
        /// </summary>
        public void Clear () {
            var t = Length;
            m_elements = new Optional<T>[t];
        }
        /// <summary>
        /// Returns the element at the specified index.
        /// Throws if the index is out of range.
        /// </summary>
        public Optional<T> ElementAt ( long index ) {
            if ( index < 0 || index >= Length ) throw new ArgumentOutOfRangeException("index");
            m_index = -1;

            return m_elements[index];
        }
        /// <summary>
        /// Removes the element at the current index.
        /// </summary>
        public bool Erase () {
            bool _ret = false;

            if ( m_elements[m_index].IsSome ) {
                m_elements[m_index].HasValue = false;
                _ret = true;
            }
            return _ret;
        }
        /// <summary>
        /// Removes the element at the specified index.
        /// </summary>
        public bool Erase ( long index ) {
            if ( index < 0 || index >= Length ) throw new ArgumentOutOfRangeException("index");
            bool _ret = false;

            if ( m_elements[index].IsSome ) {
                m_elements[index].HasValue = false;
                _ret = true;
            }
            return _ret;
        }

        /// <summary>
        /// Returns the underlying element type.
        /// </summary>
        public Type GetElementType () {
            return typeof(T);
        }

        /// <summary>
        /// Inserts an element into the container. This method attempts to place the
        /// element at the specified index. If the slot is empty, the element is stored
        /// there and the internal cursor (<see cref="m_index"/>) is updated to that
        /// position.
        ///
        /// If the target slot is already occupied, the method falls back to inserting
        /// the element into any available free slot (determined by <see cref="getFreeSlot"/>).
        /// In this case, the cursor is updated to the free slot that actually received
        /// the new element.
        ///
        /// This operation always moves the cursor to the slot where the element was
        /// successfully inserted. If no free slot exists, the insertion fails.
        /// </summary>
        public bool Insert ( long index, T entry ) {
            if ( index < 0 || index >= Length) return false;
            bool _ret = false;

            if ( m_elements[index].IsNull ) {
                m_index = index;
                _ret = true;

            } else {
                var _t = getFreeSlot();

                if ( _t > -1 ) {
                    m_elements[_t] = entry;
                    m_index = _t;
                    _ret = true;
                }
            }

            return _ret;
        }
        /// <summary>
        /// Inserts the same element into all slots within the specified range.
        /// </summary>
        public bool Insert ( long start, long end, T entry ) {
            if ( start < 0 || end < start ) return false;
            if ( end >= Length ) return false;

            for ( long i = start ; i <= end ; i++ )
                m_elements[i] = entry;

            m_index = end;

            return true;
        }

        /// <summary>
        /// Inserts an element near the current index if possible, otherwise
        /// selects any free slot. This operation does not shift elements and
        /// may overwrite sparse regions.
        /// </summary>
        public bool Push(T entry) {
            if ( IsFull ) return false;
            long _index = -1;

            if ( m_index >= 1 && m_index < Length - 1 ) {
                if ( m_elements[m_index + 1].IsNull ) _index = m_index + 1;
                else if ( m_elements[m_index - 1].IsNull ) _index = m_index - 1;
                else _index = getFreeSlot();
            }  else {
                _index = getFreeSlot();
            }


            m_elements[_index] = entry;
            m_index = _index;
            return true;
        }
        /// <summary>
        /// Removes the element at the current index and moves the cursor
        /// to the nearest occupied slot to the left.
        /// </summary>
        public Optional<T> Pop () {
            Optional<T> _ret = Optional<T>.NONE;

            if(m_elements[m_index].IsSome ) {
                _ret = new Optional<T>(m_elements[m_index].Value);
                m_elements[m_index].HasValue = false;

                for(long i = m_index ; i > -1; i-- ) {
                    if(m_elements[i].IsSome ) {
                        m_index = i;
                        break;
                    }
                }
            }

            return _ret;
        }

        /// <summary>
        /// Replaces the element at the specified index without modifying the
        /// container's current cursor position (<see cref="m_index"/>).
        ///
        /// Unlike <see cref="Insert(long, T)"/>, this operation does not represent
        /// a structural insertion. It simply overwrites the existing slot regardless
        /// of whether it was previously occupied. Because no new logical position is
        /// introduced, the cursor remains unchanged.
        ///
        /// Use <see cref="Insert(long, T)"/> when you want to place a new element
        /// into an empty slot *and* move the cursor to that newly occupied position.
        /// </summary>
        public bool Replace ( long index, T entry ) {
            if ( index < 0 || index >= Length ) return false;

            m_elements[index] = entry;

            return true;
        }

        /// <summary>
        /// Counts all occupied slots using parallel iteration.
        /// </summary>
        private long getUsedCount () {
            long count = 0;

            var local = m_elements;

            Parallel.For(0, Length, i =>
            {
                if ( local[i].IsSome ) {
                    Interlocked.Increment(ref count);
                }
            });

            return count;
        }

        /// <summary>
        /// Finds any free slot using parallel search. Order is not guaranteed.
        /// </summary>
        private long getFreeSlot () {
            if ( IsFull ) return -1;

            long result = -1;
            var elements = m_elements;

            Parallel.For(0, elements.LongLength, ( i, state ) =>
            {
                if ( !elements[i].IsSome ) {
                    result = i;
                    state.Stop();   // ← bricht alle weiteren Iterationen ab
                }
            });

            return result;
        }

    }

}
