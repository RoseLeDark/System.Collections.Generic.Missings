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
	/// \addtogroup Collections
	/// @{

	/// <summary>
	/// A lightweight, serializable three‑element tuple consisting of strongly typed
	/// first, second, and third values. Implements <see cref="ITuple"/> and
	/// <see cref="IEquatable{T}"/> for structural comparison.
	/// </summary>
	/// <typeparam name="TT">The type of the first element.</typeparam>
	/// <typeparam name="TU">The type of the second element.</typeparam>
	/// <typeparam name="TW">The type of the third element.</typeparam>
	[Serializable]
#pragma warning disable CA1067
    public struct Triple<TT, TU, TW> :
        IEquatable<Triple<TT, TU, TW>>, ITuple<TT> where TT : notnull
#pragma warning restore CA1067
    {
        /// <summary>
        /// Backing field for the first element.
        /// </summary>
        private TT m_first;

        /// <summary>
        /// Backing field for the second element.
        /// </summary>
        private TU m_second;

        /// <summary>
        /// Backing field for the third element.
        /// </summary>
        private TW m_third;

        /// <summary>
        /// Gets or sets the first element of the triple.
        /// </summary>
        public TT First {
            get => m_first;
            set => m_first = value;
        }

        /// <summary>
        /// Gets or sets the second element of the triple.
        /// </summary>
        public TU Second {
            get => m_second;
            set => m_second = value;
        }

        /// <summary>
        /// Gets or sets the third element of the triple.
        /// </summary>
        public TW Third {
            get => m_third;
            set => m_third = value;
        }

        /// <summary>
        /// Gets the number of elements in the tuple (always 3).
        /// </summary>
        public readonly int Count => 3;

        /// <summary>
        /// Creates a new triple with the specified values.
        /// </summary>
        public Triple(TT first, TU second, TW third) {
            m_first = first;
            m_second = second;
            m_third = third;
        }

        /// <summary>
        /// Determines whether this triple is equal to another triple by comparing
        /// all three elements.
        /// </summary>
        public readonly bool Equals(Triple<TT, TU, TW> other) {
            return this.EqualFirst(other.First)
                && this.EqualSecond(other.Second)
                && this.EqualThird(other.Third);
        }

        /// <summary>
        /// Returns a string representation of the triple in the form
        /// <c>[first, second, third]</c>.  
        /// Uses a stack‑allocated buffer for performance.
        /// </summary>
        public override string ToString() {
            return string.Create(null, stackalloc char[384],
                $"[{m_first}, {m_second}, {m_third}]");
        }

        /// <summary>
        /// Determines whether the first element equals the specified value.
        /// </summary>
        public readonly bool EqualFirst(TT? other) {
            if ( this.m_first == null )
                throw new ArgumentNullException(nameof(other), "All Triple have a first");
            return this.m_first.Equals(other);
        }

        /// <summary>
        /// Determines whether the second element equals the specified value.
        /// </summary>
        public readonly bool EqualSecond(TU? other) {
            if ( this.m_second == null )
                throw new ArgumentNullException(nameof(other), "All Triple have a second");
            return this.m_second.Equals(other);
        }

        /// <summary>
        /// Determines whether the third element equals the specified value.
        /// </summary>
        public readonly bool EqualThird(TW? other) {
            if ( this.m_third == null )
                throw new ArgumentNullException(nameof(other), "All Triple have a third");
            return this.m_third.Equals(other);
        }

        /// <summary>
        /// Retrieves the element at the specified index.
        /// Index 0 returns the first element, index 1 the second, index 2 the third.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown when the index is not 0, 1, or 2.
        /// </exception>
        public Optional<object> Get (int index) {
            if ( index < 0 || index >= Count )
#pragma warning disable CA2201
                throw new IndexOutOfRangeException("index");
#pragma warning restore CA2201

            return index switch
            {
                0 => m_first,
                1 => m_second,
                _ => m_third
            };
        }

        /// <summary>
        /// Determines whether the first element equals the specified object.
        /// Used for <see cref="ITuple"/> compatibility.
        /// </summary>
        public bool EqualFirst(object key) {
            if ( key is TT typed )
                return EqualFirst(typed);
            return false;
        }
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
