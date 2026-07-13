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
using SystemEx.Algorythmen;
using SystemEx.Collections.Generic;


namespace VectorSetExample {

    public static class Programm {

        static Vector<int> _vector;

        public static void Main () {
            _vector = new Vector<int>(new int[] { 10, 29, 23, 76, 29 });
            var _set = new MultiSet<int, Vector<int>> (ref _vector, new Less<int>() );

            Print(_vector);
            Vector<int> _extrated = new Vector<int>(_set.Extract(2));
            Print(_vector);
            Print(_extrated);
        }

        public static void Print( Vector<int> p) {
            Console.Write("Elements art: { ");
            foreach ( var item in p ) {
                System.Console.Write(" {0} ", item);
            }
            Console.WriteLine("}");
        }
    }

}
