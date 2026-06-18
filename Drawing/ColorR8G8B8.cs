using System.Diagnostics.Contracts;

namespace SystemEx.Drawing {

    public enum KontrastLevel { 
        VeryLow, 
        Low, 
        High, 
        Perfect 
    }

    public class ColorR8G8B8 : IColor<ColorR8G8B8> {
        private float m_red;
        private float m_green;
        private float m_blue;

        public float Red => m_red;
        public float Green => m_green;
        public float Blue => m_blue;

        public bool IsLight => Brightness() >= 0.5f;

        public bool IsDark => Brightness() < 0.5f;

        public ColorR8G8B8() {
            m_red = 0.0f;
            m_green = 0.0f;
            m_blue = 0.0f;
        }
        public ColorR8G8B8(int rgb) {
            byte r = (byte)((rgb >> 16) & 0xFF);
            byte g = (byte)((rgb >> 8) & 0xFF);
            byte b = (byte)(rgb & 0xFF);

            m_red = r * 0.003921568627450980392156862745098f;
            m_green = g * 0.003921568627450980392156862745098f;
            m_blue = b * 0.003921568627450980392156862745098f;
        }
        public ColorR8G8B8(byte r, byte g, byte b) {
            m_red = r * 0.003921568627450980392156862745098f;
            m_blue = b * 0.003921568627450980392156862745098f;
            m_green = g * 0.003921568627450980392156862745098f;
        }
        public ColorR8G8B8(float rgb) {
            this.m_red = rgb;
            this.m_green = rgb;
            this.m_blue = rgb;
        }
        public ColorR8G8B8(float r, float g, float b) {
            this.m_red = r;
            this.m_green = g;
            this.m_blue = b;
        }
        public ColorR8G8B8 ToLinear() {
            float r = (Red   <= 0.04045f) ? (Red   / 12.92f) : MathF.Pow((Red   + 0.055f) / 1.055f, 2.4f);
            float g = (Green <= 0.04045f) ? (Green / 12.92f) : MathF.Pow((Green + 0.055f) / 1.055f, 2.4f);
            float b = (Blue  <= 0.04045f) ? (Blue  / 12.92f) : MathF.Pow((Blue  + 0.055f) / 1.055f, 2.4f);

            return new ColorR8G8B8(r, g, b);
        }

        public ColorXYZ ToColorXYZ() {
            ColorR8G8B8 lin = ToLinear();

            return new ColorXYZ(
                0.4124564f * lin.Red + 0.3575761f * lin.Green + 0.1804375f * lin.Blue,
                0.2126729f * lin.Red + 0.7151522f * lin.Green + 0.0721750f * lin.Blue,
                0.0193339f * lin.Red + 0.1191920f * lin.Green + 0.9503041f * lin.Blue
            );
        }
        public override string ToString() {
            return string.Create(null, stackalloc char[256], $"[{m_red}, {m_green}, {m_blue}]");
        }

        public ColorR8G8B8 Saturation(float delta) {
            var hsl = ColorHSV.FromColor(this);
            var rgb = hsl.Saturation(delta).ToColorRGB();

            m_red = rgb.m_red;
            m_blue = rgb.m_blue;
            m_green = rgb.m_green;

            return this;
        }

        public ColorR8G8B8 Brightness(float delta) {
            var hsl = ColorHSV.FromColor(this);
            var rgb = hsl.Brightness(delta).ToColorRGB();

            m_red = rgb.m_red;
            m_blue = rgb.m_blue;
            m_green = rgb.m_green;

            return this;
        }

        public ColorR8G8B8 Addition(ColorR8G8B8 value) {
            m_red       += value.m_red; 
            m_green     += value.m_green; 
            m_blue      += value.m_blue;

            return this;
        }

        public ColorR8G8B8 Subtraction(ColorR8G8B8 value) {
            m_red       -= value.m_red;
            m_green     -= value.m_green;
            m_blue      -= value.m_blue;

            return this;
        }

        public ColorR8G8B8 Multiplication(ColorR8G8B8 value) {
            m_red       *= value.m_red;
            m_green     *= value.m_green;
            m_blue      *= value.m_blue;

            return this;
        }

        public ColorR8G8B8 Division(ColorR8G8B8 value) {
            if ( value.m_red != 0 )     m_red /= value.m_red;
            if ( value.m_green != 0 )   m_green /= value.m_green;
            if ( value.m_blue != 0 )    m_blue /= value.m_blue;

            return this;
        }
        public ColorR8G8B8 AsNormalized() {
            float _red = System.Math.Clamp(m_red, 0.0f, 1.0f);
            float _green = System.Math.Clamp(m_green, 0.0f, 1.0f);
            float _blue = System.Math.Clamp(m_blue, 0.0f, 1.0f);

            return new ColorR8G8B8(_red, _green, _blue);
        }
        public ColorR8G8B8 AsScaled() {
            float max = System.MathF.Max(m_red, MathF.Max(m_green, m_blue));
            if ( max <= 1f )
                return this;

            return new ColorR8G8B8(m_red / max, m_green / max, m_blue / max);
        }



        public ColorR8G8B8 Addition(float a, float b, float c) {
            return Addition(new ColorR8G8B8(a, b, c));
        }

        public ColorR8G8B8 Subtraction(float a, float b, float c) {
            return Subtraction(new ColorR8G8B8(a, b, c));
        }

        public ColorR8G8B8 Multiplication(float a, float b, float c) {
            return Multiplication(new ColorR8G8B8(a, b, c));
        }

        public ColorR8G8B8 Division(float a, float b, float c) {
            return Division(new ColorR8G8B8(a, b, c));
        }
        public float Brightness() {
            return m_red * 0.299f + m_green * 0.587f + m_blue * 0.114f;
        }
        public float LinearBrightness() {
            ColorR8G8B8 lin = ToLinear();
            return lin.Red * 0.2126f + lin.Green * 0.7152f + lin.Blue * 0.0722f;
        }
        public float Contrast(ColorR8G8B8 other) {
            return System.MathF.Abs(Brightness() - other.Brightness());
        }
        public KontrastLevel GetKontrast(ColorR8G8B8 other) {
            float c = Contrast(other);

            if ( c < 0.20f )
                return KontrastLevel.VeryLow;

            if ( c < 0.40f )
                return KontrastLevel.Low;

            if ( c < 0.70f )
                return KontrastLevel.High;

            return KontrastLevel.Perfect;
        }
        public ColorR8G8B8 GetNextContrastColor() {
            ColorHSV hsv = ColorHSV.FromColor(this);
            hsv.H = (hsv.H + 180f) % 360f;
            hsv.V = 1f - hsv.V;
            return hsv.ToColorRGB();
        }
        public static ColorR8G8B8 Min(ColorR8G8B8 a, ColorR8G8B8 b) {
            return new ColorR8G8B8 (System.Math.Min(a.m_red, b.m_red), System.Math.Min(a.m_green, b.m_green), System.Math.Min(a.m_blue, b.m_blue));
        }
        public static ColorR8G8B8 Max(ColorR8G8B8 a, ColorR8G8B8 b) {
            return new ColorR8G8B8(System.Math.Max(a.m_red, b.m_red), System.Math.Max(a.m_green, b.m_green), System.Math.Max(a.m_blue, b.m_blue));
        }

        public static ColorR8G8B8 FromYUV(float y, float u, float v) {
            float r = 1.164f * (y - 16) + 1.596f*(v - 128);
            float g = 1.164f * (y - 16) - 0.813f*(v - 128) - 0.391f*(u - 128);
            float b = 1.164f * (y - 16) + 2.018f*(u - 128);

            return new ColorR8G8B8(
                r * 0.003921568627450980392156862745098f, 
                g * 0.003921568627450980392156862745098f, 
                b * 0.003921568627450980392156862745098f);
        }

        public static ColorR8G8B8 FromCMY(float c, float m, float y) {
            return new ColorR8G8B8(1.0f - c, 1.0f - m, 1.0f - y);
        }

        public ColorR8G8B8 Lerp(ColorR8G8B8 value, float amount) {
            // this + amount * (value - this);

            return new ColorR8G8B8(
                m_red + amount * (value.m_red - m_red), 
                m_green + amount * (value.m_green - m_green), 
                m_blue + amount * (value.m_blue - m_blue)
            );
        }
    }
}