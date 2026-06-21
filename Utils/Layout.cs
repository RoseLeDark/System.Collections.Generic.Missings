using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace SystemEx.Utils {

    /// <summary>
    /// Exception thrown when a struct used for unmanaged interop does not specify
    /// <see cref="StructLayoutAttribute"/> with <see cref="LayoutKind.Sequential"/>.  
    /// Required for deterministic field ordering in native interop, binary
    /// serialization, and memory‑mapped structures.
    /// </summary>
    [Serializable]
    public sealed class MissingStructLayoutSequentialException : Exception {
        /// <summary>
        /// Gets the struct type that caused the exception.
        /// </summary>
        public Type StructType { get; }


#pragma warning disable CS1574 // XML-Kommentar weist ein cref-Attribut auf, das nicht aufgelöst werden konnte.
        /// <summary>
        /// Creates a new exception indicating that the specified struct type
        /// must be declared with <see cref="System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential)"/>.
        /// </summary>
        public MissingStructLayoutSequentialException(Type type)
#pragma warning restore CS1574 // XML-Kommentar weist ein cref-Attribut auf, das nicht aufgelöst werden konnte.
            : base($"{type.Name} must use [StructLayout(LayoutKind.Sequential)]") {
            StructType = type;
        }
    }


    /// <summary>
    /// Exception thrown when the managed size of a struct does not match the
    /// expected unmanaged size.  
    /// Used to validate binary compatibility between C# structs and native
    /// memory layouts.
    /// </summary>
    [Serializable]
    public sealed class SizeMismatchException : Exception {
        /// <summary>
        /// Gets the struct type that failed the size validation.
        /// </summary>
        public Type StructType { get; }

        /// <summary>
        /// Gets the size of the struct as computed by <c>sizeof(T)</c>.
        /// </summary>
        public long ManagedSize { get; }

        /// <summary>
        /// Gets the expected unmanaged size provided by the caller.
        /// </summary>
        public long ExpectedSize { get; }

        /// <summary>
        /// Creates a new exception describing the size mismatch.
        /// </summary>
        public SizeMismatchException(Type type, long managed, long expected)
            : base($"{type.Name} size mismatch: managed={managed}, expected={expected}") {
            StructType = type;
            ManagedSize = managed;
            ExpectedSize = expected;
        }
    }



    /// <summary>
    /// Provides validation utilities for unmanaged structs used in native interop,
    /// binary serialization, GPU buffers, and memory‑mapped structures.  
    /// Ensures that a struct:
    /// <list type="bullet">
    /// <item><description>uses <see cref="LayoutKind.Sequential"/></description></item>
    /// <item><description>has identical <c>sizeof(T)</c> and <see cref="System.Runtime.InteropServices.Marshal.SizeOf{T}(T)"/></description></item>
    /// <item><description>matches an optional expected unmanaged size</description></item>
    /// </list>
    /// </summary>
    public static class Layout {
        /// <summary>
        /// Validates the layout and size of an unmanaged struct.  
        /// Throws exceptions when the struct is not binary‑compatible with native code.
        /// </summary>
        /// <typeparam name="T">The unmanaged struct type to validate.</typeparam>
        /// <param name="expectedUnmanagedSize">
        /// Optional expected size in bytes.  
        /// If zero, the size check is skipped.
        /// </param>
        /// <returns><c>true</c> if the struct passes all validation checks.</returns>
        /// <exception cref="MissingStructLayoutSequentialException">
        /// Thrown when the struct does not use <see cref="LayoutKind.Sequential"/>.
        /// </exception>
        /// <exception cref="SerializationException">
        /// Thrown when <c>sizeof(T)</c> differs from <see cref="System.Runtime.InteropServices.Marshal.SizeOf{T}(T)"/>.
        /// </exception>
        /// <exception cref="SizeMismatchException">
        /// Thrown when <c>sizeof(T)</c> does not match <paramref name="expectedUnmanagedSize"/>.
        /// </exception>
        public static unsafe bool Check<T>(uint expectedUnmanagedSize) where T : unmanaged {
            Type t = typeof(T);
            long _sizeof = sizeof(T);
            int _marshalSize = Marshal.SizeOf<T>();

            // StructLayout must be Sequential
            var attr = t.StructLayoutAttribute;
            if ( attr == null || attr.Value != LayoutKind.Sequential )
                throw new MissingStructLayoutSequentialException(t);

            // sizeof(T) must match Marshal.SizeOf<T>()
            if ( _marshalSize != _sizeof )
                throw new SerializationException(
                    $"{t.Name} size mismatch: MarshalSize={_marshalSize}, sizeof={_sizeof}");

            // Optional unmanaged size check
            if ( expectedUnmanagedSize != 0 ) {
                if ( _sizeof != expectedUnmanagedSize )
                    throw new SizeMismatchException(t, _sizeof, expectedUnmanagedSize);
            }

            return true;
        }
    }

}
