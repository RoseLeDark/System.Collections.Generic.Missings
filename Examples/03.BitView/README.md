# SystemEx Example 3  
## Live Bit‑Level Manipulation with BitView

This example demonstrates how `BitView` provides a live, mutable, zero‑allocation
bit‑level view over an integer.  
It shows how individual bits can be inspected, modified, and enumerated without
creating temporary buffers or losing the connection to the underlying value.

## 🧩 Overview

`BitView` exposes an integer as a span‑like sequence of bits.  
Changes made through the view immediately update the original variable.

Key features demonstrated:

- **Live bit view** — no copying, no boxing, no heap allocation  
- **Mutable indexing** — `x[2] = true` directly sets bit 2 of the integer  
- **Deterministic indexing mode** — `FlexSpanMode.System` maps index 0 to the LSB  
- **Full enumeration** — iterating over the span prints all 32 bits  
- **Zero‑cost abstraction** — operations compile down to simple bitmask logic  

This makes `BitView` ideal for systems programming, binary protocols, encoders,
compression formats, hardware interfacing, and debugging tools.

## 🔧 What the Example Does

1. Creates an integer `a = 0`.
2. Creates a `BitView` over `a` using `FlexSpanMode.System`.
3. Sets bit 2 to `true`, changing the integer from `0` to `4`.
4. Enumerates all bits and prints the 32‑bit representation.
5. Assigns a new value (`54476`) directly to `a`.
6. Enumerates again — the view still reflects the updated integer.
7. Measures execution time using `Stopwatch`.

## 📌 Program Output

```Code
SystemEx Example 3 BitView

After setting bit 2:
-------------------------------
4
00100000000000000000000000000000

After assigning 54476:
-------------------------------
54476
00110011001010110000000000000000

Elapsed Time: 3 ms
```

The bit patterns show the exact binary representation of the integer at each step.

## 🎯 Why BitView Matters

Bit‑level operations appear in many real‑world systems:

- network packet parsing (flags, headers, protocol fields)  
- binary serialization formats (PNG, ZIP, custom codecs)  
- embedded systems and device communication  
- cryptographic primitives and numeric algorithms  
- debugging tools and visualizers  
- performance‑critical bitmask manipulation  

Traditional C# approaches require:

- manual bitmasking (`value |= 1 << n`)  
- temporary `BitArray` allocations  
- unsafe pointer logic  
- byte buffers that break the link to the original value  

`BitView` solves all of this with:

- a clean, span‑like API  
- deterministic indexing modes  
- zero allocations  
- direct access to the underlying integer  

## ⚠ Important Notes

- **Never** create a `BitView` over control variables (loop counters).  
- `FlexSpanMode.Ring` produces infinite enumerations when used with `foreach`.  
- Enumeration always reflects the *current* value of the underlying integer.
