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
    /// \addtogroup color
    /// @{
    /// <summary>
    /// Provides conversion utilities between different color spaces such as
    /// RGB, Linear RGB, XYZ, HSV, HSL, YUV, CMY and HDR.
    /// </summary>
    public static class ColorConverter {
        /// <summary>
        /// Converts an sRGB color to CIE XYZ using the D65 reference white.
        /// </summary>
        /// <param name="rgb">The sRGB color to convert.</param>
        public static ColorXYZ ToColorXYZ(this ColorR8G8B8 rgb) {
            ColorR8G8B8 lin = rgb.ToLinear();

            return new ColorXYZ(
                0.4124564f * lin.Red + 0.3575761f * lin.Green + 0.1804375f * lin.Blue,
                0.2126729f * lin.Red + 0.7151522f * lin.Green + 0.0721750f * lin.Blue,
                0.0193339f * lin.Red + 0.1191920f * lin.Green + 0.9503041f * lin.Blue
            );
        }
        /// <summary>
        /// Converts an sRGB color to linear RGB using the standard sRGB transfer function.
        /// </summary>
        /// <param name="rgb">The sRGB color.</param>
        /// <returns>The linear RGB representation.</returns>
        public static ColorR8G8B8 ToLinear(this ColorR8G8B8 rgb) {
            float r = (rgb.Red   <= 0.04045f) ? (rgb.Red   / 12.92f) : MathF.Pow((rgb.Red   + 0.055f) / 1.055f, 2.4f);
            float g = (rgb.Green <= 0.04045f) ? (rgb.Green / 12.92f) : MathF.Pow((rgb.Green + 0.055f) / 1.055f, 2.4f);
            float b = (rgb.Blue  <= 0.04045f) ? (rgb.Blue  / 12.92f) : MathF.Pow((rgb.Blue  + 0.055f) / 1.055f, 2.4f);

            return new ColorR8G8B8(r, g, b);
        }
        /// <summary>
        /// Converts a CIE XYZ color to sRGB using the D65 reference white.
        /// </summary>
        /// <param name="c">The XYZ color.</param>
        /// <returns>The corresponding sRGB color.</returns>
        public static ColorR8G8B8 ToColorR8G8B8(this ColorXYZ c) {
            float X = c.X;
            float Y = c.Y;
            float Z = c.Z;

            // XYZ → linear RGB
            float r_lin =  3.2404542f * X - 1.5371385f * Y - 0.4985314f * Z;
            float g_lin = -0.9692660f * X + 1.8760108f * Y + 0.0415560f * Z;
            float b_lin =  0.0556434f * X - 0.2040259f * Y + 1.0572252f * Z;

            // linear RGB → sRGB (Gamma)
            float r = (r_lin <= 0.0031308f) ? 12.92f * r_lin : 1.055f * MathF.Pow(r_lin, 1f / 2.4f) - 0.055f;
            float g = (g_lin <= 0.0031308f) ? 12.92f * g_lin : 1.055f * MathF.Pow(g_lin, 1f / 2.4f) - 0.055f;
            float b = (b_lin <= 0.0031308f) ? 12.92f * b_lin : 1.055f * MathF.Pow(b_lin, 1f / 2.4f) - 0.055f;

            // Clamp (sRGB muss 0..1 sein)
            return new ColorR8G8B8(r < 0f ? 0f : (r > 1f ? 1f : r),
                                    g < 0f ? 0f : (g > 1f ? 1f : g),
                                    b < 0f ? 0f : (b > 1f ? 1f : b)
                                   );
        }
        /// <summary>
        /// Converts a YUV color to sRGB using the BT.601 conversion matrix.
        /// </summary>
        /// <param name="yuv">The YUV color.</param>
        /// <returns>The corresponding sRGB color.</returns>
        public static ColorR8G8B8 ToColorR8G8B8(this ColorYUV yuv) {
            float r = 1.164f * (yuv.Y - 16) + 1.596f*(yuv.V - 128);
            float g = 1.164f * (yuv.Y - 16) - 0.813f*(yuv.V - 128) - 0.391f*(yuv.U - 128);
            float b = 1.164f * (yuv.Y - 16) + 2.018f*(yuv.U - 128);

            return new ColorR8G8B8(
                r * 0.003921568627450980392156862745098f,
                g * 0.003921568627450980392156862745098f,
                b * 0.003921568627450980392156862745098f);
        }
        /// <summary>
        /// Converts a CMY color to sRGB.
        /// </summary>
        public static ColorR8G8B8 ToColorR8G8B8(this ColorCMY color) {
            return new ColorR8G8B8(1.0f - color.C, 1.0f - color.M, 1.0f - color.Y);
        }
        /// <summary>
        /// Converts a grayscale color to sRGB.
        /// </summary>
        public static ColorR8G8B8 ToColorR8G8B8(this ColorGray color) {
            return new ColorR8G8B8(color.Gray, color.Gray, color.Gray);
        }
        /// <summary>
        /// Converts an HSV color to grayscale using the value component.
        /// </summary>
        public static ColorGray ToColorGray(this ColorHSV color) {
            return new ColorGray(color.V);
        }
        /// <summary>
        /// Converts an HDR color to HSV, clamping the value to 0–1.
        /// </summary>
        public static ColorHSV ToColorHSV(this ColorHDR color) {
            return new ColorHSV(color.H, color.S, System.Math.Clamp(color.V, 0.0f, 1.0f));
        }
        /// <summary>
        /// Converts an sRGB color to HDR via HSV.
        /// </summary>
        public static ColorHDR ToColorHDR(this ColorR8G8B8 x) {
            ColorHSV hsv = x.ToColorHSV();
            return hsv.ToColorHDR();
        }
        /// <summary>
        /// Converts an HSV color to HDR.
        /// </summary>
        public static ColorHDR ToColorHDR(this ColorHSV x) {
            return new ColorHDR(x.H, x.S, x.V);
        }
        /// <summary>
        /// Converts an sRGB color to HSV.
        /// </summary>
        public static ColorHSV ToColorHSV(this ColorR8G8B8 x) {
            float max = System.Math.Max(x.Red, System.Math.Max(x.Green, x.Blue));
            float min = System.Math.Min(x.Red, System.Math.Min(x.Green, x.Blue));
            float delta = max - min;

            float h = 0f;
            float s = 0f;
            float v = max;

            if ( delta > 0f ) {
                s = delta / max;

                if ( max == x.Red )
                    h = 60f * (((x.Green - x.Blue) / delta) % 6f);
                else if ( max == x.Green )
                    h = 60f * (((x.Blue - x.Red) / delta) + 2f);
                else
                    h = 60f * (((x.Red - x.Green) / delta) + 4f);

                if ( h < 0f )
                    h += 360f;
            }

            return new ColorHSV(h, s, v);
        }
        /// <summary>
        /// Converts an sRGB color to HWB.
        /// </summary>
        public static ColorHWB ToColorHWB(this ColorR8G8B8 color) {
            var c = color.AsNormalized();

            float max = System.Math.Max(c.Red, System.Math.Max(c.Green, c.Blue));
            float min = System.Math.Min(c.Red, System.Math.Min(c.Green, c.Blue));
            float delta = max - min;

            float w = min;
            float b = 1 - max;
            float h;

            if ( delta == 0 )
                h = 0;
            else if ( max == c.Red )
                h = (c.Green - c.Blue) / delta;
            else if ( max == c.Green )
                h = 2 + (c.Blue - c.Red) / delta;
            else
                h = 4 + (c.Red - c.Green) / delta;

            h *= 60;
            if ( h < 0 ) h += 360;

            return new ColorHWB(h, w *= 100, b *= 100);
        }
        /// <summary>
        /// Converts an HWB color to sRGB.
        /// </summary>
        public static ColorR8G8B8 ToColorR8G8B8(this ColorHWB color) {
            float h = Math.FMod(color.H, 360.0f) / 60.0f;
            float w = color.Whiteness / 100.0f;
            float bl = color.Blackness / 100.0f;

            float v = 1 - bl;
            float s = v == 0 ? 0 : 1 - w / v;
            float c = v * s;
            float x = c * (1 - System.Math.Abs( Math.FMod (h, 2.0f) - 1f));
            float m = v - c;

            float r, g, b;
            if ( h < 1 ) { r = c; g = x; b = 0; } 
            else if ( h < 2 ) { r = x; g = c; b = 0; } 
            else if ( h < 3 ) { r = 0; g = c; b = x; } 
            else if ( h < 4 ) { r = 0; g = x; b = c; } 
            else if ( h < 5 ) { r = x; g = 0; b = c; } 
            else { r = c; g = 0; b = x; }

            return new ColorR8G8B8(
                ((r + m)),
                ((g + m)),
                ((b + m))
            );
        }

        /// <summary>
        /// Converts an HSV color to sRGB.
        /// </summary>
        public static ColorR8G8B8 ToColorR8G8B8(this ColorHSV color) {
            float C = color.V * color.S;        // Chroma
            float Hp = Math.FMod(color.H / 60f, 6f);  // Hue' (0..6)
            float X = C * (1f - System.Math.Abs(Math.FMod(Hp, 2f) - 1f));
            float m = color.V - C;

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
        /// <summary>
        /// Converts an HSV color to CIE XYZ.
        /// </summary>
        public static ColorXYZ ToColorXYZ(this ColorHSV color) {
            ColorR8G8B8 rgb = color.ToColorR8G8B8();
            return rgb.ToColorXYZ();
        }
        /// <summary>
        /// Converts an HSV color to HSL.
        /// </summary>
        public static ColorHSL ToColorHSL(this ColorHSV color) {
            // Lightness
            float L = color.V * (1f - color.S * 0.5f);

            // Saturation in HSL
            float S_hsl = 0f;

            if ( L > 0f && L < 1f ) {
                float minL = (L < 0.5f) ? L : (1f - L);
                if ( minL > 0f )
                    S_hsl = (color.V - L) / minL;
            }

            return new ColorHSL(color.H, S_hsl, L);
        }
        /// <summary>
        /// Converts an HSV color to a 16‑bit RGB color (R16G16B16).
        /// Internally converts HSV → sRGB → R16G16B16.
        /// </summary>
        public static ColorR16G16B16 ToColorR16G16B16(this ColorHSV color) {
            ColorR8G8B8 rgb = color.ToColorR8G8B8();
            return new ColorR16G16B16(rgb.Red, rgb.Green, rgb.Blue);
        }
        /// <summary>
        /// Converts a 16‑bit RGB color (R16G16B16) to HSV.
        /// Internally converts R16G16B16 → sRGB → HSV.
        /// </summary>
        public static ColorHSV ToColorHSV(this ColorR16G16B16 color) {
            return new ColorR8G8B8(color.Red, color.Green, color.Blue).ToColorHSV();
        }
        /// <summary>
        /// Converts a 16‑bit RGB color (R16G16B16) from sRGB to linear RGB
        /// using the standard sRGB transfer function.
        /// </summary>
        public static ColorR16G16B16 ToLinear(this ColorR16G16B16 color) {
            float r = (color.Red   <= 0.04045f) ? (color.Red   / 12.92f) : System.MathF.Pow((color.Red   + 0.055f) / 1.055f, 2.4f);
            float g = (color.Green <= 0.04045f) ? (color.Green / 12.92f) : System.MathF.Pow((color.Green + 0.055f) / 1.055f, 2.4f);
            float b = (color.Blue  <= 0.04045f) ? (color.Blue  / 12.92f) : System.MathF.Pow((color.Blue  + 0.055f) / 1.055f, 2.4f);

            return new ColorR16G16B16(r, g, b);
        }
        /// <summary>
        /// Converts a 16‑bit RGB color (R16G16B16) to a 10‑bit RGB holder (R10G10B10).
        /// No quantization is applied; values remain normalized floats.
        /// </summary>
        public static ColorR10G10B10A2 ToColorR10G10B10(this ColorR16G16B16 color) {
            return new ColorR10G10B10A2(color.R, color.G, color.B);
        }
        /// <summary>
        /// Converts a 10‑bit RGB holder (R10G10B10) to a 16‑bit RGB color (R16G16B16).
        /// No quantization is applied; values remain normalized floats.
        /// </summary>
        public static ColorR16G16B16 ToColorR16G16B16(this ColorR10G10B10A2 color) {
            return new ColorR16G16B16(color.R, color.G, color.B);
        }
        /// <summary>
        /// Converts an HSV color to sRGB.
        /// </summary>
        public static ColorNCol ToColorNCol(this  ColorR8G8B8 color) {
            var hwb = color.ToColorHWB();

            byte     hueIndex = (byte) ((hwb.H / 60) % 6);
            byte     huePercent = (byte) (Math.FMod(hwb.H, 60) / 60 * 100) ;

            char N = ColorNCol.HUENAME[hueIndex];

            return new ColorNCol(hueIndex, huePercent, hwb.Whiteness, (100 - hwb.Blackness));
        }
        /// <summary>
        /// Converts an NCol color to sRGB.
        /// </summary>
        public static ColorR8G8B8 ToColorR8G8B8(this ColorNCol color) {
            byte hueIndex = color.I;
            byte huePercent = color.P;

            float h = Math.FMod(hueIndex * 60f + huePercent * 0.6f, 360f);
            float w = System.Math.Clamp(color.C, 0.0f, 100.0f);
            float b = System.Math.Clamp(color.L, 0.0f, 100.0f);

            var nwb = new ColorHWB(h, w, b);

            return nwb.ToColorR8G8B8();
        }
        /// <summary>
        /// Converts an NCol color to RGB16.
        /// </summary>
        public static ColorR16G16B16 ToColorR16G16B16(this ColorNCol color) {
            ColorR8G8B8 rgb = color.ToColorR8G8B8();
            return new ColorR16G16B16(rgb.Red, rgb.Green, rgb.Blue);
        }
        /// <summary>
        /// Converts an NCol color to RGB10.
        /// </summary>
        public static ColorR10G10B10A2 ToColorR10G10B10(this ColorNCol color) {
            ColorR8G8B8 rgb = color.ToColorR8G8B8();
            return new ColorR10G10B10A2(rgb.Red, rgb.Green, rgb.Blue);
        }
        /// <summary>
        /// Converts an Gray color to HSV.
        /// </summary>
        public static ColorHSV ToColorHSV(this ColorGray color) {
            return new ColorHSV(color.Gray, color.Gray, color.Gray);
        }
    }
    /// @}

}
