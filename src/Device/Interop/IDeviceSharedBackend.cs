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

namespace SystemEx.Device.Intertropt {

	/// <summary>
	/// Defines the low‑level backend interface used by <c>DeviceSharedBuffer</c>
	/// and related shared‑memory abstractions.  
	/// A backend represents a hardware‑specific buffer implementation such as
	/// RAM, OpenCL, Vulkan, CUDA, or any custom device memory.  
	/// Each backend is responsible for creating, writing, reading, and closing
	/// its own hardware buffer handle.
	/// </summary>
	public interface IDeviceSharedBackend {
        /// <summary>
        /// Creates a hardware buffer intended for reading operations.  
        /// The backend allocates a device‑specific buffer of the given size and
        /// returns an opaque handle via <paramref name="hardwareBuffer"/>.
        /// </summary>
        /// <param name="size">The size of the buffer to allocate.</param>
        /// <param name="hardwareBuffer">The backend‑specific buffer handle.</param>
        /// <returns>
        /// A backend‑specific status code.  
        /// Typically <c>0</c> for success, non‑zero for errors.
        /// </returns>
        long CreateReadHardwareBuffer(int size, out object hardwareBuffer);

        /// <summary>
        /// Creates a hardware buffer intended for writing operations and initializes
        /// it with the provided cache data.  
        /// The backend allocates a device‑specific buffer and uploads the data.
        /// </summary>
        /// <param name="cache">The data to write into the hardware buffer.</param>
        /// <param name="hardwareBuffer">The backend‑specific buffer handle.</param>
        /// <returns>
        /// A backend‑specific status code.  
        /// Typically <c>0</c> for success, non‑zero for errors.
        /// </returns>
        long CreateWriteHardwareBuffer(byte[] cache, out object hardwareBuffer);

        /// <summary>
        /// Reads data from the hardware buffer into a managed byte array.  
        /// The backend determines how the buffer is accessed and how much data is returned.
        /// </summary>
        /// <param name="data">The byte array containing the received data.</param>
        /// <param name="hardwareBuffer">The backend‑specific buffer handle.</param>
        /// <returns>
        /// The number of bytes received, or a negative value on error.
        /// </returns>
        long ReciveFromHardwareBuffer(out byte[] data, ref object hardwareBuffer);

        /// <summary>
        /// Releases and invalidates the backend‑specific hardware buffer handle.  
        /// After this call, the buffer must no longer be used.
        /// </summary>
        /// <param name="hardwareBuffer">The backend‑specific buffer handle.</param>
        void CloseHardwareBuffer(ref object hardwareBuffer);
    }

	
}
