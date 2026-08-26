# SystemEx Example 5  
## LightThread + Barrier Synchronization Demo

This example demonstrates how multiple `LightThread` workers can be synchronized
using a fully user‑mode controlled `Barrier`.  
The number of worker threads automatically scales to the number of available CPU
cores, making the example portable across small and large systems.

## 🧩 Overview

Each worker thread performs simulated work, arrives at the barrier, and then waits
until the barrier is opened.  
Unlike traditional blocking barriers, this implementation does **not** park threads
in the operating system kernel. Waiting threads remain active and yield execution
in a lightweight loop, keeping synchronization entirely in user space.

The **main thread acts as the barrier controller**:

1. It checks whether all workers have arrived.  
2. It opens the barrier.  
3. It advances the phase counter.  
4. It invokes the completion callback.  
5. After a predefined number of phases, it signals all workers to terminate.

This design provides deterministic phase synchronization without kernel waits.

## 🔄 Execution Flow

Each worker thread performs the following cycle:

1. Simulated work (`Thread.Sleep(1000–3000)`).
2. Arrival at the barrier (`Arrive(n)`).
3. Active waiting (`Yield()` loop).
4. Barrier opens → thread continues.
5. More simulated work.
6. Repeat until the main thread ends the example.

The main thread repeatedly calls:

```csharp
while ( !endMarker ) {
	barrier.WaitOpen(10);   // Wait until barrier is open
	Thread.Sleep(300);      // Slow down the loop a bit
}
```


This opens the barrier once all workers have arrived and increments the phase index.
After three completed phases, the example terminates.

## 📌 Console Output
Console output appears unordered or interleaved.
This is expected behavior because Console.WriteLine() is not thread‑safe.

The barrier guarantees logical synchronization, not ordered printing.

Even if the text appears jumbled, all threads still reach and pass the barrier
together.

## 🎯 What This Example Demonstrates

- User‑mode barrier synchronization
- Deterministic phase progression
- Lightweight thread waiting via Thread.Yield()
- Multi‑thread coordination without kernel blocking
- Real parallel execution across CPU cores
- Non‑deterministic console output despite deterministic synchronization