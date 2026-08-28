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
	/// Implements a CRC‑32 checksum calculator using a configurable reflected
	/// polynomial. Supports both rolling and one‑shot hashing modes while
	/// remaining fully compatible with the <see cref="IHash"/> pipeline.
	/// </summary>
	public class CRC32 : IHash {
		/// <summary>
		/// Stores the intermediate rolling CRC‑32 result when performing
		/// incremental updates.
		/// </summary>
		private uint m_rollingResult;

		/// <summary>
		/// Lookup table containing the 256 precomputed CRC‑32 values used
		/// for fast polynomial reduction.
		/// </summary>
		private uint[] m_table;

		private uint m_polynomial;

		/// <summary>
		/// Gets or sets the CRC‑32 polynomial used for table generation.
		/// Changing the polynomial regenerates the lookup table and resets
		/// the rolling CRC‑32 state.
		/// </summary>
		public uint Polynomial {
			get => m_polynomial;
			set { 
				m_polynomial = value;
				m_rollingResult = 0xFFFFFFFF; 
				generateTable(m_polynomial); 
			}
		}

		/// <summary>
		/// Gets the current rolling CRC‑32 state maintained across successive
		/// <see cref="Compute"/> calls. This value represents the intermediate
		/// checksum after all data processed so far.
		/// </summary>
		/// <remarks>
		/// The rolling state allows the CRC‑32 algorithm to operate in streaming
		/// mode without modifying the <see cref="IHash"/> pipeline interface.
		/// 
		/// If <see cref="Compute"/> is invoked with a seed value of zero, the
		/// computation continues from the existing rolling state. If a non‑zero
		/// seed is provided, the rolling state is replaced with the newly
		/// computed checksum.
		/// 
		/// This property exposes the internal CRC‑32 accumulator for diagnostic
		/// or pipeline‑level inspection, but does not alter hashing behavior.
		/// </remarks>
		public uint RollingState => m_rollingResult;

		/// <summary>
		/// Initializes a new instance of the <see cref="CRC32"/> class and
		/// generates the lookup table required for CRC‑32 computation.
		/// </summary>
		public CRC32 () { 
			m_rollingResult = 0;
			m_polynomial = 0xEDB88320;
			m_table = new uint[256];
			generateTable(0xEDB88320);
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CRC32"/> class and
		/// generates the lookup table required for CRC‑32 computation, 
		/// with custom polynomial.
		/// </summary>
		public CRC32 ( uint polynomial ) {
			m_rollingResult = 0;
			m_polynomial = polynomial;
			m_table = new uint[256];
			generateTable(polynomial);
		}

		/// <summary>
		/// Computes a CRC‑32 checksum over the specified input buffer.
		/// If a non‑zero seed is provided, the computation is performed as a
		/// one‑shot operation. If the seed is zero, the computation continues
		/// from the internal rolling CRC‑32 state.
		/// </summary>
		/// <remarks>
		/// This design allows CRC‑32 to operate in both streaming (rolling)
		/// and one‑shot modes without modifying the <see cref="IHash"/> pipeline.
		/// </remarks>
		public Hash32 Compute ( FixedVector<byte> input, uint seed ) {

			uint state = (seed == 0) ? m_rollingResult : seed;
			state = privateCompute(state, input);
			m_rollingResult = state;

			return new Hash32(state);
		}

		/// <summary>
		/// Computes a 64‑bit hash value. CRC‑32 does not naturally produce
		/// 64‑bit output, so this method is not implemented.
		/// </summary>
		/// <exception cref="NotImplementedException">
		/// Always thrown, as CRC‑32 is a 32‑bit algorithm.
		/// </exception>
		public Hash64 ComputeLong ( FixedVector<byte> input, ulong seed ) {
			throw new NotImplementedException();
		}

		/// <summary>
		/// Updates the CRC‑32 value using the specified initial state and
		/// input buffer. This method performs the core CRC‑32 computation
		/// using the lookup table generated during construction.
		/// </summary>
		/// <param name="initial">The initial CRC value.</param>
		/// <param name="buffer">The input data to process.</param>
		/// <returns>The final CRC‑32 checksum.</returns>
		private unsafe uint privateCompute ( uint initial, FixedVector<byte> buffer ) {
			

			uint c = initial ^ 0xFFFFFFFF;

			for (long i = 0; i < buffer.Length; ++i) {
				c = m_table[(c ^ buffer[i]) & 0xFF] ^ (c >> 8);
			}
				return c ^ 0xFFFFFFFF;
		}

		/// <summary>
		/// Generates the CRC‑32 lookup table using the standard polynomial
		/// 0xEDB88320. The table contains 256 entries corresponding to all
		/// possible byte values and is used to accelerate CRC computation.
		/// </summary>
		private void generateTable ( uint polynomial ) {
		
			for ( uint i = 0 ; i < 256 ; i++ ) {
				uint c = i;
				for ( ulong j = 0 ; j < 8 ; j++ ) {
					if ( (c & 1) == 1 )
						c = polynomial ^ (c >> 1);
					else
						c >>= 1;
				}
				m_table[i] = c;
			}
		}
	}

	/// <summary>
	/// Implements the standard CRC‑32 (IEEE 802.3, PKZip, PNG) checksum algorithm.
	/// Polynomial: 0xEDB88320 (reflected).
	/// </summary>
	public sealed class CRC32IEEE : CRC32 {

		/// <summary>
		/// Initializes a new instance of the <see cref="CRC32IEEE "/> class using
		/// the standard CRC‑32C polynomial.
		/// </summary>
		public CRC32IEEE () : base(0xEDB88320U) { }
	}

	/// <summary>
	/// Implements the CRC‑32C (Castagnoli) checksum algorithm.
	/// Polynomial: 0x82F63B78 (reflected).
	/// </summary>
	public sealed class CRC32C : CRC32 {
		/// <summary>
		/// Initializes a new instance of the <see cref="CRC32C"/> class using
		/// the standard CRC‑32C polynomial.
		/// </summary>
		public CRC32C () : base(0x82F63B78U) { }
	}

	/// <summary>
	/// Implements the CRC‑32K (Koopman) checksum algorithm.
	/// Polynomial: 0xEB31D82E (reflected).
	/// </summary>
	public sealed class CRC32Koopman : CRC32 {
		/// <summary>
		/// Initializes a new instance of the <see cref="CRC32Koopman"/> class using
		/// the standard CRC‑32C polynomial.
		/// </summary>
		public CRC32Koopman () : base(0xEB31D82EU) { }
	}

	/// <summary>
	/// Implements the CRC‑32/BZIP2 checksum algorithm.
	/// Polynomial: 0x04C11DB7 (normal).
	/// </summary>
	public sealed class CRC32BZip2 : CRC32 {
		/// <summary>
		/// Initializes a new instance of the <see cref="CRC32BZip2 "/> class using
		/// the standard CRC‑32C polynomial.
		/// </summary>
		public CRC32BZip2 () : base(0x04C11DB7U) { }
	}

	/// <summary>
	/// Implements the CRC‑32/MPEG‑2 checksum algorithm.
	/// Polynomial: 0x04C11DB7 (normal), Final XOR = 0.
	/// </summary>
	public sealed class CRC32Mpeg2 : CRC32 {
		/// <summary>
		/// Initializes a new instance of the <see cref="CRC32Mpeg2"/> class using
		/// the standard CRC‑32C polynomial.
		/// </summary>
		public CRC32Mpeg2 () : base(0x04C11DB7U) { }
	}

	/// <summary>
	/// Implements the CRC‑32/POSIX checksum algorithm.
	/// Polynomial: 0x04C11DB7 (normal), Initial XOR = 0.
	/// </summary>
	public sealed class CRC32Posix : CRC32 {
		/// <summary>
		/// Initializes a new instance of the <see cref="CRC32Posix"/> class using
		/// the standard CRC‑32C polynomial.
		/// </summary>
		public CRC32Posix () : base(0x04C11DB7U) { }
	}


}
