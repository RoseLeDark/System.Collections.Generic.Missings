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
	/// \addtogroup Collections
	/// @{

	/// <summary>
	/// A random‑access iterator for <see cref="List{T}"/> that also implements
	/// <see cref="Iterrator{T}"/> to support foreach‑style enumeration.
	/// Provides forward, backward, and offset‑based movement.
	/// </summary>
	/// <typeparam name="T">The element type stored in the list.</typeparam>
	public struct ListIterator<T> : Iterrator<T>, IEnumerator {
        /// <summary>
        /// The underlying list being iterated over.
        /// </summary>
        private List<T> m_list;
        /// <summary>
        /// The current index within the list.
        /// </summary>
        private int m_index;


        /// <summary>
        /// Creates a new iterator for the specified list at the given position.
        /// </summary>
        /// <param name="list">The list to iterate over.</param>
        /// <param name="index">The initial iterator position.</param>  
        public ListIterator(List<T> list, int index) {
            m_list = list;
            m_index = index;
        }
        /// <summary>
        /// Gets the element at the current iterator position.
        /// </summary>
        public Optional<T> Current => m_list[m_index];
        /// <summary>
        /// Indicates whether the iterator has reached the end of the list.
        /// </summary>
        public bool IsEnd => m_index >= m_list.Count;
        
   
        /// <summary>
        /// Indicates whether the iterator is positioned at the beginning.
        /// </summary>
        public bool IsBegin => m_index == 0;

        object IEnumerator.Current => Current!;

        /// <summary>
        /// Gets the current index within the list.
        /// </summary>
        public long Index { 
            get => this.Index; 
            set => throw new NotImplementedException(); 
        }

        /// <summary>
        /// Moves the iterator one step forward unless it is already at the end.
        /// </summary>
        public void Forward() {
            if ( !IsEnd ) m_index++;
        }
        /// <summary>
        /// Moves the iterator N step forward
        /// </summary>
        public void Forward ( long i ) {
            var n = i;
            while ( n > 0 ) {
                --n;
                Forward();
            }
        }
        /// <summary>
        /// Moves the iterator one step backward unless it is already at the beginning.
        /// </summary>
        public void Back() {
            if ( m_index > 0 )
                m_index--;
        }
        /// <summary>
        /// Advances the iterator by the specified offset and returns itself.
        /// </summary>
        /// <param name="offset">The number of positions to move.</param>
        /// <returns>The same iterator instance after movement.</returns>
        public ListIterator<T> Advance( long offset ) { m_index += (int)offset; return this; }

        /// <summary>
        /// Determines whether this iterator is equal to another iterator.
        /// </summary>
        /// <param name="other">The iterator to compare with.</param>
        /// <returns><c>true</c> if both iterators reference equal lists and positions.</returns>
        public bool Equals(ListIterator<T> other) {
            return m_list.SequenceEqual( other.m_list) && m_index == other.m_index;
        }
        /// <inheritdoc/>
        public override bool Equals(object? obj) {
            if ( obj is ListIterator<T> ) {
                return Equals((ListIterator<T>)obj);
            }
            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode() {
            unchecked {
                int h = m_list.GetHashCode();
                h = (h * 397) ^ m_index;
                return h;
            }
        }
        /// <summary>
        /// Creates a deep clone of the iterator, including a copy of the underlying list.
        /// </summary>
        /// <returns>A new iterator instance with its own list copy.</returns>
        public ListIterator<T> Clone () {
            return new ListIterator<T>(m_list, m_index);
        }
        /// <summary>
        /// Returns this iterator as an enumerator.
        /// </summary>
        public IEnumerator<T> GetEnumerator() => m_list.GetEnumerator();

        /// <summary>
        /// Moves to the next element for foreach enumeration.
        /// </summary>
        /// <returns><c>true</c> if the iterator advanced; otherwise <c>false</c>.</returns>
        public bool MoveNext() {
            if ( !IsEnd ) { m_index++; return true; }
            return false;
        }
        /// <summary>
        /// Reset is not supported for this iterator.
        /// </summary>
        public void Reset() { }

        /// <summary>
        /// Equality operator for comparing two iterators.
        /// </summary>
        public static bool operator ==(ListIterator<T>? a, ListIterator<T>? b) {
            if ( ReferenceEquals(a, b) ) return true;
            if ( a is null || b is null ) return false;
            return a.Equals(b);
        }
        /// <summary>
        /// Inequality operator for comparing two iterators.
        /// </summary>
        public static bool operator !=(ListIterator<T>? a, ListIterator<T>? b) {
            return !(a == b);
        }
    }


    /// <summary>
    /// Provides extension methods for creating iterators from <see cref="List{T}"/>.
    /// </summary>
    public static class ListIteratorExtensions {

        /// <summary>
        /// Returns an iterator positioned at the beginning of the list.
        /// </summary>
        public static ListIterator<T> First<T>(this List<T> list)
            => new ListIterator<T>(list, 0);

        /// <summary>
        /// Returns an iterator positioned at the specified index.
        /// </summary>
        public static ListIterator<T> At<T>(this List<T> list, int index)
            => new ListIterator<T>(list, index);

        /// <summary>
        /// Returns an iterator positioned at the end of the list.
        /// </summary>
        public static ListIterator<T> End<T>(this List<T> list)
            => new ListIterator<T>(list, list.Count);
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
