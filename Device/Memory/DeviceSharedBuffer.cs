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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.Versioning;

namespace SystemEx.Device.Memory {
	// \addtogroup SystemEx.Device.Memory
	/// @{
	/// <summary>
	/// Specifies how a shared cache interacts with the hardware backend.  
	/// Determines the direction of data flow between the managed cache and the
	/// device‑specific hardware buffer.
	/// </summary>
	public enum SharedCacheType : byte {
        /// <summary>
        /// Data flows from the cache to the hardware buffer only.  
        /// Hardware cannot write back into the cache.
        /// </summary>
        ReadOnly,

        /// <summary>
        /// Data flows from the hardware buffer into the cache only.  
        /// The cache does not upload its contents to the hardware.
        /// </summary>
        WriteOnly,

        /// <summary>
        /// Bidirectional synchronization between cache and hardware buffer.  
        /// The cache uploads its data to the hardware, and the hardware may write
        /// modified data back into the cache.
        /// </summary>
        ReadWrite
    }


    /// <summary>
    /// Represents a shared memory bridge between a <see cref="DeviceBuffer"/> and a
    /// hardware‑specific backend implementing <see cref="IDeviceSharedBackend"/>.  
    /// Manages the lifecycle of a hardware buffer, including creation, upload,
    /// download, and cleanup, depending on the configured <see cref="SharedCacheType"/>.
    /// </summary>
    /// <typeparam name="TDeviceSharedBackend">
    /// The backend type responsible for allocating, reading, writing, and closing
    /// the hardware buffer.
    /// </typeparam>
    public class DeviceSharedBuffer<TDeviceSharedBackend>
        where TDeviceSharedBackend : IDeviceSharedBackend {
        /// <summary>
        /// The underlying cache whose contents are synchronized with the hardware buffer.
        /// </summary>
        private DeviceBuffer m_cache;

        /// <summary>
        /// The configured access mode controlling read/write direction.
        /// </summary>
        private readonly SharedCacheType m_type;

        /// <summary>
        /// The backend responsible for hardware buffer operations.
        /// </summary>
        private TDeviceSharedBackend m_backend;

        /// <summary>
        /// The opaque backend‑specific hardware buffer handle.
        /// </summary>
        internal object m_hardwareBuffer;

        /// <summary>
        /// Indicates whether the hardware buffer may be read from.
        /// </summary>
        public bool CanRead =>
            m_type == SharedCacheType.ReadOnly ||
            m_type == SharedCacheType.ReadWrite;

        /// <summary>
        /// Indicates whether the hardware buffer may be written to.
        /// </summary>
        public bool CanWrite =>
            m_type == SharedCacheType.WriteOnly ||
            m_type == SharedCacheType.ReadWrite;

        /// <summary>
        /// Indicates whether the shared buffer supports both reading and writing.
        /// </summary>
        public bool IsReadWrite => m_type == SharedCacheType.ReadWrite;

        /// <summary>
        /// Indicates whether the underlying cache is currently locked for shared access.
        /// </summary>
        public bool IsLocked => m_cache.IsShared;

        /// <summary>
        /// Gets the backend‑specific hardware buffer handle.
        /// </summary>
        public object HardwareBuffer => m_hardwareBuffer;

        /// <summary>
        /// Creates a new shared buffer wrapper for the specified cache and backend.
        /// </summary>
        internal DeviceSharedBuffer( DeviceBuffer cache, TDeviceSharedBackend backend, int flags, object? config) {

            m_cache = cache;

            if ( cache.Type == Collections.Generic.CacheType.ToDevice )
                m_type = SharedCacheType.WriteOnly;
            else if ( cache.Type == Collections.Generic.CacheType.FromDevice )
                m_type = SharedCacheType.ReadOnly;
            else if ( cache.Type == Collections.Generic.CacheType.Both )
                m_type = SharedCacheType.ReadWrite;
            else
                throw new NotSupportedException("DeviceBuffer is System Only ");

            m_backend = backend;
            m_hardwareBuffer = new object();
        }

        /// <summary>
        /// Begins a shared memory session by locking the cache and creating the
        /// appropriate hardware buffer.  
        /// For <see cref="SharedCacheType.ReadOnly"/>, a read buffer is created.  
        /// For <see cref="SharedCacheType.WriteOnly"/> and <see cref="SharedCacheType.ReadWrite"/>,
        /// the cache contents are uploaded to a write buffer.
        /// </summary>
        /// <returns><c>true</c> if the operation succeeds.</returns>
        public bool Begin() {
            m_cache.Lock();

            if ( m_type != SharedCacheType.ReadOnly ) {
                m_backend.CreateWriteHardwareBuffer(m_cache.ToArray(), out m_hardwareBuffer);
            } else {
                m_backend.CreateReadHardwareBuffer((int)m_cache.Length, out m_hardwareBuffer);
            }

            return true;
        }

        /// <summary>
        /// Ends the shared memory session by downloading data from the hardware buffer
        /// when required and unlocking the cache.  
        /// For write‑enabled modes, the backend writes modified data back into the cache.  
        /// For read‑only mode, the hardware buffer is simply closed.
        /// </summary>
        public void End() {

            if ( m_type != SharedCacheType.ReadOnly ) {
                byte[] cachetmp;
                long startPos = m_backend.ReciveFromHardwareBuffer(out cachetmp, ref m_hardwareBuffer);

                m_cache.WriteRange((ulong)startPos, cachetmp);
            } else {
                m_backend.CloseHardwareBuffer(ref m_hardwareBuffer);
            }

            m_cache.Unlock();
        }
    }

	/// @}
}
