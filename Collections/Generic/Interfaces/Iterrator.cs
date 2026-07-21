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
    /// \addtogroup collections
    /// @{
    /// \addtogroup interfaces
    /// @{
    /// <summary>
    /// Defines the base functionality for all iterators.
    /// Provides forward-only movement.
    /// </summary>
    public interface IIterator {
        /// <summary>
        /// Moves the iterator one step forward.
        /// </summary>
        void Forward();
        /// <summary>
        /// Moves the iterator N step forward.
        /// </summary>
        /// <param name="i">N</param>
        void Forward ( long i );
    }

    /// <summary>
    /// Extends <see cref="IIterator"/> with cloning support,
    /// allowing iterators to be duplicated without affecting the original.
    /// </summary>
    /// <typeparam name="T">The element type being iterated.</typeparam>
    public interface IIterator<T> : IIterator where T : allows ref struct {
        /// <summary>
        /// Creates a copy of the iterator at its current position.
        /// </summary>
        /// <returns>A new iterator instance positioned identically.</returns>
        IIterator<T>? Clone();
    }

    /// <summary>
    /// Represents a forward-only iterator that exposes the current element
    /// and an end-of-range indicator.
    /// </summary>
    /// <typeparam name="T">The element type being iterated.</typeparam>
    public interface IForwardIterator<T> : IIterator<T> where T : allows ref struct  {
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
    public interface IBidirectionalIterator<T> : IIterator<T> where T : allows ref struct {

        /// <summary>
        /// Gets or sets the element at the current iterator position.
        /// </summary>
        T Current { get; set; }

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
    public interface IRandomAccessIterator<T> : IIterator<T> where T : allows ref struct {

        /// <summary>
        /// Returns a new iterator advanced by the specified offset.
        /// </summary>
        /// <param name="offset">The number of positions to move forward.</param>
        /// <returns>A new iterator positioned at the computed index.</returns>
        IRandomAccessIterator<T> Advance( long offset );

        /// <summary>
        /// Gets the element at the current iterator position.
        /// </summary>
        T Current { get; set; }

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
    public interface IPairForwardIterator<T, TU> : IForwardIterator<Pair<T, TU>>  {

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
    public interface IForeachIterator<T> : IEnumerable<T>, IEnumerator<T> where T : allows ref struct {
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
        /// Advances a forward iterator by <paramref name="n"/> steps by repeatedly
        /// calling <see cref="IIterator{T}.Forward"/>.
        ///
        /// The iterator is cloned before advancing.  
        /// 
        /// In C# iterators are reference‑based objects; advancing the original
        /// iterator would mutate the caller's iterator state.  
        /// 
        /// Cloning ensures that <c>Advance</c> behaves like the C++ STL version:
        /// it returns a new iterator positioned <c>n</c> steps ahead, while the
        /// original iterator remains unchanged.
        /// </summary>
        public static IForwardIterator<T> Advance<T> ( IForwardIterator<T> first, long n ) {
            IForwardIterator<T> it = (IForwardIterator<T>)first.Clone();

            while ( n > 0 ) {
                --n;
                it.Forward();
            }

            return it;
        }
        /// <summary>
        /// Advances a forward iterator by <paramref name="n"/> steps by repeatedly
        /// calling <see cref="IIterator{T}.Forward()"/>.
        ///
        /// The iterator is cloned before advancing.  
        /// 
        /// In C# iterators are reference‑based objects; advancing the original
        /// iterator would mutate the caller's iterator state.  
        /// 
        /// Cloning ensures that <c>Advance</c> behaves like the C++ STL version:
        /// it returns a new iterator positioned <c>n</c> steps ahead, while the
        /// original iterator remains unchanged.
        /// </summary>
        public static IForwardIterator<T> Next<T> ( IForwardIterator<T> first, long n ) => Advance<T>(first, n);

        /// <summary>
        /// Advances a random‑access iterator by <paramref name="n"/> steps using
        /// <see cref="IIterator{T}.Forward"/>.
        ///
        /// The iterator is cloned before advancing.  
        /// 
        /// In C# iterators are objects, not value‑types.  
        /// Without cloning, advancing would mutate the caller's iterator, breaking
        /// STL‑style semantics and making algorithms like <c>Distance</c>,
        /// <c>LowerBound</c>, <c>UpperBound</c> or hashing routines unsafe.
        /// 
        /// Cloning preserves the expected C++ behavior:  
        /// <c>Advance</c> returns a new iterator at <c>first + n</c>, leaving
        /// <c>first</c> untouched.
        /// </summary>
        public static IRandomAccessIterator<T> Advance<T> ( IRandomAccessIterator<T> first, long n ) {
            IRandomAccessIterator<T> it = (IRandomAccessIterator<T>)first.Clone();

            while ( n > 0 ) {
                --n;
                it.Forward();
            }
            while ( n < 0 ) {
                ++n;
                it.Back();
            }
            return it;
        }
        /// <summary>
        /// Returns a new iterator advanced by <paramref name="n"/> steps from <paramref name="itt"/>.
        /// 
        /// This is an STL‑style helper that delegates to <see cref="Advance{T}(IRandomAccessIterator{T}, long)"/>.
        /// The original iterator remains unchanged; the returned iterator represents <c>first + n</c>.
        /// </summary>
        public static IRandomAccessIterator<T> Next<T> ( IRandomAccessIterator<T> itt, long n )
            => Advance<T>(itt, n);

        /// <summary>
        /// Returns a new iterator moved <paramref name="n"/> positions backward from
        /// <paramref name="itt"/>.
        /// 
        /// This is the STL‑style equivalent of <c>std::prev</c>.  
        /// Internally delegates to <see cref="Advance{T}(IRandomAccessIterator{T}, long)"/>
        /// with a negative offset.  
        /// 
        /// Because <c>Advance</c> supports both forward and backward stepping,
        /// this method performs actual backward movement using <see cref="IRandomAccessIterator{T}.Back"/>.
        /// 
        /// The original iterator remains unchanged.
        /// </summary>
        public static IRandomAccessIterator<T> Prev<T> ( IRandomAccessIterator<T> itt, long n )
            => Advance<T>(itt, -n);

        /// <summary>
        /// Determines whether the iterator range [<paramref name="first"/>, <paramref name="last"/>)
        /// is empty.
        /// 
        /// The range is considered empty if <paramref name="first"/> and <paramref name="last"/>
        /// refer to the same position (i.e. <c>first.Equals(last)</c>).
        /// </summary>
        public static bool Empty<T>(IIterator<T> first, IIterator<T> last) {
            return first.Equals(last);
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
 #pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
