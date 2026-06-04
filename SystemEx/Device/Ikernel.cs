using System;
using System.Collections.Generic;
using System.Text;
using SystemEx.Device.Intertropt;

namespace SystemEx.Device.Memory {

    public enum BufferType {
        Write,
        Read,
        Both
    }
    public interface IKernel<TIDeviceSharedBackend> where TIDeviceSharedBackend : IDeviceSharedBackend {
        public int AddBuffer(DeviceSharedBuffer<TIDeviceSharedBackend> buffer, string name, BufferType type, object? confgs);
        public int RemoveBuffer(string name, BufferType type);
        public int RemoveBuffer(int  index, BufferType type);

        public bool Begin(string strFunction);
        public bool Run(object? options);
        public bool IsRunning();
        public void End();
    }

}
