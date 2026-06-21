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

using System.ComponentModel;
using System.Xml.Serialization;
using SystemEx.Collections.Generic;
using SystemEx.Collections.Generic.Interfaces;

namespace SystemEx.IO.Provider {


    /// <summary>
    /// A schema defines total size, header size, endianness and fixed field offsets.
    /// Implementations must be deterministic and contain no dynamic or computed layout.
    /// </summary>
    public interface IByteFormatSchema  {
        /// <summary>
        /// Gets the total number of bytes required to represent
        /// in its packed binary form.
        /// </summary>
        long TotalSize { get; }

        /// <summary>
        /// Gets the size of the header portion in bytes.
        /// </summary>
        long HeaderSize { get; }

        /// <summary>
        /// Gets the endianness used for all multi‑byte fields.
        /// </summary>
        Endian Endian { get; }

        /// <summary>
        /// Gets a read‑only mapping of field names to their byte offsets.
        /// Offsets are relative to the start of the packed structure.
        /// </summary>
        IReadOnlyMap<string, long> Offsets { get; } 
    }

    /// <summary>
    /// Provides deterministic binary serialization and deserialization for a type
    /// <typeparamref name="T"/> using a schema <typeparamref name="TSchema"/>.
    /// 
    /// Implementations must operate strictly on <c>Cache</c> and must not allocate
    /// temporary buffers other than the final output array.
    /// </summary>
    /// <typeparam name="T">
    /// The data type being serialized.
    /// </typeparam>
    /// <typeparam name="TSchema">
    /// The schema describing the binary layout of <typeparamref name="T"/>.
    /// </typeparam>
    public interface IByteSerialize<T, TSchema>
        where TSchema : IByteFormatSchema  {
        /// <summary>
        /// Packs the current instance into a new <c>byte[]</c> using the specified schema.
        /// </summary>
        /// <param name="schema">The schema describing the binary layout.</param>
        /// <returns>A newly allocated byte array containing the packed data.</returns>
        Cache Pack(TSchema schema);

        /// <summary>
        /// Reconstructs a new instance of <typeparamref name="T"/> from the given
        /// binary data using the specified schema.
        /// </summary>
        /// <param name="data">The raw binary data.</param>
        /// <param name="schema">The schema describing the binary layout.</param>
        /// <returns>A fully reconstructed instance of <typeparamref name="T"/>.</returns>
        T Unpack(Cache data, TSchema schema);

        /// <summary>
        /// Returns the schema instance associated with this type.
        /// Hidden from IntelliSense to avoid accidental misuse.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        TSchema GetSchema();
    }
}

