# 📘 README.md — *SystemEx Windows AI Runtime Backend Example*

# SystemEx – Windows AI Runtime Backend Example

This example demonstrates how the **SystemEx AI Framework** integrates with the **Windows AI Runtime** (via `AIClient`).  
It shows:

- How to implement a backend (`WinCopilotBackend<T>`)
- How SystemEx tools are converted into Windows‑compatible `AITool` objects
- How environment information is collected and passed to the backend
- How a model is created, initialized, and executed
- How developers can add their own tools
- How to switch the model at runtime

The example is intentionally **simple**, **clear**, and **educational**, so developers can easily understand how SystemEx helps to work with the Windows AI Runtime.


## 🔧 Architecture Overview

### 1. **WinCopilotBackend<T>**
The backend connects SystemEx to the Windows AI Runtime:

- Converts tools (`IModelTool<T> → AITool`)
- Applies configuration (API key, model name, runtime settings)
- Interprets environment (OS, GPU, temp directory, locale)
- Executes requests via `AIClient.GenerateAsync()`
- Returns structured `ModelResult<T>` objects

### 2. **WinCopilotBackendFunctionFactory<T>**
Converts internal SystemEx tools into Windows‑compatible `AITool` objects:

- Parameter schema mapping  
- AIType conversion  
- Delegate creation  
- JSON‑compatible result handling  

### 3. **WinCopilotModel**
A concrete model that:

- Defines a system prompt  
- Registers tools  
- Allows model switching  
- Uses the backend for execution  


## 🚀 Example: Creating and Running a Model

```csharp
var factory = new WinCopilotBackendFunctionFactory<string>();
var backend = new WinCopilotBackend<string>(factory);

var model = new WinCopilotModel("phi-3-mini", backend);
model.AddTools();

model.Initialization(new Map<string, object>()
{
    ["API_KEY_FOR_MODEL"] = "1234567890"
});

var prompt = new ModelPromp<string>("Hello!", "client-1");
var result = await model.RunAsync(prompt);

Console.WriteLine(result.Result);
```


## 🧰 Adding Tools

```csharp
model.AddTool(new DateTimeTool());
model.AddTool(new DateDifferenceTool());
model.AddTool(new CalculatorTool());
```

SystemEx does **not** implement automatic tool conversion.

The Windows backend performs the conversion:

- Parameter mapping  
- AIType conversion  
- Delegate creation  
- AITool object construction  

This happens inside the registered factory `IAIFunctionFactory<T, AITool>`.  
See file: **WinCopilotBackendFunctionFactory.cs**.

Internally, the model triggers the backend, which calls:

```
WinCopilotBackendFunctionFactory<T>.Convert(...)
```

### Why doesn’t SystemEx convert tools automatically?

SystemEx is intentionally **backend‑neutral**:

- Azure backends use different tool formats  
- Web backends use JSON schemas  
- Local backends use native function objects  
- Windows AI Runtime uses `AITool`  

Therefore, each backend must define how an `IModelTool<T>` is translated into its own tool format.


## 🔄 Switching the Model

```csharp
model.TryChangeModel("chg_model phi-3-medium");
```


## 📦 Environment Integration

SystemEx automatically collects:

- Operating system (Windows/Linux/macOS)
- Architecture (x64/ARM64)
- GPU availability
- Temporary directory
- Working directory
- Locale
- Network status

The backend translates these into Windows‑AI‑compatible configuration keys.

## 🖥️ Console Demo

The `Main` program demonstrates:

- Tool registration  
- Model initialization  
- Prompt loop  
- Model switching  
- Displaying AI results  

## 📄 License

This example is licensed under **EUPL‑1.2**.  
See the file header for details.

hat you want next, Amber‑Sophia.