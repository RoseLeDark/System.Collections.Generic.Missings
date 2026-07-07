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

/**
 * @file ExampleRamKernelAdd.cs
 * @brief Beginner-friendly example demonstrating how to use a native RAM-based compute kernel in SystemEx.
 *
 * @ingroup Examples
 *
 * @details
 * This example shows how SystemEx executes a native C function through the RAM kernel backend.
 * It demonstrates the complete workflow:
 *
 * ### 1. What this example does
 * The example loads a native DLL containing a function with the following signature:
 *
 * @code
 * extern "C" int MyKernelFunction(int* A, int sizeA,
 *                                 int* B, int sizeB,
 *                                 int* C);
 * @endcode
 *
 * The function adds the integer stored in buffer A to the integer stored in buffer B,
 * and writes the result into buffer C.
 *
 * ### 2. How SystemEx handles native kernels
 * SystemEx provides a device abstraction layer that allows:
 * - Creating device buffers (`DeviceBuffer`)
 * - Sharing them with a backend (`DeviceSharedBuffer`)
 * - Passing unmanaged pointers to native code
 * - Executing native functions through delegates
 *
 * The class @ref ExampleRamKernelAdd inherits from @ref NativeRAMKernel,
 * which manages:
 * - Buffer registration
 * - Shared memory handling
 * - Native delegate resolution
 * - Kernel execution lifecycle (`Create`, `BeginRun`, `Run`, `EndRun`)
 *
 * ### 3. What this example teaches beginners
 * - How to allocate device buffers
 * - How to write initial values into buffers
 * - How to register buffers with a kernel
 * - How to call a native function using unmanaged pointers
 * - How to read back results from device memory
 *
 * ### 4. Execution flow
 * The program performs the following steps:
 *
 * 1. Create the kernel instance  
 * 2. Load the native DLL  
 * 3. Create buffers A and C  
 * 4. Write values into A and B  
 * 5. Register buffers with the kernel  
 * 6. Start the kernel (`BeginRun`)  
 * 7. Execute the native function (`Run`)  
 * 8. Wait until the kernel finishes  
 * 9. Read the result from buffer C  
 *
 * ### 5. Notes for beginners
 * - Device buffers represent memory that can be shared with native code.
 * - `UnmanagedObject.Point` is the raw pointer passed to the native function.
 * - The kernel backend ensures safe lifetime management of unmanaged memory.
 * - The example uses `RamSharedBackend`, which stores buffers in RAM.
 *
 * This example is intentionally simple and focuses on demonstrating the
 * mechanics of native kernel execution in SystemEx.
 *
 * @see NativeRAMKernel
 * @see DeviceBuffer
 * @see DeviceSharedBuffer
 * @see RamSharedBackend
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
    /// Example implementation of a RAM‑based compute kernel that adds two integers.
    ///
    /// <para>
    /// This class demonstrates how to use <see cref="NativeRAMKernel{T}"/>:
    /// - Buffers are created and registered in <see cref="OnCreate"/>.
    /// - The native function is executed in <see cref="OnRun"/>.
    /// - Unmanaged pointers are passed directly to the native DLL.
    /// </para>
    ///
    /// <para>
    /// The example is intentionally simple: it adds two integer values stored in
    /// device buffers <c>A</c> and <c>B</c>, and writes the result into <c>C</c>.
    /// </para>
    /// </summary>
    public class ExampleRamKernelAdd : NativeRAMKernel<MyKernelFunction> {
        /// <summary>
        /// Initializes a new instance of the example kernel.
        ///
        /// The native DLL is loaded later via <see cref="IKernel{T}.Create"/>.
        /// </summary>
        public ExampleRamKernelAdd () : base() {  }

        /// <summary>
        /// Creates and initializes the buffers required for this kernel.
        ///
        /// <para>
        /// In this example, only buffer <c>B</c> is created here.
        /// Buffers <c>A</c> and <c>C</c> are created in <c>Main()</c>.
        /// </para>
        ///
        /// <para>
        /// Steps:
        /// <list type="number">
        /// <item>Allocate a device buffer for integer B.</item>
        /// <item>Write the initial value (54) into the buffer.</item>
        /// <item>Register the buffer with the kernel.</item>
        /// </list>
        /// </para>
        ///
        /// </summary>
        /// <returns><c>true</c> if buffer creation succeeds.</returns>
        protected override bool OnCreate () {
            DeviceBuffer B = new DeviceBuffer(sizeof(int), CacheType.ToDevice);

            B.Write(0, 54U, Endian.System);

            AddBuffer(B, "BufferB", 0, null);

            return base.OnCreate();
        }
        /// <summary>
        /// Executes the native addition kernel.
        ///
        /// <para>
        /// The method:
        /// <list type="bullet">
        /// <item>Retrieves buffers A, B, and C</item>
        /// <item>Extracts their unmanaged memory handles</item>
        /// <item>Invokes the native function through the delegate</item>
        /// <item>Writes the result into buffer <c>C</c></item>
        /// </list>
        /// </para>
        ///
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

    /// <summary>
    /// Example program demonstrating how to run the RAM kernel.
    ///
    /// <para>
    /// Execution flow:
    /// <list type="number">
    /// <item>Create the kernel instance</item>
    /// <item>Load the native DLL</item>
    /// <item>Create buffers A and C</item>
    /// <item>Write initial value into A</item>
    /// <item>Register buffers with the kernel</item>
    /// <item>Begin kernel execution</item>
    /// <item>Run the native function</item>
    /// <item>Wait until the kernel finishes</item>
    /// <item>Read and print the result from buffer C</item>
    /// </list>
    /// </para>
    /// </summary>
    public static class Programm {
        public static void Main () {
            // Create kernel instance
            ExampleRamKernelAdd kernel = new ExampleRamKernelAdd();

            // Load native DLL containing the test function
            kernel.Create("PathToTestDllInNativC.DLL");

            DeviceBuffer A = new DeviceBuffer(sizeof(int), CacheType.ToDevice);
            DeviceBuffer C = new DeviceBuffer(sizeof(int), CacheType.FromDevice);

            // Write initial value into buffer A
            A.Write(0, 42U, Endian.System);

            // Create input buffer A and output buffer C
            kernel.AddBuffer(A, "BufferA", 0, null);
            kernel.AddBuffer(C, "BufferC", 0, null);

            Console.Write("Native Kernel IsRunning ");

            // Start kernel execution
            if ( kernel.BeginRun("NameOfTestFunctionWithExternC") ) {

                kernel.Run("Options Or NUll");

                // Wait until kernel finishes
                while ( kernel.IsRunning() ) {
                    Console.Write(".");
                    Task.Delay(10);
                }
                kernel.EndRun();
                Console.WriteLine(" Ready");

                // Read result from buffer C
                Console.WriteLine("C is: {0}", C.ReadInt(0, Endian.System));
            }
            else {
                Console.WriteLine(" Fail");
            }
        }
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
