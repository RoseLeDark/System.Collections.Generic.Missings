# Contributing to SystemEx

Thank you for your interest in contributing to SystemEx.  
This document explains how to report issues, propose improvements, and submit pull requests.

##  How to Contribute

### 1. Reporting Issues
When opening an issue, please include:

- A clear description of the problem  
- Reproduction steps (if applicable)  
- Expected vs. actual behavior  
- Relevant code snippets or logs  

Clear reports help maintainers understand and address issues efficiently.

### 2. Proposing Changes
Before submitting large changes or refactors, please open a discussion or issue.  
SystemEx is highly modular and low‑level; major changes can introduce subtle regressions.

Small patches, documentation improvements, and targeted fixes are always welcome.

### 3. Pull Requests
Pull requests should:

- Be focused and minimal  
- Include a description of the change  
- Avoid unnecessary formatting changes  
- Respect existing architecture and module boundaries  
- Not introduce deep cross‑dependencies between subsystems  

##  Architectural Notes

SystemEx intentionally avoids tight coupling between modules.  
Some structures exist in multiple forms (e.g., `Map<TKey, TValue>` based on `Pair<TKey, TValue>[]`) to keep subsystems isolated and easier to debug.

This is a deliberate design choice.

## Experimental Features and Development Flags

SystemEx contains optional experimental code guarded by the build flag
`USE_DEVBUILD_UNSTABLE`. These sections include work‑in‑progress modules,
unstable APIs, debugging utilities, or partially implemented subsystems. They
are intentionally excluded from normal builds to keep the repository stable and
free from unfinished code paths.

If you want to contribute to experimental areas, you must explicitly enable the
flag in your build configuration:

- For MSBuild: add `<DefineConstants>USE_DEVBUILD_UNSTABLE</DefineConstants>`
- For command‑line builds: pass `/p:DefineConstants=USE_DEVBUILD_UNSTABLE`
- For IDE builds: add the symbol to your project’s build settings

When contributing to experimental code:

- Expect incomplete or placeholder implementations  
- Expect rapid changes and refactors  
- Do not rely on experimental APIs for production code  
- Keep experimental contributions isolated and well‑scoped  
- Avoid introducing dependencies from stable modules into unstable ones  

Experimental features may be removed, rewritten, or replaced without notice.
They exist solely to support active development and prototyping.


## 🕒 Response Times

SystemEx is maintained by a single author (pronouns: dey/deren/dem/dem).  
Responses or reviews may occasionally be delayed, but all constructive contributions are appreciated.

Thank you for helping improve SystemEx.