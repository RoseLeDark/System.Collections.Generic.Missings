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
using SystemEx.Hash;
using SystemEx.Collections.Generic;

namespace Examples {
    /// \addtogroup Examples
    /// @{
    /// <summary>
    /// A very simple example hasher.
    /// </summary>
    /// Purpose:
    ///   - Demonstrates how to implement a custom hasher for SystemEx.
    ///   - Not optimized, not cryptographic.
    ///   - Only deterministic byte processing.
    ///
    /// Notes:
    ///   - Works on Array<byte> (SystemEx container).
    ///   - Endian is accepted but not used in this simple example.
    public sealed class SimpleHasher : IHasher {

        /// <summary>
        /// Computes a simple 32‑bit hash from the given bytes.
        /// </summary>
        public Hash32 Compute ( Array<byte> input, Endian endian ) {
            if ( input == null || input.Count == 0 )
                return new Hash32(0);

            int hash = 0;

            // Simple deterministic byte loop
            for ( int i = 0 ; i < input.Count ; i++ ) {
                hash = (hash * 31) ^ input[i];
            }

            return new Hash32(hash);
        }

        /// <summary>
        /// Computes a simple 64‑bit hash from the given bytes.
        /// </summary>
        public Hash64 ComputeLong ( Array<byte> input, Endian endian ) {
            if ( input == null || input.Count == 0 )
                return new Hash64(0);

            long hash = 0;

            // Larger multiplier for 64‑bit
            for ( int i = 0 ; i < input.Count ; i++ ) {
                hash = (hash * 1315423911L) ^ input[i];
            }

            return new Hash64(hash);
        }
    }


    /// <summary>
    /// Example data class using HashableObject.
    /// </summary>
    /// The HashAlgorithm attribute tells HashableObject:
    ///   - Use SimpleHasher
    ///   - Use LittleEndian for byte interpretation
    ///
    /// HashableObject automatically:
    ///   1. Calls ToBytes()
    ///   2. Passes the bytes to the selected hasher
    ///   3. Returns Hash32 / Hash64
    [HashAlgorithm(typeof(SimpleHasher), Endian.LittleEndian)]
    public class SensorData : HashableObject {

        /// <summary>
        /// Example fields that participate in hashing.
        /// </summary>
        public int Id;
        public float Value;

        /// <summary>
        /// Returns the deterministic byte representation of this object.
        ///
        /// Important:
        ///   - Fixed order
        ///   - Fixed endian
        ///   - Fixed size (8 bytes)
        /// </summary>
        public override Array<byte> ToBytes () {
            var b = new Array<byte>(8);

            // int → 4 bytes
            b.InsertRange(0, Id.ToBytes(Endian.LittleEndian));

            // float → 4 bytes
            b.InsertRange(4, Value.ToBytes(Endian.LittleEndian));

            return b;
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
