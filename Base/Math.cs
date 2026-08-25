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
using SystemEx.Numeric;

namespace SystemEx {
	/// \addtogroup SystemEx
	/// @{

	/// <summary>
	/// Provides mathematical helper functions that are not included in
	/// <see cref="System.Math"/>.  
	/// Contains a deterministic implementation of the C/C++ <c>fmod</c>
	/// operation for both <see cref="double"/> and <see cref="float"/>.
	/// </summary>
	public static class Math {
        /// <summary>
        /// Computes the floating‑point remainder of <paramref name="a"/> divided by
        /// <paramref name="b"/> using truncation toward zero, matching the behavior
        /// of C/C++ <c>fmod</c>.  
        /// Equivalent to: <c>a - trunc(a / b) * b</c>.
        /// </summary>
        public static double DMod ( double a, double b ) {
            return a - System.Math.Truncate(a / b) * b;
        }

        /// <summary>
        /// Computes the floating‑point remainder of <paramref name="a"/> divided by
        /// <paramref name="b"/> using truncation toward zero, matching the behavior
        /// of C/C++ <c>fmod</c>.  
        /// Equivalent to: <c>a - trunc(a / b) * b</c>.
        /// </summary>
        public static float FMod ( float a, float b ) {
            return a - (float)System.Math.Truncate(a / b) * b;
        }

        
        /// <summary>
        /// Wraps a value into the interval [min, max) using modular arithmetic.
        /// Useful for cyclic ranges such as angles or periodic values.
        /// </summary>
        public static float Wrap ( float x, float min, float max ) {
            return min + FMod(x - min, max - min);
        }
        /// <summary>
        /// Wraps a value into the interval [min, max) using modular arithmetic.
        /// Useful for cyclic ranges such as angles or periodic values.
        /// </summary>
        public static double Wrap ( double x, double min, double max ) {
            return min + DMod(x - min, max - min);
        }
        /// <summary>
        /// Performs a linear interpolation between <paramref name="a"/> and <paramref name="b"/>
        /// using the interpolation factor <paramref name="t"/>.
        /// </summary>
        public static float Lerp ( float a, float b, float t ) => a + (b - a) * t;

        /// <summary>
        /// Performs a linear interpolation between <paramref name="a"/> and <paramref name="b"/>
        /// using the interpolation factor <paramref name="t"/>.
        /// </summary>
        public static double Lerp ( double a, double b, double t ) => a + (b - a) * t;

        /// <summary>
        /// Performs a linear interpolation between <paramref name="a"/> and <paramref name="b"/>
        /// using the interpolation factor <paramref name="t"/>.
        /// </summary>
        public static int Lerp ( int a, int b, int t ) => a + (b - a) * t;

        /// <summary>
        /// Performs a linear interpolation between <paramref name="a"/> and <paramref name="b"/>
        /// using the interpolation factor <paramref name="t"/>.
        /// </summary>
        public static short Lerp ( short a, short b, short t ) => (short)(a + (b - a) * t);


        /// <summary>
        /// Performs a linear interpolation between <paramref name="a"/> and <paramref name="b"/>
        /// using the interpolation factor <paramref name="t"/>.
        /// </summary>
        public static long Lerp ( long a, long b, long t ) => a + (b - a) * t;

        /// <summary>
        /// Performs a linear interpolation between <paramref name="a"/> and <paramref name="b"/>
        /// using the interpolation factor <paramref name="t"/>.
        /// </summary>
        public static uint Lerp ( uint a, uint b, uint t ) => a + (b - a) * t;

        /// <summary>
        /// Performs a linear interpolation between <paramref name="a"/> and <paramref name="b"/>
        /// using the interpolation factor <paramref name="t"/>.
        /// </summary>
        public static ushort Lerp ( ushort a, ushort b, ushort t ) => (ushort)(a + (b - a) * t);


        /// <summary>
        /// Performs a linear interpolation between <paramref name="a"/> and <paramref name="b"/>
        /// using the interpolation factor <paramref name="t"/>.
        /// </summary>
        public static ulong Lerp ( ulong a, ulong b, ulong t ) => a + (b - a) * t;

        /// <summary>
        /// Computes the inverse linear interpolation. Returns the normalized position of
        /// <paramref name="x"/> within the interval [a, b].
        /// </summary>
        public static float Unlerp ( float a, float b, float x ) => (x - a) / (b - a);
        /// <summary>
        /// Computes the inverse linear interpolation. Returns the normalized position of
        /// <paramref name="x"/> within the interval [a, b].
        /// </summary>
        public static double Unlerp ( double a, double b, double x ) => (x - a) / (b - a);
        /// <summary>
        /// Computes the inverse linear interpolation. Returns the normalized position of
        /// <paramref name="x"/> within the interval [a, b].
        /// </summary>
        public static short Unlerp ( short a, short b, short x ) => (short)((x - a) / (b - a));
        /// <summary>
        /// Computes the inverse linear interpolation. Returns the normalized position of
        /// <paramref name="x"/> within the interval [a, b].
        /// </summary>
        public static int Unlerp ( int a, int b, int x ) => (int)((x - a) / (b - a));
        /// <summary>
        /// Computes the inverse linear interpolation. Returns the normalized position of
        /// <paramref name="x"/> within the interval [a, b].
        /// </summary>
        public static long Unlerp ( long a, long b, long x ) => (long)((x - a) / (b - a));
        /// <summary>
        /// Computes the inverse linear interpolation. Returns the normalized position of
        /// <paramref name="x"/> within the interval [a, b].
        /// </summary>
        public static ushort Unlerp ( ushort a, ushort b, ushort x ) => (ushort)((x - a) / (b - a));
        /// <summary>
        /// Computes the inverse linear interpolation. Returns the normalized position of
        /// <paramref name="x"/> within the interval [a, b].
        /// </summary>
        public static uint Unlerp ( uint a, uint b, uint x ) => (uint)((x - a) / (b - a));
        /// <summary>
        /// Computes the inverse linear interpolation. Returns the normalized position of
        /// <paramref name="x"/> within the interval [a, b].
        /// </summary>
        public static ulong Unlerp ( ulong a, ulong b, ulong x ) => (ulong)((x - a) / (b - a));
        /// <summary>
        /// Clamps a value to the range [0, 1].
        /// </summary>
        public static float Clamp ( float x ) {
            if ( x < 0f ) return 0f;
            if ( x > 1f ) return 1f;
            return x;
        }

        /// <summary>
        /// Clamps a value to the range [0, 1].
        /// </summary>
        public static double Clamp ( double x ) {
            if ( x < 0f ) return 0;
            if ( x > 1f ) return 1;
            return x;
        }

        /// <summary>
        /// Clamps a value to the range [0, 1]. Alias for <see cref="Clamp(float)"/>.
        /// </summary>
        public static float Saturate ( float x ) => Clamp(x);

        /// <summary>
        /// Clamps a value to the range [0, 1]. Alias for <see cref="Clamp(double)"/>.
        /// </summary>
        public static double Saturate ( double x ) => Clamp(x);

        /// <summary>
        /// Determines whether the given integer is a power of two.
        /// </summary>
        public static bool IsPowerOfTwo ( int x ) => (x & (x - 1)) == 0;
        /// <summary>
        /// Computes the next power of two greater than or equal to the given integer.
        /// </summary>
        public static int NextPowerOfTwo ( int x ) {
            x--;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            return x + 1;
        }
        /// <summary>
        /// Applies a smooth Hermite interpolation curve to the input value.
        /// Produces a smooth transition between 0 and 1.
        /// </summary>
        public static float SmoothStep ( float x ) {
            return x * x * (3f - 2f * x);
        }

        /// <summary>
        /// Applies a smooth Hermite interpolation curve to the input value.
        /// Produces a smooth transition between 0 and 1.
        /// </summary>
        public static double SmoothStep ( double x ) {
            return x * x * (3.0 - 2.0 * x);
        }

        /// <summary>
        /// Normalizes an angle in radians to the interval [-π, π].
        /// </summary>
        public static float NormalizeAngle ( float rad ) {
            return Wrap(rad, -MathF.PI, MathF.PI);
        }

        /// <summary>
        /// Normalizes an angle in radians to the interval [-π, π].
        /// </summary>
        public static double NormalizeAngle ( double rad ) {
            return Wrap(rad, -System.Math.PI, System.Math.PI);
        }
        /// <summary>
        /// Determines whether two floating‑point values are nearly equal within
        /// a specified epsilon tolerance.
        /// </summary>
        public static bool NearlyEqual ( float a, float b, float eps = 1e-6f ) {
            return System.Math.Abs(a - b) <= eps;
        }
        /// <summary>
        /// Determines whether two floating‑point values are nearly equal within
        /// a specified epsilon tolerance.
        /// </summary>
        public static bool NearlyEqual ( double a, double b, double eps = 1e-6f ) {
            return System.Math.Abs(a - b) <= eps;
        }
     
       
    }
	/// @}
}
