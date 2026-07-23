using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Algorithms.Compute.Interfaces {
    public interface ICVector {
        int Dimension { get; }
        float this[int index] { get; }
    }
}
