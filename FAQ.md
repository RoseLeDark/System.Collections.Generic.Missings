# SystemEx FAQ

## Why does SystemEx contain duplicated structures?
To keep subsystems isolated and easier to debug.  
For example, `Map<TKey, TValue>` uses `Pair<TKey, TValue>[]` instead of `Vector<Pair<...>>` to avoid deep cross‑dependencies.

---

## Why are there so many numeric types?
SystemEx is designed for deterministic engine development.  
Types like `Half16`, `Half16b`, `Ratio`, `Fast_Int`, and vector/matrix families provide explicit numeric semantics.

---

### Why does SystemEx use `#if USE_DEVBUILD_UNSTABLE` blocks?

SystemEx contains optional development blocks guarded by `#if USE_DEVBUILD_UNSTABLE`.
These sections include experimental features, work‑in‑progress implementations,
debugging utilities, or partially completed subsystems. They are intentionally
excluded from normal builds to keep the repository clean, stable, and free from
unfinished code paths.

Developers who want to test or contribute to experimental areas may enable these
blocks by defining `USE_DEVBUILD_UNSTABLE` in their build configuration. This
approach prevents unstable or incomplete features from affecting the main code
base, avoids accidental API exposure, and ensures that ongoing work does not
interfere with stable modules or public releases.

---

### Why is BitView dangerous when used incorrectly?

BitView exposes a live bit-level view over a referenced integer. Because the
span mutates the underlying value directly, using it on control variables or in
Ring mode can lead to severe program instability.

**Dangerous patterns include:**

- Creating a BitView over a loop counter (`ref i`)
- Creating a BitView over variables that control program flow
- Using Ring mode inside `foreach` without a manual break condition

These patterns can corrupt indices, cause infinite loops, or produce undefined
execution paths. BitView should only be used on dedicated data variables that
are intentionally manipulated at the bit level.

---

## Why does SystemEx include LightThread?

SystemEx provides `LightThread` as a deterministic, minimal thread model built
specifically for engine loops, schedulers, and low‑level systems. Unlike the
standard .NET `Thread` or `Task` abstractions, `LightThread` exposes a
predictable wait–signal lifecycle using only two primitives:

- a user‑space lock (`LightLock`) controlling the logical wait state  
- an OS‑level `AutoResetEvent` controlling the physical block state  

A `LightThread` always starts in a locked wait state:

> "m_waitState = new LightLock();  
>  m_waitState.Lock();"  

and only continues when explicitly awakened through `Signal()`. Waiting is
performed via `LightConditionVariable`, which temporarily releases a lock,
enqueues the thread in a wait queue, and blocks until a wake event occurs.

This design avoids scheduler‑driven behavior, hidden state machines, and
thread‑pool semantics. It provides deterministic pause/resume behavior, explicit
cooperative suspension, predictable wake‑up ordering, and minimal overhead.

---

## Why does SystemEx include ThreadEx?

`ThreadEx` is a managed thread wrapper that provides deterministic lifecycle
control using an `EventGroup<Fast_Int>` for signaling and `LightLock` for
synchronization. Instead of relying on .NET’s monitor or task scheduler,
`ThreadEx` models thread state transitions explicitly:

> "All state transitions are represented as bit flags inside an EventGroup<Fast_Int>."

Reserved bits include:

- **0** — Joinable  
- **1** — Started  
- **3** — Pause requested  
- **4** — Continue requested  

This allows the thread to cooperatively pause, resume, abort, or kill itself
based on explicit bit‑flags rather than runtime heuristics. The main loop checks
these bits directly:

> "if (IsSuspend()) { ... } else { OnTask.Invoke(...); }"

The result is a predictable, engine‑oriented thread model suitable for
simulation loops, schedulers, and low‑level systems.

---

## Why does SystemEx include EventGroup?

`EventGroup<TFastType>` is a lightweight bit‑flag signaling system inspired by
FreeRTOS event groups. It uses a fast bit‑storage type (`Fast_Int`) and a
`LightLock` for synchronization. Each bit represents an event, and threads can
wait on specific bits:

> "Waits until the bit at the specified position becomes set… using Thread.Yield."

EventGroup provides:

- deterministic event signaling  
- no kernel‑level wait handles  
- no monitor/condition variable overhead  
- predictable busy‑wait semantics  
- extremely low latency  

It is ideal for thread coordination, state machines, and engine subsystems.


---

## Why does SystemEx include LightConditionVariable?

`LightConditionVariable` provides a minimal wait queue for threads. Instead of
using .NET’s `Monitor.Wait` or `AutoResetEvent` directly, it implements a
cooperative wait mechanism:

- the waiting thread releases a lock  
- it is added to a wait queue  
- it blocks on its own `AutoResetEvent`  
- it reacquires the lock after waking  

This matches the behavior of condition variables in POSIX and FreeRTOS, but
implemented in pure C# with deterministic semantics.

It is designed for:

- engine loops  
- schedulers  
- lock‑free or low‑lock systems  
- predictable wake‑up ordering  

---

## Why does SystemEx include LightLock?

`LightLock` is a minimal user‑space lock used throughout the threading subsystem.
It avoids OS‑level mutexes and provides:

- extremely low overhead  
- predictable behavior  
- no kernel transitions  
- no recursion  
- no hidden scheduling effects  

It is ideal for small critical sections and deterministic engine code.

---

## Why does SystemEx include LightCountingSpinlock?

`LightCountingSpinlock` is a pure user‑space atomic spinlock designed for
high‑frequency locking scenarios where kernel mutexes are too slow. It is
intended for:

- short critical sections  
- high‑performance numeric code  
- engine subsystems  
- lock‑free‑adjacent algorithms  

It avoids blocking entirely and relies on atomic operations.

---

## Why does SystemEx include EventGroup‑based LightTask?

`LightTask` is a lightweight task abstraction built on `EventGroup` and
`LightThread`. It provides:

- cooperative pause/resume  
- event‑driven state transitions  
- deterministic task lifecycle  
- predictable scheduling behavior  

It is ideal for:

- engine task graphs  
- simulation steps  
- worker pipelines  
- custom schedulers  

---

## Why does SystemEx avoid .NET Task and ThreadPool?

SystemEx intentionally avoids:

- thread‑pool scheduling  
- async/await state machines  
- continuation chains  
- hidden runtime heuristics  
- unpredictable wake‑up timing  
- automatic load balancing  

These features are powerful but unsuitable for deterministic engine code.

SystemEx threading is designed for:

- explicit control  
- predictable timing  
- deterministic state transitions  
- minimal overhead  
- debugging clarity  

This makes it ideal for simulation engines, rendering pipelines, numeric
processing, and custom schedulers.

---

### Where can I find a list of predefined colors?

All predefined colors in SystemEx are located in the namespace `SystemEx.Drawing`.
Instead of storing all colors in a single class, SystemEx organizes them into
logical color groups. Each group contains only colors of a specific hue family,
such as red, blue, green, brown, purple, etc.

This structure makes it easy to find colors by category. For example, if you are
looking for a red tone, you only need to browse the `RedColors` class. If you
need a blue tone, you check `BlueColors`, and so on.

Available color groups include:

- `BrownColors`
- `BlueColors`
- `PinkColors`
- `PurpelColors`
- `RedColors`
- `OrangeColors`
- `YellowColors`
- `GreenColors`
- `CyanColors`
- `WhiteColors`
- `GreyColors`

Each group provides a curated selection of common web colors as `ColorR8G8B8`
instances. This avoids searching through hundreds of unrelated colors and keeps
the API clean, predictable, and easy to navigate.

---

### Why are the colors split into multiple classes instead of one big list?

SystemEx groups colors by hue family to make them easier to find and use.
Instead of searching through a single class with dozens of unrelated colors,
you can directly open the group that matches the color you need.

For example:

- Need a red tone? → `RedColors`
- Need a blue tone? → `BlueColors`
- Need a warm brown tone? → `BrownColors`
- Need something pastel? → `PinkColors` or `PurpelColors`
- Need grayscale? → `GreyColors`

This organization is more intuitive, reduces search time, and keeps the API
clean and logically structured.

---

## Why does SystemEx include an interop subsystem?
To support dynamic loading of native libraries across Linux, macOS, and Windows.

---

## Why is the random subsystem custom?
ISAAC provide deterministic, high‑quality random generation suitable for simulation and procedural generation.

---

## Who maintains SystemEx?
Amber‑Sophia Schröck  
Pronouns: dey/deren/dem/dem  
SystemEx is a solo‑maintained hobby project.

---

## Why might responses be delayed?
The project is maintained by a single author.  
Feedback and contributions are welcome, but response times may vary.

---

## How can I contribute?
See `CONTRIBUTING.md` for details.
