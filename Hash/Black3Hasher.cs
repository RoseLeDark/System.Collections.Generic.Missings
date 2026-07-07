using System;
using System.Collections.Generic;
using System.Text;
using SystemEx.Collections.Generic;
using SystemEx.Hash.impl;

namespace SystemEx.Hash {

    /// <summary>
    /// ONLY TEST!!!
    /// </summary>
    public class Black3Hasher : IHash {
        Array<byte> m_key;
        Endian m_endian;

        public Black3Hasher (Endian endian, byte[] IV) {
            m_key = new Array<byte>(IV);
            m_endian = endian;
        }
        public Hash32 Compute ( Array<byte> input, uint seed ) {
            Black3 hash = new Black3(m_key.ToArray());

            hash.Chunk.Update(input.ToArray());

            var output = hash.Chunk.Finalize();

            byte[] cv = new byte[Black3Infos.BLAKE3_OUT_LEN];
            output.chaining_value(cv);


            // Hash32 erzeugen (dein eigener Typ)
            return new Hash32(cv.ToUInt());
        }

        public Hash64 ComputeLong ( Array<byte> input, ulong seed ) {
            Black3 hash = new Black3(m_key.ToArray());

            hash.Chunk.Update(input.ToArray());

            var output = hash.Chunk.Finalize();

            byte[] cv = new byte[Black3Infos.BLAKE3_OUT_LEN];
            output.chaining_value(cv);


            // Hash32 erzeugen (dein eigener Typ)
            return new Hash64((ulong)cv.ToUInt());
        }
    }
}
