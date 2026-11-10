# Rule34.xxx Downloader

[![Latest release](https://img.shields.io/github/v/release/DaxEleven/Rule34.xxx-Downloader?sort=semver&label=release&color=2ea44f)](https://github.com/DaxEleven/Rule34.xxx-Downloader/releases/latest)
[![Downloads](https://img.shields.io/github/downloads/DaxEleven/Rule34.xxx-Downloader/total?label=downloads&color=0078D4)](https://github.com/DaxEleven/Rule34.xxx-Downloader/releases)
[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8.1-512BD4)](https://dotnet.microsoft.com/download/dotnet-framework/net481)
[![Platform](https://img.shields.io/badge/platform-Windows%2010%2B-0078D4)](https://www.microsoft.com/windows)
[![Portable](https://img.shields.io/badge/portable-single%20exe-e07b39)](https://github.com/DaxEleven/Rule34.xxx-Downloader/releases/latest)
[![License](https://img.shields.io/github/license/DaxEleven/Rule34.xxx-Downloader?label=license&color=2ea44f)](https://github.com/DaxEleven/Rule34.xxx-Downloader/blob/master/LICENSE)

A portable Windows application for downloading images, GIFs and videos from [rule34.xxx](https://rule34.xxx). You enter tags the same way you do on the site, choose a folder and the number of files, and the application downloads everything for you.

The application works through the official [rule34.xxx API](https://api.rule34.xxx), so it needs a personal API key of your own account. The key is stored locally and is sent only to the API.

# Screenshots

![image](https://github.com/user-attachments/assets/225ed476-c515-423c-8032-993dc8d27293)
![image](https://github.com/user-attachments/assets/f505f0c9-4d9d-4019-90d7-de0ee6d0d35a)
![image](https://github.com/user-attachments/assets/8ce7044b-891b-4008-8090-247c223bf71c)

## Download

Download [R34Downloader.exe](https://github.com/DaxEleven/Rule34.xxx-Downloader/releases/latest) from the latest release and run it.

The settings are written to `R34Downloader.settings.xml` next to the executable, so the application can be moved together with its settings, for example on a USB stick. If the program folder is not writable, the file is created in `%LOCALAPPDATA%\Rule34.xxx Downloader` instead. Settings of version 1.0.x are migrated automatically.

## Getting an API key

1. Create an account on [rule34.xxx](https://rule34.xxx/index.php?page=account&s=home) or log in if you already have one. The sign-up form is available from the login page of the site.
2. Open the [account options](https://rule34.xxx/index.php?page=account&s=options) page. The **API Access Credentials** section contains one ready-to-use line `&api_key=...&user_id=...`.
3. If the line is empty, tick **Generate New Key?** next to it to create your first key. Do it only once: several keys can get the account suspended.
4. Copy the whole line, start the application, click the gear icon, paste it into the **Credentials** field, press **Test key** and then **OK**. The status line reports whether the key was accepted.

The same can be done from the command line, which is useful for scripts:

```powershell
# The line copied from the account options page
R34Downloader.exe --set-credentials "&api_key=YOUR_API_KEY&user_id=YOUR_USER_ID"

# Check the saved key
R34Downloader.exe --test-key
```

Keep in mind:

* The key is personal. Do not publish it: anybody with your key can use the API on your behalf.
* Use exactly one key per account. Creating several keys can get the key or the whole account suspended.
* API limits may change at any time.

## Disclaimer

The application provides access to erotic and pornographic material published on rule34.xxx. Use it only if you have reached the legal age in your country; filters such as `rating:safe` can narrow the results, but you are responsible for what you download and for complying with the laws of your country. The content belongs to its respective authors, and the application is not affiliated with rule34.xxx or its administration. The software is provided "as is", without any warranty; use it at your own risk, and the author is not liable for any consequences of its use.

Do not use the application in a way that violates the rule34.xxx API terms of service: no advertising or paywalls around the API and the CDN content, and no more than one API key per account.

## Other

* [CHANGELOG.md](CHANGELOG.md) - the full history of all versions.
* [BUILD.md](BUILD.md) - how to build the application from source.
* [LICENSE](LICENSE) - the MIT license.
