# 📜 SystemEx Code of Conduct

## Overview

The SystemEx project is committed to providing a respectful, safe, and constructive environment for everyone 
interacting with the repository, including issue reports, discussions, pull requests, and general communication.

SystemEx is a personal hobby project developed by a single intersex author; my pronouns are dey/deren/dem/dem. 
While I aim for correctness, determinism, and clean architecture, not every issue may be visible immediately, 
and responses or fixes may occasionally be delayed. Constructive feedback, improvement ideas, and technical 
discussions are always welcome.

## 1. Expected Behavior

All participants are expected to:

- Treat others with respect and professionalism  
- Engage in constructive, technical discussion  
- Assume good intent and communicate clearly  
- Provide actionable, specific feedback  
- Respect architectural decisions and design choices  
- Keep interactions focused on the project and its goals  

## 2. Unacceptable Behavior

The following behaviors are not tolerated:

- Harassment, insults, or personal attacks  
- Discriminatory language or behavior  
- Dismissing or belittling contributors  
- Aggressive or hostile communication  
- Spamming issues, discussions, or pull requests  
- Intentionally breaking builds or sabotaging code  
- Large unsolicited refactors that ignore project structure  

## 3. Technical Collaboration Guidelines

### 3.1 Be precise  
When reporting issues or proposing changes, please include:

- A clear description  
- Reproduction steps (if applicable)  
- Expected vs. actual behavior  
- Relevant code snippets or logs  

### 3.2 Respect modular design  

SystemEx intentionally avoids deep cross‑dependencies.  
Some components exist in multiple forms (e.g., `Map<TKey, TValue>` based on `Pair<TKey, TValue>[]` 
instead of `Vector<Pair<...>>`) to keep subsystems isolated and easier to debug.  
This is a deliberate architectural choice.

### 3.3 Discuss major changes first  

Large refactors or structural changes should be discussed before implementation to avoid regressions.

## 4. Reporting Issues

If you observe unacceptable behavior or have concerns:

- Open an issue in the repository  
- Provide context and details  
- Remain respectful and factual  

Reports will be reviewed as time permits.

## 5. Enforcement

Violations of this Code of Conduct may result in:

- A request to modify behavior  
- Temporary communication restrictions  
- Removal of comments or contributions  
- Blocking from the repository in severe cases  

Enforcement decisions are made at the discretion of the project maintainer.

## 6. Acknowledgment

Thank you for contributing to a respectful, collaborative, and technically focused environment.  
Your participation helps SystemEx grow as a stable, deterministic, and modular low‑level framework for .NET.
