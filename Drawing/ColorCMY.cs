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
	/// \addtogroup SystemEx.Drawing
	/// @{
	/// <summary>
	/// A color in the CMY (Cyan–Magenta–Yellow) color space using floating‑point components.
	/// </summary>
	public class ColorCMY {
        private float m_c;
        private float m_m;
        private float m_y;
        /// <summary>
        /// The cyan component of the color, in the range 0–1.
        /// </summary>
        public float C { get { return m_c; } set { m_c = System.Math.Clamp(value, 0f, 1f); } }
        /// <summary>
        /// The magenta component of the color, in the range 0–1.
        /// </summary>
        public float M { get { return m_m; } set { m_m = System.Math.Clamp(value, 0f, 1f); } }
        /// <summary>
        /// The yellow component of the color, in the range 0–1.
        /// </summary>
        public float Y { get { return m_y; } set { m_y = System.Math.Clamp(value, 0f, 1f); } }
        /// <summary>
        /// The default constructor initializes the color to black (C=0, M=0, Y=0).
        /// </summary>
        public ColorCMY(float[] x) {
            m_c = System.Math.Clamp(x[0], 0f, 1f);
            m_m = System.Math.Clamp(x[1], 0f, 1f);
            m_y = System.Math.Clamp(x[2], 0f, 1f);
        }
        /// <summary>
        /// Initializes a new instance of the ColorCMY class with the specified component values.
        /// </summary>
        /// <param name="c">The cyan component (0–1).</param>
        /// <param name="m">The magenta component (0–1).</param>
        /// <param name="y">The yellow component (0–1).</param>
        public ColorCMY(float c, float m, float y) {
            m_c = System.Math.Clamp(c, 0f, 1f);
            m_m = System.Math.Clamp(m, 0f, 1f);
            m_y = System.Math.Clamp(y, 0f, 1f);
        }
    }
    /// @}
}
