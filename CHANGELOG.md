# Changelog
## Planned to Version 1.0
### Add
- OpenCL Kerneal Call with my  SystemEx.Device System SystemEx 2.0

---
## [0.50.00] - 03.07.2026 @Lacking
### Major Documentation Milestone
A significant version jump was made intentionally.  
With this release, the entire public API of SystemEx — including the mathematical canvas
system, layer model, blend modes, hash engines, and all related interfaces — has been fully
documented and formalized.

This marks the completion of approximately half of the planned major‑release scope.
The version number reflects this structural maturity rather than incremental feature changes.

### Added
- Full XML documentation for all canvas interfaces (`ICanvas`, `ISubCanvas`, `ICanvasList`)
- Complete documentation for all hash engines (Groestl, Adler, etc.)
- Formalized mathematical layer model and blend mode semantics
- Unified terminology across the entire SystemEx framework

### Changed
- API surface stabilized and prepared for 1.0
- Internal consistency improvements across all canvas and hash components

### Notes
The jump from `0.12.04` to `0.50.00` is intentional and represents reaching the halfway point
toward the 1.0 major release.

### Codename Update: Ignoring → Lacking
The codename for the 0.x development cycle has been updated from **Ignoring** to **Lacking**.

All SystemEx development codenames follow the theme of “Missing”, reflecting the project’s
purpose of providing features, structures, and utilities that are missing in the .NET ecosystem.

- **Ignoring** represented the early phase where documentation, structure, and formal API
  descriptions were intentionally ignored while core functionality was being built.

- **Lacking** represents the next phase: the system is functionally complete, but still lacking
  documentation, clarity, and formal definitions.

With version **0.50.00**, the documentation milestone has been reached, marking the end of the
“Lacking” phase. All subsequent 0.50.x versions will continue under this codename until the
remaining missing structural components are completed.


---

## [0.12.04] – 2026‑07‑01 @Ignoring

### Added
`System.FlexSpan<T>` — a unified view type for array‑based memory.**  
FlexSpan implements three indexing modes:  
- `SystemSpan` (forward indexing)  
- `ReverseSpan` (reverse indexing)  
- `RingSpan` (circular indexing)

FlexSpan operates directly on **raw `T[]` buffers** without allocation and provides ref‑return access to elements.  
It is now integrated across all **SystemEx.Collections** types that use array‑backed storage (Stack, Array, FixedArray, Cache, StrippedCache).  

Map‑based collections are not yet supported because they use `List<T>` internally.  
A dedicated **FlexListSpan** type will be added later to enable FlexSpan‑style views over list‑backed containers.


Wenn du willst, kann ich dir auch eine **längere Version** für Dokumentation oder eine **ultra‑kurze Version** für Git‑Commit‑Messages erstellen.

- Introduced the new node system under `SystemEx.Model`, intended to replace the legacy node types in `SystemEx.Collections.Generic`.
  

- Added new sorting algorithms under `SystemEx.Utils.Algorithm`:
  - `QuickSort` – Hoare‑partition, tail‑recursion‑eliminated variant.  
  - `HeapSort` – Deterministic down‑heap implementation.  
  - `InsertionSort` – Optimized for small spans and rope chunks.  
  - `IsSorted` – Predicate‑based sortedness check.
  - `GrøstlHash` - Implements the Grøstl-256 and Grøstl-512 hashing functions. This class provides a
    fully self-contained, allocation-free, deterministic hash engine based
    on the original Grøstl specification (SHA-3 finalist).

- Added new tree structures under `SystemEx.Model.Tree`:
  - `RBTree` – Intrusive red‑black tree with rotation and validation hooks.  
  - `RBTreeNode` – Node type supporting color, parent, and directional links.  
  - `RBTreeIterator` – Deterministic iterator for intrusive tree traversal.
  - `GenericNode` – Base node type acting solely as a value holder.  
  - `GroupedNode` – Non‑intrusive grouped node storing a value and an expandable collection of associated `GenericNode<T>` instances.  
  - `LinkedNode` – Intrusive doubly‑linked node with forward and backward links.  
  - `LinkedNodeSlice` – Represents a slice over intrusive node structures.  
  - `LinkedNodeWithSibling` – Doubly‑linked node supporting sibling relationships.  
  - `LinkedNodeRange` – Iterator‑based range over intrusive node structures.  
  - `Node` – Intrusive singly‑linked node storing a value and a single forward link.

- Added new random number generators under `SystemEx.Utils.Random`:
  - `RandX` – Fast non‑cryptographic generator family.  
  - `ISAAC` – Full ISAAC implementation with 256‑entry state and golden‑ratio seeding.  
  - `RandUtils` – Unified API for endian‑aware random generation.

- Added new hash suite under `SystemEx.Utils.Hash`:  
  - `FNV1aHash` Classic fast hash; excellent for small keys and general‑purpose hashing.
  - `WeinbergHash` Lightweight integer hash with good avalanche behavior for map/set keys.
  - `RamakrishnaHash` Simple multiplicative hash; ideal for embedded or deterministic systems. 
  - `BernsteinHash` Known as DJB2; stable, predictable, widely used in string hashing. 
  - `AdlerHash`  Fast checksum‑style hash; useful for quick integrity checks.
  - `FlatscherHash` High‑diffusion integer hash; suited for randomized indexing and table scattering.
### Planned
- Poly1305 – lightweight, high‑security MAC/hash based on simple arithmetic operations; easy to implement without external dependencies.
- HighwayHash – fast keyed hash with strong avalanche behavior; ideal for secure map/set hashing and low‑collision key generation.
- BLAKE2s – modern cryptographic hash with high performance and simpler structure than BLAKE3; suitable for general crypto hashing without external libraries.

### Fixed
- Corrected an issue in the array constructor.

---

## [0.10.15] 30.06.2026

### Added
- Added `long‑based iterator navigation`:
  - `NodeIterrator<T>.Advance(long offset)`
  - `NodeIterrator<T>.AdvanceRest` (long)
  - `NodeIterrator<T>(Node<T> current, long index)`
- Added `long‑based node navigation`:
  - `Node<T>.GetAt(long index, out long r)`
  - `Node<T>.At(long index)`
  - `Node<T>.Offset(long offset)`
  - `Node<T>.SetAt(long index, T value)`
- Added `Distance(bool ToEnd)` overload for directional chain length measurement

### Changed
- Replaced old `m_pChilds` Prev/Next sentinel‑model with `nullable Prev/Next` via:
  - `FixedArray<Node<T>?> m_pNodex`
  - `Prev` / `Next` now nullable
  - `HasPrev` / `HasNext` now check for `null`
- Updated chain navigation:
  - `Root()` and `Last()` rewritten to use nullable Prev/Next
- Updated removal logic:
  - New `Remove()` implementation with explicit handling of:
    - isolated node  
    - head node  
    - tail node  
    - middle node  
  - Optional isolation under `#if TRACE`
- Updated splice logic:
  - `Splice(ref first, ref last)` now detaches current node and reinserts `[first..last]`
  - `this.Prev = null` and `this.Next = null` after splice
- Updated insertion logic:
  - `InsertLast(Node<T>)` rewritten to use nullable Prev/Next
- Updated traversal routines:
  - `TraversPreorder` / `TraversPostorder` now traverse:
    - `m_pNodex[PREV]`
    - `m_pNodex[NEXT]`
    - all siblings via `foreach`

### Deprecated
- Marked `InsertRagen(ref Node<T>, ref Node<T>)` as obsolete  
  → replaced by `InsertRange(ref Node<T>, ref Node<T>)`

### Fixed
- Correct remainder handling in `GetAt(long index, out long r)`
- Correct Prev/Next rewiring in `Remove()` for all four structural cases
- Correct traversal behavior in `TraversListForward` / `TraversListBackward`
- Correct iterator equality and hash code behavior

---

## [0.10.12] 29.06.206

### Very Important
- `Renamed `RamKernel` → `NativeRAMKernel<TDelegate>``  
  New unified kernel base class replacing the old RAM‑only implementation.  
  See example: `Examples/ExampleRamKernelAdd.cs`.

- `Removed all legacy platform kernel loaders`  
  (`WindowsKernelLoader`, `LinuxKernelLoader`, `MacKernelLoader`, `NoSupportKernelLoader`)  
  Replaced by new module‑centric loader architecture.

### Added
- Introduced full native module system under `SystemEx.Runtime`:
  - `Module` (DLL/SO/DYLIB abstraction with handle + function lookup)
  - `NativeHost` (module cache, unload, delegate binding)

- Add platform loader backends under `SystemEx.Runtime.InteropServices.Platform`:
  - `WindowsProcLoader`
  - `LinuxProcLoader`
  - `MacProcLoader`
  - `NoSupportProcLoader` (simulated backend)

- Add new kernel base class `NativeRAMKernel<TDelegate>`:
  - Backend‑neutral kernel lifecycle (`Create`, `BeginRun`, `Run`, `EndRun`)
  - Automatic buffer lock/unlock
  - Delegate‑based native function invocation
  - Backend hooks (`OnCreate`, `OnBegin`, `OnRun`, `OnEnd`, `OnAddBuffer`)

- Add example kernel:  
  - `ExampleRamKernelAdd` demonstrating the new API.
- Add `Bernsteinhash` and `MurmurHash`, murmur with Little and Big Endian Support 
- Update `Examples\ExampleHasher.cs` 
- Add to Iterator:
    - IRandomAccessIterator<T> Advance<T> ( IRandomAccessIterator<T> first, int n )
    - IRandomAccessIterator<T> Next<T> ( IRandomAccessIterator<T> it, int n )
    - IRandomAccessIterator<T> Prev<T> ( IRandomAccessIterator<T> it, int n )
    - IForwardIterator<T> Next<T> ( IForwardIterator<T> first, int n ) 
    - IForwardIterator<T> Advance<T> ( IForwardIterator<T> first, int n )
    - public static bool Empty<T>(IIterator<T> first, IIterator<T> last) (Last and end is the same
    
### Changed
- Replaced unmanaged call pipeline:
  - Old: `KernelLoader.call(...)`
  - New: delegate binding via `Marshal.GetDelegateForFunctionPointer<T>()`

- Updated RAM backend to use unified module + loader system.

- Updated namespaces:
  - `SystemEx.Device.Intertropt` → `SystemEx.Device.Interop`
  - New runtime loaders under `SystemEx.Runtime.InteropServices.Platform`

- Updated buffer lifecycle to match new kernel execution model.

### Improved
- Unified module loading across Windows/Linux/macOS.
- Clear separation between:
  - module loading
  - function resolution
  - kernel lifecycle
  - buffer lifecycle
- More consistent behavior with future GPU backends.
- Cleaner example code and documentation.

### Removed
- `RamKernel`
- All `KernelLoader` platform variants
- Old Begin/Run/End RAM kernel implementation
- Old unmanaged call logic

### Notes
- This update contains `breaking changes`. All code using `RamKernel` or `KernelLoader` must migrate to `NativeRAMKernel<TDelegate>`.

---

## [0.9.64] 23.06.2026

### Changed
- `public int Length { get; }` → `public ulong Length { get; }`
- `Cache<T>` is now fully ulong‑addressed instead of int‑based
- Renamed `ITerator` → `IIterator` (updated across all dependent iterator‑based classes)

### Added
- Added `Free` and `Used` properties to `Cache<T>` with new internal member `m_maxUsedAddress`  
- Added `Clear()` implementation to reset cache content and usage state  
- Added `Range`, `NumberRange` and forward iterator `NumberRangeIterator<T>`  
- Added `NumberRangeStepper<T>`: cursor‑based stepper over normalized numeric ranges (fixed increments, forward/backward stepping, reset, enumeration)  
- Added `TypeBuffer<T>` and `ITypeBuffer<T>` to `SystemEx.Collections.Generic` A typed view over a raw <c>Cache</c> that exposes elements of an unmanaged type <typeparamref name="T"/>

---

## [0.9.6] 21.06.2026

### Added
- Introduced new HWB and NCol color models under `SystemEx.Drawing`:
  - Added `ColorHWB` with hue/whiteness/blackness representation
  - Added `ColorNCol` hue‑index color model with percent‑based hue interpolation
  - Added full conversion pipeline:
    - `ColorR8G8B8 → ColorHWB`
    - `ColorHWB → ColorR8G8B8`
    - `ColorR8G8B8 → ColorNCol`
    - `ColorNCol → ColorR8G8B8`
    - `ColorNCol → ColorR16G16B16`
    - `ColorNCol → ColorR10G10B10`

- Added new canvas abstraction:
  - Introduced `ICanvas<T>` interface
  - Added support for region copy, fill, clear, resize and color search

- Added new byte‑serialization infrastructure:
  - Added `ByteSeriablizeProvider`
  - Added `IByteSerialize`, `IHasByteSchema`
  - Added `RawByteProvider`
  - Added `ColorR10G10B10FormatSchema` and serializer implementation

- Added new constructors to `Cache`:
  - `Cache(byte[], CacheType)`
  - `Cache(Array<byte>, CacheType)`
  - Added `ToArrayEx()` for direct `Array<byte>` access

### Changed
- Updated `Map` and `IMap`:
  - Added `Add(key, value)` overload
  - Added `Remove(key)`
  - Added `TryGeValue(key, out value)`
  - Added `Keys` and `Values` collections
  - Extended `Map<T,TU>` to implement `IReadOnlyMap<T,TU>`

- Updated color classes:
  - Fixed namespace typo in `ColorHSL`
  - Corrected constructor names in `ColorCMY`
  - Replaced `Math.Clamp` with `System.Math.Clamp` for consistency
  - Added float‑based arithmetic helpers to `ColorHSV`

- Updated `ColorConverter`:
  - Added HWB and NCol conversion logic
  - Improved sRGB conversion accuracy
  - Added additional RGB16/RGB10 conversion helpers

- Renamed `Utils/Utils.cs` → `Utils/Conversion.cs`

### Improved`
- More consistent color conversion pipeline across all color models
- Unified clamping and normalization behavior in HSV/HSL/CMY models
- Improved documentation and XML comments across multiple files
- Cleaner separation between read‑only and mutable map interfaces

### Fixed
- Fixed incorrect index usage in `ColorCMY(float[] x)`
- Fixed namespace mismatch in `ColorHSL`
- Fixed missing `System.` prefix for `Math.Clamp` in HSV operations
- Fixed minor documentation errors and typos across drawing and collection modules
- Removed unused `m_currentCache` field from `StrippedCache`

---

## [0.9.5] 18.06.2026

### Very Important 
- Namespace: `SystemEx.Collection.Generic` To `SystemEx.Collections.Generic`
### Add
- Add class `Cluster<T>` at `SystemEx.Collections.Generic` Represents a cluster node in a weighted graph structure.
- Add class `CacheStream<TCache>` at `SystemEx.IO`A Stream wrapper around a `Cache<T>` instance.  
- Add Color Classes to `SystemEx.Drawing` - ColorHSL, ColorHSV, ColorRGB, ColorGray, ... Start with build in next
  Version are ready
### Docu
- The Doku in `SystemEx.Collections.Generic`, `SystemEx.IO`, `SysrtemEx` and `SysrtemEx.Utils` is ready

---

## [0.8.5] 04.06.2026

### Very Important
- `Major namespace restructuring` to reflect the engine architecture and clarify responsibilities across collections, device memory, and interop layers.
- This release contains `breaking changes`: update all `using`/imports to the new namespaces.

### Changed
  - Moved device memory types (`DeviceBuffer`, `DeviceSharedBuffer<TDeviceSharedBackend>`) to ``SystemEx.Device.Memory``.
  - Moved native interop and backend implementations (`UnmanagedObject`, `RamSharedBackend`, `IDeviceSharedBackend`, platform kernel loaders) to ``SystemEx.Device.Interop``.
  - Kept kernel and execution interfaces (`IKernel<TBackend>`, `RamKernel`, kernel lifecycle orchestration) under ``SystemEx.Device``.
- Removed legacy `SystemEx.Device.Memory.Missings` layout and deprecated `System.Memory.Missings` placements; types have been relocated to the new namespaces above.
- Updated internal references and XML docs to reflect new namespace locations.

### Added
- Migration guidance notes and quick reference mapping for common types:
  - `SystemEx.Device.Memory.Missings.RamSharedBackend` → `SystemEx.Device.Interop.RamSharedBackend`
  - `SystemEx.Device.Memory.DeviceSharedBuffer` → `SystemEx.Device.Memory.DeviceSharedBuffer`
  - `SystemEx.Collections.Generic.SharedCache` → `SystemEx.Collections.Generic.Cache` (and related cache types)
  
- Add a Map for ITuples `SystemEx.Collections.Generic.TupleMap`, `SystemEx.Collections.Generic.MultiTupleMap` and `SystemEx.Collections.Generic.SortedTupleMap` 
- Add `SystemEx.Collections.Generic.StrippedCache` (segmented virtual cache over N sub‑caches; global addressing, byte‑wise distribution).
- Added full support for:
    - WriteRange(ulong position, byte[] data)
    - WriteRange(ulong start, ulong end, byte[] data)
    - ToArray(int index) for exporting individual cache segments.
    - Added unsigned‑safe global addressing logic for multi‑cache memory systems.
    - Added overflow‑safe Seek logic compatible with segmented caches.

- Add `SystemEx.Collection.Generic.MirroredCache` (dual‑cache pair; writes to A and mirrored A‑reverse; consistent read/write symmetry).
    
### Improved
- Clear separation of concerns between collection‑level caches and device‑level memory/backends.
- Improved discoverability and consistency for public APIs across Collections, Device, and Interop subsystems.
- Simplified developer mental model for where to place new types: Collections for logical cache/data structures; Device.Memory for managed device memory abstractions; Device.Interop for native/backends.
- Improved Seek implementation to be fully ulong‑safe and handle negative offsets without overflow.
- Improved internal consistency between Cache and StrippedCache semantics (global vs. local addressing).
- Improved index validation for segmented cache exports (ToArray(index)).
- 
### Fixed
- Resolved ambiguous type collisions caused by previous overlapping namespaces.
- Fixed incorrect index comparison in ToArray(int index) (unsigned‑correct boundary check).
- Fixed potential overflow in SeekOrigin.Current and SeekOrigin.End when negative offsets were cast to ulong.
- Fixed incorrect cast from ulong to long in cache index validation.
### Notes / Migration
- `Breaking change:` update all `using` directives and project references to the new namespaces.  
  Example mappings:
  - `using SystemEx.Device.Memory.Missings;` → `using SystemEx.Device.Memory;`
  - `using SystemEx.Collection.Generic;` (old cache location) → `using SystemEx.Collections.Generic;`
  - `using SystemEx.Device.Intertropt;` → `using SystemEx.Device.Interop;`
- Search your codebase for the old namespace tokens (`Missings`, `Intertropt`, `System.Memory.Missings`) and replace with the new targets.

---

## [0.8.1] 04.06.2026

### Added
- Introduced full RAM backend under `System.Memory.Missings`:
  - Added `RamSharedBackend` implementing `IDeviceSharedBackend`
  - Added `RamUnmanagedObject` for unmanaged buffer handling:
    - Stores pinned GCHandle
    - Exposes native pointer via `IntPtr`
    - Holds buffer size
    - Deterministic disposal for pin/unpin lifecycle
  - Added RAM‑based hardware buffer creation:
    - `CreateWriteHardwareBuffer(byte[], out object)`
    - `CreateReadHardwareBuffer(int, out object)`
    - `CloseHardwareBuffer(ref object)`
    - `ReciveFromHardwareBuffer(out byte[], ref object)`

- Added `RamKernel`:
  - Supports multi‑buffer binding via `AddBuffer`
  - Executes native DLL kernels via `ExecuteKernel(IntPtr[], int[], int)`
  - Collects pointers and sizes from all attached RAM buffers
  - Fully compatible mit Begin/Run/End‑Flow der bestehenden Kernel‑API

### Changed
- Moved `SharedCache` / `DeviceSharedCache` aus `System.Collections.Generic.Missings`
  nach `System.Memory.Missings` und in `DeviceSharedBuffer` überführt
- Replaced previous MemoryHandle‑based RAM pinning with explicit GCHandle pinning
- Unified hardware buffer representation:
  - `DeviceSharedBuffer` now always exposes a `RamUnmanagedObject` for RAM backends
- Simplified RAM buffer lifecycle:
  - Pinning occurs exactly once during Begin()
  - Unpinning handled exclusively through `RamUnmanagedObject.Dispose()`

### Improved
- Clear separation between managed cache (`DeviceBuffer`) and unmanaged hardware buffer
- Consistent pointer and size access for all RAM kernel operations
- Improved Begin/End semantics:
  - Begin() creates pinned unmanaged buffer
  - End() copies data back depending on `SharedCacheType`
- More consistent behavior with GPU backends through unified kernel interface

### Fixed
- Fixed missing unpin in `ReciveFromHardwareBuffer`
- Fixed invalid hardware buffer state after End()
- Fixed potential GCHandle leaks during repeated Begin/End cycles
- Fixed incorrect buffer size propagation to native kernel calls

---

## [0.7.0] 03.06.2026

### Added
- Introduced `RandPasswordLevel` enum (`Simple = 16`, `Strong = 32`)
- Added `RandPassword(int length, FixedArray<char> allowed, Endian endian)`:
  - Zero‑allocation inner loop
  - Endian‑aware random character generation
  - Filtering via `FixedArray<char>.TryGet`
- Added `string.Rand(RandPasswordLevel level, Endian endian)` extension:
  - Uses predefined password character sets
  - Supports Simple and Strong password generation
- Added `StrongPasswordChars` set with safe, universal symbols

### Improved
- `RandChar` now used with `(char)0` to `(char)short.MaxValue` for stable UTF‑16 safe range
- Unified password generation logic across all levels

### Notes
- Password generation is fully deterministic per Endian mode
- No allocations inside the generation loop

---

## [0.6.5]

### Added
- Introduced `SharedCache` for hardware‑shared memory operations:
  - Supports `ReadOnly`, `WriteOnly`, and `ReadWrite` modes via `SharedCacheType`
  - Automatic pinning of hardware buffers using `MemoryHandle`
  - `Begin()` copies cache data into the hardware buffer when writable
  - `End(offset, code)` triggers callback into the parent `Cache` and releases the pin
  - Exposes `CanRead`, `CanWrite`, and `IsReadWrite` capability flags
  - Internal `MakeHardwareBuffer(int size)` for pinned byte‑array allocation
  - Extended `System.Missings.Binary` utilities:
  - Endian‑aware primitive conversions (`ToBytes`/`ToInt`/`ToUInt`/`ToShort`)
  - Generic unmanaged serialization: `ToBytes<T>(T value, Endian)`
  - Generic unmanaged array serialization: `ToBytes<T>(T[] array)`
  - Generic unmanaged deserialization: `FromBytes<T>(byte[])`
  - Unmanaged array deserialization: `FromBytesArray<T>(byte[])`
  - `ToBoundary(uint, uint)` for alignment calculations
  - `SizeCalc(string)` for parsing size strings (`4K`, `16M`, `2G`, etc.)

- Added `RandUtils` under `System.Missings.Utils`:
  - Random byte array generation (`GetArray(size)` and ranged version)
  - Endian‑aware random conversions:
    - `short Rand(this short, Endian)`
    - `int Rand(this int, Endian)`
    - `long Rand(this long, Endian)`
    - `ulong Rand(this ulong, Endian)`
  - Full range‑mapped random generation for 64‑bit values:
    - `RandLong(long min, long max, Endian)`
    - `RandULong(ulong min, ulong max, Endian)`
  - Combined high/low 32‑bit random generation for 64‑bit values
  - Ensures correct endian interpretation using `System.Missings.Binary` utilities
  - Added struct layout validation under `System.Missings.Binary.Layout`:
    - `Layout.Check<T>(uint expectedUnmanagedSize)`
    - Throws `MissingStructLayoutSequentialException` if `[StructLayout(LayoutKind.Sequential)]` is missing

### Improved
- Unified all random integer generation to use endian‑aware byte conversion
- Ensured consistent range mapping for signed and unsigned 64‑bit values
- Added argument validation for all min/max random operations

### Fixed
- Corrected handling of reversed min/max arguments in `RandLong`
- Ensured random byte arrays always fill the requested size

---

## [0.6.1]

### Added
- Introduced a full low-level cache subsystem under `System.Collections.Generic.Missings`
- Added `Cache` class providing:
  - Raw byte buffer management using custom `Array<byte>`
  - Seek operations (`SeekOrigin.Begin`, `Current`, `End`)
  - Direct indexed byte access (`cache[position]`)
  - Endian-aware read/write operations:
    - `WriteUInt32`, `WriteInt32`, `WriteInt16`
    - `ReadUInt32`, `ReadInt32`, `ReadInt16`
  - Generic byte block read/write (`Read(position, count)` and `Write(position, buffer)`)
  - Conversion to managed array via `ToArray()`
  - Internal locking mechanism preventing modification while shared
  - `CacheType` support (`ToDevice`, `FromDevice`, `Both`)

- Added shared-memory support via `SharedCache`:
  - `Cache.ToShared()` now returns a `SharedCache` instance depending on `CacheType`
  - Shared caches pin memory for hardware/driver access
  - Automatic callback into `Cache.SharedCallBack` when hardware releases the buffer
  - Proper lock/unlock semantics with `IsLocked` state tracking
  - `CacheIsSharedException` thrown when attempting to access locked cache data

- Added `CacheIsSharedException` for invalid access during shared state

### Improved
- Integrated endian conversion utilities from `System.Missings.Binary.Utils`
- Unified all integer read/write operations to use the new endian helpers
- Ensured safe bounds checking for all read/write operations
- Improved internal locking using custom `Lock` object

### Fixed
- Corrected boundary checks for read/write operations
- Ensured shared cache always unlocks on callback, even on error paths
- Fixed incorrect handling of negative seek offsets

---

## [0.5.8]

### Added
- NodeIterator: intrusive Iterator für Next/Prev Traversal
- NodeRange: Iterator-basierter Bereich (begin/end)
- NodeSlice: Teilbereich eines Node-Bereichs
- NodeChain: Verkettete Iterator-Views
- GroupNode: Node mit eigener interner LinkedList
- StarNode: Node mit Child-Fanout
- TraversOrder: Preorder, Inorder, Postorder, ListOrder, ReverseListOrder
- GlobalSuppressions für Naming/Design-Regeln

### Changed
- Iterator.Equals nutzt Value-Identity für ListIterator
- Iterator.Equals nutzt Reference-Identity für NodeIterator
- Verbesserte Advance/Offset/At-Methoden

### Fixed
- Clone() für Iteratoren nutzt korrekte Semantik
- Node.HasNext/HasPrev für zirkuläre intrusive Listen korrigiert


---

## [0.5.0] – 2026-06-03

### Added
- Erste öffentliche Version
- Grundlegende Collections (Map, Queue, Stack, BinQueue, FixedMap)
- IArray, ITuple, Quad, Pair
- Basis-Iteratoren
