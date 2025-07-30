@echo off

rem Demo installer script

echo Hello World
pause

rem Install uninstall script to user directory
set INSTALL_DIR=%USERPROFILE%\ZipInstallerDemo
if not exist "%INSTALL_DIR%" mkdir "%INSTALL_DIR%"
copy /Y "%~dp0uninstall.cmd" "%INSTALL_DIR%" >nul

rem Create Add/Remove Programs entry (per-user)
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Uninstall\ZipInstallerDemo" /v DisplayName /d "ZipInstallerDemo" /f >nul
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Uninstall\ZipInstallerDemo" /v UninstallString /d "\"%INSTALL_DIR%\uninstall.cmd\"" /f >nul

echo Installation complete.
