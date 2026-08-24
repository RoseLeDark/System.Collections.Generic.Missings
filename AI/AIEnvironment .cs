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

namespace SystemEx.AI {
	/// \addtogroup SystemEx.AI
	/// @{
	/// <summary>
	/// Describes the runtime environment required or used by an AI backend.
	/// This enum is intentionally broad and future-proof.
	/// </summary>
	public enum Environment : int {
        // OS
        AI_ENV_OS_WINDOWS        = 0x3001,
        AI_ENV_OS_LINUX          = 0x3002,
        AI_ENV_OS_MACOS          = 0x3003,
        AI_ENV_OS_ANDROID        = 0x3004,
        AI_ENV_OS_IOS            = 0x3005,
        AI_ENV_OS_BROWSER        = 0x3006,

        // Architecture
        AI_ENV_ARCH_X64          = 0x3101,
        AI_ENV_ARCH_ARM64        = 0x3102,

        // Hardware
        AI_ENV_HW_CPU            = 0x3201,
        AI_ENV_HW_GPU            = 0x3202,
        AI_ENV_HW_NPU            = 0x3203,
        AI_ENV_HW_MICROPHONE     = 0x3204,
        AI_ENV_HW_CAMERA         = 0x3205,

        // Memory / Storage
        AI_ENV_MEM_RAM           = 0x3301,
        AI_ENV_MEM_VRAM          = 0x3302,
        AI_ENV_FS_TEMP_DIR       = 0x3303,
        AI_ENV_FS_WORK_DIR       = 0x3304,
        AI_ENV_FS_HOME_DIR       = 0x3305,
        AI_ENV_FS_CACHE_DIR      = 0x3306,

        // Runtime
        AI_ENV_RT_DOTNET         = 0x3401,
        AI_ENV_RT_WASM           = 0x3402,
        AI_ENV_RT_NATIVE         = 0x3403,
        AI_ENV_RT_SANDBOX        = 0x3404,

        // Network
        AI_ENV_NET_ONLINE        = 0x3501,
        AI_ENV_NET_OFFLINE       = 0x3502,
        AI_ENV_NET_LIMITED       = 0x3503,

        // Locale / Culture
        AI_ENV_LOCALE            = 0x3601,
        AI_ENV_CULTURE           = 0x3602,

        // Security / Permissions
        AI_ENV_SEC_SANDBOXED     = 0x3701,
        AI_ENV_SEC_FULL_TRUST    = 0x3702,
        AI_ENV_SEC_NO_FILE_ACCESS= 0x3703,

        // Custom
        AI_ENV_CUSTOM            = 0x3FFF
    }
	///@}

}
