
using SystemEx.Algorithms.Interfaces;
using SystemEx.Collections.Generic;
using SystemEx.Utils;

namespace SystemEx.Algorithms {

    /// \addtogroup Algorithms
    /// @{
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
    /// Container type implementing <see cref="IVector{T}"/>.
    /// Must provide indexed access and a stable element order.
    /// </typeparam>
    public struct BinarySearcherProvider< T, TContainer> : ISearchProvider<T, TContainer>
        where TContainer : IVector<T> {

        /// <inheritdoc />
        public long Find ( ref TContainer container, ICompared<T> comp, T value ) {
            if ( container.Count == 0 ) return 0;

            long _ret = 0;
            long left = 0;
            long right = container.Count - 1;

            // Try to locate ANY match using midpoint probing
            while ( left <= right ) {
                long mid = (left + right) >> 1;
                Optional<T> item = container.ElementAt(mid);
                if ( item.IsNull ) continue;

                if ( comp.Compare(item.Value, value) == Utils.CompareResult.Equal  ) {
                    // Found one → expand to count all
                    _ret++;

                    // expand left
                    for ( var i = mid - 1 ; i >= 0 ; i-- ) {
                        Optional<T> _i2 = container.ElementAt(i);
                        if ( item.IsNull ) continue;

                        if ( comp.Compare(_i2.Value, value) == Utils.CompareResult.Equal )
                            _ret++;
                        else
                            break;
                    }

                    // expand right
                    for ( var i = mid + 1 ; i < container.Count ; i++ ) {
                        Optional<T> _i2 = container.ElementAt(i);
                        if ( item.IsNull ) continue;

                        if (  comp.Compare(_i2.Value, value) == Utils.CompareResult.Equal )
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
        public long Find ( ref TContainer container, Func< Optional<T> , CompareResult> func ) {
            if ( container.Count == 0 ) return 0;

            long _ret = 0;
            long left = 0;
            long right = container.Count - 1;

            // Try to locate ANY match using midpoint probing
            while ( left <= right ) {
                long mid = (left + right) >> 1;
                Optional<T> item = container.ElementAt(mid);
                if ( item.IsNull ) continue;

                if ( func(item.Value!) == CompareResult.Equal ) {
                     _ret++;

                    // expand left
                    for ( var i = mid - 1 ; i >= 0 ; i-- ) {
                        Optional<T> _i2 = container.ElementAt(i);
                        if ( item.IsNull ) continue;

                        if (func(container.ElementAt(i).Value!) == Utils.CompareResult.Equal )
                            _ret++;
                        else
                            break;
                    }

                    // expand right
                    for ( var i = mid + 1 ; i < container.Count ; i++ ) {
                        Optional<T> _i2 = container.ElementAt(i);
                        if ( item.IsNull ) continue;

                        if ( func(container.ElementAt(i).Value!)  == Utils.CompareResult.Equal )
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
        public Vector<Pair<long, Optional<T>>> Where ( ref TContainer container, Func<Optional<T>, CompareResult> func ) {
            Vector<Pair<long, Optional<T> >> result = new Vector<Pair<long, Optional<T> >>();

            long left = 0;
            long right = container.Count - 1;

            // Try to locate ANY match using midpoint probing
            while ( left <= right ) {
                long mid = (left + right) >> 1;
                Optional<T> item = container.ElementAt(mid);
                if ( item.IsNull ) continue;


                if ( func(item.Value!) == Utils.CompareResult.Equal ) {
                    // Found one → expand to count all
                    //if ( func(item) )
                    result.PushBack(new Pair<long, Optional<T> >(mid, item.Value!));

                    // expand left
                    for ( var i = mid - 1 ; i >= 0 ; i-- ) {
                        Optional<T> _i2 = container.ElementAt(i);
                        if ( item.IsNull ) continue;

                        if ( func(_i2.Value!) == Utils.CompareResult.Equal )
                            result.PushBack(new Pair<long, Optional<T> >(i, _i2.Value!));
                        else
                            break;
                    }

                    // expand right
                    for ( var i = mid + 1 ; i < container.Count ; i++ ) {
                        Optional<T> _i2 = container.ElementAt(i);
                        if ( item.IsNull ) continue;

                        if (  func(_i2.Value!) == Utils.CompareResult.Equal )
                            result.PushBack(new Pair<long, Optional<T> >(i, _i2.Value! ));
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
    /// @}
}
