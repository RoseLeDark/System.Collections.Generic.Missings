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
using SystemEx.Rand;
using SystemEx.Utils;

namespace SystemEx {

	/// <summary>
	/// Provides a high‑level wrapper around the Base Class Library (BCL)
	/// <see cref="System.Random"/> type. This implementation redirects all
	/// random‑number generation to the SystemEx random subsystem, allowing
	/// deterministic, engine‑agnostic, endian‑aware, and extended numeric
	/// random operations.
	/// </summary>
	/// <remarks>
	/// This class preserves the public API surface of <see cref="System.Random"/>
	/// while internally delegating all operations to <see cref="RandUtils"/>.
	/// The underlying engine is selected by <see cref="Randx"/>, which may use
	/// either <see cref="Isaac32Engine"/> (default) or <see cref="MTwisterEngine"/>
	/// depending on compilation flags or other.
	/// </remarks>	
	public class Random : System.Random {
		/// <summary>
		/// Initializes a new instance of the <see cref="Random"/> class using a
		/// time‑based seed. The seed is forwarded to <see cref="RandUtils"/> to
		/// initialize the internal pseudo‑random engine.
		/// </summary>
		public Random () {
			RandUtils.Setup( (uint)DateTime.Now.ToBinary());
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Random"/> class using the
		/// specified 32‑bit seed value.
		/// </summary>
		/// <param name="seed">The primary seed value.</param>
		public Random ( uint seed ) {
			RandUtils.Setup(seed);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Random"/> class using a
		/// custom seed container implementing <see cref="ISeed"/>.
		/// </summary>
		/// <param name="seed">The seed container used to initialize the engine.</param>
		public Random ( ISeed seed ) {
			RandUtils.SetupWithSeed(seed);
		}


		/// <summary>
		/// Returns a random 32‑bit integer within the full <see cref="int"/> range.
		/// </summary>
		public override int Next ()
			=> RandUtils.RandInt(int.MinValue, int.MaxValue, Endian.System);

		/// <summary>
		/// Returns a random 32‑bit integer within the specified range.
		/// </summary>
		public override int Next ( int minValue, int maxValue )
			=> RandUtils.RandInt(minValue, maxValue, Endian.System);

		/// <summary>
		/// Returns a random 32‑bit integer less than the specified maximum.
		/// </summary>
		public override int Next ( int maxValue )
			=> RandUtils.RandInt(int.MinValue, maxValue, Endian.System);

		/// <summary>
		/// Returns a random 16‑bit signed integer within the full <see cref="short"/> range.
		/// </summary>
		public virtual short NextInt16 ()
			=> RandUtils.RandShort(short.MinValue, short.MaxValue, Endian.System);

		/// <summary>
		/// Returns a random 16‑bit signed integer within the specified range.
		/// </summary>
		public virtual short NextInt16 ( short minValue, short maxValue )
			=> RandUtils.RandShort(minValue, maxValue, Endian.System);

		/// <summary>
		/// Returns a random 16‑bit signed integer less than the specified maximum.
		/// </summary>
		public virtual short NextInt16 ( short maxValue )
			=> RandUtils.RandShort(short.MinValue, maxValue, Endian.System);

		/// <summary>
		/// Returns a random 32‑bit signed integer within the full <see cref="int"/> range.
		/// </summary>
		public virtual int NextInt32 ()
			=> RandUtils.RandInt(int.MinValue, int.MaxValue, Endian.System);

		/// <summary>
		/// Returns a random 32‑bit signed integer within the specified range.
		/// </summary>
		public virtual int NextInt32 ( int minValue, int maxValue )
			=> RandUtils.RandInt(minValue, maxValue, Endian.System);

		/// <summary>
		/// Returns a random 32‑bit signed integer less than the specified maximum.
		/// </summary>
		public virtual int NextInt32 ( int maxValue )
			=> RandUtils.RandInt(int.MinValue, maxValue, Endian.System);

		/// <summary>
		/// Returns a random 64‑bit signed integer within the full <see cref="long"/> range.
		/// </summary>
		public override long NextInt64 ()
			=> RandUtils.RandLong(long.MinValue, long.MaxValue, Endian.System);

		/// <summary>
		/// Returns a random 64‑bit signed integer within the specified range.
		/// </summary>
		public override long NextInt64 ( long minValue, long maxValue )
			=> RandUtils.RandLong(minValue, maxValue, Endian.System);

		/// <summary>
		/// Returns a random 64‑bit signed integer less than the specified maximum.
		/// </summary>
		public override long NextInt64 ( long maxValue )
			=> RandUtils.RandLong(long.MinValue, maxValue, Endian.System);



		/// <summary>
		/// Returns a random 16‑bit unsigned integer within the full <see cref="ushort"/> range.
		/// </summary>
		public virtual ushort NextUInt16 ()
			=> RandUtils.RandUShort(ushort.MinValue, ushort.MaxValue, Endian.System);

		/// <summary>
		/// Returns a random 16‑bit unsigned integer within the specified range.
		/// </summary>
		public virtual ushort NextUInt16 ( ushort minValue, ushort maxValue )
			=> RandUtils.RandUShort(minValue, maxValue, Endian.System);

		/// <summary>
		/// Returns a random 16‑bit unsigned integer less than the specified maximum.
		/// </summary>
		public virtual ushort NextUInt16 ( ushort maxValue )
			=> RandUtils.RandUShort(ushort.MinValue, maxValue, Endian.System);

		/// <summary>
		/// Returns a random 32‑bit unsigned integer within the full <see cref="uint"/> range.
		/// </summary>
		public virtual uint NextUInt32 ()
			=> RandUtils.RandUInt(uint.MinValue, uint.MaxValue, Endian.System);

		/// <summary>
		/// Returns a random 32‑bit unsigned integer within the specified range.
		/// </summary>
		public virtual uint NextUInt32 ( uint minValue, uint maxValue )
			=> RandUtils.RandUInt(minValue, maxValue, Endian.System);

		/// <summary>
		/// Returns a random 32‑bit unsigned integer less than the specified maximum.
		/// </summary>
		public virtual uint NextUInt32 ( uint maxValue )
			=> RandUtils.RandUInt(uint.MinValue, maxValue, Endian.System);

		/// <summary>
		/// Returns a random 64‑bit unsigned integer within the full <see cref="ulong"/> range.
		/// </summary>
		public virtual ulong NextUInt64 ()
			=> RandUtils.RandULong(ulong.MinValue, ulong.MaxValue, Endian.System);

		/// <summary>
		/// Returns a random 64‑bit unsigned integer within the specified range.
		/// </summary>
		public virtual ulong NextUInt64 ( ulong minValue, ulong maxValue )
			=> RandUtils.RandULong(minValue, maxValue, Endian.System);

		/// <summary>
		/// Returns a random 64‑bit unsigned integer less than the specified maximum.
		/// </summary>
		public virtual ulong NextUInt64 ( ulong maxValue )
			=> RandUtils.RandULong(ulong.MinValue, maxValue, Endian.System);

		/// <summary>
		/// Returns a random double‑precision floating‑point value. The result is
		/// generated from eight random bytes and validated to be a normal IEEE‑754
		/// value.
		/// </summary>
		public override double NextDouble () {
			return NextDouble(0.0, 1.0);
		}
		/// <summary>
		/// Returns a random double‑precision floating‑point value. The result is
		/// generated from eight random bytes and validated to be a normal IEEE‑754
		/// value.
		/// </summary>
		public virtual double NextDouble ( double maxValue ) {
			return NextDouble(0.0, maxValue);
		}

		/// <summary>
		/// Returns a random double‑precision floating‑point value. The result is
		/// generated from eight random bytes and validated to be a normal IEEE‑754
		/// value.
		/// </summary>
		public virtual double NextDouble ( double minValue, double maxValue ) {
			if ( minValue > maxValue )
				throw new ArgumentException("minValue must be <= maxValue");

			double value;

			// Generate a normal IEEE‑754 double using your byte‑based system
			do {
				var tmp = RandUtils.GetArray(8);
				value = tmp.ToDouble();
			}
			while ( !double.IsNormal(value) );


			// Scale to [minValue, maxValue]
			return minValue + value * (maxValue - minValue);
		}


		/// <summary>
		/// Returns a random single‑precision floating‑point value. The result is
		/// generated from four random bytes and validated to be a normal IEEE‑754
		/// value.
		/// </summary>
		public override float NextSingle () {
			return NextSingle(0.0f, 1.0f);
		}

		public virtual float NextSingle ( float maxValue ) {
			return NextSingle(float.MinValue, maxValue);
		}

		/// <summary>
		/// Returns a random single‑precision floating‑point value. The result is
		/// generated from four random bytes and validated to be a normal IEEE‑754
		/// value.
		/// </summary>
		public virtual float NextSingle ( float minValue, float maxValue ) {
			if ( minValue > maxValue )
				throw new ArgumentException("minValue must be <= maxValue");

			float value;

			// Generate a normal IEEE‑754 float using your byte‑based system
			do {
				var tmp = RandUtils.GetArray(4);
				value = tmp.ToFloat();
			}
			while ( !float.IsNormal(value) );

			// Scale to [minValue, maxValue]
			return minValue + value * (maxValue - minValue);
		}
		/// <summary>
		/// Returns a random <see cref="Half16"/> value generated from four random bytes.
		/// </summary>
		public virtual Half16 NextHalf16 () {
			return NextHalf16(Half16.Zero, Half16.One);
		}

		/// <summary>
		/// Returns a random <see cref="Half16"/> value generated from four random bytes.
		/// </summary>
		public virtual Half16 NextHalf16 (Half16 max) {
			return NextHalf16(Half16.MinValue, max);
		}

		/// <summary>
		/// Returns a random <see cref="Half16"/> value generated from four random bytes.
		/// </summary>
		public virtual Half16 NextHalf16 ( Half16 minValue, Half16 maxValue ) {
			if ( minValue > maxValue )
				throw new ArgumentException("minValue must be <= maxValue");

			Half16 value;

			do {
				var tmp = RandUtils.GetArray(2);
				value = tmp.ToHalf16();
			}
			while ( !Half16.IsNormal(value) );

			// Scale to [minValue, maxValue]
			return minValue + value * (maxValue - minValue);
		}

		/// <summary>
		/// Returns a random <see cref="Half16b"/> value generated from four random bytes.
		/// </summary>
		public virtual Half16b NextHalf16b () {
			return NextHalf16b(Half16b.Zero, Half16b.One);
		}

		/// <summary>
		/// Returns a random <see cref="Half16"/> value generated from four random bytes.
		/// </summary>
		public virtual Half16b NextHalf16b ( Half16b max ) {
			return NextHalf16b(Half16b.MinValue, max);
		}

		/// <summary>
		/// Returns a random <see cref="Half16"/> value generated from four random bytes.
		/// </summary>
		public virtual Half16b NextHalf16b ( Half16b minValue, Half16b maxValue ) {
			if ( minValue > maxValue )
				throw new ArgumentException("minValue must be <= maxValue");

			Half16b value;

			do {
				var tmp = RandUtils.GetArray(2);
				value = tmp.ToHalf16b();
			}
			while ( !Half16b.IsNormal(value) );

			// Scale to [minValue, maxValue]
			return minValue + value * (maxValue - minValue);
		}

		/// <summary>
		/// Returns a random <see cref="FloatE4M3"/> value generated from a single byte.
		/// </summary>
		public virtual FloatE4M3 NextFloatE4M3 () {
			FloatE4M3 _ret;

			do {
				var _tmp = RandUtils.RandByte(0, 255, Endian.System);
				_ret = new FloatE4M3(_tmp);
			} while ( !FloatE4M3.IsNormal(_ret) );

			return _ret;
		}

		/// <summary>
		/// Returns a random <see cref="FloatE5M2"/> value generated from a single byte.
		/// </summary>
		public virtual FloatE5M2 NextFloatE5M2 () {
			FloatE5M2 _ret;

			do {
				var _tmp = RandUtils.RandByte(0, 255, Endian.System);
				_ret = new FloatE5M2(_tmp);
			} while ( !FloatE5M2.IsNormal(_ret) );

			return _ret;
		}

		/// <summary>
		/// Fills the specified buffer with random bytes.
		/// </summary>
		public override void NextBytes ( byte[] buffer ) {
			for ( int i = 0 ; i < buffer.Length ; i++ ) {
				buffer[i] = RandUtils.RandByte(0, 255, Endian.System);
			}
		}

		/// <summary>
		/// Fills the specified span with random bytes.
		/// </summary>
		public override void NextBytes ( Span<byte> buffer ) {
			for ( int i = 0 ; i < buffer.Length ; i++ ) {
				buffer[i] = RandUtils.RandByte(0, 255, Endian.System);
			}
		}

		/// <summary>
		/// Fills the specified <see cref="FlexSpan{T}"/> with random bytes.
		/// </summary>
		/// <exception cref="Exception">
		/// Thrown when the buffer is in <see cref="FlexSpanMode.Ring"/> mode.
		/// </exception>
		public virtual void NextBytes ( FlexSpan<byte> buffer ) {
			if ( buffer.Mode == FlexSpanMode.Ring )
				throw new Exception("buffer is in Ring Mode please use own impl with escape.");

			for ( int i = 0 ; i < buffer.Length ; i++ ) {
				buffer[i] = RandUtils.RandByte(0, 255, Endian.System);
			}
		}

		public virtual void NextBytes ( FlexSpan<byte> buffer, long start, long end ) {
			for ( long i = start ; i < end ; i++ ) {
				buffer[i] = RandUtils.RandByte(0, 255, Endian.System);
			}
		}
		/// <summary>
		/// Generates a random 2‑component vector of <see cref="float"/> values.
		/// Each component is independently sampled within the specified range.
		/// </summary>
		/// <param name="minX">Minimum value for the X component.</param>
		/// <param name="maxX">Maximum value for the X component.</param>
		/// <param name="minY">Minimum value for the Y component.</param>
		/// <param name="maxY">Maximum value for the Y component.</param>
		/// <returns>A <see cref="Vec2f"/> containing random values in the given ranges.</returns>
		public virtual Vec2f NextVec2f( float minX = float.MinValue, float maxX = float.MaxValue,
			float minY = float.MinValue, float maxY = float.MaxValue) {
			var _x = NextSingle(minX, maxX);
			var _y = NextSingle(minY, maxY);

			return new Vec2f(_x, _y);
		}

		/// <summary>
		/// Generates a random 2‑component vector of <see cref="double"/> values.
		/// Each component is independently sampled within the specified range.
		/// </summary>
		/// <param name="minX">Minimum value for the X component.</param>
		/// <param name="maxX">Maximum value for the X component.</param>
		/// <param name="minY">Minimum value for the Y component.</param>
		/// <param name="maxY">Maximum value for the Y component.</param>
		/// <returns>A <see cref="Vec2d"/> containing random values in the given ranges.</returns>
		public virtual Vec2d NextVec2d ( double minX = double.MinValue, double maxX = double.MaxValue,
			double minY = double.MinValue, double maxY = double.MaxValue ) {
			var _x = NextDouble(minX, maxX);
			var _y = NextDouble(minY, maxY);

			return new Vec2d(_x, _y);
		}
		/// <summary>
		/// Generates a random 2‑component vector of <see cref="Half16 "/> values.
		/// Each component is independently sampled within the specified range.
		/// </summary>
		/// <param name="minX">Minimum value for the X component.</param>
		/// <param name="maxX">Maximum value for the X component.</param>
		/// <param name="minY">Minimum value for the Y component.</param>
		/// <param name="maxY">Maximum value for the Y component.</param>
		/// <returns>A <see cref="Vec2h"/> containing random values in the given ranges.</returns>
		public virtual Vec2h NextVec2h ( Half16 minX, Half16 maxX, Half16 minY, Half16 maxY ) {
			var _x = NextHalf16(minX, maxX);
			var _y = NextHalf16(minY, maxY);

			return new Vec2h(_x, _y);
		}
		/// <summary>
		/// Generates a random 2‑component vector of <see cref="Half16b"/> values.
		/// Each component is independently sampled within the specified range.
		/// </summary>
		/// <param name="minX">Minimum value for the X component.</param>
		/// <param name="maxX">Maximum value for the X component.</param>
		/// <param name="minY">Minimum value for the Y component.</param>
		/// <param name="maxY">Maximum value for the Y component.</param>
		/// <returns>A <see cref="Vec2hb"/> containing random values in the given ranges.</returns>
		public virtual Vec2hb NextVec2hb ( Half16b minX, Half16b maxX, Half16b minY, Half16b maxY ) {
			var _x = NextHalf16b(minX, maxX);
			var _y = NextHalf16b(minY, maxY);

			return new Vec2hb(_x, _y);
		}

		/// <summary>
		/// Generates a random 3‑component vector of <see cref="float"/> values.
		/// Each component is independently sampled within the specified range.
		/// </summary>
		/// <param name="minX">Minimum value for the X component.</param>
		/// <param name="maxX">Maximum value for the X component.</param>
		/// <param name="minY">Minimum value for the Y component.</param>
		/// <param name="maxY">Maximum value for the Y component.</param>
		/// <param name="minZ">Minimum value for the Z component.</param>
		/// <param name="maxZ">Maximum value for the Z component.</param>
		/// <returns>A <see cref="Vec3f"/> containing random values in the given ranges.</returns>
		public virtual Vec3f NextVec3f (
			float minX = float.MinValue, float maxX = float.MaxValue,
			float minY = float.MinValue, float maxY = float.MaxValue,
			float minZ = float.MinValue, float maxZ = float.MaxValue ) {
			var _x = NextSingle(minX, maxX);
			var _y = NextSingle(minY, maxY);
			var _z = NextSingle(minZ, maxZ);

			return new Vec3f(_x, _y, _z);
		}
		/// <summary>
		/// Generates a random 3‑component vector of <see cref="double"/> values.
		/// Each component is independently sampled within the specified range.
		/// </summary>
		/// <param name="minX">Minimum value for the X component.</param>
		/// <param name="maxX">Maximum value for the X component.</param>
		/// <param name="minY">Minimum value for the Y component.</param>
		/// <param name="maxY">Maximum value for the Y component.</param>
		/// <param name="minZ">Minimum value for the Z component.</param>
		/// <param name="maxZ">Maximum value for the Z component.</param>
		/// <returns>A <see cref="Vec3d"/> containing random values in the given ranges.</returns>
		public virtual Vec3d NextVec3d (
			double minX = double.MinValue, double maxX = double.MaxValue,
			double minY = double.MinValue, double maxY = double.MaxValue,
			double minZ = double.MinValue, double maxZ = double.MaxValue) {
			var _x = NextDouble(minX, maxX);
			var _y = NextDouble(minY, maxY);
			var _z = NextDouble(minZ, maxZ);

			return new Vec3d(_x, _y, _z);
		}
		/// <summary>
		/// Generates a random 3‑component vector of <see cref="Half16"/> values.
		/// Each component is independently sampled within the specified range.
		/// </summary>
		/// <param name="minX">Minimum value for the X component.</param>
		/// <param name="maxX">Maximum value for the X component.</param>
		/// <param name="minY">Minimum value for the Y component.</param>
		/// <param name="maxY">Maximum value for the Y component.</param>
		/// <param name="minZ">Minimum value for the Z component.</param>
		/// <param name="maxZ">Maximum value for the Z component.</param>
		/// <returns>A <see cref="Vec3h"/> containing random values in the given ranges.</returns>
		public virtual Vec3h NextVec3h (
			Half16 minX, Half16 maxX,
			Half16 minY, Half16 maxY,
			Half16 minZ, Half16 maxZ ) {
			var _x = NextHalf16(minX, maxX);
			var _y = NextHalf16(minY, maxY);
			var _z = NextHalf16(minZ, maxZ);

			return new Vec3h(_x, _y, _z);
		}
		/// <summary>
		/// Generates a random 3‑component vector of <see cref="Half16b"/> values.
		/// Each component is independently sampled within the specified range.
		/// </summary>
		/// <param name="minX">Minimum value for the X component.</param>
		/// <param name="maxX">Maximum value for the X component.</param>
		/// <param name="minY">Minimum value for the Y component.</param>
		/// <param name="maxY">Maximum value for the Y component.</param>
		/// <param name="minZ">Minimum value for the Z component.</param>
		/// <param name="maxZ">Maximum value for the Z component.</param>
		/// <returns>A <see cref="Vec3hb"/> containing random values in the given ranges.</returns>
		public virtual Vec3hb NextVec3hb (
			Half16b minX, Half16b maxX,
			Half16b minY, Half16b maxY,
			Half16b minZ, Half16b maxZ ) {
			var _x = NextHalf16b(minX, maxX);
			var _y = NextHalf16b(minY, maxY);
			var _z = NextHalf16b(minZ, maxZ);

			return new Vec3hb(_x, _y, _z);
		}

		/// <summary>
		/// Generates a random 4‑component vector of <see cref="float"/> values.
		/// Each component is independently sampled within the specified range.
		/// </summary>
		/// <param name="minX">Minimum value for the X component.</param>
		/// <param name="maxX">Maximum value for the X component.</param>
		/// <param name="minY">Minimum value for the Y component.</param>
		/// <param name="maxY">Maximum value for the Y component.</param>
		/// <param name="minZ">Minimum value for the Z component.</param>
		/// <param name="maxZ">Maximum value for the Z component.</param>
		/// <param name="minW">Minimum value for the Z component.</param>
		/// <param name="maxW">Maximum value for the Z component.</param>
		/// <returns>A <see cref="Vec4f"/> containing random values in the given ranges.</returns>
		public virtual Vec4f NextVec4f (
			float minX = float.MinValue, float maxX = float.MaxValue,
			float minY = float.MinValue, float maxY = float.MaxValue,
			float minZ = float.MinValue, float maxZ = float.MaxValue,
			float minW = float.MinValue, float maxW = float.MaxValue ) {
			var _x = NextSingle(minX, maxX);
			var _y = NextSingle(minY, maxY);
			var _z = NextSingle(minZ, maxZ);
			var _w = NextSingle(minW, maxW);

			return new Vec4f(_x, _y, _z, _w);
		}
		/// <summary>
		/// Generates a random 4‑component vector of <see cref="double"/> values.
		/// Each component is independently sampled within the specified range.
		/// </summary>
		/// <param name="minX">Minimum value for the X component.</param>
		/// <param name="maxX">Maximum value for the X component.</param>
		/// <param name="minY">Minimum value for the Y component.</param>
		/// <param name="maxY">Maximum value for the Y component.</param>
		/// <param name="minZ">Minimum value for the Z component.</param>
		/// <param name="maxZ">Maximum value for the Z component.</param>
		/// <param name="minW">Minimum value for the Z component.</param>
		/// <param name="maxW">Maximum value for the Z component.</param>
		/// <returns>A <see cref="Vec4d"/> containing random values in the given ranges.</returns>
		public virtual Vec4d NextVec4d (
			double minX = double.MinValue, double maxX = double.MaxValue,
			double minY = double.MinValue, double maxY = double.MaxValue,
			double minZ = double.MinValue, double maxZ = double.MaxValue,
			double minW = double.MinValue, double maxW = double.MaxValue ) {
			var _x = NextDouble(minX, maxX);
			var _y = NextDouble(minY, maxY);
			var _z = NextDouble(minZ, maxZ);
			var _w = NextDouble(minW, maxW);

			return new Vec4d(_x, _y, _z, _w);
		}
		/// <summary>
		/// Generates a random 4‑component vector of <see cref=" Half16"/> values.
		/// Each component is independently sampled within the specified range.
		/// </summary>
		/// <param name="minX">Minimum value for the X component.</param>
		/// <param name="maxX">Maximum value for the X component.</param>
		/// <param name="minY">Minimum value for the Y component.</param>
		/// <param name="maxY">Maximum value for the Y component.</param>
		/// <param name="minZ">Minimum value for the Z component.</param>
		/// <param name="maxZ">Maximum value for the Z component.</param>
		/// <param name="minW">Minimum value for the Z component.</param>
		/// <param name="maxW">Maximum value for the Z component.</param>
		/// <returns>A <see cref="Vec4h"/> containing random values in the given ranges.</returns>
		public virtual Vec4h NextVec4h (
			Half16 minX, Half16 maxX,
			Half16 minY, Half16 maxY,
			Half16 minZ, Half16 maxZ,
			Half16 minW, Half16 maxW ) {
			var _x = NextHalf16(minX, maxX);
			var _y = NextHalf16(minY, maxY);
			var _z = NextHalf16(minZ, maxZ);
			var _w = NextHalf16(minW, maxW);

			return new Vec4h(_x, _y, _z, _w);
		}
		/// <summary>
		/// Generates a random 4‑component vector of <see cref=" Half16b"/> values.
		/// Each component is independently sampled within the specified range.
		/// </summary>
		/// <param name="minX">Minimum value for the X component.</param>
		/// <param name="maxX">Maximum value for the X component.</param>
		/// <param name="minY">Minimum value for the Y component.</param>
		/// <param name="maxY">Maximum value for the Y component.</param>
		/// <param name="minZ">Minimum value for the Z component.</param>
		/// <param name="maxZ">Maximum value for the Z component.</param>
		/// <param name="minW">Minimum value for the Z component.</param>
		/// <param name="maxW">Maximum value for the Z component.</param>
		/// <returns>A <see cref="Vec4hb"/> containing random values in the given ranges.</returns>
		public virtual Vec4hb NextVec4hb (
			Half16b minX, Half16b maxX,
			Half16b minY, Half16b maxY,
			Half16b minZ, Half16b maxZ,
			Half16b minW, Half16b maxW ) {
			var _x = NextHalf16b(minX, maxX);
			var _y = NextHalf16b(minY, maxY);
			var _z = NextHalf16b(minZ, maxZ);
			var _w = NextHalf16b(minW, maxW);

			return new Vec4hb(_x, _y, _z, _w);
		}

	}
}
