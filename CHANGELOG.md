# Changelog
## Planned to Version 1.0
### Add
- Cache and collection primitives ( `CacheRaid`, `CacheRaid4`, etc.) to **`SystemEx.Collections.Generic`**.
- Add Locking System 
- OpenCL Kerneal Call with my  SystemEx.Device System

## [0.10.70] 29.06.206

### Very Important
- **Renamed `RamKernel` → `NativeRAMKernel<TDelegate>`**  
  New unified kernel base class replacing the old RAM‑only implementation.  
  See example: `Examples/ExampleRamKernelAdd.cs`.

- **Removed all legacy platform kernel loaders**  
  (`WindowsKernelLoader`, `LinuxKernelLoader`, `MacKernelLoader`, `NoSupportKernelLoader`)  
  Replaced by new module‑centric loader architecture.

### Added
- Introduced full native module system under `SystemEx.Runtime`:
  - `Module` (DLL/SO/DYLIB abstraction with handle + function lookup)
  - `NativeHost` (module cache, unload, delegate binding)

- Added platform loader backends under `SystemEx.Runtime.InteropServices.Platform`:
  - `WindowsProcLoader`
  - `LinuxProcLoader`
  - `MacProcLoader`
  - `NoSupportProcLoader` (simulated backend)

- Added new kernel base class `NativeRAMKernel<TDelegate>`:
  - Backend‑neutral kernel lifecycle (`Create`, `BeginRun`, `Run`, `EndRun`)
  - Automatic buffer lock/unlock
  - Delegate‑based native function invocation
  - Backend hooks (`OnCreate`, `OnBegin`, `OnRun`, `OnEnd`, `OnAddBuffer`)

- Added example kernel:  
  - `ExampleRamKernelAdd` demonstrating the new API.

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
- This update contains **breaking changes**. All code using `RamKernel` or `KernelLoader` must migrate to `NativeRAMKernel<TDelegate>`.

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

### Improved**
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
- **Major namespace restructuring** to reflect the engine architecture and clarify responsibilities across collections, device memory, and interop layers.
- This release contains **breaking changes**: update all `using`/imports to the new namespaces.

### Changed
  - Moved device memory types (`DeviceBuffer`, `DeviceSharedBuffer<TDeviceSharedBackend>`) to **`SystemEx.Device.Memory`**.
  - Moved native interop and backend implementations (`UnmanagedObject`, `RamSharedBackend`, `IDeviceSharedBackend`, platform kernel loaders) to **`SystemEx.Device.Interop`**.
  - Kept kernel and execution interfaces (`IKernel<TBackend>`, `RamKernel`, kernel lifecycle orchestration) under **`SystemEx.Device`**.
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
- **Breaking change:** update all `using` directives and project references to the new namespaces.  
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
