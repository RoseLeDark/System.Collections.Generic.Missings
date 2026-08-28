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
namespace SystemEx.Utils {
	/// \addtogroup Utils
	/// @{

	/// <summary>
	/// Provides low‑level bit manipulation utilities for all primitive integer types.  
	/// Includes bit extraction, bit setting, bit flipping, bit masks, and bit rotation
	/// for signed and unsigned 8‑, 16‑, 32‑ and 64‑bit values.
	/// </summary>
	public static class BitUtils {
		// ---------------------------------------------------------------------
		//  GET BIT
		// ---------------------------------------------------------------------

		/// <summary>
		/// Returns whether the bit at the specified position is set.
		/// </summary>
		[Obsolete("please use Bit.BitIntSpan, Bit.BitLongSpan, Bit.BitUintSpan or Bit.BitULongSpan. This util will be remove in future release")]
		public static bool GetBit(this byte value, short pos)
            => ((value >> pos) & 1) != 0;

		/// <summary>
		/// Returns whether the bit at the specified position is set.
		/// </summary>
		[Obsolete("please use Bit.BitIntSpan, Bit.BitLongSpan, Bit.BitUintSpan or Bit.BitULongSpan. This util will be remove in future release")]
		public static bool GetBit(this short value, short pos)
            => ((value >> pos) & 1) != 0;

		/// <summary>
		/// Returns whether the bit at the specified position is set.
		/// </summary>
		[Obsolete("please use Bit.BitIntSpan, Bit.BitLongSpan, Bit.BitUintSpan or Bit.BitULongSpan. This util will be remove in future release")]
		public static bool GetBit(this int value, int pos)
            => ((value >> pos) & 1) != 0;

		/// <summary>
		/// Returns whether the bit at the specified position is set.
		/// </summary>
		[Obsolete("please use Bit.BitIntSpan, Bit.BitLongSpan, Bit.BitUintSpan or Bit.BitULongSpan. This util will be remove in future release")]
		public static bool GetBit(this long value, int pos)
            => ((value >> pos) & 1L) != 0;

		/// <summary>
		/// Returns whether the bit at the specified position is set.
		/// </summary>
		[Obsolete("please use Bit.BitIntSpan, Bit.BitLongSpan, Bit.BitUintSpan or Bit.BitULongSpan. This util will be remove in future release")]
		public static bool GetBit(this ushort value, short pos)
            => ((value >> pos) & 1) != 0;

		/// <summary>
		/// Returns whether the bit at the specified position is set.
		/// </summary>
		[Obsolete("please use Bit.BitIntSpan, Bit.BitLongSpan, Bit.BitUintSpan or Bit.BitULongSpan. This util will be remove in future release")]
		public static bool GetBit(this uint value, int pos)
            => ((value >> pos) & 1U) != 0;

		/// <summary>
		/// Returns whether the bit at the specified position is set.
		/// </summary>
		[Obsolete("please use Bit.BitIntSpan, Bit.BitLongSpan, Bit.BitUintSpan or Bit.BitULongSpan. This util will be remove in future release")]
		public static bool GetBit(this ulong value, int pos)
            => ((value >> pos) & 1UL) != 0;

		// ---------------------------------------------------------------------
		//  SET BIT
		// ---------------------------------------------------------------------

		/// <summary>
		/// Sets or clears the bit at the specified position.
		/// </summary>
		[Obsolete("please use Bit.BitIntSpan, Bit.BitLongSpan, Bit.BitUintSpan or Bit.BitULongSpan. This util will be remove in future release")]
		public static byte SetBit(this byte value, bool bit, byte pos) {
            return (byte)(bit ? (byte)value | (1 << pos)
                              : (byte)(value & ~(1 << pos)));
        }

		/// <summary>
		/// Sets or clears the bit at the specified position.
		/// </summary>
		[Obsolete("please use Bit.BitIntSpan, Bit.BitLongSpan, Bit.BitUintSpan or Bit.BitULongSpan. This util will be remove in future release")]
		public static short SetBit(this short value, bool bit, int pos) {
            ushort mask = (ushort)(1 << pos);
            return (short)(bit ? (ushort)((ushort)value | mask)
                               : (ushort)(value & ~mask));
        }

		/// <summary>
		/// Sets or clears the bit at the specified position.
		/// </summary>
		[Obsolete("please use Bit.BitIntSpan, Bit.BitLongSpan, Bit.BitUintSpan or Bit.BitULongSpan. This util will be remove in future release")]
		public static int SetBit(this int value, bool bit, int pos)
            => bit ? (value | (1 << pos)) : (value & ~(1 << pos));

		/// <summary>
		/// Sets or clears the bit at the specified position.
		/// </summary>
		[Obsolete("please use Bit.BitIntSpan, Bit.BitLongSpan, Bit.BitUintSpan or Bit.BitULongSpan. This util will be remove in future release")]
		public static long SetBit(this long value, bool bit, int pos) {
            long mask = 1L << pos;
            return bit ? (value | mask) : (value & ~mask);
        }

		/// <summary>
		/// Sets or clears the bit at the specified position.
		/// </summary>
		[Obsolete("please use Bit.BitIntSpan, Bit.BitLongSpan, Bit.BitUintSpan or Bit.BitULongSpan. This util will be remove in future release")]
		public static short SetBit(this ushort value, bool bit, short pos) {
            return (short)(bit ? (int)value | (1 << pos)
                               : (short)(value & ~(1 << pos)));
        }

		/// <summary>
		/// Sets or clears the bit at the specified position.
		/// </summary>
		[Obsolete("please use Bit.BitIntSpan, Bit.BitLongSpan, Bit.BitUintSpan or Bit.BitULongSpan. This util will be remove in future release")]
		public static uint SetBit(this uint value, bool bit, int pos) {
            uint mask = 1U << pos;
            return bit ? (value | mask) : (value & ~mask);
        }

		/// <summary>
		/// Sets or clears the bit at the specified position.
		/// </summary>
		[Obsolete("please use Bit.BitIntSpan, Bit.BitLongSpan, Bit.BitUintSpan or Bit.BitULongSpan. This util will be remove in future release")]
		public static ulong SetBit(this ulong value, bool bit, int pos) {
            ulong mask = 1UL << pos;
            return bit ? (value | mask) : (value & ~mask);
        }

		// ---------------------------------------------------------------------
		//  MASK RANGE
		// ---------------------------------------------------------------------

		/// <summary>
		/// Creates a bit mask covering <paramref name="length"/> bits starting at
		/// <paramref name="start"/>.  
		/// Example: <c>MaskRange(4, 3)</c> → <c>0b0001110000</c>.
		/// </summary>
		public static int MaskRange(int start, int length) {
            if ( length <= 0 ) return 0;
            if ( start < 0 || start > 31 ) return 0;
            if ( length >= 32 ) return unchecked((int)0xFFFFFFFF);

            int mask = (1 << length) - 1;
            return mask << start;
        }

		// ---------------------------------------------------------------------
		//  ROTATE LEFT
		// ---------------------------------------------------------------------

		/// <summary>
		/// Rotates the bits of an 8‑bit value to the left.
		/// </summary>
		[Obsolete("please use Bit.BitIntSpan, Bit.BitLongSpan, Bit.BitUintSpan or Bit.BitULongSpan. This util will be remove in future release")]
		public static int RotateLeft(this byte value, int count) {
            count &= 7;
            uint v = (uint)value;
            return (int)((v << count) | (v >> (8 - count)));
        }

		/// <summary>
		/// Rotates the bits of a 16‑bit value to the left.
		/// </summary>
		[Obsolete("please use Bit.BitIntSpan, Bit.BitLongSpan, Bit.BitUintSpan or Bit.BitULongSpan. This util will be remove in future release")]
		public static int RotateLeft(this short value, int count) {
            count &= 15;
            uint v = (uint)value;
            return (int)((v << count) | (v >> (16 - count)));
        }

		/// <summary>
		/// Rotates the bits of a 32‑bit value to the left.
		/// </summary>
		[Obsolete("please use Bit.BitIntSpan, Bit.BitLongSpan, Bit.BitUintSpan or Bit.BitULongSpan. This util will be remove in future release")]
		public static int RotateLeft(this int value, int count) {
            count &= 31;
            uint v = (uint)value;
            return (int)((v << count) | (v >> (32 - count)));
        }

		/// <summary>
		/// Rotates the bits of a 64‑bit value to the left.
		/// </summary>
		[Obsolete("please use Bit.BitIntSpan, Bit.BitLongSpan, Bit.BitUintSpan or Bit.BitULongSpan. This util will be remove in future release")]
		public static long RotateLeft(this long value, int count) {
            count &= 63;
            ulong v = (ulong)value;
            return (long)((v << count) | (v >> (64 - count)));
        }

		// ---------------------------------------------------------------------
		//  ROTATE RIGHT
		// ---------------------------------------------------------------------

		/// <summary>
		/// Rotates the bits of an 8‑bit value to the right.
		/// </summary>
		[Obsolete("please use Bit.BitIntSpan, Bit.BitLongSpan, Bit.BitUintSpan or Bit.BitULongSpan. This util will be remove in future release")]
		public static long RotateRight(this byte value, int count) {
            count &= 7;
            ulong v = (ulong)value;
            return (long)((v >> count) | (v << (8 - count)));
        }

		/// <summary>
		/// Rotates the bits of a 16‑bit value to the right.
		/// </summary>
		[Obsolete("please use Bit.BitIntSpan, Bit.BitLongSpan, Bit.BitUintSpan or Bit.BitULongSpan. This util will be remove in future release")]
		public static long RotateRight(this short value, int count) {
            count &= 15;
            ulong v = (ulong)value;
            return (long)((v >> count) | (v << (16 - count)));
        }

		/// <summary>
		/// Rotates the bits of a 32‑bit value to the right.
		/// </summary>
		[Obsolete("please use Bit.BitIntSpan, Bit.BitLongSpan, Bit.BitUintSpan or Bit.BitULongSpan. This util will be remove in future release")]
		public static int RotateRight(this int value, int count) {
            count &= 31;
            uint v = (uint)value;
            return (int)((v >> count) | (v << (32 - count)));
        }

		/// <summary>
		/// Rotates the bits of a 64‑bit value to the right.
		/// </summary>
		[Obsolete("please use Bit.BitIntSpan, Bit.BitLongSpan, Bit.BitUintSpan or Bit.BitULongSpan. This util will be remove in future release")]
		public static long RotateRight(this long value, int count) {
            count &= 63;
            ulong v = (ulong)value;
            return (long)((v >> count) | (v << (64 - count)));
        }

		// ---------------------------------------------------------------------
		//  FLIP BIT
		// ---------------------------------------------------------------------

		/// <summary>
		/// Toggles the bit at the specified position.
		/// </summary>
		[Obsolete("please use Bit.BitIntSpan, Bit.BitLongSpan, Bit.BitUintSpan or Bit.BitULongSpan. This util will be remove in future release")]
		public static byte FlipBit(this byte value, int pos)
            => (byte)(value ^ (1 << pos));

		/// <summary>
		/// Toggles the bit at the specified position.
		/// </summary>
		[Obsolete("please use Bit.BitIntSpan, Bit.BitLongSpan, Bit.BitUintSpan or Bit.BitULongSpan. This util will be remove in future release")]
		public static short FlipBit(this short value, int pos)
            => (short)(value ^ (1 << pos));

		/// <summary>
		/// Toggles the bit at the specified position.
		/// </summary>
		[Obsolete("please use Bit.BitIntSpan, Bit.BitLongSpan, Bit.BitUintSpan or Bit.BitULongSpan. This util will be remove in future release")]
		public static int FlipBit(this int value, int pos)
            => value ^ (1 << pos);

		/// <summary>
		/// Toggles the bit at the specified position.
		/// </summary>
		[Obsolete("please use Bit.BitIntSpan, Bit.BitLongSpan, Bit.BitUintSpan or Bit.BitULongSpan. This util will be remove in future release")]
		public static long FlipBit(this long value, int pos)
            => value ^ (1L << pos);

		/// <summary>
		/// Toggles the bit at the specified position.
		/// </summary>
		[Obsolete("please use Bit.BitIntSpan, Bit.BitLongSpan, Bit.BitUintSpan or Bit.BitULongSpan. This util will be remove in future release")]
		public static uint FlipBit(this uint value, int pos)
            => value ^ (1U << pos);

		/// <summary>
		/// Toggles the bit at the specified position.
		/// </summary>
		[Obsolete("please use Bit.BitIntSpan, Bit.BitLongSpan, Bit.BitUintSpan or Bit.BitULongSpan. This util will be remove in future release")]
		public static ushort FlipBit(this ushort value, int pos)
            => (ushort)(value ^ (1 << pos));

		/// <summary>
		/// Toggles the bit at the specified position.
		/// </summary>
		[Obsolete("please use Bit.BitIntSpan, Bit.BitLongSpan, Bit.BitUintSpan or Bit.BitULongSpan. This util will be remove in future release")]
		public static ulong FlipBit(this ulong value, int pos)
            => value ^ (1UL << pos);
    }
	
}
