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

namespace SystemEx.Collections.Generic.Interfaces {

    /// <summary>
    /// Defines the base functionality for all iterators.
    /// Provides forward-only movement.
    /// </summary>
    public interface IIterator {
        /// <summary>
        /// Moves the iterator one step forward.
        /// </summary>
        void Forward();
    }

    /// <summary>
    /// Extends <see cref="IIterator"/> with cloning support,
    /// allowing iterators to be duplicated without affecting the original.
    /// </summary>
    /// <typeparam name="T">The element type being iterated.</typeparam>
    public interface IIterator<T> : IIterator {
        /// <summary>
        /// Creates a copy of the iterator at its current position.
        /// </summary>
        /// <returns>A new iterator instance positioned identically.</returns>
        IIterator<T> Clone();
    }

    /// <summary>
    /// Represents a forward-only iterator that exposes the current element
    /// and an end-of-range indicator.
    /// </summary>
    /// <typeparam name="T">The element type being iterated.</typeparam>
    public interface IForwardIterator<T> : IIterator<T> {
        /// <summary>
        /// Gets the element at the current iterator position.
        /// </summary>
        T Current { get; }

        /// <summary>
        /// Indicates whether the iterator has reached the end of the sequence.
        /// </summary>
        bool IsEnd { get; }
    }


    /// <summary>
    /// Represents an iterator that can move both forward and backward.
    /// </summary>
    /// <typeparam name="T">The element type being iterated.</typeparam>
    public interface IBidirectionalIterator<T> : IIterator<T> {

        /// <summary>
        /// Gets or sets the element at the current iterator position.
        /// </summary>
        T Current { get; internal set; }

        /// <summary>
        /// Indicates whether the iterator has reached the end of the sequence.
        /// </summary>
        bool IsEnd { get; }

        /// <summary>
        /// Indicates whether the iterator is positioned at the beginning.
        /// </summary>
        bool IsBegin { get; }

        /// <summary>
        /// Moves the iterator one step backward.
        /// </summary>
        void Back();
    }

    /// <summary>
    /// Represents a random-access iterator that supports offset-based movement
    /// in addition to forward and backward stepping.
    /// </summary>
    /// <typeparam name="T">The element type being iterated.</typeparam>
    public interface IRandomAccessIterator<T> : IIterator<T> {

        /// <summary>
        /// Returns a new iterator advanced by the specified offset.
        /// </summary>
        /// <param name="offset">The number of positions to move forward.</param>
        /// <returns>A new iterator positioned at the computed index.</returns>
        IRandomAccessIterator<T> Advance(int offset);

        /// <summary>
        /// Gets the element at the current iterator position.
        /// </summary>
        T Current { get; }

        /// <summary>
        /// Indicates whether the iterator has reached the end of the sequence.
        /// </summary>
        bool IsEnd { get; }

        /// <summary>
        /// Indicates whether the iterator is positioned at the beginning.
        /// </summary>
        bool IsBegin { get; }

        /// <summary>
        /// Moves the iterator one step backward.
        /// </summary>
        void Back();
    }

    /// <summary>
    /// Represents a forward iterator over key/value pairs.
    /// </summary>
    /// <typeparam name="T">The key type.</typeparam>
    /// <typeparam name="TU">The value type.</typeparam>
    public interface IPairForwardIterator<T, TU> : IForwardIterator<Pair<T, TU>> {

        /// <summary>
        /// Gets the key of the current pair.
        /// </summary>
        T? First { get; }

        /// <summary>
        /// Gets the value of the current pair.
        /// </summary>
        TU? Second { get; }
    }

    /// <summary>
    /// Represents an iterator that can be used directly in foreach loops.
    /// Combines <see cref="IEnumerable{T}"/> and <see cref="IEnumerator{T}"/>.
    /// </summary>
    /// <typeparam name="T">The element type being iterated.</typeparam>
    public interface IForeachIterator<T> : IEnumerable<T>, IEnumerator<T> {
    }

    /// <summary>
    /// Provides generic iterator algorithms such as distance calculation,
    /// searching, bounds detection, reversal, rotation, and iteration helpers.
    /// </summary>
    public static class Iterator {
        /// <summary>
        /// Computes the number of steps between two iterators by repeatedly advancing
        /// the first iterator until it equals the second.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="first">The starting iterator.</param>
        /// <param name="last">The ending iterator.</param>
        /// <returns>The number of steps required to reach <paramref name="last"/>.</returns>
        public static int Distance<T>(IIterator<T> first, IIterator<T> last) {
            int count = 0;
            while ( !first.Equals(last) ) {
                first.Forward();
                count++;
            }
            return count;
        }
        /// <summary>
        /// Searches for the first occurrence of a value in a forward iterator range.
        /// </summary>
        public static IForwardIterator<T> Find<T>( IForwardIterator<T> it, IForwardIterator<T> end, T value, CompFunc<T> cmp) {
            while ( !it.Equals(end) ) {
                if ( cmp(it.Current, value) == CompareResult.Equal )
                    return it;

                it.Forward();
            }
            return end;
        }
        /// <summary>
        /// Finds the first iterator position where <paramref name="value"/> could be inserted
        /// without violating ordering (lower bound).
        /// </summary>
        public static IRandomAccessIterator<T> LowerBound<T>( IRandomAccessIterator<T> first, IRandomAccessIterator<T> last, T value, CompFunc<T> cmp) {
            int count = Distance(first.Clone(), last.Clone());
            IRandomAccessIterator<T> it = (IRandomAccessIterator<T>)first.Clone();

            while ( count > 0 ) {
                int step = count / 2;
                IRandomAccessIterator<T> mid = (IRandomAccessIterator<T>)it.Clone();
                mid.Advance(step);

                if ( cmp(mid.Current, value) == CompareResult.AIsSmallerB ) {
                    it = mid;
                    it.Forward();
                    count -= step + 1;
                } else {
                    count = step;
                }
            }

            return it;
        }
        /// <summary>
        /// Finds the first iterator position where <paramref name="value"/> would appear
        /// after all equivalent elements (upper bound).
        /// </summary>
        public static IRandomAccessIterator<T> UpperBound<T>( IRandomAccessIterator<T> first, IRandomAccessIterator<T> last, T value, CompFunc<T> cmp) {
            int count = Distance(first.Clone(), last.Clone());
            var it = first.Clone();

            while ( count > 0 ) {
                int step = count / 2;
                IRandomAccessIterator<T> mid = (IRandomAccessIterator<T>)it.Clone();
                mid.Advance(step);

                if ( cmp(value, mid.Current) != CompareResult.AIsLargerB ) {
                    count = step;
                } else {
                    it = mid;
                    it.Forward();
                    count -= step + 1;
                }
            }

            return (IRandomAccessIterator <T> )it;
        }
        /// <summary>
        /// Reverses the elements in the iterator range [first, last).
        /// </summary>
        public static void Reverse<T>(IBidirectionalIterator<T> first, IBidirectionalIterator<T> last) {
            last.Back(); 

            while ( !first.Equals(last) && !first.IsEnd && !last.IsBegin ) {

                T f = first.Current; T l = last.Current;
                Algorithm.Swap(ref f, ref l);
                first.Current = f;  last.Current = l;

                first.Forward();
                last.Back();
            }
        }
        /// <summary>
        /// Rotates the iterator range so that <paramref name="middle"/> becomes the new beginning.
        /// </summary>
        public static void Rotate<T>(  IBidirectionalIterator<T> first, IBidirectionalIterator<T> middle, IBidirectionalIterator<T> last) {
            Reverse(first, middle);
            Reverse(middle, last);
            Reverse(first, last);
        }
        /// <summary>
        /// Applies an action to each element in the iterator range [first, last).
        /// </summary>
        public static void ForEach<T>(IForwardIterator<T> first, IForwardIterator<T> last, Action<T> action) {
            while ( !first.Equals(last) ) {
                action(first.Current);
                first.Forward();
            }
        }

    }
}
