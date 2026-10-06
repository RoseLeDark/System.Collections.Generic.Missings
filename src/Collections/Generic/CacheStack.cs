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

namespace SystemEx.Collections.Generic {

    /// <summary>
    /// A fixed‑size, backward‑growing stack implementation that operates on a shared cache buffer.
    /// </summary>
    public ref struct CacheStack<T>
    {
        private ref Cache m_cache;

        /// <summary>
        /// Global lower boundary for the main stack.
        /// </summary>
        private long m_end;

        /// <summary>
        /// Global upper boundary for the main stack.
        /// </summary>
        private long m_start;

        /// <summary>
        /// Current pointer for the main stack.
        /// </summary>
        private long m_current;

        /// <summary>
        /// Indicates whether the main stack is full.
        /// </summary>
        public bool IsFull => m_current == m_end;

        /// <summary>
        /// Indicates whether the main stack is empty.
        /// </summary>
        public bool IsEmpty => m_current == Size;

        public long Size
        {
            get { return (long)(m_cache.Length-1); }
        }
        /// <summary>
        /// Initializes a fixed‑size, backward‑growing stack implementation that operates on a shared cache buffer.
        /// </summary>
        public CacheStack(ref Cache cache)
        {
            m_cache = ref cache;
            m_end = 0;
            m_start = (long)(m_cache.Length - 1);
            m_current = (long)(m_cache.Length - 1);
        } 

        /// <summary>
        /// Initializes a fixed‑size, backward‑growing stack implementation that operates on a shared cache buffer.
        /// </summary>
        public CacheStack(ref Cache cache, uint start, uint end = 0)
        {
            if(end >= cache.Length)
                throw new ArgumentOutOfRangeException(nameof(end), "End index must be within the bounds of the cache.");

            m_cache = ref cache;

            if(start >= cache.Length)
                m_start = (long)(m_cache.Length - 1);

            if(start < end) {
                m_end = (long)start;
                m_start = (long)end;   
            } else {
                m_start = (long)start;
                m_end = (long)end;
            }

            m_current = m_end;
        }
        /// <summary>
        /// Pushes an element onto the stack.
        /// </summary>
        public bool Push(byte element) {
            if ( IsFull ) return false;
            m_cache[m_current] = element;
            m_current--;
            return true;
        }
        public long Push(byte[] bytes) {
            long _pushed = 0;

            for (int i = 0; i < bytes.Length; i++) {
                if ( IsFull ) break;

                m_cache[m_current] = bytes[i];
                m_current--;
                _pushed++;
            }
            return _pushed;
        }

        /// <summary>
        /// Pops an element from the stack.
        /// </summary>
		public bool Pop(out byte item) {
            if ( IsEmpty ) { item = default; return false; }
            item = m_cache[m_current];
            m_current++;
            return true;
        }

        public long Pop(byte[] buffer) {
            long _popped = 0;

            for (int i = 0; i < buffer.Length; i++) {
                if ( IsEmpty ) break;

                buffer[i] = m_cache[m_current];
                m_current++;
                _popped++;
            }
            return _popped;
        }

        /// <summary>
        /// Retrieves the top element of the stack without removing it.
        /// </summary>
        public bool Peek(ref byte item) {
            if ( IsEmpty ) return false;
            item = m_cache[m_current];
            return true;
        }

        public long Peek(byte[] buffer) {
            long _peeked = 0;

            for (int i = 0; i < buffer.Length; i++) {
                if ( IsEmpty ) break;

                buffer[i] = m_cache[m_current];
                _peeked++;
            }
            return _peeked;
        }

        
    }

}