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

using System.Runtime.CompilerServices;
using SystemEx.Collections.Generic.Interfaces;
using SystemEx.Utils;

namespace SystemEx.Collections.Generic {
    /// \addtogroup collections
    /// @{
    /// <summary>
    /// Represents an unordered set view over an underlying container.
    /// Elements are not sorted and no ordering guarantees are provided.
    /// Duplicate values are not allowed. All operations work directly on
    /// the underlying container without applying any sorting logic.
    /// </summary>
    /// <typeparam name="T">The element type stored in the container.</typeparam>
    /// <typeparam name="TContainer">
    /// The container type that stores the elements. 
    /// Must implement IContainerEx for the same element type.
    /// </typeparam>
    public ref struct VectorUnorderedSet<T, TContainer> : IEquatable<VectorUnorderedSet<T, TContainer>>
        where TContainer : IVector<T> {

        private ref TContainer m_pKeys;
        private Find<T, TContainer> m_finder;

        /// <summary>
        ///  Creates a new unsorted view over the given container.
        /// </summary>
        /// <param name="keys">Reference to the underlying container.</param>
        public VectorUnorderedSet ( ref TContainer keys ) {
            m_pKeys = ref keys;
            m_finder = new Find<T, TContainer>(ref keys);

        }

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
        /// Inserts a value into the container.
        /// </summary>
        /// <param name="value">The value to insert.</param>
        public bool Insert ( T value ) {
            bool _ret = false;

            if ( m_finder.First(value) == -1 )
                _ret = m_pKeys.PushBack(value);

            return _ret;
        }

        /// <summary>
        /// Inserts a value at the specified index.
        /// </summary>
        /// <param name="index">The index at which the value is inserted.</param>
        /// <param name="value">The value to insert.</param>
        public bool Insert ( long index, T value ) {
            bool _ret = false;

            if ( m_finder.First(value) == -1 )
                _ret = m_pKeys.Insert(index, value);

            return _ret;
        }

        /// <summary>
        /// Inserts a value into a range.
        /// </summary>
        /// <param name="start">Start index of the range.</param>
        /// <param name="end">End index of the range.</param>
        /// <param name="value">The value to insert.</param>
        public bool Insert ( long start, long end, T value ) {
            bool _ret = false;

            if ( m_finder.First(value) == -1 )
                _ret = m_pKeys.Insert(start, end, value);

            return _ret;
        }

        /// <summary>
        /// Inserts multiple values starting at the specified index.
        /// </summary>
        /// <param name="start">The index at which the range begins.</param>
        /// <param name="values">Array of values to insert.</param>
        public bool InsertRange ( long start, T[] values ) {
            bool insertedAny = false;

            foreach ( var item in values ) {
                // Element existiert bereits → abbrechen
                if ( m_finder.First(item) != -1 )
                    continue;

                // Einfügen fehlgeschlagen → abbrechen
                if ( !m_pKeys.PushBack(item) )
                    break;

                insertedAny = true;
            }

            return insertedAny;
        }

        /// <summary>
        /// tests is this view validated return only true
        /// </summary>
        public bool Validate () {
            return true;
        }



        /// <summary>
        /// Removes the element at the specified index.
        /// </summary>
        /// <param name="index">The index of the element to remove.</param>
        public bool Erase ( long index ) {
            bool _ret = m_pKeys.Erase(index);
            
            return _ret;
        }
        /// <summary>
        /// Removes a continuous range of elements from the underlying container.
        /// The range is defined by the start index <paramref name="first"/> and
        /// the end index <paramref name="last"/>. 
        /// </summary>
        /// <param name="first">The starting index of the range to erase.</param>
        /// <param name="last">The ending index of the range to erase.</param>
        /// <returns>
        /// True if the specified range was removed; otherwise false.
        /// </returns>
        public bool Erase ( long first, long last ) {
            bool _ret = m_pKeys.Erase(first, last);
            
            return _ret;
        }
        /// <summary>
        /// Removes the first occurrence of the specified value from the UnorderedSet.
        /// The element is located using the internal finder. If the element
        /// exists, it is removed from the underlying container. 
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
            

            return _ret;
        }

       
        /// <summary>
        /// Moves all elements from this UnorderedSet into the specified target UnorderedSet.
        /// The elements are copied into a temporary array, removed from the
        /// underlying container of this UnorderedSet, and then inserted into the
        /// target UnorderedSet. This UnorderedSet becomes empty after the
        /// operation.
        /// </summary>
        /// <param name="other">
        /// The target UnorderedSet that receives all elements from this UnorderedSet.
        /// </param>
        public void SwapIn ( VectorUnorderedSet<T, TContainer> other ) {

            if ( m_pKeys.Count > 0 ) {
                // copy all elements into a temporary array
                T[] tmp = m_pKeys.ToNative();

                // clear myself
                m_pKeys.Clear();

                // insert all elements into the other UnorderedSet
                other.m_pKeys.InsertRange(0, tmp);

            }
        }

        /// <summary>
        /// Merges all unique elements from the specified source set into this set.
        /// Each element in <paramref name="source"/> is checked using the internal
        /// finder. If this set does not contain the element, it is extracted from
        /// the source and inserted into this set. Elements already present in this
        /// set remain in the source. No elements are duplicated.
        /// </summary>
        /// <param name="source">
        /// The set from which elements are merged into this set.
        /// </param>
        public void Merge ( VectorUnorderedSet<T, TContainer> source ) {
            long count = source.m_pKeys.Count;

            for ( long i = 0 ; i < count ; i++ ) {
                T? value = source.m_pKeys.ElementAt(i);
                if ( value == null )
                    continue;

                // check if this set already contains the value
                long idx = m_finder.First(value);

                if ( idx == -1 ) {
                    // extract from source
                    T[] extracted = source.Extract(i);

                    // insert into this set
                    m_pKeys.InsertRange(m_pKeys.Count, extracted);

                    // adjust loop because source shrinks
                    count--;
                    i--;
                }
            }
        }


        /// <summary>
        /// Compares this UnorderedSet with another UnorderedSet for structural equality.
        /// Both UnorderedSets must contain the same number of elements and each
        /// element at the same position must be equal. The comparison
        /// assumes both containers are sorted and aligned.
        /// </summary>
        /// <param name="other">The UnorderedSet to compare with.</param>
        /// <returns>
        /// True if both UnorderedSets contain the same elements in the same order;
        /// otherwise false.
        /// </returns>
        public bool Equals ( VectorUnorderedSet<T, TContainer> other ) {
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
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
