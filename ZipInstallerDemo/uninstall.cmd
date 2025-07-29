@echo off
rem Uninstall script for ZipInstallerDemo

echo Uninstalling ZipInstallerDemo...

rem Remove this registry entry from Add/Remove Programs
reg delete "HKCU\Software\Microsoft\Windows\CurrentVersion\Uninstall\ZipInstallerDemo" /f >nul 2>&1

rem Remove installed files
set INSTALL_DIR=%USERPROFILE%\ZipInstallerDemo
if exist "%INSTALL_DIR%" (
    rd /s /q "%INSTALL_DIR%"
)

echo Done.
pause
