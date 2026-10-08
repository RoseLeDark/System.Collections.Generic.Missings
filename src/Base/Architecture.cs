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
    ///  The processor architecture.
    /// </summary>
    public enum Architecture
    {
        /// <summary>
        /// Intel32-bit processor architecture.
        /// </summary>
        x86 = 0,
        /// <summary>
        /// Intel64-bit processor architecture.
        /// </summary>
        x64 = 1,
        /// <summary>
        /// ARM 32-bit processor architecture.
        /// </summary>
        ARM = 2,
        /// <summary>
        /// ARM 64-bit processor architecture.
        /// </summary>
        ARM64 = 3,
        /// <summary>
        /// RISC-V 64-bit processor architecture.
        /// </summary>
        RiscV64 = 4,

        Other = 99,
    }

}