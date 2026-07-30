using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Collections.Generic {
    internal interface IUsedIterrator<T, TItterator>  where TItterator : Iterrator<T> {

        /// <summary>
        /// 
        /// </summary>
        public TItterator Begin { get; }

        /// <summary>
        /// 
        /// </summary>
        public TItterator End { get; }

        /// <summary>
        /// 
        /// </summary>
        public TItterator ReverseBegin { get; }

        /// <summary>
        /// 
        /// </summary>
        public TItterator ReverseEnd { get; }
    }
}
