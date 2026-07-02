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

using System.Runtime.InteropServices;

namespace SystemEx.Drawing {
    /// \addtogroup color
    /// @{
    /// <summary>
    /// Represents a color in a specific color space and provides methods for color manipulation.
    /// </summary>
    public interface IColor<T> {
        /// <summary>
        /// Adjusts the saturation of the color.
        /// </summary>
        /// <param name="delta">The amount to adjust the saturation.</param>
        /// <returns>The adjusted color.</returns>
        T Saturation(float delta);

        /// <summary>
        /// Adjusts the brightness of the color.
        /// </summary>
        /// <param name="delta">The amount to adjust the brightness.</param>
        /// <returns>The adjusted color.</returns>
        T Brightness(float delta);

        /// <summary>
        /// Adds the specified color to this color.
        /// </summary>
        /// <param name="value">The color to add.</param>
        /// <returns>The resulting color.</returns>
        T Addition(T value);

        /// <summary>
        /// Subtracts the specified color from this color.
        /// </summary>
        /// <param name="value">The color to subtract.</param>
        /// <returns>The resulting color.</returns>
        T Subtraction(T value);

        /// <summary>
        /// Multiplies this color with the specified color.
        /// </summary>
        /// <param name="value">The color to multiply with.</param>
        /// <returns>The resulting color.</returns>
        T Multiplication(T value);

        /// <summary>
        /// Divides this color by the specified color.
        /// </summary>
        /// <param name="value">The color to divide by.</param>
        /// <returns>The resulting color.</returns>
        T Division(T value);

        // Komponentenweise Operationen
        /// <summary>
        /// Performs component-wise addition of three colors.
        /// </summary>
        /// <param name="a">The first component value.</param>
        /// <param name="b">The second component value.</param>
        /// <param name="c">The third component value.</param>
        /// <returns>The resulting color.</returns>
        T Addition(float a, float b, float c);
        /// <summary>
        /// Performs component-wise subtraction of three colors.
        /// </summary>
        /// <param name="a">The first component value.</param>
        /// <param name="b">The second component value.</param>
        /// <param name="c">The third component value.</param>
        /// <returns>The resulting color.</returns>
        T Subtraction(float a, float b, float c);
        /// <summary>
        /// Performs component-wise multiplication of three colors.
        /// </summary>
        /// <param name="a">The first component value.</param>
        /// <param name="b">The second component value.</param>
        /// <param name="c">The third component value.</param>
        /// <returns>The resulting color.</returns>
        T Multiplication(float a, float b, float c);
        /// <summary>
        /// Performs component-wise division of three colors.
        /// </summary>
        /// <param name="a">The first component value.</param>
        /// <param name="b">The second component value.</param>
        /// <param name="c">The third component value.</param>
        /// <returns>The resulting color.</returns>
        T Division(float a, float b, float c);

        /// <summary>
        /// Performs linear interpolation between this color and another color.
        /// </summary>
        /// <param name="value">Ziel Value</param>
        /// <param name="amount">Interpolationsfaktor</param>
        /// <returns></returns> 
        T Lerp(T value, float amount);

    }
    /// @}
}