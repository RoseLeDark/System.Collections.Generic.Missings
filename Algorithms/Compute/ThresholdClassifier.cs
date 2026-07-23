using System;
using System.Collections.Generic;
using System.Text;
using SystemEx.Algorithms.Compute.Interfaces;

namespace SystemEx.Algorithms.Compute {
    public sealed class ThresholdClassifier : IThreshold {
        private readonly float _trueThreshold;
        private readonly float _falseThreshold;

        public ThresholdClassifier ( float trueThreshold, float falseThreshold ) {
            _trueThreshold = trueThreshold;
            _falseThreshold = falseThreshold;
        }

        public Triple Evaluate ( float value ) {
            if ( value >= _trueThreshold ) return triple.True;
            if ( value <= _falseThreshold ) return triple.False;
            return triple.Nin;
        }
    }

}
