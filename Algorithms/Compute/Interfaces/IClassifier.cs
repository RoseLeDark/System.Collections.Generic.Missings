using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Algorithms.Compute.Interfaces {
    internal interface IClassifier<T> {
        Triple Classify ( T input );
    }

    public interface ISimilarity<T> {
        Triple IsSimilar ( T a, T b );
    }
    public interface IThreshold {
        Triple Evaluate ( float value );
    }

}
