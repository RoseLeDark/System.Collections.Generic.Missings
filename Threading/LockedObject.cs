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



namespace SystemEx.Threading {
	// \addtogroup SystemEx.Threading
	/// @{
	/// <summary>
	/// Provides a minimal lock‑protected wrapper around a value of type
	/// <typeparamref name="T"/> using a caller‑supplied lock implementation.
	/// 
	/// <para>
	/// <see cref="LockedObject{T, TL}"/> offers simple, scoped access to a
	/// protected value. Read and write operations attempt to acquire the
	/// underlying lock and return <c>false</c> if the lock cannot be obtained
	/// within the specified timeout.
	/// </para>
	/// 
	/// <para>
	/// This type is intentionally lightweight and does not provide RAII‑style
	/// access or complex synchronization semantics. It is suitable for small,
	/// isolated critical sections where a single value must be protected.
	/// </para>
	/// </summary>
	/// <typeparam name="T">The type of the stored value.</typeparam>
	/// <typeparam name="TL">
	/// The lock type used to guard access. Must implement <see cref="ILock"/>.
	/// </typeparam>
	public class LockedObject<T> : IEquatable<LockedObject<T>>, ILockedObject<T>  {

		private ILock m_lock;
		private T? m_value;

		/// <summary>
		/// Gets or sets the protected value. Accessing the value attempts to
		/// acquire the underlying lock. If the lock cannot be obtained, an
		/// <see cref="UnauthorizedAccessException"/> is thrown.
		/// </summary>
		public T? Value {
			get {
				using ( var _l = new ScopedLock<ILock>(ref m_lock) ) {
					return m_value;
				}
			}
			set => WriteValue(value);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LockedObject{T}"/>
		/// class using the specified initial value.
		/// </summary>
		/// <param name="value">The initial value to store.</param>
		public LockedObject ( T value) {
			m_lock = new LightLock();
			m_value = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LockedObject{T}"/>
		/// class using the specified initial value and lock implementation.
		/// </summary>
		/// <param name="value">The initial value to store.</param>
		/// <param name="l">The lock instance used to guard access.</param>
		public LockedObject ( T value, ILock l ) {
			m_lock = l;
			m_value = value;
		}

	
		public Result ReadValue (int timeoutms = -1 ) {
			try {
				using ( var _l = new ScopedLock<ILock>(ref m_lock, timeoutms) ) {
					Result _res = new Result();
					_res[0] = m_value;
					return _res;
				}
			} catch (Exception ex) {
				return new Result(ex);
			}
		}

		/// <summary>
		/// Attempts to write a new value. The method tries to acquire the
		/// underlying lock within the specified timeout. If successful, the
		/// stored value is updated.
		/// </summary>
		/// <param name="value">The new value to store.</param>
		/// <param name="timeoutms">
		/// The timeout in milliseconds. A value of <c>-1</c> waits indefinitely.
		/// </param>
		/// <returns>
		/// <c>true</c> if the lock was acquired and the value updated;
		/// otherwise <c>false</c>.
		/// </returns>
		public void WriteValue ( T? value, int timeoutms = -1 ) {
			using ( var _l = new ScopedLock<ILock>(ref m_lock, timeoutms) ) {
				m_value = value;
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
		public static implicit operator LockedObject<T> ( T value ) {
			return new LockedObject<T>(value);
		}

		/// <summary>
		/// Determines whether this instance and the specified <see cref="LockedObject{T}"/>
		/// contain equal values. Both objects are read using RCU semantics, ensuring
		/// lock‑free comparison of the protected values.
		/// </summary>
		public bool Equals ( LockedObject<T>? obj ) {
			if ( obj == null ) return false;

			bool _ret = false;
			var _thisValue = ReadValue();
			var _otherValue = obj.ReadValue();

			if ( _thisValue.Get() != null && _otherValue.Get() != null ) {
				_ret = _thisValue.Get()!.Equals(_otherValue.Get());
			}
			return _ret;
		}
	}
	/// @}

}
