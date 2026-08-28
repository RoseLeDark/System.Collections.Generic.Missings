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

namespace SystemEx {



	/// <summary>
	/// Provides metadata about the SystemEx framework, including versioning,
	/// build information, platform API, release state, and fork/user identity.
	/// 
	/// This class is intended to be used by applications or libraries that
	/// reference SystemEx to determine runtime characteristics such as:
	/// - Framework version (semantic + integer)
	/// - Build identifiers
	/// - Platform API (Windows, Linux, MacOS, AnyCPU)
	/// - Release state (Beta, Release, Debug)
	/// - Fork state and user identity
	///
	/// When the framework is forked, contributors may set <see cref="User"/>
	/// to identify their fork, similar to how Linux kernel maintainers add
	/// their own signature.
	/// 
	/// This class also exposes utility functions such as <see cref="Infos"/> for
	/// retrieving structured information similar to the Unix 'uname' command,
	/// and <see cref="IsCompatible"/> for validating version compatibility.
	/// </summary>
	public static class Framework {

		/// <summary>
		/// Defines the level of detail returned by <see cref="Infos"/>.
		/// </summary>
		public enum InfoLevel {
			/// <summary>
			/// Minimal information, similar to the Unix 'uname' command.
			/// </summary>
			Minimal,

			/// <summary>
			/// Extended information including version, codename, API, and flags.
			/// </summary>
			Extended,

			/// <summary>
			/// Full diagnostic information including all metadata fields.
			/// </summary>
			Full

		}

		/// <summary>
		/// Gets the human‑readable semantic version of the framework.
		/// </summary>
		public static string Version => "0.92.1902";
		/// <summary>
		/// Gets the current code name
		/// </summary>
		public static string CodeName => "Lacking";

		/// <summary>
		/// Gets the platform API identifier based on the current build target.
		/// Possible values:
		/// - Windows
		/// - Linux
		/// - MacOS
		/// - AnyCPU (fallback)
		/// </summary>
#if WINDOWS
		public static string API => "Windows";
#elif LINUX
		public static string API => "Linux";
#elif MACOS
		public static string API => "MacOS";
#else
		public static string API => "AnyCPU";
#endif
		/// <summary>
		/// Gets the major version component.
		/// </summary>
		public static int Major => 0;

		/// <summary>
		/// Gets the minor version component.
		/// </summary>
		public static int Minor => 92;

		/// <summary>
		/// Gets the build number of this version.
		/// </summary>
		public static int Build => 1902;

		/// <summary>
		/// Gets the integer‑encoded version number.
		/// Useful for fast comparison and binary compatibility checks.
		/// </summary>
		public static uint iVersion => 0x00009201902;

		/// <summary>
		/// Indicates whether this build is a beta version.
		/// </summary>
		public static bool IsBeta => true;

		/// <summary>
		/// Indicates whether this build is a stable release version.
		/// </summary>
		public static bool IsRelease => false;

		/// <summary>
		/// Indicates whether this framework instance is a fork.
		/// Fork maintainers may set this to true and specify their name in <see cref="User"/>.
		/// </summary>
		public static bool IsForked => false;

		/// <summary>
		/// Indicates whether this build was compiled in DEBUG mode.
		/// </summary>
#if DEBUG
		public static bool IsDebug => true;
#else
    public static bool IsDebug => false;
#endif

		/// <summary>
		/// Gets the user or maintainer name associated with this fork.
		/// If the framework is forked, contributors may set this value to identify
		/// their fork, similar to how Linux kernel maintainers annotate their builds.
		/// </summary>
		public static string User => "";

		/// <summary>
		/// Returns a formatted string containing framework information.
		/// The level of detail is controlled by <paramref name="level"/>.
		/// 
		/// Minimal: Similar to 'uname', returns API and Version.
		/// Extended: Adds codename, debug/release flags, and fork info.
		/// Full: Includes all metadata fields.
		/// </summary>
		/// <param name="level">The desired detail level.</param>
		/// <returns>A formatted information string.</returns>
		public static string Infos ( InfoLevel level ) {
			switch ( level ) {
			case InfoLevel.Minimal:
			return $"{API} {Version}";

			case InfoLevel.Extended:
			return
				$"API: {API}\n" +
				$"Version: {Version}\n" +
				$"Codename: {CodeName}\n" +
				$"Debug: {IsDebug}\n" +
				$"Beta: {IsBeta}\n" +
				$"Forked: {IsForked}\n" +
				$"User: {User}";

			case InfoLevel.Full:
			default:
			return
				$"API: {API}\n" +
				"Version: {Version}\n" +
				$"Major: {Major}\n" +
				$"Minor: {Minor}\n" +
				$"Build: {Build}\n" +
				$"iVersion: {iVersion}\n" +
				$"Codename: {CodeName}\n" +
				$"Debug: {IsDebug}\n" +
				$"Beta: {IsBeta}\n" +
				$"Release: {IsRelease}\n" +
				$"Forked: {IsForked}\n" +
				$"User: {User}";
			}
		}

		/// <summary>
		/// Determines whether the specified version is compatible with the current
		/// SystemEx version. Compatibility is defined as:
		/// - Same major version
		/// - Requested minor version gt;= current minor version
		/// 
		/// This ensures backward compatibility within the same major version.
		/// </summary>
		/// <param name="major">The major version to check.</param>
		/// <param name="minor">The minor version to check.</param>
		/// <returns>True if compatible; otherwise false.</returns>
		public static bool IsCompatible ( int major, int minor ) {
			if ( major != Major )
				return false;

			return minor <= Minor;
		}

		/// <summary>
		/// Builds a compact, human-readable identifier string for the SystemEx framework.
		/// 
		/// The format is:
		/// <c>SystemEx-{Version}-{CodeName}{BetaFlag}{ForkFlag}</c>
		/// 
		/// Flags:
		/// - <c>-bt</c>   : appended when <see cref="IsBeta"/> is true
		/// - <c>-fk</c>   : appended when <see cref="IsForked"/> is true and no user name is set
		/// - <c>-{User}</c> : appended when <see cref="IsForked"/> is true and a user name is provided
		/// 
		/// This string is intended for logging, diagnostics, CLI tools, and
		/// "uname"-style information output. It is intentionally minimal and stable.
		/// </summary>
		/// <returns>A formatted build identifier string.</returns>
		public static string BuildString() {
			var _isBeta = IsBeta ? "-bt" : "";
			var _forked = IsForked ? User == string.Empty ? "-fk" : $"-{User}" : "";

			return $"SystemEx-{Version}-{CodeName}{_isBeta}{_forked}";
		}
	}
}
