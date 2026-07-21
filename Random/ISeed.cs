using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Random {


    /// <summary>
    /// Represents a generic seed source for random number generators.
    /// Implementations provide an array of 32‑bit seed values, support
    /// indexed access, and may update their internal state when required.
    /// </summary>
    public interface ISeed {
        /// <summary>
        /// Returns the underlying seed values as an array of 32‑bit unsigned integers.
        /// The returned array defines the complete state of the seed at the time of access.
        /// </summary>
        /// <returns>
        /// A <see cref="uint"/> array containing the current seed values.
        /// </returns>
        uint[] GetSeed ();

        /// <summary>
        /// Gets the number of 32‑bit values contained in the seed.
        /// This corresponds to the length of the array returned by <see cref="GetSeed"/>.
        /// </summary>
        int Length { get; }


        /// <summary>
        /// Provides indexed access to individual seed values. Both reading and writing
        /// are supported, allowing engines or mixing operations to modify specific
        /// seed components directly.
        /// </summary>
        /// <param name="i">The zero‑based index of the seed value.</param>
        /// <returns>
        /// The 32‑bit unsigned integer stored at the specified index.
        /// </returns>
        uint this[int i] { get; set; }
    }


    /// <summary>
    /// Extends <see cref="ISeed"/> with a typed update mechanism. This generic
    /// interface allows seed implementations to accept external values that
    /// influence or mutate the internal seed state. The type parameter defines
    /// the kind of input used for updating the seed.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value used to update the seed. Implementations may use
    /// this value to modify individual seed components, refresh time‑based
    /// values, apply mixing logic, or perform any custom mutation.
    /// </typeparam>
    public interface ISeed<T> : ISeed {
        /// <summary>
        /// Updates the internal seed state using the specified value. The exact
        /// behavior depends on the implementation and may include mutation,
        /// recomputation, or partial replacement of seed components.
        /// </summary>
        /// <param name="value">
        /// The value used to update the seed. Its interpretation is defined by
        /// the implementing type.
        /// </param>
        void Update ( T value );
    }



}
