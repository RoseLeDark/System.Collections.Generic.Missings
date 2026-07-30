using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SystemEx.Collections.Generic;
using SystemEx.Utils;

namespace SystemEx {

    public interface IDelegate<T> {

        void Invoke ( T arg );

        void Subscribe ( Action<IDelegate<T>, T> func );

        void UnSubscribe ( Action<IDelegate<T>, T> func );

        void Clear ();
    }

    public struct Delegate<T> : IDelegate<T> , IComparable<Delegate<T>>, IEquatable< Delegate<T> >, IEnumerable<Action<IDelegate<T>, T>> {

        private Vector< Action<IDelegate<T>, T > > m_functions;
   
        public Delegate ( Action<IDelegate<T>, T> func ) {
            m_functions = new Vector< Action<IDelegate<T>, T>   >();
            if(func != null) m_functions.PushBack(func);
        }
        public void Invoke ( T arg ) {

            foreach ( var item in m_functions ) {
               if( item != null) item?.Invoke(this, arg);
            }
  
        }

        public void Subscribe ( Action<IDelegate<T>, T> func ) {
            m_functions.PushBack(func);
        }

        public void UnSubscribe ( Action<IDelegate<T>, T> func ) {
            m_functions.Erase(func);
        }

        public void Clear () {
            m_functions.Clear();
        }

        public int CompareTo ( Delegate<T> other ) {
            long _mA = 0, _mB = 0;

            long A = m_functions.Count;
            long B = other.m_functions.Count;

            long min = System.Math.Min(A, B);


            for ( long i = 0 ; i < min ; i++ ) {
                var iten = m_functions.ElementAt(i);
                var oi = other.m_functions.ElementAt(i);

                int cmp = Comparer< Action<IDelegate<T>, T>>.Default.Compare(iten.Value, oi.Value);

                if ( cmp < 0 ) _mB++;
                else if(cmp > 0) _mA ++;
            }

            if (_mA > _mB) return (int)CompareResult.Greater;
            if ( _mA < _mB ) return (int)CompareResult.Less;
            
            return 0;
        }

        public IEnumerator<Action<IDelegate<T>, T>> GetEnumerator () {
            return m_functions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator () {
            return GetEnumerator();
        }

       
        public bool Equals ( Delegate<T> other ) {
            return m_functions == other.m_functions ;
        }

        public static bool operator == (Delegate<T> a, Delegate<T> b) {
            return a.Equals(b);
        }
        public static bool operator != ( Delegate<T> a, Delegate<T> b ) {
            return !a.Equals(b);
        }
    }

}
