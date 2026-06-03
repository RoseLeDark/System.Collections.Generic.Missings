
namespace System.Collections.Generic.Missings {
    public class ListIterator<T> : IRandomAccessIterator<T>, IEquatable, IForeachIterator<T>  {
        private  List<T> m_list;
        private int m_index;

        public ListIterator(List<T> list, int index) {
            m_list = list;
            m_index = index;
        }

        public T Current => m_list[m_index];
        public bool IsEnd => m_index >= m_list.Count;

        public int Index => m_index;

        public bool IsBegin => m_index == 0;

        object IEnumerator.Current => Current!;

        public void Forward() {
            if ( !IsEnd ) m_index++;
        }
        public void Back() {
            if ( m_index > 0 )
                m_index--;
        }
        public IRandomAccessIterator<T> Advance(int offset) { m_index += offset; return this; }

        public bool Equals(ListIterator<T>? other) {
            if ( other == null ) return false;

            // For Multitask in C# not STL Like
            return m_list.SequenceEqual( other.m_list) && m_index == other.m_index;
        }

        public override bool Equals(object? obj) {
            if ( obj is ListIterator<T> ) {
                return Equals((ListIterator<T>)obj);
            }
            return false;
        }

        public override int GetHashCode() {
            unchecked {
                int h = m_list.GetHashCode();
                h = (h * 397) ^ m_index;
                return h;
            }
        }

        public ITerator<T> Clone() {
            return new ListIterator<T>(m_list.ToList(), m_index);
        }

        public IEnumerator<T> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;

        public bool MoveNext() {
            if ( !IsEnd ) { m_index++; return true; }
            return false;
        }

        public void Reset() { }

        public void Dispose() {
            GC.SuppressFinalize(this);
        }

        public static bool operator ==(ListIterator<T>? a, ListIterator<T>? b) {
            if ( ReferenceEquals(a, b) ) return true;
            if ( a is null || b is null ) return false;
            return a.Equals(b);
        }

        public static bool operator !=(ListIterator<T>? a, ListIterator<T>? b) {
            return !(a == b);
        }
    }


    public static class ListIteratorExtensions {
        public static IRandomAccessIterator<T> First<T>(this List<T> list)
            => new ListIterator<T>(list, 0);

        public static IRandomAccessIterator<T> At<T>(this List<T> list, int index)
            => new ListIterator<T>(list, index);

        public static IRandomAccessIterator<T> End<T>(this List<T> list)
            => new ListIterator<T>(list, list.Count);
    }


}
