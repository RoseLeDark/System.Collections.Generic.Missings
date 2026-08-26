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
	/// Provides binary serialization and deserialization capabilities for a type.
	/// Implementations define how an instance is encoded into a byte sequence and
	/// how it is reconstructed from raw binary data.
	/// 
	/// The interface supports endian‑aware serialization and allows writing the
	/// encoded representation into an existing buffer at a specified offset.
	/// </summary>
	/// <typeparam name="TSelf">
	/// The implementing type that provides its own binary encoding and decoding logic.
	/// </typeparam>
	public interface IByteSerializable<TSelf> {

        /// <summary>
        /// Creates an instance of <typeparamref name="TSelf"/> from a byte array.
        /// The method reads the encoded representation starting at the specified
        /// <paramref name="offset"/> using the given <paramref name="endian"/> order.
        /// </summary>
        /// <param name="bytes">
        /// The source buffer containing the binary representation of the value.
        /// </param>
        /// <param name="offset">
        /// The position within <paramref name="bytes"/> where the encoded data begins.
        /// </param>
        /// <param name="endian">
        /// The byte order used to interpret the binary data.
        /// </param>
        /// <returns>
        /// A reconstructed <typeparamref name="TSelf"/> instance based on the provided
        /// binary data.
        /// </returns>
        static abstract TSelf FromBytes ( byte[] bytes, long offset, Endian endian );

        /// <summary>
        /// Encodes the current instance into a newly allocated byte array using the
        /// specified <paramref name="endian"/> order.
        /// </summary>
        /// <param name="endian">
        /// The byte order used to encode the value.
        /// </param>
        /// <returns>
        /// A new byte array containing the binary representation of the instance.
        /// </returns>
        byte[] ToBytes ( Endian endian );

        /// <summary>
        /// Writes the binary representation of the current instance into an existing
        /// <paramref name="destination"/> buffer at the specified <paramref name="offset"/>.
        /// 
        /// The caller is responsible for ensuring that the buffer is large enough to
        /// hold the encoded data starting at the given offset. Bytes before the offset
        /// remain untouched and can contain arbitrary user data.
        /// </summary>
        /// <param name="destination">
        /// The target buffer into which the encoded value will be written.
        /// </param>
        /// <param name="offset">
        /// The position within <paramref name="destination"/> where the encoded data
        /// will be written. For example, if <c>offset = 100</c>, bytes <c>0</c> through
        /// <c>99</c> remain unchanged, and the encoded value begins at index <c>100</c>.
        /// </param>
        /// <param name="endian">
        /// The byte order used to encode the value.
        /// </param>
        void ToBytes ( ref byte[] destination, long offset, Endian endian );
    }
	//@}
}
