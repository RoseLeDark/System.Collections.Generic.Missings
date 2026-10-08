# Build & Publish Guide

The build output is located in `build/net10.0/`.

## Building Locally

### Windows (PowerShell)
```powershell
./Build.ps1

```

### Linux / Unix (Make)

```sh
make

```

## Publishing to NuGet

### 1. Pre-Publish Checklist

Before publishing a new version, ensure the following files are updated:

* **`CHANGELOG.md`**: Document the changes for the new release.
* **`nuget/version.txt`**: Update with the target version string.
* **`nuget/ReleaseNotes.md`**: Add the current changelog entry.
> 📌 **Note:** Do not modify any lines below the `---` separator in `ReleaseNotes.md`.



### 2. API Key Setup

1. Create a file named `API.txt` in the `nuget/` directory (`nuget/API.txt`) and paste your NuGet API key into it.
2. **Security Warning:** Ensure that `API.txt` (along with your `.snk` files) is listed in your `.gitignore`. Your credentials and private keys must **under no circumstances** be committed or pushed online!

### 3. Run Build & Push

Execute the following commands in your terminal:

```sh
make 
make push API_KEY=$(cat ./nuget/API.txt)

```
