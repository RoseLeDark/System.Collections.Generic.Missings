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
using SystemEx.Utils;

namespace SystemEx.Collections.Generic {
    /// \addtogroup collections
    /// @{
    /// <summary>
    /// Defines a typed cache a low-level byte buffer with position tracking, 
    /// typed read/write helpers, and optional locking behavior.
    /// </summary>
    public interface ICache {
        /// <summary>
        /// Gets the total buffer length in bytes.
        /// </summary>
        public ulong Length { get; }
        /// <summary>
        /// Gets the logical length of the cache as an unsigned 64-bit value.
        /// </summary>
        public ulong LongLength { get; }
        /// <summary>
        /// Indicates whether the cache contains no data.
        /// </summary>
        public bool IsEmpty => LongLength == 0;
        /// <summary>
        /// Gets the configured cache type (read, write, or both).
        /// </summary>
        public CacheType Type { get;  }
        /// <summary>
        /// Indicates whether the cache is writable.
        /// </summary>
        public bool CanWrite { get; }
        /// <summary>
        /// Indicates whether the cache is readable.
        /// </summary>
        public bool CanRead { get; }
        /// <summary>
        /// Gets the current read/write position.
        /// </summary>
        public ulong Position { get; }

        /// <summary>
        /// Reads bytes into the specified buffer and advances the internal position.
        /// </summary>
        public int Read(byte[] buffer, int offset, int count);

        /// <summary>
        /// Writes bytes from the specified buffer and advances the internal position.
        /// </summary>
        public int Write(byte[] buffer, int offset, int count);

        /// <summary>
        /// Writes a 32-bit unsigned integer at the specified position.
        /// </summary>
        public int Write(ulong position, uint value, Endian endian);
        /// <summary>
        /// Writes a 32-bit signed integer at the specified position.
        /// </summary>
        public int Write(ulong position, int value, Endian endian);
        /// <summary>
        /// Writes a 16-bit signed short at the specified position.
        /// </summary>
        public int Write(ulong position, short value, Endian endian);
        /// <summary>
        /// Writes a character at the specified position.
        /// </summary>
        public int Write(ulong position, char value);
        /// <summary>
        /// Writes a byte at the specified position.
        /// </summary>
        public int Write(ulong position, byte value);
        /// <summary>
        /// Writes a 16-bit unsigned short at the specified position.
        /// </summary>
        public int Write(ulong position, ushort value, Endian endian);
        /// <summary>
        /// Writes a 64-bit signed long at the specified position.
        /// </summary>
        public int Write(ulong position, long value, Endian endian);
        /// <summary>
        /// Writes a 64-bit unsigned long at the specified position.
        /// </summary>
        public int Write(ulong position, ulong value, Endian endian);
        /// <summary>
        /// Writes a single-precision floating-point value at the specified position.
        /// </summary>
        public int Write(ulong position, float value, Endian endian);
        /// <summary>
        /// Writes a double-precision floating-point value at the specified position.
        /// </summary>
        public int Write(ulong position, double value, Endian endian);

        /// <summary>
        /// Reads a 32-bit unsigned integer from the specified position.
        /// </summary>
        public uint ReadUInt(ulong position, Endian endian);
        /// <summary>
        /// Reads a 32-bit signed integer from the specified position.
        /// </summary>
        public int ReadInt(ulong position, Endian endian);
        /// <summary>
        /// Reads a 16-bit signed short from the specified position.
        /// </summary>
        public short ReadShort(ulong position, Endian endian);
        /// <summary>
        /// Reads a 16-bit unsigned short from the specified position.
        /// </summary>
        public ushort ReadUShort(ulong position, Endian endian);

        /// <summary>
        /// Reads a 64-bit signed long from the specified position.
        /// </summary>
        public long ReadLong(ulong position, Endian endian);

        /// <summary>
        /// Reads a 64-bit unsigned long from the specified position.
        /// </summary>
        public ulong ReadULong(ulong position, Endian endian);

        /// <summary>
        /// Reads a single character from the specified position.
        /// </summary>
        public char ReadChar(ulong position);

        /// <summary>
        /// Reads a single-precision floating-point value from the specified position.
        /// </summary>
        public float ReadFloat(ulong position, Endian endian);

        /// <summary>
        /// Reads a double-precision floating-point value from the specified position.
        /// </summary>
        public double ReadDouble(ulong position, Endian endian);

        /// <summary>
        /// Writes a single byte at the current position and advances the position.
        /// </summary>
        public int Write(byte data);

        /// <summary>
        /// Reads a single byte from the specified position.
        /// </summary>
        public byte Read(ulong position);
        /// <summary>
        /// Writes a byte range into the cache starting at the specified position.
        /// </summary>
        public ulong WriteRange(ulong position, byte[] data);

       
        /// <summary>
        /// Writes a byte range into the cache between <paramref name="start"/> and <paramref name="iend"/>.
        /// </summary>
        public ulong WriteRange(ulong start, ulong iend, byte[] data);

        /// <summary>
        /// Reads a range of bytes starting at the specified position.
        /// </summary>
        public byte[]? ReadRange(ulong position, uint count);

        /// <summary>
        /// Returns a copy of the internal buffer.
        /// </summary>
        public byte[] ToArray();
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
