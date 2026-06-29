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
using SystemEx.Collections.Generic;
using SystemEx.Device;
using SystemEx.Device.Intertropt;
using SystemEx.Device.Memory;

namespace Examples {
    /// \addtogroup Examples
    /// @{
    /// <summary>
    /// Delegate representing the native kernel function signature used by this example.  
    /// The function performs an addition on two input buffers <c>A</c> and <c>B</c> and
    /// writes the result into buffer <c>C</c>.  
    /// All buffers are passed as unmanaged pointers along with their sizes.
    /// </summary>
    public delegate int MyKernelFunction ( IntPtr A, int dwSize, IntPtr B, int dwSizeB, IntPtr C );

    /// <summary>
    /// Example implementation of a RAM‑based compute kernel that adds two integer
    /// values stored in device buffers.  
    /// This class demonstrates how to use <see cref="NativeRAMKernel{TD}"/> by
    /// providing buffer creation logic (<see cref="OnCreate"/>) and the actual
    /// kernel invocation (<see cref="OnRun"/>).
    /// </summary>
    public class ExampleRamKernelAdd : NativeRAMKernel<MyKernelFunction> {
        /// <summary>
        /// Initializes a new instance of the example kernel.  
        /// The native module is loaded later via <see cref="IKernel{T}.Create"/>.
        /// </summary>
        public ExampleRamKernelAdd () : base() {  }

        /// <summary>
        /// Creates and initializes the buffers required for this kernel.  
        /// Three buffers are allocated:
        /// <list type="bullet">
        /// <item><description><c>BufferA</c> — input value A</description></item>
        /// <item><description><c>BufferB</c> — input value B</description></item>
        /// <item><description><c>BufferC</c> — output value C</description></item>
        /// </list>
        /// Initial values are written into <c>A</c> and <c>B</c>.  
        /// All buffers are registered with the kernel using <see cref="NativeRAMKernel<MyKernelFunction>.AddBuffer"/>.
        /// </summary>
        protected override bool OnCreate () {
            DeviceBuffer A = new DeviceBuffer(sizeof(int), CacheType.ToDevice);
            DeviceBuffer B = new DeviceBuffer(sizeof(int), CacheType.ToDevice);
            DeviceBuffer C = new DeviceBuffer(sizeof(int), CacheType.FromDevice);

            A.Write(0, 42U, Endian.LittleEndian);
            B.Write(0, 54U, Endian.LittleEndian);

            AddBuffer(A, "BufferA", 0, null);
            AddBuffer(B, "BufferB", 0, null);
            AddBuffer(C, "BufferC", 0, null);

            return base.OnCreate();
        }
        /// <summary>
        /// Executes the native addition kernel using the resolved delegate.  
        /// The method retrieves the shared buffers, extracts their unmanaged
        /// memory handles, and invokes the native function.  
        /// The result is written into the output buffer <c>C</c>.
        /// </summary>
        /// <param name="function">
        /// The resolved native kernel delegate.  
        /// If <c>null</c>, the kernel cannot be executed.
        /// </param>
        /// <returns><c>true</c> if execution succeeds.</returns>
        protected override bool OnRun ( MyKernelFunction function ) {

            
            DeviceSharedBuffer < RamSharedBackend >? A = base.GetBuffer("BufferA")!;
            DeviceSharedBuffer < RamSharedBackend >? B = base.GetBuffer("BufferB")!;
            DeviceSharedBuffer < RamSharedBackend >? C = base.GetBuffer("BufferC")!;

            var unmanagedA = (UnmanagedObject)A.HardwareBuffer!;
            var unmanagedB = (UnmanagedObject)B.HardwareBuffer!;
            var unmanagedC = (UnmanagedObject)C.HardwareBuffer!;

            unmanagedC.Size = function(unmanagedA.Point, unmanagedA.Size, unmanagedB.Point, unmanagedB.Size, unmanagedC.Point);

            return true;
        }
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
