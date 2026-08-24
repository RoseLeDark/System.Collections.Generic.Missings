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
	/// \addtogroup SystemEx.Collections.Generic 
	/// @{
	/// <summary>
	/// A lightweight, serializable four‑element tuple consisting of strongly typed
	/// first, second, third, and fourth values.  
	/// Implements <see cref="ITuple"/> for indexed access and structural comparison.
	/// </summary>
	/// <typeparam name="TT">The type of the first element.</typeparam>
	/// <typeparam name="TU">The type of the second element.</typeparam>
	/// <typeparam name="TW">The type of the third element.</typeparam>
	/// <typeparam name="TJ">The type of the fourth element.</typeparam>
	[Serializable]
    public struct Quad<TT, TU, TW, TJ> : ITuple<TT> where TT : notnull {

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
        /// Backing field for the fourth element.
        /// </summary>
        private TJ m_fourth;

        /// <summary>
        /// Gets or sets the first element of the quad.
        /// </summary>
        public TT First {
            readonly get => m_first;
            set => m_first = value;
        }

        /// <summary>
        /// Gets or sets the second element of the quad.
        /// </summary>
        public TU Second {
            readonly get => m_second;
            set => m_second = value;
        }

        /// <summary>
        /// Gets or sets the third element of the quad.
        /// </summary>
        public TW Third {
            readonly get => m_third;
            set => m_third = value;
        }

        /// <summary>
        /// Gets or sets the fourth element of the quad.
        /// </summary>
        public TJ Fourth {
            readonly get => m_fourth;
            set => m_fourth = value;
        }

        /// <summary>
        /// Gets the number of elements in the tuple (always 4).
        /// </summary>
        public readonly int Count => 4;

        /// <summary>
        /// Creates a new quad with the specified values.
        /// </summary>
        public Quad(TT first, TU second, TW third, TJ fourth) {
            m_first = first;
            m_second = second;
            m_third = third;
            m_fourth = fourth;
        }

        /// <summary>
        /// Determines whether this quad is equal to another quad by comparing
        /// all four elements.
        /// </summary>
        public bool Equals(Quad<TT, TU, TW, TJ> other) {
            return this.EqualFirst(other.First)
                && this.EqualSecond(other.Second)
                && this.EqualThird(other.Third)
                && this.EqualFourth(other.Fourth);
        }

        /// <summary>
        /// Returns a string representation of the quad in the form
        /// <c>[first, second, third, fourth]</c>.  
        /// Uses a stack‑allocated buffer for performance.
        /// </summary>
        public override string ToString() {
            return string.Create(null, stackalloc char[512],
                $"[{m_first}, {m_second}, {m_third}, {m_fourth}]");
        }

        /// <summary>
        /// Determines whether the first element equals the specified value.
        /// </summary>
        public readonly bool EqualFirst(TT? other) {
            if ( this.m_first == null )
                throw new ArgumentNullException(nameof(other), "All Quad have a first");
            return this.m_first.Equals(other);
        }

        /// <summary>
        /// Determines whether the second element equals the specified value.
        /// </summary>
        public readonly bool EqualSecond(TU? other) {
            if ( this.m_second == null )
                throw new ArgumentNullException(nameof(other), "All Quad have a second");
            return this.m_second.Equals(other);
        }

        /// <summary>
        /// Determines whether the third element equals the specified value.
        /// </summary>
        public readonly bool EqualThird(TW? other) {
            if ( this.m_third == null )
                throw new ArgumentNullException(nameof(other), "All Quad have a third");
            return this.m_third.Equals(other);
        }

        /// <summary>
        /// Determines whether the fourth element equals the specified value.
        /// </summary>
        public readonly bool EqualFourth(TJ? other) {
            if ( this.m_fourth == null )
                throw new ArgumentNullException(nameof(other), "All Quad have a fourth");
            return this.m_fourth.Equals(other);
        }

        /// <summary>
        /// Retrieves the element at the specified index.  
        /// Index 0 returns the first element, index 1 the second,  
        /// index 2 the third, index 3 the fourth.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown when the index is not 0–3.
        /// </exception>
        public readonly Optional<object> Get (int index) {
            if ( index < 0 || index >= Count )
#pragma warning disable CA2201
                throw new IndexOutOfRangeException("index");
#pragma warning restore CA2201

            return index switch
            {
                0 => m_first,
                1 => m_second,
                2 => m_third,
                3 => m_fourth,
                _ => null // unreachable, but included for safety
            };
        }


		/// <summary>
		/// Retrieves the element at the specified index.  
		/// Index 0 returns the first element, index 1 the second,  
		/// index 2 the third, index 3 the fourth.
		/// </summary>
		/// <exception cref="IndexOutOfRangeException">
		/// Thrown when the index is not 0–3.
		/// </exception>
		Optional<object> ITuple<TT>.Get ( int index ) {
            return Get(index);
        }
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
