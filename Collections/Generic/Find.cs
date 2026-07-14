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
using System.Runtime.InteropServices;
using SystemEx.Algorithms.Interfaces;
using SystemEx.Collections.Generic.Interfaces;
using SystemEx.Utils;

namespace SystemEx.Collections.Generic {
    /// \addtogroup collections
    /// @{
    /// <summary>
    /// Provides search operations over a container implementing <see cref="IContainerEx{T}"/>.
    /// This struct performs non‑modifying lookups such as first/last occurrence,
    /// existence checks and counting matches.
    /// 
    /// <example>
    /// Example usage with a vector:
    /// <code>
    /// // Create a vector of int with initial values and auto grow enabled
    /// // AutoGrow is controlled by the second parameter:
    /// // growSize > 0  -> AutoGrow ON
    /// // growSize == 0 -> AutoGrow OFF
    /// var vec = new vector&lt;int&gt;( new int[]{10, 20, 30, 40 } );
    ///
    /// var finder = new Find&lt;int, vector&lt;int&gt;&gt;(ref vec);
    ///
    /// long first20 = finder.First(20);   // returns 1
    /// long last20  = finder.Last(20);    // returns 2
    /// int count20  = finder.Of(20);      // returns 2
    /// bool exists30 = finder.Exists(30); // returns true
    /// </code>
    /// </example>
    /// </summary>
    /// <typeparam name="T">The element type stored inside the container.</typeparam>
    /// <typeparam name="TContainer">
    /// The container type implementing <see cref="IContainerEx{T}"/> used as the search target.
    /// </typeparam>
    public ref struct Find<T, TContainer> where TContainer : IContainerEx<T> {

        private ref TContainer m_container;

        /// <summary>
        /// Initializes a new search helper bound to the specified container.
        /// </summary>
        /// <param name="container">Reference to the container to operate on.</param>
        public Find ( ref TContainer container ) {
            m_container = ref container;
        }


        /// <summary>
        /// Returns the index of the first occurrence of the specified value.
        /// If the value is not found, -1 is returned.
        /// </summary>
        /// <param name="value">The value to search for.</param>
        public long First ( T value ) {
            long _ret = -1;
            for ( long i = 0 ; i < m_container.Count ; i++ ) {
                var item =  m_container.ElementAt(i);

                if ( item != null && item.Equals(value) ) {
                    _ret = i;
                    break;
                }
            }
            return _ret;
        }

        /// <summary>
        /// Returns the index of the first element matching the given predicate.
        /// If no element matches, -1 is returned.
        /// </summary>
        /// <param name="pred">The predicate used to test each element.</param>
        public long First ( Func<T, bool> pred ) {
            long _ret = -1;

            for ( long i = 0 ; i < m_container.Count ; i++ ) {
                var item = m_container.ElementAt(i);

                if ( item != null && pred(item) ) {
                    _ret = i;
                    break;
                }
            }
            return _ret;
        }

        /// <summary>
        /// Returns the index of the last occurrence of the specified value.
        /// If the value is not found, -1 is returned.
        /// </summary>
        /// <param name="value">The value to search for.</param>
        public long Last ( T value ) {
            long _ret = -1;
            for ( long i = m_container.Count - 1 ; i >= 0 ; i-- ) {
                var item =  m_container.ElementAt(i);

                if ( item != null && item.Equals(value) ) {
                    _ret = i;
                    break;
                }
            }
            return _ret;
        }

        /// <summary>
        /// Returns the index of the last element matching the given predicate.
        /// If no element matches, -1 is returned.
        /// </summary>
        /// <param name="pred">The predicate used to test each element.</param>
        public long Last ( Func<T, bool> pred ) {
            long _ret = -1;

            for ( long i = m_container.Count - 1 ; i >= 0 ; i-- ) {
                var item = m_container.ElementAt(i);

                if ( item != null && pred(item) ) {
                    _ret = i;
                    break;
                }
            }
            return _ret;
        }

        /// <summary>
        /// Counts how many elements in the container equal the specified value.
        /// </summary>
        /// <param name="value">The value to count.</param>
        public int Of ( T value ) {
            int _ret = 0;
            for ( long i = 0 ; i < m_container.Count ; i++ ) {
                var item =  m_container.ElementAt(i);

                if ( item != null && item.Equals(value) ) {
                    _ret++;
                }
            }
            return _ret;
        }

        /// <summary>
        /// Counts how many elements in the container satisfy the given predicate.
        /// </summary>
        /// <param name="pred">The predicate used to test each element.</param>
        public int Where ( Func<T, bool> pred ) {
            int _ret = -1;
            for ( long i = 0 ; i < m_container.Count ; i++ ) {
                var item =  m_container.ElementAt(i);

                if ( item != null && pred(item) ) {
                    _ret++;
                }
            }
            return _ret;
        }

        /// <summary>
        /// Determines whether the container contains at least one element equal to the specified value.
        /// </summary>
        /// <param name="value">The value to search for.</param>
        public bool Exists ( T value ) {
            bool _ret = false;
            for ( int i = 0 ; i < m_container.Count ; i++ ) {
                var item = m_container.ElementAt(i);

                if ( item != null && item.Equals(value) ) {
                    _ret = true;
                    break;
                }
            }
            return _ret;
        }


        /// <summary>
        /// Attempts to find the index of the specified key.
        /// </summary>
        public bool TryGet ( T Key, out int index ) {
            for ( int i = 0 ; i < m_container.Count ; i++ ) {
                var p = m_container.ElementAt(i);

                if ( p != null && p.Equals(Key) ) {
                    index = i;
                    return true;
                }
            }
            index = -1;
            return false;
        }

        /// <summary>
        /// Finds the first index at which the element is not smaller than the
        /// specified key. The comparison is performed using the provided
        /// <see cref="ISimpleCompare{T}"/> predicate, which defines the relation
        /// <c>a &lt; b</c>.
        /// </summary>
        /// <param name="key">The key used for comparison.</param>
        /// <param name="cmp">
        /// A simple comparison predicate that returns <c>true</c> when
        /// <paramref name="a"/> is considered smaller than <paramref name="b"/>.
        /// </param>
        /// <returns>
        /// The index of the first element that is not smaller than
        /// <paramref name="key"/>, or -1 if no such element exists.
        /// </returns>
        public long LowerBound( T key, ISimpleCompare<T> cmp ) {
            long _ret = -1;

            long lo = 0;
            long hi = m_container.Count;

            while ( lo < hi ) {
                long mid = (lo + hi) >> 1;
                T? val = m_container.ElementAt(mid);

                if ( val == null ) {
                    lo = mid + 1;
                    continue;
                }

                
                // val < key → move right
                if ( cmp.Compare(val, key)  ) {
                    lo = mid + 1;
                } else {
                    _ret = mid;
                    hi = mid;
                }
            }

            return _ret;
        }


        /// <summary>
        /// Finds the first index at which the element is strictly larger than
        /// the specified key. The comparison is performed using the provided
        /// <see cref="ISimpleCompare{T}"/> predicate, which defines the relation
        /// <c>a &lt; b</c>.
        /// </summary>
        /// <param name="key">The key used for comparison.</param>
        /// <param name="cmp">
        /// A simple comparison predicate that returns <c>true</c> when
        /// <paramref name="a"/> is considered smaller than <paramref name="b"/>.
        /// </param>
        /// <returns>
        /// The index of the first element that is strictly larger than
        /// <paramref name="key"/>, or -1 if no such element exists.
        /// </returns>
        public long UpperBound ( T key, ISimpleCompare<T> cmp ) {
            long _ret = -1;

            long lo = 0;
            long hi = m_container.Count;

            while ( lo < hi ) {
                long mid = (lo + hi) >> 1;
                T? val = m_container.ElementAt(mid);

                if ( val == null ) {
                    lo = mid + 1;
                    continue;
                }
                
                // val <= key → move right
                if ( cmp.Compare(val, key)  ) {
                    lo = mid + 1;
                } else {
                    _ret = mid;
                    hi = mid;
                }
            }

            return _ret;
        }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
        /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    }
}
