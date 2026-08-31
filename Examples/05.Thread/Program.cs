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
using SystemEx.Threading;
using Barrier = SystemEx.Threading.Barrier;

namespace MyFirstSystemEx {

	/// <summary>
	/// Demonstrates how multiple <c>LightThread</c> workers can be synchronized using a 
	/// user‑mode <c>Barrier</c>. The number of worker threads is automatically scaled to 
	/// the number of available CPU cores, making the example suitable for both small and 
	/// large systems.
	///
	/// Each worker thread performs simulated work, signals its arrival at the barrier, 
	/// and then waits in a lightweight user‑mode loop until the barrier is opened. 
	/// Unlike traditional blocking barriers, this implementation does not park threads 
	/// in the operating system kernel. Instead, waiting threads yield execution, keeping 
	/// synchronization fully in user space and avoiding kernel transitions.
	///
	/// The main thread acts as the barrier controller: it periodically checks whether all 
	/// workers have arrived, opens the barrier, advances the phase counter, and invokes 
	/// the completion callback. Once a predefined number of phases has been completed, 
	/// the main thread signals all workers to terminate cleanly.
	///
	/// Because <c>Console.WriteLine()</c> is not thread‑safe, the console output may 
	/// appear unordered or interleaved. This is expected behavior and does not indicate 
	/// any problem with the barrier or thread logic. All worker threads still synchronize 
	/// correctly at each barrier phase.
	///
	/// In short: the barrier provides deterministic phase synchronization, while the 
	/// console output remains non‑deterministic due to concurrent printing.
	/// </summary>
	internal class Program {
		// Number of worker threads = number of CPU cores
		static int NUM_THREAD = Environment.ProcessorCount ;
		static int NUM_RUNS = 3; // 3-5

		// Flag used to stop all threads after a few barrier cycles
		static bool endMarker = false;

		// Barrier that waits for all threads to arrive before continuing
		static Barrier barrier = new((uint)NUM_THREAD);

		static void Main ( string[] args ) {
			Console.WriteLine($"SystemEx Example 5 — LightThread + Barrier Demo ( {NUM_THREAD} Threads, {NUM_RUNS} Barrier Runs)\n\n");

			// This callback runs every time all threads reach the barrier
			barrier.OnComplition = ( index, sender ) => {
				index += 1;
				Console.WriteLine($"\n>> Barrier: reached: all threads synchronized Run: {index} from {NUM_RUNS} \n");

				// Stop after a few completed barrier phases
				if ( index >= NUM_RUNS ) endMarker = true;
			};

			// Create and start all worker threads
			LightThread[] threads = new LightThread[NUM_THREAD];
			int ThreadStackSize = (int)Conversion.SizeCalc("4MI");
			for (int i = 0 ; i < NUM_THREAD ; i++) {

				Console.Write($"Create Task{i} with ThreadPriority: Normal and startStackSize: {ThreadStackSize} ... ");
				threads[i] = new($"Task{i}", ThreadPriority.Normal, ThreadStackSize) { OnTask = Task_OnRun };
				if(threads[i].Start(0)) {
					Console.WriteLine("Started");
				}
				// Small delay to ensure the thread have started
				Thread.Sleep(10);
			}

			

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
			Console.WriteLine($">> {e.Name} (ID: {e.ID}) started");

			// Simulate some random work time
			Thread.Sleep(new Random().Next(1000, 3000));

			// Thread arrives at the barrier and waits for others
			Console.WriteLine($">> {e.Name} (ID: {e.ID}) waiting at barrier");
			barrier.ArriveAndWait(1);

			// All threads passed the barrier together
			Console.WriteLine($">> {e.Name} (ID: {e.ID}) passed barrier");

			// More simulated work
			Thread.Sleep(new Random().Next(1000, 3000));

			// Thread finishes its cycle
			Console.WriteLine($">> {e.Name} (ID: {e.ID}) completed");

			return 0;

		}

	}
}
 