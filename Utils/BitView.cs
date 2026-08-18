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


namespace SystemEx.Utils {
    public static class BitView {

        public static BitIntSpan AsFlexSpan ( ref int value, FlexSpanMode mode, short start = 0, short send = 32 ) {
            return new BitIntSpan(ref value, start, send, mode);
        }
        public static BitUIntSpan AsFlexSpan ( ref uint value, FlexSpanMode mode, short start = 0, short send = 32 ) {
            return new BitUIntSpan(ref value, start, send, mode);
        }

        public static BitLongSpan AsFlexSpan ( ref long value, FlexSpanMode mode, short start = 0, short send = 64 ) {
            return new BitLongSpan(ref value, start, send, mode);
        }
        public static BitULongSpan AsFlexSpan ( ref ulong value, FlexSpanMode mode, short start = 0, short send = 64 ) {
            return new BitULongSpan(ref value, start, send, mode);
        }
    }
}
