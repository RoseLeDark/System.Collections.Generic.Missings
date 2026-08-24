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
	/// Represents a basic geometric projection descriptor containing field‑of‑view,
	/// aspect ratio, and near/far clipping planes.
	/// 
	/// <para>
	/// <see cref="Projection"/> stores the fundamental scalar parameters required
	/// to construct a perspective projection transform. Although this type does not
	/// generate matrices itself, it provides the canonical values used by rendering
	/// pipelines, shader systems, or future matrix utilities.
	/// </para>
	/// 
	/// <para>
	/// The structure is intentionally lightweight and immutable in layout. It is
	/// suitable for passing projection parameters between CPU subsystems, GPU
	/// kernels, or mathematical utilities without imposing a specific matrix
	/// representation.
	/// </para>
	/// </summary>
	public struct Projection {

		/// <summary>
		/// Field of view in radians. Defines the vertical viewing angle of the
		/// perspective projection.
		/// </summary>
		float m_fFov;

		/// <summary>
		/// Aspect ratio of the viewport (width divided by height).
		/// </summary>
		float m_fAspect;

		/// <summary>
		/// Distance to the near clipping plane. Must be positive and non‑zero.
		/// </summary>
		float m_fNearPlane;

		/// <summary>
		/// Distance to the far clipping plane. Must be greater than the near plane.
		/// </summary>
		float m_fFarPlane;

		/// <summary>
		/// Initializes a new <see cref="Projection"/> instance with the specified
		/// field‑of‑view, aspect ratio, and clipping plane distances.
		/// </summary>
		/// <param name="fFov">Field of view in radians.</param>
		/// <param name="fAspect">Aspect ratio (width / height).</param>
		/// <param name="fNearPlane">Near clipping plane distance.</param>
		/// <param name="fFarPlane">Far clipping plane distance.</param>
		public Projection ( float fFov, float fAspect, float fNearPlane, float fFarPlane ) {
			m_fFov = fFov;
			m_fAspect = fAspect;
			m_fNearPlane = fNearPlane;
			m_fFarPlane = fFarPlane;
		}
	}
	//@}
}
