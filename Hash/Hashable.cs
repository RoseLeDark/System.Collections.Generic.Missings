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


using SystemEx.Utils;
using SystemEx.Collections.Generic;

namespace SystemEx.Hash {
	/// \addtogroup SystemEx.Hash
	/// @{
	/// <summary>
	/// Base class for objects that can be hashed using the SystemEx hashing subsystem.
	/// 
	/// A <c>Hashable</c> instance provides its binary representation through
	/// <see cref="ToBytes"/> and selects a hashing algorithm via
	/// <see cref="HashAlgorithmAttribute"/> applied on the concrete type.
	/// 
	/// The class supports both 32‑bit and 64‑bit hashing.  
	/// A per‑instance random seed is generated once and stored, ensuring
	/// stable hashing for the lifetime of the object while preventing
	/// predictable hash streams.
	/// </summary>
	[Obsolete("Hashable is deprecated. Use IHashable<T> with HashFactory instead. See Example: Examples/ExampleHasher.cs")]
    public abstract class Hashable {
        /// <summary>
        /// Per‑instance random seed used for hashing.  
        /// Generated once at construction time using <see cref="RandUtils.RandULong"/>.
        /// </summary>
        private ulong m_seed  = RandUtils.RandULong(1455644, ulong.MaxValue , Endian.LittleEndian);

        /// <summary>
        /// Converts the object into a byte array representation used by hashers.
        /// Implementations must return a deterministic and stable encoding.
        /// </summary>
        public abstract FixedVector<byte> ToBytes ();
        /// <summary>
        /// Gets the random seed assigned to this instance.
        /// </summary>
        public ulong Seed { get => m_seed;  }

        /// <summary>
        /// Computes a 32‑bit hash using the algorithm specified by
        /// <see cref="HashAlgorithmAttribute"/> on the concrete type.
        /// 
        /// If no attribute is present, <c>base.GetHashCode()</c> is used.
        /// The hasher instance is created transiently via reflection.
        /// </summary>
        public override int GetHashCode () {
            int _hash = 0;

            // Attribut vom konkreten Typ lesen (nicht typeof(Hashable ))
            var attr = (HashAlgorithmAttribute?)Attribute.GetCustomAttribute(this.GetType(), typeof(HashAlgorithmAttribute));
            if ( attr == null ) {
                _hash = base.GetHashCode();
            } else {
                // Bytes erzeugen
                FixedVector<byte> input = ToBytes();

                // Hasher transient erzeugen: zuerst versuchen, Konstruktor mit Endian, sonst parameterlos
                object? inst = null;
                try {
                    inst = Activator.CreateInstance(attr.HasherType, attr.Endian);
                } catch {
                    try {
                        inst = Activator.CreateInstance(attr.HasherType);
                    } catch {
                        inst = null;
                    }
                }

                if ( inst is IHash hasher ) {
                    var h = hasher.Compute(input, (uint)m_seed );
                    _hash = (int)h.Value;
                } else {
                    _hash = base.GetHashCode();
                }
            }

            return _hash;
        }
        /// <summary>
        /// Computes a 64‑bit hash using the algorithm specified by
        /// <see cref="HashAlgorithmAttribute"/> on the concrete type.
        /// 
        /// If no attribute is present, <c>base.GetHashCode()</c> is used.
        /// The hasher instance is created transiently via reflection.
        /// </summary>
        public virtual long GetHashCodeLong () {
            long _hash = 0;

            // Attribut vom konkreten Typ lesen (nicht typeof(Hashable ))
            var attr = (HashAlgorithmAttribute?)Attribute.GetCustomAttribute(this.GetType(), typeof(HashAlgorithmAttribute));
            if ( attr == null ) {
                _hash = base.GetHashCode();
            } else {
                // Bytes erzeugen
                FixedVector<byte> input = ToBytes();

                // Hasher transient erzeugen: zuerst versuchen, Konstruktor mit Endian, sonst parameterlos
                object? inst = null;
                try {
                    inst = Activator.CreateInstance(attr.HasherType, attr.Endian);
                } catch {
                    try {
                        inst = Activator.CreateInstance(attr.HasherType);
                    } catch {
                        inst = null;
                    }
                }

                if ( inst is IHash hasher ) {
                    var h = hasher.ComputeLong(input, m_seed );
                    _hash = (long)h.Value;
                } else {
                    _hash = base.GetHashCode();
                }
            }

            return _hash;
        }
    }
    /// @}
}
