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

/** Plan:
 * IKernel OpenCL = new OpenCLKErnel("Main.cl", ....);
DeviceBuffer <OpenCLDeviceSharedBackend> memA = new DeviceBuffer(new OpenCLSharedBackend(..), 32, CacheType.ToDevice);
DeviceBuffer <OpenCLDeviceSharedBackend> memB = new DeviceBuffer(new OpenCLSharedBackend(..), 32, CacheType.ToDevice);
DeviceBuffer <OpenCLDeviceSharedBackend> memC =new DeviceBuffer(new OpenCLSharedBackend(..), 32, CacheType.Both);

memA.WriteInt32(0, 5, System.Missings.Binary.Endian.LittleEndian);
memB.WriteInt32(0, 56, System.Missings.Binary.Endian.LittleEndian);

OpenCL.AddBuffer(memA.ToSharedBuffer(), BufferType.Read); 
OpenCL.AddBuffer(memB.ToSharedBuffer(), BufferType.Read);
OpenCL.AddBuffer(memC.ToSharedBuffer(), BufferType.Both);

//OpenCL.Begin();
OpenCL.Run();

//while( OpenCL .IsRunning()) { }
//OpenCL.End();
*/


namespace SystemEx.Device.Memory {
    /// <summary>
    /// Represents a cache that can be shared with a hardware backend through
    /// <see cref="DeviceSharedBuffer{TDeviceSharedBackend}"/>.  
    /// A <see cref="DeviceBuffer"/> behaves like a normal <see cref="Cache"/>,
    /// but supports locking and shared‑memory synchronization for device kernels.
    /// </summary>
    public class DeviceBuffer : Cache {
        /// <summary>
        /// Internal lock object used to mark the buffer as shared and prevent
        /// concurrent modifications while a hardware backend is accessing it.
        /// </summary>
        private Lock m_lockObj = new Lock();

        /// <summary>
        /// Indicates whether the buffer is currently locked for shared access.
        /// </summary>
        public bool IsShared => IsLocked;

        /// <summary>
        /// Creates a new device buffer with the specified capacity and cache type.
        /// </summary>
        public DeviceBuffer(int capacity, CacheType type)
            : base(capacity, type) { }

        /// <summary>
        /// Creates a <see cref="DeviceSharedBuffer{TDeviceSharedBackend}"/> wrapper
        /// for this buffer using the specified backend and configuration.  
        /// The resulting shared buffer type is determined by the <see cref="CacheType"/>:
        /// <list type="bullet">
        /// <item><description><c>ToDevice</c> → <see cref="SharedCacheType.ReadOnly"/></description></item>
        /// <item><description><c>FromDevice</c> → <see cref="SharedCacheType.WriteOnly"/></description></item>
        /// <item><description>otherwise → <see cref="SharedCacheType.ReadWrite"/></description></item>
        /// </list>
        /// </summary>
        /// <typeparam name="TDeviceSharedBackend">The backend type.</typeparam>
        /// <param name="shared">The backend instance.</param>
        /// <param name="flags">Backend‑specific flags.</param>
        /// <param name="config">Optional backend configuration.</param>
        public DeviceSharedBuffer<TDeviceSharedBackend> ToShared<TDeviceSharedBackend>(
            TDeviceSharedBackend shared,
            int flags,
            object? config)
            where TDeviceSharedBackend : IDeviceSharedBackend {
            return this.Type switch
            {
                CacheType.ToDevice =>
                    new DeviceSharedBuffer<TDeviceSharedBackend>(this, SharedCacheType.ReadOnly, shared, flags, config),

                CacheType.FromDevice =>
                    new DeviceSharedBuffer<TDeviceSharedBackend>(this, SharedCacheType.WriteOnly, shared, flags, config),

                _ =>
                    new DeviceSharedBuffer<TDeviceSharedBackend>(this, SharedCacheType.ReadWrite, shared, flags, config),
            };
        }

        /// <summary>
        /// Locks the buffer for shared access.  
        /// Prevents modifications while a hardware backend is reading or writing.
        /// </summary>
        internal void Lock() {
            m_lockObj.EnterScope();
            IsLocked = true;
        }

        /// <summary>
        /// Unlocks the buffer after shared access has completed.
        /// </summary>
        internal void Unlock() {
            m_lockObj.Exit();
            IsLocked = false;
        }
    }

}






