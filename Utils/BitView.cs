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
	// \addtogroup SystemEx.Utils
	/// @{
	/// <summary>
	/// Provides factory methods for creating bit‑level span views over primitive
	/// integer types. Each method returns a windowed, mutable bit span that
	/// exposes the underlying value without copying it.
	/// 
	/// <para>
	/// <see cref="BitView"/> acts as the entry point for SystemEx’s bit‑span
	/// subsystem. It constructs the appropriate <c>Bit*Span</c> type based on the
	/// referenced integer (<see cref="int"/>, <see cref="uint"/>,
	/// <see cref="long"/>, <see cref="ulong"/>), allowing callers to treat any
	/// numeric value as a mutable bit array.
	/// </para>
	/// 
	/// <para>
	/// All returned spans:
	/// <list type="bullet">
	/// <item>
	/// <description>
	/// operate directly on the referenced integer using <c>ref</c> semantics;
	/// changes to the span immediately modify the original value.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// expose only the bits between <paramref name="start"/> and
	/// <paramref name="send"/> (exclusive), forming a windowed view.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// support multiple indexing modes via <see cref="FlexSpanMode"/>:
	/// System (forward), Reverse (backward), and Ring (cyclic).
	/// </description>
	/// </item>
	/// </list>
	/// </para>
	/// 
	/// <para>
	/// <b>Warning:</b> Bit spans do not perform bounds normalization or safety
	/// checks beyond basic range validation. Incorrect window definitions or
	/// ring‑mode enumeration may lead to unexpected behavior.
	/// </para>
	/// </summary>
	public static class BitView {
		/// <summary>
		/// Creates a bit‑span view over a referenced <see cref="int"/> value.
		/// 
		/// <para>
		/// The returned <see cref="BitIntSpan"/> exposes the bits in the range
		/// <c>[start, send)</c> using the specified indexing mode. All mutations
		/// performed through the span directly modify the referenced integer.
		/// </para>
		/// </summary>
		/// <param name="value">The referenced integer whose bits are exposed.</param>
		/// <param name="mode">The indexing mode used for bit access.</param>
		/// <param name="start">The first bit index of the window.</param>
		/// <param name="send">The exclusive end bit index of the window.</param>
		/// <returns>A new <see cref="BitIntSpan"/> referencing <paramref name="value"/>.</returns>
		public static BitIntSpan AsFlexSpan ( ref int value, FlexSpanMode mode, short start = 0, short send = 32 ) {
            return new BitIntSpan(ref value, start, send, mode);
        }

		/// <summary>
		/// Creates a bit‑span view over a referenced <see cref="uint"/> value.
		/// 
		/// <para>
		/// The returned <see cref="BitUIntSpan"/> exposes the bits in the range
		/// <c>[start, send)</c> using the specified indexing mode. All mutations
		/// performed through the span directly modify the referenced integer.
		/// </para>
		/// </summary>
		/// <param name="value">The referenced unsigned integer whose bits are exposed.</param>
		/// <param name="mode">The indexing mode used for bit access.</param>
		/// <param name="start">The first bit index of the window.</param>
		/// <param name="send">The exclusive end bit index of the window.</param>
		/// <returns>A new <see cref="BitUIntSpan"/> referencing <paramref name="value"/>.</returns>
		public static BitUIntSpan AsFlexSpan ( ref uint value, FlexSpanMode mode, short start = 0, short send = 32 ) {
            return new BitUIntSpan(ref value, start, send, mode);
        }

		/// <summary>
		/// Creates a bit‑span view over a referenced <see cref="long"/> value.
		/// 
		/// <para>
		/// The returned <see cref="BitLongSpan"/> exposes the bits in the range
		/// <c>[start, send)</c> using the specified indexing mode. All mutations
		/// performed through the span directly modify the referenced integer.
		/// </para>
		/// </summary>
		/// <param name="value">The referenced 64‑bit integer whose bits are exposed.</param>
		/// <param name="mode">The indexing mode used for bit access.</param>
		/// <param name="start">The first bit index of the window.</param>
		/// <param name="send">The exclusive end bit index of the window.</param>
		/// <returns>A new <see cref="BitLongSpan"/> referencing <paramref name="value"/>.</returns>
		public static BitLongSpan AsFlexSpan ( ref long value, FlexSpanMode mode, short start = 0, short send = 64 ) {
            return new BitLongSpan(ref value, start, send, mode);
        }
		/// <summary>
		/// Creates a bit‑span view over a referenced <see cref="ulong"/> value.
		/// 
		/// <para>
		/// The returned <see cref="BitULongSpan"/> exposes the bits in the range
		/// <c>[start, send)</c> using the specified indexing mode. All mutations
		/// performed through the span directly modify the referenced integer.
		/// </para>
		/// </summary>
		/// <param name="value">The referenced unsigned 64‑bit integer whose bits are exposed.</param>
		/// <param name="mode">The indexing mode used for bit access.</param>
		/// <param name="start">The first bit index of the window.</param>
		/// <param name="send">The exclusive end bit index of the window.</param>
		/// <returns>A new <see cref="BitULongSpan"/> referencing <paramref name="value"/>.</returns>
		public static BitULongSpan AsFlexSpan ( ref ulong value, FlexSpanMode mode, short start = 0, short send = 64 ) {
            return new BitULongSpan(ref value, start, send, mode);
        }
    }
    //@}
}
