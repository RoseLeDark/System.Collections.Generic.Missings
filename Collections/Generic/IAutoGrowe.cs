using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Collections.Generic {
    /// <summary>
    /// 
    /// </summary>
    public interface IAutoGrowe {
        /// <summary>
        /// 
        /// </summary>
        long GrowSize { get; set; }
        /// <summary>
        /// 
        /// </summary>
        bool AutoGrow { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool Grow ();

    }
}
