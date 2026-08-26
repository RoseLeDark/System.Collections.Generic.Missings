
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
using System;
using System.Diagnostics;
using SystemEx;
using SystemEx.Utils;

namespace MyFirstSystemEx {

	/// <summary>
	/// Example 01:
	/// Demonstrates how to access SystemEx framework metadata and print
	/// a formatted build string to the console.
	/// 
	/// This example is intentionally minimal and serves as a "Hello World"
	/// entry point for new users who fork or clone SystemEx.
	/// </summary>
	internal class Program {

		/// <summary>
		/// SystemEx Example 3: BitView and Stopwatch
		///
		/// This example demonstrates:
		/// - <b>BitView</b>: Creates a live, mutable bit-level view over an integer.
		///   Any modification through the span directly updates the underlying variable.
		/// - <b>FlexSpanMode.System</b>: Index 0 corresponds to the least significant bit (LSB),
		///   index 1 to the next bit, and so on.
		/// - <b>Enumeration</b>: Iterating over the span prints each bit as 0 or 1,
		///   reflecting the current state of the integer.
		/// - <b>Stopwatch</b>: Measures execution time of the demo to show performance.
		///
		/// Program output illustrates:
		/// - Setting bit 2 changes the integer from 0 to 4.
		/// - Enumeration prints the bit pattern of the integer.
		/// - Assigning a new value (54476) updates the span immediately,
		///   and enumeration reflects the new bit sequence.
		/// - Elapsed time is reported at the end (e.g., ~3 ms).
		/// </summary>
		/// 
		/// Why this is useful:
		// -------------------
		// BitView provides a direct, zero-cost abstraction for bit-level manipulation.
		// In many real-world scenarios—binary protocols, encoders/decoders, compression,
		// graphics formats, hardware interfacing, and debugging—developers need precise
		// control over individual bits of numeric values. Traditional C# tools either
		// require manual bitmasking (verbose and error-prone) or allocate temporary
		// buffers (BitArray, byte[]), which break the connection to the original value.
		//
		// BitView solves this by offering:
		//   • A live view: changes apply directly to the underlying integer.
		//   • A span-like API: intuitive indexing instead of manual bitmask math.
		//   • Deterministic indexing modes: System, Reverse, and Ring.
		//   • Zero allocations: no heap usage, no copying, no boxing.
		//   • Full transparency: enumeration shows the real bit pattern at any moment.
		//
		// Why this matters:
		// -----------------
		// Bit-level operations are foundational in systems programming. They appear in:
		//   • network packet parsing (flags, headers, protocol fields)
		//   • binary serialization formats (PNG, ZIP, custom codecs)
		//   • embedded systems and device communication
		//   • cryptographic primitives and numeric algorithms
		//   • debugging tools and visualizers
		//   • performance-critical bitmask manipulation
		//
		// Key concepts shown in this example:
		// -----------------------------------
		// 1. Live bit view (no copy)
		// 2. Mutable bit indexing
		// 3. Deterministic indexing mode
		// 4. Enumeration of all bits
		// 5. Practical usage
		//
		// Output explanation:
		//   4      → after setting bit 2
		//   54476  → assigned new value
		//   00110011001010110000000000000000 → 32-bit representation of 54476
		//
		// WARNING: Never create a BitView over control variables (loop counters).
		// WARNING: FlexSpanMode.Ring creates infinite enumerations if used with foreach.
		static void Main ( string[] args ) {
			Console.WriteLine("SystemEx Example 3 BitView\n");

			Stopwatch ws = Stopwatch.StartNew();

			int a = 0;

			// Create a bit-span over the integer 'a'.
			// Mode: System → forward indexing (LSB first).
			var x = BitView.AsFlexSpan(ref a, FlexSpanMode.System);

			// Set bit 2 to true.
			// This writes directly into 'a': a |= (1 << 2).
			x[2] = true;

			Console.WriteLine("After setting bit 2:");
			Console.WriteLine("-------------------------------");
			Console.WriteLine(a);

			foreach ( var it in x )
				Console.Write(it ? 1 : 0);
			Console.WriteLine("\n");

			// Assign a new value to 'a'.
			// The span still references the same integer, so enumeration reflects the new bits.
			a = 54476;

			Console.WriteLine("After assigning 54476:");
			Console.WriteLine("-------------------------------");
			Console.WriteLine(a);

			foreach ( var it in x )
				Console.Write(it ? 1 : 0);
			Console.WriteLine("\n");

			ws.Stop();
			Console.WriteLine($"Elapsed Time: {ws.ElapsedMilliseconds} ms");
		}
	}
}
 