namespace SystemEx.Numeric {

    /// <summary>
    /// Defines the structural layout of a 16‑bit floating format.
    /// Implementations specify how many bits are used for sign,
    /// exponent and mantissa, including exponent bias and bit positions.
    /// </summary>
    public interface IHalf<T> {

        public ushort SignBits;
        public ushort ExponentBits;
        public ushort MantissaBits;
        public ushort ExponentBias;
        public ushort TotalBits;

        public bool Sign;
        public ushort Exponent;

        public ushort Mantissa;

        public ushort AsUShort ();

        public byte[] ToBytes ( Endian endian );

    }
}
