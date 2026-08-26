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
using SystemEx.Collections.Generic;

namespace SystemEx.Runtime.InteropServices {
	/// \addtogroup Runtime
	/// @{

	/// <summary>
	/// Provides a high‑level management layer for native modules and their exported
	/// functions.  
	/// <see cref="NativeHost"/> maintains an internal cache of loaded modules to
	/// avoid redundant loading operations and to ensure that multiple components
	/// referencing the same library share a single module instance.  
	/// It also provides helper methods for resolving native function pointers and
	/// converting them into managed delegates.
	/// </summary>
	public static class NativeHost {
        /// <summary>
        /// Stores all modules loaded through <see cref="LoadModule(string)"/>.  
        /// The key is the module path, and the value is the corresponding
        /// <see cref="Module"/> instance.  
        /// This cache ensures that each native library is loaded only once.
        /// </summary>
        private static Map<string, Module> m_loadedModule = new Map<string, Module>();
        /// <summary>
        /// Loads a native module from the specified path.  
        /// If the module has already been loaded previously, the cached instance
        /// is returned instead of loading it again.  
        /// This prevents duplicate OS handles and improves performance.
        /// </summary>
        /// <param name="strPath">The full path to the native module.</param>
        /// <returns>
        /// A <see cref="Module"/> instance representing the loaded library,
        /// or <c>null</c> if loading fails.
        /// </returns>
        public static Module? LoadModule(string strPath) {
            Module? _ret =  null;

            if ( !m_loadedModule.ContainsKey(strPath) ) {
                _ret =  Module.LoadModule(strPath);
                if(_ret != null) m_loadedModule.PushBack(strPath, _ret);
            } else {
                var value =m_loadedModule[strPath];
                _ret = value.IsNull ? null : value.Value!;
            }

            return _ret;
        }
        /// <summary>
        /// Frees a previously loaded module and removes it from the internal cache.  
        /// If the module is not currently tracked, the method returns <c>false</c>.  
        /// The underlying OS handle is released using <see cref="Module.Unload(Module)"/>.
        /// </summary>
        /// <param name="module">The module instance to unload.</param>
        /// <returns>
        /// <c>true</c> if the module was found and successfully removed;
        /// otherwise <c>false</c>.
        /// </returns>
        public static bool FreeModule ( Module module ) {
            if ( !m_loadedModule.ContainsValue(module) )
                return false;

            Module.Unload(module);

            // Key finden und entfernen
            foreach ( var kv in m_loadedModule ) {
                if ( kv.EqualSecond(module) ) {
                    m_loadedModule.Remove(kv.First);
                    break;
                }
            }

            return true;
        }
        /// <summary>
        /// Resolves a native function from a module identified by its path.  
        /// If the module is loaded, the function is resolved and converted into
        /// a managed delegate of type <typeparamref name="T"/>.  
        /// If the module is not found, <c>null</c> is returned.
        /// </summary>
        /// <typeparam name="T">The delegate type representing the function signature.</typeparam>
        /// <param name="module">The module path used as cache key.</param>
        /// <param name="func">The name of the exported native function.</param>
        /// <returns>
        /// A managed delegate bound to the native function, or <c>null</c> if the
        /// module or function cannot be resolved.
        /// </returns>
        public static T? GetFunction<T> ( string module, string func ) where T : Delegate {
            Optional<Module> x = m_loadedModule[module];
            if(x.IsSome) {
                return GetFunction<T>(x.Value!, func);
            }
            return null;

            
        }
        /// <summary>
        /// Resolves a native function from the specified module and converts the
        /// resulting function pointer into a managed delegate of type
        /// <typeparamref name="T"/>.  
        /// The conversion is performed using
        /// <see cref="Marshal.GetDelegateForFunctionPointer{T}(IntPtr)"/>.
        /// </summary>
        /// <typeparam name="T">The delegate type representing the function signature.</typeparam>
        /// <param name="module">The module from which the function is resolved.</param>
        /// <param name="func">The name of the exported native function.</param>
        /// <returns>
        /// A managed delegate bound to the native function, or <c>null</c> if the
        /// module is <c>null</c> or the function cannot be found.
        /// </returns>
        public static T? GetFunction<T>( Module? module, string func ) where T : Delegate {
            if ( module == null ) return null;

            IntPtr _func = module.LoadFunc(func);
            if ( _func == IntPtr.Zero ) return null;

            return Marshal.GetDelegateForFunctionPointer<T>(_func);
        }
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
