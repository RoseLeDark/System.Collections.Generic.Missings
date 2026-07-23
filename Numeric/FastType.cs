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
using System.Numerics;
using System.Text;
using SystemEx.Collections.Generic;


namespace SystemEx.Numeric {

    public interface IFastType {
        public byte Count { get; }

        public byte Is ( byte pos ); // get
        public byte IsIt (); // welche sind 1
        public void At ( byte pos, byte value ); // set

        public void Flip ( byte pos ); // flippen

        public byte IsItNot ();// welche sind 0

        public void RotateRight ( byte count );

        public void RotateLeft ( byte count );

        public FixedVector<byte> Where ();

        public FixedVector<byte> WhereNot ();

    }
    public interface IFastType<T> : IFastType {
       
        public T Value { get;  }

        public IFastType<T> CmpOne ();

        public IFastType<T> CmpTwo ();

        public void Mask ( T mask );

        public T CreateMask ( byte start, byte end );

        public IFastType<T> Combine ( IFastType<T> other );

        
    }

}
