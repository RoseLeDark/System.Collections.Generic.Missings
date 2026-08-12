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
    /// Represents the qualitative contrast level between two colors.
    /// </summary>
    public enum ContrastLevel {
        /// <summary>Colors are nearly identical with minimal visible difference.</summary>
        VeryLow,

        /// <summary>Colors differ slightly with low visual separation.</summary>
        Low,

        /// <summary>Colors are clearly distinguishable.</summary>
        High,

        /// <summary>Maximum visual contrast between the colors.</summary>
        Perfect
    }

    /// <summary>
    /// Represents an RGB color using normalized float channels (0–1).
    /// Provides basic color manipulation, brightness evaluation,
    /// contrast classification and interpolation utilities.
    /// </summary>
    public class ColorR8G8B8 : IColor<ColorR8G8B8> {
        private float m_red;
        private float m_green;
        private float m_blue;

        /// <summary>Gets the red channel.</summary>
        public float Red => m_red;

        /// <summary>Gets the green channel.</summary>
        public float Green => m_green;

        /// <summary>Gets the blue channel.</summary>
        public float Blue => m_blue;

        /// <summary>Indicates whether the color appears visually bright.</summary>
        public bool IsLight => Brightness() >= 0.5f;

        /// <summary>Indicates whether the color appears visually dark.</summary>
        public bool IsDark => Brightness() < 0.5f;


        /// <summary>
        /// Creates a black color (0,0,0).
        /// </summary>
        public ColorR8G8B8() {
            m_red = 0.0f;
            m_green = 0.0f;
            m_blue = 0.0f;
        }

        /// <summary>
        /// Creates a color from a packed 24‑bit RGB integer (0xRRGGBB).
        /// </summary>
        public ColorR8G8B8(int rgb) {
            byte r = (byte)((rgb >> 16) & 0xFF);
            byte g = (byte)((rgb >> 8) & 0xFF);
            byte b = (byte)(rgb & 0xFF);

            m_red = r * 0.00392156862745098f;
            m_green = g * 0.00392156862745098f;
            m_blue = b * 0.00392156862745098f;
        }
		/// <summary>
		/// Creates a grayscale color using a single byte value.
		/// </summary>
		public ColorR8G8B8 ( byte b ) {
			m_red = b * 0.00392156862745098f;
			m_green = b * 0.00392156862745098f;
			m_blue = b * 0.00392156862745098f;
		}

		/// <summary>
		/// Creates a color from 8‑bit RGB components.
		/// </summary>
		public ColorR8G8B8(byte r, byte g, byte b) {
            m_red = r * 0.00392156862745098f;
            m_green = g * 0.00392156862745098f;
            m_blue = b * 0.00392156862745098f;
        }
        /// <summary>
        /// Creates a grayscale color using a single intensity value.
        /// </summary>
        public ColorR8G8B8(float rgb) {
            m_red = rgb;
            m_green = rgb;
            m_blue = rgb;
        }

        /// <summary>
        /// Creates a color from normalized RGB components.
        /// </summary>
        public ColorR8G8B8(float r, float g, float b) {
            m_red = r;
            m_green = g;
            m_blue = b;
        }

        /// <summary>
        /// Creates a color from a System.Drawing.Color.
        /// </summary>
        public ColorR8G8B8(System.Drawing.Color x)
            : this(x.R, x.G, x.B) { }

        /// <summary>
        /// Returns a string representation of the color in normalized RGB format.
        /// </summary>
        public override string ToString() {
            return string.Create(null, stackalloc char[256], $"[{m_red}, {m_green}, {m_blue}]");
        }

        /// <summary>
        /// Changes the perceived saturation of the color.
        /// </summary>
        public ColorR8G8B8 Saturation(float delta) {
            var hsl = this.ToColorHSV();
            var rgb = hsl.Saturation(delta).ToColorR8G8B8();

            m_red = rgb.m_red;
            m_blue = rgb.m_blue;
            m_green = rgb.m_green;

            return this;
        }

        /// <summary>
        /// Changes the perceived brightness of the color.
        /// </summary>
        public ColorR8G8B8 Brightness(float delta) {
            var hsl = this.ToColorHSV();
            var rgb = hsl.Brightness(delta).ToColorR8G8B8();

            m_red = rgb.m_red;
            m_blue = rgb.m_blue;
            m_green = rgb.m_green;

            return this;
        }
        /// <summary>
        /// Adds another color to this one, channel by channel.
        /// </summary>
        public ColorR8G8B8 Addition(ColorR8G8B8 value) {
            m_red       += value.m_red; 
            m_green     += value.m_green; 
            m_blue      += value.m_blue;

            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        public ColorR8G8B8 Subtraction(ColorR8G8B8 value) {
            m_red       -= value.m_red;
            m_green     -= value.m_green;
            m_blue      -= value.m_blue;

            return this;
        }
        /// <summary>
        /// Multiplies this color with another, channel by channel.
        /// </summary>
        public ColorR8G8B8 Multiplication(ColorR8G8B8 value) {
            m_red       *= value.m_red;
            m_green     *= value.m_green;
            m_blue      *= value.m_blue;

            return this;
        }
        /// <summary>
        /// Divides this color by another, channel by channel.
        /// </summary>
        public ColorR8G8B8 Division(ColorR8G8B8 value) {
            if ( value.m_red != 0 )     m_red /= value.m_red;
            if ( value.m_green != 0 )   m_green /= value.m_green;
            if ( value.m_blue != 0 )    m_blue /= value.m_blue;

            return this;
        }
        /// <summary>
        /// Returns a version of the color with all channels clamped to 0–1.
        /// </summary>
        public ColorR8G8B8 AsNormalized() {
            float _red = System.Math.Clamp(m_red, 0.0f, 1.0f);
            float _green = System.Math.Clamp(m_green, 0.0f, 1.0f);
            float _blue = System.Math.Clamp(m_blue, 0.0f, 1.0f);

            return new ColorR8G8B8(_red, _green, _blue);
        }
        /// <summary>
        /// Returns a version of the color scaled so the brightest channel becomes 1.
        /// </summary>
        public ColorR8G8B8 AsScaled() {
            float max = System.MathF.Max(m_red, MathF.Max(m_green, m_blue));
            if ( max <= 1f )
                return this;

            return new ColorR8G8B8(m_red / max, m_green / max, m_blue / max);
        }


        /// <summary>
        /// Addition  another color in r, g, b channels with this one
        /// </summary>
        /// <param name="a">The red color channel</param>
        /// <param name="b">The green color channel</param>
        /// <param name="c">The blue color channel</param>
        /// <returns>this</returns>
        public ColorR8G8B8 Addition(float a, float b, float c) {
            return Addition(new ColorR8G8B8(a, b, c));
        }
        /// <summary>
        /// Subtraction  another color in r, g, b channels with this one
        /// </summary>
        /// <param name="a">The red color channel</param>
        /// <param name="b">The green color channel</param>
        /// <param name="c">The blue color channel</param>
        /// <returns>this</returns>
        public ColorR8G8B8 Subtraction(float a, float b, float c) {
            return Subtraction(new ColorR8G8B8(a, b, c));
        }
        /// <summary>
        /// Multiplication  another color in r, g, b channels with this one
        /// </summary>
        /// <param name="a">The red color channel</param>
        /// <param name="b">The green color channel</param>
        /// <param name="c">The blue color channel</param>
        /// <returns>this</returns>
        public ColorR8G8B8 Multiplication(float a, float b, float c) {
            return Multiplication(new ColorR8G8B8(a, b, c));
        }
        /// <summary>
        /// Division  another color in r, g, b channels with this one
        /// </summary>
        /// <param name="a">The red color channel</param>
        /// <param name="b">The green color channel</param>
        /// <param name="c">The blue color channel</param>
        /// <returns>this</returns>
        public ColorR8G8B8 Division(float a, float b, float c) {
            return Division(new ColorR8G8B8(a, b, c));
        }

        /// <summary>
        /// Computes the perceived brightness of the color.
        /// </summary>
        public float Brightness() {
            return m_red * 0.299f + m_green * 0.587f + m_blue * 0.114f;
        }
        /// <summary>
        /// Computes the brightness using linear RGB.
        /// </summary>
        public float LinearBrightness() {
            ColorR8G8B8 lin = this.ToLinear();
            return lin.Red * 0.2126f + lin.Green * 0.7152f + lin.Blue * 0.0722f;
        }
        /// <summary>
        /// Measures the visual contrast between this color and another.
        /// </summary>
        public float Contrast(ColorR8G8B8 other) {
            return System.MathF.Abs(Brightness() - other.Brightness());
        }
        /// <summary>
        /// Classifies the contrast between this color and another.
        /// </summary>
        public ContrastLevel GetKontrast(ColorR8G8B8 other) {
            float c = Contrast(other);

            if ( c < 0.20f )
                return ContrastLevel.VeryLow;

            if ( c < 0.40f )
                return ContrastLevel.Low;

            if ( c < 0.70f )
                return ContrastLevel.High;

            return ContrastLevel.Perfect;
        }
        /// <summary>
        /// Generates a color that visually contrasts with this one.
        /// </summary>
        public ColorR8G8B8 GetNextContrastColor() {
            
            ColorHSV hsv = this.ToColorHSV();
            hsv.H = (hsv.H + 180f) % 360f;
            hsv.V = 1f - hsv.V;
            return hsv.ToColorR8G8B8();
        }
        /// <summary>
        /// Returns the darkest components of two colors.
        /// </summary>
        public static ColorR8G8B8 Min(ColorR8G8B8 a, ColorR8G8B8 b) {
            return new ColorR8G8B8 (System.Math.Min(a.m_red, b.m_red), System.Math.Min(a.m_green, b.m_green), System.Math.Min(a.m_blue, b.m_blue));
        }
        /// <summary>
        /// Returns the brightest components of two colors.
        /// </summary>
        public static ColorR8G8B8 Max(ColorR8G8B8 a, ColorR8G8B8 b) {
            return new ColorR8G8B8(System.Math.Max(a.m_red, b.m_red), System.Math.Max(a.m_green, b.m_green), System.Math.Max(a.m_blue, b.m_blue));
        }
        /// <summary>
        /// Blends this color toward another by the given amount.
        /// </summary>
        public ColorR8G8B8 Lerp(ColorR8G8B8 value, float amount) {
            // this + amount * (value - this);

            return new ColorR8G8B8(
                m_red + amount * (value.m_red - m_red), 
                m_green + amount * (value.m_green - m_green), 
                m_blue + amount * (value.m_blue - m_blue)
            );
        }
    }
    /// @}
}