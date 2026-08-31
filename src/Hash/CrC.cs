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
	/// Provides a generic wrapper combining a CRC‑32 and a CRC‑64 hashing
	/// implementation. The generic parameters <typeparamref name="TC32"/> and
	/// <typeparamref name="TC64"/> specify the concrete CRC‑32 and CRC‑64
	/// algorithm variants to use, allowing flexible configuration of
	/// polynomial families without modifying the pipeline.
	/// </summary>
	/// <typeparam name="TC32">
	/// A concrete <see cref="CRC32"/> implementation used for 32‑bit hashing.
	/// Must provide a parameterless constructor.
	/// </typeparam>
	/// <typeparam name="TC64">
	/// A concrete <see cref="CRC64"/> implementation used for 64‑bit hashing.
	/// Must provide a parameterless constructor.
	/// </typeparam>
	public class CrC<TC32, TC64> : IHash 
		where TC32 : CRC32, new() 
		where TC64 : CRC64, new()  {

		private TC32 m_crc32;
		private TC64 m_crc64;


		/// <summary>
		/// Gets or sets the CRC‑32 and CRC‑64 polynomials simultaneously.
		/// The first element of the pair corresponds to the CRC‑32 polynomial,
		/// and the second element corresponds to the CRC‑64 polynomial.
		///
		/// <para>
		/// If an element is <see cref="Optional{T}.HasValue"/> = <c>false</c>,
		/// the corresponding polynomial remains unchanged.
		/// </para>
		/// </summary>
		/// <remarks>
		/// Setting either polynomial regenerates the lookup table of the
		/// underlying CRC implementation and resets its rolling state.
		/// </remarks>
		public Pair<Optional<uint>, Optional<ulong>> Polynomial {
			set {
				var firs = value.First;
				var sec = value.Second;

				if ( firs.HasValue ) m_crc32.Polynomial = firs.Value!;
				if ( sec.HasValue ) m_crc64.Polynomial = sec.Value!;
			}
			get =>  new(m_crc32.Polynomial, m_crc64.Polynomial);
		}

		/// <summary>
		/// Initializes a new <see cref="CrC{TC32, TC64}"/> instance using the
		/// default CRC‑32 and CRC‑64 polynomials provided by the concrete
		/// algorithm types <typeparamref name="TC32"/> and <typeparamref name="TC64"/>.
		/// </summary>
		public CrC () {
			m_crc32 = new TC32();
			m_crc64 = new TC64();
		}

		/// <summary>
		/// Initializes a new <see cref="CrC{TC32, TC64}"/> instance using optional
		/// custom CRC‑32 and CRC‑64 polynomials. If a polynomial is not provided,
		/// the corresponding CRC implementation uses its default polynomial.
		/// </summary>
		/// <param name="poly">
		/// A pair containing optional CRC‑32 and CRC‑64 polynomial values.
		/// </param>
		public CrC ( Pair<Optional<uint>, Optional<ulong>> poly) {
			var firs = poly.First;
			var sec = poly.Second;

			m_crc32 = new(); 
			m_crc64 = new();
			
			if( (firs.HasValue) ) m_crc32.Polynomial = firs.Value!;
			if( (sec.HasValue) ) m_crc64.Polynomial = sec.Value!;
		}

		/// <summary>
		/// Computes a CRC‑32 checksum over the specified input buffer by
		/// delegating to the internal <typeparamref name="TC32"/> instance.
		/// </summary>
		/// <param name="input">The input byte sequence to hash.</param>
		/// <param name="seed">
		/// The initial CRC‑32 seed. A value of zero continues from the rolling
		/// CRC‑32 state; a non‑zero value performs a one‑shot computation.
		/// </param>
		/// <returns>A <see cref="Hash32"/> containing the CRC‑32 checksum.</returns>
		public Hash32 Compute ( FixedVector<byte> input, uint seed ) {
			return m_crc32.Compute(input, seed);
		}

		/// <summary>
		/// Computes a CRC‑64 checksum over the specified input buffer by
		/// delegating to the internal <typeparamref name="TC64"/> instance.
		/// </summary>
		/// <param name="input">The input byte sequence to hash.</param>
		/// <param name="seed">
		/// The initial CRC‑64 seed. A value of zero continues from the rolling
		/// CRC‑64 state; a non‑zero value performs a one‑shot computation.
		/// </param>
		public Hash64 ComputeLong ( FixedVector<byte> input, ulong seed ) {
			return m_crc64.ComputeLong(input, seed);
		}
	}
}
