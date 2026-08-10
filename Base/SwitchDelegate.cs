using System;
using System.Collections.Generic;
using System.Text;
using SystemEx.Collections.Generic;

namespace SystemEx {
    public class SwitchDelegate<TS, T> where TS : notnull {
        private Map<TS, Delegate<T>> m_switchState;

        private readonly Func<T, TS> m_condition;

        public SwitchDelegate(Func<T, TS> condition) {
            m_condition = condition;
            m_switchState = new Map<TS, Delegate<T>>();
        }
        public void Subscribe ( Action<IDelegate<T>, T> func, TS _case ) {
            if(m_switchState.ContainsKey(_case)) {
                m_switchState[_case].Value!.Subscribe(func);
            } else {
                m_switchState[_case] = new Delegate<T>(func);
            }
        }

        /// <summary>
        /// Invokes the appropriate delegate branch based on the condition.
        /// </summary>
        /// <param name="arg">The argument passed to the selected delegate.</param>
        public void Invoke ( T arg ) {
            var _switch = m_condition(arg);

            var _state  = m_switchState[_switch];

            if(_state.IsSome) {
                _state.Value!.Invoke(arg);
            }
        }
        public void UnSubscribe ( Action<IDelegate<T>, T> func, TS _case ) {
            if ( m_switchState.ContainsKey(_case) ) {
                m_switchState[_case].Value!.UnSubscribe(func);
            }
        }
    }
}
