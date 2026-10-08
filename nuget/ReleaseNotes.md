
## ⭐ [0.95.6023-rc4] 08.10.2026 — internal Preview for 1.0 → Lacking

### Added

- Added `Empty`:
  - Represents a reusable empty-value marker type for generic APIs and result pipelines.

- Added `Timeout<T>`:
  Provides a generic timeout abstraction with support for finite and infinite timeout values.
  Includes helper utilities for timeout validation and timeout-state checks.

- Added `Reference`:
  Provides a base class for reference-counted objects.
  Supports deterministic lifetime tracking through explicit reference management.

- Added `ThreadSafeIndex`:
  Provides an atomic thread-safe index helper for concurrent container and synchronization scenarios.

- Added cache-backed stack support:
  Introduced stack operations for reference-based cache implementations.

- Added `ModulePreloader`:
  Provides preloading support for native modules before runtime invocation.

- Added `Function<TReturn, TDelegate>`:
  Strongly typed wrapper around native function bindings and delegate-based invocation.

- Added debug metadata support to `IValueReader` implementations:
  Read operations can now expose additional diagnostic metadata for debugging and inspection.

- Added utility helpers for `Triple<T>`:
  Additional logical and comparison helper functions (`And`, `Or`, comparison helpers, and related utilities).
  
- Added `Version.cs`: Represents a mutable version identifier with major, minor, and build components 
    and additional build metadata.
### Changed

- `CompareResult` was moved to the root namespace `SystemEx`.
  Simplifies discoverability and reduces unnecessary namespace dependencies.

- `RCUHistory` now uses `ByteIndex` addressing internally.
  Improves consistency with the cache and memory subsystem.

- `Result` can now contain and aggregate other `Result` instances.
  Enables hierarchical result composition and nested processing pipelines.

- Updated operator support across several utility and value types.
  Improved consistency of arithmetic and comparison semantics.

### Improved

- Improved `Epoch`:
  Added `GetHashCode()` implementation.
  Added `operator true` and `operator false`.

- Fixed epoch evaluation and RCU validation logic.
  Resolved several issues affecting RCU state checks and epoch handling.

- Updated XML documentation and inline comments across the framework.

### Removed

- Removed the legacy node system.
  Preparation for a redesigned node architecture, in Future version I will added a new version, sry

### Build

- Added build and repository maintenance infrastructure.
  - Added build configuration files.
  - Updated push automation scripts.
  - Updated repository ignore rules (`.gitignore`) for build artifacts and generated files.

### Fixed

- Fixed various bugs across `Result`, `ResultBuilder`, `BigDecimal`, and the RCU subsystem.
- Corrected exponent conversion issues in `BigDecimal`.
- Fixed assertion indexing issues in `Result`.
- Improved stability of fluent result-building operations.

---

See CHANGELOG.md for complete release notes:
https://github.com/RoseLeDark/System.Collections.Generic.Missings/blob/main/CHANGELOG.md