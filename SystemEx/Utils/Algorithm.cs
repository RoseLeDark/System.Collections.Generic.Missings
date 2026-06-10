using SystemEx;
using SystemEx.Collection.Generic;

namespace SystemEx.Utils {
    public enum CompareResult {
        AISLargerB = 1,
        AisSmallerB = 2,
        Equal = 3, 
    }
    public delegate CompareResult CompFunc<T>(T? a, T? b);

    public static class Algorithm {
        public static ulong Rand(ulong min, ulong max, Endian endian)
                => RandUtils.RandULong(min, max, endian);

        public static long Rand(long min, long max, Endian endian)
                => RandUtils.RandLong(min, max, endian);

        public static void Swap<T>(ref T x, ref T y) {
            T temp = x;
            x = y;
            y = temp;
        }
        public static void Fill<T>(this T[] items, T value) {
            for ( int i = 0; i < items.Length; i++ ) {
                items[i] = value;
            }
        }
        public static void FillN<T>(this T[] items, uint start, uint count, T value) {
            uint end = start + count;
            if ( end > items.Length ) throw new ArgumentException("Index out of range");

            for ( uint i = start; i < start+count; i++ ) {
                items[i] = value;
            }
        }

        public static T? MaxElement<T>(this T[] items, CompFunc<T> cmp) {
            T? largest = items[0];

            foreach ( T item in items ) {
                if(cmp(largest, item) == CompareResult.AISLargerB) largest = item; 
            }
            return largest;
        }

        public static T? MinElement<T>(this T[] items, CompFunc<T> cmp) {
            T? smallest = items[0];

            foreach ( T item in items ) {
                if ( cmp(smallest, item) == CompareResult.AisSmallerB ) smallest = item;
            }
            return smallest;
        }
        public static Pair<T, T> MinMaxElement<T>(this T[] items, CompFunc<T> cmp) {
            T? min = MinElement(items, cmp);
            T? max = MaxElement(items, cmp);

            return new Pair<T, T>(min!, max!);
        }
        public static Pair<T, T> MinMax<T>(T a,T b, CompFunc<T> cmp ) {
            return cmp(a, b) == CompareResult.AisSmallerB ? new Pair<T, T>(b, a) : new Pair<T, T>(a, b);
        }

        public static T Min<T>(T a, T b, CompFunc<T> cmp) => cmp(a, b) == CompareResult.AisSmallerB ? a : b;

        public static T Max<T>(T a, T b, CompFunc<T> cmp) => cmp(a, b) == CompareResult.AISLargerB ? a : b;

        public static T Clamp<T>(T value, T min, T max, CompFunc<T> cmp) {
            if ( cmp(value, min) == CompareResult.AisSmallerB ) return min;
            if ( cmp(value, max) == CompareResult.AISLargerB ) return max;
            return value;
        }

        public static void Copy<T>(T[] src, uint srcIndex, T[] dst, uint dstIndex, uint count) {
            if ( srcIndex + count > src.Length ) throw new ArgumentException("src out of range");
            if ( dstIndex + count > dst.Length ) throw new ArgumentException("dst out of range");

            for ( uint i = 0; i < count; i++ )
                dst[dstIndex + i] = src[srcIndex + i];
        }

        public static void Move<T>(T[] src, uint srcIndex, T[] dst, uint dstIndex, uint count) {
            if ( srcIndex + count > src.Length ) throw new ArgumentException("src out of range");
            if ( dstIndex + count > dst.Length ) throw new ArgumentException("dst out of range");

            if ( src == dst && dstIndex > srcIndex ) {
                // rückwärts kopieren
                for ( uint i = count; i > 0; i-- )
                    dst[dstIndex + i - 1] = src[srcIndex + i - 1];
            } else {
                // vorwärts kopieren
                for ( uint i = 0; i < count; i++ )
                    dst[dstIndex + i] = src[srcIndex + i];
            }
        }

        public static void Reverse<T>(T[] items) {
            uint i = 0;
            uint j = (uint)items.Length - 1;

            while ( i < j ) {
                Swap(ref items[i], ref items[j]);
                i++;
                j--;
            }
        }

        public static void Rotate<T>(T[] items, uint middle) {
            uint n = (uint)items.Length;
            if ( middle >= n ) return;

            ReverseRange(items, 0, middle - 1);
            ReverseRange(items, middle, n - 1);
            ReverseRange(items, 0, n - 1);
        }

        private static void ReverseRange<T>(T[] items, uint start, uint end) {
            while ( start < end ) {
                Swap(ref items[start], ref items[end]);
                start++;
                end--;
            }
        }

        public static bool Equal<T>(T[] a, T[] b, CompFunc<T> cmp) {
            if ( a.Length != b.Length ) return false;

            for ( int i = 0; i < a.Length; i++ )
                if ( cmp(a[i], b[i]) != CompareResult.Equal )
                    return false;

            return true;
        }

        public static bool LexicographicalCompare<T>(T[] a, T[] b, CompFunc<T> cmp) {
            int n = Math.Min(a.Length, b.Length);

            for ( int i = 0; i < n; i++ ) {
                var r = cmp(a[i], b[i]);
                if ( r == CompareResult.AisSmallerB ) return true;
                if ( r == CompareResult.AISLargerB ) return false;
            }

            return a.Length < b.Length;
        }



    }
}
