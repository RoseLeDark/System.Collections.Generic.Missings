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
	/// @{
	
	/// <summary>
	/// Provides endian-aware read operations for primitive numeric types.
	/// Implementations convert the underlying byte sequence into the requested
	/// value using the specified <see cref="Endian"/> format. If insufficient
	/// bytes are available, the provided fallback value is returned.
	/// </summary>
	public interface IValueReader {
		/// <summary>
		/// Reads an unsigned 32-bit integer using the specified endian format.
		/// Returns the fallback value if insufficient bytes are available.
		/// </summary>
		uint ReadUInt ( Endian endian, uint ervlue );

		/// <summary>
		/// Reads a signed 32-bit integer using the specified endian format.
		/// Returns the fallback value if insufficient bytes are available.
		/// </summary>
		int ReadInt ( Endian endian, int ervlue );

		/// <summary>
		/// Reads a signed 16-bit integer using the specified endian format.
		/// Returns the fallback value if insufficient bytes are available.
		/// </summary>
		short ReadShort ( Endian endian, short ervlue );

		/// <summary>
		/// Reads an unsigned 16-bit integer using the specified endian format.
		/// Returns the fallback value if insufficient bytes are available.
		/// </summary>
		ushort ReadUShort ( Endian endian, ushort ervlue );

		/// <summary>
		/// Reads a signed 64-bit integer using the specified endian format.
		/// Returns the fallback value if insufficient bytes are available.
		/// </summary>
		long ReadLong ( Endian endian, long ervlue );

		/// <summary>
		/// Reads an unsigned 64-bit integer using the specified endian format.
		/// Returns the fallback value if insufficient bytes are available.
		/// </summary>
		ulong ReadULong ( Endian endian, ulong ervlue );

		/// <summary>
		/// Reads a single-precision floating-point value using the specified endian format.
		/// Returns the fallback value if insufficient bytes are available.
		/// </summary>
		float ReadFloat ( Endian endian, float ervlue );

		/// <summary>
		/// Reads a double-precision floating-point value using the specified endian format.
		/// Returns the fallback value if insufficient bytes are available.
		/// </summary>
		double ReadDouble ( Endian endian, double ervlue );
	}


	/// <summary>
	/// Provides a strongly typed read operation for a specific value type.
	/// Implementations convert the underlying byte sequence into the requested
	/// type using the specified <see cref="Endian"/> format. If insufficient
	/// bytes are available, the provided fallback value is returned.
	/// </summary>
	/// <typeparam name="T">
	/// The type of the value returned by <see cref="ReadValue(Endian, T)"/>.
	/// </typeparam>
	public interface IValueReader<T> {
		/// <summary>
		/// Reads a value of type <typeparamref name="T"/> using the specified
		/// endian format. The conversion rules depend on the concrete type and
		/// the implementation of the reader. If insufficient bytes are available,
		/// the specified fallback value is returned.
		/// </summary>
		/// <param name="endian">The byte order used for conversion.</param>
		/// <param name="errvalue">
		/// The fallback value returned when the underlying data source does not
		/// provide enough bytes to complete the read operation.
		/// </param>
		/// <returns>
		/// The value read from the underlying byte sequence, converted into
		/// <typeparamref name="T"/>, or <paramref name="errvalue"/> if the read
		/// operation cannot be completed.
		/// </returns>
		T ReadValue ( Endian endian, T? errvalue = default(T) );
	}
	//@}

}