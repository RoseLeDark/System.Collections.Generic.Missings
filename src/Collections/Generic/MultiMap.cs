using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Collections.Generic {

	/// <summary>
	/// Represents a sparse, linear key–value container, with multkex support.
	/// <para>
	/// Automatic growth is controlled through <see cref="Map{T, TU}.GrowSize"/> and
	/// <see cref="Map{T, TU}.AutoGrow"/>. When enabled, the internal buffer expands
	/// automatically to accommodate new elements.
	/// </para>
	/// </summary>
	/// <typeparam name="T">Key type. Must be non‑nullable.</typeparam>
	/// <typeparam name="TU">Value type.</typeparam>
	public class MultiMap<T, TU> : Map<T, TU> where T : notnull {

		/// <summary>
		/// Initializes a new  eempty map.
		/// </summary>
		public MultiMap () 
			: base() { }
		/// <summary>
		/// Initializes a new map with the specified initial capacity.
		/// </summary>
		public MultiMap ( long size, int growSize = 16 ) 
			: base(size, growSize) { }
		/// <summary>
		/// Initializes a new map by copying an existing array of pairs.
		/// </summary>
		public MultiMap ( Pair<T, TU>[] e, int growSize = 16 ) 
			: base(e, growSize) { }
		/// <summary>
		/// Initializes a new map from an enumerable sequence of pairs.
		/// </summary>
		public MultiMap ( IEnumerable<Pair<T, TU>> e, int growSize = 16 ) 
			: base(e, growSize) { }

		/// <summary>
		/// Initializes a new map by copying another map instance.
		/// </summary>
		public MultiMap ( MultiMap<T, TU> other ) 
			: base(other) { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public override bool Replace ( T key, Optional<TU> value ) {
			if(value.HasValue)
				return PushBack(key, value.Value!);
			return false;
		}
		/// <summary>
		/// return always flase
		/// </summary>
		protected override bool intContainsKey ( T Key ) {
			return false;
		}
	}
}
