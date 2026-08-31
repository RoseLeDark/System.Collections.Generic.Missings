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

using SystemEx.Algorithms;
using SystemEx.Utils;

namespace SystemEx.Collections.Generic {
	/// \addtogroup Collections
	/// @{

	/// <summary>
	/// Represents a generic iterator over a collection.
	/// Provides access to the current element and supports advancing the iterator.
	/// </summary>
	/// <typeparam name="T">The element type.</typeparam>
	public interface Iterrator<T> {

        /// <summary>
        /// Gets a value indicating whether the iterator has reached the end of the sequence.
        /// </summary>
        bool IsEnd { get; }

        /// <summary>
        /// Gets or sets the current index of the iterator.
        /// </summary>
        long Index { get; set; }

        /// <summary>
        /// Advances the iterator by one position.
        /// </summary>
        void Forward ();

        /// <summary>
        /// Gets the element at the current iterator position.
        /// Returns an <see cref="Optional{T}"/> that may be empty if the iterator is out of range.
        /// </summary>
        Optional<T> Current { get; }
    }

    /// <summary>
    /// A forward-only iterator over a container.
    /// Supports sequential access starting at a specified index.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <typeparam name="TCollection">The underlying container type.</typeparam>
    public struct ForwardIterrator<T, TCollection>: Iterrator<T>
        where TCollection : IContainer<T> {

        private  TCollection m_collection;
        private long m_index;

        /// <summary>
        /// Gets a value indicating whether the iterator has reached the end of the container.
        /// </summary>
        public bool IsEnd => m_index >= m_collection.Count;

        /// <summary>
        /// Gets a value indicating whether another element is available.
        /// </summary>
        public bool IsNext => !IsEnd;

        /// <inheritdoc/>
        public long Index {
            get => m_index;
            set => m_index = value;
        }
        /// <inheritdoc/>
        public Optional<T> Current => m_collection.ElementAt(m_index);

        /// <summary>
        /// Initializes a new instance of the <see cref="ForwardIterrator{T, TCollection}"/> struct.
        /// </summary>
        /// <param name="collection">The container to iterate over.</param>
        /// <param name="index">The starting index.</param>
        public ForwardIterrator ( TCollection collection, long index ) {
            m_collection = collection;
            m_index = index;
        }

        /// <inheritdoc/>
        public void Forward () {
            if ( !IsEnd )
                m_index++;
        }

        /// <summary>
        /// Advances the iterator by <paramref name="i"/> positions.
        /// Stops early if the end of the container is reached.
        /// </summary>
        /// <param name="i">The number of positions to advance.</param>
        public void Forward ( long i ) {
            while ( i != 0 ) {
                if ( IsEnd ) break;
                m_index++;
                i--;
            }
        }

    }
    /// <summary>
    /// A bidirectional iterator supporting forward and backward movement.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <typeparam name="TCollection">The underlying container type.</typeparam>
    public struct BidirectionalIterator<T, TCollection> : Iterrator<T>
        where TCollection : IContainer<T> {

        private TCollection m_collection;
        private long m_index;

        /// <inheritdoc/>
        public bool IsEnd => m_index >= m_collection.Count;

        /// <summary>
        /// Gets a value indicating whether another element is available.
        /// </summary>
        public bool IsNext => !IsEnd;

        /// <inheritdoc/>
        public long Index {
            get => m_index;
            set => m_index = value;
        }

        /// <inheritdoc/>
        public Optional<T> Current {
            get => m_collection.ElementAt(m_index);
            set => m_collection.Replace(m_index, Current.Value!);
        }

        /// <summary>
        /// Gets a value indicating whether the iterator is at the beginning.
        /// </summary>
        public bool IsBegin => throw new NotImplementedException();

        /// <summary>
        /// Initializes a new instance of the <see cref="BidirectionalIterator{T, TCollection}"/> struct.
        /// </summary>
        /// <param name="collection">The container to iterate over.</param>
        /// <param name="index">The starting index.</param>
        public BidirectionalIterator ( TCollection collection, long index ) {
            m_collection = collection;
            m_index = index;
        }

        /// <inheritdoc/>
        public void Forward () {
            if ( !IsEnd )
                m_index++;
        }

        /// <summary>
        /// Advances the iterator by <paramref name="i"/> positions.
        /// </summary>
        /// <param name="i">The number of positions to advance.</param>
        public void Forward ( long i ) {
            while ( i != 0 ) {
                if ( IsEnd ) break;
                m_index++;
                i--;
            }
        }

        /// <summary>
        /// Moves the iterator one position backward.
        /// </summary>
        public void Back () {
            if ( m_index != 0 )
                m_index--;
        }
    }



    /// <summary>
    /// A random-access iterator supporting forward, backward, and indexed movement.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <typeparam name="TCollection">The underlying container type.</typeparam>
    public struct RandomAccessIterator<T, TCollection> : Iterrator<T>
       where TCollection : IContainer<T> {

        private  TCollection m_collection;
        private long m_index;


        /// <inheritdoc/>
        public bool IsEnd => m_index >= m_collection.Count;

        /// <summary>
        /// Gets a value indicating whether another element is available.
        /// </summary>
        public bool IsNext => !IsEnd;

        /// <inheritdoc/>
        public Optional<T> Current => m_collection.ElementAt(m_index);

        /// <inheritdoc/>
        public long Index {
            get => m_index;
            set => m_index = value;
        }

        /// <summary>
        /// Gets a value indicating whether the iterator is positioned at the beginning.
        /// </summary>
        public bool IsBegin => throw new NotImplementedException();

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomAccessIterator{T, TCollection}"/> struct.
        /// </summary>
        /// <param name="collection">The container to iterate over.</param>
        /// <param name="index">The starting index.</param>
        public RandomAccessIterator ( TCollection collection, long index ) {
            m_collection = collection;
            m_index = index;
        }

        /// <inheritdoc/>
        public void Forward () {
            if ( !IsEnd )
                m_index++;
        }

        /// <summary>
        /// Advances the iterator by <paramref name="i"/> positions.
        /// </summary>
        /// <param name="i">The number of positions to advance.</param>
        public void Forward ( long i ) {
            while ( i != 0 ) {
                if ( IsEnd ) break;
                m_index++;
                i--;
            }
        }

        /// <summary>
        /// Moves the iterator one position backward.
        /// </summary>
        public void Back () {
            if ( m_index != 0 )
                m_index--;
        }

        /// <summary>
        /// Moves the iterator forward or backward by <paramref name="n"/> positions.
        /// </summary>
        /// <param name="n">The number of positions to move. Positive values move forward; negative values move backward.</param>
        /// <returns>The updated iterator.</returns>
        public RandomAccessIterator<T, TCollection> Advance ( long n ) {
            while ( n > 0 ) {
                --n;
                Forward();
            }
            while ( n < 0 ) {
                ++n;
                Back();
            }
            return this;
        }
    }
    /// <summary>
    /// Provides utility algorithms for working with iterators.
    /// </summary>
    public static class IteratorUtils {
        /// <summary>
        /// Computes the number of steps between two iterators.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="first">The starting iterator.</param>
        /// <param name="last">The ending iterator.</param>
        /// <returns>The number of steps required to reach <paramref name="last"/>.</returns>
        public static int Distance<T> ( Iterrator<T> first, Iterrator<T> last ) {
            int count = 0;
            var _index = first.Index;

            while ( !first.Equals(last) ) {
                first.Forward();
                count++;
            }
            first.Index = _index;

            return count;
        }


        /// <summary>
        /// Searches for the first occurrence of <paramref name="value"/> in the iterator range.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="xfirst">The starting iterator.</param>
        /// <param name="end">The iterator marking the end of the range.</param>
        /// <param name="value">The value to search for.</param>
        /// <param name="cmp">The comparison strategy.</param>
        /// <returns>The iterator pointing to the found element, or <paramref name="end"/> if not found.</returns>
        public static Iterrator<T> Find<T> ( Iterrator<T> xfirst, Iterrator<T> end, T value, ISimpleCompare<T> cmp )  {

            long indx = xfirst.Index;
            Iterrator<T>? _end = null;

            while ( true ) {
                if ( xfirst.IsEnd ) break;

                if(cmp.Compare(xfirst.Current, value) ) {
                    _end = xfirst;
                }

                xfirst.Forward();

            }
            xfirst.Index = indx;
           
            return end;
        }
        /// <summary>
        /// Finds the first position where <paramref name="value"/> can be inserted
        /// without violating ordering (lower bound).
        /// </summary>
        public static Iterrator<T>? LowerBound<T, TCollection> ( Iterrator<T> first, Iterrator<T> last, T value ) {
            

            return Find<T>(first, last, value, new GreaterEqual<T>() );
        }
        /// <summary>
        /// Finds the first iterator position where <paramref name="value"/> would appear
        /// after all equivalent elements (upper bound).
        /// </summary>
        public static Iterrator<T>? UpperBound<T, TCollection> ( Iterrator<T> first, Iterrator<T> last, T value )  {


            return Find<T>(first, last, value, new Greater<T>());
        }
        /// <summary>
        /// Reverses the elements in the iterator range [first, last).
        /// </summary>
        public static void Reverse<T, TCollection> ( ref BidirectionalIterator<T, TCollection> first, ref BidirectionalIterator<T, TCollection> last ) where TCollection : IContainer<T> {
            last.Back();

            while ( !first.Equals(last) && !first.IsEnd && !last.IsBegin ) {

                Optional<T> f = first.Current; Optional<T> l = last.Current;
                Algorithm.Swap(ref f, ref l);
                first.Current = f; last.Current = l;

                first.Forward();
                last.Back();
            }
        }
        /// <summary>
        /// Rotates the iterator range so that <paramref name="middle"/> becomes the new beginning.
        /// </summary>
        public static void Rotate<T, TCollection> ( ref BidirectionalIterator<T, TCollection> first, ref BidirectionalIterator<T, TCollection> middle, ref BidirectionalIterator<T, TCollection> last ) where TCollection : IContainer<T> {
            Reverse(ref first, ref middle);
            Reverse(ref middle, ref last);
            Reverse(ref first, ref last);
        }

        /// <summary>
        /// Applies an action to each element in the iterator range [first, last).
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="first">The starting iterator.</param>
        /// <param name="last">The ending iterator.</param>
        /// <param name="action">The action to apply to each element.</param>
        public static void ForEach<T> ( Iterrator<T> first, Iterrator<T> last, Action<Optional<T>> action )  {
            while ( !first.Equals(last) ) {
                action(first.Current);
                first.Forward();
            }
        }
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
