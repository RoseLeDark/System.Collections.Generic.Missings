using SystemEx.Utils;
using System.Text;
using SystemEx.Collection.Generic;

namespace SystemEx {
    public enum RandPasswordLevel {
        Simple = 16,
        Strong = 32,
    }
    public static class RandUtils {
        static readonly Random r = new Random((int)DateTime.Now.ToBinary());

        #region Char
        private static char[] PasswordChars = {
            'A','B','C','D','E','F','G','H','I','J','K','L','M',
            'N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
            'a','b','c','d','e','f','g','h','i','j','k','l','m',
            'n','o','p','q','r','s','t','u','v','w','x','y','z',
            '0','1','2','3','4','5','6','7','8','9',
            '!','@','#','$','%','&','*','?','+','-','_'
        };
        public static readonly char[] StrongPasswordChars = {
            // Uppercase
            'A','B','C','D','E','F','G','H','I','J','K','L','M',
            'N','O','P','Q','R','S','T','U','V','W','X','Y','Z',

            // Lowercase
            'a','b','c','d','e','f','g','h','i','j','k','l','m',
            'n','o','p','q','r','s','t','u','v','w','x','y','z',

            // Digits
            '0','1','2','3','4','5','6','7','8','9',

            // Symbols (sicher, keine Whitespaces, keine Quotes)
            '!','@','#','$','%','&','*','?','+','-','_','=',':'
        };

        public static string RandPassword(int length, FixedArray<char> allowed, Endian endian) {

            StringBuilder sb = new StringBuilder(length);

            int i = 0, d = 0;
            char c = '\0';
            while(i < length )  {
                c = RandChar((char)0, (char)short.MaxValue, endian);
                if ( allowed.TryGet(c, out d) ) {
                    i++;
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        public static string Rand(this string a, RandPasswordLevel level, Endian endian) {
            FixedArray<char> allowed;

            if ( level == RandPasswordLevel.Simple)
                allowed = new FixedArray<char>(RandUtils.PasswordChars);
            else
                allowed = new FixedArray<char>(RandUtils.StrongPasswordChars);

            return RandPassword((int)level, allowed, endian);
        }


        public static char RandAExtended(this char a, Endian endian) {
            return RandChar((char)0, (char)255, endian);
        }
        public static char RandAscii(this char a, Endian endian) {
            return RandChar('\x20', '\x7F', endian);
        }
        public static char RandChar(char min, char max, Endian endian) {
            char _c = (char)RandShort((short)min, (short)max, endian);

            return _c;
        }
        #endregion
        #region BYTE
        public static byte Rand(this byte a, Endian endian) {
            return RandByte(byte.MinValue, byte.MaxValue, endian);
        }
        public static byte Rand(this byte a, byte min, Endian endian) {
            return RandByte(min, byte.MaxValue, endian);
        }
        public static byte Rand(this byte a, byte min, byte max, Endian endian) {
            return RandByte(min, max, endian);
        }

        public static byte RandByte(byte min, byte max, Endian endian) {
            if ( min > max ) {
                byte tmp = min;
                min = max;
                max = tmp;
            }

            byte _h = (byte)GetArray(1)[0];
            int range = (max - min) + 1;

            return (byte)(min + (_h % range));
        }

        #endregion

        #region INT
        public static int Rand(this int a, Endian endian) {
            return RandInt(int.MinValue, int.MaxValue, endian);
        }
        public static int Rand(this int a, int min, Endian endian) {
            return RandInt(min, int.MaxValue, endian);
        }
        public static int Rand(this int a, int min, int max, Endian endian) {
            return RandInt(min, max, endian);
        }

        public static uint Rand(this uint a, Endian endian) {
            return RandUInt(uint.MinValue, uint.MaxValue, endian);
        }
        public static uint Rand(this uint a, uint min, Endian endian) {
            return RandUInt(min, uint.MaxValue, endian);
        }
        public static uint Rand(this uint a, uint min, uint max, Endian endian) {
            return RandUInt(min, max, endian);
        }
        public static uint RandUInt(uint min, uint max, Endian endian) {
            if ( min > max ) {
                uint tmp = min;
                min = max;
                max = tmp;
            }

            uint _h = GetArray(4).ToUInt(endian);
            uint range = (max - min) + 1;

            return min + (_h % range);
        }
        public static int RandInt(int min, int max, Endian endian) {
            if ( min > max ) {
                int tmp = min;
                min = max;
                max = tmp;
            }

            int _h = GetArray(4).ToInt(endian);
            int range = (int)(max - min) + 1;

            return min + (_h % range);
        }

        #endregion

        #region Short
        public static short Rand(this short a, Endian endian) {
            return RandShort(short.MinValue, short.MaxValue, endian);
        }
        public static short Rand(this short a, short min, Endian endian) {
            return RandShort(min, short.MaxValue, endian);
        }
        public static short Rand(this short a, short min, short max, Endian endian) {
            return RandShort(min, max, endian);
        }

        public static ushort Rand(this ushort a, Endian endian) {
            return RandUShort(ushort.MinValue, ushort.MaxValue, endian);
        }
        public static ushort Rand(this ushort a, ushort min, Endian endian) {
            return RandUShort(min, ushort.MaxValue, endian);
        }
        public static ushort Rand(this ushort a, ushort min, ushort max, Endian endian) {
            return RandUShort(min, max, endian);
        }
        public static ushort RandUShort(ushort min, ushort max, Endian endian) {
            if ( min > max ) {
                ushort tmp = min;
                min = max;
                max = tmp;
            }

            ushort _h = (ushort)GetArray(2).ToUInt(endian);
            int range = (max - min) + 1;

            return (ushort)(min + (_h % range));
        }
        public static short RandShort(short min, short max, Endian endian) {
            if ( min > max ) {
                short tmp = min;
                min = max;
                max = tmp;
            }

            short _h = (short)GetArray(2).ToInt(endian);
            int range = (max - min) + 1;

            return (short)(min + (_h % range));
        }

        #endregion

        #region LONG    
        public static long Rand(this long a, Endian endian) {
            return RandLong(long.MinValue, long.MaxValue, endian);
        }
        public static long Rand(this long a, long min, Endian endian) {
            return RandLong(min, long.MaxValue, endian);
        }
        public static long Rand(this long a, long min, long max, Endian endian) {
            return RandLong(min, max, endian);
        }

        public static ulong Rand(this ulong a, Endian endian) {
            return RandULong(ulong.MinValue, ulong.MaxValue, endian);
        }
        public static ulong Rand(this ulong a, ulong min, Endian endian) {
            return RandULong(min, ulong.MaxValue, endian);
        }
        public static ulong Rand(this ulong a, ulong min, ulong max, Endian endian) {
            return RandULong(min, max, endian);
        }
        public static ulong RandULong(ulong min, ulong max, Endian endian) {
            if ( min > max )
                throw new ArgumentException("min must be <= max");

            // 1. 8 zufällige Bytes erzeugen
            byte[] _array_h = GetArray(4);
            byte[] _array_l = GetArray(4);

            // 3. Range-Mapping (raw % range)
            ulong range = (ulong)(max - min + 1);
            ulong value = ((_array_h.ToUInt(endian) << 32) | _array_l.ToUInt(endian)) % range;

            // 4. Offset hinzufügen
            return (ulong)(min + (ulong)value);
        }
        public static long RandLong(long min, long max, Endian endian) {
            if ( min > max ) {
                long tmp = min;
                min = max;
                max = tmp;
            }

            int _h = GetArray(4).ToInt(endian);
            uint _l = GetArray(4).ToUInt(endian);

            ulong raw = ((ulong)(uint)_h << 32) | _l;

            ulong range = (ulong)(max - min) + 1;

            long value = (long)(raw % range);

            return min + value;
        }


        #endregion

        public static byte[] GetArray(int size) {
            byte[] buffer = new byte[size];
            r.NextBytes(buffer);
            return buffer;
        }

        public static byte[] GetArray(int size, byte min, byte max) {
            if ( min > max )
                throw new ArgumentException("min must be <= max");

            byte[] buffer = new byte[size];

            for ( int i = 0; i < size; i++ ) {
                buffer[i] = (byte)r.Next(min, max + 1);
            }

            return buffer;
        }


    }
}
