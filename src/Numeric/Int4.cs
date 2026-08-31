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

namespace SystemEx.Numeric {

	/// <summary>
	/// Represents a signed 4‑bit integer stored inside a <see cref="Fast_Byte"/>.
	/// <para>
	/// The format uses 1 sign bit (bit 3) and 3 magnitude bits (bits 0–2),
	/// providing a value range from −8 to +7. All arithmetic operations are
	/// implemented through explicit bitwise logic using temporary bits inside
	/// the underlying <see cref="Fast_Byte"/> instance.
	/// </para>
	/// </summary>
	public struct Int4 {
		private Fast_Byte m_value;


		/// <summary>
		/// Gets the smallest representable <see cref="Int4"/> value (−8).
		/// </summary>
		public static Int4 MinValue => new Int4(true, 0x08);

		/// <summary>
		/// Gets the largest representable <see cref="Int4"/> value (+7).
		/// </summary>
		public static Int4 MaxValue => new Int4(false, 0x07);

		/// <summary>
		/// Gets the number of sign bits used by the format (1).
		/// </summary>
		public static byte SignBit => 1;

		/// <summary>
		/// Gets the number of magnitude bits used by the format (3).
		/// </summary>
		public static byte ValueBits => 3;

		/// <summary>
		/// Gets the total number of bits used by the <see cref="Int4"/> format (4).
		/// </summary>
		public static byte TotalBits => 4;

		/// <summary>
		/// Gets the value zero (+0).
		/// </summary>
		public static Int4 Zero => new Int4(false, 0x00);

		/// <summary>
		/// Gets the negative zero (−0). This value is representable but not distinct
		/// in arithmetic operations.
		/// </summary>
		public static Int4 NegativeZero => new Int4(true, 0x00);

		/// <summary>
		/// Gets the value +1.
		/// </summary>
		public static Int4 One => new Int4(false, 0x01);

		/// <summary>
		/// Gets the value −1.
		/// </summary>
		public static Int4 NegativeOne => new Int4(true, 0x01);

		/// <summary>
		/// Gets the value +2.
		/// </summary>
		public static Int4 Two => new Int4(false, 0x02);

		/// <summary>
		/// Gets the value −2.
		/// </summary>
		public static Int4 NegativeTwo => new Int4(true, 0x02);

		/// <summary>
		/// Gets the value +4.
		/// </summary>
		public static Int4 Four => new Int4(false, 0x04);

		/// <summary>
		/// Gets the value −4.
		/// </summary>
		public static Int4 NegativeFour => new Int4(true, 0x04);

		/// <summary>
		/// Initializes a new <see cref="Int4"/> with all bits cleared.
		/// </summary>
		public Int4 () {
			m_value = 0;
		}
		/// <summary>
		/// Initializes a new <see cref="Int4"/> using explicit sign and magnitude bits.
		/// </summary>
		/// <param name="sign">True for negative values, false for positive.</param>
		/// <param name="value">The 3‑bit magnitude (0–7).</param>
		public Int4 ( bool sign, byte value ) {
			m_value = new Fast_Byte();

			// Signbit setzen (Bit 3)
			m_value.At(3, (byte)(sign ? 1 : 0));

			Fast_Byte _val = value;

			// Bits 0–2 setzen
			m_value.At(0, _val.Is(0));
			m_value.At(1, _val.Is(1));
			m_value.At(2, _val.Is(2));
		}

		/// <summary>
		/// Initializes a new <see cref="Int4"/> by copying another instance.
		/// </summary>
		public Int4 ( Int4 otj ) {
			m_value = otj.m_value;
		}

		/// <summary>
		/// Adds another <see cref="Int4"/> to this instance using explicit bitwise
		/// ripple‑carry addition. Temporary bits 5–7 are used for carry propagation.
		/// </summary>
		public Int4 Add ( Int4 oth ) {
			// Carry löschen
			m_value.At(5, 0);
			m_value.At(4, 0);

			for ( byte i = 0 ; i < 4 ; i++ ) {
				// Summe = a XOR b XOR carry
				m_value.At(6, (byte)(m_value.Is(i) ^ oth.m_value.Is(i) ^ m_value.Is(5)));

				// Carry = (a & b) | (carry & (a ^ b))
				m_value.At(7, (byte)(
					(m_value.Is(i) & oth.m_value.Is(i)) |
					(m_value.Is(5) & (m_value.Is(i) ^ oth.m_value.Is(i)))
				));

				// Ergebnisbit setzen
				m_value.At(i, m_value.Is(6));

				// Carry übernehmen
				m_value.At(5, m_value.Is(7));
			}

			// Temp-Bits säubern
			m_value.At(5, 0);
			m_value.At(6, 0);
			m_value.At(7, 0);

			return this;
		}
		/// <summary>
		/// Subtracts another <see cref="Int4"/> from this instance using explicit
		/// bitwise borrow‑based subtraction. Temporary bits 5–7 are used for borrow
		/// propagation.
		/// </summary>
		public Int4 Sub ( Int4 oth ) {
			// Borrow löschen
			m_value.At(5, 0);
			m_value.At(4, 0);

			for ( byte i = 0 ; i < 4 ; i++ ) {
				// Differenz = a XOR b XOR borrow
				m_value.At(6, (byte)(m_value.Is(i) ^ oth.m_value.Is(i) ^ m_value.Is(5)));

				// Neuer Borrow = (NOT a AND b) OR (NOT (a XOR b) AND borrow)
				// NOT wird durch XOR 1 erreicht
				m_value.At(7, (byte)(
					((m_value.Is(i) ^ 1) & oth.m_value.Is(i)) |
					(((m_value.Is(i) ^ oth.m_value.Is(i)) ^ 1) & m_value.Is(5))
				));

				// Ergebnisbit setzen
				m_value.At(i, m_value.Is(6));

				// Borrow übernehmen
				m_value.At(5, m_value.Is(7));
			}

			// Temp-Bits säubern

			m_value.At(5, 0);
			m_value.At(6, 0);
			m_value.At(7, 0);

			return this;
		}
		/// <summary>
		/// Multiplies this <see cref="Int4"/> by another using repeated addition.
		/// </summary>
		public Int4 Mul ( Int4 oth ) {
			// Anzahl der Wiederholungen aus oth extrahieren
			byte count = (byte)oth.m_value;

			oth = new Int4(this);

			for ( byte i = 0 ; i < count ; i++ ) {
				this.Add(oth);
			}

			return this;
		}

		/// <summary>
		/// Divides this <see cref="Int4"/> by another using repeated subtraction.
		/// The remainder is returned through the <paramref name="rest"/> parameter.
		/// </summary>
		public Int4 Div ( Int4 oth, ref Int4 rest ) {

			Int4 quotient = Zero;
			rest = new Int4(this);

			// Solange this >= oth → subtract
			while ( this.m_value >= oth.m_value ) {
				rest.Sub(oth);   // this = this - oth
				quotient++;
			}

			return quotient;
		}

		/// <inheritdoc/>
		public static bool operator == ( Int4 a, Int4 b ) {
			return a.m_value == b.m_value;
		}

		/// <inheritdoc/>
		public static bool operator != ( Int4 a, Int4 b ) {
			return !(a == b);
		}

		/// <inheritdoc/>
		public static bool operator <= ( Int4 a, Int4 b ) {
			return a.m_value <= b.m_value;
		}
		/// <inheritdoc/>
		public static bool operator >= ( Int4 a, Int4 b ) {
			return a.m_value >= b.m_value;
		}
		/// <inheritdoc/>
		public static bool operator < ( Int4 a, Int4 b ) {
			return a.m_value < b.m_value;
		}
		/// <inheritdoc/>
		public static bool operator > ( Int4 a, Int4 b ) {
			return a.m_value > b.m_value;
		}

		/// <summary>
		/// Adds two <see cref="Int4"/> values.
		/// </summary>
		public static Int4 operator + ( Int4 a, Int4 b ) => a.Add(b);

		/// <summary>
		/// Subtracts one <see cref="Int4"/> from another.
		/// </summary>
		public static Int4 operator - ( Int4 a, Int4 b ) => a.Sub(b);

		/// <summary>
		/// Decrements the value by one.
		/// </summary>
		public static Int4 operator -- ( Int4 a ) => a.Sub(One);

		/// <summary>
		/// Increments the value by one.
		/// </summary>
		public static Int4 operator ++ ( Int4 a ) => a.Add(One);

		/// <summary>
		/// Multiplies two <see cref="Int4"/> values.
		/// </summary>
		public static Int4 operator * ( Int4 a, Int4 b ) => a.Mul(b);

		/// <summary>
		/// Divides one <see cref="Int4"/> by another.
		/// </summary>
		public static Int4 operator / ( Int4 a, Int4 b ) {
			Int4 s = Zero;
			return a.Div(b, ref s);
		}

		/// <summary>
		/// Computes the remainder of a division between two <see cref="Int4"/> values.
		/// </summary>
		public static Int4 operator % ( Int4 a, Int4 b ) {
			Int4 s = Zero;
			a.Div(b, ref s);
			return s;
		}

		/// <summary>
		/// Performs a bitwise AND between two <see cref="Int4"/> values.
		/// </summary>
		public static Int4 operator & ( Int4 a, Int4 b ) {
			Int4 s = Zero;
			s.m_value.At(0, (byte)(a.m_value.Is(0) & b.m_value.Is(0)));
			s.m_value.At(1, (byte)(a.m_value.Is(1) & b.m_value.Is(1)));
			s.m_value.At(2, (byte)(a.m_value.Is(2) & b.m_value.Is(2)));
			s.m_value.At(3, (byte)(a.m_value.Is(3) & b.m_value.Is(3)));
			return s;
		}

		/// <summary>
		/// Performs a bitwise OR between two <see cref="Int4"/> values.
		/// </summary>
		public static Int4 operator | ( Int4 a, Int4 b ) {
			Int4 s = Zero;
			s.m_value.At(0, (byte)(a.m_value.Is(0) | b.m_value.Is(0)));
			s.m_value.At(1, (byte)(a.m_value.Is(1) | b.m_value.Is(1)));
			s.m_value.At(2, (byte)(a.m_value.Is(2) | b.m_value.Is(2)));
			s.m_value.At(3, (byte)(a.m_value.Is(3) | b.m_value.Is(3)));
			return s;
		}

		/// <summary>
		/// Performs a bitwise XOR between two <see cref="Int4"/> values.
		/// </summary>
		public static Int4 operator ^ ( Int4 a, Int4 b ) {
			Int4 s = Zero;
			s.m_value.At(0, (byte)(a.m_value.Is(0) ^ b.m_value.Is(0)));
			s.m_value.At(1, (byte)(a.m_value.Is(1) ^ b.m_value.Is(1)));
			s.m_value.At(2, (byte)(a.m_value.Is(2) ^ b.m_value.Is(2)));
			s.m_value.At(3, (byte)(a.m_value.Is(3) ^ b.m_value.Is(3)));
			return s;
		}

		/// <inheritdoc/>
		public override bool Equals ( object? obj ) {
			if( obj is Int4 s) 
				return this == s;
			return false; 
		}

		/// <inheritdoc/>
		public override int GetHashCode () {
			return m_value.GetHashCode();
		}
	}
}
