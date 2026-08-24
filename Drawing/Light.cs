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
using System.Drawing;
using System.Numerics;
using System.Text;
using SystemEx.Numeric;

namespace SystemEx.Drawing {
	/// \addtogroup SystemEx.Drawing
	/// @{
	/// <summary>
	/// Base light class used for all light types in the SystemEx.Drawing namespace.
	/// Provides shared parameters such as position, direction, diffuse/ambient color
	/// and projection settings. Specific light types (spot, point, directional)
	/// extend this class with additional behavior.
	/// </summary>
	public class Light {
        /// <summary>
        /// projection-based lights (e.g., spot lights).
        /// </summary>
        Projection m_projection;


        /// <summary>
        /// World-space position of the light.
        /// </summary>
        protected Vec3f m_Position;

        /// <summary>
        /// Normalized world-space direction of the light.
        /// Used for directional and spot lights.
        /// </summary>
        protected Vec3f m_Direction;

        protected Vec3f m_Up;

        /// <summary>
        /// Diffuse color contribution of the light.
        /// Defines the main visible light color.
        /// </summary>
        protected ColorHSV m_DiffuseColor;

        /// <summary>
        /// Ambient color contribution of the light.
        /// Used for low-frequency indirect lighting.
        /// </summary>
        protected ColorHSV m_AmbientColor;

        public ColorHSV Ambient { get => m_AmbientColor; set => m_AmbientColor = value; }

        public ColorHSV Diffuse { get => m_DiffuseColor; set => m_DiffuseColor = value; }

        public Vec3f Direction { get => m_Direction; set => m_Direction = value; }

        public Vec3f Position { get => m_Position; set => m_Position = value; }

        public Vec3f Up { get => m_Up; set => m_Up = value; }

        public Vec3f LookAt => m_Position + m_Direction;

        public Projection Projection => m_projection;



        public Light() {
            m_Position = new Vec3f(-10.0f, 2.0f, 0.0f);
            m_Direction = new Vec3f(1.0f, 0.0f, 0.0f);
            m_DiffuseColor = WhiteColors.Snow.ToColorHSV();
            m_AmbientColor = YellowColors.LightYellow.ToColorHSV();

            m_Up = new Vec3f(0.0f, 1.0f, 0.0f);

        }

        public Light( Vec3f position, Vec3f direction, ColorR8G8B8 ambient, ColorR8G8B8 diffuse) {
            m_Position = position;
            m_Direction = direction;
            m_DiffuseColor = diffuse.ToColorHSV();
            m_AmbientColor = ambient.ToColorHSV();
        }

        

        /// <summary>
        /// Sets the projection parameters for lights that require a frustum
        /// (typically spot lights or shadow-casting lights).
        /// </summary>
        public void SetProjParams ( Projection projection) {
            m_projection = projection;
        }
    }


    /// <summary>
    /// Spot light implementation.
    /// Adds inner and outer cone angles (phi/theta) expressed as cosine values.
    /// These values are used for smooth falloff inside the spotlight cone.
    /// </summary>
    public class SpotLight : Light {

        /// <summary>
        /// Cosine of half the outer cone angle (theta).
        /// Defines the outer boundary of the spotlight.
        /// </summary>
        protected float m_cosHalfTheta;

        /// <summary>
        /// Cosine of half the inner cone angle (phi).
        /// Defines the fully illuminated inner region of the spotlight.
        /// </summary>
        protected float m_cosHalfPhi;

        public float CosHalfPhi { get => m_cosHalfPhi; set => m_cosHalfPhi = value; }

        public float CosHalfTheta { get => m_cosHalfTheta; set => m_cosHalfTheta = value; }

        /// <summary>
        /// Inner cone angle of the spotlight in degrees.
        /// Defines the fully illuminated inner region.
        /// </summary>
        public float InnerConeAngle {
            get => MathF.Acos(m_cosHalfPhi) * 2.0f * (180.0f / MathF.PI);
            set {
                float halfAngleRad = (value * MathF.PI / 180.0f) * 0.5f;
                m_cosHalfPhi = MathF.Cos(halfAngleRad);
            }
        }
        /// <summary>
        /// Outer cone angle of the spotlight in degrees.
        /// Defines the soft falloff region of the spotlight.
        /// </summary>
        public float OuterConeAngle {
            get => MathF.Acos(m_cosHalfTheta) * 2.0f * (180.0f / MathF.PI);
            set {
                float halfAngleRad = (value * MathF.PI / 180.0f) * 0.5f;
                m_cosHalfTheta = MathF.Cos(halfAngleRad);
            }
        }

        public SpotLight() : base() {
            m_cosHalfPhi = 0.4f;
            m_cosHalfTheta = 0.9f;
        }
        public SpotLight ( float HalfPhi, float HalfTheta) : base() {
            m_cosHalfPhi = HalfPhi;
            m_cosHalfTheta = HalfTheta;
        }

        public SpotLight ( float HalfPhi, float HalfTheta, 
            Vec3f position ,  Vec3f direction,  ColorR8G8B8 ambient,  ColorR8G8B8 diffuse )
            : base(position, direction, ambient, diffuse) {
            m_cosHalfPhi = HalfPhi;
            m_cosHalfTheta = HalfTheta;
        }
    }


    /// <summary>
    /// Point light implementation.
    /// Adds attenuation parameters controlling how light intensity decreases
    /// over distance. Typically contains constant, linear, and quadratic terms.
    /// </summary>
    public class PointLight : Light {
        /// <summary>
        /// Attenuation parameters for the point light.
        /// Usually encoded as (constant, linear, quadratic, range).
        /// </summary>
        protected Vec4f m_Attenuation;

        /// <summary>
        /// Constant attenuation term.
        /// Controls base brightness independent of distance.
        /// </summary>
        public float Brightness {
            get => m_Attenuation.X;
            set => m_Attenuation.X = value;
        }

        /// <summary>
        /// Brightness falloff proportional to distance.
        /// Higher values reduce brightness faster as the viewer moves away.
        /// </summary>
        public float BrightnessFalloff {
            get => m_Attenuation.Y;
            set => m_Attenuation.Y = value;
        }

        /// <summary>
        /// Brightness falloff proportional to the square of the distance.
        /// Controls how quickly the light fades at long ranges.
        /// </summary>
        public float BrightnessDistance {
            get => m_Attenuation.Z;
            set => m_Attenuation.Z = value;
        }

        /// <summary>
        /// Maximum effective range of the light.
        /// Used for culling and optional shader optimization.
        /// </summary>
        public float Range {
            get => m_Attenuation.W;
            set => m_Attenuation.W = value;
        }

		/// <summary>
		/// Create a basic PointLigh with Brightness: 0, BrightnessFalloff: 0, 
        /// BrightnessDistance:0.4f and Range: 0
		/// </summary>
		public PointLight () : base() {
            m_Attenuation = new Vec4f(0, 0, 0.4f, 0);
        }
		/// <summary>
		/// Create a basic PointLigh with given Values for Brightness: X, BrightnessFalloff: Y, 
		/// BrightnessDistance: Z and Range: W
		/// </summary>
		public PointLight ( Vec4f Attenuation ) : base() {
            m_Attenuation = Attenuation;
        }
		/// <summary>
		/// Create A Basic Pointligt with given parameter
		/// </summary>
		/// <param name="Attenuation">Properties Brightness: X, BrightnessFalloff: Y, 
		/// BrightnessDistance: Z and Range: W</param>
		/// <param name="position">The Position</param>
		/// <param name="direction">The light direction</param>
		/// <param name="ambient">The light ambient</param>
		/// <param name="diffuse">The light diffuse</param>
		public PointLight ( Vec4f Attenuation,
            Vec3f position, Vec3f direction, ColorR8G8B8 ambient, ColorR8G8B8 diffuse )
            : base(position, direction, ambient, diffuse) {
            m_Attenuation = Attenuation;
        }
    }

    ///  @}
}
