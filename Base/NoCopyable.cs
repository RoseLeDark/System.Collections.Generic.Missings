using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx {
	/// <summary>
	/// Base class that strictly forbids any form of copying.
	/// Pedantisch safe: no cloning, no copy constructor, no MemberwiseClone.
	/// </summary>
	public abstract class NoCopyable {
		/// <summary>
		/// Pedantisch safe: private copy constructor throws.
		/// Prevents accidental or reflection-based copying.
		/// </summary>
		private NoCopyable ( NoCopyable other ) {
			throw new NotSupportedException("Copy constructor is disabled.");
		}

		/// <summary>
		/// Protected default constructor.
		/// </summary>
		protected NoCopyable () {
		}

		/// <summary>
		/// Pedantisch safe: cloning is forbidden.
		/// </summary>
		public object Clone ()
			=> throw new NotSupportedException("Clone is disabled.");

		/// <summary>
		/// Pedantisch safe: MemberwiseClone is blocked.
		/// </summary>
		protected new object MemberwiseClone ()
			=> throw new NotSupportedException("MemberwiseClone is disabled.");
	}
}
