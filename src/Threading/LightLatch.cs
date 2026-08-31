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

	/// <summary>
	/// A one-shot countdown synchronization primitive.
	/// The latch starts in the locked state and releases all waiting threads
	/// once its internal counter reaches zero. After releasing, the latch
	/// remains permanently open.
	/// </summary>
	/// <typeparam name="T">
	/// The lock type used to block threads until the latch opens.
	/// Must implement <see cref="ILock"/>.
	/// </typeparam>
	public struct LightLatch {
		private int m_counter;
		private LightConditionVariable m_cv;
		private ILock m_lock;

		public LightLatch ( int initialCount, ILock lockType, string name = "Latch" ) {
			m_counter = initialCount;
			m_cv = new LightConditionVariable(name + ".CV");
			m_lock = lockType;
		}

		public void Arrive ( int maxWait = -1 ) {
			m_lock.Lock(maxWait);

			m_counter--;
			if ( m_counter == 0 ) {
				m_cv.Broadcast();   // alle wartenden Threads wecken
			}

			m_lock.Unlock();
		}

		public void Wait (ref LightThread thread, int maxWait = -1) {
			m_lock.Lock(maxWait);

			if ( m_counter == 0 ) {
				m_lock.Unlock();
				return;
			}

			m_cv.Add(thread);
			m_lock.Unlock();

			thread.Wait(ref m_cv, ref m_lock, maxWait);
		}
	}
	
	
}
