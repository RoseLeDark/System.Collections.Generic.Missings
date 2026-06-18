using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Drawing {

    /// <summary>
    /// Represents a YUV color using floating‑point components.
    /// Provides basic construction, comparison, and string formatting.
    /// </summary>
    public class ColorYUV : IEquatable<ColorYUV> {
        private float m_y;
        private float m_u;
        private float m_v;

        /// <summary>
        /// Gets or sets the Y (luma) component.
        /// </summary>
        public float Y { get => m_y; set => m_y = value; }

        /// <summary>
        /// Gets or sets the U (chrominance blue) component.
        /// </summary>
        public float U { get => m_u; set => m_u = value; }

        /// <summary>
        /// Gets or sets the V (chrominance red) component.
        /// </summary>
        public float V { get => m_v; set => m_v = value; }

        /// <summary>
        /// Initializes a new YUV color from an array of three floating‑point values.
        /// </summary>
        /// <param name="x">An array containing Y, U, and V in that order.</param>
        public ColorYUV(float[] x) {
            m_y = x[0];
            m_u = x[1];
            m_v = x[2]; 
        }
        /// <summary>
        /// Initializes a new YUV color with the specified component values.
        /// </summary>
        public ColorYUV(float y, float u, float v) { 
            m_y = y;
            m_u = u;
            m_v = v;
        }
        /// <summary>
        /// Determines whether this instance is equal to another YUV color.
        /// </summary>
        /// <param name="other">The color to compare with.</param>
        /// <returns>
        /// <c>true</c> if the components match; otherwise <c>false</c>.
        /// </returns>
        public bool Equals(ColorYUV other) {

            return m_y.Equals(other.m_y) &&
                m_u.Equals(other.m_u) &&
                m_v.Equals(other.m_y);
        }
        /// <summary>
        /// Returns a hash code based on the Y, U, and V components.
        /// </summary>
        public override int GetHashCode() {
            return HashCode.Combine(m_y, m_u, m_v);
        }
        /// <summary>
        /// Determines whether this instance is equal to another object.
        /// </summary>
        public override bool Equals(object? obj) {
            if ( obj == null ) return false;
            if ( obj is ColorYUV ) return Equals(obj as ColorYUV);
            return false;
        }
        /// <summary>
        /// Returns a string representation of the YUV color.
        /// </summary>
        public override string ToString() {
            return string.Create(null, stackalloc char[256], $"[{m_y}, {m_u}, {m_v}]");
        }
    }
}
