using System;
using System.Buffers;
using System.Collections.Generic;
using System.Drawing;
using System.Missings.Binary;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Collections.Generic.Missings {
    public enum CacheType {
        ToDevice,
        FromDevice,
        Both
    }

    public class CacheIsSharedException : Exception {
        public CacheIsSharedException() : base() { }
    }

    public class Cache {
        private Array<byte> m_rawBuffer;
        private int m_position;
        private Lock  m_lockObj = new Lock ();
        private bool m_isLocked;
       

        public byte this[int adress] {
            get {  return m_rawBuffer[adress]; }
            set {  m_rawBuffer[adress] = value; }
        }
        public bool IsLocked { get => m_isLocked; }
        public int Length => m_rawBuffer.Size;
        public bool IsEmpty => m_rawBuffer.Size == 0;

        public CacheType Type { get; private set; }

        public Cache(int capacity, CacheType type) {
            m_rawBuffer = new Array<byte>(capacity);
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
        public SharedCache? ToShared() {
            m_lockObj.EnterScope();
            m_isLocked = true;

            SharedCache? _return = null;

            switch ( this.Type ) {
            case CacheType.ToDevice: 
                _return = new SharedCache(this, SharedCacheType.ReadOnly); break;
            case CacheType.FromDevice: 
                _return = new SharedCache(this, SharedCacheType.WriteOnly);
                break;
            case CacheType.Both: 
            default: 
                _return = new SharedCache(this, SharedCacheType.ReadWrite); 
                break;
            }
            return _return;        
        }
        internal bool SharedCallBack(SharedCache sender, byte[] data, int offset, int code ) {
            if ( code == 0 ) {
                m_rawBuffer.InsertRange( offset, data );
            }


            // Fehlerfall
            m_isLocked = false;
            m_lockObj.Exit();
            return false;
        }

        public int WriteUInt32(int position, uint value, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            if ( position < 0 || position + 4 > m_rawBuffer.Size )
                return 0;

            byte[] b = value.ToBytes(endian);

            m_rawBuffer[position + 0] = b[0];
            m_rawBuffer[position + 1] = b[1];
            m_rawBuffer[position + 2] = b[2];
            m_rawBuffer[position + 3] = b[3];

            return 4;
        }

        public int WriteInt32(int position, int value, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            if ( position < 0 || position + 4 > m_rawBuffer.Size )
                return 0;

            byte[] b = value.ToBytes(endian);


            m_rawBuffer[position + 0] = b[0];
            m_rawBuffer[position + 1] = b[1];
            m_rawBuffer[position + 2] = b[2];
            m_rawBuffer[position + 3] = b[3];

            return 4;
        }
        public int WriteInt16(int position, short value, Endian endian) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            if ( position < 0 || position + 2 > m_rawBuffer.Size )
                return 0;

            byte[] b = value.ToBytes(endian);

            m_rawBuffer[position + 0] = b[0];
            m_rawBuffer[position + 1] = b[1];

            return 2;
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
        public int Write(int position, byte[] buffer) {
            if ( m_isLocked ) throw new InvalidOperationException("is Locked");

            if ( position < 0 || position >= m_rawBuffer.Size )
                return 0;

            int writable = Math.Min(buffer.Length, m_rawBuffer.Size - position);

            for ( int i = 0; i < writable; i++ )
                m_rawBuffer[position + i] = buffer[i];

            return writable;
        }


        

        public byte[] ToArray() {
            if ( m_isLocked ) throw new CacheIsSharedException();
            return m_rawBuffer.ToArray();
        }
    }
}
