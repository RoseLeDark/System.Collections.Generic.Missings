# SystemEx Example 4  
## Custom GetHashCode for Structs using SystemEx HashFactory

This example demonstrates how a `struct` can participate in the SystemEx hashing
pipeline using the `IHashable<T>` interface, the `HashAlgorithmAttribute`, and the
`HashFactory`.  
It shows how to generate deterministic 32‑bit and 64‑bit hash values for value
types without relying on the old `Hashable` base class.

## 🧩 Overview

SystemEx provides a unified hashing system for both classes and structs.  
Structs cannot inherit from a base class, so hashing is implemented through:

- `HashAlgorithmAttribute` — selects the hashing algorithm and endianness  
- `IHashable<T>` — enforces a deterministic byte representation  
- `HashFactory` — computes 32‑bit and 64‑bit hashes from the byte sequence  

This example uses the `BernsteinHash` algorithm.

## 🔧 The `SensorData` Struct

The struct contains two fields:

- `int Id`  
- `float Value`  

It implements `IHashable<SensorData>` and defines a deterministic `ToBytes()` method:

- The integer is serialized using the system endianness.  
- The float is serialized using big‑endian to demonstrate per‑field endianness.  
- The total byte size is fixed (8 bytes).  
- Field order must never change.

The struct also supports an optional `Seed` value.  
If `Seed == 0`, a random seed is generated; otherwise, the provided seed is used.


## 🔄 Hashing Process

1. The struct is annotated with:

```csharp
[HashAlgorithm(typeof(BernsteinHash), Endian.System)]
```

2. ToBytes() returns a deterministic byte sequence.
3. HashFactory.Hash32() or HashFactory.Hash64() consumes the bytes.
4. The selected algorithm computes the hash.
5. If the result is zero, a fallback value is used.
6. This ensures stable and reproducible hashing across releases.

## 📌 Program Output
Example output using a static seed:

```Code
SystemEx Example 4 Costum GetHashCode

Hash with static seed:
SensorData (ID: 748, Value: 1,00928) Bernstein Hash32: 0x40A11541
SensorData (ID: 748, Value: 1,00928) Bernstein Hash64: 0xC618A59645445FD4
Elapsed Time: 5 ms
The elapsed time is dominated by console output; hashing itself is extremely fast.
```

## 🎯 What This Example Demonstrates

- How structs participate in the SystemEx hashing pipeline
- How to define deterministic byte sequences
- How to attach hashing algorithms via attributes
- How to compute 32‑bit and 64‑bit hashes
- How seeds influence hash output
- How SystemEx replaces the old Hashable base class with IHashable<T>