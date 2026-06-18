using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;

namespace SystemEx.Drawing {
    /// <summary>
    /// Represents a color in the HSV (Hue–Saturation–Value) color space using
    /// floating‑point components.  
    /// Provides hue‑aware interpolation, component manipulation, arithmetic
    /// operations, and normalization utilities.
    /// </summary>
    public class ColorHSV : IColor<ColorHSV>, IEquatable<ColorHSV> {
        internal float m_hue;         // 0–360°
        internal float m_saturation;  // 0–1
        internal float m_value;       // 0–1

        /// <summary>
        /// Gets or sets the hue component in degrees (0–360).  
        /// Values outside the range are wrapped automatically.
        /// </summary>
        public virtual float H { get => m_hue; set => m_hue = ClampHue(value); }

        /// <summary>
        /// Gets or sets the value (brightness) component in the range 0–1.
        /// </summary>
        public virtual float V { get => m_value; set => m_value = Math.Clamp(value, 0f, 1f); }

        /// <summary>
        /// Gets or sets the saturation component in the range 0–1.
        /// </summary>
        public virtual float S { get => m_saturation; set => m_saturation = Math.Clamp(value, 0f, 1f); }

        /// <summary>
        /// Initializes a new HSV color with the specified component values.
        /// </summary>
        public ColorHSV(float h, float s, float v) {
            H = h;
            S = s;
            V = v;
        }

        /// <summary>
        /// Adjusts the saturation by the specified delta.
        /// </summary>
        public virtual ColorHSV Saturation(float delta) {
            m_saturation = Math.Clamp(m_saturation + delta, 0f, 1f);
            return this;
        }

        /// <summary>
        /// Adjusts the value (brightness) by the specified delta.
        /// </summary>
        public virtual ColorHSV Brightness(float delta) {
            m_value = Math.Clamp(m_value + delta, 0f, 1f);
            return this;
        }

        /// <summary>
        /// Adds another HSV color to this one, component by component.
        /// Hue is wrapped, saturation and value are clamped.
        /// </summary>
        public virtual ColorHSV Addition(ColorHSV a) {
            m_saturation = Math.Clamp(m_saturation + a.m_saturation, 0f, 1f);
            m_value = Math.Clamp(m_value + a.m_value, 0f, 1f);
            m_hue = ClampHue(m_hue + a.m_hue);
            return this;
        }

        /// <summary>
        /// Subtracts another HSV color from this one, component by component.
        /// Hue is wrapped, saturation and value are clamped.
        /// </summary>
        public virtual ColorHSV Subtraction(ColorHSV a) {
            m_saturation = Math.Clamp(m_saturation - a.m_saturation, 0f, 1f);
            m_value = Math.Clamp(m_value - a.m_value, 0f, 1f);
            m_hue = ClampHue(m_hue - a.m_hue);
            return this;
        }

        /// <summary>
        /// Multiplies this color with another, component by component.
        /// Hue is not affected.
        /// </summary>
        public virtual ColorHSV Multiplication(ColorHSV a) {
            m_saturation = Math.Clamp(m_saturation * a.m_saturation, 0f, 1f);
            m_value = Math.Clamp(m_value * a.m_value, 0f, 1f);
            return this;
        }

        /// <summary>
        /// Divides this color by another, component by component.
        /// Hue is not affected.
        /// </summary>
        public virtual ColorHSV Division(ColorHSV a) {
            if ( a.m_saturation != 0f )
                m_saturation = Math.Clamp(m_saturation / a.m_saturation, 0f, 1f);

            if ( a.m_value != 0f )
                m_value = Math.Clamp(m_value / a.m_value, 0f, 1f);

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
        public bool Equals(ColorHSV? other) {
            if ( other == null )
                return false;

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
        protected static float ClampHue(float h) {
            h %= 360f;
            return h < 0f ? h + 360f : h;
        }

        /// <summary>
        /// Returns a string representation of the HSV color.
        /// </summary>
        public override string ToString() {
            return string.Create(null, stackalloc char[256], $"[{m_hue}, {m_saturation}, {m_value}]");
        }
    }
}
