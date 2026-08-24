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

namespace SystemEx.Collections.Generic {
	/// \addtogroup SystemEx.Collections.Generic 
	/// @{
	/// <summary>
	/// A typed view over a raw <c>Cache</c> that exposes elements of an unmanaged type <typeparamref name="T"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <c>TypeBuffer&lt;T&gt;</c> provides element access, cloning, clearing and simple fill semantics
	/// for fixed‑stride unmanaged types. The buffer owns an internal <c>Cache</c> instance sized to
	/// <c>length * sizeof(T)</c>. All reads and writes perform endian conversion using the configured
	/// <see cref="Endian"/> value.
	/// </para>
	/// <para>
	/// This type assumes <typeparamref name="T"/> is an unmanaged value type and uses a fixed stride
	/// equal to <c>sizeof(T)</c>. Index and offset parameters are validated by the underlying cache
	/// and may throw on out‑of‑range access.
	/// </para>
	/// </remarks>
	/// <typeparam name="T">An unmanaged value type stored in the buffer.</typeparam>
	public class TypeBuffer<T> : ITypeBuffer<T>
        where T : unmanaged {
        private readonly Cache m_cache;
        private readonly int m_stride;
        private readonly int m_length;
        private Endian m_endian;

        /// <summary>
        /// Gets the number of elements in the buffer.
        /// </summary>
        public int Length => m_length;

        /// <summary>
        /// Gets the configured endian mode used for conversions.
        /// </summary>
        public Endian Endian => m_endian;

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <remarks>
        /// The indexer performs a full read or write of a single element using the buffer's stride
        /// and endian configuration. Index bounds are validated by the underlying cache operations.
        /// </remarks>
        /// <param name="i">Zero‑based element index.</param>
        /// <returns>The element value at index <paramref name="i"/>.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown when <paramref name="i"/> is outside [0, <see cref="Length"/>).</exception>
        public T this[int i] {
            get => Read(i * m_stride, m_stride, m_endian);
            set => Write(value, i * m_stride, m_stride, m_endian);
        }

        /// <summary>
        /// Creates a new <see cref="TypeBuffer{T}"/> with the specified element count and endian mode.
        /// </summary>
        /// <param name="length">Number of elements to allocate.</param>
        /// <param name="endian">Endian mode used for conversions.</param>
        /// <remarks>
        /// The constructor allocates an internal <c>Cache</c> sized to <c>length * sizeof(T)</c>.
        /// </remarks>
        public TypeBuffer ( int length, Endian endian ) {
            m_length = length;
            unsafe { m_stride = sizeof(T); }
            m_cache = new Cache(length * m_stride, CacheType.OnlySystem);
            m_endian = endian;
        }

        /// <summary>
        /// Copy constructor. Creates a new buffer that is a copy of <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The source <see cref="TypeBuffer{T}"/> to clone.</param>
        /// <remarks>
        /// The new instance receives its own <c>Cache</c> copy so subsequent modifications do not
        /// affect the original buffer.
        /// </remarks>
        public TypeBuffer ( TypeBuffer<T> other ) {
            m_length = other.m_length;
            m_stride = other.m_stride;
            m_cache = new Cache(other.m_cache);
            m_endian = other.m_endian;
        }

        /// <summary>
        /// Clears the entire buffer content to zero and resets internal usage state.
        /// </summary>
        /// <remarks>
        /// This method zeroes the underlying cache and resets the cache's used address counter.
        /// It does not modify any external cursor or position values that may exist outside the cache.
        /// </remarks>
        public void Clear () {
            m_cache.SetZero();
        }

        /// <summary>
        /// Creates a deep clone of this <see cref="TypeBuffer{T}"/>.
        /// </summary>
        /// <returns>A new <see cref="ITypeBuffer{T}"/> instance containing a copy of the current data.</returns>
        public ITypeBuffer<T> Clone () {
            return new TypeBuffer<T>(this);
        }

        /// <summary>
        /// Alias for <see cref="Clear"/>. Zeroes the buffer content and resets usage.
        /// </summary>
        public void Zero () {
            m_cache.SetZero();
        }

        /// <summary>
        /// Fills the entire buffer with the specified value.
        /// </summary>
        /// <param name="value">The value to write into every element slot.</param>
        /// <returns>The current instance for fluent usage.</returns>
        /// <remarks>
        /// The fill operation writes each element using the configured endian mode.
        /// This method performs <c>Length</c> individual writes and may be slower than a block
        /// memory fill for large buffers; it is intentionally simple and portable.
        /// </remarks>
        public ITypeBuffer<T> Fill ( T value ) {
            for ( int i = 0 ; i < m_length ; i++ )
                Write(value, i * m_stride, m_stride, m_endian);
            return this;
        }

        /// <summary>
        /// Writes a single element value into the underlying cache at the specified byte offset.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="offset">Byte offset within the cache where the element will be written.</param>
        /// <param name="count">Number of bytes to write; expected to equal the element stride.</param>
        /// <param name="endian">Endian mode to use for the conversion.</param>
        /// <returns>The number of bytes written.</returns>
        /// <remarks>
        /// The method converts <paramref name="value"/> to a byte array using <c>Conversion.ToBytes&lt;T&gt;</c>
        /// and forwards the result to the cache write implementation. The caller is responsible for
        /// providing a valid offset and count consistent with the buffer stride.
        /// </remarks>
        public int Write ( T value, int offset, int count, Endian endian ) {
            var data = Conversion.ToBytes<T>(value, endian);
            return m_cache.Write(data, offset, count);
        }

        /// <summary>
        /// Reads a single element value from the underlying cache at the specified byte offset.
        /// </summary>
        /// <param name="offset">Byte offset within the cache where the element will be read.</param>
        /// <param name="count">Number of bytes to read; expected to equal the element stride.</param>
        /// <param name="endian">Endian mode to use for the conversion.</param>
        /// <returns>The element value read from the buffer.</returns>
        /// <remarks>
        /// The method reads <c>count</c> bytes into a temporary buffer and converts them to <typeparamref name="T"/>
        /// using <c>Conversion.FromBytes&lt;T&gt;</c>. If the underlying cache cannot satisfy the read,
        /// the conversion may receive a zeroed or partial buffer depending on the cache semantics.
        /// </remarks>
        public T Read ( int offset, int count, Endian endian ) {
            byte[] buffer = new byte[m_stride];
            int read = m_cache.Read(buffer, offset, count);
            return Conversion.FromBytes<T>(buffer, endian);
        }
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
