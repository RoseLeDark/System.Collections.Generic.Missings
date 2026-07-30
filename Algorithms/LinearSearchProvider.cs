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
using SystemEx.Utils;
using SystemEx.Collections.Generic;
using SystemEx.Algorithms.Interfaces;
using SystemEx.Collections.Generic.Interfaces;


namespace SystemEx.Algorithms {
    /// \addtogroup Algorithms
    /// @{
    /// <summary>
    /// A simple fallback search provider that performs a linear scan over the
    /// container and counts how many elements match the given condition.
    ///
    /// <para>
    /// <b>Important:</b>
    /// This provider does <b>not</b> return an index. It returns the
    /// <b>number of matches</b>. The <see cref="VectorSearch{T, TContainer}"/>
    /// system is designed for complex search algorithms that may evaluate
    /// multiple matches, patterns, or conditions. It is not a replacement
    /// for the traditional <c>Find</c> API.
    /// </para>
    ///
    /// <para>
    /// <b>Purpose:</b>
    /// LinearSearchProvider serves as a universal fallback implementation.
    /// More advanced providers (binary search, segmented search, pattern
    /// matching, domain‑specific logic) should be implemented separately.
    /// </para>
    /// </summary>
    public struct LinearSearchProvider<T, TContainer> : ISearchProvider<T, TContainer>
        where TContainer : IVector<T> {

        /// <inheritdoc />
        public long Find ( ref TContainer container, ICompared<T> comp, T? value ) {
            long _ret = 0;

            for ( var i = 0 ; i < container.Count ; i++ ) {
                var item = container.ElementAt(i);

                if ( comp.Compare(item, value) == CompareResult.Equal ) {
                    _ret++;
                }
            }
            return _ret;
        }
        /// <inheritdoc />
        public long Find ( ref TContainer container, Func<Optional<T>, CompareResult> func ) {
            long _ret = 0;

            for ( var i = 0 ; i < container.Count ; i++ ) {
                Optional<T> item = container.ElementAt(i);

                if ( func(item) == CompareResult.Equal) {
                    _ret++;
                }
            }
            return _ret;
        }

        /// <inheritdoc />
        public Vector<Pair<long, Optional<T>>> Where ( ref TContainer container, Func<Optional<T>, CompareResult> func ) {
            Vector<Pair<long, Optional<T>>> _elements = new Vector<Pair<long, Optional<T>>>();

            for ( var i = 0 ; i < container.Count ; i++ ) {
                Optional<T> item = container.ElementAt(i);

                if ( func(item) == CompareResult.Equal  ) {
                    _elements.PushBack(new Pair<long, Optional<T>>(i, item));
                }
            }

            return _elements;
        }
    }
    /// @}
}
