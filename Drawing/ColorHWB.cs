using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Drawing {
    public class ColorHWB : IEquatable<ColorHWB> {
        private float m_hue;
        private float m_whiteness;
        private float m_blackness;

        public float Hue => m_hue;
        public float Whiteness => m_whiteness;
        public float Blackness => m_blackness;

        /// <summary>
        /// Initializes a new HWB color with the specified component values.
        /// </summary>
        public ColorHWB(float h, float whiteness, float blackness) { 
            m_hue = h;
            m_whiteness = whiteness;   
            m_blackness = blackness;
        }
        public ColorHWB(float[] x) {
            m_hue = x[0];
            m_whiteness = x[1];
            m_blackness = x[2];
        }

        /// <summary>
        /// Determines whether this instance is equal to another YUV color.
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
            if ( obj == null ) return false;
            if ( obj is ColorHWB) return Equals(obj as ColorHWB);
            return false;
        }
        /// <summary>
        /// Returns a string representation of the HWB color.
        /// </summary>
        public override string ToString() {
            return string.Create(null, stackalloc char[256], $"[{m_hue}, {m_whiteness}, {m_blackness}]");
        }

        
    }
}
