namespace SystemEx.Drawing {
    /// <summary>
    /// Represents a color in the CIE XYZ color space using floating‑point components.
    /// Provides basic construction, comparison, and string formatting utilities.
    /// </summary>
    public class ColorXYZ : IEquatable<ColorXYZ> {
        private float m_x;
        private float m_y;
        private float m_z;

        /// <summary>
        /// Gets or sets the X component of the XYZ color.
        /// </summary>
        public float X { get => m_x; set => m_x = value; }

        /// <summary>
        /// Gets or sets the Y component of the XYZ color.
        /// </summary>
        public float Y { get => m_y; set => m_y = value; }

        /// <summary>
        /// Gets or sets the Z component of the XYZ color.
        /// </summary>
        public float Z { get => m_z; set => m_z = value; }

        /// <summary>
        /// Initializes a new XYZ color with the specified component values.
        /// </summary>
        /// <param name="x">The X component.</param>
        /// <param name="y">The Y component.</param>
        /// <param name="z">The Z component.</param>
        public ColorXYZ(float x, float y, float z) {
            this.m_x = x;
            this.m_y = y;
            this.m_z = z;
        }

        /// <summary>
        /// Determines whether this instance is equal to another XYZ color.
        /// </summary>
        /// <param name="other">The color to compare with.</param>
        /// <returns>
        /// <c>true</c> if the components match; otherwise <c>false</c>.
        /// </returns>
        public bool Equals(ColorXYZ? other) {
            if ( other == null )
                return false;

            return m_x.Equals(other.m_x) &&
                   m_y.Equals(other.m_y) &&
                   m_z.Equals(other.m_z); 
        }
        /// <summary>
        /// Determines whether this instance is equal to another object.
        /// </summary>
        public override bool Equals(object? obj) {
            if(obj == null) return false;
            if(obj is ColorXYZ)  return Equals(obj as ColorXYZ);
            return false;
        }
        /// <summary>
        /// Returns a hash code based on the X, Y, and Z components.
        /// </summary>
        public override int GetHashCode() {
            return m_x.GetHashCode() ^ m_y.GetHashCode() ^ m_z.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of the XYZ color.
        /// </summary>
        public override string ToString() {
            return string.Create(null, stackalloc char[256], $"[{m_x}, {m_y}, {m_z}]");
        }
    }
}