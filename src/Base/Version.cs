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
 * Change: Added English XML documentation. (2026-10-08)
 */

using SystemEx.Collections.Generic;

namespace SystemEx {
    /// <summary>
    /// Represents a mutable version identifier with major, minor, and build
    /// components and additional build metadata.
    /// </summary>
    public struct Version 
        : IEquatable<Version>, 
          IEquatable<int>, 
          IEquatable< Pair<int, int> >, 
          IEquatable<int[]>
    {  
        private const int MaxMinor = 100;
        private const int MaxBuid = 9999;

        private int m_major;
        private int m_minor;
        private int m_build;

        private bool m_isBeta;
        private bool m_isRelease;

        private bool m_debug;
        private bool m_isForked;

        private string m_text;

        /// <summary>
        /// Gets or sets the major version component.
        /// </summary>
        /// <value>The major version component.</value>
		public int Major { get => m_major; set => m_major = value; }

		/// <summary>
		/// Gets or sets the minor version component.
		/// </summary>
        /// <value>The minor version component.</value>
		public int Minor { get => m_minor; set => m_minor = value; }

		/// <summary>
		/// Gets or sets the build number of this version.
		/// </summary>
        /// <value>The build number.</value>
		public int Build { get => m_build; set => m_build = value; }


		/// <summary>
		/// Gets or sets whether this version represents a beta build.
		/// </summary>
        /// <value><see langword="true"/> if this is a beta build; otherwise, <see langword="false"/>.</value>
		public bool IsBeta { get => m_isBeta; set => m_isBeta = value; }

		/// <summary>
		/// Gets or sets whether this version represents a stable release.
		/// </summary>
        /// <value><see langword="true"/> if this is a stable release; otherwise, <see langword="false"/>.</value>
		public bool IsRelease { get => m_isRelease; set => m_isRelease = value; }

		/// <summary>
		/// Gets or sets whether this version belongs to a fork.
		/// </summary>
        /// <value><see langword="true"/> if this version belongs to a fork; otherwise, <see langword="false"/>.</value>
		public bool IsForked { get => m_isForked; set => m_isForked = value; }

		/// <summary>
		/// Gets or sets whether this version was built with the <c>DEBUG</c> symbol defined.
		/// </summary>
        /// <value><see langword="true"/> if the <c>DEBUG</c> symbol was defined; otherwise, <see langword="false"/>.</value>
        public bool IsDebug { get => m_debug; set => m_debug = value; }

        /// <summary>
        /// Gets or sets the descriptive text associated with this version.
        /// </summary>
        /// <value>The descriptive text.</value>
        /// <remarks>The member name retains its historical spelling.</remarks>
        public string Discritions { get => m_text; set => m_text = value; }

        /// <summary>
        /// Initializes a version with the specified major component and zero minor and build components.
        /// </summary>
        /// <param name="major">The major version component.</param>
        public Version (int major)
            : this(major, 0, 0, false) { }

        /// <summary>
        /// Initializes a version with the specified major and minor components and a zero build component.
        /// </summary>
        /// <param name="major">The major version component.</param>
        /// <param name="minor">The minor version component.</param>
        public Version (int major, int minor)
            : this(major, minor, 0, false) { }
        
        /// <summary>
        /// Initializes a version with the specified major, minor, and build components.
        /// </summary>
        /// <param name="major">The major version component.</param>
        /// <param name="minor">The minor version component.</param>
        /// <param name="build">The build component.</param>
        public Version (int major, int minor, int build)
            : this(major, minor, build, false) { }

        /// <summary>
        /// Initializes a version with the specified components and beta status.
        /// </summary>
        /// <param name="major">The major version component.</param>
        /// <param name="minor">The minor version component.</param>
        /// <param name="build">The build component.</param>
        /// <param name="beta"><see langword="true"/> if this is a beta build; otherwise, <see langword="false"/>.</param>
        /// <remarks>
        /// The initial release and debug flags are set according to whether the
        /// <c>DEBUG</c> symbol is defined.
        /// </remarks>
        public Version (int major, int minor, int build, bool beta)
        {
            m_major = major;
            m_minor = minor;
            m_build = build;
            m_isBeta = beta;
            
            #if DEBUG
            m_isRelease = false;
            m_debug = true;
            #else 
            m_isRelease = true;
            m_debug = false;
            #endif

            m_text = "";
        }

        /// <summary>
        /// Returns this version as a string.
        /// </summary>
        /// <returns>
        /// A string in the form <c>major.minor-build</c>, with <c>-Beta</c> and
        /// <c>-dbg</c> suffixes when the corresponding flags are set.
        /// </returns>
        public override string ToString()
        {
            return $"{Major}.{Minor}-{m_build}" + 
                (IsBeta ? "-Beta" : "") + 
                (IsDebug ? "-dbg" : "");
        }

        /// <summary>
        /// Determines whether this version is equal to the specified object.
        /// </summary>
        /// <param name="obj">The object to compare with this version.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="obj"/> is a
        /// <see cref="Version"/> equal to this instance; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public override bool Equals(object? obj)
        {
            return (obj is Version other) ? Equals(other) : false;
        }

        /// <summary>
        /// Determines whether this version is equal to another version.
        /// </summary>
        /// <param name="obj">The version to compare with this instance.</param>
        /// <returns>
        /// <see langword="true"/> if all version components and metadata match;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public bool Equals(Version obj)
        {
            if(m_major != obj.m_major) return false;
            if(m_minor != obj.m_minor) return false;
            if(m_build != obj.m_build) return false;
            if(m_isBeta != obj.m_isBeta) return false;
            if(m_debug != obj.m_debug) return false;
            if(m_isRelease != obj.m_isRelease) return false;
            if(m_isForked != obj.m_isForked) return false;

            return m_text == obj.m_text;
        }

        /// <summary>
        /// Returns a hash code for this version.
        /// </summary>
        /// <returns>A hash code based on the version components and metadata.</returns>
        public override int GetHashCode()
        {
            return System.HashCode.Combine(
                m_major, m_minor, m_build, m_isBeta,
                m_debug, m_isRelease, m_isForked, m_text);
        }

        /// <summary>
        /// Determines whether the major component of this version equals the specified value.
        /// </summary>
        /// <param name="obj">The major component to compare with.</param>
        /// <returns><see langword="true"/> if the major components are equal; otherwise, <see langword="false"/>.</returns>
        public bool Equals(int obj) => (m_major == obj);

        /// <summary>
        /// Determines whether the major and minor components of this version
        /// equal the values in the specified pair.
        /// </summary>
        /// <param name="obj">The pair containing the major and minor components.</param>
        /// <returns>
        /// <see langword="true"/> if both components are equal; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool Equals( Pair<int, int> obj ) => (m_major == obj.First && m_minor == obj.Second);

        /// <summary>
        /// Determines whether this version matches the components in the specified array.
        /// </summary>
        /// <param name="obj">
        /// An array containing one component (major), two components (major and minor),
        /// or at least three components (major, minor, and build). A null array does not match.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the components selected by the array length match;
        /// otherwise, including when the array is null, <see langword="false"/>.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// The array is empty.
        /// </exception>
        public bool Equals( int[] obj )
        {
            if(obj == null) return false;

            return obj.Length switch {
                2 => (m_major == obj[0] && m_minor == obj[1]),
                1 => (m_major == obj[0]),
                _ => (m_major == obj[0] && m_minor == obj[1] && m_build == obj[2] )
            };
        }

        /// <summary>
        /// Advances this version to the next minor component and returns the updated value.
        /// </summary>
        /// <returns>The updated version.</returns>
        /// <remarks>
        /// If the build component exceeds its maximum, it is reset to zero and
        /// the minor component is advanced again. If the minor component exceeds
        /// its maximum, the major component is advanced and the minor component
        /// is reset to zero.
        /// </remarks>
        public Version Increment()
        {
            m_minor++;
            if(m_build > MaxBuid) { m_minor++; m_build = 0; }
            if(m_minor > MaxMinor) { m_major++; m_minor = 0; }
            return this;
        }

        /// <summary>
        /// Returns this version without changing its components.
        /// </summary>
        /// <returns>This version, unchanged.</returns>
        public Version Decrement()
        {
            return this;
        }

        /// <summary>
        /// Advances the specified version using <see cref="Increment"/>.
        /// </summary>
        /// <param name="ver">The version to advance.</param>
        /// <returns>The advanced version.</returns>
        public static Version operator ++ (Version ver) => ver.Increment();

        /// <summary>
        /// Returns the specified version unchanged using <see cref="Decrement"/>.
        /// </summary>
        /// <param name="ver">The version to decrement.</param>
        /// <returns>The unchanged version.</returns>
        public static Version operator -- (Version ver) => ver.Decrement();

        /// <summary>
        /// Determines whether two versions are equal.
        /// </summary>
        /// <param name="left">The first version to compare.</param>
        /// <param name="right">The second version to compare.</param>
        /// <returns>
        /// <see langword="true"/> if the versions are equal; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public static bool operator ==(Version left, Version right) => left.Equals(right);

        /// <summary>
        /// Determines whether two versions are not equal.
        /// </summary>
        /// <param name="left">The first version to compare.</param>
        /// <param name="right">The second version to compare.</param>
        /// <returns>
        /// <see langword="true"/> if the versions are not equal; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public static bool operator !=(Version left, Version right) => !left.Equals(right);
    }



}