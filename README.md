# 📘 SystemEx Framework

SystemEx is a modular, low‑level extension framework for .NET providing
deterministic numeric types, high‑performance collections, STL‑style
algorithms, advanced color models, random engines, interop utilities,
threading primitives, and bit‑level tools.

It is designed for engine development, simulation, rendering pipelines,
compiler tooling, and any environment requiring predictable, allocation‑aware,
byte‑level control.

> **Design Note**
>
> SystemEx is intentionally structured to avoid deep cross‑dependencies between
> modules. In several areas — especially within the collection and numeric
> subsystems — certain data structures appear in multiple forms or are implemented
> more than once (for example, `Map<TKey, TValue>` using `Pair<TKey, TValue>[]`
> instead of reusing `Vector<Pair<...>>`).  
> 
> This duplication is deliberate. SystemEx is a large, highly generic low‑level
> framework, and isolating components prevents errors or design changes in one
> subsystem from propagating unpredictably into others. This approach keeps the
> library stable, easier to debug, and safer to extend, especially when working
> with complex iterator models, custom numeric types, and engine‑oriented memory
> layouts.
>
> SystemEx is a personal hobby project developed by a single intersex author; my pronouns 
> are dey/deren/dem/dem. While I aim for correctness, determinism, and clean architecture, 
> not every issue may be visible immediately, and responses or fixes may occasionally
> be delayed. Constructive feedback, improvement ideas, and technical discussions 
> are always welcome.

## ✨ Core Philosophy

SystemEx follows these principles:

- **Deterministic behavior**  
- **Zero‑magic, zero‑hidden‑allocation APIs**  
- **Explicit numeric semantics** (Half16, Ratio, Fast_Int, Fast_Byte)  
- **STL‑inspired algorithms and iterators**  
- **Modular namespaces** for clean separation  
- **Engine‑oriented design** (Random, Threading, Runtime, Drawing)  
- **Interop‑friendly** (Linux, macOS, Windows dynamic loading)  

# 🧩 Namespaces Overview

SystemEx is divided into clear, purpose‑built modules.

## 🧱 SystemEx (Base Layer)

Core primitives and utilities:

- `Buffer`, `FlexSpan`
- `IComparableEx`, `IRange`
- `Math`, `NumberRange`, `NumberRangeIterator`, `NumberRangeStepper`
- Base tuple types (`Triple`, etc.)

This layer provides the foundation for all other modules.

## 📚 SystemEx.Collections.Generic

High‑performance, STL‑style collections:

- `Vector<T>`, `FixedVector<T>`
- `Deque<T>`, `Queue<T>`, `Stack<T>`
- `BinQueue<T>`, `PriorityQueue<T>`
- `Set<T>`, `MultiSet<T>`, `UnorderedSet<T>`
- `Map<TKey, TValue>`, `MultiMap`, `SortedMap`, `FixedMap`
- `Tuple`, `Pair`, `Triple`, `Quad`, `TupleList<T>`
- Node‑based traversal: `Node`, `NodeChain`, `NodeRange`, `NodeSlice`, `GroupNode`, `StarNode`

All containers support deterministic memory behavior and iterator‑based traversal.

## 🧬 SystemEx.Collections.Model

Higher‑level collection models and abstractions built on top of the generic layer.

## 🧮 SystemEx.Algorithms

STL‑style algorithms:

- `Distance`, `Find`, `LowerBound`, `UpperBound`
- `Reverse`, `Rotate`, `Copy`, `Move`
- `Min`, `Max`, `Clamp`, `MinMaxElement`
- `LexicographicalCompare`
- `ForEach`, `Fill`, `FillN`, `Swap`

All algorithms operate on SystemEx iterators and avoid allocations.

## 🎨 SystemEx.Drawing

Advanced color and light models:

- `ColorR8G8B8`, `ColorR10G10B10`, `ColorR16G16B16`
- `ColorHSV`, `ColorHDR` (HSV‑based HDR with extended Value range)
- `CMY`, `XYZ`, `YUV`, `NCol`
- `Light` models
- `Canvas` rendering utilities

Designed for deterministic color conversion, HDR workflows, and engine pipelines.

## 🔐 SystemEx.Hash

Hashing utilities:

- `Bernstein`
- `HashFactory`
- `HashAttribute`

Useful for hashing structures, identifiers, and engine metadata.

## 📁 SystemEx.IO

Low‑level IO utilities:

- `CacheStream`
- `WriteStream`

Optimized for deterministic, buffered IO operations.

## 🔢 SystemEx.Numeric

A complete numeric subsystem:

### Scalar Types
- `Fast_Byte`, `Fast_Int`
- `IFloat` abstraction
- `Half16`, `Half16b` (custom IEEE‑754‑style half precision)
- `Ratio` (exact rational type)

### Vector Types
- `vec2/3/4i`
- `vec2/3/4d`
- `vec2/3/4f`
- `vec2/3/4h` (Half16)
- `vec2/3/4hb` (Half16b)
- `vec2/3/4r` (Ratio)

### Matrix Types
- `m44f`, `m44d` (not yet public - under working)

### Other
- `AxisAngle`
- `Projection`
- `DQuadv`

All numeric types are deterministic and engine‑oriented.

## 🎲 SystemEx.Random

High‑quality random engines:

- `IsaacEngine` (ISAAC32)
- `SeedMixer`
- `HashedSeed`

Suitable for simulation, procedural generation, and cryptographic‑adjacent tasks.

## ⚙️ SystemEx.Runtime

Cross‑platform dynamic interop:

- `InteropService`
- `GetProcAddress`‑style dynamic loading
- Linux, macOS, Windows support
- Cache‑based backend for fast symbol lookup

Ideal for plugin systems, native bindings, and runtime extension.

## 🧵 SystemEx.Threading

Lightweight threading primitives:

- `EventGroup` (bit‑level event flags, FreeRTOS‑style)
- `LightConditionalVariable`
- `LightCountingSpinlock` (pure user‑space atomic)
- `LightTask` (EventGroup‑based task state machine)
- OS‑level spinlock wrappers

Designed for engine loops, schedulers, and lock‑free systems.

## 🧰 SystemEx.Utils

General utilities:

- `FlexSpan`
- `Random` helpers
- `Conversion`
- `Layout` (struct layout validation)
- `BitUtils`
- `Utils`

## 🧰 SystemEx.Utils.Bits

Bit‑level operations and helpers.

# 📦 Installation

### NuGet
$ dotnet add package RoseLeDark.Collections.Missings 

### Manual
- Clone the repository  
- Add the project reference  
- Import the namespaces you need:

```csharp
using SystemEx;
using SystemEx.Numeric;
using SystemEx.Collections.Generic;
using SystemEx.Utils;

```

## 🚧 Status
Active development.
New numeric types, threading primitives, and interop modules are added as needed.

## 📝 License
Licensed under the European Union Public Licence (EUPL) v1.2.
See the LICENSE.md file for details.