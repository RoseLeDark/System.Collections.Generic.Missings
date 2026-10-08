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

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using SystemEx.Collections.Generic;

namespace SystemEx.Runtime.Backend.Loader {


    public interface IPreloaderBackend
    {
        public string Name { get; }
        public Version Version { get; }

        public bool UseSignatureCheak { get; }

        public string Destination { get; }

        public bool UseTempFS { get; }

        public bool CanUpdate { get; }

        public IPreloaderBackendConfig Config { get; } // in jeden Backend muss es eine SystemEx.Module.cfg geben in der die Namen aufgelöst sind und Hashs stehen


        public bool DownLoadModule(ref string destionation, Platform platform, Architecture arch);


    }


}