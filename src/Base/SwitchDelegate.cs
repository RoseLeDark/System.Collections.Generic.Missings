/* 
 * SPDX-License-Identifier: EUPL-1.2
 *
 * Copyright (c) 2026 Amber-Sophia Schröck <ambersophia.schroeck@mail.de>
 *
 * This file is licensed under the European Union Public Licence (EUPL) version 1.2.
 * You can obtain a copy of the licence at:
 *   https://joinup.ec.europa.eu/collection/eupl/eupl-text-eupl-12
 *
 * Unless required by applicable law or agreed to in writing, software distributed
 * under the Licence is distributed on an "AS IS" basis, WITHOUT WARRANTIES OR
 * CONDITIONS OF ANY KIND, either express or implied.
 *
 * If you modify this file, retain this notice and add a short description of your
 * changes and the date.
 */

using SystemEx.Collections.Generic;

namespace SystemEx {

	/// <summary>
	/// Provides a multi‑branch delegate dispatcher driven by a switch‑condition.
	/// 
	/// <para>
	/// <see cref="SwitchDelegate{TS, T}"/> extends the concept of
	/// <c>IFDelegate&lt;T&gt;</c> by supporting an arbitrary number of
	/// conditional branches. Each branch is identified by a case key of type
	/// <typeparamref name="TS"/> and holds its own delegate chain.
	/// </para>
	/// 
	/// <para>
	/// When <see cref="Invoke"/> is called, the condition function selects the
	/// active case, and the corresponding delegate chain is executed. If no
	/// delegate is registered for the selected case, nothing is invoked.
	/// </para>
	/// 
	/// <para>
	/// This type is useful for state‑based dispatching, multi‑mode processing,
	/// or any scenario where behavior depends on a discrete selector rather
	/// than a boolean condition.
	/// </para>
	/// </summary>
	/// <typeparam name="TS">
	/// The switch key type used to identify delegate branches.
	/// Must be non‑nullable.
	/// </typeparam>
	/// <typeparam name="T">
	/// The argument type passed to subscribed delegates.
	/// </typeparam>
	public class SwitchDelegate<TS, T> where TS : notnull {
        private Map<TS, Delegate<T>> m_switchState;

        private readonly Func<T, TS> m_condition;
		private  Optional<TS> m_defaultKey;

		/// <summary>
		/// Initializes a new switch‑based delegate dispatcher.
		/// </summary>
		public SwitchDelegate ( Func<T, TS> condition ) {
			m_condition = condition;
			m_switchState = new Map<TS, Delegate<T>>();
		}

		/// <summary>
		/// Initializes a new switch‑based delegate dispatcher.
		/// </summary>
		public SwitchDelegate (Func<T, TS> condition, Optional<Pair<Action<IDelegate<T>, T>, TS>> deft_case ) {
            m_condition = condition;
            m_switchState = new Map<TS, Delegate<T>>();
			
			if ( deft_case.HasValue ) {
				m_defaultKey = deft_case.Value!.Second;

				m_switchState[deft_case.Value!.Second] = new Delegate<T>(deft_case.Value!.First);

			}
		}

		/// <summary>
		/// Subscribes a callback to the delegate chain associated with the given case.
		/// If the case does not exist yet, it is created.
		/// </summary>
		/// <param name="func">The callback to subscribe.</param>
		/// <param name="_case">The case key identifying the delegate branch.</param>
		public void Subscribe ( Action<IDelegate<T>, T> func, TS _case ) {
            if(m_switchState.ContainsKey(_case)) {
                m_switchState[_case].Value!.Subscribe(func);
            } else {
                m_switchState[_case] = new Delegate<T>(func);
            }
        }

		/// <summary>
		/// Invokes the delegate chain associated with the case selected by the
		/// condition function. If the selected case has no delegate, nothing happens.
		/// </summary>
		/// <param name="arg">The argument passed to the selected delegate chain.</param>
		public void Invoke ( T arg ) {
            var _switch = m_condition(arg);

            var _state  = m_switchState[_switch];

            if(_state.IsSome) {
                _state.Value!.Invoke(arg);
            } else if ( m_defaultKey.HasValue) {
				var _def  = m_switchState[m_defaultKey.Value!];
				if ( _def.IsSome ) _def.Value!.Invoke(arg);
			}
        }
		/// <summary>
		/// Removes a callback from the delegate chain associated with the given case.
		/// </summary>
		/// <param name="func">The callback to remove.</param>
		/// <param name="_case">The case key identifying the delegate branch.</param>
		public void UnSubscribe ( Action<IDelegate<T>, T> func, TS _case ) {
            if ( m_switchState.ContainsKey(_case) ) {
                m_switchState[_case].Value!.UnSubscribe(func);
            }
        }

    }
	
}
