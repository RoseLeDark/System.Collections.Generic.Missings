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


using SystemEx.Hash;

namespace SystemEx.Collections.Model {
    
    public interface INode  {

    }

    public interface INode<T> : INode {
        public int Count { get; }
        public Optional<T> Data { get; }
       
    }
    /// <summary>
    /// node class to be used by all Trees in ths Lib
    /// </summary>
    /// <typeparam name="T">generic type for data to be stored.</typeparam>
    /// <typeparam name="TSelf"></typeparam>
    public interface INode<T, TSelf> : INode<T> where TSelf : INode<T> {
        public TSelf? GetChild ( uint index );
    }

    public interface IParentebleNode<T, TSelf> : INode<T, TSelf> where TSelf : INode<T>  {
        public TSelf Parent { get;  }
    }
}
