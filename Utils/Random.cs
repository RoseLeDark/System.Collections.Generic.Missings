/* 
 * SPDX-License-Identifier: EUPL-1.2
 *
 * Copyright (c) 2026 Amber-Sophia Schröck <ambersophia.schroeck@mail.de>
 *
 * This file is licensed under the European Union Public Licence (EUPL) version 1.2.
 * You can obtain a copy of the licence at:
 *   https://joinup.ec.europa.eu/collection/eupl/eupl-text-eupl-12
 *
 * Unless required by applicable law or agreed to in writing, software distributed
 * under the Licence is distributed on an "AS IS" basis, WITHOUT WARRANTIES OR
 * CONDITIONS OF ANY KIND, either express or implied.
 *
 * If you modify this file, retain this notice and add a short description of your
 * changes and the date.
 */

using System.Text;
using SystemEx.Collections.Generic;
using SystemEx.Random;

namespace SystemEx {
    /// <summary>
    /// Specifies predefined password lengths for random password generation.
    /// </summary>
    public enum RandPasswordLevel {
        /// <summary>
        /// Generates a 16‑character password using the basic character set.
        /// </summary>
        Simple = 16,

        /// <summary>
        /// Generates a 32‑character password using the extended strong character set.
        /// </summary>
        Strong = 32,
    }
    /// <summary>
    /// Provides random number and random character generation utilities for all
    /// primitive numeric types, including endian‑aware range mapping.  
    /// Also includes password generation using configurable character sets.
    /// </summary>
    public static class RandUtils {
        /// <summary>
        /// Internal pseudo‑random generator used for all random operations.
        /// </summary>
#if !TEST
        static readonly Randx r = new Randx(1,2,3);
#else
        static readonly System.Random r = new System.Random((int)DateTime.Now.ToBinary());
#endif
        #region Char
        /// <summary>
        /// Default character set for simple passwords (letters, digits, symbols).
        /// </summary>
        private static char[] PasswordChars = {
            'A','B','C','D','E','F','G','H','I','J','K','L','M',
            'N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
            'a','b','c','d','e','f','g','h','i','j','k','l','m',
            'n','o','p','q','r','s','t','u','v','w','x','y','z',
            '0','1','2','3','4','5','6','7','8','9',
            '!','@','#','$','%','&','*','?','+','-','_'
        };

        /// <summary>
        /// Extended character set for strong passwords (letters, digits, safe symbols).
        /// </summary>
        public static readonly char[] StrongPasswordChars = {
            // Uppercase
            'A','B','C','D','E','F','G','H','I','J','K','L','M',
            'N','O','P','Q','R','S','T','U','V','W','X','Y','Z',

            // Lowercase
            'a','b','c','d','e','f','g','h','i','j','k','l','m',
            'n','o','p','q','r','s','t','u','v','w','x','y','z',

            // Digits
            '0','1','2','3','4','5','6','7','8','9',

            // Symbols (safe, no whitespace or quotes)
            '!','@','#','$','%','&','*','?','+','-','_','=',':'
        };

        /// <summary>
        /// Generates a random password of the specified length using the given
        /// allowed character set.  
        /// Characters are selected by repeatedly generating random Unicode values
        /// and filtering them through the allowed set.
        /// </summary>
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

        /// <summary>
        /// Generates a random password using a predefined password level.
        /// </summary>
        public static string Rand(this string a, RandPasswordLevel level, Endian endian) {
            FixedArray<char> allowed;

            if ( level == RandPasswordLevel.Simple)
                allowed = new FixedArray<char>(RandUtils.PasswordChars);
            else
                allowed = new FixedArray<char>(RandUtils.StrongPasswordChars);

            return RandPassword((int)level, allowed, endian);
        }

        /// <summary>
        /// Returns a random extended ASCII character (0–255).
        /// </summary>
        public static char RandAExtended(this char a, Endian endian) {
            return RandChar((char)0, (char)255, endian);
        }
        /// <summary>
        /// Returns a random printable ASCII character (0x20–0x7F).
        /// </summary>
        public static char RandAscii(this char a, Endian endian) {
            return RandChar('\x20', '\x7F', endian);
        }
        /// <summary>
        /// Returns a random character in the inclusive range [min, max].
        /// </summary>
        public static char RandChar(char min, char max, Endian endian) {
            char _c = (char)RandShort((short)min, (short)max, endian);

            return _c;
        }
        #endregion
        #region BYTE
        /// <summary>
        /// Returns a random byte in the full byte range.
        /// </summary>
        public static byte Rand(this byte a, Endian endian)
            => RandByte(byte.MinValue, byte.MaxValue, endian);

        /// <summary>
        /// Returns a random byte in the range [min, 255].
        /// </summary>
        public static byte Rand(this byte a, byte min, Endian endian)
            => RandByte(min, byte.MaxValue, endian);

        /// <summary>
        /// Returns a random byte in the inclusive range [min, max].
        /// </summary>
        public static byte Rand(this byte a, byte min, byte max, Endian endian)
            => RandByte(min, max, endian);

        /// <summary>
        /// Returns a random byte in the inclusive range [min, max].
        /// </summary>
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
        /// <summary>
        /// Returns a random <see cref="int"/> in the full range.
        /// </summary>
        public static int Rand(this int a, Endian endian) {
            return RandInt(int.MinValue, int.MaxValue, endian);
        }

        /// <summary>
        /// Returns a random <see cref="int"/> in the range [min, int.MaxValue].
        /// </summary>
        public static int Rand(this int a, int min, Endian endian)
            => RandInt(min, int.MaxValue, endian);

        /// <summary>
        /// Returns a random <see cref="int"/> in the inclusive range [min, max].
        /// </summary>
        public static int Rand(this int a, int min, int max, Endian endian)
            => RandInt(min, max, endian);

        /// <summary>
        /// Returns a random <see cref="uint"/> in the full range.
        /// </summary>
        public static uint Rand(this uint a, Endian endian)
            => RandUInt(uint.MinValue, uint.MaxValue, endian);

        /// <summary>
        /// Returns a random <see cref="uint"/> in the range [min, uint.MaxValue].
        /// </summary>
        public static uint Rand(this uint a, uint min, Endian endian)
            => RandUInt(min, uint.MaxValue, endian);

        /// <summary>
        /// Returns a random <see cref="uint"/> in the inclusive range [min, max].
        /// </summary>
        public static uint Rand(this uint a, uint min, uint max, Endian endian)
            => RandUInt(min, max, endian);

        /// <summary>
        /// Returns a random <see cref="uint"/> in the inclusive range [min, max].
        /// </summary>
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
        /// <summary>
        /// Returns a random <see cref="int"/> in the inclusive range [min, max].
        /// </summary>
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
        /// <summary>
        /// Returns a random <see cref="short"/> in the full range.
        /// </summary>
        public static short Rand(this short a, Endian endian)
            => RandShort(short.MinValue, short.MaxValue, endian);

        /// <summary>
        /// Returns a random <see cref="short"/> in the range [min, short.MaxValue].
        /// </summary>
        public static short Rand(this short a, short min, Endian endian)
            => RandShort(min, short.MaxValue, endian);

        /// <summary>
        /// Returns a random <see cref="short"/> in the inclusive range [min, max].
        /// </summary>
        public static short Rand(this short a, short min, short max, Endian endian)
            => RandShort(min, max, endian);

        /// <summary>
        /// Returns a random <see cref="ushort"/> in the full range.
        /// </summary>
        public static ushort Rand(this ushort a, Endian endian)
            => RandUShort(ushort.MinValue, ushort.MaxValue, endian);

        /// <summary>
        /// Returns a random <see cref="ushort"/> in the range [min, ushort.MaxValue].
        /// </summary>
        public static ushort Rand(this ushort a, ushort min, Endian endian)
            => RandUShort(min, ushort.MaxValue, endian);

        /// <summary>
        /// Returns a random <see cref="ushort"/> in the inclusive range [min, max].
        /// </summary>
        public static ushort Rand(this ushort a, ushort min, ushort max, Endian endian)
            => RandUShort(min, max, endian);

        /// <summary>
        /// Returns a random <see cref="ushort"/> in the inclusive range [min, max].
        /// </summary>
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
        /// <summary>
        /// Returns a random <see cref="short"/> in the inclusive range [min, max].
        /// </summary>
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
        /// <summary>
        /// Returns a random <see cref="long"/> in the full range.
        /// </summary>
        public static long Rand(this long a, Endian endian)
            => RandLong(long.MinValue, long.MaxValue, endian);

        /// <summary>
        /// Returns a random <see cref="long"/> in the range [min, long.MaxValue].
        /// </summary>
        public static long Rand(this long a, long min, Endian endian)
            => RandLong(min, long.MaxValue, endian);

        /// <summary>
        /// Returns a random <see cref="long"/> in the inclusive range [min, max].
        /// </summary>
        public static long Rand(this long a, long min, long max, Endian endian)
            => RandLong(min, max, endian);

        /// <summary>
        /// Returns a random <see cref="ulong"/> in the full range.
        /// </summary>
        public static ulong Rand(this ulong a, Endian endian)
            => RandULong(ulong.MinValue, ulong.MaxValue, endian);

        /// <summary>
        /// Returns a random <see cref="ulong"/> in the range [min, ulong.MaxValue].
        /// </summary>
        public static ulong Rand(this ulong a, ulong min, Endian endian = Endian.LittleEndian )
            => RandULong(min, ulong.MaxValue, endian);

        /// <summary>
        /// Returns a random <see cref="ulong"/> in the inclusive range [min, max].
        /// </summary>
        public static ulong Rand(this ulong a, ulong min, ulong max, Endian endian = Endian.LittleEndian)
            => RandULong(min, max, endian);

        /// <summary>
        /// Returns a random <see cref="ulong"/> in the inclusive range [min, max].
        /// Uses two 32‑bit random blocks to construct a 64‑bit value.
        /// </summary>
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
        /// <summary>
        /// Returns a random <see cref="long"/> in the inclusive range [min, max].
        /// Uses two 32‑bit random blocks to construct a 64‑bit value.
        /// </summary>
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
        /// <summary>
        /// Returns an array of random bytes of the specified size.
        /// </summary>
        public static byte[] GetArray(int size) {
            byte[] buffer = new byte[size];
            r.NextBytes(buffer);
            return buffer;
        }
        /// <summary>
        /// Returns an array of random bytes in the inclusive range [min, max].
        /// </summary>
        public static byte[] GetArray(int size, byte min, byte max) {
            if ( min > max )
                throw new ArgumentException("min must be <= max");

            byte[] buffer = new byte[size];

            for ( int i = 0; i < size; i++ ) {
#if !TEST
                buffer[i] = (byte)r.Next(min, (uint)max + 1);
#else
                buffer[i] = (byte)r.Next(min, max + 1);
#endif
            }

            return buffer;
        }


    }
}
