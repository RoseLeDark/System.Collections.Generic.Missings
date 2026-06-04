using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using SystemEx.Collection.Generic;
using SystemEx.Device.Intertropt;

/** Plan:
 * IKernel OpenCL = new OpenCLKErnel("Main.cl", ....);
DeviceBuffer <OpenCLDeviceSharedBackend> memA = new DeviceBuffer(new OpenCLSharedBackend(..), 32, CacheType.ToDevice);
DeviceBuffer <OpenCLDeviceSharedBackend> memB = new DeviceBuffer(new OpenCLSharedBackend(..), 32, CacheType.ToDevice);
DeviceBuffer <OpenCLDeviceSharedBackend> memC =new DeviceBuffer(new OpenCLSharedBackend(..), 32, CacheType.Both);

memA.WriteInt32(0, 5, System.Missings.Binary.Endian.LittleEndian);
memB.WriteInt32(0, 56, System.Missings.Binary.Endian.LittleEndian);

OpenCL.AddBuffer(memA.ToSharedBuffer(), BufferType.Read); 
OpenCL.AddBuffer(memB.ToSharedBuffer(), BufferType.Read);
OpenCL.AddBuffer(memC.ToSharedBuffer(), BufferType.Both);

//OpenCL.Begin();
OpenCL.Run();

//while( OpenCL .IsRunning()) { }
//OpenCL.End();
*/


namespace SystemEx.Device.Memory {
    public class DeviceBuffer : Cache {

        private Lock  m_lockObj = new Lock ();

        public bool IsShared => IsLocked;
        public DeviceBuffer(int capacity, CacheType type) : base(capacity, type) {

        }

        public DeviceSharedBuffer<TDeviceSharedBackend> ToShared<TDeviceSharedBackend>(TDeviceSharedBackend shared, int flags, object? config) 
            where TDeviceSharedBackend : IDeviceSharedBackend {
            return this.Type switch
            {
                CacheType.ToDevice => new DeviceSharedBuffer<TDeviceSharedBackend>(this, SharedCacheType.ReadOnly, shared, flags, config),
                CacheType.FromDevice => new DeviceSharedBuffer<TDeviceSharedBackend>(this, SharedCacheType.WriteOnly, shared, flags, config),
                _ => new DeviceSharedBuffer<TDeviceSharedBackend>(this, SharedCacheType.ReadWrite, shared, flags, config),
            };
        }
        internal void Lock() {
            m_lockObj.EnterScope();
            IsLocked = true;
        }
        internal void Unlock() {
            m_lockObj.Exit();
            IsLocked = false;
        }
    }
}






