using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic.Missings {
    public interface ITuple {
        int Count { get; }
        [Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Bezeichner dürfen nicht mit Schlüsselwörtern übereinstimmen", Justification = "<Ausstehend>")]
        object? Get(int index);
    }

}
