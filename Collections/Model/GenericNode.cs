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
using SystemEx.Collections.Generic;
using SystemEx.Collections.Generic.Interfaces;

namespace SystemEx.Collections.Model {
    public class GenericNodeIterator<T> : IIterator<T>, IEquatable<GenericNodeIterator<T>> {
        private GenericNode<T> m_pNode;

        /// <inheritdoc/>
        public IIterator<T> Clone () {
            return new GenericNodeIterator<T>(m_pNode);
        }

        public void Forward () {
            return;
        }

        public void Forward ( long i ) {
            return;
        }
        public GenericNodeIterator(GenericNode<T> node) {
            m_pNode = node;
        }
        public bool Equals ( GenericNodeIterator<T>? other ) {
            if ( other == null ) return false;
            return m_pNode.Equals(other.m_pNode);
        }

        public override bool Equals ( object obj ) {
            return Equals(obj as GenericNodeIterator<T>);
        }

        public override int GetHashCode () {
            return m_pNode.GetHashCode();
        }
    }
    public class GenericNode<T> : IEquatable<GenericNode<T>> {
        protected T? m_tValue;

        public virtual IIterator Begin() => new GenericNodeIterator<T>(this.Clone());
        public virtual IIterator End () => new GenericNodeIterator<T>(this.Clone());

        public virtual T? Value { get => m_tValue; set => m_tValue = value; }

        public virtual bool HasValue => m_tValue != null;

        public GenericNode() {
            m_tValue = default(T);
        }
        public GenericNode(T? value) {
            m_tValue = value;
        }

        public GenericNode(GenericNode<T> other) {
            m_tValue = other.Value;
        }

        protected virtual GenericNode<T> Clone() {
            return new GenericNode<T>(m_tValue);
        }

        public virtual bool Equals ( GenericNode<T>? other ) {
            bool _ret = false;
            if ( other != null ) {
                if ( m_tValue != null )
                    _ret = m_tValue.Equals(other.m_tValue);
                else
                    _ret = m_tValue == null && other.m_tValue == null;
            }
            return _ret;
        }

        public override bool Equals ( object obj ) {
            return Equals(obj as GenericNode<T>);
        }

        public override int GetHashCode () {
            if ( m_tValue != null) return m_tValue.GetHashCode();
            return base.GetHashCode();
        }
    }
}
