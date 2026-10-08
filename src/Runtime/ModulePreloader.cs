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
using SystemEx;
using SystemEx.Runtime.Backend.Loader;

namespace SystemEx.Runtime {


    public class ModulePreloader
    {
        private readonly List<Module> m_modules;
        private readonly string m_path;
        public string Path => m_path;

        public IReadOnlyList<Module> Modules => m_modules;
        
        public ModulePreloader(string path)
        {
            m_path = System.IO.Path.GetFullPath(path);
            m_modules = new List<Module>();
        }
        public ModulePreloader(string path, Platform architecture)
        {
            m_path = System.IO.Path.GetFullPath(path);
            m_path = System.IO.Path.Combine(m_path, architecture.ToString());

            m_modules = new List<Module>();
        }
        public ModulePreloader(string path, string  systemPath, Platform architecture)
        {
            m_path = System.IO.Path.GetFullPath(path);
            m_path = System.IO.Path.Combine(m_path, systemPath);
            m_path = System.IO.Path.Combine(m_path, architecture.ToString());

            m_modules = new List<Module>();
        }
        
        /// <summary>
        /// Preloads all native modules from the specified directory and architecture subfolder.
        /// </summary>
        public Result Preload()
        {
            Result result;

            if (!Directory.Exists(m_path))
            {
                result = new Result(new DirectoryNotFoundException($"The specified path '{m_path}' does not exist."));
            } 
            else
            {
                result = new Result();
                try {
                    foreach (string file in Directory.GetFiles(m_path))
                    {
                        string name = System.IO.Path.GetFileName(file);

                        if(IsNativeModule(file) == false)
                            continue;

                        if(IsAlreadyInList(name))
                            continue;

                        Module? module = Module.LoadModule(name, m_path);
                        

                        if (module != null) {
                            result.Add("New module loaded ", module);
                            module.AddRef();
                            m_modules.Add(module);
                        }
                    } 

                } catch (Exception ex) {
                    result.Catch(ex);
                    return result;
                } 
            }
            return result;
        }

        public void UnloadAll()
        {
            for(int i = m_modules.Count - 1; i >= 0; i--)
            {
                if(m_modules[i].RemoveRef() == 0)
                    m_modules.RemoveAt(i);
            }
        }

        public void Unload(Module module)
        {
            if(module == null) return;

            if (m_modules.Contains(module))
            {
                if(module.RemoveRef() == 0)
                m_modules.Remove(module);
            }
        }

        public void Unload(string name)
        {
            Module? moduleToUnload = null;

            foreach (Module module in m_modules)
            {
                if (module.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    moduleToUnload = module;
                    break;
                }
            }

            if (moduleToUnload != null)
            {
                Module.Unload(moduleToUnload);
                m_modules.Remove(moduleToUnload);
            }
        }


        private bool IsAlreadyInList(string name)
        {
            foreach (Module module in m_modules)
            {
                if (module.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        protected virtual bool IsNativeModule(string file)
        {
        #if WINDOWS
            return file.EndsWith(".dll", StringComparison.OrdinalIgnoreCase);
        #elif LINUX
            return file.EndsWith(".so", StringComparison.OrdinalIgnoreCase);
        #elif MACOS
            return file.EndsWith(".dylib", StringComparison.OrdinalIgnoreCase);
        #else
            return false;
        #endif
        }
    }

 }