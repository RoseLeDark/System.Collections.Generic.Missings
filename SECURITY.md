# Security Policy

## Supported Versions

SystemEx is a hobby project under active development.  
Security‑related issues will be addressed as time permits.

## Reporting a Vulnerability

If you discover a security vulnerability, please report it privately:

**Email:** ambersophia.schroeck@mail.de  
**Maintainer:** Amber‑Sophia Schröck (pronouns: dey/deren/dem/dem)

Please include:

- A clear description of the issue  
- Steps to reproduce  
- Potential impact  
- Suggested mitigation (if any)

## Scope

Security concerns may include:

- Unsafe memory operations  
- Interop misuse (Linux/macOS/Windows dynamic loading)  
- Random engine misuse (ISAAC, Randx)  
- Threading primitives (spinlocks, EventGroup)  
- IO handling (CacheStream, WriteStream)

## Response Process

1. The maintainer reviews the report.  
2. The issue is reproduced and validated.  
3. A fix is prepared and tested.  
4. A patch release or commit is published.  
5. The reporter is notified.

Thank you for helping keep SystemEx safe and reliable.
