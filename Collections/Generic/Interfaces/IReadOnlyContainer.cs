using System;

namespace SystemEx.Collections.Generic {
    /// \addtogroup collections
    /// @{
    /// <summary>
    /// Defines the minimal read‑only functionality required for a generic
    /// container used by the SystemEx collection framework. Implementations
    /// provide indexed access and structural inspection without allowing
    /// modification of stored elements.
    /// </summary>
    /// <typeparam name="T">The element type stored in the container.</typeparam>
    public interface IReadOnlyContainer<T>  {

        /// <summary>
        /// Gets the first element of the container.
        /// </summary>
        public T Front { get; }


        /// <summary>
        /// Indicates whether the container is full.
        /// </summary>
        public bool IsFull { get; }

        /// <summary>
        /// Gets a value indicating whether the container contains no elements.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Gets the element at the current logical position. The meaning of
        /// the current position is implementation‑defined.
        /// </summary>
        Optional<T> Current { get; }

        /// <summary>
        /// Gets the number of elements currently stored in the container.
        /// </summary>
        long Count { get; }

        /// <summary>
        /// Gets the total capacity of the container, including unused slots.
        /// </summary>
        long Length { get; }

        /// <summary>
        /// Returns the element at the specified index.
        /// </summary>
        /// <param name="index">The index of the element.</param>
        /// <returns>The element at the specified index.</returns>
        Optional<T> ElementAt ( long index );

        /// <summary>
        /// Returns the runtime type of the stored elements.
        /// </summary>
        /// <returns>The element type.</returns>
        Type GetElementType ();

        
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
