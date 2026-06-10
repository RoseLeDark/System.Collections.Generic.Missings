using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using SystemEx.Collection.Generic.Interfaces;
using SystemEx.Utils;

namespace SystemEx.Collection.Generic {
    public class SortedArray<T> : Array<T>, ISortedArray<T> {
        private SortObjectFunc<T> m_sort;
        private ICompared<T>? m_comparer;

        public ICompared<T>? Comparer {
            get => m_comparer;
            set {
                m_comparer = value;
                if ( AutoSort ) Sort();
            }
        }

        public SortObjectFunc<T> SortFunctions {
            get => m_sort!;
            set {
                m_sort = value;
                if ( AutoSort ) Sort();
            }
        }
        public bool AutoSort { get; set; }

        public SortedArray(int size, SortObjectFunc<T> sorter, int growSize = 16) 
            : base(size, growSize) { m_sort = sorter; }

        public override bool Add(T entry) {
            bool _ret = base.Add(entry);
            if ( _ret && AutoSort ) Sort();
            return _ret;
        }
        public override int Insert(int pos, T item) {
            int _ret = base.Insert(pos, item);
            if ( AutoSort ) Sort();
            return _ret;
        }
        public override int InsertRange(int pos, IEnumerable<T> items) {
            int _ret = base.InsertRange(pos, items);
            if ( AutoSort ) Sort();
            return _ret;
        }
        public override bool Remove() {
            bool _ret = base.Remove();
            if ( _ret && AutoSort ) Sort();
            return _ret;
        }

        public void Sort() {
            for ( int i = 0; i < Size - 1; i++ ) {
                for ( int j = i + 1; j < Size; j++ ) {

                    CompareResult cmp = m_comparer != null
                    ? m_comparer.Compare(m_elements[i], m_elements[j])
                    : m_sort!(m_elements[i], m_elements[j]);


                    if ( cmp == CompareResult.AISLargerB ) {
                        Swap(i, j);
                    }
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Swap(int i, int j) {
            var tmp = m_elements[i];
            m_elements[i] = m_elements[j];
            m_elements[j] = tmp;
        }


        public IArray<T> ToUnorderedArray() {
            return new Array<T> ( this.ToArray());
        }
    }
}
