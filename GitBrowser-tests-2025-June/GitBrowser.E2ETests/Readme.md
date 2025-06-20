

#  how to install playwright

playwright is required for E2E-testing of GitBrowser

https://www.skptricks.com/2025/03/how-to-setup-playwright-on-windows-and-mac.html

* install node.js and npm
* npm init playwright@latest
* cd C:\work\ScriptCollection\CSharp\GitBrowser.E2ETests
* check installed browsers in C:\Users\edward\AppData\Local\ms-playwright
  (there are browser, but not the version required by GitBrowser.E2E

* Set-ExecutionPolicy -Scope CurrentUser Unrestricted
* bin/Debug/net8.0/playwright.ps1 install
* check installed browsers in C:\Users\edward\AppData\Local\ms-playwright
  (now there are the versions required by GitBrowser.E2E




