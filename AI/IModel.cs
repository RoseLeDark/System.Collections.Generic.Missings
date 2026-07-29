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

using System;
using System.Collections.Generic;
using System.Text;
using SystemEx.Algorithms.Compute.Interfaces;
using SystemEx.Collections.Generic;

namespace SystemEx.AI {

    public interface IModel<T, TTOOL> {
        string Name { get; }
        string SystemPrompt { get; }
        string WorkPath { get; }
        Map<string, object> Capabilities { get; }
        Map<string, object> Configuration { get; }

        IModelBackend<T, TTOOL> Backend { get; }

        bool this[string capabilities] { get; }

        bool AddConfig ( string key, object value );
        bool GetConfigValue ( string key, ref object value );

        bool HaveCap ( string strCapabilities );

        bool AddTool ( IModelTool<T> tool );
        bool RemoveTool ( string toolName );
        bool HasTool ( string toolName );


        void Begin ();

        Task<IModelResult<T>> RunAsync ( IModelPromp<T> input );

        void End ( bool wait = false );
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
