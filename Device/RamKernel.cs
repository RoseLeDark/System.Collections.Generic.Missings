using SystemEx.Device.Intertropt;
using SystemEx.Device.Memory;
using SystemEx.Collections.Generic;


#if WINDOWS
using KernelLoader = SystemEx.Device.Interop.WindowsKernelLoader;
#elif LINUX
using KernelLoader = SystemEx.Device.Interop.LinuxKernelLoader;
#elif MACOS
using KernelLoader = SystemEx.Device.Interop.MacKernelLoader;
#else
using KernelLoader = SystemEx.Device.Interop.NoSupportKernelLoader;
#endif



namespace SystemEx.Device {

    public class RamKernel : IKernel<RamSharedBackend> {


        private readonly string m_dllPath;
        private readonly Map<string, DeviceSharedBuffer<RamSharedBackend>> m_buffers;
        private Task? m_task;
        private bool m_running;

        private string  m_func;

        public RamKernel(string dllPath, string funcName) {
            m_dllPath = dllPath;
            m_buffers = new Map<string, DeviceSharedBuffer<RamSharedBackend>>();
            m_func = funcName;
        }
        public int AddBuffer(DeviceSharedBuffer<RamSharedBackend> buffer, string name, BufferType type, object? confgs) {
            m_buffers.Add(name, buffer);

            return m_buffers.Count;
        }
        public bool Begin(string strFunction) {
            foreach ( var buf in m_buffers ) {

#if DEBUG
             Console.Write("Buffer {0} begin ... ", buf.First);
#endif
             buf.Second.Begin();

#if DEBUG
            Console.WriteLine("OK");
#endif
            }
            return true;
        }

        public void End() {
            foreach ( var buf in m_buffers ) {

#if DEBUG
                Console.Write("Buffer {0} end ... ", buf.First);
#endif
                buf.Second.End();

#if DEBUG
                Console.WriteLine("OK");
#endif
            }
        }

        public bool IsRunning() {
            return m_running;
        }

        public int RemoveBuffer(string name, BufferType type) {
            return 0;
        }

        public int RemoveBuffer(int index, BufferType type) {
            return 0;
        }

        public unsafe bool Run(object? options) {
            if ( m_running )
                return false; // läuft schon

            m_running = true;

            m_task = Task.Run(() =>
            {
                try {
                    DeviceSharedBuffer < RamSharedBackend >? A = m_buffers[0].Second;
                    DeviceSharedBuffer < RamSharedBackend >? B = m_buffers[1].Second;
                    DeviceSharedBuffer < RamSharedBackend >? C = m_buffers[2].Second;

                    var unmanagedA = (UnmanagedObject)A.HardwareBuffer!;
                    var unmanagedB = (UnmanagedObject)B.HardwareBuffer!;
                    var unmanagedC = (UnmanagedObject)C.HardwareBuffer!;

                    // DLL-Kernel aufrufen
                    unmanagedC.Size = KernelLoader.call(m_dllPath, m_func, unmanagedA.Point, unmanagedA.Size, unmanagedB.Point, unmanagedB.Size, unmanagedC.Point);

                } finally {
                    m_running = false;
                }
            });

            return true;
        }
    }
}
