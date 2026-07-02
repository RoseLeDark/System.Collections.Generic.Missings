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

using SystemEx.Collections.Generic.Interfaces;

namespace SystemEx.Collections.Model {
    /// \addtogroup model
    /// @{
    /// <summary>
    /// Represents an iterator for traversing a generic node in the collection.
    /// </summary>
    /// <typeparam name="T">The type of the values stored in the node.</typeparam>
    public class GenericNodeIterator<T> : IIterator<T>, IEquatable<GenericNodeIterator<T>> {
        private GenericNode<T> m_pNode;

        /// <inheritdoc/>
        public IIterator<T> Clone () {
            return new GenericNodeIterator<T>(m_pNode);
        }

        /// <inheritdoc/>
        public T? Value => m_pNode.Value;

        /// <inheritdoc/>
        public void Forward () {
            return;
        }
        /// <summary>
        /// Moves the iterator forward by the specified number of positions.
        /// </summary>
        /// <param name="i">The number of positions to move forward.</param>
        public void Forward ( long i ) {
            return;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericNodeIterator{T}"/> class with the specified node.
        /// </summary>
        /// <param name="node">The generic node to iterate over.</param>
        public GenericNodeIterator(GenericNode<T> node) {
            m_pNode = node;
        }

        /// <summary>
        /// Compares this iterator with another for equality.
        /// </summary>
        /// <param name="other">The other iterator to compare with.</param>
        /// <returns>true if the iterators are equal, false otherwise.</returns>
        public bool Equals ( GenericNodeIterator<T>? other ) {
            if ( other == null ) return false;
            return m_pNode.Equals(other.m_pNode);
        }
        /// <inheritdoc/>
        public override bool Equals ( object? obj ) {
            return Equals(obj as GenericNodeIterator<T>);
        }
        /// <inheritdoc/>
        public override int GetHashCode () {
            return m_pNode.GetHashCode();
        }
    }

    /// <summary>
    /// Represents a generic node in the collection.
    /// </summary>
    /// <typeparam name="T">The type of the values stored in the node.</typeparam>
    public class GenericNode<T> : IEquatable<GenericNode<T>> {
        protected T? m_tValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericNode{T}"/> class.
        /// </summary>
        public GenericNode() {
            m_tValue = default(T);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericNode{T}"/> class with the specified value.
        /// </summary>
        /// <param name="value">The value to initialize the node with.</param>
        public GenericNode(T? value) {
            m_tValue = value;
        }

        /// <summary>
        /// Gets an iterator pointing to the beginning of the collection.
        /// </summary>
        /// <returns>An iterator pointing to the beginning of the collection.</returns>
        public virtual IIterator Begin() => new GenericNodeIterator<T>(this.Clone());
        /// <summary>
        /// Gets an iterator pointing to the end of the collection.
        /// </summary>
        /// <returns>An iterator pointing to the end of the collection.</returns>
        public virtual IIterator End () => new GenericNodeIterator<T>(this.Clone());

        /// <summary>
        /// Gets or sets the value of the node.
        /// </summary>
        public virtual T? Value { get => m_tValue; set => m_tValue = value; }

        /// <summary>
        /// Gets a value indicating whether the node has a value.
        /// </summary>
        public virtual bool HasValue => m_tValue != null;

     
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericNode{T}"/> class with the specified generic node.
        /// </summary>
        /// <param name="other">The generic node to initialize the node with.</param>
        public GenericNode(GenericNode<T> other) {
            m_tValue = other.Value;
        }
        /// <summary>
        /// Creates a clone of the node.
        /// </summary>
        /// <returns>A clone of the node.</returns>
        protected virtual GenericNode<T> Clone() {
            return new GenericNode<T>(m_tValue);
        }
        /// <summary>
        /// Compares this node with another for equality.
        /// </summary>
        /// <param name="other">The other node to compare with.</param>
        /// <returns>true if the nodes are equal, false otherwise.</returns>
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
        /// <inheritdoc/>
        public override bool Equals ( object obj ) {
            return Equals(obj as GenericNode<T>);
        }
        /// <inheritdoc/>
        public override int GetHashCode () {
            if ( m_tValue != null) return m_tValue.GetHashCode();
            return base.GetHashCode();
        }
    }
    /// @}
}
