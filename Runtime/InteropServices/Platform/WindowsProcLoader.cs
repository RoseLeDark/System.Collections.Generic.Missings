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

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using SystemEx.Collections.Generic;

namespace SystemEx.Runtime.InteropServices.Platform {
	/// \addtogroup SystemEx.Runtime.InteropServices.Platform
	/// @{
	/// <summary>
	/// Provides Windows-specific native module loading and symbol resolution
	/// using the <c>dll</c> API.  
	/// This backend supports loading dynamic libraries (.dll).
	/// </summary>
	[SupportedOSPlatform("windows")]
    public class WindowsProcLoader  {
        
        /// <summary>
        /// The name of the Windows system library that provides native
        /// module loading and symbol resolution functions.
        /// </summary>
        private const string Library = "kernel32";
        /// <summary>
        /// Special marker value returned when no valid library path
        /// can be resolved for a requested module.
        /// </summary>
        public const string NO_PATH = "NO_PATH";

        /// <summary>
        /// Loads a native library (DLL) into the current process using
        /// the Windows API function <c>LoadLibraryW</c>.  
        /// Returns a handle that can be used to resolve exported symbols.
        /// </summary>
        /// <param name="dllToLoad">The full path of the DLL to load.</param>
        /// <returns>
        /// A native module handle, or <see cref="IntPtr.Zero"/> on failure.
        /// </returns>
        [DllImport(Library, CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibraryW ( string dllToLoad );

        /// Retrieves the address of an exported function from a loaded
        /// module using the Windows API function <c>GetProcAddress</c>.  
        /// The returned pointer can be converted into a managed delegate.
        /// </summary>
        /// <param name="hModule">The handle of the loaded module.</param>
        /// <param name="procedureName">The name of the exported function.</param>
        /// <returns>
        /// A raw function pointer, or <see cref="IntPtr.Zero"/> if the
        /// function cannot be found.
        /// </returns>
        [DllImport(Library, CharSet = CharSet.Unicode)]
        private static extern IntPtr GetProcAddressW ( IntPtr hModule, string procedureName );

        /// <summary>
        /// Frees a previously loaded native module using the Windows API
        /// function <c>FreeLibrary</c>.  
        /// This decreases the module's reference count and may unload it
        /// when the count reaches zero.
        /// </summary>
        /// <param name="hModule">The handle of the module to unload.</param>
        /// <returns>
        /// Zero on success, or a non‑zero error code on failure.
        /// </returns>
        [DllImport(Library, CharSet = CharSet.Unicode)]
        private static extern int FreeLibraryW ( IntPtr hModule );
        /// <summary>
        /// Loads a native module from the specified file path and wraps
        /// the resulting handle in a <see cref="Module"/> instance.
        /// </summary>
        /// <param name="path">The full path to the DLL file.</param>
        /// <returns>
        /// A new <see cref="Module"/> instance representing the loaded DLL.
        /// </returns>
        public static Module LoadLibrary ( string path ) {
            return new Module(LoadLibraryW(path), path);
        }
        /// <summary>
        /// Resolves an exported function from a loaded module using
        /// <c>GetProcAddress</c>.  
        /// The returned pointer can be marshaled into a managed delegate
        /// by higher‑level runtime components.
        /// </summary>
        /// <param name="lib">The module containing the function.</param>
        /// <param name="name">The name of the exported function.</param>
        /// <returns>
        /// A raw function pointer, or <see cref="IntPtr.Zero"/> if the
        /// function cannot be resolved.
        /// </returns>
        public static IntPtr LoadFunction ( Module lib, string name ) {
            return GetProcAddressW(lib.Handle, name);
        }
        /// <summary>
        /// Unloads a previously loaded module by calling <c>FreeLibrary</c>.
        /// </summary>
        /// <param name="lib">The module to unload.</param>
        /// <returns>
        /// Zero on success, or a non‑zero error code on failure.
        /// </returns>
        public static int FreeLibrary ( Module lib ) {
            return FreeLibraryW(lib.Handle);
        }
        /// <summary>
        /// Resolves the file path of a DLL.  
        /// On Windows, DLL search rules are handled by the OS loader, so
        /// this method simply returns the provided name unchanged.  
        /// Other backends (Linux/macOS) override this behavior to search
        /// system library paths.
        /// </summary>
        /// <param name="dllname">The name of the DLL.</param>
        /// <returns>
        /// The resolved library path, or <see cref="NO_PATH"/> if no path
        /// can be determined.
        /// </returns>
        public static string GetLibaryPath(string dllname) {
            return dllname;
        }
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
