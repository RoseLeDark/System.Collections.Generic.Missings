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
using SystemEx.Collections;
using SystemEx.Collections.Generic;


namespace SystemEx.Base {

	/// <summary>
	/// Provides a cursor-based stepper over a normalized numeric range.
	/// The stepper moves from Start to End in fixed increments and exposes
	/// forward/backward stepping, reset, and enumeration.
	/// </summary>
	public struct NumberRangeStepper : IRange<long>, IEquatable<NumberRangeStepper>, IEnumerable<long> {

        /// <summary>From value of the range.</summary>
        public long From { get; set; }
        /// <summary>End value of the range.</summary>
        public long To { get; set; }

        public long Step { get; set; }

        private Vector<long> m_range;
        /// <summary>
        /// Creates a new numeric range trom T.Zero to T.One
        /// </summary>
        public NumberRangeStepper () {
            this.From = 0;
            this.To = 1;
            this.Step = 1;
        }
        /// <summary>
        /// Creates a new numeric range with the specified start and end values.
        /// </summary>
        /// <param name="start">From value.</param>
        /// <param name="end">To value.</param>
        /// <param name="step">The next step</param>
        public NumberRangeStepper ( long start, long end, long step = 1 ) {
            this.From = start;
            this.To = end;
            Step = step;

            m_range = new Vector<long>(2);

            for ( long i = 0 ; i <= end ; i += step ) {
                m_range.PushBack(i);
            }
        }
        /// <summary>
        /// Gets the length of the range (To - From).
        /// </summary>
        public long Length => this.To - From;
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
        public NumberRangeIterator<long> Begin => new NumberRangeIterator<long>(m_range, 0);
        /// <summary>
        /// Returns an iterator positioned past the end of the range.
        /// </summary>
        public NumberRangeIterator<long> End => new NumberRangeIterator<long>(m_range, m_range.Length - 1);

        /// <inheritdoc/>
        public override int GetHashCode () =>
            HashCode.Combine(this.From, this.To);

        /// <summary>
        /// Determines whether the specified value lies within the range.
        /// Works for both ascending and descending ranges.
        /// </summary>
        /// <param name="x">Value to test.</param>
        /// <returns>True if the value is inside the range.</returns>
        public bool Contains ( long x ) => (x - this.To) * (x - this.From) <= 0;

        /// <summary>
        /// Returns a normalized version of the range where From ≤ To.
        /// </summary>
        /// <returns>A normalized range.</returns>
        public NumberRangeStepper Normalize () =>
            (this.From <= this.To) ? this : new NumberRangeStepper(this.To, this.From, this.Step);

        

        /// <summary>
        /// Creates a new range using optional override values for start and end.
        /// </summary>
        /// <param name="tstart">Optional new start value.</param>
        /// <param name="tend">Optional new end value.</param>
        /// <returns>A new range with substituted boundaries.</returns>
        public IRange<long> GetRange ( long tstart, long tend ) {
            return new NumberRangeStepper(tstart, tend, this.Step);
        }

        /// <summary>
        /// Enumerates all values from From to To in increments of 1.
        /// </summary>
        /// <returns>Sequence of values in the range.</returns>
        public IEnumerator<long> GetEnumerator () {
            return m_range.GetEnumerator();
        }

        /// <summary>
        /// Determines whether this range overlaps with another range.
        /// </summary>
        /// <param name="other">Range to test.</param>
        /// <returns>True if the ranges overlap.</returns>
        public bool Overlaps ( IRange<long> other ) {
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
        public IRange<long>? Intersect ( IRange<long> other ) {
            NumberRangeStepper? _ret = null;

            if ( Overlaps(other) ) {
                _ret = new NumberRangeStepper(long.Max(this.From, other.From),
                                              long.Min(this.To, other.To), this.Step );
            }

            return _ret;
        }

        /// <summary>
        /// Determines whether this range touches another range at exactly one boundary.
        /// </summary>
        /// <param name="other">Range to test.</param>
        /// <returns>True if the ranges are adjacent.</returns>
        public bool IsAdjacent ( IRange<long> other ) {
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
        public IRange<long>? Union ( IRange<long> other ) {
            IRange<long>? result = null;

            if ( Overlaps(other) || IsAdjacent(other) ) {
                long start = long.Min(this.From, other.From);
                long end   = long.Max(this.To, other.To);
                result = new NumberRangeStepper(start, end, this.Step);
            }

            return result;
        }

        /// <summary>
        /// Returns a string representation of the range.
        /// </summary>
        public override string ToString () {
            return (IsValid) ?
                string.Create(null, stackalloc char[256], $"Range: [{this.From} ... $[{this.To}]") :
                string.Create(null, stackalloc char[256], $"Not Valid Range: [{this.From} ... $[{this.To}]");
        }

        public bool Equals ( NumberRangeStepper other ) {
            return (m_range.Equals(other));
        }

        IEnumerator<long> IEnumerable<long>.GetEnumerator () {
            return m_range.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator () {
            return GetEnumerator();
        }
    }
	
}
