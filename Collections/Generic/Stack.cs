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

namespace SystemEx.Collections.Generic {
	/// \addtogroup Collections
	/// @{

	/// <summary>
	/// Represents a logical sub‑stack inside a shared stack buffer.
	/// A layer defines its own independent boundaries and a current pointer,
	/// allowing multiple virtual stacks to coexist within the same array.
	/// </summary>
	public struct StackLayer {
        /// <summary>
        /// Gets or sets the lower boundary (inclusive) of the layer.
        /// This is the point where the layer becomes full.
        /// </summary>
        public int EndMarker { get; set; }

        /// <summary>
        /// Gets or sets the upper boundary (exclusive) of the layer.
        /// This is the point where the layer becomes empty.
        /// </summary>
        public int StartMarker { get; set; }

        /// <summary>
        /// Gets or sets the current index of the layer's top element.
        /// </summary>
        public int Current { get; set; }

        /// <summary>
        /// Indicates whether this layer is active and allowed to perform push/pop operations.
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// Creates a new stack layer with the specified boundaries and current pointer.
        /// </summary>
        /// <param name="end">The lower boundary of the layer.</param>
        /// <param name="start">The upper boundary of the layer.</param>
        /// <param name="c">The initial current pointer.</param>
        public StackLayer(int end, int start, int c) {
            EndMarker = end;
            StartMarker = start;
            Current = c;
            Enable = false;
        }
    }
    /// <summary>
    /// A fixed‑size, backward‑growing stack implementation that supports
    /// multiple independent virtual stack layers sharing the same underlying buffer.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the stack.</typeparam>
#pragma warning disable CA1711
    public class Stack<T> {
#pragma warning restore CA1711
        /// <summary>
        /// Internal storage buffer for all stack elements.
        /// </summary>
        private T[] m_elements;

        /// <summary>
        /// Global lower boundary for the main stack.
        /// </summary>
        private int m_end;

        /// <summary>
        /// Global upper boundary for the main stack.
        /// </summary>
        private int m_start;

        /// <summary>
        /// Current pointer for the main stack.
        /// </summary>
        private int m_current;

        /// <summary>
        /// Internal array of up to four independent stack layers.
        /// </summary>
        internal StackLayer[] m_stackLayer = new StackLayer[4];

        /// <summary>
        /// Gets the underlying element buffer.
        /// </summary>
        protected T[] Elements => m_elements;

        /// <summary>
        /// Gets the global lower boundary of the main stack.
        /// </summary>
        protected int Ende => m_end;

        /// <summary>
        /// Gets the global upper boundary of the main stack.
        /// </summary>
        protected int Start => m_start;

        /// <summary>
        /// Gets the current pointer of the main stack.
        /// </summary>
        protected int Current => m_current;

        /// <summary>
        /// Indicates whether the main stack is full.
        /// </summary>
        public bool IsFull => m_current == m_end;

        /// <summary>
        /// Indicates whether the main stack is empty.
        /// </summary>
        public bool IsEmpty => m_current == m_elements.Length - 1;


        /// <summary>
        /// Gets or sets the lower boundary of the main stack.
        /// </summary>
        public int EndFilter { get => m_end; set => SetEnd(value); }

        /// <summary>
        /// Gets the upper boundary of the main stack.
        /// </summary>
        public int StartFilter => m_start;
        /// <summary>
        /// Resets the main stack boundaries to the full buffer range.
        /// </summary>
        public void ResetFilter() {
            m_end = 0;
            m_start = m_elements.Length-1;

        }
        /// <summary>
        /// Creates a new stack with the specified capacity.
        /// </summary>
        /// <param name="size">The number of elements the stack can hold.</param>

        public Stack(int size) {
            m_elements = new T[size];
            m_end = 0;
            m_start = m_elements.Length - 1;
            m_current = size-1;

            for (int i = 0; i < m_stackLayer.Length; i++) {
                m_stackLayer[i] = new StackLayer(m_end, m_start, m_current);
                m_stackLayer[i].Enable = false;
                m_stackLayer[i].EndMarker = m_end;
                m_stackLayer[i].StartMarker = m_start;
                m_stackLayer[i].Current = m_current;
            }
        }


        /// <summary>
        /// Creates a FlexSpan view over the valid portion of this array.
        /// The span does not copy data; it directly references the internal buffer.
        /// </summary>
        /// <param name="mode">
        /// The indexing mode of the span (System, Reverse, Ring).
        /// </param>
        /// <returns>
        /// A FlexSpan that views the range [0 .. m_index).
        /// </returns>
        public FlexSpan<T> AsFlexSpan ( FlexSpanMode mode = FlexSpanMode.System )
            => new FlexSpan<T>(ref m_elements!, 0, m_elements.Length, mode);


        /// <summary>
        /// Creates a FlexSpan view starting at the specified offset.
        /// The span references the internal buffer directly and does not allocate.
        /// </summary>
        /// <param name="start">
        /// The starting index inside the internal array.
        /// </param>
        /// <param name="mode">
        /// The indexing mode of the span (System, Reverse, Ring).
        /// </param>
        /// <returns>
        /// A FlexSpan that views the range [start .. m_index).
        /// </returns>
        public FlexSpan<T> AsFlexSpan ( long start, FlexSpanMode mode = FlexSpanMode.System )
            => new FlexSpan<T>(ref m_elements!, start, m_elements.Length, mode);

        /// <summary>
        /// Pushes an element onto the main stack.
        /// </summary>
        /// <returns><c>true</c> if the element was pushed; otherwise <c>false</c>.</returns>
        public bool Push(T element) {
            if ( IsFull ) return false;
            m_elements[m_current] = element;
            m_current--;
            return true;
        }

        /// <summary>
        /// Pushes an element onto the specified layer.
        /// </summary>
        public bool Push(T item, uint LayerID) {
            if ( LayerID >= 4 ) return false;
            if ( !m_stackLayer[LayerID].Enable ) return false;

            if ( m_stackLayer[LayerID].Current == m_stackLayer[LayerID].EndMarker )
                return false;

            m_elements[m_stackLayer[LayerID].Current] = item;
            m_stackLayer[LayerID].Current--;
            return true;
        }

        /// <summary>
        /// Pops an element from the main stack.
        /// </summary>
        /// <param name="item">Receives the popped element.</param>
        /// <returns><c>true</c> if an element was popped; otherwise <c>false</c>.</returns>
        public bool Pop(out T? item) {
            if ( IsEmpty ) { item = default; return false; }
            item = m_elements[m_current];
            m_current++;
            return true;
        }

        /// <summary>
        /// Retrieves the top element of the main stack without removing it.
        /// </summary>
        public bool Peek(ref T item) {
            if ( IsEmpty ) return false;
            item = m_elements[m_current];
            return true;
        }

        /// <summary>
        /// Pushes a range of elements onto either the main stack or a layer.
        /// </summary>
        /// <param name="items">The elements to push.</param>
        /// <param name="LayerID">The layer index (0–3), or 100 for the main stack.</param>
        /// <returns>The number of successfully pushed elements.</returns>
        public int PushRange(T[] items, uint LayerID = 100) {
            if ( LayerID >= 4 ) return -1;
            if ( items == null ) return -1;

            bool layered = LayerID <= m_stackLayer.Length;
            int i = 0;

            for ( i = 0; i < items.Length; i++ ) {
                if ( layered ) {
                    if ( !Push(items[i], LayerID) ) break;
                } else {
                    if ( !Push(items[i]) ) break;
                }
            }

            return i;
        }

        /// <summary>
        /// Pops a range of elements from either the main stack or a layer.
        /// </summary>
        /// <param name="size">The number of elements to pop.</param>
        /// <param name="LayerID">The layer index (0–3), or 100 for the main stack.</param>
        /// <returns>An array of popped elements, or <c>null</c> if invalid.</returns>
        public T[]? PopRange(int size, uint LayerID = 100) {
            if ( LayerID >= 4 ) return null;

            List<T> popped = new List<T>();
            T? item;

            bool layered = LayerID <= m_stackLayer.Length;

            for ( int i = 0; i < size; i++ ) {
                if ( layered ) {
                    if ( !Pop(out item, LayerID) ) break;
                } else {
                    if ( !Pop(out item) ) break;
                }

                if ( item != null ) popped.Add(item);
            }

            return popped.ToArray();
        }
        /// <summary>
        /// Pops an element from the specified layer.
        /// </summary>
        public bool Pop(out T? item, uint LayerID) {
            if ( LayerID >= 4 ) { item = default; return false; }
            if ( (m_stackLayer[LayerID].Enable != true) ) { item = default; return false; }
            // IsEmpty 
            if ( m_stackLayer[LayerID].Current == m_stackLayer[LayerID].StartMarker - 1 ) { item = default; return false; }
            item = m_elements[m_stackLayer[LayerID].Current];

            m_stackLayer[LayerID].Current++;

            return true;

        }

        /// <summary>
        /// Retrieves the top element of the specified layer without removing it.
        /// </summary>
        public bool Peek(ref T item, uint LayerID) {
            if(LayerID >= 4) return false;
            if ( (m_stackLayer[LayerID].Enable != true) ) return false;

            if ( m_stackLayer[LayerID].Current == m_stackLayer[LayerID].StartMarker - 1 ) return false;
            item = m_elements[m_stackLayer[LayerID].Current];

            return true;
        }
        /// <summary>
        /// Enables or disables a specific layer.
        /// </summary>
        public void SetLayerOn(uint id, bool enable) {
            if ( id >= m_stackLayer.Length ) throw new InvalidOperationException();
            m_stackLayer[id].Enable = enable;
        }
        /// <summary>
        /// Configures the boundaries and activation state of a layer.
        /// </summary>
        public void SetLayer(uint id, int startLayer, int endLayer, bool enable) {
            if(id >= m_stackLayer.Length) throw new InvalidOperationException();
            m_stackLayer[id] = new StackLayer {
                EndMarker = startLayer < 0 ? 0 : startLayer,
                StartMarker =  endLayer >= m_elements.Length ? m_elements.Length - 2 : endLayer,
                Enable = enable
            };
        }
        /// <summary>
        /// Sets the lower boundary of the main stack.
        /// </summary>
        private void SetEnd(int value) {
            if ( m_current < value ) throw new InvalidOperationException();
            m_end = value;
        }
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
