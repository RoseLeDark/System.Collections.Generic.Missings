using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace SystemEx.Collections.Generic.Interfaces {
    /// \addtogroup collections
    /// @{
    /// \addtogroup interfaces
    /// @{
    /// <summary>
    /// Defines the minimal functionality required for a generic container
    /// used by the SystemEx collection framework. Implementations provide
    /// indexed access, insertion, replacement, removal, and structural
    /// duplication of stored elements.
    /// </summary>
    /// <typeparam name="T">The element type stored in the container.</typeparam>
    public interface IContainerEx<T> {
        /// <summary>
        /// Gets a value indicating whether the container cannot accept
        /// additional elements without growing.
        /// </summary>
        bool IsFull { get;  }
        /// <summary>
        /// Gets a value indicating whether the container contains no elements.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Gets the element at the current logical position. The meaning of
        /// the current position is implementation‑defined.
        /// </summary>
        T Current { get; }

        /// <summary>
        /// Gets the number of elements currently stored in the container.
        /// </summary>
        long Count { get; }


        /// <summary>
        /// Gets the total capacity of the container, including unused slots.
        /// </summary>
        long Length { get; }

        /// <summary>
        /// Appends an element to the end of the container. Implementations may
        /// grow automatically or fail depending on their configuration.
        /// </summary>
        /// <param name="entry">The element to append.</param>
        /// <returns>
        /// True if the element was appended; otherwise false.
        /// </returns>
        bool PushBack ( T entry );

        /// <summary>
        /// Inserts an element at the specified index, shifting subsequent
        /// elements to the right.
        /// </summary>
        /// <param name="index">The insertion index.</param>
        /// <param name="entry">The element to insert.</param>
        /// <returns>
        /// True if the element was inserted; otherwise false.
        /// </returns>
        bool Insert ( long index, T entry );

        /// <summary>
        /// Inserts an element into the specified range. Implementations define
        /// how the range affects the insertion behavior.
        /// </summary>
        /// <param name="start">The start index of the range.</param>
        /// <param name="end">The end index of the range.</param>
        /// <param name="entry">The element to insert.</param>
        /// <returns>
        /// True if the element was inserted; otherwise false.
        /// </returns>
        bool Insert ( long start, long end, T entry );

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
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The index to replace.</param>
        /// <param name="entry">The new element.</param>
        /// <returns>
        /// True if the element was replaced; otherwise false.
        /// </returns>
        bool Replace ( long index, T entry );

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
        /// Removes the element at the specified index, shifting subsequent
        /// elements to the left.
        /// </summary>
        /// <param name="index">The index of the element to remove.</param>
        /// <returns>
        /// True if the element was removed; otherwise false.
        /// </returns>
        bool Erase ( long index );

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
        /// Returns the underlying storage as a native array. The returned array
        /// may reflect the container's internal representation.
        /// </summary>
        /// <returns>A native array containing the stored elements.</returns>
        T[] ToNative ();

        /// <summary>
        /// Returns the element at the specified index.
        /// </summary>
        /// <param name="index">The index of the element.</param>
        /// <returns>The element at the specified index.</returns>
        T ElementAt ( long index );

        /// <summary>
        /// Removes all elements from the container, resetting its logical size.
        /// </summary>
        void Clear ();

        /// <summary>
        /// Returns the runtime type of the stored elements.
        /// </summary>
        /// <returns>The element type.</returns>
        Type GetElementType ();

        /// <summary>
        /// Swaps the elements at the specified indices.
        /// </summary>
        /// <param name="i">The first index.</param>
        /// <param name="j">The second index.</param>
        void Swap ( long i, long j );

        /// <summary>
        /// Creates a structural duplicate of the container using its
        /// copy constructor. The returned instance is independent and does
        /// not reference the original storage.
        /// </summary>
        /// <returns>
        /// A new <see cref="IContainerEx{T}"/> instance containing the same
        /// elements as the original container.
        /// </returns>
        IContainerEx<T> Duplicate ();
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
