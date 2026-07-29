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

namespace SystemEx.AI {
    public interface IModelPromp<T> {
        public T Prompt { get; }
        public Map<string, object> Context { get; }
        public Optional<string> SessionId { get; }
        public Map<string, object> Parameters { get; }
        public Map<string, object> Tags { get; }
        public bool Cancel { get; }

        object this[string parameter] { get; set; }
    }



    /// 
    /*public sealed class FileSearchTool : IModelTool<string> {
        public Task<IModelResult<string>> PreProcessAsync ( IModel<string> model, IModelPromp<string> input ) {
            // Beispiel: KI darf nach Dateien fragen
            if ( input.Prompt.Contains("find file") ) {
                var files = Directory.GetFiles(model.WorkPath);
                //return Task.FromResult(input + "\n\nFiles:\n" + string.Join("\n", files));
            }

            //return Task.FromResult(input);
        }

        public Task<IModelResult<string>> PostProcessAsync ( IModel<string> model, IModelPromp<string> output ) {
           // return Task.FromResult(output);
        }
    }*/
}
