# ZipInstallerDemo

This demo shows how to build a self-extracting archive using **7-Zip**. The resulting `ZipInstallerDemo.exe` automatically extracts its contents and runs a small installer script.

## Files

- `hello.cmd` – prints *Hello World*, installs an uninstall entry and copies `uninstall.cmd`.
- `uninstall.cmd` – removes the demo and its registry entry.
- `config.txt` – 7‑Zip SFX configuration telling the extractor to run `hello.cmd`.

## Building

1. Install 7‑Zip so that `7z.exe` and the SFX module (for example `7z.sfx` or `7zSD.sfx`) are on your system.
2. Run the following commands in this folder:

```bat
@echo off
rem Create an archive containing the script
7z a payload.zip hello.cmd uninstall.cmd

rem Combine the SFX stub, configuration and archive
copy /b "C:\Program Files\7-Zip\7z.sfx" + config.txt + payload.zip ZipInstallerDemo.exe
```

This will produce `ZipInstallerDemo.exe`. When executed, it extracts the archive to a temporary folder and automatically runs `hello.cmd`. The script prints **Hello World**, copies `uninstall.cmd` to `%USERPROFILE%\ZipInstallerDemo` and registers it under *Add/Remove Programs* so the demo can be removed later.

### Linux

If you have the `p7zip-full` package installed, a similar sequence works:

```sh
7z a -tzip payload.zip hello.cmd uninstall.cmd
cat /usr/lib/p7zip/7z.sfx config.txt payload.zip > ZipInstallerDemo.exe
chmod +x ZipInstallerDemo.exe
```

