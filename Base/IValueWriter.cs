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

namespace SystemEx {
	/// \addtogroup SystemEx
	/// @
	/// <summary>
	/// Provides endian-aware write operations for primitive numeric types.
	/// Implementations convert the specified value into a byte sequence using
	/// the given <see cref="Endian"/> format and write it to the underlying
	/// data sink.
	/// </summary>
	public interface IValueWriter : IValueReader {
		/// <summary>
		/// Writes an unsigned 32-bit integer using the specified endian format.
		/// </summary>
		void Write ( uint value, Endian endian );

		/// <summary>
		/// Writes a signed 32-bit integer using the specified endian format.
		/// </summary>
		void Write ( int value, Endian endian );

		/// <summary>
		/// Writes a signed 16-bit integer using the specified endian format.
		/// </summary>
		void Write ( short value, Endian endian );

		/// <summary>
		/// Writes an unsigned 16-bit integer using the specified endian format.
		/// </summary>
		void Write ( ushort value, Endian endian );

		/// <summary>
		/// Writes a signed 64-bit integer using the specified endian format.
		/// </summary>
		void Write ( long value, Endian endian );

		/// <summary>
		/// Writes an unsigned 64-bit integer using the specified endian format.
		/// </summary>
		void Write ( ulong value, Endian endian );

		/// <summary>
		/// Writes a single-precision floating-point value using the specified endian format.
		/// </summary>
		void Write ( float value, Endian endian );

		/// <summary>
		/// Writes a double-precision floating-point value using the specified endian format.
		/// </summary>
		void Write ( double value, Endian endian );
	}


	/// <summary>
	/// Extends <see cref="IValueReader{T}"/> by providing a strongly typed
	/// write operation for a specific value type. Implementations convert the
	/// given value into a byte sequence using the specified <see cref="Endian"/>
	/// format.
	/// </summary>
	/// <typeparam name="T">
	/// The type of the value written by <see cref="Write(T, Endian)"/>.
	/// </typeparam>
	public interface IValueWriter<T> : IValueReader<T> {
		/// <summary>
		/// Writes a value of type <typeparamref name="T"/> using the specified
		/// endian format. The conversion rules depend on the concrete type and
		/// the implementation of the writer.
		/// </summary>
		/// <param name="value">The value to write.</param>
		/// <param name="endian">The byte order used for conversion.</param>
		void Write ( T value, Endian endian );
	}
	//@}
}
