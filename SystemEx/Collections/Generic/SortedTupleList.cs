using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using SystemEx.Collection.Generic.Interfaces;
using SystemEx.Utils;

namespace SystemEx.Collection.Generic {
    public class SortedTupleList : TupleList {
        private SortTupleFunc m_sort;

        private ICompared<ITuple>? m_comparer;

        public ICompared<ITuple>? Comparer {
            get => m_comparer;
            set {
                m_comparer = value;
                if ( AutoSort ) Sort();
            }
        }

        public SortTupleFunc Sorter {
            get => m_sort;
            set {
                m_sort = value;
                if ( AutoSort ) Sort();
            }
        }


        public bool AutoSort { get; set; }

        public SortedTupleList(SortTupleFunc sort) {  m_sort = sort; AutoSort = true;  }
        public SortedTupleList(int size, SortTupleFunc sort) : base(size) { m_sort = sort; AutoSort = true; }
        public SortedTupleList(IEnumerable<ITuple> collection, SortTupleFunc sort) : base(collection) { m_sort = sort; AutoSort = true; }

        public override void Add(ITuple tuple) {
            base.Add(tuple);
            if ( AutoSort ) Sort();
        }
        public override void AddRange(IEnumerable<ITuple> items) {
            base.AddRange(items);
            if ( AutoSort ) Sort();
        }
        public override bool Remove(ITuple item) {
            bool _ret = base.Remove(item);
            if ( AutoSort && _ret) Sort();
            return _ret;
        }
        public override void Insert(int index, ITuple item) {
            base.Insert(index, item);
            if ( AutoSort ) Sort();
        }

        public override void RemoveAt(int index) {
            base.RemoveAt(index);
            if ( AutoSort ) Sort();
        }
        public void Sort() {
            for ( int i = 0; i < base.Count - 1; i++ ) {
                for ( int j = i + 1; j < base.Count; j++ ) {

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
        public TupleList ToUnorderedList() {
            TupleList list = [.. m_elements];
            return list;
        }
    }
}
