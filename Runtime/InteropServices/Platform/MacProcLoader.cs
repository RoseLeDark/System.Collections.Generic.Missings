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
using System.Runtime.Versioning;

namespace SystemEx.Runtime.InteropServices.Platform {
	/// \addtogroup SystemEx.Runtime.InteropServices.Platform
	/// @{
	/// <summary>
	/// Provides macOS-specific native module loading and symbol resolution
	/// using the <c>libdl</c> API.  
	/// This backend supports loading dynamic libraries (.dylib),
	/// resolving exported symbols, and searching system library paths
	/// including <c>/usr/lib</c>, <c>/usr/local/lib</c>, and
	/// <c>DYLD_LIBRARY_PATH</c>.
	/// </summary>
	[SupportedOSPlatform("macos")]
    internal class MacProcLoader {
        /// <summary>
        /// POSIX dynamic loader library used for <c>dlopen</c>, <c>dlsym</c>,
        /// and <c>dlclose</c>.
        /// </summary>
        private const string Library = "libdl";

        /// <summary>
        /// Specifies immediate symbol resolution when loading a module.
        /// </summary>
        private const int RTLD_NOW = 2;

        /// <summary>
        /// Marker returned when no valid library path can be resolved.
        /// </summary>
        public const string NO_PATH = "NO_PATH";

        /// <summary>
        /// Loads a dynamic library (.dylib) using <c>dlopen</c>.
        /// </summary>
        [DllImport(Library, EntryPoint = "dlopen", CharSet = CharSet.Unicode)]
        private static extern IntPtr dlopen ( string path, int mode );


        /// <summary>
        /// Resolves an exported symbol from a loaded module using <c>dlsym</c>.
        /// </summary>
        [DllImport(Library, EntryPoint = "dlsym", CharSet = CharSet.Unicode)]
        private static extern IntPtr dlsym ( IntPtr handle, string name );

        /// <summary>
        /// Unloads a previously loaded module using <c>dlclose</c>.
        /// </summary>
        [DllImport(Library, EntryPoint = "dlclose", CharSet = CharSet.Unicode)]
        private static extern int dlclose ( IntPtr handle );

        /// <summary>
        /// Loads a module from an explicit file path and wraps the handle
        /// in a <see cref="Module"/> instance.
        /// </summary>
        public static Module LoadLibrary ( string path ) {
            return new Module(dlopen(path, RTLD_NOW), path);
        }

        /// <summary>
        /// Resolves an exported function from a loaded module.
        /// </summary>
        public static IntPtr LoadFunction ( Module lib, string name ) {
            return dlsym(lib.Handle, name);
        }

        /// <summary>
        /// Unloads a module using <c>dlclose</c>.
        /// </summary>
        public static int FreeLibrary ( Module lib ) {
            return dlclose(lib.Handle);
        }

        /// <summary>
        /// Attempts to locate a dynamic library (.dylib) by searching common
        /// macOS library directories and the <c>DYLD_LIBRARY_PATH</c> environment
        /// variable.  
        /// Returns the resolved path or <see cref="NO_PATH"/> if not found.
        /// </summary>
        public static string GetLibaryPath ( string dynmodule ) {
            string _ret = NO_PATH;

            // Cheak usr/lib
            _ret = GetFromPath("/usr/lib/", dynmodule);
            if(_ret == NO_PATH ) 
                _ret = GetFromPath("/usr/local/lib", dynmodule);

            if ( _ret == NO_PATH ) {

                string? env = Environment.GetEnvironmentVariable("DYLD_LIBRARY_PATH");
                if ( env != null ) {
                    var paths = env.Split(':');
                    string _t = "";
                    foreach ( string path in paths ) {
                        _t = GetFromPath(path, dynmodule);
                        if ( _t != NO_PATH ) { _ret = _t; break; }
                    }
                }
            }

            return _ret;
        }
        /// <summary>
        /// Searches a directory for a file matching the requested module name.
        /// Returns the full path if found, otherwise <see cref="NO_PATH"/>.
        /// </summary>
        static string GetFromPath ( string path, string module ) {
            string _ret = NO_PATH;
            foreach ( var file in Directory.GetFiles(path) ) {
                if ( Path.GetFileName(file) == module ) {
                    _ret = Path.Combine(path, file);
                }
            }
            return _ret;
        }
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
	/// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
