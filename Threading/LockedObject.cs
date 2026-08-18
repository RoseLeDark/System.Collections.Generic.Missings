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
    public class LockedObject<T, TL>  where TL : ILock {

        private readonly TL m_lock;
        private T? m_value;
        private bool m_bLocked;

        public T? Value { 
            get {
                T? _out = default;
                if ( !GetValue(out _out) ) throw new UnauthorizedAccessException();
                return _out;
            }
            set => SetValue(value);
        }
        public LockedObject ( T value, TL l  ) {
            m_lock = l;
            m_value = value;
        }

        public bool GetValue(out T? value, int timeoutms = -1) {
            bool _ret = false;
            value = default;

            if( m_lock.Lock(timeoutms) ) {
				m_bLocked = true;
				value = m_value;
                _ret = true;
                
                m_lock.Unlock();
            }
			m_bLocked = false;
			return _ret;
        }
        public bool SetValue(T? value, int timeoutms = -1) {
            bool _ret = false;
            
            if ( m_lock.Lock(timeoutms) ) {
				m_bLocked = true;
				m_value = value;
                _ret = true;
                m_lock.Unlock();
            }
			m_bLocked = false;
			return _ret;
        }
    }


    public readonly struct UniqueLock<T, TL> : IDisposable
         where TL : ILock  {
        private readonly TL m_lock;
        public T Value { get; }

        public UniqueLock ( TL l, ref T value, int timeout = -1) {
            m_lock = l;

            if ( !m_lock.Lock(timeout) )
                throw new UnauthorizedAccessException();

            Value = value;
        }

        public void Dispose () {
            m_lock.Unlock();
        }
    }
}
