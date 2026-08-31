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


using SystemEx.Threading.intern;

namespace SystemEx.Threading {

	/// <summary>
	/// Indicates the current operational state of an <see cref="RCUObject{T}"/>.
	/// Readers observe <see cref="Current"/> when no writer is active, and
	/// <see cref="Update"/> when a writer holds the exclusive lock.
	/// </summary>
	public enum RCUState {
		/// <summary>
		/// No writer is active; read operations observe the current stable data.
		/// </summary>
		Current,

		/// <summary>
		/// A writer is active; readers may observe stale data while the update
		/// operation is in progress.
		/// </summary>
		Update
	}
	/// <summary>
	/// Represents a minimal Read-Copy-Update (RCU) container that provides
	/// lock-free read access and exclusive write access to a protected value.
	/// Reader activity is tracked through an <see cref="Epoch"/> instance,
	/// while writers are synchronized using an <see cref="ILock"/>.
	/// </summary>
	/// <typeparam name="T">
	/// The type of the value being protected by the RCU object.
	/// </typeparam>
	public class RCUObject<T> : IEquatable<RCUObject<T>>, ILockedObject<T> {
		/// <summary>
		/// Index of the stored value inside the <see cref="Result"/> container.
		/// </summary>
		public static readonly byte VALUE_INDEX = 0;

		/// <summary>
		/// Index of the RCU state (<see cref="RCUState"/>) inside the
		/// <see cref="Result"/> container.
		/// </summary>
		public static readonly byte STATE_INDEX = 1;

		/// <summary>
		/// Index of the reader epoch counter inside the <see cref="Result"/> container.
		/// </summary>
		public static readonly byte COUNT_INDEX = 2;

		private T? m_object;
		private Epoch m_epochReader;
		private ILock m_writerLock;

		private RCUGlobalDomain<T> m_rcuWriter;

		/// <summary>
		/// Gets or sets the protected value. Reading the value enters a reader
		/// epoch, while writing requires exclusive writer access and is only
		/// permitted when no readers are active.
		/// </summary>
		public T? Value {
			get => (T?)ReadValue().Get(VALUE_INDEX);
			set => WriteValue( value );
		}

		/// <summary>
		/// Retrieves the current value together with its associated RCU metadata.
		/// 
		/// <para>
		/// The returned <see cref="Result"/> contains:
		/// <list type="bullet">
		/// <item><description><c>[0]</c> — the stored value</description></item>
		/// <item><description><c>[1]</c> — the current RCU state (<see cref="RCUState"/>)</description></item>
		/// <item><description><c>[2]</c> — the active reader epoch count</description></item>
		/// </list>
		/// </para>
		/// </summary>
		public Result ValueWithState => ReadValue();

		/// <summary>
		/// Initializes a new instance of the <see cref="RCUObject{T}"/> class
		/// using a default <see cref="LightLock"/> for writer synchronization.
		/// </summary>
		/// <param name="orig">The initial value to store.</param>
		public RCUObject ( T orig ) {
			m_object = orig;
			m_epochReader = new Epoch();
			m_writerLock = new LightLock();
			m_rcuWriter = new RCUGlobalDomain<T>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RCUObject{T}"/> class
		/// using a custom writer lock implementation.
		/// </summary>
		/// <param name="orig">The initial value to store.</param>
		/// <param name="writer">
		/// The lock instance used to synchronize writer access.
		/// </param>
		public RCUObject (T orig, ILock writer ) {
			m_object = orig;
			m_epochReader = new Epoch();
			m_writerLock = writer;
			m_rcuWriter = new RCUGlobalDomain<T>();
		}

		/// <summary>
		/// Writes a new value to the RCU object. Writer access is exclusive and
		/// requires acquiring the writer lock. The write operation is only allowed
		/// when no readers are currently active.
		/// </summary>
		/// <param name="value">The new value to store.</param>
		/// <exception cref="InvalidOperationException">
		/// Thrown when one or more readers are active during the write attempt.
		/// </exception>
		public void WriteValue ( T? value , int timeout = -1) {
			
			using ( var lk = new ScopedLock<ILock>(ref m_writerLock, timeout) ) {
				if ( m_epochReader )
					throw new InvalidOperationException("Readers active");

				m_rcuWriter.Push(value);
			}	
		}

		/// <summary>
		/// Reads the current value from the RCU object. The reader epoch is
		/// entered upon beginning the read operation and exited automatically
		/// when the scope ends, ensuring deterministic tracking of reader
		/// activity even in the presence of exceptions.
		/// </summary>
		/// <returns>The currently stored value.</returns>
		public Result ReadValue (int timeout = -1 ) {
			using (var _le = new UniqueEpoch(ref m_epochReader) ) {
				var _res = new Result();

				m_object = m_rcuWriter.Pop(m_object);

				_res[VALUE_INDEX] = m_object;
				_res[STATE_INDEX] = m_writerLock.IsHeld ? RCUState.Update : RCUState.Current;
				_res[COUNT_INDEX] = m_epochReader.Value;

				return _res;
			}
		}


		/// <summary>
		/// Implicitly converts a value of type <typeparamref name="T"/> into an
		/// <see cref="RCUObject{T}"/> instance. This allows direct assignment of
		/// a raw value to an RCU container without explicitly invoking the constructor.
		/// </summary>
		/// <param name="value">The value to wrap inside the RCU object.</param>
		/// <returns>
		/// A new <see cref="RCUObject{T}"/> containing the specified value.
		/// </returns>
		public static implicit operator RCUObject<T> ( T value ) {
			return new RCUObject<T>(value);
		}

		/// <summary>
		/// Determines whether this instance and the specified <see cref="RCUObject{T}"/>
		/// contain equal values. Both objects are read using RCU semantics, ensuring
		/// lock‑free comparison of the protected values.
		/// </summary>
		public override bool Equals ( object? obj ) {
			bool _ret = false;

			if ( obj is RCUObject<T> b ) {
				_ret = this.Equals(b);
			}
			return _ret;
		}

		/// <summary>
		/// Determines whether this instance and the specified <see cref="RCUObject{T}"/>
		/// contain equal values. Both objects are read using RCU semantics, ensuring
		/// lock‑free comparison of the protected values.
		/// </summary>
		public bool Equals ( RCUObject<T>? obj ) {
			if ( obj == null ) return false;

			bool _ret = false;
			var _thisValue = ReadValue().GetAs<T>(0);
			var _otherValue = obj.ReadValue().GetAs<T>(0);

			if ( _thisValue.HasValue && _otherValue.HasValue ) {
				_ret = _thisValue.Value!.Equals(_otherValue.Value);
			}
			return _ret;
		}


		/// <summary>
		/// Computes a hash code based on the protected value. The value is retrieved
		/// using RCU semantics, ensuring deterministic behavior even under concurrent
		/// access.
		/// </summary>
		public override int GetHashCode () {
			var _thisValue = ReadValue().GetAs<T>(0); 
			return _thisValue.Value!.GetHashCode();
		}

		/// <summary>
		/// Returns a string representation of the protected value. The value is read
		/// using RCU semantics and converted via its <c>ToString()</c> implementation.
		/// </summary>
		public override string ToString () {
			var _thisValue = ReadValue().GetAs<T>(0);

			return _thisValue.ToString();
		}

		/// <summary>
		/// Determines whether two <see cref="RCUObject{T}"/> instances contain equal
		/// values. Both operands are compared using RCU‑safe read operations.
		/// </summary>
		public static bool operator == (RCUObject<T>? a, RCUObject<T>? b) {
			if ( a == null) return false;

			return a.Equals(b);
		}
		/// <summary>
		/// Determines whether two <see cref="RCUObject{T}"/> instances contain
		/// different values. This is the logical negation of <see cref="operator=="/>.
		/// </summary>
		public static bool operator != ( RCUObject<T>? a, RCUObject<T>? b ) {
			return !(a == b);
		}
	}
	
}
