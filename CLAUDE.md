# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

Working rules (versioning, CHANGELOG, `.meta` files, licenses, compatibility) are in @AGENTS.md. Follow them.

## What this is

`com.rwth.unity.omilaxr.pseudonymization`: Module that generates pseudonyms for learners, either from the MTLG web service or randomly, and optionally remembers them across sessions.

## Commands

There is no command-line build or linter: the package compiles inside a Unity project. Paths below are relative to the workspace root (`..`).

- **Develop:** open `OmiLAXR_Dev/` in Unity 6000.3.13f1. Its `Packages/manifest.json` links `com.rwth.unity.omilaxr.pseudonymization` from disk (`file:`), so edits recompile live.
- **Tests** (`Tests/`, assembly `OmiLAXR.Pseudonymization.Tests.Runtime`): the test asmdef is a runtime assembly, so the tests are PlayMode-only. Run them in the Unity Test Runner (PlayMode tab).
- **CLI test run.** Batch mode fails while the editor has the project open. In that case, run against an APFS copy (`cp -c -R OmiLAXR_Dev <scratch>/Dev`).

  ```bash
  /Applications/Unity/Hub/Editor/6000.3.13f1/Unity.app/Contents/MacOS/Unity -batchmode \
    -projectPath OmiLAXR_Dev -runTests -testPlatform PlayMode \
    -assemblyNames OmiLAXR.Pseudonymization.Tests.Runtime -testResults results.xml
  # single test: replace -assemblyNames … with -testFilter OmiLAXR.Pseudonymization.Tests.<Class>.<Method>
  ```

## Architecture

- **`PseudonymBehaviour`** (abstract) holds `generateOnStartup` and `rememberPseudonym`, raises `OnCreatedPseudonym`, and persists the value under the key `pseudonym` through `Store` (core `IKeyValueStore`, default `PlayerPrefsStore`).
- **`PseudonymProvider`** fetches `<baseUrl><languageCode>?capitalize=…` from https://mtlg.elearn.rwth-aachen.de/pseudo/ via `HttpClient` (`IHttpClient`, default `UnityWebRequestHttpClient`). It needs an internet connection.
- **`RandomPseudonymGenerator`** produces 6 characters from `A–Z0–9`. It is a singleton (`Instance`); a second instance destroys itself.
- **`PseudonymBehaviourPrinter`** logs created pseudonyms. Prefabs for each generator are in `Resources/Prefabs`. `Runtime/Helper` holds the URI and query builders and the web-request helpers.
