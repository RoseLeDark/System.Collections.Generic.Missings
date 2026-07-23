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

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using SystemEx.Algorithms;
using SystemEx.Algorithms.Interfaces;
using SystemEx.Algorythmen;
using SystemEx.Collections.Generic.Interfaces;

namespace SystemEx.Collections.Generic {
    


    
    public struct Map<T, TU> : IEnumerable< Pair<T, TU> >, IEnumerable, ITraverse<Pair<T, TU>>, ICollection<Pair<T, TU>>
        where T : notnull {

        private long m_growSize;
        private bool m_autoGrow;

        public long Size => Int64.MaxValue;
  
        public Optional<TU> this[T key] {
            get  {
                return Get(key);
            }
        }

        /// <summary>
        /// Internal storage buffer for Map elements.
        /// </summary>
        private Pair<T, TU>[] m_elements;
        private byte [] m_state;

        /// <summary>
        /// Current number of valid elements stored in the Map.
        /// </summary>
        private long m_index;

        public ICollection<TU> Values {
            get {
                List<TU> tmp = new List<TU>();
                for(int i = 0 ; i < Length ; i++) {
                    if ( m_state[i] == 0 ) continue;

                    tmp.Add(m_elements[i].Second);
                }
                return tmp;
            }
        }

        public ICollection<T> Keys {
            get {
                List<T> tmp = new List<T>();
                for(int i = 0 ; i < Length ; i++) {
                    if ( m_state[i] == 0 ) continue;
                    else tmp.Add(m_elements[i].First);
                }
                return tmp;
            }
        }


        public long Length => m_elements.LongLength;

        public Pair<T, TU> Front => m_elements[0];
      
        public Pair<T, TU> Back => m_elements[Count-1];

      
        public long GrowSize {
            get => (m_autoGrow ? m_growSize : 0);
            set {
                m_growSize = value;
                m_autoGrow = (m_growSize > 0);
            }
        }
    
        public bool AutoGrow { get => (m_growSize == 0 ? false : m_autoGrow); set => m_autoGrow = value; }


        public long Count => m_index;

        public bool IsFull => (AutoGrow ? false : m_index >= Length);

        public bool IsEmpty => m_index == 0;

        /// <summary>
        /// Gets the element at the current logical position (m_index).
        /// 
        /// This is primarily useful during manual iteration or when treating the
        /// vector as a stack-like structure. Accessing Current when the vector is
        /// empty or m_index is out of range is undefined.
        /// </summary>
        public Pair<T, TU>? Current => m_elements[m_index];
    #if REPLAYE

        /// <summary>
        /// Creates a FlexSpan view over the entire map starting at index 0.
        /// 
        /// The view uses the specified indexing mode (System, Reverse, Ring) and
        /// provides a span-like interface backed directly by this vector.
        /// </summary>
        /// <param name="map">
        /// Reference to the map. Passed by ref to avoid copying the struct and
        /// to ensure the FlexSpan reflects the actual container.
        /// </param>
        /// <param name="mode">
        /// Indexing mode for the view:
        /// System  = forward indexing,
        /// Reverse = backward indexing,
        /// Ring    = circular wrap-around indexing.
        /// </param>
        /// <returns>
        /// A FlexSpan representing the full vector.
        /// </returns>
        public static ContainerFlexSpan<Pair<T, TU>, Map<T, TU>> AsFlexSpan ( ref Map<T, TU> map, FlexSpanMode mode = FlexSpanMode.System )
            => new ContainerFlexSpan<Pair<T, TU>, Map<T, TU>>(ref map, 0, mode);


        public static Slices<Pair<T, TU>,  Map<T, TU> > AsMultiSlices ( ref Map<T, TU> map, int devider ) {
            return new Slices<Pair<T, TU>, Map<T, TU> >(ref map, (int)(map.Count / devider));
        }

        public static ContainerFlexSpan<Pair<T, TU>, Map<T, TU>> AsFlexSpan ( ref Map<T, TU> map, long start, long end, FlexSpanMode mode = FlexSpanMode.System )
            => new ContainerFlexSpan<Pair<T, TU>, Map<T, TU>>(ref map, start, end, mode);

        public static Find<Pair<T, TU>, Map<T, TU>> AsFinder ( ref Map<T, TU> map )
            => new Find<Pair<T, TU>, Map<T, TU>>(ref map);

        public static Set<Pair<T, TU>, Map<T, TU> > AsSet ( ref Map<T, TU> map, ISimpleCompare<Pair<T, TU>>? comparer = null, SortAction<ISimpleCompare<Pair<T, TU>>, Map<T, TU> >? sorter = null )
            => new Set<Pair<T, TU>, Map<T, TU> >( ref map, comparer == null ? new Less<Pair<T, TU>>() : comparer, sorter == null ? SortActions.ShellSorter : sorter );

        public static MultiSet<Pair<T, TU>, Map<T, TU>> AsMultiSet ( ref Map<T, TU> map, ISimpleCompare<Pair<T, TU>>? comparer = null, SortAction<ISimpleCompare<Pair<T, TU>>, Map<T, TU>>? sorter = null )
            => new MultiSet<Pair<T, TU>, Map<T, TU>>(ref map, comparer == null ? new Less<Pair<T, TU>>() : comparer, sorter == null ? SortActions.ShellSorter : sorter);

        public static UnorderedSet<Pair<T, TU>, Map<T, TU>> AsUnorderedSet ( ref Map<T, TU> vec )
            => new UnorderedSet<Pair<T, TU>, Map<T, TU>>(ref vec);

        public RandomAccessIterator<Pair<T, TU>, Map<T, TU>> Begin
            => new RandomAccessIterator<Pair<T, TU>, Map<T, TU>>(this, 0);

        public RandomAccessIterator<Pair<T, TU>, Map<T, TU>> End
            => new RandomAccessIterator<Pair<T, TU>, Map<T, TU>>(this, Count);

        public RandomAccessIterator<Pair<T, TU>, Map<T, TU>> ReverseBegin
            => End;

        public RandomAccessIterator<Pair<T, TU>, Map<T, TU>> ReverseEnd
            => Begin;

        public RandomAccessIterator<Pair<T, TU>, Map<T, TU>> At ( long index ) {
            if ( index >= Length ) return End;
            else return new RandomAccessIterator<Pair<T, TU>, Map<T, TU>>(this, index);
        }

        public static UnorderedMultiSet<Pair<T, TU>, Map<T, TU>> AsUnorderedMultiSet ( ref Map<T, TU> vec )
            => new UnorderedMultiSet<Pair<T, TU>, Map<T, TU>>(ref vec);


        public static Search< Pair<UU, UT>, Map<UU, UT> > AsSearch<UU, UT> ( ref Map<UU, UT> map, ISearchProvider<Pair<UU, UT>, Map<UU, UT>>? provider = null )
            => new Search< Pair<UU, UT> , Map<UU, UT> >(ref map, provider == null ? new LinearSearchProvider< Pair<UU, UT>, Map<UU, UT> >() : provider);

#endif
        /// <summary>
        /// Gets the first element in the map.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the map is empty.</exception>
        public Pair<T, TU>? First {
            get {
                if ( m_elements.Length == 0 )
                    throw new InvalidOperationException("Map is empty");
                return m_elements[0];
            }
        }

        public Pair<T, TU>? Last {
            get {
                if ( m_elements.Length == 0 )
                    throw new InvalidOperationException("Map is empty");
                return m_elements[m_elements.Length - 1];
            }
        }

        int ICollection<Pair<T, TU>>.Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public Map ( long size, int growSize = 16 ) {
            m_elements = new Pair<T, TU>[size];

            m_state = new byte[size];
            m_index = 0;


            GrowSize = growSize;
        }
        public Map ( Pair<T, TU>[] e, int growSize = 16 ) {
            m_elements = new Pair<T, TU>[e.Length];
            m_state = new byte[e.Length];

            Buffer.LongCopy<Pair<T, TU>>(e, 0, m_elements, 0, e.Length);
            m_index = e.LongLength;

            GrowSize = growSize;
        }

        public Map ( IEnumerable<Pair<T, TU>> e, int growSize = 16 ) {
            var arr = e.ToArray();

            m_elements = new Pair<T, TU>[arr.Length];

            m_state = new byte[m_elements.Length];

            Buffer.LongCopy<Pair<T, TU>>(arr, 0, m_elements, 0, arr.Length);
            Buffer.LongFill<byte>(m_state, 0,1, m_elements.Length);

            m_index = arr.Length;

            GrowSize = growSize;
        }

        public Map ( Map<T, TU> other ) {
            m_elements = new Pair<T, TU>[other.Length];
            m_state = new byte[other.Length]; 

            Buffer.LongCopy<Pair<T, TU>>(other.m_elements, 0, m_elements, 0, other.Length);
            Buffer.LongCopy<byte>(other.m_state, 0, m_state, 0, other.Length);

            m_index = other.m_index;
            GrowSize = other.GrowSize;
        }

        public Map<T, TU> AsSegment ( long start, long length ) {
            if ( start < 0 || length < 0 )
                throw new ArgumentOutOfRangeException();

            // Ensure the segment lies fully inside the internal buffer
            ArgumentOutOfRangeException.ThrowIfLessThan((start + length), m_elements.Length);

            // Create a new Map<T, TU> that shares the same buffer
            // A segment must not grow, otherwise it would corrupt the shared buffer
            var seg = new Map<T, TU>(m_elements, 0);
            // Logical end of the segment
            seg.m_index = (int)(start + length);

            return seg;
        }

        public bool PushBack ( Pair<T,TU> entry ) {
            if ( ContainsKey(entry.First) ) return false;

            if ( m_index >= Length ) {
                if ( AutoGrow ) Grow();
                return false;
            }

            m_elements[m_index] = entry;
            m_index++;
            return true;
        }

        public bool PushFront ( Pair<T, TU> entry ) {
            if( ContainsKey(entry.First) ) return false;

            // If full, attempt to grow
            if ( m_index + 1 >= Length ) {
                if ( AutoGrow ) {
                    if ( !Resize(Length + GrowSize) )
                        return false;
                } else {
                    return false;
                }
            }

            // Shift all valid elements one slot to the right
            // m_index is the last valid index, so we shift [0 .. m_index]
            for ( long i = m_index ; i >= 0 ; i-- )
                m_elements[i + 1] = m_elements[i];

            // Insert new element at the front
            m_elements[0] = entry;

            // Increase logical count
            m_index++;

            return true;
        }

        public bool Insert ( long index, Pair<T, TU> entry ) {
            if ( index < 0 ) return false;
            if ( ContainsKey(entry.First) ) return false;

            // Grow wie im Indexer
            if ( index >= m_elements.Length || m_index >= m_elements.Length ) {
                if ( AutoGrow ) {
                    if ( !Resize(m_elements.Length + GrowSize) )
                        return false;
                } else {
                    return false;
                }
            }

            // Speicher nach rechts verschieben
            for ( long i = m_index ; i > index ; i-- )
                m_elements[i] = m_elements[i - 1];

            m_elements[index] = entry;

            m_index++;

            return true;
        }

        public IEnumerable<Pair<T, TU>> Find ( T Key ) {
            foreach ( var item in m_elements ) {
                
                if(item.First.Equals(Key) ) {
                    yield return item;
                }
            }
        }

        public bool InsertRange ( int start, IEnumerable<Pair<T, TU>> items ) {
            var _arr = items.ToArray();

            for ( long i = 0 ; i < _arr.Length ; i++ ) {
                if ( !Insert(start + i, _arr[i]) )
                    return false;
            }
            return true;
        }

        public bool Insert ( long start, long end, Pair<T, TU> entry ) => Insert(start, entry);

        public bool InsertRange ( long start, Pair<T, TU>[] entrys ) {
            for ( long i = 0 ; i < entrys.Length ; i++ ) {
                if ( !Insert(start + i, entrys[i]) )
                    return false;
            }
            return true;
        }


        public bool Replace ( long index, Pair<T, TU> entry ) {
            if ( index < 0 ) return false;

            if ( m_index >= m_elements.Length ) Grow();

            if ( index >= m_elements.Length ) {
                if ( AutoGrow ) {
                    if ( !Resize(m_elements.Length + GrowSize) )
                        return false;
                } else {
                    return false;
                }
            }

            m_elements[index] = entry;
            return true;
        }


        public bool Replace ( long start, long end, Pair<T, TU> entry ) {
            if ( start < 0 || end < start ) return false;
            if ( m_index >= m_elements.Length ) Grow();

            for ( long i = start ; i <= end ; i++ )
                m_elements[i] = entry;

            return true;
        }

        
        public bool ReplaceRange ( long start, Pair<T, TU>[] entrys ) {
            for ( long i = 0 ; i < entrys.Length ; i++ ) {
                if ( !Replace(start + i, entrys[i]) )
                    return false;
            }
            return true;
        }

      
        public bool Erase () {
            if ( IsEmpty ) return false;

            m_index--;


            return true;
        }

        public bool Erase ( long index ) {
            if ( IsEmpty ) return false;
            if ( index >= Length ) return false;

            

            return true;
        }

        public bool Erase ( long start, long end ) {
            if ( start < 0 || end < start ) return false;
            if ( start >= Length ) return false;

            var _real_end = System.Math.Min(end, Length);

            


            return true;
        }

        public bool Erase ( Pair<T, TU> value ) {
            bool _ret = false;

            for ( long i = 0 ; i < Count ; i++ ) {

                var _item = m_elements[i];

                if ( _item.Equals(value) ) {
                   
                }
            }

            return _ret;
        }

        public void Swap ( long i, long j ) {
            if ( i < 0 || j < 0 ) return;
            if ( i >= m_index || j >= m_index ) return;

            Pair<T, TU> tmp = m_elements[i];
            m_elements[i] = m_elements[j];
            m_elements[j] = tmp;
        }

 
        public Pair<T, TU> ElementAt ( long index ) {
            if ( IsEmpty || index >= Length ) throw new ArgumentOutOfRangeException();

            return m_elements[index];
        }

        public bool Grow () {
            if ( !AutoGrow ) return false;
            return Resize(Length + GrowSize);
        }

        public void Clear () {
            m_index = 0;
            var len = Length;
            m_elements = new Pair<T, TU>[len];
        }

        public IEnumerator<Pair<T, TU>> GetEnumerator () {
            for ( int i = 0 ; i < m_index ; i++ )
                yield return m_elements[i];
        }

        public void Traverse ( TraversMode mode, long startIndex, long endIndex, Action<Pair<T, TU>> func ) {
            var start = System.Math.Max(startIndex, 0);
            var end = System.Math.Min(endIndex,  m_index);

            if ( mode == TraversMode.Forwards ) {
                for ( long i = start ; i < end ; i++ )
                    func(m_elements[i]);
            } else if ( mode == TraversMode.Backwards ) {
                for ( long i = end ; i >= start ; i-- )
                    func(m_elements[i]);
            }
        }


        public Pair<bool, long> CopyTo ( Map<T, TU> vector, ulong VectorIndex ) {
            return CopyTo(0, vector, 0, VectorIndex);
        }


        public Pair<bool, long> CopyTo ( uint sourceOffset, Map<T, TU> destination, ulong destinationOffset, ulong count ) {

            long src = (long)sourceOffset;
            long dst = (long)destinationOffset;

            if ( src > Length ) src = (long)Length;

            long toCopy = System.Math.Min((long)count,
            System.Math.Min(System.Math.Max(0, (long)Length - src),
                     System.Math.Max(0, destination.Length - dst)));

            if ( toCopy <= 0 ) return new Pair<bool, long>(false, 0);

            Buffer.LongCopy<Pair<T, TU>>(m_elements, src, destination.m_elements, dst, toCopy);
            return new Pair<bool, long>(true, toCopy);
        }



        public Pair<bool, long> CopyFrom ( Map<T, TU> source, ulong sourceOffset, ulong destinationOffset, ulong count ) {
            long src = (long)sourceOffset;
            long dst = (long)destinationOffset;

            if ( dst > Length )
                dst = Length;

            long toCopy = System.Math.Min((long)count,
                System.Math.Min(System.Math.Max(0, source.Length - src),
                System.Math.Max(0, Length - dst)));

            // Wenn nichts passt → prüfen ob wir wachsen müssen
            if ( toCopy <= 0 ) {
                if ( !AutoGrow )
                    return new Pair<bool, long>(false, 0);

                long required = dst + (long)count;

                long newSize = Length;
                while ( required > newSize )
                    newSize += GrowSize;

                if ( !Resize(newSize) )
                    return new Pair<bool, long>(false, 0);

                // Nach Resize neu berechnen
                toCopy = System.Math.Min((long)count,
                    System.Math.Min(System.Math.Max(0, source.Length - src),
                        System.Math.Max(0, (long)Length - dst)));

                if ( toCopy <= 0 )
                    return new Pair<bool, long>(false, 0);
            }

            Buffer.LongCopy<Pair<T, TU>>(source.m_elements, src, m_elements, dst, toCopy);

            // m_index anpassen, falls wir weiter hinten geschrieben haben
            long end = dst + toCopy;
            if ( end > m_index )
                m_index = end;

            return new Pair<bool, long>(true, toCopy);
        }

        /// <summary>
        /// Returns a copy of the internal buffer.
        /// </summary>
        public Pair<T, TU>[] ToNative () => m_elements.ToArray();

        
        private bool Resize ( long size ) {
            if ( size == Length ) return false;
            if ( m_index > size )
                m_index = size;

            try {
                Array.Resize(ref m_elements, (int)size);
            } catch {
                return false;
            }
            return true;
        }

        public Type GetElementType () {
            return typeof(Pair<T, TU>);
        }

        public bool ContainsKey ( T Key ) {
            bool _ret = false;
            foreach ( var p in m_elements ) {
               
                if(p.EqualFirst(Key)) {
                    _ret = true;
                    break;
                }
            }
            return _ret;
        }

        /// <summary>
        /// Determines whether any element has the specified value.
        /// </summary>
        public bool ContainsValue ( TU value ) {
            bool _ret = false;
            foreach ( var p in m_elements ) {
                
                if ( p.EqualSecond(value) ) {
                    _ret = true;
                    break;
                }
            }
            return _ret;
        }

        public Optional<TU> Get ( T Key ) {

             Optional<TU> _ret = Optional<TU>.NONE;

            foreach ( var p in m_elements ) {
                //if ( p.IsNull ) continue;

                if ( p.EqualFirst(Key) ) {
                    _ret = p.Second;
                    break;
                }
            }
            return _ret;
        }
        public void Add ( Pair<T, TU> item ) => PushBack(item);

        public void RemoveAt ( int pos ) => Erase(pos);
        public void RemoveAt ( int start, int iend ) => Erase(start, iend);


        public bool Remove (  Pair<T, TU>  item ) => Erase(item);

        public bool Contains ( Pair<T, TU> item ) {
            return m_elements.Contains(item);
        }

        IEnumerator<Pair<T, TU>> IEnumerable<Pair<T, TU>>.GetEnumerator () {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator () {
            return GetEnumerator();
        }

        public void Traverse ( TraversMode mode, int startIndex, int endIndex, Action<Pair<T, TU>> func ) {
            int start = System.Math.Max(startIndex, 0);
            int end =   System.Math.Min(endIndex, m_elements.Length);

            if ( mode == TraversMode.Forwards ) {
                for ( int i = start ; i < end ; i++ ) {
                   
                    func(m_elements[i]);
                }
                   
            } else if ( mode == TraversMode.Backwards ) {
                for ( int i = end ; i >= start ; i-- ) {
                 
                    func(m_elements[i]);
                }
            }
        }


        public void CopyTo ( Pair<T, TU>[] array, int arrayIndex ) {
            m_elements.CopyTo(array, arrayIndex);
        }

        public bool TryGeValue ( T key, [MaybeNullWhen(false)] out TU value ) {

            foreach ( var p in m_elements ) {
                //if ( p.IsNull ) continue;

                if ( p.EqualFirst(key) ) {
                    value = p.Second;
                    return true;
                }
            }
            value = default!;
            return false;
        }

        public IVector<Pair<T, TU>> Duplicate () {
            throw new NotImplementedException();
        }
    }
}
