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
	/// \addtogroup Drawing
	/// @{

	/// <summary>
	/// Represents a compact N‑Color classification based on the HWB color model.
	/// The color is expressed using a hue segment index (0–5), a percentage
	/// within that segment (0–100), and the whiteness/blackness components.
	/// </summary>
	public struct ColorNCol : IEquatable<ColorNCol> {
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
        public bool Equals(ColorNCol other) {
       
            return m_c.Equals(other.m_c) &&
                   m_l.Equals(other.m_l) &&
                   m_n == (other.m_n);
        }
        /// <summary>
        /// Determines whether this instance is equal to another object.
        /// </summary>
        public override bool Equals(object? obj) {
            if ( obj is ColorNCol obc ) return Equals(obc);
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
    /// @}
}
