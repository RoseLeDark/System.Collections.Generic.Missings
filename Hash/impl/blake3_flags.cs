using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Hash.impl {
    internal enum blake3_flags : byte {
        CHUNK_START         = 1 << 0,
        CHUNK_END           = 1 << 1,
        PARENT              = 1 << 2,
        ROOT                = 1 << 3,
        KEYED_HASH          = 1 << 4,
        DERIVE_KEY_CONTEXT  = 1 << 5,
        DERIVE_KEY_MATERIAL = 1 << 6,
    };
}
