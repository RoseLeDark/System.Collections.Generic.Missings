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
namespace SystemEx {
    /// <summary>
    /// Represents a timeout value that can be used to specify a duration for operations that may block or wait.
    /// </summary>
    public struct TimeOut : IEquatable<TimeOut> {
        private TimeSpan m_timeSpan;
        /// <summary>
        /// Represents an infinite timeout, indicating that an operation should wait indefinitely.
        /// </summary>
        public static readonly TimeOut Infinite = new TimeOut(-1);

        public static readonly TimeOut Zero = new TimeOut(0);
        
        public bool IsInfinite => m_timeSpan.TotalMilliseconds < 0;

        public int Milliseconds => (int)m_timeSpan.Milliseconds;

        /// <summary>
        /// Creates a <see cref="TimeOut"/> instance from a specified  <see cref="TimeSpan"/>.
        /// </summary>
        public TimeOut ( TimeSpan timeSpan ) {
            m_timeSpan = timeSpan;
        }
        /// <summary>
        /// Creates a <see cref="TimeOut"/> instance from a specified number of milliseconds.
        /// A negative value indicates an infinite timeout.
        /// </summary>
        public TimeOut ( int ms ) {
            m_timeSpan = new TimeSpan(0, 0, 0, 0, ms);
        }
        /// <summary>
        /// Get the underlying <see cref="TimeSpan"/> value of the timeout.
        /// </summary>
        public TimeSpan ToTimeSpan () => m_timeSpan;

        /// <summary>
        /// Implicitly converts a <see cref="TimeSpan"/> or an integer (milliseconds) to a <see cref="TimeOut"/> instance.
        /// </summary>
        public static implicit operator TimeOut ( TimeSpan timeSpan ) => new TimeOut(timeSpan);

        /// <summary>
        /// Implicitly converts an integer (milliseconds) to a <see cref="TimeOut"/> instance.
        /// </summary>
        public static implicit operator TimeOut ( int ms ) => new TimeOut(ms);

        /// <summary>
        /// Implicitly converts a <see cref="TimeOut"/> instance to a <see cref="TimeSpan"/>.
        /// </summary>
        public static implicit operator TimeSpan ( TimeOut timeout ) => timeout.m_timeSpan;

        /// <summary>
        /// Compares two <see cref="TimeOut"/> instances for equality.
        /// </summary>
        public static bool operator == ( TimeOut a, TimeOut b ) => a.Equals(b);

        /// <summary>
        /// Compares two <see cref="TimeOut"/> instances for inequality.
        /// </summary>
        public static bool operator != ( TimeOut a, TimeOut b ) => !(a == b);

        /// <summary>
        /// Standard override for equality comparison.
        /// </summary>
        public override bool Equals ( object? obj ) {
            if ( obj is TimeOut other )
                return this == other;
            return false;
        }
        /// <summary>
        /// Compares two <see cref="TimeOut"/> instances for equality.
        /// </summary>
        public bool Equals ( TimeOut other ) {
            return this.m_timeSpan == other.m_timeSpan;
        }

        /// <summary>
        /// Returns a hash code for the <see cref="TimeOut"/> instance.
        /// </summary>
        public override int GetHashCode () => m_timeSpan.GetHashCode();
    }

}