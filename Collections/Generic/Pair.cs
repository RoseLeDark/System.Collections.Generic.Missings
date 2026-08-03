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

namespace SystemEx.Collections.Generic {
    /// \addtogroup collections
    /// @{
    /// <summary>
    /// A lightweight, serializable two‑element tuple consisting of a strongly typed
    /// key and value. Implements <see cref="IPair{T, TU}"/> and provides typed
    /// comparison helpers as well as <see cref="ITuple"/> compatibility.
    /// </summary>
    /// <typeparam name="T">The type of the first element (key).</typeparam>
    /// <typeparam name="TU">The type of the second element (value).</typeparam>
    [Serializable]
    public struct Pair<T, TU> : IPair<T, TU>, IComparable<Pair<T, TU> > where T : notnull {

        /// <summary>
        /// Backing field for the first element (key).
        /// </summary>
        private T m_key;

        /// <summary>
        /// Backing field for the second element (value).
        /// </summary>
        private TU m_value;

        /// <summary>
        /// Gets or sets the second element of the pair.
        /// </summary>
        public TU Second {
            get => m_value;
            set => m_value = value;
        }

        /// <summary>
        /// Gets the number of elements in the tuple (always 2).
        /// </summary>
        public readonly int Count => 2;
        /// <summary>
        /// 
        /// </summary>
        public T First { get => m_key; set => m_key = value; }

        /// <summary>
        /// Creates a new pair with the specified key and value.
        /// </summary>
        /// <param name="first">The first element (key).</param>
        /// <param name="second">The second element (value).</param>
        public Pair(T first, TU second) {
            m_key = first;
            m_value = second;
        }

        /// <summary>
        /// Determines whether this pair is equal to another pair by comparing
        /// both the first and second elements.
        /// </summary>
        /// <param name="other">The pair to compare with.</param>
        /// <returns><c>true</c> if both elements match; otherwise <c>false</c>.</returns>
        public bool Equals(Pair<T, TU> other) {
            return this.EqualFirst(other.First) && this.EqualSecond(other.Second);
        }

        /// <summary>
        /// Returns a string representation of the pair in the form <c>[key, value]</c>.
        /// Uses a stack‑allocated buffer for performance.
        /// </summary>
        public override string ToString() {
            return string.Create(null, stackalloc char[256], $"[{m_key}, {m_value}]");
        }

        /// <summary>
        /// Determines whether the first element equals the specified value.
        /// </summary>
        /// <param name="other">The value to compare against the first element.</param>
        /// <returns><c>true</c> if equal; otherwise <c>false</c>.</returns>
        public readonly bool EqualFirst(T other) => this.m_key!.Equals(other);

        /// <summary>
        /// Determines whether the second element equals the specified value.
        /// </summary>
        /// <param name="other">The value to compare against the second element.</param>
        /// <returns><c>true</c> if equal; otherwise <c>false</c>.</returns>
        public readonly bool EqualSecond(TU other) => this.m_value!.Equals(other);

        /// <summary>
        /// Retrieves the element at the specified index.
        /// Index 0 returns the key, index 1 returns the value.
        /// </summary>
        /// <param name="index">The index of the element (0 or 1).</param>
        /// <returns>The element at the given index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the index is not 0 or 1.
        /// </exception>
        public readonly Optional<object> Get (int index) {
            if ( index < 0 || index >= Count )
                throw new ArgumentOutOfRangeException(nameof(index));

            return index == 0 ? m_key : m_value;
        }

        public int CompareTo ( Pair<T, TU> other ) {
            return Comparer<T>.Default.Compare(First, other.First);
        }
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
