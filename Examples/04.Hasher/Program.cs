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
using SystemEx.Collections.Generic;
using SystemEx.Hash;
using SystemEx.Utils;

namespace MyFirstSystemEx {
	/// <summary>
	/// Simple example showing how a struct can participate in the SystemEx
	/// hashing system using <see cref="IHashable{SensorData}"/>.
	///
	/// This demonstrates:
	///   - How to attach a hash algorithm using the HashAlgorithm attribute
	///   - How to provide a deterministic byte representation via ToBytes()
	///   - How HashFactory computes 32‑bit and 64‑bit hashes for structs
	/// </summary>
	[HashAlgorithm(typeof(BernsteinHash), Endian.System)]
	public struct SensorData : IHashable<SensorData> {

		public uint Seed = 0;

		/// <summary>
		/// Example fields that will be included in the hash.
		/// </summary>
		public int Id;
		public float Value;
		/// <summary>
		/// Initializes the struct with example values.
		/// </summary>
		public SensorData () {
			Id = 748;
			Value = 1.00928f;
		}
		/// <summary>
		/// Converts this struct into a deterministic byte sequence.
		///
		/// Important notes for beginners:
		///   - The order of fields must never change.
		///   - The endianness must be chosen intentionally.
		///   - The size must always stay the same (here: 8 bytes).
		///
		/// HashFactory will use these bytes as input for the selected hash algorithm.
		/// </summary>
		public FixedVector<byte> ToBytes () {
			var b = new FixedVector<byte>(8);


			// Convert the integer field into 4 bytes.
			// Endian.System means: use the machine's native endianness.
			b.ReplaceRange(0, Id.ToBytes(Endian.System));

			// Convert the float field into 4 bytes.
			// Here we intentionally use BigEndian to show that each field
			// can choose its own byte order if needed.
			b.ReplaceRange(4, Value.ToBytes(Endian.BigEndian));

			return b;
		}
		/// <summary>
		/// Computes a 32‑bit hash for this struct.
		///
		/// How it works:
		///   1. HashFactory reads the HashAlgorithm attribute on this struct.
		///   2. It creates the specified hasher (BernsteinHash).
		///   3. It calls ToBytes() to get the raw data.
		///   4. It computes a 32‑bit hash using the given seed.
		///
		/// If the hash result is non‑zero, it is returned.
		/// Otherwise, the fallback is the default .NET hash code.
		/// </summary>
		public override int GetHashCode () {
			
			var x =  HashFactory.Hash32(this, (this.Seed == 0) ?  RandUtils.RandUInt(uint.MinValue, uint.MaxValue, Endian.System) : this.Seed );
			if ( x.Value != 0 ) return (int)x.Value;

			return base.GetHashCode();
		}
		/// <summary>
		/// Computes a 64‑bit hash for this struct.
		///
		/// This works exactly like GetHashCode(), but produces a 64‑bit value.
		/// If the computed hash is zero, the method returns 0 as a fallback.
		/// </summary>
		public ulong GetHashCodeLong () {
			var x =  HashFactory.Hash64(this, (this.Seed == 0) ?  RandUtils.RandUInt(uint.MinValue, uint.MaxValue, Endian.System) : this.Seed );
			if ( x.Value != 0 ) return x.Value;

			return 0;
		}
	}



	/// <summary>
	/// Demonstrates how a <c>struct</c> participates in the SystemEx hashing pipeline.
	/// 
	/// <para>
	/// Hashing in SystemEx is now entirely based on the combination of:
	/// <list type="bullet">
	///   <item><description><see cref="HashAlgorithmAttribute"/> to select the algorithm and endianness.</description></item>
	///   <item><description><see cref="IHashable{T}"/> to enforce a deterministic byte representation.</description></item>
	///   <item><description><see cref="HashFactory"/> to compute 32‑bit and 64‑bit hashes.</description></item>
	/// </list>
	/// </para>
	/// 
	/// <para>
	/// <b>Step‑by‑step process:</b>
	/// <ol>
	///   <li>The struct is annotated with <c>[HashAlgorithm(typeof(BernsteinHash), Endian.System)]</c>.</li>
	///   <li>The struct implements <c>IHashable&lt;SensorData&gt;</c> and defines <c>ToBytes()</c>.</li>
	///   <li><c>ToBytes()</c> returns a fixed, deterministic sequence of bytes representing the fields.</li>
	///   <li><c>HashFactory</c> reads the attribute, instantiates the specified hasher, and consumes the bytes.</li>
	///   <li>The hasher computes a 32‑bit or 64‑bit hash value.</li>
	/// </ol>
	/// </para>
	/// 
	/// <para>
	/// <b>Important rules:</b>
	/// <list type="bullet">
	///   <item><description>Field order in <c>ToBytes()</c> must never change.</description></item>
	///   <item><description>Endianness must remain consistent across releases.</description></item>
	///   <item><description>The byte sequence must always have the same length.</description></item>
	/// </list>
	/// </para>
	/// 
	/// <para>
	/// <b>Program output:</b>
	/// <code>
	/// SystemEx Example 4 Costum GetHashCode - Seed is Random
	///
	/// SensorData (ID: 748, Value: 1,00928) Bernstein Hash32: 0x7CAC31D2
	/// SensorData ( ID: 748, Value: 1,00928) Bernstein Hash64: 0xFB5D48F7965A0B57
	/// Elapsed Time: 6 ms
	/// </code>
	/// </para>
	/// 
	/// <para>
	/// <b>Explanation:</b>
	/// <list type="bullet">
	///   <item><description><c>Hash32</c> shows the 32‑bit Bernstein hash of the struct’s byte sequence.</description></item>
	///   <item><description><c>Hash64</c> shows the 64‑bit Bernstein hash of the same struct.</description></item>
	///   <item><description>The elapsed time is dominated by console output; hashing itself is near‑instant.</description></item>
	/// </list>
	/// </para>
	/// 
	/// <para>
	/// <b>Note:</b> The old <c>Hashable</c> base class has been removed. All new types must use
	/// <c>IHashable&lt;T&gt;</c> together with <c>HashFactory</c>.
	/// </para>
	/// </summary>

	internal class Program {

		public static void Main () {
			Console.WriteLine("SystemEx Example 4 Costum GetHashCode\n");

			Stopwatch ws = Stopwatch.StartNew();

			// Create example instance
			SensorData Data = new SensorData();

			// Remove // when you use a static seed for example
			Data.Seed = 563724;

			Console.WriteLine("Hash with {0} seed: ", (Data.Seed != 0) ? "static" : "random");
			var hash64 = Data.GetHashCodeLong().ToString("X");
			var hash32 = Data.GetHashCode().ToString("X");


			// 32‑bit hash
			Console.WriteLine($"SensorData (ID: {Data.Id}, Value: {Data.Value}) Bernstein Hash32: 0x{hash32}");

			// 64‑bit hash
			Console.WriteLine($"SensorData (ID: {Data.Id}, Value: {Data.Value}) Bernstein Hash64: 0x{hash64}");


			ws.Stop();
			Console.WriteLine($"Elapsed Time: {ws.ElapsedMilliseconds} ms");
		}
	}
}
 