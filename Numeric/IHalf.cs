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

namespace SystemEx.Numeric {
	/// \addtogroup SystemEx.Numeric
	/// @{
	/// <summary>
	/// Represents a 256‑bit unsigned integer composed of two 128‑bit halves.
	/// 
	/// <para>
	/// <see cref="Uint256"/> is a structural container used by high‑precision
	/// floating‑point formats and extended numeric systems. It stores the lower
	/// and upper 128‑bit segments explicitly, enabling deterministic bit‑level
	/// operations, custom floating‑point encodings, and arbitrary‑precision
	/// arithmetic.
	/// </para>
	/// 
	/// <para>
	/// The type does not implement arithmetic itself; it is intended as a raw
	/// storage primitive for <see cref="IBigFloat{TSelf}"/> and other extended
	/// numeric abstractions.
	/// </para>
	/// </summary>
	public struct Uint256 {
		/// <summary>
		/// The lower 128‑bit segment of the value.
		/// </summary>
		public Int128 Low;

		/// <summary>
		/// The upper 128‑bit segment of the value.
		/// </summary>
		public Int128 High;
	}


	/// <summary>
	/// Defines a 16‑bit floating‑point format based on <see cref="IFloat{TSelf, ushort}"/>.
	/// 
	/// <para>
	/// Implementations of <see cref="IHalf{TSelf}"/> represent compact floating‑point
	/// numbers such as IEEE‑754 binary16 or custom 16‑bit formats. The exponent,
	/// mantissa, and sign layout is determined by the underlying
	/// <see cref="IFloat{TSelf, ushort}"/> contract.
	/// </para>
	/// 
	/// <para>
	/// Typical use cases include graphics, neural networks, embedded systems,
	/// and memory‑efficient numeric processing.
	/// </para>
	/// </summary>
	public interface IHalf<TSelf> : IFloat<TSelf, ushort>
		where TSelf : struct, IHalf<TSelf> {
	}

	/// <summary>
	/// Defines an 8‑bit floating‑point format based on <see cref="IFloat{TSelf, byte}"/>.
	/// 
	/// <para>
	/// <see cref="IMini{TSelf}"/> is intended for extremely compact numeric
	/// representations such as FP8 variants used in machine learning or embedded
	/// systems. The exact bit layout (sign, exponent, mantissa) is determined by
	/// the <see cref="IFloat{TSelf, byte}"/> implementation.
	/// </para>
	/// </summary>
	public interface IMini<TSelf> : IFloat<TSelf, byte>
		where TSelf : struct, IMini<TSelf> {
	}

	/// <summary>
	/// Defines a 32‑bit floating‑point format based on <see cref="IFloat{TSelf, uint}"/>.
	/// 
	/// <para>
	/// Implementations of <see cref="ICFloat{TSelf}"/> represent custom 32‑bit
	/// floating‑point formats. Unlike <see cref="float"/>, these formats may use
	/// alternative exponent biases, mantissa widths, or encoding rules.
	/// </para>
	/// </summary>
	public interface ICFloat<TSelf> : IFloat<TSelf, uint>
		where TSelf : struct, ICFloat<TSelf> {
	}


	/// <summary>
	/// Defines a 64‑bit floating‑point format based on <see cref="IFloat{TSelf, ulong}"/>.
	/// 
	/// <para>
	/// <see cref="ICDouble{TSelf}"/> is used for custom double‑precision formats
	/// that differ from IEEE‑754 binary64. Implementations may adjust exponent
	/// bias, mantissa width, or special‑value encodings.
	/// </para>
	/// </summary>
	public interface ICDouble<TSelf> : IFloat<TSelf, ulong>
		where TSelf : struct, ICDouble<TSelf> {
	}


	/// <summary>
	/// Defines a 128‑bit floating‑point format based on <see cref="IFloat{TSelf, UInt128}"/>.
	/// 
	/// <para>
	/// Implementations of <see cref="ICQuad{TSelf}"/> represent quad‑precision
	/// floating‑point formats. These formats are suitable for scientific computing,
	/// high‑precision simulation, and extended numeric analysis.
	/// </para>
	/// </summary>
	public interface ICQuad<TSelf> : IFloat<TSelf, UInt128>
		where TSelf : struct, ICQuad<TSelf> {
	}


	/// <summary>
	/// Defines an extended‑precision floating‑point format based on
	/// <see cref="IFloat{TSelf, Uint256}"/>.
	/// 
	/// <para>
	/// <see cref="IBigFloat{TSelf}"/> supports extremely high‑precision floating‑point
	/// representations using a 256‑bit bias and structural layout. These formats
	/// are suitable for arbitrary‑precision math, symbolic computation, and
	/// advanced numeric research.
	/// </para>
	/// </summary>
	public interface IBigFloat<TSelf> : IFloat<TSelf, Uint256>
		where TSelf : struct, IBigFloat<TSelf> {
	}


	//@}
}
