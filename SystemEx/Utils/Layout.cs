using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace SystemEx.Utils {

    [Serializable]
    public sealed class MissingStructLayoutSequentialException : Exception {
        public Type StructType { get; }

        public MissingStructLayoutSequentialException(Type type)
            : base($"{type.Name} must use [StructLayout(LayoutKind.Sequential)]") {
            StructType = type;
        }
    }

    [Serializable]
    public sealed class SizeMismatchException : Exception {
        public Type StructType { get; }
        public long ManagedSize { get; }
        public long ExpectedSize { get; }

        public SizeMismatchException(Type type, long managed, long expected)
            : base($"{type.Name} size mismatch: managed={managed}, expected={expected}") {
            StructType = type;
            ManagedSize = managed;
            ExpectedSize = expected;
        }
    }


    public static class Layout {
        public static unsafe  bool Check<T>(uint expectedUnmanagedSize) where T : unmanaged {
            Type t = typeof(T);
            long _sizeof = sizeof(T);
            int _marshalSize = Marshal.SizeOf<T>();

            // 1. StructLayout muss Sequential sein
            var attr = t.StructLayoutAttribute;
            if ( attr == null || attr.Value != LayoutKind.Sequential )
                throw new MissingStructLayoutSequentialException(t);

            

            if ( _marshalSize != _sizeof )
                throw new SerializationException(
                    $"{t.Name} size mismatch: MarshalSize={_marshalSize}, sizeof={_sizeof}");

            if ( expectedUnmanagedSize != 0 ) {
                if ( _sizeof != expectedUnmanagedSize )
                    throw new SizeMismatchException(t, _sizeof, expectedUnmanagedSize);
            }
            return true;
        }
    }
}
