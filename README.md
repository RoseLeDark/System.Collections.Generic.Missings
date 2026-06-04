# 📘 SystemEx.Collections.Generic

A low‑level extension framework for .NET providing STL‑style iterators, traversal interfaces, random‑access iteration, advanced generic data structures, fixed‑size buffers, maps, multimaps, and a modular cache/RAID subsystem.  
Designed for deterministic, high‑performance, byte‑level applications without external dependencies.

## ✨ Overview

This library extends the .NET collection ecosystem with:

- **Iterator interfaces** inspired by C++ STL  
- **Generic algorithms** (Distance, Find, LowerBound, Reverse, Rotate, etc.)  
- **Advanced collection types** (Map, MultiMap, SortedMap, Deque, Queue, Stack, Array, FixedArray, FixedMap)  
- **Node‑based traversal structures**  
- **Deterministic, fixed‑size memory containers**  
- **Cache and RAID‑style memory layouts**  
- **Utility modules** for comparison, endian‑aware random, bit operations, and layout validation  

All components are **allocation‑controlled**, **predictable**, and **engine‑oriented**.

## 📦 Installation

### NuGet
```
dotnet add package RoseLeDark.Collections.Missings
```

### Manual
- Clone the repository  
- Add the project reference  
- Import the namespaces:

```csharp
using SystemEx.Collections.Generic;
using SystemEx.Utils;
```

## 🧩 Iterator System

The library provides a full iterator hierarchy:

- `ITerator<T>`
- `IForwardIterator<T>`
- `IBidirectionalIterator<T>`
- `IRandomAccessIterator<T>`
- Node‑based iterators for tree and list structures

These iterators support:

- `Forward()`, `Back()`, `Advance(n)`  
- `Clone()` for safe traversal  
- `Current` element access  
- `IsBegin`, `IsEnd`  
- Structural traversal via `Node`, `NodeChain`, `NodeRange`, `NodeSlice`, `GroupNode`, `StarNode`

## 🧮 Generic Algorithms

Located under `SystemEx.Utils.Iterator` and `SystemEx.Utils`:

- `Distance(first, last)`
- `Find(first, last, value, cmp)`
- `LowerBound(first, last, value, cmp)`
- `UpperBound(first, last, value, cmp)`
- `Reverse(first, last)`
- `Rotate(first, middle, last)`
- `ForEach(first, last, action)`
- `Swap`, `Fill`, `FillN`
- `Copy`, `Move`
- `Min`, `Max`, `Clamp`
- `MinElement`, `MaxElement`, `MinMaxElement`
- `LexicographicalCompare`

These algorithms behave similarly to STL equivalents, but are implemented in pure C# with zero allocations.

## 📚 Collections

### Core Types
- `Array<T>`  
- `FixedArray<T>`  
- `Deque<T>`  
- `Queue<T>`  
- `Stack<T>`  
- `BinQueue<T>`  

### Map System
- `Map<TKey, TValue>`  
- `MultiMap<TKey, TValue>`  
- `SortedMap<TKey, TValue>`  
- `FixedMap<TKey, TValue>`  
- `IMap`, `ISortedMap`, `INode`, `ITraverse`  

### Tuple & Pair Types
- `ITuple`
- `Pair<T1, T2>` : `ITuple`
- `Triple<T1, T2, T3>`  : `ITuple`
- `Quad<T1, T2, T3, T4>`  : `ITuple`
- `Tuple<T1, T2>`  : `ITuple`
- `TupleList<T>`  where T : `ITuple`

---

## 🧱 Cache & RAID Subsystem (Future Release)

- `Cache`  
- `CacheRaid`  
- `CacheRaidMirror`  
- `CacheRaidStripe`  
- `CacheRaid4`  

These provide deterministic, fixed‑size memory layouts suitable for:

- engine subsystems  
- device memory staging  
- zero‑copy pipelines  
- shared memory regions  

---

## 🛠 Utilities

Located under `SystemEx.Utils`:

- `BitUtils`  
- `Layout` (struct layout validation)  
- `Random` (endian‑aware random generation)  
- `Utils` (general helpers)  
- `CompareResult` + `CompFunc<T>`  

---

## 📁 Project Structure

```
SystemEx/
 ├─ Device/
 │    ├─ IKernel.cs
 │    ├─ RamKernel.cs
 │    ├─ Interop/
 │    │   ├─ IDeviceSharedBackend.cs
 │    │   ├─ RamKernelLoader.cs
 │    │   └─ RamSharedBackend.cs
 │    └─ Memory/
 │         ├─ DeviceBuffer.cs
 │         └─ DeviceSharedBuffer.cs
 ├─ Collections/
 │    └─ Generic/
 │         ├─ Array.cs
 │         ├─ BinQueue.cs
 │         ├─ Cache.cs
 │         ├─ Deque.cs
 │         ├─ FixedArray.cs
 │         ├─ FixedMap.cs
 │         ├─ GroupNode.cs
 │         ├─ IArray.cs
 │         ├─ IMap.cs
 │         ├─ INode.cs
 │         ├─ IPair.cs
 │         ├─ ISortedMap.cs
 │         ├─ IteratorList.cs
 │         ├─ Iterator.cs
 │         ├─ ITraverse.cs
 │         ├─ ITuple.cs
 │         ├─ Map.cs
 │         ├─ MultiMap.cs
 │         ├─ Node.cs
 │         ├─ NodeChain.cs
 │         ├─ NodeRange.cs
 │         ├─ NodeSlice.cs
 │         ├─ Pair.cs
 │         ├─ Quad.cs
 │         ├─ Queue.cs
 │         ├─ SortedMap.cs
 │         ├─ Stack.cs
 │         ├─ StarNode.cs
 │         ├─ Triple.cs
 │         ├─ Tuple.cs
 │         └─ TupleList.cs
 ├─ Utils/
 │    ├─ BitUtils.cs
 │    ├─ Layout.cs
 │    ├─ Utils.cs
 │    ├─ Random.cs
 │    └─ standart.cs
 ├─ LICENSE.md
 ├─ CHANGELOG.md
 ├─ RoseLeDark.Collections.Missings.png
 ├─ RoseLeDark.Collections.Missings.svg
 └─ README.md
```

---

## 📝 License

Licensed under the **European Union Public Licence (EUPL) v1.2**.  
See the [LICENSE.md](LICENSE.md) file for details.

## 🚧 Status

Active development.  
More iterator types, algorithms, and data structures will be added as needed.
