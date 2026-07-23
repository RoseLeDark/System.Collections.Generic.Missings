using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic;
using SystemEx.Algorithms.Interfaces;
using SystemEx.Collections.Generic.Interfaces;
using SystemEx.Utils;

namespace SystemEx.Collections.Generic {

    public interface Iterrator<T> {
        bool IsEnd { get; }

        long Index { get; set; }
        void Forward ();
        T Current { get; set; }
    }
    public struct ForwardIterrator<T, TCollection>: Iterrator<T>
        where TCollection : IVector<T> {

        private  TCollection m_collection;
        private long m_index;

        public bool IsEnd => m_index >= m_collection.Count;
        public bool IsNext => !IsEnd;

        public long Index { 
            get => m_index; 
            set => m_index = value; 
        }

        public T Current {
            get => m_collection.ElementAt(m_index);
            set => m_collection.Replace(m_index, Current);
        }

        public ForwardIterrator (  TCollection Collection, long index ) {
            m_collection =  Collection;
            m_index = index;
        }
        public void Forward () {
            if(!IsEnd )
                m_index++;
        }

        public void Forward ( long i ) {
            while( i != 0) {
                if ( IsEnd ) break;

                m_index++;
                i--;
            }
        }
        public ForwardIterrator<T, TCollection> Clone () {
            return new ForwardIterrator<T, TCollection>(m_collection, m_index);
        }
    }

    public  struct BidirectionalIterator<T, TCollection> : Iterrator<T>
       where TCollection : IVector<T> {

        private  TCollection m_collection;
        private long m_index;

        public bool IsEnd => m_index >= m_collection.Count;
        public bool IsNext => !IsEnd;

        public long Index {
            get => m_index;
            set => m_index = value;
        }
        public T Current { 
            get => m_collection.ElementAt(m_index); 
            set => m_collection.Replace(m_index, Current); 
        }

        public bool IsBegin => throw new NotImplementedException();

        public BidirectionalIterator (  TCollection Collection, long index ) {
            m_collection =  Collection;
            m_index = index;
        }
        public void Forward () {
            if ( !IsEnd )
                m_index++;
        }

        public void Forward ( long i ) {
            while ( i != 0 ) {
                if ( IsEnd ) break;

                m_index++;
                i--;
            }
        }

        public void Back () {
            if ( m_index != 0 )
                m_index--;
        }

        public BidirectionalIterator<T, TCollection> Clone () {
            return new BidirectionalIterator<T, TCollection>(m_collection, m_index);
        }
    }



    public  struct RandomAccessIterator<T, TCollection> : Iterrator<T>
       where TCollection : IVector<T> {

        private  TCollection m_collection;
        private long m_index;

        public bool IsEnd => m_index >= m_collection.Count;
        public bool IsNext => !IsEnd;

        public T Current {
            get => m_collection.ElementAt(m_index);
        }

        public long Index {
            get => m_index;
            set => m_index = value;
        }

        public bool IsBegin => throw new NotImplementedException();

        public RandomAccessIterator (  TCollection Collection, long index ) {
            m_collection =  Collection;
            m_index = index;
        }
        public void Forward () {
            if ( !IsEnd )
                m_index++;
        }

        public void Forward ( long i ) {
            while ( i != 0 ) {
                if ( IsEnd ) break;

                m_index++;
                i--;
            }
        }

        public void Back () {
            if ( m_index != 0 )
                m_index--;
        }

        public RandomAccessIterator<T, TCollection> Advance ( long n ) {
            while ( n > 0 ) {
                --n;
                Forward();
            }
            while ( n < 0 ) {
                ++n;
                Back();
            }
            return this;
        }

        public RandomAccessIterator<T, TCollection> Clone () {
            return new RandomAccessIterator<T, TCollection>(m_collection, m_index);
        }
    }

    public static class IteratorUtils {
        public static int Distance<T> ( Iterrator<T> first, Iterrator<T> last ) {
            int count = 0;
            var _index = first.Index;

            while ( !first.Equals(last) ) {
                first.Forward();
                count++;
            }
            first.Index = _index;

            return count;
        }

        
        /// <summary>
        /// Searches for the first occurrence of a value in a forward iterator range.
        /// </summary>
        public static ForwardIterrator<T, TCollection>? Find<T, TCollection> ( ForwardIterrator<T, TCollection> xfirst,
            ForwardIterrator<T, TCollection> end, T value, ISimpleCompare<T> cmp ) where TCollection : IVector<T> {
            long indx = xfirst.Index;
            ForwardIterrator<T, TCollection>? _end = null;

            while ( true ) {
                if ( xfirst.IsEnd ) break;

                if(cmp.Compare(xfirst.Current, value) ) {
                    _end = xfirst;
                }

                xfirst.Forward();

            }
            xfirst.Index = indx;
           
            return end;
        }
        /// <summary>
        /// Finds the first iterator position where <paramref name="value"/> could be inserted
        /// without violating ordering (lower bound).
        /// </summary>
        public static IRandomAccessIterator<T> LowerBound<T> ( IRandomAccessIterator<T> first, IRandomAccessIterator<T> last, T value, CompFunc<T> cmp ) where TCollection : IVector<T> {
            int count = Distance(first.Clone(), last.Clone());
            IRandomAccessIterator<T> it = (IRandomAccessIterator<T>)first.Clone();

            while ( count > 0 ) {
                int step = count / 2;
                IRandomAccessIterator<T> mid = (IRandomAccessIterator<T>)it.Clone();
                mid.Advance(step);

                if ( cmp(mid.Current, value) == CompareResult.AIsSmallerB ) {
                    it = mid;
                    it.Forward();
                    count -= step + 1;
                } else {
                    count = step;
                }
            }

            return it;
        }
        /// <summary>
        /// Finds the first iterator position where <paramref name="value"/> would appear
        /// after all equivalent elements (upper bound).
        /// </summary>
        public static IRandomAccessIterator<T> UpperBound<T> ( IRandomAccessIterator<T> first, IRandomAccessIterator<T> last, T value, CompFunc<T> cmp ) where TCollection : IVector<T> {
            int count = Distance(first.Clone(), last.Clone());
            var it = first.Clone();

            while ( count > 0 ) {
                int step = count / 2;
                IRandomAccessIterator<T> mid = (IRandomAccessIterator<T>)it.Clone();
                mid.Advance(step);

                if ( cmp(value, mid.Current) != CompareResult.AIsLargerB ) {
                    count = step;
                } else {
                    it = mid;
                    it.Forward();
                    count -= step + 1;
                }
            }

            return (IRandomAccessIterator<T>)it;
        }
        /// <summary>
        /// Reverses the elements in the iterator range [first, last).
        /// </summary>
        public static void Reverse<T> ( IBidirectionalIterator<T> first, IBidirectionalIterator<T> last ) where TCollection : IVector<T> {
            last.Back();

            while ( !first.Equals(last) && !first.IsEnd && !last.IsBegin ) {

                T f = first.Current; T l = last.Current;
                Algorithm.Swap(ref f, ref l);
                first.Current = f; last.Current = l;

                first.Forward();
                last.Back();
            }
        }
        /// <summary>
        /// Rotates the iterator range so that <paramref name="middle"/> becomes the new beginning.
        /// </summary>
        public static void Rotate<T> ( IBidirectionalIterator<T> first, IBidirectionalIterator<T> middle, IBidirectionalIterator<T> last ) {
            Reverse(first, middle);
            Reverse(middle, last);
            Reverse(first, last);
        }
        /// <summary>
        /// Applies an action to each element in the iterator range [first, last).
        /// </summary>
        public static void ForEach<T> ( IForwardIterator<T> first, IForwardIterator<T> last, Action<T> action ) {
            while ( !first.Equals(last) ) {
                action(first.Current);
                first.Forward();
            }
        }
    }
    }
