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
    /// Defines a comparison interface for types that support custom ordering.
    /// Implementations return a <see cref="CompareResult"/> describing the
    /// relationship between two values.
    /// </summary>
    /// <typeparam name="T">
    /// The type being compared. The constraint allows <c>ref struct</c> types,
    /// enabling high‑performance stack‑only comparisons.
    /// </typeparam>
    public interface ICompared<in T> where T : allows ref struct {

        /// <summary>
        /// Compares two values and returns a <see cref="CompareResult"/> indicating
        /// whether <paramref name="x"/> is smaller, larger, or equal to <paramref name="y"/>.
        /// </summary>
        /// <param name="x">The first value to compare.</param>
        /// <param name="y">The second value to compare.</param>
        /// <returns>
        /// A <see cref="CompareResult"/> describing the ordering relationship
        /// between the two values.
        /// </returns>
        CompareResult Compare(T? x, T? y);
    }

}
