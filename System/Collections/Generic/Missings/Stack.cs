using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace System.Collections.Generic.Missings {

    public struct StackLayer {
        public int EndMarker;
        public int StartMarker;
        public int Current;
        public bool Enable;
    }
    public class Stack<T> {
        protected T[] m_elements;
        protected int m_end;
        protected int m_start;
        protected int m_current;

        protected StackLayer[] m_stackLayer = new StackLayer[4];
        

        public bool IsFull => m_current == m_end;
        public bool IsEmpty => m_current == m_elements.Length - 1;

        public int EndFiler { get { return m_end; } set => SetEnd(value);  }
        public int StartFilter { get { return m_start; } }

        public void ResetFilter() {
            m_end = 0;
            m_start = m_elements.Length-1;

        }
        public Stack(int size) {
            m_elements = new T[size];
            m_end = 0;
            m_start = m_elements.Length - 1;
            m_current = size-1;

            for (int i = 0; i < m_stackLayer.Length; i++) {
                m_stackLayer[i].Enable = false;
                m_stackLayer[i].EndMarker = m_end;
                m_stackLayer[i].StartMarker = m_start;
                m_stackLayer[i].Current = m_current;
            }
        }

        public bool Push(T element) {
            if ( IsFull ) return false;

            m_elements[m_current] = element;
            m_current--;
            return true;
        }

        public bool Pop(out T? item) {
            if ( IsEmpty ) { item = default; return false; }

            item = m_elements[m_current];
            m_current++;

            return true;
        }

        public bool Peek(ref T item) {
            if ( IsEmpty ) return false;
            item = m_elements[m_current];
            return true;
        }

        public int PushRange(T[] items, uint LayerID = 100 ) {
            if ( LayerID >= 4 ) return -1;
            if ( items == null ) return -1;

            bool layered= LayerID <= m_stackLayer.Length;
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

        public T[]? PopRange(int size, uint LayerID = 100) {
            if ( LayerID >= 4 ) { return null; }

            List<T> popped = new List<T>();
            T? item ;

            bool layered= LayerID <= m_stackLayer.Length;

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

        public bool Pop(out T? item, uint LayerID) {
            if ( LayerID >= 4 ) { item = default; return false; }
            if ( (m_stackLayer[LayerID].Enable != true) ) { item = default; return false; }
            // IsEmpty 
            if ( m_stackLayer[LayerID].Current == m_stackLayer[LayerID].StartMarker - 1 ) { item = default; return false; }
            item = m_elements[m_stackLayer[LayerID].Current];

            m_stackLayer[LayerID].Current++;

            return true;

        }

        public bool Push(T item, uint LayerID) {
            if ( LayerID >= 4 ) return false;
            if ( (m_stackLayer[LayerID].Enable != true) ) return false;
            // IsFull
            if ( m_stackLayer[LayerID].Current == m_stackLayer[LayerID].EndMarker ) return false;
            m_elements[m_stackLayer[LayerID].Current] = item;

            m_stackLayer[LayerID].Current--;

            return true;

        }
        public bool Peek(ref T item, uint LayerID) {
            if(LayerID >= 4) return false;
            if ( (m_stackLayer[LayerID].Enable != true) ) return false;

            if ( m_stackLayer[LayerID].Current == m_stackLayer[LayerID].StartMarker - 1 ) return false;
            item = m_elements[m_stackLayer[LayerID].Current];

            return true;
        }

        public void SetLayerOn(uint id, bool enable) {
            if ( id >= m_stackLayer.Length ) throw new InvalidOperationException();
            m_stackLayer[id].Enable = enable;
        }

        public void SetLayer(uint id, int startLayer, int endLayer, bool enable) {
            if(id >= m_stackLayer.Length) throw new InvalidOperationException();
            m_stackLayer[id] = new StackLayer {
                EndMarker = startLayer < 0 ? 0 : startLayer,
                StartMarker =  endLayer >= m_elements.Length ? m_elements.Length - 2 : endLayer,
                Enable = enable
            };
        }

        private void SetEnd(int value) {
            if ( m_current < value ) throw new InvalidOperationException();
            m_end = value;
        }
    }
}
