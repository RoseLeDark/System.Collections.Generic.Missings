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
using SystemEx.Algorithms;
using SystemEx.Collections.Generic;

namespace MyFirstSystemEx {
	/// <summary>
	/// Example 02:
	/// Demonstrates how to use the SystemEx Vector<T> container together with
	/// the Find<T, C> utility to locate elements, count occurrences, and check
	/// existence. Also shows how to sort the vector using AsMultiSet().
	/// </summary>
	internal class Program {
		static Vector<int> _vector = new Vector<int>(new int[] { 437, 5, 9823, 76, 29, 12, 76, 3, 999, 42, 29, 18, 7, 250, 5 });

		/// <summary>
		/// SystemEx Example 2: Vector Find and Set
		///
		/// This example demonstrates how the SystemEx framework handles live vectors,
		/// snapshots, and search operations using the Finder utility.
		///
		/// Key concepts:
		/// - <b>Find</b>: Provides search operations (First, Last, Count, Exists) on a Vector.
		///   It works directly on the current state of the Vector, so indices reflect
		///   the live ordering of elements.
		/// - <b>MultiSet</b>: Converts a Vector into a sorted multiset structure. This
		///   operation reorders the underlying Vector and allows insertion of new values
		///   while maintaining sorted order. As a result, element indices change compared
		///   to the original unsorted state.
		/// - <b>Snapshot</b>: A copy of the Vector taken before modifications. This
		///   preserves the original ordering and allows stable index lookups even after
		///   the live Vector has been sorted or altered.
		///
		/// Program output:
		///
		/// SystemEx Example 2 Vector Find and Set
		///
		/// Original Vector
		/// -------------------------------
		///  437  5  9823  76  29  12  76  3  999  42  29  18  7  250  5
		///
		/// Finder on Live Vector (before sort)
		/// -------------------------------
		/// Index of First 29: 4
		/// Index of Last 29: 10
		/// Count of 29: 2
		/// Exists 30: False
		///
		/// Vector after MultiSet + Insert
		/// -------------------------------
		///  3  5  5  7  12  18  29  29  30  42  76  76  250  437  999  9823
		///
		/// Snapshot (Original State)
		/// -------------------------------
		///  437  5  9823  76  29  12  76  3  999  42  29  18  7  250  5
		///
		/// Finder on Live Vector (after sort)
		/// -------------------------------
		/// Index of First 29: 6
		/// Index of Last 29: 7
		/// Count of 29: 2
		/// Exists 30: True
		///
		/// Finder on Snapshot (original state)
		/// -------------------------------
		/// Index of First 29: 4
		/// Index of Last 29: 10
		/// Count of 29: 2
		/// Exists 30: False
		///
		/// This illustrates how Find reflects the current state of the Vector,
		/// while snapshots preserve the original ordering for stable analysis.
		/// </summary>
		static void Main ( string[] args ) {
			Console.WriteLine("SystemEx Example 2 Vector Find and Set\n");

			Stopwatch ws = Stopwatch.StartNew();

			PrintVector("Original Vector", ref _vector);
			RunFinder("Finder on Live Vector(before sort)", ref _vector);

			Vector<int> _snapshot = new Vector<int>(_vector);

			RunSetOperations("Apply MultiSet + Insert on Vector", ref _vector);

			PrintVector("Vector after MultiSet + Insert", ref _vector);
			PrintVector("Snapshot (Original State)", ref _snapshot);

			RunFinder("Finder on Live Vector (after sort)", ref _vector);
			RunFinder("Finder on Snapshot (original state)", ref _snapshot);

			ws.Stop();
			Console.WriteLine($"Elapsed Time: {ws.ElapsedMilliseconds} ms");
		}
		// Ausgabe des Vectors
		static void PrintVector ( string title, ref Vector<int> vec  ) {

			Console.WriteLine(title);
			Console.WriteLine("-------------------------------");
			foreach ( var item in vec )
				Console.Write($" {item} ");

			Console.WriteLine("\n");

		}
		// Finder-Demo
		static void RunFinder ( string title, ref Vector<int> vec ) {
			var finder = new Find<int, Vector<int>>(ref vec);

			Console.WriteLine(title);
			Console.WriteLine("-------------------------------");
			Console.WriteLine("Index of First 29: {0}", finder.First(29));
			Console.WriteLine("Index of Last 29: {0}", finder.Last(29));
			Console.WriteLine("Count of 29: {0}", finder.Of(29));
			Console.WriteLine("Exists 30: {0}", finder.Exists(30));

			Console.WriteLine("\n");
		}

		// Set-Demo
		static void RunSetOperations ( string title, ref Vector<int> vec ) {
			var _set = Vector<int>.AsMultiSet(ref vec, new Less<int>());
			_set.Insert(30);
		}
	}
}
 