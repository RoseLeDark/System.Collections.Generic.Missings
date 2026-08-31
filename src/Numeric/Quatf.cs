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


using System.Diagnostics;
using System.Runtime.InteropServices;
using SystemEx.Collections.Generic;

using SystemEx.Hash;
using SystemEx.Utils;

namespace SystemEx.Numeric {
	/// \addtogroup Numeric
	/// @{


	/// <summary>
	/// Defines the comparison mode used by <see cref="Quatf"/>.
	/// </summary>
	public enum CompareType {
        /// <summary>
        /// Compares quaternions by their scalar component <c>w</c>.
        /// </summary>
        Skalar,

        /// <summary>
        /// Compares quaternions by their norm (length).
        /// </summary>
        Norm,

        /// <summary>
        /// Compares quaternions by their rotation angle.
        /// </summary>
        Rotation
    }
    /// <summary>
    /// Represents a floating‑point quaternion used for 3D rotations.
    ///
    /// <para>
    /// <see cref="Quatf"/> stores a scalar component <c>w</c> and a vector
    /// component <c>(x,y,z)</c> in a sequential memory layout, making it suitable
    /// for native interop, compute kernels, and deterministic hashing.
    /// </para>
    ///
    /// <para>
    /// The struct is annotated with <see cref="HashAlgorithmAttribute"/> to enable
    /// attribute‑driven hashing via <see cref="HashFactory"/>.  
    /// BernsteinHash is used because it is fast, byte‑linear, and ideal for small
    /// fixed‑size numeric types.
    /// </para>
    ///
    /// <para>
    /// <see cref="Quatf"/> implements multiple comparison and hashing interfaces:
    /// <list type="bullet">
    /// <item><description><see cref="IComparable"/> and <see cref="IComparable{T}"/> for ordering</description></item>
    /// <item><description><see cref="IEquatable{T}"/> for equality checks</description></item>
    /// <item><description><see cref="IHashable{T}"/> for deterministic byte‑level hashing</description></item>
    /// </list>
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [HashAlgorithm(typeof(BernsteinHash), Endian.System)]
    public struct Quatf : IComparable, IComparableEx<Quatf>, IComparable<Quatf>, IEquatable<Quatf>, IHashable<Quatf> {
        /// <summary>
        /// The vector part of the quaternion (x,y,z).
        /// </summary>
        private Vec3f m_v;

        /// <summary>
        /// The scalar part of the quaternion (w).
        /// </summary>
        private float m_s;

        /// <summary>
        /// Gets or sets the scalar component (w) of the quaternion.
        /// </summary>
        public float S { get => m_s; set => m_s = value; }

        /// <summary>
        /// Gets or sets the vector component (x,y,z) of the quaternion.
        /// </summary>
        public Vec3f V { get => m_v; set => m_v = value; }

        /// <summary>
        /// Gets or sets the X component of the quaternion's vector part.
        /// </summary>
        public float X { get => m_v.X; set => m_v.X = value; }

        /// <summary>
        /// Gets or sets the Y component of the quaternion's vector part.
        /// </summary>
        public float Y { get => m_v.Y; set => m_v.Y = value; }

        /// <summary>
        /// Gets or sets the Z component of the quaternion's vector part.
        /// </summary>
        public float Z { get => m_v.Z; set => m_v.Z = value; }

        /// <summary>
        /// Gets the axis‑angle representation of this quaternion.
        /// </summary>
        public AxisAngle<Vec3f, float> AxisAngle => ToAxisAngle(this);

        /// <summary>
        /// The comparison mode used by <see cref="CompareTo(Quatf)"/>.
        /// </summary>
        private CompareType m_comparebleMode;

        /// <summary>
        /// Gets or sets the comparison mode used by <see cref="CompareTo(Quatf)"/>.
        /// </summary>
        public CompareType CompareMode { get => m_comparebleMode; set => m_comparebleMode = value; }

        /// <summary>
        /// Initializes the identity quaternion (1,0,0,0).
        /// </summary>
        public Quatf () {
            m_v = new Vec3f(0);
            m_s = 1.0f;
            m_comparebleMode = CompareType.Skalar;
        }
        /// <summary>
        /// Gets a component by index .
        /// </summary>
        public float Get ( int index ) {
            return index switch
            {
                0 => S,
                1 => X,
                2 => Y,
                3 => Z,
                _ => throw new ArgumentOutOfRangeException(nameof(index))
            };
        }

        /// <summary>
        /// Initializes a quaternion from Euler angles (XYZ order).
        /// </summary>
        public Quatf ( Vec3f angles ) {
            float cos_z_2 = System.MathF.Cos(0.5f*angles.Z);
            float cos_y_2 = System.MathF.Cos(0.5f*angles.Y);
            float cos_x_2 = System.MathF.Cos(0.5f*angles.X);

            float sin_z_2 = System.MathF.Sin(0.5f*angles.Z);
            float sin_y_2 = System.MathF.Sin(0.5f*angles.Y);
            float sin_x_2 = System.MathF.Sin(0.5f*angles.X);

            // and now compute quaternion
            m_s = cos_z_2 * cos_y_2 * cos_x_2 + sin_z_2 * sin_y_2 * sin_x_2;
            m_v = new Vec3f
            (
                cos_z_2 * cos_y_2 * sin_x_2 - sin_z_2 * sin_y_2 * cos_x_2,
                cos_z_2 * sin_y_2 * cos_x_2 + sin_z_2 * cos_y_2 * sin_x_2,
                sin_z_2 * cos_y_2 * cos_x_2 - cos_z_2 * sin_y_2 * sin_x_2
            );
            m_comparebleMode = CompareType.Skalar;
        }
        /// <summary>
        /// Initializes a quaternion from explicit components.
        /// </summary>
        public Quatf ( float fs, float fx, float fy, float fz ) {
            m_s = fs;
            m_v = new Vec3f(fx, fy, fz);
            m_comparebleMode = CompareType.Skalar;
        }

        /// <summary>
        /// Initializes a quaternion from a float array.
        /// </summary>
        public Quatf ( float[] pfs ) {
            m_s = pfs[0];
            m_v = new Vec3f(pfs[1], pfs[2], pfs[3]);
            m_comparebleMode = CompareType.Skalar;
        }

        /// <summary>
        /// Initializes a quaternion from an axis‑angle representation.
        /// </summary>
        public Quatf ( AxisAngle<Vec3f, float> axisAngle ) {
            var tmp = FromAxis(axisAngle);
            m_s = tmp.m_s;
            m_v = tmp.m_v;
            m_comparebleMode = CompareType.Skalar;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        public Quatf (Quatf other) {
            m_s = other.m_s;
            m_v = other.m_v;
            m_comparebleMode = CompareType.Skalar;
        }

        /// <summary>
        /// Computes the normalized rotation axis of this quaternion.
        /// </summary>
        public Vec3f GetRotationAxis () {
            float sinThetaOver2Sq = 1.0f - m_s * m_s;

            if ( sinThetaOver2Sq <= 0.0f ) {
                return new  Vec3f(1.0f, 0.0f, 0.0f);
            }
            float   oneOverSinThetaOver2 = 1.0f / System.MathF.Sqrt(sinThetaOver2Sq);

            return Vec3f.Normalize(new Vec3f(
                m_v.X * oneOverSinThetaOver2,
                m_v.Y * oneOverSinThetaOver2,
                m_v.Z * oneOverSinThetaOver2
             ));

        }
        /// <summary>
        /// Converts a quaternion into an axis‑angle representation.
        /// </summary>
        public static AxisAngle<Vec3f, float> ToAxisAngle ( Quatf q ) {
            AxisAngle<Vec3f, float> _ret = new AxisAngle<Vec3f, float>();

            _ret.Angle = q.GetRotationAngle();
            _ret.Axis = q.GetRotationAxis();

            return _ret;
        }
        /// <summary>
        /// Creates a quaternion from an axis‑angle representation.
        /// </summary>
        public static Quatf FromAxis ( AxisAngle<Vec3f, float> axisAngle ) {

            float len = Vec3f.LenghtSqrt(axisAngle.Axis);
            Quatf temp = new Quatf();

            if ( len > 1e-12f ) {
                float inv = 1.0f / len;

                // Achse normalisieren
                float nx = axisAngle.Axis.X * inv;
                float ny = axisAngle.Axis.Y * inv;
                float nz = axisAngle.Axis.Z * inv;

                float omega = 0.5f * axisAngle.Angle;
                float s = MathF.Sin(omega);

                temp.m_v = new Vec3f(s * nx, s * ny, s * nz);
                temp.m_s = MathF.Cos(omega);
            } else {
                // Ungültige Achse → Identity
                temp.m_s = 1.0f;
                temp.m_v = Vec3f.Zero;
            }

            return Normalize(temp);

        }
        /// <summary>
        /// Returns the rotation angle represented by this quaternion.
        /// </summary>
        public float GetRotationAngle() {
            float s = System.Math.Clamp(m_s, -1.0f, 1.0f);
            float thetaOver2 = System.MathF.Acos(s);
            return thetaOver2 * 2.0f;
        }
        /// <summary>
        /// Determines whether a quaternion is the identity rotation.
        /// </summary>
        public static bool IsIdentity ( Quatf v ) {
            return v.m_v == Vec3f.Zero && MathF.Abs(v.m_s - 1.0f) <= 0.00001f;
        }
        /// <summary>
        /// Computes the quaternion exponential.
        /// </summary>
        public static Quatf Exponent ( Quatf v ) {
            float Mul;

            Quatf temp = new Quatf( v);
            float Length = Vec3f.LenghtSqrt(temp.m_v);

            if ( Length > 1.0e-4f )
                Mul = System.MathF.Sin(Length) / Length;
            else
                Mul = 1.0f;

            temp.m_s = System.MathF.Cos(Length);

            temp.m_v.X *= Mul;
            temp.m_v.Y *= Mul;
            temp.m_v.Z *= Mul;

            return temp;
        }

        /// <summary>
        /// Computes the quaternion logarithm.
        /// </summary>
        public static Quatf Logarithm ( Quatf v ) {

            Quatf temp = new Quatf( v);

            float vLen = Vec3f.LenghtSqrt(temp.m_v);
            float angle = MathF.Atan2(vLen, temp.m_s);

            temp.m_s = 0.0f;

            if ( vLen > 1e-6f ) {
                float scale = angle / vLen;
                temp.m_v.X *= scale;
                temp.m_v.Y *= scale;
                temp.m_v.Z *= scale;
            } else {
                temp.m_v = Vec3f.Zero;
            }

            return temp;
        }
        /// <summary>
        /// Computes the Euclidean length of the quaternion.
        /// </summary>
        public static float LenghtSqrt ( Quatf v ) {
            return System.MathF.Sqrt(v.m_s * 
                                    v.m_s + v.m_v.X * 
                                    v.m_v.X + v.m_v.Y * 
                                    v.m_v.Y + v.m_v.Z * 
                                    v.m_v.Z);
        }

        /// <summary>
        /// Computes the squared length of the quaternion.
        /// </summary>
        public static float Lenght ( Quatf v ) {
            return (v.m_s * v.m_s + 
                v.m_v.X * v.m_v.X + 
                v.m_v.Y * v.m_v.Y +
                v.m_v.Z * v.m_v.Z);
        }
        /// <summary>
        /// Raises a quaternion to a scalar power.
        /// </summary>
        public static Quatf Power ( Quatf v, float Exp ) {
            if ( System.MathF.Abs(v.m_s) > .9999f ) {
                return v;
            }

            // Erhalte halbe angle alpha (alpha = theta/2)
            float   alpha = System.MathF.Acos(v.m_s);

            // Berechne neuen alpha Wert
            float   newAlpha = alpha * Exp;

            // Berechne neuen s wert
            Quatf result = new Quatf();
            result.m_s = System.MathF.Cos(newAlpha);

            // Berechne neue xyz Werte

            float   mult = System.MathF.Sin(newAlpha) / System.MathF.Sin(alpha);
            result.m_v.X = v.m_v.X * mult;
            result.m_v.Y = v.m_v.Y * mult;
            result.m_v.Z = v.m_v.Z * mult;

            return Normalize(result);
        }

        /// <summary>
        /// Computes the DotProduct product of two quaternions.
        /// </summary>
        public static float DotProduct ( Quatf a, Quatf b ) {
            return a.m_s * b.m_s +
                   a.m_v.X * b.m_v.X +
                   a.m_v.Y * b.m_v.Y +
                   a.m_v.Z * b.m_v.Z;
        }

        /// <summary>
        /// Computes the inverse of a quaternion.
        /// </summary>
        public static Quatf Invert ( Quatf q ) {
            float temp = Lenght(q);
            Quatf tq = new Quatf(q);

            tq.m_s /= temp;
            tq.m_v.X /= -temp;
            tq.m_v.Y /= -temp;
            tq.m_v.Z /= -temp;
            return tq;       // Okay, same norm.
        }
        /// <summary>
        /// Normalizes a quaternion to unit length.
        /// </summary>
        public static Quatf Normalize ( Quatf v ) {
            float norme = LenghtSqrt(v);
            Quatf temp = new Quatf(v);

            if ( norme < 1e-12f ) {
                temp.m_s = 1.0f;
                temp.m_v = new Vec3f(0,0,0);
            } else {
                float recip = 1.0f/norme;

                temp.m_s *= recip;
                temp.m_v.X *= recip;
                temp.m_v.Y *= recip;
                temp.m_v.Z *= recip;
            }
            return temp;
        }


        //public raMatrix     ToMatrix (Quatf v);
        /// <summary>
        /// Computes the quaternion conjugate.
        /// </summary>
        public static Quatf Conjugate ( Quatf v ) {
            Quatf temp = new Quatf( v);

            temp.m_v.X = -temp.m_v.X;
            temp.m_v.Y = -temp.m_v.Y;
            temp.m_v.Z = -temp.m_v.Z;

            return temp;
        }

        /// <summary>
        /// Performs spherical linear interpolation between two quaternions.
        /// </summary>
        public static Quatf Slerp ( Quatf q0, Quatf q1, float t ) { // Spherical linear interpolation. 
            if ( t <= 0.0f ) return q0;
            if ( t >= 1.0f ) return q1;

            float cosOmega = DotProduct(q0, q1);

            float q1w = q1.m_s;
            float q1x = q1.m_v.X;
            float q1y = q1.m_v.Y;
            float q1z = q1.m_v.Z;

            if ( cosOmega < 0.0f ) {
                q1w = -q1w;
                q1x = -q1x;
                q1y = -q1y;
                q1z = -q1z;
                cosOmega = -cosOmega;
            }

            Debug.Assert(cosOmega < 1.0f);

            // Compute interpolation fraction, checking for quaternions
            // almost exactly the same

            float k0, k1;
            if ( cosOmega > 0.9999f ) {
                k0 = 1.0f - t;
                k1 = t;
            } else {
                // Compute the sin of the angle using the
                // trig identity sin^2(omega) + cos^2(omega) = 1

                float sinOmega = System.MathF.Sqrt(1.0f - cosOmega*cosOmega);

                // Compute the angle from its sin and cosine

                float omega = System.MathF.Atan2(sinOmega, cosOmega);

                // Compute inverse of denominator, so we only have
                // to divide once

                float oneOverSinOmega = 1.0f / sinOmega;

                // Compute interpolation parameters

                k0 = System.MathF.Sin((1.0f - t) * omega) * oneOverSinOmega;
                k1 = System.MathF.Sin(t * omega) * oneOverSinOmega;
            }

            // Interpolate

            Quatf result = new Quatf();

            result.m_v.X = k0 * q0.m_v.X + k1 * q1x;
            result.m_v.Y = k0 * q0.m_v.Y + k1 * q1y;
            result.m_v.Z = k0 * q0.m_v.Z + k1 * q1z;
            result.m_s = k0 * q0.m_s + k1 * q1w;

            // Return it

            return result;
        }
        /// <summary>
        /// Compares two quaternions using the configured <see cref="CompareMode"/>.
        /// </summary>
        public CompareResult CompareTo ( Quatf a ) {

            CompareResult _ret = CompareResult.Equal;

            float _a = 0;
            float _b = 0;

            switch ( m_comparebleMode) {
            case CompareType.Skalar:
                _ret = (CompareResult)m_s.CompareTo(a.m_s);
                break;
            case CompareType.Norm:
                _a = m_s * m_s + Vec3f.Lenght(m_v);
                _b = a.m_s * a.m_s + Vec3f.Lenght(a.m_v);
                _ret = (CompareResult)_a.CompareTo(_b);
                break;
            case CompareType.Rotation:
                _a = GetRotationAngle(); 
                _b = a.GetRotationAngle();
                _ret = (CompareResult)_a.CompareTo(_b);
                break;
            }
            return _ret;
        }

        int IComparable<Quatf>.CompareTo ( Quatf other ) {
            return (int)CompareTo(other);
        }

        /// <summary>
        /// Compares this quaternion to another object.
        /// </summary>
        public int CompareTo ( object? obj ) {
            if ( (obj is Quatf) ) {
                return (int)CompareTo((Quatf)(obj));
            }
            throw new ArgumentException("Object is not a Quatf object");
        }
        public bool Equals ( Quatf other ) {
            return m_s == other.m_s && m_v == other.m_v;
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
            Cache m = new Cache(sizeof(float) * 4);

            for ( byte i = 0 ; i < 4 ; i++ )
                m.WriteRange((ulong)(sizeof(float) * i), Get(i).ToBytes());

            return m.ToArrayEx();
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

            return m_v.GetHashCode() ^ m_s.GetHashCode() ;

        }

        
        /// <summary>
        /// Determines whether this instance is equal to another object.
        /// 
        /// The object is considered equal if it is a <see cref="Quatf"/>
        /// and compares equal using <see cref="Equals(Quatf)"/>.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is a <see cref="Quatf"/>
        /// and equal to this instance; otherwise <c>false</c>.
        /// </returns>
        public override bool Equals ( object? obj ) {
            if ( obj == null ) return false;
            return (obj is Quatf) && Equals((Quatf)obj);
        }
        /// <summary>
        /// Multiplies two Quatf component‑wise.
        /// </summary>
        public static Quatf operator * ( Quatf a, Quatf b ) {
            Quatf qtmp = new Quatf();

            float ax = a.m_v.X;
            float ay = a.m_v.Y;
            float az = a.m_v.Z;

            float bx = b.m_v.X;
            float by = b.m_v.Y;
            float bz = b.m_v.Z;

            float as_ = a.m_s;
            float bs_ = b.m_s;

            // Skalarteil
            qtmp.m_s = as_ * bs_ - ax * bx - ay * by - az * bz;

            // Vektorteil
            qtmp.m_v = new Vec3f(
                as_ * bx + bs_ * ax + ay * bz - az * by,
                as_ * by + bs_ * ay + az * bx - ax * bz,
                as_ * bz + bs_ * az + ax * by - ay * bx
            );

            return qtmp;
        }
        /// <summary>
        /// Multiplicate a float with a quaternion
        /// </summary>
        public static Quatf operator * (float a, Quatf b) {
            Quatf _ret = new Quatf();

            _ret.m_s = a * b.m_s;
            _ret.m_v = a * b.m_v;

            return _ret;
        }
        /// <summary>
        /// Multiplicate a quaternion with a float
        /// </summary>
        public static Quatf operator * ( Quatf a, float b ) {
            Quatf _ret = new Quatf();

            _ret.m_s = a.m_s * b;
            _ret.m_v = a.m_v * b;

            return _ret;
        }
        /// <summary>
        /// Adds two Quatf component‑wise.
        /// </summary>
        public static Quatf operator + ( Quatf a, Quatf b ) {
            return new Quatf(
                a.m_s + b.m_s,
                a.m_v.X + b.m_v.X,
                a.m_v.Y + b.m_v.Y,
                a.m_v.Z + b.m_v.Z
            );
        }
        /// <summary>
        /// Subtracts two Quatf component‑wise.
        /// </summary>
        public static Quatf operator - ( Quatf a, Quatf b ) {
            return new Quatf(
                a.m_s - b.m_s,
                a.m_v.X - b.m_v.X,
                a.m_v.Y - b.m_v.Y,
                a.m_v.Z - b.m_v.Z
            );
        }
        /// <summary>
        /// Invert 
        /// </summary>
        public static Quatf operator - ( Quatf a ) {
            return new Quatf(
                -a.m_s,
                -a.m_v.X,
                -a.m_v.Y,
                -a.m_v.Z
            );
        }
        /// <summary>
        /// Divides two Quatf component‑wise.
        /// </summary>
        public static Quatf operator / ( Quatf a, Quatf b ) {
            return a * Invert(b);
        }
        /// <summary>
        /// Divides both components of the Quatf by a float.
        /// </summary>
        public static Quatf operator / ( Quatf a, float b ) {
            Quatf _ret = new Quatf();

            _ret.m_s = a.m_s / b;
            _ret.m_v = a.m_v / b;

            return _ret;
        }
       
    }
    
}
