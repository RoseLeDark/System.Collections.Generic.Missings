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


using SystemEx.Threading;

namespace SystemEx.Random {
	/// \addtogroup Random
	/// @{

	/// <summary>
	/// Defines the available mixing strategies used to combine two random seeds.
	/// These operations are purely mathematical and intended for non‑cryptographic
	/// random number generation. Each mode describes how the corresponding seed
	/// values are combined to produce a new mixed seed.
	/// </summary>
	public enum SeedMix : byte {
        /// <summary>
        /// Bitwise exclusive OR (XOR) between corresponding seed values.
        /// Produces strong bit diffusion and is commonly used in PRNG mixing.
        /// </summary>
        XOR,

        /// <summary>
        /// Adds corresponding seed values. Useful for linear seed combination.
        /// </summary>
        Addition,

        /// <summary>
        /// Subtracts the other seed value from the current one.
        /// </summary>
        Subtraction,

        /// <summary>
        /// Multiplies corresponding seed values. Produces strong variation and
        /// is used in several classical random generators.
        /// </summary>
        Multiplication,

        /// <summary>
        /// Bitwise AND between corresponding seed values. Can be used to mask
        /// or restrict certain bits.
        /// </summary>
        And,

        /// <summary>
        /// Bitwise OR between corresponding seed values. Aggregates bits from
        /// both seeds.
        /// </summary>
        Or,

        /// <summary>
        /// Shifts the current seed value left by a shift amount derived from
        /// the other seed and the provided mix parameter.
        /// </summary>
        ShiftLeft,

        /// <summary>
        /// Shifts the current seed value right by a shift amount derived from
        /// the other seed and the provided mix parameter.
        /// </summary>
        ShiftRight,

        /// <summary>
        /// Rotates the current seed value left by a rotation amount derived
        /// from the other seed and the provided mix parameter.
        /// </summary>
        RotateLeft,

        /// <summary>
        /// Rotates the current seed value right by a rotation amount derived
        /// from the other seed and the provided mix parameter.
        /// </summary>
        RotateRight,

        /// <summary>
        /// Bitwise negation (NOT) of the current seed value.
        /// </summary>
        Not,

        /// <summary>
        /// Takes the minimum of the two corresponding seed values.
        /// </summary>
        Minimal,

        /// <summary>
        /// Takes the maximum of the two corresponding seed values.
        /// </summary>
        Maximal,

        /// <summary>
        /// Computes the average of the two corresponding seed values.
        /// </summary>
        Average,

        /// <summary>
        /// Replaces the current seed entirely with the other seed.
        /// </summary>
        New,
        /// <summary>
        /// Uses a user‑defined mixing function provided via
        /// <c>Func&lt;ISeed, ISeed, ISeed&gt; OnUserMix</c>.
        /// This allows custom mixing logic beyond the predefined strategies.
        /// </summary>
        User
    }

    /// <summary>
    /// Represents a composite seed that can be dynamically mixed with other seeds
    /// using a variety of mathematical operations. <see cref="SeedMixed"/> acts as
    /// a wrapper around an existing <see cref="ISeed"/> instance and provides
    /// thread‑safe mixing functionality suitable for all random engines in SystemEx.
    /// </summary>
    public struct SeedMixed : ISeed<ISeed> {
        ISeed m_usable;
        LightLock m_lLock;

        /// <summary>
        /// Gets an optional user‑defined mixing function. When the mix type
        /// <see cref="SeedMix.User"/> is selected, this delegate is invoked to
        /// perform custom mixing logic between the current seed and the provided
        /// seed instance.
        /// </summary>
        public Func<ISeed, ISeed, ISeed>? OnUserMix { get; }

        /// <summary>
        /// Gets the underlying seed currently wrapped by this instance.
        /// </summary>
        public ISeed Current { get => m_usable; }

        /// <inheritdoc/>
        public uint this[int index] {
            get => m_usable[index];
            set => m_usable[index] = value;
        }

        /// <inheritdoc/>
        public int Length => m_usable.Length;

        /// <summary>
        /// Initializes a new <see cref="SeedMixed"/> instance wrapping the specified
        /// seed. The internal lock is created to ensure thread‑safe mixing.
        /// </summary>
        /// <param name="seed">The seed to wrap and operate on.</param>
        public SeedMixed ( ISeed seed ) {
            m_usable = seed;
            m_lLock = new LightLock();
            OnUserMix = null;
        }

        /// <inheritdoc/>
        public uint[] GetSeed () => m_usable.GetSeed();

        /// <summary>
        /// Updates the seed by mixing it with another seed using the default
        /// multiplication strategy. This provides a simple way to refresh the
        /// composite seed without specifying a mix type explicitly.
        /// </summary>
        /// <param name="value">The seed used for updating.</param>
        public void Update ( ISeed value ) {
            Mix<ISeed>(value, SeedMix.Multiplication, 1);
        }

        /// <summary>
        /// Mixes the current seed with another seed using the specified mixing
        /// strategy. The operation is performed in a thread‑safe manner and may
        /// apply arithmetic, bitwise, shift, or rotation‑based transformations.
        /// When <see cref="SeedMix.User"/> is selected, the user‑defined mixing
        /// delegate <see cref="OnUserMix"/> is invoked instead.
        /// </summary>
        /// <typeparam name="T">The seed type, constrained to <see cref="ISeed"/>.</typeparam>
        /// <param name="other">The seed to mix with the current seed.</param>
        /// <param name="type">The mixing strategy to apply.</param>
        /// <param name="value">
        /// An optional parameter used by certain mix operations (e.g. shift and
        /// rotation amounts). Defaults to 2.
        /// </param>
        /// <returns>
        /// The current <see cref="SeedMixed"/> instance after mixing.
        /// </returns>
        public SeedMixed Mix<T> ( T other, SeedMix type, uint value = 2 ) where T : ISeed {

            int _l = System.Math.Min(other.Length, m_usable.Length);

            m_lLock.Lock();

            switch ( type ) {
            case SeedMix.Addition: for ( int i = 0 ; i < _l ; i++ ) m_usable[i] += other[i]; break;
            case SeedMix.Subtraction: for ( int i = 0 ; i < _l ; i++ ) m_usable[i] -= other[i]; break;
            case SeedMix.Multiplication: for ( int i = 0 ; i < _l ; i++ ) m_usable[i] *= other[i]; break;
            case SeedMix.XOR: for ( int i = 0 ; i < _l ; i++ ) m_usable[i] ^= other[i]; break;
            case SeedMix.Or: for ( int i = 0 ; i < _l ; i++ ) m_usable[i] |= other[i]; break;
            case SeedMix.And: for ( int i = 0 ; i < _l ; i++ ) m_usable[i] &= other[i]; break;
            case SeedMix.Minimal: for ( int i = 0 ; i < _l ; i++ ) m_usable[i] = System.Math.Min(m_usable[i], other[i]); break;
            case SeedMix.Maximal: for ( int i = 0 ; i < _l ; i++ ) m_usable[i] = System.Math.Max(m_usable[i], other[i]); break;
            case SeedMix.Average: for ( int i = 0 ; i < _l ; i++ ) m_usable[i] = (m_usable[i] + other[i]) >> 1; break;
            case SeedMix.New: for ( int i = 0 ; i < _l ; i++ ) m_usable[i] = other[i]; break;
            case SeedMix.ShiftRight: for ( int i = 0 ; i < _l ; i++ ) { var _x = (m_usable[i] >> (int)(other[i] & value)); m_usable[i] = _x; } break;
            case SeedMix.ShiftLeft: for ( int i = 0 ; i < _l ; i++ ) { var _x = (m_usable[i] << (int)(other[i] & value)); m_usable[i] = _x; } break;
            case SeedMix.RotateLeft: for ( int i = 0 ; i < _l ; i++ ) { int _r = (int)(other[i] + value) & 31; m_usable[i] = (m_usable[i] << _r) | (m_usable[i] >> (32 - _r));  } break;
            case SeedMix.RotateRight: for ( int i = 0 ; i < _l ; i++ ) { int _r = (int)(other[i] + value) & 31; m_usable[i] = (m_usable[i] >> _r) | (m_usable[i] << (32 - _r)); } break;
            default: if ( OnUserMix != null ) m_usable = OnUserMix.Invoke(m_usable, other); break;
            }

            m_lLock.Unlock();

            return this;
        }
    }
	
}
