using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SystemEx.Utils;

namespace SystemEx.Collection.Generic {

    public interface ITerator {
        public void Forward();
    }

    public interface ITerator<T> : ITerator {
        public ITerator<T> Clone();
    }

    public interface IForwardIterator<T> : ITerator<T> {
        public T Current { get; }
        public bool IsEnd { get; }
    }


    public interface IBidirectionalIterator<T> : ITerator<T> {

        public T Current { get; internal set; }
        public bool IsEnd { get; }
        public bool IsBegin { get; }

        public void Back();
    }

    public interface IRandomAccessIterator<T> : ITerator<T> {
        public IRandomAccessIterator<T> Advance(int offset);
        public T Current { get; }
        public bool IsEnd { get; }
        public bool IsBegin { get; }
        public void Back();

    }

    public interface IPairForwardIterator<T, TU> : IForwardIterator<Pair<T, TU>> {
        public T? First { get; }
        public TU? Second { get; }
    }

    public interface IForeachIterator<T> : IEnumerable<T>, IEnumerator<T> {

    }


    public static class Iterator {
        public static int Distance<T>(ITerator<T> first, ITerator<T> last) {
            int count = 0;
            while ( !first.Equals(last) ) {
                first.Forward();
                count++;
            }
            return count;
        }

        public static IForwardIterator<T> Find<T>( IForwardIterator<T> it, IForwardIterator<T> end, T value, CompFunc<T> cmp) {
            while ( !it.Equals(end) ) {
                if ( cmp(it.Current, value) == CompareResult.Equal )
                    return it;

                it.Forward();
            }
            return end;
        }
        public static IRandomAccessIterator<T> LowerBound<T>( IRandomAccessIterator<T> first, IRandomAccessIterator<T> last, T value, CompFunc<T> cmp) {
            int count = Distance(first.Clone(), last.Clone());
            IRandomAccessIterator<T> it = (IRandomAccessIterator<T>)first.Clone();

            while ( count > 0 ) {
                int step = count / 2;
                IRandomAccessIterator<T> mid = (IRandomAccessIterator<T>)it.Clone();
                mid.Advance(step);

                if ( cmp(mid.Current, value) == CompareResult.AisSmallerB ) {
                    it = mid;
                    it.Forward();
                    count -= step + 1;
                } else {
                    count = step;
                }
            }

            return it;
        }

        public static IRandomAccessIterator<T> UpperBound<T>( IRandomAccessIterator<T> first, IRandomAccessIterator<T> last, T value, CompFunc<T> cmp) {
            int count = Distance(first.Clone(), last.Clone());
            var it = first.Clone();

            while ( count > 0 ) {
                int step = count / 2;
                IRandomAccessIterator<T> mid = (IRandomAccessIterator<T>)it.Clone();
                mid.Advance(step);

                if ( cmp(value, mid.Current) != CompareResult.AISLargerB ) {
                    count = step;
                } else {
                    it = mid;
                    it.Forward();
                    count -= step + 1;
                }
            }

            return (IRandomAccessIterator <T> )it;
        }
        public static void Reverse<T>(IBidirectionalIterator<T> first, IBidirectionalIterator<T> last) {
            last.Back(); 

            while ( !first.Equals(last) && !first.IsEnd && !last.IsBegin ) {

                T f = first.Current; T l = last.Current;
                Algorithm.Swap(ref f, ref l);
                first.Current = f;  last.Current = l;

                first.Forward();
                last.Back();
            }
        }
        public static void Rotate<T>(  IBidirectionalIterator<T> first, IBidirectionalIterator<T> middle, IBidirectionalIterator<T> last) {
            Reverse(first, middle);
            Reverse(middle, last);
            Reverse(first, last);
        }

        public static void ForEach<T>(IForwardIterator<T> first, IForwardIterator<T> last, Action<T> action) {
            while ( !first.Equals(last) ) {
                action(first.Current);
                first.Forward();
            }
        }

    }
}
