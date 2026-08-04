using SystemEx.Hash;

namespace SystemEx.Numeric {
    /// \addtogroup Numeric
    /// @{
    /// <summary>
    /// Defines the structural layout of a 16‑bit floating format.
    /// Implementations specify how many bits are used for sign,
    /// exponent and mantissa, including exponent bias and bit positions.
    /// </summary>
    public interface IFloat<TSelf, TBias> : IEquatable<TSelf>, IComparable, IComparable<TSelf>, 
        IComparableEx<TSelf>, IHashable<TSelf>, IByteSerializable<TSelf>

        where TSelf : struct, IFloat<TSelf, TBias> {

        public TBias ToBase { get;  }

        /// <summary>
        /// Number of bits used for the sign field
        /// </summary>
        public TBias SignBits { get; }
        /// <summary>
        /// Number of bits used for the exponent field
        /// </summary>
        public TBias ExponentBits { get; }
        /// <summary>
        /// Number of bits used for the mantissa (fraction) 
        /// </summary>
        public TBias MantissaBits { get; }
        /// <summary>
        /// Exponent bias used by the binary16 format 
        /// </summary>
        public TBias ExponentBias { get; }
        /// <summary>
        /// Total number of bits in the representation 
        /// </summary>
        public TBias TotalBits { get; }

        public ushort HiddenBit { get; }

        /// <summary>
        /// Gets the sign bit (true = negative).
        /// </summary>
        public bool Sign { get; }
        /// <summary>
        /// Gets the exponent field (5 bits).
        /// </summary>
        public TBias Exponent { get; }
        /// <summary>
        /// Gets the mantissa (fraction) field (10 bits).
        /// </summary>
        public TBias Mantissa { get; }

        // --- Static constants ---
        static abstract TSelf Zero { get; }
        static abstract TSelf One { get; }
        static abstract TSelf NegativeOne { get; }
        static abstract TSelf NegativeZero { get; }
        static abstract TSelf PositiveInfinity { get; }
        static abstract TSelf NegativeInfinity { get; }
        static abstract TSelf NaN { get; }
        static abstract TSelf NaN2 { get; }
        static abstract TSelf Epsilon { get; }
        static abstract TSelf E { get; }
        static abstract TSelf Tau { get; }
        static abstract TSelf Pi { get; }

        // --- Static classification ---
        static abstract bool IsZero ( TSelf value );
        static abstract bool IsNegative ( TSelf value );
        static abstract bool IsNaN ( TSelf value );
        static abstract bool IsInfinity ( TSelf value );
        static abstract bool IsFinite ( TSelf value );
        static abstract bool IsSubnormal ( TSelf value );
        static abstract bool IsNormal ( TSelf value );
        static abstract bool IsInteger ( TSelf value );

        // --- Static unary operations ---
        static abstract TSelf Abs ( TSelf value );
        static abstract TSelf Negate ( TSelf value );
        static abstract TSelf Signum ( TSelf value );
        static abstract TSelf Floor ( TSelf value );
        static abstract TSelf Ceil ( TSelf value );
        static abstract TSelf Trunc ( TSelf value );

        static abstract TSelf Clamp ( TSelf x, TSelf min, TSelf max );

        // --- Static binary operations ---
        static abstract TSelf Add ( TSelf a, TSelf b );
        static abstract TSelf Mul ( TSelf a, TSelf b );
        static abstract TSelf Div ( TSelf a, TSelf b );

        static abstract TSelf Min ( TSelf a, TSelf b );
        static abstract TSelf Max ( TSelf a, TSelf b );

        // --- Static comparison ---
        static abstract bool operator < ( TSelf a, TSelf b );
        static abstract bool operator > ( TSelf a, TSelf b );
        static abstract bool operator <= ( TSelf a, TSelf b );
        static abstract bool operator >= ( TSelf a, TSelf b );
        static abstract bool operator == ( TSelf a, TSelf b );
        static abstract bool operator != ( TSelf a, TSelf b );

        
    }
    /// @}
}
