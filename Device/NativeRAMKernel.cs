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

using SystemEx.Collections.Generic;
using SystemEx.Device.Intertropt;
using SystemEx.Device.Memory;
using SystemEx.Runtime.InteropServices;
using Module = SystemEx.Runtime.Module;



namespace SystemEx.Device {
	// \addtogroup Device
	/// @{

	/// <summary>
	/// Provides a base implementation for RAM‑based compute kernels that operate on
	/// <see cref="DeviceSharedBuffer{RamSharedBackend}"/> instances.  
	/// This class handles module loading, buffer management, backend initialization,
	/// locking/unlocking of shared buffers, and asynchronous kernel execution.  
	/// Derived classes only need to implement backend‑specific behavior such as
	/// buffer creation (<see cref="OnCreate"/>) and kernel invocation (<see cref="OnRun"/>).
	/// </summary>
	/// <typeparam name="TD">
	/// The delegate type representing the native kernel function signature.  
	/// Must be a <see cref="Delegate"/> compatible with the loaded module.
	/// </typeparam>
	public abstract class NativeRAMKernel<TD> : IKernel<RamSharedBackend> where TD : Delegate{


        private string m_dllPath = "";
        private readonly Map<string, DeviceSharedBuffer<RamSharedBackend>> m_buffers;
        private Task? m_task;
        private bool m_running;
        private Module? m_module;
        private TD? m_function;
        private RamSharedBackend m_backend;

        /// <summary>
        /// Gets the backend instance used for RAM‑based shared buffer operations.
        /// </summary>
        public RamSharedBackend Backend => m_backend;

        /// <summary>
        /// Gets the native module loaded for this kernel.  
        /// The module represents the underlying shared library (DLL, SO, DYLIB)
        /// from which the kernel function delegate is resolved.  
        /// This property is assigned during <see cref="Create"/> and remains
        /// available for the entire lifetime of the kernel instance.
        /// </summary>
        public Module? Module { get => m_module; protected set => m_module = value; }

        /// <summary>
        /// Gets the resolved native kernel function delegate.  
        /// The delegate is created during <see cref="BeginRun"/> using the function
        /// name provided by the caller and represents the entry point of the
        /// native compute routine.  
        /// If the function cannot be resolved, this property is <c>null</c>.
        /// </summary>
        public TD? Function { get => m_function; protected set => m_function = value; }


        /// <summary>
        /// Indicates whether the kernel is currently running.
        /// </summary>
        public bool Running { get => m_running;  }
  
        /// <summary>
        /// Initializes a new RAM kernel instance with an empty buffer set and a
        /// default <see cref="RamSharedBackend"/> backend.  
        /// The native module is not loaded until <see cref="Create"/> is called.
        /// </summary>
        public NativeRAMKernel (  ) {
            
            m_buffers = new Map<string, DeviceSharedBuffer<RamSharedBackend>>();
            m_backend = new RamSharedBackend();


        }
        /// <summary>
        /// Loads the native module located at <paramref name="dllPath"/> and performs
        /// backend‑specific initialization via <see cref="OnCreate"/>.  
        /// Must be called before buffers are added or the kernel is executed.
        /// </summary>
        /// <param name="dllPath">The path to the native module containing the kernel function.</param>
        /// <returns>
        /// <c>true</c> if the module is successfully loaded and initialization succeeds;
        /// otherwise <c>false</c>.
        /// </returns>
        public bool Create( string dllPath ) {
            bool _ret = false;
            m_dllPath = dllPath;
            Module = NativeHost.LoadModule(m_dllPath);
            if(Module != null) {
                _ret = ( OnCreate() ) ; 
            }
            return _ret;
        }
        /// <summary>
        /// Adds a shared buffer to the kernel and converts the provided
        /// <see cref="DeviceBuffer"/> into a <see cref="DeviceSharedBuffer{RamSharedBackend}"/>
        /// using the RAM backend.  
        /// Derived classes may override <see cref="OnAddBuffer"/> to perform validation
        /// or backend‑specific configuration.
        /// </summary>
        public int AddBuffer(DeviceBuffer buffer, string name, int flags, object? configs ) {
            var x = buffer.ToShared<RamSharedBackend>(m_backend, flags, configs);

            int _ret = -1;

            if ( OnAddBuffer(name, x) ) {
                m_buffers.PushBack(name, x);
                _ret = m_buffers.Count();
            }
            return _ret;
        }
        /// <summary>
        /// Retrieves a shared buffer by its symbolic name.
        /// </summary>
        public DeviceSharedBuffer<RamSharedBackend>? GetBuffer ( string name ) {

            Optional<DeviceSharedBuffer<RamSharedBackend>> _ret =  m_buffers.Get(name);

            return _ret.IsNull ? null : _ret.Value!;
        }

        /// <summary>
        /// Removes a buffer by name and buffer type.
        /// </summary>
        /// <param name="name">The symbolic buffer name.</param>
        /// <returns>
        /// true by  removing the buffers, or a false on failure.
        /// </returns>
        public bool RemoveBuffer ( string name) {
            return m_buffers.Remove(name);
        }
        /// <summary>
        /// Removes a buffer by index and buffer type.
        /// </summary>
        /// <param name="index">The buffer index.</param>
        /// <returns>
        /// true by  removing the buffers, or a false on failure.
        /// </returns>
        public bool RemoveBuffer ( int index) {
            var pair = m_buffers.ElementAt(index);

            return RemoveBuffer(pair.First);
        }

        /// <summary>
        /// Begins a kernel execution session by loading the native function specified
        /// by <paramref name="strFunction"/> and locking all shared buffers.  
        /// Derived classes may override <see cref="OnBegin"/> to perform additional
        /// backend‑specific setup.
        /// </summary>
        public virtual bool BeginRun(string strFunction) {
            bool _ret = false;

            if ( OnBegin(strFunction) ) {
                _ret = LockedAllBuffer();
            }
            return _ret;
        }

        /// <summary>
        /// Ends the kernel execution session, unlocks all shared buffers, and performs
        /// backend cleanup via <see cref="OnEnd"/>.
        /// </summary>
        public virtual void EndRun () {
            OnEnd();
            
            UnLoockedAllBuffer();
        }
        /// <summary>
        /// Indicates whether the kernel is currently running.
        /// </summary>
        public bool IsRunning() {
            return m_running;
        }


        /// <summary>
        /// Executes the kernel asynchronously using the delegate loaded during
        /// <see cref="BeginRun"/>.  
        /// The actual kernel logic is implemented in <see cref="OnRun"/>.
        /// </summary>
        public unsafe bool Run ( object? options ) {
            if ( m_running )
                return false; // läuft schon

            m_running = true;

            m_task = Task.Run(() => {
                try {
                    if(m_function != null)
                        OnRun(m_function);
                    

                } finally {
                    m_running = false;
                }
            });

            return true;
        }

        /// <summary>
        /// Called after the native module is loaded.  
        /// Derived classes may override this method to create buffers or perform
        /// backend‑specific initialization.
        /// </summary>
        protected virtual bool OnCreate() {
            return true;
        }
        /// <summary>
        /// Called when a kernel execution session begins.  
        /// Loads the native function delegate using <see cref="NativeHost"/> and
        /// returns <c>true</c> if the function is successfully resolved.
        /// </summary>
        protected virtual bool OnBegin (string strFunction) {
            m_function = NativeHost.GetFunction<TD>(Module, strFunction);

            return (Function != null);
        }
        /// <summary>
        /// Called when a kernel execution session ends.  
        /// Derived classes may override this method to perform backend‑specific cleanup.
        /// </summary>
        protected virtual void OnEnd() {
            
        }
        /// <summary>
        /// Called when a buffer is added to the kernel.  
        /// Derived classes may override this method to validate buffer names or
        /// apply backend‑specific configuration.
        /// </summary>
        protected virtual bool OnAddBuffer (string name, DeviceSharedBuffer<RamSharedBackend> buffer ) {
            return true;
        }
        /// <summary>
        /// Executes the native kernel function.  
        /// Derived classes must override this method to implement backend‑specific
        /// compute logic using the resolved delegate <paramref name="function"/>.
        /// </summary>
        protected virtual bool OnRun( TD function ) {
            return true;
        }
        /// <summary>
        /// Locks all shared buffers.  
        /// If locking fails, previously locked buffers are unlocked to maintain
        /// consistent state.
        /// </summary>
        private bool LockedAllBuffer() {
            bool _ret = true;
            int _i = 0;

            try {
                for(; _i < m_buffers.Count ; _i ++) {
                    var opt = m_buffers.ElementAt(_i);
                
                    if ( opt.Second.Begin() == false ) {
                        _ret = false;
                        break;
                    }
                }
            } catch(Exception ) {
                _ret = false;
            }
            if(_ret == false) {
                UnLoockedAllBuffer(_i);
            }
            return _ret;
        }
        /// <summary>
        /// Unlocks all shared buffers up to the specified index.  
        /// Used to revert partial locking operations when an error occurs.
        /// </summary>
        private void UnLoockedAllBuffer(int k = 0) {
            long count = (k != 0) ? k : m_buffers.Length;

            for(long i = count ; i >= 0 ; i-- ) {
                if( m_buffers.ElementAt(i).Second.IsLocked)
                    m_buffers.ElementAt(i).Second.End();
            }
            
        }
    }
    /// @}
}
