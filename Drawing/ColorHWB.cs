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
	/// \addtogroup SystemEx.Drawing
	/// @{
	/// <summary>
	/// Represents a color in the HWB (Hue, Whiteness, Blackness)
	/// </summary>
	public struct ColorHWB : IEquatable<ColorHWB> {
        private float m_hue;
        private float m_whiteness;
        private float m_blackness;

        /// <summary>
        /// The hue component of the color, in degrees (0–360).
        /// </summary>
        public float H { get => m_hue; set => m_hue = ClampHue(value); }
        /// <summary>
        /// The whiteness component of the color, in the range [0, 1].
        /// </summary>
        public float Whiteness { get => m_whiteness; set => m_whiteness = System.Math.Clamp(value, 0f, 1f); }

        /// <summary>
        /// The blackness component of the color, in the range [0, 1].
        /// </summary>
        public float Blackness { get => m_blackness; set => m_blackness = System.Math.Clamp(value, 0f, 1f); }

        /// <summary>
        /// Initializes a new HWB color with the specified component values.
        /// </summary>
        /// <param name="h">The hue component (0–360).</param>
        /// <param name="whiteness">The whiteness component (0–1).</param>
        /// <param name="blackness">The blackness component (0–1).</param>
        public ColorHWB(float h, float whiteness, float blackness) { 
            m_hue = ClampHue(h);
            m_whiteness = System.Math.Clamp(whiteness, 0f, 1f);   
            m_blackness = System.Math.Clamp(blackness, 0f, 1f);
        }

        /// <summary>
        /// Initializes a new HWB color from an array of component values.
        /// </summary>
        /// <param name="x">An array containing the hue, whiteness, and blackness components.</param>
        public ColorHWB(float[] x) {
            m_hue = ClampHue(x[0]);
            m_whiteness = System.Math.Clamp(x[1], 0f, 1f);
            m_blackness = System.Math.Clamp(x[2], 0f, 1f);
        }

        /// <summary>
        /// Determines whether this instance is equal to another HWB color.
        /// </summary>
        /// <param name="other">The color to compare with.</param>
        /// <returns>
        /// <c>true</c> if the components match; otherwise <c>false</c>.
        /// </returns>
        public bool Equals(ColorHWB other) {
            
            return m_hue == other.m_hue && m_blackness == other.m_blackness && m_whiteness == other.m_whiteness;
        }
        /// <summary>
        /// Returns a hash code based on the H, W, and B components.
        /// </summary>
        public override int GetHashCode() {
            return HashCode.Combine(m_hue, m_blackness, m_whiteness);
        }
        /// <summary>
        /// Determines whether this instance is equal to another object.
        /// </summary>
        public override bool Equals(object? obj) {

            if ( obj is ColorHWB d) return Equals(d);
            return false;
        }
        /// <summary>
        /// Returns a string representation of the HWB color.
        /// </summary>
        public override string ToString() {
            return string.Create(null, stackalloc char[256], $"[{m_hue}, {m_whiteness}, {m_blackness}]");
        }
        /// <summary>
        /// Wraps a hue value into the range 0–360 degrees.
        /// </summary>
        private static float ClampHue(float h) {
            h %= 360f;
            return h < 0f ? h + 360f : h;
        }
        
    }
    /// @}
}
