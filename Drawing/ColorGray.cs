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
    /// The ColorGray class represents a grayscale color using a single floating-point component.
    /// Only V Drom HSV is used to represent the gray value, where 0 represents black and 1 represents white.
    /// </summary>
    public class ColorGray {
        private float m_v;
        /// <summary>
        /// Gets the grayscale value.
        /// </summary>
        public float Gray { get { return m_v; } }
        /// <summary>
        /// Initializes a new instance of the ColorGray class with the specified grayscale value.
        /// </summary>
        /// <param name="v">The grayscale value (0–1).</param>
        public ColorGray(float v) { m_v = System.Math.Clamp(v, 0f, 1f); }
        /// <summary>
        /// Initializes a new instance of the ColorGray class with the specified grayscale value array.
        /// </summary>
        /// <param name="x">The grayscale value array (0–1). </param>
        public ColorGray(float[] x) { m_v = System.Math.Clamp(x[0], 0f, 1f); }
        /// <summary>
        /// Creates a new ColorGray instance from a ColorHSV instance, using the value (V) component of the HSV color.
        /// </summary>
        /// <param name="hsv"></param> 
        public ColorGray(ColorHSV hsv) { m_v = System.Math.Clamp(hsv.V, 0f, 1f); }
    }
    /// @}
}
