// Why this is useful:
// -------------------
// BitView provides a direct, zero-cost abstraction for bit-level manipulation.
// In many real-world scenarios—binary protocols, encoders/decoders, compression,
// graphics formats, hardware interfacing, and debugging—developers need precise
// control over individual bits of numeric values. Traditional C# tools either
// require manual bitmasking (verbose and error-prone) or allocate temporary
// buffers (BitArray, byte[]), which break the connection to the original value.
//
// BitView solves this by offering:
//   • A live view: changes apply directly to the underlying integer.
//   • A span-like API: intuitive indexing instead of manual bitmask math.
//   • Deterministic indexing modes: System, Reverse, and Ring.
//   • Zero allocations: no heap usage, no copying, no boxing.
//   • Full transparency: enumeration shows the real bit pattern at any moment.
//
// Why this matters:
// -----------------
// Bit-level operations are foundational in systems programming. They appear in:
//   • network packet parsing (flags, headers, protocol fields)
//   • binary serialization formats (PNG, ZIP, custom codecs)
//   • embedded systems and device communication
//   • cryptographic primitives and numeric algorithms
//   • debugging tools and visualizers
//   • performance-critical bitmask manipulation
//
// BitView allows developers to treat integers as mutable bit arrays without
// unsafe code, without temporary buffers, and without losing the connection to
// the original value. This makes low-level operations safer, faster, and easier
// to reason about.
//
//
// Key concepts shown in this example:
// -----------------------------------
//
// 1. Live bit view (no copy)
//    BitView.AsFlexSpan(ref a, ...) returns a BitIntSpan that directly references
//    the integer 'a'. Any change made through the span immediately modifies 'a'.
//
// 2. Mutable bit indexing
//    The indexer x[bit] = value writes directly into the corresponding bit of 'a'.
//    This allows treating an integer as a true bit-array without unsafe code.
//
// 3. Deterministic indexing mode
//    FlexSpanMode.System means that index 0 corresponds to the least significant
//    bit (LSB), index 1 to the next bit, and so on.
//
// 4. Enumeration of all bits
//    Iterating over the span yields each bit as a boolean. Because the span is
//    live, any mutation of 'a' is immediately reflected in the enumerator.
//
// 5. Practical usage
//    This pattern is extremely useful for debugging, binary protocols, encoders,
//    bitmask manipulation, and low-level numeric experimentation.
//
// Output explanation:
//   4
//       After setting bit 2, the integer becomes 000...0100 (decimal 4).
//
//   54476
//       We assign a new value to 'a'. The span still references the same integer,
//       so enumeration prints the bit pattern of 54476.
//
//   00110011001010110000000000000000
//       This is the 32-bit representation of 54476 in System mode (LSB → MSB).


// WARNING: Never create a BitView over a loop counter or any variable that
// controls program flow. BitIntSpan mutates the referenced integer directly,
// which means the loop variable 'i' is no longer stable.
//
// In this example:
//
//   - BitView.AsFlexSpan(ref i, ...) creates a live bit-span over 'i'
//   - DONTDO[31] = 1 sets the highest bit of 'i'
//   - This changes the loop counter itself
//
// Consequences:
//
//   • The loop index becomes corrupted
//   • The loop may skip elements, repeat elements, or terminate early
//   • The loop may jump to a huge value (bit 31 = 1 → i becomes negative)
//   • The program flow becomes undefined and extremely hard to debug
//
// Summary:
//   BitView should NEVER be used on control variables, counters, indices,
//   or anything that participates in program logic. Only use it on dedicated
//   data variables that are meant to be mutated at the bit level.
/*
 * for ( int i = 0 ; i < liste.Length ; i++ ) {
 *	var DONTDO = BitView.AsFlexSpan(ref i, FlexSpanMode.System);
 *	// Do things with DONTDO
 *	DONTDO[31] = 1;
 *	liste[i] ...
 * }
*/

// WARNING: FlexSpanMode.Ring creates a cyclic bit-span where indexing wraps
// around the view window. The enumerator for Ring mode NEVER terminates.
//
// In this example:
//
//   - DONOTTWO is created in Ring mode
//   - The enumerator's HasNext property always returns true
//   - foreach(...) enters an infinite loop
//
// Consequences:
//
//   • The loop never ends
//   • The program hangs or consumes 100% CPU
//   • No automatic break condition exists
//   • The only way out is an external termination (break, counter, cancellation)
//
// Summary:
//   Ring mode is intended for specialized low-level scenarios where infinite
//   cyclic iteration is desired. It must NEVER be used with foreach unless the
//   loop body contains an explicit termination condition.
/*
 * int n = 2;
 * var DONOTTWO = BitView.AsFlexSpan(ref a, FlexSpanMode.Ring);
 * foreach ( var it in DONOTTWO ) {
 *		Console.Write(it ? 1 : 0);
 * }
*/

using SystemEx;
using SystemEx.Utils;

int a = 0;

// Create a bit-span over the integer 'a'.
// Mode: System → forward indexing (LSB first).
var x = BitView.AsFlexSpan(ref a, FlexSpanMode.System);

// Set bit 2 to true.
// This writes directly into 'a': a |= (1 << 2).
x[2] = true;

// Prints: 4
Console.WriteLine(a);

// Assign a new value to 'a'.
// The span still references the same integer, so enumeration reflects the new bits.
a = 54476;

// Prints: 54476
Console.WriteLine(a);

// Enumerate all bits in the span.
// Each bit is printed as 0 or 1.
foreach ( var it in x ) {
	Console.Write(it ? 1 : 0);
}
Console.WriteLine();



