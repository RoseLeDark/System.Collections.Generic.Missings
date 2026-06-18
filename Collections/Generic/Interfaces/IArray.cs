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

namespace SystemEx.Collections.Generic.Interfaces {
    /// <summary>
    /// Defines the basic operations for a fixed-size or dynamic array structure,
    /// including indexed access, insertion, removal, traversal, and search utilities.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the array.</typeparam>
    public interface IArray<T> {
        /// <summary>
        /// Gets the total capacity of the array.
        /// </summary>
        public int Size {  get; }

        /// <summary>
        /// Gets the first element of the array.
        /// </summary>
        public T Front { get; }

        /// <summary>
        /// Gets the last element of the array.
        /// </summary>
        public T Back { get; }
        /// <summary>
        /// Indicates whether the array is full.
        /// </summary>
        public bool IsFull { get; }

        /// <summary>
        /// Indicates whether the array contains no elements.
        /// </summary>
        public bool IsEmpty { get; }

        /// <summary>
        /// Indicates whether the array has a fixed size and cannot grow.
        /// </summary>
        public bool IsFixed { get; }


        /// <summary>
        /// Attempts to retrieve an element at the specified index.
        /// </summary>
        /// <param name="index">The index to read from.</param>
        /// <param name="item">Receives the retrieved element.</param>
        /// <returns><c>true</c> if the element was retrieved; otherwise <c>false</c>.</returns>
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Bezeichner dürfen nicht mit Schlüsselwörtern übereinstimmen", Justification = "<Ausstehend>")]
        public bool Get(int index, ref T item);


        /// <summary>
        /// Removes the last element from the array.
        /// </summary>
        /// <returns><c>true</c> if an element was removed; otherwise <c>false</c>.</returns>
        public bool Remove();


        /// <summary>
        /// Inserts an element at the specified position.
        /// </summary>
        /// <param name="pos">The position to insert at.</param>
        /// <param name="item">The element to insert.</param>
        /// <returns>The number of elements written (1 or 0).</returns>
        public int Insert(int pos, T item);
        /// <summary>
        /// Inserts a range of elements starting at the specified position.
        /// </summary>
        /// <param name="pos">The starting index.</param>
        /// <param name="items">The items to insert.</param>
        /// <returns>The number of elements successfully written.</returns>
        public int InsertRange(int pos, IEnumerable<T> items);

        /// <summary>
        /// Counts how many elements equal the specified key.
        /// </summary>
        /// <param name="Key">The value to compare against.</param>
        /// <returns>The number of matching elements.</returns>
        public UInt64 NumberOfElements(T Key);

        /// <summary>
        /// Traverses a range of elements in forward or backward order.
        /// </summary>
        /// <param name="mode">Traversal direction.</param>
        /// <param name="startIndex">Start index.</param>
        /// <param name="endIndex">End index (exclusive).</param>
        /// <param name="func">The action to apply to each element.</param>
        public void Traverse(TraversMode mode, int startIndex, int endIndex, Action<T> func);

        /// <summary>
        /// Finds the first element equal to the specified key.
        /// </summary>
        /// <param name="key">The value to search for.</param>
        /// <returns>The first matching element, or <c>null</c> if none is found.</returns>
        public T? FindFirst(T key);

        /// <summary>
        /// Finds the last element equal to the specified key.
        /// </summary>
        /// <param name="key">The value to search for.</param>
        /// <returns>The last matching element, or <c>null</c> if none is found.</returns>
        public T? FindLast(T key);

        /// <summary>
        /// Attempts to find the index of the specified key.
        /// </summary>
        /// <param name="Key">The value to search for.</param>
        /// <param name="index">Receives the index if found.</param>
        /// <returns><c>true</c> if the element was found; otherwise <c>false</c>.</returns>
        public bool TryGet(T Key, out int index);

        /// <summary>
        /// Returns the internal elements as a raw array.
        /// </summary>
        /// <returns>A new array containing all elements.</returns>
        public T[] ToArray();
    }

    /// <summary>
    /// Extends <see cref="IArray{T}"/> with dynamic resizing capabilities.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the array.</typeparam>
    public interface IDynamicArray<T> : IArray<T> {

        /// <summary>
        /// Resizes the internal buffer to the specified size.
        /// </summary>
        /// <param name="size">The new capacity.</param>
        /// <returns><c>true</c> if the resize succeeded; otherwise <c>false</c>.</returns>
        public bool Resize(int size);

        /// <summary>
        /// Gets or sets the number of elements the array grows by when resizing.
        /// </summary>
        public int GrowSize { get; set; }

        /// <summary>
        /// Enables or disables automatic resizing when the array becomes full.
        /// </summary>
        public bool AutoGrow { get; set; }
    }
}
