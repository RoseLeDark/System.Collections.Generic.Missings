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
 * 
 * Based on the original MT19937 implementation by
 * Makoto Matsumoto and Takuji Nishimura (1997–2002),
 * distributed under the BSD 3‑Clause license.
 * 
 * Copyright (C) 1997–2002
 * Makoto Matsumoto and Takuji Nishimura.
 * All rights reserved.
 *  
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 1. Redistributions of source code must retain this notice.
 * 2. Redistributions in binary form must reproduce this notice in the documentation.
 * 3. The names of the contributors may not be used to endorse or promote products.
 * 
 * THIS SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND.
 */

namespace SystemEx.Rand.Engine {
	/// <summary>
	/// Represents the seed container used by the <see cref="MTwisterEngine"/>.
	/// The seed consists of five 32-bit values: the primary user-provided seed
	/// and four additional system-derived values to increase initialization entropy.
	/// </summary>
	public struct MTwisterEngineSeed : ISeed {
		private uint[] m_seed;
		/// <summary>
		/// Gets or sets the seed value at the specified index.
		/// </summary>
		/// <param name="i">The index of the seed value.</param>
		/// <returns>The seed value at the specified index.</returns>
		public uint this[int i] { get => m_seed[i]; set => m_seed[i] = value; }
		/// <summary>
		/// Gets the number of seed values stored in this seed container.
		/// </summary>
		public int Length => m_seed.Length;
		/// <summary>
		/// Returns the underlying array of seed values.
		/// </summary>
		/// <returns>An array containing all seed values.</returns>
		public uint[] GetSeed () {
			return m_seed;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="MTwisterEngineSeed"/> struct
		/// using the default MT19937 seed value (19650218).
		/// </summary>
		public MTwisterEngineSeed () 
			: this(19650218U) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="MTwisterEngineSeed"/> struct
		/// using the specified primary seed value. Additional system-derived values
		/// are included to enrich the initialization state.
		/// </summary>
		/// <param name="seed">The primary 32-bit seed value.</param>
		public MTwisterEngineSeed ( uint seed ) {

			m_seed = new uint[] { seed, 
				(uint)DateTime.Now.Ticks & 0xffffffffU,
				(uint)Thread.CurrentThread.ManagedThreadId, 
				Framework.iVersion,
				(uint)Environment.TickCount };
		}
	}

	// <summary>
	/// Implements the MT19937 32-bit Mersenne Twister random number generator.
	/// Produces a deterministic sequence of 32-bit values based on the provided seed.
	/// </summary>
	public sealed class MTwisterEngine : IRandomEngine {
		private ISeed m_startSeed;
		
		
		private UInt32[] m_vecState; 
		private Int16 m_usT;
		private UInt32[] m_uiMag;

		/// <summary>
		/// Gets the seed object used to initialize this engine.
		/// </summary>
		public ISeed StartSeed => m_startSeed;
		/// <summary>
		/// Initializes a new instance of the <see cref="MTwisterEngine"/> class
		/// using the specified 32-bit seed value. The engine is fully initialized
		/// using the MT19937 "init_by_array" procedure.
		/// </summary>
		/// <param name="seed">The primary seed value.</param>
		public MTwisterEngine ( uint seed ) {
			m_startSeed = new MTwisterEngineSeed( seed );
			m_vecState = new UInt32[624];

			m_uiMag = new uint[] { m_startSeed[0] & 0xffffffffU, 0x9908b0df };

			Setup(m_startSeed);
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="MTwisterEngine"/> class
		/// </summary>
		/// <param name="seed">The primary seed value.</param>
		public MTwisterEngine ( ISeed seed ) {
			m_startSeed = seed;
			m_vecState = new UInt32[624];

			m_uiMag = new uint[] { m_startSeed[0] & 0xffffffffU, 0x9908b0df };

			Setup(m_startSeed);
		}
		/// <summary>
		/// Generates the next 32-bit unsigned integer in the MT19937 sequence.
		/// Performs the twist transformation when the internal state has been exhausted.
		/// </summary>
		/// <returns>The next 32-bit random value.</returns>
		public uint Next () {
			UInt32 _ret;

			var _klen = m_vecState.Length;

			if ( m_usT >= _klen ) 
			{
				Int16 i = 0;

				for ( ; i < _klen - 397 ; ++i ) {
					_ret = (m_vecState[i] & 0x80000000) | (m_vecState[i + 1] & 0x7fffffff);
					m_vecState[i] = m_vecState[i + 397] ^ (_ret >> 1) ^ m_uiMag[_ret & 0x1];
				}

				for ( ; i < _klen - 1 ; ++i ) {
					_ret = (m_vecState[i] & 0x80000000) | (m_vecState[i + 1] & 0x7fffffff);
					m_vecState[i] = m_vecState[i + (397 - _klen)] ^ (_ret >> 1) ^ m_uiMag[_ret & 0x1];
				}

				_ret = (m_vecState[_klen - 1] & 0x80000000) | (m_vecState[0] & 0x7fffffff);
				m_vecState[_klen - 1] = m_vecState[396] ^ (_ret >> 1) ^ m_uiMag[_ret & 0x1];

				m_usT = 0;
			}

			var y = m_vecState[m_usT++];
			y ^= (y << 7);
			y ^= (y << 7) & 0x9d2c5680;
			y ^= (y << 15) & 0xefc60000;
			y ^= (y >> 18);

			_ret = y;
			return _ret;
		}
		/// <summary>
		/// Initializes the MT19937 state array using the "init_by_array" algorithm.
		/// This method expands the provided seed values into the full 624-element
		/// internal state vector.
		/// </summary>
		/// <param name="key">The seed container used for initialization.</param>
		private void Setup ( ISeed key ) {
			Int32 i, j, k;

			var _vecLen = m_vecState.Length;
			var _klen= key.Length;

			for ( m_usT = 1 ; m_usT < _vecLen ; m_usT++ ) {
				m_vecState[m_usT] = (uint)(1812433253U * (m_vecState[m_usT - 1] ^ (m_vecState[m_usT - 1] >> 30)) + m_usT);
				m_vecState[m_usT] &= 0xffffffffU;
			}


			
			i = 1; j = 0;
			k = (_vecLen > _klen) ? _vecLen : _klen ;

			for ( ; k > 0 ; k-- ) {
				m_vecState[i] = (uint)((m_vecState[i] ^ ((m_vecState[i - 1] ^ (m_vecState[i - 1] >> 30)) * 1664525U)) + key[j] + j);
				m_vecState[i] &= 0xffffffffU; 
				i++; j++;
				if ( i >= _vecLen ) { m_vecState[0] = m_vecState[_vecLen  - 1]; i = 1; }
				if ( j >= _klen) j = 0;
			}

			for ( k = _vecLen - 1 ; k > 0 ; k-- ) {
				m_vecState[i] = (uint)((m_vecState[i] ^ ((m_vecState[i - 1] ^ (m_vecState[i - 1] >> 30)) * 1566083941U)) - i);
				m_vecState[i] &= 0xffffffffU; 
				i++;

				if ( i >= _vecLen )
					m_vecState[0] = m_vecState[_vecLen - 1]; i = 1;
			}

			m_vecState[0] = 0x80000000U;
		}
	}
}
