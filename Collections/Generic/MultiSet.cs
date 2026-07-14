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
using System.Runtime.CompilerServices;
using SystemEx.Algorithms.Interfaces;
using SystemEx.Algorythmen;
using SystemEx.Collections.Generic.Interfaces;
using SystemEx.Utils;

namespace SystemEx.Collections.Generic {

    /// <summary>
    /// Equivalent to a std::flat_multiset, but implemented as an open, non-owning sorted view 
    /// over any IContainerEx instance. The MultiSet struct does not store elements itself; 
    /// it maintains ordering by sorting the referenced container using either a 
    /// user-provided sorting delegate or a built-in BubbleSort fallback when no 
    /// delegate is supplied.
    /// </summary>
    /// <typeparam name="T">The element type stored in the container.</typeparam>
    /// <typeparam name="TContainer">
    /// The container type that stores the elements. 
    /// Must implement IContainerEx for the same element type.
    /// </typeparam>
    public ref struct MultiSet<T, TContainer> : IEquatable<MultiSet<T, TContainer>>
        where TContainer : IContainerEx<T>
         {

        private ref TContainer m_pKeys;
        private ISimpleCompare<T>  m_compare;
        private SortAction<ISimpleCompare<T>, TContainer>? m_sorter;
        private Find<T, TContainer> m_finder;


        /// <summary>
        /// Indicates whether the underlying container is full.
        /// </summary>
        public bool IsFull => m_pKeys.IsFull;

        /// <summary>
        /// Indicates whether the underlying container is empty.
        /// </summary>
        public bool IsEmpty => m_pKeys.IsEmpty;

        /// <summary>
        /// Returns the current element of the underlying container.
        /// </summary>
        public T Current => m_pKeys.Current;

        /// <summary>
        /// Returns the number of elements stored in the container.
        /// </summary>
        public long Count => m_pKeys.Count;

        /// <summary>
        /// Returns the total capacity of the underlying container.
        /// </summary>
        public long Length => m_pKeys.Length;

        /// <summary>
        ///  Creates a new sorted view over the given container.
        /// The container reference is stored and immediately sorted using the comparer and with an optional  sort delegate
        /// </summary>
        /// <param name="keys">Reference to the underlying container.</param>
        /// <param name="sorter">Sort delegate, when null use bubble sort</param>
        public MultiSet ( ref TContainer keys,  SortAction<ISimpleCompare<T>, TContainer>? sorter = null ) {
            m_pKeys = ref keys;
            m_compare = new Less<T>();
            m_sorter = sorter;
            m_finder = new Find<T, TContainer>(ref m_pKeys);

            Sort();
        }

        /// <summary>
        ///  Creates a new sorted view over the given container.
        /// The container reference is stored and immediately sorted using the comparer and with an optional  sort delegate
        /// </summary>
        /// <param name="keys">Reference to the underlying container.</param>
        /// <param name="comparer">Comparer used to order the elements.</param>
        /// <param name="sorter">Sort delegate, when null use bubble sort</param>
        public MultiSet ( ref TContainer keys, ISimpleCompare<T> comparer, SortAction<ISimpleCompare<T>, TContainer>? sorter = null ) {
            m_pKeys = ref keys;
            m_compare = comparer;
            m_sorter = sorter;
            m_finder = new Find<T, TContainer>(ref m_pKeys);

            Sort();
        }
        /// <summary>
        /// Extracts all elements starting at the given index. The extracted
        /// elements are removed from the underlying container and returned
        /// as a new array.
        /// </summary>
        /// <param name="index">Start index of extraction.</param>
        /// <returns>
        /// A new array containing all elements from index to the end.
        /// If the index is out of range, an empty array is returned.
        /// </returns>
        public T[] Extract ( long index ) {
            T[] _ret = Array.Empty<T>();

            long count = m_pKeys.Count;

            if ( index >= 0 && index < count ) {
                long extractCount = count - index;
                _ret = new T[extractCount];

                // copy extracted elements
                for ( long i = 0 ; i < extractCount ; i++ ) {
                    var itm = m_pKeys.ElementAt(index + i);
                    if ( itm != null ) {
                        _ret[i] = itm;
                    }
                }

                // remove extracted elements from container
                for ( long i = count - 1 ; i >= index ; i-- ) {
                    m_pKeys.Erase(i);
                }
            }

            return _ret;
        }
        /// <summary>
        /// Inserts a value into the container and re-sorts the elements.
        /// </summary>
        /// <param name="value">The value to insert.</param>
        public bool Insert ( T value ) {
            bool _ret = false;

            _ret = m_pKeys.PushBack(value);

            if ( _ret ) Sort();
            return _ret;
        }

        /// <summary>
        /// Inserts a value at the specified index and re-sorts the elements.
        /// </summary>
        /// <param name="index">The index at which the value is inserted.</param>
        /// <param name="value">The value to insert.</param>
        public bool Insert ( long index, T value ) {
            bool _ret = false;

            _ret = m_pKeys.Insert(index, value);

            if ( _ret ) Sort();
            return _ret;
        }

        /// <summary>
        /// Inserts a value into a range and re-sorts the elements.
        /// </summary>
        /// <param name="start">Start index of the range.</param>
        /// <param name="end">End index of the range.</param>
        /// <param name="value">The value to insert.</param>
        public bool Insert ( long start, long end, T value ) {
            bool _ret = false;

            _ret = m_pKeys.Insert(start, end, value);

            if ( _ret ) Sort();
            return _ret;
        }

        /// <summary>
        /// Inserts multiple values starting at the specified index and re-sorts the elements.
        /// </summary>
        /// <param name="start">The index at which the range begins.</param>
        /// <param name="values">Array of values to insert.</param>
        public bool InsertRange ( long start, T[] values ) {
            bool insertedAny = false;

            foreach ( var item in values ) {

                // Einfügen fehlgeschlagen → abbrechen
                if ( !m_pKeys.PushBack(item) )
                    break;

                insertedAny = true;
            }

            if ( insertedAny ) Sort();
            return insertedAny;
        }

        /// <summary>
        /// Validates the internal ordering of the set. The container must
        /// be sorted according to the comparison function.
        /// </summary>
        /// <returns>
        /// True if the container is sorted;
        /// otherwise false.
        /// </returns>
        public bool Validate () {
            bool _ret = true;

            if ( m_pKeys.Count > 1 ) {
                for ( int i = 1 ; i < m_pKeys.Count ; i++ ) {
                    var prev = m_pKeys.ElementAt(i - 1); // T?
                    var curr = m_pKeys.ElementAt(i);     // T?

                    if ( prev == null || curr == null ) {
                        _ret = false;
                        break;
                    }

                    if (  !this.m_compare.Compare(prev, curr)  ) {
                        _ret = false;
                        break;
                    }
                }
            }

            return _ret;
        }


        /// <summary>
        /// Removes the element at the specified index and re-sorts the remaining elements.
        /// </summary>
        /// <param name="index">The index of the element to remove.</param>
        public bool Erase ( long index ) {
            bool _ret = m_pKeys.Erase(index);
            if ( _ret ) Sort();
            return _ret;
        }
        /// <summary>
        /// Removes a continuous range of elements from the underlying container.
        /// The range is defined by the start index <paramref name="first"/> and
        /// the end index <paramref name="last"/>. If the removal succeeds, the
        /// set is re-sorted to maintain ordering guarantees.
        /// </summary>
        /// <param name="first">The starting index of the range to erase.</param>
        /// <param name="last">The ending index of the range to erase.</param>
        /// <returns>
        /// True if the specified range was removed; otherwise false.
        /// </returns>
        public bool Erase ( long first, long last ) {
            bool _ret = m_pKeys.Erase(first, last);
            if ( _ret ) Sort();
            return _ret;
        }
        /// <summary>
        /// Removes the first occurrence of the specified value from the set.
        /// The element is located using the internal finder. If the element
        /// exists, it is removed from the underlying container. After removal,
        /// the set is re-sorted to preserve ordering guarantees.
        /// </summary>
        /// <param name="value">The value to erase.</param>
        /// <returns>
        /// True if the value was found and removed; otherwise false.
        /// </returns>
        public bool Erase ( T value ) {
            bool _ret = false;
            long index = m_finder.First(value);

            if ( index != -1 ) {
                _ret = m_pKeys.Erase(index);
            }
            if ( _ret ) Sort();

            return _ret;
        }

        /// <summary>
        /// Returns the index of the first element not less than key.
        /// If no such element exists, returns -1.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long LowerBound ( T key ) {
            long _ret = -1;

            long lo = 0;
            long hi = m_pKeys.Count;

            while ( lo < hi ) {
                long mid = (lo + hi) >> 1;
                T? val = m_pKeys.ElementAt(mid);

                if ( val == null ) {
                    lo = mid + 1;
                    continue;
                }

                // val < key → move right
                if ( !m_compare.Compare(val, key) ) {
                    lo = mid + 1;
                } else {
                    _ret = mid;
                    hi = mid;
                }
            }

            return _ret;
        }
        /// <summary>
        /// Moves all elements from this set into the specified target set.
        /// The elements are copied into a temporary array, removed from the
        /// underlying container of this set, and then inserted into the
        /// target set. After insertion, the target set is re-sorted to
        /// maintain ordering guarantees. This set becomes empty after the
        /// operation.
        /// </summary>
        /// <param name="other">
        /// The target set that receives all elements from this set.
        /// </param>
        public void SwapIn ( MultiSet<T, TContainer> other ) {

            if ( m_pKeys.Count > 0 ) {
                // copy all elements into a temporary array
                T[] tmp = m_pKeys.ToNative();

                // clear myself
                m_pKeys.Clear();

                // insert all elements into the other set
                other.m_pKeys.InsertRange(0, tmp);

                // sort the other set (flat_set semantics)
                other.Sort();
            }
        }



        /// <summary>
        /// Returns the index of the first element greater than key. 
        /// If no such element exists, returns -1.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long UpperBound ( T key ) {
            long _ret = -1;

            long lo = 0;
            long hi = m_pKeys.Count;

            while ( lo < hi ) {
                long mid = (lo + hi) >> 1;
                T? val = m_pKeys.ElementAt(mid);

                if ( val == null ) {
                    lo = mid + 1;
                    continue;
                }

                // val <= key → move right
                if ( !m_compare.Compare(val, key)  ) {
                    lo = mid + 1;
                } else {
                    _ret = mid;
                    hi = mid;
                }
            }

            return _ret;
        }

        /// <summary>
        /// Compares this set with another set for structural equality.
        /// Both sets must contain the same number of elements and each
        /// element at the same position must be equal. The comparison
        /// assumes both containers are sorted and aligned.
        /// </summary>
        /// <param name="other">The set to compare with.</param>
        /// <returns>
        /// True if both sets contain the same elements in the same order;
        /// otherwise false.
        /// </returns>
        public bool Equals ( MultiSet<T, TContainer> other ) {
            bool _ret = true;

            if ( m_pKeys.Count != other.m_pKeys.Count ) {
                _ret = false;
            } else {
                for ( int i = 0 ; i < m_pKeys.Count ; i++ ) {
                    var a_key = m_pKeys.ElementAt(i);
                    var b_key = other.m_pKeys.ElementAt(i);

                    if ( !EqualsKey(a_key, b_key) ) {
                        _ret = false;
                        break;
                    }
                }
            }

            return _ret;
        }
        /// <summary>
        /// Compares two nullable key values for equality.
        /// Null values are treated as non-equal.
        /// </summary>
        /// <param name="a">First key.</param>
        /// <param name="b">Second key.</param>
        /// <returns>
        /// True if both keys are non-null and equal; otherwise false.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool EqualsKey ( T? a, T? b ) {
            if ( a == null ) return false;
            if ( b == null ) return false;

            return a.Equals(b);
        }


        /// <summary>
        /// Sorts the underlying container using the provided comparer.
        /// A simple comparison-based sorting algorithm is used to ensure
        /// that the container remains ordered after modifications.
        /// </summary>
        private void Sort () {
            if ( m_sorter != null ) {
                m_sorter(ref m_pKeys, m_compare);
            } else {
                // Fallback
                for ( int i = 0 ; i < m_pKeys.Count - 1 ; i++ ) {
                    for ( int j = i + 1 ; j < m_pKeys.Count ; j++ ) {
                        var cmp = m_compare.Compare( m_pKeys.ElementAt(i), m_pKeys.ElementAt(j) );

                        if ( cmp ) {
                            Swap(i, j);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Swaps two elements inside the underlying container.
        /// Used internally by the sorting routine to reorder elements.
        /// </summary>
        /// <param name="i">Index of the first element.</param>
        /// <param name="j">Index of the second element.</param>
        private void Swap ( long i, long j ) {
            m_pKeys.Swap(i, j);
        }

    }
}
