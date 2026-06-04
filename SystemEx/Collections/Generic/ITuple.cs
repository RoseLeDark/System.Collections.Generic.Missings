using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Collection.Generic {
    public interface ITuple {
        int Count { get; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Bezeichner dürfen nicht mit Schlüsselwörtern übereinstimmen", Justification = "<Ausstehend>")]
        object? Get(int index);
    }

}
