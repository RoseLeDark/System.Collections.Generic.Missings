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
using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx {
	/// \addtogroup SystemEx
	/// @
	/// <summary>
	/// Represents a conditional delegate dispatcher that routes invocation
	/// to one of two delegate branches based on a boolean condition.
	/// </summary>
	/// <typeparam name="T">The argument type passed to the delegates.</typeparam>
	public class IFDelegate<T> {
        /// <summary>
        /// The condition function used to determine which delegate branch is invoked.
        /// </summary>
        private readonly Func<T, bool> m_condition;

        /// <summary>
        /// The delegate invoked when the condition evaluates to <c>true</c>.
        /// </summary>
        private readonly IDelegate<T> m_then;

        /// <summary>
        /// The delegate invoked when the condition evaluates to <c>false</c>.
        /// </summary>
        private readonly IDelegate<T> m_else;

        /// <summary>
        /// Gets or sets a value indicating whether new subscriptions should be added
        /// to the <c>then</c> branch by default.
        /// </summary>
        public bool AddToThen { get; set; }

        /// <summary>
        /// Initializes a new conditional delegate dispatcher.
        /// </summary>
        /// <param name="condition">
        /// A boolean function that determines which delegate branch is invoked.
        /// </param>
        /// <param name="thenDelegate">
        /// The delegate invoked when the condition evaluates to <c>true</c>.
        /// </param>
        /// <param name="elseDelegate">
        /// The delegate invoked when the condition evaluates to <c>false</c>.
        /// </param>
        public IFDelegate (
            Func<T, bool> condition,
            IDelegate<T> thenDelegate,
            IDelegate<T> elseDelegate ) {
            m_condition = condition;
            m_then = thenDelegate;
            m_else = elseDelegate;
        }

        /// <summary>
        /// Subscribes a callback function to either the <c>then</c> or <c>else</c> branch.
        /// </summary>
        /// <param name="func">The callback function to subscribe.</param>
        /// <param name="then">
        /// If <c>true</c>, the callback is added to the <c>then</c> branch;
        /// otherwise it is added to the <c>else</c> branch.
        /// </param>
        public void Subscribe ( Action<IDelegate<T>, T> func, bool then ) {
            if ( then )
                m_then.Subscribe(func);
            else
                m_else.Subscribe(func);
        }

        /// <summary>
        /// Invokes the appropriate delegate branch based on the condition.
        /// </summary>
        /// <param name="arg">The argument passed to the selected delegate.</param>
        public void Invoke ( T arg ) {
            if ( m_condition(arg) )
                m_then.Invoke(arg);
            else
                m_else.Invoke(arg);
        }

        /// <summary>
        /// Removes a callback function from either the <c>then</c> or <c>else</c> branch.
        /// </summary>
        /// <param name="func">The callback function to remove.</param>
        /// <param name="then">
        /// If <c>true</c>, the callback is removed from the <c>then</c> branch;
        /// otherwise it is removed from the <c>else</c> branch.
        /// </param>
        public void UnSubscribe ( Action<IDelegate<T>, T> func, bool then ) {
            if ( then )
                m_then.UnSubscribe(func);
            else
                m_else.UnSubscribe(func);
        }

        /// <summary>
        /// Clears both the <c>then</c> and <c>else</c> delegate branches.
        /// </summary>
        public void Clear () {
            m_then.Clear();
            m_else.Clear();
        }
    }
	//@}
}
