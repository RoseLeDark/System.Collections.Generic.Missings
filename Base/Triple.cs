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
using System.Diagnostics.CodeAnalysis;

namespace SystemEx {
    /// <summary>
    /// Represents a three-valued logic type with True, False, and Nin (neither true nor false) states.
    /// </summary>
    public enum triple : sbyte {
        True = 1,
        False = 0,
        Nin = -1
    }
    /// <summary>
    /// Represents a three-valued logic value.
    /// </summary>
    public readonly struct Triple : IEquatable<triple> {
        /// <summary>
        /// Gets the underlying value of the current instance.
        /// </summary>
        private readonly triple m_value; 

        /// <summary>
        /// Gets the value representing true.
        /// </summary>
        internal const triple True = triple.True;

        /// <summary>
        /// Gets the value representing false.
        /// </summary>
        internal const triple False = triple.False;

        /// <summary>
        /// Gets the value representing neither true nor false.
        /// </summary>
        internal const triple Nin = triple.Nin;

        /// <summary>
        /// Gets the string representation for the true value.
        /// </summary>
        public static readonly string TrueString = "True";
        /// <summary>
        /// Gets the string representation for the false value.
        /// </summary>
        public static readonly string FalseString = "False";
        /// <summary>
        /// Gets the string representation for the neither true nor false value.
        /// </summary>
        public static readonly string NinString = "Nin";

        /// <summary>
        /// Gets the hash code for the current instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode () {
            if ( m_value == True ) return 1;
            if ( m_value == False ) return 0;
            return -1;
        }

        /// <summary>
        /// Returns a string that represents the current instance.
        /// </summary>
        /// <returns>A string that represents the current instance.</returns>
        public override string ToString () {
            string _ret = NinString;

            if ( m_value == triple.False) {
                _ret = FalseString;
            } else if ( m_value == triple.True )
                _ret = TrueString;

            return _ret;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Triple"/> struct.
        /// </summary>
        public Triple() {
            m_value = triple.Nin;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Triple"/> struct.
        /// </summary>
        /// <param name="v">The boolean value to initialize with.</param>
        public Triple(bool v) {
            m_value = v ? triple.True : triple.False;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Triple"/> struct.
        /// </summary>
        /// <param name="v">The triple value to initialize with.</param>
        public Triple ( triple v ) {
            m_value = v;
        }

        /// <summary>
        /// Determines whether the current instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>true if the current instance and <paramref name="obj"/> are equal; otherwise, false.</returns>
        public override bool Equals ( [NotNullWhen(true)] object? obj ) {
            bool _ret = false;

            if( (obj is Boolean) )       _ret = ((Boolean)obj == this.ToBoolean());
            else if ( (obj is Triple) )  _ret = m_value == ((Triple)obj).m_value;

            return _ret;
        }


        /// <summary>
        /// Determines whether the current instance and a specified boolean value are equal.
        /// </summary>
        /// <param name="obj">The boolean value to compare with the current instance.</param>
        /// <returns>true if the current instance and <paramref name="obj"/> are equal; otherwise, false.</returns>
        public bool Equals ( bool obj ) {
            return ToBoolean() == obj;
        }
        /// <summary>
        /// Returns the boolean value of the current instance.
        /// </summary>
        /// <returns>The boolean value of the current instance.</returns>
        public bool ToBoolean () {
            return m_value == triple.True;
        }
        /// <summary>
        /// Determines whether the current instance and a specified triple value are equal.
        /// </summary>
        /// <param name="obj">The triple value to compare with the current instance.</param>
        /// <returns>true if the current instance and <paramref name="obj"/> are equal; otherwise, false.</returns>
        public bool Equals ( triple obj ) {
            return m_value == obj;
        }
        /// <summary>
        /// Determines whether two instances of the <see cref="Triple"/> struct are equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the instances are equal; otherwise, false.</returns>
        public static bool operator == ( Triple left, Triple right ) {
            return left.Equals(right);
        }

        /// <summary>
        ///     Determines whether an instance of the <see cref="Triple"/> struct and a boolean value are equal.
        /// </summary>
        /// <param name="left">The instance of the <see cref="Triple"/> struct to compare.</param>
        /// <param name="right">The boolean value to compare with the instance.</param>
        /// <returns>true if the instance and the boolean value are equal; otherwise, false.</returns>
        public static bool operator == ( Triple left, bool right ) {
            return left.Equals(right);
        }

        /// <summary>
        ///     Determines whether a boolean value and an instance of the <see cref="Triple"/> struct are equal.
        /// </summary>
        /// <param name="left">The boolean value to compare.</param>
        /// <param name="right">The instance of the <see cref="Triple"/> struct to compare with the boolean value.</param>
        /// <returns>true if the boolean value and the instance are equal; otherwise, false.</returns>
        public static bool operator == ( bool left, Triple right ) {
            return right.Equals(left);
        }
        /// <summary>
        ///     Determines whether two instances of the <see cref="Triple"/> struct are not equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the instances are not equal; otherwise, false.</returns>
        public static bool operator != ( Triple left, Triple right ) {
            return !(left.Equals(right));
        }
        /// <summary>
        ///     Determines whether an instance of the <see cref="Triple"/> struct and a boolean value are not equal.
        /// </summary>
        /// <param name="left">The instance of the <see cref="Triple"/> struct to compare.</param>
        /// <param name="right">The boolean value to compare with the instance.</param>
        /// <returns>true if the instance and the boolean value are not equal; otherwise, false.</returns>
        public static bool operator != ( Triple left, bool right ) {
            return !(left.Equals(right));
        }
        /// <summary>
        ///     Determines whether a boolean value and an instance of the <see cref="Triple"/> struct are not equal.
        /// </summary>
        /// <param name="left">The boolean value to compare.</param>
        /// <param name="right">The instance of the <see cref="Triple"/> struct to compare with the boolean value.</param>
        /// <returns>true if the boolean value and the instance are not equal; otherwise, false.</returns>
        public static bool operator != ( bool left, Triple right ) {
            return !(right.Equals(left));
        }
    }
}
