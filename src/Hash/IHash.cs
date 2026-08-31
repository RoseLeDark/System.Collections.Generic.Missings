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

using System.Drawing;
using SystemEx.Collections.Generic;

namespace SystemEx.Hash {
	/// \addtogroup Hash
	/// @{

	/// <summary>
	/// Represents a 32‑bit hash value produced by a SystemEx hashing algorithm.
	/// 
	/// The struct is immutable and stores the raw 32‑bit result exactly as
	/// returned by the underlying hasher. No normalization or reinterpretation
	/// is performed.
	/// </summary>
	public readonly struct Hash32 {
        /// <summary>
        /// The raw 32‑bit hash value.
        /// </summary>
        public readonly uint Value;

        /// <summary>
        /// Creates a new 32‑bit hash wrapper.
        /// </summary>
        public Hash32 ( uint value ) => Value = value;

        /// <inheritdoc/>
        public override string ToString () {
            return Value.ToString("X4");
        }
    }

    /// <summary>
    /// Represents a 64‑bit hash value produced by a SystemEx hashing algorithm.
    /// 
    /// The struct is immutable and stores the raw 64‑bit result exactly as
    /// returned by the underlying hasher. No normalization or reinterpretation
    /// is performed.
    /// </summary>
    public readonly struct Hash64 {
        /// <summary>
        /// The raw 64‑bit hash value.
        /// </summary>
        public readonly ulong Value;

        /// <summary>
        /// Creates a new 64‑bit hash wrapper.
        /// </summary>
        public Hash64 ( ulong value ) => Value = value;

        /// <inheritdoc/>
        public override string ToString () {
            return Value.ToString("X4");
        }
    }

    /// <summary>
    /// Interface for SystemEx hashing algorithms.  
    /// 
    /// A hasher consumes a byte vector (<see cref="Vector{T}"/>) and produces
    /// either a 32‑bit or 64‑bit hash.  
    /// 
    /// Implementations must be endian‑aware and iterator‑driven, following the
    /// SystemEx data model. They may be instantiated dynamically via
    /// <see cref="HashAlgorithmAttribute"/>.
    /// </summary>
    public interface IHash {

        /// <summary>
        /// Computes a 32‑bit hash over the given byte vector using the specified seed
        /// and endian mode.
        /// </summary>
        Hash32 Compute ( FixedVector<byte> input, uint seed );

        /// <summary>
        /// Computes a 64‑bit hash over the given byte vector using the specified seed
        /// and endian mode.
        /// </summary>
        Hash64 ComputeLong ( FixedVector<byte> input, ulong seed );
    }
    

}
