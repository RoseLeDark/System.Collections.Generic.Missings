# SystemEx Example 2  
## Searching, Sorting, and Snapshotting with Vector<T>

This example demonstrates how the SystemEx `Vector<T>` container works together
with the `Find<T, C>` search utility and the `AsMultiSet()` sorting mechanism.
It shows how live vectors can be searched, sorted, modified, and snapshotted
while preserving deterministic behavior.

## 🧩 What Vector<T> Really Is

`Vector<T>` in SystemEx is **not** a high‑performance HPC container.  
It is a **modern, policy‑driven dynamic array** designed for flexibility,
reinterpretation, and multi‑view semantics — not raw speed.

### ✔ Core characteristics

- A simple dynamic array with:
  - indexed access  
  - optional auto‑growth  
  - insertion, replacement, removal  
  - zero‑allocation reinterpretation views  

- No hidden allocations  
- No copying when creating views  
- No boxing  
- No kernel‑level complexity  
- Deterministic behavior

### ✔ Multi‑View Architecture

`Vector<T>` can instantly transform into different logical “shapes”:

- **FlexSpan** — forward, reverse, or ring traversal  
- **Set** — sorted unique view  
- **MultiSet** — sorted view with duplicates  
- **UnorderedSet** — unique, no sorting  
- **UnorderedMultiSet** — duplicates allowed, no sorting  
- **Search** — binary or linear search view  
- **Find** — linear search for unsorted data  
- **Segment** — logical sub‑vector without copying  
- **Slices** — multi‑segment slicing  

All of these reinterpretations are **zero‑overhead wrappers** over the same
underlying buffer.

### ✔ Why this matters

This design is ideal for:

- deterministic algorithms  
- data analysis  
- simulations  
- domain‑specific containers  
- debugging tools  
- educational examples  

It is **not** intended to replace high‑performance containers like
SIMD‑optimized arrays or lock‑free structures.

## 🔄 What This Example Demonstrates

This example shows three major features:

### 1. **Find<T, C> — Live Search Operations**

`Find` operates directly on the current state of the vector:

- `First(value)`  
- `Last(value)`  
- `Of(value)`  
- `Exists(value)`  

Indices always reflect the **live ordering** of the vector.

### 2. **AsMultiSet() — Sorted Multiset Conversion**

`Vector<T>.AsMultiSet()`:

- sorts the underlying vector  
- maintains stable ordering for equal elements  
- allows insertion while preserving sorted order  

After sorting, element indices change — and `Find` reflects the new positions.

### 3. **Snapshot — Preserving Original State**

A snapshot is a **copy** of the vector before modifications:

```csharp
Vector<int> snapshot = new Vector<int>(_vector);
```

Snapshots preserve the original ordering and allow stable index lookups even
after the live vector has been sorted or altered.

## 📌 Program Output

```Code
SystemEx Example 2 Vector Find and Set

Original Vector
-------------------------------
 437  5  9823  76  29  12  76  3  999  42  29  18  7  250  5

Finder on Live Vector(before sort)
-------------------------------
Index of First 29: 4
Index of Last 29: 10
Count of 29: 2
Exists 30: False


Vector after MultiSet + Insert
-------------------------------
 3  5  5  7  12  18  29  29  30  42  76  76  250  437  999  9823

Snapshot (Original State)
-------------------------------
 437  5  9823  76  29  12  76  3  999  42  29  18  7  250  5

Finder on Live Vector (after sort)
-------------------------------
Index of First 29: 6
Index of Last 29: 7
Count of 29: 2
Exists 30: True


Finder on Snapshot (original state)
-------------------------------
Index of First 29: 4
Index of Last 29: 10
Count of 29: 2
Exists 30: False


Elapsed Time: 8 ms
```


## 🎯 Summary
This example shows how SystemEx combines:

- flexible containers (Vector<T>)
- deterministic search utilities (Find<T,C>)
- sorted multiset structures (AsMultiSet())
- snapshots for stable analysis

Vector<T> is intentionally simple, but extremely powerful due to its
multi‑view architecture.
It is designed for clarity, determinism, and reinterpretation — not raw speed.