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

using SystemEx.Utils;
using SystemEx.Collections.Generic.Interfaces;
using System.Collections;

namespace SystemEx.Collections.Generic {
    /// <summary>
    /// Represents a dynamic list of <see cref="ITuple"/> objects.  
    /// Provides filtering utilities, indexed access, and basic list operations
    /// for working with heterogeneous tuple collections.
    /// </summary>
    public class TupleList : IEnumerable<ITuple>, IEnumerable {
        /// <summary>
        /// Internal storage for all tuple elements.
        /// </summary>
        internal List<ITuple> m_elements;

        /// <summary>
        /// Gets the number of tuples stored in the list.
        /// </summary>
        public int Count => m_elements.Count;

        /// <summary>
        /// Gets or sets the tuple at the specified index.
        /// </summary>
        public ITuple this[int index] {
            get => m_elements[index];
            set => m_elements[index] = value;
        }

        /// <summary>
        /// Creates an empty tuple list.
        /// </summary>
        public TupleList() {
            m_elements = new List<ITuple>();
        }

        /// <summary>
        /// Creates a tuple list with the specified initial capacity.
        /// </summary>
        public TupleList(int size) {
            m_elements = new List<ITuple>(size);
        }
        /// <summary>
        /// Creates a tuple list initialized with the specified collection.
        /// </summary>
        public TupleList(IEnumerable<ITuple> collection) {
            m_elements = new List<ITuple>(collection);
        }

        /// <summary>
        /// Returns all tuples of the specified type <typeparamref name="TU"/>.
        /// </summary>
        /// <typeparam name="TU">The tuple type to filter for.</typeparam>
        /// <returns>A list containing all matching tuples.</returns>
        public List<TU> GetAll<TU>() where TU : ITuple {
            List<TU> _ret = new List<TU>();

            foreach ( var item in m_elements ) {
                if ( item == null ) continue;
                if ( item is TU ) _ret.Add((TU)item);
            }

            return _ret;
        }

        /// <summary>
        /// Returns all tuples whose <see cref="ITuple.Count"/> matches the given value.
        /// </summary>
        /// <param name="count">The required tuple element count.</param>
        public List<ITuple> GetByCount(byte count) {
            List<ITuple> _ret = new List<ITuple>();

            foreach ( var item in m_elements ) {
                if ( item == null ) continue;
                if ( item.Count == count ) _ret.Add(item);
            }

            return _ret;
        }

        /// <summary>
        /// Adds a tuple to the list.
        /// </summary>
        public virtual void Add(ITuple tuple) {
            m_elements.Add(tuple);
        }

        /// <summary>
        /// Adds a range of tuples to the list.
        /// </summary>
        public virtual void AddRange(IEnumerable<ITuple> items) {
            m_elements.AddRange(items);
        }

        /// <summary>
        /// Removes all tuples from the list.
        /// </summary>
        public void Clear() => m_elements.Clear();

        /// <summary>
        /// Determines whether the list contains the specified tuple.
        /// </summary>
        public bool Contains(ITuple item) => m_elements.Contains(item);

        /// <summary>
        /// Copies the tuples into the specified array.
        /// </summary>
        public void CopyTo(ITuple[] array, int arrayIndex) =>
            m_elements.CopyTo(array, arrayIndex);

        /// <summary>
        /// Removes the specified tuple from the list.
        /// </summary>
        public virtual bool Remove(ITuple item) => m_elements.Remove(item);

        /// <summary>
        /// Returns an enumerator that iterates through the tuple list.
        /// </summary>
        public IEnumerator<ITuple> GetEnumerator() => m_elements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Returns the index of the specified tuple, or -1 if not found.
        /// </summary>
        public int IndexOf(ITuple item) => m_elements.IndexOf(item);

        /// <summary>
        /// Inserts a tuple at the specified index.
        /// </summary>
        public virtual void Insert(int index, ITuple item) =>
            m_elements.Insert(index, item);

        /// <summary>
        /// Removes the tuple at the specified index.
        /// </summary>
        public virtual void RemoveAt(int index) =>
             m_elements.RemoveAt(index);
    }
}
