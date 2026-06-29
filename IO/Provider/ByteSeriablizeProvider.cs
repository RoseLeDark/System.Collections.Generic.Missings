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

using System;
using System.Collections.Generic;
using System.Text;
using SystemEx.Collections.Generic;
using SystemEx.Collections.Generic.Interfaces;
using SystemEx.SystemEx.Drawing;

namespace SystemEx.IO.Provider {
    /// <summary>
    /// 
    /// </summary>
    public abstract class ByteSeriablizeProvider  {
        private IByteFormatSchema m_schema;
        private Endian m_endian;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="endian"></param>
        public ByteSeriablizeProvider(IByteFormatSchema schema, Endian endian) {
            m_schema = schema;
            m_endian = endian;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Cache? ToBytes<T>(T? obj) {
            if ( obj == null ) return null;
            
            var ret = new Cache((int)(m_schema.TotalSize / 8), CacheType.Both); // Bits → Bytes

            foreach ( Pair<string, long> entry in m_schema.Offsets ) {
                var name = entry.First;
                var offset = entry.Second;

                Array<byte>? curr = GetBytesForEntry(obj, name, m_endian);
                if ( curr != null ) {
                    ret.WriteRange((ulong)offset, curr.ToArray());
                }
            }

            return ret;
        }
       /// <summary>
       /// 
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <param name="obj"></param>
       /// <returns></returns>
        public T? FromBytes<T>(Cache obj)  {
 
            var entries = new Map<string, byte[]>();

            foreach ( Pair<string, long> entry in m_schema.Offsets ) {
                string name = entry.First;
                long offset = entry.Second;

                var size = GetEntrySize(obj, name, m_endian);
                if ( size <= 0 ) continue; 

                byte[] raw = obj.ReadRange((ulong)offset, (uint)size)!;

                entries.Add(name, raw);
            }
            return (T)CreateObjectFromEntrys(entries, m_endian)!;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj">The object</param>
        /// <param name="name">The entrry</param>
        /// <param name="endian">Endian</param>
        /// <returns></returns>
        protected abstract Array<byte> GetBytesForEntry(object obj, string name, Endian endian);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <param name="endian"></param>
        /// <returns></returns>
        protected abstract long GetEntrySize(Cache obj, string name, Endian endian); // return <= 0 nothing
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entries"></param>
        /// <param name="endian"></param>
        /// <returns></returns>
        protected abstract object? CreateObjectFromEntrys(Map<string, byte[]> entries, Endian endian);

    }
}
