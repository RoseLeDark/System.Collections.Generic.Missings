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

using System;
using System.Numerics;
using System.Text;
using SystemEx.Collections.Generic;


namespace SystemEx.Numeric {
	// \addtogroup SystemEx.Numeric
	/// @{
	/// <summary>
	/// Provides a low‑level, bit‑precise manipulation interface for any value
	/// that can be interpreted as a contiguous sequence of bits. Implementations
	/// expose direct access to individual bit positions, bit counting, rotation,
	/// and mask‑based operations without imposing a specific underlying numeric
	/// type.
	/// 
	/// <para>
	/// <see cref="IFastType"/> is designed for high‑performance bit operations
	/// where predictable behavior, zero‑allocation access, and explicit control
	/// over bit patterns are required. Typical use cases include:
	/// </para>
	/// 
	/// <list type="bullet">
	/// <item><description>flag sets and event masks</description></item>
	/// <item><description>embedded‑style bitfield logic</description></item>
	/// <item><description>numeric inspection (IEEE‑754 fields, custom formats)</description></item>
	/// <item><description>cryptographic or hashing primitives</description></item>
	/// <item><description>low‑level simulation or hardware‑style operations</description></item>
	/// </list>
	/// 
	/// <para>
	/// The interface is intentionally minimal and does not assume any particular
	/// storage type (integer, floating‑point, struct). Implementations define how
	/// the underlying bits are accessed and modified.
	/// </para>
	/// </summary>
	public interface IFastType {
		/// <summary>
		/// Gets the total number of bits exposed by the implementation.
		/// </summary>
		public byte Count { get; }

		/// <summary>
		/// Returns the bit at the specified position as <c>0</c> or <c>1</c>.
		/// </summary>
		/// <param name="pos">The bit position to inspect.</param>
		public byte Is ( byte pos ); // get
		/// <summary>
		/// Counts the number of bits set to <c>1</c> in the underlying value.
		/// </summary>
		public byte IsIt (); // welche sind 1
		/// <summary>
		/// Sets the bit at the specified position to <c>0</c> or <c>1</c>.
		/// </summary>
		/// <param name="pos">The bit position to modify.</param>
		/// <param name="value">The new bit value.</param>
		public void At ( byte pos, byte value ); // set

		/// <summary>
		/// Toggles (flips) the bit at the specified position.
		/// </summary>
		/// <param name="pos">The bit position to flip.</param>
		public void Flip ( byte pos ); // flippen

		/// <summary>
		/// Counts the number of bits set to <c>0</c> in the underlying value.
		/// Equivalent to <c>Count - IsIt()</c>.
		/// </summary>
		public byte IsItNot ();// welche sind 0

		/// <summary>
		/// Rotates all bits to the right by the specified count. Rotation wraps
		/// around the bit range and preserves all bits.
		/// </summary>
		/// <param name="count">The number of positions to rotate.</param>
		public void RotateRight ( byte count );

		/// <summary>
		/// Rotates all bits to the left by the specified count. Rotation wraps
		/// around the bit range and preserves all bits.
		/// </summary>
		/// <param name="count">The number of positions to rotate.</param>
		public void RotateLeft ( byte count );

		/// <summary>
		/// Returns a collection of all bit positions where the bit is <c>1</c>.
		/// </summary>
		public FixedVector<byte> Where ();

		/// <summary>
		/// Returns a collection of all bit positions where the bit is <c>0</c>.
		/// </summary>
		public FixedVector<byte> WhereNot ();

    }
	/// <summary>
	/// Extends <see cref="IFastType"/> with typed access to the underlying
	/// storage value. Implementations define how the raw value is represented
	/// (e.g., <see cref="uint"/>, <see cref="ulong"/>, <see cref="ushort"/>,
	/// <see cref="double"/>, <see cref="long"/>), enabling bit‑level inspection
	/// and mutation of arbitrary numeric formats.
	/// 
	/// <para>
	/// This interface is type‑agnostic and supports both integer and floating‑point
	/// representations. For floating‑point types, the bit operations apply to the
	/// raw IEEE‑754 encoding (or custom formats such as <c>Half16</c>).
	/// </para>
	/// 
	/// <para>
	/// Additional operations include:
	/// </para>
	/// 
	/// <list type="bullet">
	/// <item>
	/// <description>
	/// <b>One’s complement</b> (<see cref="CmpOne"/>) — invert all bits.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// <b>Two’s complement</b> (<see cref="CmpTwo"/>) — invert all bits and add one.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// <b>Masking</b> (<see cref="Mask"/>) — apply a bitmask using AND.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// <b>Mask creation</b> (<see cref="CreateMask"/>) — generate a contiguous
	/// bitmask of arbitrary length.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// <b>Combination</b> (<see cref="Combine"/>) — merge two bit patterns using OR.
	/// </description>
	/// </item>
	/// </list>
	/// 
	/// <para>
	/// Implementations must guarantee that all operations modify or inspect the
	/// underlying value deterministically and without hidden conversions.
	/// </para>
	/// </summary>
	/// <typeparam name="T">
	/// The underlying storage type (e.g., <see cref="uint"/>, <see cref="ulong"/>,
	/// <see cref="ushort"/>, <see cref="double"/>, <see cref="long"/>).
	/// </typeparam>
	public interface IFastType<T> : IFastType {

		/// <summary>
		/// Gets the raw underlying value represented by this bit‑manipulation type.
		/// </summary>
		public T Value { get; }

		/// <summary>
		/// Produces the one’s complement of the underlying value (bitwise NOT).
		/// </summary>
		public IFastType<T> CmpOne ();

		/// <summary>
		/// Produces the two’s complement of the underlying value
		/// (<c>~value + 1</c>).
		/// </summary>
		public IFastType<T> CmpTwo ();

		/// <summary>
		/// Applies a bitmask to the underlying value using bitwise AND.
		/// Only bits that are <c>1</c> in the mask remain set.
		/// </summary>
		/// <param name="mask">The mask to apply.</param>
		public void Mask ( T mask );

		/// <summary>
		/// Creates a contiguous bitmask beginning at <paramref name="start"/>
		/// with <paramref name="end"/> bits set to <c>1</c>.
		/// </summary>
		/// <param name="start">The starting bit position.</param>
		/// <param name="end">The number of bits to include.</param>
		/// <returns>A mask of type <typeparamref name="T"/>.</returns>
		public T CreateMask ( byte start, byte end );

		/// <summary>
		/// Combines the underlying value with another <see cref="IFastType{T}"/>
		/// using bitwise OR. All bits set in either value become set.
		/// </summary>
		/// <param name="other">The other bit‑type to combine with.</param>
		/// <returns>The modified instance.</returns>
		public IFastType<T> Combine ( IFastType<T> other );
	}

	//@}
}
