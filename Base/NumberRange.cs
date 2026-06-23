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
using System.Numerics;
using SystemEx.Collections.Generic.Interfaces;


namespace SystemEx {
    /// <summary>
    /// Represents a numeric range defined by a start and end value.
    /// Provides range validation, containment checks, normalization,
    /// intersection, adjacency, union, and enumeration.
    /// </summary>
    /// <typeparam name="T">Numeric type implementing <see cref="INumber{T}"/>.</typeparam>
    public class NumberRange<T> : IRange<T>, IEquatable<NumberRange<T>>, IEnumerable<T>
        where T : INumber<T> {
        /// <summary>From value of the range.</summary>
        public T From { get; set; }
        /// <summary>End value of the range.</summary>
        public T To { get; set; }

        /// <summary>
        /// Creates a new numeric range trom T.Zero to T.One
        /// </summary>
        public NumberRange () {
            this.From = T.Zero;
            this.To = T.One;
        }
        /// <summary>
        /// Creates a new numeric range with the specified start and end values.
        /// </summary>
        /// <param name="start">From value.</param>
        /// <param name="end">To value.</param>
        public NumberRange (T start, T end) {
            this.From = start;
            this.To = end;
        }
        /// <summary>
        /// Gets the length of the range (To - From).
        /// </summary>
        public T Length => this.To - From;
        /// <summary>
        /// Indicates whether the range is valid (From ≤ To).
        /// </summary>
        public bool IsValid => this.From <= this.To;
        /// <summary>
        /// Indicates whether the range represents a single value (From == To).
        /// </summary>
        public bool IsSame => this.From == this.To;
        /// <summary>
        /// Returns an iterator positioned at the start of the range.
        /// </summary>
        public IForwardIterator<T> Begin => new NumberRangeIterator<T>(this.From, this.To);
        /// <summary>
        /// Returns an iterator positioned past the end of the range.
        /// </summary>
        public IForwardIterator<T> End => new NumberRangeIterator<T>(this.To + T.One, this.To);

        /// <inheritdoc/>
        public override int GetHashCode() => 
            HashCode.Combine(this.From, this.To);

        /// <inheritdoc/>
        public override bool Equals(object? obj) =>
            Equals(obj as NumberRange<T>);

        /// <summary>
        /// Determines equality based on start and end values.
        /// </summary>
        /// <param name="other">Range to compare with.</param>
        /// <returns>True if both ranges are equal.</returns>
        public bool Equals(NumberRange<T>? other) =>
            other is not null &&
            From.Equals(other.From) &&
            this.To.Equals(other.To);

        /// <summary>
        /// Determines whether the specified value lies within the range.
        /// Works for both ascending and descending ranges.
        /// </summary>
        /// <param name="x">Value to test.</param>
        /// <returns>True if the value is inside the range.</returns>
        public bool Contains(T x) => (x - this.To) * (x - this.From) <= T.Zero;

        /// <summary>
        /// Returns a normalized version of the range where From ≤ To.
        /// </summary>
        /// <returns>A normalized range.</returns>
        public NumberRange<T> Normalize() =>
            (this.From <= this.To) ? this : new NumberRange<T>(this.To, this.From);

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => 
            GetEnumerator();

        /// <summary>
        /// Creates a new range using optional override values for start and end.
        /// </summary>
        /// <param name="tstart">Optional new start value.</param>
        /// <param name="tend">Optional new end value.</param>
        /// <returns>A new range with substituted boundaries.</returns>
        public IRange<T> GetRange(T? tstart, T? tend) {
            return new NumberRange<T>((tstart != null) ? tstart : this.From,
                                      (tend != null) ? tend : this.To);
        }

        /// <summary>
        /// Enumerates all values from From to To in increments of 1.
        /// </summary>
        /// <returns>Sequence of values in the range.</returns>
        public IEnumerator<T> GetEnumerator() {
            for ( T i = From; i <= this.To; i += T.One )
                yield return i;
        }

        /// <summary>
        /// Determines whether this range overlaps with another range.
        /// </summary>
        /// <param name="other">Range to test.</param>
        /// <returns>True if the ranges overlap.</returns>
        public bool Overlaps(IRange<T> other) {
            bool result = false;

            if ( IsValid && other.IsValid )
                result = !(other.To < From || other.From > this.To);

            return result;
        }

        /// <summary>
        /// Computes the intersection of this range with another range.
        /// </summary>
        /// <param name="other">Range to intersect with.</param>
        /// <returns>
        /// A new range representing the intersection, or null if no overlap exists.
        /// </returns>
        public IRange<T>? Intersect(IRange<T> other) {
            NumberRange<T>? _ret = null;

            if ( Overlaps(other) ) {
                _ret = new NumberRange<T>(T.Max(this.From, other.From),
                                          T.Min(this.To, other.To));
            }

            return _ret;
        }

        /// <summary>
        /// Determines whether this range touches another range at exactly one boundary.
        /// </summary>
        /// <param name="other">Range to test.</param>
        /// <returns>True if the ranges are adjacent.</returns>
        public bool IsAdjacent(IRange<T> other) {
            bool result = false;

            if ( IsValid && other.IsValid )
                result =
                    this.To == other.From ||
                    other.To == this.From;

            return result;
        }

        /// <summary>
        /// Computes the union of this range with another range if they overlap
        /// or are adjacent. Otherwise returns null.
        /// </summary>
        /// <param name="other">Range to merge with.</param>
        /// <returns>The merged range, or null if no merge is possible.</returns>
        public IRange<T>? Union( IRange<T> other) {
            IRange<T>? result = null;

            if ( Overlaps(other) || IsAdjacent(other) ) {
                T start = T.Min(this.From, other.From);
                T end   = T.Max(this.To, other.To);
                result = new NumberRange<T>(start, end);
            }

            return result;
        }

        /// <summary>
        /// Returns a string representation of the range.
        /// </summary>
        public override string ToString() {
            return (IsValid) ?
                string.Create(null, stackalloc char[256], $"Range: [{this.From} ... $[{this.To}]") :
                string.Create(null, stackalloc char[256], $"Not Valid Range: [{this.From} ... $[{this.To}]");
        }
    }
}

