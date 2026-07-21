using System;
using System.Collections.Generic;
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

        public Array<byte> Where ();

        public Array<byte> WhereNot ();

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
