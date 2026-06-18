// SPDX-License-Identifier: EUPL-1.2

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

using System.Numerics;
using System.Runtime.CompilerServices;
using SystemEx.Utils;
using SystemEx.Collections.Generic.Interfaces;

namespace SystemEx.Collections.Generic {
    public class SortedMultiMap<T, TU> : SortedMap<T, TU> {
        public SortedMultiMap(SortFunc<T, TU> sort) : base(sort) { }
        public SortedMultiMap(IMap<T, TU> source, SortFunc<T, TU> sort) : base(source, sort) { }
        public SortedMultiMap(ICompared<IPair<T, TU>> comparer) : base(comparer) { }
        public SortedMultiMap(IEnumerable<Pair<T, TU>> elements, SortFunc<T, TU> sort) : base(elements, sort) { }

        public override void Add(Pair<T, TU> item) {
            m_elements.Add(item);
            if ( AutoSort ) Sort();
        }
    }
}
