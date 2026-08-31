using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Numeric {
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


	using System.Runtime.CompilerServices;
	using global::SystemEx.Collections.Generic;
	using global::SystemEx.Utils;


	namespace SystemEx.Numeric {
		/// \addtogroup Numeric
		/// @{

		/// <summary>
		/// Represents an 16‑bit fast bit‑manipulation type. 
		/// This struct provides low‑level operations for inspecting, modifying,
		/// rotating, masking, and counting bits inside a single int.
		/// 
		/// Fast_Int is intended for systems that require precise bit control,
		/// such as event groups, flag sets, embedded‑style logic, or any 
		/// performance‑critical bitmask operations. 
		/// 
		/// Users must understand bitwise operations, as incorrect usage can 
		/// intentionally overwrite or corrupt the underlying value.
		/// </summary>
		public struct Fast_Int  : IFastType<int>, IComparable<Fast_Int>, IComparableEx<Fast_Int>, IEquatable<Fast_Int> {
			private int m_value;
			private byte m_size;

			/// <summary>
			/// Gets the number of bits available in this type (always 32).
			/// </summary>
			public byte Count => m_size;

			/// <summary>
			/// Gets the raw underlying 32‑bit unsigned integer value.
			/// </summary>

			public int Value => m_value;

			/// <summary>
			/// Is signed number
			/// </summary>
			public bool IsSigned => true;
			/// <summary>
			/// Is Zero
			/// </summary>
			public bool IsZero => m_value == 0;

			/// <summary>
			/// Initializes a new Fast_Int instance with an optional initial value.
			/// </summary>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Fast_Int  () : this(0) { }

			/// <summary>
			/// Initializes a new Fast_Int instance with an optional initial value.
			/// </summary>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Fast_Int  ( int value ) {
				m_value = value;
				m_size = sizeof(int) * 8;
			}
			/// <summary>
			/// Sets the bit at the specified position to the given value (0 or 1).
			/// The bit is only modified if the new value differs from the current one.
			/// This avoids unnecessary writes and preserves performance.
			/// </summary>
			/// <param name="pos">Bit position (0–16).</param>
			/// <param name="value">New bit value (0 or 1).</param>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void At ( byte pos, byte value ) {
				byte current = (byte)((m_value  >> pos) & 1U) ;

				if ( current != value ) {
					m_value = (value == 1) ? (m_value | 1 << pos) : (m_value & ~(1 << pos));
				}
			}
			/// <summary>
			/// Produces the one's complement of the current value.
			/// All bits are inverted (bitwise NOT).
			/// </summary>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public IFastType<int> CmpOne () => new Fast_Int ((int)~m_value);
			/// <summary>
			/// Produces the two's complement of the current value.
			/// This is equivalent to (~value + 1) and is commonly used
			/// for subtraction in low‑level arithmetic.
			/// </summary>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public IFastType<int> CmpTwo () => new Fast_Int ((int)(~m_value + 1));

			/// <summary>
			/// Flips (toggles) the bit at the specified position.
			/// </summary>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Flip ( byte pos ) {
				m_value = (int)(m_value ^ (1 << pos));
			}

			/// <summary>
			/// Returns the bit at the specified position (0 or 1).
			/// </summary>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public byte Is ( byte pos ) => (byte)((m_value >> pos) & 1);

			/// <summary>
			/// Applies a bitmask to the current value using bitwise AND.
			/// Only bits that are 1 in the mask remain set.
			/// </summary>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Mask ( int mask ) {
				int v = m_value;
				m_value = (int)(v & mask);
			}


			/// <summary>
			/// Rotates the bits to the left by the specified count.
			/// Rotation is limited to 0–31 to ensure correct 32‑bit behavior.
			/// </summary>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void RotateLeft ( byte count ) {
				count &= (byte)(Count - 1);
				ulong v = (ulong)m_value;
				m_value = (int)((v << count) | (v >> (Count - count)));
			}

			/// <summary>
			/// Rotates the bits to the right by the specified count.
			/// Rotation is limited to 0–15 to ensure correct 32‑bit behavior.
			/// </summary>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void RotateRight ( byte count ) {
				count &= (byte)(Count - 1);
				ulong v = (ulong)m_value;
				m_value = (int)((v >> count) | (v << (Count - count)));
			}

			/// <summary>
			/// Creates a bitmask with a given start position and length.
			/// The mask contains 'length' consecutive 1‑bits beginning at 'start'.
			/// </summary>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public int CreateMask ( byte start, byte length ) {
				if ( length <= 0 ) return 0;
				if ( start < 0 || start > (Count - 1) ) return 0;
				if ( length >= Count ) return unchecked((int)0xFFFFFFFF);

				int mask = (1 << length) - 1;
				return (int)(mask << start);
			}

			/// <summary>
			/// Combines this value with another Fast_Int using bitwise OR.
			/// All bits that are set in either value become set.
			/// </summary>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public IFastType<int> Combine ( IFastType<int> other ) {
				m_value = (int)(m_value | other.Value);
				return this;
			}

			/// <summary>
			/// Counts the number of bits set to 1 in a 32‑bit unsigned integer.
			/// Uses a branchless parallel bit‑count algorithm ("Hacker's Delight").
			/// This avoids loops and provides excellent performance.
			/// </summary>
			public byte IsIt () {
				int v = m_value;

				v = v - ((v >> 1) & 0x55555555);

				v = (v & 0x33333333) + ((v >> 2) & 0x33333333);

				v = (v + (v >> 4)) & 0x0F0F0F0F;

				v = v + (v >> 8);

				v = v + (v >> 16);

				return (byte)(v & 0x3F);
			}



			/// <summary>
			/// Counts the number of bits set to 0.
			/// Equivalent to Count - IsIt().
			/// </summary>
			public byte IsItNot () => (byte)(Count - IsIt());

			/// <summary>
			/// Returns all bit positions where the bit is set to 1.
			/// </summary>
			public FixedVector<byte> Where () {
				var _size = IsIt();

				FixedVector<byte> _set = new FixedVector<byte>();

				if ( _size != 0 ) {

					for ( byte i = 0 ; i < Count ; i++ ) {
						if ( Is(i) == 1 ) { _set.PushBack(i); }
					}
				}
				return _set;
			}

			/// <summary>
			/// Returns all bit positions where the bit is set to 0.
			/// </summary>
			public FixedVector<byte> WhereNot () {
				var _size = IsItNot();

				FixedVector<byte> _set = new FixedVector<byte>();

				if ( _size != 0 ) {

					for ( byte i = 0 ; i < Count ; i++ ) {
						if ( Is(i) == 0 ) { _set.PushBack(i); }
					}
				}
				return _set;
			}


			/// <summary>
			/// Addition
			/// </summary>
			public Fast_Int  Add ( Fast_Int  oth ) {
				m_value += oth.m_value; return this;
			}
			/// <summary>
			/// Subtraktion
			/// </summary>
			public Fast_Int  Sub ( Fast_Int  oth ) {
				m_value -= oth.m_value; return this;
			}
			/// <summary>
			/// Multiplikation
			/// </summary>
			public Fast_Int  Mul ( Fast_Int  oth ) {
				m_value *= oth.m_value; return this;
			}
			/// <summary>
			/// Division
			/// </summary>
			public Pair<Fast_Int , Fast_Int > Div ( Fast_Int  oth ) {
				m_value /= oth.m_value;
				byte v = (byte)(m_value % oth.m_value);

				return new(this, new(v));
			}

			public CompareResult CompareTo ( Fast_Int  a ) {
				if ( m_value > a.m_value ) return CompareResult.Greater;
				else if ( m_value < a.m_value ) return CompareResult.Less;

				return CompareResult.Equal;
			}
			int IComparable<Fast_Int >.CompareTo ( Fast_Int  a ) {
				return (int)CompareTo(a);
			}

			public static Fast_Int  Min ( Fast_Int  a, Fast_Int  b ) {
				return a.m_value < b.m_value ? a : b;
			}
			public static Fast_Int  Max ( Fast_Int  a, Fast_Int  b ) {
				return a.m_value > b.m_value ? a : b;
			}

			/// <inheritdoc/>
			public static bool operator == ( Fast_Int  a, Fast_Int  b ) {
				return a.m_value == b.m_value;
			}

			/// <inheritdoc/>
			public static bool operator != ( Fast_Int  a, Fast_Int  b ) {
				return !(a == b);
			}

			/// <inheritdoc/>
			public static bool operator <= ( Fast_Int  a, Fast_Int  b ) {
				return a.m_value <= b.m_value;
			}
			/// <inheritdoc/>
			public static bool operator >= ( Fast_Int  a, Fast_Int  b ) {
				return a.m_value >= b.m_value;
			}
			/// <inheritdoc/>
			public static bool operator < ( Fast_Int  a, Fast_Int  b ) {
				return a.m_value < b.m_value;
			}
			/// <inheritdoc/>
			public static bool operator > ( Fast_Int  a, Fast_Int  b ) {
				return a.m_value > b.m_value;
			}

			public static Fast_Int  operator + ( Fast_Int  a, Fast_Int  b ) {
				a.m_value += b.m_value;
				return a;
			}
			public static Fast_Int  operator - ( Fast_Int  a, Fast_Int  b ) {
				a.m_value -= b.m_value;
				return a;
			}
			public static Fast_Int  operator - ( Fast_Int  a ) {
				a.m_value = (byte)(-a.m_value);
				return a;
			}
			public static Fast_Int  operator -- ( Fast_Int  a ) {
				a.m_value--;
				return a;
			}
			public static Fast_Int  operator ++ ( Fast_Int  a ) {
				a.m_value++;
				return a;
			}
			public static Fast_Int  operator * ( Fast_Int  a, Fast_Int  b ) {
				a.m_value *= b.m_value;
				return a;
			}
			public static Fast_Int  operator / ( Fast_Int  a, Fast_Int  b ) {
				a.m_value /= b.m_value;
				return a;
			}


			public static implicit operator Fast_Int  ( Fast_Byte value ) {
				return new Fast_Int (value.Value);
			}
			public static implicit operator Fast_Int  ( Fast_UShort value ) {
				return new Fast_Int (value.Value);
			}
			public static implicit operator Fast_Int ( Fast_UInt value ) {
				return new Fast_Int((int)value.Value);
			}

			/// <summary>
			/// Implicitly converts a int value into an <see cref="Fast_Int  "/>.
			/// </summary>
			/// <param name="value">The int value to convert.</param>
			public static implicit operator Fast_Int  ( int value ) {
				return new Fast_Int ((int)value);
			}


			/// <summary>
			/// Explicitly extracts the underlying value from an <see cref="Fast_Int  "/>.
			/// </summary>
			/// <param name="opt">The optional instance to extract from.</param>
			/// <returns>The stored value.</returns>
			public static explicit operator int ( Fast_Int  opt ) {
				return opt.m_value;
			}

			public static explicit operator Fast_Byte ( Fast_Int  opt ) {
				return opt.m_value;
			}

			public static explicit operator Fast_UShort ( Fast_Int  opt ) {
				return opt.m_value;
			}

			/// <inheritdoc/>
			public override bool Equals ( object? obj ) {
				if ( obj is Fast_Int  key )
					return Equals(key);
				return false;
			}

			/// <inheritdoc/>
			public bool Equals ( Fast_Int  b ) {
				return this.m_value == b.m_value;
			}

			public bool Equals ( ushort b ) {
				return this.m_value == b;
			}

			/// <inheritdoc/>
			public override int GetHashCode () {
				return m_value.GetHashCode();
			}
			/// <inheritdoc/>
			public override string ToString () {
				return m_value.ToString();
			}

			public static Fast_Int  operator & ( Fast_Int  a, Fast_Int  b ) {
				return (int)(a.m_value & b.m_value);
			}
			public static Fast_Int  operator % ( Fast_Int  a, Fast_Int  b ) {
				return (int)(a.m_value % b.m_value);
			}
			public static Fast_Int  operator | ( Fast_Int  a, Fast_Int  b ) {
				return (int)(a.m_value | b.m_value);
			}
			public static Fast_Int  operator ^ ( Fast_Int  a, Fast_Int  b ) {
				return (int)(a.m_value ^ b.m_value);
			}

			public static Fast_Int  operator << ( Fast_Int  a, Fast_Int  b ) {
				a.m_value = (int)(a.m_value << (int)b.m_value);
				return a;
			}
			public static Fast_Int  operator >> ( Fast_Int  a, Fast_Int  b ) {
				a.m_value = (int)(a.m_value >> (int)b.m_value);
				return a;
			}
		}

	}

}
