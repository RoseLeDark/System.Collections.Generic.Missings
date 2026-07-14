using System;
using System.Collections.Generic;
using System.Text;
using SystemEx.Algorithms.Interfaces;
using SystemEx.Collections.Generic;
using SystemEx.Collections.Generic.Interfaces;
using SystemEx.Utils;

namespace SystemEx.Algorithms {


    /// <summary>
    /// A specialized search provider implementing binary-search-based probing.
    /// 
    /// <para>
    /// This provider assumes that the underlying <typeparamref name="TContainer"/> is
    /// sorted according to the predicate or comparison logic used. It performs a
    /// midpoint probe to locate any matching element and then expands outward to
    /// collect all adjacent matches. This makes the provider efficient for clustered
    /// or repeated values in sorted sequences.
    /// </para>
    /// 
    /// <para>
    /// <b>Important:</b>
    /// This search provider is only valid for sorted containers. If the data is not
    /// sorted, use a linear search provider instead.
    /// </para>
    /// 
    /// <para>
    /// <b>Behavior:</b>
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// <c>Find</c> returns the number of matching elements by probing the midpoint
    /// and expanding left/right until non-matching elements are encountered.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <c>Where</c> returns all matching elements together with their indices.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Matching is determined solely by the provided predicate or
    /// <see cref="ISimpleCompare{T}"/> implementation.
    /// </description>
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
    /// <typeparam name="T">Element type stored in the container.</typeparam>
    /// <typeparam name="TContainer">
    /// Container type implementing <see cref="IContainerEx{T}"/>.
    /// Must provide indexed access and a stable element order.
    /// </typeparam>
    public struct BinarySearcherProvider< T, TContainer> : ISearchProvider<T, TContainer>
        where TContainer : IContainerEx<T> {

        /// <inheritdoc />
        public long Find ( ref TContainer container, ICompared<T> comp, T value ) {
            if ( container.Count == 0 ) return 0;

            long _ret = 0;
            long left = 0;
            long right = container.Count - 1;

            // Try to locate ANY match using midpoint probing
            while ( left <= right ) {
                long mid = (left + right) >> 1;
                T item = container.ElementAt(mid);

                if ( comp.Compare(item, value) == Utils.CompareResult.Equal  ) {
                    // Found one → expand to count all
                    _ret++;

                    // expand left
                    for ( var i = mid - 1 ; i >= 0 ; i-- ) {
                        if ( comp.Compare(container.ElementAt(i), value) == Utils.CompareResult.Equal )
                            _ret++;
                        else
                            break;
                    }

                    // expand right
                    for ( var i = mid + 1 ; i < container.Count ; i++ ) {
                        if ( comp.Compare(container.ElementAt(i), value) == Utils.CompareResult.Equal )
                            _ret++;
                        else
                            break;
                    }
                    break;
                }

                // Without ordering, we cannot decide direction → shrink inward
                left++;
                right--;
            }

            return _ret;
        }
        /// <inheritdoc />
        public long Find ( ref TContainer container, Func<T, CompareResult> func ) {
            if ( container.Count == 0 ) return 0;

            long _ret = 0;
            long left = 0;
            long right = container.Count - 1;

            // Try to locate ANY match using midpoint probing
            while ( left <= right ) {
                long mid = (left + right) >> 1;
                T item = container.ElementAt(mid);

                if ( func(item) == CompareResult.Equal ) {
                     _ret++;

                    // expand left
                    for ( var i = mid - 1 ; i >= 0 ; i-- ) {
                        if ( func(container.ElementAt(i)) == Utils.CompareResult.Equal )
                            _ret++;
                        else
                            break;
                    }

                    // expand right
                    for ( var i = mid + 1 ; i < container.Count ; i++ ) {
                        if ( func(container.ElementAt(i)) == Utils.CompareResult.Equal )
                            _ret++;
                        else
                            break;
                    }
                    break;
                }

                // Without ordering, we cannot decide direction → shrink inward
                left++;
                right--;
            }

            return _ret; 
        }


        /// <inheritdoc />
        public Vector<Pair<long, T>> Where ( ref TContainer container, Func<T, CompareResult> func ) {
            Vector<Pair<long, T>> result = new Vector<Pair<long, T>>();

            long left = 0;
            long right = container.Count - 1;

            // Try to locate ANY match using midpoint probing
            while ( left <= right ) {
                long mid = (left + right) >> 1;
                T item = container.ElementAt(mid);

                if ( func(item) == Utils.CompareResult.Equal ) {
                    // Found one → expand to count all
                    //if ( func(item) )
                    result.PushBack(new Pair<long, T>(mid, item));

                    // expand left
                    for ( var i = mid - 1 ; i >= 0 ; i-- ) {
                        if ( func(container.ElementAt(i)) == Utils.CompareResult.Equal )
                            result.PushBack(new Pair<long, T>(i, container.ElementAt(i)));
                        else
                            break;
                    }

                    // expand right
                    for ( var i = mid + 1 ; i < container.Count ; i++ ) {
                        if ( func(container.ElementAt(i)) == Utils.CompareResult.Equal )
                            result.PushBack(new Pair<long, T>(i, container.ElementAt(i)));
                        else
                            break;
                    }
                    break;
                }

                // Without ordering, we cannot decide direction → shrink inward
                left++;
                right--;
            }

            return result;
        }
    }
}
