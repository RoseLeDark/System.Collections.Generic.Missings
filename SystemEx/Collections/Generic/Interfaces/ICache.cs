using System;
using System.Collections.Generic;
using System.Text;
using SystemEx.Utils;

namespace SystemEx.Collection.Generic.Interfaces {
    public  interface ICache {
        public int Length { get; }

        public ulong LongLength { get; }
        public bool IsEmpty { get; }
        public ulong Seek(SeekOrigin org, int pos);

        public int Write(ulong position, char value);
        public int Write(ulong position, byte value);
        public int Write(ulong position, uint value, Endian endian);
        public int Write(ulong position, int value, Endian endian);
        public int Write(ulong position, short value, Endian endian);
        public int Write(ulong position, ushort value, Endian endian);
        public int Write(ulong position, long value, Endian endian);
        public int Write(ulong position, ulong value, Endian endian);
        public int Write(ulong position, float value, Endian endian);
        public int Write(ulong position, double value, Endian endian);

        public uint ReadUInt(ulong position, Endian endian);
        public int ReadInt(ulong position, Endian endian);
        public short ReadShort(ulong position, Endian endian);
        public ushort ReadUShort(ulong position, Endian endian);

        public long ReadLong(ulong position, Endian endian);
        public ulong ReadULong(ulong position, Endian endian);

        public char ReadChar(ulong position);
        public float ReadFloat(ulong position, Endian endian);
        public double ReadDouble(ulong position, Endian endian);

        
        public int Write(byte data);
        public byte Read(ulong position);
        public ulong WriteRange(ulong position, byte[] data);
        public ulong WriteRange(ulong start, ulong iend, byte[] data);

        public byte[]? ReadRange(ulong position, uint count);

        public byte[] ToArray();
    }
}
