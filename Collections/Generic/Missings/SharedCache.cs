using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Collections.Generic.Missings {

    public enum SharedCacheType : byte {
        ReadOnly,   // Cache -> Hardware
        WriteOnly,  // Hardware -> Cache
        ReadWrite   // Beides
    }

    public class SharedCache {
        private readonly Cache m_cache;
        private MemoryHandle m_pin;
        private readonly SharedCacheType m_type;

        private byte[]? m_hardwareBuffer;

        public bool CanRead => m_type == SharedCacheType.ReadOnly
                             || m_type == SharedCacheType.ReadWrite;

        public bool CanWrite => m_type == SharedCacheType.WriteOnly
                             || m_type == SharedCacheType.ReadWrite;

        public bool IsReadWrite => m_type == SharedCacheType.ReadWrite;

        internal SharedCache(Cache cache, SharedCacheType type) {
            m_cache = cache;
            m_type = type;

            if(m_type != SharedCacheType.ReadOnly) {
                m_pin = MakeHardwareBuffer(cache.Length + 256);
            }
        }

        public void Begin() {
            if ( m_hardwareBuffer == null ) return;

            if ( m_type != SharedCacheType.ReadOnly ) {
                Array.Copy(m_cache.ToArray(), m_hardwareBuffer, m_cache.Length);
            }
        }

        public void End(int offset, int code) {

            if ( m_type != SharedCacheType.ReadOnly ) {
                m_cache.SharedCallBack(this, m_hardwareBuffer!, offset, code);
                m_pin.Dispose();
            }

        }

        internal unsafe MemoryHandle MakeHardwareBuffer(int size) {
            m_hardwareBuffer = new Byte[size];

            GCHandle handle = GCHandle.Alloc(m_hardwareBuffer, GCHandleType.Pinned);
            void* ptr = (void*)handle.AddrOfPinnedObject();
            return new MemoryHandle(ptr, handle, null);
        }
    }

}
