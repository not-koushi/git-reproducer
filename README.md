# Git Reproducer

A local developer tool that automatically clones a repository, builds it, runs tests, and shows whether a failure can be reproduced on a clean machine.

The goal is to help maintainers quickly answer: **"Does this issue actually happen from a fresh environment?"**

---

## What It Does

Given only a GitHub repository URL, the tool will:

1. Clone the repository
2. Detect .NET projects
3. Restore dependencies
4. Build the project
5. Run unit tests
6. Capture logs and failures
7. Show the result through an interactive CLI

---

## Features

* Background worker execution
* Persistent job tracking (SQLite)
* Interactive CLI interface
* Automatic environment cleanup
* Failure detection with stack traces
* Clean shutdown lifecycle

---

## How To Run

From the project root:

```
run-dev
```

This starts:

* API server
* Worker processor
* Interactive client

---

## CLI Commands

| Command | Description                      |
| ------- | -------------------------------- |
| `/url`  | Submit a repository to reproduce |
| `/id`   | Check job result                 |
| `/exit` | Stop all services                |

---

## Example Workflow

```
/url
Azure-Samples/dotnetcore-docs-hello-world

/id
<job-id>
```

Output will show build and test results.

---

## Project Structure

```
src/
  Api/          -> HTTP endpoints
  Workers/      -> Background execution engine
  Domain/       -> Job models & status
  Infrastructure/ -> Database & process runner
workspaces/     -> Cloned repositories
client.ps1      -> Interactive CLI
run-dev.bat     -> Local launcher
```

---

## Current Capabilities

* Reproduce build failures
* Reproduce test failures
* Capture stack traces

---

## Future Goals

* Multi-language support (Node, Python, Rust, Go)
* Failure summarization
* Docker sandbox execution
* GitHub issue integration

---

## Purpose

This project is designed as a lightweight reproducibility assistant — a tool that verifies problems before humans start debugging them.