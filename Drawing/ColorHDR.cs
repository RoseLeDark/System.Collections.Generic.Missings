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
    /// \addtogroup color
    /// @{
    /// <summary>
    /// Represents a high‑dynamic‑range color in the HSV color space,
    /// where the value component can exceed the normalized range (0–1)
    /// and extend up to <see cref="MaxValue"/>.
    /// Provides hue‑aware interpolation, brightness manipulation,
    /// and arithmetic operations for HDR color processing.
    /// </summary>
    public class ColorHDR : IColor<ColorHDR> {
        private float m_maxValue = 10.0f;
        internal float m_hue;   // 0–360°
        internal float m_saturation;   // 0–1
        internal float m_value;   // 0–m_maxValue

        /// <summary>
        /// Gets or sets the hue component in degrees (0–360).
        /// Values outside the range are wrapped automatically.
        /// </summary>
        public virtual float H { get => m_hue; set => m_hue = ClampHue(value); }
        /// <summary>
        /// Gets or sets the HDR value (brightness) component.
        /// The value is clamped to the range 0–<see cref="MaxValue"/>.
        /// </summary>
        public virtual float V { get => m_value; set => m_value = System.Math.Clamp(value, 0.0f, m_maxValue); }
        /// <summary>
        /// Gets or sets the saturation component in the range 0–1.
        /// </summary>
        public virtual float S { get => m_saturation; set => m_saturation = System.Math.Clamp(value, 0.0f, 1.0f); }
        /// <summary>
        /// Gets or sets the maximum allowed HDR value.
        /// </summary>
        public virtual float MaxValue { get => m_maxValue; set => m_maxValue = value; }
        /// <summary>
        /// Initializes a new HDR color with the specified HSV components.
        /// </summary>
        public ColorHDR(float h, float s, float v) {
            H = h;
            S = s;
            V = v;
        }
        /// <summary>
        /// Adjusts the saturation by the specified delta.
        /// </summary>
        public virtual ColorHDR Saturation(float delta) {
            m_saturation = System.Math.Clamp(m_saturation + delta, 0.0f, 1.0f);
            return this;
        }
        /// <summary>
        /// Adjusts the HDR brightness by the specified delta.
        /// </summary>
        public virtual ColorHDR Brightness(float delta) {
            m_value = System.Math.Clamp(m_value + delta, 0.0f, m_maxValue);
            return this;
        }
        /// <summary>
        /// Adds another HDR color to this one, component by component.
        /// Hue is wrapped, saturation and value are clamped.
        /// </summary>
        public virtual ColorHDR Addition(ColorHDR a) {
            m_saturation = System.Math.Clamp(m_saturation + a.m_saturation, 0.0f, 1.0f);
            m_value = System.Math.Clamp(m_value + a.m_value, 0.0f, m_maxValue);
            m_hue = ClampHue(m_hue + a.m_hue);
            return this;
        }
        /// <summary>
        /// Subtracts another HDR color from this one, component by component.
        /// Hue is wrapped, saturation and value are clamped.
        /// </summary>
        public virtual ColorHDR Subtraction(ColorHDR a) {
            m_saturation = System.Math.Clamp(m_saturation - a.m_saturation, 0.0f, 1.0f);
            m_value = System.Math.Clamp(m_value - a.m_value, 0.0f, m_maxValue);
            m_hue = ClampHue(m_hue - a.m_hue);
            return this;
        }
        /// <summary>
        /// Multiplies this HDR color with another, component by component.
        /// Hue is not affected.
        /// </summary>
        public virtual ColorHDR Multiplication(ColorHDR a) {
            m_saturation = System.Math.Clamp(m_saturation * a.m_saturation, 0.0f, 1.0f);
            m_value = System.Math.Clamp(m_value * a.m_value, 0.0f, m_maxValue);
      
            return this;
        }
        /// <summary>
        /// Divides this HDR color by another, component by component.
        /// Hue is not affected.
        /// </summary>
        public virtual ColorHDR Division(ColorHDR a) {

            if ( a.m_saturation != 0 ) m_saturation = System.Math.Clamp(m_saturation / a.m_saturation, 0.0f, 1.0f);
            if ( a.m_value != 0 ) m_value = System.Math.Clamp(m_value / a.m_value, 0.0f, m_maxValue);
  
            return this;
        }
        /// <summary>
        /// Increases the HDR brightness by the specified intensity.
        /// </summary>
        public virtual ColorHDR LightIntensity(float intensity) {
            V += intensity;
            return this;
        }
        /// <summary>
        /// Adds the specified HSV components to this HDR color.
        /// This is a convenience overload that constructs a temporary HDR color.
        /// </summary>
        /// <param name="a">Hue component.</param>
        /// <param name="b">Saturation component.</param>
        /// <param name="c">Value component.</param>
        /// <returns>The modified HDR color.</returns>
        public ColorHDR Addition(float a, float b, float c) {
            return Addition(new ColorHDR(a, b, c));
        }
        /// <summary>
        /// Subtracts the specified HSV components from this HDR color.
        /// This is a convenience overload that constructs a temporary HDR color.
        /// </summary>
        /// <param name="a">Hue component.</param>
        /// <param name="b">Saturation component.</param>
        /// <param name="c">Value component.</param>
        /// <returns>The modified HDR color.</returns>
        public ColorHDR Subtraction(float a, float b, float c) {
            return Subtraction(new ColorHDR(a, b, c));
        }
        /// <summary>
        /// Multiplies this HDR color by the specified HSV components.
        /// This is a convenience overload that constructs a temporary HDR color.
        /// </summary>
        /// <param name="a">Hue component.</param>
        /// <param name="b">Saturation component.</param>
        /// <param name="c">Value component.</param>
        /// <returns>The modified HDR color.</returns>
        public ColorHDR Multiplication(float a, float b, float c) {
            return Multiplication(new ColorHDR(a, b, c));
        }
        /// <summary>
        /// Divides this HDR color by the specified HSV components.
        /// This is a convenience overload that constructs a temporary HDR color.
        /// </summary>
        /// <param name="a">Hue component.</param>
        /// <param name="b">Saturation component.</param>
        /// <param name="c">Value component.</param>
        /// <returns>The modified HDR color.</returns>
        public ColorHDR Division(float a, float b, float c) {
            return Division(new ColorHDR(a, b, c));
        }

        /// <summary>
        /// Wraps a hue value into the range 0–360 degrees.
        /// </summary>
        protected static float ClampHue(float h) {
            h %= 360f;
            return h < 0f ? h + 360f : h;
        }
        /// <summary>
        /// Performs a quadratic interpolation toward another HDR color.
        /// Hue interpolation follows the shortest path around the color wheel.
        /// </summary>
        public ColorHDR Lerp(ColorHDR value, float amount) {    
            // Quadratische Kurve
            float q = amount * amount;

            // Hue shortest-path interpolation
            float dh = value.m_hue - m_hue;
            dh = (dh + 540f) % 360f - 180f;

            return new ColorHDR(
                ClampHue(m_hue + dh * q),
                m_saturation + (value.m_saturation - m_saturation) * q,
                m_value + (value.m_value - m_value) * q
            );
        }
        /// <summary>
        /// Determines whether this instance is equal to another HDR color.
        /// </summary>
        public bool Equals(ColorHDR? other) {
            if ( other == null ) return false;

            return other.m_hue == m_hue &&
                other.m_saturation == m_saturation &&
                other.m_value == m_value;
        }
        /// <summary>
        /// Determines whether this instance is equal to another object.
        /// </summary>
        public override bool Equals(object? obj) {
            if ( obj == null ) return false;

            if ( obj is ColorHDR ) {
                var c = obj as ColorHDR;
                return Equals(c);
            }
            return false;
        }
        /// <summary>
        /// Returns a hash code based on the hue, saturation, and HDR value.
        /// </summary>
        public override int GetHashCode() {
            return m_hue.GetHashCode() ^ m_saturation.GetHashCode() ^ m_value.GetHashCode();
        }
        /// <summary>
        /// Returns a string representation of the HDR color.
        /// </summary>
        public override string ToString() {
            return string.Create(null, stackalloc char[256], $"[{m_hue}, {m_saturation}, {m_value}]");
        }
    }
    /// @}
}
