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


namespace SystemEx.AI {

    /// <summary>
    /// Converts the <see cref="IModelTool{T}"/> definitions into a compatible Object for the usend Backend
    /// 
    /// This factory is used by the backend to expose tools to the AI model.
    /// </summary>
    /// <typeparam name="T">
    /// The prompt type used by the model.
    /// </typeparam>
    /// <typeparam name="TAITOOL">
    /// 
    /// </typeparam>
    public interface IAIFunctionFactory<T, TAITOOL> {
        
        bool Convert ( IModelTool<T> tool, out TAITOOL converted );
    }
}
