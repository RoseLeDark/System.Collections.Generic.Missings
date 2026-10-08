**SystemEx** is a modular, low‑level extension framework for .NET, developed by a single author over several months.  
It provides a large collection of missing building blocks that the BasFe Class Library never offered — lightweight containers, deterministic numeric types, threading primitives, interop helpers, hashing engines, color models, and more.

SystemEx is not a monolithic engine.  
It is a toolbox: you only use what you need, and every module stays independent.

## 🔧 What SystemEx provides

### **Low‑level collections**
- `Vector&lt;T&gt;` (std::vector‑style dynamic array)
- `Map&lt;TKey, TValue&gt;` (lightweight dictionary alternative)
- `Pair&lt;T1, T2&gt;`, `Triple&lt;T1, T2, T3&gt;`
- `Queue&lt;T&gt;`, `Deque&lt;T&gt;`
- `FixedVector&lt;T&gt;`, `Slices&lt;T&gt;`, `FlexSpan&lt;T&gt;`
- `RCUObject&lt;T&gt;` (lock‑free read, exclusive write)
- Search providers (linear, binary, clustered)
- Set/multiset/unordered set views without allocation

### **Numeric systems**
- `BigDecimal` (scientific decimal with BigInteger mantissa)
- `Ratio` (exact rational numbers)
- `Fast_Int`, `Fast_Byte`, `Fast_Short` (bit‑level types)
- Full custom floating‑point interfaces:
  - `IHalf&lt;T&gt;`, `IMini&lt;T&gt;`, `ICFloat&lt;T&gt;`, `ICDouble&lt;T&gt;`, `ICQuad&lt;T&gt;`, `IBigFloat&lt;T&gt;`
- `Uint256` and `UInt128` primitives
- Vector types: `vec2h`, `vec3h`, `vec4h`, `vec2r`, `vec3r`, `vec4r`
- `MathR` — rational math utilities (Pow, Clamp, Sqrt, conversions)

### **Hashing &amp; CRC**
- Attribute‑driven hashing (`IHashable&lt;T&gt;`, `HashFactory`)
- Bernstein, FNV‑1a, Adler, Flatscher, Ramakrishna, Weinberg
- Full CRC suite:
  - CRC‑32: IEEE, Castagnoli, Koopman, BZip2, MPEG‑2, POSIX
  - CRC‑64: ECMA, ISO, WE, XZ
- Generic dual‑engine wrapper: `CrC&lt;TC32, TC64&gt;`

### **Threading primitives**
- `LightThread` (lightweight user‑mode thread)
- `Barrier`, `Latch`, `LightLatch`
- `LightLock`, `LightMutex`, `AtomicLock`
- `Epoch` + RCU domain system
- Scoped locking utilities

### **Interop &amp; Runtime**
- Unified native module loader (`Module`)
- Platform backends (Windows, Linux, macOS)
- Delegate‑based kernel execution (`NativeRAMKernel&lt;TDelegate&gt;`)

### **Drawing &amp; Color Models**
- Full RGB/HSL/HSV/CMY/HWB/NCol models
- HDR color (`ColorHDR`)
- Named color groups (Brown, Blue, Pink, Purple, Red, Orange, Yellow, Green, Cyan, White, Grey)

### **Random engines**
- ISAAC‑32
- RandX family
- Endian‑aware random utilities

### **AI subsystem**
- `Model&lt;T, TTool&gt;`
- `IModelBackend&lt;T&gt;`
- Web‑based AI backend abstraction
- Prompt/result pipeline

## Philosophy

SystemEx focuses on:

- **Deterministic behavior**
- **Zero hidden allocations**
- **Explicit numeric semantics**
- **Small, isolated modules**
- **Engine‑oriented design**
- **Interop‑friendly architecture**

It is intentionally built without deep cross‑dependencies.  
Many structures exist in multiple forms (e.g., `Map` vs. `Vector&lt;Pair&lt;...&gt;&gt;`) to keep modules independent and predictable.

SystemEx is not meant to replace .NET — it fills the gaps for scenarios where the BCL is too heavy, too implicit, or too high‑level.

## Author note

SystemEx is developed by a single intersex author (dey/deren/dem/dem).  
The project is large — more than 140 objects across numeric, threading, collections, interop, and drawing subsystems — but still personal.  
Not every issue is visible immediately, and fixes may take time.  
Constructive feedback is welcome.