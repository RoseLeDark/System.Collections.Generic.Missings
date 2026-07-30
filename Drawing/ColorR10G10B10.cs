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
using SystemEx.Collections.Generic.Interfaces;
using SystemEx.IO.Provider;

namespace SystemEx.Drawing {
    /// \addtogroup color
    /// @{
    ///  <summary>
    /// A Scbema is used to serialize and deserialize a ColorR10G10B10A2 color to and from a binary representation.
    /// </summary>
    public struct ColorR10G10B10FormatSchema : IByteFormatSchema {

        /// <summary>
        /// The total number of bytes required to represent a ColorR10G10B10A2 color in its packed binary form.
        /// </summary>
        public long TotalSize => 32;
        /// <summary>
        /// The size of the header portion in bytes. For ColorR10G10B10A2, there is no header, so this is 0.
        /// </summary>
        public long HeaderSize => 0;
        /// <summary>
        /// The endianness used for all multi‑byte fields. This is specified when creating the schema.
        /// </summary>
        public Endian Endian { get; private set; }

        /// <summary>
        /// A read‑only mapping of field names to their byte offsets. For ColorR10G10B10A2, 
        /// there is only one field named "DEFAULT" at offset 0.
        /// </summary>
        public Map<string, long> Offsets { get; private set; }


        /// <summary>
        /// Initializes a new instance of the ColorR10G10B10FormatSchema class with the specified endianness.
        /// </summary>
        /// <param name="endian"></param>
        public ColorR10G10B10FormatSchema(Endian endian)  {
            Endian = endian;

            var x = new Pair<string, long>("DEFAULT", 0);

            // Define offsets
            Offsets = new Map<string, long>(new Pair<string, long>[] { x }, 0);
        }
    }
    /// <summary>
    /// The ColorR10G10B10FormatSchema class defines the schema for serializing and deserializing a 
    /// ColorR10G10B10A2 color to and from a binary representation.
    /// It specifies the total size, header size, endianness, and field offsets for the color data.
    /// </summary>
    public class ColorR10G10B10Serializer : ByteSeriablizeProvider {
        /// <summary>
        ///  Created a new ColorR10G10B10Serializer with the specified schema and endianness.   
        /// </summary>
        /// <param name="schema">The format schema for the color data.</param>
        /// <param name="endian">The endianness for the binary representation.</param>
        public ColorR10G10B10Serializer(IByteFormatSchema schema, Endian endian) : base(schema, endian) { }

        /// <summary>
        /// Gets the byte representation of the specified object for the given field name.
        /// /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="name">The name of the field to serialize.</param>
        /// <param name="endian">The endianness for the binary representation.</param>
        /// <returns>The byte array representing the serialized field, or null if not found.</returns>
        protected override FixedVector<byte> GetBytesForEntry(object obj, string name, Endian endian) {
            var objx = obj as ColorR10G10B10A2;
            if ( objx == null ) throw new InvalidCastException();

            Cache _ret = new Cache(4, CacheType.Both);

            if ( name == "DEFAULT") {
                
                int red = (int)(objx.R * 1023.0f + 0.5f); // 16
                int green = (int)(objx.G * 1023.0f + 0.5f);// 16
                int blue = (int)(objx.B * 1023.0f + 0.5f);// 16
                int alpha = (int)(objx.A * 3.0f + 0.5f);


                int packed =  (alpha << 30) | (blue << 20) | (green << 10) | red;
                byte[] raw = packed.ToBytes(endian);

                _ret.WriteRange(0, raw);

            }
            return _ret.ToArrayEx();
        }
        /// <summary>
        /// Gets the size of the specified entry in the cache.
        /// /// </summary>
        /// <param name="obj">The cache object.</param>
        /// <param name="name">The name of the entry.</param>
        /// <param name="endian">The endianness for the binary representation.</param>
        /// <returns>The size of the entry, or -1 if not found.</returns>

        protected override long GetEntrySize(Cache obj, string name, Endian endian) => ( name == "COLOR" || name == "DEFAULT" ) ? 4 : -1;
        /// <summary>
        /// Get the object from the given entries and endianness.
        /// </summary>
        /// <param name="entries">The map of entries.</param>
        /// <param name="endian">The endianness for the binary representation.</param>
        /// <returns>The created object, or null if not found.</returns>
        protected override ColorR10G10B10A2? CreateObjectFromEntrys(Map<string, byte[]> entries, Endian endian) {

            if ( entries.ContainsKey("DEFAULT") ) {
                byte[] raw = entries["DEFAULT"].Value!;
                uint packed = raw.ToUInt(endian);

                uint ri = packed & 0x3ff;
                uint gi = (packed >> 10) & 0x3ff;
                uint bi = (packed >> 20) & 0x3ff;
                uint ai = packed >> 30;


                return new ColorR10G10B10A2( ri / 1023.0f, gi / 1023.0f, bi / 1023.0f, ai /3.0f);
            }


            return null;
        }
    }

    /// <summary>
    /// Represents a 10‑bit per channel RGB color (R10G10B10A2),
    /// stored internally as normalized floating‑point values (0–1).
    /// </summary>
    public class ColorR10G10B10A2 : IEquatable<ColorR10G10B10A2>, IIsByteSeriablize  {
        private float m_r;
        private float m_g;
        private float m_b;
        private float m_a;

        /// <summary>
        /// Gets the inverse of the color component range.
        /// </summary>
        public const float COLORINV = 1f / 1023f;


        /// <summary>Gets or sets the red component.</summary>
        public float R { get => m_r; set => m_r = System.Math.Clamp(value, 0.0f, 1.0f); }

        /// <summary>Gets or sets the green component.</summary>
        public float G { get => m_g; set => m_g = System.Math.Clamp(value, 0.0f, 1.0f); }

        /// <summary>Gets or sets the blue component.</summary>
        public float B { get => m_b; set => m_b = System.Math.Clamp(value, 0.0f, 1.0f); }
        /// <summary>Gets or sets the alpha component.</summary>
        public float A { get => m_a; set => m_a = System.Math.Clamp(value, 0.0f, 1.0f); }

        /// <summary>
        /// Creates a new R10G10B10A2 color from normalized float values (0–1).
        /// </summary>
        /// <param name="r">The red component (0–1).</param>
        /// <param name="g">The green component (0–1).</param>
        /// <param name="b">The blue component (0–1).</param>
        public ColorR10G10B10A2(float r, float g, float b) : this( r, g, b,0f) {  }
        /// <summary>
        /// Initializes a new R10G10B10A2 color from normalized float values (0–1).
        /// </summary>
        public ColorR10G10B10A2(float r, float g, float b, float a) {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <summary>
        /// Initializes a new R10G10B10A2 color from 10‑bit UNORM components (0–1023).
        /// </summary>
        /// <param name="r10">The red component (0–1023).</param>
        /// <param name="g10">The green component (0–1023).</param>
        /// <param name="b10">The blue component (0–1023).</param>
        /// <param name="a10">The alpha component (0–3).</param>
        public ColorR10G10B10A2(ushort r10, ushort g10, ushort b10, ushort a10) {
            
            R = r10 / 1023.0f;
            G = g10 / 1023.0f;
            B = b10 / 1023.0f;
            A = a10 / 3.0f;
        }

        /// <summary>
        /// Determines whether this instance is equal to another R10G10B10A2 color.
        /// </summary>
        public bool Equals(ColorR10G10B10A2? other) {
            if ( other == null ) return false;

            return m_r.Equals(other.m_r) &&
                   m_g.Equals(other.m_g) &&
                   m_b.Equals(other.m_b) &&
                   m_a.Equals(other.m_a);
        }

        /// <summary>
        /// Determines whether this instance is equal to another object.
        /// </summary>
        public override bool Equals(object? obj) {
            if ( obj is ColorR10G10B10A2 c )
                return Equals(c);

            return false;
        }

        /// <summary>
        /// Returns a hash code based on the R, G, and B components.
        /// </summary>
        public override int GetHashCode() {
            return System.HashCode.Combine(m_r, m_g, m_b, m_a);
        }

        /// <summary>
        /// Returns a string representation of the R10G10B10A2 color.
        /// </summary>
        public override string ToString() {
            return string.Create(null, stackalloc char[256], $"[{m_r}, {m_g}, {m_b}, {m_a}]");
        }
    }
    /// @}
}
