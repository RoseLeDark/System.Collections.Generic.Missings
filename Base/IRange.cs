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

using SystemEx.Collections.Generic.Interfaces;

namespace SystemEx {
    /// \addtogroup STL
    /// @
    /// <summary>
    /// Defines the basic contract for a numeric range with a start and end value.
    /// Provides validation, containment checks, range slicing, merging,
    /// adjacency detection, and intersection logic.
    /// </summary>
    /// <typeparam name="T">Numeric type used for the range boundaries.</typeparam>
    public interface IRange<T> {
        /// <summary>
        /// Returns an iterator positioned at the start of the range.
        /// </summary>
        /// 
        public IForwardIterator<T> Begin { get; }
        /// <summary>
        /// Returns an iterator positioned past the end of the range.
        /// </summary>
        /// 
        public IForwardIterator<T> End { get;  }
        /// <summary>
        /// Gets or sets the start value of the range.
        /// </summary>
        /// 
        public T From { get; set; }
        /// <summary>
        /// Gets or sets the end value of the range.
        /// </summary>
        /// 
        public T To { get; set; }
        /// <summary>
        /// Indicates whether the range is valid (Start ≤ End).
        /// </summary>
        /// 
        bool IsValid { get; }
        /// <summary>
        /// Indicates whether the range represents a single value (Start == End).
        /// </summary>
        /// 
        bool IsSame { get; }
        /// <summary>
        /// Creates a new range using optional override values for start and end.
        /// </summary>
        /// <param name="tstart">Optional new start value.</param>
        /// <param name="tend">Optional new end value.</param>
        /// <returns>A new range with substituted boundaries.</returns>
        /// 
        public IRange<T> GetRange(T? tstart, T? tend);

        /// <summary>
        /// Computes the union of this range with another range if they overlap
        /// or are adjacent. Otherwise returns null.
        /// </summary>
        /// <param name="other">Range to merge with.</param>
        /// <returns>The merged range, or null if no merge is possible.</returns>
        /// 
        public IRange<T>? Union ( IRange<T> other );

        /// <summary>
        /// Determines whether this range touches another range at exactly one boundary.
        /// </summary>
        /// <param name="other">Range to test.</param>
        /// <returns>True if the ranges are adjacent.</returns>
        public bool IsAdjacent ( IRange<T> other );

        /// <summary>
        /// Determines whether the specified value lies within the range.
        /// </summary>
        /// <param name="x">Value to test.</param>
        /// <returns>True if the value is inside the range.</returns>
        public bool Contains(T x);

        /// <summary>
        /// Computes the intersection of this range with another range.
        /// </summary>
        /// <param name="other">Range to intersect with.</param>
        /// <returns>
        /// A new range representing the intersection, or null if no overlap exists.
        /// </returns>
        public IRange<T>? Intersect ( IRange<T> other );

        /// <summary>
        /// Determines whether this range overlaps with another range.
        /// </summary>
        /// <param name="other">Range to test.</param>
        /// <returns>True if the ranges overlap.</returns>
        public bool Overlaps ( IRange<T> other );

    }
    /// @}
}
