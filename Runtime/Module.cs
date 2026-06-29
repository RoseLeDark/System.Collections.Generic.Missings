using System;
using System.Runtime.InteropServices;
using System.Text;
using SystemEx.Collections.Generic;
using SystemEx.Runtime.InteropServices;



#if WINDOWS
using ProcLoader = SystemEx.Runtime.InteropServices.Platform.WindowsProcLoader;
#elif LINUX
using ProcLoader = SystemEx.Runtime.InteropServices.Platform.LinuxProcLoader;
#elif MACOS
using ProcLoader = SystemEx.Runtime.InteropServices.Platform.MacProcLoader;
#else
using ProcLoader = SystemEx.Runtime.InteropServices.Platform.NoSupportProcLoader;
#endif

namespace SystemEx.Runtime {
    /// \addtogroup Runtime
    /// @{
    /// <summary>
    /// Represents a loaded native module (DLL, SO, or DYLIB).  
    /// A <see cref="Module"/> encapsulates the operating system handle of the
    /// loaded library and provides helper methods for resolving exported
    /// functions and unloading the module.
    /// </summary>
    public class Module {
        /// <summary>
        /// Gets the native handle of the loaded module.  
        /// This value corresponds to the OS‑specific library handle returned by
        /// <c>LoadLibrary</c> (Windows), <c>dlopen</c> (Linux), or <c>dlopen</c> (macOS).
        /// </summary>
        public nint Handle { get; internal set; }

        /// <summary>
        /// Initializes a new <see cref="Module"/> instance using the specified
        /// native handle and file path.  
        /// The constructor extracts the module's file name and directory path
        /// for informational purposes.
        /// </summary>
        /// <param name="v">The native module handle.</param>
        /// <param name="strPath">The full path to the loaded module file.</param>
        public Module ( nint v, string strPath ) {
            this.Handle = v;
            this.Name = System.IO.Path.GetFileName(strPath);
            this.Path = System.IO.Path.GetDirectoryName(strPath)!;
        }

        /// <summary>
        /// Gets the file name of the loaded module (e.g., <c>kernel.dll</c>,
        /// <c>libfoo.so</c>, <c>libbar.dylib</c>).
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the directory path where the module file resides.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Unloads the specified module using the platform‑specific backend.  
        /// This method is internal because unloading is managed by the runtime
        /// and should not be performed directly by user code.
        /// </summary>
        /// <param name="module">The module to unload.</param>
        /// <returns>
        /// Zero on success, or a non‑zero error code depending on the backend.
        /// </returns>
        internal static int Unload ( Module module ) {
            return ProcLoader.FreeLibrary(module);
        }

        /// <summary>
        /// Loads a module from an explicit directory and file name.  
        /// This method constructs the full path and attempts to load the module
        /// using the platform‑specific loader.
        /// </summary>
        /// <param name="name">The module file name.</param>
        /// <param name="path">The directory containing the module.</param>
        /// <returns>
        /// A new <see cref="Module"/> instance if loading succeeds; otherwise <c>null</c>.
        /// </returns>
        internal static Module? LoadModule ( string name, string path ) {
            Module? _ret = null;
            string new_path = System.IO.Path.Combine(path, name);

            if ( System.IO.File.Exists(new_path) )
                _ret = ProcLoader.LoadLibrary(new_path);

            return _ret;
        }

        /// <summary>
        /// Loads a module by searching platform‑specific library paths.  
        /// The backend resolves the correct file location using mechanisms such as
        /// <c>LD_LIBRARY_PATH</c>, <c>DYLD_LIBRARY_PATH</c>, or Windows search rules.
        /// </summary>
        /// <param name="name">The module file name.</param>
        /// <returns>
        /// A new <see cref="Module"/> instance if loading succeeds; otherwise <c>null</c>.
        /// </returns>
        internal static Module? LoadModule ( string name ) {
            Module? _ret = null;
            string new_path = ProcLoader.NO_PATH;

            new_path = ProcLoader.GetLibaryPath(name);

            if ( new_path != ProcLoader.NO_PATH ) {
                _ret = ProcLoader.LoadLibrary(new_path);
            }
            return _ret;
        }

        /// <summary>
        /// Resolves a function exported by the native module using the active
        /// platform backend loader.  
        /// The returned pointer is obtained through the OS‑specific implementation
        /// of <see cref="ProcLoader"/>, which internally calls:
        /// <list type="bullet">
        /// <item><description><c>GetProcAddress</c> on Windows</description></item>
        /// <item><description><c>dlsym</c> on Linux</description></item>
        /// <item><description><c>dlsym</c> on macOS</description></item>
        /// </list>
        /// The caller is responsible for converting the returned pointer into a
        /// managed delegate if required.
        /// </summary>
        /// <param name="func">The name of the exported native function.</param>
        /// <returns>
        /// A raw function pointer (<see cref="IntPtr"/>).  
        /// Returns <see cref="IntPtr.Zero"/> if the function cannot be resolved.
        /// </returns>
        internal IntPtr LoadFunc ( string func ) {
            return ProcLoader.LoadFunction(this, func);
        }
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
