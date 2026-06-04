using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Missings.Binary;
using System.Runtime.InteropServices;
using System.Text;

namespace SystemEx.Collection.Generic{
    public enum CacheType {
        ToDevice,
        FromDevice,
        Both
    }

    public class CacheIsSharedException : Exception {
        public CacheIsSharedException() : base() { }
    }

    public class Cache {
        private FixedArray<byte> m_rawBuffer;
        private int m_position;
        private bool m_isLocked;
       
        protected bool IsLocked { get { return m_isLocked; }  set => m_isLocked = value;  }
        public byte this[int adress] {
            get {  return m_rawBuffer[adress]; }
            set {  m_rawBuffer[adress] = value; }
        }
        public int Length => m_rawBuffer.Size;
        public bool IsEmpty => m_rawBuffer.Size == 0;

        public CacheType Type { get; private set; }

        public Cache(int capacity, CacheType type) {
            m_rawBuffer = new FixedArray<byte>(capacity);
            this.Type = type;
        }

        public int Seek(SeekOrigin org, int pos) {
            if ( m_rawBuffer.Size == 0 ) return 0;

            switch ( org ) {
                case SeekOrigin.Begin: m_position = pos; break;
                case SeekOrigin.Current: m_position += pos; break;
                case SeekOrigin.End:  m_position = (m_rawBuffer.Size) - pos; break;

            }

            if( m_position < 0) m_position = 0;
            else if ( m_position > m_rawBuffer.Size ) m_position = m_rawBuffer.Size;

            return m_position;
        }
        

        public int Write(int position, uint value, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = value.ToBytes(endian);

            return WriteRange(position, b);
        }

        public int Write(int position, int value, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = value.ToBytes(endian);

            return WriteRange(position, b);
        }
        public int Write(int position, short value, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = value.ToBytes(endian);

            return WriteRange(position, b);
        }
        public uint ReadUInt32(int position, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = new byte[4];

            b[0] = m_rawBuffer[position + 0];
            b[1] = m_rawBuffer[position + 1];
            b[2] = m_rawBuffer[position + 2];
            b[3] = m_rawBuffer[position + 3];

            return b.ToUInt(endian); 
        }
        public int ReadInt32(int position, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = new byte[4];

            b[0] = m_rawBuffer[position + 0];
            b[1] = m_rawBuffer[position + 1];
            b[2] = m_rawBuffer[position + 2];
            b[3] = m_rawBuffer[position + 3];

            return b.ToInt(endian);
        }
        public short ReadInt16(int position, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            byte[] b = new byte[2];

            b[0] = m_rawBuffer[position + 0];
            b[1] = m_rawBuffer[position + 1];


            return b.ToShort(endian);
        }



        public byte[]? Read(int position, int count) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            if ( position < 0 || count < 0 ) return null;
            if ( position + count > m_rawBuffer.Size ) return null;

            byte[] result = new byte[count];
            m_rawBuffer.CopyTo(position, result, 0, count);
            return result;
        }

        public int Write(byte[] data) {
            int written = WriteRange(m_position, m_position + data.Length, data);
            m_position += written;
            return written;
        }
        public int WriteRange(int position, byte[] data) {

            return WriteRange(position, data.Length, data);
        }
        public int WriteRange(int start, int end, byte[] data) {
            if ( m_isLocked )
                throw new InvalidOperationException("is Locked");

            // Start ungültig?
            if ( start < 0 || start >= m_rawBuffer.Size )
                return 0;

            // End über Größe → kappen
            if ( end > m_rawBuffer.Size )
                end = m_rawBuffer.Size;

            // Bereich ungültig?
            if ( end <= start )
                return 0;

            int rangeLen = end - start;
            int writable = Math.Min(rangeLen, data.Length);

            for ( int i = 0; i < writable; i++ )
                m_rawBuffer[start + i] = data[i];

            return writable;
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




        public byte[] ToArray() {
            if ( m_isLocked ) throw new CacheIsSharedException();
            return m_rawBuffer.ToArray();
        }
    }
}
