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
using Microsoft.VisualBasic;
using System.Reflection;
using SystemEx.Collections.Generic;
using SystemEx.IO.Provider;
using SystemEx.SystemEx.Drawing;

namespace SystemEx {
    /// <summary>
    /// Specifies the byte order used when converting values to and from raw byte
    /// sequences.
    /// </summary>
    public enum Endian {
        /// <summary>
        /// Least significant byte first.
        /// </summary>
        LittleEndian,

        /// <summary>
        /// Most significant byte first.
        /// </summary>
        BigEndian
    }

    /// <summary>
    /// 
    /// </summary>
    public static class Conversion {
        /// <summary>
        /// Provides low‑level conversion utilities for primitive numeric types,
        /// unmanaged structs, and arrays.  
        /// Supports endian‑aware serialization, deserialization, memory copying,
        /// and size parsing.
        /// </summary>
        public static int ToBoundary(this uint number, uint boundary) {
            if ( boundary == 0 ) return (int)number;
            uint div = number / boundary;
            uint mod = number % boundary;
            return (int)(mod == 0 ? div * boundary : (div + 1) * boundary);
        }

        #region BYTE

        /// <summary>
        /// Converts a <see cref="byte"/> value into a one‑byte array.
        /// Endianness is ignored because the value is a single byte.
        /// </summary>
        public unsafe static byte[] ToBytes(this byte value, Endian endian) {
            byte[] bytes = new byte[1];
            bytes[0] = (byte)value;
            return bytes;
        }
        /// <summary>
        /// Converts a one‑byte array into a <see cref="byte"/> value.
        /// </summary>
        public static byte ToByte(this byte[] bytes, Endian endian) {
            if ( bytes.Length < 1 ) throw new ArgumentException("byte requires exactly 1 byte");

            return (byte)bytes[0];
        }
        #endregion

        #region INT

        /// <summary>
        /// Converts a <see cref="uint"/> value into a 4‑byte array using the specified endianness.
        /// </summary>
        public unsafe static byte[] ToBytes(this uint value, Endian endian) {
            byte[] bytes = new byte[4];

            fixed ( byte* b = bytes )
                *(uint*)b = value;

            if ( endian == Endian.BigEndian ) {
                // reverse in-place
                byte tmp;
                tmp = bytes[0]; bytes[0] = bytes[3]; bytes[3] = tmp;
                tmp = bytes[1]; bytes[1] = bytes[2]; bytes[2] = tmp;
            }

            return bytes;
        }

        /// <summary>
        /// Converts an <see cref="int"/> value into a 4‑byte array using the specified endianness.
        /// </summary>
        public unsafe static byte[] ToBytes(this int value, Endian endian) {
            byte[] bytes = new byte[4];

            fixed ( byte* b = bytes )
                *(int*)b = value;

            if ( endian == Endian.BigEndian ) {
                byte tmp;
                tmp = bytes[0]; bytes[0] = bytes[3]; bytes[3] = tmp;
                tmp = bytes[1]; bytes[1] = bytes[2]; bytes[2] = tmp;
            }

            return bytes;
        }

        /// <summary>
        /// Converts a 4‑byte array into an <see cref="int"/> using the specified endianness.
        /// </summary>
        public unsafe static int ToInt(this byte[] bytes, Endian endian) {
            if ( bytes.Length < 4 ) throw new ArgumentException("uint requires exactly 4 bytes");

            if ( endian == Endian.BigEndian ) {
                byte tmp;
                tmp = bytes[0]; bytes[0] = bytes[3]; bytes[3] = tmp;
                tmp = bytes[1]; bytes[1] = bytes[2]; bytes[2] = tmp;
            }

            fixed ( byte* b = bytes )
                return *(int*)b;
        }
        /// <summary>
        /// Converts a 4‑byte array into a <see cref="uint"/> using the specified endianness.
        /// </summary>
        public unsafe static uint ToUInt(this byte[] bytes, Endian endian) {
            if ( bytes.Length < 4 ) throw new ArgumentException("uint requires exactly 4 bytes");

            if ( endian == Endian.BigEndian ) {
                byte tmp;
                tmp = bytes[0]; bytes[0] = bytes[3]; bytes[3] = tmp;
                tmp = bytes[1]; bytes[1] = bytes[2]; bytes[2] = tmp;
            }

            fixed ( byte* b = bytes )
                return *(uint*)b;
        }

        #endregion

        #region SHORT
        /// <summary>
        /// Converts a <see cref="short"/> value into a 2‑byte array using the specified endianness.
        /// </summary>
        public unsafe static byte[] ToBytes(this short value, Endian endian) {
            byte[] bytes = new byte[2];

            fixed ( byte* b = bytes )
                *(short*)b = value;

            if ( endian == Endian.BigEndian ) {
                byte tmp = bytes[0];
                bytes[0] = bytes[1];
                bytes[1] = tmp;
            }

            return bytes;
        }
        /// <summary>
        /// Converts a <see cref="ushort"/> value into a 2‑byte array using the specified endianness.
        /// </summary>
        public unsafe static byte[] ToBytes(this ushort value, Endian endian) {
            byte[] bytes = new byte[2];

            fixed ( byte* b = bytes )
                *(ushort*)b = value;

            if ( endian == Endian.BigEndian ) {
                byte tmp = bytes[0];
                bytes[0] = bytes[1];
                bytes[1] = tmp;
            }

            return bytes;
        }
        /// <summary>
        /// Converts a 2‑byte array into a <see cref="ushort"/> using the specified endianness.
        /// </summary>
        public unsafe static ushort ToUShort(this byte[] bytes, Endian endian) {
            if ( bytes.Length < 2) throw new ArgumentException("short requires exactly 2 bytes");

            if ( endian == Endian.BigEndian ) {
                byte tmp = bytes[0];
                bytes[0] = bytes[1];
                bytes[1] = tmp;
            }

            fixed ( byte* b = bytes )
                return *(ushort*)b;
        }

        /// <summary>
        /// Converts a 2‑byte array into a <see cref="short"/> using the specified endianness.
        /// </summary>
        public unsafe static short ToShort(this byte[] bytes, Endian endian) {
            if ( bytes.Length < 2 ) throw new ArgumentException("short requires exactly 2 bytes");

            if ( endian == Endian.BigEndian ) {
                byte tmp = bytes[0];
                bytes[0] = bytes[1];
                bytes[1] = tmp;
            }

            fixed ( byte* b = bytes )
                return *(short*)b;
        }

        #endregion

        #region STRUCT
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static byte[] ToBytes<T>(this T value, ByteSeriablizeProvider provider) 
            where T : IIsByteSeriablize  {

            return provider.ToBytes<T>(value).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static T FromBytes<T>(this byte[] bytes, ByteSeriablizeProvider provider) 
            where T : IIsByteSeriablize {
           
            return provider.FromBytes<T>(new Cache(bytes, CacheType.Both));
        }

        /// <summary>
        /// Converts an unmanaged struct into a byte array using the specified endianness.
        /// </summary>
        public static unsafe byte[] ToBytes<T>(this T value, Endian endian) where T : unmanaged {
            int size = sizeof(T);
            byte[] bytes = new byte[size];

            fixed ( byte* b = bytes ) {
                *(T*)b = value;
            }

            if ( endian == Endian.BigEndian )
                Array.Reverse(bytes);

            return bytes;
        }
        /// <summary>
        /// Converts an array of unmanaged structs into a contiguous byte array.
        /// </summary>
        public static unsafe byte[] ToBytes<T>(this T[] array) where T : unmanaged {
            int size = sizeof(T) * array.Length;
            byte[] bytes = new byte[size];

            fixed ( T* src = array )
            fixed ( byte* dst = bytes ) {
                Buffer.MemoryCopy(src, dst, size, size);
            }

            return bytes;
        }
        /// <summary>
        /// Converts a byte array into an unmanaged struct of type <typeparamref name="T"/>.
        /// </summary>
        public static unsafe T FromBytes<T>(byte[] bytes, Endian endian) where T : unmanaged {
            T value = default;

            int size = sizeof(T);
            if ( bytes.Length < size )  throw new ArgumentException($"Byte array too small for type {typeof(T).Name}");

            if ( endian == Endian.BigEndian ) {
                Array.Reverse(bytes);
            }

            fixed ( byte* b = bytes ) {
                value = *(T*)b;
            }

            return value;
        }
        /// <summary>
        /// Converts a byte array into an array of unmanaged structs of type <typeparamref name="T"/>.
        /// </summary>
        public static unsafe T[] FromBytesArray<T>(byte[] bytes) where T : unmanaged {
            int size = sizeof(T);
            int count = bytes.Length / size;

            T[] array = new T[count];

            fixed ( byte* src = bytes )
            fixed ( T* dst = array ) {
                Buffer.MemoryCopy(src, dst, bytes.Length, bytes.Length);
            }

            return array;
        }
        #endregion

        #region FLOAT

        /// <summary>
        /// Converts a <see cref="float"/> value into a 4‑byte array using the specified endianness.
        /// </summary>
        public unsafe static byte[] ToBytes(this float value, Endian endian) {
            byte[] bytes = new byte[4];

            fixed ( byte* b = bytes )
                *(float*)b = value;

            if ( endian == Endian.BigEndian ) {
                byte tmp;
                tmp = bytes[0]; bytes[0] = bytes[3]; bytes[3] = tmp;
                tmp = bytes[1]; bytes[1] = bytes[2]; bytes[2] = tmp;
            }

            return bytes;
        }

        /// <summary>
        /// Converts a 4‑byte array into a <see cref="float"/> using the specified endianness.
        /// </summary>
        public unsafe static float ToFloat(this byte[] bytes, Endian endian) {
            if ( bytes.Length < 4 ) throw new ArgumentException("Float requires exactly 4 bytes");

            if ( endian == Endian.BigEndian ) {
                byte tmp;
                tmp = bytes[0]; bytes[0] = bytes[3]; bytes[3] = tmp;
                tmp = bytes[1]; bytes[1] = bytes[2]; bytes[2] = tmp;
            }

            fixed ( byte* b = bytes )
                return *(float*)b;
        }
        #endregion

        #region DOUBLE

        /// <summary>
        /// Converts a <see cref="double"/> value into an 8‑byte array using the specified endianness.
        /// </summary>
        public unsafe static byte[] ToBytes(this double value, Endian endian) {
            byte[] bytes = new byte[8];

            fixed ( byte* b = bytes )
                *(double*)b = value;

            if ( endian == Endian.BigEndian ) {
                // reverse 8 bytes
                byte tmp;
                tmp = bytes[0]; bytes[0] = bytes[7]; bytes[7] = tmp;
                tmp = bytes[1]; bytes[1] = bytes[6]; bytes[6] = tmp;
                tmp = bytes[2]; bytes[2] = bytes[5]; bytes[5] = tmp;
                tmp = bytes[3]; bytes[3] = bytes[4]; bytes[4] = tmp;
            }

            return bytes;
        }

        /// <summary>
        /// Converts an 8‑byte array into a <see cref="double"/> using the specified endianness.
        /// </summary>
        public unsafe static double ToDouble(this byte[] bytes, Endian endian) {
            if ( bytes.Length < 8 ) throw new ArgumentException("Double requires at least 8 bytes");

            if ( endian == Endian.BigEndian ) {
                byte tmp;
                tmp = bytes[0]; bytes[0] = bytes[7]; bytes[7] = tmp;
                tmp = bytes[1]; bytes[1] = bytes[6]; bytes[6] = tmp;
                tmp = bytes[2]; bytes[2] = bytes[5]; bytes[5] = tmp;
                tmp = bytes[3]; bytes[3] = bytes[4]; bytes[4] = tmp;
            }

            fixed ( byte* b = bytes )
                return *(double*)b;
        }

        #endregion



        #region LONG

        /// <summary>
        /// Converts a <see cref="long"/> value into an 8‑byte array using the specified endianness.
        /// </summary>
        public unsafe static byte[] ToBytes(this long value, Endian endian) {
            byte[] bytes = new byte[8];

            fixed ( byte* b = bytes )
                *(long*)b = value;

            if ( endian == Endian.BigEndian ) {
                // reverse 8 bytes
                byte tmp;
                tmp = bytes[0]; bytes[0] = bytes[7]; bytes[7] = tmp;
                tmp = bytes[1]; bytes[1] = bytes[6]; bytes[6] = tmp;
                tmp = bytes[2]; bytes[2] = bytes[5]; bytes[5] = tmp;
                tmp = bytes[3]; bytes[3] = bytes[4]; bytes[4] = tmp;
            }

            return bytes;
        }
        /// <summary>
        /// Converts an 8‑byte array into a <see cref="long"/> using the specified endianness.
        /// </summary>
        public unsafe static long ToLong(this byte[] bytes, Endian endian) {
            if ( bytes.Length < 8 ) throw new ArgumentException("long requires at least 8 bytes");

            if ( endian == Endian.BigEndian ) {
                byte tmp;
                tmp = bytes[0]; bytes[0] = bytes[7]; bytes[7] = tmp;
                tmp = bytes[1]; bytes[1] = bytes[6]; bytes[6] = tmp;
                tmp = bytes[2]; bytes[2] = bytes[5]; bytes[5] = tmp;
                tmp = bytes[3]; bytes[3] = bytes[4]; bytes[4] = tmp;
            }

            fixed ( byte* b = bytes )
                return *(long*)b;
        }
        /// <summary>
        /// Converts a <see cref="ulong"/> value into an 8‑byte array using the specified endianness.
        /// </summary>
        public unsafe static byte[] ToBytes(this ulong value, Endian endian) {
            byte[] bytes = new byte[8];

            fixed ( byte* b = bytes )
                *(ulong*)b = value;

            if ( endian == Endian.BigEndian ) {
                // reverse 8 bytes
                byte tmp;
                tmp = bytes[0]; bytes[0] = bytes[7]; bytes[7] = tmp;
                tmp = bytes[1]; bytes[1] = bytes[6]; bytes[6] = tmp;
                tmp = bytes[2]; bytes[2] = bytes[5]; bytes[5] = tmp;
                tmp = bytes[3]; bytes[3] = bytes[4]; bytes[4] = tmp;
            }

            return bytes;
        }
        /// <summary>
        /// Converts an 8‑byte array into a <see cref="ulong"/> using the specified endianness.
        /// </summary>
        public unsafe static ulong ToULong(this byte[] bytes, Endian endian) {
            if ( bytes.Length < 8 ) throw new ArgumentException("ulong requires at least 8 bytes");

            if ( endian == Endian.BigEndian ) {
                byte tmp;
                tmp = bytes[0]; bytes[0] = bytes[7]; bytes[7] = tmp;
                tmp = bytes[1]; bytes[1] = bytes[6]; bytes[6] = tmp;
                tmp = bytes[2]; bytes[2] = bytes[5]; bytes[5] = tmp;
                tmp = bytes[3]; bytes[3] = bytes[4]; bytes[4] = tmp;
            }

            fixed ( byte* b = bytes )
                return *(ulong*)b;
        }
        #endregion



        /// <summary>
        /// Parses a human‑readable size string such as "4K", "16M", "1G" or "512"
        /// into a byte count.  
        /// Defaults to 512 bytes if parsing fails.
        /// </summary>
        public static uint SizeCalc(string value) {
            string str = value.ToUpper(System.Globalization.CultureInfo.CurrentCulture).Trim();
            uint multiplier = 1;

            if ( str.EndsWith("G", StringComparison.OrdinalIgnoreCase) ) {
                multiplier = 1024u * 1024u * 1024u;
                str = str[..^1];
            } else if ( str.EndsWith("M", StringComparison.OrdinalIgnoreCase) ) {
                multiplier = 1024u * 1024u;
                str = str[..^1];
            } else if ( str.EndsWith("K", StringComparison.OrdinalIgnoreCase) ) {
                multiplier = 1024u;
                str = str[..^1];
            } else if ( str.EndsWith("B", StringComparison.OrdinalIgnoreCase) ) {
                multiplier = 1;
                str = str[..^1];
            }

            return uint.TryParse(str, out uint size)
                ? size * multiplier
                : 512 * multiplier;
        }
        /// <summary>
        /// Compares two byte arrays for exact equality.
        /// </summary>
        public static bool EqualArray(this byte[] a, byte[] b) {
            if ( a.Length != b.Length )
                return false;

            for ( int i = 0; i < a.Length; i++ ) {
                if ( a[i] != b[i] )
                    return false;
            }
            return true;
        }

    }

}
