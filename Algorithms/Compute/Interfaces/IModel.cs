using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Algorithms.Compute.Interfaces {
    internal interface IModel<TInput, TOutput> {
        TOutput Evaluate ( TInput input );
    }
}
