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

namespace SystemEx.Numeric {

    public struct Uint256 {
        public Int128 Low;
        public Int128 High;
    }

    public interface IHalf<TSelf> : IFloat<TSelf, ushort>
        where TSelf : struct, IHalf<TSelf> {
    }

    public interface IMini<TSelf> : IFloat<TSelf, byte>
        where TSelf : struct, IMini<TSelf> {
    }
    public interface ICFloat<TSelf> : IFloat<TSelf, uint>
        where TSelf : struct, ICFloat<TSelf> {
    }
    public interface ICDouble<TSelf> : IFloat<TSelf, ulong>
        where TSelf : struct, ICDouble<TSelf> {
    }

    public interface ICQuad<TSelf> : IFloat<TSelf, UInt128>
        where TSelf : struct, ICQuad<TSelf> {
    }

    public interface IBigFloat<TSelf> : IFloat<TSelf, Uint256>
        where TSelf : struct, IBigFloat<TSelf> {
    }
}
