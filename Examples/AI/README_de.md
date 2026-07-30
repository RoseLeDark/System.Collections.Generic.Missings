
# 📘 README.md — *SystemEx Windows AI Backend Example*

# SystemEx – Windows AI Runtime Backend Example

Dieses Beispiel zeigt, wie das **SystemEx AI Framework** mit der **Windows AI Runtime**
(über `AIClient`) verbunden werden kann.  
Es demonstriert:

- Wie ein Backend implementiert wird (`WinCopilotBackend<T>`)
- Wie Tools aus SystemEx in Windows‑kompatible `AITool` Objekte konvertiert werden
- Wie Environment‑Informationen automatisch gesammelt und an das Backend übergeben werden
- Wie ein Model erstellt, initialisiert und ausgeführt wird
- Wie Entwickler eigene Tools hinzufügen können
- Wie man das Modell zur Laufzeit wechseln kann

Das Beispiel ist bewusst **einfach**, **verständlich** und **didaktisch** gehalten,
damit auch Einsteiger sofort sehen, wie SystemEx hilft, die Windows AI Runtime
komfortabel zu nutzen.

## 🔧 Architekturüberblick

### 1. **WinCopilotBackend<T>**
Das Backend verbindet SystemEx mit der Windows AI Runtime:

- Konvertiert Tools (`IModelTool<T> → AITool`)
- Übernimmt Konfiguration (API‑Key, Model‑Name, Runtime‑Settings)
- Interpretiert Environment (OS, GPU, Temp‑Dir, Locale)
- Führt Requests über `AIClient.GenerateAsync()` aus
- Liefert strukturierte `ModelResult<T>` zurück

### 2. **WinCopilotBackendFunctionFactory<T>**
Konvertiert interne SystemEx‑Tools in Windows‑kompatible `AITool` Objekte:

- Parameter‑Schema
- Delegate‑Handler
- Beschreibung
- JSON‑kompatible Rückgabe

### 3. **WinCopilotModel**
Ein konkretes Model, das:

- SystemPrompt definiert
- Tools registriert
- Model‑Wechsel erlaubt
- Backend nutzt


## 🚀 Beispiel: Model erstellen und ausführen

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


## 🧰 Tools hinzufügen

```csharp
model.AddTool(new DateTimeTool());
model.AddTool(new DateDifferenceTool());
model.AddTool(new CalculatorTool());
```

SystemEx implementiert keine automatische Tool‑Konvertierung.

Das Windows‑Backend implementiert:
- Parameter‑Mapping
- AIType‑Konvertierung
- Delegate‑Erstellung
- AITool‑Objektbau

Dies geschieht in der registrierten Factory IAIFunctionFactory<T, AITool>.
Siehe Datei: WinCopilotBackendFunctionFactory.cs.

Intern ruft das Model über das Backend die Methode
WinCopilotBackendFunctionFactory<T>.Convert(...) auf.

### Warum konvertiert SystemEx Tools nicht automatisch?

SystemEx ist bewusst backend‑neutral:

- Azure‑Backends verwenden andere Tool‑Formate
- Web‑Backends verwenden JSON‑Schemas
- Lokale Backends verwenden native Funktionsobjekte
- Windows AI Runtime verwendet `AITool`

Darum muss jedes Backend selbst definieren,
wie ein IModelTool<T> in ein backend‑spezifisches Tool‑Objekt übersetzt wird.

## 🔄 Modell wechseln

```csharp
model.TryChangeModel("chg_model phi-3-medium");
```

## 📦 Environment‑Integration

SystemEx sammelt automatisch:

- OS (Windows/Linux/macOS)
- Architektur (x64/ARM64)
- GPU‑Status
- Temp‑Dir
- Work‑Dir
- Locale
- Network‑Status


## 🖥️ Konsolen‑Demo

Die Main zeigt:

- Tool‑Registrierung
- Model‑Initialisierung
- Prompt‑Loop
- Model‑Wechsel
- Ausgabe des KI‑Ergebnisses


## 📄 Lizenz

Dieses Beispiel ist unter der **EUPL‑1.2** lizenziert.  
Siehe Datei‑Header für Details.
