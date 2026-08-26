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
using SystemEx.Threading;
using Barrier = SystemEx.Threading.Barrier;

namespace MyFirstSystemEx {

	/// <summary>
	/// Demonstrates how multiple <c>LightThread</c> workers can be synchronized using a 
	/// user‑mode <c>Barrier</c>. The number of threads is automatically scaled to the 
	/// number of available CPU cores, making the example suitable for both small and 
	/// large systems.
	///
	/// Each thread performs simulated work, arrives at the barrier, waits until all 
	/// other threads have reached the same synchronization point, and then continues 
	/// execution. After every completed synchronization cycle, the barrier increments 
	/// its phase index and invokes the completion callback. Once a predefined number 
	/// of phases has been reached, the main thread signals all workers to terminate.
	///
	/// Although the barrier guarantees correct logical synchronization, the console 
	/// output may appear unordered or interleaved. This is expected behavior: 
	/// <c>Console.WriteLine()</c> is not thread‑safe, and concurrent writes from 
	/// multiple threads naturally produce non‑deterministic output. The barrier 
	/// ensures that all threads reach and pass the synchronization point together, 
	/// but it does not serialize or coordinate console printing.
	///
	/// In short: the barrier is fully synchronized, but the console output is not. 
	/// The seemingly jumbled text does not indicate any problem with the barrier or 
	/// the thread logic.
	/// </summary>
	internal class Program {
		// Number of worker threads = number of CPU cores
		static int NUM_THREAD = Environment.ProcessorCount;

		// Flag used to stop all threads after a few barrier cycles
		static bool endMarker = false;

		// Barrier that waits for all threads to arrive before continuing
		static Barrier barrier = new((uint)NUM_THREAD);

		static void Main ( string[] args ) {
			Console.WriteLine("SystemEx Example 5 LightThread + Barrier Demo");

			// This callback runs every time all threads reach the barrier
			barrier.OnComplition = ( index, sender ) => {
				Console.WriteLine($"\n-- Barrier reached: all threads synchronized -- {index}\n");

				// Stop after a few completed barrier phases
				if ( index >= 3 ) endMarker = true;
			};

			// Create and start all worker threads
			LightThread[] threads = new LightThread[NUM_THREAD];
			for(int i = 0 ; i < NUM_THREAD ; i++) {
				threads[i] = new($"Task{i}", ThreadPriority.Normal, 4096) { OnTask = Task_OnRun };
				threads[i].Start(0);
			}

			// Small delay to ensure all threads have started
			Thread.Sleep(10);

			// Main thread waits for barrier cycles until endMarker becomes true
			while ( !endMarker ) {
				barrier.WaitOpen(10);   // Wait until barrier is open
				Thread.Sleep(300);      // Slow down the loop a bit
			}
		}
		static int Task_OnRun ( ThreadEx e, object? userdata ) {
			// If the program should stop, exit the thread
			if ( endMarker ) return 1;

			// Thread begins its work
			Console.WriteLine($"{e.Name} (ID: {e.ID}) started");

			// Simulate some random work time
			Thread.Sleep(new Random().Next(1000, 3000));

			// Thread arrives at the barrier and waits for others
			Console.WriteLine($"{e.Name} (ID: {e.ID}) waiting at barrier");
			barrier.ArriveAndWait(1);

			// All threads passed the barrier together
			Console.WriteLine($"{e.Name} (ID: {e.ID}) passed barrier");

			// More simulated work
			Thread.Sleep(new Random().Next(1000, 3000));

			// Thread finishes its cycle
			Console.WriteLine($"{e.Name} (ID: {e.ID}) completed");

			return 0;

		}

	}
}
 