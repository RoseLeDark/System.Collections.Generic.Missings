using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Hash {
    /// \addtogroup hash
    /// @{
    /// <summary>
    /// Implements the XXHash3 hash algorithm.
    /// </summary>
    internal class XXHash3Hasher {
        Endian m_endian;
        /// <summary>
        /// Craate a new instance
        /// </summary>
        /// <param name="endian">The suing endian for creating a hash</param>
        public XXHash3Hasher ( Endian endian ) {
            m_endian = endian;
        }

    }
    /// @}
}
