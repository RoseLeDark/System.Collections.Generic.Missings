using System;
using SystemEx.Collections.Generic;
using System.Text;

namespace SystemEx.Hash {
    /// \addtogroup hash
    /// @{
    /// <summary>
    /// Simple non‑cryptographic hash function based on the Bernstein family of hash algorithms.
    /// 
    /// This implementation provides both 32‑bit and 64‑bit variants and supports seeding,
    /// allowing deterministic or randomized hash streams depending on the caller.
    /// 
    /// Characteristics:
    ///   - Very fast
    ///   - Deterministic
    ///   - Suitable for hash tables, indexing, lightweight hashing
    ///   - Not intended for cryptographic use
    /// 
    /// The algorithm uses a classic multiply‑and‑xor mixing step:
    ///     hash = (hash * M) ^ byte
    /// where M is a constant chosen for diffusion.
    /// </summary>
    public class BernsteinHash : IHash {
        /// <summary>
        /// Computes a simple 32‑bit hash from the given byte sequence.
        ///
        /// The hash starts with the provided <paramref name="seed"/> and mixes each byte
        /// using a small multiplier (31), similar to traditional Bernstein/DJB hash variants.
        ///
        /// Endian does not affect this algorithm directly; it is included for interface
        /// compatibility with other SystemEx hashers.
        /// </summary>
        public Hash32 Compute ( Array<byte> input, uint seed, Endian endian ) {
            if ( input == null || input.Count == 0 )
                return new Hash32(0);

            uint hash = seed;

            // Simple deterministic byte loop
            for ( int i = 0 ; i < input.Count ; i++ ) {
                hash = (hash * 31) ^ input[i];
            }

            return new Hash32(hash);
        }

        /// <summary>
        /// Computes a simple 64‑bit hash from the given byte sequence.
        ///
        /// The hash starts with the provided <paramref name="seed"/> and mixes each byte
        /// using a larger multiplier (1315423911), a common constant used in JSHash‑style
        /// Bernstein derivatives to improve diffusion in 64‑bit space.
        ///
        /// Endian does not affect this algorithm directly; it is included for interface
        /// compatibility with other SystemEx hashers.
        /// </summary>
        public Hash64 ComputeLong ( Array<byte> input, ulong seed, Endian endian ) {
            if ( input == null || input.Count == 0 )
                return new Hash64(0);

            ulong hash = seed;

            // Larger multiplier for 64‑bit
            for ( int i = 0 ; i < input.Count ; i++ ) {
                hash = (hash * 1315423911L) ^ input[i];
            }

            return new Hash64(hash);
        }
    }
    /// @}
}
