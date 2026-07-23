
using System.Security.Cryptography;
using SystemEx.Algorithms.Interfaces;
using SystemEx.Algorythmen;
using SystemEx.Collections.Generic;
using SystemEx.Utils;

namespace SystemEx {
    public struct DelegateBus<T> : IDelegate<T> {
        private Vector< IDelegate<T> > m_listeners;


        public DelegateBus () {
            m_listeners = new Vector<IDelegate<T>>();
        }

        public void AddFunc ( IDelegate<T>  func ) {
            m_listeners.PushBack(func);
        }

        public void AddFunc ( Action<IDelegate<T>, T> func ) {
            return;
        }

        public void Clear () {
            m_listeners.Clear();
        }

        public void Invoke ( T arg ) {

            for ( int i = 0 ; i < m_listeners.Count ; i++ ) {
                m_listeners.ElementAt(i).Invoke( arg);
            }
        }

        public void Invoke ( long start, long end, T Arg ) {
            if ( end > m_listeners.Count ) end = m_listeners.Count;

            for ( long i = start ; i <= end ; i++ ) {
                m_listeners.ElementAt(i).Invoke(Arg);
            }
        }

        public long Invoke ( Func<IDelegate<T>, long, bool> func, T Arg ) {
            long _ret = 0;

            for ( long i = 0 ; i < m_listeners.Count ; i++ ) {
                var _t = m_listeners.ElementAt(i);


                if ( ( func(_t, i) ) ) {
                    _t.Invoke(Arg);
                    _ret++;
                }
            }
            return _ret;
        }

        public long Invoke ( ISimpleCompare<long> cmp, long b, T Arg ) {
            long _ret = 0;

            for ( long i = 0 ; i < m_listeners.Count ; i++ ) {
                var _t = m_listeners.ElementAt(i);

                if(cmp.Compare(i, b)) {
                    _t.Invoke(Arg);
                    _ret++;
                }
            }

            return _ret;
        }

        public void RemoveLast () {
            m_listeners.Erase();
        }

        public void RemoveFunc ( Action<IDelegate<T>, T> func ) {
          
        }
    }
}
