# SystemEx Example 1  
## Minimal “Hello SystemEx” and Framework Build Information

This example is intentionally minimal and serves as the entry point for new users
exploring the SystemEx framework.  
It demonstrates how to access framework metadata and print a formatted build
string to the console.

## 🧩 Overview

SystemEx provides a central `Framework` class containing metadata such as:

- API name  
- version  
- codename  
- build number  
- debug/fork flags  
- user information (if available)  

`Framework.BuildString()` returns a compact, human‑readable summary of this
metadata.  
Example 1 shows how to print this information as part of a simple greeting.

## 🔧 What the Example Does

1. Starts a `Stopwatch` to measure execution time.  
2. Prints a greeting combined with the SystemEx build string:  

   ```csharp
   Console.WriteLine("Hallo " + Framework.BuildString());
   ```
3. Prints the elapsed time.

This example contains no additional logic and is designed to verify that the
framework is correctly referenced, initialized, and functional.

```Code
Hallo SystemEx-0.92.1902-Lacking-bt
Elapsed Time: 3 ms
```

4. Actual output varies depending on:

	- operating system
	- debug/release mode
	- fork state
	- user information
	- current SystemEx version

## 🎯 Purpose of This Example
- Acts as a “Hello World” for SystemEx
- Confirms that the framework is installed and working
- Demonstrates how to retrieve build metadata
- Provides a minimal template for future examples
- Helps users verify environment setup before running more complex demos