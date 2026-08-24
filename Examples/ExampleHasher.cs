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
 * @file SensorData.cs
 * @brief Beginner-friendly example demonstrating the new struct-based hashing system in SystemEx.
 *
 * @details
 * This file shows how a value type (struct) can participate in the SystemEx hashing pipeline.
 * Traditionally, hashing in SystemEx was based on the abstract class `Hashable`, which only works
 * for reference types (classes). Value types cannot inherit from an abstract base class, so a new
 * mechanism was introduced:
 *
 *   - `IHashable<T>`: A minimal interface that requires a deterministic byte representation.
 *   - `HashFactory`: A new component that reads the `HashAlgorithm` attribute and computes
 *                    32‑bit or 64‑bit hashes for both classes and structs.
 *
 * Together, these two additions allow any type—class or struct—to be hashed using the same
 * attribute-driven system.
 *
 * ## How hashing works (beginner explanation)
 *
 * 1. The type is annotated with `HashAlgorithmAttribute`, which selects:
 *      - The hashing algorithm (e.g., BernsteinHash)
 *      - The endianness used by the hasher
 *
 * 2. The type implements `IHashable<T>` and provides a `ToBytes()` method.
 *      - This method must return a deterministic byte sequence.
 *      - Field order, size, and endianness must never change.
 *
 * 3. `HashFactory`:
 *      - Reads the attribute
 *      - Instantiates the selected hasher
 *      - Calls `ToBytes()`
 *      - Computes a 32‑bit or 64‑bit hash
 *
 * This example demonstrates all of these steps in a simple, easy-to-understand struct.
 *
 * @note
 * Struct hashing is now fully supported thanks to `HashFactory`. This was not possible with the
 * old class-based `Hashable` system.
 *
 * @see IHashable
 * @see HashFactory
 * @see HashAlgorithmAttribute
 * @see BernsteinHash
 */

using SystemEx;
using SystemEx.Collections.Generic;
using SystemEx.Hash;
using SystemEx.Utils;

namespace Examples {


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
            var x =  HashFactory.Hash32(this, RandUtils.RandUInt(uint.MinValue, uint.MaxValue, Endian.System) );
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
            var x =  HashFactory.Hash64(this, RandUtils.RandUInt(uint.MinValue, uint.MaxValue, Endian.System) );
            if ( x.Value != 0 ) return x.Value;

            return 0;
        }
    }


    /// <summary>
    /// Example program showing:
    ///   - How to create a SensorData instance
    ///   - How Hash32 and Hash64 are produced
    /// </summary>
    public static class Programm {
        public static void Main () {

            // Create example instance
            SensorData Data = new SensorData();
            Data.Id = 0;
            Data.Value = 19;

            // 32‑bit hash
            Console.WriteLine(Data.GetHashCode());

            // 64‑bit hash
            Console.WriteLine(Data.GetHashCodeLong());
        }
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
