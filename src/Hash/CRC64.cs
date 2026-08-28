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
	/// Implements a CRC‑64 checksum calculator using a configurable reflected
	/// polynomial. This class supports both rolling and one‑shot hashing modes
	/// while remaining fully compatible with the <see cref="IHash"/> pipeline.
	/// </summary>
	public class CRC64 : IHash {
		/// <summary>
		/// Stores the intermediate rolling CRC‑64 result across successive
		/// <see cref="ComputeLong"/> calls.
		/// </summary>
		private ulong m_rollingResult;

		/// <summary>
		/// Lookup table containing 256 precomputed CRC‑64 values used for
		/// fast polynomial reduction during hashing.
		/// </summary>
		private ulong[] m_table;
		/// <summary>
		/// The reflected CRC‑64 polynomial used for lookup‑table generation.
		/// </summary>
		private ulong m_polynomial;

		/// <summary>
		/// Gets or sets the CRC‑64 polynomial used for table generation.
		/// Changing the polynomial regenerates the lookup table and resets
		/// the rolling CRC‑64 state.
		/// </summary>
		public ulong Polynomial {
			get => m_polynomial;
			set {
				m_polynomial = value;
				m_rollingResult = 0UL;
				generateTable(m_polynomial);
			}
		}

		/// <summary>
		/// Gets the current rolling CRC‑64 state maintained across successive
		/// <see cref="ComputeLong"/> calls. This value represents the intermediate
		/// checksum after all data processed so far.
		/// </summary>
		/// <remarks>
		/// The rolling state allows CRC‑64 to operate in streaming mode without
		/// modifying the <see cref="IHash"/> pipeline interface.
		///
		/// If <see cref="ComputeLong"/> is invoked with a seed value of zero,
		/// the computation continues from the existing rolling state. If a
		/// non‑zero seed is provided, the rolling state is replaced with the
		/// newly computed checksum.
		///
		/// This property exposes the internal CRC‑64 accumulator for diagnostic
		/// or pipeline‑level inspection, but does not alter hashing behavior.
		/// </remarks>
		public ulong RollingState => m_rollingResult;

		/// <summary>
		/// Initializes a new instance of the <see cref="CRC64"/> class using the
		/// standard reflected ECMA‑182 polynomial (0xC96C5795D7870F42).
		/// </summary>
		public CRC64 () {
			m_rollingResult = 0;
			m_table = new ulong[256];
			m_polynomial = 0xC96C5795D7870F42UL;
			generateTable(0xC96C5795D7870F42UL);

		}
		/// <summary>
		/// Initializes a new instance of the <see cref="CRC64"/> class using a
		/// custom reflected CRC‑64 polynomial.
		/// </summary>
		public CRC64 ( ulong polynomial ) {
			m_rollingResult = 0;
			m_table = new ulong[256];
			m_polynomial = polynomial;
			generateTable(polynomial);

		}

		/// <summary>
		/// Computes a 32‑bit hash value. CRC‑64 does not produce 32‑bit output,
		/// therefore this method is not implemented.
		/// </summary>
		/// <exception cref="NotImplementedException">
		/// Always thrown, as CRC‑64 is a 64‑bit algorithm.
		/// </exception>
		public Hash32 Compute ( FixedVector<byte> input, uint seed ) {
			throw new NotImplementedException();
			
		}

		/// <summary>
		/// Computes a CRC‑64 checksum over the specified input buffer.
		/// If a non‑zero seed is provided, the computation is performed as a
		/// one‑shot operation. If the seed is zero, the computation continues
		/// from the internal rolling CRC‑64 state.
		/// </summary>
		/// <remarks>
		/// This design allows CRC‑64 to operate in both streaming (rolling)
		/// and one‑shot modes without modifying the <see cref="IHash"/> pipeline.
		/// </remarks>
		public Hash64 ComputeLong ( FixedVector<byte> input, ulong seed ) {
			ulong state = (seed == 0) ? m_rollingResult : seed;
			state = privateCompute(state, input);
			m_rollingResult = state;

			return new Hash64(state);
		}

		/// <summary>
		/// Performs the core CRC‑64 update operation using the specified initial
		/// state and input buffer. This method applies the reflected CRC‑64
		/// polynomial and uses the lookup table for efficient reduction.
		/// </summary>
		/// <param name="initial">The initial CRC‑64 state.</param>
		/// <param name="buffer">The input data to process.</param>
		/// <returns>The updated CRC‑64 checksum.</returns>
		private unsafe ulong privateCompute ( ulong initial, FixedVector<byte> buffer ) {
			ulong c = initial;

			for ( long j = 0 ; j < buffer.Length ; j++ ) {
				c = m_table[c ^ buffer[j] ] ^ (c >> 8);
			}
			return c;
		}

		/// <summary>
		/// Generates the CRC‑64 lookup table using the specified reflected polynomial.
		/// The table contains 256 entries corresponding to all possible byte values
		/// and is used to accelerate CRC‑64 computation.
		/// </summary>
		private void generateTable ( ulong polynomial ) {
			m_table = new ulong[256];

			for ( ulong i = 0 ; i < 256 ; i++ ) {
				ulong c = i;

				for ( int j = 0 ; j < 8 ; j++ ) {
					if ( (c & 1UL) != 0 )
						c = polynomial ^ (c >> 1);
					else
						c >>= 1;
				}

				m_table[i] = c;
			}
		}

		/// <summary>
		/// Implements the CRC‑64/ECMA‑182 checksum algorithm.
		/// Polynomial: 0xC96C5795D7870F42 (reflected).
		/// </summary>
		public sealed class CRC64Ecma : CRC64 {
			/// <summary>
			/// Initializes a new instance of the <see cref="CRC64Ecma"/> class using
			/// the CRC‑64/ISO polynomial.
			/// </summary>
			public CRC64Ecma () : base(0xC96C5795D7870F42UL) { }
		}

		/// <summary>
		/// Implements the CRC‑64/ISO checksum algorithm.
		/// Polynomial: 0x000000000000001B (normal).
		/// </summary>
		public sealed class CRC64Iso : CRC64 {
			/// <summary>
			/// Initializes a new instance of the <see cref="CRC64Iso"/> class using
			/// the CRC‑64/ISO polynomial.
			/// </summary>
			public CRC64Iso () : base(0x000000000000001BUL) { }
		}

		/// <summary>
		/// Implements the CRC‑64/WE (Weiner) checksum algorithm.
		/// Polynomial: 0x42F0E1EBA9EA3693 (reflected).
		/// </summary>
		public sealed class CRC64We : CRC64 {
			/// <summary>
			/// Initializes a new instance of the <see cref="CRC64We"/> class using
			/// the CRC‑64/ISO polynomial.
			/// </summary>
			public CRC64We () : base(0x42F0E1EBA9EA3693UL) { }
		}

		/// <summary>
		/// Implements the CRC‑64/XZ checksum algorithm (used in LZMA/XZ).
		/// Polynomial: 0x42F0E1EBA9EA3693 (reflected).
		/// </summary>
		public sealed class CRC64Xz : CRC64 {
			/// <summary>
			/// Initializes a new instance of the <see cref="CRC64Xz"/> class using
			/// the CRC‑64/ISO polynomial.
			/// </summary>
			public CRC64Xz () : base(0x42F0E1EBA9EA3693UL) { }
		}

	}
}
