using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using SystemEx.Collection.Generic.Interfaces;
using SystemEx.Utils;

namespace SystemEx.Collection.Generic{
    public enum CacheType {
        ToDevice,
        FromDevice,
        Both
    }

    public class CacheIsSharedException : Exception {
        public CacheIsSharedException() : base() { }
    }

    public class Cache : ICache {
        private FixedArray<byte> m_rawBuffer;
        private ulong m_position;
        private bool m_isLocked;
       
        protected bool IsLocked { get { return m_isLocked; }  set => m_isLocked = value;  }
        public byte this[int adress] {
            get {  return m_rawBuffer[adress]; }
            set {  m_rawBuffer[adress] = value; }
        }
        public virtual int Length => m_rawBuffer.Size;

        public virtual ulong LongLength { get; internal set; }
        public bool IsEmpty => LongLength == 0;

        public CacheType Type { get; private set; }

        public ulong Position { get => m_position; internal set => m_position = value; }
        public Cache(int capacity, CacheType type) {
            m_rawBuffer = new FixedArray<byte>(capacity);
            this.Type = type;
            LongLength = (ulong)m_rawBuffer.Size;
        }

        public ulong Seek(SeekOrigin org, int pos) {
            if ( m_rawBuffer.Size == 0 ) return 0;

            switch ( org ) {

            case SeekOrigin.Begin:
                m_position = (ulong)Math.Max(pos, 0);
                break;
            case SeekOrigin.Current:
                if ( pos >= 0 )
                    m_position += (ulong)pos;
                else {
                    ulong neg = (ulong)(-pos);
                    m_position = (m_position > neg) ? (m_position - neg) : 0;
                }
                break;
            case SeekOrigin.End:
                if ( pos >= 0 ) {
                    ulong p = (ulong)pos;
                    m_position = (p > LongLength) ? 0 : (LongLength - p);
                } else {
                    // End + negative pos = End - (-pos) = End + pos
                    ulong add = (ulong)(-pos);
                    m_position = LongLength + add; // wird unten gekappt
                }
                break;
            }

            if ( m_position > LongLength )
                m_position = LongLength;

            return m_position;
        }


        public virtual ulong WriteRange(ulong position, byte[] data) {

            return WriteRange(position, (ulong)data.LongLength, data);
        }
        public virtual ulong WriteRange(ulong start, ulong end, byte[] data) {
            if ( m_isLocked )
                throw new InvalidOperationException("is Locked");

            // Start ungültig?
            if ( start < 0 || start >= (ulong)m_rawBuffer.Size )
                return 0;

            // End über Größe → kappen
            if ( end > (ulong)m_rawBuffer.Size )
                end = (ulong)m_rawBuffer.Size;

            // Bereich ungültig?
            if ( end <= start )
                return 0;

            ulong rangeLen = end - start;
            ulong writable = Math.Min((uint)rangeLen, (uint)data.Length);

            for ( ulong i = 0; i < (ulong)writable; i++ )
                m_rawBuffer[(int)(start + i)] = data[i];

            return (ulong)writable;
        }
        

        public static unsafe byte[] ToBytes<T>(ref T value) where T : unmanaged {
            
            int size = sizeof(T);
            byte[] data = new byte[size];

            fixed ( T* pValue = &value )
            fixed ( byte* pData = data ) {
                Buffer.MemoryCopy(pValue, pData, size, size);
            }

            return data;
        }


        

        public int Write(ulong position, uint value, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = value.ToBytes(endian);

            return (int)WriteRange(position, b);
        }

        public int Write(ulong position, int value, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = value.ToBytes(endian);

            return (int)WriteRange(position, b);
        }
        public int Write(ulong position, short value, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = value.ToBytes(endian);

            return (int)WriteRange(position, b);
        }

        public int Write(ulong position, char value) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = value.ToBytes(Endian.LittleEndian);

            return (int)WriteRange(position, b);
        }

        public int Write(ulong position, byte value) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = value.ToBytes(Endian.LittleEndian);

            return (int)WriteRange(position, b);
        }

        public int Write(ulong position, ushort value, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = value.ToBytes(endian);

            return (int)WriteRange(position, b);
        }

        public int Write(ulong position, long value, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = value.ToBytes(endian);

            return (int)WriteRange(position, b);
        }

        public int Write(ulong position, ulong value, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = value.ToBytes(endian);

            return (int)WriteRange(position, b);
        }

        public int Write(ulong position, float value, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = value.ToBytes(endian);

            return (int)WriteRange(position, b);
        }

        public int Write(ulong position, double value, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = value.ToBytes(endian);

            return (int)WriteRange(position, b);
        }

        public uint ReadUInt(ulong position, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = new byte[4];

            b[0] = m_rawBuffer[(int)position + 0];
            b[1] = m_rawBuffer[(int)position + 1];
            b[2] = m_rawBuffer[(int)position + 2];
            b[3] = m_rawBuffer[(int)position + 3];

            return b.ToUInt(endian);
        }

        public int ReadInt(ulong position, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = new byte[4];
            int _pos = (int)position;

            b[0] = m_rawBuffer[_pos + 0];
            b[1] = m_rawBuffer[_pos + 1];
            b[2] = m_rawBuffer[_pos + 2];
            b[3] = m_rawBuffer[_pos + 3];

            return b.ToInt(endian);
        }

        public short ReadShort(ulong position, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = new byte[2];
            int _pos = (int)position;

            b[0] = m_rawBuffer[_pos + 0];
            b[1] = m_rawBuffer[_pos + 1];

            return b.ToShort(endian);
        }

        public ushort ReadUShort(ulong position, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = new byte[2];
            int _pos = (int)position;

            b[0] = m_rawBuffer[_pos + 0];
            b[1] = m_rawBuffer[_pos + 1];

            return b.ToUShort(endian);
        }

        public long ReadLong(ulong position, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = new byte[8];
            int _pos = (int)position;

            b[0] = m_rawBuffer[_pos + 0];
            b[1] = m_rawBuffer[_pos + 1];
            b[2] = m_rawBuffer[_pos + 2];
            b[3] = m_rawBuffer[_pos + 3];
            b[4] = m_rawBuffer[_pos + 4];
            b[5] = m_rawBuffer[_pos + 5];
            b[6] = m_rawBuffer[_pos + 6];
            b[7] = m_rawBuffer[_pos + 7];

            return b.ToLong(endian);
        }

        public ulong ReadULong(ulong position, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = new byte[8];
            int _pos = (int)position;

            b[0] = m_rawBuffer[_pos + 0];
            b[1] = m_rawBuffer[_pos + 1];
            b[2] = m_rawBuffer[_pos + 2];
            b[3] = m_rawBuffer[_pos + 3];
            b[4] = m_rawBuffer[_pos + 4];
            b[5] = m_rawBuffer[_pos + 5];
            b[6] = m_rawBuffer[_pos + 6];
            b[7] = m_rawBuffer[_pos + 7];

            return b.ToULong(endian);
        }

        public char ReadChar(ulong position) {
            byte b = m_rawBuffer[(int)position];
            return (char)b;
        }

        public float ReadFloat(ulong position, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = new byte[4];

            b[0] = m_rawBuffer[(int)position + 0];
            b[1] = m_rawBuffer[(int)position + 1];
            b[2] = m_rawBuffer[(int)position + 2];
            b[3] = m_rawBuffer[(int)position + 3];

            return b.ToFloat(endian);
        }

        public double ReadDouble(ulong position, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = new byte[8];

            b[0] = m_rawBuffer[(int)position + 0];
            b[1] = m_rawBuffer[(int)position + 1];
            b[2] = m_rawBuffer[(int)position + 2];
            b[3] = m_rawBuffer[(int)position + 3];
            b[4] = m_rawBuffer[(int)position + 4];
            b[5] = m_rawBuffer[(int)position + 5];
            b[6] = m_rawBuffer[(int)position + 6];
            b[7] = m_rawBuffer[(int)position + 7];

            return b.ToDouble(endian);
        }

        public int Write(byte data) {
            var written = WriteRange(m_position, m_position + 1, new byte[1] { data } );
            m_position += written;
            return (int)written;
        }

        public byte Read(ulong position) {
            return m_rawBuffer[(int)position];
        }

        public virtual byte[]? ReadRange(ulong position, uint count) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            if ( (int)position + count > m_rawBuffer.Size ) return null;

            byte[] result = new byte[count];
            m_rawBuffer.CopyTo((uint)position, result, 0, (uint)count);
            return result;
        }
        public byte[] ToArray() {
            if ( m_isLocked ) throw new CacheIsSharedException();
            return m_rawBuffer.ToArray();
        }

        
    }
}
