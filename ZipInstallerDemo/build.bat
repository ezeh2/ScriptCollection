@echo off
REM Build self-extracting installer

REM Create a zip archive with the payload
7z a payload.zip hello.cmd uninstall.cmd

REM Combine SFX stub, config and archive into final EXE
copy /b "C:\Program Files\7-Zip\7z.sfx" + config.txt + payload.zip ZipInstallerDemo.exe

echo Created ZipInstallerDemo.exe
