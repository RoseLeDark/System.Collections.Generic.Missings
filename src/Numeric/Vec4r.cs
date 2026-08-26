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
	/// Represents a 4‑component Ratio vector.
	///
	/// <para>
	/// <see cref="Vec4r"/> is a lightweight numeric type used throughout SystemEx
	/// for geometry, math utilities, device operations, and compute kernels.
	/// It stores two <see cref="Ratio"/> values (<c>X</c> and <c>Y</c>) in a
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
	/// <see cref="Vec3r"/> implements multiple comparison and hashing interfaces:
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
	public struct Vec4r : IComparable, IComparableEx<Vec4r>, IComparable<Vec4r>, IEquatable<Vec4r>, IHashable<Vec4r> {
		private Ratio m_x;
		private Ratio m_y;
		private Ratio m_z;
		private Ratio m_w;

		/// <summary>
		/// Represents Vector(atio.Zero,atio.Zero,Ratio.Zero)
		/// </summary>
		public static readonly Vec4r Zero = new Vec4r(Ratio.Zero);
		/// <summary>
		/// Represents Vector(Ratio.One,Ratio.One,Ratio.One)
		/// </summary>
		public static readonly Vec4r One  = new Vec4r(Ratio.One);

		/// <summary>
		/// Represents Vector(-Ratio.One,-Ratio.One,-Ratio.One)
		/// </summary>
		public static readonly Vec4r NegativeOne  = new Vec4r(-Ratio.One);

		/// <summary>
		/// Represents Vector(MIN,MIN,MIN)
		/// </summary>
		public static readonly Vec4r Min  = new Vec4r(Ratio.MinValue);

		/// <summary>
		/// Represents Vector(MAX,MAX,MAX)
		/// </summary>
		public static readonly Vec4r Max  = new Vec4r(Ratio.MaxValue);

		/// <summary>
		/// Gets the number of components in this vector (always 4).
		/// </summary>
		public int Count => 4;

		/// <summary>
		/// Gets or sets the X component.
		/// </summary>
		public Ratio X { get => m_x; set => m_x = value; }

		/// <summary>
		/// Gets or sets the Y component.
		/// </summary>
		public Ratio Y { get => m_y; set => m_y = value; }
		/// <summary>
		/// Gets or sets the Z component.
		/// </summary>
		public Ratio Z { get => m_z; set => m_z = value; }
		/// <summary>
		/// Gets or sets the W component.
		/// </summary>
		public Ratio W { get => m_w; set => m_w = value; }

		/// <summary>
		/// Initializes a zero vector (0,0).
		/// </summary>
		public Vec4r () {
			m_x = m_y = m_z = m_w = Ratio.Zero;
		}

		/// <summary>
		/// Initializes a vector with explicit X Y Z W values.
		/// </summary>
		public Vec4r ( Ratio _x, Ratio _y, Ratio _z, Ratio _w ) {
			m_x = _x;
			m_y = _y;
			m_z = _z;
			m_w = _w;
		}


		/// <summary>
		/// Initializes both components with the same value.
		/// </summary>
		public Vec4r ( Ratio _f ) {
			m_x = _f;
			m_y = _f;
			m_z = _f;
			m_w = _f;
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		public Vec4r ( Vec4r vec ) {
			m_x = vec.m_x;
			m_y = vec.m_y;
			m_z = vec.m_z;
			m_w = vec.m_w;
		}

		/// <summary>
		/// Initializes the vector from a Ratio array.
		/// </summary>
		public Vec4r ( Ratio[] lpvec ) {
			m_x = lpvec[0];
			m_y = lpvec[1];
			m_z = lpvec[2];
			m_w = lpvec[3];
		}

		/// <summary>
		/// Gets a component by index (0 = X, 1 = Y).
		/// </summary>
		public Ratio Get ( int index ) {
			return index switch
			{
				0 => m_x,
				1 => m_y,
				2 => m_z,
				3 => m_w,
				_ => throw new ArgumentOutOfRangeException(nameof(index))
			};
		}


		/// <summary>
		/// Computes the squared length of the vector.
		/// </summary>
		public static Ratio Lenght ( Vec4r v ) => (v.m_x * v.m_x + v.m_y * v.m_y + v.m_z * v.m_z + v.m_w * v.m_w);


		/// <summary>
		/// Computes the dot product of two vectors.
		/// </summary>
		public static Ratio Dot ( Vec4r v1, Vec4r v2 ) {
			return (v1.m_x * v2.m_x + v1.m_y * v2.m_y + v1.m_z * v2.m_z + v1.m_w * v2.m_w);
		}


		/// <summary>
		/// Linearly interpolates between two vectors.
		/// </summary>
		public static Vec4r InterpolateCoords ( Vec4r v1, Vec4r v2, Ratio p ) {
			return v1 + p * (v2 - v1);
		}

		/// <summary>
		/// Linearly interpolates between two vectors.
		/// </summary>
		public static Vec4r InterpolateCoords ( Vec4r v1, Vec4r v2, int p ) {
			return v1 + p * (v2 - v1);
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

			return m_x.GetHashCode() ^ m_y.GetHashCode() ^ m_z.GetHashCode() ^ m_w.GetHashCode();

		}
		/// <summary>
		/// Compares this vector to another object.
		/// </summary>
		public int CompareTo ( object? obj ) {
			if ( (obj is Vec4r) ) {
				return (int)CompareTo((Vec4r)(obj));
			}
			throw new ArgumentException("Object is not a Vec4r object");
		}
		/// <summary>
		/// Compares two vectors lexicographically.
		/// </summary>
		public CompareResult CompareTo ( Vec4r a ) {
			CompareResult _ret = CompareResult.Equal;

			if ( this < a ) _ret = CompareResult.AIsSmallerB;
			else if ( this > a ) _ret = CompareResult.AIsLargerB;


			return _ret;
		}

		/// <summary>
		/// Determines whether this instance is equal to another
		/// <see cref="Half16"/> value, using the same semantics as the
		/// <see cref="operator ==(Vec4r,Vec4r)"/>.
		/// </summary>
		/// <param name="other">The value to compare with.</param>
		/// <returns>
		/// <c>true</c> if the values are equal; otherwise <c>false</c>.
		/// </returns>
		public bool Equals ( Vec4r other ) {
			return this.X == other.X && this.Y == other.Y && this.Z == other.Z && this.W == other.W;
		}

		/// <summary>
		/// Determines whether this instance is equal to another object.
		/// 
		/// The object is considered equal if it is a <see cref="Vec4r"/>
		/// and compares equal using <see cref="Equals(Vec4r)"/>.
		/// </summary>
		/// <param name="obj">The object to compare with.</param>
		/// <returns>
		/// <c>true</c> if <paramref name="obj"/> is a <see cref="Vec4r"/>
		/// and equal to this instance; otherwise <c>false</c>.
		/// </returns>
		public override bool Equals ( object? obj ) {
			if ( obj == null ) return false;
			return (obj is Vec4r) && Equals((Vec4r)obj);
		}

		int IComparable<Vec4r>.CompareTo ( Vec4r other ) {
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
			Cache m = new Cache(48);

			m.WriteRange(0, m_x.ToBytes());
			m.WriteRange(16, m_y.ToBytes());
			m.WriteRange(32, m_z.ToBytes());
			m.WriteRange(48, m_w.ToBytes());

			return m.ToArrayEx();
		}
		/// <summary>
		/// Adds two vectors component‑wise.
		/// </summary>
		public static Vec4r operator + ( Vec4r a, Vec4r b ) {
			return new Vec4r(a.m_x + b.m_x, a.m_y + b.m_y, a.m_z + b.m_z, a.m_w + b.m_w);
		}
		/// <summary>
		/// Subtracts two vectors component‑wise.
		/// </summary>
		public static Vec4r operator - ( Vec4r a, Vec4r b ) {
			return new Vec4r(a.m_x - b.m_x, a.m_y - b.m_y, a.m_z - b.m_z, a.m_w - b.m_w);
		}
		/// <summary>
		/// Divides two vectors component‑wise.
		/// </summary>
		public static Vec4r operator / ( Vec4r a, Vec4r b ) {
			return new Vec4r(a.m_x / b.m_x, a.m_y / b.m_y, a.m_z / b.m_z, a.m_w / b.m_w);
		}
		/// <summary>
		/// Multiplies two vectors component‑wise.
		/// </summary>
		public static Vec4r operator * ( Vec4r a, Vec4r b ) {
			return new Vec4r(a.m_x * b.m_x, a.m_y * b.m_y, a.m_z * b.m_z, a.m_w * b.m_w);
		}
		/// <summary>
		/// Adds a scalar to both components of the vector.
		/// </summary>
		public static Vec4r operator + ( Vec4r a, Ratio b ) {
			return new Vec4r(a.m_x + b, a.m_y + b, a.m_z + b, a.m_w + b);
		}
		/// <summary>
		/// Subtracts a scalar from both components of the vector.
		/// </summary>
		public static Vec4r operator - ( Vec4r a, Ratio b ) {
			return new Vec4r(a.m_x - b, a.m_y - b, a.m_z - b, a.m_w - b);
		}
		/// <summary>
		/// Divides both components of the vector by a scalar.
		/// </summary>
		public static Vec4r operator / ( Vec4r a, Ratio b ) {
			return new Vec4r(a.m_x / b, a.m_y / b, a.m_z / b, a.m_w / b);
		}
		// <summary>
		/// Multiplies both components of the vector by a scalar.
		/// </summary>
		public static Vec4r operator * ( Vec4r a, Ratio b ) {
			return new Vec4r(a.m_x * b, a.m_y * b, a.m_z * b, a.m_w * b);
		}
		/// <summary>
		/// Subtracts each component of the vector from a scalar.
		/// </summary>
		public static Vec4r operator - ( Ratio a, Vec4r b ) {
			return new Vec4r(a - b.m_x, a - b.m_y, a - b.m_z, a - b.m_w);
		}
		// <summary>
		/// Divides a scalar by each component of the vector.
		/// </summary>
		public static Vec4r operator / ( Ratio a, Vec4r b ) {
			return new Vec4r(a / b.m_x, a / b.m_y, a / b.m_z, a / b.m_w);
		}
		/// <summary>
		/// Multiplies a scalar with each component of the vector.
		/// </summary>
		public static Vec4r operator * ( Ratio a, Vec4r b ) {
			return new Vec4r(a * b.m_x, a * b.m_y, a * b.m_z, a * b.m_w);
		}
		/// <summary>
		/// Adds a scalar to each component of the vector.
		/// </summary>
		public static Vec4r operator + ( Ratio a, Vec4r b ) {
			return new Vec4r(a + b.m_x, a + b.m_y, a + b.m_z, a + b.m_w);
		}
		/// <summary>
		/// Determines whether two vectors are equal component‑wise.
		/// </summary>
		public static bool operator == ( Vec4r a, Vec4r b ) {
			return a.m_x == b.m_x && a.m_y == b.m_y && a.m_z == b.m_z && a.m_w == b.m_w;
		}
		/// <summary>
		/// Determines whether two vectors differ in any component.
		/// </summary>
		public static bool operator != ( Vec4r a, Vec4r b ) {
			return a.m_x != b.m_x && a.m_y != b.m_y && a.m_z != b.m_z && a.m_w != b.m_w;
		}
		/// <summary>
		/// Determines whether all components of <paramref name="a"/> are less than or equal to those of <paramref name="b"/>.
		/// </summary>
		public static bool operator <= ( Vec4r a, Vec4r b ) {
			return a.m_x <= b.m_x && a.m_y <= b.m_y && a.m_z <= b.m_z && a.m_w <= b.m_w;
		}
		/// <summary>
		/// Determines whether all components of <paramref name="a"/> are greater than or equal to those of <paramref name="b"/>.
		/// </summary>
		public static bool operator >= ( Vec4r a, Vec4r b ) {
			return a.m_x >= b.m_x && a.m_y >= b.m_y && a.m_z >= b.m_z && a.m_w >= b.m_w;
		}
		/// <summary>
		/// Determines whether all components of <paramref name="a"/> are strictly less than those of <paramref name="b"/>.
		/// </summary>
		public static bool operator < ( Vec4r a, Vec4r b ) {
			return a.m_x < b.m_x && a.m_y < b.m_y && a.m_z < b.m_z && a.m_w < b.m_w;
		}
		/// <summary>
		/// Determines whether all components of <paramref name="a"/> are strictly greater than those of <paramref name="b"/>.
		/// </summary>
		public static bool operator > ( Vec4r a, Vec4r b ) {
			return a.m_x > b.m_x && a.m_y > b.m_y && a.m_z > b.m_z && a.m_w > b.m_w;
		}

		/// <summary>
		/// Determines whether a scalar equals both components of the vector.
		/// </summary>
		public static bool operator == ( Ratio a, Vec4r b ) {
			return a == b.m_x && a == b.m_y && a == b.m_z && a == b.m_w;
		}

		/// <summary>
		/// Determines whether a scalar differs from any component of the vector.
		/// </summary>
		public static bool operator != ( Ratio a, Vec4r b ) {
			return a != b.m_x && a != b.m_y && a != b.m_z && a != b.m_w;
		}

		/// <summary>
		/// Determines whether a scalar is less than or equal to both vector components.
		/// </summary>
		public static bool operator <= ( Ratio a, Vec4r b ) {
			return a <= b.m_x && a <= b.m_y && a <= b.m_z && a <= b.m_w;
		}

		/// <summary>
		/// Determines whether a scalar is greater than or equal to both vector components.
		/// </summary>
		public static bool operator >= ( Ratio a, Vec4r b ) {
			return a >= b.m_x && a >= b.m_y && a >= b.m_z && a >= b.m_w;
		}

		/// <summary>
		/// Determines whether a scalar is strictly less than both vector components.
		/// </summary>
		public static bool operator < ( Ratio a, Vec4r b ) {
			return a < b.m_x && a < b.m_y && a < b.m_z && a < b.m_w;
		}

		/// <summary>
		/// Determines whether a scalar is strictly greater than both vector components.
		/// </summary>
		public static bool operator > ( Ratio a, Vec4r b ) {
			return a > b.m_x && a > b.m_y && a > b.m_z && a > b.m_w;
		}

		/// <summary>
		/// Determines whether both vector components equal the scalar.
		/// </summary>
		public static bool operator == ( Vec4r a, Ratio b ) {
			return a.m_x == b && a.m_y == b && a.m_z == b && a.m_w == b;
		}

		/// <summary>
		/// Determines whether any vector component differs from the scalar.
		/// </summary>
		public static bool operator != ( Vec4r a, Ratio b ) {
			return a.m_x != b && a.m_y != b && a.m_z != b && a.m_w != b;
		}

		/// <summary>
		/// Determines whether both vector components are less than or equal to the scalar.
		/// </summary>
		public static bool operator <= ( Vec4r a, Ratio b ) {
			return a.m_x <= b && a.m_y <= b && a.m_z <= b && a.m_w <= b;
		}

		/// <summary>
		/// Determines whether both vector components are greater than or equal to the scalar.
		/// </summary>
		public static bool operator >= ( Vec4r a, Ratio b ) {
			return a.m_x >= b && a.m_y >= b && a.m_z >= b && a.m_w >= b;
		}

		/// <summary>
		/// Determines whether both vector components are strictly less than the scalar.
		/// </summary>
		public static bool operator < ( Vec4r a, Ratio b ) {
			return a.m_x < b && a.m_y < b && a.m_z < b && a.m_w < b;
		}

		/// <summary>
		/// Determines whether both vector components are strictly greater than the scalar.
		/// </summary>
		public static bool operator > ( Vec4r a, Ratio b ) {
			return a.m_x > b && a.m_y > b && a.m_z > b && a.m_w > b;
		}
	}
	/// @}
}
