# CodeQL Installation Difficulties

1. **Title of Issue:** `codeql` package not available via APT repositories
   * **Reason it does not work:** Attempting to install the CodeQL CLI with `sudo apt-get install codeql` fails because Ubuntu's default repositories do not provide a `codeql` package. The package manager therefore cannot locate it and aborts the installation.
   * **How to make it work:** Download the official CodeQL CLI release archive from GitHub (https://github.com/github/codeql-cli-binaries/releases), extract it to a desired location (e.g., `/opt/codeql`), and add the extracted directory to the `PATH` environment variable so the `codeql` command is available in the shell.
