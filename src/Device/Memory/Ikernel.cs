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

using SystemEx.Device.Intertropt;

namespace SystemEx.Device.Memory {

	/// \addtogroup Device
	/// @{

	/// <summary>
	/// Defines the interface for a compute kernel that operates on one or more
	/// <see cref="DeviceSharedBuffer{TDeviceSharedBackend}"/> instances.  
	/// A kernel represents a unit of computation that may run on different
	/// backends such as RAM, OpenCL, Vulkan, CUDA, or custom hardware.
	/// </summary>
	/// <typeparam name="TIDeviceSharedBackend">
	/// The backend type used for shared buffer operations.  
	/// Must implement <see cref="IDeviceSharedBackend"/>.
	/// </typeparam>
	public interface IKernel<TIDeviceSharedBackend>
        where TIDeviceSharedBackend : IDeviceSharedBackend {

        /// <summary>
        /// Gets the backend instance associated with this kernel.  
        /// The backend defines how shared buffers are allocated, synchronized,
        /// locked, and accessed during kernel execution.
        /// </summary>
        TIDeviceSharedBackend Backend { get; }

        /// <summary>
        /// Initializes the kernel by loading the native module located at
        /// <paramref name="dllPath"/>.  
        /// This typically resolves the underlying library, prepares backend‑specific
        /// resources, and performs any required setup before buffers can be added.
        /// </summary>
        /// <param name="dllPath">
        /// The absolute or symbolic path to the native module containing the kernel
        /// function(s).
        /// </param>
        /// <returns>
        /// <c>true</c> if the module is successfully loaded and the kernel is ready
        /// for buffer registration; otherwise <c>false</c>.
        /// </returns>
        bool Create ( string dllPath );

        /// <summary>
        /// Adds a shared buffer to the kernel with the specified name and usage type.
        /// </summary>
        /// <param name="buffer">The shared buffer to attach.</param>
        /// <param name="name">The symbolic name used by the kernel.</param>
        /// <param name="flags">Flags for the buffer</param>
        /// <param name="configs">Optional backend‑specific configuration data.</param>
        /// <returns>
        /// The index of the added buffer, or a negative value on failure.
        /// </returns>
        int AddBuffer ( DeviceBuffer buffer, string name, int flags, object? configs );

        /// <summary>
        /// Retrieves a shared buffer by its symbolic name.
        /// </summary>
        /// <param name="name">The symbolic buffer name.</param>
        /// <returns>
        /// The associated <see cref="DeviceSharedBuffer{TIDeviceSharedBackend}"/>,
        /// or <c>null</c> if no buffer with the given name exists.
        /// </returns>
        DeviceSharedBuffer<TIDeviceSharedBackend>? GetBuffer ( string name );

        /// <summary>
        /// Removes a buffer from the kernel by its symbolic name.  
        /// This typically releases backend‑specific resources and detaches the buffer
        /// from the kernel execution pipeline.
        /// </summary>
        /// <param name="name">The symbolic buffer name.</param>
        /// <returns>
        /// <c>true</c> if the buffer was successfully removed; otherwise <c>false</c>.
        /// </returns>
        bool RemoveBuffer ( string name );

        /// <summary>
        /// Removes a buffer from the kernel by its index in the internal buffer list.
        /// </summary>
        /// <param name="index">The buffer index.</param>
        /// <returns>
        /// <c>true</c> if the buffer was successfully removed; otherwise <c>false</c>.
        /// </returns>
        bool RemoveBuffer ( int index );

        /// <summary>
        /// Begins a kernel execution session and prepares the backend function
        /// identified by <paramref name="strFunction"/>.  
        /// Typically uploads buffers, locks shared memory regions, and performs
        /// backend‑specific initialization required before <see cref="Run"/> can be called.
        /// </summary>
        /// <param name="strFunction">The name of the kernel function to execute.</param>
        /// <returns><c>true</c> if initialization succeeds.</returns>
        bool BeginRun ( string strFunction );

        /// <summary>
        /// Executes the kernel with optional backend‑specific parameters.  
        /// Implementations may run synchronously or asynchronously depending on the
        /// backend and kernel type.
        /// </summary>
        /// <param name="options">Optional execution parameters.</param>
        /// <returns>
        /// <c>true</c> if the kernel starts successfully; otherwise <c>false</c>.
        /// </returns>
        bool Run ( object? options );

        /// <summary>
        /// Indicates whether the kernel is currently executing.  
        /// Useful for asynchronous backends or long‑running compute operations.
        /// </summary>
        bool IsRunning ();

        /// <summary>
        /// Ends the kernel execution session, synchronizes buffers, unlocks shared
        /// memory regions, and performs backend cleanup operations.  
        /// Must be called after <see cref="Run"/> completes.
        /// </summary>
        void EndRun ();
    }


	
}
