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
	/// Represents a color with 16‑bit precision per channel (R16G16B16),
	/// stored internally as normalized floating‑point values (0–1).
	/// </summary>
	public struct ColorR16G16B16 : IColor<ColorR16G16B16>, IEquatable<ColorR16G16B16> {
        private float m_red;
        private float m_green;
        private float m_blue;

        /// <summary>Gets the red channel.</summary>
        public float Red => m_red;

        /// <summary>Gets the green channel.</summary>
        public float Green => m_green;

        /// <summary>Gets the blue channel.</summary>
        public float Blue => m_blue;

        /// <summary>Gets the red channel.</summary>
        public float R => m_red;

        /// <summary>Gets the green channel.</summary>
        public float G => m_green;

        /// <summary>Gets the blue channel.</summary>
        public float B => m_blue;

        /// <summary>
        /// Creates a black color (0,0,0).
        /// </summary>
        public ColorR16G16B16() {
            m_red = 0f;
            m_green = 0f;
            m_blue = 0f;
        }
        /// <summary>
        /// Creates a color from normalized RGB components.
        /// </summary>
        public ColorR16G16B16(float r, float g, float b) {
            m_red = r;
            m_green = g;
            m_blue = b;
        }
        /// <summary>
        /// Creates a color from 16‑bit UNORM components (0–65535).
        /// </summary>
        public ColorR16G16B16 (ushort r, ushort g, ushort b) {
            const float inv = 1f / 65535f;
            m_red = r * inv;
            m_green = g * inv;
            m_blue = b * inv;
        }
        /// <summary>
        /// Creates a grayscale color using a single intensity value.
        /// </summary>
        public ColorR16G16B16(float gray) {
            m_red = gray;
            m_green = gray;
            m_blue = gray;
        }

        /// <summary>Adds another color to this one.</summary>
        public ColorR16G16B16 Addition(ColorR16G16B16 value) {
            m_red += value.m_red;
            m_green += value.m_green;
            m_blue += value.m_blue;
            return this;
        }

        /// <summary>Subtracts another color from this one.</summary>
        public ColorR16G16B16 Subtraction(ColorR16G16B16 value) {
            m_red -= value.m_red;
            m_green -= value.m_green;
            m_blue -= value.m_blue;
            return this;
        }

        /// <summary>Multiplies this color with another.</summary>
        public ColorR16G16B16 Multiplication(ColorR16G16B16 value) {
            m_red *= value.m_red;
            m_green *= value.m_green;
            m_blue *= value.m_blue;
            return this;
        }

        /// <summary>Divides this color by another.</summary>
        public ColorR16G16B16 Division(ColorR16G16B16 value) {
            if ( value.m_red != 0f ) m_red /= value.m_red;
            if ( value.m_green != 0f ) m_green /= value.m_green;
            if ( value.m_blue != 0f ) m_blue /= value.m_blue;
            return this;
        }

        /// <summary>
        /// Returns a version of the color with all channels clamped to 0–1.
        /// </summary>
        public ColorR16G16B16 AsNormalized() {
            return new ColorR16G16B16(
                System.Math.Clamp(m_red, 0f, 1f),
                System.Math.Clamp(m_green, 0f, 1f),
                System.Math.Clamp(m_blue, 0f, 1f)
            );
        }

        /// <summary>
        /// Returns a version of the color scaled so the brightest channel becomes 1.
        /// </summary>
        public ColorR16G16B16 AsScaled() {
            float max = System.MathF.Max(m_red, System.MathF.Max(m_green, m_blue));
            if ( max <= 1f )
                return this;

            return new ColorR16G16B16(m_red / max, m_green / max, m_blue / max);
        }

        /// <summary>Computes the perceived brightness of the color.</summary>
        public float Brightness() {
            return m_red * 0.299f + m_green * 0.587f + m_blue * 0.114f;
        }

        /// <summary>Computes the brightness using linear RGB.</summary>
        public float LinearBrightness() {
            var lin = this.ToLinear();
            return lin.Red * 0.2126f + lin.Green * 0.7152f + lin.Blue * 0.0722f;
        }

        /// <summary>Measures the visual contrast between this color and another.</summary>
        public float Contrast(ColorR16G16B16 other) {
            return System.MathF.Abs(Brightness() - other.Brightness());
        }

        /// <summary>Classifies the contrast between this color and another.</summary>
        public ContrastLevel GetKontrast(ColorR16G16B16 other) {
            float c = Contrast(other);

            if ( c < 0.20f ) return ContrastLevel.VeryLow;
            if ( c < 0.40f ) return ContrastLevel.Low;
            if ( c < 0.70f ) return ContrastLevel.High;
            return ContrastLevel.Perfect;
        }
        /// <summary>Generates a visually contrasting color.</summary>
        public ColorR16G16B16 GetNextContrastColor() {
            var hsv = this.ToColorHSV();
            hsv.H = (hsv.H + 180f) % 360f;
            hsv.V = 1f - hsv.V;
            return hsv.ToColorR16G16B16();
        }

        /// <summary>Blends this color toward another by the given amount.</summary>
        public ColorR16G16B16 Lerp(ColorR16G16B16 value, float amount) {
            return new ColorR16G16B16(
                m_red + amount * (value.m_red - m_red),
                m_green + amount * (value.m_green - m_green),
                m_blue + amount * (value.m_blue - m_blue)
            );
        }

        /// <summary>
        /// Returns a string representation of the color.
        /// </summary>
        public override string ToString() {
            return string.Create(null, stackalloc char[256], $"[{m_red}, {m_green}, {m_blue}]");
        }
        /// <summary>
        /// Adjusts the perceived saturation of the color.
        /// </summary>
        public ColorR16G16B16 Saturation(float delta) {
            var hsv = this.ToColorHSV();
            var rgb = hsv.Saturation(delta).ToColorR16G16B16();

            m_red = rgb.m_red;
            m_green = rgb.m_green;
            m_blue = rgb.m_blue;

            return this;
        }

        /// <summary>
        /// Adjusts the perceived brightness of the color.
        /// </summary>
        public ColorR16G16B16 Brightness(float delta) {
            var hsv = this.ToColorHSV();
            var rgb = hsv.Brightness(delta).ToColorR16G16B16();

            m_red = rgb.m_red;
            m_green = rgb.m_green;
            m_blue = rgb.m_blue;

            return this;
        }

        /// <summary>
        /// Addition  another color in r, g, b channels with this one
        /// </summary>
        public ColorR16G16B16 Addition(float a, float b, float c) {
            return Addition(new ColorR16G16B16(a, b, c));
        }
        /// <summary>
        /// Subtraction  another color in r, g, b channels with this one
        /// </summary>
        public ColorR16G16B16 Subtraction(float a, float b, float c) {
            return Subtraction(new ColorR16G16B16(a, b, c));
        }
        /// <summary>
        /// Multiplication  another color in r, g, b channels with this one
        /// </summary>
        public ColorR16G16B16 Multiplication(float a, float b, float c) {
            return Multiplication(new ColorR16G16B16(a, b, c));
        }
        /// <summary>
        /// Division  another color in r, g, b channels with this one
        /// </summary>
        public ColorR16G16B16 Division(float a, float b, float c) {
            return Division(new ColorR16G16B16(a, b, c));
        }

        /// <summary>
        /// Determines whether this instance is equal to another YUV color.
        /// </summary>
        /// <param name="other">The color to compare with.</param>
        /// <returns>
        /// <c>true</c> if the components match; otherwise <c>false</c>.
        /// </returns>
        public bool Equals(ColorR16G16B16 other) {

            return R.Equals(other.R) &&
                G.Equals(other.G) &&
                B.Equals(other.B);
        }
        /// <summary>
        /// Returns a hash code based on the R, G, and B components.
        /// </summary>
        public override int GetHashCode() {
            return HashCode.Combine(this.m_red, this.m_blue, this.m_blue);
        }
        /// <summary>
        /// Determines whether this instance is equal to another object.
        /// </summary>
        public override bool Equals(object? obj) {
			if ( obj == null ) return false;
			if ( obj is ColorR16G16B16 d) return Equals(d);
            return false;
        }
    }
    /// @}
}
