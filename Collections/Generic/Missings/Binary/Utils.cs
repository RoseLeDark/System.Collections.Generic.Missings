


namespace System.Missings.Binary {
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


        public unsafe static int ToInt(this byte[] bytes, Endian endian) {
            if ( endian == Endian.BigEndian ) {
                byte tmp;
                tmp = bytes[0]; bytes[0] = bytes[3]; bytes[3] = tmp;
                tmp = bytes[1]; bytes[1] = bytes[2]; bytes[2] = tmp;
            }

            fixed ( byte* b = bytes )
                return *(int*)b;
        }

        public unsafe static uint ToUInt(this byte[] bytes, Endian endian) {
            if ( endian == Endian.BigEndian ) {
                byte tmp;
                tmp = bytes[0]; bytes[0] = bytes[3]; bytes[3] = tmp;
                tmp = bytes[1]; bytes[1] = bytes[2]; bytes[2] = tmp;
            }

            fixed ( byte* b = bytes )
                return *(uint*)b;
        }


        public unsafe static short ToShort(this byte[] bytes, Endian endian) {
            if ( endian == Endian.BigEndian ) {
                byte tmp = bytes[0];
                bytes[0] = bytes[1];
                bytes[1] = tmp;
            }

            fixed ( byte* b = bytes )
                return *(short*)b;
        }

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



        

        
        public static uint SizeCalc(string value) {
            string str = value.ToUpper(Globalization.CultureInfo.CurrentCulture).Trim();
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
