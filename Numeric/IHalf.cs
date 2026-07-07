namespace SystemEx.Numeric {
    /// \addtogroup Numeric
    /// @{
    /// <summary>
    /// Defines the structural layout of a 16‑bit floating format.
    /// Implementations specify how many bits are used for sign,
    /// exponent and mantissa, including exponent bias and bit positions.
    /// </summary>
    public interface IHalf<T> {

        public ushort SignBits { get;  }
        public ushort ExponentBits { get;  }
        public ushort MantissaBits { get;  }
        public ushort ExponentBias { get; }
        public ushort TotalBits { get; }

        public bool Sign { get; }
        public ushort Exponent { get; }

        public ushort Mantissa { get; }

        public ushort AsUShort ();

        public byte[] ToBytes ( Endian endian );

    }
    /// @}
}
