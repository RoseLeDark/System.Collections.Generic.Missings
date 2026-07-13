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
using SystemEx.Collections.Generic;


namespace VectorFindExample {

    public static class Programm {

        static Vector<int> _vector;

        public static void Main () {
            _vector = new Vector<int>(new int[] { 10, 29, 23, 76, 29 });
            var finder = new Find<int, Vector<int>> (ref vec);

            Console.WriteLine("Index of First 29: {0}", finder.First(29));
            Console.WriteLine("Index of Last 29: {0}", finder.Last(29));
            Console.WriteLine("Count of Last 29: {0}", finder.Of(29));
            Console.WriteLine("Exist the 30: {0}", finder.Exists(30));
        }
    }

}
