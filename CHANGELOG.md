# Changelog

All notable changes to this project are documented in this file.
The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.0] - 2026-11-11

The application now works only through the official rule34.xxx API with a personal key. HTML parsing was removed, because Cloudflare CAPTCHA makes it unusable, and the application became a single portable executable.

### Added

* Authentication with the personal `user_id` + `api_key` of a rule34.xxx account.
* Settings dialog with the credentials field, the **Test key** button, the status line and a link to the page where the key is generated.
* Command line helpers: `--set-credentials` and `--test-key`.
* Portable settings: the download folder and the credentials are stored in `R34Downloader.settings.xml` next to the executable, with `%LOCALAPPDATA%\Rule34.xxx Downloader` as a fallback for read-only folders. The settings of 1.0.x are migrated from `user.config`.
* Browser-like request headers (User-Agent, Accept, Accept-Language, sec-ch-ua, Referer) and the GDPR cookies, so Cloudflare does not reject the requests.
* Readable error messages for a missing or rejected key, a Cloudflare block, a timeout and a lost connection.
* Shared retry policy for API requests and file downloads: up to 5 attempts, exponential backoff (1-30 s) with jitter, `Retry-After` support (1 s - 2 min) and retries only for transient errors (408, 429, 5xx and network failures).
* Per-file progress in the status bar and a report about the files that could not be downloaded.
* Instant resume: an existing file is found by the post id in its name, so a repeated run skips already downloaded posts without a single file system or network request.
* Global error handling: unexpected exceptions are shown in a friendly dialog, and service errors offer to open the settings.
* The **Stop** button finishes the current file and then stops.

### Changed

* NuGet is not used anymore: `packages.config` is gone and there are no third-party dependencies.
* The project targets .NET Framework 4.8.1 with C# 7.3, the builds are deterministic, and the executable prefers 64-bit.
* The release build contains no debug symbols and no local build paths, so a single `.exe` is the whole distribution.
* 125 ms pause between file downloads and between API pages; HTTP 429 responses honour `Retry-After`.
* No pause between posts that are already on disk: the polite delay is applied only after real network requests.
* Files are streamed to disk, so large videos no longer have to fit into memory.
* The download dialog uses a validated text box.
* Search, its result and the download run in background tasks, so the UI does not freeze.
* Startup connectivity check that tells "no internet" from "rule34.xxx is unreachable".
* `longPathAware` in `app.manifest`. The application is intentionally DPI-unaware: Windows scales the window itself, so the interface looks the same at any display scaling (the picture is softer at 125% and above).
* Verdana as the UI font.

### Fixed

* 16-bit overflow when the search returns more than 65 535 posts.
* Pagination duplicated posts when the last page had a different size.
* Videos were saved from `sample_url` instead of `file_url`.
* Silent download failures: files are written to a `.part` file, checked against `Content-Length` and removed if the transfer breaks; failed files are counted and reported.
* Empty or already complete files are not downloaded again.

### Removed

* The HTML parsing download method and the `HtmlAgilityPack` dependency. The settings dialog no longer offers a choice of the download method.

## [1.0.5] - 2025-11-03

* Adaptation of the application to the site changes.
* HTML parsing works again and is selected by default, so the release is based on it.
* The API requires an authentication key after the site change and was not restored in this release.
* Downloads and page requests now send the browser User-Agent, the `Referer` and the GDPR cookies; the HTML parser takes the image from `og:image`, returns `ushort.MaxValue` instead of `-1` when the number of pages is unknown, and HtmlAgilityPack was updated to 1.12.4.
* The header image was updated.

## [1.0.4] - 2024-12-25

* Minor improvements for file downloading.
* `WebClient` was replaced with `HttpClient` for the file download and for the connectivity check.
* The API parser reads the attributes by name (`file_url`, `id`) instead of by index, so the attribute order no longer matters, and the file type is detected by the extension.
* Videos started to be downloaded from `sample_url` instead of `file_url`.
* 100 ms pause between the files of a page in the HTML parsing mode.
* HtmlAgilityPack 1.11.46 -> 1.11.71, `highDpiMode` added to `App.config`.

## [1.0.3] - 2023-03-05

* Bug fixes.
* The code was restructured.
* The choice between the API and HTML parsing moved from the main form to the settings dialog; only the download folder is saved between runs.
* .NET Framework 4.7.1 -> 4.8.1, HtmlAgilityPack 1.7.1 -> 1.11.46.

## [1.0.2] - 2018-03-04

* Fixed several bugs.
* The project moved to this repository: the same application as 1.0.1, but all sources now live in one project.
* .NET Framework 4.7.1, HtmlAgilityPack 1.7.1.

## [1.0.1] - 2017-04-28

* Bug fixes and improved stability.
* File names now contain the post id.
* Published in the previous repository `r34dl/r34downloader`.

## [1.0.0] - 2017-04-28

* First public release of Rule34.xxx Downloader.
* A Windows Forms application that downloads content from rule34.xxx through the official API and through HTML parsing.

[1.1.0]: https://github.com/DaxEleven/Rule34.xxx-Downloader/compare/1.0.5...1.1.0
[1.0.5]: https://github.com/DaxEleven/Rule34.xxx-Downloader/compare/1.0.4...1.0.5
[1.0.4]: https://github.com/DaxEleven/Rule34.xxx-Downloader/compare/1.0.3...1.0.4
[1.0.3]: https://github.com/DaxEleven/Rule34.xxx-Downloader/compare/1.0.2...1.0.3
[1.0.2]: https://github.com/DaxEleven/Rule34.xxx-Downloader/releases/tag/1.0.2
[1.0.1]: https://github.com/r34dl/r34downloader/releases/tag/1.0.1
[1.0.0]: https://github.com/r34dl/r34downloader/releases
