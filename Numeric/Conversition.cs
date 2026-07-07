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


namespace SystemEx.Numeric.Utils {
    /// \addtogroup Numeric
    /// @{
    /// \addtogroup Conversition
    /// @{
    /// <summary>
    /// Provides extension methods for converting between different vector types
    /// (float, double, int) and dimensions (2D, 3D, 4D).
    /// </summary>
    public static class Conversition {

        /// <summary>
        /// Converts a 3D float vector to a 2D float vector by dropping the Z component.
        /// </summary>
        public static Vec2f ToVec2f ( this Vec3f vec ) {
            return new Vec2f(vec.X, vec.Y);
        }
        /// <summary>
        /// Converts a 4D float vector to a 2D float vector by dropping the Z and W components.
        /// </summary>
        public static Vec2f ToVec2f ( this Vec4f vec ) {
            return new Vec2f(vec.X, vec.Y);
        }
        /// <summary>
        /// Converts a 2D float vector to a 3D float vector, setting Z to zero.
        /// </summary>
        public static Vec3f ToVec3f ( this Vec2f vec ) {
            return new Vec3f(vec.X, vec.Y, 0.0f);
        }
        /// <summary>
        /// Converts a 4D float vector to a 3D float vector by dropping the W component.
        /// </summary>
        public static Vec3f ToVec3f ( this Vec4f vec ) {
            return new Vec3f(vec.X, vec.Y, vec.Z);
        }
        /// <summary>
        /// Converts a 2D float vector to a 4D float vector, setting Z and W to zero.
        /// </summary>
        public static Vec4f ToVec4f ( this Vec2f vec ) {
            return new Vec4f(vec.X, vec.Y, 0, 0);
        }
        /// <summary>
        /// Converts a 3D float vector to a 4D float vector, setting W to zero.
        /// </summary>
        public static Vec4f ToVec4f ( this Vec3f vec ) {
            return new Vec4f(vec.X, vec.Y, vec.Z, 0);
        }

        /// <summary>
        /// Converts a 3D double vector to a 2D double vector by dropping the Z component.
        /// </summary>
        public static Vec2d ToVec2d ( this Vec3d vec ) {
            return new Vec2d(vec.X, vec.Y);
        }
        /// <summary>
        /// Converts a 4D double vector to a 2D double vector by dropping the Z and W components.
        /// </summary>
        public static Vec2d ToVec2d ( this Vec4d vec ) {
            return new Vec2d(vec.X, vec.Y);
        }
        /// <summary>
        /// Converts a 2D double vector to a 3D double vector, setting Z to zero.
        /// </summary>
        public static Vec3d ToVec3d ( this Vec2d vec ) {
            return new Vec3d(vec.X, vec.Y, 0);
        }
        /// <summary>
        /// Converts a 4D double vector to a 3D double vector
        /// </summary>
        public static Vec3d ToVec3d ( this Vec4d vec ) {
            return new Vec3d(vec.X, vec.Y, vec.Z);
        }
        /// <summary>
        /// Converts a 2D double vector to a 4D double vector, setting Z and W to zero.
        /// </summary>
        public static Vec4d ToVec4d ( this Vec2d vec ) {
            return new Vec4d(vec.X, vec.Y, 0, 0);
        }
        /// <summary>
        /// Converts a 3D double vector to a 4D double vector, setting Z to zero.
        /// </summary>
        public static Vec4d ToVec4d ( this Vec3d vec ) {
            return new Vec4d(vec.X, vec.Y, vec.Z, 0);
        }

        /// <summary>
        /// Converts a 3D int vector to a 2D int vector by dropping the Z component.
        /// </summary>
        public static Vec2i ToVec2i ( this Vec3i vec ) {
            return new Vec2i(vec.X, vec.Y);
        }

        /// <summary>
        /// Converts a 4D int vector to a 2D int vector by dropping the Z and W component.
        /// </summary>
        public static Vec2i ToVec2i ( this Vec4i vec ) {
            return new Vec2i(vec.X, vec.Y);
        }

        /// <summary>
        /// Converts a 2D int vector to a 3D int vector , set Z to 0
        /// </summary>
        public static Vec3i ToVec3i ( this Vec2i vec ) {
            return new Vec3i(vec.X, vec.Y, 0);
        }
        /// <summary>
        /// Converts a 4D int vector to a 3D int vector 
        /// </summary>
        public static Vec3i ToVec3i ( this Vec4i vec ) {
            return new Vec3i(vec.X, vec.Y, vec.Z);
        }

        /// <summary>
        /// Converts a 2D int vector to a 4D int vector 
        /// </summary>
        public static Vec4i ToVec4i ( this Vec2i vec ) {
            return new Vec4i(vec.X, vec.Y, 0, 0);
        }
        /// <summary>
        /// Converts a 3D int vector to a 4D int vector 
        /// </summary>
        public static Vec4i ToVec4i ( this Vec3i vec ) {
            return new Vec4i(vec.X, vec.Y, vec.Z, 0);
        }
        /// <summary>
        /// Converts a 2D double vector to a 2D float vector 
        /// </summary>
        public static Vec2f ToVec2f ( this Vec2d vec ) {
            return new Vec2f((float)vec.X, (float)vec.Y);
        }
        /// <summary>
        /// Converts a 3D double vector to a 3D float vector 
        /// </summary>
        public static Vec3f ToVec3f ( this Vec3d vec ) {
            return new Vec3f((float)vec.X, (float)vec.Y, (float)vec.Z);
        }
        /// <summary>
        /// Converts a 4D double vector to a 4D float vector 
        /// </summary>
        public static Vec4f ToVec4f ( this Vec4d vec ) {
            return new Vec4f((float)vec.X, (float)vec.Y, (float)vec.Z, (float)vec.W);
        }
        /// <summary>
        /// Converts a 2D float vector to a 2D double vector 
        /// </summary>
        public static Vec2d ToVec2d ( this Vec2f vec ) {
            return new Vec2d(vec.X, vec.Y);
        }
        /// <summary>
        /// Converts a 3D float vector to a 3D double vector 
        /// </summary>
        public static Vec3d ToVec3d ( this Vec3f vec ) {
            return new Vec3d(vec.X, vec.Y, vec.Z);
        }
        /// <summary>
        /// Converts a 4D float vector to a 4D double vector 
        /// </summary>
        public static Vec4d ToVec4d ( this Vec4f vec ) {
            return new Vec4d(vec.X, vec.Y, vec.Z, vec.W);
        }

        /// <summary>
        /// Converts a 2D double vector to a 2D int vector 
        /// </summary>
        public static Vec2i ToVec2i ( this Vec2d vec ) {
            return new Vec2i((int)vec.X, (int)vec.Y);
        }
        /// <summary>
        /// Converts a 3D double vector to a 3D int vector 
        /// </summary>
        public static Vec3i ToVec3i ( this Vec3d vec ) {
            return new Vec3i((int)vec.X, (int)vec.Y, (int)vec.Z);
        }
        /// <summary>
        /// Converts a 4D double vector to a 4D int vector 
        /// </summary>
        public static Vec4i ToVec4i ( this Vec4d vec ) {
            return new Vec4i((int)vec.X, (int)vec.Y, (int)vec.Z, (int)vec.W);
        }

        /// <summary>
        /// Converts a 2D float vector to a 2D int vector 
        /// </summary>
        public static Vec2i ToVec2i ( this Vec2f vec ) {
            return new Vec2i((int)vec.X, (int)vec.Y);
        }
        /// <summary>
        /// Converts a 3D float vector to a 3D int vector 
        /// </summary>
        public static Vec3i ToVec3i ( this Vec3f vec ) {
            return new Vec3i((int)vec.X, (int)vec.Y, (int)vec.Z);
        }
        /// <summary>
        /// Converts a 4D float vector to a 4D int vector 
        /// </summary>
        public static Vec4i ToVec4i ( this Vec4f vec ) {
            return new Vec4i((int)vec.X, (int)vec.Y, (int)vec.Z, (int)vec.W);
        }
    }
    /// @}
    /// @}
}
