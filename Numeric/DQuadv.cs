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
using System.Text;

namespace SystemEx.Numeric {
	/// \addtogroup Numeric
	/// @{

	/// <summary>
	/// Generic interface for dual quaternions.
	/// TQ represents the quaternion type (rotation + dual part),
	/// T represents the vector type used for translation.
	/// Provides the essential operations required for dual-quaternion-based
	/// rigid transformations.
	/// </summary>
	/// <typeparam name="TQ">Quaternion-Typ (Rotation + Dualteil)</typeparam>
	/// <typeparam name="T">Vektor-Typ (Translation / Position)</typeparam>
	internal interface IDQuatv<TQ, T> {
        /// <summary>
        /// Gets the real part of the dual quaternion (rotation component).
        /// </summary>
        TQ Real { get; }

        /// <summary>
        /// Gets the dual part of the dual quaternion (translation component).
        /// </summary>
        TQ Dual { get; }

        /// <summary>
        /// Transforms a vector using the dual quaternion.
        /// Applies rotation and translation in a drift-free manner.
        /// </summary>
        T Transform ( T v );

        /// <summary>
        /// Sets the translation component of the dual quaternion.
        /// </summary>
        void Translation ( T translation );

        /// <summary>
        /// Sets the rotation component of the dual quaternion.
        /// </summary>
        void Rotation ( TQ rotation );

        /// <summary>
        /// Sets both rotation and translation components at once.
        /// </summary>
        void RotationTranslation ( TQ rotation, T translation );

        /// <summary>
        /// Normalizes the dual quaternion for numerical stability.
        /// </summary>
        void Normalize ();
    }

    /// <summary>
    /// Dual quaternion using single-precision floating point values.
    /// Represents a rigid 3D transformation consisting of rotation (real part)
    /// and translation (dual part). Provides stable, non-drifting transformations
    /// compared to matrix-based approaches.
    /// </summary>
    public struct DQuatf : IDQuatv<Quatf, Vec3f> {
        /// <summary>
        /// Rotation component (real part).
        /// </summary>
        Quatf m_rot;

        /// <summary>
        /// Translation component (dual part).
        /// </summary>
        Quatf m_translation;

        /// <summary>
        /// Gets the rotation component of the dual quaternion.
        /// </summary>
        public Quatf Real => m_rot;

        /// <summary>
        /// Gets the translation component of the dual quaternion.
        /// </summary>
        public Quatf Dual => m_translation;

        /// <summary>
        /// Creates a dual quaternion with rotation only.
        /// Translation is initialized to zero.
        /// </summary>
        public DQuatf ( Quatf rotation ) {
            m_rot = rotation;
            m_translation = new Quatf(0f, 0f, 0f, 0f);
        }


        /// <summary>
        /// Creates a dual quaternion from rotation and translation.
        /// </summary>
        public DQuatf ( Quatf rotation, Vec3f translation ) {
            var x = FromRotationTranslation(rotation, translation);
            m_rot = x.m_rot;
            m_translation = x.m_translation;
        }


        /// <summary>
        /// Normalizes both rotation and dual components.
        /// Required for stable interpolation and accumulation.
        /// </summary>
        public void Normalize () {
            float len = Quatf.Lenght(m_rot);
            m_rot /= len;
            m_translation /= len;
        }


        /// <summary>
        /// Sets the rotation component of the dual quaternion.
        /// </summary>
        public void Rotation ( Quatf rotation ) {
            m_rot = rotation;
        }


        /// <summary>
        /// Sets the translation component of the dual quaternion.
        /// Translation is encoded into the dual part using:
        /// qd = 0.5 * (translation_quat * rotation)
        /// </summary>
        public void Translation ( Vec3f translation ) {
            Quatf t = new Quatf(0f, translation.X, translation.Y, translation.Z);
            m_translation = 0.5f * (t * m_rot);
        }


        /// <summary>
        /// Sets both rotation and translation components.
        /// Translation is encoded into the dual part.
        /// </summary>
        public void RotationTranslation ( Quatf rotation, Vec3f translation ) {
            m_rot = rotation;
            Quatf t = new Quatf(0f, translation.X, translation.Y, translation.Z);
            m_translation = 0.5f * (t * rotation);
        }


        /// <summary>
        /// Computes the inverse of the dual quaternion.
        /// Inverts rotation and adjusts translation accordingly.
        /// </summary>
        public DQuatf Invert () {
            Quatf rInv = Quatf.Invert(m_rot);
            Quatf dInv = -(rInv * m_translation * rInv);

            return new DQuatf
            {
                m_rot = rInv,
                m_translation = dInv
            };
        }



        /// <summary>
        /// Transforms a vector using the dual quaternion.
        /// Applies rotation and translation without matrix drift.
        /// </summary>
        public Vec3f Transform ( Vec3f v ) {
            Quatf p = new Quatf(0f, v.X, v.Y, v.Z);
            Quatf rInv = Quatf.Invert(m_rot);
            Quatf rotated = m_rot * p * rInv;

            Quatf trans = m_translation * rInv;
            Vec3f t = new Vec3f(trans.X, trans.Y, trans.Z) * 2f;

            return new Vec3f(rotated.X, rotated.Y, rotated.Z) + t;
        }


        /// <summary>
        /// Performs screw-motion interpolation between two dual quaternions.
        /// Interpolates rotation and translation simultaneously.
        /// </summary>
        public static DQuatf Slerp ( DQuatf a, DQuatf b, float t ) {
            Quatf rot = Quatf.Slerp(a.m_rot, b.m_rot, t);
            Quatf dual = Quatf.Slerp(a.m_translation, b.m_translation, t);

            return new DQuatf
            {
                m_rot = rot,
                m_translation = dual
            };
        }


        /// <summary>
        /// Multiplies two dual quaternions, combining their transformations.
        /// </summary>
        public static DQuatf operator * ( DQuatf a, DQuatf b ) {
            Quatf real = a.m_rot * b.m_rot;
            Quatf dual = a.m_rot * b.m_translation + a.m_translation * b.m_rot;

            DQuatf result;
            result.m_rot = real;
            result.m_translation = dual;
            return result;
        }

        /// <summary>
        /// Linear addition of dual quaternions.
        /// Not a valid rigid transformation; used for blending only.
        /// </summary>
        public static DQuatf operator + ( DQuatf a, DQuatf b ) {
            return new DQuatf
            {
                m_rot = a.m_rot + b.m_rot,
                m_translation = a.m_translation + b.m_translation
            };
        }


        /// <summary>
        /// Linear subtraction of dual quaternions.
        /// Not a valid rigid transformation; used for blending only.
        /// </summary>
        public static DQuatf operator - ( DQuatf a, DQuatf b ) {
            return new DQuatf
            {
                m_rot = a.m_rot - b.m_rot,
                m_translation = a.m_translation - b.m_translation
            };
        }

        /// <summary>
        /// Divides both components by a scalar.
        /// Useful for normalization and weighted blending.
        /// </summary>
        public static DQuatf operator / ( DQuatf a, float s ) {
            return new DQuatf
            {
                m_rot = a.m_rot / s,
                m_translation = a.m_translation / s
            };
        }

        /// <summary>
        /// Creates a dual quaternion from rotation and translation.
        /// Translation is encoded into the dual part using:
        /// qd = 0.5 * (translation_quat * rotation)
        /// </summary>
        public static DQuatf FromRotationTranslation ( Quatf rotation, Vec3f translation ) {
            Quatf real = rotation;
            Quatf t = new Quatf(0f, translation.X, translation.Y, translation.Z);
            Quatf dual = 0.5f * (t * rotation);

            return new DQuatf
            {
                m_rot = real,
                m_translation = dual
            };
        }


        /// <summary>
        /// Creates a dual quaternion using the conjugated rotation.
        /// Useful for inverse transformation chains.
        /// </summary>
        public static DQuatf FromRotationTranslationConjugated ( Quatf rotation, Vec3f translation ) {
            Quatf r = Quatf.Conjugate(rotation);
            Quatf t = new Quatf(0f, translation.X, translation.Y, translation.Z);
            Quatf dual = 0.5f * (t * r);

            return new DQuatf
            {
                m_rot = r,
                m_translation = dual
            };
        }

        /*
         * public m44f ToMatrix()
{
    // Rotation als Matrix
    Matrix4x4f rot = m_rot.ToMatrix();

    // Translation extrahieren
    Quatf rInv = m_rot.Inverse();
    Quatf trans = m_translation * rInv;
    Vec3f t = new Vec3f(trans.X, trans.Y, trans.Z) * 2f;

    rot.M41 = t.X;
    rot.M42 = t.Y;
    rot.M43 = t.Z;

    return rot;
}
*/
    }
	/// @}
}
