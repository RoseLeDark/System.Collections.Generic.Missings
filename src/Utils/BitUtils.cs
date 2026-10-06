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

namespace SystemEx.Utils {

	/// <summary>
	/// Provides low‑level bit manipulation utilities for all primitive integer types.  
	/// Includes bit extraction, bit setting, bit flipping, bit masks, and bit rotation
	/// for signed and unsigned 8‑, 16‑, 32‑ and 64‑bit values.
	/// </summary>
	public static class BitUtils {
		
		// ---------------------------------------------------------------------
		//  MASK RANGE
		// ---------------------------------------------------------------------

		/// <summary>
		/// Creates a bit mask covering <paramref name="length"/> bits starting at
		/// <paramref name="start"/>.  
		/// Example: <c>MaskRange(4, 3)</c> → <c>0b0001110000</c>.
		/// </summary>
		public static int MaskRange(int start, int length) {
            if ( length <= 0 ) return 0;
            if ( start < 0 || start > 31 ) return 0;
            if ( length >= 32 ) return unchecked((int)0xFFFFFFFF);

            int mask = (1 << length) - 1;
            return mask << start;
        }


		 public static uint Align(uint position, uint alignment) {
      		
            uint result = position & ~(alignment - 1);
            if (result == position) {
                return result;
            }

            return result + alignment;
        }

        public static int Align(int position, int alignment) {
			//Debug.Assert(BitOperations.PopCount(alignment) == 1, "Alignment must be a power of two.");
		//	Debug.Assert(position >= 0, "Position must be non-negative.");

            int result = position & ~(alignment - 1);
            if (result == position)
            {
                return result;
            }

            return result + alignment;
        }
    }
	
}
