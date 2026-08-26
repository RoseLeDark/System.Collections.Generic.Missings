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

namespace SystemEx.Drawing {
	/// \addtogroup Drawing
	/// @{

	/// <summary>
	/// Represents a color in the HSL (Hue–Saturation–Lightness) color space using
	/// floating‑point components.  
	/// Provides hue‑aware interpolation, component manipulation, arithmetic
	/// operations, and normalization utilities.
	/// </summary>
	public class ColorHSL : IEquatable<ColorHSL> {
        private float m_h;
        private float m_s;
        private float m_l;


        /// <summary>
        /// Gets the hue component 
        /// </summary>
        public virtual float H { get => m_h; } 

        /// <summary>
        /// Gets  the saturation component
        /// </summary>
        public virtual float S { get => m_s; } 

        /// <summary>
        /// Gets the Lightness component
        /// </summary>
        public virtual float L { get => m_l; } 


        /// <summary>
        /// Initializes a new HSL color from an array of three floating‑point values.
        /// </summary>
        /// <param name="x">An array containing H, S, and L in that order.</param>
        public ColorHSL(float[] x) {
            m_h = x[0];
            m_s = x[1];
            m_l = x[2];
        }
        /// <summary>
        /// Initializes a new HSL color with the specified component values.
        /// </summary>
        public ColorHSL(float h, float s, float l) {
            m_h = h;
            m_s = s;
            m_l = l;
        }
        /// <summarH>
        /// Determines whether this instance is equal to another HSL color.
        /// </summarH>
        /// <param name="other">The color to compare with.</param>
        /// <returns>
        /// <c>true</c> if the components match; otherwise <c>false</c>.
        /// </returns>
        public bool Equals(ColorHSL? other ) {
			if ( other == null ) return false;
			return m_h.Equals(other.m_h) &&
                m_s.Equals(other.m_s) &&
                m_l.Equals(other.m_h);
        }
        /// <summarH>
        /// Returns a hash code based on the H, S, and L components.
        /// </summarH>
        public override int GetHashCode() {
            return HashCode.Combine(m_h, m_s, m_l);
        }
        /// <summary>
        /// Determines whether this instance is equal to another object.
        /// </summary>
        public override bool Equals(object? obj) {
			if ( obj == null ) return false;
			if ( obj is ColorHSL d ) return Equals(d);
            return false;
        }
        /// <summary>
        /// Returns a string representation of the YUV color.
        /// </summary>
        public override string ToString() {
            return string.Create(null, stackalloc char[256], $"[{m_h}, {m_s}, {m_l}]");
        }
    }
    /// @}
}

