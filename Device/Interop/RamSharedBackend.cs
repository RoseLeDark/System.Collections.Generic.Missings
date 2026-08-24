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

namespace SystemEx.Device.Intertropt {

	// \addtogroup SystemEx.Device.Intertropt 
	/// @{
	/// <summary>
	/// Represents a pinned unmanaged memory block used by SystemEx backends.
	///
	/// <para>
	/// This structure is essential for passing raw pointers to native code.
	/// When a byte array is pinned using <see cref="GCHandle"/>, the garbage
	/// collector is prevented from relocating it. This guarantees that the
	/// unmanaged pointer (<see cref="Point"/>) remains stable for the entire
	/// duration of the native kernel execution.
	/// </para>
	///
	/// <para>
	/// SystemEx uses <see cref="UnmanagedObject"/> as the bridge between
	/// managed buffers (<c>byte[]</c>) and native compute kernels.  
	/// It provides:
	/// <list type="bullet">
	/// <item><description>A stable unmanaged pointer</description></item>
	/// <item><description>The raw byte data</description></item>
	/// <item><description>The size of the memory block</description></item>
	/// <item><description>Automatic cleanup via <see cref="Dispose"/></description></item>
	/// </list>
	/// </para>
	///
	/// <para>
	/// This type is lightweight and disposable. Backends create it when a
	/// buffer is shared with native code and release it once the kernel
	/// finishes execution.
	/// </para>
	/// </summary>
	public struct UnmanagedObject : IDisposable {
        /// <summary>
        /// The pinned data 
        /// </summary>
        public GCHandle     Handle { get; internal set; }
        /// <summary>
        /// The raw byte data
        /// </summary>
        public byte[]       Data { get; internal set; }
        /// <summary>
        /// A stable unmanaged pointer
        /// </summary>
        public IntPtr       Point { get; internal set; }
        /// <summary>
        /// The size of the memory block
        /// </summary>
        public int          Size {  get; internal set; }


        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="handle"><see cref="GCHandle"/></param>
        /// <param name="data">The raw byte data</param>
        public UnmanagedObject (GCHandle handle, byte[] data) {
            Handle = handle;
            Point = Handle.AddrOfPinnedObject();
            Data = data;
            Size = data.Length;
        }

        /// <summary>
        /// Releases the pinned memory block.  
        /// This must be called once the native kernel no longer needs the pointer.
        /// </summary>
        public void Dispose() {
            if ( Handle.IsAllocated ) {
                Handle.Free();
            }
        }
    }

    /// <summary>
    /// RAM‑based backend for shared device buffers.
    ///
    /// <para>
    /// <see cref="RamSharedBackend"/> is the simplest backend in SystemEx.
    /// It stores buffer data directly in managed RAM and exposes it to native
    /// kernels by pinning the underlying <c>byte[]</c> and providing an
    /// unmanaged pointer via <see cref="UnmanagedObject"/>.
    /// </para>
    ///
    /// <para>
    /// This backend is primarily used for:
    /// <list type="bullet">
    /// <item><description>Testing native kernels without GPU or hardware devices</description></item>
    /// <item><description>Debugging compute logic in a safe environment</description></item>
    /// <item><description>Providing a fallback backend for platforms without specialized hardware</description></item>
    /// </list>
    /// </para>
    ///
    /// <para>
    /// The backend implements the <see cref="IDeviceSharedBackend"/> interface,
    /// allowing SystemEx to treat RAM buffers exactly like hardware buffers.
    /// This makes native kernel execution backend‑agnostic:  
    /// kernels do not care whether the memory comes from RAM, GPU, or another device.
    /// </para>
    /// </summary>
    public class RamSharedBackend : IDeviceSharedBackend {
        private byte[]? m_Buffer;

        /// <summary>
        /// Create a new instance
        /// </summary>
        internal RamSharedBackend() {
            m_Buffer = null;
        }
        /// <summary>
        /// Copies the contents of the backend buffer into a new managed array.
        /// 
        /// <para>
        /// A defensive copy is used instead of returning <c>m_Buffer</c> directly.
        /// This prevents callers from accidentally modifying the backend's internal
        /// memory and ensures that the shared buffer lifetime remains isolated from
        /// user code.
        /// </para>
        /// 
        /// <para>
        /// The unmanaged buffer is released after copying, because native kernels
        /// no longer require access to the pinned memory once execution has finished.
        /// </para>
        /// </summary>
        public long ReciveFromHardwareBuffer(out byte[] data, ref object hardwareBuffer) {
            if ( m_Buffer == null  ) throw new IOException(nameof(m_Buffer));

            long _size = m_Buffer.Length;

            data = new byte[_size];

           // data = m_Buffer;
            Array.Copy(m_Buffer, data, m_Buffer.Length);

            ((UnmanagedObject)hardwareBuffer).Dispose();

            m_Buffer = null;

            return _size;
        }
        /// <summary>
        /// Closes a hardware buffer and releases its unmanaged memory.
        /// </summary>
        public void CloseHardwareBuffer(ref object hardwareBuffer) {
            ((UnmanagedObject)hardwareBuffer).Dispose();

            m_Buffer = null;
        }
        /// <summary>
        /// Creates a pinned hardware buffer for writing data to the native kernel.
        ///
        /// <para>
        /// The provided cache is pinned and exposed to native code as a stable
        /// unmanaged pointer. Kernels can safely read or modify the buffer.
        /// </para>
        /// </summary>
        public long CreateWriteHardwareBuffer(byte[] cache, out object hardwareBuffer) {

            m_Buffer = cache;
            GCHandle handle = GCHandle.Alloc(m_Buffer, GCHandleType.Pinned);
            hardwareBuffer = new UnmanagedObject(handle, m_Buffer);

            return m_Buffer.Length;
        }
        /// <summary>
        /// Creates a pinned hardware buffer for reading data produced by the native kernel.
        ///
        /// <para>
        /// A new buffer of the requested size is allocated and pinned.  
        /// The native kernel writes its output directly into this memory block.
        /// </para>
        /// </summary>
        public long CreateReadHardwareBuffer(int size, out object hardwareBuffer) {
            m_Buffer = new byte[size];

            GCHandle handle = GCHandle.Alloc(m_Buffer, GCHandleType.Pinned);
            hardwareBuffer = new UnmanagedObject(handle, m_Buffer);

            return m_Buffer.Length;
        }
    }
	/// @}
}
