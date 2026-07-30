using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace SystemEx.Collections.Generic {
    /// \addtogroup collections
    /// @{
    /// <summary>
    /// Defines the minimal functionality required for a generic vextzu
    /// used by the SystemEx collection framework. Implementations provide
    /// indexed access, insertion, replacement, removal, and structural
    /// duplication of stored elements.
    /// </summary>
    /// <typeparam name="T">The element type stored in the container.</typeparam>
    public interface IVector<T> : IContainer<T> {

        /// <summary>
        /// Inserts multiple elements starting at the specified index.
        /// </summary>
        /// <param name="start">The insertion index.</param>
        /// <param name="entrys">The elements to insert.</param>
        /// <returns>
        /// True if the elements were inserted; otherwise false.
        /// </returns>
        bool InsertRange ( long start, T[] entrys );

        /// <summary>
        /// Replaces all elements within the specified range.
        /// </summary>
        /// <param name="start">The start index of the range.</param>
        /// <param name="end">The end index of the range.</param>
        /// <param name="entry">The replacement element.</param>
        /// <returns>
        /// True if the range was replaced; otherwise false.
        /// </returns>
        bool Replace ( long start, long end, T entry );

        /// <summary>
        /// Replaces multiple elements starting at the specified index.
        /// </summary>
        /// <param name="start">The start index.</param>
        /// <param name="entrys">The replacement elements.</param>
        /// <returns>
        /// True if the elements were replaced; otherwise false.
        /// </returns>
        bool ReplaceRange ( long start, T[] entrys );


        /// <summary>
        /// Removes all elements within the specified range.
        /// </summary>
        /// <param name="start">The start index of the range.</param>
        /// <param name="end">The end index of the range.</param>
        /// <returns>
        /// True if the range was removed; otherwise false.
        /// </returns>
        bool Erase ( long start, long end );

        /// <summary>
        /// Removes the first occurrence of the specified value.
        /// </summary>
        /// <param name="value">The value to remove.</param>
        /// <returns>
        /// True if the value was found and removed; otherwise false.
        /// </returns>
        bool Erase ( T value );

        /// <summary>
        /// Attempts to increase the container's capacity. Implementations may
        /// grow automatically or fail depending on their configuration.
        /// </summary>
        /// <returns>
        /// True if the container grew; otherwise false.
        /// </returns>
        bool Grow ();

        /// <summary>
        /// Creates a structural duplicate of the container using its
        /// copy constructor. The returned instance is independent and does
        /// not reference the original storage.
        /// </summary>
        /// <returns>
        /// A new <see cref="IVector{T}"/> instance containing the same
        /// elements as the original container.
        /// </returns>
        IVector<T> Duplicate ();


        T[] ToNative ();
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
