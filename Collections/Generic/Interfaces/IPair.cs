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
namespace SystemEx.Collections.Generic.Interfaces {
    /// \addtogroup collections
    /// @{
    /// <summary>
    /// Represents a two‑element tuple consisting of a key and a value.
    /// Extends <see cref="ITuple"/> with strongly typed accessors and
    /// comparison helpers for the first and second elements.
    /// </summary>
    /// <typeparam name="T">The type of the first element (key).</typeparam>
    /// <typeparam name="TU">The type of the second element (value).</typeparam>
    public interface IPair<T, TU> : ITuple {

        /// <summary>
        /// Gets or sets the first element of the pair.
        /// </summary>
        T First { get; set; }

        /// <summary>
        /// Gets or sets the second element of the pair.
        /// </summary>
        TU Second { get; set; }

        /// <summary>
        /// Determines whether the first element equals the specified value.
        /// </summary>
        /// <param name="other">The value to compare against the first element.</param>
        /// <returns>
        /// <c>true</c> if the first element equals <paramref name="other"/>; 
        /// otherwise <c>false</c>.
        /// </returns>
        bool EqualFirst(T other);

        /// <summary>
        /// Determines whether the second element equals the specified value.
        /// </summary>
        /// <param name="other">The value to compare against the second element.</param>
        /// <returns>
        /// <c>true</c> if the second element equals <paramref name="other"/>; 
        /// otherwise <c>false</c>.
        /// </returns>
        bool EqualSecond(TU other);
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
