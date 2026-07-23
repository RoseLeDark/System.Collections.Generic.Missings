using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SystemEx.Collections.Generic;
using SystemEx.Utils;

namespace SystemEx {

    public interface IDelegate<T> {

        void Invoke ( T arg );

        void AddFunc ( Action<IDelegate<T>, T> func );

        void RemoveFunc ( Action<IDelegate<T>, T> func );

        void Clear ();
    }

    public struct Delegate<T> : IDelegate<T> , IComparable<Delegate<T>>, IEquatable< Delegate<T> >, IEnumerable<Action<IDelegate<T>, T>> {

        private List< Action<IDelegate<T>, T >?> m_functions;
   
        public Delegate ( Action<IDelegate<T>, T>? func = null) {
            m_functions = new List<Action<IDelegate<T>, T>?>();
            if(func != null) m_functions.Add(func);
        }
        public void Invoke ( T arg ) {

            foreach ( var item in m_functions ) {
               if( item != null) item?.Invoke(this, arg);
            }
  
        }

        public void AddFunc( Action<IDelegate<T>, T> func ) {
            m_functions.Add(func);
        }

        public void RemoveFunc ( Action<IDelegate<T>, T> func ) {
            m_functions.Remove(func);
        }

        public void Clear () {
            m_functions.Clear();
        }

        public int CompareTo ( Delegate<T> other ) {
            if ( m_functions == null ) return (int)CompareResult.AIsSmallerB;
            if ( other.m_functions == null ) return (int)CompareResult.AIsLargerB;

            int A = m_functions.Count;
            int B = other.m_functions.Count;

            if ( A > B ) return (int)CompareResult.AIsLargerB;
            if ( A < B ) return (int)CompareResult.AIsSmallerB;

            return 0;
        }

        public IEnumerator<Action<IDelegate<T>, T>> GetEnumerator () {
            return m_functions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator () {
            return GetEnumerator();
        }

       
        public bool Equals ( Delegate<T> other ) {
            return (other.m_functions == m_functions) ;
        }
    }

}
