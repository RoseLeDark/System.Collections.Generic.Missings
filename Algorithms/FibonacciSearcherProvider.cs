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


using SystemEx.Collections.Generic;
using SystemEx.Utils;

namespace SystemEx.Algorithms {
	/// \addtogroup SystemEx.Algorithms
	/// @{
	/// <summary>
	/// Provides a Fibonacci‑based search strategy for containers implementing
	/// <see cref="IContainer{T}"/>.  
	/// 
	/// Fibonacci search is an alternative to binary search that uses decreasing
	/// Fibonacci intervals to narrow the search space. It is particularly suited
	/// for sorted containers and scenarios where comparison operations are more
	/// expensive than index arithmetic.
	/// </summary>
	/// <typeparam name="T">
	/// The element type stored in the container.
	/// </typeparam>
	/// <typeparam name="TContainer">
	/// The container type being searched. Must implement <see cref="IVector{T}"/>.
	/// </typeparam>
	public struct FibonacciSearcherProvider<T, TContainer> : ISearchProvider<T, TContainer>
        where TContainer : IVector<T> {

        /// <summary>
        /// Searches the container for elements matching <paramref name="value"/>
        /// using a Fibonacci search strategy and a comparison provider.
        /// 
        /// The search iteratively reduces the interval size using Fibonacci numbers,
        /// comparing elements at calculated probe positions until the target value
        /// is found or the search space is exhausted.
        /// </summary>
        /// <param name="container">
        /// The container to search. Passed by reference to avoid copying.
        /// </param>
        /// <param name="comp">
        /// The comparison provider used to evaluate the relation between container
        /// elements and the target value.
        /// </param>
        /// <param name="value">
        /// The value to search for, wrapped in <see cref="Optional{T}"/>.
        /// </param>
        /// <returns>
        /// The number of matches found.  
        /// Typically <c>0</c> or <c>1</c> for strictly ordered containers.
        /// </returns>
        public long Find ( ref TContainer container, ICompared<T> comp, T value ) {
            long _arCount= container.Count;
            long _ret = 0;

            if ( _arCount> 0 ) {
                // find the smallest Fibonacci number that equals or is greater than the array length
                long _fbPrevB = 0;
                long _fbNPrev= 1;
                long _fbNum = _fbNPrev;

                while ( _fbNum <= _arCount) {
                    _fbPrevB = _fbNPrev;
                    _fbNPrev= _fbNum;
                    _fbNum = _fbPrevB + _fbNPrev;
                }

                // offset to drop the left part of the array
                long offset = -1;

                while ( _fbNum > 1 ) {
                    var index = System.Math.Min(offset + _fbPrevB, _arCount- 1);

                    switch ( comp.Compare(container.ElementAt(index), value) ) {
                    case CompareResult.AIsLargerB:
                    _fbNum = _fbNPrev;
                    _fbNPrev= _fbPrevB;
                    _fbPrevB = _fbNum - _fbNPrev;
                    offset = index;
                    break;

                    // reject approximately 2/3 of the existing array behind
                    // by moving Fibonacci numbers
                    case CompareResult.AIsSmallerB:
                    _fbNum = _fbPrevB;
                    _fbNPrev= _fbNPrev- _fbPrevB;
                    _fbPrevB = _fbNum - _fbNPrev;
                    break;
                    default:
                    _ret = index; break;
                    }
                }


                // check the last element
                Optional<T> _it = container.ElementAt(_arCount - 1);

                if ( _it.IsSome ) {
                    if ( _fbNPrev == 1 && comp.Compare(value, _it) == CompareResult.Equal ) {
                        _ret++;
                    }
                }
            }
            return _ret;
        }
        /// <summary>
        /// Performs a best‑effort Fibonacci‑based search and counts the number of
        /// elements for which the provided predicate returns a non‑strict match.
        /// 
        /// Fibonacci search is inherently designed for locating a single target value
        /// in a sorted container. This overload extends the algorithm to count matches
        /// encountered during Fibonacci probing, but due to the nature of the search
        /// — which discards large portions of the container during interval reduction —
        /// it does <b>not</b> guarantee that all matching elements will be visited.
        /// 
        /// This method is suitable for experimental or specialized scenarios where
        /// approximate or probe‑based counting is acceptable.
        /// </summary>
        /// <param name="container">
        /// The container to search. Passed by reference to avoid copying.
        /// </param>
        /// <param name="func">
        /// A callback that evaluates each probed element and returns a
        /// <see cref="CompareResult"/> describing its relation to the desired target.
        /// </param>
        /// <returns>
        /// The number of matches encountered during Fibonacci probing.  
        /// This value may be less than the actual number of matching elements.
        /// </returns>
        public long Find ( ref TContainer container, Func<Optional<T>, CompareResult> func ) {
            long _arCount= container.Count;
            long _ret = 0;

            if ( _arCount > 0 ) {
                // find the smallest Fibonacci number that equals or is greater than the array length
                long _fbPrevB = 0;
                long _fbNPrev= 1;
                long _fbNum = _fbNPrev;

                while ( _fbNum <= _arCount ) {
                    _fbPrevB = _fbNPrev;
                    _fbNPrev = _fbNum;
                    _fbNum = _fbPrevB + _fbNPrev;
                }

                // offset to drop the left part of the array
                long offset = -1;

                while ( _fbNum > 1 ) {
                    var index = System.Math.Min(offset + _fbPrevB, _arCount- 1);

                    switch ( func(container.ElementAt(index)) ) {
                    case CompareResult.AIsLargerB:
                    _fbNum = _fbNPrev;
                    _fbNPrev = _fbPrevB;
                    _fbPrevB = _fbNum - _fbNPrev;
                    offset = index;
                    break;

                    // reject approximately 2/3 of the existing array behind
                    // by moving Fibonacci numbers
                    case CompareResult.AIsSmallerB:
                    _fbNum = _fbPrevB;
                    _fbNPrev = _fbNPrev - _fbPrevB;
                    _fbPrevB = _fbNum - _fbNPrev;
                    break;
                    default:
                    _ret++; break;
                    }
                }

                // check the last element
                Optional<T> _it = container.ElementAt(_arCount - 1);

                if ( _it.IsSome ) {
                    if ( _fbNPrev == 1 && func(_it) == CompareResult.Equal ) {
                        _ret++;
                    }
                }

               
            }
            return _ret;
        }

        /// <summary>
        /// Performs a best‑effort multi‑match search using Fibonacci probing.
        /// 
        /// Although Fibonacci search is traditionally designed for locating a single
        /// target value in a sorted container, this method extends the algorithm to
        /// collect all elements for which the provided predicate returns a non‑strict
        /// match result.
        /// 
        /// Due to the nature of Fibonacci search — which discards large portions of
        /// the search space during probing — this method does not guarantee that all
        /// matching elements will be visited. It is intended for experimental or
        /// specialized use cases only.
        /// 
        /// In other words: only Bob can do this.
        /// </summary>
        /// <param name="container">
        /// The container to search. Passed by reference to avoid copying.
        /// </param>
        /// <param name="func">
        /// A callback that evaluates each probed element and returns a
        /// <see cref="CompareResult"/> describing its relation to the desired target.
        /// </param>
        /// <returns>
        /// A <see cref="Vector{T}"/> containing index/value pairs
        /// for all elements encountered during Fibonacci probing that satisfy the
        /// predicate.
        /// </returns>
        public Vector<Pair<long, Optional<T> >> Where ( ref TContainer container, Func<Optional<T>, CompareResult> func ) {
            long _arCount= container.Count;
            Vector<Pair<long, Optional<T> >> _elements = new Vector<Pair<long, Optional<T> >>();


            if ( _arCount > 0 ) {
                // find the smallest Fibonacci number that equals or is greater than the array length
                long _fbPrevB = 0;
                long _fbNPrev= 1;
                long _fbNum = _fbNPrev;

                while ( _fbNum <= _arCount ) {
                    _fbPrevB = _fbNPrev;
                    _fbNPrev = _fbNum;
                    _fbNum = _fbPrevB + _fbNPrev;
                }

                // offset to drop the left part of the array
                long offset = -1;

                while ( _fbNum > 1 ) {
                    var _index = System.Math.Min(offset + _fbPrevB, _arCount- 1);
                    var _item = container.ElementAt(_index);

                    switch ( func(_item) ) {
                    case CompareResult.AIsLargerB:
                    _fbNum = _fbNPrev;
                    _fbNPrev = _fbPrevB;
                    _fbPrevB = _fbNum - _fbNPrev;
                    offset = _index;
                    break;

                    // reject approximately 2/3 of the existing array behind
                    // by moving Fibonacci numbers
                    case CompareResult.AIsSmallerB:
                    _fbNum = _fbPrevB;
                    _fbNPrev = _fbNPrev - _fbPrevB;
                    _fbPrevB = _fbNum - _fbNPrev;
                    break;
                    default:
                    _elements.PushBack(new Pair<long, Optional<T> >(_index, _item));  
                    break;
                    }
                }

                // check the last element
                Optional<T> _it = container.ElementAt(_arCount - 1);

                if ( _it.IsSome ) {
                    if ( _fbNPrev == 1 && func(_it) == CompareResult.Equal ) {


                        _elements.PushBack(new Pair<long, Optional<T>>(_arCount - 1, _it.Value));
                    }
                }
            }
            return _elements;
        }
    }
    /// @}
}
