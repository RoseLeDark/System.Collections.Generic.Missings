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

using System.Runtime.InteropServices;
using SystemEx.Collections.Generic;
using SystemEx.Hash;
using SystemEx.Utils;

namespace SystemEx.Numeric {
	/// \addtogroup Numeric
	/// @{

	/// <summary>
	/// Represents a 2‑component doubleing‑point vector.
	///
	/// <para>
	/// <see cref="Vec2d"/> is a lightweight numeric type used throughout SystemEx
	/// for geometry, math utilities, device operations, and compute kernels.
	/// It stores two <see cref="double"/> values (<c>X</c> and <c>Y</c>) in a
	/// sequential memory layout, making it compatible with native interop and
	/// high‑performance compute backends.
	/// </para>
	///
	/// <para>
	/// The struct is annotated with <see cref="HashAlgorithmAttribute"/> to enable
	/// attribute‑driven hashing via <see cref="HashFactory"/>.  
	/// BernsteinHash is used because it is fast, byte‑linear, and ideal for small
	/// fixed‑size numeric types such as vectors.
	/// </para>
	///
	/// <para>
	/// <see cref="Vec2d"/> implements multiple comparison and hashing interfaces:
	/// <list type="bullet">
	/// <item><description><see cref="IComparable"/> and <see cref="IComparable{T}"/> for ordering</description></item>
	/// <item><description><see cref="IEquatable{T}"/> for equality checks</description></item>
	/// <item><description><see cref="IHashable{T}"/> for deterministic byte‑level hashing</description></item>
	/// </list>
	/// This makes the type suitable for use in dictionaries, sorting, spatial
	/// hashing, and compute pipelines.
	/// </para>
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
    [HashAlgorithm(typeof(BernsteinHash), Endian.System)]
    public struct Vec2d : IComparable, IComparableEx<Vec2d>, IComparable<Vec2d>, IEquatable<Vec2d>, IHashable<Vec2d> {
        private double m_x;
        private double m_y;

        /// <summary>
        /// Gets the number of components in this vector (always 2).
        /// </summary>
        public int Count => 2;

        /// <summary>
        /// Represents Vector(0,0)
        /// </summary>
        public static readonly Vec2d Zero = new Vec2d(0);
        /// <summary>
        /// Represents Vector(1,1)
        /// </summary>
        public static readonly Vec2d One  = new Vec2d(1f);

        /// <summary>
        /// Represents Vector(-1,-1)
        /// </summary>
        public static readonly Vec2d NegativeOne  = new Vec2d(-1f);

        /// <summary>
        /// Represents Vector(MIN,MIN)
        /// </summary>
        public static readonly Vec2d Min  = new Vec2d(int.MinValue);

        /// <summary>
        /// Represents Vector(MAX,MAX)
        /// </summary>
        public static readonly Vec2d Max  = new Vec2d(int.MaxValue);

        /// <summary>
        /// Gets or sets the X component.
        /// </summary>
        public double X { get => m_x; set => m_x = value; }

        /// <summary>
        /// Gets or sets the Y component.
        /// </summary>
        public double Y { get => m_y; set => m_y = value; }

        /// <summary>
        /// Initializes a zero vector (0,0).
        /// </summary>
        public Vec2d () {
            m_x = m_y = 0.0f;
        }

        /// <summary>
        /// Initializes a vector with explicit X and Y values.
        /// </summary>
        public Vec2d ( double _x, double _y ) {
            m_x = _x;
            m_y = _y;
        }


        /// <summary>
        /// Initializes both components with the same value.
        /// </summary>
        public Vec2d ( double _f ) {
            m_x = _f;
            m_y = _f;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        public Vec2d ( Vec2d vec ) {
            m_x = vec.m_x;
            m_y = vec.m_y;
        }

        /// <summary>
        /// Initializes the vector from a double array.
        /// </summary>
        public Vec2d ( double[] lpvec ) {
            m_x = lpvec[0];
            m_y = lpvec[1];
        }

        /// <summary>
        /// Gets a component by index (0 = X, 1 = Y).
        /// </summary>
        public double Get ( int index ) {
            if ( index >= Count ) throw new ArgumentOutOfRangeException();
            return index == 0 ? m_x : m_y;
        }

        /// <summary>
        /// Computes the squared length of the vector.
        /// </summary>
        public static double Lenght ( Vec2d v ) => (v.m_x * v.m_x + v.m_y * v.m_y);

        /// <summary>
        /// Computes the Euclidean length of the vector.
        /// </summary>
        public static double LenghtSqrt ( Vec2d v ) => System.Math.Sqrt(Lenght(v));

        /// <summary>
        /// Computes the dot product of two vectors.
        /// </summary>
        public static double Dot ( Vec2d v1, Vec2d v2 ) {
            return (v1.m_x * v2.m_x + v1.m_y * v2.m_y);
        }

        /// <summary>
        /// Computes the angle between two vectors.
        /// </summary>
        public static double Angle ( Vec2d v1, Vec2d v2 ) {
            return System.Math.Acos(v1.m_x * v2.m_x + v1.m_y * v2.m_y) /
                   System.Math.Sqrt((v1.m_x * v1.m_x + v1.m_y * v1.m_y) *
                              (v2.m_x * v2.m_x + v2.m_y * v2.m_y));
        }

        /// <summary>
        /// Linearly interpolates between two vectors.
        /// </summary>
        public static Vec2d InterpolateCoords ( Vec2d v1, Vec2d v2, double p ) {
            return v1 + p * (v2 - v1);
        }

        /// <summary>
        /// Interpolates and normalizes the result.
        /// </summary>
        public static Vec2d InterpolateNormal ( Vec2d v1, Vec2d v2, double p ) {
            return Normalize(v1 + p * (v2 - v1));
        }

        /// <summary>
        /// Checks whether two vectors are approximately equal.
        /// </summary>
        public static bool NearEqual ( Vec2d v1, Vec2d v2, double epsilon ) {
            return (System.Math.Abs(v1.m_x - v2.m_x) <= epsilon) &&
                   (System.Math.Abs(v1.m_y - v2.m_y) <= epsilon);
        }

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        public static Vec2d Normalize ( Vec2d v, bool ex = false ) {
            var f = v / LenghtSqrt(v);
            return ex ? (f + 0.0001f) : f;
        }




        /// <summary>
        /// Computes a hash code for this vector.
        ///
        /// <para>
        /// The primary hash is generated using <see cref="HashFactory"/> and the
        /// <see cref="HashAlgorithmAttribute"/> applied to this struct.  
        /// If hashing fails (rare), a fallback XOR‑based hash is used.
        /// </para>
        /// </summary>
        public override int GetHashCode () {
            var x =  HashFactory.Hash32(this, 674545);
            if ( x.Value != 0 ) return (int)x.Value;

            return m_x.GetHashCode() ^ m_y.GetHashCode();
        }
        /// <summary>
        /// Compares this vector to another object.
        /// </summary>
        public int CompareTo ( object? obj ) {
            if ( (obj is Vec2d) ) {
                return (int)CompareTo((Vec2d)(obj));
            }
            throw new ArgumentException("Object is not a Vec2d object");
        }
        /// <summary>
        /// Compares two vectors lexicographically.
        /// </summary>
        public CompareResult CompareTo ( Vec2d a ) {
            CompareResult _ret = CompareResult.Equal;

            if ( this < a ) _ret = CompareResult.AIsSmallerB;
            else if ( this > a ) _ret = CompareResult.AIsLargerB;


            return _ret;
        }

        /// <summary>
        /// Determines whether this instance is equal to another
        /// <see cref="Vec2d"/> value, using the same semantics as the
        /// <see cref="operator ==(Vec2d,Vec2d)"/>.
        /// </summary>
        /// <param name="other">The value to compare with.</param>
        /// <returns>
        /// <c>true</c> if the values are equal; otherwise <c>false</c>.
        /// </returns>
        public bool Equals ( Vec2d other ) {
            return this.X == other.X && this.Y == other.Y;
        }

        /// <summary>
        /// Determines whether this instance is equal to another object.
        /// 
        /// The object is considered equal if it is a <see cref="Vec2d"/>
        /// and compares equal using <see cref="Equals(Vec2d)"/>.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is a <see cref="Vec2d"/>
        /// and equal to this instance; otherwise <c>false</c>.
        /// </returns>
        public override bool Equals ( object? obj ) {
            if ( obj == null ) return false;
            return (obj is Vec2d) && Equals((Vec2d)obj);
        }

        int IComparable<Vec2d>.CompareTo ( Vec2d other ) {
            return (int)CompareTo(other);
        }
        /// <summary>
        /// Converts the vector into a deterministic byte sequence.
        ///
        /// <para>
        /// This method is used by <see cref="HashFactory"/> to compute
        /// attribute‑driven hashes. The byte layout is stable and platform‑safe,
        /// ensuring consistent hashing across devices and backends.
        /// </para>
        /// </summary>
        public FixedVector<byte> ToBytes () {
            Cache m = new Cache(sizeof(double) * Count);

            for ( byte i = 0 ; i < Count ; i++ )
                m.WriteRange((ulong)(sizeof(double) * i), Get(i).ToBytes());

            return m.ToArrayEx();
        }
        /// <summary>
        /// Adds two vectors component‑wise.
        /// </summary>
        public static Vec2d operator + ( Vec2d a, Vec2d b ) {
            return new Vec2d(a.m_x + b.m_x, a.m_y + b.m_y);
        }
        /// <summary>
        /// Subtracts two vectors component‑wise.
        /// </summary>
        public static Vec2d operator - ( Vec2d a, Vec2d b ) {
            return new Vec2d(a.m_x - b.m_x, a.m_y - b.m_y);
        }
        /// <summary>
        /// Divides two vectors component‑wise.
        /// </summary>
        public static Vec2d operator / ( Vec2d a, Vec2d b ) {
            return new Vec2d(a.m_x / b.m_x, a.m_y / b.m_y);
        }
        /// <summary>
        /// Multiplies two vectors component‑wise.
        /// </summary>
        public static Vec2d operator * ( Vec2d a, Vec2d b ) {
            return new Vec2d(a.m_x * b.m_x, a.m_y * b.m_y);
        }
        /// <summary>
        /// Adds a scalar to both components of the vector.
        /// </summary>
        public static Vec2d operator + ( Vec2d a, double b ) {
            return new Vec2d(a.m_x + b, a.m_y + b);
        }
        /// <summary>
        /// Subtracts a scalar from both components of the vector.
        /// </summary>
        public static Vec2d operator - ( Vec2d a, double b ) {
            return new Vec2d(a.m_x - b, a.m_y - b);
        }
        /// <summary>
        /// Divides both components of the vector by a scalar.
        /// </summary>
        public static Vec2d operator / ( Vec2d a, double b ) {
            return new Vec2d(a.m_x / b, a.m_y / b);
        }
        // <summary>
        /// Multiplies both components of the vector by a scalar.
        /// </summary>
        public static Vec2d operator * ( Vec2d a, double b ) {
            return new Vec2d(a.m_x * b, a.m_y * b);
        }
        /// <summary>
        /// Subtracts each component of the vector from a scalar.
        /// </summary>
        public static Vec2d operator - ( double a, Vec2d b ) {
            return new Vec2d(a - b.m_x, a - b.m_y);
        }
        // <summary>
        /// Divides a scalar by each component of the vector.
        /// </summary>
        public static Vec2d operator / ( double a, Vec2d b ) {
            return new Vec2d(a / b.m_x, a / b.m_y);
        }
        /// <summary>
        /// Multiplies a scalar with each component of the vector.
        /// </summary>
        public static Vec2d operator * ( double a, Vec2d b ) {
            return new Vec2d(a * b.m_x, a * b.m_y);
        }
        /// <summary>
        /// Adds a scalar to each component of the vector.
        /// </summary>
        public static Vec2d operator + ( double a, Vec2d b ) {
            return new Vec2d(a + b.m_x, a + b.m_y);
        }
        /// <summary>
        /// Determines whether two vectors are equal component‑wise.
        /// </summary>
        public static bool operator == ( Vec2d a, Vec2d b ) {
            return a.m_x == b.m_x && a.m_y == b.m_y;
        }
        /// <summary>
        /// Determines whether two vectors differ in any component.
        /// </summary>
        public static bool operator != ( Vec2d a, Vec2d b ) {
            return a.m_x != b.m_x && a.m_y != b.m_y;
        }
        /// <summary>
        /// Determines whether all components of <paramref name="a"/> are less than or equal to those of <paramref name="b"/>.
        /// </summary>
        public static bool operator <= ( Vec2d a, Vec2d b ) {
            return a.m_x <= b.m_x && a.m_y <= b.m_y;
        }
        /// <summary>
        /// Determines whether all components of <paramref name="a"/> are greater than or equal to those of <paramref name="b"/>.
        /// </summary>
        public static bool operator >= ( Vec2d a, Vec2d b ) {
            return a.m_x >= b.m_x && a.m_y >= b.m_y;
        }
        /// <summary>
        /// Determines whether all components of <paramref name="a"/> are strictly less than those of <paramref name="b"/>.
        /// </summary>
        public static bool operator < ( Vec2d a, Vec2d b ) {
            return a.m_x < b.m_x && a.m_y < b.m_y;
        }
        /// <summary>
        /// Determines whether all components of <paramref name="a"/> are strictly greater than those of <paramref name="b"/>.
        /// </summary>
        public static bool operator > ( Vec2d a, Vec2d b ) {
            return a.m_x > b.m_x && a.m_y > b.m_y;
        }

        /// <summary>
        /// Determines whether a scalar equals both components of the vector.
        /// </summary>
        public static bool operator == ( double a, Vec2d b ) {
            return a == b.m_x && a == b.m_y;
        }

        /// <summary>
        /// Determines whether a scalar differs from any component of the vector.
        /// </summary>
        public static bool operator != ( double a, Vec2d b ) {
            return a != b.m_x && a != b.m_y;
        }

        /// <summary>
        /// Determines whether a scalar is less than or equal to both vector components.
        /// </summary>
        public static bool operator <= ( double a, Vec2d b ) {
            return a <= b.m_x && a <= b.m_y;
        }

        /// <summary>
        /// Determines whether a scalar is greater than or equal to both vector components.
        /// </summary>
        public static bool operator >= ( double a, Vec2d b ) {
            return a >= b.m_x && a >= b.m_y;
        }

        /// <summary>
        /// Determines whether a scalar is strictly less than both vector components.
        /// </summary>
        public static bool operator < ( double a, Vec2d b ) {
            return a < b.m_x && a < b.m_y;
        }

        /// <summary>
        /// Determines whether a scalar is strictly greater than both vector components.
        /// </summary>
        public static bool operator > ( double a, Vec2d b ) {
            return a > b.m_x && a > b.m_y;
        }

        /// <summary>
        /// Determines whether both vector components equal the scalar.
        /// </summary>
        public static bool operator == ( Vec2d a, double b ) {
            return a.m_x == b && a.m_y == b;
        }

        /// <summary>
        /// Determines whether any vector component differs from the scalar.
        /// </summary>
        public static bool operator != ( Vec2d a, double b ) {
            return a.m_x != b && a.m_y != b;
        }

        /// <summary>
        /// Determines whether both vector components are less than or equal to the scalar.
        /// </summary>
        public static bool operator <= ( Vec2d a, double b ) {
            return a.m_x <= b && a.m_y <= b;
        }

        /// <summary>
        /// Determines whether both vector components are greater than or equal to the scalar.
        /// </summary>
        public static bool operator >= ( Vec2d a, double b ) {
            return a.m_x >= b && a.m_y >= b;
        }

        /// <summary>
        /// Determines whether both vector components are strictly less than the scalar.
        /// </summary>
        public static bool operator < ( Vec2d a, double b ) {
            return a.m_x < b && a.m_y < b;
        }

        /// <summary>
        /// Determines whether both vector components are strictly greater than the scalar.
        /// </summary>
        public static bool operator > ( Vec2d a, double b ) {
            return a.m_x > b && a.m_y > b;
        }
    }
    /// @}
}