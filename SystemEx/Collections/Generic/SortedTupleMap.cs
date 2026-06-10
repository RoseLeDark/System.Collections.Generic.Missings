using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using SystemEx.Collection.Generic.Interfaces;
using SystemEx.Utils;

namespace SystemEx.Collection.Generic {
    [Serializable]
#pragma warning disable CA1710 // Bezeichner müssen ein korrektes Suffix aufweisen
    public class SortedTupleMap : TupleMap, ISortedTupleMap
#pragma warning restore CA1710 // Bezeichner müssen ein korrektes Suffix aufweisen
    {

        private SortTupleFunc m_sort;

        private ICompared<ITuple>? m_comparer;

        public ICompared<ITuple>? Comparer {
            get => m_comparer;
            set {
                m_comparer = value;
                if ( AutoSort ) Sort();
            }
        }

        public SortTupleFunc SortFunctions {
            get => m_sort;
            set {
                m_sort = value;
                if ( AutoSort ) Sort();
            }
        }

        public bool AutoSort { get; set; }

        public SortedTupleMap(SortTupleFunc sort) : base() {
            m_sort = sort;
            AutoSort = true;
        }

        public SortedTupleMap(ITupleMap source, SortTupleFunc sort) : base() {
            m_sort = sort;
            m_elements = [.. source.ToArray()];
            Sort();
        }

        public override void Add(ITuple item) {
            base.Add(item);
            if ( AutoSort ) Sort();
        }

        public override bool Insert(int pos, ITuple item) {
            m_elements.Insert(pos, item);
            Sort();

            return true;
        }
        public override bool InsertRange(int pos, IEnumerable<ITuple> items) {
            m_elements.InsertRange(pos, items);
            if ( AutoSort ) Sort();

            return true;
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
        public ITupleMap ToUnorderedMap() {
            TupleMap map = [.. m_elements];
            return map;
        }
    }
}
