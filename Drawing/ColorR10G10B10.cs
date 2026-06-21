using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using SystemEx.Collections.Generic;
using SystemEx.Collections.Generic.Interfaces;
using SystemEx.Drawing;
using SystemEx.IO.Provider;

namespace SystemEx.SystemEx.Drawing {

    /// <summary>
    /// 
    /// </summary>
    public class ColorR10G10B10FormatSchema : IByteFormatSchema {
        /// <summary>
        /// /
        /// </summary>
        public long TotalSize => 32;
        /// <summary>
        /// /
        /// </summary>
        public long HeaderSize => 0;
        /// <summary>
        /// /
        /// </summary>
        public Endian Endian { get; private set; }

        /// <summary>
        /// /
        /// </summary>
        public IReadOnlyMap<string, long> Offsets { get; private set; }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="endian"></param>
        public ColorR10G10B10FormatSchema(Endian endian)  {
            Endian = endian;

            var x = new Pair<string, long>("DEFAULT", 0);

            // Define offsets
            Offsets = new Map<string, long>(new Pair<string, long>[] { x });
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ColorR10G10B10Serializer : ByteSeriablizeProvider {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="endian"></param>
        public ColorR10G10B10Serializer(IByteFormatSchema schema, Endian endian) : base(schema, endian) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <param name="endian"></param>
        /// <returns></returns>
        protected override Array<byte>? GetBytesForEntry(object obj, string name, Endian endian) {
            var objx = obj as ColorR10G10B10A2;
            if ( objx == null ) return null;

            if ( name == "DEFAULT") {
                Cache _ret = new Cache(4, CacheType.Both);
                
                int red = (int)(objx.R * 1023.0f + 0.5f); // 16
                int green = (int)(objx.G * 1023.0f + 0.5f);// 16
                int blue = (int)(objx.B * 1023.0f + 0.5f);// 16
                int alpha = (int)(objx.A * 3.0f + 0.5f);


                int packed =  (alpha << 30) | (blue << 20) | (green << 10) | red;
                byte[] raw = packed.ToBytes(endian);

                _ret.WriteRange(0, raw);

                return _ret.ToArrayEx();
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <param name="endian"></param>
        /// <returns></returns>
        protected override long GetEntrySize(Cache obj, string name, Endian endian) => ( name == "COLOR" || name == "DEFAULT" ) ? 4 : -1;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entries"></param>
        /// <param name="endian"></param>
        /// <returns></returns>
        protected override ColorR10G10B10A2? CreateObjectFromEntrys(Map<string, byte[]> entries, Endian endian) {

            if ( entries.ContainsKey("DEFAULT") ) {
                byte[] raw = entries["DEFAULT"];
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
        /// 
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
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
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

}
