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

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using SystemEx.Collections.Generic.Interfaces;

namespace SystemEx {
    /// <summary>
    /// Forward iterator over a normalized numeric range. Supports stepping
    /// by one unit and exposes the current value and end-of-range state.
    /// </summary>
    /// <typeparam name="T">Numeric type implementing <see cref="INumber{T}"/>.</typeparam>
    public struct NumberRangeIterator<T> : IForwardIterator<T>
        where T : INumber<T> {
        /// <summary>Current iterator position.</summary>
        private T m_current;

        /// <summary>End boundary of the range (inclusive).</summary>
        private readonly T m_end;

        /// <summary>
        /// Initializes a new iterator positioned at <paramref name="start"/>
        /// and iterating until <paramref name="end"/>.
        /// </summary>
        /// <param name="start">Initial iterator position.</param>
        /// <param name="end">End boundary (inclusive).</param>
        public NumberRangeIterator ( T start, T end ) {
            m_current = start;
            m_end = end;
        }

        /// <summary>
        /// Gets the element at the current iterator position.
        /// </summary>
        public T Current => m_current;

        /// <summary>
        /// Indicates whether the iterator has reached the end boundary.
        /// </summary>
        public bool IsEnd => m_current > m_end;

        /// <summary>
        /// Moves the iterator one step forward.
        /// </summary>
        public void Forward () {
            if ( !IsEnd )
                m_current += T.One;
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
        /// Creates a copy of this iterator at its current position.
        /// </summary>
        /// <returns>A new iterator instance positioned identically.</returns>
        public IIterator<T> Clone ()
            => new NumberRangeIterator<T>(m_current, m_end);

        /// <summary>
        /// Compares two iterators for equality based on their current position
        /// and end boundary.
        /// </summary>
        public override bool Equals ( object? obj )
            => obj is NumberRangeIterator<T> it &&
               it.m_current.Equals(m_current) &&
               it.m_end.Equals(m_end);

        /// <summary>
        /// Computes a hash code for this iterator.
        /// </summary>
        public override int GetHashCode ()
            => HashCode.Combine(m_current, m_end);

        /// <summary>
        /// Compares two iterators for equality.
        /// Two iterators are equal if both their current position
        /// and end boundary match.
        /// </summary>
        /// <param name="left">Left iterator.</param>
        /// <param name="right">Right iterator.</param>
        /// <returns>True if both iterators are equal.</returns>
        public static bool operator == ( NumberRangeIterator<T> left, NumberRangeIterator<T> right ) {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two iterators for inequality.
        /// </summary>
        /// <param name="left">Left iterator.</param>
        /// <param name="right">Right iterator.</param>
        /// <returns>True if the iterators differ.</returns>
        public static bool operator != ( NumberRangeIterator<T> left, NumberRangeIterator<T> right ) {
            return !left.Equals(right);
        }
    }
}
