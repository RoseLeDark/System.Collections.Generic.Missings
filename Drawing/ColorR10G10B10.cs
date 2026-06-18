using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.SystemEx.Drawing {

    /// <summary>
    /// Represents a 10‑bit per channel RGB color (R10G10B10),
    /// stored internally as normalized floating‑point values (0–1).
    /// This type acts purely as a transport/holder format.
    /// </summary>
    public class ColorR10G10B10 : IEquatable<ColorR10G10B10> {
        private float m_r;
        private float m_g;
        private float m_b;

        /// <summary>Gets or sets the red component.</summary>
        public float R { get => m_r; set => m_r = value; }

        /// <summary>Gets or sets the green component.</summary>
        public float G { get => m_g; set => m_g = value; }

        /// <summary>Gets or sets the blue component.</summary>
        public float B { get => m_b; set => m_b = value; }

        /// <summary>
        /// Initializes a new R10G10B10 color from normalized float values (0–1).
        /// </summary>
        public ColorR10G10B10(float r, float g, float b) {
            m_r = r;
            m_g = g;
            m_b = b;
        }

        /// <summary>
        /// Initializes a new R10G10B10 color from 10‑bit UNORM components (0–1023).
        /// </summary>
        public ColorR10G10B10(ushort r10, ushort g10, ushort b10) {
            const float inv = 1f / 1023f;
            m_r = r10 * inv;
            m_g = g10 * inv;
            m_b = b10 * inv;
        }

        /// <summary>
        /// Determines whether this instance is equal to another R10G10B10 color.
        /// </summary>
        public bool Equals(ColorR10G10B10? other) {
            if ( other == null ) return false;

            return m_r.Equals(other.m_r) &&
                   m_g.Equals(other.m_g) &&
                   m_b.Equals(other.m_b);
        }

        /// <summary>
        /// Determines whether this instance is equal to another object.
        /// </summary>
        public override bool Equals(object? obj) {
            if ( obj is ColorR10G10B10 c )
                return Equals(c);

            return false;
        }

        /// <summary>
        /// Returns a hash code based on the R, G, and B components.
        /// </summary>
        public override int GetHashCode() {
            return System.HashCode.Combine(m_r, m_g, m_b);
        }

        /// <summary>
        /// Returns a string representation of the R10G10B10 color.
        /// </summary>
        public override string ToString() {
            return string.Create(null, stackalloc char[256], $"[{m_r}, {m_g}, {m_b}]");
        }
    }

}
