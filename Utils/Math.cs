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
namespace SystemEx {
    /// <summary>
    /// Provides mathematical helper functions that are not included in
    /// <see cref="System.Math"/>.  
    /// Contains a deterministic implementation of the C/C++ <c>fmod</c>
    /// operation for both <see cref="double"/> and <see cref="float"/>.
    /// </summary>
    public static class Math {
        /// <summary>
        /// Computes the floating‑point remainder of <paramref name="a"/> divided by
        /// <paramref name="b"/> using truncation toward zero, matching the behavior
        /// of C/C++ <c>fmod</c>.  
        /// Equivalent to: <c>a - trunc(a / b) * b</c>.
        /// </summary>
        public static double FMod(double a, double b) {
            return a - System.Math.Truncate(a / b) * b;
        }

        /// <summary>
        /// Computes the floating‑point remainder of <paramref name="a"/> divided by
        /// <paramref name="b"/> using truncation toward zero, matching the behavior
        /// of C/C++ <c>fmod</c>.  
        /// Equivalent to: <c>a - trunc(a / b) * b</c>.
        /// </summary>
        public static float FMod(float a, float b) {
            return a - (float)System.Math.Truncate(a / b) * b;
        }
    }

}
