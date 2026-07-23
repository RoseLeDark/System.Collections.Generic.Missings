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
    /// Represents a doubleing‑point quaternion used for 3D rotations.
    ///
    /// <para>
    /// <see cref="Quatd"/> stores a scalar component <c>w</c> and a vector
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
    /// <see cref="Quatd"/> implements multiple comparison and hashing interfaces:
    /// <list type="bullet">
    /// <item><description><see cref="IComparable"/> and <see cref="IComparable{T}"/> for ordering</description></item>
    /// <item><description><see cref="IEquatable{T}"/> for equality checks</description></item>
    /// <item><description><see cref="IHashable{T}"/> for deterministic byte‑level hashing</description></item>
    /// </list>
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [HashAlgorithm(typeof(BernsteinHash), Endian.System)]
    public struct Quatd : IComparable, IComparableEx<Quatd>, IComparable<Quatd>, IEquatable<Quatd>, IHashable<Quatd> {
        /// <summary>
        /// The vector part of the quaternion (x,y,z).
        /// </summary>
        private Vec3d m_v;

        /// <summary>
        /// The scalar part of the quaternion (w).
        /// </summary>
        private double m_s;

        /// <summary>
        /// Gets or sets the scalar component (w) of the quaternion.
        /// </summary>
        public double S { get => m_s; set => m_s = value; }

        /// <summary>
        /// Gets or sets the vector component (x,y,z) of the quaternion.
        /// </summary>
        public Vec3d V { get => m_v; set => m_v = value; }

        /// <summary>
        /// Gets or sets the X component of the quaternion's vector part.
        /// </summary>
        public double X { get => m_v.X; set => m_v.X = value; }

        /// <summary>
        /// Gets or sets the Y component of the quaternion's vector part.
        /// </summary>
        public double Y { get => m_v.Y; set => m_v.Y = value; }

        /// <summary>
        /// Gets or sets the Z component of the quaternion's vector part.
        /// </summary>
        public double Z { get => m_v.Z; set => m_v.Z = value; }

        /// <summary>
        /// Gets the axis‑angle representation of this quaternion.
        /// </summary>
        public AxisAngle<Vec3d, double> AxisAngle => ToAxisAngle(this);

        /// <summary>
        /// The comparison mode used by <see cref="CompareTo(Quatd)"/>.
        /// </summary>
        private CompareType m_comparebleMode;

        /// <summary>
        /// Gets or sets the comparison mode used by <see cref="CompareTo(Quatd)"/>.
        /// </summary>
        public CompareType CompareMode { get => m_comparebleMode; set => m_comparebleMode = value; }

        /// <summary>
        /// Initializes the identity quaternion (1,0,0,0).
        /// </summary>
        public Quatd () {
            m_v = new Vec3d(0);
            m_s = 1.0;
            m_comparebleMode = CompareType.Skalar;
        }
        /// <summary>
        /// Initializes a quaternion from Euler angles (XYZ order).
        /// </summary>
        public Quatd ( Vec3d angles ) {
            double cos_z_2 = System.Math.Cos(0.5*angles.Z);
            double cos_y_2 = System.Math.Cos(0.5*angles.Y);
            double cos_x_2 = System.Math.Cos(0.5*angles.X);

            double sin_z_2 = System.Math.Sin(0.5*angles.Z);
            double sin_y_2 = System.Math.Sin(0.5*angles.Y);
            double sin_x_2 = System.Math.Sin(0.5*angles.X);

            // and now compute quaternion
            m_s = cos_z_2 * cos_y_2 * cos_x_2 + sin_z_2 * sin_y_2 * sin_x_2;
            m_v = new Vec3d
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
        public Quatd ( double fs, double fx, double fy, double fz ) {
            m_s = fs;
            m_v = new Vec3d(fx, fy, fz);
            m_comparebleMode = CompareType.Skalar;
        }

        /// <summary>
        /// Initializes a quaternion from a double array.
        /// </summary>
        public Quatd ( double[] pfs ) {
            m_s = pfs[0];
            m_v = new Vec3d(pfs[1], pfs[2], pfs[3]);
            m_comparebleMode = CompareType.Skalar;
        }

        /// <summary>
        /// Initializes a quaternion from an axis‑angle representation.
        /// </summary>
        public Quatd ( AxisAngle<Vec3d, double> axisAngle ) {
            var tmp = FromAxis(axisAngle);
            m_s = tmp.m_s;
            m_v = tmp.m_v;
            m_comparebleMode = CompareType.Skalar;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        public Quatd ( Quatd other ) {
            m_s = other.m_s;
            m_v = other.m_v;
            m_comparebleMode = CompareType.Skalar;
        }

        /// <summary>
        /// Gets a component by index .
        /// </summary>
        public double Get ( int index ) {
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
        /// Computes the normalized rotation axis of this quaternion.
        /// </summary>
        public Vec3d GetRotationAxis () {
            double sinThetaOver2Sq = 1.0 - m_s * m_s;

            if ( sinThetaOver2Sq <= 0.0 ) {
                return new Vec3d(1.0, 0.0, 0.0);
            }
            double   oneOverSinThetaOver2 = 1.0 / System.Math.Sqrt(sinThetaOver2Sq);

            return Vec3d.Normalize(new Vec3d(
                m_v.X * oneOverSinThetaOver2,
                m_v.Y * oneOverSinThetaOver2,
                m_v.Z * oneOverSinThetaOver2
             ));

        }
        /// <summary>
        /// Converts a quaternion into an axis‑angle representation.
        /// </summary>
        public static AxisAngle<Vec3d, double> ToAxisAngle ( Quatd q ) {
            AxisAngle<Vec3d, double> _ret = new AxisAngle<Vec3d, double>();

            _ret.Angle = q.GetRotationAngle();
            _ret.Axis = q.GetRotationAxis();

            return _ret;
        }
        /// <summary>
        /// Creates a quaternion from an axis‑angle representation.
        /// </summary>
        public static Quatd FromAxis ( AxisAngle<Vec3d, double> axisAngle ) {

            double len = Vec3d.LenghtSqrt(axisAngle.Axis);
            Quatd temp = new Quatd();

            if ( len > 1e-12 ) {
                double inv = 1.0 / len;

                // Achse normalisieren
                double nx = axisAngle.Axis.X * inv;
                double ny = axisAngle.Axis.Y * inv;
                double nz = axisAngle.Axis.Z * inv;

                double omega = 0.5f * axisAngle.Angle;
                double s = System.Math.Sin(omega);

                temp.m_v = new Vec3d(s * nx, s * ny, s * nz);
                temp.m_s = System.Math.Cos(omega);
            } else {
                // Ungültige Achse → Identity
                temp.m_s = 1.0;
                temp.m_v = Vec3d.Zero;
            }

            return Normalize(temp);

        }
        /// <summary>
        /// Returns the rotation angle represented by this quaternion.
        /// </summary>
        public double GetRotationAngle () {
            double s = System.Math.Clamp(m_s, -1.0, 1.0);
            double thetaOver2 = System.Math.Acos(s);
            return thetaOver2 * 2.0;
        }
        /// <summary>
        /// Determines whether a quaternion is the identity rotation.
        /// </summary>
        public static bool IsIdentity ( Quatd v ) {
            return v.m_v == Vec3d.Zero && System.Math.Abs(v.m_s - 1.0) <= 0.00001f;
        }
        /// <summary>
        /// Computes the quaternion exponential.
        /// </summary>
        public static Quatd Exponent ( Quatd v ) {
            double Mul;

            Quatd temp = new Quatd( v);
            double Length = Vec3d.LenghtSqrt(temp.m_v);

            if ( Length > 1.0e-4f )
                Mul = System.Math.Sin(Length) / Length;
            else
                Mul = 1.0;

            temp.m_s = System.Math.Cos(Length);

            temp.m_v.X *= Mul;
            temp.m_v.Y *= Mul;
            temp.m_v.Z *= Mul;

            return temp;
        }

        /// <summary>
        /// Computes the quaternion logarithm.
        /// </summary>
        public static Quatd Logarithm ( Quatd v ) {

            Quatd temp = new Quatd( v);

            double vLen = Vec3d.LenghtSqrt(temp.m_v);
            double angle = System.Math.Atan2(vLen, temp.m_s);

            temp.m_s = 0.0;

            if ( vLen > 1e-6 ) {
                double scale = angle / vLen;
                temp.m_v.X *= scale;
                temp.m_v.Y *= scale;
                temp.m_v.Z *= scale;
            } else {
                temp.m_v = Vec3d.Zero;
            }

            return temp;
        }
        /// <summary>
        /// Computes the Euclidean length of the quaternion.
        /// </summary>
        public static double LenghtSqrt ( Quatd v ) {
            return System.Math.Sqrt(v.m_s *
                                    v.m_s + v.m_v.X *
                                    v.m_v.X + v.m_v.Y *
                                    v.m_v.Y + v.m_v.Z *
                                    v.m_v.Z);
        }

        /// <summary>
        /// Computes the squared length of the quaternion.
        /// </summary>
        public static double Lenght ( Quatd v ) {
            return (v.m_s * v.m_s +
                v.m_v.X * v.m_v.X +
                v.m_v.Y * v.m_v.Y +
                v.m_v.Z * v.m_v.Z);
        }
        /// <summary>
        /// Raises a quaternion to a scalar power.
        /// </summary>
        public static Quatd Power ( Quatd v, double Exp ) {
            if ( System.Math.Abs(v.m_s) > .9999f ) {
                return v;
            }

            // Erhalte halbe angle alpha (alpha = theta/2)
            double   alpha = System.Math.Acos(v.m_s);

            // Berechne neuen alpha Wert
            double   newAlpha = alpha * Exp;

            // Berechne neuen s wert
            Quatd result = new Quatd();
            result.m_s = System.Math.Cos(newAlpha);

            // Berechne neue xyz Werte

            double   mult = System.Math.Sin(newAlpha) / System.Math.Sin(alpha);
            result.m_v.X = v.m_v.X * mult;
            result.m_v.Y = v.m_v.Y * mult;
            result.m_v.Z = v.m_v.Z * mult;

            return Normalize(result);
        }

        /// <summary>
        /// Computes the DotProduct product of two quaternions.
        /// </summary>
        public static double DotProduct ( Quatd a, Quatd b ) {
            return a.m_s * b.m_s +
                   a.m_v.X * b.m_v.X +
                   a.m_v.Y * b.m_v.Y +
                   a.m_v.Z * b.m_v.Z;
        }

        /// <summary>
        /// Computes the inverse of a quaternion.
        /// </summary>
        public static Quatd Invert ( Quatd q ) {
            double temp = Lenght(q);
            Quatd tq = new Quatd(q);

            tq.m_s /= temp;
            tq.m_v.X /= -temp;
            tq.m_v.Y /= -temp;
            tq.m_v.Z /= -temp;
            return tq;       // Okay, same norm.
        }
        /// <summary>
        /// Normalizes a quaternion to unit length.
        /// </summary>
        public static Quatd Normalize ( Quatd v ) {
            double norme = LenghtSqrt(v);
            Quatd temp = new Quatd(v);

            if ( norme < 1e-12f ) {
                temp.m_s = 1.0;
                temp.m_v = new Vec3d(0, 0, 0);
            } else {
                double recip = 1.0/norme;

                temp.m_s *= recip;
                temp.m_v.X *= recip;
                temp.m_v.Y *= recip;
                temp.m_v.Z *= recip;
            }
            return temp;
        }


        //public raMatrix     ToMatrix (Quatd v);
        /// <summary>
        /// Computes the quaternion conjugate.
        /// </summary>
        public static Quatd Conjugate ( Quatd v ) {
            Quatd temp = new Quatd( v);

            temp.m_v.X = -temp.m_v.X;
            temp.m_v.Y = -temp.m_v.Y;
            temp.m_v.Z = -temp.m_v.Z;

            return temp;
        }

        /// <summary>
        /// Performs spherical linear interpolation between two quaternions.
        /// </summary>
        public static Quatd Slerp ( Quatd q0, Quatd q1, double t ) { // Spherical linear interpolation. 
            if ( t <= 0.0 ) return q0;
            if ( t >= 1.0 ) return q1;

            double cosOmega = DotProduct(q0, q1);

            double q1w = q1.m_s;
            double q1x = q1.m_v.X;
            double q1y = q1.m_v.Y;
            double q1z = q1.m_v.Z;

            if ( cosOmega < 0.0 ) {
                q1w = -q1w;
                q1x = -q1x;
                q1y = -q1y;
                q1z = -q1z;
                cosOmega = -cosOmega;
            }

            Debug.Assert(cosOmega < 1.0);

            // Compute interpolation fraction, checking for quaternions
            // almost exactly the same

            double k0, k1;
            if ( cosOmega > 0.9999 ) {
                k0 = 1.0 - t;
                k1 = t;
            } else {
                // Compute the sin of the angle using the
                // trig identity sin^2(omega) + cos^2(omega) = 1

                double sinOmega = System.Math.Sqrt(1.0 - cosOmega*cosOmega);

                // Compute the angle from its sin and cosine

                double omega = System.Math.Atan2(sinOmega, cosOmega);

                // Compute inverse of denominator, so we only have
                // to divide once

                double oneOverSinOmega = 1.0 / sinOmega;

                // Compute interpolation parameters

                k0 = System.Math.Sin((1.0 - t) * omega) * oneOverSinOmega;
                k1 = System.Math.Sin(t * omega) * oneOverSinOmega;
            }

            // Interpolate

            Quatd result = new Quatd();

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
        public CompareResult CompareTo ( Quatd a ) {

            CompareResult _ret = CompareResult.Equal;

            double _a = 0;
            double _b = 0;

            switch ( m_comparebleMode ) {
            case CompareType.Skalar:
            _ret = (CompareResult)m_s.CompareTo(a.m_s);
            break;
            case CompareType.Norm:
            _a = m_s * m_s + Vec3d.Lenght(m_v);
            _b = a.m_s * a.m_s + Vec3d.Lenght(a.m_v);
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

        int IComparable<Quatd>.CompareTo ( Quatd other ) {
            return (int)CompareTo(other);
        }

        /// <summary>
        /// Compares this quaternion to another object.
        /// </summary>
        public int CompareTo ( object? obj ) {
            if ( (obj is Quatd) ) {
                return (int)CompareTo((Quatd)(obj));
            }
            throw new ArgumentException("Object is not a Quatd object");
        }

        /// <inheritdoc/>
        public bool Equals ( Quatd other ) {
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
            Cache m = new Cache(sizeof(double) * 4);

            for ( byte i = 0 ; i < 4 ; i++ )
                m.WriteRange((ulong)(sizeof(double) * i), Get(i).ToBytes());

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

            return m_v.GetHashCode() ^ m_s.GetHashCode();

        }


        /// <summary>
        /// Determines whether this instance is equal to another object.
        /// 
        /// The object is considered equal if it is a <see cref="Quatd"/>
        /// and compares equal using <see cref="Equals(Quatd)"/>.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is a <see cref="Quatd"/>
        /// and equal to this instance; otherwise <c>false</c>.
        /// </returns>
        public override bool Equals ( object? obj ) {
            if ( obj == null ) return false;
            return (obj is Quatd) && Equals((Quatd)obj);
        }
        /// <summary>
        /// Multiplies two Quatd component‑wise.
        /// </summary>
        public static Quatd operator * ( Quatd a, Quatd b ) {
            Quatd qtmp = new Quatd();

            double ax = a.m_v.X;
            double ay = a.m_v.Y;
            double az = a.m_v.Z;

            double bx = b.m_v.X;
            double by = b.m_v.Y;
            double bz = b.m_v.Z;

            double as_ = a.m_s;
            double bs_ = b.m_s;

            // Skalarteil
            qtmp.m_s = as_ * bs_ - ax * bx - ay * by - az * bz;

            // Vektorteil
            qtmp.m_v = new Vec3d(
                as_ * bx + bs_ * ax + ay * bz - az * by,
                as_ * by + bs_ * ay + az * bx - ax * bz,
                as_ * bz + bs_ * az + ax * by - ay * bx
            );

            return qtmp;
        }
        /// <summary>
        /// Multiplicate a double with a quaternion
        /// </summary>
        public static Quatd operator * ( double a, Quatd b ) {
            Quatd _ret = new Quatd();

            _ret.m_s = a * b.m_s;
            _ret.m_v = a * b.m_v;

            return _ret;
        }
        /// <summary>
        /// Multiplicate a quaternion with a double
        /// </summary>
        public static Quatd operator * ( Quatd a, double b ) {
            Quatd _ret = new Quatd();

            _ret.m_s = a.m_s * b;
            _ret.m_v = a.m_v * b;

            return _ret;
        }
        /// <summary>
        /// Adds two Quatd component‑wise.
        /// </summary>
        public static Quatd operator + ( Quatd a, Quatd b ) {
            return new Quatd(
                a.m_s + b.m_s,
                a.m_v.X + b.m_v.X,
                a.m_v.Y + b.m_v.Y,
                a.m_v.Z + b.m_v.Z
            );
        }
        /// <summary>
        /// Subtracts two Quatd component‑wise.
        /// </summary>
        public static Quatd operator - ( Quatd a, Quatd b ) {
            return new Quatd(
                a.m_s - b.m_s,
                a.m_v.X - b.m_v.X,
                a.m_v.Y - b.m_v.Y,
                a.m_v.Z - b.m_v.Z
            );
        }
        /// <summary>
        /// Invert 
        /// </summary>
        public static Quatd operator - ( Quatd a ) {
            return new Quatd(
                -a.m_s,
                -a.m_v.X,
                -a.m_v.Y,
                -a.m_v.Z
            );
        }
        /// <summary>
        /// Divides two Quatd component‑wise.
        /// </summary>
        public static Quatd operator / ( Quatd a, Quatd b ) {
            return a * Invert(b);
        }
        /// <summary>
        /// Divides both components of the Quatd by a double.
        /// </summary>
        public static Quatd operator / ( Quatd a, double b ) {
            Quatd _ret = new Quatd();

            _ret.m_s = a.m_s / b;
            _ret.m_v = a.m_v / b;

            return _ret;
        }
    }
    /// @}
}
