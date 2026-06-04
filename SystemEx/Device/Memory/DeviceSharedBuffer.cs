using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using SystemEx.Device.Intertropt;

namespace SystemEx.Device.Memory {

    public enum SharedCacheType : byte {
        ReadOnly,   // Cache -> Hardware
        WriteOnly,  // Hardware -> Cache
        ReadWrite   // Beides
    }

    public class DeviceSharedBuffer<TDeviceSharedBackend> where TDeviceSharedBackend : IDeviceSharedBackend {
        private readonly DeviceBuffer m_cache;
        private readonly SharedCacheType m_type;
        private TDeviceSharedBackend m_backend;
        internal object m_hardwareBuffer;

        public bool CanRead => m_type == SharedCacheType.ReadOnly
                             || m_type == SharedCacheType.ReadWrite;

        public bool CanWrite => m_type == SharedCacheType.WriteOnly
                             || m_type == SharedCacheType.ReadWrite;

        public bool IsReadWrite => m_type == SharedCacheType.ReadWrite;

        public bool IsLocked => m_cache.IsShared;

        public object HardwareBuffer => m_hardwareBuffer;

        internal DeviceSharedBuffer(DeviceBuffer cache, SharedCacheType type, TDeviceSharedBackend backend, int flags, object? config) {
            m_cache = cache;
            m_type = type;
            m_hardwareBuffer = new object();
            m_backend = backend;
        }

        public bool Begin() {
            m_cache.Lock();

            if ( m_type != SharedCacheType.ReadOnly ) {
                m_backend.CreateWriteHardwareBuffer(m_cache.ToArray(), out m_hardwareBuffer);
            } else {
                m_backend.CreateReadHardwareBuffer(m_cache.Length, out m_hardwareBuffer);
            }

            return true;
        }

        public void End() {

            if ( m_type != SharedCacheType.ReadOnly ) {
                byte[] cahetmp;
                long startPos = m_backend.ReciveFromHardwareBuffer(out cahetmp, ref m_hardwareBuffer);

                m_cache.Write((int)startPos, cahetmp); 
            } else {
                m_backend.CloseHardwareBuffer(ref m_hardwareBuffer);
            }
            m_cache.Unlock();
        }
    }

}
