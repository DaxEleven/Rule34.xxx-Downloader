# Building Rule34.xxx Downloader

Requirements: Windows 10 or newer, Visual Studio 2022 (17.3 or newer) with the **.NET desktop development** workload, or MSBuild with the .NET Framework 4.8.1 targeting pack.

Build the `Release` configuration from a Developer Command Prompt (or use **Build → Rebuild Solution** in Visual Studio):

```powershell
msbuild "Rule34.xxx Downloader.sln" /t:Rebuild /p:Configuration=Release
```

The result is a single portable file: `Rule34.xxx Downloader\bin\Release\R34Downloader.exe`. It runs without an installer, without a config file and without any DLLs next to it, so this file is the whole release.

There are no NuGet packages, so no restore step is needed.
