using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx {
    public struct IFDelegate<T> : IDelegate<T> {
        private readonly Func<T, bool> m_condition;
        private readonly IDelegate<T> m_then;
        private readonly IDelegate<T> m_else;

        public bool AddToThen { get; set; }
        public IFDelegate ( Func<T, bool> condition,
                          IDelegate<T> thenDelegate,
                          IDelegate<T> elseDelegate ) {
            m_condition = condition;
            m_then = thenDelegate;
            m_else = elseDelegate;
        }

        public void AddFunc ( Action<IDelegate<T>, T> func ) => AddFunc(func, AddToThen);

        public void AddThen ( Action<IDelegate<T>, T> func ) => AddFunc(func, true);

        public void AddElse ( Action<IDelegate<T>, T> func ) => AddFunc(func, false);

        private void AddFunc ( Action<IDelegate<T>, T> func, bool then ) {
            if ( then ) m_then.AddFunc(func);
            else m_else.AddFunc(func);
        }

        public void Invoke ( T arg ) {
            if ( m_condition(arg) )
                m_then.Invoke(arg);
            else
                m_else.Invoke(arg);
        }

        public void RemoveFunc ( Action<IDelegate<T>, T> func ) {
            m_then.RemoveFunc(func);
            m_else.RemoveFunc(func);
        }

        public void Clear () {
            m_then.Clear();
            m_else.Clear();
        }
    }
}
