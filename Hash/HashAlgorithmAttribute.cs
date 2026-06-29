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

namespace SystemEx.Hash {
    /// \addtogroup hash
    /// @{
    /// <summary>
    /// Declares which hashing algorithm a <c>Hashable</c> type should use.
    /// 
    /// The attribute binds a concrete hasher implementation (<see cref="IHash"/>)
    /// to a class or struct and specifies the byte‑order (<see cref="Endian"/>)
    /// used when interpreting the binary representation returned by
    /// <see cref="Hashable.ToBytes"/>.
    /// 
    /// The SystemEx hashing subsystem reads this attribute at runtime to
    /// instantiate the correct hasher and compute 32‑bit or 64‑bit hashes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class HashAlgorithmAttribute : Attribute {

        /// <summary>
        /// The hasher implementation type.  
        /// Must implement <see cref="IHash"/> and provide either a constructor
        /// accepting <see cref="Endian"/> or a parameterless constructor.
        /// </summary>
        public Type HasherType { get; }

        /// <summary>
        /// The endian mode used when assembling hash blocks from the byte array.
        /// </summary>
        public Endian Endian { get; }

        /// <summary>
        /// Creates a new hash algorithm descriptor for a <c>Hashable</c> type.
        /// </summary>
        /// <param name="hasherType">Concrete hasher type implementing <see cref="IHash"/>.</param>
        /// <param name="endian">Byte‑order used by the hasher.</param>
        public HashAlgorithmAttribute ( Type hasherType, Endian endian ) {
            HasherType = hasherType;
            Endian = endian;
        }
    }
    /// @}

}
