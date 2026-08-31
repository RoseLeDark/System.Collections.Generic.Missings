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
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using SystemEx.Collections.Generic;

using SystemEx.Hash;
using SystemEx.Utils;

namespace SystemEx.Numeric {
	/// \addtogroup Numeric
	/// @{

	/// <summary>
	/// Represents a 3‑component floating‑point vector.
	///
	/// <para>
	/// <see cref="Vec3f"/> is a lightweight numeric type used throughout SystemEx
	/// for geometry, math utilities, device operations, and compute kernels.
	/// It stores two <see cref="float"/> values (<c>X</c> and <c>Y</c>) in a
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
	/// <see cref="Vec3f"/> implements multiple comparison and hashing interfaces:
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
    public struct Vec3f : IComparable, IComparableEx<Vec3f>, IComparable<Vec3f>, IEquatable<Vec3f>, IHashable<Vec3f> {
        private float m_x;
        private float m_y;
        private float m_z;

        /// <summary>
        /// Represents Vector(0,0,0)
        /// </summary>
        public static readonly Vec3f Zero = new Vec3f(0,0,0);
        /// <summary>
        /// Represents Vector(1,1,1)
        /// </summary>
        public static readonly Vec3f One  = new Vec3f(1f, 1f, 1f);

        /// <summary>
        /// Represents Vector(-1,-1,-1)
        /// </summary>
        public static readonly Vec3f NegativeOne  = new Vec3f(-1f, -1f, -1f);

        /// <summary>
        /// Represents Vector(MIN,MIN,MIN)
        /// </summary>
        public static readonly Vec3f Min  = new Vec3f(float.MinValue, float.MinValue, float.MinValue);

        /// <summary>
        /// Represents Vector(MAX,MAX,MAX)
        /// </summary>
        public static readonly Vec3f Max  = new Vec3f(float.MaxValue, float.MaxValue, float.MaxValue);

        /// <summary>
        /// Gets the number of components in this vector (always 3).
        /// </summary>
        public int Count => 3;

        /// <summary>
        /// Gets or sets the X component.
        /// </summary>
        public float X { get => m_x; set => m_x = value; }

        /// <summary>
        /// Gets or sets the Y component.
        /// </summary>
        public float Y { get => m_y; set => m_y = value; }
        /// <summary>
        /// Gets or sets the Z component.
        /// </summary>
        public float Z { get => m_z; set => m_z = value; }

        /// <summary>
        /// Initializes a zero vector (0,0).
        /// </summary>
        public Vec3f () {
            m_x = m_y = m_z = 0.0f;
        }

        /// <summary>
        /// Initializes a vector with explicit X and Y values.
        /// </summary>
        public Vec3f ( float _x, float _y, float _z ) {
            m_x = _x;
            m_y = _y;
            m_z = _z;
        }


        /// <summary>
        /// Initializes both components with the same value.
        /// </summary>
        public Vec3f ( float _f ) {
            m_x = _f;
            m_y = _f;
            m_z = _f;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        public Vec3f ( Vec3f vec ) {
            m_x = vec.m_x;
            m_y = vec.m_y;
            m_z = vec.m_z;
        }

        /// <summary>
        /// Initializes the vector from a float array.
        /// </summary>
        public Vec3f ( float[] lpvec ) {
            m_x = lpvec[0];
            m_y = lpvec[1];
            m_z = lpvec[2];
        }

        /// <summary>
        /// Gets a component by index (0 = X, 1 = Y).
        /// </summary>
        public float Get ( int index ) {
            return index switch
            {
                0 => m_x,
                1 => m_y,
                2 => m_z,
                _ => throw new ArgumentOutOfRangeException(nameof(index))
            };
        }


        /// <summary>
        /// Computes the squared length of the vector.
        /// </summary>
        public static float Lenght ( Vec3f v ) => (v.m_x * v.m_x + v.m_y * v.m_y + v.m_z * v.m_z);

        /// <summary>
        /// Computes the Euclidean length of the vector.
        /// </summary>
        public static float LenghtSqrt ( Vec3f v ) => System.MathF.Sqrt(Lenght(v));

        /// <summary>
        /// Computes the dot product of two vectors.
        /// </summary>
        public static float Dot ( Vec3f v1, Vec3f v2 ) {
            return (v1.m_x * v2.m_x + v1.m_y * v2.m_y + v1.m_z * v2.m_z);
        }

        /// <summary>
        /// Computes the angle between two vectors.
        /// </summary>
        public static float Angle ( Vec3f v1, Vec3f v2 ) {
            return System.MathF.Acos(v1.m_x * v2.m_x + v1.m_y * v2.m_y + v1.m_z * v2.m_z) /
                   System.MathF.Sqrt((v1.m_x * v1.m_x + v1.m_y * v1.m_y + v1.m_z * v1.m_z) *
                              (v2.m_x * v2.m_x + v2.m_y * v2.m_y + v2.m_z * v2.m_z));
        }

        /// <summary>
        /// Linearly interpolates between two vectors.
        /// </summary>
        public static Vec3f InterpolateCoords ( Vec3f v1, Vec3f v2, float p ) {
            return v1 + p * (v2 - v1);
        }

        /// <summary>
        /// Interpolates and normalizes the result.
        /// </summary>
        public static Vec3f InterpolateNormal ( Vec3f v1, Vec3f v2, float p ) {
            return Normalize(v1 + p * (v2 - v1));
        }

        /// <summary>
        /// Checks whether two vectors are approximately equal.
        /// </summary>
        public static bool NearEqual ( Vec3f v1, Vec3f v2, float epsilon ) {
            return (System.MathF.Abs(v1.m_x - v2.m_x) <= epsilon) &&
                   (System.MathF.Abs(v1.m_y - v2.m_y) <= epsilon) &&
                   (System.MathF.Abs(v1.m_z - v2.m_z) <= epsilon);
        }

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        public static Vec3f Normalize ( Vec3f v, bool ex = false ) {
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

            return m_x.GetHashCode() ^ m_y.GetHashCode() ^ m_z.GetHashCode();

        }
        /// <summary>
        /// Compares this vector to another object.
        /// </summary>
        public int CompareTo ( object? obj ) {
            if ( (obj is Vec3f) ) {
                return (int)CompareTo((Vec3f)(obj));
            }
            throw new ArgumentException("Object is not a Vec3f object");
        }
        /// <summary>
        /// Compares two vectors lexicographically.
        /// </summary>
        public CompareResult CompareTo ( Vec3f a ) {
            CompareResult _ret = CompareResult.Equal;

            if ( this < a ) _ret = CompareResult.AIsSmallerB;
            else if ( this > a ) _ret = CompareResult.AIsLargerB;


            return _ret;
        }

        /// <summary>
        /// Determines whether this instance is equal to another
        /// <see cref="Half16"/> value, using the same semantics as the
        /// <see cref="operator ==(Vec3f,Vec3f)"/>.
        /// </summary>
        /// <param name="other">The value to compare with.</param>
        /// <returns>
        /// <c>true</c> if the values are equal; otherwise <c>false</c>.
        /// </returns>
        public bool Equals ( Vec3f other ) {
            return this.X == other.X && this.Y == other.Y && this.Z == other.Z;
        }

        /// <summary>
        /// Determines whether this instance is equal to another object.
        /// 
        /// The object is considered equal if it is a <see cref="Vec3f"/>
        /// and compares equal using <see cref="Equals(Vec3f)"/>.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is a <see cref="Vec3f"/>
        /// and equal to this instance; otherwise <c>false</c>.
        /// </returns>
        public override bool Equals ( object? obj ) {
            if ( obj == null ) return false;
            return (obj is Vec3f) && Equals((Vec3f)obj);
        }

        int IComparable<Vec3f>.CompareTo ( Vec3f other ) {
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
            Cache m = new Cache(sizeof(float) * Count);

            for ( byte i = 0 ; i < Count ; i++ )
                m.WriteRange((ulong)(sizeof(float) * i), Get(i).ToBytes());

            return m.ToArrayEx();
        }
        /// <summary>
        /// Adds two vectors component‑wise.
        /// </summary>
        public static Vec3f operator + ( Vec3f a, Vec3f b ) {
            return new Vec3f(a.m_x + b.m_x, a.m_y + b.m_y, a.m_z + b.m_z);
        }
        /// <summary>
        /// Subtracts two vectors component‑wise.
        /// </summary>
        public static Vec3f operator - ( Vec3f a, Vec3f b ) {
            return new Vec3f(a.m_x - b.m_x, a.m_y - b.m_y, a.m_z - b.m_z);
        }
        /// <summary>
        /// Divides two vectors component‑wise.
        /// </summary>
        public static Vec3f operator / ( Vec3f a, Vec3f b ) {
            return new Vec3f(a.m_x / b.m_x, a.m_y / b.m_y, a.m_z / b.m_z);
        }
        /// <summary>
        /// Multiplies two vectors component‑wise.
        /// </summary>
        public static Vec3f operator * ( Vec3f a, Vec3f b ) {
            return new Vec3f(a.m_x * b.m_x, a.m_y * b.m_y, a.m_z * b.m_z);
        }
        /// <summary>
        /// Adds a scalar to both components of the vector.
        /// </summary>
        public static Vec3f operator + ( Vec3f a, float b ) {
            return new Vec3f(a.m_x + b, a.m_y + b, a.m_z + b);
        }
        /// <summary>
        /// Subtracts a scalar from both components of the vector.
        /// </summary>
        public static Vec3f operator - ( Vec3f a, float b ) {
            return new Vec3f(a.m_x - b, a.m_y - b, a.m_z - b);
        }
        /// <summary>
        /// Divides both components of the vector by a scalar.
        /// </summary>
        public static Vec3f operator / ( Vec3f a, float b ) {
            return new Vec3f(a.m_x / b, a.m_y / b, a.m_z / b);
        }
        /// <summary>
        /// Multiplies both components of the vector by a scalar.
        /// </summary>
        public static Vec3f operator * ( Vec3f a, float b ) {
            return new Vec3f(a.m_x * b, a.m_y * b, a.m_z * b);
        }
        /// <summary>
        /// Subtracts each component of the vector from a scalar.
        /// </summary>
        public static Vec3f operator - ( float a, Vec3f b ) {
            return new Vec3f(a - b.m_x, a - b.m_y, a - b.m_z);
        }
        /// <summary>
        /// Divides a scalar by each component of the vector.
        /// </summary>
        public static Vec3f operator / ( float a, Vec3f b ) {
            return new Vec3f(a / b.m_x, a / b.m_y, a / b.m_z);
        }
        /// <summary>
        /// Multiplies a scalar with each component of the vector.
        /// </summary>
        public static Vec3f operator * ( float a, Vec3f b ) {
            return new Vec3f(a * b.m_x, a * b.m_y, a * b.m_z);
        }
        /// <summary>
        /// Adds a scalar to each component of the vector.
        /// </summary>
        public static Vec3f operator + ( float a, Vec3f b ) {
            return new Vec3f(a + b.m_x, a + b.m_y, a + b.m_z);
        }
        /// <summary>
        /// Determines whether two vectors are equal component‑wise.
        /// </summary>
        public static bool operator == ( Vec3f a, Vec3f b ) {
            return a.m_x == b.m_x && a.m_y == b.m_y && a.m_z == b.m_z;
        }
        /// <summary>
        /// Determines whether two vectors differ in any component.
        /// </summary>
        public static bool operator != ( Vec3f a, Vec3f b ) {
            return a.m_x != b.m_x && a.m_y != b.m_y && a.m_z != b.m_z;
        }
        /// <summary>
        /// Determines whether all components of <paramref name="a"/> are less than or equal to those of <paramref name="b"/>.
        /// </summary>
        public static bool operator <= ( Vec3f a, Vec3f b ) {
            return a.m_x <= b.m_x && a.m_y <= b.m_y && a.m_z <= b.m_z;
        }
        /// <summary>
        /// Determines whether all components of <paramref name="a"/> are greater than or equal to those of <paramref name="b"/>.
        /// </summary>
        public static bool operator >= ( Vec3f a, Vec3f b ) {
            return a.m_x >= b.m_x && a.m_y >= b.m_y && a.m_z >= b.m_z;
        }
        /// <summary>
        /// Determines whether all components of <paramref name="a"/> are strictly less than those of <paramref name="b"/>.
        /// </summary>
        public static bool operator < ( Vec3f a, Vec3f b ) {
            return a.m_x < b.m_x && a.m_y < b.m_y && a.m_z < b.m_z;
        }
        /// <summary>
        /// Determines whether all components of <paramref name="a"/> are strictly greater than those of <paramref name="b"/>.
        /// </summary>
        public static bool operator > ( Vec3f a, Vec3f b ) {
            return a.m_x > b.m_x && a.m_y > b.m_y && a.m_z > b.m_z;
        }

        /// <summary>
        /// Determines whether a scalar equals both components of the vector.
        /// </summary>
        public static bool operator == ( float a, Vec3f b ) {
            return a == b.m_x && a == b.m_y && a == b.m_z;
        }

        /// <summary>
        /// Determines whether a scalar differs from any component of the vector.
        /// </summary>
        public static bool operator != ( float a, Vec3f b ) {
            return a != b.m_x && a != b.m_y && a != b.m_z;
        }

        /// <summary>
        /// Determines whether a scalar is less than or equal to both vector components.
        /// </summary>
        public static bool operator <= ( float a, Vec3f b ) {
            return a <= b.m_x && a <= b.m_y && a <= b.m_z;
        }

        /// <summary>
        /// Determines whether a scalar is greater than or equal to both vector components.
        /// </summary>
        public static bool operator >= ( float a, Vec3f b ) {
            return a >= b.m_x && a >= b.m_y && a >= b.m_z;
        }

        /// <summary>
        /// Determines whether a scalar is strictly less than both vector components.
        /// </summary>
        public static bool operator < ( float a, Vec3f b ) {
            return a < b.m_x && a < b.m_y && a < b.m_z;
        }

        /// <summary>
        /// Determines whether a scalar is strictly greater than both vector components.
        /// </summary>
        public static bool operator > ( float a, Vec3f b ) {
            return a > b.m_x && a > b.m_y && a > b.m_z;
        }

        /// <summary>
        /// Determines whether both vector components equal the scalar.
        /// </summary>
        public static bool operator == ( Vec3f a, float b ) {
            return a.m_x == b && a.m_y == b && a.m_z == b;
        }

        /// <summary>
        /// Determines whether any vector component differs from the scalar.
        /// </summary>
        public static bool operator != ( Vec3f a, float b ) {
            return a.m_x != b && a.m_y != b && a.m_z != b;
        }

        /// <summary>
        /// Determines whether both vector components are less than or equal to the scalar.
        /// </summary>
        public static bool operator <= ( Vec3f a, float b ) {
            return a.m_x <= b && a.m_y <= b && a.m_z <= b;
        }

        /// <summary>
        /// Determines whether both vector components are greater than or equal to the scalar.
        /// </summary>
        public static bool operator >= ( Vec3f a, float b ) {
            return a.m_x >= b && a.m_y >= b && a.m_z >= b;
        }

        /// <summary>
        /// Determines whether both vector components are strictly less than the scalar.
        /// </summary>
        public static bool operator < ( Vec3f a, float b ) {
            return a.m_x < b && a.m_y < b && a.m_z < b;
        }

        /// <summary>
        /// Determines whether both vector components are strictly greater than the scalar.
        /// </summary>
        public static bool operator > ( Vec3f a, float b ) {
            return a.m_x > b && a.m_y > b && a.m_z > b;
        }
    }
    
}