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


using System.ComponentModel;
using SystemEx.Collections.Generic.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SystemEx.Collections.Generic {
    public class BasicRope<T, TN> where TN : RopeChunkValue {
        public const long MAXSIZE = long.MaxValue;

        public IRandomAccessIterator<T> Begin { get; }
        public IRandomAccessIterator<T> End { get; }

        public IRandomAccessIterator<T> ReverseBegin { get; }
        public IRandomAccessIterator<T> ReverseEnd { get; }

        protected RopeChunkValue Entry { get;  }

        

        public BasicRope () {
            
        }
        public BasicRope ( Array<T> array ) {
            foreach ( T i in array ) {
                PushBack(i);
            }
        }
        public BasicRope ( T[] array ) {
            foreach ( T i in array ) {
                PushBack(i);
            }
        }

        public BasicRope ( T value ) {
            PushBack(value);
        }
        public void swap ( ref BasicRope<T, TN> x ) {
            
        }
        public T GetAt ( long index ) {
            return default(T);
        }
        public void PushFront () {

        }

        public void PushBack ( T value ) {
          
        }

        public void PopFront () {

        }
        public void PopBack () {

        }
        public BasicRope<T, TN> Insert ( long position, T value ) {
            return this;
        }
        public BasicRope<T, TN> Insert ( long position, T[] val, long lenght = -1 ) {
            return this;
        }
        public BasicRope<T, TN> Insert ( long position, T[] f, T[] l ) {
            return this;
        }
        public BasicRope<T, TN> Insert ( long position, BasicRope<T, TN> other ) {
            return this;
        }
        public BasicRope<T, TN> Append ( T[] s ) {
            return this;
        }
        public BasicRope<T, TN> Append ( T s ) {
            return this;
        }
        public BasicRope<T, TN> Append ( T[] f, T[] l ) {
            return this;
        }
        public BasicRope<T, TN> Append ( BasicRope<T, TN> other ) {
            return this;
        }
        public BasicRope<T, TN> Replace ( long position, T value ) {
            return this;
        }
        public BasicRope<T, TN> Replace ( long position, T[] val, long lenght = -1 ) {
            return this;
        }
        public BasicRope<T, TN> Replace ( long position, T[] f, T[] l ) {
            return this;
        }
        public void Copy ( ref Array<T> buf ) {

        }
        public long Copy( long position, ref Array<T> buf ) {
            return 0;
        }
        /// <summary>
        /// Sequence    Erases the element pointed to by p.
        /// </summary>
        /// <param name="p"></param>
        public void Erase (IRandomAccessIterator<T> p) {

        }
        /// <summary>
        /// Sequence    Erases the range [f, l).
        /// </summary>
        /// <param name="f"></param>
        /// <param name="l"></param>
        public void Erase (IRandomAccessIterator<T> f, IRandomAccessIterator<T> l) {

        }
        /// <summary>
        /// Erases n elements, starting with the ith element.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="n"></param>
        public void Erase ( long i, long n )  { 
        
        }


        public BasicRope<T, TN> Substr ( IRandomAccessIterator<T> f ) {
            return this;
        }

        public BasicRope<T, TN> Substr ( IRandomAccessIterator<T> f, IRandomAccessIterator<T> l ) {
            return this;
        }
        public BasicRope<T, TN> Substr ( long i, long n = 1 ) {
            return this;
        }
    }

    
}
