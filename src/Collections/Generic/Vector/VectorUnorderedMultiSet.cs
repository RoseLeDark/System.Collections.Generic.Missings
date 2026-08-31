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
using SystemEx.Utils;

namespace SystemEx.Collections.Generic {
	/// \addtogroup Collections
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
	public ref struct VectorUnorderedMultiSet<T, TContainer> : IEquatable<VectorUnorderedMultiSet<T, TContainer>>
        where TContainer : IVector<T> {

        private ref TContainer m_pKeys;
        private Find<T, TContainer> m_finder;

        /// <summary>
        ///  Creates a new unsorted view over the given container.
        /// </summary>
        /// <param name="keys">Reference to the underlying container.</param>
        public VectorUnorderedMultiSet ( ref TContainer keys ) {
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
        public T? Current => m_pKeys.Current.Value;

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
                    if ( itm.IsSome ) {
                        _ret[i] = itm.Value!;
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
        /// Removes the first occurrence of the specified value from the UnorderedMultiSet.
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
        /// Moves all elements from this UnorderedMultiSet into the specified target UnorderedMultiSet.
        /// The elements are copied into a temporary array, removed from the
        /// underlying container of this UnorderedMultiSet, and then inserted into the
        /// target UnorderedMultiSet. This UnorderedMultiSet becomes empty after the
        /// operation.
        /// </summary>
        /// <param name="other">
        /// The target UnorderedMultiSet that receives all elements from this UnorderedMultiSet.
        /// </param>
        public void SwapIn ( VectorUnorderedMultiSet<T, TContainer> other ) {

            if ( m_pKeys.Count > 0 ) {
                // copy all elements into a temporary array
                T[] tmp = m_pKeys.ToNative();

                // clear myself
                m_pKeys.Clear();

                // insert all elements into the other UnorderedMultiSet
                other.m_pKeys.InsertRange(0, tmp);

            }
        }

        /// <summary>
        /// Compares this UnorderedMultiSet with another UnorderedMultiSet for structural equality.
        /// Both UnorderedMultiSets must contain the same number of elements and each
        /// element at the same position must be equal. The comparison
        /// assumes both containers are sorted and aligned.
        /// </summary>
        /// <param name="other">The UnorderedMultiSet to compare with.</param>
        /// <returns>
        /// True if both UnorderedMultiSets contain the same elements in the same order;
        /// otherwise false.
        /// </returns>
        public bool Equals ( VectorUnorderedMultiSet<T, TContainer> other ) {
            bool _ret = true;

            if ( m_pKeys.Count != other.m_pKeys.Count ) {
                _ret = false;
            } else {
                for ( int i = 0 ; i < m_pKeys.Count ; i++ ) {
                    var a_key = m_pKeys.ElementAt(i);
                    var b_key = other.m_pKeys.ElementAt(i);

                    if ( !EqualsKey(a_key.Value, b_key.Value) ) {
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
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
        
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    }
}
