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
	/// Represents a 2‑component Ratio vector.
	///
	/// <para>
	/// <see cref="Vec2r"/> is a lightweight numeric type used throughout SystemEx
	/// for geometry, math utilities, device operations, and compute kernels.
	/// It stores two <see cref="Ratio"/> values (<c>X</c> and <c>Y</c>).
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
	/// <see cref="Vec2r"/> implements multiple comparison and hashing interfaces:
	/// <list type="bullet">
	/// <item><description><see cref="IComparable"/> and <see cref="IComparable{T}"/> for ordering</description></item>
	/// <item><description><see cref="IEquatable{T}"/> for equality checks</description></item>
	/// <item><description><see cref="IHashable{T}"/> for deterministic byte‑level hashing</description></item>
	/// </list>
	/// This makes the type suitable for use in dictionaries, sorting, spatial
	/// hashing.
	/// </para>
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	[HashAlgorithm(typeof(BernsteinHash), Endian.System)]
	public struct Vec2r : IComparable, IComparableEx<Vec2r>, IComparable<Vec2r>, IEquatable<Vec2r>, IHashable<Vec2r> {
		private Ratio m_x;
		private Ratio m_y;

		/// <summary>
		/// Gets the number of components in this vector (always 2).
		/// </summary>
		public int Count => 2;

		/// <summary>
		/// A vector with both components set to zero (Ratio.Zero, Ratio.Zero).
		/// </summary>
		public static readonly Vec2r Zero = new Vec2r(Ratio.Zero);

		/// <summary>
		/// A vector with both components set to one (Ratio.One, Ratio.One).
		/// </summary>
		public static readonly Vec2r One  = new Vec2r(Ratio.One);

		/// <summary>
		/// A vector with both components set to negative one (-Ratio.One, -Ratio.One).
		/// </summary>
		public static readonly Vec2r NegativeOne  = new Vec2r(-Ratio.One);

		/// <summary>
		/// A vector with both components set to <see cref="Ratio.MinValue"/>.
		/// </summary>
		public static readonly Vec2r Min  = new Vec2r(Ratio.MinValue);

		/// <summary>
		/// A vector with both components set to <see cref="Ratio.MaxValue"/>.
		/// </summary>
		public static readonly Vec2r Max  = new Vec2r(Ratio.MaxValue);

		/// <summary>
		/// Gets or sets the X component of the vector.
		/// </summary>
		public Ratio X { get => m_x; set => m_x = value; }

		/// <summary>
		/// Gets or sets the Y component of the vector.
		/// </summary>
		public Ratio Y { get => m_y; set => m_y = value; }

		/// <summary>
		/// Initializes a new vector with both components set to One.
		/// </summary>
		public Vec2r () {
			m_x = m_y = Ratio.One;
		}
		/// <summary>
		/// Initializes a new vector with the specified X and Y components.
		/// </summary>
		/// <param name="_x">The X component.</param>
		/// <param name="_y">The Y component.</param>
		public Vec2r ( Ratio _x, Ratio _y ) {
			m_x = _x;
			m_y = _y;
		}

		/// <summary>
		/// Initializes a new vector with both components set to the same scalar value.
		/// </summary>
		/// <param name="_f">The scalar value assigned to both components.</param>
		public Vec2r ( Ratio _f ) {
			m_x = _f;
			m_y = _f;
		}
		/// <summary>
		/// Initializes a new vector by copying the components of another vector.
		/// </summary>
		/// <param name="vec">The vector to copy.</param>
		public Vec2r ( Vec2r vec ) {
			m_x = vec.m_x;
			m_y = vec.m_y;
		}

		/// <summary>
		/// Initializes a new vector from an Ratio array of length 2.
		/// </summary>
		/// <param name="lpvec">An array containing two Ratio values.</param>
		/// <exception cref="IndexOutOfRangeException">
		/// Thrown if the array does not contain at least two elements.
		/// </exception>
		public Vec2r ( Ratio[] lpvec ) {
			m_x = lpvec[0];
			m_y = lpvec[1];
		}

		/// <summary>
		/// Gets the component at the specified index.
		/// </summary>
		/// <param name="index">The component index (0 for X, 1 for Y).</param>
		/// <returns>The requested component.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if the index is outside the valid range.
		/// </exception>
		public Ratio Get ( int index ) {
			if ( index >= Count ) throw new ArgumentOutOfRangeException();
			return index == 0 ? m_x : m_y;
		}

		/// <summary>
		/// Computes the squared length of the vector (x² + y²).
		/// </summary>
		/// <param name="v">The vector whose length is computed.</param>
		/// <returns>The squared length as a <see cref="Ratio"/>.</returns>
		public static Ratio Lenght ( Vec2r v ) => (v.m_x * v.m_x + v.m_y * v.m_y);

		/// <summary>
		/// Computes the dot product of two vectors.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>The dot product as a <see cref="Ratio"/>.</returns>
		public static Ratio Dot ( Vec2r v1, Vec2r v2 ) {
			return (v1.m_x * v2.m_x + v1.m_y * v2.m_y);
		}

		/// <summary>
		/// Linearly interpolates between two vectors using the parameter <paramref name="p"/>.
		/// </summary>
		/// <param name="v1">The start vector.</param>
		/// <param name="v2">The end vector.</param>
		/// <param name="p">
		/// The interpolation factor Values between 0 and 1 (values outside this range perform extrapolation)
		/// </param>
		/// <returns>The interpolated vector.</returns>
		public static Vec2r InterpolateCoords ( Vec2r v1, Vec2r v2, int p ) {
			return v1 + p * (v2 - v1);
		}

		/// <summary>
		/// Linearly interpolates between two vectors using the parameter <paramref name="p"/>.
		/// </summary>
		/// <param name="v1">The start vector.</param>
		/// <param name="v2">The end vector.</param>
		/// <param name="p">
		/// The interpolation factor Values between 0 and 1 (values outside this range perform extrapolation)
		/// </param>
		/// <returns>The interpolated vector.</returns>
		public static Vec2r InterpolateCoords ( Vec2r v1, Vec2r v2, Ratio p ) {
			return v1 + p * (v2 - v1);
		}

		/// <summary>
		/// Compares this vector to another object.
		/// </summary>
		/// <param name="obj">The object to compare with.</param>
		/// <returns>
		/// A signed integer indicating the comparison result.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="obj"/> is not a <see cref="Vec2r"/>.
		/// </exception>
		public int CompareTo ( object? obj ) {
			if ( (obj is Vec2r vec) ) {
				return (int)CompareTo(vec);
			}
			throw new ArgumentException("Object is not a Vec2r object");
		}
		/// <summary>
		/// Compares two vectors lexicographically.
		/// </summary>
		public CompareResult CompareTo ( Vec2r a ) {
			CompareResult _ret = CompareResult.Equal;

			if ( this < a ) _ret = CompareResult.Less;
			else if ( this > a ) _ret = CompareResult.Greater;


			return _ret;
		}

		/// <summary>
		/// Determines whether this instance is equal to another
		/// <see cref="Vec2r"/> value, using the same semantics as the
		/// <see cref="operator ==(Vec2r,Vec2r)"/>.
		/// </summary>
		/// <param name="other">The value to compare with.</param>
		/// <returns>
		/// <c>true</c> if the values are equal; otherwise <c>false</c>.
		/// </returns>
		public bool Equals ( Vec2r other ) {
			return this.X == other.X && this.Y == other.Y;
		}

		/// <summary>
		/// Determines whether this instance is equal to another object.
		/// 
		/// The object is considered equal if it is a <see cref="Vec2r"/>
		/// and compares equal using <see cref="Equals(Vec2r)"/>.
		/// </summary>
		/// <param name="obj">The object to compare with.</param>
		/// <returns>
		/// <c>true</c> if <paramref name="obj"/> is a <see cref="Vec2r"/>
		/// and equal to this instance; otherwise <c>false</c>.
		/// </returns>
		public override bool Equals ( object? obj ) {
			if ( obj == null ) return false;
			return (obj is Vec2r vec) && Equals(vec);
		}

		int IComparable<Vec2r>.CompareTo ( Vec2r other ) {
			return (int)CompareTo(other);
		}

		/// <summary>
		/// Checks whether two vectors are approximately equal.
		/// </summary>
		public static bool NearEqual ( Vec2r v1, Vec2r v2, Ratio epsilon ) {
			return (MathR.Abs(v1.m_x - v2.m_x) <= epsilon) &&
				   (MathR.Abs(v1.m_y - v2.m_y) <= epsilon);
		}
		/// <summary>
		/// Convert To Byte
		/// </summary>
		/// <returns></returns>
		public FixedVector<byte> ToBytes () {
			Cache m = new Cache(4 * sizeof(long));
			m.WriteRange(0, m_x.ToBytes());
			m.WriteRange(16, m_y.ToBytes());

			return m.ToArrayEx();
		}

		/// <summary>
		/// Adds two vectors component‑wise.
		/// </summary>
		public static Vec2r operator + ( Vec2r a, Vec2r b ) {
			return new Vec2r(a.m_x + b.m_x, a.m_y + b.m_y);
		}
		/// <summary>
		/// Subtracts two vectors component‑wise.
		/// </summary>
		public static Vec2r operator - ( Vec2r a, Vec2r b ) {
			return new Vec2r(a.m_x - b.m_x, a.m_y - b.m_y);
		}
		/// <summary>
		/// Divides two vectors component‑wise.
		/// </summary>
		public static Vec2r operator / ( Vec2r a, Vec2r b ) {
			return new Vec2r(a.m_x / b.m_x, a.m_y / b.m_y);
		}
		/// <summary>
		/// Multiplies two vectors component‑wise.
		/// </summary>
		public static Vec2r operator * ( Vec2r a, Vec2r b ) {
			return new Vec2r(a.m_x * b.m_x, a.m_y * b.m_y);
		}
		/// <summary>
		/// Adds a scalar to both components of the vector.
		/// </summary>
		public static Vec2r operator + ( Vec2r a, Ratio b ) {
			return new Vec2r(a.m_x + b, a.m_y + b);
		}
		/// <summary>
		/// Subtracts a scalar from both components of the vector.
		/// </summary>
		public static Vec2r operator - ( Vec2r a, Ratio b ) {
			return new Vec2r(a.m_x - b, a.m_y - b);
		}
		/// <summary>
		/// Divides both components of the vector by a scalar.
		/// </summary>
		public static Vec2r operator / ( Vec2r a, Ratio b ) {
			return new Vec2r(a.m_x / b, a.m_y / b);
		}
		/// <summary>
		/// Multiplies both components of the vector by a scalar.
		/// </summary>
		public static Vec2r operator * ( Vec2r a, Ratio b ) {
			return new Vec2r(a.m_x * b, a.m_y * b);
		}
		/// <summary>
		/// Subtracts each component of the vector from a scalar.
		/// </summary>
		public static Vec2r operator - ( Ratio a, Vec2r b ) {
			return new Vec2r(a - b.m_x, a - b.m_y);
		}
		/// <summary>
		/// Divides a scalar by each component of the vector.
		/// </summary>
		public static Vec2r operator / ( Ratio a, Vec2r b ) {
			return new Vec2r(a / b.m_x, a / b.m_y);
		}
		/// <summary>
		/// Multiplies a scalar with each component of the vector.
		/// </summary>
		public static Vec2r operator * ( Ratio a, Vec2r b ) {
			return new Vec2r(a * b.m_x, a * b.m_y);
		}
		/// <summary>
		/// Adds a scalar to each component of the vector.
		/// </summary>
		public static Vec2r operator + ( Ratio a, Vec2r b ) {
			return new Vec2r(a + b.m_x, a + b.m_y);
		}
		/// <summary>
		/// Determines whether two vectors are equal component‑wise.
		/// </summary>
		public static bool operator == ( Vec2r a, Vec2r b ) {
			return a.m_x == b.m_x && a.m_y == b.m_y;
		}
		/// <summary>
		/// Determines whether two vectors differ in any component.
		/// </summary>
		public static bool operator != ( Vec2r a, Vec2r b ) {
			return a.m_x != b.m_x && a.m_y != b.m_y;
		}
		/// <summary>
		/// Determines whether all components of <paramref name="a"/> are less than or equal to those of <paramref name="b"/>.
		/// </summary>
		public static bool operator <= ( Vec2r a, Vec2r b ) {
			return a.m_x <= b.m_x && a.m_y <= b.m_y;
		}
		/// <summary>
		/// Determines whether all components of <paramref name="a"/> are greater than or equal to those of <paramref name="b"/>.
		/// </summary>
		public static bool operator >= ( Vec2r a, Vec2r b ) {
			return a.m_x >= b.m_x && a.m_y >= b.m_y;
		}
		/// <summary>
		/// Determines whether all components of <paramref name="a"/> are strictly less than those of <paramref name="b"/>.
		/// </summary>
		public static bool operator < ( Vec2r a, Vec2r b ) {
			return a.m_x < b.m_x && a.m_y < b.m_y;
		}
		/// <summary>
		/// Determines whether all components of <paramref name="a"/> are strictly greater than those of <paramref name="b"/>.
		/// </summary>
		public static bool operator > ( Vec2r a, Vec2r b ) {
			return a.m_x > b.m_x && a.m_y > b.m_y;
		}

		/// <summary>
		/// Determines whether a scalar equals both components of the vector.
		/// </summary>
		public static bool operator == ( Ratio a, Vec2r b ) {
			return a == b.m_x && a == b.m_y;
		}

		/// <summary>
		/// Determines whether a scalar differs from any component of the vector.
		/// </summary>
		public static bool operator != ( Ratio a, Vec2r b ) {
			return a != b.m_x && a != b.m_y;
		}

		/// <summary>
		/// Determines whether a scalar is less than or equal to both vector components.
		/// </summary>
		public static bool operator <= ( Ratio a, Vec2r b ) {
			return a <= b.m_x && a <= b.m_y;
		}

		/// <summary>
		/// Determines whether a scalar is greater than or equal to both vector components.
		/// </summary>
		public static bool operator >= ( Ratio a, Vec2r b ) {
			return a >= b.m_x && a >= b.m_y;
		}

		/// <summary>
		/// Determines whether a scalar is strictly less than both vector components.
		/// </summary>
		public static bool operator < ( Ratio a, Vec2r b ) {
			return a < b.m_x && a < b.m_y;
		}

		/// <summary>
		/// Determines whether a scalar is strictly greater than both vector components.
		/// </summary>
		public static bool operator > ( Ratio a, Vec2r b ) {
			return a > b.m_x && a > b.m_y;
		}

		/// <summary>
		/// Determines whether both vector components equal the scalar.
		/// </summary>
		public static bool operator == ( Vec2r a, Ratio b ) {
			return a.m_x == b && a.m_y == b;
		}

		/// <summary>
		/// Determines whether any vector component differs from the scalar.
		/// </summary>
		public static bool operator != ( Vec2r a, Ratio b ) {
			return a.m_x != b && a.m_y != b;
		}

		/// <summary>
		/// Determines whether both vector components are less than or equal to the scalar.
		/// </summary>
		public static bool operator <= ( Vec2r a, Ratio b ) {
			return a.m_x <= b && a.m_y <= b;
		}

		/// <summary>
		/// Determines whether both vector components are greater than or equal to the scalar.
		/// </summary>
		public static bool operator >= ( Vec2r a, Ratio b ) {
			return a.m_x >= b && a.m_y >= b;
		}

		/// <summary>
		/// Determines whether both vector components are strictly less than the scalar.
		/// </summary>
		public static bool operator < ( Vec2r a, Ratio b ) {
			return a.m_x < b && a.m_y < b;
		}

		/// <summary>
		/// Determines whether both vector components are strictly greater than the scalar.
		/// </summary>
		public static bool operator > ( Vec2r a, Ratio b ) {
			return a.m_x > b && a.m_y > b;
		}
		/// <summary>
		/// Computes a hash code for this object.
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
	}
	
}
