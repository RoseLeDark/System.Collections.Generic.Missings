using System.Runtime.InteropServices;

namespace SystemEx.Device.Intertropt {


    /// <summary>
    /// Represents an unmanaged object that can be pinned in memory.
    /// </summary>
    public struct UnmanagedObject : IDisposable {
        public GCHandle     Handle { get; internal set; }
        public byte[]       Data { get; internal set; }
        public IntPtr       Point { get; internal set; }
        public int          Size {  get; internal set; }

        public UnmanagedObject(GCHandle handle, byte[] data) {
            Handle = handle;
            Point = Handle.AddrOfPinnedObject();
            Data = data;
            Size = data.Length;
        }


        public void Dispose() {
            if ( Handle.IsAllocated ) {
                Handle.Free();
            }
        }
    }

    /// <summary>
    ///     
    /// </summary> 
    public class RamSharedBackend : IDeviceSharedBackend {
        private byte[]? m_Buffer;

        internal RamSharedBackend() {
            m_Buffer = null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="hardwareBuffer"></param>
        /// <returns></returns>
        /// <exception cref="IOException"></exception>
        public long ReciveFromHardwareBuffer(out byte[] data, ref object hardwareBuffer) {
            if ( m_Buffer == null  ) throw new IOException(nameof(m_Buffer));

            long _size = m_Buffer.Length;
            data = m_Buffer;
            ((UnmanagedObject)hardwareBuffer).Dispose();

            m_Buffer = null;

            return _size;
        }
        public void CloseHardwareBuffer(ref object hardwareBuffer) {
            ((UnmanagedObject)hardwareBuffer).Dispose();

            m_Buffer = null;
        }

        public long CreateWriteHardwareBuffer(byte[] cache, out object hardwareBuffer) {

            m_Buffer = cache;
            GCHandle handle = GCHandle.Alloc(m_Buffer, GCHandleType.Pinned);
            hardwareBuffer = new UnmanagedObject(handle, m_Buffer);

            return m_Buffer.Length;
        }
        public long CreateReadHardwareBuffer(int size, out object hardwareBuffer) {
            m_Buffer = new byte[size];

            GCHandle handle = GCHandle.Alloc(m_Buffer, GCHandleType.Pinned);
            hardwareBuffer = new UnmanagedObject(handle, m_Buffer);

            return m_Buffer.Length;
        }
    }
}
