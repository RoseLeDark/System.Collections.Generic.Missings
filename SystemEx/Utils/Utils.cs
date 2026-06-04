


namespace SystemEx.Utils {
    public enum Endian {
        LittleEndian,
        BigEndian
    }

    public static class Utils {
        public static int ToBoundary(this uint number, uint boundary) {
            if ( boundary == 0 ) return (int)number;
            uint div = number / boundary;
            uint mod = number % boundary;
            return (int)(mod == 0 ? div * boundary : (div + 1) * boundary);
        }

        #region BYTE
        public unsafe static byte[] ToBytes(this byte value, Endian endian) {
            byte[] bytes = new byte[1];
            bytes[0] = (byte)value;
            return bytes;
        }

        public static byte ToByte(this byte[] bytes, Endian endian) {
            if ( bytes.Length < 1 ) throw new ArgumentException("byte requires exactly 1 byte");

            return (byte)bytes[0];
        }
        #endregion

        #region INT
        public unsafe static byte[] ToBytes(this uint value, Endian endian) {
            byte[] bytes = new byte[4];

            fixed ( byte* b = bytes )
                *(uint*)b = value;

            if ( endian == Endian.BigEndian ) {
                // reverse in-place
                byte tmp;
                tmp = bytes[0]; bytes[0] = bytes[3]; bytes[3] = tmp;
                tmp = bytes[1]; bytes[1] = bytes[2]; bytes[2] = tmp;
            }

            return bytes;
        }


        public unsafe static byte[] ToBytes(this int value, Endian endian) {
            byte[] bytes = new byte[4];

            fixed ( byte* b = bytes )
                *(int*)b = value;

            if ( endian == Endian.BigEndian ) {
                byte tmp;
                tmp = bytes[0]; bytes[0] = bytes[3]; bytes[3] = tmp;
                tmp = bytes[1]; bytes[1] = bytes[2]; bytes[2] = tmp;
            }

            return bytes;
        }
        public unsafe static int ToInt(this byte[] bytes, Endian endian) {
            if ( bytes.Length < 4 ) throw new ArgumentException("uint requires exactly 4 bytes");

            if ( endian == Endian.BigEndian ) {
                byte tmp;
                tmp = bytes[0]; bytes[0] = bytes[3]; bytes[3] = tmp;
                tmp = bytes[1]; bytes[1] = bytes[2]; bytes[2] = tmp;
            }

            fixed ( byte* b = bytes )
                return *(int*)b;
        }
        public unsafe static uint ToUInt(this byte[] bytes, Endian endian) {
            if ( bytes.Length < 4 ) throw new ArgumentException("uint requires exactly 4 bytes");

            if ( endian == Endian.BigEndian ) {
                byte tmp;
                tmp = bytes[0]; bytes[0] = bytes[3]; bytes[3] = tmp;
                tmp = bytes[1]; bytes[1] = bytes[2]; bytes[2] = tmp;
            }

            fixed ( byte* b = bytes )
                return *(uint*)b;
        }

        #endregion

        #region SHORT
        public unsafe static byte[] ToBytes(this short value, Endian endian) {
            byte[] bytes = new byte[2];

            fixed ( byte* b = bytes )
                *(short*)b = value;

            if ( endian == Endian.BigEndian ) {
                byte tmp = bytes[0];
                bytes[0] = bytes[1];
                bytes[1] = tmp;
            }

            return bytes;
        }
        public unsafe static byte[] ToBytes(this ushort value, Endian endian) {
            byte[] bytes = new byte[2];

            fixed ( byte* b = bytes )
                *(ushort*)b = value;

            if ( endian == Endian.BigEndian ) {
                byte tmp = bytes[0];
                bytes[0] = bytes[1];
                bytes[1] = tmp;
            }

            return bytes;
        }

        public unsafe static ushort ToUShort(this byte[] bytes, Endian endian) {
            if ( bytes.Length < 2) throw new ArgumentException("short requires exactly 2 bytes");

            if ( endian == Endian.BigEndian ) {
                byte tmp = bytes[0];
                bytes[0] = bytes[1];
                bytes[1] = tmp;
            }

            fixed ( byte* b = bytes )
                return *(ushort*)b;
        }

        
        public unsafe static short ToShort(this byte[] bytes, Endian endian) {
            if ( bytes.Length < 2 ) throw new ArgumentException("short requires exactly 2 bytes");

            if ( endian == Endian.BigEndian ) {
                byte tmp = bytes[0];
                bytes[0] = bytes[1];
                bytes[1] = tmp;
            }

            fixed ( byte* b = bytes )
                return *(short*)b;
        }

        #endregion

        #region STRUCT
        public static unsafe byte[] ToBytes<T>(this T value, Endian endian) where T : unmanaged {
            int size = sizeof(T);
            byte[] bytes = new byte[size];

            fixed ( byte* b = bytes ) {
                *(T*)b = value;
            }

            if ( endian == Endian.BigEndian )
                Array.Reverse(bytes);

            return bytes;
        }
        public static unsafe byte[] ToBytes<T>(this T[] array) where T : unmanaged {
            int size = sizeof(T) * array.Length;
            byte[] bytes = new byte[size];

            fixed ( T* src = array )
            fixed ( byte* dst = bytes ) {
                Buffer.MemoryCopy(src, dst, size, size);
            }

            return bytes;
        }
        public static unsafe T FromBytes<T>(byte[] bytes) where T : unmanaged {
            T value = default;

            int size = sizeof(T);
            if ( bytes.Length < size )
                throw new ArgumentException($"Byte array too small for type {typeof(T).Name}");

            fixed ( byte* b = bytes ) {
                value = *(T*)b;
            }

            return value;
        }
        public static unsafe T[] FromBytesArray<T>(byte[] bytes) where T : unmanaged {
            int size = sizeof(T);
            int count = bytes.Length / size;

            T[] array = new T[count];

            fixed ( byte* src = bytes )
            fixed ( T* dst = array ) {
                Buffer.MemoryCopy(src, dst, bytes.Length, bytes.Length);
            }

            return array;
        }
        #endregion

        #region FLOAT
        public unsafe static byte[] ToBytes(this float value, Endian endian) {
            byte[] bytes = new byte[4];

            fixed ( byte* b = bytes )
                *(float*)b = value;

            if ( endian == Endian.BigEndian ) {
                byte tmp;
                tmp = bytes[0]; bytes[0] = bytes[3]; bytes[3] = tmp;
                tmp = bytes[1]; bytes[1] = bytes[2]; bytes[2] = tmp;
            }

            return bytes;
        }

        public unsafe static float ToFloat(this byte[] bytes, Endian endian) {
            if ( bytes.Length < 4 ) throw new ArgumentException("Float requires exactly 4 bytes");

            if ( endian == Endian.BigEndian ) {
                byte tmp;
                tmp = bytes[0]; bytes[0] = bytes[3]; bytes[3] = tmp;
                tmp = bytes[1]; bytes[1] = bytes[2]; bytes[2] = tmp;
            }

            fixed ( byte* b = bytes )
                return *(float*)b;
        }
        #endregion

        #region DOUBLE
        public unsafe static byte[] ToBytes(this double value, Endian endian) {
            byte[] bytes = new byte[8];

            fixed ( byte* b = bytes )
                *(double*)b = value;

            if ( endian == Endian.BigEndian ) {
                // reverse 8 bytes
                byte tmp;
                tmp = bytes[0]; bytes[0] = bytes[7]; bytes[7] = tmp;
                tmp = bytes[1]; bytes[1] = bytes[6]; bytes[6] = tmp;
                tmp = bytes[2]; bytes[2] = bytes[5]; bytes[5] = tmp;
                tmp = bytes[3]; bytes[3] = bytes[4]; bytes[4] = tmp;
            }

            return bytes;
        }
        public unsafe static double ToDouble(this byte[] bytes, Endian endian) {
            if ( bytes.Length < 8 ) throw new ArgumentException("Double requires at least 8 bytes");

            if ( endian == Endian.BigEndian ) {
                byte tmp;
                tmp = bytes[0]; bytes[0] = bytes[7]; bytes[7] = tmp;
                tmp = bytes[1]; bytes[1] = bytes[6]; bytes[6] = tmp;
                tmp = bytes[2]; bytes[2] = bytes[5]; bytes[5] = tmp;
                tmp = bytes[3]; bytes[3] = bytes[4]; bytes[4] = tmp;
            }

            fixed ( byte* b = bytes )
                return *(double*)b;
        }

        #endregion



        #region LONG
        public unsafe static byte[] ToBytes(this long value, Endian endian) {
            byte[] bytes = new byte[8];

            fixed ( byte* b = bytes )
                *(double*)b = value;

            if ( endian == Endian.BigEndian ) {
                // reverse 8 bytes
                byte tmp;
                tmp = bytes[0]; bytes[0] = bytes[7]; bytes[7] = tmp;
                tmp = bytes[1]; bytes[1] = bytes[6]; bytes[6] = tmp;
                tmp = bytes[2]; bytes[2] = bytes[5]; bytes[5] = tmp;
                tmp = bytes[3]; bytes[3] = bytes[4]; bytes[4] = tmp;
            }

            return bytes;
        }
        public unsafe static long ToLong(this byte[] bytes, Endian endian) {
            if ( bytes.Length < 8 ) throw new ArgumentException("long requires at least 8 bytes");

            if ( endian == Endian.BigEndian ) {
                byte tmp;
                tmp = bytes[0]; bytes[0] = bytes[7]; bytes[7] = tmp;
                tmp = bytes[1]; bytes[1] = bytes[6]; bytes[6] = tmp;
                tmp = bytes[2]; bytes[2] = bytes[5]; bytes[5] = tmp;
                tmp = bytes[3]; bytes[3] = bytes[4]; bytes[4] = tmp;
            }

            fixed ( byte* b = bytes )
                return *(long*)b;
        }

        public unsafe static byte[] ToBytes(this ulong value, Endian endian) {
            byte[] bytes = new byte[8];

            fixed ( byte* b = bytes )
                *(double*)b = value;

            if ( endian == Endian.BigEndian ) {
                // reverse 8 bytes
                byte tmp;
                tmp = bytes[0]; bytes[0] = bytes[7]; bytes[7] = tmp;
                tmp = bytes[1]; bytes[1] = bytes[6]; bytes[6] = tmp;
                tmp = bytes[2]; bytes[2] = bytes[5]; bytes[5] = tmp;
                tmp = bytes[3]; bytes[3] = bytes[4]; bytes[4] = tmp;
            }

            return bytes;
        }
        public unsafe static ulong ToULong(this byte[] bytes, Endian endian) {
            if ( bytes.Length < 8 ) throw new ArgumentException("ulong requires at least 8 bytes");

            if ( endian == Endian.BigEndian ) {
                byte tmp;
                tmp = bytes[0]; bytes[0] = bytes[7]; bytes[7] = tmp;
                tmp = bytes[1]; bytes[1] = bytes[6]; bytes[6] = tmp;
                tmp = bytes[2]; bytes[2] = bytes[5]; bytes[5] = tmp;
                tmp = bytes[3]; bytes[3] = bytes[4]; bytes[4] = tmp;
            }

            fixed ( byte* b = bytes )
                return *(ulong*)b;
        }
        #endregion




        public static uint SizeCalc(string value) {
            string str = value.ToUpper(System.Globalization.CultureInfo.CurrentCulture).Trim();
            uint multiplier = 1;

            if ( str.EndsWith("G", StringComparison.OrdinalIgnoreCase) ) {
                multiplier = 1024u * 1024u * 1024u;
                str = str[..^1];
            } else if ( str.EndsWith("M", StringComparison.OrdinalIgnoreCase) ) {
                multiplier = 1024u * 1024u;
                str = str[..^1];
            } else if ( str.EndsWith("K", StringComparison.OrdinalIgnoreCase) ) {
                multiplier = 1024u;
                str = str[..^1];
            } else if ( str.EndsWith("B", StringComparison.OrdinalIgnoreCase) ) {
                multiplier = 1;
                str = str[..^1];
            }

            return uint.TryParse(str, out uint size)
                ? size * multiplier
                : 512 * multiplier;
        }
    }

}
