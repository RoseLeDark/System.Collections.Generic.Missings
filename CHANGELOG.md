# Changelog

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
