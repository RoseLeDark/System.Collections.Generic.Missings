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

namespace SystemEx.Drawing {

	/// \addtogroup Drawing
	/// @{
    
	/// <summary>
	/// Represents a mathematical 2D canvas storing color values of type <typeparamref name="T"/>.
	/// This interface defines a deterministic data structure that models a rectangular grid of
	/// values. The canvas does not perform graphical drawing; instead, it provides structured
	/// access, mutation, and region‑based operations on its underlying data.
	///
	/// All operations are purely mathematical and affect only the internal buffer. Visual output
	/// is never produced directly; color values are only observable through <see cref="GetPixel"/>,
	/// <see cref="Buffer"/>, or by transferring computed data into another canvas.
	/// </summary>
	/// <typeparam name="T">The color type stored in the canvas.</typeparam>
	public interface ICanvas<T>
    {
        /// <summary>
        /// Returns the color value at the specified coordinates. This method provides direct
        /// access to the underlying mathematical buffer without performing any rendering.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <returns>The color value at the given position.</returns>
        T GetPixel(int x, int y);

        /// <summary>
        /// Gets the height of the canvas in mathematical units (rows).
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets the width of the canvas in mathematical units (columns).
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets an enumerable view of the underlying buffer. The buffer contains the raw
        /// mathematical color values in row‑major order. No rendering or transformation
        /// occurs when accessing this property.
        /// </summary>
        IEnumerable<T> Buffer { get; }

        /// <summary>
        /// Resizes the canvas to the specified total size (Width × Height). Existing values
        /// may be truncated or expanded depending on the implementation. This operation
        /// modifies only the mathematical buffer.
        /// </summary>
        /// <param name="size">The new total number of elements.</param>
        /// <returns><c>true</c> if the resize succeeded; otherwise <c>false</c>.</returns>
        bool Resize(int size);

        /// <summary>
        /// Creates a new canvas containing a rectangular region of this canvas. The region
        /// is copied mathematically; no rendering or blending occurs.
        /// </summary>
        /// <param name="x">The X coordinate of the region.</param>
        /// <param name="y">The Y coordinate of the region.</param>
        /// <param name="width">The width of the region.</param>
        /// <param name="height">The height of the region.</param>
        /// <returns>A new canvas containing the extracted region.</returns>
        ICanvas<T> CopyRegion(int x, int y, int width, int height);

        /// <summary>
        /// Creates a full mathematical clone of this canvas, duplicating all stored values.
        /// </summary>
        /// <returns>A new canvas with identical content.</returns>
        ICanvas<T> Clone();

        /// <summary>
        /// Fills the entire canvas with the specified color value. This operation replaces
        /// all stored data deterministically and does not perform any graphical drawing.
        /// </summary>
        /// <param name="objcolor">The color value to assign to every element.</param>
        void Fill(T objcolor);

        /// <summary>
        /// Fills a rectangular region of the canvas with the specified color value. This
        /// modifies only the mathematical buffer and does not perform rendering.
        /// </summary>
        /// <param name="x1">The starting X coordinate.</param>
        /// <param name="y1">The starting Y coordinate.</param>
        /// <param name="x2">The ending X coordinate.</param>
        /// <param name="y2">The ending Y coordinate.</param>
        void FillRect(int x1, int y1, int x2, int y2);

        /// <summary>
        /// Clears the canvas by resetting all stored values to their default state. This
        /// operation affects only the mathematical buffer.
        /// </summary>
        void Clear();

        /// <summary>
        /// Searches the canvas for the first occurrence of the specified color value.
        /// Returns the coordinates of the match or <c>(-1, -1)</c> if no match is found.
        /// </summary>
        /// <param name="color">The color value to search for.</param>
        /// <returns>A pair containing the coordinates of the first match.</returns>
        Pair<int, int> Find(T color);

        /// <summary>
        /// Searches the canvas for the last occurrence of the specified color value.
        /// Returns the index in the underlying buffer or <c>-1</c> if no match is found.
        /// </summary>
        /// <param name="color">The color value to search for.</param>
        /// <returns>The buffer index of the last match.</returns>
        int FindLast(T color);
    }
    /// @}
}
