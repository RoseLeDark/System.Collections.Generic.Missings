using System;
using System.Collections.Generic;
using System.Text;
using SystemEx.Collections.Generic.Interfaces;
using SystemEx.Threading;

namespace SystemEx.Collections.Generic {
     
    public struct SubSlice<T, TContainer>
        where TContainer : IContainerEx<T> {

        private long m_start;
        private long m_len;
        private long m_end;

        private LightLock m_lock;
        private int m_managedID;

        public bool HasOwner => m_managedID != -1;

        public SubSlice ( long sliceStart, long sliceLen ) {
            m_start = sliceStart;
            m_len = sliceLen;
            m_lock = new LightLock();
            m_managedID = -1;
            m_end = m_start + m_len;
        }

        public long GetIndex ( int sub_index ) {
            long _ret = -1;
            var _index = sub_index + m_start;
           
            if( (_index >= m_start && _index <= m_end) ) {
                _ret = sub_index + m_start;
            }
            
            return _ret;
        }

        internal bool IsOwner(int id) {
            bool _ret = false;
            if ( m_lock.Lock() ) {
                _ret =  m_managedID == id;
            }
            return _ret;
        }
 
        internal bool SetOwner(int id) {
            bool _ret = false;

            if(m_lock.Lock()) {
                if ( m_managedID == -1 ) { 
                    m_managedID = id;
                    _ret = true;
                }
                m_lock.Unlock();
            }
            return _ret;
        }

        internal bool UnsetOwner(int id) {
            bool _ret = false;

            if ( m_lock.Lock() ) {
                if ( m_managedID == Thread.CurrentThread.ManagedThreadId ) {
                    m_managedID = -1;
                    _ret = true;
                }
                m_lock.Unlock();
            }
            return _ret;
        }
    }


    /// <summary>
    /// Provides a deterministic multi-thread slicing model over a container.
    /// A slice represents a continuous range of elements inside the container.
    /// Each slice can be exclusively owned by exactly one thread.
    /// </summary>
    /// <typeparam name="T">Element type stored in the container.</typeparam>
    /// <typeparam name="TContainer">
    /// Container type implementing <see cref="IContainerEx{T}"/>.
    /// </typeparam>
    public ref struct Slices<T, TContainer> : IEquatable<Slices<T, TContainer>>
        where TContainer : IContainerEx<T> {

        /// <summary>
        /// Reference to the underlying container. All slice operations index directly into it.
        /// </summary>
        private ref TContainer m_container;

        /// <summary>
        /// All computed SubSlices. Each SubSlice describes a mathematical range and ownership state.
        /// </summary>
        private Vector<SubSlice<T, TContainer>> m_subSlices;

        /// <summary>
        /// Number of elements assigned to each SubSlice (except the last one if a remainder exists).
        /// This value is the slice length, not the number of slices.
        /// </summary>
        private int m_sliceLen;

        /// <summary>
        /// Constructs a new Slices instance by dividing the container into fixed-size SubSlices.
        /// The last SubSlice may contain fewer elements if the container size is not divisible
        /// by <paramref name="sliceLen"/>.
        /// </summary>
        /// <param name="con">Reference to the underlying container.</param>
        /// <param name="sliceLen">Number of elements per SubSlice.</param>
        public Slices (ref TContainer con, int sliceLen) {
            m_container = ref con;
            m_sliceLen = sliceLen;

            var _size = con.Count / sliceLen;
            var _rem = con.Count % sliceLen;



            m_subSlices = new Vector<SubSlice<T, TContainer>>(_size, 1);

            for (int i = 0 ; i< _size ; i++ ) {
                m_subSlices.PushBack(new SubSlice<T, TContainer>(i * m_sliceLen, m_sliceLen ));
            }

            if ( _rem != 0 ) {
                long start = _size * m_sliceLen;
                m_subSlices.PushBack(new SubSlice<T, TContainer>(start, _rem));
            }

            m_subSlices.AutoGrow = false;
        }


        /// <summary>
        /// Assigns ownership of a SubSlice to a thread if the slice is currently unowned.
        /// Ownership is exclusive: only one thread may own a slice at any time.
        /// </summary>
        /// <param name="slice">The SubSlice to claim.</param>
        /// <param name="thread_id">The requesting thread ID.</param>
        /// <returns>
        /// <c>true</c> if ownership was successfully assigned; otherwise <c>false</c>.
        /// </returns>
        public bool SetOwner( SubSlice<T, TContainer> slice, int thread_id ) {
            bool _ret = false;
            

            if (!slice.HasOwner ) {
                _ret = slice.SetOwner(thread_id);
            }
            return _ret;
        }

        /// <summary>
        /// Retrieves a SubSlice by index and attempts to assign ownership to the requesting thread.
        /// If the slice is already owned, <c>null</c> is returned.
        /// </summary>
        /// <param name="sliceID">Index of the slice to retrieve.</param>
        /// <param name="thread_id">Thread ID requesting ownership.</param>
        /// <returns>
        /// The owned SubSlice if ownership was granted; otherwise <c>null</c>.
        /// </returns>
        public SubSlice<T, TContainer>? GetJobSlice (int sliceID, int thread_id ) {
            bool _ret = false;
            

            if ( !(sliceID < 0 || sliceID >= m_subSlices.Length) ) {
                if(!m_subSlices[sliceID].HasOwner) {
                    _ret = m_subSlices[sliceID].SetOwner(thread_id);
                }
            }
                

            return _ret ? m_subSlices[sliceID] : null;
        }

        /// <summary>
        /// Reads a value from the container through a SubSlice.
        /// Reading is only permitted if the calling thread owns the slice.
        /// The index is a slice-relative index.
        /// </summary>
        /// <param name="slice">The SubSlice providing access.</param>
        /// <param name="index">slice-relative index</param>
        /// <param name="value">Output value if the read succeeds.</param>
        /// <returns>
        /// <c>true</c> if the thread owns the slice and the index lies inside the slice; otherwise <c>false</c>.
        /// </returns>
        public bool GetValue( SubSlice<T, TContainer> slice, int index, out T? value ) {
            bool _ret = false;
            int id = Thread.CurrentThread.ManagedThreadId;

            value = default(T);

            // Lesen wenn owner bin
            if ( slice.IsOwner(id) ) {
                var _realindex = slice.GetIndex(index);

                if ( _realindex != -1) {
                    value = m_container.ElementAt(_realindex);
                    _ret = true;
                }
            }
            return _ret;
        }

        /// <inheritdoc/>
        public bool Equals ( Slices<T, TContainer> other ) {
            return m_sliceLen == other.m_sliceLen && 
                m_container.Equals(other.m_container) && 
                m_subSlices.Equals(other.m_subSlices);
        }
    }
}
