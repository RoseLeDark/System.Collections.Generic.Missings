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

namespace SystemEx.Runtime.InteropServices.Platform {
	/// \addtogroup InteropServices
	/// @{

	/// <summary>
	/// Provides a fallback implementation for platforms that do not support
	/// native module loading or symbol resolution.  
	/// Instead of performing real dynamic loading, this loader simulates all
	/// operations by printing diagnostic messages to the console.  
	/// It allows the rest of the runtime to function without throwing exceptions,
	/// making it useful for testing, unsupported environments, or platforms
	/// without dynamic linking capabilities.
	/// </summary>
	internal class NoSupportProcLoader {
        /// <summary>
        /// Stores the last requested module name for diagnostic output.
        /// </summary>
        private static string m_name = "";

        /// <summary>
        /// Marker returned when no valid library path can be resolved.
        /// </summary>
        public const string NO_PATH = "NO_PATH";

        /// <summary>
        /// Simulates loading a native library.  
        /// Prints a diagnostic message and returns a <see cref="Module"/> instance
        /// with a zero handle, indicating that no real library was loaded.
        /// </summary>
        /// <param name="path">The requested library path.</param>
        /// <returns>
        /// A <see cref="Module"/> instance with <see cref="IntPtr.Zero"/> as its handle.
        /// </returns>
        public static Module LoadLibrary ( string path ) {
            m_name = path;
            Console.WriteLine("Load Simulator: your library '{0}'", path);
            return new Module(IntPtr.Zero, path);
        }

        /// <summary>
        /// Simulates resolving a function from a native module.  
        /// Prints a diagnostic message and returns <see cref="IntPtr.Zero"/>,
        /// indicating that no real function pointer exists.
        /// </summary>
        /// <param name="lib">The simulated module.</param>
        /// <param name="name">The requested function name.</param>
        /// <returns>
        /// Always <see cref="IntPtr.Zero"/>, because no real symbol resolution occurs.
        /// </returns>
        public static IntPtr LoadFunction ( Module lib, string name ) {
            Console.WriteLine("Load Simulator: your function '{0}' in '{1}'", name, m_name);
            return IntPtr.Zero;
        }

        /// <summary>
        /// Simulates unloading a native module.  
        /// Prints a diagnostic message and returns zero to indicate success.
        /// </summary>
        /// <param name="lib">The simulated module.</param>
        /// <returns>
        /// Always <c>0</c>, indicating a successful simulated unload.
        /// </returns>
        public static int FreeLibrary ( Module lib ) {
            Console.WriteLine("Load Simulator: free your library '{0}'", m_name);
            return 0;
        }

        /// <summary>
        /// Simulates resolving a library path.  
        /// Prints a diagnostic message and always returns <see cref="NO_PATH"/>,
        /// indicating that no real path resolution is performed.
        /// </summary>
        /// <param name="dllname">The requested library name.</param>
        /// <returns>
        /// Always <see cref="NO_PATH"/> because this backend does not support
        /// library discovery.
        /// </returns>
        public static string GetLibaryPath ( string dllname ) {
            Console.WriteLine("Load Simulator: GetLibaryPath '{0}'", dllname);
            return NO_PATH;
        }
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
	
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
