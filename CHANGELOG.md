# Changelog

## [1.0.2] - 2026-11-09

### Added
- `IHttpClient` and its default `UnityWebRequestHttpClient` in `OmiLAXR.Pseudonymization.Helper`.
- `PseudonymProvider.HttpClient` to replace the HTTP client that fetches pseudonyms. Setting `null` restores the default.
- `PseudonymBehaviour.Store` (`IKeyValueStore`, default `PlayerPrefsStore`) to replace where pseudonyms are remembered. Setting `null` restores the default.

### Changed
- Raised the core dependency `com.rwth.unity.omilaxr` to 2.2.5, which provides `IKeyValueStore` and `PlayerPrefsStore`.
- Updated package version metadata.

### Fixed
- `PseudonymProvider` fires `OnCreatedPseudonym` only once per created pseudonym.

## [1.0.1] - 2024-12-17

### Added
- CHANGELOG.md
- CITATION.cff

## [1.0.0] - 2024-10-10

### Added
- PseudonymProvider generating an 100%-safe-unique pseudonym by using the web service https://mtlg.elearn.rwth-aachen.de/pseudo/.
- RandomPseudonymGenerator generating a random Pseudonym with a chartset of "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".