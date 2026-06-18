namespace SystemEx.Drawing {
    public class ColorXYZ : IEquatable<ColorXYZ> {
        private float m_x;
        private float m_y;
        private float m_z;

        public float X { get => m_x; set => m_x = value;  }
        public float Y { get => m_y; set => m_y = value;  }
        public float Z { get => m_z; set => m_z = value; }

        public ColorXYZ(float x, float y, float z) {
            this.m_x = x;
            this.m_y = y;
            this.m_z = z;
        }
        
        
        public ColorR8G8B8 ToColorRGB() {
            float X = this.m_x;
            float Y = this.m_y;
            float Z = this.m_z;

            // 1. XYZ → linear RGB
            float r_lin =  3.2404542f * X - 1.5371385f * Y - 0.4985314f * Z;
            float g_lin = -0.9692660f * X + 1.8760108f * Y + 0.0415560f * Z;
            float b_lin =  0.0556434f * X - 0.2040259f * Y + 1.0572252f * Z;

            // 2. linear RGB → sRGB (Gamma)
            float r = (r_lin <= 0.0031308f) ? 12.92f * r_lin : 1.055f * MathF.Pow(r_lin, 1f / 2.4f) - 0.055f;
            float g = (g_lin <= 0.0031308f) ? 12.92f * g_lin : 1.055f * MathF.Pow(g_lin, 1f / 2.4f) - 0.055f;
            float b = (b_lin <= 0.0031308f) ? 12.92f * b_lin : 1.055f * MathF.Pow(b_lin, 1f / 2.4f) - 0.055f;

            // 3. Clamp (sRGB muss 0..1 sein)
            r = r < 0f ? 0f : (r > 1f ? 1f : r);
            g = g < 0f ? 0f : (g > 1f ? 1f : g);
            b = b < 0f ? 0f : (b > 1f ? 1f : b);

            return new ColorR8G8B8(r, g, b);
        }

        public bool Equals(ColorXYZ? other) {
            if(other == null) return false;

            return m_x.Equals(other.m_x) || 
                m_y.Equals(other.m_y) || 
                m_z.Equals(other.m_z);
        }
        public override bool Equals(object? obj) {
            if(obj == null) return false;
            if(obj is ColorXYZ)  return Equals(obj as ColorXYZ);
            return false;
        }
        public override int GetHashCode() {
            return m_x.GetHashCode() ^ m_y.GetHashCode() ^ m_z.GetHashCode();
        }
        public override string ToString() {
            return string.Create(null, stackalloc char[256], $"[{m_x}, {m_y}, {m_z}]");
        }
    }
}