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

namespace SystemEx.Device.Interop {

    [SupportedOSPlatform("windows")]
    public static unsafe class WindowsKernelLoader {

        private const string Library = "kernel32";

        [DllImport(Library, CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibraryW(string dllToLoad);

        [DllImport(Library, CharSet = CharSet.Unicode)]
        private static extern IntPtr GetProcAddressW(IntPtr hModule, string procedureName);

        [DllImport(Library, CharSet = CharSet.Unicode)]
        private static extern int FreeLibraryW(IntPtr hModule);

        public static IntPtr LoadLibrary(string path) => LoadLibraryW(path);
        public static IntPtr LoadFunction(IntPtr lib, string name) => GetProcAddressW(lib, name);
        public static int FreeLibrary(IntPtr lib) => FreeLibraryW(lib);


        [UnmanagedFunctionPointer(CallingConvention.Winapi)] 
        public delegate int MyKernelFunction( IntPtr A, int dwSize, IntPtr B, int dwSizeB, out IntPtr C);


        public static int call(string dll, string funcname, IntPtr A, int dwSize, IntPtr B, int dwSizeB, IntPtr C) {
            IntPtr hh = LoadLibrary(dll);
            IntPtr proc = LoadFunction(hh, funcname);

            MyKernelFunction kernel_func = Marshal.GetDelegateForFunctionPointer<MyKernelFunction>(proc);

            int _ret = kernel_func(A, dwSize, B, dwSizeB, out C);
            FreeLibrary(hh);
            return _ret;
        }

    }

    [SupportedOSPlatform("linux")]
    public static unsafe class LinuxKernelLoader {

        private const string Library = "libdl";
        private const int RTLD_NOW = 2;

        [DllImport(Library, EntryPoint = "dlopen", CharSet = CharSet.Unicode)]
        private static extern IntPtr dlopen(string path, int mode);

        [DllImport(Library, EntryPoint = "dlsym", CharSet = CharSet.Unicode)]
        private static extern IntPtr dlsym(IntPtr handle, string name);

        [DllImport(Library, EntryPoint = "dlclose", CharSet = CharSet.Unicode)]
        private static extern int dlclose(IntPtr handle);

        public static IntPtr LoadLibrary(string path) => dlopen(path, RTLD_NOW);
        public static IntPtr LoadFunction(IntPtr lib, string name) => dlsym(lib, name);
        public static int FreeLibrary(IntPtr lib) => dlclose(lib);

     
        public delegate int MyKernelFunction(IntPtr A, int dwSize, IntPtr B, int dwSizeB, out IntPtr C);

        public static int call(string dll, string funcname, IntPtr A, int dwSize, IntPtr B, int dwSizeB, IntPtr C) {
            IntPtr hh = LoadLibrary(dll);
            IntPtr proc = LoadFunction(hh, funcname);

            MyKernelFunction kernel_func = Marshal.GetDelegateForFunctionPointer<MyKernelFunction>(proc);

            int _ret = kernel_func(A, dwSize, B, dwSizeB, out C);
            FreeLibrary(hh);
            return _ret;
        }
    }

    [SupportedOSPlatform("macos")]
    public static unsafe class MacKernelLoader {

        private const string Library = "libdl";
        private const int RTLD_NOW = 2;

        [DllImport(Library, EntryPoint = "dlopen", CharSet = CharSet.Unicode)]
        private static extern IntPtr dlopen(string path, int mode);

        [DllImport(Library, EntryPoint = "dlsym", CharSet = CharSet.Unicode)]
        private static extern IntPtr dlsym(IntPtr handle, string name);

        [DllImport(Library, EntryPoint = "dlclose", CharSet = CharSet.Unicode)]
        private static extern int dlclose(IntPtr handle);

        public static IntPtr LoadLibrary(string path) => dlopen(path, RTLD_NOW);
        public static IntPtr LoadFunction(IntPtr lib, string name) => dlsym(lib, name);
        public static int FreeLibrary(IntPtr lib) => dlclose(lib);


        public delegate int MyKernelFunction(IntPtr A, int dwSize, IntPtr B, int dwSizeB, out IntPtr C);

        public static int call(string dll, string funcname, IntPtr A, int dwSize, IntPtr B, int dwSizeB, IntPtr C) {
            IntPtr hh = LoadLibrary(dll);
            IntPtr proc = LoadFunction(hh, funcname);

            MyKernelFunction kernel_func = Marshal.GetDelegateForFunctionPointer<MyKernelFunction>(proc);

            int _ret = kernel_func(A, dwSize, B, dwSizeB, out C);
            FreeLibrary(hh);
            return _ret;
        }
    }

    public static unsafe class NoSupportKernelLoader {
        private static string m_name = "";
        public static IntPtr LoadLibrary(string path) { m_name = path; Console.WriteLine("Load Simulator your lib {0}", path); return IntPtr.Zero; }
        public static IntPtr LoadFunction(IntPtr lib, string name) { Console.WriteLine("Load Simulator your function {0}@{1}", name, m_name); return IntPtr.Zero; }
        public static void FreeLibrary(IntPtr lib) { Console.WriteLine("Load Simulator free your lib {0}", m_name); }

        public static int call(string dll, string funcname, IntPtr A, int dwSize, IntPtr B, int dwSizeB, IntPtr C) {
            Console.WriteLine("Call Simulator {0}@{1} (string dll, string funcname, IntPtr A, int dwSize, IntPtr B, int dwSizeB, IntPtr C)", dll, funcname);
            return 0;
        }
    }

}
