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
	/// Represents a color in the HSV (Hue–Saturation–Value) color space using
	/// floating‑point components.  
	/// Provides hue‑aware interpolation, component manipulation, arithmetic
	/// operations, and normalization utilities. 
	/// <Note> The Main Color in this Libary</Note>
	/// </summary>
	public struct ColorHSV : IColor<ColorHSV>, IEquatable<ColorHSV>, IComparable<ColorHSV> {
        internal float m_hue;         // 0–360°
        internal float m_saturation;  // 0–1
        internal float m_value;       // 0–1

        /// <summary>
        /// A static property that returns a ColorHSV instance representing black (H=0, S=0, V=0).
        /// </summary>
        /// <returns></returns>
        public static ColorHSV Zero => new ColorHSV(0f, 0f, 0f);

        /// <summary>
        /// A static property that returns a ColorHSV instance representing white (H=0, S=0, V=1).
        /// </summary>
        /// <returns></returns>
        public static ColorHSV One => new ColorHSV(0f, 0f, 1f);

        /// <summary>
        /// Gets or sets the hue component in degrees (0–360).  
        /// Values outside the range are wrapped automatically.
        /// </summary>
        public float H { get => m_hue; set => m_hue = ClampHue(value); }

        /// <summary>
        /// Gets or sets the value (brightness) component in the range 0–1.
        /// </summary>
        public float V { get => m_value; set => m_value = System.Math.Clamp(value, 0f, 1f); }

        /// <summary>
        /// Gets or sets the saturation component in the range 0–1.
        /// </summary>
        public float S { get => m_saturation; set => m_saturation = System.Math.Clamp(value, 0f, 1f); }

        /// <summary>
        /// Initializes a new HSV color with the specified component values.
        /// </summary>
        public ColorHSV(float h, float s, float v) {
            m_hue = ClampHue(h);
            m_saturation = System.Math.Clamp(s, 0f, 1f);
            m_value = System.Math.Clamp(v, 0f, 1f);
        }

        /// <summary>
        /// Adjusts the saturation by the specified delta.
        /// </summary>
        public ColorHSV Saturation(float delta) {
            m_saturation = System.Math.Clamp(m_saturation + delta, 0f, 1f);
            return this;
        }

        /// <summary>
        /// Adjusts the value (brightness) by the specified delta.
        /// </summary>
        public ColorHSV Brightness(float delta) {
            m_value = System.Math.Clamp(m_value + delta, 0f, 1f);
            return this;
        }

        /// <summary>
        /// Adds another HSV color to this one, component by component.
        /// Hue is wrapped, saturation and value are clamped.
        /// </summary>
        public ColorHSV Addition(ColorHSV a) {
            m_saturation = System.Math.Clamp(m_saturation + a.m_saturation, 0f, 1f);
            m_value = System.Math.Clamp(m_value + a.m_value, 0f, 1f);
            m_hue = ClampHue(m_hue + a.m_hue);
            return this;
        }

        /// <summary>
        /// Subtracts another HSV color from this one, component by component.
        /// Hue is wrapped, saturation and value are clamped.
        /// </summary>
        public ColorHSV Subtraction(ColorHSV a) {
            m_saturation = System.Math.Clamp(m_saturation - a.m_saturation, 0f, 1f);
            m_value = System.Math.Clamp(m_value - a.m_value, 0f, 1f);
            m_hue = ClampHue(m_hue - a.m_hue);
            return this;
        }

        /// <summary>
        /// Multiplies this color with another, component by component.
        /// Hue is not affected.
        /// </summary>
        public ColorHSV Multiplication(ColorHSV a) {
            m_saturation = System.Math.Clamp(m_saturation * a.m_saturation, 0f, 1f);
            m_value = System.Math.Clamp(m_value * a.m_value, 0f, 1f);
            return this;
        }

        /// <summary>
        /// Divides this color by another, component by component.
        /// Hue is not affected.
        /// </summary>
        public ColorHSV Division(ColorHSV a) {
            if ( a.m_saturation != 0f )
                m_saturation = System.Math.Clamp(m_saturation / a.m_saturation, 0f, 1f);

            if ( a.m_value != 0f )
                m_value = System.Math.Clamp(m_value / a.m_value, 0f, 1f);

            return this;
        }

        /// <summary>
        /// Performs a quadratic interpolation toward another HSV color.  
        /// Hue interpolation follows the shortest path around the color wheel.
        /// </summary>
        /// <param name="value">The target color.</param>
        /// <param name="amount">Interpolation factor in the range 0–1.</param>
        public ColorHSV Lerp(ColorHSV value, float amount) {
            float q = amount * amount;

            // Shortest-path hue interpolation
            float dh = value.m_hue - m_hue;
            dh = (dh + 540f) % 360f - 180f;

            return new ColorHSV(
                ClampHue(m_hue + dh * q),
                m_saturation + (value.m_saturation - m_saturation) * q,
                m_value + (value.m_value - m_value) * q
            );
        }

        /// <summary>
        /// Determines whether this instance is equal to another HSV color.
        /// </summary>
        public bool Equals(ColorHSV other) {

            return other.m_hue == m_hue &&
                   other.m_saturation == m_saturation &&
                   other.m_value == m_value;
        }

        /// <summary>
        /// Determines whether this instance is equal to another object.
        /// </summary>
        public override bool Equals(object? obj) {
            if ( obj is ColorHSV hsv )
                return Equals(hsv);

            return false;
        }

        /// <summary>
        /// Returns a hash code based on the hue, saturation, and value components.
        /// </summary>
        public override int GetHashCode() {
            return m_hue.GetHashCode() ^
                   m_saturation.GetHashCode() ^
                   m_value.GetHashCode();
        }

        /// <summary>
        /// Wraps a hue value into the range 0–360 degrees.
        /// </summary>
        private float ClampHue(float h) {
            h %= 360f;
            return h < 0f ? h + 360f : h;
        }

        /// <summary>
        /// Returns a string representation of the HSV color.
        /// </summary>
        public override string ToString() {
            return string.Create(null, stackalloc char[256], $"[{m_hue}, {m_saturation}, {m_value}]");
        }
        /// <summary>
        /// Addition  another color in r, g, b channels with this one
        /// </summary>
        public ColorHSV Addition(float a, float b, float c) {
            return Addition(new ColorHSV(a, b, c));
        }
        /// <summary>
        /// Subtraction  another color in r, g, b channels with this one
        /// </summary>
        public ColorHSV Subtraction(float a, float b, float c) {
            return Subtraction(new ColorHSV(a, b, c));
        }
        /// <summary>
        /// Multiplication  another color in r, g, b channels with this one
        /// </summary>
        public ColorHSV Multiplication(float a, float b, float c) {
            return Multiplication(new ColorHSV(a, b, c));
        }
        /// <summary>
        /// Divisionication  another color in r, g, b channels with this one
        /// </summary>
        public ColorHSV Division(float a, float b, float c) {
            return Division(new ColorHSV(a, b, c));
        }

        public int CompareTo(ColorHSV other) {
            if(this > other ) return -1;
            return 0;
        }

        public static bool operator ==(ColorHSV left, ColorHSV right) {
            return left.Equals(right);
        }

        public static bool operator !=(ColorHSV left, ColorHSV right) {
            return !(left == right);
        }

        public static bool operator <(ColorHSV left, ColorHSV right) {
            return left.GetHashCode() < right.GetHashCode();
        }

        public static bool operator <=(ColorHSV left, ColorHSV right) {
            return left.GetHashCode() <= right.GetHashCode();
        }

        public static bool operator >(ColorHSV left, ColorHSV right) {
            return left.GetHashCode() > right.GetHashCode();
        }

        public static bool operator >=(ColorHSV left, ColorHSV right) {
            return left.GetHashCode() >= right.GetHashCode();
        }
    }
    
}
