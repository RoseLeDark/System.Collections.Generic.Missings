using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using SystemEx.Drawing;

namespace SystemEx.SystemEx.Drawing {
    public class ColorHDR : IColor<ColorHDR> {
        private float m_maxValue = 10.0f;
        internal float m_hue;   // 0–360°
        internal float m_saturation;   // 0–1
        internal float m_value;   // 0–1

        public virtual float H { get => m_hue; set => m_hue = ClampHue(value); }
        public virtual float V { get => m_value; set => m_value = System.Math.Clamp(value, 0.0f, m_maxValue); }
        public virtual float S { get => m_saturation; set => m_saturation = System.Math.Clamp(value, 0.0f, 1.0f); }
        public virtual float MaxValue { get => m_maxValue; set => m_maxValue = value; }

        public ColorHDR(float h, float s, float v) {
            H = h;
            S = s;
            V = v;
        }

        public virtual ColorHDR Saturation(float delta) {
            m_saturation = System.Math.Clamp(m_saturation + delta, 0.0f, 1.0f);
            return this;
        }
        public virtual ColorHDR Brightness(float delta) {
            m_value = System.Math.Clamp(m_value + delta, 0.0f, m_maxValue);
            return this;
        }

        public virtual ColorHDR Addition(ColorHDR a) {
            m_saturation = System.Math.Clamp(m_saturation + a.m_saturation, 0.0f, 1.0f);
            m_value = System.Math.Clamp(m_value + a.m_value, 0.0f, m_maxValue);
            m_hue = ClampHue(m_hue + a.m_hue);
            return this;
        }

        public virtual ColorHDR Subtraction(ColorHDR a) {
            m_saturation = System.Math.Clamp(m_saturation - a.m_saturation, 0.0f, 1.0f);
            m_value = System.Math.Clamp(m_value - a.m_value, 0.0f, m_maxValue);
            m_hue = ClampHue(m_hue - a.m_hue);
            return this;
        }

        public virtual ColorHDR Multiplication(ColorHDR a) {
            m_saturation = System.Math.Clamp(m_saturation * a.m_saturation, 0.0f, 1.0f);
            m_value = System.Math.Clamp(m_value * a.m_value, 0.0f, m_maxValue);
      
            return this;
        }

        public virtual ColorHDR Division(ColorHDR a) {

            if ( a.m_saturation != 0 ) m_saturation = System.Math.Clamp(m_saturation / a.m_saturation, 0.0f, 1.0f);
            if ( a.m_value != 0 ) m_value = System.Math.Clamp(m_value / a.m_value, 0.0f, m_maxValue);
  
            return this;
        }

        public virtual ColorHDR LightIntensity(float intensity) {
            V += intensity;
            return this;
        }
        
      
        public ColorHSV ToHSV() {
            return new ColorHSV(m_hue, m_saturation, System.Math.Clamp(m_value, 0.0f, 1.0f));
        }


        public ColorHDR Addition(float a, float b, float c) {
            return Addition(new ColorHDR(a, b, c));
        }

        public ColorHDR Subtraction(float a, float b, float c) {
            return Subtraction(new ColorHDR(a, b, c));
        }

        public ColorHDR Multiplication(float a, float b, float c) {
            return Multiplication(new ColorHDR(a, b, c));
        }

        public ColorHDR Division(float a, float b, float c) {
            return Division(new ColorHDR(a, b, c));
        }

      
        protected static float ClampHue(float h) {
            h %= 360f;
            return h < 0f ? h + 360f : h;
        }
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
        public bool Equals(ColorHDR? other) {
            if ( other == null ) return false;

            return other.m_hue == m_hue &&
                other.m_saturation == m_saturation &&
                other.m_value == m_value;
        }
        public override bool Equals(object? obj) {
            if ( obj == null ) return false;

            if ( obj is ColorHDR ) {
                var c = obj as ColorHDR;
                return Equals(c);
            }
            return false;
        }
        public override int GetHashCode() {
            return m_hue.GetHashCode() ^ m_saturation.GetHashCode() ^ m_value.GetHashCode();
        }
        public static ColorHDR FromSystemColor(Color color) {
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;

            return FromRGBFloats(r, g, b);
        }
        public static ColorHDR FromColor(ColorR8G8B8 color) {
            return FromRGBFloats(color.Red, color.Green, color.Blue);
        }
        private static ColorHDR FromRGBFloats(float r, float g, float b) {
            float max = System.Math.Max(r, System.Math.Max(g, b));
            float min = System.Math.Min(r, System.Math.Min(g, b));
            float delta = max - min;

            float h = 0f;
            float s = 0f;
            float v = max;

            if ( delta > 0f ) {
                s = delta / max;

                if ( max == r )
                    h = 60f * (((g - b) / delta) % 6f);
                else if ( max == g )
                    h = 60f * (((b - r) / delta) + 2f);
                else
                    h = 60f * (((r - g) / delta) + 4f);

                if ( h < 0f )
                    h += 360f;
            }

            return new ColorHDR(h, s, v);
        }

        public override string ToString() {
            return string.Create(null, stackalloc char[256], $"[{m_hue}, {m_saturation}, {m_value}]");
        }
    }
}
