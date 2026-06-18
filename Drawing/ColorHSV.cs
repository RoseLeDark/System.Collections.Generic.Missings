using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;

namespace SystemEx.Drawing {
    public class ColorHSV : IColor<ColorHSV>, IEquatable<ColorHSV> {
        internal float m_hue;   // 0–360°
        internal float m_saturation;   // 0–1
        internal float m_value;   // 0–1

        public virtual float H {  get => m_hue; set => m_hue = ClampHue(value); }
        public virtual float V { get => m_value; set => m_value = System.Math.Clamp(value, 0.0f, 1.0f); }
        public virtual float S { get => m_saturation; set => m_saturation = System.Math.Clamp(value, 0.0f, 1.0f); }

        public ColorHSV(float h, float s, float v) {
            H = h;
            S = s;
            V = v;
        }

        public ColorR8G8B8 ToColorRGB()  {
            float C = m_value * m_saturation;        // Chroma
            float Hp = Math.FMod(m_hue / 60f, 6f);  // Hue' (0..6)
            float X = C * (1f - System.Math.Abs(Math.FMod(Hp, 2f) - 1f));
            float m = m_value - C;

            float r, g, b;

            if ( Hp < 1f ) { r = C; g = X; b = 0; } 
            else if ( Hp < 2f ) { r = X; g = C; b = 0; } 
            else if ( Hp < 3f ) { r = 0; g = C; b = X; } 
            else if ( Hp < 4f ) { r = 0; g = X; b = C; } 
            else if ( Hp < 5f ) { r = X; g = 0; b = C; } 
            else if ( Hp < 6f ) { r = C; g = 0; b = X; } 
            else { r = 0; g = 0; b = 0; }

            return new ColorR8G8B8(r + m, g + m, b + m);
        }
        public ColorXYZ ToColorXYZ() {
            // 1. HSV → RGB
            ColorR8G8B8 rgb = ToColorRGB();

            // 2. sRGB → Linear RGB
            ColorR8G8B8 lin = rgb.ToLinear();

            float r = lin.Red;
            float g = lin.Green;
            float b = lin.Blue;

            // 3. Linear RGB → XYZ
            float X = 0.4124564f * r + 0.3575761f * g + 0.1804375f * b;
            float Y = 0.2126729f * r + 0.7151522f * g + 0.0721750f * b;
            float Z = 0.0193339f * r + 0.1191920f * g + 0.9503041f * b;

            return new ColorXYZ(X, Y, Z);
        }
        public ColorHSL ToColorHSL() {
            float H = m_hue;
            float S = m_saturation;
            float V = m_value;

            // Lightness
            float L = V * (1f - S * 0.5f);

            // Saturation in HSL
            float S_hsl = 0f;

            if ( L > 0f && L < 1f ) {
                float minL = (L < 0.5f) ? L : (1f - L);
                if ( minL > 0f )
                    S_hsl = (V - L) / minL;
            }

            return new ColorHSL(H, S_hsl, L);
        }



        public virtual ColorHSV Saturation(float delta) {
            m_saturation = System.Math.Clamp(m_saturation + delta, 0.0f, 1.0f);
            return this;
        }

        public virtual ColorHSV Brightness(float delta) {
            m_value = System.Math.Clamp(m_value + delta, 0.0f, 1.0f);
            return this;
        }

        public virtual ColorHSV Addition(ColorHSV a) {
            m_saturation = System.Math.Clamp(m_saturation + a.m_saturation, 0.0f, 1.0f);
            m_value = System.Math.Clamp(m_value + a.m_value, 0.0f, 1.0f);
            m_hue = ClampHue(m_hue + a.m_hue);
            return this;
        }

        public virtual ColorHSV Subtraction(ColorHSV a) {
            m_saturation = System.Math.Clamp(m_saturation - a.m_saturation, 0.0f, 1.0f);
            m_value = System.Math.Clamp(m_value - a.m_value, 0.0f, 1.0f);
            m_hue = ClampHue(m_hue - a.m_hue);
            return this;
        }

        public virtual ColorHSV Multiplication(ColorHSV a) {
            m_saturation = System.Math.Clamp(m_saturation * a.m_saturation, 0.0f, 1.0f);
            m_value = System.Math.Clamp(m_value * a.m_value, 0.0f, 1.0f);

            return this;
        }

        public virtual ColorHSV Division(ColorHSV a) {

            if ( a.m_saturation != 0 ) m_saturation = System.Math.Clamp(m_saturation / a.m_saturation, 0.0f, 1.0f);
            if ( a.m_value != 0 )       m_value = System.Math.Clamp(m_value / a.m_value, 0.0f, 1.0f);

            return this;
        }

        public ColorHSV Addition(float a, float b, float c) {
            return Addition(new ColorHSV(a, b, c));
        }

        public ColorHSV Subtraction(float a, float b, float c) {
            return Subtraction(new ColorHSV(a, b, c));
        }

        public ColorHSV Multiplication(float a, float b, float c) {
            return Multiplication(new ColorHSV(a, b, c));
        }

        public ColorHSV Division(float a, float b, float c) {
            return Division(new ColorHSV(a, b, c));
        }

        public ColorHSV Lerp(ColorHSV value, float amount) {
            // Quadratische Kurve
            float q = amount * amount;

            // Hue shortest-path interpolation
            float dh = value.m_hue - m_hue;
            dh = (dh + 540f) % 360f - 180f;

            return new ColorHSV(
                ClampHue(m_hue + dh * q),
                m_saturation + (value.m_saturation - m_saturation) * q,
                m_value + (value.m_value - m_value) * q
            );
        }
        public bool Equals(ColorHSV? other) {
            if ( other == null ) return false;

            return other.m_hue == m_hue &&
                other.m_saturation == m_saturation &&
                other.m_value == m_value;
        }
        public override bool Equals(object? obj) {
            if(obj == null ) return false;

            if(obj is ColorHSV) {
                var c = obj as ColorHSV;
                return Equals(c);
            }
            return false;
        }
        public override int GetHashCode() {
            return m_hue.GetHashCode() ^ m_saturation.GetHashCode() ^ m_value.GetHashCode();
        }
        protected static float ClampHue(float h) {
            h %= 360f;
            return h < 0f ? h + 360f : h;
        }
        public static ColorHSV FromSystemColor(Color color) {
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;

            return FromRGBFloats(r, g, b);
        }
        public static ColorHSV FromColor(ColorR8G8B8 color) {
            return FromRGBFloats(color.Red, color.Green, color.Blue);
        }
        private static ColorHSV FromRGBFloats(float r, float g, float b) {
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

            return new ColorHSV(h, s, v);
        }

        public override string ToString() {
            return string.Create(null, stackalloc char[256], $"[{m_hue}, {m_saturation}, {m_value}]");
        }
    }
}
