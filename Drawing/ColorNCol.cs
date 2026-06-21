using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Drawing {
    /// <summary>
    /// Represents a compact N‑Color classification based on the HWB color model.
    /// The color is expressed using a hue segment index (0–5), a percentage
    /// within that segment (0–100), and the whiteness/blackness components.
    /// </summary>
    public class ColorNCol : IEquatable<ColorNCol> {
        /// <summary>
        /// Hue name mapping for the six primary hue segments:
        /// R = Red, Y = Yellow, G = Green, C = Cyan, B = Blue, M = Magenta.
        /// </summary>
        static internal char[] HUENAME = { 'R', 'Y', 'G', 'C', 'B', 'M' };

        private float m_c;   // Whiteness (0–100)
        private float m_l;   // Lightness (100 - Blackness)
        private string m_n;  // Name string, e.g. "R20"

        /// <summary>
        /// Gets the hue segment index (0–5).
        /// </summary>
        public byte I { get; internal set; }

        /// <summary>
        /// Gets the percentage position within the hue segment (0–100).
        /// </summary>
        public byte P { get; internal set; }

        /// <summary>
        /// Gets the generated N‑Color name, e.g. "R20", "G55", "B03".
        /// </summary>
        public string N => m_n;

        /// <summary>
        /// Gets the whiteness component (0–100).
        /// </summary>
        public float C => m_c;

        /// <summary>
        /// Gets the lightness component (0–100).
        /// </summary>
        public float L => m_l;

        /// <summary>
        /// Erstellt eine neue N‑Color‑Struktur.
        /// </summary>
        public ColorNCol(byte index, byte percent, float c, float l) {
            I = index;
            P = percent;
            m_n = $"{HUENAME[index]}{percent}";
            m_c = c;    
            m_l = l;
        }
        /// <summary>
        /// Determines whether this instance is equal to another NCol color.
        /// </summary>
        /// <param name="other">The color to compare with.</param>
        /// <returns>
        /// <c>true</c> if the components match; otherwise <c>false</c>.
        /// </returns>
        public bool Equals(ColorNCol? other) {
            if ( other == null )
                return false;

            return m_c.Equals(other.m_c) &&
                   m_l.Equals(other.m_l) &&
                   m_n == (other.m_n);
        }
        /// <summary>
        /// Determines whether this instance is equal to another object.
        /// </summary>
        public override bool Equals(object? obj) {
            if ( obj == null ) return false;
            if ( obj is ColorNCol ) return Equals(obj as ColorNCol);
            return false;
        }
        /// <summary>
        /// Returns a hash code based on the C, L, and N components.
        /// </summary>
        public override int GetHashCode() {
            return m_c.GetHashCode() ^ m_l.GetHashCode() ^ m_n.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of the NCOL color.
        /// </summary>
        public override string ToString() {
            return string.Create(null, stackalloc char[16], $"[{m_n} {m_c}% {m_l}%");
        }
    }
}
