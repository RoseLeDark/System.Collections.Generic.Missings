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
using System.Runtime.InteropServices;
using System.Text;
using SystemEx.Collections.Generic;
using SystemEx.Runtime.InteropServices;



/// \addtogroup  Runtime
/// @{

#if DOXYGEN

/// Used on Windows
using ProcLoader = SystemEx.Runtime.InteropServices.Platform.WindowsProcLoader;
/// Used on Linux
using ProcLoader = SystemEx.Runtime.InteropServices.Platform.LinuxProcLoader;
/// Used on Mac
using ProcLoader = SystemEx.Runtime.InteropServices.Platform.MacProcLoader;
/// Used when no PLatform supported
using ProcLoader = SystemEx.Runtime.InteropServices.Platform.NoSupportProcLoader;

#else

#if WINDOWS
using ProcLoader = SystemEx.Runtime.InteropServices.Platform.WindowsProcLoader;
#elif LINUX
using ProcLoader = SystemEx.Runtime.InteropServices.Platform.LinuxProcLoader;
#elif MACOS
using ProcLoader = SystemEx.Runtime.InteropServices.Platform.MacProcLoader;
#else
using ProcLoader = SystemEx.Runtime.InteropServices.Platform.NoSupportProcLoader;
#endif

#endif

namespace SystemEx.Runtime {
    /// <summary>
    /// Represents a loaded native module (DLL, SO, or DYLIB).  
    /// A <see cref="Module"/> encapsulates the operating system handle of the
    /// loaded library and provides helper methods for resolving exported
    /// functions and unloading the module.
    /// </summary>
    public class Function<TReturn, TDelegate> : IDisposable
         where TDelegate : Delegate {
        /// <summary>
        /// Gets the raw pointer to the native function.
        /// </summary>
        public IntPtr Pointer { get; private set; }
        /// <summary>
        /// Gets the name of the function as specified when loading it from the module.
        /// </summary>
        public string Name { get; private set; }

        private bool m_disposed = false;

        private Module m_module;

        /// <summary>
        /// Initializes a new <see cref="Function{TReturn, TDelegate}"/> instance with the specified
        /// function pointer and name.
        /// </summary>
        internal Function(IntPtr pointer, string name, Module parent)
        {
            Pointer = pointer;
            Name = name;
            m_module = parent;
        }
        /// <summary>
        /// Gets a managed delegate that wraps the native function pointer.
        /// </summary>
        public TDelegate GetDelegate()
        {
            if (Pointer == IntPtr.Zero)
                throw new InvalidOperationException($"Function '{Name}' has a null pointer.");

            return Marshal.GetDelegateForFunctionPointer<TDelegate>(Pointer);
        }
        /// <summary>
        /// Invokes the native function with the specified arguments and returns the result.
        /// </summary>
        public TReturn? Call(params object[] args)
        {
            var del = GetDelegate();
            var dnobj = del.DynamicInvoke(args);

            return (dnobj == null) ? default : (TReturn)dnobj;
        }
        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if(!this.m_disposed) {
                if(disposing) {
                    m_module.RemoveRef();
                }
                m_disposed = true;
            }
        }
    }

	/// <summary>
	/// Represents a loaded native module (DLL, SO, or DYLIB).  
	/// A <see cref="Module"/> encapsulates the operating system handle of the
	/// loaded library and provides helper methods for resolving exported
	/// functions and unloading the module.
	/// </summary>
	public class Module {
        private int m_count;
        private bool m_loaded ;

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
            m_count = 0;
            m_loaded = true;
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
        public static int Unload ( Module module ) {
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
        public static Module? LoadModule ( string name, string path ) {
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
        public static Module? LoadModule ( string name ) {
            Module? _ret = null;
            string new_path = ProcLoader.NO_PATH;

            new_path = ProcLoader.GetLibaryPath(name);

            if ( new_path != ProcLoader.NO_PATH ) {
                _ret = ProcLoader.LoadLibrary(new_path);
            }
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
        internal IntPtr LoadFunc ( string func ) {
            if(!m_loaded) return IntPtr.Zero;

            try {
                IntPtr x = ProcLoader.LoadFunction(this, func);
                return x;
            } catch
            {
                return IntPtr.Zero;
            }
        }

        /// <summary>
        /// Resolves a function exported by the native module using the active
        /// platform backend loader.  
        /// </summary>
        public Function<TReturn, TDelegate>? LoadFunc<TReturn, TDelegate> ( string func ) where TDelegate : Delegate {
            if(!m_loaded) return null;

            Function<TReturn, TDelegate>? _ret = null;
            IntPtr ptr = ProcLoader.LoadFunction(this, func);
            

            if ( ptr != IntPtr.Zero ) {
                _ret = new Function<TReturn, TDelegate>(ptr, func, this);
                
            }
            if(_ret != null) {m_count++; }

            return _ret;
        }

        internal int RemoveRef()
        {
            if(!m_loaded) return -1;

            m_count--;
            if(m_count <= 0)
                Unload(this);
            return m_count;
        }
        internal int AddRef()
        {
            if(!m_loaded) return -1;

            m_count++;
            return m_count;
        }
    }
}
