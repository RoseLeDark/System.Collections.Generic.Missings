using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Hash.impl {
    internal class Black3Infos {
        public const byte BLAKE3_KEY_LEN = 32;
        public const byte BLAKE3_BLOCK_LEN = 64;
        public const UInt16 BLAKE3_CHUNK_LEN = 1024;
        public const byte BLAKE3_MAX_DEPTH = 54;
        public const byte BLAKE3_OUT_LEN = 32;
        public const int BLAKE3_CV_STACK_LEN = (BLAKE3_MAX_DEPTH + 1) * BLAKE3_OUT_LEN;
    }
}
