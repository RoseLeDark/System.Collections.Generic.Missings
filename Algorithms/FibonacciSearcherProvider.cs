using System;
using System.Collections.Generic;
using System.Text;
using SystemEx.Algorithms.Interfaces;
using SystemEx.Collections.Generic;
using SystemEx.Collections.Generic.Interfaces;
using SystemEx.Utils;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SystemEx.Algorithms {
    public struct FibonacciSearcherProvider<T, TContainer> : ISearchProvider<T, TContainer>
        where TContainer : IContainerEx<T> {


        public long Find ( ref TContainer container, ICompared<T> comp, T value ) {
            long _arCount= container.Count;
            long _ret = 0;

            if ( _arCount> 0 ) {
                // find the smallest Fibonacci number that equals or is greater than the array length
                long _fbPrevB = 0;
                long _fbNPrev= 1;
                long _fbNum = _fbNPrev;

                while ( _fbNum <= _arCount) {
                    _fbPrevB = _fbNPrev;
                    _fbNPrev= _fbNum;
                    _fbNum = _fbPrevB + _fbNPrev;
                }

                // offset to drop the left part of the array
                long offset = -1;

                while ( _fbNum > 1 ) {
                    var index = System.Math.Min(offset + _fbPrevB, _arCount- 1);

                    switch ( comp.Compare(container.ElementAt(index), value) ) {
                    case CompareResult.AIsLargerB:
                    _fbNum = _fbNPrev;
                    _fbNPrev= _fbPrevB;
                    _fbPrevB = _fbNum - _fbNPrev;
                    offset = index;
                    break;

                    // reject approximately 2/3 of the existing array behind
                    // by moving Fibonacci numbers
                    case CompareResult.AIsSmallerB:
                    _fbNum = _fbPrevB;
                    _fbNPrev= _fbNPrev- _fbPrevB;
                    _fbPrevB = _fbNum - _fbNPrev;
                    break;
                    default:
                    _ret++; break;
                    }
                }

                // check the last element
                if ( _fbNPrev == 1 && comp.Compare(value, container.ElementAt(_arCount - 1) ) == CompareResult.Equal ) {
                    _ret++;
                }
            }
            return _ret;
        }

        public long Find ( ref TContainer container, Func<T, CompareResult> func ) {
            long _arCount= container.Count;
            long _ret = 0;

            if ( _arCount > 0 ) {
                // find the smallest Fibonacci number that equals or is greater than the array length
                long _fbPrevB = 0;
                long _fbNPrev= 1;
                long _fbNum = _fbNPrev;

                while ( _fbNum <= _arCount ) {
                    _fbPrevB = _fbNPrev;
                    _fbNPrev = _fbNum;
                    _fbNum = _fbPrevB + _fbNPrev;
                }

                // offset to drop the left part of the array
                long offset = -1;

                while ( _fbNum > 1 ) {
                    var index = System.Math.Min(offset + _fbPrevB, _arCount- 1);

                    switch ( func(container.ElementAt(index)) ) {
                    case CompareResult.AIsLargerB:
                    _fbNum = _fbNPrev;
                    _fbNPrev = _fbPrevB;
                    _fbPrevB = _fbNum - _fbNPrev;
                    offset = index;
                    break;

                    // reject approximately 2/3 of the existing array behind
                    // by moving Fibonacci numbers
                    case CompareResult.AIsSmallerB:
                    _fbNum = _fbPrevB;
                    _fbNPrev = _fbNPrev - _fbPrevB;
                    _fbPrevB = _fbNum - _fbNPrev;
                    break;
                    default:
                    _ret++; break;
                    }
                }

                // check the last element
                if ( _fbNPrev == 1 && func(container.ElementAt(_arCount - 1)) == CompareResult.Equal ) {
                    _ret++;
                }
            }
            return _ret;
        }

        public Vector<Pair<long, T>> Where ( ref TContainer container, Func<T, CompareResult> func ) {
            throw new NotImplementedException();
        }
    }
}
