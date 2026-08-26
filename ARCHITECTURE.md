# 🏛️ SystemEx Architecture Overview

SystemEx is a modular, deterministic low‑level framework for .NET.  
Its architecture is intentionally divided into isolated subsystems to ensure stability, clarity, and predictable behavior.

## 🧱 Core Principles

- Deterministic behavior  
- Zero hidden allocations  
- Explicit numeric semantics  
- STL‑style algorithms and iterators  
- Modular subsystems with minimal cross‑dependencies  
- Engine‑oriented design (numeric, threading, random, interop)  
- Debug‑friendly structure  

## 🧩 Module Overview

### SystemEx (Base Layer)
Foundational primitives: `Buffer`, `FlexSpan`, `Math`, ranges, base tuples.

### SystemEx.Collections.Generic
High‑performance containers, node‑based traversal.

### SystemEx.Collections.Model
Higher‑level abstractions built on generic collections.

### SystemEx.Algorithms
STL‑style algorithms operating on SystemEx iterators.

### SystemEx.Drawing
HDR color models, HSV, RGB, CMY, XYZ, YUV, NCol, light models, canvas utilities.

### SystemEx.Hash
Hashing utilities: Bernstein, HashFactory, HashAttribute.

### SystemEx.IO
Deterministic IO streams: `CacheStream`, `WriteStream`.

### SystemEx.Numeric
Custom numeric types (Half16, Half16b, Ratio, Fast_Int, Fast_Byte), vectors, matrices, AxisAngle, projections.

### SystemEx.Random
ISAAC engine, Randx, SeedMixer, HashedSeed.

### SystemEx.Runtime
Cross‑platform dynamic loading and interop utilities.

### SystemEx.Threading
Lightweight threading primitives: EventGroup, LightTask, spinlocks.

### SystemEx.Utils / Utils.Bits
General utilities, bit operations, layout validation, random helpers.


## 🧱 Design Choice: Controlled Duplication

Some structures exist in multiple forms (e.g., `Map<TKey, TValue>` using `Pair<TKey, TValue>[]` instead of `Vector<Pair<...>>`).  
This is intentional:

- Prevents deep dependency chains  
- Keeps subsystems isolated  
- Makes debugging easier  
- Allows safe experimentation without affecting other modules  
- Reduces risk of regressions in a large generic codebase  

## Build Flags and Development Channels

SystemEx uses build‑flags to separate stable functionality from experimental
work. The primary flag is:

### `USE_DEVBUILD_UNSTABLE`
This flag enables experimental subsystems, unfinished modules, and active
development code. These blocks are wrapped in:

```csharp
#if USE_DEVBUILD_UNSTABLE
    // experimental code
#endif
```

The purpose of this flag is architectural isolation:

- Clean repository: unstable code does not pollute stable modules
- Safe iteration: new ideas can be tested without affecting existing APIs
- Debuggability: experimental code paths remain invisible in normal builds
- Controlled exposure: contributors must opt‑in to unstable features
- Reduced regression risk: stable modules remain unaffected by ongoing work

Build‑flag isolation ensures that
new subsystems can be developed safely without destabilizing the stable
architecture.

Development Channels
SystemEx conceptually distinguishes two build channels:

- Stable Build (default)  
Contains only production‑ready modules. No experimental code is compiled.

- Unstable Devbuild (USE_DEVBUILD_UNSTABLE)  
Contains experimental features, prototypes, debugging utilities, and
unfinished subsystems. Intended for contributors and advanced users.

This separation keeps the architecture predictable, maintainable, and safe for
long‑term evolution.

## 🧩 Maintainer Note

SystemEx is developed by a single author (pronouns: dey/deren/dem/dem).  
The architecture reflects the need for clarity, stability, and maintainability in a solo‑maintained project.

Contributions and technical discussions are welcome.
