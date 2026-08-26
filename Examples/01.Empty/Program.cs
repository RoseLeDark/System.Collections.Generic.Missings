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
using System.Diagnostics;
using SystemEx;

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
		/// Application entry point.
		/// Prints a greeting followed by SystemEx build information.
		/// </summary>
		/// <param name="args">Command-line arguments (unused).</param>
		static void Main ( string[] args ) {
			Stopwatch ws = Stopwatch.StartNew();

			Console.WriteLine("Hallo " + Framework.BuildString() );

			ws.Stop();
			Console.WriteLine($"Elapsed Time: {ws.ElapsedMilliseconds} ms");
		}
	}
}
 