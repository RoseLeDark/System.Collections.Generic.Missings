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




using SystemEx.Collections.Generic;

namespace SystemEx.Hash {
    /// <summary>
    /// Defines a contract for objects that can be converted into a raw byte
    /// representation suitable for hashing, serialization, or low‑level
    /// processing.
    ///
    /// <para>
    /// The interface is intentionally minimal: any type implementing
    /// <c>IHashable&lt;T&gt;</c> must provide a deterministic and stable
    /// <see cref="ToBytes"/> method. The returned byte sequence is used by
    /// SystemEx hashing algorithms and must remain consistent across platforms
    /// and runtime sessions.
    /// </para>
    ///
    /// <para>
    /// <b>Example:</b><br/>
    /// A type implementing both <c>IHashable&lt;T&gt;</c> and a hash attribute:
    /// </para>
    /// <code>
    /// [HashAlgorithm(typeof(Bernstein))]
    /// public class Foo : IHashable&lt;Foo&gt;
    /// {
    ///     public FixedVector&lt;byte&gt; ToBytes()
    ///     {
    ///         // Convert internal fields into a deterministic byte array.
    ///         return new FixedVector&lt;byte&gt;( ... );
    ///     }
    /// }
    /// </code>
    /// </summary>
    /// <typeparam name="T">
    /// The type that provides its own byte representation.
    /// </typeparam>
    public interface IHashable<T> {
        /// <summary>
        /// Converts this instance into a deterministic byte array.
        /// The returned data is used as input for hashing algorithms.
        /// </summary>
        FixedVector<byte> ToBytes ();
    }

    /// <summary>
    /// Provides factory methods for computing 32‑bit and 64‑bit hashes for
    /// objects implementing <see cref="IHashable{T}"/>.  
    /// 
    /// <para>
    /// HashFactory inspects the runtime type of the object for a
    /// <see cref="HashAlgorithmAttribute"/>. If present, the attribute specifies
    /// which hashing algorithm to use and which endianness to apply.
    /// </para>
    /// 
    /// <para>
    /// The factory attempts to create an instance of the specified hasher type
    /// using <see cref="System.Activator"/>. If the hasher supports a constructor
    /// accepting an <see cref="Endian"/> value, it is used; otherwise a parameterless
    /// constructor is attempted.
    /// </para>
    /// 
    /// <para>
    /// If no hasher can be created, a default hash value is returned.
    /// </para>
    /// 
    /// <para>
    /// <b>Example usage:</b>
    /// </para>
    /// <code>
    /// [HashAlgorithm(typeof(Bernstein), Endian = Endian.System)]
    /// public class Foo : IHashable&lt;Foo&gt;
    /// {
    ///     public FixedVector&lt;byte&gt; ToBytes()
    ///     {
    ///         return new FixedVector&lt;byte&gt;( ... );
    ///     }
    /// }
    ///
    /// Foo f = new Foo();
    /// Hash32 h = HashFactory.Hash32(f);
    /// </code>
    /// </summary>
    public static class HashFactory {
        /// <summary>
        /// Computes a 32‑bit hash for the specified object using the hashing
        /// algorithm declared via <see cref="HashAlgorithmAttribute"/> on the
        /// object's type.  
        /// </summary>
        /// <typeparam name="T">
        /// A type implementing <see cref="IHashable{T}"/>.
        /// </typeparam>
        /// <param name="obj">
        /// The object to hash.
        /// </param>
        /// <param name="seed">
        /// Optional seed value passed to the hashing algorithm.
        /// </param>
        /// <returns>
        /// A computed <see cref="Hash32"/> value.
        /// </returns>
        public static Hash32 Hash32<T>(T obj, uint seed = 45544544U) where T : IHashable<T> {
            Hash32? _hash = null;
            object? inst = null;
            Endian endian = Endian.System;

            // Attribut vom konkreten Typ lesen (nicht typeof(Hashable ))
            var attr = (HashAlgorithmAttribute?)Attribute.GetCustomAttribute(obj.GetType(), typeof(HashAlgorithmAttribute));
            if ( attr != null ) {
                endian = attr.Endian;

                try {
                    inst = Activator.CreateInstance(attr.HasherType, endian);
                } catch {
                    try {
                        inst = Activator.CreateInstance(attr.HasherType);
                    } catch {
                        inst = null;
                    }
                }
            }


            if ( inst is IHash hasher ) {

                FixedVector<byte> input = obj.ToBytes();
                _hash = hasher.Compute(input, seed);

            } else {
                _hash = new Hash32(0);
            }

            return _hash.Value;
        }

        /// <summary>
        /// Computes a 64‑bit hash for the specified object using the hashing
        /// algorithm declared via <see cref="HashAlgorithmAttribute"/> on the
        /// object's type.  
        /// </summary>
        /// <typeparam name="T">
        /// A type implementing <see cref="IHashable{T}"/>.
        /// </typeparam>
        /// <param name="obj">
        /// The object to hash.
        /// </param>
        /// <param name="seed">
        /// Optional seed value passed to the hashing algorithm.
        /// </param>
        /// <returns>
        /// A computed <see cref="Hash64"/> value.
        /// </returns>
        public static Hash64 Hash64<T> ( T obj, uint seed ) where T : IHashable<T> {
            Hash64? _hash = null;
            object? inst = null;
            Endian endian = Endian.System;

            // Attribut vom konkreten Typ lesen (nicht typeof(Hashable ))
            var attr = (HashAlgorithmAttribute?)Attribute.GetCustomAttribute(obj.GetType(), typeof(HashAlgorithmAttribute));
            if ( attr != null ) {
                endian = attr.Endian;

                try {
                    inst = Activator.CreateInstance(attr.HasherType);
                } catch {
                    try {
                        inst = Activator.CreateInstance(attr.HasherType);
                    } catch {
                        inst = null;
                    }
                }
            }


            if ( inst is IHash hasher ) {

                FixedVector<byte> input = obj.ToBytes();
                _hash = hasher.ComputeLong(input, seed );

            } else {
                _hash = new Hash64(0);
            }

            return _hash.Value;
        }
    }
}
