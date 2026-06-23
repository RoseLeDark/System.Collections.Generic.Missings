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

namespace SystemEx.Base {
    /// <summary>
    /// Provides a cursor-based stepper over a normalized numeric range.
    /// The stepper moves from Start to End in fixed increments and exposes
    /// forward/backward stepping, reset, and enumeration.
    /// </summary>
    /// <typeparam name="T">Numeric type implementing <see cref="INumber{T}"/>.</typeparam>
    public class NumberRangeStepper<T> : IEnumerable<T>, IEquatable<NumberRangeStepper<T>> where T : INumber<T> {

        /// <summary>Fixed step size applied during iteration.</summary>
        private T m_step;

        /// <summary>Current working range, always normalized (Start ≤ End).</summary>
        private NumberRange<T> m_current;

        /// <summary>Original start value used for reset and backward stepping.</summary>
        private T m_startOld;

        /// <summary>
        /// Initializes a new stepper for the specified range using the given step size.
        /// The input range is normalized to ensure monotonic forward stepping.
        /// </summary>
        /// <param name="orig">Original range to iterate over.</param>
        /// <param name="step">Step size applied to the cursor.</param>
        public NumberRangeStepper(NumberRange<T> orig, T step) {
            m_step = step;
            m_current = orig.Normalize();
            m_startOld = m_current.From;
        }

        /// <summary>
        /// Enumerates all values from Start to End using the configured step size.
        /// </summary>
        /// <returns>Sequence of stepped values.</returns>
        public IEnumerator<T> GetEnumerator() {
            for ( T i = m_current.From; i <= m_current.To; i += m_step ) {
                yield return i;
            }
        }
        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();

        /// <summary>
        /// Advances the cursor by one step. If no further step is possible,
        /// the cursor is clamped to the End value.
        /// </summary>
        /// <returns>The updated cursor position.</returns>
        public T Next() {
            if( HasNext() ) m_current.From += m_step;
            else m_current.From = m_current.To;

            return m_current.From;
        }
        /// <summary>
        /// Advances the cursor by a dynamic step value. If the next position
        /// exceeds the range, the cursor is clamped to the End value.
        /// </summary>
        /// <param name="step">Dynamic step size to apply.</param>
        /// <returns>The updated cursor position.</returns>
        public T Next(T step) {
            if ( HasNext(step) ) m_current.From += step;
            else m_current.From = m_current.To;

            return m_current.From;
        }
        /// <summary>
        /// Moves the cursor backward by a dynamic step value. If the previous
        /// position would fall below the original start value, the cursor is
        /// clamped to the original Start.
        /// </summary>
        /// <param name="step">Dynamic step size to apply.</param>
        /// <returns>The updated cursor position.</returns>
        public T Prev(T step) {
            if ( HasPrev(step) ) m_current.From -= step;
            else m_current.From = m_startOld;
            return m_current.From;
        }

        /// <summary>
        /// Moves the cursor one step backward. If no backward step is possible,
        /// the cursor is clamped to the original start value.
        /// </summary>
        /// <returns>The updated cursor position.</returns>
        public T Prev() {
            if ( HasPrev() ) m_current.From -= m_step;
            else m_current.From = m_startOld;
            return m_current.From;
        }

        /// <summary>
        /// Resets the cursor to the original start value.
        /// </summary>
        /// <returns>The reset cursor position.</returns>
        public T Reset() {
            m_current.From = m_startOld;
            return m_current.From;
        }

        /// <summary>
        /// Determines whether a forward step remains within the range.
        /// </summary>
        public bool HasNext() => 
            ( (m_current.From + m_step) < m_current.To ) ;

        /// <summary>
        /// Determines whether a backward step remains within the range.
        /// </summary>
        public bool HasPrev() => 
            ((m_current.From - m_step) >= m_startOld);

        /// <summary>
        /// Determines whether a forward step of the given size remains within the range.
        /// </summary>
        /// <param name="step">Dynamic step size to test.</param>
        /// <returns>
        /// True if <c>Start + step</c> is still strictly below <c>End</c>;
        /// otherwise false.
        /// </returns>
        public bool HasNext(T step)
            => (m_current.From + step) < m_current.To;

        /// <summary>
        /// Determines whether a backward step of the given size remains within the range.
        /// </summary>
        /// <param name="step">Dynamic step size to test.</param>
        /// <returns>
        /// True if <c>Start - step</c> is still greater than or equal to the
        /// original start value; otherwise false.
        /// </returns>
        public bool HasPrev(T step)
            => (m_current.From - step) >= m_startOld;


        /// <summary>
        /// Checks equality based on range and step size.
        /// The original start value is derived from the normalized range and
        /// therefore does not need to be compared explicitly.
        /// </summary>
        /// <param name="other">Stepper to compare with.</param>
        /// <returns>True if both steppers are equivalent.</returns>
        public bool Equals( NumberRangeStepper<T>? other) {
            if ( other == null ) return false;

            return m_current.Equals(other.m_current) && 
                   m_step.Equals(other.m_step)    ;
        }
        /// <inheritdoc/>
        public override bool Equals(object? obj) {
            return Equals(obj as NumberRangeStepper<T>);
        }

        /// <summary>
        /// Computes a hash code based on the range and step size.
        /// </summary>
        public override int GetHashCode() {
            return HashCode.Combine(m_current, m_current, m_step);
        }
    }
}
