using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Device.Intertropt {
    public interface IDeviceSharedBackend {
        public long CreateReadHardwareBuffer(int size, out object hardwareBuffer);
        public long CreateWriteHardwareBuffer(byte[] cache, out object hardwareBuffer);
        public long ReciveFromHardwareBuffer(out byte[] data, ref object hardwareBuffer);
        public void CloseHardwareBuffer(ref object hardwareBuffer);
    }

}
