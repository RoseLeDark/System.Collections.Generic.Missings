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
using System.Numerics;
using SystemEx.Collections.Generic;
using SystemEx.IO.Provider;
using SystemEx.Numeric;

namespace SystemEx {
	// addtogroup SystemEx
	/// @{

	/// <summary>
	/// Specifies the byte order used when converting values to and from raw byte
	/// sequences.
	/// 
	/// <para>
	/// Endianness determines how multi‑byte numeric values are represented in
	/// memory and serialized into byte arrays. All conversion methods in
	/// <see cref="SystemEx.Utils.Conversion"/> support explicit endianness to
	/// ensure deterministic binary layouts across platforms.
	/// </para>
	/// 
	/// <para>
	/// The enumeration provides three modes:
	/// <list type="bullet">
	/// <item>
	/// <description>
	/// <see cref="Endian.LittleEndian"/> — least significant byte first.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// <see cref="Endian.BigEndian"/> — most significant byte first.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// <see cref="Endian.System"/> — use the native endianness of the current
	/// machine as detected by <see cref="SystemEx.Utils.Conversion.GetEndian"/>.
	/// </description>
	/// </item>
	/// </list>
	/// </para>
	/// 
	/// <para>
	/// <b>Note:</b> When <see cref="Endian.System"/> is specified, conversion
	/// methods do not perform byte‑swapping. They serialize values exactly as the
	/// CPU represents them in memory. This is typically little‑endian on modern
	/// x86/x64 systems.
	/// </para>
	/// </summary>
	public enum Endian {

		/// <summary>
		/// Least significant byte first (little‑endian). This is the native format
		/// on most modern architectures including x86 and x64.
		/// </summary>
		LittleEndian,

		/// <summary>
		/// Most significant byte first (big‑endian). Common in network protocols
		/// and some older CPU architectures.
		/// </summary>
		BigEndian,

		/// <summary>
		/// Uses the system’s native endianness as detected by
		/// <see cref="SystemEx.Utils.Conversion.GetEndian"/>. This mode ensures
		/// that values are serialized exactly as the CPU stores them in memory,
		/// without performing any byte‑swapping.
		/// </summary>
		System
	}


	/// <summary>
	/// Provides low‑level numeric and binary conversion utilities, including
	/// endian‑aware serialization, deserialization, boundary alignment, and
	/// human‑readable size parsing.
	/// 
	/// <para>
	/// The <see cref="Conversion"/> class forms the core of SystemEx's primitive
	/// transformation layer. It exposes extension methods for converting all
	/// built‑in numeric types (<see cref="byte"/>, <see cref="short"/>,
	/// <see cref="int"/>, <see cref="long"/>, <see cref="float"/>, <see cref="Half16"/>,
	/// <see cref="double"/>, <see cref="uint"/>, <see cref="ulong"/>, <see cref="Half16b"/>,
	/// <see cref="ushort"/>) to and from byte arrays using explicit endianness.
	/// </para>
	/// 
	/// <para>
	/// All <c>ToBytes</c> methods use unsafe pointer assignment to guarantee
	/// deterministic IEEE‑754 and two’s‑complement representation without
	/// intermediate boxing or per‑byte arithmetic. Endianness is applied by
	/// reversing the byte sequence when required.
	/// </para>
	/// 
	/// <para>
	/// The <c>FromBytes</c> methods perform the inverse operation and reconstruct
	/// numeric values from byte arrays or <see cref="SystemEx.Collections.Generic.Vector{T}"/>
	/// instances. Overloads accepting an offset allow extracting values from
	/// structured binary buffers.
	/// </para>
	/// 
	/// <para>
	/// Additional helpers include:
	/// <list type="bullet">
	/// <item>
	/// <description>
	/// <see cref="GetEndian"/> — detects the system’s native endianness.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// <see cref="ToBoundary{T}(T, T)"/> — aligns a numeric value upward to the
	/// nearest multiple of a boundary using generic math.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// <see cref="SizeCalc(string)"/> — parses human‑readable size strings such as
	/// <c>"4Ki"</c>, <c>"128M"</c>, <c>"2Gi"</c>, <c>"512B"</c>, converting them
	/// into byte counts using binary (Ki/Mi/Gi) or decimal (K/M/G) multipliers.
	/// </description>
	/// </item>
	/// </list>
	/// </para>
	/// 
	/// <para>
	/// <b>Warning:</b> Many methods in this class use unsafe code and operate
	/// directly on raw memory. They assume valid input sizes and do not perform
	/// defensive normalization. Incorrect usage may lead to corrupted buffers or
	/// undefined behavior.
	/// </para>
	/// </summary>
	public static class Conversion {
        /// <summary>
        /// Provides low‑level conversion utilities for primitive numeric types,
        /// unmanaged structs, and arrays.  
        /// Supports endian‑aware serialization, deserialization, memory copying,
        /// and size parsing.
        /// </summary>
        public static T ToBoundary<T>(this T number, T boundary) where T : INumber<T> {
            if ( boundary == T.Zero ) return (T)number;
            T div = number / boundary;
            T mod = number % boundary;
            return (T)(mod == T.Zero ? div * boundary : (div + T.One) * boundary);
        }
        

        /// <summary>
        /// Get The System Byte Order
        /// </summary>
        /// <returns>The byte order of this Maschine</returns>
        public static Endian GetEndian() {
            if ( m_isLittleEndianSystem == triple.Nin ) InitEndian();
            return m_isLittleEndianSystem == triple.True ? Endian.LittleEndian : Endian.BigEndian;

        }

        private static triple m_isLittleEndianSystem = triple.Nin;

        private unsafe static void InitEndian () {
    
            int test = 0x01020304;
            byte* p = (byte*)&test;

            // Wenn das erste Byte 0x04 ist → Little Endian
            // Wenn das erste Byte 0x01 ist → Big Endian
            m_isLittleEndianSystem = (p[0] == 0x04) ? triple.True : triple.False;
          
        }

        /// <summary>
        /// Creates a <see cref="FlexSpan{T}"/> over the specified byte array using
        /// a starting offset and the given span mode. The span covers the array
        /// from <paramref name="start"/> to the end of the underlying buffer,
        /// interpreted according to the selected <see cref="FlexSpanMode"/>.
        /// 
        /// This overload is typically used when only a starting position is required
        /// and the span length is implicitly determined by the array boundary.
        /// </summary>
        /// <param name="value">Reference to the underlying byte array.</param>
        /// <param name="start">The starting index of the span.</param>
        /// <param name="mode">
        /// Determines how indexing is interpreted:
        /// <see cref="FlexSpanMode.System"/> (forward),
        /// <see cref="FlexSpanMode.Reverse"/> (reverse),
        /// <see cref="FlexSpanMode.Ring"/> (circular).
        /// </param>
        /// <returns>A new <see cref="FlexSpan{byte}"/> instance.</returns>
        public static FlexSpan<byte> AsFlexSpan ( ref byte[] value, long start, FlexSpanMode mode ) {
            return new FlexSpan<byte>(ref value, start, mode);
        }


        /// <summary>
        /// Creates a <see cref="FlexSpan{T}"/> over the specified byte array using
        /// an explicit start and end range together with the selected span mode.
        /// The span covers the region from <paramref name="start"/> to
        /// <paramref name="end"/> inclusively, interpreted according to the chosen
        /// <see cref="FlexSpanMode"/>.
        /// 
        /// This overload is used when a precise subrange of the array is required.
        /// </summary>
        /// <param name="value">Reference to the underlying byte array.</param>
        /// <param name="start">The starting index of the span.</param>
        /// <param name="end">The ending index of the span.</param>
        /// <param name="mode">
        /// Determines how indexing is interpreted:
        /// <see cref="FlexSpanMode.System"/> (forward),
        /// <see cref="FlexSpanMode.Reverse"/> (reverse),
        /// <see cref="FlexSpanMode.Ring"/> (circular).
        /// </param>
        /// <returns>A new <see cref="FlexSpan{byte}"/> instance.</returns>
        public static FlexSpan<byte> AsFlexSpan ( ref byte[] value, long start, long end, FlexSpanMode mode ) {
            return new FlexSpan<byte>(ref value, start, end, mode);
        }


        #region BYTE

        /// <summary>
        /// Converts a <see cref="byte"/> value into a one‑byte array.
        /// Endianness is ignored because the value is a single byte.
        /// </summary>
        public unsafe static byte[] ToBytes(this byte value, Endian endian = Endian.System) {
            if ( m_isLittleEndianSystem == triple.Nin ) InitEndian();

            byte[] bytes = new byte[1];
            bytes[0] = (byte)value;
            return bytes;
        }
        /// <summary>
        /// Converts a one‑byte array into a <see cref="byte"/> value.
        /// </summary>
        public static byte ToByte(this byte[] bytes, Endian endian = Endian.System) {
            if ( m_isLittleEndianSystem == triple.Nin ) InitEndian();

            if ( bytes.Length < 1 ) throw new ArgumentException("byte requires exactly 1 byte");

            return (byte)bytes[0];
        }
        /// <summary>
        /// Converts a one‑byte array into a <see cref="byte"/> value.
        /// </summary>
        public static byte ToByte ( this byte[] bytes, long offsets, Endian endian = Endian.System ) {
            if ( offsets < 0 )
                throw new ArgumentOutOfRangeException(nameof(offsets));

            if ( offsets  > bytes.Length )
                throw new ArgumentException("uint requires exactly 1 bytes at the given offset");

            // Die bestehende Methode aufrufen
            return bytes[offsets];
        }
        #endregion

        #region INT

        

        /// <summary>
        /// Splits a 32-bit unsigned integer into its high and low 16-bit components.
        /// </summary>
        /// <param name="value">
        /// The 32-bit unsigned integer to split.
        /// </param>
        /// <returns>
        /// A <see cref="Pair{T1,T2}"/> where:
        /// <list type="bullet">
        /// <item><description><b>Item1</b> is the high 16-bit word (unsigned).</description></item>
        /// <item><description><b>Item2</b> is the low 16-bit word (signed).</description></item>
        /// </list>
        /// </returns>
        public static Pair<ushort, short> ToShort(this uint value) {
            byte [] arr = value.ToBytes(Endian.LittleEndian);

            ushort h = arr.ToUShort(2);
            short l = arr.ToShort(0);

            return new Pair<ushort, short>(h, l);
        }

        /// <summary>
        /// Splits a 32-bit signed integer into its high and low 16-bit components.
        /// </summary>
        /// <param name="value">
        /// The 32-bit signed integer to split.
        /// </param>
        /// <returns>
        /// A <see cref="Pair{T1,T2}"/> where:
        /// <list type="bullet">
        /// <item><description><b>Item1</b> is the high 16-bit word (signed).</description></item>
        /// <item><description><b>Item2</b> is the low 16-bit word (signed).</description></item>
        /// </list>
        /// </returns>
        public static Pair<short, short> ToShort ( this int value  ) {
            byte [] arr = value.ToBytes(Endian.LittleEndian);

            short h = arr.ToShort(2);
            short l = arr.ToShort(0);

            return new Pair<short, short>(h, l);
        }

        /// <summary>
        /// Converts a <see cref="uint"/> value into a 4‑byte array using the specified endianness.
        /// </summary>
        public unsafe static byte[] ToBytes(this uint value, Endian endian = Endian.System) {
            if ( m_isLittleEndianSystem == triple.Nin ) InitEndian();

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
        public unsafe static byte[] ToBytes(this int value, Endian endian = Endian.System) {
            if ( m_isLittleEndianSystem == triple.Nin ) InitEndian();

            byte[] bytes = new byte[4];

            fixed ( byte* b = bytes )
                *(int*)b = value;

            if ( endian != Endian.System ) {
                if ( (endian == Endian.BigEndian && m_isLittleEndianSystem == triple.True) || 
                     (endian == Endian.LittleEndian && m_isLittleEndianSystem == triple.False) ) {
                    byte tmp;
                    tmp = bytes[0]; bytes[0] = bytes[3]; bytes[3] = tmp;
                    tmp = bytes[1]; bytes[1] = bytes[2]; bytes[2] = tmp;
                } 
            }
            return bytes;
        }
        /// <summary>
        /// Converts a 4‑byte array into an <see cref="int"/> using the specified endianness.
        /// </summary>
        public unsafe static int ToInt(this byte[] bytes, Endian endian = Endian.System) {
            if ( m_isLittleEndianSystem == triple.Nin ) InitEndian();

            if ( bytes.Length < 4 ) throw new ArgumentException("uint requires exactly 4 bytes");

            if ( endian != Endian.System ) {
                // Swap nur wenn nötig
                if ( (endian == Endian.BigEndian && m_isLittleEndianSystem == triple.True) ||
                (endian == Endian.LittleEndian && m_isLittleEndianSystem == triple.False) ) {
                    byte tmp;
                    tmp = bytes[0]; bytes[0] = bytes[3]; bytes[3] = tmp;
                    tmp = bytes[1]; bytes[1] = bytes[2]; bytes[2] = tmp;
                }
            }

            fixed ( byte* b = bytes )
                return *(int*)b;
        }
        /// <summary>
        /// Converts a 4‑byte region of a byte array into an <see cref="int"/> using the
        /// specified endianness. This overload reads the value starting at the given
        /// <paramref name="offsets"/> position and delegates the actual conversion to
        /// the base <c>ToInt(byte[], Endian)</c> method.
        /// 
        /// The method requires at least four bytes starting at the offset. The original
        /// array is not modified; a 4‑byte slice is extracted and passed to the primary
        /// conversion routine.
        /// </summary>
        /// <param name="bytes">The source byte array containing the integer value.</param>
        /// <param name="offsets">
        /// The starting position within <paramref name="bytes"/> from which the
        /// 4‑byte integer value is read.
        /// </param>
        /// <param name="endian">
        /// The endianness used to interpret the extracted 4‑byte sequence.
        /// </param>
        /// <returns>
        /// The converted 32‑bit integer value.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="offsets"/> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when fewer than four bytes are available starting at
        /// <paramref name="offsets"/>.
        /// </exception>

        public static int ToInt ( this byte[] bytes, long offsets, Endian endian = Endian.System ) {
            if ( offsets < 0 )
                throw new ArgumentOutOfRangeException(nameof(offsets));

            if ( offsets + 4 > bytes.Length )
                throw new ArgumentException("uint requires exactly 4 bytes at the given offset");

            // 4‑Byte‑Slice erzeugen
            byte[] slice = new byte[4];
            slice[0] = bytes[offsets + 0];
            slice[1] = bytes[offsets + 1];
            slice[2] = bytes[offsets + 2];
            slice[3] = bytes[offsets + 3];

            // Die bestehende Methode aufrufen
            return slice.ToInt(endian);
        }
        /// <summary>
        /// Converts a Arra<paramref name="bytes"/> into an <see cref="int"/> using the
        /// specified endianness. This overload reads the value starting at the given
        /// <paramref name="offsets"/> position and delegates the actual conversion to
        /// the base <c>ToInt(byte[], Endian)</c> method.
        /// </summary>
        public static int ToInt ( this Collections.Generic.Vector<byte> bytes, int offsets, Endian endian = Endian.System ) {
            if ( offsets < 0 )
                throw new ArgumentOutOfRangeException(nameof(offsets));

            if ( offsets + 4 > bytes.Count )
                throw new ArgumentException("uint requires exactly 4 bytes at the given offset");

            byte[] slice = new byte[4];
            slice[0] = bytes[offsets + 0];
            slice[1] = bytes[offsets + 1];
            slice[2] = bytes[offsets + 2];
            slice[3] = bytes[offsets + 3];

            // Die bestehende Methode aufrufen
            return slice.ToInt(endian);
        }

        /// <summary>
        /// Converts a 4‑byte array into a <see cref="uint"/> using the specified endianness.
        /// </summary>
        public unsafe static uint ToUInt(this byte[] bytes, Endian endian = Endian.System) {
            if ( m_isLittleEndianSystem == triple.Nin ) InitEndian();

            if ( bytes.Length < 4 ) throw new ArgumentException("uint requires exactly 4 bytes");

            if ( endian != Endian.System ) {
                if ( (endian == Endian.BigEndian && m_isLittleEndianSystem == triple.True) ||
                (endian == Endian.LittleEndian && m_isLittleEndianSystem == triple.False) ) {
                    byte tmp;
                    tmp = bytes[0]; bytes[0] = bytes[3]; bytes[3] = tmp;
                    tmp = bytes[1]; bytes[1] = bytes[2]; bytes[2] = tmp;
                }
            }

            fixed ( byte* b = bytes )
                return *(uint*)b;
        }

        /// <summary>
        /// Converts byte array into an <see cref="uint"/> using the
        /// specified endianness. This overload reads the value starting at the given
        /// <paramref name="offsets"/> position and delegates the actual conversion to
        /// the base <c>ToUInt(byte[], Endian)</c> method.
        /// </summary>
        /// <param name="bytes">The source byte array containing the integer value.</param>
        /// <param name="offsets">
        /// The starting position within <paramref name="bytes"/> from which the value is read.
        /// </param>
        /// <param name="endian">
        /// The endianness used to interpret the extracted the sequence.
        /// </param>
        /// <returns>
        /// The converted value.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="offsets"/> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when fewer than four bytes are available starting at
        /// <paramref name="offsets"/>.
        /// </exception>
        public static uint ToUInt ( this byte[] bytes, long offsets, Endian endian = Endian.System ) {
            if ( offsets < 0 )
                throw new ArgumentOutOfRangeException(nameof(offsets));

            if ( offsets + 4 > bytes.Length )
                throw new ArgumentException("uint requires exactly 4 bytes at the given offset");

            // 4‑Byte‑Slice erzeugen
            byte[] slice = new byte[4];
            slice[0] = bytes[offsets + 0];
            slice[1] = bytes[offsets + 1];
            slice[2] = bytes[offsets + 2];
            slice[3] = bytes[offsets + 3];

            // Die bestehende Methode aufrufen
            return slice.ToUInt(endian);
        }

        /// <summary>
        /// Converts a Arra<paramref name="bytes"/> into an <see cref="uint"/> using the
        /// specified endianness. This overload reads the value starting at the given
        /// <paramref name="offsets"/> position and delegates the actual conversion to
        /// the base <c>ToInt(byte[], Endian)</c> method.
        /// </summary>
        public static uint ToUInt ( this Collections.Generic.Vector<byte> bytes, int offsets, Endian endian = Endian.System ) {
            if ( offsets < 0 )
                throw new ArgumentOutOfRangeException(nameof(offsets));

            if ( offsets + 4 > bytes.Count )
                throw new ArgumentException("uint requires exactly 4 bytes at the given offset");

            byte[] slice = new byte[4];
            slice[0] = bytes[offsets + 0];
            slice[1] = bytes[offsets + 1];
            slice[2] = bytes[offsets + 2];
            slice[3] = bytes[offsets + 3];

            // Die bestehende Methode aufrufen
            return slice.ToUInt(endian);
        }

        #endregion

        #region SHORT

        /// <summary>
        /// Splits a 16-bit signed integer into its high and low 8-bit components.
        /// </summary>
        /// <param name="value">
        /// The 16-bit signed integer to split.
        /// </param>
        /// <returns>
        /// A <see cref="Pair{T1,T2}"/> where:
        /// <list type="bullet">
        /// <item><description><b>Item1</b> is the high 8-bit component (signed).</description></item>
        /// <item><description><b>Item2</b> is the low 8-bit component (unsigned).</description></item>
        /// </list>
        /// </returns>
        public static Pair<sbyte, byte> ToByte ( this short value ) {
            byte [] arr = value.ToBytes(Endian.LittleEndian);

            sbyte h = (sbyte)arr[1];
            byte l = arr[0];

            return new Pair<sbyte, byte>(h, l);
        }

        /// <summary>
        /// Splits a 16-bit unsigned integer into its high and low 8-bit components.
        /// </summary>
        /// <param name="value">
        /// The 16-bit signed integer to split.
        /// </param>
        /// <returns>
        /// A <see cref="Pair{T1,T2}"/> where:
        /// <list type="bullet">
        /// <item><description><b>Item1</b> is the high 8-bit component (unsigned).</description></item>
        /// <item><description><b>Item2</b> is the low 8-bit component (unsigned).</description></item>
        /// </list>
        /// </returns>
        public static Pair<byte, byte> ToByte ( this ushort value ) {
            byte [] arr = value.ToBytes(Endian.LittleEndian);

            byte h = arr[1];
            byte l = arr[0];

            return new Pair<byte, byte>(h, l);
        }
        /// <summary>
        /// Converts a <see cref="short"/> value into a 2‑byte array using the specified endianness.
        /// </summary>
        public unsafe static byte[] ToBytes(this short value, Endian endian = Endian.System) {
            if ( m_isLittleEndianSystem == triple.Nin ) InitEndian();

            byte[] bytes = new byte[2];

            fixed ( byte* b = bytes )
                *(short*)b = value;

            if ( endian != Endian.System ) {
                if ( (endian == Endian.BigEndian && m_isLittleEndianSystem == triple.True) ||
                (endian == Endian.LittleEndian && m_isLittleEndianSystem == triple.False) ) {
                    byte tmp = bytes[0];
                    bytes[0] = bytes[1];
                    bytes[1] = tmp;
                }
            }

            return bytes;
        }
        /// <summary>
        /// Converts a <see cref="ushort"/> value into a 2‑byte array using the specified endianness.
        /// </summary>
        public unsafe static byte[] ToBytes(this ushort value, Endian endian = Endian.System) {
            if ( m_isLittleEndianSystem == triple.Nin ) InitEndian();

            byte[] bytes = new byte[2];

            fixed ( byte* b = bytes )
                *(ushort*)b = value;

            if ( endian != Endian.System ) {
                if ( (endian == Endian.BigEndian && m_isLittleEndianSystem == triple.True) ||
                (endian == Endian.LittleEndian && m_isLittleEndianSystem == triple.False) ) {
                    byte tmp = bytes[0];
                    bytes[0] = bytes[1];
                    bytes[1] = tmp;
                }
            }

            return bytes;
        }
        /// <summary>
        /// Converts a 2‑byte array into a <see cref="ushort"/> using the specified endianness.
        /// </summary>
        public unsafe static ushort ToUShort(this byte[] bytes, Endian endian = Endian.System) {
            if ( m_isLittleEndianSystem == triple.Nin ) InitEndian();

            if ( bytes.Length < 2) throw new ArgumentException("short requires exactly 2 bytes");

            if ( endian != Endian.System ) {
                if ( (endian == Endian.BigEndian && m_isLittleEndianSystem == triple.True) ||
                (endian == Endian.LittleEndian && m_isLittleEndianSystem == triple.False) ) {
                    byte tmp = bytes[0];
                    bytes[0] = bytes[1];
                    bytes[1] = tmp;
                }
            }

            fixed ( byte* b = bytes )
                return *(ushort*)b;
        }
        /// <summary>
        /// Converts a Arra<paramref name="bytes"/> into an <see cref="short"/> using the
        /// specified endianness. This overload reads the value starting at the given
        /// <paramref name="offsets"/> position and delegates the actual conversion to
        /// the base <c>ToInt(byte[], Endian)</c> method.
        /// </summary>
        public static short ToShort ( this Collections.Generic.Vector<byte> bytes, int offsets, Endian endian = Endian.System ) {
            if ( offsets < 0 )
                throw new ArgumentOutOfRangeException(nameof(offsets));

            if ( offsets + 2 > bytes.Count )
                throw new ArgumentException("uint requires exactly 2 bytes at the given offset");

            byte[] slice = new byte[2];
            slice[0] = bytes[offsets + 0];
            slice[1] = bytes[offsets + 1];

            // Die bestehende Methode aufrufen
            return slice.ToShort(endian);
        }
        /// <summary>
        /// Converts byte array into an <see cref="ushort"/> using the
        /// specified endianness. This overload reads the value starting at the given
        /// <paramref name="offsets"/> position and delegates the actual conversion to
        /// the base <c>ToUShort(byte[], Endian)</c> method.
        /// </summary>
        /// <param name="bytes">The source byte array containing the short value.</param>
        /// <param name="offsets">
        /// The starting position within <paramref name="bytes"/> from which the value is read.
        /// </param>
        /// <param name="endian">
        /// The endianness used to interpret the extracted the sequence.
        /// </param>
        /// <returns>
        /// The converted value.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="offsets"/> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when fewer than four bytes are available starting at
        /// <paramref name="offsets"/>.
        /// </exception>
        public static ushort ToUShort ( this byte[] bytes, long offsets, Endian endian = Endian.System ) {
            if ( offsets < 0 )
                throw new ArgumentOutOfRangeException(nameof(offsets));

            if ( offsets + 2 > bytes.Length )
                throw new ArgumentException("uint requires exactly 2 bytes at the given offset");

            // 4‑Byte‑Slice erzeugen
            byte[] slice = new byte[4];
            slice[0] = bytes[offsets + 0];
            slice[1] = bytes[offsets + 1];

            // Die bestehende Methode aufrufen
            return slice.ToUShort(endian);
        }
        /// <summary>
        /// Converts a 2‑byte array into a <see cref="short"/> using the specified endianness.
        /// </summary>
        public unsafe static short ToShort(this byte[] bytes, Endian endian = Endian.System) {
            if ( m_isLittleEndianSystem == triple.Nin ) InitEndian();

            if ( bytes.Length < 2 ) throw new ArgumentException("short requires exactly 2 bytes");

            if ( endian != Endian.System ) {
                if ( (endian == Endian.BigEndian && m_isLittleEndianSystem == triple.True) ||
                (endian == Endian.LittleEndian && m_isLittleEndianSystem == triple.False) ) {
                    byte tmp = bytes[0];
                    bytes[0] = bytes[1];
                    bytes[1] = tmp;
                }
            }

            fixed ( byte* b = bytes )
                return *(short*)b;
        }
        /// <summary>
        /// Converts byte array into an <see cref="ushort"/> using the
        /// specified endianness. This overload reads the value starting at the given
        /// <paramref name="offsets"/> position and delegates the actual conversion to
        /// the base <c>ToShort(byte[], Endian)</c> method.
        /// </summary>
        /// <param name="bytes">The source byte array containing the short value.</param>
        /// <param name="offsets">
        /// The starting position within <paramref name="bytes"/> from which the value is read.
        /// </param>
        /// <param name="endian">
        /// The endianness used to interpret the extracted the sequence.
        /// </param>
        /// <returns>
        /// The converted value.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="offsets"/> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when fewer than four bytes are available starting at
        /// <paramref name="offsets"/>.
        /// </exception>
        public static short ToShort ( this byte[] bytes, long offsets, Endian endian = Endian.System ) {
            if ( offsets < 0 )
                throw new ArgumentOutOfRangeException(nameof(offsets));

            if ( offsets + 2 > bytes.Length )
                throw new ArgumentException("uint requires exactly 2 bytes at the given offset");

            // 2‑Byte‑Slice erzeugen
            byte[] slice = new byte[4];
            slice[0] = bytes[offsets + 0];
            slice[1] = bytes[offsets + 1];

            // Die bestehende Methode aufrufen
            return slice.ToShort(endian);
        }

        /// <summary>
        /// Converts a Arra<paramref name="bytes"/> into an <see cref="ushort"/> using the
        /// specified endianness. This overload reads the value starting at the given
        /// <paramref name="offsets"/> position and delegates the actual conversion to
        /// the base <c>ToInt(byte[], Endian)</c> method.
        /// </summary>
        public static ushort ToUShort ( this Collections.Generic.Vector<byte> bytes, int offsets, Endian endian = Endian.System ) {
            if ( offsets < 0 )
                throw new ArgumentOutOfRangeException(nameof(offsets));

            if ( offsets + 2 > bytes.Count )
                throw new ArgumentException("uint requires exactly 2 bytes at the given offset");

            byte[] slice = new byte[4];
            slice[0] = bytes[offsets + 0];
            slice[1] = bytes[offsets + 1];

            // Die bestehende Methode aufrufen
            return slice.ToUShort(endian);
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
            if ( m_isLittleEndianSystem == triple.Nin ) InitEndian();

            var x = provider.ToBytes(value);
            if(x == null) return new byte[]  {  0 };
            else return x.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static T? FromBytes<T>(this byte[] bytes, ByteSeriablizeProvider provider) 
            where T : IIsByteSeriablize {
            if ( m_isLittleEndianSystem == triple.Nin ) InitEndian();

            var x = new Cache(bytes, CacheType.OnlySystem);

            return (T?)provider.FromBytes(x)!;
        }

        /// <summary>
        /// Converts an unmanaged struct into a byte array using the specified endianness.
        /// </summary>
        public static unsafe byte[] ToBytes<T>(this T value, Endian endian = Endian.System) where T : unmanaged {
            int size = sizeof(T);
            if ( m_isLittleEndianSystem == triple.Nin ) InitEndian();

            byte[] bytes = new byte[size];

            fixed ( byte* b = bytes ) {
                *(T*)b = value;
            }

            return bytes;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="endian"></param>
        /// <returns></returns>
        public static unsafe byte[] ToBytes ( object obj, Endian endian ) {
            if ( m_isLittleEndianSystem == triple.Nin ) InitEndian();

            var type = obj.GetType();
            int size = System.Runtime.InteropServices.Marshal.SizeOf(type);
            byte[] bytes = new byte[size];

            fixed ( byte* b = bytes ) {
#pragma warning disable CS8500 // Erfasst die Adresse, ermittelt die Größe oder deklariert einen Zeiger auf einen verwalteten Typ.
                *(object*)b = obj; // funktioniert, wenn obj wirklich unmanaged ist
#pragma warning restore CS8500 // Erfasst die Adresse, ermittelt die Größe oder deklariert einen Zeiger auf einen verwalteten Typ.
            }

            return bytes;
        }

        /// <summary>
        /// Converts an array of unmanaged structs into a contiguous byte array.
        /// </summary>
        public static unsafe byte[] ToBytes<T>(this T[] array) where T : unmanaged {
            if ( m_isLittleEndianSystem == triple.Nin ) InitEndian();

            int size = sizeof(T) * array.Length;
            byte[] bytes = new byte[size];

            fixed ( T* src = array )
            fixed ( byte* dst = bytes ) {
                System.Buffer.MemoryCopy(src, dst, size, size);
            }

            return bytes;
        }
        /// <summary>
        /// Converts a byte array into an unmanaged struct of type <typeparamref name="T"/>.
        /// </summary>
        public static unsafe T FromBytes<T>(byte[] bytes, Endian endian = Endian.System) where T : unmanaged {
            if ( m_isLittleEndianSystem == triple.Nin ) InitEndian();

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
            if ( m_isLittleEndianSystem == triple.Nin ) InitEndian();

            int size = sizeof(T);
            int count = bytes.Length / size;

            T[] array = new T[count];

            fixed ( byte* src = bytes )
            fixed ( T* dst = array ) {
                System.Buffer.MemoryCopy(src, dst, bytes.Length, bytes.Length);
            }

            return array;
        }
        #endregion

        #region FLOAT

        /// <summary>
        /// Converts a <see cref="float"/> value into a 4‑byte array using the specified endianness.
        /// </summary>
        public unsafe static byte[] ToBytes(this float value, Endian endian = Endian.System) {
            if ( m_isLittleEndianSystem == triple.Nin ) InitEndian();

            byte[] bytes = new byte[4];

            fixed ( byte* b = bytes )
                *(float*)b = value;

            if ( endian != Endian.System ) {
                if ( (endian == Endian.BigEndian && m_isLittleEndianSystem == triple.True) ||
                (endian == Endian.LittleEndian && m_isLittleEndianSystem == triple.False) ) {
                    byte tmp;
                    tmp = bytes[0]; bytes[0] = bytes[3]; bytes[3] = tmp;
                    tmp = bytes[1]; bytes[1] = bytes[2]; bytes[2] = tmp;
                }
            }

            return bytes;
        }

        /// <summary>
        /// Converts a 4‑byte array into a <see cref="float"/> using the specified endianness.
        /// </summary>
        public unsafe static float ToFloat(this byte[] bytes, Endian endian = Endian.System) {
            if ( m_isLittleEndianSystem == triple.Nin ) InitEndian();

            if ( bytes.Length < 4 ) throw new ArgumentException("Float requires exactly 4 bytes");

            if ( endian != Endian.System ) {
                if ( (endian == Endian.BigEndian && m_isLittleEndianSystem == triple.True) ||
                (endian == Endian.LittleEndian && m_isLittleEndianSystem == triple.False) ) {
                    byte tmp;
                    tmp = bytes[0]; bytes[0] = bytes[3]; bytes[3] = tmp;
                    tmp = bytes[1]; bytes[1] = bytes[2]; bytes[2] = tmp;
                }
            }

            fixed ( byte* b = bytes )
                return *(float*)b;
        }
        /// <summary>
        /// Converts byte array into an <see cref="float"/> using the
        /// specified endianness. This overload reads the value starting at the given
        /// <paramref name="offsets"/> position and delegates the actual conversion to
        /// the base <c>ToFloat(byte[], Endian)</c> method.
        /// </summary>
        /// <param name="bytes">The source byte array containing the float value.</param>
        /// <param name="offsets">
        /// The starting position within <paramref name="bytes"/> from which the value is read.
        /// </param>
        /// <param name="endian">
        /// The endianness used to interpret the extracted the sequence.
        /// </param>
        /// <returns>
        /// The converted value.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="offsets"/> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when fewer than four bytes are available starting at
        /// <paramref name="offsets"/>.
        /// </exception>
        public static float ToFloat ( this byte[] bytes, long offsets, Endian endian = Endian.System ) {
            if ( offsets < 0 )
                throw new ArgumentOutOfRangeException(nameof(offsets));

            if ( offsets + 4 > bytes.Length )
                throw new ArgumentException("uint requires exactly 4 bytes at the given offset");

            // 4‑Byte‑Slice erzeugen
            byte[] slice = new byte[4];
            slice[0] = bytes[offsets + 0];
            slice[1] = bytes[offsets + 1];
            slice[2] = bytes[offsets + 2];
            slice[3] = bytes[offsets + 3];

            // Die bestehende Methode aufrufen
            return slice.ToFloat(endian);
        }

        /// <summary>
        /// Converts a Arra<paramref name="bytes"/> into an <see cref="float"/> using the
        /// specified endianness. This overload reads the value starting at the given
        /// <paramref name="offsets"/> position and delegates the actual conversion to
        /// the base <c>ToInt(byte[], Endian)</c> method.
        /// </summary>
        public static float ToFloat ( this Collections.Generic.Vector<byte> bytes, int offsets = 0, Endian endian = Endian.System ) {
            if ( offsets < 0 )
                throw new ArgumentOutOfRangeException(nameof(offsets));

            if ( offsets + 4 > bytes.Count )
                throw new ArgumentException("uint requires exactly 4 bytes at the given offset");

            byte[] slice = new byte[4];
            slice[0] = bytes[offsets + 0];
            slice[1] = bytes[offsets + 1];
            slice[2] = bytes[offsets + 2];
            slice[3] = bytes[offsets + 3];

            // Die bestehende Methode aufrufen
            return slice.ToFloat(endian);
        }

        #endregion

        #region DOUBLE

        /// <summary>
        /// Converts a <see cref="double"/> value into an 8‑byte array using the specified endianness.
        /// </summary>
        public unsafe static byte[] ToBytes(this double value, Endian endian = Endian.System) {
            if ( m_isLittleEndianSystem == triple.Nin ) InitEndian();

            byte[] bytes = new byte[8];

            fixed ( byte* b = bytes )
                *(double*)b = value;

            if ( endian != Endian.System ) {
                if ( (endian == Endian.BigEndian && m_isLittleEndianSystem == triple.True) ||
                (endian == Endian.LittleEndian && m_isLittleEndianSystem == triple.False) ) {
                    // reverse 8 bytes
                    byte tmp;
                    tmp = bytes[0]; bytes[0] = bytes[7]; bytes[7] = tmp;
                    tmp = bytes[1]; bytes[1] = bytes[6]; bytes[6] = tmp;
                    tmp = bytes[2]; bytes[2] = bytes[5]; bytes[5] = tmp;
                    tmp = bytes[3]; bytes[3] = bytes[4]; bytes[4] = tmp;
                }
            }

            return bytes;
        }
        /// <summary>
        /// Converts byte array into an <see cref="double"/> using the
        /// specified endianness. This overload reads the value starting at the given
        /// <paramref name="offsets"/> position and delegates the actual conversion to
        /// the base <c>ToDouble(byte[], Endian)</c> method.
        /// </summary>
        /// <param name="bytes">The source byte array containing the double value.</param>
        /// <param name="offsets">
        /// The starting position within <paramref name="bytes"/> from which the value is read.
        /// </param>
        /// <param name="endian">
        /// The endianness used to interpret the extracted the sequence.
        /// </param>
        /// <returns>
        /// The converted value.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="offsets"/> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when fewer than four bytes are available starting at
        /// <paramref name="offsets"/>.
        /// </exception>
        public static double ToDouble ( this byte[] bytes, long offsets, Endian endian ) {
            if ( offsets < 0 )
                throw new ArgumentOutOfRangeException(nameof(offsets));

            if ( offsets + 8 > bytes.Length )
                throw new ArgumentException("uint requires exactly 8 bytes at the given offset");

            // 8‑Byte‑Slice erzeugen
            byte[] slice = new byte[8];
            slice[0] = bytes[offsets + 0];
            slice[1] = bytes[offsets + 1];
            slice[2] = bytes[offsets + 2];
            slice[3] = bytes[offsets + 3];
            slice[4] = bytes[offsets + 4];
            slice[5] = bytes[offsets + 5];
            slice[6] = bytes[offsets + 6];
            slice[7] = bytes[offsets + 7];

            // Die bestehende Methode aufrufen
            return slice.ToDouble(endian);
        }

        /// <summary>
        /// Converts an 8‑byte array into a <see cref="double"/> using the specified endianness.
        /// </summary>
        public unsafe static double ToDouble(this byte[] bytes, Endian endian = Endian.System) {
            if ( m_isLittleEndianSystem == triple.Nin ) InitEndian();

            if ( bytes.Length < 8 ) throw new ArgumentException("Double requires at least 8 bytes");

            if ( endian != Endian.System ) {
                if ( (endian == Endian.BigEndian && m_isLittleEndianSystem == triple.True) ||
                (endian == Endian.LittleEndian && m_isLittleEndianSystem == triple.False) ) {
                    byte tmp;
                    tmp = bytes[0]; bytes[0] = bytes[7]; bytes[7] = tmp;
                    tmp = bytes[1]; bytes[1] = bytes[6]; bytes[6] = tmp;
                    tmp = bytes[2]; bytes[2] = bytes[5]; bytes[5] = tmp;
                    tmp = bytes[3]; bytes[3] = bytes[4]; bytes[4] = tmp;
                }
            }

            fixed ( byte* b = bytes )
                return *(double*)b;
        }

        /// <summary>
        /// Converts a Arra<paramref name="bytes"/> into an <see cref="double"/> using the
        /// specified endianness. 
        /// </summary>
        public static double ToDouble ( this Collections.Generic.Vector<byte> bytes, int offsets = 0, Endian endian = Endian.System ) {
            if ( offsets < 0 )
                throw new ArgumentOutOfRangeException(nameof(offsets));

            if ( offsets + 8 > bytes.Count )
                throw new ArgumentException("uint requires exactly 8 bytes at the given offset");

            byte[] slice = new byte[8];
            slice[0] = bytes[offsets + 0];
            slice[1] = bytes[offsets + 1];
            slice[2] = bytes[offsets + 2];
            slice[3] = bytes[offsets + 3];
            slice[4] = bytes[offsets + 4];
            slice[5] = bytes[offsets + 5];
            slice[6] = bytes[offsets + 6];
            slice[7] = bytes[offsets + 7];

            // Die bestehende Methode aufrufen
            return slice.ToDouble(endian);
        }

        #endregion


        #region HALF

        /// <summary>
        /// Converts byte array into an <see cref="Half16"/> using the
        /// specified endianness. This overload reads the value starting at the given
        /// <paramref name="offsets"/> position and delegates the actual conversion to
        /// the base <c>ToDouble(byte[], Endian)</c> method.
        /// </summary>
        /// <param name="bytes">The source byte array containing the double value.</param>
        /// <param name="offsets">
        /// The starting position within <paramref name="bytes"/> from which the value is read.
        /// </param>
        /// <param name="endian">
        /// The endianness used to interpret the extracted the sequence.
        /// </param>
        /// <returns>
        /// The converted value.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="offsets"/> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when fewer than four bytes are available starting at
        /// <paramref name="offsets"/>.
        /// </exception>
        public static Half16 ToHalf16( this byte[] bytes, long offsets, Endian endian ) {
			ushort raw = bytes.ToUShort(offsets, endian);

			return new Half16(raw);
		}

		/// <summary>
		/// Converts an 8‑byte array into a <see cref="Half16"/> using the specified endianness.
		/// </summary>
		public unsafe static Half16 ToHalf16 ( this byte[] bytes, Endian endian = Endian.System ) {
            ushort raw = bytes.ToUShort(endian);

            return new Half16(raw);
        }

        /// <summary>
        /// Converts a Arra<paramref name="bytes"/> into an <see cref="double"/> using the
        /// specified endianness. 
        /// </summary>
        public static Half16 ToHalf16 ( this Collections.Generic.Vector<byte> bytes, int offsets = 0, Endian endian = Endian.System ) {
			ushort raw = bytes.ToUShort(offsets, endian);
			return new Half16(raw);
		}

		#endregion

		#region HALF16b

		/// <summary>
		/// Converts byte array into an <see cref="Half16"/> using the
		/// specified endianness. This overload reads the value starting at the given
		/// <paramref name="offsets"/> position and delegates the actual conversion to
		/// the base <c>ToDouble(byte[], Endian)</c> method.
		/// </summary>
		/// <param name="bytes">The source byte array containing the double value.</param>
		/// <param name="offsets">
		/// The starting position within <paramref name="bytes"/> from which the value is read.
		/// </param>
		/// <param name="endian">
		/// The endianness used to interpret the extracted the sequence.
		/// </param>
		/// <returns>
		/// The converted value.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when <paramref name="offsets"/> is negative.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown when fewer than four bytes are available starting at
		/// <paramref name="offsets"/>.
		/// </exception>
		public static Half16b ToHalf16b ( this byte[] bytes, long offsets, Endian endian ) {
			ushort raw = bytes.ToUShort(offsets, endian);

			return new Half16b(raw);
		}

		/// <summary>
		/// Converts an 8‑byte array into a <see cref="Half16"/> using the specified endianness.
		/// </summary>
		public unsafe static Half16b ToHalf16b ( this byte[] bytes, Endian endian = Endian.System ) {
			ushort raw = bytes.ToUShort(endian);

			return new Half16b(raw);
		}

		/// <summary>
		/// Converts a Arra<paramref name="bytes"/> into an <see cref="double"/> using the
		/// specified endianness. 
		/// </summary>
		public static Half16b ToHalf16b ( this Collections.Generic.Vector<byte> bytes, int offsets = 0, Endian endian = Endian.System ) {
			ushort raw = bytes.ToUShort(offsets, endian);
			return new Half16b(raw);
		}

		#endregion

		#region LONG



		/// <summary>
		/// Splits a 64-bit unsigned integer into its high and low 32-bit components.
		/// </summary>
		/// <param name="value">
		/// The 64-bit unsigned integer to split.
		/// </param>
		/// <returns>
		/// A <see cref="Pair{T1,T2}"/> where:
		/// <list type="bullet">
		/// <item><description><b>Item1</b> is the high 32-bit word (unsigned).</description></item>
		/// <item><description><b>Item2</b> is the low 32-bit word (signed).</description></item>
		/// </list>
		/// </returns>
		public static Pair<uint, int> ToShort ( this ulong value ) {
            byte [] arr = value.ToBytes(Endian.LittleEndian);

            uint h = arr.ToUInt(4);
            int l = arr.ToInt(0);

            return new Pair<uint, int>(h, l);
        }

        /// <summary>
        /// Splits a 64-bit unsigned integer into its high and low 32-bit components.
        /// </summary>
        /// <param name="value">
        /// The 64-bit unsigned integer to split.
        /// </param>
        /// <returns>
        /// A <see cref="Pair{T1,T2}"/> where:
        /// <list type="bullet">
        /// <item><description><b>Item1</b> is the high 32-bit word (signed).</description></item>
        /// <item><description><b>Item2</b> is the low 32-bit word (signed).</description></item>
        /// </list>
        /// </returns>
        public static Pair<int, int> ToShort ( this long value ) {
            byte [] arr = value.ToBytes(Endian.LittleEndian);

            int h = arr.ToInt(4);
            int l = arr.ToInt(0);

            return new Pair<int, int>(h, l);
        }

        /// <summary>
        /// Converts a <see cref="long"/> value into an 8‑byte array using the specified endianness.
        /// </summary>
        public unsafe static byte[] ToBytes(this long value, Endian endian = Endian.System) {
            if ( m_isLittleEndianSystem == triple.Nin ) InitEndian();

            byte[] bytes = new byte[8];

            fixed ( byte* b = bytes )
                *(long*)b = value;

            if ( endian != Endian.System ) {
                if ( (endian == Endian.BigEndian && m_isLittleEndianSystem == triple.True) ||
                (endian == Endian.LittleEndian && m_isLittleEndianSystem == triple.False) ) {
                    // reverse 8 bytes
                    byte tmp;
                    tmp = bytes[0]; bytes[0] = bytes[7]; bytes[7] = tmp;
                    tmp = bytes[1]; bytes[1] = bytes[6]; bytes[6] = tmp;
                    tmp = bytes[2]; bytes[2] = bytes[5]; bytes[5] = tmp;
                    tmp = bytes[3]; bytes[3] = bytes[4]; bytes[4] = tmp;
                }
            }

            return bytes;
        }
        /// <summary>
        /// Converts an 8‑byte array into a <see cref="long"/> using the specified endianness.
        /// </summary>
        public unsafe static long ToLong(this byte[] bytes, Endian endian = Endian.System) {
            if ( m_isLittleEndianSystem == triple.Nin ) InitEndian();

            if ( bytes.Length < 8 ) throw new ArgumentException("long requires at least 8 bytes");

            if ( endian != Endian.System ) {
                if ( (endian == Endian.BigEndian && m_isLittleEndianSystem == triple.True) ||
                (endian == Endian.LittleEndian && m_isLittleEndianSystem == triple.False) ) {
                    byte tmp;
                    tmp = bytes[0]; bytes[0] = bytes[7]; bytes[7] = tmp;
                    tmp = bytes[1]; bytes[1] = bytes[6]; bytes[6] = tmp;
                    tmp = bytes[2]; bytes[2] = bytes[5]; bytes[5] = tmp;
                    tmp = bytes[3]; bytes[3] = bytes[4]; bytes[4] = tmp;
                }
            }

            fixed ( byte* b = bytes )
                return *(long*)b;
        }
        /// <summary>
        /// Converts byte array into an <see cref="long"/> using the
        /// specified endianness. This overload reads the value starting at the given
        /// <paramref name="offsets"/> position and delegates the actual conversion to
        /// the base <c>ToLong(byte[], Endian)</c> method.
        /// </summary>
        /// <param name="bytes">The source byte array containing the long value.</param>
        /// <param name="offsets">
        /// The starting position within <paramref name="bytes"/> from which the value is read.
        /// </param>
        /// <param name="endian">
        /// The endianness used to interpret the extracted the sequence.
        /// </param>
        /// <returns>
        /// The converted value.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="offsets"/> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when fewer than four bytes are available starting at
        /// <paramref name="offsets"/>.
        /// </exception>
        public static long ToLong ( this byte[] bytes, long offsets, Endian endian = Endian.System ) {
            if ( offsets < 0 )
                throw new ArgumentOutOfRangeException(nameof(offsets));

            if ( offsets + 8 > bytes.Length )
                throw new ArgumentException("uint requires exactly 8 bytes at the given offset");

            // 4‑Byte‑Slice erzeugen
            byte[] slice = new byte[8];
            slice[0] = bytes[offsets + 0];
            slice[1] = bytes[offsets + 1];
            slice[2] = bytes[offsets + 2];
            slice[3] = bytes[offsets + 3];
            slice[4] = bytes[offsets + 4];
            slice[5] = bytes[offsets + 5];
            slice[6] = bytes[offsets + 6];
            slice[7] = bytes[offsets + 7];

            // Die bestehende Methode aufrufen
            return slice.ToLong(endian);
        }
        /// <summary>
        /// Converts a <see cref="ulong"/> value into an 8‑byte array using the specified endianness.
        /// </summary>
        public unsafe static byte[] ToBytes(this ulong value, Endian endian = Endian.System) {
            if ( m_isLittleEndianSystem == triple.Nin ) InitEndian();

            byte[] bytes = new byte[8];

            fixed ( byte* b = bytes )
                *(ulong*)b = value;

            if ( endian != Endian.System ) {
                if ( (endian == Endian.BigEndian && m_isLittleEndianSystem == triple.True) ||
                (endian == Endian.LittleEndian && m_isLittleEndianSystem == triple.False) ) {
                    // reverse 8 bytes
                    byte tmp;
                    tmp = bytes[0]; bytes[0] = bytes[7]; bytes[7] = tmp;
                    tmp = bytes[1]; bytes[1] = bytes[6]; bytes[6] = tmp;
                    tmp = bytes[2]; bytes[2] = bytes[5]; bytes[5] = tmp;
                    tmp = bytes[3]; bytes[3] = bytes[4]; bytes[4] = tmp;
                }
            }

            return bytes;
        }
        /// <summary>
        /// Converts an 8‑byte array into a <see cref="ulong"/> using the specified endianness.
        /// </summary>
        public unsafe static ulong ToULong(this byte[] bytes, Endian endian = Endian.System) {
            if ( m_isLittleEndianSystem == triple.Nin ) InitEndian();

            if ( bytes.Length < 8 ) throw new ArgumentException("ulong requires at least 8 bytes");

            if ( endian != Endian.System ) {
                if ( (endian == Endian.BigEndian && m_isLittleEndianSystem == triple.True) ||
                (endian == Endian.LittleEndian && m_isLittleEndianSystem == triple.False) ) {
                    byte tmp;
                    tmp = bytes[0]; bytes[0] = bytes[7]; bytes[7] = tmp;
                    tmp = bytes[1]; bytes[1] = bytes[6]; bytes[6] = tmp;
                    tmp = bytes[2]; bytes[2] = bytes[5]; bytes[5] = tmp;
                    tmp = bytes[3]; bytes[3] = bytes[4]; bytes[4] = tmp;
                }
            }

            fixed ( byte* b = bytes )
                return *(ulong*)b;
        }
        /// <summary>
        /// Converts byte array into an <see cref="ulong"/> using the
        /// specified endianness. This overload reads the value starting at the given
        /// <paramref name="offsets"/> position and delegates the actual conversion to
        /// the base <c>ToULong(byte[], Endian)</c> method.
        /// </summary>
        /// <param name="bytes">The source byte array containing the ulong value.</param>
        /// <param name="offsets">
        /// The starting position within <paramref name="bytes"/> from which the value is read.
        /// </param>
        /// <param name="endian">
        /// The endianness used to interpret the extracted the sequence.
        /// </param>
        /// <returns>
        /// The converted value.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="offsets"/> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when fewer than four bytes are available starting at
        /// <paramref name="offsets"/>.
        /// </exception>
        public static ulong ToULong ( this byte[] bytes, long offsets, Endian endian = Endian.System ) {
            if ( offsets < 0 )
                throw new ArgumentOutOfRangeException(nameof(offsets));

            if ( offsets + 8 > bytes.Length )
                throw new ArgumentException("uint requires exactly 8 bytes at the given offset");

            // 4‑Byte‑Slice erzeugen
            byte[] slice = new byte[8];
            slice[0] = bytes[offsets + 0];
            slice[1] = bytes[offsets + 1];
            slice[2] = bytes[offsets + 2];
            slice[3] = bytes[offsets + 3];
            slice[4] = bytes[offsets + 4];
            slice[5] = bytes[offsets + 5];
            slice[6] = bytes[offsets + 6];
            slice[7] = bytes[offsets + 7];

            // Die bestehende Methode aufrufen
            return slice.ToULong(endian);
        }

        /// <summary>
        /// Converts a Arra<paramref name="bytes"/> into an <see cref="long"/> using the
        /// specified endianness. 
        /// </summary>
        public static long ToLong ( this Collections.Generic.Vector<byte> bytes, int offsets = 0, Endian endian = Endian.System ) {
            if ( offsets < 0 )
                throw new ArgumentOutOfRangeException(nameof(offsets));

            if ( offsets + 8 > bytes.Count )
                throw new ArgumentException("uint requires exactly 8 bytes at the given offset");

            byte[] slice = new byte[8];
            slice[0] = bytes[offsets + 0];
            slice[1] = bytes[offsets + 1];
            slice[2] = bytes[offsets + 2];
            slice[3] = bytes[offsets + 3];
            slice[4] = bytes[offsets + 4];
            slice[5] = bytes[offsets + 5];
            slice[6] = bytes[offsets + 6];
            slice[7] = bytes[offsets + 7];

            // Die bestehende Methode aufrufen
            return slice.ToLong(endian);
        }

        /// <summary>
        /// Converts a Arra<paramref name="bytes"/> into an <see cref="ulong"/> using the
        /// specified endianness. 
        /// </summary>
        public static ulong ToULong ( this Collections.Generic.Vector<byte> bytes, int offsets = 0, Endian endian = Endian.System ) {
            if ( offsets < 0 )
                throw new ArgumentOutOfRangeException(nameof(offsets));

            if ( offsets + 8 > bytes.Count )
                throw new ArgumentException("uint requires exactly 8 bytes at the given offset");

            byte[] slice = new byte[8];
            slice[0] = bytes[offsets + 0];
            slice[1] = bytes[offsets + 1];
            slice[2] = bytes[offsets + 2];
            slice[3] = bytes[offsets + 3];
            slice[4] = bytes[offsets + 4];
            slice[5] = bytes[offsets + 5];
            slice[6] = bytes[offsets + 6];
            slice[7] = bytes[offsets + 7];

            // Die bestehende Methode aufrufen
            return slice.ToULong(endian);
        }

        #endregion

        #region DLong


        #endregion


        #region QLong


        #endregion


        /// <summary>
        /// Parses a human‑readable size string such as "4K", "16M", "1G" or "512"
        /// into a byte count.  
        /// Defaults to 512 bytes if parsing fails.
        /// </summary>
        public static ulong SizeCalc ( string value ) {
            string str = value.ToUpper(System.Globalization.CultureInfo.CurrentCulture).Trim();
            uint multiplier = 1;

            if ( str.EndsWith("GI", StringComparison.Ordinal) ) {
                multiplier = 1024u * 1024u * 1024u;
                str = str[..^2];
            } else if ( str.EndsWith("MI", StringComparison.Ordinal) ) {
                multiplier = 1024u * 1024u;
                str = str[..^2];
            } else if ( str.EndsWith("KI", StringComparison.Ordinal) ) {
                multiplier = 1024u;
                str = str[..^2];
            } else if ( str.EndsWith("BI", StringComparison.Ordinal) ) {
                multiplier = 1;
                str = str[..^2];
            } else if ( str.EndsWith("G", StringComparison.Ordinal) ) {
                multiplier = 1000u * 1000u * 1000u;
                str = str[..^1];
            } else if ( str.EndsWith("M", StringComparison.Ordinal) ) {
                multiplier = 1000u * 1000u;
                str = str[..^1];
            } else if ( str.EndsWith("K", StringComparison.Ordinal) ) {
                multiplier = 1000u;
                str = str[..^1];
            } else if ( str.EndsWith("B", StringComparison.Ordinal) ) {
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
	/// @}
}
