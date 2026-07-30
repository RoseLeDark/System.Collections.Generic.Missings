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
        public void Subscribe ( Action<IDelegate<T>, T> func ) => Subscribe(func, AddToThen);

        public void UnSubscribe ( Action<IDelegate<T>, T> func ) => RemoveFunc(func);

        public void SubscribeTrue ( Action<IDelegate<T>, T> func ) => Subscribe(func, true);

        public void SubscribeElse ( Action<IDelegate<T>, T> func ) => Subscribe(func, false);

        private void Subscribe ( Action<IDelegate<T>, T> func, bool then ) {
            if ( then ) m_then.Subscribe(func);
            else m_else.Subscribe(func);
        }

        public void Invoke ( T arg ) {
            if ( m_condition(arg) )
                m_then.Invoke(arg);
            else
                m_else.Invoke(arg);
        }

        public void RemoveFunc ( Action<IDelegate<T>, T> func ) {
            m_then.UnSubscribe(func);
            m_else.UnSubscribe(func);
        }

        public void Clear () {
            m_then.Clear();
            m_else.Clear();
        }

        
    }
}
