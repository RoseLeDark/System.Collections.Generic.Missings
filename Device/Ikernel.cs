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

    /// <summary>
    /// Specifies how a buffer is used by a kernel during execution.  
    /// Determines whether data flows from cache to hardware, hardware to cache,
    /// or both.
    /// </summary>
    public enum BufferType {
        /// <summary>
        /// The buffer is written by the kernel (cache → hardware).
        /// </summary>
        Write,

        /// <summary>
        /// The buffer is read by the kernel (hardware → cache).
        /// </summary>
        Read,

        /// <summary>
        /// The buffer is both read and written by the kernel.
        /// </summary>
        Both
    }

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
        /// Adds a shared buffer to the kernel with the specified name and usage type.
        /// </summary>
        /// <param name="buffer">The shared buffer to attach.</param>
        /// <param name="name">The symbolic name used by the kernel.</param>
        /// <param name="type">The buffer access mode.</param>
        /// <param name="confgs">Optional backend‑specific configuration data.</param>
        /// <returns>
        /// The index of the added buffer, or a negative value on failure.
        /// </returns>
        int AddBuffer(DeviceSharedBuffer<TIDeviceSharedBackend> buffer,
                      string name,
                      BufferType type,
                      object? confgs);

        /// <summary>
        /// Removes a buffer by name and buffer type.
        /// </summary>
        /// <param name="name">The symbolic buffer name.</param>
        /// <param name="type">The buffer access mode.</param>
        /// <returns>
        /// The number of remaining buffers, or a negative value on failure.
        /// </returns>
        int RemoveBuffer(string name, BufferType type);

        /// <summary>
        /// Removes a buffer by index and buffer type.
        /// </summary>
        /// <param name="index">The buffer index.</param>
        /// <param name="type">The buffer access mode.</param>
        /// <returns>
        /// The number of remaining buffers, or a negative value on failure.
        /// </returns>
        int RemoveBuffer(int index, BufferType type);

        /// <summary>
        /// Begins a kernel execution session and prepares the backend function
        /// identified by <paramref name="strFunction"/>.  
        /// Typically uploads buffers and performs backend‑specific setup.
        /// </summary>
        /// <param name="strFunction">The name of the kernel function to execute.</param>
        /// <returns><c>true</c> if initialization succeeds.</returns>
        bool Begin(string strFunction);

        /// <summary>
        /// Executes the kernel with optional backend‑specific parameters.
        /// </summary>
        /// <param name="options">Optional execution parameters.</param>
        /// <returns><c>true</c> if the kernel starts successfully.</returns>
        bool Run(object? options);

        /// <summary>
        /// Indicates whether the kernel is currently running.
        /// </summary>
        bool IsRunning();

        /// <summary>
        /// Ends the kernel execution session, synchronizes buffers, and performs
        /// backend cleanup operations.
        /// </summary>
        void End();
    }


}
