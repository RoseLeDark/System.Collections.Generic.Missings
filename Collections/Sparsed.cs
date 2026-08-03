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
    /// <summary>
    /// A sparse container that stores elements in dynamic slots without enforcing
    /// contiguous or linear placement. Slots may be empty, and operations do not
    /// shift or compact elements. Designed for scenarios where random-access
    /// sparse storage is required with dynamic resizing.
    /// </summary>
    /// <typeparam name="T">The element type stored in the container.</typeparam>
    public struct Sparsed<T> : IContainer<T>, IAutoGrowe, IEnumerable<T>, IEquatable<Sparsed<T>>, ITraverse<Optional<T>>, ISwappable<long> {
        private Optional<T>[] m_elements;
        private long m_index;
        private long m_growSize;
        private bool m_autoGrow;

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
                    if ( m_elements[i].IsSome ) {
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

                for ( long i = Length - 1 ; i >= 0 ; i-- ) {
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
        public Optional<T> Current { get { if ( IsEmpty ) throw new IndexOutOfRangeException(); return m_elements[m_index]; } }

        /// <summary>
        /// Gets the number of occupied slots.
        /// </summary>
        public long Count => getUsedCount();

        /// <summary>
        /// Gets the total number of slots in the container.
        /// </summary>
        public long Length => m_elements.LongLength;

        /// <summary>
        /// 
        /// </summary>
        public long GrowSize {
            get => (m_autoGrow ? m_growSize : 0);
            set {
                m_growSize = value;
                m_autoGrow = (m_growSize > 0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AutoGrow { get => (m_growSize == 0 ? false : m_autoGrow); set => m_autoGrow = value; }


        /// <summary>
        /// Initializes a new sparse container with the specified number of slots.
        /// </summary>
        public Sparsed ( long N , int growSize = 4) {
            m_elements = new Optional<T>[N];
            m_growSize = growSize;
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
            if ( index < 0 ) return false;
            
            // Grow wie im Indexer
            if ( index >= m_elements.Length  ) {
                if ( AutoGrow ) {
                    if ( !Resize(index + GrowSize) )
                        return false;
                } else {
                    return false;
                }
            }

            bool _ret =  false;

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
            if ( end >= Length || start >= Length ) Resize(end + GrowSize);

            for ( long i = start ; i < end ; i++ ) {
                if ( !Insert(i, entry) )
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Inserts an element near the current index if possible, otherwise
        /// selects any free slot. This operation does not shift elements and
        /// may overwrite sparse regions.
        /// </summary>
        public bool Push ( T entry ) {
            if ( IsFull ) return false;
            long _index = -1;

            if ( m_index >= 1 && m_index < Length - 1 ) {
                if ( m_elements[m_index + 1].IsNull ) _index = m_index + 1;
                else if ( m_elements[m_index - 1].IsNull ) _index = m_index - 1;
                else _index = getFreeSlot();
            } else {
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

            if ( m_elements[m_index].IsSome ) {
                _ret = new Optional<T>(m_elements[m_index].Value);
                m_elements[m_index].HasValue = false;

                for ( long i = m_index ; i > -1 ; i-- ) {
                    if ( m_elements[i].IsSome ) {
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
        /// </summary
        public bool Replace ( long index, T entry ) {
            if ( index < 0 ) return false;

            if ( index >= Length ) {
                if ( AutoGrow ) {
                    if ( !Resize(index+GrowSize) )
                        return false;
                } else {
                    return false;
                }
            }

            m_elements[index] = entry;

            return true;
        }

        /// <summary>
        /// Counts all occupied slots using parallel iteration.
        /// </summary>
        private long getUsedCount () {
            long count = 0;

            var local = m_elements;

            Parallel.For(0, Length, i => {
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

            Parallel.For(0, Length, ( i, state ) => {
                if ( !elements[i].IsSome ) {
                    result = i;
                    state.Stop();   // ← bricht alle weiteren Iterationen ab
                }
            });

            if ( result < 0 ) {
                var oldLength = Length;
                if ( Grow() ) {
                    result = oldLength;
                }

            }
            return result;
        }
        /// <inheritdoc/>
        public bool Grow () {
            if ( !AutoGrow ) return false;
            return Resize(Length + GrowSize); ;
        }

        /// <inheritdoc/>
        private bool Resize ( long size ) {
            if ( size == Length ) return false;

            try {
                Array.Resize(ref m_elements, (int)size);
            } catch {
                return false;
            }
            return true;
        }

        /// <inheritdoc/>
        public bool Equals ( Sparsed<T> other ) {
            bool _ret = true;

            if ( Length != other.Length ) {
                _ret = false;
            } else {
                for ( long i = 0 ; i < other.Length ; i++ ) {
                    var a =       ElementAt(i);
                    var b = other.ElementAt(i);

                    if ( a != b ) {
                        _ret = false;
                        break;
                    }
                }
            }
            return _ret;
        }
        /// <inheritdoc/>
        public override bool Equals ( object? obj ) {
            if ( obj is Sparsed<T> k ) return this.Equals(k);
            return false;
        }
        /// <inheritdoc/>
        public override int GetHashCode () {
            return m_elements.GetHashCode() ^ m_index.GetHashCode();
        }
        /// <inheritdoc/>
        public void Swap ( long i, long j ) {
            if ( i < 0 || j < 0 ) return;
            if ( i >= Length || j >= Length ) return;

            var tmp = m_elements[i];
            m_elements[i] = m_elements[j];
            m_elements[j] = tmp;
        }
        /// <inheritdoc/>
        public void Traverse ( TraversMode mode, long startIndex, long endIndex, Action<Optional<T>> func ) {
            var start = System.Math.Max(startIndex, 0);
            var end = System.Math.Min(endIndex,  m_index);

            if ( mode == TraversMode.Forwards ) {
                for ( long i = start ; i < end ; i++ ) {
                    if ( m_elements[i].IsSome  )
                        func(m_elements[i]);
                }
            } else if ( mode == TraversMode.Backwards ) {
                for ( long i = end ; i >= start ; i-- ) {
                    if ( m_elements[i].IsSome )
                        func(m_elements[i]);
                }
            }
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator () {
            for(long i = 0 ; i < Length ; i++) {
                if(m_elements[i].IsSome ) {
                    yield return m_elements[i].Value!;
                }
            }
        }
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator () {
            return GetEnumerator();
        }
        /// <inheritdoc/>
        

        /// <inheritdoc/>
        public static bool operator == ( Sparsed<T> a, Sparsed<T> b ) {
            return  a.Equals(b);
        }

        /// <inheritdoc/>
        public static bool operator != ( Sparsed<T> a, Sparsed<T> b ) {
            return !(a == b);
        }
    }

}
