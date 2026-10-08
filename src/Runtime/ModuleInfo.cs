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

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using SystemEx.Collections.Generic;

namespace SystemEx.Runtime {
    public enum HashType
    {
        Bernstein,
        CRC32,
        SHA256,
        MURMUR3,
        None

    }
    public struct ModuleInfo : IEquatable<ModuleInfo>
    {
        public string RealName { get; }
        public string HumanName { get; }

        public string Hash { get; }

        public HashType HashType { get; }

        public string Version { get; }

        public bool Equals (ModuleInfo other )
        {
            if(RealName != other.RealName) return false;
            if(HumanName != other.HumanName) return false;
            if(Hash != other.Hash) return false;
            if(Version != other.Version) return false;

            return true;
        }
        public override bool Equals(object obj)
        {
            if(obj is ModuleInfo other) 
                return Equals(other);
            return false;
        }
        public override int GetHashCode()=>  base.GetHashCode();
        public static bool operator == (ModuleInfo a, ModuleInfo b) => a.Equals(b);
        public static bool operator != (ModuleInfo a, ModuleInfo b) => !(a.Equals(b));
    }


}