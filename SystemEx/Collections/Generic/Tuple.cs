
namespace SystemEx.Collection.Generic {


    public class Tuple : ITuple  {
        private Array<object> m_elements;

        public int Count => m_elements.Count();

        public object this[int index] {
            get => m_elements.ElementAt(index);
            set => Set(index, value);
        }

        public Tuple() {
            m_elements = new Array<object>(5);
        }
        public Tuple(int N) {
            m_elements = new Array<object>(N);
        }
        
        public Tuple(Array<object> elements) {
            m_elements = elements;
        }

        public object? Get(int index) {
            if ( index < 0 || index >= Count )
#pragma warning disable CA2201 // Keine reservierten Ausnahmetypen auslösen
                throw new IndexOutOfRangeException("index");
#pragma warning restore CA2201 // Keine reservierten Ausnahmetypen auslösen

            return m_elements.ElementAt(index);
        }

        public void Set(int index, object value) {
            if ( index < 0 || index >= m_elements.Count() ) return;

            m_elements.Insert(index, value);
        }
    }
}
