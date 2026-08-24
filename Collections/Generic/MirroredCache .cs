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
	/// \addtogroup SystemEx.Collections.Generic 
	/// @{
	/// <summary>
	/// A cache that maintains a mirrored copy of all written data.  
	/// Every write operation is performed on the primary cache and a reversed
	/// (byte‑mirrored) version of the data is written to the secondary cache.  
	/// During reads, both caches are compared to ensure data integrity.
	/// </summary>
	public class MirroredCache : Cache {

        /// <summary>
        /// The secondary cache storing the mirrored byte sequence.
        /// </summary>
        private readonly Cache m_secondary;

        /// <summary>
        /// Gets the primary cache (this instance).
        /// </summary>
        public Cache Primary => this;

        /// <summary>
        /// Gets the secondary cache containing mirrored data.
        /// </summary>
        public Cache Secondary => m_secondary;

        /// <summary>
        /// Gets the total cache size in bytes.
        /// </summary>
        public int Size => (int)Length;

        /// <summary>
        /// Creates a mirrored cache with the specified size.  
        /// The primary and secondary caches have identical capacity.
        /// </summary>
        /// <param name="size">The size of each cache in bytes.</param>
        public MirroredCache(int size)
            : base(size, CacheType.Both) {
            m_secondary = new Cache(size, CacheType.Both);
        }

        /// <summary>
        /// Writes a byte range to both primary and secondary caches.  
        /// The primary receives the original data, while the secondary receives
        /// a reversed (mirrored) copy of the data.
        /// </summary>
        /// <param name="start">Start offset.</param>
        /// <param name="iend">End offset (exclusive).</param>
        /// <param name="data">The data to write.</param>
        /// <returns>The number of bytes written to the primary cache.</returns>
        public override ulong WriteRange(ulong start, ulong iend, byte[] data) {
            // 1. Write original data to primary
            ulong written = base.WriteRange(start, iend, data);

            // 2. Create mirrored copy
            byte[] mirrored = Mirror(data);

            // 3. Write mirrored data to secondary
            m_secondary.WriteRange(start, iend, mirrored);

            return written;
        }

        /// <summary>
        /// Reads a byte range from both caches and verifies integrity.  
        /// The secondary data is reversed back and compared with the primary.  
        /// If both match, the primary data is returned; otherwise <c>null</c>.
        /// </summary>
        /// <param name="position">Start offset.</param>
        /// <param name="count">Number of bytes to read.</param>
        /// <returns>The verified data, or <c>null</c> if mismatch occurs.</returns>
        public override byte[]? ReadRange(ulong position, uint count) {
            byte[]? _readed = base.ReadRange(position, count);
            byte[]? _mirrowed = m_secondary.ReadRange(position, count);

            if ( _mirrowed == null || _readed == null )
                return null;

            // Reverse the mirrored data back
            MirrorRead(_mirrowed);

            // Compare both buffers
            bool cheak = _readed.EqualArray(_mirrowed);

            return cheak ? _readed : null;
        }

        /// <summary>
        /// Reverses the byte array in place (used for read‑side verification).
        /// </summary>
        private static byte[] MirrorRead(byte[] data) {
            int i = 0;
            int j = data.Length - 1;

            while ( i < j ) {
                byte tmp = data[i];
                data[i] = data[j];
                data[j] = tmp;
                i++;
                j--;
            }
            return data;
        }

        /// <summary>
        /// Creates a reversed copy of the given byte array (used for write‑side mirroring).
        /// </summary>
        private static byte[] Mirror(byte[] data) {
            byte[] m = new byte[data.Length];
            int last = data.Length - 1;

            for ( int i = 0; i < data.Length; i++ )
                m[i] = data[last - i];

            return m;
        }
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
